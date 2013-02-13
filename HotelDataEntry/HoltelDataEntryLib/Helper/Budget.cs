using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelDataEntryLib.Helper
{
    public class Budget
    {
        public int HotelBudgetId { get; set; }
        public int BudgetId { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string CurrencyCode { get; set; }
    }
}
