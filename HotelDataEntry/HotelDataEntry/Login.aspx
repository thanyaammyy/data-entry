<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="HotelDataEntry.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
    <meta content="Onyx Hospitality Group" name="KEYWORDS">
    <meta content="Onyx Hospitality: Corporate Office" name="DESCRIPTION">
    <title>Onyx-Hospitality | Login</title>
    <link href="Style/main.css" rel="stylesheet" type="text/css" />
    <script src="Script/fancybox/lib/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        function clearLabel() {
            $("#lbRequired").hide();
            $("#lbError").hide();
            $("#lbPwdRequired").hide();
            $("#lbUserRequired").hide();
        }
    </script>
</head>
<body>
    <form id="formLogin" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="updateTeamPanel" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <table width="300px" border="0" cellspacing="3" cellpadding="3" align="center">
                <tr>
                    <td colspan="2" style="text-align: center">
                        <img src="Style/images/ONYX.gif" />
                        <br />
                        <hr style="color: goldenrod" />
                        <br />
                        <span style="color: #ba9963; font-weight: bold">HOTEL DATA ENTRY</span>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right" class="TextBlack12">
                        Username
                    </td>
                    <td>
                        <asp:TextBox ID="tbUsername" class="TextBlack12" Width="100" MaxLength="50" runat="server"
                            ErrorMessage="Username is required." ToolTip="Username is required."></asp:TextBox><asp:Label
                                ID="lbUserRequired" Visible="False" CssClass="asteric" runat="server">*</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right" class="TextBlack12">
                        Password
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbPassword" TextMode="Password" ToolTip="Password is required."
                            ErrorMessage="Password is required." class="TextBlack12" Width="100" MaxLength="50"></asp:TextBox><asp:Label
                                ID="lbPwdRequired" Visible="False" CssClass="asteric" runat="server">*</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: center; margin-top: 10px">
                        <asp:Button runat="server" ID="btnLogin" Text="Login" OnClientClick="clearLabel();"
                            OnClick="btnLogin_Click" />
                        <asp:Button runat="server" ID="btnReset" Text="Reset" OnClick="btnReset_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: center;">
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                            <ProgressTemplate>
                                <div style="filter: progid:DXImageTransform.Microsoft.Alpha(opacity=70);">
                                    <asp:Image ID="Image1" runat="server" ImageUrl="/Style/images/loader.gif"></asp:Image><br />
                                    <asp:Label ID="lblUpdateProgress" runat="server" Font-Size="12px" ForeColor="#778899"
                                        Text="Please wait while authenticating your account..."></asp:Label>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <asp:Label ID="lbError" CssClass="redText" Visible="False" runat="server">Your login attempt was not successful. Please try again.</asp:Label>
                        <asp:Label ID="lbRequired" CssClass="redText" Visible="False" runat="server">Pleae enter username and password</asp:Label>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
