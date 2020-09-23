Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Dto

    <Serializable()>
    Public Class CollegeIpAddressDto

        Public Property CollegeIpAddressId() As Integer
        Public Property CollegeId() As Integer
        <UIHint("CollegeSelector")>
        Public Property CollegeName() As String
        Public Property Campus() As String
        Public Property IpAddress() As String
        Public Property SubnetMask() As String
        Public Property RegularExpression() As String
        Public Property ModifiedBy() As String
        Public Property ModifiedDate() As DateTime
        Public Property IsActive() As Boolean

        Public Shared Function GetDto(domain As Domain.CollegeIpAddress) As CollegeIpAddressDto
            Dim dto As New CollegeIpAddressDto() With {
                .CollegeIpAddressId = domain.CollegeIpAddressId,
                .CollegeId = domain.CollegeId,
                .CollegeName = domain.College.Name,
                .Campus = domain.Campus,
                .IpAddress = domain.IpAddress,
                .SubnetMask = domain.SubnetMask,
                .RegularExpression = domain.RegularExpression,
                .IsActive = domain.IsActive
            }

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
