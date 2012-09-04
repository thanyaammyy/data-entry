using System.Collections.Generic;
using System.Linq;

namespace HotelDataEntryLib.Page
{
    public static class DataEntryTypeHelper
    {
        public static IEnumerable<DataEntryType> ListDataEntryType()
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                var listRevenue = new List<DataEntryType> { new DataEntryType() { DataEntryTypeId = 0, DataEntryTypeName = "Select a revenue" } };
                listRevenue.AddRange(hdc.DataEntryTypes.ToList());
                return listRevenue;
            }
        }
    }
}
