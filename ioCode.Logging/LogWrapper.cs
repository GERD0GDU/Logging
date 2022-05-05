//------------------------------------------------------------------------------ 
// 
// File provided for Reference Use Only by ioCode (c) 2022.
// Copyright (c) ioCode. All rights reserved.
//
// Author: Gokhan Erdogdu
// 
//------------------------------------------------------------------------------
using System;

namespace ioCode.Logging
{
    public abstract class LogWrapper
    {
        private Logger m_logWriter = null;
        private LogLevels m_nLoggingLevel = LogLevels.Disabled;
        private string m_sFilePath = null;
        private string m_sMemberName = null;
        private int m_nLineNumber = 0;

        public LogWrapper(LogLevels loggingLevel, Logger logWriter = null,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            m_nLoggingLevel = loggingLevel;
            m_logWriter = logWriter ?? Logger.Current;
            m_sFilePath = filePath;
            m_sMemberName = memberName;
            m_nLineNumber = lineNumber;
        }

        public LogLevels LoggingLevel => m_nLoggingLevel;
        public string FilePath => m_sFilePath;
        public string MemberName => m_sMemberName;
        public int LineNumber => m_nLineNumber;

        public void WriteLine(string message)
        {
            m_logWriter.WriteLine(m_nLoggingLevel, message ?? "", m_sMemberName, m_sFilePath, m_nLineNumber);
        }

        public void WriteLine(string format, object arg0)
        {
            m_logWriter.WriteLine(m_nLoggingLevel, string.Format(format ?? "", arg0), m_sMemberName, m_sFilePath, m_nLineNumber);
        }

        public void WriteLine(string format, params object[] args)
        {
            m_logWriter.WriteLine(m_nLoggingLevel, string.Format(format ?? "", args), m_sMemberName, m_sFilePath, m_nLineNumber);
        }

        public void WriteLine(Exception ex)
        {
            if (ex.IsNotNull())
            {
                m_logWriter.WriteLine(m_nLoggingLevel, ex.ToMessages(), m_sMemberName, m_sFilePath, m_nLineNumber);
            }
        }
    }
}
