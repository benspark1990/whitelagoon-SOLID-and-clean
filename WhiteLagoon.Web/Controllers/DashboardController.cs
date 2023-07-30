using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.SharedModels;

namespace WhiteLagoon.Web.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboadNotSolidService;

        public DashboardController(IDashboardService dashboadNotSolidService)
        {
            _dashboadNotSolidService = dashboadNotSolidService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetTotalBookingsChartData()
        {
            RadialBarChartVM dashboardRadialBarChartVM = await _dashboadNotSolidService.GetBookingsChartDataAsync();

            var data = ResultData(dashboardRadialBarChartVM);                            
            return Json(data);
        }
        public async Task<IActionResult> GetTotalRevenueChartData()
        {
            RadialBarChartVM dashboardRadialBarChartVM = await _dashboadNotSolidService.GetRevenueChartDataAsync();

            var data = ResultData(dashboardRadialBarChartVM);
            return Json(data);
        }
        public async Task<IActionResult> GetRegisteredUserChartData()
        {
            RadialBarChartVM dashboardRadialBarChartVM = await _dashboadNotSolidService.GetRegisteredUserChartDataAsync();

            var data = ResultData(dashboardRadialBarChartVM);
            return Json(data);
        }
        public async Task<IActionResult> GetMemberAndBookingChartData()
        {
            DashboardLineChartVM dashboardLineChartVM = await _dashboadNotSolidService.GetMemberAndBookingChartDataAsync();

            // Retrieve your data and format it as needed
            var data = new
            {
                series = dashboardLineChartVM.ChartData,
                categories = dashboardLineChartVM.Categories
            };

            // Manually serialize the data to JSON
            return Json(data);
        }
        public async Task<IActionResult> GetCustomerBookingsPieChartData()
        {
            DashboardPieChartVM dashboardPieChartVM = await _dashboadNotSolidService.GetBookingPieChartDataAsync();

            // Retrieve your data and format it as needed
            var data = new
            {
                series = dashboardPieChartVM.Series,
                labels = dashboardPieChartVM.Labels
            };

            // Manually serialize the data to JSON
            return Json(data);
        }
        private static object ResultData(RadialBarChartVM dashboardRadialBarChartVM)
        {
            return new
            {
                series = dashboardRadialBarChartVM.Series, //new int[] { 30 },
                totalCount = dashboardRadialBarChartVM.TotalCount,
                increaseDecreaseRatio = dashboardRadialBarChartVM.IncreaseDecreaseRatio,
                hasRatioIncreased = dashboardRadialBarChartVM.HasRatioIncreased,
                increaseDecreaseAmount = dashboardRadialBarChartVM.IncreaseDecreaseAmount
            };
        }
    }
}
