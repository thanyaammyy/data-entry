using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace HotelDataEntryLib.Page
{
     public static class BudgetHelper
    {
         public static void AddBudgetEntryListByYear(HotelBudget hotelEntry, string username)
         {
             using (var hdc = new HotelDataEntryDataContext())
             {
                 for (var i = 0; i < 12; i++)
                 {
                     hdc.BudgetEntries.InsertOnSubmit(new BudgetEntry()
                     {
                         HotelBudgetId = hotelEntry.HotelBudgetId,
                         OccupancyRoom = 0,
                         RoomBudget = 0.00,
                         FBBudget = 0.00,
                         SpaBudget = 0.00,
                         Others = 0.00,
                         Total = 0.00,
                         UpdateDateTime = DateTime.Now,
                         UpdateUser = username,
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
         public static List<BudgetEntry> ListBudgetEntryByYear(HotelBudget hotelEntry)
         {
             var hdc = new HotelDataEntryDataContext();
             var revenueEntryList = hdc.BudgetEntries.Where(item => item.HotelBudgetId == hotelEntry.HotelBudgetId).ToList();
             return revenueEntryList;
         }
         public static void UpdateBudgetEntry(BudgetEntry budgetEntry)
         {
             using (var hdc = new HotelDataEntryDataContext())
             {
                 try
                 {
                     var entry = hdc.BudgetEntries.Single(item => item.BudgetId == budgetEntry.BudgetId);
                     entry.OccupancyRoom = budgetEntry.OccupancyRoom;
                     entry.RoomBudget = budgetEntry.RoomBudget;
                     entry.FBBudget = budgetEntry.FBBudget;
                     entry.SpaBudget = budgetEntry.SpaBudget;
                     entry.Others = budgetEntry.Others;
                     entry.Total = budgetEntry.Total;
                     entry.UpdateDateTime = DateTime.Now;
                     entry.UpdateUser = budgetEntry.UpdateUser;
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
