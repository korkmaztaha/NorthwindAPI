using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Domain.Constants
{
    public static class CacheKeys
    {
        public const string Customers = "customers";
        public const string CustomerById = "customer_{0}"; 
    }
}
