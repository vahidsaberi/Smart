namespace Base.Application.Common.FileStorage;

public class FileUploadRequest
{
    public string Name { get; set; } = default!;
    public string Extension { get; set; } = default!;
    public string Data { get; set; } = default!;
}

public class FileUploadRequestValidator : CustomValidator<FileUploadRequest>
{
    public FileUploadRequestValidator(IStringLocalizer<FileUploadRequestValidator> T)
    {
        RuleFor(_ => _.Name).NotEmpty().WithMessage("Image name cannot be empty!").MaximumLength(150);
        RuleFor(_ => _.Extension).NotEmpty().WithMessage("Image extension cannot be empty!").MaximumLength(5);
        RuleFor(_ => _.Data).NotEmpty().WithMessage("Image data cannot be empty!");
    }
}