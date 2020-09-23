Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Dto

    <Serializable()>
    Public Class EzProxyServerDto

        Public Property EzProxyServerId() As Integer
        Public Property Name() As String
        Public Property FileShare As String
        Public Property FileName As String
        Public Property FileShareUsername() As String
        Public Property FileSharePassword() As String
        Public Property FileShareDomain() As String

        Public Shared Function GetDto(domain As Domain.EzProxyServer) As EzProxyServerDto
            Return New EzProxyServerDto() With {
                .EzProxyServerId = domain.EzProxyServerId,
                .Name = domain.Name,
                .FileName = domain.FileName,
                .FileShare = domain.FileShare,
                .FileShareUsername = domain.FileShareUsername,
                .FileSharePassword = domain.FileSharePassword,
                .FileShareDomain = domain.FileShareDomain
            }
        End Function

    End Class

End Namespace
