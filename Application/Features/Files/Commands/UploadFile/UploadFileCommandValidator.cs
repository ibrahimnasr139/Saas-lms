using Application.Contracts.Files;
using FluentValidation;

namespace Application.Features.Files.Commands.UploadFile
{
    public sealed class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
    {
        private readonly IFileService _fileService;
        public UploadFileCommandValidator(IFileService fileService)
        {
            _fileService = fileService;

            RuleFor(x => x.File)
                .NotNull().WithMessage("File must be provided.")
                .Must(file => file!.Length > 0)
                .WithMessage("File size must be greater than zero.");

            RuleFor(x => x.File)
                .Must(file => file != null &&
                    (file.ContentType.StartsWith(FileConstants.Image) ||
                    file.ContentType.StartsWith(FileConstants.Video) ||
                    file.ContentType == FileConstants.Pdf))
                .WithMessage("Unsupported file type.");


            RuleFor(x => x.File)
                .Must((request, file) =>
                {
                    var type = _fileService.GetFileType(file!.ContentType);
                    return file.Length <= _fileService.GetMaxSize(type);
                })
                .When(x => x.File != null)
                .WithMessage(request =>
                {
                    var type = _fileService.GetFileType(request.File!.ContentType);
                    return $"File size exceeds allowed limit for {type}.";
                });

            RuleFor(x => x.Folder)
                .Must(folder => string.IsNullOrEmpty(folder) || folder.Length <= 100)
                .WithMessage("Folder name is too long.");
        }
    }
}
