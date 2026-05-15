using Application.Features.Modules.Dtos;


namespace Application.Contracts.Repositories
{
    public interface IModuleRepository
    {
        Task<int> GetMaxOrder(int courseId, CancellationToken cancellationToken);
        Task IncreaseOrder(int moduleId, int courseId, int minOrder, CancellationToken cancellationToken, int maxOrder = int.MaxValue);
        Task DecreaseOrder(int moduleId, int courseId, int minOrder, CancellationToken cancellationToken, int maxOrder = int.MaxValue);
        Task<int> CreateModule(Module module, CancellationToken cancellationToken);
        Task<Module?> GetModuleByIdAsync(int moduleId, int courseId, string subdomain, CancellationToken cancellationToken);
        Task<ModuleDto?> GetModuleWithItemsAsync(int moduleId, int courseId, string subdomain, CancellationToken cancellationToken);
        Task RemoveModule(Module module, CancellationToken cancellationToken);
        Task<List<AllModulesDto>> GetAllModulesAsync(int courseId, CancellationToken cancellationToken);
        Task<int?> GetFirstModuleIdAsync(int courseId, CancellationToken cancellationToken);
        Task<string?> GetModuleNameAsync(int itemId, int courseId, CancellationToken cancellationToken);
        Task<string> GetModuleTitleAsync(int moduleId, string subDomain, CancellationToken cancellationToken);
    }
}