<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.Master" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="Aspect.UI.Web.Loging.View" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="headPlace" runat="server">
    <script type="text/javascript">
        $().ready(function() {
            $("#header").css("display", "none");
        });
        function fnUpdateCard(line) {
            Ext.getCmp('ctl00_mainPlace_Card').load({
                url: '/Callback/ViewLogLine.aspx',
                params: {
                    lineNumber: line,
                    logFile: Ext.getCmp('ctl00_mainPlace_listFilesSelect').getValue()
                }
            });            
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainPlace" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" Theme="Gray" />
    <ext:Hidden runat="server" ID="listFilesSelect" />
    <ext:Viewport ID="Viewport1" runat="server" Layout="border">
        <Items>
            <ext:Panel ID="Panel6" runat="server" Collapsible="true" Layout="Fit" Region="West"
                Split="true" Title="Список лог файлов" Width="175">
                <Items>                    
                    <ext:ListView runat="server" ID="listFiles" SingleSelect="true">
                        <Listeners>
                            <SelectionChange Fn="function(lv) {if (lv.getSelectionCount() == 1) { #{listFilesSelect}.setValue(lv.getSelectedRecords()[0].data.FileName); #{logLinesStore}.reload(); } }" />                            
                        </Listeners>
                        <Store>
                            <ext:Store runat="server" ID="listFilesStore">
                                <Reader>
                                    <ext:JsonReader>
                                        <Fields>
                                            <ext:RecordField Name="FileName" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <Columns>
                            <ext:ListViewColumn Header="Имя файла" DataIndex="FileName" />
                        </Columns>
                    </ext:ListView>                    
                </Items>
            </ext:Panel>
            <ext:Panel Border="false" ID="Panel1" runat="server" Layout="Border" Region="Center" Title="Строки лог файла">
                <Items>
                    <ext:Panel Layout="Fit" runat="server" Region="Center">
                        <Items>
                            <ext:GridPanel runat="server" ColumnLines="true" Border="false" ID="logLines" StripeRows="true">
                            <LoadMask ShowMask="true" Msg="Загрузка файла..." />
                            <Store>
                                <ext:Store runat="server" ID="logLinesStore" OnRefreshData="RefreshLogLines">
                                    <Reader>                                    
                                        <ext:JsonReader>
                                            <Fields>
                                                <ext:RecordField Name="evTime" />
                                                <ext:RecordField Name="evUser" />
                                                <ext:RecordField Name="evLevel" />
                                                <ext:RecordField Name="evObject" />
                                                <ext:RecordField Name="evAction" />
                                                <ext:RecordField Name="evIndex" Type="Int" />
                                            </Fields>                                        
                                        </ext:JsonReader>
                                    </Reader>
                                </ext:Store>
                            </Store>
                            <ColumnModel>
                                <Columns>
                                    <ext:Column DataIndex="evTime" Header="Время" />                            
                                    <ext:Column DataIndex="evUser" Header="Пользователь" />
                                    <ext:Column DataIndex="evLevel" Header="Уровень" />
                                    <ext:Column DataIndex="evObject" Header="Объект" />
                                    <ext:Column DataIndex="evAction" Header="Действие" />
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">                                                                
                                    <Listeners>
                                        <RowSelect Handler="function(selModel,index,row){ fnUpdateCard(row.data.evIndex); }" />
                                    </Listeners>
                                </ext:RowSelectionModel>
                            </SelectionModel>                            
                        </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                    <ext:Panel AutoScroll="true" ID="Card" Split="true" Height="150" Layout="Fit" runat="server" Region="South">
                    </ext:Panel>
                </Items>
            </ext:Panel>
        </Items>
    </ext:Viewport>
</asp:Content>
