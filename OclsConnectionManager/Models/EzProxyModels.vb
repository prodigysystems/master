Imports System.Collections
Imports System.Collections.Generic
Imports OclsConnectionManager.Dto

Public Class EzProxyDatabaseListViewModel

    Public Property Databases() As IEnumerable(Of EzProxyDatabaseDto)
    Public Property AllDirectives() As List(Of EzProxyDirectiveDto)

    Public Property DatabaseSearchTerm() As String
    Public Property URLSearchTerm() As String
    Public Property DomainNameSearchTerm() As String
    ''This will search all the other fields by keywords
    Public Property KeywordSearchTerm() As String
    Public Property PageSizeInDropDown() As Integer()
    Public Property CurrentPageSize() As Integer
    Public Property FilterEzProxyServerId() As Integer
    Public Property SaveOrPreviewEzProxyServerId() As Integer
    Public Property EzProxyServers() As IEnumerable(Of EzProxyServerDto)

    Public Sub New()
        Databases = New List(Of EzProxyDatabaseDto)()
        AllDirectives = New List(Of EzProxyDirectiveDto)
        EzProxyServers = New List(Of EzProxyServerDto)
        PageSizeInDropDown = New Integer() {10, 20, 50, 100, 1000, 10000}
        CurrentPageSize = 20
    End Sub
End Class
