Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Domain

    <Serializable()>
    Public Class CollegeIpAddress

        <Key()>
        Public Property CollegeIpAddressId As Integer
        Public Property CollegeId() As Integer
        Public Overridable Property College As College
        Public Property Campus As String
        Public Property IpAddress As String
        Public Property SubnetMask As String
        Public Property RegularExpression As String
        Public Property CreatedByUserId() As Integer
        Public Overridable Property CreatedByUser() As Domain.UserProfile
        Public Property CreatedDate() As DateTime
        Public Property ModifiedByUserId() As Nullable(Of Integer)
        Public Overridable Property ModifiedByUser() As Domain.UserProfile
        Public Property ModifiedDate As Nullable(Of DateTime)
        Public Property IsActive As Boolean

    End Class

End Namespace
