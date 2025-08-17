using Jabiss.Business.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Jabiss.Business.Services
{
    public class SecurityService:ISecurityService
    {
        public string Hash(string value)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                var valueBytes = Encoding.UTF8.GetBytes(value);

                var bytes = sha256Hash.ComputeHash(valueBytes);

                var builder = new StringBuilder();

                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
