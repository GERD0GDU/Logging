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
    [DebuggerDisplay(@"\{this.ToString(),nq\}")]
    public class LogEventArgs : EventArgs
    {
        public LogEventArgs(LogMessageLine message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return Message;
        }

        public LogMessageLine Message { get; private set; }
    }
}
