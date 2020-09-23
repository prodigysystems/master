Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic

Namespace Dto

    <Serializable()>
    Public Class EzProxyDatabaseDirectiveDto

        Public Property EzProxyDatabaseDirectiveId() As Integer
        Public Property EzProxyDatabaseId() As Integer
        Public Property EzProxyDirectiveId() As Integer
        Public Property Name() As String
        Public Property OutputAs() As String
        Public Property OutputOrder() As Integer
        Public Property Comment() As String
        Public Property IsActive() As Boolean
        Public Overridable Property Options As IEnumerable(Of EzProxyDatabaseDirectiveOptionDto)
        'Public Overridable Property ProxyOptions As IEnumerable(Of EzProxyOptionDto)

        Public Shared Function GetDto(domain As Domain.EzProxyDatabaseDirective) As EzProxyDatabaseDirectiveDto
            'Return New EzProxyDatabaseDirectiveDto() With {
            '    .EzProxyDatabaseDirectiveId = domain.EzProxyDatabaseDirectiveId,
            '    .EzProxyDatabaseId = domain.EzProxyDatabaseId,
            '    .EzProxyDirectiveId = domain.EzProxyDirectiveId,
            '     .Name = domain.EzProxyDirective.Name,
            '    .OutputAs = domain.OutputAs,
            '    .Comment = domain.Comment,
            '    .IsActive = domain.IsActive,
            '    .Options = domain.Options.Select(Function(o) EzProxyDatabaseDirectiveOptionDto.GetDto(o)).ToArray().ToList()
            '}
            Dim dto As New EzProxyDatabaseDirectiveDto()

            If Not domain Is Nothing Then
                dto.EzProxyDatabaseDirectiveId = domain.EzProxyDatabaseDirectiveId
                dto.EzProxyDatabaseId = domain.EzProxyDatabaseId
                dto.EzProxyDirectiveId = domain.EzProxyDirectiveId
                If Not IsNothing(domain.EzProxyDirective) Then
                    dto.Name = domain.EzProxyDirective.Name
                End If
                dto.OutputAs = domain.OutputAs
                dto.OutputOrder = domain.OutputOrder
                dto.Comment = domain.Comment
                dto.IsActive = domain.IsActive

                If Not domain.Options Is Nothing Then
                    dto.Options = domain.Options.Select(Function(o) EzProxyDatabaseDirectiveOptionDto.GetDto(o)).ToArray().ToList()


                    'Dim ids As List(Of Integer) = domain.Options.Select(Function(d) d.EzProxyDatabaseDirective.EzProxyDirectiveId).ToList()
                    'Using dbc As New DataAccess.OclsConnectionManagerDataContext

                    '    Dim lstProxies As New List(Of Dto.EzProxyDatabaseDirectiveOptionDto)

                    '    'If ids.Count > 0 Then
                    '    For Each opts As Domain.EzProxyOption In dbc.EzProxyOptions.Where(Function(d) d.EzProxyDirectiveId = domain.EzProxyDirectiveId)

                    '        Dim ezProxyDatabaseDirectiveOptionDomain As Domain.EzProxyDatabaseDirectiveOption = Nothing

                    '        If (ids.Contains(opts.EzProxyDirectiveId)) Then

                    '            Dim ezProxyDatabaseDirective As Domain.EzProxyDatabaseDirective = dbc.EzProxyDatabaseDirectives.Where(Function(f) f.EzProxyDirectiveId = opts.EzProxyDirectiveId And f.EzProxyDatabaseId = domain.EzProxyDatabaseId).FirstOrDefault()
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
                    '    ''dto.ProxyOptions = dbc.EzProxyOptions.Where(Function(d) ids.Contains(d.EzProxyDirectiveId) = True).Select(Function(o) EzProxyOptionDto.GetDto(o)).ToArray().ToList()

                    'End Using


                End If


            End If

            Return dto

        End Function

    End Class

End Namespace