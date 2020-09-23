<%@ Page Language="VB" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage(Of System.Web.Mvc.HandleErrorInfo)" %>

<asp:Content ID="errorTitle" ContentPlaceHolderID="TitleContent" runat="server">
    OCLS Connection Manager Error
</asp:Content>

<asp:Content ID="errorContent" ContentPlaceHolderID="MainContent" runat="server">
    <hgroup class="title">
        <h2 class="error">An error occurred while processing your request.</h2>
    </hgroup>
    <div>
        <code style="color: #CC6600; font-size: 11px;">
            <%= Model.ControllerName %>/<%= Model.ActionName %><br />
            <%  Dim exp As System.Exception = Model.Exception
                While Not exp Is Nothing  %>
                    <%= exp.Message%>
                    <br /><br />
                <% exp = exp.InnerException
                End While
                %>                    
        </code>
    </div>

</asp:Content>
