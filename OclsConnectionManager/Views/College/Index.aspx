<%@ Page Title="" Language="VB" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage(Of OclsConnectionManager.CollegeListViewModel)" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
OCLS Connection Manager - Colleges
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <script type="text/javascript">

  function GridOnBinding(e) { 

  }
 </script>
   <div class="" style="width: 630px; height: 400px;">
        <br />
    
        <table class="editTemplateTable">
           
            <tr>
                <td colspan="3">
                    <fieldset>
                        <legend>Colleges</legend>
                        <br />
                        <% Html.Telerik().Grid(Model.Colleges) _
                .Name("CollegeGrid") _
                .HtmlAttributes(New With {.style = "width:600px;"}) _
                .NoRecordsTemplate("<br/>") _
                .DataKeys(Sub(k)
                              k.Add("CollegeId").RouteKey("CollegeId")
                          End Sub) _
                            .Selectable(Sub(s) s.Enabled(False)) _
                              .Scrollable(Sub(s) s.Enabled(True).Height(450)) _
                            .DataBinding(Function(db) db.Ajax().Select("SearchCollege", "College").Update("UpdateCollege", "College").Insert("InsertCollege", "College").Delete("DeleteCollege", "College").OperationMode(GridOperationMode.Client)) _
                                            .Footer(False) _
                                            .ToolBar(Function(cmds) cmds.Insert()) _
                                            .Columns(Sub(c)
                                                         c.Bound(Function(db) db.CollegeId).Hidden(True)
                                                         c.Bound(Function(db) db.Code).Title("College Code").Width(100)
                                                         c.Bound(Function(db) db.Name).Title("College Name").Width(300)
                                                     
                                                         c.Command(Function(cmd) cmd.Edit().ButtonType(GridButtonType.BareImage).Text("Edit").InsertText("Insert").UpdateText("Update").CancelText("Cancel"))
                                                         c.Command(Function(cmd) cmd.Delete().ButtonType(GridButtonType.BareImage).Text("Delete"))
                                                     End Sub
                                            ) _
                                            .ClientEvents(Function(m) m.OnComplete("GridOnComplete").OnSave("GridOnSave").OnDelete("GridnDelete").OnDataBinding("GridOnBinding")) _
                                            .Editable(Function(e) e.Mode(GridEditMode.InLine).DisplayDeleteConfirmation(False)) _
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
            var grid = $('#CollegeGrid').data("tGrid");
            grid.rebind();
        }
    }
    function GridOnSave(e) {
        e.preventDefault();
        var data = e.values;
        var name = data["Name"];
        var code = data["Code"];
        var id = data["CollegeId"];
        if (name == "") {
            e.preventDefault();
            alert("Name is required.");
            return false;
        }
        if (code == ""||code == "0") {
            e.preventDefault();
            alert("Code is required.");
            return false;
        }
          var url = '<%= OclsConnectionManager.RouteConfig.GetActionUri("College", "CollegeCodeExist") %>';
          $.post(url, { id: id, code: code }, function (data) {
              var s = data;
              if (s == true) {
                  e.preventDefault();
                  alert("College Code already exists.");
                  return false;
              }
              else {
                  url = '<%= OclsConnectionManager.RouteConfig.GetActionUri("College", "CollegeExist") %>';
                  $.post(url, { id: id, name: name }, function (data) {
                      var s = data;
                      if (s == true) {
                          e.preventDefault();
                          alert("College Name already exists.");
                          return false;
                      }
                      else {
                          if (id == "0") {
                              url = '<%= OclsConnectionManager.RouteConfig.GetActionUri("College", "InsertCollegeCustom") %>';
                              $.post(url, { code: code, name: name }, function (data) {
                                  var s = data;
                                  if (s == true) {
                                      var grid = $("#CollegeGrid").data("tGrid");
                                      grid.rebind();
                                      return true;
                                  }
                                  else {
                                      alert("duplicate college.");
                                  }
                              });
                          }
                          else {
                              url = '<%= OclsConnectionManager.RouteConfig.GetActionUri("College", "UpdateCollegeCustom") %>';

                              $.post(url, { id: id, code: code, name: name }, function (data) {
                                  var s = data;

                                  if (s == true) {
                                      var grid = $("#CollegeGrid").data("tGrid");
                                      grid.rebind();
                                  }
                                  else {
                                      alert("duplicate college.")
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
        var url = '<%= OclsConnectionManager.RouteConfig.GetActionUri("College", "DeleteCollegeCustom") %>'; 
        $.post(url, { name: name }, function (data) {
            var s = data;
            if (s == false) {
                e.preventDefault();
                alert("College is used and cannot be deleted.");
                return false;
            }
            else {
                $("#CollegeGrid").data("tGrid").rebind();
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
