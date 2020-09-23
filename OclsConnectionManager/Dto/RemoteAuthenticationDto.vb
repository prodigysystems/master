Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Dto

    <Serializable()>
    Public Class RemoteAuthenticationDto

        Public Property RemoteAuthenticationId() As Integer
        Public Property EzProxyDatabaseId() As Integer
        Public Property DatabaseName As String
        Public Property CollegeId() As Integer
        Public Property CollegeName As String
        Public Property ConnectionMethodId() As Integer
        Public Property ConnectionMethodName() As String
        Public Property ModifiedBy() As String
        Public Property ModifiedDate() As DateTime
        Public Property IsActive() As Boolean
        Public Property Url As String
        Public Property FullUrl As String
        Public Property DomainName As String
        Public Property CampusRestriction As String 'string of subscribing campuses for publisher

        Public Shared Function GetDto(domain As Domain.RemoteAuthentication) As RemoteAuthenticationDto

            Dim directive As Domain.EzProxyDatabaseDirective
            Dim directiveOption As Domain.EzProxyDatabaseDirectiveOption
            Dim dto As New RemoteAuthenticationDto() With {
                .RemoteAuthenticationId = domain.RemoteAuthenticationId,
                .EzProxyDatabaseId = domain.EzProxyDatabaseId,
                .DatabaseName = domain.EzProxyDatabase.Name,
                .CollegeId = domain.CollegeId,
                .CollegeName = domain.College.Name,
                .ConnectionMethodId = domain.ConnectionMethodId,
                .ConnectionMethodName = domain.ConnectionMethod.Name,
                .CampusRestriction = domain.CampusRestriction,
                .IsActive = domain.IsActive,
                .FullUrl = domain.FullUrl
            }

            If Not String.IsNullOrEmpty(domain.Url) Then
                dto.Url = domain.Url
            Else
                directive = domain.EzProxyDatabase.Directives.Where(Function(d) d.EzProxyDirective.Name = "URL").FirstOrDefault()
                If Not directive Is Nothing Then
                    directiveOption = directive.Options.Where(Function(o) o.EzProxyOption.Name = "url").FirstOrDefault()
                    If Not directiveOption Is Nothing Then
                        dto.Url = directiveOption.OptionValue
                    End If
                End If
            End If

            If Not String.IsNullOrEmpty(domain.DomainName) Then
                dto.DomainName = domain.DomainName
            Else
                directive = domain.EzProxyDatabase.Directives.Where(Function(d) d.EzProxyDirective.Name = "Domain").FirstOrDefault()
                If Not directive Is Nothing Then
                    directiveOption = directive.Options.Where(Function(o) o.EzProxyOption.Name = "wilddomain").FirstOrDefault()
                    If Not directiveOption Is Nothing Then
                        dto.DomainName = directiveOption.OptionValue
                    End If
                End If
            End If

            If domain.ModifiedByUserId.HasValue Then
                dto.ModifiedBy = domain.ModifiedByUser.UserName
                dto.ModifiedDate = domain.ModifiedDate.Value
            Else
                dto.ModifiedBy = domain.CreatedByUser.UserName
                dto.ModifiedDate = domain.CreatedDate
            End If

            Return dto

        End Function

    End Class

End Namespace
