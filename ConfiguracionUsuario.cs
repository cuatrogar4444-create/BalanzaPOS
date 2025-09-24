using System;
using System.IO;
using System.Data.SQLite;
using System.IO.Ports;
using System.Windows.Forms;

namespace BalanzaPOSNuevo
{
    public static class ConfiguracionUsuario
    {
        public static int WeightDecimals { get; private set; } = 2;
        public static int CurrencyDecimals { get; private set; } = 2;
        public static string CurrencySymbol { get; private set; } = "S/.";
        public static int BaudRate { get; private set; } = 9600;
        public static string SerialPort { get; private set; } = "COM1";
        public static Parity Parity { get; private set; } = Parity.None;
        public static int DataBits { get; private set; } = 8;
        public static StopBits StopBits { get; private set; } = StopBits.One;

        public static void LoadSettings()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT WeightDecimals, CurrencyDecimals, CurrencySymbol, BaudRate, SerialPort, Parity, DataBits, StopBits FROM Settings WHERE Id = 1";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                WeightDecimals = reader.GetInt32(reader.GetOrdinal("WeightDecimals"));
                                CurrencyDecimals = reader.GetInt32(reader.GetOrdinal("CurrencyDecimals"));
                                CurrencySymbol = reader.GetString(reader.GetOrdinal("CurrencySymbol"));
                                BaudRate = reader.GetInt32(reader.GetOrdinal("BaudRate"));
                                SerialPort = reader.GetString(reader.GetOrdinal("SerialPort"));
                                Parity = (Parity)Enum.Parse(typeof(Parity), reader.GetString(reader.GetOrdinal("Parity")));
                                DataBits = reader.GetInt32(reader.GetOrdinal("DataBits"));
                                StopBits = (StopBits)Enum.Parse(typeof(StopBits), reader.GetString(reader.GetOrdinal("StopBits")));
                            }
                            else
                            {
                                WeightDecimals = 2;
                                CurrencyDecimals = 2;
                                CurrencySymbol = "S/.";
                                BaudRate = 9600;
                                SerialPort = "COM1";
                                Parity = Parity.None;
                                DataBits = 8;
                                StopBits = StopBits.One;
                                Logger.Log("Advertencia al cargar configuraciones", "No se encontraron datos en la tabla Settings. Usando valores predeterminados.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error al cargar configuraciones", ex.Message);
                WeightDecimals = 2;
                CurrencyDecimals = 2;
                CurrencySymbol = "S/.";
                BaudRate = 9600;
                SerialPort = "COM1";
                Parity = Parity.None;
                DataBits = 8;
                StopBits = StopBits.One;
            }
        }

        public static void SaveSettings(int weightDecimals, int currencyDecimals, string currencySymbol, int baudRate, string serialPort, Parity parity, int dataBits, StopBits stopBits)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        UPDATE Settings 
                        SET WeightDecimals = @WeightDecimals, 
                            CurrencyDecimals = @CurrencyDecimals, 
                            CurrencySymbol = @CurrencySymbol, 
                            BaudRate = @BaudRate, 
                            SerialPort = @SerialPort, 
                            Parity = @Parity, 
                            DataBits = @DataBits, 
                            StopBits = @StopBits 
                        WHERE Id = 1;
                        INSERT OR IGNORE INTO Settings (Id, WeightDecimals, CurrencyDecimals, CurrencySymbol, BaudRate, SerialPort, Parity, DataBits, StopBits)
                        VALUES (1, @WeightDecimals, @CurrencyDecimals, @CurrencySymbol, @BaudRate, @SerialPort, @Parity, @DataBits, @StopBits)";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@WeightDecimals", weightDecimals);
                        cmd.Parameters.AddWithValue("@CurrencyDecimals", currencyDecimals);
                        cmd.Parameters.AddWithValue("@CurrencySymbol", currencySymbol);
                        cmd.Parameters.AddWithValue("@BaudRate", baudRate);
                        cmd.Parameters.AddWithValue("@SerialPort", serialPort);
                        cmd.Parameters.AddWithValue("@Parity", parity.ToString());
                        cmd.Parameters.AddWithValue("@DataBits", dataBits);
                        cmd.Parameters.AddWithValue("@StopBits", stopBits.ToString());
                        cmd.ExecuteNonQuery();
                    }
                }

                WeightDecimals = weightDecimals;
                CurrencyDecimals = currencyDecimals;
                CurrencySymbol = currencySymbol;
                BaudRate = baudRate;
                SerialPort = serialPort;
                Parity = parity;
                DataBits = dataBits;
                StopBits = stopBits;

                Logger.Log("Configuración guardada", "Configuración actualizada correctamente en la base de datos.");
            }
            catch (Exception ex)
            {
                Logger.Log("Error al guardar configuraciones", ex.Message);
                MessageBox.Show($"Error al guardar configuración: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}