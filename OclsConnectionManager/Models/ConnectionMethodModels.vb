Imports System.Collections
Imports System.Collections.Generic
Imports OclsConnectionManager.Dto
Public Class ConnectionMethodListViewModel
    Public Property ConnectionMethods() As IEnumerable(Of ConnectionMethodDto )
    Public Sub New()
        ConnectionMethods = New List(Of ConnectionMethodDto)()

    End Sub
End Class
