<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Aspect.UI.Web._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Аспект</title>
    <link id="Link1" runat="server" href="~/css/login.css" rel="Stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
<center>
        <div class="baseContent">
            <div class="divMainContent subBaseContent" style="margin: 92px 0;">
                <div class="divBreadcrumb">
                    <div class="L">
                        <span class="darkgray">
                            <span class="darkgray"><asp:Literal ID="CustomerCompanyName" runat="server" /></span>
                        </span>
                    </div>
                </div>
                
                <div class="subMainContent1">
                    <div class="logoContent">
                        <asp:Image runat="server" ID="CustomerLogo" ImageUrl="~/img/logo_light.gif" />
                    </div>
                    <div class="inputsContent">
                        <div style="inputContent">
                            <asp:ValidationSummary ID="ValidationSummary1" ValidationGroup="AuthenticateValidationGroup" DisplayMode="List" EnableClientScript="true" runat="server" Font-Names="Trebuchet MS" />
                        </div>
                        <div class="inputContent">
                            <%= "Имя пользователя" %>:<br />
                            <asp:TextBox runat="server" ID="UserNameTextBox" Visible="false" MaxLength="100" CssClass="loginBox"></asp:TextBox>
                            <asp:DropDownList runat="server" CssClass="loginBox" ID="UsersDropDown" DataTextField="Name" DataValueField="ID" />
                            <asp:RequiredFieldValidator ID="ValidatorLoginEmpty" ValidationGroup="AuthenticateValidationGroup" runat="server" EnableClientScript="true" ErrorMessage='Укажите имя пользователя' ControlToValidate="UsersDropDown">!</asp:RequiredFieldValidator>
                        </div>
                        <div class="inputContent">
                            <%= "Пароль" %>:<br />
                            <asp:TextBox runat="server" ID="UserPasswordTextBox" MaxLength="100" TextMode="Password" CssClass="loginBox" />
                            <asp:RequiredFieldValidator ID="ValidatorPasswordEmpty" ValidationGroup="AuthenticateValidationGroup" runat="server" EnableClientScript="true" ErrorMessage='Введите пароль' ControlToValidate="UserPasswordTextBox">!</asp:RequiredFieldValidator>
                        </div>
                        <div class="inputContent">
                            <br />
                            <asp:Button ID="LoginButton" CssClass="btnLogin" ValidationGroup="AuthenticateValidationGroup" OnClick="LoginButton_Click" runat="server" Text="Войти" />
                        </div>
                        <div class="C" style="text-align: left">
                            <br />
                            <asp:Literal ID="LastLogin" runat="server" Visible="false" />
                        </div>
                    </div>
                    <div class="C">
                    </div>
                </div>
                <div class="C">
                </div>
                <div class="divMainContent subBaseContent" style="margin-top: 20px;">
                    <div class="divBreadcrumb">
                        <div class="L">
                            <span class="darkgray"><%= "Информация" %></span>
                        </div>
                    </div>
                    <div class="subMainContent1">
                        <table width="100%" border="0" cellpadding="0" cellspacing="0">
                            <tr>
                                <td style="text-align:left; vertical-align:top; padding-left:20px;">
                                    <p>ОАО "ХМЗ" Свет шахтера" производит скребковые конвейеры и перегружатели, шахтные дробилки, предохранительные гидравлические муфты, взрывобезопасные головные аккумуляторные светильники, </p>
                                    <p>сигнализаторы метана, фары электровозов и другое оборудование для горнорудной промышленности. Продукция, выпускаемая предприятием, поставляется более чем в 18 стран мира и </p>
                                    <p>эксплуатируется в самых различных горнотехнических и климатических условиях в угольной, сланцевой, калийной и других добывающих отраслях.</p>
                                </td>
                                <td  style="width:75px">
                                    &nbsp;
                                </td>
                            </tr>
                        </table>
                        <div class="C">
                        </div>
                    </div>
                    <div class="C">
                    </div>
                    <div style="float: right;">
                    </div>
                    <div style="clear: both;">
                    </div>
                </div>
            </div>
        </div>
    </center>
    <asp:CustomValidator ID="ValidatorAuthenticationFaild" Display="None" runat="server" ErrorMessage="Имя пользователя или пароль указаны неправильно" ValidationGroup="AuthenticateValidationGroup" />
    </form>
</body>
</html>
