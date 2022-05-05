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
    [Flags]
    public enum LogLevels
    {
        Disabled = 0,
        Notification = 1,
        Warning = 2,
        Error = 4,
        Debug = 8,
        All = Notification | Warning | Error | Debug,
    }
}
