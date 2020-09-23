Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Dto

    <Serializable()>
    Public Class CollegeDto

        Public Property CollegeId() As Integer
        Public Property Code() As Integer?
        Public Property Name() As String

        Public Shared Function GetDto(domain As Domain.College) As CollegeDto
            Return New CollegeDto() With {
                .CollegeId = domain.CollegeId,
                .Code = domain.Code,
                .Name = domain.Name
            }
        End Function

    End Class

End Namespace
