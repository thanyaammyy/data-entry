using System;
using System.Data.SqlClient;
using System.Linq;

namespace HotelDataEntryLib.Page
{
    public static class HotelEntryHelper
    {
        public static bool ExistMothYear(HotelDataEntry hotelEntry)
        {
            var hdc = new HotelDataEntryDataContext();
            var count = hdc.HotelDataEntries.Count(item => item.Month == hotelEntry.Month &&item.Year==hotelEntry.Year && item.PropertyId == hotelEntry.PropertyId);
            return count != 0;
        }

        public static bool ExistYear(HotelDataEntry hotelEntry)
        {
            var hdc = new HotelDataEntryDataContext();
            var count = hdc.HotelDataEntries.Count(item=> item.Year == hotelEntry.Year && item.PropertyId == hotelEntry.PropertyId );
            return count != 0;
        }

        public static HotelDataEntry GetHotelEntry(HotelDataEntry hotelEntry)
        {
            var hdc = new HotelDataEntryDataContext();
            var hEntry = hdc.HotelDataEntries.Single(item => item.Month == hotelEntry.Month &&item.Year==hotelEntry.Year && item.PropertyId == hotelEntry.PropertyId );
            return hEntry;
        }

        public static HotelDataEntry GetHotelEntryByYear(HotelDataEntry hotelEntry)
        {
            var hdc = new HotelDataEntryDataContext();
            var hEntry = hdc.HotelDataEntries.Single(item => item.Year == hotelEntry.Year && item.PropertyId == hotelEntry.PropertyId);
            return hEntry;
        }

        public static HotelDataEntryLib.HotelDataEntry AddHotelEntryListByMonthYear(HotelDataEntry hotelEntry)
        {
            HotelDataEntry hotelEntrySubmit;
            using (var hdc = new HotelDataEntryDataContext())
            {
                hotelEntrySubmit = new HotelDataEntry()
                                           {
                                               PropertyId = hotelEntry.PropertyId,
                                               Month = hotelEntry.Month,
                                               Year =hotelEntry.Year,
                                               UpdateDateTime = DateTime.Now
                                           };
                hdc.HotelDataEntries.InsertOnSubmit(hotelEntrySubmit);

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
