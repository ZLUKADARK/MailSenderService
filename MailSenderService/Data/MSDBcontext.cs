using MailSenderService.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailSenderService.Data
{
    public class MSDBcontext : DbContext
    {
        public MSDBcontext(DbContextOptions<MSDBcontext> options) 
            : base(options)
        {

        }

        public DbSet<Mails> Mails { get; set; }
        public DbSet<MailsResult> MailsResults { get; set; }
    }
}
