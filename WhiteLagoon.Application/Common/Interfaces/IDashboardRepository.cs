using WhiteLagoon.Application.Common.Dtos;
using WhiteLagoon.Application.Common.Enums;

namespace WhiteLagoon.Application.Common.Interfaces
{
    public interface IDashboardRepository 
    {
        Task<RadialBarChartDto> GetBookingsChartDataAsync();
        Task<RadialBarChartDto> GetRevenueChartDataAsync();
        Task<RadialBarChartDto> GetRegisteredUserChartDataAsync();
        Task<DashboardLineChartDto> GetMemberAndBookingChartDataAsync();
        Task<int> GetCustomerBookingsAsync(CustomerType customerType); 
    }
}
