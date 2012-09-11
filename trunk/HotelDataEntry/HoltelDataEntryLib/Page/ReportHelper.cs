using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace HotelDataEntryLib.Page
{
    public static class ReportHelper
    {
        public static object CalOccupiedRoom(DateTime dateFrom, DateTime dateTo, int propertyId)
        {
            
            var hdc = new HotelDataEntryDataContext();
            object dataEntryList = (from hotelEntry in hdc.HotelEntries
                                    join dataEntry in hdc.DataEntries on hotelEntry.HotelEntryId equals dataEntry.HotelEntryId
                                    join dataSubEntryType in hdc.DataEntrySubTypes on hotelEntry.DataEntrySubTypeId equals  dataSubEntryType.DataEntrySubTypeId
                                    join dataEntryType in hdc.DataEntryTypes on dataSubEntryType.DataEntryTypeId equals  dataEntryType.DataEntryTypeId
                                    where hotelEntry.DataEntrySubTypeId == 1
                                          && hotelEntry.PropertyId == propertyId
                                          && dataEntry.PositionDate >= dateFrom
                                          && dataEntry.PositionDate<=dateTo
                                    group dataEntry by new
                                    {
                                        dataEntry.ActualData,
                                        dataEntry.Budget,
                                        dataEntry.YTDActual,
                                        dataEntry.YTDBudget,
                                        dataSubEntryType.DataEntrySubTypeName,
                                        dataEntryType.DataEntryTypeName
                                    } into g
                                    select new 
                                    {
                                        g.Key.DataEntryTypeName,
                                        g.Key.DataEntrySubTypeName, 
                                        YTD = g.Sum(item=>item.ActualData),
                                        BudgetTY = g.Sum(item=>item.Budget)       
                                    });
            
            return dataEntryList;
        }
    }
}
