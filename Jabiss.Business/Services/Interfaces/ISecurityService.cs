using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jabiss.Business.Services.Interfaces
{
    public interface ISecurityService
    {
        string Hash(string value);
    }
}
