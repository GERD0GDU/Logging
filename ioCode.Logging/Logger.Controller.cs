//------------------------------------------------------------------------------ 
// 
// File provided for Reference Use Only by ioCode (c) 2022.
// Copyright (c) ioCode. All rights reserved.
//
// Author: Gokhan Erdogdu
// 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ioCode.Logging
{
    partial class Logger
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), Browsable(false)]
        private Timer m_timerCheck = null;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), Browsable(false)]
        private bool m_firstInitLogFileInfos = false;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), Browsable(false)]
        private readonly List<FileInfo> m_logFileInfos = new List<FileInfo>();

        #region Properties

        /// <summary>
        /// 'Yerel diskte saklama suresi' dolan gunluk dosyalarinin
        /// silinip silinmeyecegini belirten ozelliktir.
        /// True olarak ayarlandiginda aktiflesir.
        /// </summary>
        public bool DeleteExpiredFiles
        {
            get
            {
                lock (m_oLocker)
                {
                    return m_timerCheck.IsNotNull();
                }
            }
            set
            {
                lock (m_oLocker)
                {
                    bool oldValue = m_timerCheck.IsNotNull();
                    if (oldValue == value)
                    {
                        return;
                    }

                    if (value)
                    {
                        m_timerCheck = new Timer(new TimerCallback(DeleteExpiredFiles_TimerCallback), null, 1000 /* start after 1000 milliseconds */, -1 /* disable periodic signaling */);
                    }
                    else
                    {
                        m_timerCheck.Dispose();
                        m_timerCheck = null;
                    }
                }
            }
        }

        #endregion

        #region Delete Expired Files Methods

        private List<FileInfo> GetFileInfos(DirectoryInfo directory)
        {
            List<FileInfo> retVal = new List<FileInfo>();
            if (directory.IsNull() || !directory.Exists)
                return retVal;

            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                retVal.AddRange(GetFileInfos(subDirectory));
            }

            retVal.AddRange(directory.GetFiles());

            return retVal;
        }

        private void FirstInitLogFileInfos()
        {
            if (m_firstInitLogFileInfos)
                return;
            m_firstInitLogFileInfos = true;

            try
            {
                m_logFileInfos.AddRange(GetFileInfos(new DirectoryInfo(RootDirectoryPath)));
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Belirtilen klasor bos ise ve 'RootDirectoryPath' olarak belirtilen
        /// klasorun alt dizini ise; tum alt dizinlerle birlikte silinecektir.
        /// </summary>
        /// <param name="directory"></param>
        private void DeleteDirectoryIfEmpty(DirectoryInfo directory)
        {
            // Silinecek olan klasor, 'RootDirectoryPath' olarak ayarlanmis olan
            // klasorun alt dizini olmalidir. 
            if (Utilities.IsPathContainRoot(directory.FullName, RootDirectoryPath)
                && !Utilities.PathEquals(directory.FullName, RootDirectoryPath)
                && directory.IsEmpty())
            {
                try
                {
                    directory.Delete();
                }
                catch (Exception)
                {
                    return; // error
                }

                DeleteDirectoryIfEmpty(directory.Parent);
            }
        }

        /// <summary>
        /// Periyodik olarak gunluk dosyalarini kontrol eden prosedurdur
        /// </summary>
        /// <param name="state"></param>
        private void DeleteExpiredFiles_TimerCallback(object state)
        {
            const int nCheckPeriodeInMilliseconds = 5000;
            int lifetimeInDays = LifetimeInDays;

            if (lifetimeInDays > 0)
            {
                // first initialize
                FirstInitLogFileInfos();

                FileInfo fileInfo = m_logFileInfos.Find(x => (DateTime.Now - x.LastWriteTime).TotalDays >= lifetimeInDays);
                if (fileInfo.IsNotNull())
                {
                    try
                    {
                        fileInfo.Delete();
                        // remove from list
                        m_logFileInfos.Remove(fileInfo);
                        // delete parent directory
                        DeleteDirectoryIfEmpty(fileInfo.Directory);
                    }
                    catch (Exception)
                    { }
                }
            }

            lock (m_oLocker)
            {
                if (m_timerCheck.IsNotNull())
                {
                    m_timerCheck.Change(nCheckPeriodeInMilliseconds, -1 /* disable periodic signaling */);
                }
            }
        }

        #endregion
    }
}
