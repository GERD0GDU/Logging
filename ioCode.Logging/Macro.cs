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
using System.Globalization;
using System.Text.RegularExpressions;

namespace ioCode.Logging
{
    [DebuggerDisplay(@"\{CultureInfo={CultureInfo}\}")]
    internal sealed class Macro
    {
        #region Public Static Properties, Field and Methods

        [DebuggerBrowsable(DebuggerBrowsableState.Never), Browsable(false)]
        private static readonly Macro _Current = new Macro();
        public static Macro Current => _Current;

        #endregion // Public Static Properties, Field and Methods

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never), Browsable(false)]
        private readonly Regex m_regex = new Regex(@"\${.*?\}");

        #endregion // Fields

        public Macro()
        {
            CultureInfo = CultureInfo.InvariantCulture;
        }

        #region Properties

        public CultureInfo CultureInfo { get; set; }

        #endregion // Properties

        /// <summary>
        /// [macroString] -> [returns]
        /// "${dddd, dd MMMM yyyy HH:mm:ss}" -> "Friday, 29 May 2015 05:50:06"
        /// "${ddd, dd MMM yyy HH':'mm':'ss 'GMT'}" -> "Fri, 16 May 2015 05:50:06 GMT"
        /// ------------------------------------------------------------------------------------------------
        /// ${h}:    12-hour clock hour (e.g. 4)
        /// ${hh}:   12-hour clock, with a leading 0 (e.g. 06)
        /// ${H}:    24-hour clock hour (e.g. 15)
        /// ${HH}:   24-hour clock hour, with a leading 0 (e.g. 22)
        /// ${m}:    Minutes
        /// ${mm}:   Minutes with a leading zero
        /// ${s}:    Seconds
        /// ${ss}:   Seconds with a leading zero
        /// ${f}:    Represents the tenths of a second
        /// ${ff}:   Represents the two most significant digits of the seconds' fraction in date and time
        /// ${fff}:  Milliseconds
        /// ${t}:    Abbreviated AM / PM (e.g. A or P)
        /// ${tt}:   AM / PM (e.g. AM or PM)
        /// ${d}:    Represents the day of the month as a number from 1 through 31
        /// ${dd}:   Represents the day of the month as a number from 01 through 31
        /// ${ddd}:  Represents the abbreviated name of the day (Mon, Tues, Wed, etc)
        /// ${dddd}: Represents the full name of the day (Monday, Tuesday, etc)
        /// ${wd}:   Represents the day of the week (e.g. 7 for Sunday)
        /// ${M}:    Month number (eg. 3)
        /// ${MM}:   Month number with leading zero (eg. 04)
        /// ${MMM}:  Abbreviated Month Name (e.g. Dec)
        /// ${MMMM}: Full month name (e.g. December)
        /// ${y}:    Year, no leading zero (e.g. 2015 would be 15)
        /// ${yy}:   Year, leading zero (e.g. 2015 would be 015)
        /// ${yyy}:  Year, (e.g. 2015)
        /// ${yyyy}: Year, (e.g. 2015)
        /// ${z}:    With DateTime values represents the signed offset of the local operating 
        ///          system's time zone from Coordinated Universal Time (UTC), measured in hours. (e.g. +6)
        /// ${zz}:   As z, but with leading zero (e.g. +06)
        /// ${zzz}:  With DateTime values represents the signed offset of the local operating 
        ///          system's time zone from UTC, measured in hours and minutes. (e.g. +06:00)
        /// ------------------------------------------------------------------------------------------------
        /// </summary>
        /// <param name="macroString">${DATE_TIME_FORMAT}</param>
        /// <returns>The compiled result of the macro in string</returns>
        public string Compile(string macroString, DateTime dtValue)
        {
            if (string.IsNullOrWhiteSpace(macroString))
            {
                return string.Empty;
            }

            string retVal = string.Empty;
            string sTemp;
            int nPos = 0;

            foreach (Match match in m_regex.Matches(macroString))
            {
                int nLen = match.Index - nPos;
                if (nLen > 0)
                    retVal += macroString.Substring(nPos, nLen);
                nPos = match.Index + match.Value.Length;

                try
                {
                    sTemp = match.Value.Trim('$', '{', '}');
                    if (!string.IsNullOrWhiteSpace(sTemp))
                    {
                        if (sTemp.Length == 1)
                        {
                            sTemp = dtValue.ToString("%" + sTemp, CultureInfo);
                        }
                        else
                        {
                            if (string.Compare(sTemp, "wd", true) == 0)
                            {
                                sTemp = string.Format("{0}", new int[] { 7, 1, 2, 3, 4, 5, 6 }[(int)dtValue.DayOfWeek]);
                            }
                            else
                            {
                                sTemp = dtValue.ToString(sTemp, CultureInfo);
                            }
                        }
                        retVal += sTemp;
                    }
                }
                catch (Exception)
                { }
            }

            if (nPos < macroString.Length)
            {
                retVal += macroString.Substring(nPos);
            }

            return retVal;
        }

        /// <summary>
        /// [macroString] -> [returns]<br/>
        /// "${dddd, dd MMMM yyyy HH:mm:ss}" -> "Friday, 29 May 2015 05:50:06"<br/>
        /// "${ddd, dd MMM yyy HH':'mm':'ss 'GMT'}" -> "Fri, 16 May 2015 05:50:06 GMT"<br/>
        /// ------------------------------------------------------------------------------------------------<br/>
        /// ${h}:    12-hour clock hour (e.g. 4)<br/>
        /// ${hh}:   12-hour clock, with a leading 0 (e.g. 06)<br/>
        /// ${H}:    24-hour clock hour (e.g. 15)<br/>
        /// ${HH}:   24-hour clock hour, with a leading 0 (e.g. 22)<br/>
        /// ${m}:    Minutes<br/>
        /// ${mm}:   Minutes with a leading zero<br/>
        /// ${s}:    Seconds<br/>
        /// ${ss}:   Seconds with a leading zero<br/>
        /// ${f}:    Represents the tenths of a second<br/>
        /// ${ff}:   Represents the two most significant digits of the seconds' fraction in date and time<br/>
        /// ${fff}:  Milliseconds<br/>
        /// ${t}:    Abbreviated AM / PM (e.g. A or P)<br/>
        /// ${tt}:   AM / PM (e.g. AM or PM)<br/>
        /// ${d}:    Represents the day of the month as a number from 1 through 31<br/>
        /// ${dd}:   Represents the day of the month as a number from 01 through 31<br/>
        /// ${ddd}:  Represents the abbreviated name of the day (Mon, Tues, Wed, etc)<br/>
        /// ${dddd}: Represents the full name of the day (Monday, Tuesday, etc)<br/>
        /// ${wd}:   Represents the day of the week (e.g. 7 for Sunday)<br/>
        /// ${M}:    Month number (eg. 3)<br/>
        /// ${MM}:   Month number with leading zero (eg. 04)<br/>
        /// ${MMM}:  Abbreviated Month Name (e.g. Dec)<br/>
        /// ${MMMM}: Full month name (e.g. December)<br/>
        /// ${y}:    Year, no leading zero (e.g. 2015 would be 15)<br/>
        /// ${yy}:   Year, leading zero (e.g. 2015 would be 015)<br/>
        /// ${yyy}:  Year, (e.g. 2015)<br/>
        /// ${yyyy}: Year, (e.g. 2015)<br/>
        /// ${z}:    With DateTime values represents the signed offset of the local operating<br/>
        ///          system's time zone from Coordinated Universal Time (UTC), measured in hours. (e.g. +6)<br/>
        /// ${zz}:   As z, but with leading zero (e.g. +06)<br/>
        /// ${zzz}:  With DateTime values represents the signed offset of the local operating<br/>
        ///          system's time zone from UTC, measured in hours and minutes. (e.g. +06:00)<br/>
        /// ------------------------------------------------------------------------------------------------<br/>
        /// </summary>
        /// <param name="macroString">${DATE_TIME_FORMAT}</param>
        /// <returns>The compiled result of the macro in string</returns>
        public string Compile(string macroString)
        {
            return Compile(macroString, DateTime.Now);
        }
    }
}
