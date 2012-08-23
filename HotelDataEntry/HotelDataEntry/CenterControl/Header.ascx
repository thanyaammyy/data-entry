<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="HotelDataEntry.CenterControl.Header" %>
<div>
    <div class="Texthead" style="padding-top: 20px; padding-right: 10px; text-align: right">
        <span style="color: slategrey">Welcome :</span> <a class="fancybox fancybox.iframe"
            href="UserInfo.aspx?UserId=<%=StrUserId%>&Email=<%=Email %>">
            <asp:Label runat="server" ID="lbUsername" ToolTip="View or edit your infromation"></asp:Label>
        </a>[<asp:HyperLink ID="hplLogout" runat="server" ToolTip="Log out of system" NavigateUrl="~/Logout.aspx"
            Text="logout"></asp:HyperLink>]
    </div>
    <div>
        <p align="center" style="font-size: 12px; font-family: Helvetica;">
            <a href="#" id="inputForm">Data Input Form</a><span style="color: #727272; padding: 2px">l</span><a href="#" id="report">Report</a>
            <span style="color: #727272; padding: 2px">l</span><a href="#" id="compMa">Company Maintenance</a> <span style="color: #727272; padding: 2px">l</span> <a href="#" id="userMa">User Maintenance</a>
            <span style="color: #727272; padding: 2px">l</span><a href="#" id="currencyMa">Currency Maintenance</a>
        </p>
    </div>
    <hr style="height: 0px; border-bottom: 2px solid #ba9963;">
</div>
