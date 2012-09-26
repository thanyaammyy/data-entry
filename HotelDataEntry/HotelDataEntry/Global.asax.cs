using System;
using HotelDataEntryLib.Page;

namespace HotelDataEntry
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError().GetBaseException();
            if (Request.UrlReferrer != null)
                LogHelper.StoreError(ex.Message, ex.StackTrace, Request.Url + " ::[From]:: " + Request.UrlReferrer);
            else
                LogHelper.StoreError(ex.Message, ex.StackTrace, Request.Url + " ::[From]:: Unknown");
            //End

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}