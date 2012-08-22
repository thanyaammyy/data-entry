<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserInfo.aspx.cs" Inherits="HotelDataEntry.UserInfo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
    <meta content="Onyx Hospitality Group" name="KEYWORDS">
    <meta content="Onyx Hospitality: Corporate Office" name="DESCRIPTION">
    <title>Onyx-Hospitality | User Information</title>
    <link href="Style/main.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div style="width: 320px;">
        <table width="320" style="padding-bottom: 30px; padding-top: 30px; padding-left: 10px;"
            border="0" cellspacing="0" cellpadding="0" align="center">
            <tr>
                <td align="center" class="TextBlack12">
                    <b>User</b>
                </td>
            </tr>
            <tr>
                <td>
                    <br>
                    <fieldset class="TextBlack12" />
                    <legend class="TextBlack12"><b>User Information</b></legend>
                    <table width="400px" border="0" cellspacing="3" cellpadding="3" align="center">
                        <tr>
                            <td style="text-align: right" class="TextBlack12">
                                First name
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="tbFirstName" ToolTip="Fist name is required" class="TextBlack12"
                                    Width="170" MaxLength="50"></asp:TextBox>
                                <asp:Label ID="lbFirstNameRequired" CssClass="asteric" runat="server">*</asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right" class="TextBlack12">
                                Last name
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="tbLastName" ToolTip="Last name is required" class="TextBlack12"
                                    Width="170" MaxLength="50"></asp:TextBox>
                                <asp:Label ID="lbLastNameRequired" CssClass="asteric" runat="server">*</asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right" class="TextBlack12">
                                E-mail
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lbEmail" class="TextBlack12" Width="170"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right" class="TextBlack12">
                                Company
                            </td>
                            <td>
                                <asp:UpdatePanel ID="updateTeamPanel" UpdateMode="Conditional" runat="server">
                                    <ContentTemplate>
                                        <asp:DropDownList runat="server" ID="ddlCompany" ToolTip="Company is required" class="TextBlack12"
                                            DataSourceID="CompanyDataSource" DataTextField="PropertyCodeWithName" DataValueField="PropertyId"
                                            Width="175px" AutoPostBack="True" OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged1" />
                                        <asp:Label ID="lbCompanyRequired" CssClass="asteric" runat="server">*</asp:Label>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right" class="TextBlack12">
                                Alternative Company
                            </td>
                            <td>
                                <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">
                                    <ContentTemplate>
                                        <asp:DropDownList runat="server" ID="ddlAlterCompany" class="TextBlack12" Width="175px"
                                            AutoPostBack="True" DataSourceID="AlterCompanyDataSource" DataTextField="PropertyCodeWithName"
                                            DataValueField="PropertyId" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <asp:Button runat="server" ID="btnUpdateProfile" Text="Update" OnClick="btnUpdateProfile_Click" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                                <asp:Label ID="lbRequired" CssClass="redText" Visible="False" runat="server">Please fullfill the required information</asp:Label>
                            </td>
                        </tr>
                    </table>
        </table>
    </div>
    <asp:ObjectDataSource ID="CompanyDataSource" DataObjectTypeName="HotelDataEntryLib.Property"
        SelectMethod="ListCompany" TypeName="HotelDataEntryLib.Page.PropertyHelper" runat="server">
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="AlterCompanyDataSource" DataObjectTypeName="HotelDataEntryLib.Property"
        SelectMethod="ListAlterCompany" TypeName="HotelDataEntryLib.Page.PropertyHelper"
        runat="server">
        <SelectParameters>
            <asp:SessionParameter Name="PropertyId" SessionField="PropertyId" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    </form>
</body>
</html>
