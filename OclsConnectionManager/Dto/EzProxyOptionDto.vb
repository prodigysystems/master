Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Dto

    <Serializable()>
    Public Class EzProxyOptionDto

        Public Property EzProxyOptionId() As Integer
        Public Property EzProxyDirectiveId() As Integer
        Public Property OptionOrQualifier() As String
        Public Property Name() As String
        Public Property Description() As String
        Public Property OutputAs() As String
        Public Property OutputOrder() As Nullable(Of Integer)
        Public Property IsRequired() As Boolean
        Public Property HasInputValue() As Boolean

        Public Shared Function GetDto(domain As Domain.EzProxyOption) As EzProxyOptionDto
            Return New EzProxyOptionDto() With {
                .EzProxyOptionId = domain.EzProxyOptionId,
                .EzProxyDirectiveId = domain.EzProxyDirectiveId,
                .Name = domain.Name,
                .Description = domain.Description,
                .OutputAs = domain.OutputAs,
                .OutputOrder = domain.OutputOrder,
                .IsRequired = domain.IsRequired,
                .HasInputValue = domain.HasInputValue,
                .OptionOrQualifier = domain.OptionOrQualifier
            }
        End Function

    End Class

End Namespace
