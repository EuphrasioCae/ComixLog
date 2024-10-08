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
            if (!IsValidCnpj(newUser.CNPJ))
            {
                return BadRequest("CNPJ inválido.");
            }
        
            await _usersService.CreateAsync(newUser);
            return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
        
        }

        private bool IsValidCnpj(string cnpj)
        {
            // Remove caracteres especiais
            cnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
        
            if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length != 14 || !long.TryParse(cnpj, out _))
                return false;
        
            var cnpjBase = cnpj.Substring(0, 12);
            var digits = cnpj.Substring(12, 2);
        
            var calculatedDigits = CalculateCnpjDigits(cnpjBase);
            return digits == calculatedDigits;
        }
        
        private string CalculateCnpjDigits(string cnpjBase)
        {
            var weights = new int[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var firstDigit = CalculateDigit(cnpjBase, weights);
            cnpjBase += firstDigit;
        
            weights = new int[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var secondDigit = CalculateDigit(cnpjBase, weights);
        
            return $"{firstDigit}{secondDigit}";
        }
        
        private int CalculateDigit(string baseCnpj, int[] weights)
        {
            var sum = 0;
            for (int i = 0; i < baseCnpj.Length; i++)
            {
                sum += int.Parse(baseCnpj[i].ToString()) * weights[i];
            }
        
            var remainder = sum % 11;
            return remainder < 2 ? 0 : 11 - remainder;
        }

        [HttpPut("{id:length(24)}")]
        [Authorize(Roles = "Admin")]
        
        public async Task<IActionResult> Update(string id, User userUpdated)
        {
            // Valida o CNPJ antes de prosseguir com a atualização
            if (!IsValidCnpj(userUpdated.CNPJ))
            {
                return BadRequest("CNPJ inválido.");
            }
        
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

