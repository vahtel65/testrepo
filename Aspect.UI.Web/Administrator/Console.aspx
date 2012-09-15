<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.Master" AutoEventWireup="true" CodeBehind="Console.aspx.cs" Inherits="Aspect.UI.Web.Administrator.Console" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headPlace" runat="server">
    <script type="text/javascript">
        $(function() {
            $("#ctl00_mainPlace_MenuPanel_Content").append($("#ctl00_MenuPanel"));
        });
        var loadExample = function(id, url, title) {
            document.title = "Консоль администратора :: " + title;
            contentPanel = Ext.getCmp('ctl00_mainPlace_Content');
            contentPanel.load({
                url: url
            });
        }

    </script>    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainPlace" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" Theme="Gray" />
    <div id="viewPortPlace">
        <ext:Viewport ID="Viewport" runat="server" Layout="border" Margins="25px 0 0 0">
            <Items>
                <ext:Panel ID="MenuPanel" runat="server" Region="North" Border="false" BodyCssCls="overflowVisible" BaseCls="MenuPanel" >
                    <Content>
                        <!-- Container to MainMenu -->
                    </Content>
                </ext:Panel>
                <ext:TreePanel Width="200" ID="TreePanel" runat="server" RootVisible="false" Region="West" Split="true">
                    <Root>
                        <ext:TreeNode runat="server">
                            <Nodes>
                                <ext:TreeNode Text="Общие настройки" Expanded="true">
                                    <Nodes>
                                        <ext:TreeNode Text="Пользователи" Leaf="true">
                                            <CustomAttributes>
                                                <ext:ConfigItem Name="href" Value="/Administrator/Users.aspx" Mode="Value" />
                                            </CustomAttributes>
                                        </ext:TreeNode>
                                    </Nodes>
                                </ext:TreeNode>
                            </Nodes>
                        </ext:TreeNode>                        
                    </Root>
                    <Listeners>
                        <Click Handler="if (node.isLeaf()) { e.stopEvent(); loadExample(node.id, node.attributes.href, node.text); }" />
                    </Listeners>
                </ext:TreePanel>                
                <ext:Panel ID="Content" runat="server" Region="Center">
                    <AutoLoad Mode="IFrame" ShowMask="true" MaskMsg="Загрузка..." />
                </ext:Panel>
            </Items>
        </ext:Viewport>
    </div>
</asp:Content>
