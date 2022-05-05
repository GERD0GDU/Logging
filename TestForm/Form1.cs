using System;
using System.Diagnostics;
using System.Windows.Forms;
using ioCode.Logging;

namespace TestForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeLogging();
        }

        private void InitializeLogging()
        {
            Logger.Current.LogWriterEvent += new LogWriterEventHandler(Logger_LogWriterEvent);
            Logger.Current.RootDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Logger.Current.FileNameFormat = @".\ioCode.Logging.Test\${yyyy}\${MMMM}\ioCodeLogging_${yyyyMMdd}.log";
            Logger.Current.LogLevel = LogLevels.All;
            Logger.Current.LifetimeInDays = 7;
            Logger.Current.DeleteExpiredFiles = true;

            new LogNotification().WriteLine("The '{0} {1}' application is started.",  AssemblyInfo.Title, AssemblyInfo.Version);
            new LogNotification().WriteLine("File: \"{0}\"", Logger.Current.FileName);
        }

        private void TerminateLogging()
        {
            Logger.Current.LogWriterEvent -= new LogWriterEventHandler(Logger_LogWriterEvent);
            Logger.Current.DeleteExpiredFiles = false;

            new LogNotification().WriteLine("The application has been closed.");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            TerminateLogging();
            base.OnFormClosed(e);
        }

        private void Logger_LogWriterEvent(object sender, LogEventArgs e)
        {
            listBoxLogs.Items.Add(e.ToString());
            while (listBoxLogs.Items.Count > 4096) { listBoxLogs.Items.RemoveAt(0); }
            listBoxLogs.SelectedIndex = listBoxLogs.Items.Count - 1;
        }

        private void Buttons_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            LogLevels logLevel = (LogLevels)Enum.Parse(typeof(LogLevels), btn.Text);
            switch (logLevel)
            {
                case LogLevels.Notification:
                    new LogNotification().WriteLine($"{logLevel} message");
                    break;
                case LogLevels.Warning:
                    new LogWarning().WriteLine($"{logLevel} message");
                    break;
                case LogLevels.Error:
                    new LogError().WriteLine($"{logLevel} message");
                    break;
                case LogLevels.Debug:
                    new LogDebug().WriteLine($"{logLevel} message");
                    break;
            }
        }

        private void btnOpenLogFileLocation_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                Process.Start("explorer.exe", $"/select, \"{Logger.Current.FileName}\"");
            }            
            catch (Exception error)
            {
                new LogError().WriteLine(error);
                MessageBox.Show(this, error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}
