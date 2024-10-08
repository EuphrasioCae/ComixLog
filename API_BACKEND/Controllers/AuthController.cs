using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ComixLog.Models;
using ComixLog.Services;
 
namespace ComixLog.Controllers;
 
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // Chave secreta (deve ter pelo menos 32 caracteres para HS256)
    private const string SecretKey = "MySuperSecureKey1234567890!@#$%^"; // Chave com 32 caracteres
 
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel login, [FromServices] UsersService usersService)
    {
        // Obtenha o usuário com base no nome de usuário ou email
        var user = usersService.GetByUsernameAsync(login.Username).Result;
 
        // Verifique se o usuário existe e a senha está correta
        if (user == null || user.Password != login.Password)
        {
            return Unauthorized("Credenciais inválidas.");
        }
 
        // Verifique se o usuário é Admin
        var isAdmin = user.Admin.HasValue && user.Admin.Value;
 
        // Se o usuário não for Admin, retornar credenciais inválidas
        if (!isAdmin)
        {
            return Unauthorized("Credenciais inválidas. Acesso restrito a administradores.");
        }
 
        // Claims dinâmicas (adiciona a role Admin)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name ?? user.EmailAddress),
            new Claim(ClaimTypes.Role, "Admin") // Define o papel como "Admin"
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
 
        // Retornar o token gerado
        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            isAdmin = isAdmin // Confirmação que o usuário é Admin
        });
    }
}
 
// Modelo de login
public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}
