<%@ Page Title="" Language="VB" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage(Of OclsConnectionManager.IpAddressListViewModel)" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    OCLS Connection Manager - IP Addresses
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="" style="width: 775px; height: 400px;">

        <%: Html.ValidationSummary("")%>
             <br />
        <table class="editTemplateTable" >
          
            <tr>
                <td colspan="3">
                    <fieldset>
                        <legend>IP Addresses</legend>
                        <br />
                        
                        <% Using (Html.BeginForm("", "", FormMethod.Post, New With {.defaultbutton = "btnSearch", .clearButton="btnClear", .id = "SearchForm"}))%>
                        <table style="width:100%" >
                            <tr>
                                <td class="editTemplateLabel"  style="text-align: right; width:5%" >College</td>
                                <td style="text-align: left;">
                                    <%= Html.DropDownListFor(Function(m) m.CollegeId, CType(ViewData("AvailableColleges"), List(Of SelectListItem)), New With {.style = "width:300px"})%>
                                </td>
                                <td style="text-align: right; width: 20%">
                                    <input type="button" value="Search" id="btnSearch" onclick="SearchIpAddresses();"
                                        class="t-button" style="width: 55px" />&nbsp;
                                        <input type="button" value="Clear" id="btnClear" onclick="ClearSearch();" class="t-button" style="width: 55px" />
                                </td>
                            </tr>
                        </table>
                        <%End Using %>

                        <br />
                        <% Html.Telerik().Grid(Model.IpAddresses) _
                        .Name("IpAddressesGrid") _
                        .HtmlAttributes(New With {.style = "width:965px;"}) _
                        .NoRecordsTemplate("<br/>") _
                        .DataKeys(Sub(k)
                                      k.Add("CollegeIpAddressId").RouteKey("CollegeIpAddressId")
                                      k.Add("CollegeId").RouteKey("CollegeId")
                                  End Sub) _
                            .Selectable(Sub(s) s.Enabled(False)) _
                              .Scrollable(Sub(s) s.Enabled(True).Height(450)) _
                            .DataBinding(Function(db) db.Ajax().Select("SearchIpAddresses", "IpAddress").Update("UpdateIpAddess", "IpAddress").Insert("InsertIpAddress", "IpAddress").Delete("DeleteIpAddress", "IpAddress").OperationMode(GridOperationMode.Client)) _
                                            .Footer(False) _
                                            .ToolBar(Function(cmds) cmds.Insert()) _
                                            .Columns(Sub(c)
                                                         c.Bound(Function(db) db.CollegeIpAddressId).Hidden()
                                                         c.Bound(Function(db) db.CollegeId).Hidden()
                                                         c.Bound(Function(db) db.CollegeName).Title("College Name").Width(171)
                                                         c.Bound(Function(db) db.Campus).Title("Campus").Width(130)
                                                         c.Bound(Function(db) db.IpAddress).Title("IP Address").Width(150)
                                                         c.Bound(Function(db) db.SubnetMask).Title("Subnet Mask").Width(150)
                                                         c.Bound(Function(db) db.RegularExpression).Title("Regular Expression").Width(200)
                                                         c.Command(Function(cmd) cmd.Edit().ButtonType(GridButtonType.BareImage).Text("Edit").InsertText("Insert").UpdateText("Update").CancelText("Cancel"))
                                                         c.Command(Function(cmd) cmd.Delete().ButtonType(GridButtonType.BareImage).Text("Delete"))
                                                     End Sub
                                            ) _
                                            .ClientEvents(Function(e) e.OnDataBinding("IpAddressesGrid_OnDataBinding").OnSave("IpGridOnSave").OnEdit("IpGridOnEdit")) _
                                            .Editable(Function(e) e.Mode(GridEditMode.InLine).TemplateName("IpAddress").Window(Function(w) w.Modal(True).Title("Add/Edit Ip Address"))) _
                                            .Sortable() _
                                            .Pageable(Function(p) p.Enabled(False)) _
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

        function ClearSearch() {
            //$("#CollegeId").attr('selectedIndex', 0);
            $('#CollegeId').prop('selectedIndex', 0);
            SearchIpAddresses();
        }

        function IpAddressesGrid_OnDataBinding(e) {
            e.data = {
                collegeId: $("#CollegeId").val()   
            };
        }
        function SearchIpAddresses() {          
                var grid = $("#IpAddressesGrid").data("tGrid");
                grid.rebind();          
        }
        function IsValidInt(obj)
        {
            var intRegex = /^\d+$/;
            if (!intRegex.test(obj))
                return false;
            else if (parseInt(obj) < 0 || parseInt(obj) > 255)
            {
                return false;
            }
            return true;
        }
        function IpGridOnEdit(e) {
           
            var mode = e.mode;
            var form = e.form;
            if (mode == "insert") {             
                var chk = $(form).find("#IsActive");
                chk.prop('checked', true);
                var txtSubnetMask = $(form).find("#SubnetMask");
                txtSubnetMask.val("255.255.255.0");

  
                
            }
            else {
                var college = $(form).find("#CollegeName");
                college.data("tDropDownList").disable();
            }

            var oldIpAddress = $(form).find("#IpAddress").val();
            var oldsubnetMask = $(form).find("#SubnetMask").val();
            var intRegex = /^\d+$/;

            $(form).find("#IpAddress")
              .change(function (e) {
                  var txtIA = $(form).find("#IpAddress");
                  var txtReg = $(form).find("#RegularExpression");
                  var arr = txtIA.val().split(".");
                  if (arr.length != 4 || !IsValidInt(arr[0]) || !IsValidInt(arr[1]) || !IsValidInt(arr[2]) ||!IsValidInt(arr[3]) ) {
                      e.preventDefault();
                      $(form).find("#IpAddress").val(oldIpAddress);
                      alert("Ip Address is invalid.");
                      return;
                  }
                  if (arr.length == 4) {
                      var reg = "^" + arr[0] + "\\." + arr[1] + "\\." + arr[2] + "\\." + arr[3] + "$"; //“^204\.225\.107\.49$”
                      txtReg.val(reg);
                  }
              });
            $(form).find("#SubnetMask")
           .change(function (e) {
               var SubnetMask = $(form).find("#SubnetMask").val();
               var arr = SubnetMask.split(".");
               if (arr.length != 4 || !IsValidInt(arr[0]) || !IsValidInt(arr[1]) || !IsValidInt(arr[2]) || !IsValidInt(arr[3])) {
                   e.preventDefault();
                   $(form).find("#SubnetMask").val(oldsubnetMask);
                   alert("Subnet mask is invalid.");
                   return;
               }
           });
        }

        function IpGridOnSave(e) {
            e.preventDefault();
            var collegeId = $("#CollegeId").val();
            var id = $("#CollegeIpAddressId").val();
            var collegeName = $("#CollegeName").val();
            var campus = $("#Campus").val();
            var isActive;
            if ($('#IsActive').is(':checked'))
                isActive = true;
            else
                isActive = false;
            var data = e.values;
            data["CollegeId"] = collegeId;
            e.values = data;
            var ipAddress = data["IpAddress"];
            var SubnetMask = data["SubnetMask"];
            var RegularExpression = data["RegularExpression"];
            if (ipAddress == "" || SubnetMask == "" || RegularExpression == "") {
                e.preventDefault();
                alert("IP Address, Subnet Mask and Regular Expression are required.");
                return false;
            }
            var arr = ipAddress.split(".");
            if (arr.length != 4 || !IsValidInt(arr[0]) || !IsValidInt(arr[1]) || !IsValidInt(arr[2]) || !IsValidInt(arr[3])) {
                e.preventDefault();
                alert("Ip Address is invalid.");
                return false;
            }

             arr = SubnetMask.split(".");
            if (arr.length != 4 || !IsValidInt(arr[0]) || !IsValidInt(arr[1]) || !IsValidInt(arr[2]) || !IsValidInt(arr[3])) {
                e.preventDefault();
                alert("Subnet mask is invalid.");
                return false;
            }

            //validate campus formatting
            var reBadCharacterCheck = /[-!$%^&*()_+|~=`{}\[\]:";'<>?,.\/]/; //check formatting for comma separators
            var reNoSpaceCheck = /^\S+$/; //check that spaces aren't used as separators
            campus = campus.trim().toUpperCase();
            if (campus != "" &&
               (reBadCharacterCheck.test(campus) == true || reNoSpaceCheck.test(campus) == false)) {
                alert("Campus is not correctly formatted.\n" +
                      "Specify only one campus, without any spaces or delimiters.\n\n" +
                      "Examples:\n" +
                      "a)   PROGRESS\n" +
                      "b)   MORNINGSIDE");
                return false;
            }
            if (campus.length > 100) {
                alert("Campus exceeded field size: 100 characters max.");
                return false;
            }


            if (id == "0") {
                if (collegeName == "0") {
                    e.preventDefault();
                    alert("Please select a college.");
                    return false;
                }

                var url = '<%= OclsConnectionManager.RouteConfig.GetActionUri("IpAddress", "InsertIpAddressCustom") %>'; 
                $.post(url, { collegeId: collegeName, campus: campus, ipAddress: ipAddress, subnetMask: SubnetMask, reg: RegularExpression, isActive: isActive }, function (data) 
                {
                    var s = data;
                    if (s == true) {
                        var grid = $("#IpAddressesGrid").data("tGrid");
                        grid.rebind();
                        return true;
                    }
                    else {
                        alert("IP address already exists for the selected college.");
                    }
                });
            }
            else {
                var url = '<%= OclsConnectionManager.RouteConfig.GetActionUri("IpAddress", "UpdateIpAddressCustom") %>'; 

                $.post(url, { ipAddressId: id, collegeId: collegeId, campus: campus, ipAddress: ipAddress, subnetMask: SubnetMask, reg: RegularExpression, isActive: isActive }, function (data) {
                    var s = data;

                    if (s == true) {
                        var grid = $("#IpAddressesGrid").data("tGrid");
                        grid.rebind();
                        return true;
                    }
                    else {
                        alert("IP address already exists for the selected college.");
                    }
                });
            }
            return true;
        }//EOF IpGridOnSave(e) 

        $(function () {

            var gridHeight = $(window).height() - $(".header-content").height() - 400;
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

        });
       
    </script>
</asp:Content>
