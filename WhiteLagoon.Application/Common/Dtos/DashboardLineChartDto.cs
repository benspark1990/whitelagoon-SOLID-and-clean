using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteLagoon.Application.Common.Dtos
{
    public class DashboardLineChartDto
    {
        public IEnumerable<int> NewMembers { get; set; }
        public IEnumerable<int> NewBookings { get; set; }
        public IEnumerable<string> Categories { get; set; }

        public DashboardLineChartDto(IEnumerable<int> newMember, IEnumerable<int> newBookings, IEnumerable<string> categories) 
        {
            NewMembers = newMember;
            NewBookings = newBookings;
            Categories = categories;
        }
    }
}
