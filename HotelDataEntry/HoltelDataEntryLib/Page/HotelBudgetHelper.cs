using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace HotelDataEntryLib.Page
{
    public static class HotelBudgetHelper
    {
        public static bool ExistYear(HotelBudget hotelEntry)
        {
            var hdc = new HotelDataEntryDataContext();
            var count = hdc.HotelBudgets.Count(item => item.Year==hotelEntry.Year && item.PropertyId == hotelEntry.PropertyId);
            return count != 0;
        }


        public static HotelBudget GetHotelEntry(HotelBudget hotelEntry)
        {
            var hdc = new HotelDataEntryDataContext();
            var hEntry = hdc.HotelBudgets.Single(item => item.Year == hotelEntry.Year && item.PropertyId == hotelEntry.PropertyId);
            return hEntry;
        }

        public static HotelDataEntryLib.HotelBudget AddHotelEntryListByYear(HotelBudget hotelEntry)
        {
            HotelBudget hotelEntrySubmit;
            using (var hdc = new HotelDataEntryDataContext())
            {
                hotelEntrySubmit = new HotelBudget()
                                           {
                                               PropertyId = hotelEntry.PropertyId,
                                               Year =hotelEntry.Year,
                                               UpdateDateTime = DateTime.Now
                                           };
                hdc.HotelBudgets.InsertOnSubmit(hotelEntrySubmit);

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
            return hotelEntrySubmit;
        }
    }
}
