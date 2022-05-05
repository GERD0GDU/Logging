## ioCode.Logging

It is the C# library that helps to keep the errors, warnings and notifications experienced during the runtime of your applications in the log file.

### Classes

[ioCode.Logging](https://github.com/GERD0GDU/Logging) library contains the following classes.
- **Logger** Common class used to write messages to log file. It allows basic settings such as target root folder and target file name.
- **LogNotification** It is the class that writes "Notification" messages to the log file.
- **LogWarning** It is the class that writes "Warning" messages to the log file.
- **LogError** It is the class that writes "Error" messages to the log file.
- **LogDebug** It is the class that writes "Debug" messages to the log file.

### How to use

#### Write to log file
```
  .
  .
  .
  using ioCode.Logging;

  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      // Initial settings
      Logger.Current.RootDirectoryPath = @"C:\users\[current_user]\documents"; // must be exists
      Logger.Current.FileNameFormat = @".\TestLogs\${yyyy}\${MMMM}\ioCodeLogging_${yyyyMMdd}.log";
      Logger.Current.LogLevel = LogLevels.All;

      new LogNotification().WriteLine("notification message");
      new LogWarning().WriteLine("warning message");
      new LogError().WriteLine("error message");
      new LogDebug().WriteLine("debug message");
    }
  }
```
The library automatically creates all subdirectories specified by the "FileNameFormat" property.
In the example above, a new log file is created for each day based on the local date.

#### The file content will be as follows.
```
  #
  # [LOG_TYPES]
  #   [N]: Notification
  #   [W]: Warning
  #   [E]: Error
  #   [D]: Debug
  # Log Line Format: 'yyyy-MM-ddTHH:mm:ss.fffzzz [THREAD_ID] [LOG_TYPES] LOG_MESSAGE - MEMBER_NAME(FILE_NAME[Ln:LINE])'
  #
  2022-05-05T12:58:22.610+03:00 [0x00000001] [N] notification message
  2022-05-05T12:58:22.616+03:00 [0x00000001] [W] warning message - Main(Program.cs[Ln:24])
  2022-05-05T12:58:22.617+03:00 [0x00000001] [E] error message - Main(Program.cs[Ln:25])
  2022-05-05T12:58:22.617+03:00 [0x00000001] [D] debug message - Main(Program.cs[Ln:26])
```

#### Delete Expired Files
A timeout period can be set for files stored on the local disk.
The following example deletes log files older than 7 days from local disk.
```
  // Initial settings
  Logger.Current.LifetimeInDays = 7;
  Logger.Current.DeleteExpiredFiles = true;
```
