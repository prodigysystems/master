<%@ Control Language="VB" Inherits="System.Web.Mvc.ViewUserControl(Of OclsConnectionManager.Dto.EzProxyDatabaseDirectiveDto)" %>

<script type="text/javascript">

    var IsAddMode = true;

    $(function () {

        if (TryParseInt($('#EzProxyDirectiveId').val(), 0) > 0) {
            IsAddMode = false;
        }

        //alert("Is add mode" + IsAddMode);

        $('#DirectivesGridform').find('#OutputOrder').on('keypress', function (e) {

            return !(e.which != 8 && e.which != 0 &&
                 (e.which < 48 || e.which > 57) && e.which != 46);
        });

        $('.t-grid-insert').click(function (e) {

            if (validateDirective()) {

                var allOptions = new Array();
                var thisOption = null;
                var rows = $('#OptionsGrid').data('tGrid').$rows();
                for (i = 0; i < rows.length; i++) {
                    thisOption = new Object();
                    thisOption.EzProxyDatabaseDirectiveOptionId = $(rows[i]).find('td').eq(0).text();
                    thisOption.IsActive = $(rows[i]).find('input[type="checkbox"]').is(':checked');
                    thisOption.OptionType = $(rows[i]).find('td').eq(2).text();
                    thisOption.Name = $(rows[i]).find('td').eq(3).text();
                    thisOption.OptionValue = $(rows[i]).find('td').eq(4).text();
                    thisOption.EzProxyDatabaseDirectiveId = $(rows[i]).find('td').eq(5).text();
                    thisOption.EzProxyOptionId = $(rows[i]).find('td').eq(6).text();
                    thisOption.EzProxyDirectiveId = $(rows[i]).find('td').eq(7).text();
                    thisOption.OutputAs = $(rows[i]).find('td').eq(8).text();
                    thisOption.IsRequired = $(rows[i]).find('td').eq(9).text();
                    thisOption.HasInputValue = $(rows[i]).find('td').eq(10).text();

                    if (thisOption.EzProxyOptionId > 0)
                        allOptions.push(thisOption);
                }

                var url = '<%= Url.Action("AddDatabaseDirectiveOption", "EzProxy")%>';

                var oEzProxyDatabaseDirectiveDto =
                {
                    Name: getSelectedDirectiveName(),
                    Options: allOptions,
                    Comment: $('#DirectivesGridform').find('#Comment').val(),
                    IsActive: $('#DirectivesGridform').find('#IsActive').is(':checked'),
                    OutputOrder: TryParseInt($('#DirectivesGridform').find('#OutputOrder').val(), 0),
                    EzProxyDatabaseId: $('#EzProxyDatabaseId').val(),
                    EzProxyDirectiveId: $('#EzProxyDirectiveId').val(),
                    EzProxyDatabaseDirectiveId: $('#EzProxyDatabaseDirectiveId').val()
                };

                var isValidate = false;

                $.ajax({
                    async: false,
                    url: url,
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify({ ezProxyDatabaseDirectiveDto: oEzProxyDatabaseDirectiveDto }),
                    success: function (data) {

                        if (data) {
                            if (data.value == true)
                                isValidate = data.value;
                            else {
                                alert('Validation error: Please correct any errors and try again.');
                            }
                        }
                    },
                    complete: function (xhr, reqStatus) {
                        ////Rebind directives grid to show updated values
                        if (isValidate)
                            EzProxyDatabase_OnLoad();
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

        $('.t-grid-update').click(function (e) {

            if (validateDirective()) {

                var allOptions = new Array();
                var thisOption = null;
                var rows = $('#OptionsGrid').data('tGrid').$rows();
                for (i = 0; i < rows.length; i++) {

                    thisOption = new Object();
                    thisOption.EzProxyDatabaseDirectiveOptionId = $(rows[i]).find('td').eq(0).text();
                    thisOption.IsActive = $(rows[i]).find('input[type="checkbox"]').is(':checked');
                    thisOption.OptionType = $(rows[i]).find('td').eq(2).text();
                    thisOption.Name = $(rows[i]).find('td').eq(3).text();
                    thisOption.OptionValue = $(rows[i]).find('td').eq(4).text();
                    thisOption.EzProxyDatabaseDirectiveId = $(rows[i]).find('td').eq(5).text();
                    thisOption.EzProxyOptionId = $(rows[i]).find('td').eq(6).text();
                    thisOption.EzProxyDirectiveId = $(rows[i]).find('td').eq(7).text();
                    thisOption.OutputAs = $(rows[i]).find('td').eq(8).text();
                    thisOption.IsRequired = $(rows[i]).find('td').eq(9).text();
                    thisOption.HasInputValue = $(rows[i]).find('td').eq(10).text();

                    if (thisOption.EzProxyOptionId > 0)
                        allOptions.push(thisOption);
                }

                var oEzProxyDatabaseDirectiveDto =
                {
                    Name: getSelectedDirectiveName(),
                    Options: allOptions,
                    Comment: $('#DirectivesGridform').find('#Comment').val(),
                    IsActive: $('#DirectivesGridform').find('#IsActive').is(':checked'),
                    OutputOrder: TryParseInt($('#DirectivesGridform').find('#OutputOrder').val(), 0),
                    EzProxyDatabaseId: $('#EzProxyDatabaseId').val(),
                    EzProxyDirectiveId: $('#DirectivesGridform').find('#EzProxyDirectiveId').val(),
                    EzProxyDatabaseDirectiveId: $('#DirectivesGridform').find('#EzProxyDatabaseDirectiveId').val()
                };

                var url = '<%= Url.Action("EditDatabaseDirectiveOption", "EzProxy")%>';

                var isValidate = false;

                $.ajax({
                    async: false,
                    url: url,
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify({ ezProxyDatabaseDirectiveDto: oEzProxyDatabaseDirectiveDto }),
                    success: function (data) {
                        if (data) {

                            if (data.value == true) {
                                isValidate = data.value;
                            }
                            else {
                                alert('Validation error: Please correct any errors and try again.');
                            }
                        }
                    },
                    complete: function (xhr, reqStatus) {
                        ////Rebind directives grid to show updated values
                        if (isValidate)
                            EzProxyDatabase_OnLoad();

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

    });

    function onDirectiveChange(e) {
      
        $("#EzProxyDirectiveId").val(e.options[e.selectedIndex].value);
        $('#EzProxyDirectiveName').val(e.options[e.selectedIndex].innerHTML);

        var grd = $("#OptionsGrid").data("tGrid");
        if (grd)
            grd.rebind();
        
    }

    function getSelectedDirectiveName() {

        var directiveName = $('#EzProxyDirectiveName').val();

        $("#EzProxyDirectiveId > option").each(function (id) {

            if ($('#EzProxyDirectiveId').val() == this.value) {
                
                $('#EzProxyDirectiveName').val(this.text);
                directiveName = this.text;
                
                return false;
            }
          
        });
        return directiveName;
    }

    function validateDirective() {

        var returnValue = true;
        
        if ($('#DirectivesGridform #EzProxyDirectiveId').val() == "0") {
            alert('Please select directive and try again');
            returnValue = false;
            $('#DirectivesGridform #EzProxyDirectiveId').focus();
            return returnValue;
        }

        var rows = $('#OptionsGrid').data('tGrid').$rows();
        if (rows != undefined && rows != null && rows.length > 0) {

            for (var i = 0; i < rows.length; i++) {

                var isRequired = $(rows[i]).find('td').eq(9).text() == "true" ? true : false;
                var hasInputValue = $(rows[i]).find('td').eq(10).text() == "true" ? true : false;
                var isActive = $(rows[i]).find('input[type="checkbox"]').is(':checked');

                if (hasInputValue == true) {

                    if (isActive && $(rows[i]).find('td').eq(4).text() == "") {

                        alert('Please specify the value for the option: ' + $(rows[i]).find('td').eq(3).text());

                        $(rows[i]).find('input[type="checkbox"]').attr('checked', false);

                        returnValue = false;
                        break;
                    }

                    if (isRequired == true) {

                        if ($(rows[i]).find('td').eq(4).text() == "") {
                            alert('Please enter value for required option(s) and try again.');
                            returnValue = false;
                            break;
                        }
                    }
                }

            }
        }
        else {
            alert('There are no options to save for the selected directive');
            returnValue = false;
        }
        

        return returnValue;

    }

    function EzProxyDatabaseDirective_OnLoad() {
        //var millisecondsToWait = 200;
        //setTimeout(function () {
        //    var grd = $('#OptionsGrid').data('tGrid');
        //    if (grd) {
        //        grd.rebind();    
        //    }
        //}, millisecondsToWait);

            var grd = $('#OptionsGrid').data('tGrid');
            if (grd) {
                grd.rebind();    
            }

        
        if ($('#EzProxyDatabaseDirectiveId').val() != "0") {
            $('#EzProxyDirectiveId').attr('disabled', true);
        }
        else {
            $('#DirectivesGridform').find('#IsActive').attr('checked', true);
        }
    }

    function OptionsGrid_OnEdit(e) {

        if (e.dataItem.HasInputValue == false) {
            $(e.cell).find('input[type="text"]').remove();
            $(e.cell).parent().find('td').find('input[type="checkbox"]').attr('checked', true);
            e.preventDefault();
            return false;
        }
    }     

    function OptionsGrid_OnSave(e) {

        if (e.dataItem.HasInputValue == true) {

            if (e.values.OptionValue != "" && e.values.OptionValue != null)
                $(e.cell).parent().find('td').find('input[type="checkbox"]').attr('checked', true);
            else
                $(e.cell).parent().find('td').find('input[type="checkbox"]').attr('checked', false);
        }
    }

    function OptionsGrid_OnDataBinding(e) {
        
        //alert("db " + $('#EzProxyDatabaseId').val() + "\n DirectiveId " + $('#EzProxyDirectiveId').val() + "\n DatabaseDirectiveId " + $('#EzProxyDatabaseDirectiveId').val());

        var ezProxyDatabaseId = ($('#DirectivesGridform #EzProxyDatabaseId').val() == undefined || $('#DirectivesGridform #EzProxyDatabaseId').val() == "") ? 0 : parseInt($('#DirectivesGridform #EzProxyDatabaseId').val());
        var ezProxyDirectiveId = ($('#DirectivesGridform #EzProxyDirectiveId').val() == undefined || $('#DirectivesGridform #EzProxyDirectiveId').val() == "") ? 0 : parseInt($('#DirectivesGridform #EzProxyDirectiveId').val());
        var ezProxyDatabaseDirectiveId = ($('#DirectivesGridform #EzProxyDatabaseDirectiveId').val() == undefined || $('#DirectivesGridform #EzProxyDatabaseDirectiveId').val() == "") ? 0 : parseInt($('#DirectivesGridform #EzProxyDatabaseDirectiveId').val());

        //alert("db " + $('#DirectivesGridform #EzProxyDatabaseId').val() + "\n DirectiveId " + $('#DirectivesGridform #EzProxyDirectiveId').val() + "\n DatabaseDirectiveId " + $('#DirectivesGridform #EzProxyDatabaseDirectiveId').val());
        
        e.data = {
            ezProxyDatabaseId: ezProxyDatabaseId,
            ezProxyDirectiveId: ezProxyDirectiveId,
            ezProxyDatabaseDirectiveId: ezProxyDatabaseDirectiveId
        };
    }

    function OptionsGrid_OnRowDataBound(e)
    {
        ////Use this method to bind input event for any inline editors (i.e. Textbox, DatePicker etc.)
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

</script>

<div class="editTemplateDiv" style="width: 775px; height: 350px;">
    <%= Html.HiddenFor(Function(m) m.EzProxyDatabaseId)%>
    <%= Html.HiddenFor(Function(m) m.EzProxyDatabaseDirectiveId)%>
    <%= Html.HiddenFor(Function(m) m.OutputAs)%>
    <%= Html.Hidden("EzProxyDirectiveName")%>
    <%: Html.ValidationSummary("The following errors must be corrected:", New With {.class = "validation-summary-errors-flat"})%>
    <table class="editTemplateTable">
        <tr>
            <td class="editTemplateLabel">
                Directive
            </td>
            <td>
                <%: Html.DropDownListFor(Function(m) m.EzProxyDirectiveId, CType(ViewData("AllDirectives"), SelectList), New With {.style = "width:496px", .onchange = "onDirectiveChange(this);"})%>
            </td>
        </tr>
        <tr>
            <td class="editTemplateLabel">
                Comment
            </td>
            <td>
                <%: Html.TextAreaFor(Function(m) m.Comment, New With {.style = "width:496px", .maxLength = "1000"})%>
            </td>
        </tr>
        <tr>
            <td class="editTemplateLabel">
                Output Order
            </td>
            <td>
                <%: Html.TextBoxFor(Function(m) m.OutputOrder, New With {.style = "width:100px"} ) %>
            </td>
        </tr>
        <tr>
            <td class="editTemplateLabel">
                Active
            </td>
            <td>
                <%: Html.CheckBoxFor(Function(m) m.IsActive)%>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <fieldset>
                    <legend>Qualifiers and Options</legend>
                    <br />
                    <% Html.Telerik().Grid(Model.Options) _
              .Name("OptionsGrid") _
              .HtmlAttributes(New With {.style = "width:765px;"}) _
              .NoRecordsTemplate("<br/>") _
              .DataKeys(Function(keys) keys.Add("EzProxyDatabaseDirectiveOptionId")) _
              .Selectable(Sub(s) s.Enabled(True)).DataBinding(Function(db) db.Ajax().Select("GetDatabaseDirectiveOptions", "EzProxy", New With {.ezProxyDatabaseId = Model.EzProxyDatabaseId, .ezProxyDirectiveId = Model.EzProxyDirectiveId, .ezProxyDatabaseDirectiveId = Model.EzProxyDatabaseDirectiveId}) _
                                                                  .OperationMode(GridOperationMode.Client)) _
              .Footer(True) _
              .Columns(Sub(c)
                           c.Bound(Function(db) db.EzProxyDatabaseDirectiveOptionId).Title("OptionId").Hidden()
                           c.Bound(Function(db) db.IsActive).Title("Active").ReadOnly().ClientTemplate("<input type='checkbox' id='chkIsActive<#= EzProxyDatabaseDirectiveOptionId #>' name='chkIsActive' class='chkIsActive' <#= IsActive == 1 ? checked='checked' : '' #> /> ").Width(50)
                           c.Bound(Function(db) db.OptionType).Title("Type").Width(80).ReadOnly()
                           c.Bound(Function(db) db.Name).Title("Option Name").Width(150).ReadOnly()
                           c.Bound(Function(db) db.OptionValue).Title("Option Value").Width(300)
                           c.Bound(Function(db) db.EzProxyDatabaseDirectiveId).Hidden()
                           c.Bound(Function(db) db.EzProxyOptionId).Hidden()
                           c.Bound(Function(db) db.EzProxyDirectiveId).Hidden()
                           c.Bound(Function(db) db.OutputAs).Hidden()
                           c.Bound(Function(db) db.IsRequired).Title("Required?").ReadOnly().Width(80)
                           c.Bound(Function(db) db.HasInputValue).Hidden()
                       End Sub
          ) _
          .ClientEvents(Function(evnt) evnt.OnDataBinding("OptionsGrid_OnDataBinding").OnEdit("OptionsGrid_OnEdit").OnSave("OptionsGrid_OnSave").OnRowDataBound("OptionsGrid_OnRowDataBound")) _
          .Editable(Function(e) e.Mode(GridEditMode.InCell).Enabled(True)) _
          .Sortable() _
          .Scrollable(Function(scroll) scroll.Enabled(True).Height(95)) _
          .Pageable(Function(p) p.Enabled(False)) _
          .Render()
                    %>
                </fieldset>
            </td>
        </tr>
    </table>
</div>
