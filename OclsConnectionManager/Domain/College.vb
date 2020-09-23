Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Domain

    <Serializable()>
    Public Class College

        <Key()>
        Public Property CollegeId() As Integer
        Public Property Code() As Integer
        Public Property Name() As String

    End Class

End Namespace
