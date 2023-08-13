using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.SharedModels;
using WhiteLagoon.Application.Common.Enums;
using WhiteLagoon.Application.Common.Dtos;

namespace WhiteLagoon.Application.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RadialBarChartDto> GetBookingsChartDataAsync()
        {
            RadialBarChartDto chartData = await _unitOfWork.Dashboard.GetBookingsChartDataAsync();
        
            return chartData;
        }

        public async Task<RadialBarChartDto> GetRevenueChartDataAsync()
        {
            RadialBarChartDto chartData = await _unitOfWork.Dashboard.GetRevenueChartDataAsync();
        
            return chartData;
        }

        public async Task<RadialBarChartDto> GetRegisteredUserChartDataAsync()
        {
            RadialBarChartDto chartData = await _unitOfWork.Dashboard.GetRegisteredUserChartDataAsync();
        
            return chartData;
        }

        public async Task<DashboardLineChartVM> GetMemberAndBookingChartDataAsync()
        {
            DashboardLineChartVM dashboardLineChartVM = await _unitOfWork.Dashboard.GetMemberAndBookingChartDataAsync();
        
            return dashboardLineChartVM;
        }

        public async Task<int> GetCustomerBookingsAsync(CustomerType customerType)
        {
            int bookingsCount = await _unitOfWork.Dashboard.GetCustomerBookingsAsync(customerType);

            return bookingsCount;
        }
    }
}
