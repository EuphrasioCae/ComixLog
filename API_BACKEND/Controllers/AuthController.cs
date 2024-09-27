using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ComixLog.Models;

namespace ComixLog.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // Chave secreta (deve ter pelo menos 32 caracteres para HS256)
    private const string SecretKey = "MySuperSecureKey1234567890!@#$%^"; // Chave com 32 caracteres

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel login)
    {
        // Simples validação de credenciais (substitua com sua lógica de autenticação real)
        if (login.Username == "admin" && login.Password == "admin123")
        {
            // Claims (incluindo role "Admin")
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, login.Username),
                new Claim(ClaimTypes.Role, "Admin") // Adicionando a role "Admin"
            };

            // Chave secreta
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Gerar token JWT
            var token = new JwtSecurityToken(
                issuer: "yourIssuer",
                audience: "yourAudience",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            // Retornar o token
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token) // Token JWT gerado
            });
        }

        return Unauthorized(); // Retorna 401 se as credenciais forem inválidas
    }
}

public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}
