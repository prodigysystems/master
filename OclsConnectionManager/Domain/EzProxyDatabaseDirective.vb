Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports OclsConnectionManager

Namespace Domain

    <Serializable()>
    Public Class EzProxyDatabaseDirective

        <Key()>
        Public Property EzProxyDatabaseDirectiveId() As Integer
        Public Property EzProxyDatabaseId() As Integer
        Public Overridable Property EzProxyDatabase() As EzProxyDatabase
        Public Property EzProxyDirectiveId() As Integer
        Public Overridable Property EzProxyDirective() As EzProxyDirective
        Public Property OutputAs() As String
        Public Property OutputOrder() As Integer
        Public Property Comment() As String
        Public Property IsActive() As Boolean
        Public Overridable Property Options As ICollection(Of EzProxyDatabaseDirectiveOption)



        Public Sub New()
            Options = New List(Of EzProxyDatabaseDirectiveOption)()
        End Sub

        Public Shared Function GetDto(domain As EzProxyDatabaseDirective, includeDetails As Boolean) As Dto.EzProxyDatabaseDirectiveDto
            Dim dto As New OclsConnectionManager.Dto.EzProxyDatabaseDirectiveDto() With {
                .EzProxyDirectiveId = domain.EzProxyDirectiveId,
                .EzProxyDatabaseId = domain.EzProxyDatabaseId,
                .OutputOrder = domain.OutputOrder,
                .OutputAs = domain.OutputAs,
                .Comment = domain.Comment,
                .IsActive = domain.IsActive
            }

            'If includeDetails Then
            '    dto.Options = domain.Options.Select(Function(o) EzProxyOptionDto.GetDto(o)).ToArray().ToList()
            'End If

            If includeDetails Then
                If Not domain.Options Is Nothing Then
                    'dto.Options = domain.Options.Select(Function(d) dto.EzProxyDatabaseDirectiveOptiond.GetDto(d)).ToArray().ToList()
                    dto.Options = domain.Options.Select(Function(d) OclsConnectionManager.Dto.EzProxyDatabaseDirectiveOptionDto.GetDto(d)).ToArray().ToList()
                    'dto.Options = domain.Options.ToArray().ToList()
                Else
                    dto.Options = New List(Of Dto.EzProxyDatabaseDirectiveOptionDto)
                    'dto.Options = New List(Of EzProxyDatabaseDirectiveOption)
                End If

                'Dim ids As List(Of Integer) = domain.Options.Select(Function(d) d.EzProxyDatabaseDirective.EzProxyDirectiveId).ToList()
                'Using dbc As New DataAccess.OclsConnectionManagerDataContext

                '    Dim lstProxies As New List(Of Dto.EzProxyDatabaseDirectiveOptionDto)

                '    'If ids.Count > 0 Then
                '    For Each opts As Domain.EzProxyOption In dbc.EzProxyOptions.Where(Function(d) d.EzProxyDirectiveId = domain.EzProxyDirectiveId)

                '        Dim ezProxyDatabaseDirectiveOptionDomain As Domain.EzProxyDatabaseDirectiveOption = Nothing

                '        If (ids.Contains(opts.EzProxyDirectiveId)) Then

                '            Dim ezProxyDatabaseDirective As Domain.EzProxyDatabaseDirective = Nothing

                '            If domain.EzProxyDatabaseId > 0 Then
                '                ezProxyDatabaseDirective = dbc.EzProxyDatabaseDirectives.Where(Function(f) f.EzProxyDirectiveId = opts.EzProxyDirectiveId And f.EzProxyDatabaseId = domain.EzProxyDatabaseId).FirstOrDefault()
                '            End If

                '            If Not ezProxyDatabaseDirective Is Nothing Then

                '                ezProxyDatabaseDirectiveOptionDomain = ezProxyDatabaseDirective.Options.Where(Function(o) o.EzProxyDatabaseDirectiveId = ezProxyDatabaseDirective.EzProxyDatabaseDirectiveId And o.EzProxyOptionId = opts.EzProxyOptionId).FirstOrDefault()
                '                'ezProxyDatabaseDirectiveOptionDomain.EzProxyDatabaseDirective = New Domain.EzProxyDatabaseDirective
                '                'ezProxyDatabaseDirectiveOptionDomain.EzProxyOption = opts
                '                'ezProxyDatabaseDirectiveOptionDomain.IsActive = True
                '                'ezProxyDatabaseDirectiveOptionDomain.OptionValue = String.Empty
                '            End If

                '        Else
                '            ezProxyDatabaseDirectiveOptionDomain = New Domain.EzProxyDatabaseDirectiveOption
                '            ezProxyDatabaseDirectiveOptionDomain.EzProxyDatabaseDirective = New Domain.EzProxyDatabaseDirective
                '            ezProxyDatabaseDirectiveOptionDomain.EzProxyOption = opts
                '            ezProxyDatabaseDirectiveOptionDomain.IsActive = True
                '            ezProxyDatabaseDirectiveOptionDomain.OptionValue = String.Empty

                '        End If
                '        lstProxies.Add(OclsConnectionManager.Dto.EzProxyDatabaseDirectiveOptionDto.GetDto(ezProxyDatabaseDirectiveOptionDomain))
                '    Next
                '    'End If

                '    dto.Options = lstProxies
                ''dto.ProxyOptions = dbc.EzProxyOptions.Where(Function(d) ids.Contains(d.EzProxyDirectiveId) = True).Select(Function(o) EzProxyOptionDto.GetDto(o)).ToArray().ToList()

                'End Using

            End If

            Return dto

        End Function

    End Class

End Namespace