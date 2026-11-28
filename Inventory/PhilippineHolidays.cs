using System;

namespace Inventory
{
    /// <summary>
    /// Fixed-date and algorithmically-computable Philippine national holidays only (see
    /// Form1.UpdateGreeting, which swaps the time-of-day greeting for "Happy {holiday}!" on
    /// these dates). Deliberately excludes movable holidays set yearly by presidential
    /// proclamation off a lunar calendar (Chinese New Year, Eid'l Fitr, Eid'l Adha) - those
    /// have no formula and would need manual upkeep every year, which is out of scope here.
    /// Dates follow RA 9492 (regular holidays) and the usual special non-working day list;
    /// Holy Week dates are computed from Easter Sunday via the Anonymous Gregorian algorithm.
    /// </summary>
    internal static class PhilippineHolidays
    {
        /// <summary>Null if <paramref name="date"/> isn't a covered holiday.</summary>
        public static string GetHolidayName(DateTime date)
        {
            int month = date.Month;
            int day = date.Day;

            switch (month)
            {
                case 1 when day == 1:
                    return "New Year's Day";
                case 2 when day == 25:
                    return "EDSA People Power Anniversary";
                case 4 when day == 9:
                    return "Araw ng Kagitingan";
                case 5 when day == 1:
                    return "Labor Day";
                case 6 when day == 12:
                    return "Independence Day";
                case 8 when IsLastMondayOfAugust(date):
                    return "National Heroes Day";
                case 11 when day == 1:
                    return "All Saints' Day";
                case 11 when day == 2:
                    return "All Souls' Day";
                case 11 when day == 30:
                    return "Bonifacio Day";
                case 12 when day == 24:
                    return "Christmas Eve";
                case 12 when day == 25:
                    return "Christmas Day";
                case 12 when day == 30:
                    return "Rizal Day";
                case 12 when day == 31:
                    return "New Year's Eve";
            }

            DateTime easter = GetEasterSunday(date.Year);
            if (date.Date == easter.AddDays(-3))
                return "Maundy Thursday";
            if (date.Date == easter.AddDays(-2))
                return "Good Friday";
            if (date.Date == easter.AddDays(-1))
                return "Black Saturday";

            return null;
        }

        private static bool IsLastMondayOfAugust(DateTime date) =>
            date.DayOfWeek == DayOfWeek.Monday && date.Day > DateTime.DaysInMonth(date.Year, 8) - 7;

        // Anonymous Gregorian algorithm (Meeus/Jones/Butcher) - same formula used to derive
        // Holy Week's fixed offset from Easter Sunday every year without a lookup table.
        private static DateTime GetEasterSunday(int year)
        {
            int a = year % 19;
            int b = year / 100;
            int c = year % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * l) / 451;
            int month = (h + l - 7 * m + 114) / 31;
            int day = ((h + l - 7 * m + 114) % 31) + 1;
            return new DateTime(year, month, day);
        }
    }
}
