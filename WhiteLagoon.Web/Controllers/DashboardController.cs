using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Web.Constants;
using WhiteLagoon.Application.Common.Enums;
using WhiteLagoon.Application.Common.Dtos;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboadService;

        public DashboardController(IDashboardService dashboadService)
        {
            _dashboadService = dashboadService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetTotalBookingsChartData()
        {
            RadialBarChartDto chartData = await _dashboadService.GetBookingsChartDataAsync();

            var data = ResultData(chartData);                            
            return Json(data);
        }
        public async Task<IActionResult> GetTotalRevenueChartData()
        {
            RadialBarChartDto chartData = await _dashboadService.GetRevenueChartDataAsync();

            var data = ResultData(chartData);
            return Json(data);
        }
        public async Task<IActionResult> GetRegisteredUserChartData()
        {
            RadialBarChartDto chartData = await _dashboadService.GetRegisteredUserChartDataAsync();

            var data = ResultData(chartData);
            return Json(data);
        }
        public async Task<IActionResult> GetMemberAndBookingChartData()
        {
            DashboardLineChartDto dashboardLineChartDto = await _dashboadService.GetMemberAndBookingChartDataAsync();

            // Retrieve your data and format it as needed
            List<DashboardLineChartData> chartDataList = new List<DashboardLineChartData>
            {
                new DashboardLineChartData { Name = DashboardConstants.NewMembers, Data = dashboardLineChartDto.NewMembers },
                new DashboardLineChartData { Name = DashboardConstants.NewBookings, Data = dashboardLineChartDto.NewBookings }
            };

            var data = new
            {
                series = chartDataList,
                categories = dashboardLineChartDto.Categories
            };

            // Manually serialize the data to JSON
            return Json(data);
        }
        public async Task<IActionResult> GetCustomerBookingsPieChartData()
        {
            // Retrieve data 
            var newCustomerBookings = _dashboadService.GetCustomerBookingsAsync(CustomerType.New);
            var returningCustomerBookings = _dashboadService.GetCustomerBookingsAsync(CustomerType.Returning);

            await Task.WhenAll(new Task[2] { newCustomerBookings, returningCustomerBookings });
           
            // Format data as needed
            var data = new
            {
                series = new decimal[2] { newCustomerBookings.Result, returningCustomerBookings.Result },
                labels = new string[2] { DashboardConstants.NewCustomer, DashboardConstants.ReturningCustomer }
            };

            // Manually serialize the data to JSON
            return Json(data);
        }
        private static object ResultData(RadialBarChartDto chartData)
        {
            double increaseDecreaseRatio = 100;
            bool hasIncreased = true;

            // Considering any non-zero count in current month as 100% increase.
            if (chartData.PreviousMonthTotal != 0)
            {
                increaseDecreaseRatio = Math.Round((chartData.CurrentMonthTotal - chartData.PreviousMonthTotal) / chartData.PreviousMonthTotal * 100, 2);
                hasIncreased = chartData.CurrentMonthTotal > chartData.PreviousMonthTotal;
            }

            return new
            {
                series = new double[] { increaseDecreaseRatio },
                totalCount = chartData.Total,
                increaseDecreaseRatio = increaseDecreaseRatio,
                hasRatioIncreased = hasIncreased,
                increaseDecreaseAmount = chartData.CurrentMonthTotal
            };
        }
    }
}

