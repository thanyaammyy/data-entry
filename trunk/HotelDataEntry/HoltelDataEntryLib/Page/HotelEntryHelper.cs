﻿using System;
using System.Data.SqlClient;
using System.Linq;

namespace HotelDataEntryLib.Page
{
    public static class HotelEntryHelper
    {
        public static bool ExistMothYear(HotelEntry hotelEntry)
        {
            var hdc = new HotelDataEntryDataContext();
            var count = hdc.HotelEntries.Count(item => item.MonthYear == hotelEntry.MonthYear && item.PropertyId==hotelEntry.PropertyId && item.DataEntrySubTypeId== hotelEntry.DataEntrySubTypeId);
            return count != 0;
        }

        public static HotelEntry GetHotelEntry(HotelEntry hotelEntry)
        {
            var hdc = new HotelDataEntryDataContext();
            var hEntry = hdc.HotelEntries.Single(item => item.MonthYear == hotelEntry.MonthYear && item.PropertyId == hotelEntry.PropertyId && item.DataEntrySubTypeId == hotelEntry.DataEntrySubTypeId);
            return hEntry;
        }

        public static HotelEntry AddHotelEntryListByMonthYear(HotelEntry hotelEntry)
        {
            HotelEntry hotelEntrySubmit;
            using (var hdc = new HotelDataEntryDataContext())
            {
                hotelEntrySubmit = new HotelEntry()
                                           {
                                               PropertyId = hotelEntry.PropertyId,
                                               DataEntrySubTypeId = hotelEntry.DataEntrySubTypeId,
                                               MonthYear = hotelEntry.MonthYear,
                                               UpdateDateTime = DateTime.Now
                                           };
                hdc.HotelEntries.InsertOnSubmit(hotelEntrySubmit);

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