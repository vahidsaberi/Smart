﻿namespace Base.Infrastructure.BackgroundJobs.RecurringJobs;

public interface IRecurringJobInitialization
{
    void InitializeRecurringJobs(string tenantId);
    Task InitializeJobsForTenantAsync(CancellationToken cancellationToken);
}