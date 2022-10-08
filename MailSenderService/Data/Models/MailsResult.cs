using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MailSenderService.Data.Models
{
    /// <summary>
    /// Это сущность MailsResult в БД
    /// </summary>
    /// <param name="Id">Id сущности</param>
    /// <param name="CreatedDate">Дата создания</param>
    /// <param name="Result">Результат выполнения "Ok/Failed"</param>
    /// <param name="FailedMessage">Причина по которой не доставлено письмо, или иная ошибка</param>
    /// <param name="MailsId">Foreign key сущности Mails</param>
    /// <param name="Mails">Навигационное свойство к сущности Mails</param>
    public class MailsResult
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Result { get; set; }
        public string FailedMessage { get; set; }

        [ForeignKey("MailsId")]
        public int MailsId { get; set; } 
        public Mails Mails { get; set; }
    }
}
