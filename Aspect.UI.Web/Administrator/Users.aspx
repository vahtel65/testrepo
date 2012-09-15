<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="Aspect.UI.Web.Administrator.Users" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headPlace" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainPlace" runat="server">
    <ext:ResourceManager ID="ResourceManager" runat="server" Theme="Gray" />
    <ext:Viewport runat="server" Layout="border">
        <Items>
            <ext:GridPanel Border="false" Region="Center" ID="UsersGrid" runat="server">
                <Store>
                    <ext:Store runat="server" ID="UsersStore">
                        <Reader>
                            <ext:JsonReader>
                                <Fields>
                                    <ext:RecordField Name="userID" />
                                    <ext:RecordField Name="userName" />
                                    <ext:RecordField Name="userRoles" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <ColumnModel>
                    <Columns>
                        <ext:Column 
                            Header="Имя пользователя"
                            DataIndex="userName" 
                            Width="150"/>
                        <ext:Column 
                            Header="Роли пользователя"
                            DataIndex="userRoles" 
                            Width="550"/>
                    </Columns>                    
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true">
                    </ext:RowSelectionModel>
                </SelectionModel>
            </ext:GridPanel>
        </Items>
    </ext:Viewport>
    
</asp:Content>
