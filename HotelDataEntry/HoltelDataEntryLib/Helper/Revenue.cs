using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelDataEntryLib.Helper
{
    public class Revenue
    {
        public int RevenueId { get; set; }
        public DateTime? PositionDate { get; set; }
        public int HotelRevenueId{ get; set; }
        public double OccupancyRoom{ get; set; }
        public double RoomRevenue { get; set; }
        public double FBRevenue { get; set; }
        public double SpaRevenue { get; set; }
        public double Others { get; set; }
        public double Total { get; set; }
        public double Budget { get; set; }
        public DateTime UpdateDateTime { get; set; }
    }
}
