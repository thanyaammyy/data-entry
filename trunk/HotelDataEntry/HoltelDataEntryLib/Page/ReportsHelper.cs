using System;
using System.Collections.Generic;
using System.Linq;
using HotelDataEntryLib.Helper;

namespace HotelDataEntryLib.Page
{
    public static class ReportsHelper
    {
        public static List<Reports> BudgetReport(HotelBudget hBudget)
        {
            var hdc = new HotelDataEntryDataContext();
            List<Reports> list = null;
            list = (from hotelBudget in hdc.HotelBudgets
                    join budgetEntry in hdc.BudgetEntries on hotelBudget.HotelBudgetId equals budgetEntry.HotelBudgetId
                    where hotelBudget.PropertyId == hBudget.PropertyId && hotelBudget.Year == hBudget.Year
                    orderby budgetEntry.BudgetId
                    select new Reports()
                               {
                                   BudgetId = budgetEntry.BudgetId,
                                   MonthYear = budgetEntry.PositionMonth,
                                   OccupancyRoomBudget = budgetEntry.OccupancyRoom,
                                   FBBudget = budgetEntry.FBBudget,
                                   SpaBudget = budgetEntry.SpaBudget,
                                   RoomBudget = budgetEntry.RoomBudget,
                                   OtherBudget = budgetEntry.Others,
                                   OccupancyRoomActual = 0,
                                   FBActual = 0.00,
                                   RoomActual = 0.00,
                                   SpaActual = 0.00,
                                   OtherActual = 0.00
                               }).ToList();
            return list;
        }
    }
}
