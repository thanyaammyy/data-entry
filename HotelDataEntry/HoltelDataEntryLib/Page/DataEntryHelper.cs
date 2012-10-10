using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace HotelDataEntryLib.Page
{
    public static class DataEntryHelper
    {
        //public static void AddDataEntryListByMonthYear(HotelEntry hotelEntry)
        //{
        //    var monthYear = hotelEntry.MonthYear;
        //    var str = monthYear.Split('/');
        //    if(string.IsNullOrEmpty(str[0])||string.IsNullOrEmpty(str[1])) return;
        //    var dates = GetLastDayOfMonth((Convert.ToInt32(str[0])), Convert.ToInt32(str[1]));
        //    using (var hdc = new HotelDataEntryDataContext())
        //    {
        //        var month = Convert.ToInt32(str[0]);
        //        var year = Convert.ToInt32(str[1]);
        //        for (var i = 0; i < dates; i++)
        //        {                  
        //            hdc.DataEntries.InsertOnSubmit(new DataEntry()
        //                {
        //                    HotelEntryId = hotelEntry.HotelEntryId,
        //                    ActualData = 0.00,
        //                    Budget = 0.00,
        //                    UpdateDateTime = DateTime.Now,
        //                    PositionDate = new DateTime(year, month,(i+1))

        //                });

        //            try
        //            {
        //                hdc.SubmitChanges();
        //            }
        //            catch (SqlException ex)
        //            {
        //                if (ex.Number == 2601 || ex.Number == 2627)
        //                {
        //                    throw;
        //                }
        //            }
        //        }
        //    }

        //}

        public static int GetLastDayOfMonth(int month, int year)
        {
            var n = new DateTime(year, month, 1);
            var dates = n.AddMonths(1).AddDays(-1).Day;
            return dates;
        }

        //public static List<DataEntry> ListDataEntryByMonthYear(HotelEntry hotelEntry)
        //{
        //    var hdc = new HotelDataEntryDataContext();
        //    var dataEntryList = hdc.DataEntries.Where(item=>item.HotelEntryId==hotelEntry.HotelEntryId).ToList();
        //    return dataEntryList;
        //}

        //public static void UpdateDataEntry(DataEntry dataEntry)
        //{
        //    using (var hdc = new HotelDataEntryDataContext())
        //    {
        //        var entry = hdc.DataEntries.Single(item => item.DataEntryId == dataEntry.DataEntryId);

        //        entry.ActualData = dataEntry.ActualData;
        //        entry.Budget = dataEntry.Budget;
        //        entry.UpdateDateTime = DateTime.Now;

        //        try
        //        {
        //            hdc.SubmitChanges();
        //        }
        //        catch (SqlException ex)
        //        {
        //            if (ex.Number == 2601 || ex.Number == 2627)
        //            {
        //                throw;
        //            }
        //        }
        //    }
        //}
    }
}
