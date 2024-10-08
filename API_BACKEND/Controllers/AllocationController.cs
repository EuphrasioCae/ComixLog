using ComixLog.Models;
using ComixLog.Services;
using Microsoft.AspNetCore.Mvc;

namespace ComixLog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AllocationController : ControllerBase
    {
        private readonly IAllocationService _alocacaoService;

        public AllocationController(IAllocationService allocationService)
        {
            _alocacaoService = allocationService;
        }

        [HttpPost("{containerId:length(24)}/alocar/{userId:length(24)}/{quantidade:int}")]
        public async Task<IActionResult> AlocarUsuario(string containerId, string userId, int quantidade)
        {
            try
            {
                await _alocacaoService.AlocarUsuarioAsync(containerId, userId, quantidade);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }

        [HttpPost("{containerId:length(24)}/remover/{userId:length(24)}")]
        public async Task<IActionResult> RemoverUsuario(string containerId, string userId)
        {
            try
            {
                await _alocacaoService.RemoverUsuarioAsync(containerId, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }

        [HttpGet("usuario/{userId:length(24)}/containers")]
        public async Task<ActionResult<List<Container>>> GetContainersByUser(string userId)
        {
            try
            {
                var containers = await _alocacaoService.GetContainersByUserAsync(userId);

                if (containers == null || containers.Count == 0)
                {
                    return NotFound(new ErrorResponse($"Nenhum container encontrado para o usuário com ID {userId}."));
                }

                return Ok(containers);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }
    }
}
