namespace ValueType.Extensions.Dates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class WorkingDays
    {
        /// <summary>
        /// Add working days to a date.
        /// </summary>
        /// <param name="dt">A DateTime</param>
        /// <param name="workingDays">The number of days to add</param>
        /// <param name="skipDays">List of bank holidays</param>
        /// <returns>The resultant date</returns>
        public static DateTime AddWorkingDays(this DateTime dt, double workingDays, IEnumerable<DateTime> skipDays = null)
        {
            if (skipDays == null)
            {
                skipDays = new DateTime[0];
            }

            var dayOfWeek = (int)dt.DayOfWeek;
            var theDayAfterThat = workingDays + dayOfWeek + 1;

            if (dayOfWeek != 0)
            {
                theDayAfterThat--;
            }

            var workingDaysToAdd = Math.Floor(theDayAfterThat / 5) * 2 - dayOfWeek + theDayAfterThat - 2 * Convert.ToInt32(Math.Abs(theDayAfterThat % 5 - 0) < double.Epsilon);
            var startDateWithWorkingDaysAdded = dt.AddDays(workingDaysToAdd);

            var nonWorkingDaysToAdd = skipDays.Count(bh => bh >= dt && bh <= startDateWithWorkingDaysAdded);
            return startDateWithWorkingDaysAdded.AddDays(nonWorkingDaysToAdd);
        }

        /// <summary>
        /// Determines whether a certain date is a working day.
        /// </summary>
        /// <param name="dt">A DateTime</param>
        /// <param name="skipDays">List of bank holidays</param>
        /// <returns>True if working day</returns>
        public static bool IsWorkingDay(this DateTime dt, IEnumerable<DateTime> skipDays = null)
        {
            if (skipDays == null)
            {
                skipDays = new DateTime[0];
            }

            var weekendDays = new[] { DayOfWeek.Saturday, DayOfWeek.Sunday };

            return !weekendDays.Contains(dt.DayOfWeek) && !skipDays.Contains(dt);
        }

        /// <summary>
        /// Gets the working days between two dates.
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <param name="exclusive">Exclude the start and end date from the count</param>
        /// <param name="skipDays">List of bank holidays</param>
        /// <returns>Total working days between</returns>
        public static double GetWorkingDaysUntil(this DateTime startDate, DateTime endDate, bool exclusive = false, ICollection<DateTime> skipDays = null)
        {
            if (skipDays == null)
            {
                skipDays = new DateTime[0];
            }

            var firstDay = startDate.Date;
            var lastDay = endDate.Date;
            var isNegative = false;

            if (firstDay > lastDay)
            {
                var temp = firstDay;
                firstDay = lastDay;
                lastDay = temp;
                isNegative = true;
            }

            var workingDays = 0;

            for (var date = firstDay; exclusive ? date < lastDay : date <= lastDay; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday &&
                    !skipDays.Contains(date))
                {
                    workingDays++;
                }
            }

            return isNegative ? 0 - workingDays : workingDays;
        }
    }
}
