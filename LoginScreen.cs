using System;
using System.Data.SQLite;
using System.Windows.Forms;
using System.IO; // Agregado para File

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
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;

                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT Id, IsAdmin FROM Users WHERE Username = @Username AND Password = @Password AND Active = 1", conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password); // Considerar usar hash en producción
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                long userId = reader.GetInt64(0);
                                bool isAdmin = reader.GetInt32(1) == 1;
                                Session.UserId = (int)userId;
                                Session.Username = username;
                                Session.IsAdmin = isAdmin;

                                MainScreen mainScreen = new MainScreen(userId, isAdmin);
                                mainScreen.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Usuario o contraseña incorrectos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en btnLogin_Click: {ex.Message}\nStackTrace: {ex.StackTrace}\n");
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