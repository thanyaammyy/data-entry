using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace HotelDataEntryLib.Page
{
     public static class BudgetHelper
    {
         public static void AddBudgetEntryListByMonthYear(HotelDataEntry hotelEntry)
         {
             var year = hotelEntry.MonthYear;
             if (string.IsNullOrEmpty(year)) return;
             using (var hdc = new HotelDataEntryDataContext())
             {
                 for (var i = 0; i < 11; i++)
                 {
                     hdc.BudgetEntries.InsertOnSubmit(new BudgetEntry()
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
                         PositionMonth = (i+1)+"/"+year

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
         public static List<BudgetEntry> ListBudgetEntryByMonthYear(HotelDataEntry hotelEntry)
         {
             var hdc = new HotelDataEntryDataContext();
             var revenueEntryList = hdc.BudgetEntries.Where(item => item.HotelEntryId == hotelEntry.HotelEntryId).ToList();
             return revenueEntryList;
         }
         public static void UpdateBudgetEntry(RevenueEntry revenueEntry)
         {
             using (var hdc = new HotelDataEntryDataContext())
             {
                 try
                 {
                     var entry = hdc.BudgetEntries.Single(item => item.BudgetId == revenueEntry.RevenueId);

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
