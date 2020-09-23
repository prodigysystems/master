<%@ Page Title="" Language="VB" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage(Of OclsConnectionManager.ConnectionMethodListViewModel)" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
OCLS Connection Manager - Connection Methods
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


   <div class="" style="width: 630px; height: 400px;">
      <br />
        <%: Html.ValidationSummary("The following errors must be corrected:", New With {.class = "validation-summary-errors-flat"})%>
        <table class="editTemplateTable">
           
            <tr>
                <td colspan="3" style="text-align:left">
                    <fieldset>
                        <legend>Connection Methods</legend>
                        <br />
                        <% Html.Telerik().Grid(Model.ConnectionMethods) _
                .Name("ConnectionMethodGrid") _
                .HtmlAttributes(New With {.style = "width:600px;"}) _
                .NoRecordsTemplate("<br/>") _
                .DataKeys(Sub(k)
                              k.Add("ConnectionMethodId").RouteKey("ConnectionMethodId")
                          End Sub) _
                            .Selectable(Sub(s) s.Enabled(False)) _
                             .Scrollable(Sub(s) s.Enabled(True).Height(450)) _
                            .DataBinding(Function(db) db.Ajax().Select("SearchConnectionMethod", "ConnectionMethod").Update("UpdateConnectionMethod", "ConnectionMethod").Insert("InsertConnectionMethod", "ConnectionMethod").Delete("DeleteConnectionMethod", "ConnectionMethod").OperationMode(GridOperationMode.Client)) _
                                            .Footer(False) _
                                            .ToolBar(Function(cmds) cmds.Insert()) _
                                            .Columns(Sub(c)
                                                         c.Bound(Function(db) db.ConnectionMethodId).Hidden(True)
                                                         c.Bound(Function(db) db.Code).Title("Connection Method Code").Width(200)
                                                         c.Bound(Function(db) db.Name).Title("Connection Method").Width(200)
                                                     
                                                         c.Command(Function(cmd) cmd.Edit().ButtonType(GridButtonType.BareImage).Text("Edit").InsertText("Insert").UpdateText("Update").CancelText("Cancel"))
                                                         c.Command(Function(cmd) cmd.Delete().ButtonType(GridButtonType.BareImage).Text("Delete"))
                                                     End Sub
                                            ) _
                                            .ClientEvents(Function(m) m.OnComplete("GridOnComplete").OnSave("GridOnSave").OnDelete("GridnDelete")) _
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
        if (e.name == "update" || e.name == "delete") {
            var grid = $('#ConnectionMethodGrid').data("tGrid");
            grid.rebind();
        }
    }
    function GridOnSave(e) {
        e.preventDefault();
        var data = e.values;
        var name = data["Name"];
        var id = data["ConnectionMethodId"];
        var code = data["Code"];
        if (name == "") {
            e.preventDefault();
            alert("Name is required.");
            return false;
        }
        if (code == "" || code == "0") {
            e.preventDefault();
            alert("Code is required.");
            return false;
        }
        var url = '<%= OclsConnectionManager.RouteConfig.GetActionUri("ConnectionMethod", "ConnectionMethodCodeExist") %>';
        $.post(url, {id:id, code: code }, function (data) {
            var s = data;
            if (s == true) {
                e.preventDefault();
                alert("Connection method code already exists.");
                return true;
            }
            else {
                var url = '<%= OclsConnectionManager.RouteConfig.GetActionUri("ConnectionMethod", "ConnectionMethodExist") %>';
                $.post(url, {id:id, name: name }, function (data) {
                    var s = data;
                    if (s == true) {
                        e.preventDefault();
                        alert("Connection method name already exists.");
                        return true;
                    }
                    else {
                        if (id == "0") {
                            var url = '<%= OclsConnectionManager.RouteConfig.GetActionUri("ConnectionMethod", "InsertConnectionMethodCustom") %>';
                            $.post(url, {code:code, name: name }, function (data) {
                                var s = data;
                                if (s == true) {
                                    var grid = $("#ConnectionMethodGrid").data("tGrid");
                                    grid.rebind();
                                    return true;
                                }
                                else {
                                    alert("duplicate Connection method.");
                                }
                            });
                        }
                        else {
                            var url = '<%= OclsConnectionManager.RouteConfig.GetActionUri("ConnectionMethod", "UpdateConnectionMethodCustom") %>';

                            $.post(url, { id: id, code: code, name: name }, function (data) {
                                var s = data;

                                if (s == true) {
                                    var grid = $("#ConnectionMethodGrid").data("tGrid");
                                    grid.rebind();
                                }
                                else {
                                    alert("duplicate Connection method.")
                                }
                            });
                        }

                    }

                });
            }
        });

 
        return true;
    }
    function GridnDelete(e) {
        if (confirm("Do you want to delete this record?")) {
        }
        else {
            e.preventDefault();
            return true;
        }
        e.preventDefault();
        var item = e.dataItem;
        var name = item.Name
        var url = '<%= OclsConnectionManager.RouteConfig.GetActionUri("ConnectionMethod", "DeleteConnectionMethodCustom") %>'; 
        $.post(url, { name: name }, function (data) {
            var s = data;
            if (s == false) {
                e.preventDefault();
                alert("Connection method is used and cannot be deleted.");
                return true;
            }
            else {
                $("#ConnectionMethodGrid").data("tGrid").rebind();
            }
        });
        return true
    }
    $(function () {
        var gridHeight = $(window).height() - $(".header-content").height() - 400;
        $(".t-grid-content").height(gridHeight);
    });
  </script>
</asp:Content>
