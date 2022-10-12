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

        public async Task<MailsDto> MassageSender(Mails mails, MSDBcontext _context, IConfiguration _configuration )
        {
            MailsResult result = new MailsResult();
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


            return resultmail;
        }
    }
}
