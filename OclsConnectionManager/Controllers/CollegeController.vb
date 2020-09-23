Imports Telerik.Web.Mvc
Imports System.Linq
Public Class CollegeController
    Inherits BaseController

    '
    ' GET: /College

    Function Index() As ActionResult
        Dim model As New CollegeListViewModel()
        Dim ras As List(Of Dto.CollegeDto) = Nothing
        ras = Services.LookupTablesService.GetColleges().ToList()
        model.Colleges = ras
        Return View(model)
    End Function
    <GridAction()>
    Public Function SearchCollege() As ActionResult
        Dim ras As List(Of Dto.CollegeDto) = Nothing
        ras = Services.LookupTablesService.GetColleges().ToList()
        Return View(New GridModel(ras))
    End Function
    <GridAction()>
    Public Function DeleteCollege() As ActionResult
        Dim college As Dto.CollegeDto = New Dto.CollegeDto
        TryUpdateModel(college)
        Dim ret As Boolean = Services.LookupTablesService.DeleteCollege(college)
    
        Dim ras As List(Of Dto.CollegeDto) = Nothing
        ras = Services.LookupTablesService.GetColleges().ToList()
        Return View(New GridModel(ras))

    End Function

    <HttpPost()>
    Function DeleteCollegeCustom(name As String) As JsonResult
        Dim ret As Boolean = Services.LookupTablesService.DeleteCollegebyName(name)
        Return Json(ret)
    End Function

    <GridAction()>
    Public Function UpdateCollege() As ActionResult
        Dim college As Dto.CollegeDto = New Dto.CollegeDto
        TryUpdateModel(college)
        Services.LookupTablesService.UpdateCollege(college)
        Dim ras As List(Of Dto.CollegeDto) = Nothing
        ras = Services.LookupTablesService.GetColleges().ToList()
        Return View(New GridModel(ras))
    End Function
    <GridAction()>
    Function InsertCollege() As ActionResult
         Dim college As Dto.CollegeDto = New Dto.CollegeDto
        TryUpdateModel(college)
        ViewBag.Message = "successfully save."
        Services.LookupTablesService.AddCollege(college)
        Dim ras As List(Of Dto.CollegeDto) = Nothing
        ras = Services.LookupTablesService.GetColleges().ToList()
        Return View(New GridModel(ras))
    End Function
    <HttpPost()>
    Function CollegeExist(id As Integer, name As String) As JsonResult

        Dim div As Boolean = Services.LookupTablesService.CollegeExist(id, name)
        Return Json(div)
    End Function
    <HttpPost()>
    Function CollegeCodeExist(id As Integer, code As Integer) As JsonResult

        Dim div As Boolean = Services.LookupTablesService.CollegeCodeExist(id, code)
        Return Json(div)
    End Function
    <HttpPost()>
    Function InsertCollegeCustom(code As String, name As String) As JsonResult
        Dim dto As Dto.CollegeDto = New Dto.CollegeDto
        dto.Name = name
        dto.Code = code
        Dim div As Boolean = Services.LookupTablesService.AddCollege(dto)
        Return Json(div)
    End Function

    <HttpPost()>
    Function UpdateCollegeCustom(id As String, code As String, name As String) As JsonResult
        Dim dto As Dto.CollegeDto = New Dto.CollegeDto
        dto.Code = code
        dto.Name = name
        dto.CollegeId = id
        Dim div As Boolean = Services.LookupTablesService.UpdateCollege(dto)
        Return Json(div)
    End Function


End Class
