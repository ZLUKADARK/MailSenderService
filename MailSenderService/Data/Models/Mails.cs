using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MailSenderService.Data.Models
{
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
