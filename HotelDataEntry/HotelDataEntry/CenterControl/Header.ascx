<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="HotelDataEntry.CenterControl.Header" %>
<div style="padding-top: 20px; padding-bottom: 10px">
    <div class="Texthead" style="padding-top: 30px; padding-right: 10px; text-align: right">
        <span style="color: slategrey">Welcome :</span> <a class="fancybox fancybox.iframe"
            href="UserInfo.aspx?UserId=<%=StrUserId%>&Email=<%=Email %>">
            <asp:Label runat="server" ID="lbUsername" ToolTip="View or edit your infromation"></asp:Label>
        </a>[<asp:HyperLink ID="hplLogout" runat="server" ToolTip="Log out of system" NavigateUrl="~/Logout.aspx"
            Text="logout"></asp:HyperLink>]
    </div>
    <div style="padding: 20px">
        <p align="center" style="font-size: 12px; font-family: Helvetica;">
            <a href="DataEntry.aspx?UserId=<%=StrUserId%>&Email=<%=Email %>" id="inputForm">Data Input Form</a> <span style="color: black; padding: 2px">
                l </span> <a href="#" id="report">Report</a> <span style="color: black; padding: 2px">
                    l</span> <a href="Company.aspx?UserId=<%=StrUserId%>&Email=<%=Email %>" id="compMa">Company Maintenance</a> <span style="color: black;
                        padding: 2px">l</span> <a href="User.aspx?UserId=<%=StrUserId%>&Email=<%=Email %>" id="userMa">User Maintenance</a>
            <span style="color: black; padding: 2px">l</span> <a href="Currency.aspx?UserId=<%=StrUserId%>&Email=<%=Email %>" id="currencyMa">Currency
                Maintenance</a>
        </p>
    </div>
    <hr style="height: 0px; border-bottom: 2px solid #ba9963;">
</div>
