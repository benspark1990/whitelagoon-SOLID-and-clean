using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteLagoon.Application.Common.Dtos
{
    public class RadialBarChartDto
    {
        public double Total { get; set; }
        public double PreviousMonthTotal { get; set; }
        public double CurrentMonthTotal { get; set; }
        public RadialBarChartDto(double total, double currentMonthTotal, double previousMonthTotal) 
        {
            Total = total;
            CurrentMonthTotal = currentMonthTotal;
            PreviousMonthTotal = previousMonthTotal;
        }
    }
}
