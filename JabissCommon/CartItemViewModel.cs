using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JabissCommon
{
    public class CartItemViewModel
    {
        public int Id { get; set; }            // CartItem DB Id
        public int CartId { get; set; }        // Cart DB Id
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageBase64 { get; set; }
        public int Stock { get; set; }

    }

}
