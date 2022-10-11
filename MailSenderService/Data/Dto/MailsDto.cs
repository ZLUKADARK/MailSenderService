using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailSenderService.Data.Dto
{
    public class MailsDto
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Recipient { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Result { get; set; }
        public string FailedMessage { get; set; }
    }
}
