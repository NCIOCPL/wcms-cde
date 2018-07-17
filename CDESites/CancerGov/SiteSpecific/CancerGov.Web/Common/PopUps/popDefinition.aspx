<%@ Page Language="c#" Debug="true" CodeBehind="popDefinition.aspx.cs" AutoEventWireup="True" Inherits="Www.Common.PopUps.PopDefinition" %>
<%@ Import Namespace="NCI.Web.Dictionary.BusinessObjects" %>

<!DOCTYPE html>
<html>
   <head id="header" runat="server">
      <title>Dictionary of Cancer Terms</title>
      <meta content="text/html;charset=ISO-8859-1" http-equiv="content-type" />
      <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
      <meta name="robots" content="noindex, nofollow" />
      <meta name="dcterms.coverage" content="nciglobal,ncienterprise" />
      <meta ID="MetaSubject" name="dcterms.subject" runat="server"/>
      <asp:Literal ID="DTMTop" Mode="PassThrough" runat="server" />
      <asp:Literal ID="WebAnalytics" Mode="PassThrough" runat="server" />
      <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.1.1/jquery.min.js" type="text/javascript"></script>
      <script src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.12.1/jquery-ui.min.js" type="text/javascript"></script>
      <link href="/PublishedContent/Styles/nvcg.css" rel="stylesheet" type="text/css">
      <script src="/PublishedContent/js/Common.js" type="text/javascript"></script>
      <script src="/PublishedContent/js/Popups.js" type="text/javascript"></script>

   </head>
   
   <body>
      <div class="popup">
         <div class="clearfix">
            <div class="popuplogo">
               <a id="logoAnchor" runat="server">
               <img id="logoImage" runat="server" alt="National Cancer Institute" src="/publishedcontent/images/images/design-elements/logos/nci-logo-full.svg"/>
               </a> 
            </div>
            <div class="popup-close">
               <a href="javascript:window.parent.window.close();">
                  <span class="hidden" id="closeWindowText" runat="server" />
               </a>
            </div>
         </div>
         <p>
         </p>
         <div id='dictionary_jPlayer'>
         </div>
         <asp:PlaceHolder ID="phDefinition" runat="server">
            <div class="heading">
               <asp:Literal ID="definitionLabel" runat="server" />
            </div>
            <asp:Repeater ID="termDictionaryDefinitionView" runat="server" OnItemDataBound="termDictionaryDefinitionView_OnItemDataBound">
               <ItemTemplate>
                  <div class="audioPronounceLink">
                     <span class="term">
                     <%# ((DictionaryTerm)(Container.DataItem)).Term%></span>
                     <asp:PlaceHolder ID="phPronunciation" runat="server">
                        <asp:Label ID="pronunciationKey" runat="server" CssClass="pronunciation" />
                        <a id="pronunciationLink" runat="server" class="CDR_audiofile">
                           <span class="hidden">listen</span>
                        </a> 
                     </asp:PlaceHolder>
                  </div>
                  <div class="definition">
                     <%# ((DictionaryTerm)(Container.DataItem)).Definition.Html%>
                  </div>
                  <asp:Panel runat="server" ID="pnlRelatedInfo">
                     <div class="definitionImage">
                        <asp:Repeater ID="relatedImages" runat="server" Visible="false" OnItemDataBound="relatedImages_OnItemDataBound">
                           <ItemTemplate>
                              <figure class="image-left-medium">
                                 <a id="termEnlargeImage" runat="server" target="_blank" class="article-image-enlarge no-resize"/>
                                 <img id="termImage" runat="server" src="" alt="" />
                                 <figcaption>
                                    <div class="caption-container no-resize">
                                       <p><%# ((ImageReference)(Container.DataItem)).Caption  %></p>
                                    </div>
                                 </figcaption>
                              </figure>
                           </ItemTemplate>
                        </asp:Repeater>
                     </div>
                  </asp:Panel>
               </ItemTemplate>
            </asp:Repeater>
         </asp:PlaceHolder>
         <asp:PlaceHolder ID="phNoResult" runat="server" Visible="false">
            <div class="definition">The term you are looking for does not exist in the glossary.</div>
         </asp:PlaceHolder>
      </div>
      <asp:Literal ID="DTMBottom" Mode="PassThrough" runat="server" />
   </body>
</html>
