using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;

namespace TenxOCC.Web.Helpers
{
    public static class FileErrorLogger
    {
        private static readonly object _logLock = new object();
        private const string LogFolderName = "EInvoice Errors";

        public static void Log(
            Exception ex,
            string controller,
            string action)
        {
            if (ex == null) return;

            lock (_logLock)
            {
                try
                {
                    string folderPath = GetLogsFolderPath();

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string fileName = $"ErrorLog_{DateTime.Now:yyyyMMdd}.txt";
                    string filePath = Path.Combine(folderPath, fileName);

                    int lineNumber = GetLineNumber(ex);
                    string innerExceptionDetails = ex.InnerException != null 
                        ? ex.InnerException.ToString() 
                        : "None";

                    StringBuilder log = new StringBuilder();

                    log.AppendLine("==================================================");
                    log.AppendLine("E-INVOICE SYSTEM ERROR LOG");
                    log.AppendLine("==================================================");
                    log.AppendLine($"DATE AND TIME      : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    log.AppendLine($"CONTROLLER NAME    : {controller ?? "N/A"}");
                    log.AppendLine($"METHOD/ACTION NAME : {action ?? "N/A"}");
                    log.AppendLine($"ERROR MESSAGE      : {ex.Message}");
                    log.AppendLine($"LINE NUMBER        : {(lineNumber > 0 ? lineNumber.ToString() : "N/A (Line number not available in stack trace)")}");
                    log.AppendLine("--------------------------------------------------");
                    log.AppendLine("EXCEPTION DETAILS :");
                    log.AppendLine(ex.ToString());
                    log.AppendLine("--------------------------------------------------");
                    log.AppendLine("INNER EXCEPTION :");
                    log.AppendLine(innerExceptionDetails);
                    log.AppendLine("--------------------------------------------------");
                    log.AppendLine("STACK TRACE :");
                    log.AppendLine(ex.StackTrace ?? "No stack trace available.");
                    log.AppendLine("==================================================");
                    log.AppendLine();

                    using (var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    using (var writer = new StreamWriter(fs, Encoding.UTF8))
                    {
                        writer.Write(log.ToString());
                        writer.Flush();
                    }
                }
                catch (Exception loggerEx)
                {
                    // Emergency fallback log write to guarantee log creation even if primary path fails
                    try
                    {
                        string fallbackFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFolderName);
                        if (!Directory.Exists(fallbackFolder))
                        {
                            Directory.CreateDirectory(fallbackFolder);
                        }

                        string fallbackPath = Path.Combine(fallbackFolder, "Emergency_ErrorLog.txt");
                        string fallbackMsg = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Failed to write primary log for {controller}/{action}: {ex.Message}{Environment.NewLine}Logger Error: {loggerEx.Message}{Environment.NewLine}{ex}{Environment.NewLine}";

                        using (var fs = new FileStream(fallbackPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                        using (var writer = new StreamWriter(fs, Encoding.UTF8))
                        {
                            writer.Write(fallbackMsg);
                            writer.Flush();
                        }
                    }
                    catch
                    {
                        // Best effort emergency catch
                    }
                }
            }
        }

        private static string GetLogsFolderPath()
        {
            try
            {
                if (HttpContext.Current != null && HttpContext.Current.Server != null)
                {
                    string mappedPath = HttpContext.Current.Server.MapPath("~/" + LogFolderName);
                    if (!string.IsNullOrWhiteSpace(mappedPath))
                    {
                        return mappedPath;
                    }
                }
            }
            catch
            {
                // Fall back if HttpContext is not available
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFolderName);
        }

        private static int GetLineNumber(Exception ex)
        {
            try
            {
                if (ex == null) return 0;

                StackTrace trace = new StackTrace(ex, true);
                StackFrame[] frames = trace.GetFrames();

                if (frames != null)
                {
                    foreach (StackFrame frame in frames)
                    {
                        int lineNumber = frame.GetFileLineNumber();
                        if (lineNumber > 0)
                        {
                            return lineNumber;
                        }
                    }
                }
            }
            catch
            {
                // Ignore extraction failures
            }

            return 0;
        }
    }
}