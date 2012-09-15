<%@ Page Title="Продукция : " Language="C#" MasterPageFile="~/SiteMaster.Master"
    AutoEventWireup="true" CodeBehind="Grid.aspx.cs" Inherits="Aspect.UI.Web.Grid" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headPlace" runat="server">
    <script type="text/javascript">
        function refresh() {
            //__doPostBack('ctl00$mainPlace$RefreshState', '');
            Ext.net.DirectMethods.RefreshState({ timeout: 300000 });
        }
        $().ready(function() {
            $("#MySplitter").splitter({
                splitVertical: true,
                outline: true,
                sizeLeft: true,
                cookie: 'spliterCookieVerticalGrid',
                anchorToWindow: true,
                accessKey: "I"
            });
            $("#RightPane").splitter({
                splitHorizontal: true,
                outline: true,
                sizeTop: true,
                cookie: 'spliterCookieHorizontalGrid',
                accessKey: "H"
            });            
        });
    </script>
    <style type="text/css">
    .x-grid3-dirty-cell {
        background-image: none !important;
    }
    </style>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainPlace" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" Theme="Gray">
    </ext:ResourceManager>
    <div class="toolbar">
        <ul>
            <li><a id="ChooseColumns" title="Колонки" runat="server" class="btn-choosecolumns" href="#"></a></li>
            <li><a id="ChooseColumnsOrder" title="Колонки" runat="server" class="btn-choosecolumnsorder" href="#"></a></li>
            <li><a id="ChooseCardFields" title="Карточка" runat="server" class="btn-choosecolumnscard" href="#"></a></li>           
            <li>
                <ext:LinkButton ID="upd1" runat="server" Cls="btn-refresh">
                    <ToolTips>
                        <ext:ToolTip Title="Обновить"></ext:ToolTip>
                    </ToolTips>
                    <Listeners>
                        <Click Handler="ctl00_mainPlace_GridPanel1.getStore().reload();" />
                    </Listeners>
                </ext:LinkButton>
            </li>
           
            <li style="width:5px;">&nbsp;</li>
            <li><a id="ShowSummaryWeight" title="Показать вес разузлованного состава" runat="server" class="btn-showsummaryweight" href="#"></a></li>
            <li><asp:LinkButton ID="PrintSelected" runat="server" CssClass="btn-printselected" OnClick="PrintSelected_Click" /></li>
            <li style="width:5px;">&nbsp;</li>
            <li><asp:LinkButton ID="ShowSelectedProducts" runat="server" CssClass="btn-showselected" ToolTip="Показать выбранные" Text="" OnClick="ShowSelectedProducts_Click" /> </li>
            <asp:PlaceHolder ID="CopyToBufferPlaceHolder" runat="server">
            <li><a id="A1" title="Копировать в буфер" class="btn-copyselected" onclick="addToBuffer('ctl00_mainPlace_hiddenSelectedProducts')" href="#"></a></li>
            </asp:PlaceHolder>            
            <li><a id="ShowBuffer" title="Просмотреть буфер" runat="server" class="btn-showbuffer" href="#"></a></li>
            <asp:PlaceHolder ID="ClearBufferPlaceHolder" runat="server">
            <li><a id="A2" title="Очистить буфер" class="btn-clearbuffer" onclick="clearBuffer()" href="#"></a></li>
            </asp:PlaceHolder>
            <li><asp:LinkButton ID="InsertFromBuffer" runat="server" CssClass="btn-bufferpaste" ToolTip="Вставить из буфера" Text="" OnClick="InsertFromBuffer_Click" /> </li>
            <li><asp:LinkButton ID="DeleteFromClass" runat="server" Visible="false" CssClass="btn-deletefromclass" ToolTip="Удалить из класса" Text="" OnClick="DeleteFromClass_Click" /> </li>
            
            <li><asp:LinkButton ID="AddToFavorites" runat="server" CssClass="btn-fav" ToolTip="В избраные" Text="" OnClick="AddToFavorites_Click" /></li>
            <li><asp:LinkButton ID="SetMainVersion" runat="server" CssClass="btn-setmainversion" ToolTip="Установить признак основной версии" Text="" OnClick="SetMainVersion_Click" /> </li>
            <!--<li><a id="ProductSearch" title="поиск" runat="server" class="btn-search" href="#"></a></li>-->
            <li><a class="btn-multiexchange" href="/Tools/MultiExchange.aspx" title="Инструмент :: Множественная замена" target="_blank"></a></li>
                       
        </ul>
        <div style="float: right; padding-right: 20px;">
            Пользователь: <b>
                <asp:Literal ID="UserName" runat="server" /></b><br />
            <asp:Literal ID="DateTimeLabel" runat="server" />
        </div>
       <asp:HyperLink ID="HyperLink27" runat="server" 
            ImageUrl="~/img/ico_MultiInsert.gif" NavigateUrl="~/Tools/MultiInsert.aspx" 
            ToolTip="Инструмент :: Множественная вставка">HyperLink</asp:HyperLink>
    </div>
    <div id="MySplitter">
        <div id="LeftPane" style="overflow: hidden;">
            <ext:TreePanel AutoScroll="true" ID="ClassView" runat="server" RootVisible="false" Border="false">
                <Root>
                    <ext:TreeNode Text="root" Expanded="true">                    
                    </ext:TreeNode>
                </Root>
                <SelectionModel>
                    <ext:DefaultSelectionModel ID="ClassViewSelModel">
                    </ext:DefaultSelectionModel>
                </SelectionModel>
                <BottomBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <ext:Button ID="Button3" runat="server" Text="Развернуть" Icon="ChartOrganisation">
                                <Listeners>
                                    <Click Handler="#{ClassView}.expandAll();" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="Button4" runat="server" Text="Свернуть" Icon="ChartOrganisationDelete">
                                <Listeners>
                                    <Click Handler="#{ClassView}.collapseAll();" />
                                </Listeners>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </BottomBar>
            </ext:TreePanel>            
        </div>
        <div id="RightPane">
            <div id="TopPane" style="overflow: hidden;">     
                <ext:Hidden runat="server" ID="hiddenSelectedProducts"></ext:Hidden>
                <ext:Panel ID="ProductGridPanel" runat="server" Layout="fit" Border="false">
                    <Items>
                        <ext:GridPanel 
                            ID="GridPanel1"
                            runat="server"                             
                            Border="false"
                            StripeRows="true"
                            ColumnLines="true"
                            TrackMouseOver="true"  
                            AutoScroll="true">                            
                            <Listeners>
                                <AfterEdit Handler="function(event) { if (event.value) fnProdSelect(event.record.data.ID); else fnProdDeselect(event.record.data.ID);}" />
                            </Listeners>
                            <Store>
                               <ext:Store 
                                   ID="Store1" 
                                   runat="server"
                                   WarningOnDirty="false"
                                   RemoteSort="true"                                   
                                   OnRefreshData="Store1_RefreshData">
                                   <Reader>
                                        <ext:ArrayReader></ext:ArrayReader>
                                    </Reader>
                                    <AutoLoadParams>
                                        <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                        <ext:Parameter Name="limit" Value="20" Mode="Raw" />
                                    </AutoLoadParams>
                                    <Proxy>
                                        <ext:PageProxy /> 
                                    </Proxy>
                                    <Listeners>
                                        <Load Handler="function(store, records) {for(i=0;i<records.length;i++) records[i].set('Checked', fnIsProdSelected(records[i].data.ID)); #{GridPanel1}.getSelectionModel().selectFirstRow(); }" />
                                    </Listeners>
                                 <DirectEventConfig Timeout="300000" />
                                </ext:Store>
                            </Store>
                            <ColumnModel ID="ColumnModel1" runat="server">
                                <Columns>
                                    <ext:CheckColumn Hideable="false" Resizable="false" Sortable="false" Locked="true" Editable="true" Header="#" Width="40"  DataIndex="Checked" />
                                    <ext:CommandColumn Hideable="false" Resizable="false" Sortable="false" Locked="true" Align="Center" Width="26">
                                        <Commands>                                            
                                            <ext:GridCommand Icon="NoteEdit" CommandName="Menu">
                                                <ToolTip Text="Меню действий" />
                                            </ext:GridCommand>
                                        </Commands>
                                    </ext:CommandColumn>
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                    <Listeners>
                                        <RowSelect Handler="function(selModel,index,row){onGridViewRowSelectedCallback2(row.data['ID'], #{ClassViewSelModel}.getSelectedNode().id.substr(5)); }" />
                                    </Listeners>
                                </ext:RowSelectionModel>
                            </SelectionModel>
                            <Listeners>
                                <Command Handler="#{RowSelectionModel1}.selectRecords([record]);tb_show('Меню действий','Popup/ProductActions.aspx?height=500&width=400&ID='+record.data.ID+'&CID='+record.data.CID,'');" />
                                <ColumnResize Handler="function(index,size){Ext.net.DirectMethods.OnColumnResize(#{ColumnModel1}.getColumnId(index), size);}" />
                            </Listeners>
                            <BottomBar>
                                <ext:PagingToolbar ID="PagingToolbar1" 
                                    runat="server" 
                                    PageSize="20" 
                                    DisplayInfo="true" 
                                    DisplayMsg="Отображены элементы {0} - {1} из {2}" 
                                    EmptyMsg="Нет элементов для отображения">
                                    <Items>                                        
                                        <ext:ComboBox
                                            ID="FilterView"
                                            runat="server"
                                            Editable="false"
                                            SelectedIndex="0">
                                            <Items>
                                                <ext:ListItem Text="без фильтра" Value="" />
                                                <ext:ListItem Text="только основные" Value="mainVers" />
                                                <ext:ListItem Text="только приказные" Value="prikazVers" />
                                                <ext:ListItem Text="основные и приказные" Value="mainVers prikazVers"/>
                                            </Items>
                                            <Listeners>
                                                <Select Handler="#{Store1}.load({params:{start:0, limit:20}});" />
                                            </Listeners>
                                        </ext:ComboBox>
                                        <ext:Button ID="Button2"
                                            runat="server"
                                            Text="Поиск"
                                            OnClientClick="#{SearchWindow}.show();">
                                        </ext:Button>
                                        <ext:SplitButton ID="SelectInfo" runat="server" Text="Выделено - 0 элементов" IconCls="add16">
                                            <Menu>
                                                <ext:Menu ID="Menu1" runat="server">
                                                    <Items>
                                                        <ext:MenuItem ID="MenuItem1" runat="server" Text="Снять выделение">
                                                            <Listeners>
                                                                <Click Fn="function () {fnProdDeselectAll();}" />
                                                            </Listeners>
                                                        </ext:MenuItem>
                                                    </Items>
                                                </ext:Menu>
                                            </Menu>
                                        </ext:SplitButton>
                                    </Items>
                                </ext:PagingToolbar>
                            </BottomBar>
                            <LoadMask ShowMask="true" Msg="Загрузка..." />
                        </ext:GridPanel>
                    </Items>
                </ext:Panel>
                <script type="text/javascript">
                    function ResizeProductGridPanel() {
                        var uid = "ctl00_mainPlace_ProductGridPanel";
                        var pane = $("#TopPane");
                        Ext.getCmp(uid).setHeight(pane.height());
                        Ext.getCmp(uid).setWidth(pane.width());
                    }
                    function ResizeClassView() {
                        var uid = "ctl00_mainPlace_ClassView";
                        var pane = $("#LeftPane");
                        Ext.getCmp(uid).setHeight(pane.height());
                        Ext.getCmp(uid).setWidth(pane.width());
                    }
                    function fnIsProdSelected(ID) {
                        if (!(ctl00_mainPlace_hiddenSelectedProducts.value))
                            return false;
                        else var selected = ctl00_mainPlace_hiddenSelectedProducts.value.split(',');                        
                        if (selected.indexOf(ID) >= 0)
                            return true;
                        else return false;
                    }
                    function fnProdSelect(ID) {
                        if (ctl00_mainPlace_hiddenSelectedProducts.value != '')
                            var selected = ctl00_mainPlace_hiddenSelectedProducts.value.split(',');
                        else
                            var selected = new Array();
                        selected.push(ID);
                        ctl00_mainPlace_hiddenSelectedProducts.setValue(selected.toString());
                        ctl00_mainPlace_SelectInfo.setText('Выделено - ' + selected.length + ' элементов');
                    }
                    function fnProdDeselect(ID) {
                        var selected = ctl00_mainPlace_hiddenSelectedProducts.value.split(',');
                        selected.splice(selected.indexOf(ID),1);
                        ctl00_mainPlace_hiddenSelectedProducts.setValue(selected.toString());
                        ctl00_mainPlace_SelectInfo.setText('Выделено - ' + selected.length + ' элементов');
                    }
                    function fnProdDeselectAll() {
                        var selected = new Array();
                        ctl00_mainPlace_hiddenSelectedProducts.setValue(selected.toString());
                        ctl00_mainPlace_SelectInfo.setText('Выделено - 0 элементов');


                        ctl00_mainPlace_Store1.each(function(rec) {
                            rec.set('Checked', false);
                        });
                    }
                    Ext.onReady(function() {
                        if (!(ctl00_mainPlace_hiddenSelectedProducts.value))
                            ctl00_mainPlace_hiddenSelectedProducts.setValue('');
                        $("#TopPane").resize(ResizeProductGridPanel);
                        $("#LeftPane").resize(ResizeClassView);
                        ResizeClassView();
                        ResizeProductGridPanel();
                    });
                    function fnHiddenSearchUpdate(cleaning) {
                        var win = Ext.getCmp('ctl00_mainPlace_SearchWindow');
                        var bars = win.find('ctCls', 'searchBar');
                        if (cleaning) {
                            for (i = 0; i < bars.length; i++) {
                                bars[i].find('ctCls', 'condition')[0].setValue("~");
                                bars[i].find('ctCls', 'value')[0].setValue("");
                            }
                            Ext.getCmp('ctl00_mainPlace_hiddenSearch').setValue('');
                        } else {                                                        
                            var searchItems = new Array();
                            for (i = 0; i < bars.length; i++) {
                                var searchItem = new Object({
                                    id: bars[i].find('ctCls', 'identifier')[0].getValue(),
                                    cond: bars[i].find('ctCls', 'condition')[0].getValue(),
                                    value: bars[i].find('ctCls', 'value')[0].getValue()
                                })
                                if (searchItem.value != '')
                                    searchItems.push(searchItem);
                            }
                            var hiddenSearch = new Object({
                                mode: win.getTopToolbar().find('ctCls', 'searchMode')[0].getValue(),
                                items: searchItems
                            });
                            Ext.getCmp('ctl00_mainPlace_hiddenSearch').setValue($.toJSON(hiddenSearch));
                        }
                        win.hide();
                        Ext.getCmp('ctl00_mainPlace_GridPanel1').getStore().load({params:{start:0, limit:20}});
                    }
                </script>
                <ext:Hidden runat="server" ID="hiddenSearch"></ext:Hidden>
                <ext:Window ID="SearchWindow" EnableViewState="false" runat="server" Modal="true" Hidden="true" Height="485" Icon="Zoom"
                    Title="Расширенный поиск элементов" Width="750" AutoScroll="true">
                    <TopBar>
                        <ext:Toolbar>
                            <Items>
                                <ext:HBoxLayout ID="HBoxLayout1" Padding="3" Align="Middle" Pack="Start">
                                    <BoxItems>
                                        <ext:BoxItem Margins="0 10 0 0">
                                            <ext:Label Text="Режим совмещения полей:" />                                            
                                        </ext:BoxItem>
                                        <ext:BoxItem>
                                            <ext:ComboBox CtCls="searchMode" Width="80" Editable="false" SelectedIndex="0">
                                                <Items>
                                                    <ext:ListItem Text="ИЛИ" Value="OR" />
                                                    <ext:ListItem Text="И" Value="AND" />
                                                </Items>
                                            </ext:ComboBox>
                                        </ext:BoxItem>                                        
                                    </BoxItems>
                                </ext:HBoxLayout> 
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <FooterBar>
                        <ext:Toolbar>
                            <Items>
                                <ext:HBoxLayout ID="HBoxLayout2" Padding="3" Align="Middle" Pack="End">
                                    <BoxItems>
                                        <ext:BoxItem>
                                            <ext:Button Icon="Zoom" ID="Button1" runat="server" Text="Поиск">
                                                <Listeners>
                                                    <Click Handler="function(){fnHiddenSearchUpdate(false);}" />
                                                </Listeners>
                                            </ext:Button>
                                        </ext:BoxItem>
                                        <ext:BoxItem>
                                            <ext:Button Icon="Cancel" Margins="0 0 0 15" ID="Button5" runat="server" Text="Отмена">
                                                <Listeners>
                                                    <Click Handler="function(){fnHiddenSearchUpdate(true);}" />
                                                </Listeners>
                                            </ext:Button>
                                        </ext:BoxItem>
                                    </BoxItems>
                                </ext:HBoxLayout>
                            </Items>
                        </ext:Toolbar>
                    </FooterBar>
                </ext:Window>
            </div>            
            <div id="BottomPane">
                <div id="ProductCardPlaceHolder">
                </div>
            </div>
        </div>
    </div>
</asp:Content>
