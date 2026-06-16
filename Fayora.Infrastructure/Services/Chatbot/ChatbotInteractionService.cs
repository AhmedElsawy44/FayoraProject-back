using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Fayora.Application.Common.Interfaces.Services.ChatbotModule;
using Fayora.Application.Features.ChatbotModule.Commands.SendChatbotMessage;
using Fayora.Contracts.ChatbotModule;
using Fayora.Domain.Entities.AccommodationModule;
using Fayora.Domain.Entities.Booking;
using Fayora.Domain.Entities.ChatbotModule;
using Fayora.Domain.Entities.GuideModule;
using Fayora.Domain.Entities.SharedModule;
using Fayora.Domain.Entities.TouristModule;
using Fayora.Domain.Enums.BookingModule;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Infrastructure.Persistence.Repositories;
using Fayora.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Fayora.Infrastructure.Services.Chatbot;

public class ChatbotInteractionService(
    ApplicationDbContext context,
    OpenRouterChatbotService openRouterService,
    IOptions<GeminiSettings> geminiSettings) : IChatbotInteractionService
{
    private readonly GeminiSettings _geminiSettings = geminiSettings.Value;

    public async Task<ChatbotMessageResult> ProcessMessageAsync(
        string deviceId,
        string content,
        Guid? sessionId,
        Guid? userId,
        CancellationToken cancellationToken)
    {
        ChatbotSession? session = null;

        if (sessionId.HasValue && sessionId.Value != Guid.Empty)
        {
            session = await context.ChatbotSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId.Value, cancellationToken);
        }

        if (session == null)
        {
            session = await context.ChatbotSessions
                .FirstOrDefaultAsync(s => s.DeviceId == deviceId, cancellationToken);
        }

        if (session == null)
        {
            session = ChatbotSession.Create(deviceId, userId);
            context.ChatbotSessions.Add(session);
            await context.SaveChangesAsync(cancellationToken);
        }

        // --- Rate Limiting ---
        var today = DateTimeOffset.UtcNow.Date;
        var messageCountToday = await context.ChatbotMessages
            .CountAsync(m => m.SessionId == session.Id && m.Role == "user" && m.CreatedAt >= today, cancellationToken);

        int dailyLimit = _geminiSettings.DailyMessageLimit;

        if (messageCountToday >= dailyLimit)
        {
            var limitResponse = new ChatbotFinalResponse(
                session.Id,
                $"لقد وصلت للحد اليومي المسموح به ({dailyLimit} رسالة) 🌟 رقّي حسابك أو جرب العودة غداً!",
                new List<ChatbotCard>(),
                new List<string> { "الرجوع للبداية ↩️" },
                session.Id.ToString()
            );
            return new ChatbotMessageResult(session.Id, JsonSerializer.Serialize(limitResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }

        // --- Retrieve last 10 messages from DB ---
        var dbHistory = await context.ChatbotMessages
            .Where(m => m.SessionId == session.Id)
            .OrderByDescending(m => m.CreatedAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        dbHistory.Reverse();
        var history = dbHistory.Select(m => (m.Role, m.Content)).ToList();

        // --- User Context Enrichment ---
        string userName = "صديقي";
        string userPreferencesContext = "";
        if (userId.HasValue)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);
            if (user != null)
            {
                userName = user.FirstName;
            }

            var touristProfile = await context.Tourists
                .FirstOrDefaultAsync(t => t.UserId == userId.Value, cancellationToken);

            if (touristProfile != null)
            {
                var prefs = new List<string>();
                if (touristProfile.BudgetTier.HasValue)
                    prefs.Add($"الميزانية المفضلة: {touristProfile.BudgetTier.Value}");
                if (touristProfile.TravelStyle.HasValue)
                    prefs.Add($"أسلوب السفر المفضل: {touristProfile.TravelStyle.Value}");

                if (touristProfile.Interests != null && touristProfile.Interests.Any())
                {
                    var interestIds = touristProfile.Interests.ToList();
                    var interestNames = await context.MasterInterests
                        .Where(mi => interestIds.Contains(mi.Id) && mi.IsActive)
                        .Select(mi => mi.Name)
                        .ToListAsync(cancellationToken);

                    if (interestNames.Any())
                    {
                        prefs.Add($"الاهتمامات السياحية: {string.Join("، ", interestNames)}");
                    }
                }

                if (prefs.Any())
                {
                    userPreferencesContext = "\nتفضيلات المستخدم الحالية (استخدمها لتوجيه ترشيحاتك بشكل خفي ولطيف):\n- " + string.Join("\n- ", prefs);
                }
            }
        }

        // --- Step 1: THINK ---
        var decision = await openRouterService.ThinkAsync(content, history, cancellationToken);
        var action = decision.Action ?? "reply";
        var entities = decision.Entities ?? new List<string>();
        var queries = decision.SearchQueries ?? new Dictionary<string, string>();
        var filters = decision.Filters ?? new SearchFilters(2, null, null);

        var cards = new List<ChatbotCard>();
        var suggestions = new List<string>();
        string botResponseText = "";

        if (action == "off_topic")
        {
            botResponseText = "أنا متخصص في مساعدتك لاستكشاف وحجز الأماكن والرحلات السياحية في الفيوم فقط 😊\nبتحب تسأل عن إيه؟";
            suggestions = new List<string> { "فنادق في الفيوم 🏨", "أماكن سياحية 🏛️", "مرشد سياحي 🗺️", "خطة رحلة 📅" };
        }
        else if (action == "ask")
        {
            botResponseText = decision.Question ?? "ممكن توضح تفاصيل أكتر؟";
            suggestions = GetAskSuggestions(botResponseText);
        }
        else
        {
            // --- Step 2: SEARCH ---
            var dbContextBuilder = new System.Text.StringBuilder();
            dbContextBuilder.AppendLine("\n\n════ بيانات حقيقية من قاعدة البيانات ════");
            bool hasRealData = false;

            if (action == "search")
            {
                foreach (var entity in entities)
                {
                    var queryStr = queries.ContainsKey(entity) ? queries[entity] : content;

                    if (entity == "housing")
                    {
                        var accommodations = await SearchAccommodationsAsync(filters, queryStr, cancellationToken);
                        if (accommodations.Any())
                        {
                            hasRealData = true;
                            dbContextBuilder.AppendLine("\n🏨 أماكن الإقامة المتوفرة بالفندق والشاليهات:");
                            int i = 1;
                            foreach (var acc in accommodations)
                            {
                                dbContextBuilder.AppendLine($"{i++}. {acc.Title} | السعر: {acc.PricePerNight:F0} جنيه/ليلة | تقييم: ⭐{acc.Rating:F1} | غرف: {acc.NumberOfRooms} | أقصى عدد أفراد: {acc.MaxGuests} | العنوان: {acc.AddressDetails}");
                                if (!string.IsNullOrEmpty(acc.Description))
                                    dbContextBuilder.AppendLine($"   الوصف: {acc.Description}");
                                cards.Add(CardBuilder.FromHousingUnit(acc));
                            }
                        }
                    }
                    else if (entity == "guide")
                    {
                        var packages = await SearchGuidePackagesAsync(filters, queryStr, cancellationToken);
                        if (packages.Any())
                        {
                            hasRealData = true;
                            dbContextBuilder.AppendLine("\n🗺️ المرشدون والرحلات السياحية المتاحة:");
                            int i = 1;
                            foreach (var pkg in packages)
                            {
                                dbContextBuilder.AppendLine($"{i++}. {pkg.Title} | السعر للبالغ: {pkg.AdultPrice:F0} جنيه | المدة: {pkg.DurationHours} ساعة | السعة: {pkg.MaxCapacity} فرد | النوع: {pkg.TourTypes}");
                                if (!string.IsNullOrEmpty(pkg.Description))
                                    dbContextBuilder.AppendLine($"   التفاصيل: {pkg.Description}");
                                cards.Add(CardBuilder.FromGuidePackage(pkg));
                            }
                        }
                    }
                    else if (entity == "place")
                    {
                        var locations = await SearchPlacesAsync(filters, queryStr, cancellationToken);
                        if (locations.Any())
                        {
                            hasRealData = true;
                            dbContextBuilder.AppendLine("\n🏛️ الأماكن والمعالم السياحية بالفيوم:");
                            int i = 1;
                            foreach (var loc in locations)
                            {
                                dbContextBuilder.AppendLine($"{i++}. {loc.Name} | تقييم: ⭐{loc.Rating:F1} | الفئة: {loc.Category}");
                                if (!string.IsNullOrEmpty(loc.Description))
                                    dbContextBuilder.AppendLine($"   نبذة: {loc.Description}");
                                cards.Add(CardBuilder.FromLocation(loc));
                            }
                        }
                    }
                }
            }

            // Append User Bookings if logged in
            if (userId.HasValue)
            {
                var bookings = await context.Bookings
                    .Where(b => b.UserId == userId.Value)
                    .OrderByDescending(b => b.StartDate)
                    .Take(3)
                    .ToListAsync(cancellationToken);

                if (bookings.Any())
                {
                    hasRealData = true;
                    dbContextBuilder.AppendLine("\n📅 حجوزات المستخدم الحالية في فيورا:");
                    int i = 1;
                    foreach (var b in bookings)
                    {
                        string serviceTitle = "حجز خدمة";
                        if (b.ServiceType == ServiceType.Accommodation)
                        {
                            var acc = await context.HousingUnits.FirstOrDefaultAsync(h => h.Id == b.ServiceId, cancellationToken);
                            if (acc != null) serviceTitle = acc.Title;
                        }
                        else if (b.ServiceType == ServiceType.GuidePackage)
                        {
                            var pkg = await context.GuideTourPackages.FirstOrDefaultAsync(p => p.Id == b.ServiceId, cancellationToken);
                            if (pkg != null) serviceTitle = pkg.Title;
                        }
                        dbContextBuilder.AppendLine($"{i++}. {serviceTitle} | التكلفة الإجمالية: {b.TotalPrice:F0} جنيه | الحالة: {b.BookingStatus} | يبدأ في: {b.StartDate:yyyy-MM-dd} | ينتهي في: {b.EndDate:yyyy-MM-dd}");
                    }
                }
            }

            dbContextBuilder.AppendLine("\n════════════════════════════════════════");
            string dbContextText = hasRealData ? dbContextBuilder.ToString() : "";

            // --- Step 3: RESPOND ---
            var now = DateTime.UtcNow;
            var monthNames = new[] { "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو", "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر" };
            string currentMonth = monthNames[now.Month - 1];
            string currentSeason = (now.Month == 12 || now.Month == 1 || now.Month == 2 || now.Month == 3) ? "الشتاء — أحسن وقت للزيارة، الجو رائع والشلال في أبهى صوره" :
                                   (now.Month == 4 || now.Month == 5) ? "الربيع — جميل ومعتدل، الطبيعة خضراء" :
                                   (now.Month == 6 || now.Month == 7 || now.Month == 8) ? "الصيف — حار، الأفضل البحيرات والأنشطة المسائية" :
                                   "الخريف — هوا لطيف، مناسب جداً للرحلات";

            string systemPrompt = ChatbotPrompts.BuildRespondPrompt(userName, currentMonth, currentSeason, dbContextText, userPreferencesContext);
            var responseResult = await openRouterService.RespondAsync(content, history, systemPrompt, cancellationToken);
            
            botResponseText = responseResult.Text;
            suggestions = responseResult.Suggestions;
        }

        // Save conversation messages to database
        var userMessage = ChatbotMessage.Create(session.Id, "user", content);
        var botMessage = ChatbotMessage.Create(session.Id, "model", botResponseText);

        context.ChatbotMessages.Add(userMessage);
        context.ChatbotMessages.Add(botMessage);

        session.UpdateLastMessageAt();
        await context.SaveChangesAsync(cancellationToken);

        // Serialize output into ChatbotFinalResponse DTO as JSON string for result compatibility
        var finalResponse = new ChatbotFinalResponse(
            session.Id,
            botResponseText,
            cards,
            suggestions,
            session.Id.ToString()
        );

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var responseJson = JsonSerializer.Serialize(finalResponse, jsonOptions);
        return new ChatbotMessageResult(session.Id, responseJson);
    }

    public async Task<List<ChatbotMessageResponse>> GetHistoryAsync(
        string deviceId,
        Guid? sessionId,
        CancellationToken cancellationToken)
    {
        ChatbotSession? session = null;

        if (sessionId.HasValue && sessionId.Value != Guid.Empty)
        {
            session = await context.ChatbotSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId.Value, cancellationToken);
        }

        if (session == null)
        {
            session = await context.ChatbotSessions
                .FirstOrDefaultAsync(s => s.DeviceId == deviceId, cancellationToken);
        }

        if (session == null)
        {
            return new List<ChatbotMessageResponse>();
        }

        return await context.ChatbotMessages
            .Where(m => m.SessionId == session.Id)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new ChatbotMessageResponse(
                m.Id,
                m.Role,
                m.Content,
                m.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }

    private async Task<List<HousingUnit>> SearchAccommodationsAsync(SearchFilters filters, string? queryText, CancellationToken ct)
    {
        var query = context.HousingUnits
            .Where(h => h.Status == ItemStatus.Active);

        if (filters != null)
        {
            if (filters.BudgetMax.HasValue && filters.BudgetMax.Value > 0)
                query = query.Where(h => h.PricePerNight <= filters.BudgetMax.Value);

            if (filters.Guests.HasValue && filters.Guests.Value > 0)
                query = query.Where(h => h.MaxGuests >= filters.Guests.Value);

            if (!string.IsNullOrEmpty(filters.Area))
                query = query.Where(h => h.AddressDetails.Contains(filters.Area) || h.Title.Contains(filters.Area));
        }

        var keywords = ExtractKeywords(queryText);
        if (keywords.Any())
        {
            var keywordPredicate = BuildKeywordPredicate<HousingUnit>(
                keywords,
                h => h.Title,
                h => h.Description
            );
            query = query.Where(keywordPredicate);
        }

        return await query
            .OrderByDescending(h => h.Rating)
            .Take(4)
            .ToListAsync(ct);
    }

    private async Task<List<GuidePackage>> SearchGuidePackagesAsync(SearchFilters filters, string? queryText, CancellationToken ct)
    {
        var query = context.GuideTourPackages
            .Where(p => p.PackageStatus == ItemStatus.Active && p.IsActive);

        if (filters != null)
        {
            if (filters.BudgetMax.HasValue && filters.BudgetMax.Value > 0)
                query = query.Where(p => p.AdultPrice <= filters.BudgetMax.Value);

            if (filters.Guests.HasValue && filters.Guests.Value > 0)
                query = query.Where(p => p.MaxCapacity >= filters.Guests.Value);

            if (!string.IsNullOrEmpty(filters.Area))
                query = query.Where(p => p.Title.Contains(filters.Area) || p.Description.Contains(filters.Area));
        }

        var keywords = ExtractKeywords(queryText);
        if (keywords.Any())
        {
            var keywordPredicate = BuildKeywordPredicate<GuidePackage>(
                keywords,
                p => p.Title,
                p => p.Description
            );
            query = query.Where(keywordPredicate);
        }

        return await query
            .Take(4)
            .ToListAsync(ct);
    }

    private async Task<List<Location>> SearchPlacesAsync(SearchFilters filters, string? queryText, CancellationToken ct)
    {
        var query = context.Locations.AsQueryable();

        if (filters != null && !string.IsNullOrEmpty(filters.Area))
        {
            query = query.Where(l => l.Name.Contains(filters.Area) || (l.Description != null && l.Description.Contains(filters.Area)));
        }

        var keywords = ExtractKeywords(queryText);
        if (keywords.Any())
        {
            var keywordPredicate = BuildKeywordPredicate<Location>(
                keywords,
                l => l.Name,
                l => l.Description
            );
            query = query.Where(keywordPredicate);
        }

        return await query
            .OrderByDescending(l => l.Rating)
            .Take(4)
            .ToListAsync(ct);
    }

    private List<string> GetAskSuggestions(string question)
    {
        var q = question.ToLower();
        if (q.Contains("يوم") || q.Contains("أيام") || q.Contains("كام"))
            return new List<string> { "يوم واحد 🗓️", "يومين", "3 أيام", "أسبوع" };
        if (q.Contains("شخص") || q.Contains("ناس") || q.Contains("كام"))
            return new List<string> { "شخص واحد 👤", "اتنين 👫", "3-4 أشخاص", "أكتر من 4" };
        if (q.Contains("مزاج") || q.Contains("طبيعة") || q.Contains("تاريخ") || q.Contains("مغامرة"))
            return new List<string> { "طبيعة وهدوء 🌿", "تاريخ وآثار 🏛️", "مغامرة وسفاري 🐪", "مزيج من الكل" };
        if (q.Contains("مين") || q.Contains("مع") || q.Contains("عيلة") || q.Contains("كابل"))
            return new List<string> { "لوحدي 🎒", "كابل 💑", "عيلة 👨‍👩‍👧", "أصحاب 👫" };
        
        return new List<string> { "فنادق في الفيوم 🏨", "أماكن سياحية 🏛️", "مرشد سياحي 🗺️", "خطة رحلة 📅" };
    }

    private static readonly HashSet<string> ArabicStopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "في", "من", "على", "إلى", "عن", "مع", "أو", "ثم", "يا", "هذا", "هذه", "التي", "الذي", "عايز", "عايزة", "عاوز", "عاوزة",
        "حابب", "حاببة", "بتاع", "فندق", "شاليه", "مكان", "سكن", "إقامة", "رحلة", "جولة", "مرشد", "تكون", "يكون", "عندي", "أنا",
        "نحن", "هو", "هي", "هم", "هن", "أنت", "أنتم", "تفاصيل", "معلومات", "حاجة", "حاجات", "أماكن", "مكان", "أريد"
    };

    private static List<string> ExtractKeywords(string? queryText)
    {
        if (string.IsNullOrWhiteSpace(queryText)) return new List<string>();

        var words = queryText.Split(new[] { ' ', ',', '.', '؟', '!', '-', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        var keywords = new List<string>();

        foreach (var word in words)
        {
            var cleaned = word.Trim().ToLower();
            if (cleaned.Length > 2 && !ArabicStopWords.Contains(cleaned))
            {
                keywords.Add(cleaned);
            }
        }

        return keywords;
    }

    private static Expression<Func<T, bool>> BuildKeywordPredicate<T>(
        List<string> keywords,
        Expression<Func<T, string>> titleProperty,
        Expression<Func<T, string?>> descriptionProperty)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? body = null;

        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;

        foreach (var keyword in keywords)
        {
            var keywordConst = Expression.Constant(keyword, typeof(string));

            var titleExpr = ReplaceParameter(titleProperty, parameter);
            var titleContains = Expression.Call(titleExpr, containsMethod, keywordConst);

            Expression keywordMatch = titleContains;

            if (descriptionProperty != null)
            {
                var descExpr = ReplaceParameter(descriptionProperty, parameter);
                var nullConst = Expression.Constant(null, typeof(string));
                var notNullExpr = Expression.NotEqual(descExpr, nullConst);
                var descContains = Expression.Call(descExpr, containsMethod, keywordConst);
                var descMatch = Expression.AndAlso(notNullExpr, descContains);

                keywordMatch = Expression.OrElse(titleContains, descMatch);
            }

            body = body == null ? keywordMatch : Expression.OrElse(body, keywordMatch);
        }

        if (body == null)
        {
            return x => true;
        }

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private static Expression ReplaceParameter(LambdaExpression lambda, ParameterExpression newParameter)
    {
        var visitor = new ParameterReplacer(lambda.Parameters[0], newParameter);
        return visitor.Visit(lambda.Body);
    }

    private class ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == oldParameter ? newParameter : base.VisitParameter(node);
        }
    }
}
