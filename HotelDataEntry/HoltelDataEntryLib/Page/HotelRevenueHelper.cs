using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace HotelDataEntryLib.Page
{
    public static class HotelRevenueHelper
    {
        public static bool ExistMothYear(HotelRevenue hotelEntry)
        {
            var hdc = new HotelDataEntryDataContext();
            var count = hdc.HotelRevenues.Count(item => item.Month == hotelEntry.Month &&item.Year==hotelEntry.Year && item.PropertyId == hotelEntry.PropertyId);
            return count != 0;
        }


        public static HotelRevenue GetHotelEntry(HotelRevenue hotelEntry)
        {
            var hdc = new HotelDataEntryDataContext();
            var hEntry = hdc.HotelRevenues.Single(item => item.Month == hotelEntry.Month && item.Year == hotelEntry.Year && item.PropertyId == hotelEntry.PropertyId);
            return hEntry;
        }

        public static List<HotelRevenue> GetHotelRevenueList(HotelBudget hotelBudget)
        {
            var hdc = new HotelDataEntryDataContext();
            return
                hdc.HotelRevenues.Where(
                    item => item.Year == hotelBudget.Year && item.PropertyId == hotelBudget.PropertyId).ToList();
        }

        public static HotelDataEntryLib.HotelRevenue AddHotelEntryListByMonthYear(HotelRevenue hotelEntry)
        {
            HotelRevenue hotelEntrySubmit;
            using (var hdc = new HotelDataEntryDataContext())
            {
                hotelEntrySubmit = new HotelRevenue()
                                           {
                                               PropertyId = hotelEntry.PropertyId,
                                               Month = hotelEntry.Month,
                                               Year =hotelEntry.Year,
                                               UpdateDateTime = DateTime.Now
                                           };
                hdc.HotelRevenues.InsertOnSubmit(hotelEntrySubmit);

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
