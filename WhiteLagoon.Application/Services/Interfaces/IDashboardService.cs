using WhiteLagoon.Application.Common.Enums;
using WhiteLagoon.Application.Common.Dtos;

namespace WhiteLagoon.Application.Services.Interfaces
{
    public interface IDashboardService
    {
        public Task<RadialBarChartDto> GetBookingsChartDataAsync();
        public Task<RadialBarChartDto> GetRevenueChartDataAsync();
        public Task<RadialBarChartDto> GetRegisteredUserChartDataAsync();
        public Task<DashboardLineChartDto> GetMemberAndBookingChartDataAsync();
        public Task<int> GetCustomerBookingsAsync(CustomerType customerType);
    }
}
