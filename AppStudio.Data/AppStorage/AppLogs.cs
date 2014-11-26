using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AppStudio
{
    public class AppLogs
    {
        static public async void WriteError(string source, Exception exception)
        {
            await WriteLog(source, exception.ToString(), "Error");
        }

        static public async void WriteError(string source, string message)
        {
            await WriteLog(source, message, "Error");
        }

        static public async void WriteWarning(string source, string message)
        {
            await WriteLog(source, message, "Warning");
        }

        static public async void WriteInfo(string source, string message)
        {
            await WriteLog(source, message, "Info");
        }

        static public async Task WriteLog(string source, string message, string messageType)
        {
            try
            {
                message = CleanLogMessage(message);
                string logMessage = String.Format("{0}\t{1}\t{2}\t{3}: {4}\r\n", DateTime.Now.Ticks, DateTime.Now.ToString("yy/MM/dd HH:mm:ss"), messageType, source, message);
                await UserStorage.AppendLineToFile("AppLogs.txt", logMessage);
            }
            catch { /* Avoid any exception at this point. */ }
        }

        private static string CleanLogMessage(string message)
        {
            return message.Replace("\r", string.Empty)
                          .Replace("\n", string.Empty)
                          .Replace("\t", string.Empty);
        }

        static public async Task Clear()
        {
            try
            {
                await UserStorage.DeleteFileIfExists("AppLogs.txt");
            }
            catch { /* Avoid any exception at this point. */ }
        }
    }
}
