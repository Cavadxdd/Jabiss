using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using global::JabissStorage.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using JabissStorage.Domain.Interfaces;

namespace JabissStorage.DataAccess.Implementations
{
    public class MsSqlCartItemRepository : ICartItemRepository
    {
        private readonly string _connectionString;

        public MsSqlCartItemRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
        }

        public async Task<IEnumerable<CartItem>> GetByCartIdAsync(int cartId)
        {
            var items = new List<CartItem>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                                            SELECT ci.Id AS CartItemId, ci.CartId, ci.ProductId, ci.Quantity,
                                                   p.Id AS ProductId, p.Name, p.Price, p.Stock, p.CategoryId,
                                                   pi.ImageData
                                            FROM CartItems ci
                                            INNER JOIN Products p ON ci.ProductId = p.Id
                                            OUTER APPLY (
                                                SELECT TOP 1 ImageData
                                                FROM ProductImages
                                                WHERE ProductId = p.Id
                                                ORDER BY Id
                                            ) pi
                                            WHERE ci.CartId = @CartId
                                        ", conn);

            cmd.Parameters.AddWithValue("@CartId", cartId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var imageDataOrdinal = reader.GetOrdinal("ImageData");
                byte[]? imageData = reader.IsDBNull(imageDataOrdinal)
                    ? null
                    : (byte[])reader["ImageData"];

                var item = new CartItem
                {
                    Id = reader.GetInt32(reader.GetOrdinal("CartItemId")),
                    CartId = reader.GetInt32(reader.GetOrdinal("CartId")),
                    ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    Product = new Product
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                        Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                        CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                        ImageDatas = imageData != null
                            ? new List<ProductImage> { new ProductImage { ImageData = imageData } }
                            : new List<ProductImage>()
                    }
                };

                items.Add(item);
            }

            return items;
        }


        public async Task<int> AddAsync(CartItem item)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.sp_AddCartItem", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@CartId", item.CartId);
            cmd.Parameters.AddWithValue("@ProductId", item.ProductId);
            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task DeleteAsync(int cartItemId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("DELETE FROM CartItems WHERE Id = @CartItemId", conn);

            cmd.Parameters.AddWithValue("@CartItemId", cartItemId);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }


        public async Task UpdateQuantityAsync(int cartItemId, int quantity)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                                             UPDATE CartItems
                                             SET Quantity = @Quantity
                                             WHERE Id = @CartItemId", conn);

            cmd.Parameters.AddWithValue("@CartItemId", cartItemId);
            cmd.Parameters.AddWithValue("@Quantity", quantity);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }


        public async Task<int> GetCartItemCountAsync(int cartId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                                                 SELECT ISNULL(SUM(Quantity), 0)
                                                 FROM CartItems
                                                 WHERE CartId = @CartId ", conn);

            cmd.Parameters.AddWithValue("@CartId", cartId);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<CartItem> GetByCartIdAndProductIdAsync(int cartId, int productId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
                              SELECT Id, CartId, ProductId, Quantity
                              FROM CartItems
                              WHERE CartId = @CartId AND ProductId = @ProductId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CartId", cartId);
                    command.Parameters.AddWithValue("@ProductId", productId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new CartItem
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                CartId = reader.GetInt32(reader.GetOrdinal("CartId")),
                                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}
