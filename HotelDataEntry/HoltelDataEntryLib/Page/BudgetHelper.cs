using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace HotelDataEntryLib.Page
{
     public static class BudgetHelper
    {
         public static void AddBudgetEntryListByYear(HotelDataEntry hotelEntry)
         {
             using (var hdc = new HotelDataEntryDataContext())
             {
                 for (var i = 0; i < 12; i++)
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
                         PositionMonth = (i+1)+"/"+hotelEntry.Year

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
         public static List<BudgetEntry> ListBudgetEntryByYear(HotelDataEntry hotelEntry)
         {
             var hdc = new HotelDataEntryDataContext();
             var revenueEntryList = hdc.BudgetEntries.Where(item => item.HotelEntryId == hotelEntry.HotelEntryId).ToList();
             return revenueEntryList;
         }
         public static void UpdateBudgetEntry(BudgetEntry budgetEntry)
         {
             using (var hdc = new HotelDataEntryDataContext())
             {
                 try
                 {
                     var entry = hdc.BudgetEntries.Single(item => item.BudgetId == budgetEntry.BudgetId);

                     entry.OccupiedRoom = budgetEntry.OccupiedRoom;
                     entry.TotalRoomRevenues = budgetEntry.TotalRoomRevenues;
                     entry.Food = budgetEntry.Food;
                     entry.Beverage = budgetEntry.Beverage;
                     entry.Spa = budgetEntry.Spa;
                     entry.Service = budgetEntry.Service;
                     entry.Others = budgetEntry.Others;
                     entry.Total = budgetEntry.Total;
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
