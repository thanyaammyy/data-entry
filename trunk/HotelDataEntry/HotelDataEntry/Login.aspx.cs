using System;
using System.Configuration;
using System.DirectoryServices;
using System.Net;
using System.Web.Configuration;
using HotelDataEntryLib.Helper;
using Microsoft.Exchange.WebServices.Data;

namespace HotelDataEntry
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnLogin.Enabled = true;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            lbUserRequired.Visible = false;
            lbPwdRequired.Visible = false;
            lbRequired.Visible = false;
            lbError.Visible = false;
            Session["LoginSession"] = "False";
            Session["UserSession"] = "";
            var username = tbUsername.Text;
            var password = tbPassword.Text;
            if(!(string.IsNullOrEmpty(username)||string.IsNullOrEmpty(password)))
            {
                btnLogin.Enabled = false;
                if(AuthenticateActiveDirectory(username, password))
                {
                    Session["LoginSession"] = "True";
                    Session["UserSession"] = username;
                    var strSharedSecret = ConfigurationManager.AppSettings["SharedSecret"];
                    var encryptEmail = Encryption.EncryptStringAES(username, strSharedSecret);
                    Response.Redirect("~/DataEntry.aspx?key=" + encryptEmail);
                }
                else
                {
                    btnLogin.Enabled = true;
                    lbError.Visible = true;
                    lbUserRequired.Visible = false;
                    lbPwdRequired.Visible = false;
                    lbRequired.Visible = false;
                }
            }
            else
            {
                btnLogin.Enabled = true;
                lbUserRequired.Visible = true;
                lbPwdRequired.Visible = true;
                lbRequired.Visible = true;
                lbError.Visible = false;
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            tbPassword.Text = "";
            tbUsername.Text = "";
        }

        public bool AuthenticateActiveDirectory(string userName, string password)
        {
            try
            {
                var entry = new DirectoryEntry("LDAP://onyx-hq", userName, password);
                var nativeObject = entry.NativeObject;
                return true;
            }
            catch (DirectoryServicesCOMException)
            {
                return false;
            }
        }
    }
}