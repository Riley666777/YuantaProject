using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadingService.DTO;
using ReadingService.Models;

namespace ReadingService.Controllers
{
    [EnableCors("ReadingWebsite")]
    [Route("api/[controller]")]  
    [ApiController]
    public class TArticlesController : ControllerBase
    {
        private readonly dbYuantaProjectContext _context;

        public TArticlesController(dbYuantaProjectContext context)
        {
            _context = context;
        }

        // GET: api/TArticles
        [HttpGet("GetTitles")]
        public async Task<IActionResult> GetAllTitles()
        {
            var allArticles = await _context.TArticles
                .Select(a => new { a.FDocumentId, a.FTitle })
                .ToListAsync();
            return Ok(allArticles);
        }

        // GET: api/TArticles/5
        [HttpGet("GetArticle/{id}")]
        public async Task<IActionResult> GetArticle(int id)
        {
            var article = await _context.TArticles.FindAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                fTitle = article.FTitle,
                fContent = article.FContent
            });
        }

        [Authorize]
        [HttpPost("start")]
        public async Task<IActionResult> StartReading([FromBody] ReadingStartDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                //Console.WriteLine("userId: " + userId);
                if (userId == null)
                {
                    return Unauthorized();
                }

                var log = new TLog
                {
                    FUserId = userId,
                    FDocumentId = dto.FDocumentId,
                    FStartTime = DateTime.Now,
                    FClientIp = HttpContext.Connection.RemoteIpAddress.ToString()
                };

                _context.TLogs.Add(log);
                await _context.SaveChangesAsync();
                return Ok(new { readingLogId = log.FLogId });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpPut("end/{id}")]
        public async Task<IActionResult> EndReading(int id)
        {
            var log = await _context.TLogs.FindAsync(id);
            if (log == null)
            {
                return NotFound();
            }
            log.FEndTime = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool TArticleExists(int id)
        {
            return _context.TArticles.Any(e => e.FDocumentId == id);
        }
    }
}
