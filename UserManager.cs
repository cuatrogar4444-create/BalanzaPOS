using System.IO;
using BCrypt.Net;
using System;
using System.Data.SQLite;

namespace BalanzaPOSNuevo
{
    public static class UserManager
    {
        public static void CreateUser(string username, string password, bool isActive, string expiryDate)
        {
            try
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
                using (SQLiteConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO Users (Username, PasswordHash, IsActive, ExpiryDate) VALUES (@Username, @PasswordHash, @IsActive, @ExpiryDate)";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                        cmd.Parameters.AddWithValue("@IsActive", isActive ? 1 : 0);
                        cmd.Parameters.AddWithValue("@ExpiryDate", expiryDate ?? "2026-12-31");
                        cmd.ExecuteNonQuery();
                    }
                }
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Usuario creado: {username}\n");
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al crear usuario: {ex.Message}\n");
            }
        }

        public static bool VerifyPassword(string username, string password)
        {
            try
            {
                using (SQLiteConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT PasswordHash FROM Users WHERE Username = @Username";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader.GetString(0);
                                return BCrypt.Net.BCrypt.Verify(password, storedHash);
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al verificar contraseña: {ex.Message}\n");
                return false;
            }
        }
    }
}