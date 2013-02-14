using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelDataEntryLib.Helper
{
    public class HotelRevenue
    {
        public int HotelRevenueId { get; set; }
        public int RevenueId { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string CurrencyCode { get; set; }
    }
}
