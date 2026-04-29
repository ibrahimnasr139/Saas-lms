namespace Application.Contracts.Repositories
{
    public interface IFileRepository
    {
        Task CreateAsync(Domain.Entites.File file, CancellationToken cancellationToken);
        Task<Domain.Entites.File?> GetFileByIdAsync(string FileId, CancellationToken cancellationToken);
        Task DeleteFileAsync(Domain.Entites.File file, CancellationToken cancellationToken);
    }
}
