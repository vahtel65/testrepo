/**
*
* @class ComboTree
* @extends Ext.form.ComboBox
*/
Ext.namespace("Application");
Application.ComboTree = Ext.extend(Ext.form.ComboBox, {
    extStore: null,
    tree: null,
    treeId: 0,
    setValue: function(v) {
        var text = v;
        if (this.valueField) {
            var r = this.findExtRecord(this.valueField, v);
            if (r) {
                text = r.data[this.displayField];
            } else if (this.valueNotFoundText !== undefined) {
                text = this.valueNotFoundText;
            }
        }
        Application.ComboTree.superclass.setValue.call(this, text);
        this.lastSelectionText = text;
        if (this.hiddenField) {
            this.hiddenField.value = v;
        }
        this.value = v;
    },
    initComponent: function() {
        this.treeId = Ext.id();
        this.focusLinkId = Ext.id();
        Ext.apply(this, {
            store: new Ext.data.SimpleStore({
                fields: [],
                data: [[]]
            }),
            editable: false,
            shadow: false,
            mode: 'local',
            triggerAction: 'all',
            maxHeight: 500,
            tpl: '<tpl for="."><div style="height:500px"><div id="'
            + this.treeId + '"></div></div></tpl>',
            selectedClass: '',
            onSelect: Ext.emptyFn,
            valueField: 'uid'
        });

        var treeConfig = {
            border: false,
            rootVisible: true,
            loader: this.loader // loader is assigned here
        };
        Ext.apply(treeConfig, this.treeConfig);
        if (!treeConfig.root) {
            treeConfig.root = new Ext.tree.AsyncTreeNode({
                text: 'treeRoot',
                id: '0'
            });
        }
        this.tree = new Ext.tree.TreePanel(treeConfig);
        this.on('expand', this.onExpand);
        this.tree.on('click', this.onClick, this);
        Application.ComboTree.superclass.initComponent.apply(this,
        arguments);
    },
    findExtRecord: function(prop, value) {
        var record;
        if (this.extStore != null) {
            if (this.extStore.getCount() > 0) {
                this.extStore.each(function(r) {
                    if (r.data[prop] == value) {
                        record = r;
                        return false;
                    }
                });
            }
        }
        return record;
    },
    onClick: function(node) {
        if (node.isLeaf() == false) {
            return;
        }
        if (node.attributes.parameter == 9) {
            // 
        } else {
            // 
            this.setValue(node.text);
            this.hiddenField = node.attributes.uid;         
            this.collapse();
        }
    },
    onExpand: function() {
        this.tree.render(this.treeId);
    }
});

Ext.reg("combotree", Application.ComboTree);