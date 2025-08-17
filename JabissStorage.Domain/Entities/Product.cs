using JabissStorage.Domain.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JabissStorage.Domain.Entities
{
    public class Product : IEntity
    {
        public  int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required decimal Price { get; set; }
        public required int Stock { get; set; }
        public required int CategoryId { get; set; }
        public ICollection<ProductImage> ImageDatas { get; set; } = new List<ProductImage>();

    }
}
