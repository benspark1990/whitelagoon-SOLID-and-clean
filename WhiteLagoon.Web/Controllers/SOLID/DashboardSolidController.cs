using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Services.SOLID.I.Interfaces;
using WhiteLagoon.Domain.SharedModels;

namespace WhiteLagoon.Web.Controllers.SOLID
{
    public class DashboardSolidController : Controller
    {
        private readonly IDashboardService _dashboadCorrectSolidService;

        public DashboardSolidController(IDashboardService dashboadCorrectSolidService)
        {
            _dashboadCorrectSolidService = dashboadCorrectSolidService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetTotalBookingsChartData()
        {
            RadialBarChartVM dashboardRadialBarChartVM = await _dashboadCorrectSolidService.GetBookingsChartDataAsync();

            var data = ResultData(dashboardRadialBarChartVM);
            return Json(data);
        }
        public async Task<IActionResult> GetTotalRevenueChartData()
        {
            RadialBarChartVM dashboardRadialBarChartVM = await _dashboadCorrectSolidService.GetRevenueChartDataAsync();

            var data = ResultData(dashboardRadialBarChartVM);
            return Json(data);
        }
        public async Task<IActionResult> GetRegisteredUserChartData()
        {
            RadialBarChartVM dashboardRadialBarChartVM = await _dashboadCorrectSolidService.GetRegisteredUserChartDataAsync();

            var data = ResultData(dashboardRadialBarChartVM);
            return Json(data);
        }
        public async Task<IActionResult> GetMemberAndBookingChartData()
        {
            DashboardLineChartVM dashboardLineChartVM = await _dashboadCorrectSolidService.GetMemberAndBookingChartDataAsync();

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
            DashboardPieChartVM dashboardPieChartVM = await _dashboadCorrectSolidService.GetBookingPieChartDataAsync();

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
