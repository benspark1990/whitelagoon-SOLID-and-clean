using WhiteLagoon.Domain.SharedModels;
using WhiteLagoon.Shared.Enums;

namespace WhiteLagoon.Application.Services.Interfaces
{
    public interface IDashboardService
    {
        public Task<RadialBarChartVM> GetBookingsChartDataAsync();
        public Task<RadialBarChartVM> GetRevenueChartDataAsync();
        public Task<RadialBarChartVM> GetRegisteredUserChartDataAsync();
        public Task<DashboardLineChartVM> GetMemberAndBookingChartDataAsync();
        public Task<int> GetCustomerBookingsAsync(CustomerType customerType);
    }
}
