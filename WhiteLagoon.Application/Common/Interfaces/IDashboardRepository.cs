using WhiteLagoon.Application.Common.Dtos;
using WhiteLagoon.Application.Common.Enums;
using WhiteLagoon.Domain.SharedModels;

namespace WhiteLagoon.Application.Common.Interfaces
{
    public interface IDashboardRepository 
    {
        Task<RadialBarChartDto> GetBookingsChartDataAsync();
        Task<RadialBarChartDto> GetRevenueChartDataAsync();
        Task<RadialBarChartDto> GetRegisteredUserChartDataAsync();
        Task<DashboardLineChartVM> GetMemberAndBookingChartDataAsync();
        Task<int> GetCustomerBookingsAsync(CustomerType customerType); 
    }
}
