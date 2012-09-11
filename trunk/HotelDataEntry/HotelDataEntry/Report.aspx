<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="Report.aspx.cs" Inherits="HotelDataEntry.Report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
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
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" id="hiddenDateFrom" runat="server" />
    <input type="hidden" id="hiddenDateTo" runat="server" />
    <div class="headerMenuLabel">
        Report Summary
    </div>
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
                    <input type="text" id="dateFrom" name="dateFrom" />
                    <asp:Label ID="lbDateFrom" Visible="False" CssClass="asteric" runat="server">*</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    To
                </td>
                <td>
                    <input type="text" id="Text1" name="dateFrom" />
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
                    <asp:Button runat="server" ID="btnCreateForm" Text="Show" 
                        onclick="btnCreateForm_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Label Visible="False" runat="server" ID="lbError" Text="Please select a required feilds"
                        CssClass="redText"></asp:Label>
                </td>
            </tr>
        </table>
        <asp:ObjectDataSource ID="PropertyDataSource" DataObjectTypeName="HotelDataEntryLib.Property"
            SelectMethod="ListCompany" TypeName="HotelDataEntryLib.Page.PropertyHelper" runat="server">
        </asp:ObjectDataSource>
    </div>
</asp:Content>
