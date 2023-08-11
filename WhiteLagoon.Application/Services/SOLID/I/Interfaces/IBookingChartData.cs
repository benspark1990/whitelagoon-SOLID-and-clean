using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.SharedModels;

namespace WhiteLagoon.Application.Services.SOLID.I.Interfaces
{
    public interface IBookingChartData
    {
        Task<RadialBarChartVM> GetBookingsChartDataAsync();
    }
}
