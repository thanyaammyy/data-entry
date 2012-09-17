using System;
using System.Configuration;
using System.Globalization;
using HotelDataEntryLib.Helper;
using HotelDataEntryLib.Page;

namespace HotelDataEntry.CenterControl
{
    public partial class Header : System.Web.UI.UserControl
    {
        public int UserId;
        public string Email;
        public string Key;
        protected void Page_Load(object sender, EventArgs e)
        {
            var strSharedSecret = ConfigurationManager.AppSettings["SharedSecret"];
            Email = Request.QueryString["key"];
            var decryptEmail = Encryption.DecryptStringAES(Email, strSharedSecret);
            var userInfo = UserHelper.GetUser(decryptEmail);
            UserId = userInfo.UserId;
            var strUserId = Encryption.EncryptStringAES(UserId.ToString(), strSharedSecret);
            var strKey = strUserId + "&" + Email;
            Key = Encryption.EncryptStringAES(strKey, strSharedSecret);
            if (!ReferenceEquals(Session["LoginSession"], "True"))
            {
                Response.Redirect("Login.aspx");
            }
            else
            {
                lbUsername.Text = Session["UserSession"].ToString();
                if (UserId == 0)
                {
                    Page.ClientScript.RegisterStartupScript(GetType(), "Key", "firstLogin();", true);
                }
            }
        }
    }
}