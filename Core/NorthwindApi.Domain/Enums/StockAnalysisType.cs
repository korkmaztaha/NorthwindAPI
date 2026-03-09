using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Domain.Enums
{
    public enum StockAnalysisType
    {
        Critical,       // Minimum stok altına düşenler
        Excess,         // Uzun süredir satılmayanlar
        Turnover,       // Stok devir hızı
        Discontinued    // Discontinued ürünler
    }
}
