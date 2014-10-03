<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VisualizarAnalise.aspx.cs" Inherits="Startup.VisualizarAnalise" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Agora - Traderdata Webstockchart</title>
    <style type="text/css">
        .main
        {
        	margin: 10,10,10,10;
        	font-family: Verdana;
        	font-size: medium;
        }
        .logo
        {
        	border: 0px;
        	margin: 10px;
        } 
        .separador
        {
        	height:1px;
        	background-color: #5BA55B;
        	font-size: xx-small;
        }
        .tabela
        {
        	margin: 10px;
        	background-color: #F9F9F9;;
        }
        .style1
        {
            font-family: Verdana;
        }
        .style2
        {
            font-family: Verdana;
            font-size: x-small;
        }
        .style3
        {
            width: 108px;
        }
        </style>
</head>
<body style="margin:0; padding:0;">
    <form id="form1" runat="server" method="post">
    <asp:Image ID="imgGrafico" Width="1024px" runat="server" /><br />
    <asp:Label style="margin:10px;" ID="lblMensagem" runat="server" class="title" 
        Font-Size="Small" Font-Names="Verdana" ForeColor="#333333" Font-Bold="True"></asp:Label><br />
        <asp:Label ID="lblPublicadorData" style="background-color:#F9F9F9; margin:11px;" 
        runat="server" Font-Names="Verdana" Font-Size="Smaller" ForeColor="Gray"></asp:Label>
        <div style="margin:10px; font-size:x-small; font-family:Verdana;">ATENÇÃO. Leia <a href="<%=ConfigurationSettings.AppSettings["LINK-DISCLAIMER"]%>" target=_blank>aqui</a> as declarações do(s) analista(s) responsáveis pela elaboração desta recomendação, nos termos da Instrução CVM nº 483. </div>
    </form>
</body>
</html>
