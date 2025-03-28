using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReadingService.DTO;
using ReadingService.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ReadingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly dbYuantaProjectContext _context;
        private readonly string _secretKey = "b6t8fJH2WjwYgJt7XPTqVX37WYgKs8TZ";
        public AuthController(dbYuantaProjectContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO dto)
        {
            try
            {
                var user = _context.TUsers
                    .FirstOrDefault(u => u.FAccount == dto.Account && u.FPassword == dto.Password);
                if (user == null)
                {
                    return Unauthorized(new { Success = false, Message = "帳號或密碼錯誤" });
                }

                //Token
                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.FUserId.ToString()),
                new Claim(ClaimTypes.Name, user.FUserName)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "https://yuantaprojectapi-dbg3e4e7dcb6aegr.eastasia-01.azurewebsites.net",
                    audience: "https://yuantaprojectwebsite-gtcwb2bdf4hbfvdt.eastasia-01.azurewebsites.net",
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    Success = true,
                    Token = tokenString,
                    UserName = user.FUserName,
                    UserId = user.FUserId
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "伺服器錯誤：" + ex.Message });
            }
        }
    }
}
