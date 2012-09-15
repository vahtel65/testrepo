<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.Master" AutoEventWireup="true" CodeBehind="MultiExchange.aspx.cs" Inherits="Aspect.UI.Web.Tools.MultiExchange" %>
<asp:Content ID="Content1" ContentPlaceHolderID="headPlace" runat="server">
    <script type="text/javascript">
        $(document).ready(function() {
            var leftWin;
            var rightWin;

            Ext.QuickTips.init();
            Ext.namespace('App');

            // NOTE: This is an example showing simple state management. During development,
            // it is generally best to disable state management as dynamically-generated ids
            // can change across page loads, leading to unpredictable results.  The developer
            // should ensure that stable state ids are set for stateful components in real apps.    
            Ext.state.Manager.setProvider(new Ext.state.CookieProvider());            

            var btnSelectSlaveProduct = new Ext.Button({
                text: 'Выбрать целевой продукт',
                renderTo: 'btnSelectSlaveProduct',
                autoWidth: true,
                handler: function() {
                    if (!rightWin) {
                        rightWin = new Application.SearchForm({
                            id: 'rightWindow',
                            fieldStore: <%=jsonFields %>                            
                        });                        
                        rightWin.onSelect = function(product) {                            
                            $('#right_superpole').text(product.superpole);
                            $('#right_version').text(product.version);
                            App.right_uid = product.uid;
                        }
                    }
                    rightWin.show(this);
                }
            });

            var btnSelectMainProduct = new Ext.Button({
                text: 'Выбрать исходный продукт',
                renderTo: 'btnSelectMainProduct',
                autoWidth: true,
                handler: function() {
                    if (!leftWin) {
                        leftWin = new Application.SearchForm({
                            id: 'leftWindow',
                            fieldStore: <%=jsonFields %>,                            
                        });
                        leftWin.onSelect = function(product) {                            
                            $('#left_superpole').text(product.superpole);
                            $('#left_version').text(product.version);
                            App.left_uid = product.uid;
                            var grid = Ext.getCmp('usageGrid');
                            grid.store.proxy.conn.extraParams.uid = product.uid;
                            grid.store.load();
                        }
                    }
                    leftWin.show(this);
                }
            });
            
            $("#MySplitter").splitter({
                splitHorizontal: true,
                outline: true,
                anchorToWindow: true,
                sizeBottom: true,
                cookie: 'spliterCookieHorizontalMultiExchange',
                accessKey: "H"
            });
                        
            var rCon = new Ext.data.Connection({
                url: '/Json/UsageConfigurations.aspx',
                method: 'POST',
                id: 'usageConnection',
                extraParams: {
                    'uid': ''
                }
            });

            var store = new Ext.data.JsonStore({
                root: 'rows',
                totalProperty: 'totalCount',
                idProperty: 'threadid',
                remoteSort: true,
                fields: [
                    {name: 'uid'},
                    {name: 'superpole'},
                    {name: 'version', type: 'int'},
                    {name: 'actual'},
                    {name: 'orderyear'},
                    {name: 'ordernumber'}
                ],			                
                proxy: new Ext.data.HttpProxy(rCon),
                listeners: {
                    loadexception: function() {
                        console.log('load failed -- arguments: %o', arguments);
                    }
                },
                 
            });

            var sm = new Ext.grid.CheckboxSelectionModel({
                singleSelect: false,
                checkOnly: true
            });            
            var usagePanel = new Ext.Panel({
                renderTo: 'usage-panel',
                id: 'usagePanel',
                autoHeight: true,
                border: false,
                layout:'fit',
                items:[{
                    xtype: 'grid',
                    id: 'usageGrid',
                    columnLines: true,
                    autoHeight: true,
                    title: 'Использование исходного продукта в спецификациях:',
                    store: new Ext.data.Store(),
                    columns: [
                        sm,
                    {
                        header: 'Общ. наименование',
                        width: 350
                    },{
                        header: 'Версия',
                        width: 150
                    },{
                        header: 'Основная версия',
                        width: 150
                    },{
                        header: 'Год приказа',
                        width: 150
                    },{
                        header: 'Номер приказа',
                        width: 150
                    }],
                    sm: sm,
                    store: store
                }]
            });
            var loadMask = new Ext.LoadMask('usagePanel', {
                msg: 'Загрузка данных',
                store: store
            });
        });
    </script>
    <style type="text/css">
        .FieldValue 
        {
            margin: 2px 2px 2px 20px;
        }        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainPlace" runat="server">
    <div id="MySplitter">
    <div id="TopPane">
        <table height="100%" width="100%">
        <tr height="100px"><td>
            <table width="100%">
                <tr>
                <td width="50%">
                    <div style="border: 1px solid grey;margin:10px;padding:10px;">
                        <table>
                        <tr>
                            <td>Наименования продукта:</td>
                            <td><div class="FieldValue" id="left_superpole">&nbsp;</div></td>
                        </tr>
                        <tr>
                            <td>Версия:</td>
                            <td><div class="FieldValue" id="left_version">&nbsp;</div></td>
                        </tr>
                        </table>
                        <div style="margin-left:10px; float:right" id="btnChangeProduct"></div>
                        <script type="text/javascript">
                            btnChangeProduct = new Ext.Button({
                                text: 'Выполнить замену',
                                renderTo: 'btnChangeProduct',
                                autoWidth: true,
                                handler: function() {
                                    if (!App.left_uid || !App.right_uid) {
                                        Ext.MessageBox.alert("Сообщение", "Необходимо выбрать исходный и целевой продукт для замены.");
                                    } else {
                                        var rows = new Array();
                                        var grid = Ext.getCmp('usageGrid');
                                        grid.getSelectionModel().each(function(row) {
                                            rows.push(row.get('uid'));
                                        });

                                        if (!App.processing_mask) {
                                            App.processing_mask = new Ext.LoadMask(Ext.getBody(), {
                                                msg: 'Выполняется замена. Это может занять значительный промежуток времени. Пожалуйста ожидайте...'
                                            });
                                        }
                                        App.processing_mask.show();

                                        $.ajax({
                                            type: 'POST',
                                            url: '/Json/ExchangeConfiguration.aspx',
                                            data: {
                                                left_uid: App.left_uid,
                                                right_uid: App.right_uid,
                                                products: rows.toString()
                                            },
                                            success: function(data) {
                                                App.processing_mask.hide();
                                                Ext.MessageBox.alert("Сообщение", "Замена завершена.");
                                            }
                                        });
                                    }
                                }
                            })
                        </script>
                        <div style="margin-left:10px; float:right" id="btnSelectMainProduct"></div>                
                        <div style="clear:both" />
                    </div>
                </td>
                <td width="50%">
                    <div style="border: 1px solid grey;margin:10px;padding:10px;">
                        <table>
                        <tr>
                            <td>Наименования продукта:</td>
                            <td><div class="FieldValue" id="right_superpole">&nbsp;</div></td>
                        </tr>
                        <tr>
                            <td>Версия:</td>
                            <td><div class="FieldValue" id="right_version">&nbsp;</div></td>
                        </tr>
                        </table>
                        <div style="float:right" id="btnSelectSlaveProduct"></div>
                        <div style="clear:both" />
                    </div>
                </td>
                </tr>            
            </table>
        </td></tr>
        <tr><td style="height: 100%; vertical-align: top;">
            <div style="margin:10px;height:100%;" id="usage-panel"></div>
        </td></tr>
        </table>        
    </div>
    <div style="height: 50px;" id="BottomPane">
    </div>
    </div>        
</asp:Content>
