using JabissCommon;
using JabissStorage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jabiss.Business.Services.Interfaces
{
    public interface ICartService
    {
        Task<List<CartItemViewModel>> GetCartItemsAsync(int userId);
        Task<int> AddToCartAsync(int userId, int productId, int quantity);
        Task RemoveFromCartAsync(int cartItemId);
        Task ClearCartAsync(int userId);
        Task UpdateQuantityAsync(int cartItemId, int quantity);
        Task<CartItem> GetCartItemByUserAndProductAsync(int userId, int productId);
    }
}
