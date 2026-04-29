namespace Infrastructure.Repositories
{
    internal class FileRepository : IFileRepository
    {
        private readonly AppDbContext _context;
        public FileRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(Domain.Entites.File file, CancellationToken cancellationToken)
        {
            await _context.Files.AddAsync(file, cancellationToken);
        }
        public async Task<Domain.Entites.File?> GetFileByIdAsync(string FileId, CancellationToken cancellationToken) =>
             await _context.Files.FirstOrDefaultAsync(f => f.Id == FileId, cancellationToken);
        public async Task DeleteFileAsync(Domain.Entites.File File, CancellationToken cancellationToken) =>
            _context.Files.Remove(File);
    }
}
