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
    public class MsSqlCategoryRepository:ICategoryRepository
    {

        private readonly string _connectionString;

        public MsSqlCategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {

            var categories = new List<Category>();
            using var conn = new SqlConnection(_connectionString); // 💡 connection string burada
            using SqlCommand cmd = new("dbo.sp_GetAllCategories", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();


            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                categories.Add(new Category
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                });
            }

            return categories;
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("dbo.sp_GetCategoryById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Category
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name"))
                };
            }

            return null;
        }

        public async Task<bool> AddAsync(Category category)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.sp_InsertCategory", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Name", category.Name);

            await conn.OpenAsync();
            int affectedRows = await cmd.ExecuteNonQueryAsync();

            return affectedRows > 0;
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.sp_UpdateCategory", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", category.Id);
            cmd.Parameters.AddWithValue("@Name", category.Name);

            await conn.OpenAsync();
            int affectedRows = await cmd.ExecuteNonQueryAsync();

            return affectedRows > 0;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.sp_DeleteCategory", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            int affectedRows = await cmd.ExecuteNonQueryAsync();

            return affectedRows > 0;
        }



    }
}
