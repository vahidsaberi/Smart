﻿using Microsoft.Extensions.DependencyInjection;

namespace Base.Infrastructure.Persistence.Initialization;

public class CustomSeederRunner
{
    private readonly ICustomSeeder[] _seeders;

    public CustomSeederRunner(IServiceProvider serviceProvider)
    {
        _seeders = serviceProvider.GetServices<ICustomSeeder>().ToArray();
    }

    public async Task RunSeedersAsync(CancellationToken cancellationToken)
    {
        foreach (var seeder in _seeders) await seeder.InitializeAsync(cancellationToken);
    }
}