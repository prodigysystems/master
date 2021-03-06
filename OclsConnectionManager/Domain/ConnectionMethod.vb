﻿Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Namespace Domain

    <Serializable()>
    Public Class ConnectionMethod

        <Key()>
        Public Property ConnectionMethodId() As Integer
        Public Property Code() As Integer
        Public Property Name() As String

    End Class

End Namespace
