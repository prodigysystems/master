<%@ Control Language="VB" Inherits="System.Web.Mvc.ViewUserControl(Of OclsConnectionManager.Dto.EzProxyDatabaseDto)" %>

<script type="text/javascript">

    $(function () {

        //$('div.t-upload-button').find('span:first-child').text('Import');

        $('.t-close').parent().remove();

        $('.t-grid-insert').attr('id', 'lnkInsert');
        $('.t-grid-update').attr('id', 'lnkUpdate');

        $('.t-grid-update').removeClass();
        $('.t-grid-insert').removeClass();

        $('.t-grid-cancel').click(function (e) {
            //window.location.href = '<%= Url.Action("Index","EzProxy") %>';

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

            var winDatabasesGridPopUp = $('#DatabasesGridPopUp').data('tWindow');
            if (winDatabasesGridPopUp)
                winDatabasesGridPopUp.close();

            ReloadDatabaseGrid();

        });

        $('#lnkInsert').click(function (e) {

            if (ValidateDatabase()) {

                var allDirectives = new Array();
                var thisDirective = null;
                var rows = $('#DirectivesGrid').data('tGrid').$rows();
                for (i = 0; i < rows.length; i++) {

                    thisDirective = new Object();
                    thisDirective.Name = $(rows[i]).find('td#Name').text();
                    thisDirective.OutputAs = $(rows[i]).find('td#OutputAs ').text();
                    thisDirective.EzProxyDatabaseDirectiveId = $(rows[i]).find('td#EzProxyDatabaseDirectiveId').text();
                    thisDirective.EzProxyDatabaseId = $(rows[i]).find('td#EzProxyDatabaseId').text();
                    thisDirective.EzProxyDirectiveId = $(rows[i]).find('td#EzProxyDirectiveId').text();

                    if (thisDirective.EzProxyDirectiveId > 0)
                        allDirectives.push(thisDirective);
                }

                var url = '<%= Url.Action("AddDatabase", "EzProxy")%>';

                var oEzProxyDatabaseDto =
                {
                    Name: $('#Name').val(),
                    Directives: allDirectives,
                    DomainName: $('#DomainName').val(),
                    Url: $('#Url').val(),
                    Title: $('#Title').val(),
                    OutputOrder: $('#OutputOrder').val(),
                    Comment: $('#Comment').val(),
                    IsActive: $('#IsActive').is(':checked'),
                    EzProxyDatabaseId: $('#EzProxyDatabaseId').val(),
                    EzProxyServerId: $('#EzProxyServerId').val()
                };

                var inserted = false;

                $.ajax({
                    url: url,
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify({ database: oEzProxyDatabaseDto }),
                    success: function (data) {
                        var s = data;
                        if (s == false) {
                            e.preventDefault();
                            alert("Database name already exists.");
                            return false;
                        }
                        else {
                            inserted = true;
                            alert('Successfully saved.');
                        }
                    },
                    complete: function (xhr, statusText) {
                        if (inserted) {
                            window.location.href = '<%= Url.Action("Index","EzProxy") %>';
                        }
                    }
                });

                e.preventDefault();
                return false;
            }
            else {
                e.preventDefault();
                return false;
            }

        });


        $('#lnkUpdate').click(function (e) {
            debugger;
            if (ValidateDatabase()) {

                var allDirectives = new Array();
                var thisDirective = null;
                var rows = $('#DirectivesGrid').data('tGrid').$rows();
                for (i = 0; i < rows.length; i++) {

                    thisDirective = new Object();
                    thisDirective.Name = $(rows[i]).find('td#Name').text();
                    thisDirective.OutputAs = $(rows[i]).find('td#OutputAs ').text();
                    thisDirective.EzProxyDatabaseDirectiveId = $(rows[i]).find('td#EzProxyDatabaseDirectiveId').text();
                    thisDirective.EzProxyDatabaseId = $(rows[i]).find('td#EzProxyDatabaseId').text();
                    thisDirective.EzProxyDirectiveId = $(rows[i]).find('td#EzProxyDirectiveId').text();
                    thisDirective.IsActive = $(rows[i]).find('td#IsActive').text();

                    if (thisDirective.EzProxyDirectiveId > 0)
                        allDirectives.push(thisDirective);
                }

                var url = '<%= Url.Action("EditDatabase", "EzProxy")%>';

                var oEzProxyDatabaseDto =
                {
                    Name: $('#Name').val(),
                    Directives: allDirectives,
                    DomainName: $('#DomainName').val(),
                    Url: $('#Url').val(),
                    Title: $('#Title').val(),
                    OutputOrder: $('#OutputOrder').val(),
                    Comment: $('#Comment').val(),
                    IsActive: $('#IsActive').is(':checked'),
                    EzProxyDatabaseId: $('#EzProxyDatabaseId').val(),
                    EzProxyServerId: $('#EzProxyServerId').val()
                };

                var updated = false;

                $.ajax({
                    url: url,
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify({ database: oEzProxyDatabaseDto }),
                    success: function (data) {
                        if (data && data.value != undefined && data.value == true) {

                            updated = data.value;
                            alert('Successfully saved.');
                        }
                    },
                    complete: function (xhr, statusText) {
                        if (updated) {
                            //window.location.href = '<%= Url.Action("Index","EzProxy") %>';

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

                            var winDatabasesGridPopUp = $('#DatabasesGridPopUp').data('tWindow');
                            if (winDatabasesGridPopUp)
                                winDatabasesGridPopUp.close();

                            ReloadDatabaseGrid();

                        }
                    }
                });

                e.preventDefault();
                return false;
            }
            else {
                e.preventDefault();
                return false;
            }

        });

        if ($('#EzProxyDatabaseId').val() == "" || $('#EzProxyDatabaseId').val() == "0")
            $('#IsActive').attr('checked', true);

        $('#Name').blur(function (e) {

            if ($('#Title').val() == "" && $('#Name').val() != "") {
                $('#Title').val($('#Name').val());

                SetTitleDirective();
            }
        });

        $('#DomainName').blur(function (e) {
            if ($('#PerformDomainAutocomplete').is(":checked")) {
                SetDomainDirective();
            }
        });

        $('#Url').blur(function (e) {
            //if ($('#PerformDomainAutocomplete').is(":checked")) {
            SetUrlDirective($('#PerformDomainAutocomplete').is(":checked"));
            //}
        });

        $('#Title').blur(function (e) {
            SetTitleDirective();
        });

    });

    function SetDomainDirective() {

        
        var updateDirectiveUrl = '<%= Url.Action("UpdateDirective","EzProxy") %>';
        var isSuccessful = false;

        var oEzProxyDatabaseDirectiveDto =
            {
                Name: "Domain",
                OutputAs: $('#DomainName').val(),
                Options: [],
                IsActive: true,
                EzProxyDatabaseId: $('#EzProxyDatabaseId').val(),
                EzProxyDirectiveId: 0,
                EzProxyDatabaseDirectiveId: 0
            };

        $.ajax({
            async: false,
            url: updateDirectiveUrl,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ ezProxyDatabaseDirectiveDto: oEzProxyDatabaseDirectiveDto }),
            success: function (data) {
                if (data && data.value != undefined && data.value == true) {
                    isSuccessful = data.value;
                }
            },
            complete: function (xhr, status) {
                if (isSuccessful) {
                    var grd = $('#DirectivesGrid').data('tGrid');
                    if (grd)
                        grd.rebind();
                }
            }
        });
    }

    function SetUrlDirective(shouldSetDomainDirective) {

        var updateDirectiveUrl = '<%= Url.Action("UpdateDirective","EzProxy") %>';
        var isSuccessful = false;

        var oEzProxyDatabaseDirectiveDto =
            {
                Name: "Url",
                OutputAs: $("#Url").val(),
                Options: [],
                IsActive: true,
                EzProxyDatabaseId: $('#EzProxyDatabaseId').val(),
                EzProxyDirectiveId: 0,
                EzProxyDatabaseDirectiveId: 0
            };

            $.ajax({
                async: false,
                url: updateDirectiveUrl,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ ezProxyDatabaseDirectiveDto: oEzProxyDatabaseDirectiveDto }),
                success: function (data) {
                    if (data && data.value != undefined && data.value == true) {
                        isSuccessful = data.value;
                    }
                },
                complete: function (xhr, status) {
                    if (isSuccessful) {
                        var grd = $('#DirectivesGrid').data('tGrid');
                        if (grd)
                            grd.rebind();

                        if (!shouldSetDomainDirective) {
                            return;
                        }

                        var urlstring = $("#Url").val();
                        var re = /(\w+)\.(\w+)(\.\w+){0,1}\/*.*/;
                        if (urlstring.length > 0) {
                           
                            var matches = urlstring.match(re);
                            var newDomain = '';
                            if (matches != null && matches[3] != null) {
                                newDomain = matches[2] + matches[3];
                            }
                            else if(matches!=null)
                            {
                                newDomain = matches[1] + '.' + matches[2];
                            }
                                
                            if (newDomain != '') {
                                $("#DomainName").val(newDomain);
                                SetDomainDirective();
                            }                           
                        }
                        else {
                            $("#DomainName").val('');
                        }
                    }
                }
            });

    }

    function UpdateDatabaseValues() {

            var updateDatabaseUrl = '<%= Url.Action("GetDatabaseValues","EzProxy") %>';
            var isSuccessful = false;

            $.ajax({
                async: false,
                url: updateDatabaseUrl,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ databaseId: $('#EzProxyDatabaseId').val() }),
                success: function (data) {
                    if (data && data.value != undefined && data.value == true) {

                        if (data.database != undefined && data.database != null) {
                            
                            if (data.database.Title != undefined)
                                $('#Title').val(data.database.Title);

                            if (data.database.Url != undefined)
                                $('#Url').val(data.database.Url);

                            if (data.database.DomainName != undefined)
                                $('#DomainName').val(data.database.DomainName);
                        }
                    }
                },
                complete: function (xhr, status) {
                    if (isSuccessful) {

                    }
                }

            });
        
    }


    function SetTitleDirective() {

        var updateDirectiveUrl = '<%= Url.Action("UpdateDirective","EzProxy") %>';
        var isSuccessful = false;

        var oEzProxyDatabaseDirectiveDto =
            {
                Name: "Title",
                OutputAs: $('#Title').val(),
                Options: [],
                IsActive: true,
                EzProxyDatabaseId: $('#EzProxyDatabaseId').val(),
                EzProxyDirectiveId: 0,
                EzProxyDatabaseDirectiveId: 0
            };

            $.ajax({
                async: false,
                url: updateDirectiveUrl,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ ezProxyDatabaseDirectiveDto: oEzProxyDatabaseDirectiveDto }),
                success: function (data) {
                    if (data && data.value != undefined && data.value == true) {
                        isSuccessful = data.value;
                    }
                },
                complete: function (xhr, status) {
                    if (isSuccessful) {
                        var grd = $('#DirectivesGrid').data('tGrid');
                        if (grd)
                            grd.rebind();
                    }
                }

            });
    }

    ////Try convert string value to Integer, else return value provided in defaultValue parameter
    function TryParseInt(str, defaultValue) {

        var retValue = defaultValue;

        if (typeof str != 'undefined' && str != null && str.length > 0) {
            if (!isNaN(str))
                retValue = parseInt(str);
        }
        return retValue;
    }


    function ValidateDatabase() {
        if ($('#Name').val() == "") 
        {
            alert('Database name is required.');
            $('#Name').focus();
            return false;
        }
        else
            return true;
    }

    function EzProxyDatabase_OnLoad() {
        var grd = $('#DirectivesGrid').data('tGrid');
        if (grd)
            grd.rebind();

        UpdateDatabaseValues();
    }

    function DirectivesGrid_OnEdit(e) {
        
        $(e.form).closest(".t-window").find('#EzProxyDatabaseId').val(e.dataItem.EzProxyDatabaseId);
        $(e.form).closest(".t-window").find('#EzProxyDirectiveId').val(e.dataItem.EzProxyDirectiveId);
        $(e.form).closest(".t-window").find('#EzProxyDatabaseDirectiveId').val(e.dataItem.EzProxyDatabaseDirectiveId);

        if (e.dataItem.EzProxyDatabaseDirectiveId == "0")
        {
            $(e.form).closest(".t-window").find('#IsActive').attr('checked', true);
        }

        //alert("In Edit \n db " + e.dataItem.EzProxyDatabaseId + "\n DirectiveId " + e.dataItem.EzProxyDirectiveId + "\n DatabaseDirectiveId " + e.dataItem.EzProxyDatabaseDirectiveId);

        $(e.form)
                 .closest(".t-window")
                 .data("tWindow")
                 .center();
        
        

        $(e.form).closest(".t-window").find('#OutputOrder').on('keypress', function (e) {
            return !(e.which != 8 && e.which != 0 &&
                 (e.which < 48 || e.which > 57) && e.which != 46);
        });

        EzProxyDatabaseDirective_OnLoad();
    }

    function winEzProxyDatabaseDirective_OnClose(e) 
    {
    }

    function DirectivesGrid_OnCommand(e) {
     
        if (e.name == "btnDelete") {

            e.preventDefault();
            var item = e.dataItem;

            var isSuccessful = false;

            var url = '<%= Url.Action("DeleteDatabaseDirective", "EzProxy")%>';

            $.ajax({
                type: 'POST',
                url: url,
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ DatabaseId: item.EzProxyDatabaseId, DirectiveId: item.EzProxyDirectiveId, DatabaseDirectiveId: item.EzProxyDatabaseDirectiveId, directiveName: item.Name }),
                success: function (data) {

                    if (data) {

                        if (data.value != undefined) 
                        {
                            isSuccessful = data.value
                        }

                        if (data.message != undefined) {
                            if (data.message != null || data.message != '')
                                alert(data.message);
                        }
                    }
                },
                complete: function (xhr, status) {
                    var grd = $('#DirectivesGrid').data('tGrid');
                    if (grd)
                        grd.rebind();

                    if (isSuccessful)
                        UpdateDatabaseValues();
                }
            });

        }
    }


    function DirectivesGrid_OnDataBinding(e) {

        var ezProxyDatabaseId = ($('#EzProxyDatabaseId').val() == undefined || $('#EzProxyDatabaseId').val() == "") ? 0 : parseInt($('#EzProxyDatabaseId').val());

        e.data = { ezProxyDatabaseId: ezProxyDatabaseId };
    }


    function DirectivesGrid_OnDataBound(e) {
   
    }

    function PreviewDatabase(e) {

        var grdData = $("#DirectivesGrid").data('tGrid').data;

        if (grdData != null && grdData.length > 0) {
    
            window.location.href = '<%= Url.Action("ShowDatabasePreview", "EzProxy")%>?databaseId=' + $('#EzProxyDatabaseId').val();
        }
        else {
            alert("Please select at least one directive to see the preview of configuration for this database.");
        }
    }


    function RebindGrid()
    {
        var grd = $('#DirectivesGrid').data('tGrid');
        if (grd)
        {
            grd.rebind();
            UpdateDatabaseValues();
        }
    }

    function DatabaseDirectivesFile_OnSuccess(e)
    {

        $(".t-upload-files").remove();
        $(".t-upload-status").remove();
        $(".t-upload.t-header").addClass("t-upload-empty");
        $(".t-upload-button").removeClass("t-state-focused");
        var msg = "File imported successfully.";
        if (e.response != null && e.response.value == true) {
           
            //alert("File is imported successfully.");
            if (e.response.DirectiveExtraction != undefined && e.response.DirectiveExtraction != null) {
                if (e.response.DirectiveExtraction.EzProxyDatabase != undefined && e.response.DirectiveExtraction.EzProxyDatabase != null) {
                    var grd = $('#DirectivesGrid').data('tGrid');
                    if (grd && e.response.DirectiveExtraction.EzProxyDatabase.Directives != undefined && e.response.DirectiveExtraction.EzProxyDatabase.Directives != null)
                    {
                        ///Rebind to get new directives
                        grd.rebind();
                        //grd.dataBind(e.response.DirectiveExtraction.EzProxyDatabase.Directives);
                    }

                    SetDatabaseValues(e.response.DirectiveExtraction.EzProxyDatabase);
                }
                if (e.response.DirectiveExtraction.Errors.length > 0) {
                    msg += " with some errors:"+"\n";
                    for (var i = 0; i < e.response.DirectiveExtraction.Errors.length; i++) {
                        msg += e.response.DirectiveExtraction.Errors[i] + "\n";
                    }
                }
                else {
                    msg += ".";
                }

            }
            ////Rebind the database directives Grid to update list with new directives
            //RebindGrid();
        }
        else {
            msg="Failed to import the file."
           
        }
        alert(msg);
    }

    function SetDatabaseValues(ezProxyDatabase)
    {
        if (ezProxyDatabase != undefined && ezProxyDatabase != null)
        {
            if (ezProxyDatabase.Title != undefined)
                $('#Title').val(ezProxyDatabase.Title);

            if (ezProxyDatabase.Url != undefined)
                $('#Url').val(ezProxyDatabase.Url);

            if (ezProxyDatabase.DomainName != undefined)
                $('#DomainName').val(ezProxyDatabase.DomainName);
        }
    }

    function DatabaseDirectivesFile_OnSelect(e)
    {
        if (e.files.length > 0 && e.files[0].extension != null && $.trim(e.files[0].extension).toLowerCase() != ".txt")
        {
            alert('Please upload text file (*.txt) and try again.');
            e.preventDefault ? e.preventDefault() : e.returnValue = false;
            return false;
        }
    }

    function DatabaseDirectivesFile_OnError(e)
    {
        alert('Error occurred while uploading file. Please try again.');
    }

    function DatabaseDirectivesFile_OnUpload(e)
    {
        e.data = { ezProxyDatabaseId: $('#EzProxyDatabaseId').val() };
    }

</script>

<style type="text/css">
    input[type="file"]
    {
        width: 100px !important;
        height: 30px !important;
    }

    .t-dropzone
    {
        padding: 0px !important;
        width: 100px !important;
        text-align: right !important;
        float: right !important;
        height: 25px !important;
        /*margin-right: 45px !important;*/
    }

    .t-upload-files
    {
        max-width: 350px !important;
    }

</style>
<div class="editTemplateDiv">
    <%= Html.HiddenFor(Function(m) m.EzProxyDatabaseId)%>
    <%: Html.ValidationSummary("The following errors must be corrected:", New With {.class = "validation-summary-errors-flat"})%>
    
    <table class="editTemplateTable">
        <tr>
            <td class="editTemplateLabel">
                Name
            </td>
            <td>
                <%: Html.TextBoxFor(Function(m) m.Name, New With {.style = "width:500px", .maxLength = "200"})%>
            </td>
            <td style="text-align:right;">
                <img src="<%: Url.Content("~/Images/preview_Section.jpg") %>" alt="preview" title="Preview database settings in configuration file." onclick="PreviewDatabase(event);" />
            </td>
        </tr>
        <tr>
            <td class="editTemplateLabel">
                Title
            </td>
            <td>
                <%: Html.TextBoxFor(Function(m) m.Title, New With {.style = "width:500px", .maxLength = "994"})%>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="editTemplateLabel">
                Auto-Complete
            </td>
            <td>
                <%: Html.CheckBoxFor(Function(m) m.PerformDomainAutocomplete)%> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Uncheck to suppress domain auto-completion.
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="editTemplateLabel">
                Url
            </td>
            <td>
                <%: Html.TextBoxFor(Function(m) m.Url, New With {.style = "width:500px", .maxLength = "996"})%>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="editTemplateLabel">
                Domain
            </td>
            <td>
                <%: Html.TextBoxFor(Function(m) m.DomainName, New With {.style = "width:500px", .maxLength = "993"})%>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="editTemplateLabel">
                Comment
            </td>
            <td>
                <%: Html.TextAreaFor(Function(m) m.Comment, New With {.style = "width:496px", .maxLength = "1000"})%>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="editTemplateLabel">
                EZproxy Server
            </td>
            <td>
                <%: Html.DropDownListFor(Function(m) m.EzProxyServerId, ViewData("AvailableServersForEdit")) %>
            </td>
            <td>
                Keep as &lsquo;All servers&rsquo;
            </td>
        </tr>
        <tr>
            <td class="editTemplateLabel">
                Output Order
            </td>
            <td>
                <%: Html.Telerik().IntegerTextBoxFor(Function(m) m.OutputOrder).Spinners(False).MinValue(0).MaxValue(10000)%>
            </td>
            <td>
                unless required
            </td>
        </tr>
        <tr>
            <td class="editTemplateLabel">
                Active
            </td>
            <td>
                <%: Html.CheckBoxFor(Function(m) m.IsActive)%>
            </td>
            <td>
                 to specify.
            </td>
        </tr>
<%--        <tr>
            <td >
                &nbsp;
            </td>
            <td colspan="2" style="text-align: right;">
                <%: Html.Telerik().Upload().Name("DatabaseDirectivesFile").Multiple(False) _
                    .ClientEvents(Function(ev) ev.OnSuccess("DatabaseDirectivesFile_OnSuccess").OnSelect("DatabaseDirectivesFile_OnSelect") _
                    .OnError("DatabaseDirectivesFile_OnError").OnUpload("DatabaseDirectivesFile_OnUpload")) _
                    .Async(Function(a) a.AutoUpload(True).Save("ImportDatabaseDirectives", "EzProxy").Remove("RemoveDatabaseDirectives", "EzProxy"))
                %>
            </td>
        </tr>--%>
        <tr>
            <td colspan="3">
                <fieldset>
                    <legend>Directives</legend>
                    <br />
                    <% Html.Telerik().Grid(Model.Directives) _
                    .Name("DirectivesGrid") _
                    .HtmlAttributes(New With {.style = "width:765px;"}) _
                    .NoRecordsTemplate("<br/>") _
                    .DataKeys(Function(keys) keys.Add("EzProxyDatabaseId")) _
                        .DataKeys(Function(keys) keys.Add("EzProxyDirectiveId")) _
                        .DataKeys(Function(keys) keys.Add("EzProxyDatabaseDirectiveId")) _
                          .Selectable(Sub(s) s.Enabled(True)).DataBinding(Function(db) db.Ajax().Select("GetDatabaseDirectives", "EzProxy") _
                                                                                            .Insert("InsertDatabaseDirective", "EzProxy") _
                                                                                            .Update("UpdateDatabaseDirective", "EzProxy") _
                                                                                            .Delete("DeleteDatabaseDirective", "EzProxy") _
                                                                                            .OperationMode(GridOperationMode.Client)) _
                        .Footer(True) _
                        .ToolBar(Sub(toolbar)
                                     toolbar.Insert().Text("Add").HtmlAttributes(New With {.style = "text-align:right;"})
                                 End Sub) _
                        .Columns(Sub(c)
                                     c.Bound(Function(db) db.Name).Title("Directive Name").Width(200).HtmlAttributes(New With {.id = "Name"})
                                     c.Bound(Function(db) db.OutputAs).Title("Output As").Width(340).HtmlAttributes(New With {.id = "OutputAs"})
                                     c.Bound(Function(db) db.OutputOrder).Title("Output Order").Width(90).HtmlAttributes(New With {.id = "OutputOrder"})
                                     c.Command(Function(cmd) cmd.Edit().ButtonType(GridButtonType.BareImage)).Width(45).HtmlAttributes(New With {.id = "Edit"})
                                     c.Command(Function(cmd) cmd.Custom("btnDelete").Ajax(True).ImageHtmlAttributes(New With {.Class = "t-delete"}).ButtonType(GridButtonType.BareImage)).Width(45)
                                     c.Bound(Function(db) db.EzProxyDatabaseDirectiveId).Hidden().HtmlAttributes(New With {.id = "EzProxyDatabaseDirectiveId"})
                                     c.Bound(Function(db) db.EzProxyDatabaseId).Hidden().HtmlAttributes(New With {.id = "EzProxyDatabaseId"})
                                     c.Bound(Function(db) db.EzProxyDirectiveId).Hidden().HtmlAttributes(New With {.id = "EzProxyDirectiveId"})
                                     c.Bound(Function(db) db.IsActive).Hidden().HtmlAttributes(New With {.id = "IsActive"})
                                 End Sub
                        ) _
                        .Editable(Function(e) e.Mode(GridEditMode.PopUp).TemplateName("EzProxyDatabaseDirective").Window(Function(w) w.Modal(True).Title("EZproxy Directive").ClientEvents(Function(evnt) evnt.OnClose("winEzProxyDatabaseDirective_OnClose")))) _
                        .ClientEvents(Function(e) e.OnEdit("DirectivesGrid_OnEdit").OnDataBinding("DirectivesGrid_OnDataBinding").OnCommand("DirectivesGrid_OnCommand").OnDataBound("DirectivesGrid_OnDataBound")) _
                        .Sortable() _
                        .Resizable(Function(d) d.Columns(True)) _
                        .Scrollable(Function(scroll) scroll.Enabled(True).Height(95)) _
                         .Pageable(Function(p) p.Enabled(False)) _
                         .Render()
                    %>
                </fieldset>
            </td>
           
        </tr>
    </table>
</div>
