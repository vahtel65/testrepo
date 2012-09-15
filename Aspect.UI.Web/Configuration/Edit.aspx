<%@ Page Title="" Language="C#" MasterPageFile="~/Configuration/ConfigurationMaster.Master"
    AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Aspect.UI.Web.Configuration.Edit" %>

<%@ Import Namespace="Aspect.Model" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ContentPlaceHolderID="headPlace" runat="server" ID="Content2">
    <script type="text/javascript">
        function refresh() {
            Ext.getCmp('ctl00_ctl00_mainPlace_ContentPlaceHolder1_GridPanel').getSelectionModel().selectFirstRow();
        }
        function updateEditLocker(delay) {
            /*$.ajax({
                type: "POST",
                url: "/Callback/UpdateLocker.aspx?userid=<%=this.User.ID %>&targetid=<%=this.ProductID %>",
                data: {},
                contentType: "application/html; charset=utf-8",
                success: function(msg) {
                }
            });
            setTimeout('updateEditLocker(' + delay + ')', delay);*/
        }
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

            // timer to update locker
            //setTimeout('updateEditLocker(50000)', 50000);
        });

        var getRowClass = function(record) {
            if (record.get('ConfID') == '00000000-0000-0000-0000-000000000000') {
                return "insertedRow";
            }
        };       
    </script>
    <style type="text/css">
        .insertedRow 
        {
        	background-color:#aaFF88 !important;
        }
        .x-grid3-row-selected.insertedRow
        {
        	background-color:#99FF66 !important;
        }
        .x-grid3-row-over.insertedRow
        {
        	background-color:#CCFFAA !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <ext:ResourceManager runat="server" Theme="Gray" />
    <div class="toolbar">
        <ul>
            <li><a id="ChooseCardFields" title="Карточка" runat="server" class="btn-choosecolumnscard" href="javascript:tb_show1('Колонки','/Popup/UserColumns.aspx?cid=fb4d4ab8-a746-4186-82b8-c2ac77ac7e52&productid&fph=GridCard&url=&caller=&KeepThis=true&TB_iframe=true&width=700&height=500&modal=true','');"></a></li>
            <li style="width:5px;">&nbsp;</li>
            <li><a id="SummaryWeight" title="Показать вес разузлованного состава" runat="server" class="btn-showsummaryweight" href="#"></a></li>
            <li><asp:LinkButton ID="PrintSelected" runat="server" CssClass="btn-printselected" OnClick="PrintSelected_Click" /></li>
            <li style="width:5px;">&nbsp;</li>
            <li><a id="A1" title="Копировать в буфер" class="btn-copyselected" onclick='<%= String.Format("addToConfigurationBuffer(\"{0}\")", "ctl00_ctl00_mainPlace_ContentPlaceHolder1_hiddenSelectedProducts") %>' href="#"></a></li>
            <li><a id="ShowBuffer" title="Просмотреть буфер" runat="server" class="btn-showbuffer" href="#"></a></li>
            <asp:PlaceHolder ID="ClearBufferPlaceHolder" runat="server">
            <li><a id="A2" title="Очистить буфер" class="btn-clearbuffer" onclick="clearBuffer()" href="#"></a></li>
            </asp:PlaceHolder>
            <li><asp:LinkButton ID="AddFromBuffer" runat="server" CssClass="btn-bufferpaste" ToolTip="Вставить из буфера" Text="" OnClick="AddFromBuffer_Click" /> </li>
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
        <div id="TopPane"  style="overflow: hidden;">
        <table class="type1" style="width: 100%; height: 30px;">
            <tr class="table-header">
                <th>
                    <asp:Label runat="server" ID="ModeLabel"></asp:Label>
                </th>
            </tr>
        </table>
        <ext:Hidden ID="hiddenStoreData" runat="server" />
        <ext:Hidden runat="server" ID="hiddenSelectedProducts"></ext:Hidden>
        <ext:GridPanel
            ID="GridPanel"
            runat="server"
            Width="1000"
            Height="400"
            Border="false"
            StripeRows="true"
            ColumnLines="true"
            TrackMouseOver="true"            
            AutoScroll="true">
            <SelectionModel>
                <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                    <Listeners>
                        <RowSelect Handler="function(selModel,index,row){onGridViewRowSelectedCallback2(row.data['ID'],'fb4d4ab8-a746-4186-82b8-c2ac77ac7e52');}" />
                    </Listeners>
                </ext:RowSelectionModel>
            </SelectionModel>
            <Store>
                <ext:Store
                    ID="Store1"
                    runat="server">
                    <Reader>
                        <ext:ArrayReader />
                    </Reader>
                    <Listeners>
                        <Remove Handler="updateHiddenStoreDate(store)" />
                        <Load Handler="function(store, records) {updateHiddenStoreDate(store);for(i=0;i<records.length;i++) records[i].set('Checked', fnIsProdSelected(records[i].data.ConfID));}" />                        
                    </Listeners>                   
                    <DirectEventConfig Timeout="300000" />
                </ext:Store>                
            </Store>
            <View>
                <ext:GridView ID="GridView1" runat="server">
                    <GetRowClass Fn="getRowClass" />                       
                </ext:GridView>
            </View>            
            <ColumnModel ID="ColumnModel1">
                <Columns>
                    <ext:CheckColumn Hideable="false" Resizable="false" Sortable="false" Locked="true" Css="background-image:none !important;" Header="#" Editable="true" Width="40" DataIndex="Checked" />
                    <ext:CommandColumn Hideable="false" Resizable="false" Sortable="false" Locked="true" Align="Center" Width="48">
                        <Commands>
                            <ext:GridCommand Icon="NoteEdit" CommandName="Menu">
                                <ToolTip Text="Меню действий" />
                            </ext:GridCommand>
                            <ext:GridCommand Icon="Delete" CommandName="DeleteRow">
                                <ToolTip Text="Удалить из спецификации" />
                            </ext:GridCommand>
                        </Commands>
                    </ext:CommandColumn>
                </Columns>
                <Listeners>
                    <HiddenChange Handler="function(model, index, hidden){Ext.net.DirectMethods.OnColumnHiddenChange(#{ColumnModel1}.getColumnId(index), hidden);}" />
                    <ColumnMoved Handler="function(model, oldIndex, newIndex){Ext.net.DirectMethods.OnColumnMoved(fnGetColumnsOrder(#{ColumnModel1}.columns));}" />
                </Listeners>
            </ColumnModel>
            <Listeners>
                <ColumnResize Handler="function(index,size){Ext.net.DirectMethods.OnColumnResize(#{ColumnModel1}.getColumnId(index), size);}" />
                <AfterEdit Handler="function(ev) {if (ev.field == 'Checked')
	                {if (ev.value) fnProdSelect(ev.record.data.ConfID); else fnProdDeselect(ev.record.data.ConfID);}
                    else updateHiddenStoreDate(#{Store1});}" />
                <Command Handler="function(ev,param){if (ev=='DeleteRow') #{Store1}.remove(param); if (ev=='Menu') {
                    #{RowSelectionModel1}.selectRecords([param]);
                    tb_show('Меню действий','/Popup/ProductActions.aspx?height=500&width=400&ID='+param.data.ID+'&CID=00000000-0000-0000-0000-000000000000','');} }" />                
               <ViewReady  Handler="#{GridPanel}.getSelectionModel().selectFirstRow();" />
            </Listeners>
            <BottomBar>
                <ext:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <ext:Button ID="Button1" Icon="Accept" Text="Выделить всё" runat="server">
                            <Listeners>
                                <Click Handler="#{Store1}.each(function(rec) { rec.set('Checked', true); fnProdSelect(rec.get('ConfID')); });" />
                            </Listeners>
                        </ext:Button>
                        <ext:Button ID="Button3" Icon="Decline" Text="Снять выделение" runat="server">
                            <Listeners>
                                <Click Handler="#{Store1}.each(function(rec) { rec.set('Checked', false); fnProdDeselect(rec.get('ConfID')); });" />
                            </Listeners>
                        </ext:Button>
                        <ext:Button ID="Button4" Text="Управление позициями" runat="server">
                            <Listeners>
                                <Click Handler="#{PositionManipulateWindow}.show();" />
                            </Listeners>
                        </ext:Button>
                    </Items>
                </ext:Toolbar>
            </BottomBar>
            <LoadMask ShowMask="true" />
        </ext:GridPanel>
        <ext:Window ID="PositionManipulateWindow" Title="Управление позициями" runat="server" Hidden="true" EnableViewState="false" Width="500" Height="300" Padding="5">
            <Items>
                <ext:FormLayout ID="FormLayout4" runat="server" LabelWidth="120">
                    <Anchors>
                        <ext:Anchor Horizontal="100%">
                            <ext:RadioGroup ID="RadioGroup1" runat="server" ItemCls="x-check-group-base" FieldLabel="Функция">
                                <Items>
                                    <ext:Radio ID="SelSetPosition" runat="server" BoxLabel="Проставить позиции" Checked="true">                                        
                                    </ext:Radio>
                                    <ext:Radio ID="SelNullPosition" runat="server" BoxLabel="Обнулить позиции">
                                        <Listeners>
                                            <Check Handler="function(ar1, state){ #{TextInterval}.setDisabled(state); #{TextFrom}.setDisabled(state);}" />
                                        </Listeners>
                                    </ext:Radio>
                                </Items>
                            </ext:RadioGroup> 
                        </ext:Anchor>                        
                    </Anchors>                    
                    <Anchors>
                        <ext:Anchor Horizontal="100%">
                            <ext:RadioGroup ID="RadioGroup2" runat="server" ItemCls="x-check-group-base" FieldLabel="Область действия">
                                <Items>
                                    <ext:Radio ID="SelToChecked" runat="server" BoxLabel="Для выделенных" Checked="true" />
                                    <ext:Radio ID="SelToAll" runat="server" BoxLabel="Для всех"  />                                    
                                </Items>
                            </ext:RadioGroup> 
                        </ext:Anchor>                        
                    </Anchors> 
                    <Anchors>
                        <ext:Anchor Horizontal="100%">
                            <ext:TextField ID="TextInterval" FieldLabel="С интервалом" Text="1" runat="server"></ext:TextField>
                        </ext:Anchor>
                    </Anchors>
                    <Anchors>
                        <ext:Anchor Horizontal="100%">
                            <ext:TextField ID="TextFrom" FieldLabel="Начиная с №" Text="1" runat="server"></ext:TextField>
                        </ext:Anchor>
                    </Anchors>
                </ext:FormLayout>
            </Items>
            <FooterBar>
                <ext:Toolbar runat="server">
                    <Items>
                        <ext:HBoxLayout ID="HBoxLayout2" Padding="3" Align="Middle" Pack="End">
                            <BoxItems>
                                <ext:BoxItem>
                                    <ext:Button Icon="Accept" ID="Button2" runat="server" Text="Проставить">
                                        <Listeners>
                                            <Click Handler="fnAutoPosition(#{SelSetPosition}.getValue(), #{SelToAll}.getValue(), #{TextInterval}.getValue(), #{TextFrom}.getValue()); " />
                                        </Listeners>
                                    </ext:Button>
                                </ext:BoxItem>
                                <ext:BoxItem>
                                    <ext:Button Icon="Cancel" Margins="0 0 0 15" ID="Button5" runat="server" Text="Закрыть">
                                        <Listeners>
                                            <Click Handler="#{PositionManipulateWindow}.hide();" />
                                        </Listeners>
                                    </ext:Button>
                                </ext:BoxItem>
                            </BoxItems>
                        </ext:HBoxLayout>
                    </Items>
                </ext:Toolbar>
            </FooterBar>
        </ext:Window>
        <script type="text/javascript">
            Ext.onReady(function() {
                if (!(ctl00_ctl00_mainPlace_ContentPlaceHolder1_hiddenSelectedProducts.value))
                    ctl00_ctl00_mainPlace_ContentPlaceHolder1_hiddenSelectedProducts.setValue('');
                $("#TopPane").resize(ResizeProductGridPanel);
                ResizeProductGridPanel();
            });
            function ResizeProductGridPanel() {
                var uid = "ctl00_ctl00_mainPlace_ContentPlaceHolder1_GridPanel";
                var pane = $("#TopPane");
                Ext.getCmp(uid).setHeight(pane.height()-30);
                Ext.getCmp(uid).setWidth(pane.width());
            }
            function updateHiddenStoreDate(store) {
                var arr = new Array();
                var recs = store.getRange();
                for (i = 0; i < recs.length; i++) arr.push(recs[i].data);
                ctl00_ctl00_mainPlace_ContentPlaceHolder1_hiddenStoreData.setValue($.toJSON(arr));
            }
            function fnIsProdSelected(ID) {
                if (!(ctl00_ctl00_mainPlace_ContentPlaceHolder1_hiddenSelectedProducts.value))
                    return false;
                else var selected = ctl00_ctl00_mainPlace_ContentPlaceHolder1_hiddenSelectedProducts.value.split(',');
                if (selected.indexOf(ID) >= 0)
                    return true;
                else return false;
            }
            function fnProdSelect(ID) {
                if (ctl00_ctl00_mainPlace_ContentPlaceHolder1_hiddenSelectedProducts.value != '')
                    var selected = ctl00_ctl00_mainPlace_ContentPlaceHolder1_hiddenSelectedProducts.value.split(',');
                else
                    var selected = new Array();
                selected.push(ID);
                ctl00_ctl00_mainPlace_ContentPlaceHolder1_hiddenSelectedProducts.setValue(selected.toString());
            }
            function fnProdDeselect(ID) {
                var selected = ctl00_ctl00_mainPlace_ContentPlaceHolder1_hiddenSelectedProducts.value.split(',');
                selected.splice(selected.indexOf(ID), 1);
                ctl00_ctl00_mainPlace_ContentPlaceHolder1_hiddenSelectedProducts.setValue(selected.toString());
            }
            function fnGetColumnsOrder(columns) {
                var idList = "";
                for (var i = 0; i < columns.length; i++) {
                    if (columns[i].id.substr(0,3) != "ext") idList = idList + "," + columns[i].id;                     
                }
                return idList.substr(1, idList.length - 1);
            }
            function fnAutoPosition(doSet, toAll, interval, from) {

                interval = parseInt(interval);
                currentPosition = parseInt(from);                
                currentItem = 1;

                var store = Ext.getCmp("ctl00_ctl00_mainPlace_ContentPlaceHolder1_GridPanel").getStore();
                var recs = store.getRange();
                for (i = 0; i < recs.length; i++) {
                    // skip unchecked rows
                    if (!toAll && !recs[i].get('Checked')) continue;

                    if (doSet) {
                        // set new position
                        recs[i].set('Position', currentPosition);
                    } else {
                        // null position
                        recs[i].set('Position', 0);
                    }

                    currentItem += 1;
                    currentPosition += interval;
                }

                updateHiddenStoreDate(store);
            }
            
        </script>
    </div>
    <div id="BottomPane">
        <div style="float:left;padding:30px;">
            <asp:CheckBox ID="MadeBasicVersion" runat="server" Text="Сделать основной версией при сохранении." Checked="false" />
            <asp:Label ID="ReasonChangesLavel" runat="server" Text="Основание изменений:"></asp:Label>
            <asp:TextBox ID="ReasonChanges" runat="server" Width="300px" />
        </div>
        <div style="float:right;padding:30px;">
        <asp:Label ID="LabelErrorMessage" ForeColor="Red" runat="server" Text="" />&nbsp;&nbsp;&nbsp;        
        &nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="Save" runat="server" Text="Сохранить" OnClick="Save_Click" />&nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="Cancel" runat="server" Text="Отмена" OnClick="Cancel_Click" />&nbsp;&nbsp;&nbsp;
        </div>        
        <div id="ProductCardPlaceHolder" style="clear:both"></div>
    </div>
    </div>
</asp:Content>
