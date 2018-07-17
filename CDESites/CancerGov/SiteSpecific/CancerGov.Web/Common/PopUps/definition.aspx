<%@ Page Language="c#" CodeBehind="definition.aspx.cs" AutoEventWireup="True" Inherits="CancerGov.Web.Common.PopUps.Definition" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
   <head id="header" runat="server">
      <asp:Literal ID="DTMTop" Mode="PassThrough" runat="server" />
      <asp:Literal ID="WebAnalytics" Mode="PassThrough" runat="server" />
      <meta name="dcterms.coverage" content="nciglobal,ncienterprise" />
      <meta ID="MetaSubject" name="dcterms.subject" runat="server"/>
      <script type="text/javascript" src="/PublishedContent/js/popEvents.js"></script>
      <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jquery/1.5.1/jquery.min.js" language="JavaScript"></script>
      <link href="/PublishedContent/Styles/nvcg.css" rel="stylesheet" />
      <meta name="robots" content="noindex,nofollow">
   </head>
   <body>
      <div class="popup">
         <div id='dictionary_jPlayer'></div>
         <%=strHeading%>
         <%=this.Content.Render()%>
      </div>
      <asp:Literal ID="DTMBottom" Mode="PassThrough" runat="server" />
   </body>
</html>