using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.SharedModels;

namespace WhiteLagoon.Application.Services.Interfaces
{
    public interface IDashboardService
    {
        public Task<RadialBarChartVM> GetBookingsChartDataAsync();
        public Task<RadialBarChartVM> GetRevenueChartDataAsync();
        public Task<RadialBarChartVM> GetRegisteredUserChartDataAsync();
        public Task<DashboardLineChartVM> GetMemberAndBookingChartDataAsync();
        public Task<DashboardPieChartVM> GetBookingPieChartDataAsync();
    }
}
