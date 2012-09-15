<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.Master" AutoEventWireup="true" CodeBehind="MultiExchange.aspx.cs" Inherits="Aspect.UI.Web.Tools.MultiExchange" %>
<asp:Content ID="Content1" ContentPlaceHolderID="headPlace" runat="server">
    <script type="text/javascript">
        $(document).ready(function() {
        //    alert("hioo;glvuyfcytxckycujlvfgfgvk");
            var leftWin;
            var rightWin;

            Ext.QuickTips.init();
            Ext.namespace('App');

            // NOTE: This is an example showing simple state management. During development,
            // it is generally best to disable state management as dynamically-generated ids
            // can change across page loads, leading to unpredictable results.  The developer
            // should ensure that stable state ids are set for stateful components in real apps.    
         var prov=new Ext.state.CookieProvider() //!@#$%^&&**(()())_
            Ext.state.Manager.setProvider(prov);  
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
          /*          alert("show right window");
                  var x;

                    for (x in rightWin)
                    {
                    alert(x+'  '+x.toString());
                    }  
                   
                     alert(this.top);
                     
            */        
                   // alert(rightWin.width);
                    rightWin.show(this);
                }
            });

            var btnSelectMainProduct = new Ext.Button({
                text: 'Выбрать исходный продукт',
                renderTo: 'btnSelectMainProduct',
                autoWidth: true,
                handler: function() {
              //      alert("leftWinHandleEntered");
                    if (!leftWin) {
                        leftWin = new Application.SearchForm({
                            id: 'leftWindow',
                            fieldStore: <%=jsonFields %>,                            
                        });
                        leftWin.onSelect = function(product) {                     //alert("onSelectLeft");                         
                            $('#left_superpole').text(product.superpole);
                            $('#left_version').text(product.version);
                            App.left_uid = product.uid;
                            var grid = Ext.getCmp('usageGrid');
                            grid.store.proxy.conn.extraParams.uid = product.uid;
                                                                                      //   alert("onSelectLeft_1"); 
                            grid.store.load();
                                                                                     // alert("onSelectLeft_2");
                        }
                                                                                    // alert("leftWinHandleEntered_1");

                    }
                                                                                    // alert("leftWinHandleEntered_2");

                    leftWin.show(this);
                                                                                   // alert("leftWinHandleEntered_3");

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
    sortInfo: { // the default sort //120420
    field: 'superpole',
    direction: 'ASC' //| 'DESC'
    },
                fields: [
                    {name: 'uid'},
                    {name: 'superpole'},
                    {name: 'qdu'}, //120420
                    {name: 'qdu_new'}, //120420
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
                    xtype: 'editorgrid',
                    id: 'usageGrid',
                    columnLines: true,
                    autoHeight: true,
                    title: 'Использование исходного продукта в спецификациях:',
                    store: store,
                 //  store: new Ext.data.Store(),

                      enableColLock:false,
                      clicksToEdit:1
                      ,
                    columns: [
                                    sm,
                                {
                                    header: 'Общ. наименование',
                                    width: 350
                                },{
                                    header: 'Колич-во (было)', 
                                    dataIndex: 'qdu',  //120420
 
                                    width: 150 ,
                                },{
                                    header: 'Колич-во (стало)', 
                                    dataIndex: 'qdu_new',  //120420
 
                                    width: 150 ,
                                    editor: new Ext.form.TextField({  // rules about editing //120420
                                        allowBlank: false,
                                        maxLength: 20
                                    //   , maskRe: /([a-zA-Z0-9\s]+)$/   // alphanumeric + spaces allowed               
                                      })
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
                
       var grid= Ext.getCmp('usageGrid');
        grid.on({
        validateedit: function(e) {
         //    alert('v: Original value = ' + e.originalValue 
          //         + ' New value: ' + e.value 
           //        + ' Row: ' + e.row 
            //       + ' Col: ' + e.column);
            return true;
        },
        afterEdit: function(e) {
          //  alert('a: Original value = ' + e.originalValue 
           //       + ' New value: ' + e.value 
            //      + ' Row: ' + e.row 
            //      + ' Col: ' + e.column);
            return true;
        }
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
                                handler: function () {
                                    if (!App.left_uid || !App.right_uid) {
                                        Ext.MessageBox.alert("Сообщение", "Необходимо выбрать исходный и целевой продукт для замены"); //120420
                                    } else {
                                        var rows = new Array();
                                        var grid = Ext.getCmp('usageGrid');
                                        var qdus_new = new Array(); //120420

                                        var exists_selection //120420

                                        grid.getSelectionModel().each(function (row) {
                                            rows.push(row.get('uid'));
                                            qdus_new.push(row.get('qdu_new')); //120420
                                            exists_selection = 'yes';
                                        });

                                        if (!exists_selection) { //120420
                                            Ext.MessageBox.alert("Сообщение", "Не помечены применяемости, по которым должны выполняться замены.");
                                            return;
                                        };
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
                                                , qdus_new: qdus_new.toString() //120420
                                            },
                                            success: function (data) {
                                                App.processing_mask.hide();
                                                Ext.MessageBox.alert("Сообщение", "Замена завершена.");
                                                
                                               /*
                                                $.ajax({
                                                type: 'POST',
                                                url: '/Tools/MultiExchange.aspx'
                                                });
                                                */

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
