Imports System.Collections
Imports System.Collections.Generic
Imports OclsConnectionManager.Dto

Public Class RemoteAuthenticationListViewModel

    Public Property RemoteAuthentications() As IEnumerable(Of RemoteAuthenticationDto)
    Public Property DatabaseNameTerm() As String
    Public Property UrlTerm As String
    Public Property UseFullUrl As Boolean
    Public Property DomainNameTerm As String
    Public Property CollegeNameTerm As String
    Public Property AvailableColleges() As IEnumerable(Of CollegeDto)
    Public Property PageSizeInDropDown As Integer()
    Public Property CurrentPageSize As Integer

    Public Sub New()
        RemoteAuthentications = New List(Of RemoteAuthenticationDto)()
        AvailableColleges = New List(Of CollegeDto)()
        PageSizeInDropDown = New Integer() {10, 20, 50, 100, 1000, 10000}
        CurrentPageSize = 20
    End Sub
End Class
