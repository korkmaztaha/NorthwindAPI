using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Common.Helpers
{
    public static class DateHelper
    {
        public static string GetMonthName(int year, int month) =>
            new DateTime(year, month, 1)
                .ToString("MMMM", new CultureInfo("tr-TR"));
    }
}
