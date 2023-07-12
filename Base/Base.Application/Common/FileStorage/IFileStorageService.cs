namespace Base.Application.Common.FileStorage;

public interface IFileStorageService : ITransientService
{
    public Task<string> UploadAsync<T>(FileUploadRequest? request, FileType supportedFileType,
        CancellationToken cancellationToken)
        where T : class;

    public void Remove(string? path);
}