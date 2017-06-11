

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net.Mime;

using jwt.Models;

namespace jwt.Repositories
{
    public class EmailRepo
    {
        public static MailMessage mailMsg;

        public EmailRepo()
        {
            // Keep just assign one time
            if (mailMsg != null)
                return;

            mailMsg = new MailMessage();
        }

        public static bool SendEmail(Email email)
        {
            try
            {
                MailMessage mailMsg = new MailMessage();

                // To
                //mailMsg.To.Add(new MailAddress("emailaddress@home.com", "To Name"));
                //mailMsg.To.Add(new MailAddress("jingming20120519@gmail.com", "To Name"));
                //mailMsg.To.Add(new MailAddress("mjing3@my.bcit.ca", "To Name"));
                //mailMsg.To.Add(new MailAddress("15966057988@139.com", "To Name"));
                mailMsg.To.Add(new MailAddress(email.ToAddress, "To Name"));

                // From
                // mailMsg.From = new MailAddress("emailaddress@home.com", "From Name");
                //mailMsg.From = new MailAddress("jingming20120519@gmail.com", "From Name");
                mailMsg.From = new MailAddress("mjing3@my.bcit.ca", "Ming SendGrid2017");


                // Subject and multipart/alternative Body
                //mailMsg.Subject = "subject: Send Grid Test" + " " + DateTime.Now;
                //string text = "text body Jm test ！";
                //string html = @"<p>html body Ming Jing </p>";

                //mailMsg.Subject = email.Subject + " " + DateTime.Now;
                mailMsg.Subject = email.Subject;
                string text = email.Message;
                //string html = @"<h3>" + email.ToAddress + "</h3><br />" + "<p>Thank you for sending the message. We will get back to you by the next business day.</p>";
                string html = email.Message;

                mailMsg.AlternateViews.Add(
                        AlternateView.CreateAlternateViewFromString(text,
                        null, MediaTypeNames.Text.Plain));
                mailMsg.AlternateViews.Add(
                        AlternateView.CreateAlternateViewFromString(html,
                        null, MediaTypeNames.Text.Html));

                // Init SmtpClient and send
                SmtpClient smtpClient
                = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                System.Net.NetworkCredential credentials
                = new System.Net.NetworkCredential("mingjing",
                                                   "SendGrid2017");
                smtpClient.Credentials = credentials;
                smtpClient.Send(mailMsg);
            }
            catch (Exception ex)
            {
                return false;

            }
            return true;
        }
    }
}