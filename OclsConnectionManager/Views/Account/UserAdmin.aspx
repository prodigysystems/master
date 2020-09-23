<%@ Page Title="" Language="VB" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage(Of OclsConnectionManager.UserAdminModel)" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
OCLS Connection Manager - Users
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <div class="" style="width: 630px; height: 400px;">
    <br />
      <table class="editTemplateTable">
            
            <tr>
                <td >
                    <fieldset>
                        <legend>User Admin</legend>
                         <br />
                         <b><%= Html.ActionLink("New User", "Register", "Account")%></b>
                          <br />
                          <br />
                        <% Html.Telerik().Grid(Model.Users) _
                            .Name("UserGrid") _
                            .HtmlAttributes(New With {.style = "width:600px;"}) _
                            .NoRecordsTemplate("<br/>") _
                            .DataKeys(Sub(k)
                                          k.Add("UserName").RouteKey("UserName")
                                          'k.Add("Role").RouteKey("Role")
                                      End Sub) _
                            .ClientEvents(Function(m) m.OnComplete("GridOnComplete")) _
                            .Selectable(Sub(s) s.Enabled(False)) _
                              .Scrollable(Sub(s) s.Enabled(True).Height(450)) _
                            .DataBinding(Function(db) db.Ajax().Select("SearchUsers", "Account").Update("UpdateUser", "Account").Insert("InsertUser", "Account").Delete("DeleteUser", "Account").OperationMode(GridOperationMode.Client)) _
                                            .Footer(False) _
                                            .Columns(Sub(c)
                                                         c.Bound(Function(db) db.UserName).Title("UserName").Width(300).ReadOnly(True)
                                                         c.Bound(Function(db) db.Role).Title("Role").Width(200)
                                                         c.Command(Function(cmd) cmd.Edit().ButtonType(GridButtonType.BareImage).Text("Edit").InsertText("Insert").UpdateText("Update").CancelText("Cancel"))
                                                     End Sub
                                            ) _
                                            .Editable(Function(e) e.Mode(GridEditMode.InLine)) _
                                            .Sortable() _
                                            .Pageable(Function(p) p.Enabled(False)) _
                                            .Render()
              
                
                
                        %>
                    </fieldset>
                </td>
            </tr>
        </table>
        </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsSection" runat="server">
<script type="text/javascript">
    function GridOnComplete(e) {
        if (e.name == "update" ||e.name=="delete") {
            var grid = $('#UserGrid').data("tGrid");
                   grid.rebind();
        }
           }
           $(function () {
               var gridHeight = $(window).height() - $(".header-content").height() - 400;
               $(".t-grid-content").height(gridHeight);
           });
        </script>
</asp:Content>
