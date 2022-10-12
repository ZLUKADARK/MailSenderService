using MailKit.Net.Smtp;
using MailSenderService.Data;
using MailSenderService.Data.Dto;
using MailSenderService.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace MailSenderService.Services
{
    public class MailsSender
    {
        /// <summary>
        /// В данном конструкторе инициализируются контекст базы данных и конфигураций.
        /// </summary>
        /// <param name="context">Контекст БД</param>
        /// <param name="configuration">Конфигураций</param>

        public async Task<MailsDto> MassageSender(MailsBodyDto mails, MSDBcontext _context, IConfiguration _configuration )
        {
            Mails mailsdb = new Mails();
            MailsResult resultdb = new MailsResult();
            try
            {
                MimeMessage emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Администрация сайта", _configuration["SMTP:Mail"]));
                foreach (var address in mails.Recipient.Split(','))
                {
                    emailMessage.To.Add(new MailboxAddress("", address.Trim()));
                }
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
                    resultdb.Result = "OK";
                }
            }
            catch (Exception e)
            {
                resultdb.Result = "Failed";
                resultdb.FailedMessage = e.GetBaseException().Message;
            }

            mailsdb.Body = mails.Body;
            mailsdb.Recipient = mails.Recipient;
            mailsdb.Subject = mails.Subject;

            mailsdb.MailsResult = resultdb;
            resultdb.CreatedDate = DateTime.Now;

            _context.Mails.Add(mailsdb);
            _context.MailsResults.Add(resultdb);

            await _context.SaveChangesAsync();
            var resultmail = new MailsDto()
            {
                Id = mailsdb.Id,
                Recipient = mails.Recipient,
                Subject = mails.Subject,
                Body = mails.Body,
                CreatedDate = mailsdb.MailsResult.CreatedDate,
                FailedMessage = mailsdb.MailsResult.FailedMessage,
                Result = mailsdb.MailsResult.Result,
            };


            return resultmail;
        }
    }
}
