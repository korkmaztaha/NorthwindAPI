using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Common.BusinessRules
{
    public static class RFMSegmentCalculator
    {
        public static string DetermineSegment(int r, int f, int m)
        {
            if (r >= 4 && f >= 4 && m >= 4) return "Champions";
            if (r >= 3 && f >= 3) return "Loyal";
            if (r <= 2 && f >= 3) return "AtRisk";
            if (r == 1 && f <= 2) return "Lost";
            if (r >= 4 && f == 1) return "NewCustomers";
            return "Others";
        }
    }
}
