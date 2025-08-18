using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::JabissStorage.Domain.Interfaces;
using JabissCommon;
using Microsoft.Data.SqlClient;


namespace JabissStorage.DataAccess.Implementations
{
   
    public class MsSqlUserVerificationRepository : IUserVerificationRepository
    {
        private readonly string _connectionString;
        public MsSqlUserVerificationRepository(string connectionString) => _connectionString = connectionString;

        public async Task<int> CreateOrReplaceAsync(UserVerificationModel model)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            // Eyni email üçün aktiv qeydi sil və yenisini yaz
            var deleteSql = @"DELETE FROM UserVerifications WHERE Email=@Email AND IsUsed=0;";
            using (var del = new SqlCommand(deleteSql, conn))
            {
                del.Parameters.AddWithValue("@Email", model.Email);
                await del.ExecuteNonQueryAsync();
            }

            var insertSql = @"
                             INSERT INTO UserVerifications (Email, Name, PasswordHash, Code, ExpiresAt, AttemptCount, IsUsed, CreatedAt)
                             VALUES (@Email, @Name, @PasswordHash, @Code, @ExpiresAt, 0, 0, SYSUTCDATETIME());
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(insertSql, conn);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@PasswordHash", model.PasswordHash);
            cmd.Parameters.AddWithValue("@Code", model.Code);
            cmd.Parameters.AddWithValue("@ExpiresAt", model.ExpiresAt);

            var id = (int)await cmd.ExecuteScalarAsync();
            return id;
        }

        public async Task<UserVerificationModel?> GetActiveByEmailAsync(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = @"SELECT TOP 1 Id, Email, Name, PasswordHash, Code, ExpiresAt, AttemptCount, IsUsed, CreatedAt
                                 FROM UserVerifications
                                 WHERE Email=@Email AND IsUsed=0
                                 ORDER BY Id DESC;";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", email);

            using var r = await cmd.ExecuteReaderAsync();
            if (!await r.ReadAsync()) return null;

            return new UserVerificationModel
            {
                Id = r.GetInt32(0),
                Email = r.GetString(1),
                Name = r.GetString(2),
                PasswordHash = r.GetString(3),
                Code = r.GetString(4),
                ExpiresAt = r.GetDateTime(5),
                AttemptCount = r.GetInt32(6),
                IsUsed = r.GetBoolean(7),
                CreatedAt = r.GetDateTime(8)
            };
        }

        public async Task<bool> MarkUsedAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var sql = "UPDATE UserVerifications SET IsUsed=1 WHERE Id=@Id;";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task IncrementAttemptAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var sql = "UPDATE UserVerifications SET AttemptCount = AttemptCount + 1 WHERE Id=@Id;";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteExpiredAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var sql = "DELETE FROM UserVerifications WHERE IsUsed=0 AND ExpiresAt < SYSUTCDATETIME();";
            using var cmd = new SqlCommand(sql, conn);
            return await cmd.ExecuteNonQueryAsync();
        }
    }

}
