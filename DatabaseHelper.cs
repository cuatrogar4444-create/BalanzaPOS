using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace BalanzaPOSNuevo
{
    public static class DatabaseHelper
    {
        public static SQLiteConnection GetConnection()
        {
            try
            {
                var connection = new SQLiteConnection("Data Source=BalanzaPOS.db;Version=3;");
                Logger.Log("Info", $"Intentando conectar a la base de datos: BalanzaPOS.db");
                return connection;
            }
            catch (Exception ex)
            {
                Logger.Log("Error", $"Error al conectar a la base de datos: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public static void InitializeDatabase()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    Logger.Log("Info", $"Conexión abierta a la base de datos: {conn.Database}");
                    using (var walCmd = new SQLiteCommand("PRAGMA journal_mode=WAL;", conn))
                    {
                        walCmd.ExecuteNonQuery();
                        Logger.Log("Info", "Modo WAL activado");
                    }

                    string createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT,
                    PasswordHash TEXT,
                    IsAdmin INTEGER NOT NULL DEFAULT 0,
                    Active INTEGER NOT NULL DEFAULT 1,
                    Expires INTEGER NOT NULL DEFAULT 0,
                    ExpiryDate DATETIME
                )";
                    using (var cmd = new SQLiteCommand(createUsersTable, conn))
                    {
                        cmd.ExecuteNonQuery();
                        Logger.Log("Info", "Tabla Users creada o verificada");
                    }

                    string checkUsersQuery = "SELECT COUNT(*) FROM Users";
                    using (var checkCmd = new SQLiteCommand(checkUsersQuery, conn))
                    {
                        long userCount = (long)checkCmd.ExecuteScalar();
                        Logger.Log("Info", $"Usuarios encontrados: {userCount}");
                        if (userCount == 0)
                        {
                            string insertUserQuery = @"
                        INSERT INTO Users (Username, Password, IsAdmin, Active, Expires)
                        VALUES (@Username, @Password, @IsAdmin, @Active, @Expires)";
                            using (var insertCmd = new SQLiteCommand(insertUserQuery, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@Username", "admin");
                                insertCmd.Parameters.AddWithValue("@Password", "admin");
                                insertCmd.Parameters.AddWithValue("@IsAdmin", 1);
                                insertCmd.Parameters.AddWithValue("@Active", 1);
                                insertCmd.Parameters.AddWithValue("@Expires", 0);
                                insertCmd.ExecuteNonQuery();
                                Logger.Log("Info", "Usuario admin creado");
                            }
                        }
                    }

                    string createProductsTable = @"
                CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Code TEXT NOT NULL UNIQUE,
                    Name TEXT NOT NULL,
                    PricePerUnit DECIMAL NOT NULL,
                    Unit TEXT NOT NULL,
                    Stock DECIMAL NOT NULL,
                    MinimumStock DECIMAL NOT NULL,
                    Active INTEGER NOT NULL DEFAULT 1,
                    IsQuickProduct BOOLEAN DEFAULT 0
                )";
                    using (var cmd = new SQLiteCommand(createProductsTable, conn))
                    {
                        cmd.ExecuteNonQuery();
                        Logger.Log("Info", "Tabla Products creada o verificada");
                    }

                    string createSalesTable = @"
                CREATE TABLE IF NOT EXISTS Sales (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SaleDate DATETIME NOT NULL,
                    Total DECIMAL NOT NULL,
                    Discount DECIMAL NOT NULL,
                    PaymentMethod TEXT NOT NULL,
                    Username TEXT NOT NULL,
                    CashRegisterId INTEGER NOT NULL
                )";
                    using (var cmd = new SQLiteCommand(createSalesTable, conn))
                    {
                        cmd.ExecuteNonQuery();
                        Logger.Log("Info", "Tabla Sales creada o verificada");
                    }

                    string createSaleItemsTable = @"
                CREATE TABLE IF NOT EXISTS SaleItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SaleId INTEGER NOT NULL,
                    ProductId INTEGER NOT NULL,
                    Quantity DECIMAL NOT NULL,
                    UnitPrice DECIMAL NOT NULL,
                    Subtotal DECIMAL NOT NULL,
                    Unit TEXT NOT NULL,
                    FOREIGN KEY (SaleId) REFERENCES Sales(Id),
                    FOREIGN KEY (ProductId) REFERENCES Products(Id)
                )";
                    using (var cmd = new SQLiteCommand(createSaleItemsTable, conn))
                    {
                        cmd.ExecuteNonQuery();
                        Logger.Log("Info", "Tabla SaleItems creada o verificada");
                    }

                    string createStockHistoryTable = @"
                CREATE TABLE IF NOT EXISTS StockHistory (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProductId INTEGER NOT NULL,
                    Quantity DECIMAL NOT NULL,
                    Reason TEXT NOT NULL,
                    Username TEXT NOT NULL,
                    Date DATETIME NOT NULL,
                    FOREIGN KEY (ProductId) REFERENCES Products(Id)
                )";
                    using (var cmd = new SQLiteCommand(createStockHistoryTable, conn))
                    {
                        cmd.ExecuteNonQuery();
                        Logger.Log("Info", "Tabla StockHistory creada o verificada");
                    }

                    string createSettingsTable = @"
                CREATE TABLE IF NOT EXISTS Settings (
                    Id INTEGER PRIMARY KEY,
                    WeightDecimals INTEGER DEFAULT 2,
                    CurrencyDecimals INTEGER DEFAULT 2,
                    CurrencySymbol TEXT DEFAULT 'S/.',
                    BaudRate INTEGER DEFAULT 9600,
                    SerialPort TEXT DEFAULT 'COM1',
                    Parity TEXT DEFAULT 'None',
                    DataBits INTEGER DEFAULT 8,
                    StopBits TEXT DEFAULT 'One'
                );
                INSERT OR IGNORE INTO Settings (Id, WeightDecimals, CurrencyDecimals, CurrencySymbol, BaudRate, SerialPort, Parity, DataBits, StopBits)
                VALUES (1, 2, 2, 'S/.', 9600, 'COM1', 'None', 8, 'One');";
                    using (var cmd = new SQLiteCommand(createSettingsTable, conn))
                    {
                        cmd.ExecuteNonQuery();
                        Logger.Log("Info", "Tabla Settings creada o verificada");
                    }

                    Logger.Log("Info", "Base de datos inicializada correctamente");
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error", $"Error al inicializar la base de datos: {ex.Message}");
                throw;
            }
        }

        public static long SaveSaleToDatabase(DataTable saleItems, decimal total, decimal discount, string paymentMethod, string username, int cashRegisterId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string[] requiredColumns = { "IdProducto", "Código", "Nombre", "Cantidad", "Unidad", "PrecioUnitario", "Subtotal" };
                            var missingColumns = requiredColumns.Where(col => !saleItems.Columns.Contains(col)).ToList();
                            if (missingColumns.Any())
                            {
                                Logger.Log("Error", $"Faltan columnas: {string.Join(", ", missingColumns)}");
                                throw new ArgumentException($"Falta la columna '{string.Join(", ", missingColumns)}' en saleItemsDataTable");
                            }

                            foreach (DataRow row in saleItems.Rows)
                            {
                                if (row["IdProducto"] == DBNull.Value || Convert.ToInt64(row["IdProducto"]) <= 0)
                                {
                                    Logger.Log("Error", $"ID de producto inválido para {row["Nombre"]}");
                                    throw new ArgumentException("ID de producto inválido.");
                                }
                                if (row["Cantidad"] == DBNull.Value || Convert.ToDecimal(row["Cantidad"]) <= 0)
                                {
                                    Logger.Log("Error", $"Cantidad inválida para {row["Nombre"]}");
                                    throw new ArgumentException("Cantidad inválida.");
                                }
                            }

                            long saleId;
                            using (var cmd = new SQLiteCommand(
                                "INSERT INTO Sales (SaleDate, Total, Discount, PaymentMethod, Username, CashRegisterId) " +
                                "VALUES (@SaleDate, @Total, @Discount, @PaymentMethod, @Username, @CashRegisterId); " +
                                "SELECT last_insert_rowid();", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@SaleDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                cmd.Parameters.AddWithValue("@Total", total);
                                cmd.Parameters.AddWithValue("@Discount", discount);
                                cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                                cmd.Parameters.AddWithValue("@Username", username);
                                cmd.Parameters.AddWithValue("@CashRegisterId", cashRegisterId);
                                saleId = (long)cmd.ExecuteScalar();
                            }

                            foreach (DataRow row in saleItems.Rows)
                            {
                                using (var cmd = new SQLiteCommand(
                                    "INSERT INTO SaleItems (SaleId, ProductId, Quantity, Unit, UnitPrice, Subtotal) " +
                                    "VALUES (@SaleId, @ProductId, @Quantity, @Unit, @UnitPrice, @Subtotal)", conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@SaleId", saleId);
                                    cmd.Parameters.AddWithValue("@ProductId", Convert.ToInt64(row["IdProducto"]));
                                    cmd.Parameters.AddWithValue("@Quantity", Convert.ToDecimal(row["Cantidad"]));
                                    cmd.Parameters.AddWithValue("@Unit", row["Unidad"].ToString());
                                    cmd.Parameters.AddWithValue("@UnitPrice", Convert.ToDecimal(row["PrecioUnitario"]));
                                    cmd.Parameters.AddWithValue("@Subtotal", Convert.ToDecimal(row["Subtotal"]));
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            foreach (DataRow row in saleItems.Rows)
                            {
                                using (var cmd = new SQLiteCommand(
                                    "UPDATE Products SET Stock = Stock - @Quantity WHERE Id = @ProductId AND Active = 1", conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@Quantity", Convert.ToDecimal(row["Cantidad"]));
                                    cmd.Parameters.AddWithValue("@ProductId", Convert.ToInt64(row["IdProducto"]));
                                    int rowsAffected = cmd.ExecuteNonQuery();
                                    if (rowsAffected == 0)
                                    {
                                        Logger.Log("Error", $"Producto con ID {row["IdProducto"]} no encontrado o no activo");
                                    }
                                }
                            }

                            transaction.Commit();
                            Logger.Log("Info", $"Venta guardada: ID Venta: {saleId}, Ítems: {saleItems.Rows.Count}, Total: {total}");
                            return saleId;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Logger.Log("Error", $"Error en SaveSaleToDatabase: {ex.Message}\nStackTrace: {ex.StackTrace}");
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error", $"Error en SaveSaleToDatabase: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public static decimal ObtenerStockActual(long productId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT Stock FROM Products WHERE Id = @ProductId";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToDecimal(result) : 0;
                }
            }
        }

        public static decimal ObtenerStockMinimo(long productId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT MinimumStock FROM Products WHERE Id = @ProductId";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToDecimal(result) : 0;
                }
            }
        }

        public static long GetProductIdFromCode(string code)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT Id FROM Products WHERE Code = @Code AND Active = 1";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Code", code);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt64(result) : -1;
                }
            }
        }

        public static string GetProductNameFromCode(string code)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT Name FROM Products WHERE Code = @Code AND Active = 1";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Code", code);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "";
                }
            }
        }

        public static decimal GetProductPriceFromCode(string code)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT PricePerUnit FROM Products WHERE Code = @Code AND Active = 1";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Code", code);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToDecimal(result) : 0;
                }
            }
        }

        public static string GetProductUnitFromCode(string code)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT Unit FROM Products WHERE Code = @Code AND Active = 1";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Code", code);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "unidad";
                }
            }
        }

        public static decimal GetProductStockFromCode(string code)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT Stock FROM Products WHERE Code = @Code AND Active = 1";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Code", code);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToDecimal(result) : 0;
                }
            }
        }

        public static long GetLastSaleId()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT last_insert_rowid()";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    return (long)cmd.ExecuteScalar();
                }
            }
        }

        public static long GetProductIdFromName(string name)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT Id FROM Products WHERE Name = @Name AND Active = 1";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt64(result) : -1;
                }
            }
        }

        public static DataTable ObtenerTodosLosProductos()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT Id, Code, Name, Unit, PricePerUnit AS Price, Stock, MinimumStock, Active FROM Products WHERE Active = 1";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static void UpdateStockWithHistory(long productId, decimal quantity, string reason, string username, SQLiteConnection conn, SQLiteTransaction transaction)
        {
            string updateStockQuery = "UPDATE Products SET Stock = Stock + @Quantity WHERE Id = @ProductId";
            using (var cmd = new SQLiteCommand(updateStockQuery, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.ExecuteNonQuery();
            }

            string insertHistoryQuery = "INSERT INTO StockHistory (ProductId, Quantity, Reason, Username, Date) VALUES (@ProductId, @Quantity, @Reason, @Username, @Date)";
            using (var cmd = new SQLiteCommand(insertHistoryQuery, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@Reason", reason);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }
    }
}