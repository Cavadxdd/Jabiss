using JabissStorage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jabiss.Business.Services.Models
{
    public class ProductServiceModel
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public required decimal Price { get; set; }

        public required int Stock { get; set; }

        public required int CategoryId { get; set; }

        public List<ProductImageServiceModel> Images { get; set; } = new List<ProductImageServiceModel>();

    }
}
