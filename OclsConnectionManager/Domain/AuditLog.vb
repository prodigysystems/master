Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Domain

    <Serializable()>
    Public Class AuditLog

        Public Enum EventCodes
            CreateEzProxyDatabase = 1
            UpdateEzProxyDatabase = 2
            DeleteEzProxyDatabase = 3
            CreateRemoteAuthentication = 4
            UpdateRemoteAuthentication = 5
            DeleteRemoteAuthentication = 6
            CreateCollegeIpAddress = 7
            UpdateCollegeIpAddress = 8
            DeleteCollegeIpAddress = 9
        End Enum

        <Key()>
        Public Property AuditLogId() As Integer

        Public Property UserId() As Integer

        Public Overridable Property User() As Domain.UserProfile

        Public Property EventCode() As String

        Public Property KeyId As Nullable(Of Integer)

        Public Property EventDate As DateTime

        Public Property EventDetail() As String

    End Class

End Namespace
