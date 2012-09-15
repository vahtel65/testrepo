<%@ Page Title="" Language="C#" MasterPageFile="~/Configuration/ConfigurationMaster.Master" AutoEventWireup="true" CodeBehind="TreeEx.aspx.cs" Inherits="Aspect.UI.Web.Configuration.TreeEx" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

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
                 cookie: 'spliterCookieHorizontalConfigTree',
                 accessKey: "H"
             });
         });
    </script>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" Theme="Gray">
    </ext:ResourceManager>
    <div class="toolbar">
        <ul>
            <li><a id="ChooseColumns" runat="server" class="btn-choosecolumns" href="#"></a></li>
            <li><a id="ChooseColumnsOrder" title="Колонки" runat="server" class="btn-choosecolumnsorder" href="#"></a></li>
            <li><a id="ChooseCardFields" runat="server" class="btn-choosecolumnscard" href="#"></a></li>
            <!--<li> <asp:LinkButton ID="RefreshButton" runat="server" CssClass="btn-refresh" ToolTip="Обновить" OnClick="RefreshButton_Click"></asp:LinkButton></li>
            <li style="width:5px;">&nbsp;</li>
            <li><asp:LinkButton ID="ShowSelectedProducts" runat="server" CssClass="btn-showselected" ToolTip="Показать выбранные" Text="" OnClick="ShowSelectedProducts_Click" /> </li>
            <li><a id="A1" title="Копировать в буфер" class="btn-copyselected" onclick='<%= String.Format("addToBuffer(\"{0}\")", "selectedProducts") %>' href="#"></a></li>
            <li><a id="ShowBuffer" title="Просмотреть буфер" runat="server" class="btn-showbuffer" href="#"></a></li>-->
        </ul>
    </div>
    <div style="float:right;padding:10px;">
                <asp:Label ID="HeaderLiteral" Font-Bold="false" Font-Size="Large" runat="server" Text="Дерево состава {0} Версия {1}"></asp:Label>
                &nbsp;&nbsp;
                <asp:Label ID="HeaderDateLiteral" runat="server" Text="(на {0} {1})"></asp:Label>
            </div>
            <div style="clear:both;height:1px;"></div>
    <div id="MySplitter">
        <div id="TopPane">
            <ext:Hidden ID="selectedProducts" runat="server" />
            <ext:TreeGrid ID="ProductTree" runat="server" Width="900" Height="500" Border="false">
                <Root>
                    <ext:TreeNode Text="some" Expanded="true"></ext:TreeNode>
                </Root>                
                <SelectionModel>
                    <ext:DefaultSelectionModel>
                        <Listeners>
                            <SelectionChange Fn="function(ev,node){onGridViewRowSelectedCallback2(node.attributes.ProductID, App.cid); }" />
                        </Listeners>
                    </ext:DefaultSelectionModel>
                </SelectionModel>
                <Listeners>
                    <AfterRender Fn="function(tree) {tree.getSelectionModel().select(tree.getRootNode().childNodes[0]);}" />
                </Listeners>
            </ext:TreeGrid>
            <script type="text/javascript">
                Ext.onReady(function() {
                    $("#TopPane").resize(ResizeProductGridPanel);
                    ResizeProductGridPanel();
                });
                function ResizeProductGridPanel() {
                    var uid = "ctl00_ctl00_mainPlace_ContentPlaceHolder1_ProductTree";
                    var pane = $("#TopPane");
                    Ext.getCmp(uid).setHeight(pane.height());
                    Ext.getCmp(uid).setWidth(pane.width());
                }                
        </script>          
        </div>
        <div id="BottomPane">
            <div id="ProductCardPlaceHolder"></div>
        </div>
    </div>
</asp:Content>
