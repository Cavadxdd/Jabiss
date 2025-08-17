using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jabiss.Business.Services.Models
{
    public class ProductImageServiceModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public byte[] ImageData { get; set; }
    }
}
