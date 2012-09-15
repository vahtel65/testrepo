<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.Master" AutoEventWireup="true" CodeBehind="ListWares.aspx.cs" Inherits="Aspect.UI.Web.Technology.ListWares" %>
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
                  <param name="source" value="ListWares.xap"/>\
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
             $('body').layout();
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

         function tb_remove_ext() {
             slCtl.Content.myApp.HideGlobalMask();
             _tb_remove();
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
   <div class="ui-layout-center" style="padding: 0; margin: 24px 0 0 0;"></div>
</asp:Content>
