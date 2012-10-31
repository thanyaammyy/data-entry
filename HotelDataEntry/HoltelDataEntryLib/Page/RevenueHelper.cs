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
        public static void AddRevenueEntryListByMonthYear(HotelRevenue hotelEntry)
        {
            var dates = GetLastDayOfMonth(hotelEntry.Month,hotelEntry.Year);
            using (var hdc = new HotelDataEntryDataContext())
            {
                for (var i = 0; i < dates; i++)
                {
                    hdc.RevenueEntries.InsertOnSubmit(new RevenueEntry()
                        {
                            HotelRevenueId = hotelEntry.HotelRevenueId,
                            OccupiedRoom = 0,
                            TotalRoomRevenues = 0.00,
                            Food = 0.00,
                            Beverage = 0.00,
                            SpaProduct = 0.00,
                            Service = 0.00,
                            Others = 0.00,
                            Total = 0.00,
                            UpdateDateTime = DateTime.Now,
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
                         select new Revenue()
                                    {
                                        RevenueId = revenueEntry.RevenueId,
                                        PositionDate = revenueEntry.PositionDate,
                                        HotelRevenueId = revenueEntry.HotelRevenueId,
                                        OccupiedRoom = revenueEntry.OccupiedRoom,
                                        TotalRoomRevenues = revenueEntry.TotalRoomRevenues,
                                        Food = revenueEntry.Food,
                                        Beverage = revenueEntry.Beverage,
                                        Service = revenueEntry.Service,
                                        Spa = revenueEntry.SpaProduct,
                                        Others = revenueEntry.Others,
                                        Total = revenueEntry.Total,
                                        Budget = budgetEntry.Total/dates
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

                    entry.OccupiedRoom = revenueEntry.OccupiedRoom;
                    entry.TotalRoomRevenues = revenueEntry.TotalRoomRevenues;
                    entry.Food = revenueEntry.Food;
                    entry.Beverage = revenueEntry.Beverage;
                    entry.SpaProduct = revenueEntry.SpaProduct;
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
