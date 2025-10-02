// EN BalanzaPOSNuevo\BalanzaPOSNuevo\LoginScreen.cs

using System;
using System.Data.SQLite;
using System.Windows.Forms;
using System.IO;
using BalanzaPOSNuevo.Helpers; // Asegúrate de que este using esté presente para PasswordHasher y DatabaseHelper

namespace BalanzaPOSNuevo
{
    public partial class LoginScreen : Form
    {
        public LoginScreen()
        {
            InitializeComponent();
        }

        private void LoginScreen_Load(object sender, EventArgs e)
        {
            try
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] LoginScreen cargado\n");
                txtUsername.Focus();
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al cargar LoginScreen: {ex.Message}\n");
                MessageBox.Show($"Error al cargar LoginScreen: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // ⭐ DECLARAR VARIABLES FUERA DEL TRY PARA QUE SEAN ACCESIBLES EN EL CATCH
            string username = txtUsername.Text.Trim(); // Obtener el valor aquí
            string password = txtPassword.Text;         // Obtener el valor aquí

            try
            {
                // La lógica de SQL ya no usa la columna 'Password' ni la compara directamente.
                // Primero buscamos al usuario para obtener su hash y estado 'Active'.
                // Luego, verificamos la contraseña con PasswordHasher.

                string query = "SELECT Id, IsAdmin, PasswordHash, Active FROM Users WHERE Username = @Username";

                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Usuario encontrado, ahora recuperamos sus datos
                                long userId = reader.GetInt64(reader.GetOrdinal("Id"));
                                bool isAdmin = reader.GetInt32(reader.GetOrdinal("IsAdmin")) == 1; // SQLite almacena bool como INT
                                string storedPasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")); // ⭐ OBTENEMOS EL HASH
                                bool isActive = reader.GetInt32(reader.GetOrdinal("Active")) == 1; // ⭐ OBTENEMOS EL ESTADO ACTIVO

                                // 1. Verificar si el usuario está activo
                                if (!isActive)
                                {
                                    MessageBox.Show("Usuario inactivo. Contacte al administrador.", "Error de Acceso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return; // Salir sin iniciar sesión
                                }

                                // 2. Verificar la contraseña usando el hasher
                                if (PasswordHasher.VerifyPassword(password, storedPasswordHash)) // ⭐ ¡USAMOS EL VERIFICADOR!
                                {
                                    // Credenciales válidas y usuario activo
                                    Session.UserId = (int)userId;
                                    Session.Username = username;
                                    Session.IsAdmin = isAdmin;

                                    MainScreen mainScreen = new MainScreen(userId, isAdmin);
                                    mainScreen.Show();
                                    this.Hide();
                                    return; // Éxito en el inicio de sesión
                                }
                                else
                                {
                                    // Contraseña incorrecta (el hash no coincide)
                                    MessageBox.Show("Usuario o contraseña incorrectos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                // Usuario no encontrado
                                MessageBox.Show("Usuario o contraseña incorrectos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // El log ahora puede usar 'username' y 'password' (aunque no se loguean por seguridad la pass)
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en btnLogin_Click para usuario '{username}': {ex.Message}\nStackTrace: {ex.StackTrace}\n");
                MessageBox.Show($"Error al iniciar sesión: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            File.AppendAllText("debug.log", $"[{DateTime.Now}] Botón Cancelar presionado\n");
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}