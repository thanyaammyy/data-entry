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
            var count = hdc.HotelDataEntries.Count(item => item.MonthYear == hotelEntry.MonthYear && item.PropertyId == hotelEntry.PropertyId && item.EntryType==hotelEntry.EntryType);
            return count != 0;
        }

        public static HotelDataEntry GetHotelEntry(HotelDataEntry hotelEntry)
        {
            var hdc = new HotelDataEntryDataContext();
            var hEntry = hdc.HotelDataEntries.Single(item => item.MonthYear == hotelEntry.MonthYear && item.PropertyId == hotelEntry.PropertyId && item.EntryType == hotelEntry.EntryType);
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
                                               EntryType = hotelEntry.EntryType,
                                               MonthYear = hotelEntry.MonthYear,
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
