using ComixLog.Models;

namespace ComixLog.Services
{
    public interface IAllocationService
    {
        Task AlocarUsuarioAsync(string containerId, string userId, int quantidadeAlocada);
        Task RemoverUsuarioAsync(string containerId, string userId);
        Task<List<Container>> GetContainersByUserAsync(string userId);
    }
}
