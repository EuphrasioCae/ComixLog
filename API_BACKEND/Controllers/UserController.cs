using ComixLog.Models;
using ComixLog.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ComixLog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _usersService;

        public UsersController(UsersService usersService)
        {
            _usersService = usersService;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<List<User>> Get() =>
            await _usersService.GetAsync();


        [HttpGet("{id:length(24)}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> Get(string id)
        {
            var user = await _usersService.GetAsync(id);
            if (user == null) return NotFound();
            return Ok(user);

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(User newUser)
        {
            await _usersService.CreateAsync(newUser);
            return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
        }

        [HttpPut("{id:length(24)}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Update(string id, User userUpdated)
        {
            var user = await _usersService.GetAsync(id);
            if (user == null) return NotFound();
            userUpdated.Id = user.Id;
            await _usersService.UpdateAsync(id, userUpdated);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _usersService.GetAsync(id);
            if (user == null) return NotFound();
            await _usersService.RemoveAsync(id);
            return NoContent();
        }


    }
}

