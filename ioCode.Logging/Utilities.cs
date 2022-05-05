//------------------------------------------------------------------------------ 
// 
// File provided for Reference Use Only by ioCode (c) 2022.
// Copyright (c) ioCode. All rights reserved.
//
// Author: Gokhan Erdogdu
// 
//------------------------------------------------------------------------------
using System;
using System.IO;

namespace ioCode.Logging
{
    internal static class Utilities
    {
        public static bool PathEquals(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path1))
            {
                return string.IsNullOrEmpty(path2);
            }

            if (string.IsNullOrEmpty(path2))
            {
                return false;
            }

            path1 = Path.GetFullPath(path1);
            path2 = Path.GetFullPath(path2);

            return string.Equals(Path.GetFullPath(path1), Path.GetFullPath(path2), StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsPathContainRoot(string path, string rootPath)
        {
            string sRoot = Path.GetFullPath(string.IsNullOrWhiteSpace(rootPath) ? "." : rootPath);
            string sPath = Path.GetFullPath(Path.Combine(sRoot, string.IsNullOrWhiteSpace(path) ? "." : path));

            return (sPath.Length >= sRoot.Length) && PathEquals(sPath.Substring(0, sRoot.Length), sRoot);
        }

        public static bool CreateDirectory(string path, bool recursive = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false; // Argument is empty
            }
            else if (Directory.Exists(path))
            {
                // already exists
                return true;
            }

            try
            {
                string sParentDirectory = Path.GetDirectoryName(path);
                if (recursive)
                {
                    if (!CreateDirectory(sParentDirectory, recursive))
                    {
                        return false;
                    }
                }
                else if (!Directory.Exists(sParentDirectory))
                {
                    return false;
                }

                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception)
            { }

            return false;
        }

        public static bool DeleteDirectory(string path, bool recursive)
        {
            if (string.IsNullOrEmpty(path))
            {
                return true; // Argument is empty
            }
            else if (!Directory.Exists(path))
            {
                // already not exists
                return true;
            }

            try
            {
                Directory.Delete(path, recursive);
            }
            catch (Exception)
            { }

            return false;
        }

        public static bool DeleteDirectory(string path)
        {
            return DeleteDirectory(path, false);
        }
    }
}
