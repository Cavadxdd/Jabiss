using JabissStorage.Domain.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JabissStorage.Domain.Entities
{
    public class Category : IEntity
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        
    }
}
