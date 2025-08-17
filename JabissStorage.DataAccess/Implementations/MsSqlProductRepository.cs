using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JabissStorage.DataAccess.Implementations
{
    using System.Data;
    using Microsoft.Data.SqlClient;
    using global::JabissStorage.Domain.Entities;
    using global::JabissStorage.Domain.Interfaces;
    using Microsoft.Extensions.Configuration;
    using System.Data.Common;

    namespace JabissStorage.DataAccess.Repositories
    {
        public class MsSqlProductRepository : IProductRepository
        {
            private readonly string _connectionString;
            private readonly IProductImageRepository _imagesRepository;

            public MsSqlProductRepository(IConfiguration configuration)
            {
                _connectionString = configuration.GetConnectionString("Default");
                _imagesRepository = new MsSqlProductImageRepository(configuration);
            }

            public async Task<IEnumerable<Product>> GetAllAsync()
            {
                var products = new List<Product>();
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("dbo.sp_GetAllProducts", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var product = new Product
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                        Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                        CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId"))
                    };

                    product.ImageDatas = (await _imagesRepository.GetImagesForProductAsync(product.Id)).ToList();
                    products.Add(product);
                }

                return products;
            }

            public async Task<Product?> GetByIdAsync(int id)
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("dbo.sp_GetProductById", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Id", id);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var product = new Product
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                        Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                        CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId"))
                    };

                    product.ImageDatas = (await _imagesRepository.GetImagesForProductAsync(product.Id)).ToList();
                    return product;
                }

                return null;
            }

            public async Task<int> AddAsync(Product product)
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("dbo.sp_InsertProduct", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Name", product.Name);
                cmd.Parameters.AddWithValue("@Description", product.Description);
                cmd.Parameters.AddWithValue("@Price", product.Price);
                cmd.Parameters.AddWithValue("@Stock", product.Stock);
                cmd.Parameters.AddWithValue("@CategoryId", product.CategoryId);

                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                int newId = result != null ? Convert.ToInt32(result) : 0;

                foreach (var image in product.ImageDatas)
                    await _imagesRepository.AddImageAsync(newId, image.ImageData);

                return newId;
            }

            public async Task<bool> UpdateAsync(Product product)
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                using var tran = conn.BeginTransaction();

                try
                {
                    using (var cmd = new SqlCommand("dbo.sp_UpdateProduct", conn, tran))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", product.Id);
                        cmd.Parameters.AddWithValue("@Name", product.Name);
                        cmd.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Price", product.Price);
                        cmd.Parameters.AddWithValue("@Stock", product.Stock);
                        cmd.Parameters.AddWithValue("@CategoryId", product.CategoryId);

                        await cmd.ExecuteNonQueryAsync();
                    }

                    using (var deleteCmd = new SqlCommand("DELETE FROM ProductImages WHERE ProductId = @ProductId", conn, tran))
                    {
                        deleteCmd.Parameters.AddWithValue("@ProductId", product.Id);
                        await deleteCmd.ExecuteNonQueryAsync();
                    }

                    foreach (var img in product.ImageDatas)
                    {
                        using (var insertCmd = new SqlCommand(
                            "INSERT INTO ProductImages (ProductId, ImageData) VALUES (@ProductId, @ImageData)",
                            conn, tran))
                        {
                            insertCmd.Parameters.AddWithValue("@ProductId", product.Id);
                            insertCmd.Parameters.Add("@ImageData", SqlDbType.VarBinary).Value =
                                img.ImageData ?? (object)DBNull.Value;

                            await insertCmd.ExecuteNonQueryAsync();
                        }
                    }

                    tran.Commit();
                    return true;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }



            public async Task<bool> DeleteAsync(int id)
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("dbo.sp_DeleteProduct", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", id);

                await conn.OpenAsync();
                int affectedRows = await cmd.ExecuteNonQueryAsync();

                return affectedRows > 0;
            }

            public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
            {
                var products = new List<Product>();

                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("dbo.sp_GetProductsByCategory", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@CategoryId", categoryId);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var product = new Product
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                        Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                        CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId"))
                    };

                    product.ImageDatas = (await _imagesRepository.GetImagesForProductAsync(product.Id)).ToList();
                    products.Add(product);
                }

                return products;
            }


        }

    }
}
