<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="HotelDataEntry.CenterControl.Header" %>
<div style="padding-top: 20px; padding-bottom: 10px">
    <div class="Texthead" style="padding-top: 30px; padding-right: 10px; text-align: right">
        <span style="color: slategrey">Welcome :</span> <a class="fancybox fancybox.iframe"
            href="UserInfo.aspx?key=<%=Key %>">
            <asp:Label runat="server" ID="lbUsername" ToolTip="View or edit your infromation"></asp:Label>
        </a>[<asp:HyperLink ID="hplLogout" runat="server" ToolTip="Log out of system" NavigateUrl="~/Logout.aspx"
            Text="logout"></asp:HyperLink>]
    </div>
    <div style="padding-left: 50px; padding-bottom: 20px; padding-right: 20px; padding-top: 20px; margin: auto;">
        <p align="center" style="font-size: 12px; font-family: Helvetica;">
            <div style="float: left; ">
                <a href="Revenue.aspx?key=<%=Key %>" id="inputForm">Reveneu Entry</a> <span style="color: black; padding: 2px">l </span>
                <a href="Report.aspx?key=<%=Key %>" id="report">Budget Entry</a>
            </div>
            <div id="divAdmin" style="padding-left: 10px; float: left; display: none" runat="server">
                <span style="color: black; padding: 2px">l</span> 
                <a href="Property.aspx?key=<%=Key %>" id="propMa">Property Maintenance</a> <span style="color: black; padding: 2px">l</span> 
                <a href="User.aspx?key=<%=Key %>" id="userMa">User Maintenance</a> <span  style="color: black;padding: 2px">l</span> 
                <a href="Currency.aspx?key=<%=Key %>" id="currencyMa">Currency Maintenance</a>
            </div>
            
        </p>
    </div>
    <hr style="height: 0px; border-bottom: 2px solid #ba9963;">
</div>
