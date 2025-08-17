using JabissStorage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JabissStorage.Domain.Interfaces
{
    public interface ICartItemRepository
    {
        Task<IEnumerable<CartItem>> GetByCartIdAsync(int cartId);
        Task<int> AddAsync(CartItem item); // Entity üzerinden alıyoruz
        Task DeleteAsync(int cartItemId);
        Task UpdateQuantityAsync(int cartItemId, int quantity);
        Task<int> GetCartItemCountAsync(int cartId);
        Task<CartItem> GetByCartIdAndProductIdAsync(int cartId, int productId);
    }
    
}
