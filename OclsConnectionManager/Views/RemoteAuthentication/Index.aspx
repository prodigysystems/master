<%@ Page Title="" Language="VB" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage(Of OclsConnectionManager.RemoteAuthenticationListViewModel)" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    OCLS Connection Manager - Remote Authentication
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<%= Html.Telerik().ScriptRegistrar().OnDocumentReady("OnReady();") %>

    <script type="text/javascript">

        var isEditCommand = false;
        var copyraid = 0;

        function OnReady() {

            $('#SearchForm').keypress(function (e) {

                ///check for enter key code 13
                if (e.which == 13) {

                    e.stopImmediatePropagation();

                    var clearButtonId = $(this).attr('clearButton');

                    if ($('#' + clearButtonId).is(':focus')) {
                        $('#' + clearButtonId).click();
                    }
                    else {

                        var defaultButtonId = $(this).attr("defaultbutton");
                        $("#" + defaultButtonId).click();
                    }
                    return false;
                }
            });
        }

        function ClearSearch() {
            $('#DatabaseNameTerm, #UrlTerm, #DomainNameTerm').val('');
            $('#CollegeNameTerm').prop('selectedIndex', 0);
            SearchRemoteAuthentications();
        }

        function SearchRemoteAuthentications() {
            var grid = $("#RemoteAuthenticationsGrid").data("tGrid");
            grid.rebind();
        }

        function RemoteAuthenticationsGrid_OnDataBinding(e) {
                e.data = {
                    databaseName: $("#DatabaseNameTerm").val(),
                    url: $("#UrlTerm").val(),
                    useFullUrl: ($('#UseFullUrl').is(':checked')),
                    domainName: $("#DomainNameTerm").val(),
                    collegeId: $("#CollegeNameTerm").val(),
                    copyRaId:copyraid
                };
                copyraid = 0;
            }

            function RemoteAuthenticationsGrid_OnDataBound(e) {
                if (isEditCommand) {
                    $("#RemoteAuthenticationsGrid").data("tGrid").editRow($('#RemoteAuthenticationsGrid .t-grid-content table tr:first'));
                }
            }

        function RemoteAuthenticationsGrid_OnEdit(e) {
            $(e.form)
                 .closest(".t-window")
                 .data("tWindow")
                 .center();

            ///Save the current page index of the grid to restore after edit
            var grid = $('#RemoteAuthenticationsGrid').data("tGrid");
            if (grid) {
                $('#hdCurrentPageIndex').val(grid.currentPage);
            }

            var mode = e.mode;
            if (mode == "insert") {
                var form = e.form;
                var chk = $(form).find("#IsActive");
                chk.prop('checked', true);
                var ddl = $(form).find("#ConnectionMethodId");              
                var posturl = '<%= OclsConnectionManager.RouteConfig.GetActionUri("RemoteAuthentication", "GetConnectionMethodIdByName") %>';
                $.post(posturl, { name: "IPnTicket" }, function (data) {
                    ddl.val(data);
                });
               
            }
        }

        function RAOnDelete(e) {
            e.preventDefault();
            var item = e.dataItem;
            var id = item.RemoteAuthenticationId
            var data = e.values;
            var posturl = '<%= OclsConnectionManager.RouteConfig.GetActionUri("RemoteAuthentication", "DeleteRACustom") %>'; 
            $.post(posturl, { id: id }, function (data) {
                $("#RemoteAuthenticationsGrid").data("tGrid").rebind();
            });
            return true
        }

        function GridOnComplete(e) {
            if (e.name == "insert") {
                e.preventDefault();
                var grid = $("#RemoteAuthenticationsGrid").data("tGrid");
                grid.rebind();
            }
        }

        var dbName;

        function GridOnCommand(e) {
            isEditCommand = false;

            if (e.name == "cancel") {
                var posturl = '<%= OclsConnectionManager.RouteConfig.GetActionUri("RemoteAuthentication", "ClearSession") %>'; 
                $.post(posturl, {}, function (data) {
                });
            }
            if (e.name == "copy") {
                e.preventDefault();
                var grid = $("#RemoteAuthenticationsGrid").data("tGrid");
                var item = e.dataItem;
                var raId = item.RemoteAuthenticationId;
                copyraid = raId;
                isEditCommand = true;
                grid.rebind();
            }
        }

        function onRowDataBound(e) {

            $(e.row)
              .find(".t-grid-delete").off('click');

            $(e.row)
              .find(".t-grid-delete")
              .click(function (e) {
                  if (confirm("Do you want to delete this record?")) {
                  } else {
                  
                      e.stopPropagation(); // prevent the grid deletion code from executing.
                  }
              });
          }

        function GridOnSave(e) {
            e.preventDefault();
            var raId = $("#RemoteAuthenticationId").val();
            var dbId = $("#EzProxyDatabaseId").val();
            var collegeId = $("#CollegeId").val();
            var connectionMethodId = $("#ConnectionMethodId").val();
            var isActive = $("#IsActive").val();
            var url = $("#UrlEdit").val();
            var fullUrl = $("#FullUrlEdit").val();
            var domainName = $("#DomainNameEdit").val();
            var campusRestriction = $("#CampusRestrictionEdit").val();

            if ($('#IsActive').is(':checked'))
                isActive = true;
            else
                isActive = false;

            var data = e.values;
            if (dbId == "0" || collegeId == "0" || connectionMethodId == "0" || url == '' || domainName == '') {
                alert("Database, College, URL, Domain, and Connection Method are required.");
                return false;
            }

            //validate campusRestriction formatting
            var reCommaSeparatorCheck = /^\w(\s*,?\s*\w)*$/; //check formatting for comma separators
            var reNoSpaceCheck = /^\S+$/; //check that spaces aren't used as separators
            campusRestriction = campusRestriction.trim().toUpperCase();
            if (campusRestriction != "" &&
               (reCommaSeparatorCheck.test(campusRestriction) == false || reNoSpaceCheck.test(campusRestriction) == false)) {
                alert("Campuses are not correctly formatted.\n" +
                      "Use a comma separated value list without any spaces.\n\n" +
                      "Examples:\n" +
                      "a)   PROGRESS\n" +
                      "b)   PROGRESS,MORNINGSIDE,ASHTONBEE");
                return false;
            }
            if (campusRestriction.length > 1000) {
                alert("Campuses exceeded field size: 1000 characters max.");
                return false;
            }

            data["DomainName"] = "";
            data["Url"] = "";
            data["FullUrl"] = "";
            data["CampusRestriction"] = "";
            e.values = data;
            if (raId == "0") {
                var posturl = '<%= OclsConnectionManager.RouteConfig.GetActionUri("RemoteAuthentication", "InsertRACustom") %>'; 
                $.post(posturl, { dbId: dbId, collegeId: collegeId, url: url, fullUrl: fullUrl, domainName: domainName, campusRestriction: campusRestriction, connectionMethodId: connectionMethodId, isActive: isActive }, function (data) {
                    var s = data;
                    if (s == true) {
                        var grid = $("#RemoteAuthenticationsGrid").data("tGrid");
                        grid.rebind();
                        return true;
                    }
                    else {
                        alert("Remote authentication record already exists for this combination.");
                    }
                });
            }
            else {
                var posturl = '<%= OclsConnectionManager.RouteConfig.GetActionUri("RemoteAuthentication", "UpdateRACustom") %>';

                $.post(posturl, { raId: raId, dbId: dbId, collegeId: collegeId, url: url, fullUrl: fullUrl, domainName: domainName, campusRestriction: campusRestriction, connectionMethodId: connectionMethodId, isActive: isActive }, function (data) {
                    var s = data;

                    if (s == true) {
                        var grid = $("#RemoteAuthenticationsGrid").data("tGrid");
                        grid.rebind();

                        ////Restore page index after saving or cancel R.A. popup.
                        if ($('#hdCurrentPageIndex').val() != "" && $('#hdCurrentPageIndex').val() != "0")
                            grid.pageTo($('#hdCurrentPageIndex').val());
                        else
                            grid.pageTo(1); ////Default Page Index
                    }
                    else {
                        alert("Remote authentication record already exists for this combination.")
                    }
                });
            }
            return true;
        }
    </script>
    <div id="">
    <br />
        <table class="editTemplateTable">
            <tr>
                <td>
                    <fieldset>
                        <legend>Remote Authentication</legend>
                        <p>
                            Enter your criteria and click "Search" to find existing Remote Authentication definitions.
                        </p>

                        <% Using (Html.BeginForm("", "", FormMethod.Post, New With {.defaultbutton = "btnSearch", .clearButton="btnClear", .id = "SearchForm"}))%>
                        <table style="width: 100%;">
                            <tr>
                                <td style="width: 23%;">
                                    College:&nbsp;
                                    <%: Html.DropDownListFor(Function(m) m.CollegeNameTerm, ViewData("AvailableColleges"))%>
                                </td>
                                <td style="text-align:right; width: 20%;">
                                    Database:&nbsp;
                                    <%= Html.TextBoxFor(Function(m) m.DatabaseNameTerm, New With {.style = "width:90px;"})%>
                                </td>
                                <td style="width: 20%; text-align:right;">
                                    Url:&nbsp;
                                    <%= Html.TextBoxFor(Function(m) m.UrlTerm, New With {.style = "width:90px;", .maxLength = "1000"})%>
                                    &nbsp;Full <%=Html.CheckBoxFor(Function(m) m.UseFullUrl) %>
                                </td>
                                <td style="width: 20%;text-align:right;">
                                    Domain:&nbsp;
                                    <%= Html.TextBoxFor(Function(m) m.DomainNameTerm, New With {.style = "width:90px;", .maxLength = "1000"})%>
                                </td>
                                <td style="text-align: right;">
                                    <input type="hidden" id="hdCurrentPageIndex" />
                                    <input type="button" value="Search" id="btnSearch" onclick="SearchRemoteAuthentications();"
                                        class="t-button" style="width: 55px" />
                                        &nbsp;
                                        <input type="button" value="Clear" id="btnClear" onclick="ClearSearch();" class="t-button" style="width: 55px" />
                                </td>
                            </tr>
                        </table>
                        <%End Using %>

                        <br /><!-- change grid width back to 965px and remoteautheticationid to hidden 2013.1.219.340 -->
                        <% 
                            Html.Telerik().Grid(Model.RemoteAuthentications) _
                                            .Name("RemoteAuthenticationsGrid") _
                                            .HtmlAttributes(New With {.style = "width:965px;"}) _
                                            .NoRecordsTemplate("<br/>") _
                                            .DataKeys(Function(keys) keys.Add("RemoteAuthenticationId")) _
                                            .Footer(True) _
                                            .ToolBar(Function(cmds) cmds.Insert()) _
                                            .Selectable(Sub(s) s.Enabled(True)).DataBinding(Function(db) db.Ajax().Select("SearchRemoteAuthentications", "RemoteAuthentication").Update("UpdateRA", "RemoteAuthentication").Insert("InsertRA", "RemoteAuthentication").Delete("DeleteRA", "RemoteAuthentication").OperationMode(GridOperationMode.Client)) _
                                                              .Scrollable(Sub(s) s.Enabled(True).Height(450)) _
                                            .Columns(Sub(c)
                                                         c.Bound(Function(db) db.CollegeName).Title("College Name").Width(120)
                                                         c.Bound(Function(db) db.RemoteAuthenticationId).Hidden()
                                                         c.Bound(Function(db) db.DatabaseName).Title("Database Name").Width(150)
                                                         c.Bound(Function(db) db.Url).Title("Url").Width(180)
                                                         c.Bound(Function(db) db.FullUrl).Title("Full Url").Width(180)
                                                         c.Bound(Function(db) db.DomainName).Title("Domain").Width(150)
                                                         c.Bound(Function(db) db.CampusRestriction).Title("Campus").Hidden()
                                                         c.Bound(Function(db) db.ConnectionMethodName).Title("Connection Method").Hidden()
                                                         c.Command(Function(cmd) cmd.Edit().ButtonType(GridButtonType.BareImage))
                                                         c.Command(Function(cmd) cmd.Delete().ButtonType(GridButtonType.BareImage))
                                                         c.Command(Function(cmd) cmd.Custom("copy").Ajax(True).Text("Copy").ButtonType(GridButtonType.Image).HtmlAttributes(New With {.style = "background-image:url('/Content/images/copy.png');width:16px;height:16px;background-color:transparent;border:0"}))
                                                     End Sub
                                            ) _
                                            .ClientEvents(Function(e) e.OnDataBinding("RemoteAuthenticationsGrid_OnDataBinding").OnEdit("RemoteAuthenticationsGrid_OnEdit").OnDelete("RAOnDelete").OnComplete("GridOnComplete").OnSave("GridOnSave").OnCommand("GridOnCommand").OnRowDataBound("onRowDataBound").OnDataBound("RemoteAuthenticationsGrid_OnDataBound")) _
                                            .Editable(Function(e) e.Mode(GridEditMode.PopUp).TemplateName("RemoteAuthentication").Window(Function(w) w.Modal(True).Title("Remote Authentication"))) _
                                            .Sortable() _
                                            .Pageable(Function(p) p.Enabled(True).PageSize(Model.CurrentPageSize, Model.PageSizeInDropDown).Style(GridPagerStyles.PageSizeDropDown Or GridPagerStyles.NextPreviousAndNumeric)) _
                                            .Resizable(Function(p) p.Columns(True)) _
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
        $(function () {
            var gridHeight = $(window).height() - $(".header-content").height() - 400;
            $(".t-grid-content").height(gridHeight);
        });
    </script>
</asp:Content>
