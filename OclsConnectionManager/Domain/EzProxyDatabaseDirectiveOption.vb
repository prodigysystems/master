Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Domain

    <Serializable()>
    Public Class EzProxyDatabaseDirectiveOption

        <Key()>
        Public Property EzProxyDatabaseDirectiveOptionId() As Integer
        Public Property EzProxyDatabaseDirectiveId() As Integer
        Public Overridable Property EzProxyDatabaseDirective() As EzProxyDatabaseDirective
        Public Property EzProxyOptionId() As Integer
        Public Overridable Property EzProxyOption() As EzProxyOption

        '<MaxLength(1000, ErrorMessage:="OptionValue field cannot exceed 1000 characters.")>
        Public Property OptionValue() As String
        Public Property IsActive() As Boolean

    End Class

End Namespace