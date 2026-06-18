using System;
using System.IO;
using System.Windows.Forms;

namespace MqlGenerator
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            try
            {
                Log("Starting");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.ThreadException += delegate(object sender, System.Threading.ThreadExceptionEventArgs e)
                {
                    ShowError(e.Exception);
                };
                AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e)
                {
                    ShowError(e.ExceptionObject as Exception);
                };
                Application.Run(new MainForm());
                Log("Exited");
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private static string LogPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MQLGenerator.log");
        }

        internal static void Log(string message)
        {
            File.AppendAllText(
                LogPath(),
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + message + Environment.NewLine);
        }

        private static void ShowError(Exception ex)
        {
            string message = ex == null ? "Unknown error" : ex.ToString();
            try
            {
                Log("ERROR " + message);
            }
            catch
            {
            }

            MessageBox.Show(
                message + Environment.NewLine + Environment.NewLine + "Log: " + LogPath(),
                "MQL Generator Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
