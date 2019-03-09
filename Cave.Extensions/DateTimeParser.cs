using System;
using System.Text.RegularExpressions;

namespace Cave
{
    /// <summary>
    /// Provides a parser for DateTime strings.
    /// </summary>
    public static class DateTimeParser
    {
        static readonly string timeZoneRegEx = @"(?:\s*(?'TimeZone'" + string.Join("|", TimeZones.GetNames()) + "))?";
        static DateTime? defaultDateTime;

        static bool ConvertDate(string year, string month, string day, out DateTime date)
        {
            date = new DateTime(0, DateTimeKind.Utc);
            if (!int.TryParse(year, out var y))
            {
                return false;
            }

            if (!int.TryParse(month, out var m))
            {
                return false;
            }

            if (!int.TryParse(day, out var d))
            {
                return false;
            }

            if (y >= 100)
            {
                if (y < 1000)
                {
                    return false;
                }
            }
            else
            {
                if (y > Default.Year % 100)
                {
                    y += 1900;
                }
                else
                {
                    y += 2000;
                }
            }

            try
            {
                date = new DateTime(y, m, d, 0, 0, 0, DateTimeKind.Utc);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Amount of seconds since 1970-01-01 00:00:00 (may return negative values for earlier dates).
        /// </summary>
        /// <param name="dateTime">The DateTime.</param>
        /// <returns>seconds.</returns>
        public static long GetSecondsSinceUnixEpoch(DateTime dateTime)
        {
            TimeSpan time = dateTime - new DateTime(1970, 1, 1);
            return (long)time.TotalSeconds;
        }

        /// <summary>
        /// Gets or sets the default date used when parsing incomplete datetimes.
        /// </summary>
        public static DateTime Default
        {
            get
            {
                if (defaultDateTime.HasValue)
                {
                    return defaultDateTime.Value;
                }
                return DateTime.UtcNow.Date;
            }
            set => defaultDateTime = value;
        }

        /// <summary>
        /// Tries to find date and time within the passed string and return it as DateTime structure.
        /// </summary>
        /// <param name="text">string that contains date and/or time.</param>
        /// <param name="dateTime">parsed DateTime value.</param>
        /// <returns>Returns true if a date or time was found, false otherwise.</returns>
        public static bool TryParseDateTime(string text, out DateTime dateTime)
        {
            DateTimeStringResult result = ParseDateTime(text, out dateTime);
            return result.Time.Valid || result.Date.Valid;
        }

        /// <summary>Tries to find date and time within the passed string and return it as DateTime structure.</summary>
        /// <param name="text">string that contains date and/or time.</param>
        /// <param name="utcDateTime">The UTC date time.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>Returns true if a date or time was found, false otherwise.</returns>
        public static bool TryParseDateTime(string text, out DateTime utcDateTime, out TimeSpan offset)
        {
            DateTimeStringResult result = ParseDateTime(text, out utcDateTime, out offset);
            return result.Time.Valid || result.Date.Valid;
        }

        /// <summary>Tries to find date and time within the passed string and return it as DateTime structure.</summary>
        /// <param name="text">string that contains date and/or time.</param>
        /// <param name="utcDateTime">The UTC date time.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>Returns the string bounds of the date and time.</returns>
        public static DateTimeStringResult ParseDateTime(string text, out DateTime utcDateTime, out TimeSpan offset)
        {
            var result = default(DateTimeStringResult);
            result.Time = ParseTime(text, out TimeSpan time, out offset);
            result.Date = ParseDate(text, out utcDateTime);
            utcDateTime += time;
            return result;
        }

        /// <summary>
        /// Tries to find date and time within the passed string and return it as DateTime structure.
        /// </summary>
        /// <param name="text">string that contains date and/or time.</param>
        /// <param name="dateTime">parsed DateTime value.</param>
        /// <returns>Returns the string bounds of the date and time.</returns>
        public static DateTimeStringResult ParseDateTime(string text, out DateTime dateTime)
        {
            var result = default(DateTimeStringResult);
            result.Time = ParseTime(text, out TimeSpan time, out TimeSpan offset);
            result.Date = ParseDate(text, out DateTime date);
            dateTime = new DateTime(date.Ticks + time.Ticks + offset.Ticks, DateTimeKind.Local);
            return result;
        }

        /// <summary>
        /// Tries to find a valid timezone within the passed string and returns it.
        /// To obtain the full offset you have to use TimeZoneData.Offset + Offset.
        /// </summary>
        /// <param name="text">The string to parse.</param>
        /// <param name="timeZoneData">The detected timezone.</param>
        /// <returns></returns>
        public static SubStringResult ParseTimeZone(string text, out TimeZoneData timeZoneData)
        {
            Match match = Regex.Match(text, @"(?=^|\s*)" + timeZoneRegEx + @"(?:\s*(?'OffsetSign'[\+\-]))(?:\s*(?'Offset'\d{4})|\s*(?'OffsetHour'\d{1,2})(?:\:(?'OffsetMinute'\d{0,2})|))", RegexOptions.Compiled);
            TimeSpan offset = TimeSpan.Zero;
            timeZoneData = null;
            if (!match.Success)
            {
                return default;
            }

            if (match.Groups["Offset"].Success)
            {
                var offsetValue = int.Parse(match.Groups["Offset"].Value);
                offset += new TimeSpan(offsetValue / 100, offsetValue % 100, 0);
            }

            if (match.Groups["OffsetHour"].Success)
            {
                offset += TimeSpan.FromHours(int.Parse(match.Groups["OffsetHour"].Value));
            }

            if (match.Groups["OffsetMinute"].Success)
            {
                offset += TimeSpan.FromMinutes(int.Parse(match.Groups["OffsetMinute"].Value));
            }

            if (match.Groups["TimeZone"].Success)
            {
                timeZoneData = TimeZoneData.Get(match.Groups["TimeZone"].Value, offset);
            }

            return new SubStringResult(text, match.Index, match.Length);
        }

        /// <summary>
        /// Tries to find time within the passed string.
        /// Detects [h]h:mm[:ss[.f[f[f[f[f[f]]]]]]][offset] [PM/AM] [UTC/GMT] using a a single regex.
        /// </summary>
        /// <param name="text">String that containing the time.</param>
        /// <param name="time">Parsed time.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>Returns the string bounds of the time.</returns>
        public static SubStringResult ParseTime(string text, out TimeSpan time, out TimeSpan offset)
        {
            time = TimeSpan.Zero;
            offset = TimeSpan.Zero;

            var pattern = @"(?<hour>\d{1,2})\s*:\s*(?<minute>\d{2})\s*(?::(?<second>\d{1,2}){0,1}\s*(?:\.(?<microsecond>\d{1,7})){0,1}){0,1}\s*" +
                @"(?:(?<OffsetSign>[\+\-])(?<OffsetHour>\d{2}):{0,1}(?<OffsetMinute>\d{2}){0,1}){0,1}\s*" +
                @"(?<ampm>(?i:pm|am)){0,1}\s*" + timeZoneRegEx + @"(?=$|[^\d\w])";

            Match match = Regex.Match(text, pattern, RegexOptions.Compiled);

            if (!match.Success)
            {
                return default;
            }

            var h = int.Parse(match.Groups["hour"].Value);
            if (h < 0 || h > 23)
            {
                return default;
            }

            var m = int.Parse(match.Groups["minute"].Value);
            if (m < 0 || m > 59)
            {
                return default;
            }

            var s = 0;
            if (!string.IsNullOrEmpty(match.Groups["second"].Value))
            {
                s = int.Parse(match.Groups["second"].Value);
                if (s < 0 || s > 59)
                {
                    return default;
                }
            }

            if (string.Compare(match.Groups["ampm"].Value, "PM", true) == 0 && h < 12)
            {
                h += 12;
            }
            else if (string.Compare(match.Groups["ampm"].Value, "AM", true) == 0 && h == 12)
            {
                h -= 12;
            }

            time = new TimeSpan(h, m, s);

            // Microsecond
            if (match.Groups["microsecond"].Success)
            {
                var microsecond = int.Parse(match.Groups["microsecond"].Value.PadRight(7, '0'));
                time += new TimeSpan(microsecond);
            }

            if (match.Groups["OffsetHour"].Success)
            {
                var offsetHour = int.Parse(match.Groups["OffsetHour"].Value);
                var offsetMinute = 0;
                if (match.Groups["OffsetMinute"].Success)
                {
                    offsetMinute = int.Parse(match.Groups["OffsetMinute"].Value);
                }
                offset = new TimeSpan(offsetHour, offsetMinute, 0);
                if (match.Groups["OffsetSign"].Value == "-")
                {
                    offset = -offset;
                }
                return new SubStringResult(text, match.Index, match.Length);
            }

            if (match.Groups["TimeZone"].Success)
            {
                var data = TimeZoneData.Get(match.Groups["TimeZone"].Value, TimeSpan.Zero);
                offset = data.Offset;
                return new SubStringResult(text, match.Index, match.Length);
            }
            return new SubStringResult(text, match.Index, match.Length);
        }

        /// <summary>
        /// Tries to find date within the passed string and return it as DateTimeString object.
        /// It recognizes only date while ignoring time, so time in the returned DateTimeString is always 0:0:0.
        /// If year of the date was not found then it accepts the current year.
        /// </summary>
        /// <param name="text">string that contains date.</param>
        /// <param name="date">parsed date output.</param>
        /// <returns>Returns the string bounds of the date.</returns>
        public static SubStringResult ParseDate(string text, out DateTime date)
        {
            if (string.IsNullOrEmpty(text))
            {
                date = Default;
                return default;
            }

            // look for mm/dd/yy
            Match match = Regex.Match(text, @"(?<=^|[^\d])(?'day'\d{1,2})\s*(?'separator'[\\/\.])+\s*(?'month'\d{1,2})\s*\'separator'+\s*(?'year'\d{2}|\d{4})(?=$|[^\d])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                if (match.Groups["separator"].Value == "/")
                {
                    if (ConvertDate(match.Groups["year"].Value, match.Groups["day"].Value, match.Groups["month"].Value, out date))
                    {
                        return new SubStringResult(text, match.Index, match.Length);
                    }
                }
                else
                {
                    if (ConvertDate(match.Groups["year"].Value, match.Groups["month"].Value, match.Groups["day"].Value, out date))
                    {
                        return new SubStringResult(text, match.Index, match.Length);
                    }
                }
            }

            // look for [yy]yy-mm-dd
            match = Regex.Match(text, @"(?<=^|[^\d])(?'year'\d{2}|\d{4})\s*(?'separator'[\-])\s*(?'month'\d{1,2})\s*\'separator'+\s*(?'day'\d{1,2})(?=$|[^\d])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                if (ConvertDate(match.Groups["year"].Value, match.Groups["month"].Value, match.Groups["day"].Value, out date))
                {
                    return new SubStringResult(text, match.Index, match.Length);
                }
            }

            // look for month dd yyyy
            match = Regex.Match(text, @"(?:^|[^\d\w])(?'month'Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[uarychilestmbro]*\s+(?'day'\d{1,2})(?:-?st|-?th|-?rd|-?nd)?\s*,?\s*(?'year'\d{4})(?=$|[^\d\w])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (!match.Success)
            { // look for dd month [yy]yy
                match = Regex.Match(text, @"(?:^|[^\d\w:])(?'day'\d{1,2})(?:-?st\s+|-?th\s+|-?rd\s+|-?nd\s+|-|\s+)(?'month'Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[uarychilestmbro]*(?:\s*,?\s*|-)'?(?'year'\d{2}|\d{4})(?=$|[^\d\w])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
            if (!match.Success)
            { // look for yyyy month dd
                match = Regex.Match(text, @"(?:^|[^\d\w])(?'year'\d{4})\s+(?'month'Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[uarychilestmbro]*\s+(?'day'\d{1,2})(?:-?st|-?th|-?rd|-?nd)?(?=$|[^\d\w])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
            if (!match.Success)
            { // look for month dd hh:mm:ss MDT|UTC yyyy
                match = Regex.Match(text, @"(?:^|[^\d\w])(?'month'Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[uarychilestmbro]*\s+(?'day'\d{1,2})\s+\d{2}\:\d{2}\:\d{2}\s+(?:MDT|UTC)\s+(?'year'\d{4})(?=$|[^\d\w])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
            if (!match.Success)
            { // look for  month dd [yyyy]
                match = Regex.Match(text, @"(?:^|[^\d\w])(?'month'Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[uarychilestmbro]*\s+(?'day'\d{1,2})(?:-?st|-?th|-?rd|-?nd)?(?:\s*,?\s*(?'year'\d{4}))?(?=$|[^\d\w])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
            if (!match.Success)
            {
                date = Default;
                return default;
            }

            // SubStringResult bounds = new SubStringResult(text, match.Index, match.Length);
            string month = null;
            switch (match.Groups["month"].Value.ToUpperInvariant())
            {
                case "JAN": month = "1"; break;
                case "FEB": month = "2"; break;
                case "MAR": month = "3"; break;
                case "APR": month = "4"; break;
                case "MAY": month = "5"; break;
                case "JUN": month = "6"; break;
                case "JUL": month = "7"; break;
                case "AUG": month = "8"; break;
                case "SEP": month = "9"; break;
                case "OCT": month = "10"; break;
                case "NOV": month = "11"; break;
                case "DEC": month = "12"; break;
            }

            string year;
            if (!string.IsNullOrEmpty(match.Groups["year"].Value))
            {
                year = match.Groups["year"].Value;
            }
            else
            {
                year = Default.Year.ToString();
            }

            if (ConvertDate(year, month, match.Groups["day"].Value, out date))
            {
                return new SubStringResult(text, match.Index, match.Length);
            }
            return default;
        }
    }
}
