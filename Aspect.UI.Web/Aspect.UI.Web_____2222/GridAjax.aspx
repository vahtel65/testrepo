<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.Master" AutoEventWireup="true" CodeBehind="GridAjax.aspx.cs" Inherits="Aspect.UI.Web.GridAjax" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headPlace" runat="server">    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainPlace" runat="server">    
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <ext:Viewport ID="Viewport1" runat="server" Layout="border" Margins="10">
        <Items>
            <ext:Panel ID="Panel1" runat="server" Collapsible="True" Height="100" Region="North"
                Split="True" Title="North">
                <Items>
                </Items>
            </ext:Panel>
            <ext:Panel ID="Panel2" runat="server" Collapsible="true" Layout="Fit" Region="East"
                Split="true" Title="East" Width="175">
                <Items>
                    <ext:TabPanel ID="TabPanel1" runat="server" ActiveTabIndex="0" Border="false" TabPosition="Bottom"
                        Title="Title">
                        <Items>
                            <ext:Panel ID="Panel3" runat="server" Title="Tab 1">
                                <Items>                                    
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel4" runat="server" Title="Tab 2">
                                <Items>
                                </Items>
                            </ext:Panel>
                        </Items>
                    </ext:TabPanel>
                </Items>
            </ext:Panel>

            <ext:Panel ID="Panel6" runat="server" Collapsible="true" Layout="Fit" Region="West"
                Split="true" Title="Раздел" Width="175">
                <Items>
                    <ext:TreePanel ID="tree" runat="server" Border="false">
                    </ext:TreePanel>    
                </Items>
            </ext:Panel>
            <ext:Panel ID="Panel9" runat="server" Layout="Fit" Region="Center" Title="Center">
                <Items>
                    <ext:TabPanel ID="TabPanel2" runat="server" ActiveTabIndex="0" Border="false" Title="Center">
                        <Items>
                            <ext:Panel ID="Panel10" runat="server" Closable="true" Title="Tab 1">
                                <Items>
                                    <ext:PagingToolbar ID="PagingToolbar1" runat="server"
                                        PageSize="20"
                                        PageIndex="2"
                                    >
                                    </ext:PagingToolbar>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel11" runat="server" Title="Tab 2">
                                <Items>
                                </Items>
                            </ext:Panel>
                        </Items>
                    </ext:TabPanel>
                </Items>
            </ext:Panel>
        </Items>
    </ext:Viewport>
</asp:Content>
