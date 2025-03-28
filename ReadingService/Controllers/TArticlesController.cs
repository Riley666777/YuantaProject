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
using ReadingService.Services;

namespace ReadingService.Controllers
{
    [Route("api/[controller]")]  
    [ApiController]
    public class TArticlesController : ControllerBase
    {
        private readonly dbYuantaProjectContext _context;
        private readonly TimeService _timeService;

        public TArticlesController(dbYuantaProjectContext context, TimeService timeService)
        {
            _context = context;
            _timeService = timeService;
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
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized("無效的使用者資訊");
                }

                var log = new TLog
                {
                    FUserId = userId,
                    FDocumentId = dto.FDocumentId,
                    FStartTime = _timeService.GetTaiwanNow(),
                    FClientIp = HttpContext.Connection.RemoteIpAddress.ToString()
                };

                _context.TLogs.Add(log);
                await _context.SaveChangesAsync();
                return Ok(new { readingLogId = log.FLogId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "寫入閱讀紀錄失敗", message = ex.Message });

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
            log.FEndTime = _timeService.GetTaiwanNow();
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool TArticleExists(int id)
        {
            return _context.TArticles.Any(e => e.FDocumentId == id);
        }
    }
}
