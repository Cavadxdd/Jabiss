using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JabissStorage.Domain.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }

        public byte[] ImageData { get; set; } = null!;

        public int ProductId { get; set; }
    }
}
