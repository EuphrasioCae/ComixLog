using ComixLog.Models;
using MongoDB.Driver;

namespace ComixLog.Services

{
    public class AllocationService : IAllocationService
    {
        private readonly ContainersService _containersService;

        public AllocationService(ContainersService containersService)
        {
            _containersService = containersService;
        }

        public async Task AlocarUsuarioAsync(string containerId, string userId, int quantidadeAlocada)
        {
            var container = await _containersService.GetAsync(containerId);
            if (container == null) throw new Exception("Container não encontrado.");

            if (container.CapacidadeAtual + quantidadeAlocada > container.CapacidadeTotal)
                throw new Exception("Capacidade total do container excedida.");

            if (container.UsuariosAlocados.ContainsKey(userId))
            {
                // Se o usuário já está alocado, somamos a nova quantidade
                container.UsuariosAlocados[userId] += quantidadeAlocada;
            }
            else
            {
                // Se o usuário não está alocado, adicionamos ele
                container.UsuariosAlocados[userId] = quantidadeAlocada;
            }

            // Atualizar a capacidade atual
            container.CapacidadeAtual += quantidadeAlocada;

            // Atualizar o container no banco de dados
            await _containersService.UpdateAsync(containerId, container);
        }

        public async Task RemoverUsuarioAsync(string containerId, string userId)
        {
            var container = await _containersService.GetAsync(containerId);
            if (container == null) throw new Exception("Container não encontrado.");

            if (container.UsuariosAlocados.ContainsKey(userId))
            {
                // Subtraímos a quantidade alocada desse usuário da capacidade atual
                container.CapacidadeAtual -= container.UsuariosAlocados[userId];
                // Removemos o usuário
                container.UsuariosAlocados.Remove(userId);

                // Atualizar o container no banco de dados
                await _containersService.UpdateAsync(containerId, container);
            }
            else
            {
                throw new Exception("Usuário não está alocado nesse container.");
            }
        }
        public async Task<List<Container>> GetContainersByUserAsync(string userId)
        {
            // Obter todos os containers do banco
            var containers = await _containersService.GetAsync();

            // Filtrar containers onde o usuário está alocado
            var containersDoUsuario = containers
                .Where(c => c.UsuariosAlocados.ContainsKey(userId))
                .ToList();

            return containersDoUsuario;
        }

    }
}
