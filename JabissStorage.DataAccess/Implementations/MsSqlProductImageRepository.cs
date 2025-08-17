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
using static System.Net.Mime.MediaTypeNames;

namespace JabissStorage.DataAccess.Implementations
{
    public class MsSqlProductImageRepository:IProductImageRepository
    {
        private readonly string _connectionString;

        public MsSqlProductImageRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
        }

        public async Task<IEnumerable<ProductImage>> GetImagesForProductAsync(int productId)
        {
            var images = new List<ProductImage>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.sp_GetProductImages", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@ProductId", productId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var img = new ProductImage
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                    ImageData = reader.IsDBNull(reader.GetOrdinal("ImageData"))
                        ? Array.Empty<byte>()
                        : (byte[])reader.GetValue(reader.GetOrdinal("ImageData"))
                };
                images.Add(img);
            }

            return images;
        }

        public async Task<int> AddImageAsync(int productId, byte[] imageData)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.sp_InsertProductImage", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductId", productId);
            cmd.Parameters.AddWithValue("@ImageData", imageData);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<int> DeleteImagesForProductAsync(int productId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.sp_DeleteProductImagesByProductId", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductId", productId);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync(); 
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<bool> DeleteImageAsync(int imageId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.sp_DeleteProductImage", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", imageId);

            await conn.OpenAsync();
            int affected = await cmd.ExecuteNonQueryAsync();
            return affected > 0;
        }

    }
}
