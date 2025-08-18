using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jabiss.Business.Services.Models
{
    public class EmailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string User { get; set; }
        public string Password { get; set; }
        public string FromAddress { get; set; }  // no-reply@jabiss.com
        public string FromName { get; set; }     // Jabiss
    }

}
