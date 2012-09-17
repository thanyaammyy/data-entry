using System;
using System.Configuration;
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
            lbBrand.Visible = false;
            lbError.Visible = false;
            Session["LoginSession"] = "False";
            Session["UserSession"] = "";
            var username = tbUsername.Text;
            var password = tbPassword.Text;
            var emailSuffix = ddlBrand.SelectedValue;
            if(!(string.IsNullOrEmpty(username)||string.IsNullOrEmpty(password)||string.IsNullOrEmpty(emailSuffix)))
            {
                
                var exchangeService = new ExchangeService(ExchangeVersion.Exchange2010_SP1)
                        {
                            Credentials = new NetworkCredential(username, password, "onyx-hq"),
                            Url = new Uri("https://owa.onyx-hospitality.com/ews/Exchange.asmx"),
                            Timeout = 60
                        };
                try
                {
                    btnLogin.Enabled = false;
                    exchangeService.AutodiscoverUrl(username + emailSuffix);
                    Session["LoginSession"] = "True";
                    Session["UserSession"] = username;

                    var strSharedSecret = ConfigurationManager.AppSettings["SharedSecret"];
                    var encryptEmail = Encryption.EncryptStringAES(username + emailSuffix, strSharedSecret);
                    Response.Redirect("~/DataEntry.aspx?key="+encryptEmail);
                }
                catch (Exception ex)
                {
                    btnLogin.Enabled = true;
                    lbError.Visible = true;
                    lbUserRequired.Visible = false;
                    lbPwdRequired.Visible = false;
                    lbRequired.Visible = false;
                    lbBrand.Visible = false;
                }
            }
            else
            {
                btnLogin.Enabled = true;
                lbUserRequired.Visible = true;
                lbPwdRequired.Visible = true;
                lbRequired.Visible = true;
                lbBrand.Visible = true;
                lbError.Visible = false;
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            tbPassword.Text = "";
            tbUsername.Text = "";
        }
    }
}