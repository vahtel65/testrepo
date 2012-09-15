Ext.namespace("Application");
Application.SearchCondition = Ext.extend(Ext.Toolbar, {
    initComponent: function() {
        Ext.apply(this, {
            layout: 'hbox'
        });
        Ext.apply(this, {
            getSearchCondition: function() {
                return {
                    'Concat': this.el.child('.app-concat').getValue(),
                    'Alias': Ext.getCmp(this.el.child('.app-fieldid').id).hiddenField,
                    'Condition': this.el.child('.app-condition').getValue(),
                    'Value': this.el.child('.app-searchvalue').getValue()
                };
            }
        });
        Ext.applyIf(this, {
            items: [{
                xtype: 'button',
                width: 22,
                icon: '/images/application/plus-circle.png',
                handler: function(btn) {
                    toolbar = btn.ownerCt;
                    panel = toolbar.ownerCt;
                    form = panel.ownerCt;
                    panel.insert(panel.items.indexOf(toolbar) + 1, new Application.SearchCondition());
                    panel.setHeight(panel.items.length * 27);
                    form.doLayout();
                }
            }, {
                xtype: 'button',
                width: 22,
                icon: '/images/application/minus-circle.png',
                handler: function(btn) {
                    toolbar = btn.ownerCt;
                    panel = toolbar.ownerCt;
                    form = panel.ownerCt;
                    if (panel.items.length > 1)
                        panel.remove(toolbar);
                    panel.setHeight(panel.items.length * 27);  
                    form.doLayout(true);                      
                }
            }, {
                xtype: 'combo',
                cls: 'app-concat',
                margins: '1 0 0 4',
                typeAhead: true,
                triggerAction: 'all',
                editable: false,
                value: 'AND',
                lazyRender: true,
                width: 60,
                mode: 'local',
                store: new Ext.data.ArrayStore({
                    id: 0,
                    fields: [
						'myId',
						'displayText'
					],
                    data: [[1, 'AND'], [2, 'OR']]
                }),
                valueField: 'myId',
                displayField: 'displayText'
            }, {
                xtype: 'combotree',
                cls: 'app-fieldid',
                margins: '1 0 0 4',
                hiddenField: '_dictNomen_superpole',
                value: 'Общ. наименование',
                width: 270,
                treeConfig: {
                    rootVisible: false,
                    height: 'auto',
                    animate: true,
                    useArrows: true,
                    autoScroll: true,
                    dataUrl: '/Json/DictsPropsTree.aspx'
                }
            }, {
                xtype: 'combo',
                cls: 'app-condition',
                margins: '1 0 0 4',
                typeAhead: true,
                triggerAction: 'all',
                editable: false,
                value: '~',
                lazyRender: true,
                width: 50,
                mode: 'local',
                store: new Ext.data.ArrayStore({
                    id: 1,
                    fields: [
						'myId',
						'displayText'
					],
                    data: [[1, '>'], [2, '<'], [3, '='], [4, '!='], [5, '~'], [6, '!~']]
                }),
                valueField: 'myId',
                displayField: 'displayText'
            }, {
                xtype: 'textfield',
                cls: 'app-searchvalue',
                margins: '1 2 0 4',
                flex: 1
}]
            });
            Application.SearchCondition.superclass.initComponent.call(this);
        }
    });
Ext.reg("applicationSearchCondition", Application.SearchCondition);

Application.SearchForm = Ext.extend(Ext.Window, {
    initComponent: function() {
        Ext.apply(this, {
            title: 'Выбор продукта',
            closeAction: 'hide',
            width: 800,
            height: 700,
            onSelect: function () { alert("Действие onSelect не обработано."); },
            layout: 'vbox',
            layoutConfig: {
                align: 'stretch',
                pack: 'start'
            }
        });
        Ext.apply(this, {
            items: [{
                xtype: 'panel',
                cls: 'listSearchCondition',
                //layout: 'vbox',
                border: false,
                items: [{
                    xtype: 'applicationSearchCondition'
}],
                    listeners: {
                        add: function(self, cmp, index) {
                            /*if (cmp.initialConfig.xtype == 'applicationSearchCondition')
                            {                                                                                    
                            console.log(index);
                            self.setHeight(27+27*index);
                            }*/
                        }
                    }
                },
		   {
		       //search params
		       xtype: 'toolbar',
		       layout: 'hbox',
		       items: [



            /*
			    {
			        xtype: 'checkbox',
			        checked: true,
			        margins: '3 0 0 4',
			        boxLabel: 'ограничить поиск 2000 элементов',
			        flex: 1
			    },
               
                */
                
                 {
			        xtype: 'button',
			        text: 'Искать',
                    margins: '0 7 0 5',
			        handler: function(btn) {
			            /* Формирование запроса с условиями */
			            var jsonConditions = new Array();

			            form = btn.ownerCt.ownerCt;
			            var listConditions = form.findByType('applicationSearchCondition');
			            for (i = 0; i < listConditions.length; i++) {
			                jsonConditions.push(listConditions[i].getSearchCondition());
			            }

			            var rCon = new Ext.data.Connection({
			                url: '/Json/SearchByConditions.aspx',
			                method: 'POST',
			                extraParams: {
			                    'searchConditions': $.toJSON(jsonConditions),
			                    'maxResult': 0
			                }
			            });

			            var getFieldByAlias = function(Alias) {
			                for (ii = 0; ii < form.fieldStore.length; ii++) {
			                    if (form.fieldStore[ii].Alias == Alias) {
			                        return form.fieldStore[ii];
			                    }
			                }
			            };

			            var existsField = function(array, uid) {
			                for (ii = 0; ii < array.length; ii++) {
			                    if (array[ii].uid == uid) return true;
			                }
			                return false;
			            }

			            var fields = new Array({name: 'uid'});
			            var columns = new Array();

                        /* Обязательное поле -- Вырать */
                        columns.push(columns[0]);
                        columns[0] = new Object({
                            xtype: 'actioncolumn',
                            sortable: false,                          
                            width: 30,
                            items: [{
                                icon   : '/images/default/grid/pick-button.gif',
                                tooltip: 'Выбрать',
                                handler: function(grid, rowIndex, colIndex) {
                                    var rec = store.getAt(rowIndex);
                                    var form = grid.ownerCt.ownerCt;                                    
                                    form.onSelect({
                                        //uid: rec.get('uid'),
                                        superpole: rec.get('_dictNomen_superpole'),
                                        version: rec.get('p_vers'),
                                        uid: rec.get('uid')
                                    });                                    
                                    form.hide();
                                    //var rec = store.getAt(rowIndex);
                                    //alert("Общ. наименование: " + rec.get('_dictNomen_superpole'));
                                }
                            }]
                        });
			            
			            /* Обязательное поле -- Общ. наименование */
			            var orgfield = getFieldByAlias('_dictNomen_superpole');
			            fields.push({
			                name: orgfield.Alias,
			                type: orgfield.Type,
			                uid: orgfield.Uid
			            });
			            var column = new Object({
			                header:     orgfield.Caption,
			                dataIndex:  orgfield.Alias,
			                                         //   sortable: true,   //120420
                            width:      400
			            });			            
                        columns.push(column);

			            /* Обязательное поле -- Версия */
			            var orgfield = getFieldByAlias('p_vers');
			            fields.push({
			                name: orgfield.Alias,
			                type: orgfield.Type,
			                uid: orgfield.Uid
			            });
			            columns.push({
			                header:     orgfield.Caption,
			                dataIndex:  orgfield.Alias,
                                                      //   sortable: false,  //120420
			                width:      100
			            });

			            for (i = 0; i < jsonConditions.length; i++) {
			                var orgfield = getFieldByAlias(jsonConditions[i].Alias);
			                var field = new Object();
			                field.name = orgfield.Alias;
			                field.type = orgfield.Type;
			                field.uid = orgfield.Uid;
			                var column = new Object({
    			                header: orgfield.Caption,
			                    dataIndex: orgfield.Alias			                
			                });
			                if (orgfield.Type == 'boolean'){
			                    column.renderer = function (v) {return (v=='1')?'Да':'Нет';};
			                }

			                if (!existsField(fields, field.uid)) {
			                    columns.push(column);
			                    fields.push(field);
			                }
			            }




			            var store = new Ext.data.JsonStore({
			                root: 'rows',
			                totalProperty: 'totalCount',
			                idProperty: 'threadid',

			                fields: fields,
                            			                remoteSort: true //120420
/*
,
    sortInfo: { // the default sort //120420
    field: '_dictNomen_superpole',
    direction: 'ASC' //| 'DESC'
}
*/
,
			                proxy: new Ext.data.HttpProxy(rCon),
			                listeners: {
                                loadexception: function() {
                                    console.log('load failed -- arguments: %o', arguments);
                                }
                            },
                             
			            });
			           store.setDefaultSort('_dictNomen_superpole', 'asc');//120420
                                                                        
                        var columnModel = new Ext.grid.ColumnModel({
                            columns: columns,
                            defaults: { width: '200', sortable: true }//120420
                        });
                        
                        // находим или создаём грид с результатами поиска
                        var grid = form.find('itemCls', 'SearchResultGrid')[0];
                        if (!grid) {
                            grid = new Ext.grid.GridPanel({
			                    title: 'Результаты поиска:',
			                    itemCls: 'SearchResultGrid',
			                    border: false,
			                    columnLines: true,
			                    store: store,
			                    loadMask: {msg:"Загрузка данных..."},
			                    colModel: columnModel,
                                bbar: new Ext.PagingToolbar({
                                    pageSize: 2000, //120420
                                    displayInfo: true,
                                    store: store,
                                    cls: 'PagingToolBar',
                                    displayMsg: 'Отображено продуктов {0} - {1} из {2}',
                                    emptyMsg: "Нет продуктов для отображения"                                
                                })
			                });
			                var searchPanel = form.find('itemCls', 'SearchGridPanel')[0];
	    		            searchPanel.add(grid);
    			            searchPanel.doLayout();
                        } else {
                            var oldstore = grid.getStore();
                            grid.getBottomToolbar().bindStore(store);
                            grid.reconfigure(store, columnModel);
                            oldstore.destroy();
                        }
			            store.load({ params: { start: 0, limit: 2000} }); //120420
			            return;

			        }
}]
		   }, {
		       xtype: 'panel',
		       itemCls: 'SearchGridPanel',
		       layout: 'fit',
		       border: false,
		       flex: 1,
}]
            });
            Application.SearchForm.superclass.initComponent.call(this);
        }
    });
Ext.reg("applicationSearchForm", Application.SearchForm); 