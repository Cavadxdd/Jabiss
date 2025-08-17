using JabissStorage.Domain.Entities;
using JabissStorage.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JabissStorage.DataAccess.Implementations
{
    public class MsSqlCartRepository : ICartRepository
    {
        private readonly string _connectionString;

        public MsSqlCartRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
        }

        public async Task<Cart?> GetActiveCartByUserIdAsync(int userId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.sp_GetActiveCartByUserId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserId", userId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Cart
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                    IsCheckedOut = reader.GetBoolean(reader.GetOrdinal("IsCheckedOut")),
                };
            }

            return null;
        }

        public async Task<int> CreateCartAsync(int userId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.sp_CreateCart", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserId", userId);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task DeleteCartAsync(int cartId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.sp_DeleteCart", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@CartId", cartId);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        //Db deki UpdatedAt islesin deyedir:
        public async Task UpdateCartUpdatedAtAsync(int cartId, DateTime updatedAt)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("UPDATE Carts SET UpdatedAt = @UpdatedAt WHERE Id = @CartId", conn);
            cmd.Parameters.AddWithValue("@UpdatedAt", updatedAt);
            cmd.Parameters.AddWithValue("@CartId", cartId);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }


    }
}
