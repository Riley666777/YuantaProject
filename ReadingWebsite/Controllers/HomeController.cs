using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ReadingWebsite.Models;

namespace ReadingWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly dbYuantaProjectContext _context;
        private readonly string _secretKey = "b6t8fJH2WjwYgJt7XPTqVX37WYgKs8TZ";

        public HomeController(ILogger<HomeController> logger, dbYuantaProjectContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost] //MVC Controller Action
        public JsonResult Login(string account, string password)
        {
            try
            {
                var user = _context.TUsers
                .FirstOrDefault(u => u.FAccount == account && u.FPassword == password);
                if (user == null)
                {
                    return Json(new { Success = false, Message = "帳號或密碼錯誤" });
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

                //save to Cookie
                Response.Cookies.Append("jwt_token", tokenString, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddHours(1)
                });
                Response.Cookies.Append("userName", user.FUserName, new CookieOptions
                {
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddHours(1)
                });

                return Json(new { Success = true, Message = "登入成功" });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = "錯誤：" + ex.Message,
                    StackTrace = ex.StackTrace 
                });
            }
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt_token");
            Response.Cookies.Delete("userName");
            return RedirectToAction("Index");
        }

        public IActionResult ReadingSystem()
        {
            if(!Request.Cookies.ContainsKey("jwt_token"))
            {
                TempData["NotLoginMessage"] = "請先登入!";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Read(int id)
        {
            if (!Request.Cookies.ContainsKey("jwt_token"))
            {
                return RedirectToAction("Index");
            }
            ViewBag.fDocumentId = id;
            return View();
        }

        public IActionResult ReadingLogs()
        {
            if (!Request.Cookies.ContainsKey("jwt_token"))
            {
                TempData["NotLoginMessage"] = "請先登入!";
                return RedirectToAction("Index");
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
