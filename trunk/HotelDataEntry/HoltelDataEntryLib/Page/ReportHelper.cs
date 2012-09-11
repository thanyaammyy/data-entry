using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelDataEntryLib.Page
{
    public static class ReportHelper
    {
        public static  IEnumerable<object> CalOccupiedRoom(string dateFrom, string dateTo, int propertyId)
        {
            IEnumerable<object> dataEntryList= null;
            var hdc = new HotelDataEntryDataContext();
            
              //dataEntryList = (from hotelEntry in hdc.HotelEntries
              //                       join dataEntry in hdc.DataEntries on hotelEntry.HotelEntryId equals dataEntry.HotelEntryId
              //                       where hotelEntry.DataEntrySubTypeId==1 
              //                       && hotelEntry.PropertyId==propertyId
              //                       && dataEntry.PositionDate>= dateFrom
                                     
              //                       select new
              //                       {
                                        
              //                       }).ToList();

              //  paymentFilteredList.AddRange(from paymentLog in paymentDataList let checkTestBooking = dc.Bookings.Any(item => item.BookingCode == paymentLog.BookingCode && item.CreditCardNumber == "4111111111111111") where !checkTestBooking select paymentLog);

              //  var resultList = (from paymentData in paymentFilteredList
              //                    group paymentData by new
              //                    {
              //                        paymentData.PaymentStatusName,
              //                        paymentData.ResponseMessage
              //                    } into g
              //                    select new PaymentReport()
              //                    {
              //                        PaymentStatus = g.Key.PaymentStatusName,
              //                        PaymentStatusMessage = g.Key.ResponseMessage,
              //                        MasterCardAmount = g.Sum(item => item.PaymentType == "02" ? 1 : 0),
              //                        MasterCardPercent = (decimal)((g.Sum(item => item.PaymentType == "02" ? 1 : 0)) * 100) / paymentDataList.Count(item => item.PaymentType == "02"),
              //                        VisaCardAmount = g.Sum(item => item.PaymentType == "01" ? 1 : 0),
              //                        VisaCardPercent = (decimal)((g.Sum(item => item.PaymentType == "01" ? 1 : 0)) * 100) / paymentDataList.Count(item => item.PaymentType == "01"),
              //                        Total = g.Sum(item => (item.PaymentType == "01" || item.PaymentType == "02") ? 1 : 0),
              //                        TotalPercent = ((decimal)((g.Sum(item => item.PaymentType == "02" ? 1 : 0)) * 100) / paymentDataList.Count) + ((decimal)((g.Sum(item => item.PaymentType == "01" ? 1 : 0)) * 100) / paymentDataList.Count)
              //                    }).OrderBy(report => report.PaymentStatus).ToList();
              //  return resultList;  
            
            return dataEntryList;
        }
    }
}
