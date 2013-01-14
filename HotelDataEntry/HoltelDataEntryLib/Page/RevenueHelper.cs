using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using HotelDataEntryLib.Helper;

namespace HotelDataEntryLib.Page
{
    public static class RevenueHelper
    {
        public static void AddRevenueEntryListByMonthYear(HotelRevenue hotelEntry, string username)
        {
            var dates = GetLastDayOfMonth(hotelEntry.Month,hotelEntry.Year);
            using (var hdc = new HotelDataEntryDataContext())
            {
                for (var i = 0; i < dates; i++)
                {
                    hdc.RevenueEntries.InsertOnSubmit(new RevenueEntry()
                        {
                            HotelRevenueId = hotelEntry.HotelRevenueId,
                            OccupancyRoom = 0,
                            RoomRevenue = 0.00,
                            FBRevenue = 0.00,
                            SpaRevenue = 0.00,
                            Others = 0.00,
                            Total = 0.00,
                            UpdateDateTime = DateTime.Now,
                            UpdateUser = username,
                            PositionDate = new DateTime(hotelEntry.Year, hotelEntry.Month, (i + 1))

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

        public static List<HotelDataEntryLib.Helper.Revenue> ListRevenueEntryByMonthYear(HotelRevenue hotelEntry)
        {
            var dates = GetLastDayOfMonth(hotelEntry.Month, hotelEntry.Year);
            var positionMonth = hotelEntry.Month + "/" + hotelEntry.Year;
            var hdc = new HotelDataEntryDataContext();
            var list = (from revenueEntry in hdc.RevenueEntries
                         join hotelRevenue in hdc.HotelRevenues on revenueEntry.HotelRevenueId equals hotelRevenue.HotelRevenueId
                         join hotelBudget in hdc.HotelBudgets on new { hotelRevenue.Year, hotelRevenue.PropertyId } equals new { hotelBudget.Year, hotelBudget.PropertyId }
                         join budgetEntry in hdc.BudgetEntries on hotelBudget.HotelBudgetId equals budgetEntry.HotelBudgetId
                         where revenueEntry.HotelRevenueId == hotelEntry.HotelRevenueId
                               && budgetEntry.PositionMonth == positionMonth
                         orderby revenueEntry.PositionDate
                         select new Revenue()
                                    {
                                        RevenueId = revenueEntry.RevenueId,
                                        PositionDate = revenueEntry.PositionDate,
                                        HotelRevenueId = revenueEntry.HotelRevenueId,
                                        OccupancyRoom = revenueEntry.OccupancyRoom,
                                        RoomRevenue = revenueEntry.RoomRevenue,
                                        FBRevenue = revenueEntry.FBRevenue,
                                        SpaRevenue = revenueEntry.SpaRevenue,
                                        Others = revenueEntry.Others,
                                        Total = revenueEntry.Total,
                                        Budget = revenueEntry.Total<=0?0:budgetEntry.Total / dates,
                                        Day = revenueEntry.PositionDate.DayOfWeek.ToString()
                                    }).ToList();
            return list;
        }

        public static void UpdateRevenueEntry(RevenueEntry revenueEntry)
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                try
                {
                    var entry = hdc.RevenueEntries.Single(item => item.RevenueId == revenueEntry.RevenueId);
                    entry.OccupancyRoom = revenueEntry.OccupancyRoom;
                    entry.FBRevenue = revenueEntry.FBRevenue;
                    entry.SpaRevenue = revenueEntry.SpaRevenue;
                    entry.RoomRevenue = revenueEntry.RoomRevenue;
                    entry.Others = revenueEntry.Others;
                    entry.Total = revenueEntry.Total;
                    entry.UpdateDateTime = DateTime.Now;
                    entry.UpdateUser = revenueEntry.UpdateUser;
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

        public static List<RevenueEntry> GetRevenueEntry(int hotelRevenueId)
        {
            var hdc = new HotelDataEntryDataContext();
            return hdc.RevenueEntries.Where(item => item.HotelRevenueId == hotelRevenueId).ToList();
        }
    }
}
