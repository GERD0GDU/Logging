//------------------------------------------------------------------------------ 
// 
// File provided for Reference Use Only by ioCode (c) 2022.
// Copyright (c) ioCode. All rights reserved.
//
// Author: Gokhan Erdogdu
// 
//------------------------------------------------------------------------------
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Collections.Specialized;
using System;

namespace ioCode.Logging
{
    [DebuggerDisplay(@"\{RootDirectoryPath={RootDirectoryPath} LoggingLevel={LoggingLevel} FileNameFormat={FileNameFormat} ...\}")]
    public partial class Logger : ViewModelBase
    {
        #region Private Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never), Browsable(false)]
        private readonly object m_oLocker = new object();
        [DebuggerBrowsable(DebuggerBrowsableState.Never), Browsable(false)]
        private StreamWriter m_writer = null;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), Browsable(false)]
        private SynchronizationContext m_defSync;

        #endregion // Private Fields

        #region Public Static Properties

        [DebuggerBrowsable(DebuggerBrowsableState.Never), Browsable(false)]
        private static readonly Logger _Current = new Logger();
        public static Logger Current { get { return _Current; } }

        public static readonly string DefaultFileNameFormat = "${yyyy}\\${MMMM}\\${yyyyMMdd}.log";

        #endregion // Public Static Properties

        #region Constructures

        public Logger()
        {
            m_defSync = SynchronizationContext.Current;
        }

        public Logger(LogLevels loggingLevel, string fileFormat)
            : this()
        {
            LogLevel = loggingLevel;
            FileNameFormat = fileFormat;
            CheckCurrentFileName();
        }

        #endregion // Constructures

        #region <Dispose Methods>

        protected override void OnDisposed(bool disposing)
        {
            if (disposing)
            {
                // Free any other managed objects here. 
                //
                Close();
            }
            // Free any unmanaged objects here. 
            //

            base.OnDisposed(disposing);
        }

        #endregion // </Dispose Methods>

        #region Events

        public event LogWriterEventHandler LogWriterEvent;

        #endregion // Events

        #region Helper

        private void SynchronizeInvoke(SendOrPostCallback cb)
        {
            if (m_defSync.IsNotNull())
            {
                m_defSync.Post(cb, null);
            }
            else
            {
                cb(null);
            }
        }

        private void CheckCurrentFileName()
        {
            try
            {
                lock (m_oLocker)
                {
                    string sCompileFileName = Macro.Current.Compile(_FileNameFormat);
                    string sCurFileName = string.IsNullOrWhiteSpace(sCompileFileName)
                        ? null
                        : Path.GetFullPath(Path.Combine(_RootDirectoryPath, sCompileFileName));

                    if (!Utilities.IsPathContainRoot(sCurFileName, _RootDirectoryPath))
                    {
                        Debug.WriteLine("ERROR >> The current filename does not contain the root path.");
                        return;
                    }

                    if (!Utilities.PathEquals(sCurFileName, FileName))
                    {

                        string sPrevLogFileName = FileName;
                        FileName = sCurFileName;

                        if (m_writer.IsNotNull())
                        {
                            if (!string.IsNullOrWhiteSpace(sCurFileName))
                            {
                                LogMessageLine logLine = new LogMessageLine()
                                {
                                    LoggingLevel = LogLevels.Notification,
                                    ThreadId = Thread.CurrentThread.ManagedThreadId,
                                    Date = DateTime.Now,
                                    Message = string.Format("Next Log File = '{0}'", sCurFileName)
                                };

                                m_writer.WriteLine(logLine.ToString());
                                m_writer.Flush();
                            }

                            m_writer.Close();
                            m_writer.Dispose();
                            m_writer = null;
                        }

                        if (Directory.Exists(_RootDirectoryPath)
                            && !string.IsNullOrWhiteSpace(sCurFileName)
                            && Utilities.IsPathContainRoot(sCurFileName, _RootDirectoryPath)
                            && Utilities.CreateDirectory(Path.GetDirectoryName(sCurFileName), true))
                        {
                            bool isNewFile = !File.Exists(FileName);
                            m_writer = new StreamWriter(FileName, true);
                            if (isNewFile || (m_writer.BaseStream.Length == 0))
                            {
                                // Write log header to the new file
                                m_writer.WriteLine("#");
                                m_writer.WriteLine("# [LOG_TYPES]");
                                m_writer.WriteLine("#   [N]: Notification");
                                m_writer.WriteLine("#   [W]: Warning");
                                m_writer.WriteLine("#   [E]: Error");
                                m_writer.WriteLine("#   [D]: Debug");
                                m_writer.WriteLine("# Log Line Format: 'yyyy-MM-ddTHH:mm:ss.fffzzz [THREAD_ID] [LOG_TYPES] LOG_MESSAGE - MEMBER_NAME(FILE_NAME[Ln:LINE])'");
                                m_writer.WriteLine("#");

                                if (!string.IsNullOrWhiteSpace(sPrevLogFileName))
                                {
                                    LogMessageLine logLine = new LogMessageLine()
                                    {
                                        LoggingLevel = LogLevels.Notification,
                                        ThreadId = Thread.CurrentThread.ManagedThreadId,
                                        Date = DateTime.Now,
                                        Message = string.Format("Previous Log File = '{0}'", sPrevLogFileName)
                                    };

                                    m_writer.WriteLine(logLine.ToString());
                                }

                                m_writer.Flush();
                            }
                        }
                    }
                }
            }
            catch { }
        }

        #endregion // Helper

        #region Properties

        [DebuggerBrowsable(DebuggerBrowsableState.Never), Browsable(false)]
        private readonly List<LogMessageLine> _LogMessages = new List<LogMessageLine>();
        /// <summary>
        /// Total last '1' day messages.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never), Browsable(false)]
        public IReadOnlyList<LogMessageLine> LogMessages
        {
            get
            {
                lock (m_oLocker)
                {
                    return _LogMessages
                        .Select(x => x.SafeClone())
                        .ToList()
                        .AsReadOnly();
                }
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), Browsable(false)]
        private LogLevels _LogLevel = LogLevels.All;
        /// <summary>
        /// The level of log records.<br/>
        /// <br/>
        /// --------------------------------------------------------------------------------<br/>
        /// Levels:<br/>
        /// <code>
        /// Disabled      : Logging is disabled
        /// Notifications : Only notification lines are enabled.
        /// Warnings      : Only warning lines are enabled.
        /// Errors        : Only error lines are enabled.
        /// Debug         : Only debug lines are enabled.
        /// All           : All levels enabled
        /// </code>
        /// --------------------------------------------------------------------------------<br/>
        /// </summary>
        [DisplayName("Log File Level")]
        [Description("Notifications, Traces, Warnings, Errors and Commands levels. Default is 'All'.")]
        public LogLevels LogLevel
        {
            get { lock (m_oLocker) { return _LogLevel; } }
            set { lock (m_oLocker) { SetProperty(ref _LogLevel, value); } }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), Browsable(false)]
        private string _RootDirectoryPath = null;
        /// <summary>
        /// The root directory where log records are stored.
        /// </summary>
        [DisplayName("Logs Root Path")]
        [Description("Root directory for log files")]
        public string RootDirectoryPath
        {
            get { lock (m_oLocker) { return _RootDirectoryPath; } }
            set { lock (m_oLocker) { SetProperty(ref _RootDirectoryPath, value); } }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), Browsable(false)]
        private string _FileNameFormat = @"${yyyyMMdd}.log";
        /// <summary>
        /// Description:<br/>
        /// The file name format in which the log records will be stored is specified.<br/>
        /// Supports custom DateTime macros. These macros are listed below.<br/>
        /// <br/>
        /// Macros:<br/>
        /// <code>
        /// "${dddd, dd MMMM yyyy HH:mm:ss}" -> "Friday, 29 May 2015 05:50:06"
        /// "${ddd, dd MMM yyy HH':'mm':'ss 'GMT'}" -> "Fri, 16 May 2015 05:50:06 GMT"
        /// </code>
        /// ------------------------------------------------------------------------------------------------<br/>
        /// <code>
        /// ${h}:    12-hour clock hour (e.g. 4)
        /// ${hh}:   12-hour clock, with a leading 0 (e.g. 06)
        /// ${H}:    24-hour clock hour (e.g. 15)
        /// ${HH}:   24-hour clock hour, with a leading 0 (e.g. 22)
        /// ${m}:    Minutes
        /// ${mm}:   Minutes with a leading zero
        /// ${s}:    Seconds
        /// ${ss}:   Seconds with a leading zero
        /// ${f}:    Represents the tenths of a second
        /// ${ff}:   Represents the two most significant digits of the seconds' fraction in date and time
        /// ${fff}:  Milliseconds
        /// ${t}:    Abbreviated AM / PM (e.g. A or P)
        /// ${tt}:   AM / PM (e.g. AM or PM)
        /// ${d}:    Represents the day of the month as a number from 1 through 31
        /// ${dd}:   Represents the day of the month as a number from 01 through 31
        /// ${ddd}:  Represents the abbreviated name of the day (Mon, Tues, Wed, etc)
        /// ${dddd}: Represents the full name of the day (Monday, Tuesday, etc)
        /// ${wd}:   Represents the day of the week (e.g. 7 for Sunday)
        /// ${M}:    Month number (eg. 3)
        /// ${MM}:   Month number with leading zero (eg. 04)
        /// ${MMM}:  Abbreviated Month Name (e.g. Dec)
        /// ${MMMM}: Full month name (e.g. December)
        /// ${y}:    Year, no leading zero (e.g. 2015 would be 15)
        /// ${yy}:   Year, leading zero (e.g. 2015 would be 015)
        /// ${yyy}:  Year, (e.g. 2015)
        /// ${yyyy}: Year, (e.g. 2015)
        /// ${z}:    With DateTime values represents the signed offset of the local operating 
        ///          system's time zone from Coordinated Universal Time (UTC), measured in hours. (e.g. +6)
        /// ${zz}:   As z, but with leading zero (e.g. +06)
        /// ${zzz}:  With DateTime values represents the signed offset of the local operating
        ///          system's time zone from UTC, measured in hours and minutes. (e.g. +06:00)
        /// </code>
        /// ------------------------------------------------------------------------------------------------<br/>
        /// Example:<br/>
        /// <example>
        /// <code>
        /// @"SingleLogFile.log"
        /// @".\${yyyy}\${MMMM}\ioMCR_${yyyyMMdd}.log" --> @".\2021\September\ioMCR_20210903.log"
        /// </code>
        /// </example>
        /// </summary>
        [DisplayName("Log File Name Format")]
        [Description("Log file name format string.")]
        public string FileNameFormat
        {
            get { lock (m_oLocker) { return _FileNameFormat; } }
            set { lock (m_oLocker) { SetProperty(ref _FileNameFormat, value); } }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), Browsable(false)]
        private int _LifetimeInDays = 0;
        // Total lifetime of log files in days. (Default: 7 days)
        [DisplayName("Log File Lifetime (in days)")]
        [Description("Log files lifetime in days. A numerical value from '0' to '2147483647'. Default is '7'.")]
        public int LifetimeInDays
        {
            get { lock (m_oLocker) { return _LifetimeInDays; } }
            set { lock (m_oLocker) { SetProperty(ref _LifetimeInDays, value.Range(0, int.MaxValue)); } }
        }

        /// <summary>
        /// Current log folder directory. (Read-only)
        /// </summary>
        [ReadOnly(true)]
        [DisplayName("Current Log File Directory")]
        [Description("Directory location of current log file")]
        public string CurrentDirectoryPath
        {
            get
            {
                string sFileName = FileName;
                if (string.IsNullOrEmpty(sFileName))
                    return "";

                return Path.GetDirectoryName(sFileName);
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), Browsable(false)]
        private string _FileName = null;
        /// <summary>
        /// Current log file path. (Read-only)
        /// </summary>
        [ReadOnly(true)]
        [DisplayName("Current File Name")]
        [Description("Current log file location")]
        public string FileName
        {
            get
            {
                lock (m_oLocker) { return _FileName; }
            }
            private set
            {
                lock (m_oLocker)
                {
                    if (SetProperty(ref _FileName, value))
                    {
                        RaisePropertyChanged(nameof(CurrentDirectoryPath));
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public void Close()
        {
            try
            {
                lock (m_oLocker)
                {
                    if (m_writer.IsNull())
                    {
                        return;
                    }

                    _LogMessages.Clear();
                    m_writer.Close();
                    m_writer.Dispose();
                    m_writer = null;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Adds a new line to the existing log file.
        /// </summary>
        /// <param name="loggingLevel">Log level</param>
        /// <param name="logMessage">Log message text</param>
        /// <param name="memberName">The name of the member who wrote the log line</param>
        /// <param name="filePath">Path to the code file that writes the log line</param>
        /// <param name="lineNumber">Line number of the code file that writes the log line</param>
        public void WriteLine(LogLevels loggingLevel, string logMessage,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            try
            {
                lock (m_oLocker)
                {
                    // mevcut log dosyasini kontrol et
                    // yoksa olustur
                    CheckCurrentFileName();

                    LogMessageLine logLine = new LogMessageLine()
                    {
                        LoggingLevel = loggingLevel,
                        ThreadId = Thread.CurrentThread.ManagedThreadId,
                        MemberName = memberName,
                        Source = filePath,
                        LineNumber = lineNumber,
                        Date = DateTime.Now,
                        Message = logMessage
                    };

                    if ((_LogLevel != LogLevels.Disabled) &&
                        _LogLevel.HasFlag(loggingLevel) &&
                        m_writer.IsNotNull())
                    {
                        m_writer.WriteLine(logLine.ToString());
                        m_writer.Flush();
                    }

                    SynchronizeInvoke(o =>
                    {
                        if (LogWriterEvent.IsNotNull())
                        {
                            LogWriterEvent(this, new LogEventArgs(logLine));
                        }
                    });

                    _LogMessages.Add(logLine);

                    // Remove messages older than '1' day.
                    DateTime dtNow = DateTime.Now;
                    for (; _LogMessages.Count > 0;)
                    {
                        LogMessageLine item = _LogMessages[0];
                        if ((dtNow - item.Date).TotalDays <= 1d)
                        {
                            break;
                        }
                        _LogMessages.RemoveAt(0);
                    }
                }
            }
            catch
            {
            }
        }

        public void CopyTo(Stream output)
        {
            lock (m_oLocker)
            {
                if (m_writer.IsNull())
                {
                    return;
                }

                StreamWriter sw = new StreamWriter(output);
                foreach (LogMessageLine logLine in _LogMessages)
                {
                    sw.WriteLine(logLine.ToString());
                }
                sw.Flush();
            }
        }

        public void CopyTo(StringCollection output)
        {
            lock (m_oLocker)
            {
                if (m_writer.IsNull())
                {
                    return;
                }

                output.Clear();
                foreach (LogMessageLine logLine in _LogMessages)
                {
                    output.Add(logLine.ToString());
                }
            }
        }

        #endregion Public Methods
    }
}
