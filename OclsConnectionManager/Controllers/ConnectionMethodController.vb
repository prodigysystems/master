Option Explicit On
Option Strict On

Imports Telerik.Web.Mvc
Imports System.Linq
Public Class ConnectionMethodController
    Inherits BaseController
    Function Index() As ActionResult
        Dim model As New ConnectionMethodListViewModel()
        Dim ras As List(Of Dto.ConnectionMethodDto) = Nothing
        ras = Services.LookupTablesService.GetConnectionMethods().ToList()
        model.ConnectionMethods = ras
        Return View(model)
    End Function
    <GridAction()>
    Public Function SearchConnectionMethod() As ActionResult
        Dim ras As List(Of Dto.ConnectionMethodDto) = Nothing
        ras = Services.LookupTablesService.GetConnectionMethods().ToList()
        Return View(New GridModel(ras))
    End Function
    <GridAction()>
    Public Function DeleteConnectionMethod() As ActionResult
        Dim ConnectionMethod As Dto.ConnectionMethodDto = New Dto.ConnectionMethodDto
        TryUpdateModel(ConnectionMethod)
        Services.LookupTablesService.DeleteConnectionMethod(ConnectionMethod)
        Dim ras As List(Of Dto.ConnectionMethodDto) = Nothing
        ras = Services.LookupTablesService.GetConnectionMethods().ToList()
        Return View(New GridModel(ras))

    End Function
    <HttpPost()>
    Function DeleteConnectionMethodCustom(name As String) As JsonResult
        Dim ret As Boolean = Services.LookupTablesService.DeleteConnectionMethodbyName(name)
        Return Json(ret)
    End Function

    <GridAction()>
    Public Function UpdateConnectionMethod() As ActionResult
        Dim ConnectionMethod As Dto.ConnectionMethodDto = New Dto.ConnectionMethodDto
        TryUpdateModel(ConnectionMethod)
        Services.LookupTablesService.UpdateConnectionMethod(ConnectionMethod)
        Dim ras As List(Of Dto.ConnectionMethodDto) = Nothing
        ras = Services.LookupTablesService.GetConnectionMethods().ToList()
        Return View(New GridModel(ras))
    End Function
    <GridAction()>
    Function InsertConnectionMethod() As ActionResult
        Dim ConnectionMethod As Dto.ConnectionMethodDto = New Dto.ConnectionMethodDto
        TryUpdateModel(ConnectionMethod)
        Services.LookupTablesService.AddConnectionMethod(ConnectionMethod)
        Dim ras As List(Of Dto.ConnectionMethodDto) = Nothing
        ras = Services.LookupTablesService.GetConnectionMethods().ToList()
        Return View(New GridModel(ras))
    End Function
    <HttpPost()>
    Function ConnectionMethodExist(id As Integer, name As String) As JsonResult

        Dim div As Boolean = Services.LookupTablesService.ConnectionMethodExist(id, name)
        Return Json(div)
    End Function
    <HttpPost()>
    Function ConnectionMethodCodeExist(id As Integer, code As Integer) As JsonResult

        Dim div As Boolean = Services.LookupTablesService.ConnectionMethodCodeExist(id, code)
        Return Json(div)
    End Function

    <HttpPost()>
    Function InsertConnectionMethodCustom(code As Integer, name As String) As JsonResult
        Dim dto As Dto.ConnectionMethodDto = New Dto.ConnectionMethodDto
        dto.Name = name
        dto.Code = code
        Dim div As Boolean = Services.LookupTablesService.AddConnectionMethod(dto)
        Return Json(div)
    End Function

    <HttpPost()>
    Function UpdateConnectionMethodCustom(id As String, code As Integer, name As String) As JsonResult
        Dim dto As Dto.ConnectionMethodDto = New Dto.ConnectionMethodDto
        dto.Name = name
        dto.Code = code
        dto.ConnectionMethodId = Integer.Parse(id)
        Dim div As Boolean = Services.LookupTablesService.UpdateConnectionMethod(dto)
        Return Json(div)
    End Function

End Class
