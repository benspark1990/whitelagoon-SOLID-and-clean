using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.SharedModels;
using WhiteLagoon.Shared.Enums;

namespace WhiteLagoon.Application.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RadialBarChartVM> GetBookingsChartDataAsync()
        {
            RadialBarChartVM dashboardRadialBarChartVM = await _unitOfWork.Dashboard.GetBookingsChartDataAsync();
        
            return dashboardRadialBarChartVM;
        }

        public async Task<RadialBarChartVM> GetRevenueChartDataAsync()
        {
            RadialBarChartVM dashboardRadialBarChartVM = await _unitOfWork.Dashboard.GetBookingsChartDataAsync();
        
            return dashboardRadialBarChartVM;
        }

        public async Task<RadialBarChartVM> GetRegisteredUserChartDataAsync()
        {
            RadialBarChartVM dashboardRadialBarChartVM = await _unitOfWork.Dashboard.GetRegisteredUserChartDataAsync();
        
            return dashboardRadialBarChartVM;
        }

        public async Task<DashboardLineChartVM> GetMemberAndBookingChartDataAsync()
        {
            DashboardLineChartVM dashboardLineChartVM = await _unitOfWork.Dashboard.GetMemberAndBookingChartDataAsync();
        
            return dashboardLineChartVM;
        }

        public async Task<int> GetCustomerBookingsAsync(CustomerType customerType)
        {
            int bookingCount = await _unitOfWork.Dashboard.GetCustomerBookingsAsync(customerType);

            return bookingCount;
        }
    }
}
