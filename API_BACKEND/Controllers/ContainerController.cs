using ComixLog.Models;
using ComixLog.Services;
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
        public async Task<IActionResult> Post(Container newContainer)
        {
            await _containersService.CreateAsync(newContainer);
            return CreatedAtAction(nameof(Get), new { id = newContainer.Id }, newContainer);
        }

        [HttpPut("{id:length(24)}")]

        public async Task<IActionResult> Update(string id, Container containerUpdated)
        {
            var container = await _containersService.GetAsync(id);
            if (container == null) return NotFound();
            containerUpdated.Id = container.Id;
            await _containersService.UpdateAsync(id, containerUpdated);

            return NoContent();
        }
        
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var container = await _containersService.GetAsync(id);
            if (container == null) return NotFound();
            await _containersService.RemoveAsync(id);
            return NoContent();
        }


    }
}

