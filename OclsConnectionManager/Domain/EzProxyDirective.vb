Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Domain

    <Serializable()>
    Public Class EzProxyDirective

        <Key()>
        Public Property EzProxyDirectiveId() As Integer
        Public Property Name() As String
        Public Property Description() As String
        Public Property OutputAs() As String
        Public Property OutputOrder() As Nullable(Of Integer)
        Public Property MaxOccursPerConfig() As Nullable(Of Integer)
        Public Property MaxOccursPerDatabase() As Nullable(Of Integer)
        Public Property IsDefault() As Boolean
        Public Overridable Property Options As ICollection(Of EzProxyOption)

    End Class

End Namespace
