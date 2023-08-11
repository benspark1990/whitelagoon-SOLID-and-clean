using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteLagoon.Application.Services.SOLID.I.Interfaces
{
    public interface IDashboardService :
        IBookingChartData,
        IRevenueChartData,
        IRegisteredUserChartData,
        IMemberAndBookingChartData,
        IBookingPieChartData
    {
        // Additional methods that might be related to the dashboard can be declared here
    }
}
