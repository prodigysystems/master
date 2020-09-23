Imports System.Collections
Imports System.Collections.Generic
Imports OclsConnectionManager.Dto
Public Class CollegeListViewModel
    Public Property Colleges() As IEnumerable(Of CollegeDto)
    Public Sub New()
        Colleges = New List(Of CollegeDto)()

    End Sub
End Class
