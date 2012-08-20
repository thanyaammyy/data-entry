<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="HotelDataEntry.Main" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" language="javascript">
        function firstLogin() {
            $(".fancybox").fancybox({
                width: 370,
                showCloseButton: false,
                helpers: {
                    title: null
                }
            }).trigger('click');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    Hi
</asp:Content>

