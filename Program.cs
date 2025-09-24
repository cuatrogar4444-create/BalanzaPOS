using System;
using System.IO; // Agregado para File
using System.Windows.Forms;

namespace BalanzaPOSNuevo
{
    static class Program
    {
        private static LoginScreen loginScreen;
        private static MainScreen mainScreen;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ShowLoginScreen();
            Application.Run();

            File.AppendAllText("debug.log", $"[{DateTime.Now}] Iniciando aplicación\n");
            while (true)
            {
                using (LoginScreen loginScreen = new LoginScreen())
                {
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] Mostrando LoginScreen\n");
                    DialogResult result = loginScreen.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        File.AppendAllText("debug.log", $"[{DateTime.Now}] Login exitoso, abriendo MainScreen para UserId={Session.UserId}, IsAdmin={Session.IsAdmin}\n");
                        try
                        {
                           // Application.Run(new MainScreen(Session.IsAdmin, Session.UserId));
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al abrir MainScreen: {ex.Message}\n");
                            MessageBox.Show($"Error al abrir la ventana principal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue; // Volver a mostrar LoginScreen
                        }
                    }
                    else
                    {
                        File.AppendAllText("debug.log", $"[{DateTime.Now}] Login cancelado, saliendo\n");
                        break;
                    }
                }
            }

        }
        public static void ShowLoginScreen()
        {
            if (mainScreen != null && !mainScreen.IsDisposed)
            {
                mainScreen.Close(); // Cierra MainScreen si está abierto
            }

            loginScreen = new LoginScreen();
            loginScreen.FormClosed += (s, e) =>
            {
                if (loginScreen.DialogResult == DialogResult.OK)
                {
                    mainScreen = new MainScreen();
                    mainScreen.FormClosed += (s2, e2) => Application.Exit();
                    mainScreen.Show();
                }
                else
                {
                    Application.Exit(); // Cierra la aplicación si el login falla o se cancela
                }
            };
            loginScreen.Show();
        }
    }
}