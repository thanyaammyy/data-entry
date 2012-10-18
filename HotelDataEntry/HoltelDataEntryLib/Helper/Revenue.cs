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
        public double OccupiedRoom{ get; set; }
        public double TotalRoomRevenues { get; set; }
        public double Food { get; set; }
        public double Beverage { get; set; }
        public double Service { get; set; }
        public double Spa { get; set; }
        public double Others { get; set; }
        public double Total { get; set; }
        public double Budget { get; set; }
        public DateTime UpdateDateTime { get; set; }
    }
}
