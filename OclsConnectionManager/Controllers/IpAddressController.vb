Option Explicit On
Option Strict On

Imports Telerik.Web.Mvc
Imports System.Linq


Public Class IpAddressController
    Inherits BaseController

    Function Index() As ActionResult

        Dim model As New IpAddressListViewModel()
        model.CollegeName = ""

        Dim colleges As IEnumerable(Of Dto.CollegeDto) = Services.RemoteAuthenticationService.GetAvailableColleges().OrderBy(Function(m) m.Name)
        Dim CollegeList As List(Of SelectListItem) = New List(Of SelectListItem)
        For Each c In colleges
            CollegeList.Add(New SelectListItem With {.Text = c.Name, .Value = c.CollegeId.ToString()})
        Next
        CollegeList.Insert(0, New SelectListItem With {.Text = "--- Select a college ---", .Value = "0"})
        ViewData("AvailableColleges") = CollegeList


        Return View(model)
    End Function
    <GridAction()>
    Function SearchIpAddresses(collegeId As String) As ActionResult
        Dim intId As Integer = 0
        Dim ras As IEnumerable(Of Dto.CollegeIpAddressDto) = Nothing
        Integer.TryParse(collegeId, intId)
        ras = Services.IpAddressService.GetIpAddressesByCollegeId(intId).ToList()
        Return View(New GridModel(ras))
    End Function
  
    <GridAction()>
    Function UpdateIpAddess() As ActionResult
        Dim IpAddressUp As Dto.CollegeIpAddressDto = New Dto.CollegeIpAddressDto
        Dim dto As Dto.CollegeIpAddressDto = New Dto.CollegeIpAddressDto()
        Dim raId As Integer = 0

        TryUpdateModel(IpAddressUp)
        Dim ret As Boolean = Services.IpAddressService.UpdateIpAddress(IpAddressUp, HttpContext.User.Identity.Name)
        If Not ret Then
            ViewData("msg") = "IP address already exists for the selected college."
        End If
        raId = IpAddressUp.CollegeId
        Dim ras As List(Of Dto.CollegeIpAddressDto) = Nothing

        ras = Services.IpAddressService.GetIpAddressesByCollegeId(raId).ToList()

        Return View(New GridModel(ras))
    End Function

    <GridAction()>
    Function InsertIpAddress() As ActionResult
        Dim IpAddress As Dto.CollegeIpAddressDto = New Dto.CollegeIpAddressDto
        Dim collegeId As Integer = 0

        TryUpdateModel(IpAddress)
        IpAddress.IsActive = True
        Dim ret As Boolean = Services.IpAddressService.AddIpAddresse(IpAddress, HttpContext.User.Identity.Name)
        If Not ret Then
            ViewData("msg") = "IP address already exists for the selected college."
        End If
        collegeId = IpAddress.CollegeId
        Dim ras As List(Of Dto.CollegeIpAddressDto) = Nothing
        ras = Services.IpAddressService.GetIpAddressesByCollegeId(collegeId).ToList()
        Return View(New GridModel(ras))
    End Function
    <GridAction()>
    Function DeleteIpAddress() As ActionResult

        Dim IpAddress As Dto.CollegeIpAddressDto = New Dto.CollegeIpAddressDto
        TryUpdateModel(IpAddress)
        Services.IpAddressService.DeleteIpAddresse(IpAddress, HttpContext.User.Identity.Name)
        Dim ras As List(Of Dto.CollegeIpAddressDto) = Nothing
        ras = Services.IpAddressService.GetIpAddressesByCollegeId(IpAddress.CollegeId).ToList()
        Return View(New GridModel(ras))
    End Function
    <HttpPost()>
    Function InsertIpAddressCustom(collegeId As Integer, campus As String, ipAddress As String, subnetMask As String, _
                                   reg As String, isActive As Boolean) As JsonResult
        Dim dto As Dto.CollegeIpAddressDto = New Dto.CollegeIpAddressDto
        dto.CollegeId = collegeId
        dto.Campus = campus.ToUpper()
        dto.IpAddress = ipAddress
        dto.SubnetMask = subnetMask
        dto.RegularExpression = reg
        dto.IsActive = True
        Dim div As Boolean = Services.IpAddressService.AddIpAddresse(dto, HttpContext.User.Identity.Name)
        Return Json(div)
    End Function

    <HttpPost()>
    Function UpdateIpAddressCustom(ipAddressId As Integer, collegeId As Integer, campus As String, ipAddress As String, _
                                   subnetMask As String, reg As String, isActive As Boolean) As JsonResult
        Dim dto As Dto.CollegeIpAddressDto = New Dto.CollegeIpAddressDto
        dto.CollegeId = collegeId
        dto.Campus = campus.ToUpper()
        dto.IpAddress = ipAddress
        dto.SubnetMask = subnetMask
        dto.RegularExpression = reg
        'dto.IsActive = isActive 'original code didn't work as there is no active checkbox on the form from which to get data
        dto.IsActive = True
        dto.CollegeIpAddressId = ipAddressId
        Dim div As Boolean = Services.IpAddressService.UpdateIpAddress(dto, HttpContext.User.Identity.Name)
        Return Json(div)
    End Function
End Class
