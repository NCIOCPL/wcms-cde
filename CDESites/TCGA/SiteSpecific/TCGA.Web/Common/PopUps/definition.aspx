<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="definition.aspx.cs" Inherits="TCGA.Web.Common.PopUps.Definition" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD id="header" runat="server">
	
	<script src="/PublishedContent/js/popEvents.js" type="text/javascript"></script>
    <script type="text/javascript" language="JavaScript" src="//ajax.googleapis.com/ajax/libs/jquery/1.5.1/jquery.min.js"></script>
    <script src="/PublishedContent/js/jquery.jplayer.min.js" type="text/javascript"></script>
<link rel="stylesheet" href="/PublishedContent/Styles/tcga.css" type="text/css" />
	
  </HEAD>
	<body class="popup-pages">
	    <div class="popup">
			<%=strHeading%>
			<%=this.Content.Render()%>
		</div>
        <div class="row">
    		<a href="Javascript:window.print();"><%=strSendPrinter%></a>
		</div>
	<asp:Literal ID="litOmniturePageLoad" mode="PassThrough" runat="server" />
  </body>
</HTML>
