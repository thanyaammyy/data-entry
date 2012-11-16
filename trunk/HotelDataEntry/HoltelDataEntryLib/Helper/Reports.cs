using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelDataEntryLib.Helper
{
   public class Reports
    {
       public int BudgetId { get; set; }
       public string MonthYear { get; set; }
       public double OccupancyRoomBudget { get; set; }
       public double OccupancyRoomActual { get; set; }
       public double FBBudget { get; set; }
       public double FBActual { get; set; }
       public double RoomBudget { get; set; }
       public double RoomActual { get; set; }
       public double SpaBudget { get; set; }
       public double SpaActual { get; set; }
       public double OtherBudget { get; set; }
       public double OtherActual { get; set; }
    }
}
