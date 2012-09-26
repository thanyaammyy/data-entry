using System;
using System.Data.SqlClient;
using System.Web;


namespace HotelDataEntryLib.Page
{
    public static class LogHelper
    {
        public static void StoreError(string errMsg, string stacktrace, string Url)
        {
            try
            {
                using (var hdc = new HotelDataEntryDataContext())
                {
                    hdc.Logs.InsertOnSubmit(new HotelDataEntryLib.Log()
                    {
                       ErrorDate = DateTime.Now,
                       ClientIP = HttpContext.Current.Request.UserHostAddress,
                       Detail = stacktrace,
                       Message = errMsg,
                       Url = Url
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
