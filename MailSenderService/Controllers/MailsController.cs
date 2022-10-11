using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MailSenderService.Data;
using MailSenderService.Data.Models;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MailSenderService.Data.Dto;

namespace MailSenderService.Controllers
{
    /// <summary>
    /// Это основной API контроллер.
    /// В нем расположено 2 API метода и конструктор.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MailsController : ControllerBase
    {
        private readonly MSDBcontext _context;

        private readonly IConfiguration _configuration;

        /// <summary>
        /// В данном конструкторе инициализируются контекст базы данных и конфигураций.
        /// </summary>
        /// <param name="context">Контекст БД</param>
        /// <param name="configuration">Конфигураций</param>
        public MailsController(MSDBcontext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Это API метод возвращающий все сообщения из БД включая поля п.2.2
        /// </summary>
        /// <return>Возвращает все из сущности Mails и MailsResult</return>
        [HttpGet]
        public IQueryable<MailsDto> GetMails()
        {
            var mails = from m in _context.Mails.Include(m => m.MailsResult) 
                        select new MailsDto()
                        {
                            Id = m.Id,
                            Recipient = m.Recipient,
                            Subject = m.Subject,
                            Body = m.Body,
                            CreatedDate = m.MailsResult.CreatedDate,
                            FailedMessage = m.MailsResult.FailedMessage,
                            Result = m.MailsResult.Result,
                        };
            return mails;
        }


        /// <summary>
        /// Это API метод принимающий в себя поля mails
        /// </summary>
        /// <param name="mails"> Это тело запроса</param>
        /// <return>Возвращает новые созданные сущности Mails и MailsResult</return>
        [HttpPost]
        public async Task<ActionResult<Mails>> PostMails(Mails mails)
        {
            MailsResult result = new MailsResult();

           
            
          
            try
            {
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress("Администрация сайта", _configuration["SMTP:Mail"]));
                emailMessage.To.Add(new MailboxAddress("", mails.Recipient));
                emailMessage.Subject = mails.Subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = mails.Body
                };
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_configuration["SMTP:Domain"], Convert.ToInt32(_configuration["SMTP:PORT"]), Convert.ToBoolean(_configuration["SMTP:SSL"]));
                    await client.AuthenticateAsync(_configuration["SMTP:Mail"], _configuration["SMTP:Password"]);
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

            mails.MailsResult = result;
            result.CreatedDate = DateTime.Now;

            _context.Mails.Add(mails);
            _context.MailsResults.Add(result);

            await _context.SaveChangesAsync();

            var resultmail = new MailsDto()
            {
                Id = mails.Id,
                Recipient = mails.Recipient,
                Subject = mails.Subject,
                Body = mails.Body,
                CreatedDate = mails.MailsResult.CreatedDate,
                FailedMessage = mails.MailsResult.FailedMessage,
                Result = mails.MailsResult.Result,
            };

           
            return CreatedAtAction("GetMails", new { id = mails.Id }, resultmail);
        }
    }
}
