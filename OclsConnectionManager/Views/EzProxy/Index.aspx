<%@ Page Title="" Language="VB" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage(Of OclsConnectionManager.EzProxyDatabaseListViewModel)" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
OCLS Connection Manager - EZproxy
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<%= Html.Telerik().ScriptRegistrar().OnDocumentReady("OnReady();") %>

    <script type="text/javascript">

        function OnReady() {

            var gridHeight = $(window).height() - $(".header-content").height() - 570;
            $(".t-grid-content").height(gridHeight);

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

        function ReloadDatabaseGrid() {
            
            var grid = $("#DatabasesGrid").data("tGrid");
            if (grid) {
                //grid.rebind();
                grid.dataSource._data = [];

                ////Restore page index after saving or cancel database popup.
                if ($('#hdCurrentPageIndex').val() != "" && $('#hdCurrentPageIndex').val() != "0")
                    grid.pageTo($('#hdCurrentPageIndex').val()); 
                else
                    grid.pageTo(1); ////Default Page Index
            }
        }

        function SearchDatabases() {
            var grid = $("#DatabasesGrid").data("tGrid");
            if (grid)
                grid.rebind();
        }

        function ClearSearch() {
            $("#DatabaseSearchTerm, #URLSearchTerm, #DomainNameSearchTerm, #KeywordSearchTerm").val('');
            SearchDatabases();
        }

        function DatabasesGrid_OnDataBinding(e) {

            e.data = {
                ezproxyServerId: $("#FilterEzProxyServerId").val(),
                databaseName: $("#DatabaseSearchTerm").val(),
                urlSearchTerm: $("#URLSearchTerm").val(),
                domainName: $("#DomainNameSearchTerm").val(),
                keywordName: $("#KeywordSearchTerm").val()
            };
        }

        function DatabasesGrid_OnEdit(e) {

            ////Send Ajax request to clear previous session if any
            var url = '<%= Url.Action("ClearPreviousSession","EzProxy") %>';

            $.ajax({
                async: false,
                url: url,
                type: 'POST',
                success: function (data) {
                    if (data && data.value != undefined && data.value == true) { 
                    }
                }
            });

            ///Save the current page index of the grid to restore after edit
            var grid = $('#DatabasesGrid').data("tGrid");
            if (grid) {
                $('#hdCurrentPageIndex').val(grid.currentPage);
            }

            var editWindow = $(e.form).closest(".t-window").data("tWindow");
            if (editWindow)
            {
                ////Set popup height to 80% of window's height.
                var winHeight = $(window).height() * 80 / 100;
                $(e.form).closest(".t-window").find("div.editTemplateDiv").height(winHeight);

                var gridHeight = winHeight - $(e.form).closest(".t-window").find("div.editTemplateDiv").find(".t-grid-header").height() - 360;
                $(e.form).closest(".t-window").find("div.editTemplateDiv").find(".t-grid-content").height(gridHeight);

                editWindow.center();
            }

            //$(e.form).closest(".t-window").data("tWindow").center();

            EzProxyDatabase_OnLoad();
        }

        function ShowPreview(e) {
            var selectedEzProxyServerId = $("#SaveOrPreviewEzProxyServerId").val();
            if (selectedEzProxyServerId == 0) {
                alert("You must select an EZproxy server configuration to preview.");
                e.preventDefault();
                return false;
            }            
            window.location.href = '<%= Url.Action("ShowPreview", "EZproxy") %>' + '?ezProxyServerId=' + selectedEzProxyServerId;
        }


        function SaveConfiguration(e) {
            var selectedEzProxyServerId = $("#SaveOrPreviewEzProxyServerId").val();
            if (selectedEzProxyServerId == 0) {
                alert("You must select an EZproxy server configuration to save.");
                e.preventDefault();
                return false;
            }

            if (!confirm('Are you sure you want to overwrite the existing configuration file ?')) {
                e.preventDefault();
                return false;
            }
            else
                window.location.href = '<%= Url.Action("SaveConfiguration", "EZproxy") %>' + '?ezProxyServerId=' + selectedEzProxyServerId;
        }

        function GridnDelete(e) {
            e.preventDefault();
            if (confirm("Do you want to delete this record?")) {
            }
            else {
              
                return true;
            }
          
            var item = e.dataItem;
            var id = item.EzProxyDatabaseId
            var url = '<%= OclsConnectionManager.RouteConfig.GetActionUri("EZproxy", "DeleteDatabaseCustom") %>';
            $.post(url, { id: id }, function (data) {
                var s = data;
                if (s == false) {
                    e.preventDefault();
                    alert("Database is used and cannot be deleted.");
                    return false;
                }
                else {
                    alert("Database deleted successfully.");
                    $("#DatabasesGrid").data("tGrid").rebind();
                }
            });
            return true
        }


    </script>

    <div style="height: 5px;"></div>
    <div id="DatabasesDiv">
        <fieldset>
            <legend>EZproxy Databases</legend>
            <p>Enter your criteria and click "Search" to find existing EZproxy databases.
            </p>
            <% Using (Html.BeginForm("", "", FormMethod.Post, New With {.defaultbutton = "btnSearch", .clearButton = "btnClear", .id = "SearchForm"}))
             %>
            <table style="width: 100%;">
                <tr>
                    <td style="text-align: right;">Server:</td>
                    <td><%: Html.DropDownListFor(Function(m) m.FilterEzProxyServerId, ViewData("AvailableServersForEdit")) %></td>
                    <td style="width: 20%;">Database:&nbsp;<%= Html.TextBoxFor(Function(m) m.DatabaseSearchTerm, New With {.style = "width:100px;"})%></td>
                    <td style="text-align: right;">Url:</td><td style="width: 8%;"><%= Html.TextBoxFor(Function(m) m.URLSearchTerm, New With {.style = "width:100px;"})%></td>
                    <td style="text-align: right;">Domain:</td>
                    <td style="width: 8%;"><%= Html.TextBoxFor(Function(m) m.DomainNameSearchTerm, New With {.style = "width:100px;"})%></td>
                    <td style="text-align: right;">Keyword:</td>
                    <td style="width: 8%;"><%= Html.TextBoxFor(Function(m) m.KeywordSearchTerm, New With {.style = "width:100px;"})%></td>
                </tr>
                <tr><td style="text-align: right;" colspan="9">&nbsp;</td></tr>
                <tr>
                    <td style="text-align: right;" colspan="9">
                        <input type="hidden" id="hdCurrentPageIndex" />
                        <input type="button" value="Search" id="btnSearch" onclick="SearchDatabases();" class="t-button" style="width: 55px" />
                        &nbsp;
                        <input type="button" value="Clear" id="btnClear" onclick="ClearSearch();" class="t-button" style="width: 55px" />
                    </td>
                </tr>
            </table>
            <% End Using %>
            <br />
            <% Html.Telerik().Grid(Model.Databases) _
                .Name("DatabasesGrid") _
                .HtmlAttributes(New With {.style = "width:965px;"}) _
                .NoRecordsTemplate("<br/>") _
                .DataKeys(Function(keys) keys.Add("EzProxyDatabaseId")) _
                .Footer(True) _
                .ToolBar(Sub(toolbar)
                             toolbar.Insert().Text("Add new record").HtmlAttributes(New With {.style = "text-align:right;"})
                         End Sub) _
                .Selectable(Sub(s) s.Enabled(True)).DataBinding(Function(db) db.Ajax().Select("SearchDatabases", "EzProxy") _
                                                                    .Update("UpdateDatabase", "EzProxy") _
                                                                    .Insert("InsertDatabase", "EzProxy") _
                                                                    .Delete("DeleteDatabase", "EzProxy").OperationMode(GridOperationMode.Client)) _
                .Columns(Sub(c)
                             c.Bound(Function(db) db.EzProxyDatabaseId).Hidden()
                             c.Bound(Function(db) db.Name).Title("Database Name").Width(250)
                             c.Bound(Function(db) db.EzProxyServerName).Title("EZproxy Server").Width(200)
                             c.Bound(Function(db) db.ModifiedBy).Title("Last Modified By").Width(110)
                             c.Bound(Function(db) db.ModifiedDate).Title("Last Modified Date").Width(110).Format("{0:MM/dd/yyyy}")
                             c.Command(Function(cmd) cmd.Edit().ButtonType(GridButtonType.BareImage)).Width(40)
                             c.Command(Function(cmd) cmd.Delete().ButtonType(GridButtonType.BareImage)).Width(40)
                         End Sub
                ) _
                .ClientEvents(Function(e) e.OnDataBinding("DatabasesGrid_OnDataBinding").OnEdit("DatabasesGrid_OnEdit").OnDelete("GridnDelete")) _
                .Editable(Function(e) e.Mode(GridEditMode.PopUp).TemplateName("EzProxyDatabase").Window(Function(w) w.Modal(True).Resizable(Function(d) d.Enabled(True).MinHeight(700).MinWidth(800)).Title("EZproxy Database"))) _
                .Sortable() _
                .Scrollable(Function(scroll) scroll.Enabled(True).Height(300)) _
                .Pageable(Function(p) p.Enabled(True).PageSize(Model.CurrentPageSize, Model.PageSizeInDropDown).Style(GridPagerStyles.PageSizeDropDown Or GridPagerStyles.NextPreviousAndNumeric)) _
                .Resizable(Function(p) p.Columns(True)) _
                .Render()
            %>
        </fieldset>
        <br /><br />
        <% Html.Telerik().Window().Name("winDatabase").Modal(True)%>
        
        <fieldset>
            <legend>EZproxy Configuration</legend>
            <p>Select one EZproxy server.  Click "Preview Configuration" to view how the current configuration will look.  When you have reviewed preview and are satisfied, click "Save Configuration" to update the EZproxy configuration file on the web server.  Be sure to save to all EZproxy servers.</p>
            <p style="font-weight:bold;color:red">Reminder: Save to all EZproxy servers.</p>
            <table style="width: 100%;">
                <tr>
                    <td style="width: 50%;">&nbsp;
                    </td>
                    <td style="text-align: right;"> 
                        <%: Html.DropDownListFor(Function(m) m.SaveOrPreviewEzProxyServerId, ViewData("AvailableServersForSave"))%> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <input type="button" value="Preview Configuration" id="btnPreview" onclick="ShowPreview(event);" class="t-button" style="width: 150px" />
                        <input type="button" value="Save Configuration" id="btnSave" onclick="SaveConfiguration(event);" class="t-button" style="width: 150px" />
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
</asp:Content>
