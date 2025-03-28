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
using ReadingService.Models;

namespace ReadingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TLogsController : ControllerBase
    {
        private readonly dbYuantaProjectContext _context;

        public TLogsController(dbYuantaProjectContext context)
        {
            _context = context;
        }

        // GET: api/TLogs
        [HttpGet("myLogs")]
        public async Task<IActionResult> GetReadingLogs()
        {
            var logs = await _context.TLogs
                .Include(l=>l.FDocument)
                .Include(l => l.FUser)
                .Select(l => new
                {
                    l.FLogId,
                    l.FUserId,
                    fUserName= l.FUser.FUserName,
                    l.FDocumentId,
                    fTitle=l.FDocument.FTitle,
                    l.FStartTime,
                    l.FEndTime,
                    l.FClientIp
                })
                .ToListAsync();

            return Ok(logs);
        }

        private bool TLogExists(int id)
        {
            return _context.TLogs.Any(e => e.FLogId == id);
        }
    }
}
