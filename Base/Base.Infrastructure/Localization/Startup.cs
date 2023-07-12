using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Localization;

namespace Base.Infrastructure.Localization;

internal static class Startup
{
    internal static IServiceCollection AddPoLocalization(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var localizationSettings = configuration.GetSection(nameof(LocalizationSettings)).Get<LocalizationSettings>();

        if (localizationSettings?.EnableLocalization is true && localizationSettings.ResourcesPath is not null)
        {
            serviceCollection.AddPortableObjectLocalization(options =>
                options.ResourcesPath = localizationSettings.ResourcesPath);

            serviceCollection.Configure<RequestLocalizationOptions>(options =>
            {
                if (localizationSettings.SupportedCultures != null)
                {
                    var supportedCultures =
                        localizationSettings.SupportedCultures.Select(_ => new CultureInfo(_)).ToList();

                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                }

                options.DefaultRequestCulture =
                    new RequestCulture(localizationSettings.DefaultRequestCulture ?? "en-US");
                options.FallBackToParentCultures = localizationSettings.FallbackToParent ?? true;
                options.FallBackToParentUICultures = localizationSettings.FallbackToParent ?? true;
            });

            serviceCollection.AddSingleton<ILocalizationFileLocationProvider, SmartPoFileLocationProvider>();
        }

        return serviceCollection;
    }
}