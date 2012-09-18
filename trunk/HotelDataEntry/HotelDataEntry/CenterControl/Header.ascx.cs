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
            if (string.IsNullOrEmpty(Email)) Response.Redirect("Login.aspx");
            var decryptKey = Encryption.DecryptStringAES(Email, strSharedSecret);
            if(!decryptKey.Contains("&"))
            {
                var userInfo = UserHelper.GetUser(decryptKey);
                UserId = userInfo.UserId;
                Key = Encryption.EncryptStringAES(UserId + "&" + decryptKey, strSharedSecret);

                if (UserId == 0)
                {
                    Page.ClientScript.RegisterStartupScript(GetType(), "Key", "firstLogin();", true);
                }
                
            }
            else
            {
                Key = Encryption.EncryptStringAES(decryptKey, strSharedSecret);
            }
            lbUsername.Text = Session["UserSession"].ToString();
        }
    }
}