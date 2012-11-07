<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="HotelDataEntry.Reports" %>
<%@ Register TagPrefix="cc1" Namespace="Trirand.Web.UI.WebControls" Assembly="Trirand.Web" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta content="Onyx Hospitality Group" name="KEYWORDS" />
    <meta content="Onyx Hospitality: Corporate Office" name="DESCRIPTION" />
    <title>Onyx-Hospitality | Report</title>
    <link href="Style/main.css" rel="stylesheet" type="text/css" />
    <link href="Script/sources/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <link href="Style/jquery-ui-1.9.1.custom.css" rel="stylesheet" type="text/css" />
    <link href="Style/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <script src="Script/jqueryUI/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="Script/jqueryUI/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="Script/jqueryUI/jquery.jqDatePicker.min.js" type="text/javascript"></script>
    <script src="Script/sources/jquery.fancybox.js" type="text/javascript"></script>
    <script src="Script/jqueryUI/grid.locale-en.js" type="text/javascript"></script>
    <script src="Script/jqueryUI/jquery.jqGrid.src.js" type="text/javascript"></script>
    <script src="Script/jqueryUI/jquery.jqAutoComplete.min.js" type="text/javascript"></script>
</head>
<body style="width: 90%; height: 90%">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div style="width: 800px; height: 450px">
        <table width="450" style="padding-bottom: 10px; padding-top: 10px; padding-left: 5px;"
            border="0" cellspacing="0" cellpadding="0" align="center">
            <tr>
                <td class="TextBlack12">
                    Property
                </td>
                <td>
                    <asp:Label runat="server" ID="lbProperty" CssClass="TextBlack12"></asp:Label>
                </td>
            </tr>
            <tr >
                <td class="TextBlack12">
                    Year
                </td>
                <td>
                    <asp:Label runat="server" ID="lbYear" CssClass="TextBlack12"></asp:Label>
                </td>
            </tr>
        </table>
        <div style="padding-top: 10px;" runat="server" id="divJqgrid">
            <asp:UpdatePanel ID="updatepanel1" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <cc1:JQGrid ID="JqGridReport" AutoWidth="True" runat="server" Height="80%">
                        <Columns>
                            <cc1:JQGridColumn DataField="BudgetId" Searchable="False" PrimaryKey="True" Width="55"
                                Visible="False" />
                            <cc1:JQGridColumn HeaderText="Month/Year" Width="200" DataField="MonthYear" TextAlign="Center" FooterValue="Total:">
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn Width="200" HeaderText="Actual" DataField="RoomActual" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn Width="200" HeaderText="Budget" DataField="RoomBudget" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn Width="200" HeaderText="Actual" DataField="FBActual" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn Width="200" HeaderText="Budget" DataField="FBBudget" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn Width="200" HeaderText="Actual" DataField="SpaActual" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn Width="200" HeaderText="Budget" DataField="SpaBudget" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn Width="200" HeaderText="Actual" DataField="OtherActual" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn Width="200" HeaderText="Budget" DataField="OtherBudget" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                            </cc1:JQGridColumn>
                        </Columns>
                        <HeaderGroups>
                            <cc1:JQGridHeaderGroup StartColumnName="RoomActual" NumberOfColumns="2" TitleText="Room" />
                            <cc1:JQGridHeaderGroup StartColumnName="FBActual" NumberOfColumns="2" TitleText="F & B" />
                            <cc1:JQGridHeaderGroup StartColumnName="SpaActual" NumberOfColumns="2" TitleText="Spa" />
                            <cc1:JQGridHeaderGroup StartColumnName="OtherActual" NumberOfColumns="2" TitleText="Others" />
                        </HeaderGroups>
                        <ToolBarSettings ShowRefreshButton="True" />
                        <PagerSettings PageSize="32" />
                        <AppearanceSettings ShowRowNumbers="true" ShowFooter="true" HighlightRowsOnHover="True" />
                    </cc1:JQGrid>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

    </div>
    </form>
</body>
</html>
