<%@ Page Title="" Language="C#" MasterPageFile="~/Popup/PopupMaster.Master" AutoEventWireup="true" CodeBehind="SelectTreeClass.aspx.cs" Inherits="Aspect.UI.Web.Popup.SelectTreeClass" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headPlace" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" Theme="Gray">
    </ext:ResourceManager>
    <ext:Viewport runat="server" Layout="fit">
        <Items>
            <ext:TreePanel 
                    ID="ClassView" 
                    runat="server" 
                    AutoScroll="true"
                    RootVisible="false"
                    Region="Center">
                    <Root>
                        <ext:TreeNode>                    
                        </ext:TreeNode>
                    </Root>

                    <Listeners>                        
                        <AfterRender Handler="this.getRootNode().firstChild.expand(false, false); this.getSelectionModel().select(this.getRootNode().firstChild);" />
                    </Listeners>                          
                   
                    <BottomBar>
                        <ext:Toolbar runat="server">
                            <Items>
                                <ext:ToolbarFill />
                                <ext:Button
                                    runat="server"    
                                    Text="Выбрать">
                                    <Listeners>
                                        <Click Handler="self.parent.cbSelectClass(#{ClassView}.getSelectionModel().getSelectedNode().id, #{ClassView}.getSelectionModel().getSelectedNode().text); self.parent.tb_remove();" />
                                    </Listeners>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </BottomBar>
            </ext:TreePanel>
        </Items>
    </ext:Viewport>
</asp:Content>
