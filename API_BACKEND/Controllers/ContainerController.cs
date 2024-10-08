using ComixLog.Models;
using ComixLog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ComixLog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContainersController : ControllerBase
    {
        private readonly ContainersService _containersService;

        public ContainersController(ContainersService containersService)
        {
            _containersService = containersService;
        }
        [HttpGet]
        public async Task<List<Container>> Get() =>
            await _containersService.GetAsync();


        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Container>> Get(string id)
        {
            var container = await _containersService.GetAsync(id);
            if (container == null) return NotFound();
            return Ok(container);

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(Container newContainer)
        {
            // Validação da capacidade
            if (newContainer.CapacidadeAtual > newContainer.CapacidadeTotal)
            {
                return BadRequest("Capacidade atual excede a capacidade total.");
            }

            await _containersService.CreateAsync(newContainer);
            return CreatedAtAction(nameof(Get), new { id = newContainer.Id }, newContainer);
        }

        [HttpPut("{id:length(24)}")]
        [Authorize(Roles = "Admin")]


            public async Task<IActionResult> Update(string id, Container containerUpdated)
            {
                try
                {
                    var container = await _containersService.GetAsync(id);
                    if (container == null) throw new Exception("Container não encontrado.");

                    // Atualizar as propriedades do contêiner
                    containerUpdated.Id = container.Id;

                    // Verificar se os novos valores são válidos antes de atualizar
                    if (containerUpdated.CapacidadeAtual > containerUpdated.CapacidadeTotal)
                    throw new Exception("Capacidade atual excede a capacidade total.");

                    // Atualizar o contêiner
                    await _containersService.UpdateAsync(id, containerUpdated);

                    return NoContent();
                }
                catch (Exception ex)
                {
                    return BadRequest(new { error = ex.Message });
                }
            }
        
        [HttpDelete("{id:length(24)}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var container = await _containersService.GetAsync(id);
            if (container == null) return NotFound();
            await _containersService.RemoveAsync(id);
            return NoContent();
        }


    }
}

