<%@ Page Language="VB" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage(Of OclsConnectionManager.RegisterModel)" %>

<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Register
</asp:Content>

<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <hgroup class="title">
        <h1></h1>
    </hgroup>
    <% If Not IsNothing(ViewData("msg")) Then%>
    <script type="text/javascript">        alert('<%= ViewData("msg").ToString() %>')</script>
    <% End If%>
    <% Using Html.BeginForm() %>
        <%: Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary() %>

        <fieldset>
            <legend>Registration Form</legend>
             <br/>
              <table>
                <tr>
                    <td align="right">  
                     <%: Html.LabelFor(Function(m) m.UserName) %>
                    </td>
                     <td>  <%: Html.TextBoxFor(Function(m) m.UserName) %>
                     </td>
                </tr>
                <tr>
                    <td align="right">  <%: Html.LabelFor(Function(m) m.Password) %>
                    </td>
                     <td>     <%: Html.PasswordFor(Function(m) m.Password) %>
                     </td>
                </tr>
                 <tr>
                        <td  align="right">  <%: Html.LabelFor(Function(m) m.ConfirmPassword) %>
                        </td>
                        <td>
                                <%: Html.PasswordFor(Function(m) m.ConfirmPassword) %>
                        </td>
                </tr>
                   <tr>
                        <td  align="right">  <%: Html.LabelFor(Function(m) m.Role)%>
                        </td>
                        <td>
                                <%: Html.DropDownListFor(Function(m) m.Role, ViewData("roles"))%>
                        </td>
                </tr>
            </table>
             <br/>
            <input type="submit" value="Register" />
        </fieldset>
    <% End Using %>
</asp:Content>

<asp:Content ID="scriptsContent" ContentPlaceHolderID="ScriptsSection" runat="server">
    <%: Scripts.Render("~/bundles/jqueryval") %>
</asp:Content>
