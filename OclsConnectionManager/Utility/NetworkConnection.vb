﻿Option Infer On

Imports System
Imports System.Net
Imports System.Runtime.InteropServices

<StructLayout(LayoutKind.Sequential)>
Public Class NetResource
    Public Scope As ResourceScope
    Public ResourceType As ResourceType
    Public DisplayType As ResourceDisplaytype
    Public Usage As Integer
    Public LocalName As String
    Public RemoteName As String
    Public Comment As String
    Public Provider As String
End Class

Public Enum ResourceScope As Integer
    Connected = 1
    GlobalNetwork
    Remembered
    Recent
    Context
End Enum

Public Enum ResourceType As Integer
    Any = 0
    Disk = 1
    Print = 2
    Reserved = 8
End Enum

Public Enum ResourceDisplaytype As Integer
    Generic = &H0
    Domain = &H1
    Server = &H2
    Share = &H3
    File = &H4
    Group = &H5
    Network = &H6
    Root = &H7
    Shareadmin = &H8
    Directory = &H9
    Tree = &HA
    Ndscontainer = &HB
End Enum

Public Class NetworkConnection
    Implements IDisposable

    Private _NetworkName As String
    Protected _Disposed As Boolean = False

    <DllImport("mpr.dll")>
    Private Shared Function WNetAddConnection2(netResource As NetResource, password As String, username As String, flags As Integer) As Integer
    End Function

    <DllImport("mpr.dll")>
    Private Shared Function WNetCancelConnection2(name As String, flags As Integer, force As Boolean) As Integer
    End Function

    Public Sub New(networkName As String, credentials As NetworkCredential)

        _NetworkName = networkName

        Dim netResource As New NetResource()
        With netResource
            .Scope = ResourceScope.GlobalNetwork
            .ResourceType = ResourceType.Disk
            .DisplayType = ResourceDisplaytype.Share
            .RemoteName = networkName
        End With

        Dim userName As String
        If String.IsNullOrEmpty(credentials.Domain) Then
            userName = credentials.UserName
        Else
            userName = String.Format("{0}\{1}", credentials.Domain, credentials.UserName)
        End If

        Dim result As Integer = WNetAddConnection2(netResource, credentials.Password, userName, 0)
        If result <> 0 Then
            Throw New ApplicationException(String.Format("Unable to connect to remote resource: Share: {0}, Username: {1}, Result:{2:d}", networkName, credentials.UserName, result))
        End If

    End Sub

    Protected Overrides Sub Finalize()
        Dispose(False)
        MyBase.Finalize()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not _Disposed Then
            If disposing Then
                ' Free managed resources. 
            End If
            ' Free unmanaged resources. 
            WNetCancelConnection2(_NetworkName, 0, True)
        End If
        _Disposed = True
    End Sub

End Class
