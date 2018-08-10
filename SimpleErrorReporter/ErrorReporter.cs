using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace SimpleErrorReporter
{
    public static class ErrorReporter
    {
        /// <summary>
        /// Invoked when the error reporter attempts to display an error message.
        /// </summary>
        public static event EventHandler<MessageEventArgs> ShowError;

        public static string LogDirectory { get; set; } = "Error Reports";

        private static volatile bool IsInstalled;
        private static readonly object InstallationLock = new object();

        static ErrorReporter()
        {
            ShowError += OnShowError;
        }

        private static void OnShowError(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        public static void Install()
        {
            lock (InstallationLock)
            {
                if (IsInstalled)
                    throw new InvalidOperationException("The error reporter is already installed.");
                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
                IsInstalled = true;
            }
        }

        public static void Uninstall()
        {
            lock (InstallationLock)
            {
                if (!IsInstalled)
                    throw new InvalidOperationException("The error reporter has not been installed.");
                AppDomain.CurrentDomain.UnhandledException -= CurrentDomainOnUnhandledException;
                IsInstalled = false;
            }
        }

        private static void CurrentDomainOnUnhandledException(
            object sender, UnhandledExceptionEventArgs e)
        {
            var now = DateTime.UtcNow;
            var logFileName = $"ErrorReport-{now.Year}-{now.Month}-{now.Day}-{now.Hour}-{now.Minute}-{now.Second}-{now.Millisecond}.txt";
            var logFilePath = Path.GetFullPath(Path.Combine(LogDirectory, logFileName));

            try
            {
                Directory.CreateDirectory(LogDirectory);
                using (var file = File.OpenWrite(logFilePath))
                using (var writer = new StreamWriter(file))
                {
                    var fileVersionInfo = FileVersionInfo;
                    writer.WriteLine("======================================================");
                    writer.WriteLine("==================== Error Report ====================");
                    writer.WriteLine("======================================================");
                    writer.WriteLine();
                    writer.WriteLine("========== Product Information ==========");
                    writer.WriteLine($"Product Name     {fileVersionInfo.ProductName}");
                    writer.WriteLine($"Company Name     {fileVersionInfo.CompanyName}");
                    writer.WriteLine($"Product Version  {fileVersionInfo.ProductVersion}");
                    writer.WriteLine($"File Version     {fileVersionInfo.FileVersion}");
                    writer.WriteLine();
                    writer.WriteLine("========== Environment Information ==========");
                    writer.WriteLine($"Opering System   {Environment.OSVersion}");
                    writer.WriteLine($"Version          {Environment.Version}");
                    writer.WriteLine($"Current Date     {now.ToString(CultureInfo.InvariantCulture)}");
                    writer.WriteLine();
                    writer.WriteLine("========== Exception ==========");
                    writer.WriteLine((Exception)e.ExceptionObject);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ShowError?.Invoke(null, new MessageEventArgs(
                    "An error has occurred and the application must now close.\n" +
                    "\n" +
                    "An error report could not created. The application will now close."));
                return;
            }

            ShowError?.Invoke(null, new MessageEventArgs(
                "An error has occurred and the application must now close.\n" +
                "\n" +
                "A report of this error has been created at the following location:\n\n" +
                $"{logFilePath}"));
        }

        private static FileVersionInfo FileVersionInfo =>
            FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
    }
}
