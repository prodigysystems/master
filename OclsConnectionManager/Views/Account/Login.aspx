<%@ Page Language="VB" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage(Of OclsConnectionManager.LoginModel)" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    OCLS Connection Manager - Login
</asp:Content>
<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <hgroup class="title">
       
    </hgroup>
    <section id="loginForm">
    <h2></h2>
    <% Using Html.BeginForm(New With {.ReturnUrl = ViewData("ReturnUrl")})%>
        <%: Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary(true) %>

        <fieldset>
            <legend>Log in </legend>
             <br/>
            <table>
            <tr>
            <td align="right">  <%: Html.LabelFor(Function(m) m.UserName)%>
            </td>
             <td>  <%: Html.TextBoxFor(Function(m) m.UserName) %>
                    <%: Html.ValidationMessageFor(Function(m) m.UserName) %>
             </td>
            </tr>
            <tr>
            <td align="right"> <%: Html.LabelFor(Function(m) m.Password) %>
            </td>
             <td>   <%: Html.PasswordFor(Function(m) m.Password) %>
                    <%: Html.ValidationMessageFor(Function(m) m.Password) %>
             </td>
            </tr>
             <tr>
            <td colspan="2" align="right">  <%: Html.CheckBoxFor(Function(m) m.RememberMe) %>
                    <%: Html.LabelFor(Function(m) m.RememberMe, New With { .Class = "checkbox" }) %>
                    </td>
          
            </tr>
            </table>
            <br/>
            <input type="submit" value="Log in" />
        </fieldset>
       
    <% End Using%>
    </section>
</asp:Content>
<asp:Content ID="scriptsContent" ContentPlaceHolderID="ScriptsSection" runat="server">
</asp:Content>
