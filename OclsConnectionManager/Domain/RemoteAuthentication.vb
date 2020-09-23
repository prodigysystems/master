Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Domain

    <Serializable()>
    Public Class RemoteAuthentication

        <Key()>
        Public Property RemoteAuthenticationId() As Integer
        Public Property EzProxyDatabaseId() As Integer
        Public Overridable Property EzProxyDatabase As EzProxyDatabase
        Public Property CollegeId() As Integer
        Public Overridable Property College As College
        Public Property ConnectionMethodId() As Integer
        Public Overridable Property ConnectionMethod As ConnectionMethod
        Public Property CreatedByUserId() As Integer
        Public Overridable Property CreatedByUser() As Domain.UserProfile
        Public Property CreatedDate() As DateTime
        Public Property ModifiedByUserId() As Nullable(Of Integer)
        Public Overridable Property ModifiedByUser() As Domain.UserProfile
        Public Property ModifiedDate As Nullable(Of DateTime)
        Public Property IsActive As Boolean
        Public Property Url As String
        Public Property FullUrl As String
        Public Property DomainName As String
        Public Property CampusRestriction As String

    End Class

End Namespace
