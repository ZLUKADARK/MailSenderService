using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MailSenderService.Data;
using MailSenderService.Data.Models;

namespace MailSenderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailsController : ControllerBase
    {
        private readonly MSDBcontext _context;

        public MailsController(MSDBcontext context)
        {
            _context = context;
        }

        // GET: api/Mails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mails>>> GetMails()
        {
            return await _context.Mails.ToListAsync();
        }

        // GET: api/Mails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Mails>> GetMails(int id)
        {
            var mails = await _context.Mails.FindAsync(id);

            if (mails == null)
            {
                return NotFound();
            }

            return mails;
        }

        // PUT: api/Mails/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMails(int id, Mails mails)
        {
            if (id != mails.Id)
            {
                return BadRequest();
            }

            _context.Entry(mails).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MailsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Mails
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Mails>> PostMails(Mails mails)
        {
            _context.Mails.Add(mails);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMails", new { id = mails.Id }, mails);
        }

        // DELETE: api/Mails/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Mails>> DeleteMails(int id)
        {
            var mails = await _context.Mails.FindAsync(id);
            if (mails == null)
            {
                return NotFound();
            }

            _context.Mails.Remove(mails);
            await _context.SaveChangesAsync();

            return mails;
        }

        private bool MailsExists(int id)
        {
            return _context.Mails.Any(e => e.Id == id);
        }
    }
}
