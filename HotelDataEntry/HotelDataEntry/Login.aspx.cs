﻿using System;
using System.Configuration;
using System.DirectoryServices;
using HotelDataEntryLib.Helper;
using HotelDataEntryLib.Page;

namespace HotelDataEntry
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Clear();
            btnLogin.Enabled = true;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            lbUserRequired.Visible = false;
            lbPwdRequired.Visible = false;
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
                    var user = UserHelper.GetUser(username);
                    if (!string.IsNullOrEmpty(user.Email)&&!string.IsNullOrEmpty(user.Username))
                    {
                        if (user.Status == 1)
                        {
                            Session["loginKey"] = encryptEmail;
                            Response.Redirect("Revenue.aspx");
                        }
                        else
                        {
                            btnLogin.Enabled = true;
                            lbError.Text = "You are not authorized to access this page";
                            lbError.Visible = true;
                        }
                    }
                    else
                    {
                        Session["loginKey"] = encryptEmail;
                        Response.Redirect("Revenue.aspx");   
                    }
                }
                else
                {
                    btnLogin.Enabled = true;
                    lbError.Text = "Your login attempt was not successful. Please try again.";
                    lbError.Visible = true;
                    lbUserRequired.Visible = false;
                    lbPwdRequired.Visible = false;
                }
            }
            else
            {
                btnLogin.Enabled = true;
                lbUserRequired.Visible = true;
                lbPwdRequired.Visible = true;
                lbError.Text = "Pleae enter username and password.";
                lbError.Visible = true;
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            tbPassword.Text = "";
            tbUsername.Text = "";
            lbError.Visible = false;
            lbPwdRequired.Visible = false;
            lbUserRequired.Visible = false;
        }

        public bool AuthenticateActiveDirectory(string userName, string password)
        {
            try
            {
                var ldap = ConfigurationManager.AppSettings["Ldap"];
                var entry = new DirectoryEntry(ldap, userName, password);
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