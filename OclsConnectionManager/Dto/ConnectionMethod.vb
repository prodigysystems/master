Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Dto

    <Serializable()>
    Public Class ConnectionMethodDto

        Public Property ConnectionMethodId() As Integer
        Public Property Code() As Integer?
        Public Property Name() As String

        Public Shared Function GetDto(domain As Domain.ConnectionMethod) As ConnectionMethodDto
            Return New ConnectionMethodDto() With {
                .ConnectionMethodId = domain.ConnectionMethodId,
                .Code = domain.Code,
                .Name = domain.Name
            }
        End Function

    End Class

End Namespace
