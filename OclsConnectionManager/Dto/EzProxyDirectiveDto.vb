Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Dto

    <Serializable()>
    Public Class EzProxyDirectiveDto

        Public Property EzProxyDirectiveId() As Integer
        Public Property Name() As String
        Public Property Description() As String
        Public Property OutputAs() As String
        Public Property OutputOrder() As Nullable(Of Integer)
        Public Property IsDefault() As Boolean
        Public Overridable Property Options As IEnumerable(Of EzProxyOptionDto)

        Public Shared Function GetDto(domain As Domain.EzProxyDirective, includeDetails As Boolean) As EzProxyDirectiveDto
            Dim dto As New EzProxyDirectiveDto() With {
                .EzProxyDirectiveId = domain.EzProxyDirectiveId,
                .Name = domain.Name,
                .Description = domain.Description,
                .OutputAs = domain.OutputAs,
                .OutputOrder = domain.OutputOrder,
                .IsDefault = domain.IsDefault
            }

            If includeDetails Then
                dto.Options = domain.Options.Select(Function(o) EzProxyOptionDto.GetDto(o)).ToArray().ToList()
            End If

            Return dto

        End Function

    End Class

End Namespace
