using Domain.Enums;

namespace Application.Features.Files.Commands.CallBack
{
    internal class CallBackCommandHandler : IRequestHandler<CallBackCommand, Unit>
    {
        private readonly IFileRepository _fileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CallBackCommandHandler(IFileRepository fileRepository, IUnitOfWork unitOfWork)
        {
            _fileRepository = fileRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(CallBackCommand request, CancellationToken cancellationToken)
        {
            var file = await _fileRepository.GetFileByIdAsync(request.FileId, cancellationToken);
            if (file == null)
                return Unit.Value;

            if (request.Status == FileStatus.Success)
                file.Status = FileStatus.Success;
            else
                await _fileRepository.DeleteFileAsync(file, cancellationToken);

            await _unitOfWork.SaveAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
