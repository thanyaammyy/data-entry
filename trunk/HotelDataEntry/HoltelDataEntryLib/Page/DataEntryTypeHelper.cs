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

        public static IEnumerable<DataEntrySubType> ListDataEntrySubType(int dataEntryTypeId)
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                var listSubRevenue = new List<DataEntrySubType> { new DataEntrySubType() { DataEntrySubTypeId = 0, DataEntrySubTypeName = "select..." } };
                listSubRevenue.AddRange(hdc.DataEntrySubTypes.Where(item=>item.DataEntryTypeId==dataEntryTypeId));
                return listSubRevenue;
            }
        }
    }
}
