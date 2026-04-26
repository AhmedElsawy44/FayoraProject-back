using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Infrastructure.Persistence.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fayora.Infrastructure.Middlewares;

public class EventualConsistencyMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    //public async Task InvokeAsync(HttpContext context, IPublisher publisher, ApplicationDbContext dbContext)
    //{
    //    var transaction = await dbContext.Database.BeginTransactionAsync();

    //    context.Response.OnCompleted(async () =>
    //    {
    //        try
    //        {
    //            if (context.Items.TryGetValue("DomainEventsQueue", out var value) &&
    //                value is Queue<IDomainEvent> domainEventsQueue)
    //            {
    //                while (domainEventsQueue!.TryDequeue(out var domainEvent))
    //                {
    //                    await publisher.Publish(domainEvent);
    //                }
    //            }

    //            await transaction.CommitAsync();
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine(ex);
    //        }
    //        finally
    //        {
    //            await transaction.DisposeAsync();
    //        }

    //    });

    //    await _next(context);
    //}

    public async Task InvokeAsync(HttpContext context, IPublisher publisher, ApplicationDbContext dbContext)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            await _next(context); // شغل الـ request الأول

            // دلوقتي publish الـ events قبل ما الـ response يتبعت
            if (context.Items.TryGetValue("DomainEventsQueue", out var value) &&
                value is Queue<IDomainEvent> domainEventsQueue)
            {
                while (domainEventsQueue.TryDequeue(out var domainEvent))
                {
                    await publisher.Publish(domainEvent);
                }
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine(ex);
            throw; // عشان الـ error handling يشتغل صح
        }
        finally
        {
            await transaction.DisposeAsync();
        }
    }
}