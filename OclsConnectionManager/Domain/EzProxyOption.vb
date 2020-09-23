Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Domain

    <Serializable()>
    Public Class EzProxyOption

        <Key()>
        Public Property EzProxyOptionId() As Integer
        Public Property EzProxyDirectiveId() As Integer
        Public Overridable Property EzProxyDirective As EzProxyDirective
        Public Property OptionOrQualifier() As String
        Public Property Name() As String
        Public Property Description() As String
        Public Property OutputAs() As String
        Public Property OutputOrder() As Nullable(Of Integer)
        Public Property IsRequired() As Boolean
        Public Property HasInputValue() As Boolean

    End Class

End Namespace
