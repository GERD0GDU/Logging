//------------------------------------------------------------------------------ 
// 
// File provided for Reference Use Only by ioCode (c) 2022.
// Copyright (c) ioCode. All rights reserved.
//
// Author: Gokhan Erdogdu
// 
//------------------------------------------------------------------------------

namespace ioCode.Logging
{
    public class LogWarning : LogWrapper
    {
        public LogWarning(Logger logWriter = null,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
            : base(LogLevels.Warning, logWriter, memberName, filePath, lineNumber)
        { }
    }
}
