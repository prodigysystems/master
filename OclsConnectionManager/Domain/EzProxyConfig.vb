Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Domain

    <Serializable()>
    Public Class EzProxyConfig

        <Key()>
        Public Property EzProxyConfigId() As Integer
        Public Property ConfigContents() As String
        Public Property CreatedByUserId() As Integer
        Public Overridable Property CreatedByUser() As Domain.UserProfile
        Public Property CreatedDate() As DateTime
        Public Property EzProxyServerId() As Integer
        Public Property EzProxyServer() As EzProxyServer
    End Class

End Namespace