//------------------------------------------------------------------------------ 
// 
// File provided for Reference Use Only by ioCode (c) 2022.
// Copyright (c) ioCode. All rights reserved.
//
// Author: Gokhan Erdogdu
// 
//------------------------------------------------------------------------------
using System;
using System.Diagnostics;

namespace ioCode.Logging
{
    public class LogDebug : LogWrapper
    {
        public LogDebug(Logger logWriter = null,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
            : base(LogLevels.Debug, logWriter, memberName, filePath, lineNumber)
        { }

        [Conditional("DEBUG")]
        public void Assert(bool condition)
        {
            if (condition) {
                return;
            }

            WriteLine("ASSERT");
        }

        [Conditional("DEBUG")]
        public void Assert(bool condition, string message)
        {
            if (condition) {
                return;
            }

            WriteLine("ASSERT >> {0}", message);
        }

        [Conditional("DEBUG")]
        public void Assert(bool condition, Exception error)
        {
            if (condition) {
                return;
            }

            WriteLine("ASSERT >> [0x{0:X8}] {1}", error.HResult, error.Message);
        }
    }
}
