using MediatR;
using NorthwindApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetStockAnalysis
{
    public class GetStockAnalysisQuery : IRequest<GetStockAnalysisResponse>
    {
        public StockAnalysisType AnalysisType { get; set; } = StockAnalysisType.Critical;
        public int? DaysSinceLastSale { get; set; } = 180; 
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
