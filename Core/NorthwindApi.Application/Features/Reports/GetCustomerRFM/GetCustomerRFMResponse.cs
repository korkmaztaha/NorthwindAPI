using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetCustomerRFM
{
    //R - Recency    → Son sipariş kaç gün önce? (düşük = iyi)
    //F - Frequency  → Toplam kaç sipariş verdi? (yüksek = iyi)
    //M - Monetary   → Toplam ne kadar harcadı? (yüksek = iyi)

    //Champions       → R:5 F:5 M:5  - En değerli müşteriler
    //Loyal           → R:4-5 F:3-5  - Sadık müşteriler
    //At Risk         → R:2-3 F:3-5  - Kaybedilmek üzere
    //Lost            → R:1 F:1-2    - Kayıp müşteriler
    //New Customers   → R:5 F:1      - Yeni müşteriler
    public class GetCustomerRFMResult
    {
        public List<GetCustomerRFMResponse> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public RFMSummary Summary { get; set; } = null!;
    }

    public class GetCustomerRFMResponse
    {
        public string CustomerId { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string? Country { get; set; }
        public string? City { get; set; }

       
        public int DaysSinceLastOrder { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }

        
        public int RecencyScore { get; set; }
        public int FrequencyScore { get; set; }
        public int MonetaryScore { get; set; }
        public int RFMScore { get; set; }// R+F+M toplamı (3-15)

       
        public string Segment { get; set; } = null!;
    }

    public class RFMSummary
    {
        public int TotalCustomers { get; set; }
        public int Champions { get; set; }
        public int Loyal { get; set; }
        public int AtRisk { get; set; }
        public int Lost { get; set; }
        public int NewCustomers { get; set; }
        public int Others { get; set; }
    }

}
