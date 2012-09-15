function tb_show1(t, u) {
    u = u.replace('productid', "productid=" + selectedProductID);
    tb_show(t, u, '/img/close.gif');
}

var activeRow;
function highLightRow(row) {
    if (row != activeRow) $(row).css("background-color", "#cccccc");
}

function unHighLightRow(row) {
    if (row != activeRow) $(row).css("background-color", "");
}

var selectedProductID;
function onGridViewRowSelectedCallback(p, c, ctrl, tblid) {
    $("#ProductCardPlaceHolder").load("/Callback/ProductCard.aspx", { pid: p, cid: c });
    $("#" + tblid + " tr").css("background-color", "");
    $(ctrl).css("background-color", "#BEE9FF");
    activeRow = ctrl;
    selectedProductID = p;
}

function onGridViewRowSelectedCallback2(p, c) {
    $("#ProductCardPlaceHolder").load("/Callback/ProductCard.aspx", { pid: p, cid: c });
    selectedProductID = p;
}

var selectedProductIDLeft;
function onGridViewRowSelectedCallbackLeft(p, c, ctrl, tblid) {
    $("#ProductCardLeft").load("/Callback/ProductCard.aspx", { pid: p, cid: c });
    $("#" + tblid + " tr").css("background-color", "");
    $(ctrl).css("background-color", "#BEE9FF");
    activeRow = ctrl;
    selectedProductIDLeft= p;
}

var selectedProductIDRight;
function onGridViewRowSelectedCallbackRight(p, c, ctrl, tblid) {
    $("#ProductCardRight").load("/Callback/ProductCard.aspx", { pid: p, cid: c });
    $("#" + tblid + " tr").css("background-color", "");
    $(ctrl).css("background-color", "#BEE9FF");
    activeRow = ctrl;
    selectedProductIDRight = p;
}


function onGridViewRowSelected(ctrl, tblid) {
    $("#" + tblid + " tr").css("background-color", "");
    $(ctrl).css("background-color", "#BEE9FF");
    activeRow = ctrl;
}

function clearSelection(container) {
	$("#" + container).val('');
	window.refresh();
}

function selectProduct(ev, ctrl, p, container) {
    var selectedProducts = new Array();
    if ($("#" + container).val().length > 0) {
        selectedProducts = $("#" + container).val().split(',');
    }
    if (ctrl.checked == false) {
        for (var n = 0; n < selectedProducts.length; n++)
        {
            if(selectedProducts[n] == p)
            {
                selectedProducts.splice(n,1);
            }
        }
    }
    else {
        selectedProducts.push(p);
    }
    $("#" + container).val(selectedProducts.join(','));

    if (!ev) {
        ev = window.event;
    }
    ev.cancelBubble = true;
}


function addToBuffer(container) {
    if ($("#" + container).val().length > 0) {
        $.ajax({
            type: "POST",
            url: "/Callback/AddToBuffer.aspx",
            data: { pid: $("#" + container).val() }
        });
    }
}

function clearBuffer() {
    $.ajax({
        type: "POST",
        url: "/Callback/ClearBuffer.aspx"
    });    
}

function addToConfigurationBuffer(container) {
    if ($("#" + container).val().length > 0) {
        $.ajax({
            type: "POST",
            url: "/Callback/AddToConfigurationBuffer.aspx",
            data: { pid: $("#" + container).val() }
        });
    }
}
/*
function SelectAll(ctrl) {
    var checked_status = ctrl.checked;

    $('input:checkbox').each(function() {
        this.checked = checked_status;
    });
}*/
