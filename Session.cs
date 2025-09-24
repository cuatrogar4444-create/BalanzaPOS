using System;
using System.IO;

namespace BalanzaPOSNuevo
{
    public static class Session
    {
        public static int UserId { get; set; }
        public static string Username { get; set; }
        public static bool IsAdmin { get; set; }

        public static void Logout()
        {
            UserId = 0;
            Username = null;
            IsAdmin = false;
            File.AppendAllText("debug.log", $"[{DateTime.Now}] Sesión cerrada\n");
        }
    }
}