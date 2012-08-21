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
    <asp:UpdatePanel ID="updateTeamPanel" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <div style="width: 350px">
                <table width="340" style="padding-bottom: 30px; padding-top: 30px; padding-left: 10px;"
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
                            <table width="300px" border="0" cellspacing="3" cellpadding="3" align="center">
                                <tr>
                                    <td style="text-align: right" class="TextBlack12">
                                        First name
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="tbFirstName" class="TextBlack12" Width="115" MaxLength="50"></asp:TextBox>
                                        <asp:Label ID="lbFirstNameRequired" CssClass="asteric" runat="server">*</asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: right" class="TextBlack12">
                                        Last name
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="tbLastName" class="TextBlack12" Width="115" MaxLength="50"></asp:TextBox>
                                        <asp:Label ID="lbLastNameRequired" CssClass="asteric" runat="server">*</asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: right" class="TextBlack12">
                                        E-mail
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="tbEmail" class="TextBlack12" Width="115" MaxLength="50"></asp:TextBox>
                                        <asp:Label ID="lbEmailRequired" CssClass="asteric" runat="server">*</asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: right" class="TextBlack12">
                                        Company
                                    </td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="ddlCompany" class="TextBlack12" DataSourceID="CompanyDataSource"
                                            DataTextField="PropertyCodeWithName" DataValueField="PropertyId" Width="120px" AutoPostBack="True"
                                            OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged1" />
                                        <asp:Label ID="lbCompanyRequired" CssClass="asteric" runat="server">*</asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: right" class="TextBlack12">
                                        Alternative Company
                                    </td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="DropDownList1" class="TextBlack12" Width="120px" AutoPostBack="True"
                                            DataSourceID="AlterCompanyDataSource" DataTextField="PropertyCodeWithName" DataValueField="PropertyId" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="2">
                                        <asp:Button runat="server" ID="btnUpdateProfile" Text="Update"/>
                                    </td>
                                </tr>
                            </table>
                </table>
            </div>
            <asp:ObjectDataSource ID="CompanyDataSource" DataObjectTypeName="HotelDataEntryLib.Property"
                SelectMethod="ListCompany" TypeName="HotelDataEntryLib.Page.PropertyHelper"
                runat="server"></asp:ObjectDataSource>
            <asp:ObjectDataSource ID="AlterCompanyDataSource" DataObjectTypeName="HotelDataEntryLib.Property"
                SelectMethod="ListAlterCompany" TypeName="HotelDataEntryLib.Page.PropertyHelper"
                runat="server">
                <SelectParameters>
                    <asp:SessionParameter Name="PropertyId" SessionField="PropertyId" Type="Int32" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
