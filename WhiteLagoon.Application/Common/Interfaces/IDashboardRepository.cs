using WhiteLagoon.Domain.SharedModels;
using WhiteLagoon.Shared.Enums;

namespace WhiteLagoon.Application.Common.Interfaces
{
    public interface IDashboardRepository 
    {
        Task<RadialBarChartVM> GetBookingsChartDataAsync();
        Task<RadialBarChartVM> GetRevenueChartDataAsync();
        Task<RadialBarChartVM> GetRegisteredUserChartDataAsync();
        Task<DashboardLineChartVM> GetMemberAndBookingChartDataAsync();
        Task<int> GetCustomerBookingsAsync(CustomerType customerType); 
    }
}
