using System.Net.Mail;
using System.Net;

namespace RailConnect.Models
{
    public class EmailService
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

}
