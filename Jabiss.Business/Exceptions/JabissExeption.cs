using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jabiss.Business.Exceptions
{
    public class JabissExeption : Exception
    {
        public JabissExeption(string message) : base(message)
        { 
        }
    }
}
