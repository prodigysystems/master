<%@ Page Language="VB" Inherits="System.Web.Mvc.ViewPage(Of OclsConnectionManager.LocalPasswordModel)" %>



<% Using Html.BeginForm("Manage", "Account") %>
    <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary() %>

    <fieldset>
        <legend>Change Password Form</legend>
        <br/>
        <table cellspacing="5px" cellpadding="5px">
            <tr>
            
                <td align="right">  <%: Html.LabelFor(Function(m) m.OldPassword) %>
                </td>
                 <td> <%: Html.PasswordFor(Function(m) m.OldPassword) %>
                </td>
            </tr>
            <tr>
                <td align="right"> <%: Html.LabelFor(Function(m) m.NewPassword) %>
                </td>
                 <td>  <%: Html.PasswordFor(Function(m) m.NewPassword) %>
                </td>
            </tr>
              <tr>
                <td align="right">   <%: Html.LabelFor(Function(m) m.ConfirmPassword) %>
                </td>
                 <td> <%: Html.PasswordFor(Function(m) m.ConfirmPassword) %>
                </td>

            </tr>
        </table>
         <br/>
        <input type="submit" value="Change password" />
    </fieldset>
<% End Using %>
