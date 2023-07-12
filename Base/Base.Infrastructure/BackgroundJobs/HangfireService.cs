using System.Linq.Expressions;
using Base.Application.Common.Job;
using Hangfire;

namespace Base.Infrastructure.BackgroundJobs;

public class HangfireService : IJobService
{
    public bool Delete(string jobId)
    {
        return BackgroundJob.Delete(jobId);
    }

    public bool Delete(string jobId, string fromState)
    {
        return BackgroundJob.Delete(jobId, fromState);
    }

    public string Enqueue(Expression<Func<Task>> methodCall)
    {
        return BackgroundJob.Enqueue(methodCall);
    }

    public string Enqueue<T>(Expression<Action<T>> methodCall)
    {
        return BackgroundJob.Enqueue(methodCall);
    }

    public string Enqueue(Expression<Action> methodCall)
    {
        return BackgroundJob.Enqueue(methodCall);
    }

    public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
    {
        return BackgroundJob.Enqueue(methodCall);
    }

    public bool Requeue(string jobId)
    {
        return BackgroundJob.Requeue(jobId);
    }

    public bool Requeue(string jobId, string fromState)
    {
        return BackgroundJob.Requeue(jobId, fromState);
    }

    public string Schedule(Expression<Action> methodCall, TimeSpan delay)
    {
        return BackgroundJob.Schedule(methodCall, delay);
    }

    public string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay)
    {
        return BackgroundJob.Schedule(methodCall, delay);
    }

    public string Schedule(Expression<Action> methodCall, DateTimeOffset enqueueAt)
    {
        return BackgroundJob.Schedule(methodCall, enqueueAt);
    }

    public string Schedule(Expression<Func<Task>> methodCall, DateTimeOffset enqueueAt)
    {
        return BackgroundJob.Schedule(methodCall, enqueueAt);
    }

    public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
    {
        return BackgroundJob.Schedule(methodCall, delay);
    }

    public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
    {
        return BackgroundJob.Schedule(methodCall, delay);
    }

    public string Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt)
    {
        return BackgroundJob.Schedule(methodCall, enqueueAt);
    }

    public string Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt)
    {
        return BackgroundJob.Schedule(methodCall, enqueueAt);
    }

    public void AddOrUpdate(string id, Expression<Func<Task>> methodCall, Func<string> cron, TimeZoneInfo timeZone,
        string queue)
    {
        RecurringJob.AddOrUpdate(id, methodCall, cron, timeZone, queue);
    }

    public void AddOrUpdate<T>(string id, Expression<Func<T, Task>> methodCall, Func<string> cron,
        TimeZoneInfo timeZone, string queue)
    {
        RecurringJob.AddOrUpdate(id, methodCall, cron, timeZone, queue);
    }

    public void AddOrUpdate(string id, Expression<Action> methodCall, Func<string> cron, TimeZoneInfo timeZone,
        string queue)
    {
        RecurringJob.AddOrUpdate(id, methodCall, cron, timeZone, queue);
    }

    public void AddOrUpdate<T>(string id, Expression<Action<T>> methodCall, Func<string> cron, TimeZoneInfo timeZone,
        string queue)
    {
        RecurringJob.AddOrUpdate(id, methodCall, cron, timeZone, queue);
    }

    public void RemoveIfExist(string jobId)
    {
        RecurringJob.RemoveIfExists(jobId);
    }
}