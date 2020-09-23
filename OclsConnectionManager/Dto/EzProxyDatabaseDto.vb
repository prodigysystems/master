Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports OclsConnectionManager


Namespace Dto

    <Serializable()>
    Public Class EzProxyDatabaseDto

        Public Property EzProxyDatabaseId() As Integer
        Public Property Name() As String
        Public Property Title() As String
        Public Property PerformDomainAutocomplete() As Boolean = True
        Public Property Url() As String
        Public Property DomainName As String
        Public Property Comment() As String
        Public Property ModifiedBy() As String
        Public Property ModifiedDate() As DateTime
        Public Property OutputOrder() As Nullable(Of Integer)
        Public Property IsVisibleInRemoteAuthentication As Boolean
        Public Property IsActive() As Boolean
        Public Property EzProxyServerId() As Integer
        Public Property EzProxyServerName() As String
        Public Overridable Property Directives As IEnumerable(Of EzProxyDatabaseDirectiveDto)

        Public Shared Function GetDto(domain As Domain.EzProxyDatabase, includeDetails As Boolean) As EzProxyDatabaseDto
            Dim dto As New EzProxyDatabaseDto() With {
                .EzProxyDatabaseId = domain.EzProxyDatabaseId,
                .Name = domain.Name,
                .Title = domain.Title,
                .Url = domain.Url,
                .DomainName = domain.DomainName,
                .Comment = domain.Comment,
                .IsVisibleInRemoteAuthentication = domain.IsVisibleInRemoteAuthentication,
                .OutputOrder = domain.OutputOrder,
                .IsActive = domain.IsActive,
                .EzProxyServerId = domain.EzProxyServerId.GetValueOrDefault(),
                .EzProxyServerName = domain.EzProxyServer?.Name
            }

            If domain.ModifiedDate.HasValue Then
                dto.ModifiedDate = domain.ModifiedDate.Value
            Else
                dto.ModifiedDate = domain.CreatedDate
            End If

            If domain.ModifiedByUserId.HasValue Then
                dto.ModifiedBy = domain.ModifiedByUser.UserName
            Else
                dto.ModifiedBy = domain.CreatedByUser.UserName
            End If

            If includeDetails Then
                If Not domain.Directives Is Nothing Then
                    dto.Directives = domain.Directives.Select(Function(d) EzProxyDatabaseDirectiveDto.GetDto(d)).ToArray().ToList()
                Else
                    dto.Directives = New List(Of EzProxyDatabaseDirectiveDto)
                End If

            End If

            If String.IsNullOrEmpty(dto.EzProxyServerName) Then
                dto.EzProxyServerName = "- All servers -"
            End If

            Return dto
        End Function

    End Class

End Namespace