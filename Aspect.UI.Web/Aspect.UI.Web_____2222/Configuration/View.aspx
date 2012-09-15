<%@ Page Title="" Language="C#" MasterPageFile="~/Configuration/ConfigurationMaster.Master"
    AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="Aspect.UI.Web.Configuration.View" %>

<asp:Content ContentPlaceHolderID="headPlace" runat="server" ID="Content2">

    <script type="text/javascript">
        function refresh() {
            __doPostBack('ctl00$ctl00$mainPlace$ContentPlaceHolder1$RefreshButton', '')
        }
        $().ready(function() {
            // Vertical splitter. Set min/max/starting sizes for the left pane.
            $("#MySplitter").splitter({
                splitHorizontal: true,
                outline: true,
                sizeLeft: true,
                anchorToWindow: true,
                cookie: 'spliterCookieHorizontalConfigView',
                accessKey: "H"
            });
        });
    </script>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="toolbar">
        <ul>
            <li><a id="ChooseColumns" runat="server" class="btn-choosecolumns" href="#"></a></li>
            <li><a id="ChooseColumnsOrder" title="Колонки" runat="server" class="btn-choosecolumnsorder" href="#"></a></li>
            <li><a id="ChooseCardFields" runat="server" class="btn-choosecolumnscard" href="#"></a></li>
            <li> <asp:LinkButton ID="RefreshButton" runat="server" CssClass="btn-refresh" ToolTip="Обновить" OnClick="RefreshButton_Click"></asp:LinkButton></li>
            <li style="width:5px;">&nbsp;</li>
            <li><asp:LinkButton ID="ShowSelectedProducts" runat="server" CssClass="btn-showselected" ToolTip="Показать выбранные" Text="" OnClick="ShowSelectedProducts_Click" /> </li>
            <li><a id="A1" title="Копировать в буфер" class="btn-copyselected" onclick='<%= String.Format("addToBuffer(\"{0}\")", SelectedProductsHidden.ClientID) %>' href="#"></a></li>
            <li><a id="ShowBuffer" title="Просмотреть буфер" runat="server" class="btn-showbuffer" href="#"></a></li>
        </ul>
        <div style="float: right; padding-right: 20px;">
        </div>
    </div>
    <div style="float: right; padding: 10px;">
        <asp:Label ID="HeaderLiteral" Font-Bold="false" Font-Size="Large" runat="server" Text="Состав {0} Версия {1}"></asp:Label>
        &nbsp;&nbsp;
        <asp:Label ID="HeaderDateLiteral" runat="server" Text="(на {0} {1})"></asp:Label>
    </div>
    <div style="clear: both; height: 1px;">
    </div>
    <div id="MySplitter">
        <div id="TopPane">
            <asp:HiddenField ID="SelectedProductsHidden" runat="server" />
            <asp:GridView ID="ProductGrid" runat="server" GridLines="Vertical" BorderColor="LightGray"
                AutoGenerateColumns="false" AllowSorting="true" AllowPaging="false" CssClass="type1"
                BorderStyle="None" OnSorting="ProductGrid_Sorting" OnRowCreated="ProductGrid_RowCreated">
                <AlternatingRowStyle CssClass="row2" />
                <HeaderStyle ForeColor="Black" Font-Bold="false" HorizontalAlign="Center" Font-Names="Tahoma" />
                <Columns>
                    <asp:TemplateField ItemStyle-Width="15px">
                        <ItemTemplate>
                            <asp:HiddenField ID="EntityID" runat="server" Value='<%# Eval( "ID" ) %>' />
                            <asp:CheckBox ID="SelectCheckBox" Visible="true" BorderStyle="None" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
        <div id="BottomPane">
            <div id="ProductCardPlaceHolder">
            </div>
        </div>
    </div>
</asp:Content>
