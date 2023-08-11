using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.SOLID.I.Interfaces;
using WhiteLagoon.Domain.SharedModels;

namespace WhiteLagoon.Application.Services.SOLID.I.Implementations
{
    public class DashboardService : IDashboardService
    {
        /// <summary>
        /// Applaying the Interface Segregation Principle (ISP) to the DashboardService class by breaking down its methods into smaller, more focused interfaces. 
        /// This will help ensure that implementing classes are not forced to implement methods they don't need
        /// By splitting the original interface IDashboardService into smaller interfaces based on specific functionalities,
        /// we adhere to the Interface Segregation Principle. Now, implementing classes can choose to implement only the relevant interfaces they need,
        /// avoiding unnecessary dependencies on methods they don't use. This promotes a more flexible and maintainable design for the dashboard-related services.
        /// </summary>

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
