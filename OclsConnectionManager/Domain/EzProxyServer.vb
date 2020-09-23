Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Domain

    <Serializable()>
    Public Class EzProxyServer

        <Key()>
        Public Property EzProxyServerId() As Integer
        Public Property Name() As String
        Public Property FileShare() As String
        Public Property FileName() As String
        Public Property FileShareUsername() As String
        Public Property FileSharePassword() As String
        Public Property FileShareDomain() As String

    End Class

End Namespace