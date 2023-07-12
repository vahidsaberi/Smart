namespace Base.Application.Common.Job;

public interface IJobRecurringService : ITransientService
{
    string Id { get; }
    string Time { get; }
    TimeZoneInfo TimeZone { get; }
    string Queue { get; }

    Task CheckOut();
}