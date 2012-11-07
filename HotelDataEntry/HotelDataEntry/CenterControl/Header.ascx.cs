using System;
using System.Configuration;
using HotelDataEntryLib.Helper;
using HotelDataEntryLib.Page;

namespace HotelDataEntry.CenterControl
{
    public partial class Header : System.Web.UI.UserControl
    {
        public int UserId;
        public string UserName;
        public string Key;
        protected void Page_Load(object sender, EventArgs e)
        {
            var strSharedSecret = ConfigurationManager.AppSettings["SharedSecret"];
            UserName = Session["Key"] == null ? Request.QueryString["key"] : Session["Key"].ToString();
            if (string.IsNullOrEmpty(UserName)) Response.Redirect("Login.aspx");
            var decryptKey = Encryption.DecryptStringAES(UserName, strSharedSecret);
            if(!decryptKey.Contains("&"))
            {
                var userInfo = UserHelper.GetUser(decryptKey);
                UserId = userInfo.UserId;
                Key = Encryption.EncryptStringAES(UserId + "&" + decryptKey, strSharedSecret);
                Session["permission"] = userInfo.PermissionId;
                Session["userId"] = userInfo.UserId;

                if (UserId <= 0)
                {
                    Page.ClientScript.RegisterStartupScript(GetType(), "Key", "firstLogin();", true);
                }
                
            }
            else
            {
                Key = Encryption.EncryptStringAES(decryptKey, strSharedSecret);
            }

            if (!string.IsNullOrEmpty(Session["permission"].ToString()))
            {
                if (Convert.ToInt32(Session["permission"].ToString()) == 3)
                {
                    divAdmin.Style["display"] = "";
                } 
            }
            Session["Key"] = Key;
            lbUsername.Text = Session["UserSession"].ToString();
        }
    }
}