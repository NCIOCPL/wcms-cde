<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="popDefinition.aspx.cs"
    Inherits="TCGA.Web.Common.PopUps.PopDefinition" %>

<%@ Import Namespace="NCI.Web.Dictionary.BusinessObjects" %>
<!DOCTYPE html>
<html>
<head id="header" runat="server">
    <title>Dictionary of Cancer Terms</title>

    <link rel="stylesheet" href="/PublishedContent/Styles/tcga.css" type="text/css" />
    <meta content="text/html;charset=ISO-8859-1" http-equiv="content-type" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <meta name="dcterms.coverage" content="ncicssi-strategicscientificinitiatives,ncienterprise" />
    <meta ID="MetaSubject" name="dcterms.subject" runat="server"/>
    <asp:Literal ID="DTMTop" Mode="PassThrough" runat="server" />
    <asp:Literal ID="WebAnalytics" Mode="PassThrough" runat="server" />
    <script src="/PublishedContent/js/modernizr.custom.2.7.1.js" type="text/javascript"></script>
    <script src="/PublishedContent/js/popEvents.js" type="text/javascript"></script>
    <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jquery/1.5.1/jquery.min.js"></script>       
    <script type="text/javascript">
        $(window).load(function() {
            $(window).resize();
        });
    </script>

</head>
<body class="popup-pages">
    <!--[if lt IE 9]>
            <script src="/PublishedContent/js/respond.js"></script>
        <![endif]-->
    <div class="popup">
        <div class="clearfix">
            <div class="popuplogo">
              <a href="/">
                <img onerror="this.src='/PublishedContent/Images/SharedItems/Images/TCGA_54px-Logo.png';this.removeAttribute('onerror');" src="/PublishedContent/images/images/design-elements/logos/TCGA_54.svg" alt="The Cancer Genome Atlas">
              </a>
            </div>

            <div class="popup-close">
                <a href="javascript:window.parent.window.close();"><span class="hidden" id="closeWindowText"
                    runat="server" /></a>
            </div>
        </div>
        <p>
        </p>
        <div id='dictionary_jPlayer'>
        </div>
        <asp:PlaceHolder ID="phDefinition" runat="server">
            <div class="heading">
                <asp:Literal ID="definitionLabel" runat="server" Text="Definition:" />
            </div>
            <asp:Repeater ID="termDictionaryDefinitionView" runat="server" OnItemDataBound="termDictionaryDefinitionView_OnItemDataBound">
                <ItemTemplate>
                    <div class="audioPronounceLink">
                        <span class="term">
                            <%# ((DictionaryTerm)(Container.DataItem)).Term%></span>
                        <asp:PlaceHolder ID="phPronunciation" runat="server">
                            <asp:Label ID="pronunciationKey" runat="server" CssClass="pronunciation" />
                        </asp:PlaceHolder>
                    </div>
                    <p class="definition">
                        <%# ((DictionaryTerm)(Container.DataItem)).Definition.Html%></p>
                    <asp:Panel runat="server" ID="pnlRelatedInfo">
                        <div class="definitionImage">
                            <asp:Repeater ID="relatedImages" runat="server" Visible="false" OnItemDataBound="relatedImages_OnItemDataBound">
                                <ItemTemplate>
                                    <figure class="image-left-medium">
                                    <a id="termEnlargeImage" runat="server" target="_blank" class="article-image-enlarge no-resize">Enlarge</a>
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
