using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MailSenderService.Data.Models
{
    /// <summary>
    /// Это сущность Mails в БД
    /// </summary>
    /// <param name="Id">Id сущности</param>
    /// <param name="Subject">Тема сообщения</param>
    /// <param name="Body">Тело сообщения</param>
    /// <param name="Recipient">Кому будет отправленно сообщение</param>
    /// <param name="MailsResult">Навигационное свойство к сущности MailsResult</param>
    public class Mails
    {
        [Key]
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Recipient { get; set; }

        public MailsResult MailsResult { get; set; }
    }
}
