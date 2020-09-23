<%@ Control Language="VB" Inherits="System.Web.Mvc.ViewUserControl(Of OclsConnectionManager.Dto.RemoteAuthenticationDto)" %>
<script type="text/javascript">
    $(function () {
        $("#EzProxyDatabaseId").change(function (event) {
            var dbId = $('#EzProxyDatabaseId').val();
            var url = '<%= OclsConnectionManager.RouteConfig.GetActionUri("RemoteAuthentication", "GetDBDirective") %>';

            $.post(url, { id: dbId }, function (data) {
                var s = data;
                if (s != "") {
                    arr = s.split(",");
                    if (arr[0] != null && arr[0] != undefined) {
                        $("#UrlEdit").val(arr[0]);
                    }
                    else {
                        $("#UrlEdit").val("");
                    }
                    if (arr[1] != null && arr[1] != undefined && arr[1] !="") {
                        $("#\it").val(arr[1]);
                    }
                    else 
                    {
                        $("#DomainNameEdit").val("");
                        ////Added on July 14, 2014 to domain name from the URL if URL exist and domain name is empty.
                        SetAutoDomainName();
                    }
                }
                else {
                    $("#UrlEdit").val("");
                    $("#DomainNameEdit").val("");
                }

            });
        });

        $('#UrlEdit').blur(function (e) {

            //var urlstring = $("#UrlEdit").val();
            //var re = /(\w+)\.(\w+)(\.\w+){0,1}\/*.*/;
            //if (urlstring.length > 0) {

            //    var matches = urlstring.match(re);
            //    var newDomain = '';
            //    if (matches != null && matches[3] != null) {
            //        newDomain = matches[2] + matches[3];
            //    }
            //    else if (matches != null) {
            //        newDomain = matches[1] + '.' + matches[2];
            //    }

            //    if (newDomain != '') {
            //        $("#DomainNameEdit").val(newDomain);
            //      
            //    }
            //}
            //else {
            //    $("#DomainNameEdit").val('');
            //}

            SetAutoDomainName();

        });
    });

    function SetAutoDomainName() {

        var urlstring = $("#UrlEdit").val();
        var re = /(\w+)\.(\w+)(\.\w+){0,1}\/*.*/;
        if (urlstring.length > 0) 
        {
            var matches = urlstring.match(re);
            var newDomain = '';
            if (matches != null && matches[3] != null) {
                newDomain = matches[2] + matches[3];
            }
            else if (matches != null) {
                newDomain = matches[1] + '.' + matches[2];
            }

            if (newDomain != '') {
                $("#DomainNameEdit").val(newDomain);

            }
        }
        else 
        {
            $("#DomainNameEdit").val('');
        }

    }

</script>

<div class="editTemplateDiv" style="width:600px;height:200px;">
    <%= Html.HiddenFor(Function(m) m.RemoteAuthenticationId, New With {.ID = "RemoteAuthenticationId"})%>
    <%: Html.ValidationSummary("The following errors must be corrected:", New With {.class = "validation-summary-errors-flat"})%>
    <table class="editTemplateTable">
        <tr><td class="editTemplateLabel">College*</td><td><%= Html.DropDownListFor(Function(m) m.CollegeId, CType(ViewData("AvailableColleges"), List(Of SelectListItem)), New With {.style = "width:300px"})%></td></tr>
        <tr><td class="editTemplateLabel" style="width:250px;">Database*</td><td><%: Html.DropDownListFor(Function(m) m.EzProxyDatabaseId, CType(ViewData("AvailableDatabases"), List(Of SelectListItem)), New With {.style = "width:300px"})%></td></tr>
        <tr><td class="editTemplateLabel">Url*</td><td><%: Html.TextBoxFor(Function(m) m.Url, New With {.style = "width:296px", .ID = "UrlEdit", .maxLength = "1000"})%></td></tr>
        <tr><td class="editTemplateLabel">Domain*</td><td><%: Html.TextBoxFor(Function(m) m.DomainName, New With {.style = "width:296px", .ID = "DomainNameEdit", .maxLength = "1000"})%></td></tr>
        <tr><td class="editTemplateLabel">Connection Method*</td><td><%: Html.DropDownListFor(Function(m) m.ConnectionMethodId, CType(ViewData("AvailableConnectionMethods"), List(Of SelectListItem)), New With {.style = "width:300px"})%></td></tr>
        <tr><td class="editTemplateLabel">Campus Restriction</td><td><%: Html.TextBoxFor(Function(m) m.CampusRestriction, New With {.style = "width:296px", .ID = "CampusRestrictionEdit", .maxLength = "1000"})%></td></tr>
        <tr><td class="editTemplateLabel">Full Url</td><td><%: Html.TextBoxFor(Function(m) m.FullUrl, New With {.style = "width:296px", .ID = "FullUrlEdit", .maxLength = "1000"})%></td></tr>
        <tr><td class="editTemplateLabel">Active</td><td><%: Html.CheckBoxFor(Function(m) m.IsActive)%></td></tr>
        <tr><td colspan="2">&nbsp;</td></tr>
       
    </table>
</div>
