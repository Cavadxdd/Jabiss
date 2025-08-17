using JabissStorage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JabissStorage.Domain.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetActiveCartByUserIdAsync(int userId);
        Task<int> CreateCartAsync(int userId);
        Task DeleteCartAsync(int cartId);
        Task UpdateCartUpdatedAtAsync(int cartId, DateTime updatedAt);

    }
}
