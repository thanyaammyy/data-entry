using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace HotelDataEntryLib.Page
{
    public static class RevenueHelper
    {
        public static void AddRevenueEntryListByMonthYear(HotelDataEntry hotelEntry)
        {
            var monthYear = hotelEntry.MonthYear;
            var str = monthYear.Split('/');
            if (string.IsNullOrEmpty(str[0]) || string.IsNullOrEmpty(str[1])) return;
            var dates = GetLastDayOfMonth((Convert.ToInt32(str[0])), Convert.ToInt32(str[1]));
            using (var hdc = new HotelDataEntryDataContext())
            {
                var month = Convert.ToInt32(str[0]);
                var year = Convert.ToInt32(str[1]);
                for (var i = 0; i < dates; i++)
                {
                    hdc.RevenueEntries.InsertOnSubmit(new RevenueEntry()
                        {
                            HotelEntryId = hotelEntry.HotelEntryId,
                            OccupiedRoom = 0.00,
                            TotalRoomRevenues = 0.00,
                            Food = 0.00,
                            Beverage = 0.00,
                            Spa = 0.00,
                            Service = 0.00,
                            Others = 0.00,
                            Total = 0.00,
                            UpdateDateTime = DateTime.Now,
                            PositionDate = new DateTime(year, month, (i + 1))

                        });

                    try
                    {
                        hdc.SubmitChanges();
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2601 || ex.Number == 2627)
                        {
                            throw;
                        }
                    }
                }
            }

        }

        public static int GetLastDayOfMonth(int month, int year)
        {
            var n = new DateTime(year, month, 1);
            var dates = n.AddMonths(1).AddDays(-1).Day;
            return dates;
        }

        public static List<RevenueEntry> ListRevenueEntryByMonthYear(HotelDataEntry hotelEntry)
        {
            var hdc = new HotelDataEntryDataContext();
            var revenueEntryList = hdc.RevenueEntries.Where(item => item.HotelEntryId == hotelEntry.HotelEntryId).ToList();
            return revenueEntryList;
        }

        public static void UpdateDataEntry(RevenueEntry revenueEntry)
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                try
                {
                    var entry = hdc.RevenueEntries.Single(item => item.RevenueId == revenueEntry.RevenueId);

                    entry.OccupiedRoom = revenueEntry.OccupiedRoom;
                    entry.TotalRoomRevenues = revenueEntry.TotalRoomRevenues;
                    entry.Food = revenueEntry.Food;
                    entry.Beverage = revenueEntry.Beverage;
                    entry.Spa = revenueEntry.Spa;
                    entry.Service = revenueEntry.Service;
                    entry.Others = revenueEntry.Others;
                    entry.Total = revenueEntry.Total;
                    entry.UpdateDateTime = DateTime.Now;

                
                    hdc.SubmitChanges();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2601 || ex.Number == 2627)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
