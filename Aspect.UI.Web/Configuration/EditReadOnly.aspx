<%@ Page Title="" Language="C#" MasterPageFile="~/Configuration/ConfigurationMaster.Master"
    AutoEventWireup="true" CodeBehind="EditReadOnly.aspx.cs" Inherits="Aspect.UI.Web.Configuration.EditReadOnly" %>

<%@ Import Namespace="Aspect.Model" %>
<asp:Content ContentPlaceHolderID="headPlace" runat="server" ID="Content2">

    <script type="text/javascript">
        $().ready(function() {
            // Vertical splitter. Set min/max/starting sizes for the left pane.
            $("#MySplitter").splitter({
                splitHorizontal: true,
                outline: true,
                sizeLeft: true,
                anchorToWindow: true,
                cookie: 'spliterCookieHorizontalConfigEdit',
                accessKey: "H"
            });
        });
    </script>

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="toolbar">
        <ul>
            <li><a id="SummaryWeight" title="Показать вес разузлованного состава" runat="server" class="btn-showsummaryweight" href="#"></a></li>
            <li><asp:LinkButton ID="PrintSelected" runat="server" CssClass="btn-printselected" OnClick="PrintSelected_Click" /></li>
            <li style="width:5px;">&nbsp;</li>            
            <li><a id="A1" title="Копировать в буфер" class="btn-copyselected" onclick='<%= String.Format("addToConfigurationBuffer(\"{0}\")", SelectedProductsHidden.ClientID) %>' href="#"></a></li>
            <li><a id="ShowBuffer" title="Просмотреть буфер" runat="server" class="btn-showbuffer" href="#"></a></li>            
        </ul>
    </div>
    <div style="float: right; padding: 10px;">
        <asp:Label ID="HeaderLiteral" Font-Bold="false" Font-Size="Large" runat="server"
            Text="Спецификация {0} Версия {1}"></asp:Label>
        &nbsp;&nbsp;
        <asp:Label ID="HeaderDateLiteral" runat="server" Text="(на {0} {1})"></asp:Label>
    </div>
    <div style="clear: both; height: 1px;">
    </div>
    
    <div id="MySplitter">
        <div id="TopPane">
        <asp:ValidationSummary ID="valSummary" runat="server" HeaderText="Ошибки:" ShowSummary="true"
            DisplayMode="BulletList" />
            <asp:HiddenField ID="SelectedProductsHidden" runat="server" />
        <table class="type1" style="width: 100%">
            <tr class="table-header">
                <th>
                    Просмотр
                </th>
            </tr>
        </table>
        <asp:GridView ID="EditConfigurationGrid" runat="server" GridLines="Vertical" BorderColor="LightGray"
            AutoGenerateColumns="false" AllowSorting="false" AllowPaging="false"
            CssClass="type1" BorderStyle="None" OnRowCreated="EditConfigurationGrid_RowCreated">
            <AlternatingRowStyle CssClass="row2" />
            <HeaderStyle ForeColor="Black" Font-Bold="false" HorizontalAlign="Center" Font-Names="Tahoma" />
            <Columns>
                <asp:TemplateField ItemStyle-Width="15px">
                    <ItemTemplate>
                        <asp:HiddenField ID="EntityID" runat="server" Value='<%# Eval( Common.IDColumnTitle ) %>' />                        
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerTemplate>
            </PagerTemplate>
        </asp:GridView>
        
    </div>
    <div id="BottomPane">
        <div style="float:right;padding:30px;">        
        <asp:LinkButton ID="Cancel" runat="server" Text="Отмена" OnClick="Cancel_Click" />&nbsp;&nbsp;&nbsp;
        &nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="SelectAllRecords" runat="server" Text="Выделить все записи" OnClick="SelectAll_Click" />&nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="UnselectAllRecords" runat="server" Text="Отменить выделение" OnClick="UnselectAll_Click" />
        </div>
        <div id="ProductCardPlaceHolder"></div>
    </div>
    </div>
</asp:Content>
