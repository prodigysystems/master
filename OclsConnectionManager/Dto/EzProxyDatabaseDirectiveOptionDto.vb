Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Dto

    <Serializable()>
    Public Class EzProxyDatabaseDirectiveOptionDto

        Public Property EzProxyDatabaseDirectiveOptionId() As Integer
        Public Property EzProxyDatabaseDirectiveId() As Integer
        Public Property Name() As String
        Public Property OptionType() As String
        Public Property EzProxyOptionId() As Integer

        '<MaxLength(1000, ErrorMessage:="OptionValue field cannot exceed 1000 characters.")>
        Public Property OptionValue() As String
        Public Property IsActive() As Boolean

        Public Property EzProxyDirectiveId() As Integer
        Public Property OutputAs() As String ''''OutputAs value from EzProxyOption 
        Public Property IsRequired() As Boolean ''''IsRequired value from EzProxyOption 
        Public Property HasInputValue() As Boolean ''''HasInputValue value from EzProxyOption 

        Public Shared Function GetDto(domain As Domain.EzProxyDatabaseDirectiveOption) As EzProxyDatabaseDirectiveOptionDto
            Dim ezProxyDatabaseDirectiveOptionDto As New EzProxyDatabaseDirectiveOptionDto

            If Not domain Is Nothing Then

                ezProxyDatabaseDirectiveOptionDto.EzProxyDatabaseDirectiveOptionId = domain.EzProxyDatabaseDirectiveOptionId
                ezProxyDatabaseDirectiveOptionDto.EzProxyDatabaseDirectiveId = domain.EzProxyDatabaseDirectiveId
                ezProxyDatabaseDirectiveOptionDto.IsActive = domain.IsActive
                ezProxyDatabaseDirectiveOptionDto.OptionValue = domain.OptionValue

                If Not IsNothing(domain.EzProxyDatabaseDirective) Then
                    ezProxyDatabaseDirectiveOptionDto.EzProxyDirectiveId = domain.EzProxyDatabaseDirective.EzProxyDirectiveId
                End If

                If Not IsNothing(domain.EzProxyOption) Then
                    ezProxyDatabaseDirectiveOptionDto.EzProxyOptionId = domain.EzProxyOption.EzProxyOptionId
                    ezProxyDatabaseDirectiveOptionDto.Name = domain.EzProxyOption.Name
                    ezProxyDatabaseDirectiveOptionDto.OutputAs = domain.EzProxyOption.OutputAs
                    ezProxyDatabaseDirectiveOptionDto.OptionType = domain.EzProxyOption.OptionOrQualifier
                    ezProxyDatabaseDirectiveOptionDto.IsRequired = domain.EzProxyOption.IsRequired
                    ezProxyDatabaseDirectiveOptionDto.HasInputValue = domain.EzProxyOption.HasInputValue
                Else
                    ezProxyDatabaseDirectiveOptionDto.EzProxyOptionId = domain.EzProxyOptionId
                End If

                Return ezProxyDatabaseDirectiveOptionDto
            Else
                Return ezProxyDatabaseDirectiveOptionDto
            End If

        End Function

    End Class

End Namespace