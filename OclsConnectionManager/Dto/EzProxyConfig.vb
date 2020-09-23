Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Dto

    <Serializable()>
    Public Class EzProxyConfigDto

        Public Property EzProxyConfigId() As Integer
        Public Property ConfigContents() As String
        Public Property CreatedBy() As String
        Public Property CreatedDate() As DateTime
        Public Property EzProxyServerId() As Integer

        Public Shared Function GetDto(domain As Domain.EzProxyConfig) As EzProxyConfigDto
            Return New EzProxyConfigDto() With {
                .EzProxyConfigId = domain.EzProxyConfigId,
                .ConfigContents = domain.ConfigContents,
                .CreatedBy = domain.CreatedByUser.UserName,
                .CreatedDate = domain.CreatedDate,
                .EzProxyServerId = domain.EzProxyServerId
            }
        End Function

    End Class

End Namespace
