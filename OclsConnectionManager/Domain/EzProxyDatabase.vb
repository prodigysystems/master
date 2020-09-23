Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Domain

    <Serializable()>
    Public Class EzProxyDatabase

        <Key()>
        Public Property EzProxyDatabaseId() As Integer
        Public Property Name() As String
        Public Property Title() As String
        Public Property Url() As String
        Public Property DomainName() As String
        Public Property Comment() As String
        Public Property CreatedByUserId() As Integer
        Public Overridable Property CreatedByUser() As Domain.UserProfile
        Public Property CreatedDate() As DateTime
        Public Property ModifiedByUserId() As Nullable(Of Integer)
        Public Overridable Property ModifiedByUser() As Domain.UserProfile
        Public Property ModifiedDate() As Nullable(Of DateTime)
        Public Property OutputOrder() As Nullable(Of Integer)
        Public Property IsVisibleInRemoteAuthentication() As Boolean
        Public Property IsActive() As Boolean
        Public Overridable Property Directives As ICollection(Of EzProxyDatabaseDirective)
        Public Property EzProxyServerId As Nullable(Of Integer)
        Public Overridable Property EzProxyServer() As Domain.EzProxyServer

        Public Sub New()
            Directives = New List(Of EzProxyDatabaseDirective)()
            CreatedByUserId = CreatedByUserId
            CreatedDate = DateTime.Now
            EzProxyServerId = Nothing
        End Sub

    End Class

End Namespace