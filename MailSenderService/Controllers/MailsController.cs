using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MailSenderService.Data;
using MailSenderService.Data.Models;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;

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
            return await _context.Mails.Include(m => m.MailsResult).ToListAsync();
        }

        
        // POST: api/Mails
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Mails>> PostMails(Mails mails)
        {

            MailsResult result = new MailsResult();
            
            _context.Mails.Add(mails);
            await _context.SaveChangesAsync();

            result.MailsId = new { id = mails.Id }.id;
            result.CreatedDate = DateTime.Now;
            _context.MailsResults.Add(result);
            await _context.SaveChangesAsync();

            try
            {
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress("Администрация сайта", "@mail.ru"));
                emailMessage.To.Add(new MailboxAddress("", mails.Recipient));
                emailMessage.Subject = mails.Subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = mails.Body
                };
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.mail.ru", 25, false);
                    await client.AuthenticateAsync("@mail.ru", "");
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                    result.Result = "OK";
                }
            }
            catch (Exception e)
            {
                result.Result = "Failed";
                result.FailedMessage = e.GetBaseException().Message;
            }

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
