﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="Revenue.aspx.cs" Inherits="HotelDataEntry.DataEntry" %>

<%@ Register TagPrefix="cc1" Namespace="Trirand.Web.UI.WebControls" Assembly="Trirand.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ui-datepicker-calendar
        {
            display: none;
        }
    </style>
    <script type="text/javascript" language="javascript">
        function firstLogin() {
            $(".fancybox").fancybox({
                'width': 432,
                'closeBtn': false,
                'modal': true,
                'height': 'auto',
                scrolling: 'no',
                helpers: {
                    title: null
                }
            }).trigger('click');
        }

        function validateCurrency(value, column) {
            var pattern = /(?:^\d{1,3}(?:\.?\d{2})*(?:,\d{2})?$)|(?:^\d{1,3}(?:,?\d{3})*(?:\.\d{2})?$)/;
            if (pattern.test(value))
                return [true, ""];
            else
                return [false, "Please enter a valid number format"];
        }

        function validateRooms(value, column) {
            var pattern = /^\d+(\.\d{1,2})?$/;
            if (pattern.test(value))
                if(value>100)
                    return [false, "The occupancy should not more than 100%"];
                else
                    return [true, ""];
            else
                return [false, "Please enter a valid number format"];
        }

        function disableDate(value, column) {
            $("#PositionDate").attr("disabled", "true");
        }

        $(document).ready(function () {
            $('input[name="calendar"]').blur();
            $('input[name="calendar"]').datepicker({
                changeMonth: true,
                changeYear: true,
                yearRange: "2008:2020",
                dateFormat: 'mm/yy',
                onClose: function (dateText, inst) {
                    var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                    var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                    $(this).datepicker('setDate', new Date(year, month, 1));
                    var intMonth = parseInt(month) + 1;
                    var strMonth = intMonth >= 10 ? intMonth : 0 + "" + intMonth;
                    var my = strMonth + "/" + year;
                    document.getElementById("<%= hiddenMonthYear.ClientID %>").value = my;
                }
            });

            $(".various").fancybox({
                maxWidth: 1200,
                maxHeight: 500,
                fitToView: true,
                width: '90%',
                height: '80%',
                autoSize: false,
                openEffect: 'none',
                closeEffect: 'none',
                showCloseButton  : true,
                overlayShow: true,
                hideOnOverlayClick: false,
                hideOnContentClick: false,
                enableEscapeButton: false,
                helpers: {
                    title: null
                }
            });

        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" id="hiddenMonthYear" runat="server" />
    <div class="headerMenuLabel">
        Daily Revenue Entry</div>
    <div>
        <table class="TextBlack12" cellpadding="3" cellspacing="3">
            <tr>
                <td>
                    Property
                </td>
                <td>
                    <asp:DropDownList ID="ddlCompany" ToolTip="Select a property" DataSourceID="PropertyDataSource"
                        AutoPostBack="True" OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged"
                        DataValueField="PropertyId" DataTextField="PropertyCode" Width="150" OnDataBound="CurrencyLabel_DataBound"
                        runat="server">
                    </asp:DropDownList>
                    <asp:Label ID="lbCompany" Visible="False" CssClass="asteric" runat="server">*</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Position Date
                </td>
                <td>
                    <%if (Session["MonthYear"] == null)
                      {
                    %>
                    <input type="text" id="calendar" name="calendar" />
                    <%}
                      else
                      {%>
                    <input type="text" id="Text1" name="calendar" value="<%= Session["MonthYear"].ToString() %>" />
                    <% } %>
                    <asp:Label ID="lbCalendar" Visible="False" CssClass="asteric" runat="server">*</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Currency
                </td>
                <td>
                    <asp:Label runat="server" OnLoad="CurrencyLabel_DataBound" ID="lbCurerncy"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Button runat="server" ID="btnCreateForm" OnClick="btnCreateForm_Click" Text="Show" />
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <div id="divReport" runat="server" style="display: none">
                        <a data-fancybox-type="iframe"  href="Reports.aspx" class="various">Switch to Month-To-Date View</a>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Label Visible="False" runat="server" ID="lbError" Text="Please select the required feilds"
                        CssClass="redText"></asp:Label>
                </td>
            </tr>
        </table>
        <asp:ObjectDataSource ID="PropertyDataSource" DataObjectTypeName="HotelDataEntryLib.Property"
            SelectMethod="AccessProperty" TypeName="HotelDataEntryLib.Page.PropertyHelper"
            runat="server">
            <SelectParameters>
                <asp:SessionParameter Name="userId" SessionField="userId" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <div style="padding-top: 20px; display: none" runat="server" id="divJqgrid">
            <asp:UpdatePanel ID="updatepanel1" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <cc1:JQGrid ID="JqGridRevenueEntry" AutoWidth="True" runat="server" Height="80%"
                        OnRowEditing="JqGridDataEntry_RowEditing" oninit="JqGridRevenueEntry_Init">
                        <Columns>
                            <cc1:JQGridColumn DataField="RevenueId" Searchable="False" PrimaryKey="True" Width="55"
                                Visible="False" />
                            <cc1:JQGridColumn DataField="HotelRevenueId" Searchable="False" Width="55" Visible="False" />
                            <cc1:JQGridColumn HeaderText="Date" DataField="PositionDate" Editable="true" DataType="DateTime"
                                TextAlign="Center" DataFormatString="{0:dd/MM/yy}" FooterValue="Total:">
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn CssClass="occupied" Width="200" HeaderText="Occupancy (%)" DataField="OccupancyRoom"
                                Editable="True" TextAlign="Right">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:CustomValidator ValidationFunction="validateRooms" />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn Width="200" HeaderText="Room Revenue" DataField="RoomRevenue" Editable="True"
                                DataFormatString="{0:#,##0.00;(#,##0.00);0}" TextAlign="Right">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:CustomValidator ValidationFunction="validateCurrency" />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="F&amp;B Revenue" DataField="FBRevenue" Editable="True"
                                DataFormatString="{0:#,##0.00;(#,##0.00);0}" TextAlign="Right">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:CustomValidator ValidationFunction="validateCurrency" />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="Spa Revenue" DataField="SpaRevenue" Editable="True"
                                DataFormatString="{0:#,##0.00;(#,##0.00);0}" TextAlign="Right">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:CustomValidator ValidationFunction="validateCurrency" />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="Others" DataField="Others" Editable="True" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:CustomValidator ValidationFunction="validateCurrency" />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="Total" DataField="Total" Editable="False" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="Budget" DataField="Budget" Editable="False" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                            </cc1:JQGridColumn>
                        </Columns>
                        <ToolBarSettings ShowRefreshButton="True" ShowEditButton="True" />
                        <EditDialogSettings Modal="True" Width="350" TopOffset="500" LeftOffset="500" CloseAfterEditing="True"
                            Caption="Edit Revenue Entry"></EditDialogSettings>
                        <PagerSettings PageSize="32" />
                        <AppearanceSettings ShowRowNumbers="true" ShowFooter="true" HighlightRowsOnHover="True" />
                        <ClientSideEvents AfterEditDialogShown="disableDate"></ClientSideEvents>
                    </cc1:JQGrid>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>