Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Domain

    <Serializable()>
    Public Class UserProfile

        <Key()>
        Public Property UserId() As Integer
        Public Property UserName() As String

    End Class

End Namespace
