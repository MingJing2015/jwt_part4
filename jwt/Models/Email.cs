


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace jwt.Models
{
    public class Email
    {
        public int ID { get; set; }

        public string FromAddress { get; set; }

        [Required(ErrorMessage = "Email required.")]
        [RegularExpression(@"^(([^<>()[\]\\.,;:\s@\""]+"
                        + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                        + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                        + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                        + @"[a-zA-Z]{2,}))$", ErrorMessage = "Not a valid email address")]
        [Display(Name = "From")]
        public string ToAddress { get; set; }

        [Required(ErrorMessage = "Subject required.")]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message required.")]
        [Display(Name = "Message")]
        public string Message { get; set; }

    }

    public class EmailDBContext : DbContext
    {
        public DbSet<Email> Emails { get; set; }
    }
}
