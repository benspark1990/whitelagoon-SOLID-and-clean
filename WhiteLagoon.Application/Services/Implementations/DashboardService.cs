using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.SharedModels;

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

        public async Task<DashboardPieChartVM> GetBookingPieChartDataAsync()
        {
            DashboardPieChartVM dashboardPieChartVM = await _unitOfWork.Dashboard.GetBookingPieChartDataAsync();
        
            return dashboardPieChartVM;
        }
    }
}
