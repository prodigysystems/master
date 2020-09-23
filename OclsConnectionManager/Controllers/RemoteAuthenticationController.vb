Option Explicit On
Option Strict On

Imports Telerik.Web.Mvc
Imports System.Linq

Public Class RemoteAuthenticationController
    Inherits BaseController

    Function Index() As ActionResult
        Dim model As New RemoteAuthenticationListViewModel()
        model.CollegeNameTerm = ""

        Dim colleges As IEnumerable(Of Dto.CollegeDto) = Services.RemoteAuthenticationService.GetAvailableColleges().OrderBy(Function(m) m.Name)
        Dim CollegeList As List(Of SelectListItem) = New List(Of SelectListItem)
        For Each c In colleges
            CollegeList.Add(New SelectListItem With {.Text = c.Name, .Value = c.CollegeId.ToString()})
        Next
        CollegeList.Insert(0, New SelectListItem With {.Text = "--- Select a college ---", .Value = "0"})
        ViewData("AvailableColleges") = CollegeList

        Dim databases As IEnumerable(Of Dto.EzProxyDatabaseDto) = Services.EzProxyService.GetDatabases(0, "").OrderBy(Function(m) m.Name)
        Dim DBList As List(Of SelectListItem) = New List(Of SelectListItem)
        For Each db In databases
            DBList.Add(New SelectListItem With {.Text = db.Name, .Value = db.EzProxyDatabaseId.ToString()})
        Next
        DBList.Insert(0, New SelectListItem With {.Text = "--- Select a database ---", .Value = "0"})
        ViewData("AvailableDatabases") = DBList

        Dim ConnMethods As IEnumerable(Of Dto.ConnectionMethodDto) = Services.RemoteAuthenticationService.GetAvailableConnectionMethods().OrderBy(Function(m) m.Name)
        Dim CMList As List(Of SelectListItem) = New List(Of SelectListItem)
        For Each cm In ConnMethods
            CMList.Add(New SelectListItem With {.Text = cm.Name, .Value = cm.ConnectionMethodId.ToString()})
        Next
        CMList.Insert(0, New SelectListItem With {.Text = "--- Select a connection method ---", .Value = "0"})
        ViewData("AvailableConnectionMethods") = CMList

        Return View(model)
    End Function

    <GridAction()>
    Function SearchRemoteAuthentications(databaseName As String, url As String, useFullUrl As Boolean, domainName As String, collegeId As String, copyRaId As Integer) As ActionResult
        Dim ras As IEnumerable(Of Dto.RemoteAuthenticationDto) = Services.RemoteAuthenticationService.GetRemoteAuthenticationsList(databaseName, url, useFullUrl, domainName, collegeId)
        If copyRaId <> 0 Then
            Dim dto As Dto.RemoteAuthenticationDto = New Dto.RemoteAuthenticationDto()
            dto = Services.RemoteAuthenticationService.GetRemoteAuthentication(copyRaId)
            dto.RemoteAuthenticationId = 0
            dto.CollegeId = 0
            dto.CollegeName = ""
            Dim ret As List(Of Dto.RemoteAuthenticationDto)
            ret = ras.ToList()

            ret.Insert(0, dto)
            ras = CType(ret, IEnumerable(Of Dto.RemoteAuthenticationDto))
        End If
        Return View(New GridModel(ras))
    End Function

    <GridAction()>
    <HttpPost()>
    Function InsertRA() As ActionResult
        Dim dto As Dto.RemoteAuthenticationDto = New Dto.RemoteAuthenticationDto()
        TryUpdateModel(dto)
        Dim ras As List(Of Dto.CollegeIpAddressDto) = New List(Of Dto.CollegeIpAddressDto)
        If Not IsNothing(Session("IpAddresses")) Then

            ras = CType(Session("IpAddresses"), List(Of Dto.CollegeIpAddressDto))
        End If
        Services.RemoteAuthenticationService.AddRemoteAuthentication(dto, HttpContext.User.Identity.Name)
        Session("IpAddresses") = Nothing
        Return View(New GridModel(ras))
    End Function


    <HttpPost()>
    Function InsertRACustom(dbId As String, collegeId As String, url As String, fullUrl As String,
                            domainName As String, campusRestriction As String,
                            connectionMethodId As String, isActive As Boolean) As JsonResult

        Dim ras As List(Of Dto.CollegeIpAddressDto) = New List(Of Dto.CollegeIpAddressDto)
        Dim dto As Dto.RemoteAuthenticationDto = New Dto.RemoteAuthenticationDto()

        dto.CollegeId = Integer.Parse(collegeId)
        dto.EzProxyDatabaseId = Integer.Parse(dbId)
        dto.ConnectionMethodId = Integer.Parse(connectionMethodId)
        dto.IsActive = isActive
        dto.Url = url
        dto.FullUrl = fullUrl
        dto.DomainName = domainName
        dto.CampusRestriction = campusRestriction

        If Not IsNothing(Session("IpAddresses")) Then

            ras = CType(Session("IpAddresses"), List(Of Dto.CollegeIpAddressDto))
        End If
        Dim ret As Boolean = Services.RemoteAuthenticationService.AddRemoteAuthentication(dto, HttpContext.User.Identity.Name)
        If ret Then
            Session("IpAddresses") = Nothing
        End If

        Return Json(ret)
    End Function


    <GridAction()>
    <HttpPost()>
    Function DeleteRA(ipdelete As Dto.RemoteAuthenticationDto) As ActionResult
        Dim dto As Dto.RemoteAuthenticationDto = New Dto.RemoteAuthenticationDto()
        TryUpdateModel(dto)
        Dim ras As List(Of Dto.CollegeIpAddressDto) = Nothing
        If Not IsNothing(Session("IpAddresses")) Then

            ras = CType(Session("IpAddresses"), List(Of Dto.CollegeIpAddressDto))
        End If
        Services.RemoteAuthenticationService.AddRemoteAuthentication(dto, HttpContext.User.Identity.Name)
        Session("IpAddresses") = Nothing
        Return View(New GridModel(ras))
    End Function

    <GridAction()>
    Function UpdateRA() As ActionResult
        Dim dto As Dto.RemoteAuthenticationDto = New Dto.RemoteAuthenticationDto()
        TryUpdateModel(dto)
        Dim ras As List(Of Dto.CollegeIpAddressDto) = Nothing
        If Not IsNothing(Session("IpAddresses")) Then

            ras = CType(Session("IpAddresses"), List(Of Dto.CollegeIpAddressDto))
        End If
        Services.RemoteAuthenticationService.UpdateRemoteAuthentication(dto, HttpContext.User.Identity.Name)
        Session("IpAddresses") = Nothing
        Return View(New GridModel(ras))
    End Function

    <HttpPost()>
    Function UpdateRACustom(raId As String, dbId As String, collegeId As String, url As String, fullUrl As String,
                            domainName As String, campusRestriction As String, connectionMethodId As String,
                            isActive As Boolean) As JsonResult
        If raId = "0" Then
            InsertRACustom(dbId, collegeId, url, fullUrl, domainName, campusRestriction, connectionMethodId, isActive)
        Else
            Dim dto As Dto.RemoteAuthenticationDto = New Dto.RemoteAuthenticationDto()
            dto.RemoteAuthenticationId = Integer.Parse(raId)
            dto.CollegeId = Integer.Parse(collegeId)
            dto.EzProxyDatabaseId = Integer.Parse(dbId)
            dto.ConnectionMethodId = Integer.Parse(connectionMethodId)
            dto.IsActive = isActive
            dto.Url = url
            dto.FullUrl = fullUrl
            dto.DomainName = domainName
            dto.CampusRestriction = campusRestriction
            Dim ret As Boolean = Services.RemoteAuthenticationService.UpdateRemoteAuthentication(dto, HttpContext.User.Identity.Name)
            Return Json(ret)
        End If

        Return Nothing
    End Function

    <HttpPost()>
    Function DeleteRACustom(id As String) As Boolean
        Services.RemoteAuthenticationService.DeleteRemoteAuthentication(Integer.Parse(id), HttpContext.User.Identity.Name)
        Return True
    End Function
    <HttpPost()>
    Function GetDBDirective(id As String) As JsonResult
        Dim dbId As Integer = Integer.Parse(id)
        Dim div As String = Services.RemoteAuthenticationService.GetDatabaseDirective(dbId)
        Return Json(div)
    End Function

    <HttpPost()>
    Function RemoteAuthenticationExist(raId As String, dbId As String, collegeId As String, connectionMethodId As String) As JsonResult

        Dim div As Boolean = Services.RemoteAuthenticationService.RemoteAuthenticationExist(Integer.Parse(raId), Integer.Parse(dbId), Integer.Parse(collegeId), Integer.Parse(connectionMethodId))
        Return Json(div)
    End Function
    <HttpPost()>
    Function ClearSession() As JsonResult

        Session("IpAddresses") = Nothing
        Return Json("True")
    End Function
    <HttpPost()>
    Function GetConnectionMethodIdByName(name As String) As JsonResult
        Return Json(Services.RemoteAuthenticationService.GetConnectionMethodIdByName(name))
    End Function
End Class
