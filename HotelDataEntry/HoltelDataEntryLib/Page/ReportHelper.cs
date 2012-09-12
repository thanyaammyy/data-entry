using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace HotelDataEntryLib.Page
{
    public static class ReportHelper
    {
        public static object CalculateYearlyReport(DateTime dateFrom, DateTime dateTo, int propertyId)
        {
            
            var hdc = new HotelDataEntryDataContext();
            object dataEntryList = (from hotelEntry in hdc.HotelEntries
                                    join dataEntry in hdc.DataEntries on hotelEntry.HotelEntryId equals dataEntry.HotelEntryId
                                    join dataSubEntryType in hdc.DataEntrySubTypes on hotelEntry.DataEntrySubTypeId equals  dataSubEntryType.DataEntrySubTypeId
                                    join dataEntryType in hdc.DataEntryTypes on dataSubEntryType.DataEntryTypeId equals  dataEntryType.DataEntryTypeId
                                    where hotelEntry.PropertyId == propertyId
                                          && dataEntry.PositionDate >= dateFrom
                                          && dataEntry.PositionDate<=dateTo
                                    group new { dataEntry, dataSubEntryType, dataEntryType } by new
                                    {
                                        dataSubEntryType.DataEntrySubTypeName,
                                        dataEntryType.DataEntryTypeName,
                                    }into g
                                    orderby g.Key.DataEntryTypeName
                                    select new 
                                    {
                                        Type = g.Key.DataEntryTypeName,
                                        SubType = g.Key.DataEntrySubTypeName,
                                        BudgetTY = g.Sum(item=>item.dataEntry.Budget)  
                                    });           
            return dataEntryList;
        }
    }
}
