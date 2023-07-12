using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Base.Infrastructure.Extensions;

public static class ConfigureContainer
{
    public static void ConfigureSwagger(this IApplicationBuilder app)
    {
    }

    public static void ConfigureSwagger(this ILoggerFactory loggerFactory)
    {
        loggerFactory.AddSerilog();
    }

    public static void UseHealthCheck(this IApplicationBuilder app)
    {
    }
}