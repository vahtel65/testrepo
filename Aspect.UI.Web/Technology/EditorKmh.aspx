<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.Master" AutoEventWireup="true" CodeBehind="EditorKmh.aspx.cs" Inherits="Aspect.UI.Web.Technology.EditorKmh" %>
<asp:Content ID="Content1" ContentPlaceHolderID="headPlace" runat="server">
<script type="text/javascript" src="/scripts/Silverlight.js" />
    <script type="text/javascript">
        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
                appSource = sender.getHost().Source;
            }

            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
                return;
            }

            var errMsg = "Необработанная ошибка в приложении Silverlight " + appSource + "\n";

            errMsg += "Код: " + iErrorCode + "    \n";
            errMsg += "Категория: " + errorType + "       \n";
            errMsg += "Сообщение: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "Файл: " + args.xamlFile + "     \n";
                errMsg += "Строка: " + args.lineNumber + "     \n";
                errMsg += "Позиция: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {
                if (args.lineNumber != 0) {
                    errMsg += "Строка: " + args.lineNumber + "     \n";
                    errMsg += "Позиция: " + args.charPosition + "     \n";
                }
                errMsg += "Имя метода: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }
    </script>
    <script type="text/javascript">
        $().ready(function() {
            // Vertical splitter. Set min/max/starting sizes for the left pane.        
            var objectDom = '\
            <object data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="100%">\
	              <param name="enableGPUAcceleration" value="true"/>\
                  <param name="source" value="EditorKmh.xap"/>\
	              <param name="windowless" value="false"/>\
	              <param name="onError" value="onSilverlightError" />\
	              <param name="onLoad" value="pluginLoaded"/>\
	              <param name="background" value="white" />\
	              <param name="minRuntimeVersion" value="5.0.61118.0" />\
	              <param name="autoUpgrade" value="true" />\
	              <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=5.0.61118.0" style="text-decoration:none">\
		              <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Получить Microsoft Silverlight" style="border-style:none"/>\
	              </a>\
            </object><iframe id="_sl_historyFrame" style="visibility:hidden;height:0px;width:0px;border:0px"></iframe>';

            $('body').layout({ applyDefaultStyles: true });
            $(".ui-layout-center").append(objectDom);
        });
     </script>
    <script type="text/javascript">
        var slCtl = null;
        var _tb_remove = null;
        function pluginLoaded(sender) {
            slCtl = sender.getHost();
            _tb_remove = tb_remove;
            tb_remove = tb_remove_ext;
        }
        function setSelectedValue(controlID, id, textID, text) {
            slCtl.Content.myApp.SetSelectedMaterial(controlID, id, text);
        }
        function setSelectedMultiValue(packedList) {
            slCtl.Content.myApp.AddMultiMaterials(packedList);
        }
        function consolelog(v1) {
            console.log(v1);
        }

        function tb_remove_ext() {
            slCtl.Content.myApp.HideGlobalMask();
            _tb_remove();
        }

        function ShowMaterialAppicability(id) {
            var dataUrl = '%2FTechnology%2FApis.aspx%2FApplicabilityMaterials%3Fmaterial_id%3D' + id;
            tb_show1('Применяемость материала', '/Technology/UniGrid.aspx?data=' + dataUrl + '&width=800&height=600&modal=true&TB_iframe=true', '');
        }
        
        function SelectMaterial(id, mode) {            
            if (id == "") {
                // selector for main material
                tb_show1('Выбор материала', '/Popup/Selector.aspx?ID=&ctrlID=&treeID=316c6bc7-d883-44c8-aae0-602f49c73595&textCtrlID=&width=800&height=500&modal=true&TB_iframe=true', '');
            } else {
                // selector for additional material
                tb_show1('Выбор материала', '/Popup/Selector.aspx?mode='+mode+'&ID=&ctrlID=' + id + '&treeID=316c6bc7-d883-44c8-aae0-602f49c73595&textCtrlID=&width=800&height=500&TB_iframe=true', '');
            }                        
        }
    </script>
    <style type="text/css">
        .ui-layout-pane {
            padding: 0 0 22px !important;
            margin: 22px 0 0 !important;
            overflow: hidden !important;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainPlace" runat="server">
    <div class="ui-layout-center uix-layout-center"></div>
    <asp:HyperLink CssClass="invisible" runat="server" ID="linkSelectMaterial" Text="Some tedxt" />        
</asp:Content>
