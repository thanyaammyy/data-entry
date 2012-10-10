using System;
using System.Collections.Generic;
using System.Linq;
using HotelDataEntryLib.Helper;

namespace HotelDataEntryLib.Page
{
    public static class ReportHelper
    {
        public static IEnumerable<Report> CalculateYearlyReport(DateTime dateFrom, DateTime dateTo, int propertyId)
        {
            
            var hdc = new HotelDataEntryDataContext();
            IEnumerable<Report> list = null;
            //list = (from hotelEntry in hdc.HotelEntries
            //                        join dataEntry in hdc.DataEntries on hotelEntry.HotelEntryId equals dataEntry.HotelEntryId
            //                        join dataSubEntryType in hdc.DataEntrySubTypes on hotelEntry.DataEntrySubTypeId equals  dataSubEntryType.DataEntrySubTypeId
            //                        join dataEntryType in hdc.DataEntryTypes on dataSubEntryType.DataEntryTypeId equals  dataEntryType.DataEntryTypeId
            //                        where hotelEntry.PropertyId == propertyId
            //                              && dataEntry.PositionDate >= dateFrom
            //                              && dataEntry.PositionDate<=dateTo
            //                        group new { dataEntry, dataSubEntryType, dataEntryType } by new
            //                        {
            //                            dataEntryType.DataEntryTypeName,
            //                            dataSubEntryType.DataEntrySubTypeName
            //                        }into g
            //                        orderby g.Key.DataEntryTypeName
            //                        select new Report()
            //                        {
            //                            Type = g.Key.DataEntryTypeName,
            //                            SubType = g.Key.DataEntrySubTypeName,
            //                            BudgetTY = g.Sum(item => item.dataEntry.Budget)  
            //                        }).ToList();
            return list;
        }

        public static IEnumerable<Report> CalculateMonthlyReport(DateTime dateFrom, DateTime dateTo, int propertyId)
        {

            var hdc = new HotelDataEntryDataContext();
            IEnumerable<Report> list = null;
            //list = (from hotelEntry in hdc.HotelEntries
            //        join dataEntry in hdc.DataEntries on hotelEntry.HotelEntryId equals dataEntry.HotelEntryId
            //        join dataSubEntryType in hdc.DataEntrySubTypes on hotelEntry.DataEntrySubTypeId equals dataSubEntryType.DataEntrySubTypeId
            //        join dataEntryType in hdc.DataEntryTypes on dataSubEntryType.DataEntryTypeId equals dataEntryType.DataEntryTypeId
            //        where hotelEntry.PropertyId == propertyId
            //              && dataEntry.PositionDate >= dateFrom
            //              && dataEntry.PositionDate <= dateTo
            //        group new { dataEntry, dataSubEntryType, dataEntryType } by new
            //        {
            //            dataEntryType.DataEntryTypeName,
            //            dataSubEntryType.DataEntrySubTypeName
            //        } into g
            //        orderby g.Key.DataEntryTypeName
            //        select new Report()
            //        {
            //            Type = g.Key.DataEntryTypeName,
            //            SubType = g.Key.DataEntrySubTypeName,
            //            Actual = g.Sum(item => item.dataEntry.ActualData)
            //        }).ToList();
            return list;
        }
    
    }
}
