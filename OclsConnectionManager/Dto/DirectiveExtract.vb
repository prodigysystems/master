Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Dto

    <Serializable()>
    Public Class DirectiveExtract

        Public Sub New()
            EzProxyDatabase = New EzProxyDatabaseDto() With {.Directives = New List(Of Dto.EzProxyDatabaseDirectiveDto)}
            Errors = New List(Of String)
        End Sub

        Public Property EzProxyDatabase As EzProxyDatabaseDto
        Public Property Errors() As List(Of String)
        Public Property IsSuccessful As Boolean
    End Class
End Namespace

