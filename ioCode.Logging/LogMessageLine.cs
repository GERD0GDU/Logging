//------------------------------------------------------------------------------ 
// 
// File provided for Reference Use Only by ioCode (c) 2022.
// Copyright (c) ioCode. All rights reserved.
//
// Author: Gokhan Erdogdu
// 
//------------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace ioCode.Logging
{
    [DebuggerDisplay(@"{this.ToString(),nq}")]
    public class LogMessageLine : ICloneable
    {
        public LogMessageLine()
        {
        }

        [DisplayName("Logging Level")]
        public LogLevels LoggingLevel { get; set; }

        [DisplayName("Thread Id")]
        public int ThreadId { get; set; }

        [DisplayName("Member")]
        public string MemberName { get; set; }

        [DisplayName("Source")]
        public string Source { get; set; }

        [DisplayName("Source Name")]
        public string SourceName
        {
            get
            {
                return string.IsNullOrEmpty(Source) ? null : System.IO.Path.GetFileName(Source);
            }
        }

        [DisplayName("Line Number")]
        public int LineNumber { get; set; }

        [DisplayName("Date")]
        public DateTime Date { get; set; }

        [DisplayName("Message")]
        public string Message { get; set; }

        public object Clone()
        {
            return new LogMessageLine()
            {
                LoggingLevel = LoggingLevel,
                ThreadId = ThreadId,
                MemberName = MemberName,
                Source = Source,
                LineNumber = LineNumber,
                Date = Date,
                Message = Message,
            };
        }

        public override string ToString()
        {
            string sMessage = Message.Replace("\r\n", " ").Replace('\r', ' ').Replace('\n', ' ');
            string sSourceInfo = null;

            if (!string.IsNullOrEmpty(MemberName))
            {
                sSourceInfo = $" - {MemberName}";
                if (!string.IsNullOrEmpty(SourceName) && (LineNumber >= 0))
                {
                    sSourceInfo += $"({SourceName}[Ln:{LineNumber}])";
                }
            }

            // sadece 'Debug' icin kod kaynak dosya adi ve satir numarasi yazilacak
            //
            //  * Log Line Format: "yyyy-MM-ddTHH:mm:ss.fffzzz [THREAD_ID] [LOG_TYPES] LOG_MESSAGE - MEMBER_NAME(FILE_NAME[Ln:LINE])"
            //
            if (LoggingLevel.HasFlag(LogLevels.Error))
            {
                return $"{Date:yyyy-MM-ddTHH:mm:ss.fffzzz} [0x{ThreadId:X8}] [E] {sMessage}{sSourceInfo}";
            }
            else if (LoggingLevel.HasFlag(LogLevels.Warning))
            {
                return $"{Date:yyyy-MM-ddTHH:mm:ss.fffzzz} [0x{ThreadId:X8}] [W] {sMessage}{sSourceInfo}";
            }
            else if (LoggingLevel.HasFlag(LogLevels.Notification))
            {
                return $"{Date:yyyy-MM-ddTHH:mm:ss.fffzzz} [0x{ThreadId:X8}] [N] {sMessage}";
            }
            else if (LoggingLevel.HasFlag(LogLevels.Debug))
            {
                return $"{Date:yyyy-MM-ddTHH:mm:ss.fffzzz} [0x{ThreadId:X8}] [D] {sMessage}{sSourceInfo}";
            }           
            else
            {
#if DEBUG
                System.Diagnostics.Debug.Assert(false, "Log tipi bilinmiyor. Kodda bir hata olabilir.");
#endif
                return $"{Date:yyyy-MM-ddTHH:mm:ss.fffzzz} [0x{ThreadId:X8}] [?] {sMessage}";
            }
        }

        public static implicit operator string(LogMessageLine logMessageLine)
        {
            return logMessageLine.IsNull()
                ? null
                : logMessageLine.ToString();
        }
    }
}
