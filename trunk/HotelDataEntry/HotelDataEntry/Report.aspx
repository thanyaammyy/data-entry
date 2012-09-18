<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="Report.aspx.cs" Inherits="HotelDataEntry.Report" %>

<%@ Register TagPrefix="cc1" Namespace="Trirand.Web.UI.WebControls" Assembly="Trirand.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .ui-datepicker-calendar
        {
            display: none;
        }
    </style>
    <script type="text/javascript" language="javascript">
        function onDateClick() {
            $('.ui-datepicker-calendar').show();
            $("div.ui-datepicker-header a.ui-datepicker-prev,div.ui-datepicker-header a.ui-datepicker-next").hide();
        }

        $(document).ready(function () {
            var sessionMonthly = '<%= Session["monthly"] %>';
            if (sessionMonthly == "monthly") {
                $("#divYearlyReport").hide();
                $("#divMonthlyReport").show();
                $('input[name="monthlyDate"]').blur();
                $('input[name="monthlyDate"]').datepicker({
                    changeMonth: true,
                    changeYear: true,
                    dateFormat: 'mm/yy',
                    yearRange: "2008:2020",
                    onClose: function (dateText, inst) {
                        var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                        var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                        $(this).datepicker('setDate', new Date(year, month, 1));
                        var intMonth = parseInt(month) + 1;
                        var strMonth = intMonth > 10 ? intMonth : 0 + "" + intMonth;
                        var my = strMonth + "/" + year;
                        document.getElementById("<%= hiddenMonthYear.ClientID %>").value = my;
                    }
                });
            } else {
                $("#divMonthlyReport").hide();
                $("#divYearlyReport").show();
                $('input[name="dateFrom"]').blur();
                $('input[name="dateFrom"]').datepicker({
                    changeMonth: true,
                    changeYear: true,
                    dateFormat: 'dd/mm/yy',
                    onClose: function (dateText, inst) {
                        document.getElementById("<%= hiddenDateFrom.ClientID %>").value = dateText;
                    }
                });

                $('input[name="dateTo"]').blur();
                $('input[name="dateTo"]').datepicker({
                    changeMonth: true,
                    changeYear: true,
                    dateFormat: 'dd/mm/yy',
                    onClose: function (dateText, inst) {
                        document.getElementById("<%= hiddenDateTo.ClientID %>").value = dateText;
                    }
                });
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" id="hiddenDateFrom" runat="server" />
    <input type="hidden" id="hiddenDateTo" runat="server" />
    <input type="hidden" id="hiddenMonthYear" runat="server" />
    <div class="headerMenuLabel">
        Report Summary
    </div>
    <asp:CheckBox CssClass="TextBlack12" runat="server" ID="chkReportMonthly" Text="Monthly Report"
        AutoPostBack="True" OnCheckedChanged="chkReportMonthly_CheckedChanged" />
    <div id="divYearlyReport">
        <div>
            <table class="TextBlack12" cellpadding="3" cellspacing="3">
                <tr>
                    <td>
                        Property
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlCompany" ToolTip="Select a property" DataSourceID="PropertyDataSource"
                            AutoPostBack="True" DataValueField="PropertyId" DataTextField="PropertyCode"
                            Width="150" runat="server" OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:Label ID="lbCompany" Visible="False" CssClass="asteric" runat="server">*</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        From
                    </td>
                    <td>
                        <%if (Session["dateFrom"] == null)
                          {
                        %>
                        <input type="text" id="dateFrom" name="dateFrom" onfocus="onDateClick();" />
                        <%}
                          else
                          {%>
                        <input type="text" id="Text1" name="dateFrom" onfocus="onDateClick();" value="<%= Session["dateFrom"] %>" />
                        <% } %>
                        <asp:Label ID="lbDateFrom" Visible="False" CssClass="asteric" runat="server">*</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        To
                    </td>
                    <td>
                        <%if (Session["dateFrom"] == null)
                          {
                        %>
                        <input type="text" id="dateTo" onfocus="onDateClick();" name="dateTo" />
                        <%}
                          else
                          {%>
                        <input type="text" id="Text3" name="dateTo" onfocus="onDateClick();" value="<%= Session["dateTo"] %>" />
                        <% } %>
                        <asp:Label ID="lbDateTo" Visible="False" CssClass="asteric" runat="server">*</asp:Label>
                    </td>
                </tr>
                <tr id="displayCurrency" style="display: none" runat="server">
                    <td>
                        Currency
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lbCurerncy"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:Button runat="server" ID="btnYearlyReport" Text="Show" OnClick="btnYearlyReport_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:Label Visible="False" runat="server" ID="lbError" Text="Please select the required feilds"
                            CssClass="redText"></asp:Label>
                            <asp:Label Visible="False" runat="server" ID="lbErrorDate" Text="Date To must greater than Date From"
                            CssClass="redText"></asp:Label>
                    </td>
                </tr>
            </table>
            <asp:ObjectDataSource ID="PropertyDataSource" DataObjectTypeName="HotelDataEntryLib.Property"
                SelectMethod="ListCompany" TypeName="HotelDataEntryLib.Page.PropertyHelper" runat="server">
            </asp:ObjectDataSource>
        </div>
        <div style="padding-top: 20px; display: none" runat="server" id="divJqGridYearlyReport">
            <cc1:JQGrid ID="JqGridYearlyReport" AutoWidth="True" runat="server" 
                Height="80%" oninit="JqGridYearlyReport_Init">
                <Columns>
                    <cc1:JQGridColumn HeaderText="Type" DataField="Type" TextAlign="Center">
                    </cc1:JQGridColumn>
                    <cc1:JQGridColumn HeaderText="Data" DataField="SubType" TextAlign="Center" FooterValue="Total:">
                    </cc1:JQGridColumn>
                    <cc1:JQGridColumn HeaderText="Budget-TY" DataField="BudgetTY" GroupSummaryType="Sum"
                        GroupTemplate="Sum: {0}" TextAlign="Right">
                        <Formatter>
                            <cc1:NumberFormatter ThousandsSeparator="," DecimalSeparator="." DecimalPlaces="2"
                                DefaultValue="0.00" />
                        </Formatter>
                    </cc1:JQGridColumn>
                    <cc1:JQGridColumn HeaderText="Budget-LY" DataField="BudgetLY" GroupSummaryType="Sum"
                        GroupTemplate="Sum: {0}" TextAlign="Right">
                        <Formatter>
                            <cc1:NumberFormatter ThousandsSeparator="," DecimalSeparator="." DecimalPlaces="2"
                                DefaultValue="0.00" />
                        </Formatter>
                    </cc1:JQGridColumn>
                </Columns>
                <GroupSettings CollapseGroups="false">
                    <GroupFields>
                        <cc1:GroupField DataField="Type" GroupSortDirection="Asc" HeaderText="<b>{0}</b>"
                            ShowGroupColumn="False" ShowGroupSummary="True" />
                    </GroupFields>
                </GroupSettings>
                <ToolBarSettings ShowRefreshButton="True" />
                <PagerSettings PageSize="20" />
                <ExportSettings ExportDataRange="All" ExportHeaders="True"></ExportSettings>
                <AppearanceSettings ShowRowNumbers="False" ShowFooter="true" />
            </cc1:JQGrid>
        </div>
        <div style="display: none; padding-top: 20px" id="divExportData" runat="server">
            <table class="TextBlack12" cellpadding="3" cellspacing="3">
                <tr>
                    <td>
                        Export
                    </td>
                    <td>
                        <asp:ImageButton Width="22" Height="22" ID="btnCSV" ImageUrl="Style/images/csv.png"
                            ToolTip="Export to CSV" runat="server" OnClick="btnCSV_Click" />
                    </td>
                    <td>
                        <asp:ImageButton Width="22" Height="22" ID="btnExcel" ImageUrl="Style/images/excel.png"
                            ToolTip="Export to Excel" runat="server" OnClick="btnExcel_Click" />
                    </td>
                    <td>
                        <asp:ImageButton Width="22" Height="22" ID="btnPDF" ImageUrl="Style/images/pdf.png"
                            ToolTip="Export to PDF" runat="server" OnClick="btnPDF_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="divMonthlyReport" style="display: none">
        <div>
            <table class="TextBlack12" cellpadding="3" cellspacing="3">
                <tr>
                    <td>
                        Property
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlCompany2" ToolTip="Select a property" DataSourceID="PropertyDataSource"
                            AutoPostBack="True" DataValueField="PropertyId" DataTextField="PropertyCode"
                            Width="150" runat="server" OnSelectedIndexChanged="ddlCompany2_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:Label ID="lbCompany2" Visible="False" CssClass="asteric" runat="server">*</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        Period
                    </td>
                    <td>
                        <%if (Session["monthlyDate"] == null)
                          {
                        %>
                        <input type="text" name="monthlyDate" />
                        <%}
                          else
                          {%>
                        <input type="text" name="monthlyDate" value="<%= Session["monthlyDate"] %>" />
                        <% } %>
                        <asp:Label ID="lbMonthlyDate" Visible="False" CssClass="asteric" runat="server">*</asp:Label>
                    </td>
                </tr>
                <tr id="displayCurrency2" style="display: none" runat="server">
                    <td>
                        Currency
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lbCurrency2"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:Button runat="server" ID="btnMonthlyReport" Text="Show" OnClick="btnMonthlyReport_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:Label Visible="False" runat="server" ID="lbError2" Text="Please select the required feilds"
                            CssClass="redText"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <div style="display: none; padding-top: 20px; display: none" runat="server" id="divJqGridMonthlyReport">
            <cc1:JQGrid ID="JqGridMonthlyReport" AutoWidth="True" runat="server" 
                Height="80%" oninit="JqGridMonthlyReport_Init">
                <Columns>
                    <cc1:JQGridColumn HeaderText="Type" DataField="Type" TextAlign="Center">
                    </cc1:JQGridColumn>
                    <cc1:JQGridColumn HeaderText="Data" DataField="SubType" TextAlign="Center" FooterValue="Total:">
                    </cc1:JQGridColumn>
                    <cc1:JQGridColumn HeaderText="Actual" DataField="Actual" GroupSummaryType="Sum"
                        GroupTemplate="Sum: {0}" TextAlign="Right">
                        <Formatter>
                            <cc1:NumberFormatter ThousandsSeparator="," DecimalSeparator="." DecimalPlaces="2"
                                DefaultValue="0.00" />
                        </Formatter>
                    </cc1:JQGridColumn>
                    <cc1:JQGridColumn HeaderText="Actual-LY" DataField="ActualLY" GroupSummaryType="Sum"
                        GroupTemplate="Sum: {0}" TextAlign="Right">
                        <Formatter>
                            <cc1:NumberFormatter ThousandsSeparator="," DecimalSeparator="." DecimalPlaces="2"
                                DefaultValue="0.00" />
                        </Formatter>
                    </cc1:JQGridColumn>
                    <cc1:JQGridColumn HeaderText="Actual-YTD" DataField="ActualYtd" GroupSummaryType="Sum"
                        GroupTemplate="Sum: {0}" TextAlign="Right">
                        <Formatter>
                            <cc1:NumberFormatter ThousandsSeparator="," DecimalSeparator="." DecimalPlaces="2"
                                DefaultValue="0.00" />
                        </Formatter>
                    </cc1:JQGridColumn>
                    <cc1:JQGridColumn HeaderText="Actual-YTD-LY" DataField="ActualYtdLY" GroupSummaryType="Sum"
                        GroupTemplate="Sum: {0}" TextAlign="Right">
                        <Formatter>
                            <cc1:NumberFormatter ThousandsSeparator="," DecimalSeparator="." DecimalPlaces="2"
                                DefaultValue="0.00" />
                        </Formatter>
                    </cc1:JQGridColumn>
                </Columns>
                <GroupSettings CollapseGroups="false">
                    <GroupFields>
                        <cc1:GroupField DataField="Type" GroupSortDirection="Asc" HeaderText="<b>{0}</b>"
                            ShowGroupColumn="False" ShowGroupSummary="True" />
                    </GroupFields>
                </GroupSettings>
                <ToolBarSettings ShowRefreshButton="True" />
                <PagerSettings PageSize="20" />
                <ExportSettings ExportDataRange="All" ExportHeaders="True"></ExportSettings>
                <AppearanceSettings ShowRowNumbers="False" ShowFooter="true" />
            </cc1:JQGrid>
        </div>
        <div style="display: none; padding-top: 20px" id="divExportData2" runat="server">
            <table class="TextBlack12" cellpadding="3" cellspacing="3">
                <tr>
                    <td>
                        Export
                    </td>
                    <td>
                        <asp:ImageButton Width="22" Height="22" ID="btnCSV2" ImageUrl="Style/images/csv.png"
                            ToolTip="Export to CSV" runat="server" OnClick="btnCSV2_Click" />
                    </td>
                    <td>
                        <asp:ImageButton Width="22" Height="22" ID="btnExcel2" ImageUrl="Style/images/excel.png"
                            ToolTip="Export to Excel" runat="server" OnClick="btnExcel2_Click" />
                    </td>
                    <td>
                        <asp:ImageButton Width="22" Height="22" ID="btnPDF2" ImageUrl="Style/images/pdf.png"
                            ToolTip="Export to PDF" runat="server" OnClick="btnPDF2_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
