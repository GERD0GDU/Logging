//------------------------------------------------------------------------------ 
// 
// File provided for Reference Use Only by ioCode (c) 2022.
// Copyright (c) ioCode. All rights reserved.
//
// Author: Gokhan Erdogdu
// 
//------------------------------------------------------------------------------
using System;
using System.Linq;
using System.Collections.Generic;

namespace ioCode.Logging
{
    internal static class Extensions
    {
        public static bool IsNull(this object obj)
        {
            return obj is null;
        }

        public static bool IsNotNull(this object obj)
        {
            return !(obj is null);
        }

        public static bool IsEmpty(this System.IO.DirectoryInfo directory)
        {
            return directory.IsNotNull()
                && directory.Exists
                && !directory.EnumerateFileSystemInfos().Any();
        }

        public static string ToMessages(this Exception error)
        {
            if (error == null)
            {
                return null;
            }

            string message;
            List<string> messages = new List<string>();
            do
            {
                // remove new lines
                message = string.Join(" ", error.Message.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
                messages.Add(message);
                error = error.InnerException;
            } while (error != null);

            return string.Join(" ---> ", messages);
        }

        public static T SafeClone<T>(this T baseObject)
            where T : ICloneable
        {
            if (baseObject is ICloneable o2)
            {
                return (T)o2.Clone();
            }
            return baseObject;
        }
    }
}
