using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MailSenderService.Data.Models
{
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
