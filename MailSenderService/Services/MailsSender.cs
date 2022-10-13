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

        public async Task<MailsDto> MassageSender(MailsBodyDto mails, MSDBcontext _context, IConfiguration _configuration )
        {
            MailsDto mailsdto = new MailsDto();
            
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
                    mailsdto.Result = "OK";
                }
            }
            catch (Exception e)
            {
                mailsdto.Result = "Failed";
                mailsdto.FailedMessage = e.GetBaseException().Message;
            }
            
            mailsdto.Recipient = mails.Recipient;
            mailsdto.Body = mails.Body;
            mailsdto.Subject = mails.Subject;
            mailsdto.CreatedDate = DateTime.Now;


            return await SaveResult(mailsdto, _context); 
        }

        private async Task<MailsDto> SaveResult(MailsDto mailsDto, MSDBcontext _context)
        {
            MailsResult resultdb = new MailsResult();
            Mails mailsdb = new Mails();

            mailsdb.Subject = mailsDto.Subject;
            mailsdb.Body = mailsDto.Body;
            resultdb.CreatedDate = mailsDto.CreatedDate;
            resultdb.FailedMessage = mailsDto.FailedMessage;
            resultdb.Result = mailsDto.Result;
            mailsdb.MailsResult = resultdb;

            foreach (var mail in mailsDto.Recipient.Split(","))
            {
                mailsdb.Id = 0;
                resultdb.Id = 0;

                mailsdb.Recipient = mail.Trim();

                _context.Mails.Add(mailsdb);
                _context.MailsResults.Add(resultdb);
                await _context.SaveChangesAsync();
            }

            var resultmail = new MailsDto()
            {
                Id = resultdb.Id,
                Recipient = mailsDto.Recipient,
                Subject = mailsDto.Subject,
                Body = mailsDto.Body,
                CreatedDate = mailsDto.CreatedDate,
                FailedMessage = mailsDto.FailedMessage,
                Result = mailsDto.Result,
            };
            
            return resultmail;
        }
    }
}
