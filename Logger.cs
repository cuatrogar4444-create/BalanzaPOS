using BalanzaPOSNuevo;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace BalanzaPOSNuevo
{
    public static class Logger
    {
        private static readonly object logLock = new object();
        public static void Log(string level, string message)
        {
            lock (logLock)
            {
                try
                {
                    File.AppendAllText("debug.log", $"{DateTime.Now} [{level}] {message}\n");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al escribir en el log: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}