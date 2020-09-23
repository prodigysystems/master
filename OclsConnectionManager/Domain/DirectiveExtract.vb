Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Namespace Domain

    <Serializable()>
    Public Class DirectiveExtract
        Public Overridable Property Directives As ICollection(Of EzProxyDatabaseDirective)
        Public Property Errors() As List(Of String)
        Public Property IsSuccessful As Boolean
    End Class
End Namespace

