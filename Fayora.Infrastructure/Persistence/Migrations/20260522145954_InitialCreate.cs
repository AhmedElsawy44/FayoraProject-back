using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Fayora.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceType = table.Column<int>(type: "int", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ServiceFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PayoutAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SeatsCount = table.Column<int>(type: "int", nullable: false),
                    AppliedCancelPolicy = table.Column<int>(type: "int", nullable: false),
                    BookingStatus = table.Column<int>(type: "int", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsScanned = table.Column<bool>(type: "bit", nullable: false),
                    ScannedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsCashOnArrival = table.Column<bool>(type: "bit", nullable: false),
                    DepositAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalendarBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceType = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BlockReason = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarBlocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatbotSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatbotSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecondUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScopeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ScopeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    DeviceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastActiveAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuideRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TripPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    BudgetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NumberOfPeople = table.Column<int>(type: "int", nullable: false),
                    MeetingPointLatitude = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    MeetingPointLongitude = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuideTourPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", nullable: false),
                    TourTypes = table.Column<int>(type: "int", nullable: false),
                    ProviderType = table.Column<int>(type: "int", nullable: false),
                    DurationHours = table.Column<int>(type: "int", nullable: false),
                    NumOfDays = table.Column<int>(type: "int", nullable: false),
                    MaxCapacity = table.Column<int>(type: "int", nullable: false),
                    AdultPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ChildPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false),
                    MainImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    MainVideoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    GuestRequirements = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    CancellationPolicy = table.Column<int>(type: "int", nullable: false),
                    PackageStatus = table.Column<int>(type: "int", nullable: false),
                    MeetingPointLatitude = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    MeetingPointLongitude = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ArrivalNote = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    TransportType = table.Column<int>(type: "int", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AdminNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExcludedItemIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IncludedItemIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocationIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideTourPackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuideWeeklySchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GuideId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideWeeklySchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HousingUnitImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HousingUnitImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HousingUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    AddressDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumberOfRooms = table.Column<int>(type: "int", nullable: false),
                    BedRooms = table.Column<int>(type: "int", nullable: false),
                    BathRooms = table.Column<int>(type: "int", nullable: false),
                    NumberOfBeds = table.Column<int>(type: "int", nullable: false),
                    MaxGuests = table.Column<int>(type: "int", nullable: false),
                    CheckInTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    CheckOutTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    PricePerNight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amenities = table.Column<long>(type: "bigint", nullable: false),
                    CancellationPolicy = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false),
                    MainImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AdminNotes = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    ImageIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HousingUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocationImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    MainImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ImageIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MasterInterests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterInterests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PackageActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ActivityTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    IsOptional = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageActivities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PackageImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GatewayOrderId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    GatewayTransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.BookingId);
                });

            migrationBuilder.CreateTable(
                name: "PushCampaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    TargetAudience = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ScheduledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SentAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SuccessCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    FailureCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HangfireJobId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushCampaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionTiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlanName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AIChatLimit = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionTiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TourCompanies",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsSuperCompany = table.Column<bool>(type: "bit", nullable: false),
                    LicenseDocumentUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    LicenseClass = table.Column<int>(type: "int", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    AverageRating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    CompletedToursCount = table.Column<int>(type: "int", nullable: false),
                    IsAvailableForBooking = table.Column<bool>(type: "bit", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false),
                    ResponseRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CancellationRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AdminNotes = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TourPackageIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourCompanies", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "TourGuides",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PricingUnit = table.Column<int>(type: "int", nullable: true),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LicenseExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsSuperGuide = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Latitude = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    LastLocationUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TransportInfo = table.Column<int>(type: "int", nullable: true),
                    ProfessionalLicenseUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CancellationPolicy = table.Column<int>(type: "int", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    AverageRating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    CompletedToursCount = table.Column<int>(type: "int", nullable: false),
                    IsAvailableForBooking = table.Column<bool>(type: "bit", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false),
                    ResponseRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CancellationRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AdminNotes = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourGuides", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "TouristProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravelStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BudgetTier = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastLocationLatitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastLocationLongitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastLocationUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    OnboardingComplete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TouristProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TouristSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreateAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AIChatLimit = table.Column<int>(type: "int", nullable: false),
                    AiMessagesUsed = table.Column<int>(type: "int", nullable: false),
                    AutoRenew = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TouristSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitOwners",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerType = table.Column<int>(type: "int", nullable: false),
                    ResponseRate = table.Column<float>(type: "real", nullable: false),
                    AvgResponseTimeMinutes = table.Column<int>(type: "int", nullable: false),
                    OwnerRating = table.Column<float>(type: "real", nullable: false),
                    IsSuperHost = table.Column<bool>(type: "bit", nullable: false),
                    CommercialName = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    VerifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CancellationRate = table.Column<float>(type: "real", nullable: false),
                    PreferredPayoutMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitOwners", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "UserIdentities",
                columns: table => new
                {
                    Provider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LinkedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIdentities", x => new { x.Provider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    LastName = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "DATE", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PasswordChangedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastOtpSentAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockedUntil = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsPhoneVerified = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "DECIMAL(18,4)", nullable: false),
                    SimCountryIsoCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PreferredLanguage = table.Column<int>(type: "int", nullable: true),
                    TimeZone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProfileImageUrl = table.Column<string>(type: "NVARCHAR(2048)", nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR(1000)", nullable: true),
                    NationalityCode = table.Column<string>(type: "NVARCHAR(3)", maxLength: 3, nullable: true),
                    LastLogin = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastFailedLoginAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsProfileComplete = table.Column<bool>(type: "bit", nullable: false),
                    ViolationCount = table.Column<int>(type: "int", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    LastViolationDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Roles = table.Column<int>(type: "int", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UserLanguageProficiencies = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HashedToken = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RevokedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeviceId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TokenType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatbotMessages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatbotMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatbotMessages_ChatbotSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "ChatbotSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReadAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeleteAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuideOffers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GuideId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProposedPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuideOffers_GuideRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "GuideRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackageOccurrences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    AvailableSeats = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageOccurrences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageOccurrences_GuideTourPackages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "GuideTourPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TouristInterests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TouristId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TouristInterests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TouristInterests_MasterInterests_InterestId",
                        column: x => x.InterestId,
                        principalTable: "MasterInterests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GuideCities",
                columns: table => new
                {
                    GuideId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideCities", x => new { x.GuideId, x.CityId });
                    table.ForeignKey(
                        name: "FK_GuideCities_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuideCities_TourGuides_GuideId",
                        column: x => x.GuideId,
                        principalTable: "TourGuides",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FCMToken = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    DeviceLanguage = table.Column<int>(type: "int", nullable: false),
                    IsBanned = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUsedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDevices_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserInteractions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    InteractionType = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInteractions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInteractions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VerificationCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Target = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CodeHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Purpose = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RevokedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationCodes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "MasterInterests",
                columns: new[] { "Id", "Code", "CreateAt", "IconUrl", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "NATURE", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "wwwroot/images/interests/nature.png", true, "Nature", 1 },
                    { 2, "HISTORICAL", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "wwwroot/images/interests/historical.png", true, "Historical", 2 },
                    { 3, "CULTURAL", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "wwwroot/images/interests/cultural.png", true, "Cultural", 3 },
                    { 4, "ADVENTURE", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "wwwroot/images/interests/adventure.png", true, "Adventure", 4 },
                    { 5, "CAMPING", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "wwwroot/images/interests/camping.png", true, "Camping", 5 },
                    { 6, "WILDLIFE", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "wwwroot/images/interests/wildlife.png", true, "Wildlife", 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingStatus",
                table: "Bookings",
                column: "BookingStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_EndDate",
                table: "Bookings",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PaymentStatus",
                table: "Bookings",
                column: "PaymentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ServiceId",
                table: "Bookings",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ServiceProviderId",
                table: "Bookings",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ServiceType",
                table: "Bookings",
                column: "ServiceType");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_StartDate",
                table: "Bookings",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarBlocks_BlockReason",
                table: "CalendarBlocks",
                column: "BlockReason");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarBlocks_EndDate",
                table: "CalendarBlocks",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarBlocks_ServiceId",
                table: "CalendarBlocks",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarBlocks_ServiceId_StartDate_EndDate",
                table: "CalendarBlocks",
                columns: new[] { "ServiceId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarBlocks_ServiceType",
                table: "CalendarBlocks",
                column: "ServiceType");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarBlocks_StartDate",
                table: "CalendarBlocks",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_ChatbotMessages_SessionId",
                table: "ChatbotMessages",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatbotSessions_DeviceId",
                table: "ChatbotSessions",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatbotSessions_UserId",
                table: "ChatbotSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_Participants_Scope",
                table: "Chats",
                columns: new[] { "FirstUserId", "SecondUserId", "ScopeType", "ScopeId" });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_Token",
                table: "DeviceTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_UserId",
                table: "DeviceTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideCities_CityId",
                table: "GuideCities",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideOffers_RequestId",
                table: "GuideOffers",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideWeeklySchedules_DayOfWeek",
                table: "GuideWeeklySchedules",
                column: "DayOfWeek");

            migrationBuilder.CreateIndex(
                name: "IX_GuideWeeklySchedules_GuideId",
                table: "GuideWeeklySchedules",
                column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideWeeklySchedules_GuideId_DayOfWeek",
                table: "GuideWeeklySchedules",
                columns: new[] { "GuideId", "DayOfWeek" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HousingUnits_LocationId",
                table: "HousingUnits",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_HousingUnits_OwnerId",
                table: "HousingUnits",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_HousingUnits_Status",
                table: "HousingUnits",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MasterInterests_Code",
                table: "MasterInterests",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId_CreatedAt",
                table: "Messages",
                columns: new[] { "ChatId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PackageOccurrences_PackageId_Date",
                table: "PackageOccurrences",
                columns: new[] { "PackageId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_GatewayOrderId",
                table: "PaymentTransactions",
                column: "GatewayOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_PaymentMethod",
                table: "PaymentTransactions",
                column: "PaymentMethod");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_Status",
                table: "PaymentTransactions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionTiers_PlanCode",
                table: "SubscriptionTiers",
                column: "PlanCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TouristInterests_InterestId",
                table: "TouristInterests",
                column: "InterestId");

            migrationBuilder.CreateIndex(
                name: "IX_TouristInterests_TouristId_InterestId",
                table: "TouristInterests",
                columns: new[] { "TouristId", "InterestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TouristProfiles_UserId",
                table: "TouristProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TouristSubscriptions_ActiveUserPlan",
                table: "TouristSubscriptions",
                columns: new[] { "UserId", "Status", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_UnitOwners_UserId",
                table: "UnitOwners",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitOwners_VerificationStatus",
                table: "UnitOwners",
                column: "VerificationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_FCMToken",
                table: "UserDevices",
                column: "FCMToken");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_UserId",
                table: "UserDevices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_UserId_DeviceId",
                table: "UserDevices",
                columns: new[] { "UserId", "DeviceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentities_Email",
                table: "UserIdentities",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentities_Provider_ProviderKey",
                table: "UserIdentities",
                columns: new[] { "Provider", "ProviderKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentities_UserId_Provider",
                table: "UserIdentities",
                columns: new[] { "UserId", "Provider" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInteractions_UserId",
                table: "UserInteractions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber",
                unique: true,
                filter: "[PhoneNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_HashedToken",
                table: "UserTokens",
                column: "HashedToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_UserId",
                table: "UserTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationCodes_Target_Purpose",
                table: "VerificationCodes",
                columns: new[] { "Target", "Purpose" });

            migrationBuilder.CreateIndex(
                name: "IX_VerificationCodes_UserId_Target_Purpose",
                table: "VerificationCodes",
                columns: new[] { "UserId", "Target", "Purpose" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "CalendarBlocks");

            migrationBuilder.DropTable(
                name: "ChatbotMessages");

            migrationBuilder.DropTable(
                name: "DeviceTokens");

            migrationBuilder.DropTable(
                name: "GuideCities");

            migrationBuilder.DropTable(
                name: "GuideOffers");

            migrationBuilder.DropTable(
                name: "GuideWeeklySchedules");

            migrationBuilder.DropTable(
                name: "HousingUnitImages");

            migrationBuilder.DropTable(
                name: "HousingUnits");

            migrationBuilder.DropTable(
                name: "LocationImages");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "PackageActivities");

            migrationBuilder.DropTable(
                name: "PackageImages");

            migrationBuilder.DropTable(
                name: "PackageOccurrences");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "PushCampaigns");

            migrationBuilder.DropTable(
                name: "SubscriptionTiers");

            migrationBuilder.DropTable(
                name: "TourCompanies");

            migrationBuilder.DropTable(
                name: "TouristInterests");

            migrationBuilder.DropTable(
                name: "TouristProfiles");

            migrationBuilder.DropTable(
                name: "TouristSubscriptions");

            migrationBuilder.DropTable(
                name: "UnitOwners");

            migrationBuilder.DropTable(
                name: "UserDevices");

            migrationBuilder.DropTable(
                name: "UserIdentities");

            migrationBuilder.DropTable(
                name: "UserInteractions");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "VerificationCodes");

            migrationBuilder.DropTable(
                name: "ChatbotSessions");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "TourGuides");

            migrationBuilder.DropTable(
                name: "GuideRequests");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "GuideTourPackages");

            migrationBuilder.DropTable(
                name: "MasterInterests");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
