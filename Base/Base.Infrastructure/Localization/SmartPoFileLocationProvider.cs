using Base.Infrastructure.Common.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OrchardCore.Localization;

namespace Base.Infrastructure.Localization;

public class SmartPoFileLocationProvider : ILocalizationFileLocationProvider
{
    private readonly IFileProvider _fileProvider;
    private readonly string _resourcesContainer;

    public SmartPoFileLocationProvider(IHostEnvironment hostingEnvironment,
        IOptions<LocalizationOptions> localizationOptions)
    {
        _fileProvider = hostingEnvironment.ContentRootFileProvider;
        _resourcesContainer = localizationOptions.Value.ResourcesPath;
    }

    public IEnumerable<IFileInfo> GetLocations(string cultureName)
    {
        foreach (var file in _fileProvider.GetDirectoryContents(
                     PathExtensions.Combine(_resourcesContainer, cultureName))) yield return file;
    }
}