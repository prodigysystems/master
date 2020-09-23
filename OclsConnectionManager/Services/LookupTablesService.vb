Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports OclsConnectionManager.DataAccess
Imports OclsConnectionManager.Domain
Imports OclsConnectionManager.Dto
Imports System.Data.Entity
Imports System.Reflection
Imports System.IO
Namespace Services
    Public Class LookupTablesService
        Public Shared Function GetColleges() As IEnumerable(Of CollegeDto)
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim dom As IEnumerable(Of Domain.College) = dbc.Colleges.OrderBy(Function(m) m.Name)
                Return dom.Select(Function(r) CollegeDto.GetDto(r)).ToArray().ToList()
            End Using
        End Function

        Public Shared Function AddCollege(dto As CollegeDto) As Boolean
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim dom As New Domain.College()
                dom.Name = dto.Name
                dom.Code = dto.Code.Value
                dbc.Colleges.Add(dom)
                dbc.SaveChanges()
            End Using
            Return True
        End Function
      
        Public Shared Function UpdateCollege(dto As CollegeDto) As Boolean

            Dim dom As New Domain.College()
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                dom = dbc.Colleges.Where(Function(m) m.CollegeId = dto.CollegeId).FirstOrDefault()
                dom.Code = dto.Code.Value
                dbc.Entry(dom).Property(Function(m) m.Code).IsModified = True
                dom.Name = dto.Name
                dbc.Entry(dom).Property(Function(m) m.Name).IsModified = True
                dbc.SaveChanges()
            End Using
            Return True
        End Function
        Public Shared Function DeleteCollege(dto As CollegeDto) As Boolean
            Dim dom As New Domain.College()
            Try
                Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                    dom = dbc.Colleges.Where(Function(m) m.CollegeId = dto.CollegeId).FirstOrDefault()
                    If Not IsNothing(dom) Then
                        dbc.Colleges.Remove(dom)
                        dbc.SaveChanges()
                    End If
                End Using
            Catch ex As Exception
                Return False
            End Try
           
            Return True
        End Function
        Public Shared Function DeleteCollegebyName(name As String) As Boolean
            Dim dom As New Domain.College()
            Try
                Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                    dom = dbc.Colleges.Where(Function(m) m.Name = name).FirstOrDefault()
                    If Not IsNothing(dom) Then
                        dbc.Colleges.Remove(dom)
                        dbc.SaveChanges()
                    End If
                End Using
            Catch ex As Exception
                Return False
            End Try

            Return True
        End Function
        Public Shared Function CollegeExist(id As Integer, name As String) As Boolean

            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim dom As College
                If id = 0 Then
                    dom = dbc.Colleges.Where(Function(r) r.Name = name).FirstOrDefault()
                Else
                    dom = dbc.Colleges.Where(Function(r) r.Name = name And r.CollegeId <> id).FirstOrDefault()
                End If

                If IsNothing(dom) Then
                    Return False
                Else
                    Return True
                End If
            End Using

        End Function
        Public Shared Function CollegeCodeExist(id As Integer, code As Integer) As Boolean

            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim dom As College
                If id = 0 Then
                    dom = dbc.Colleges.Where(Function(r) r.Code = code).FirstOrDefault()
                Else
                    dom = dbc.Colleges.Where(Function(r) r.Code = code And r.CollegeId <> id).FirstOrDefault()
                End If
                If IsNothing(dom) Then
                    Return False
                Else
                    Return True
                End If
            End Using

        End Function

        Public Shared Function GetConnectionMethods() As IEnumerable(Of ConnectionMethodDto)
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim dom As IEnumerable(Of Domain.ConnectionMethod) = dbc.ConnectionMethods.OrderBy(Function(m) m.Name)
                Return dom.Select(Function(r) ConnectionMethodDto.GetDto(r)).ToArray().ToList()
            End Using
        End Function

        Public Shared Function AddConnectionMethod(dto As ConnectionMethodDto) As Boolean
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim dom As New Domain.ConnectionMethod()
                dom.Name = dto.Name
                dom.Code = dto.Code.Value
                dbc.ConnectionMethods.Add(dom)
                dbc.SaveChanges()
            End Using
            Return True
        End Function
        Public Shared Function UpdateConnectionMethod(dto As ConnectionMethodDto) As Boolean

            Dim dom As New Domain.ConnectionMethod()
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                dom = dbc.ConnectionMethods.Where(Function(m) m.ConnectionMethodId = dto.ConnectionMethodId).FirstOrDefault()
                dom.Code = dto.Code.Value
                dbc.Entry(dom).Property(Function(m) m.Code).IsModified = True
                dom.Name = dto.Name
                dbc.Entry(dom).Property(Function(m) m.Name).IsModified = True
                dbc.SaveChanges()
            End Using
            Return True
        End Function
        Public Shared Function DeleteConnectionMethod(dto As ConnectionMethodDto) As Boolean
            Dim dom As New Domain.ConnectionMethod()
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                dom = dbc.ConnectionMethods.Where(Function(m) m.ConnectionMethodId = dto.ConnectionMethodId).FirstOrDefault()
                If Not IsNothing(dom) Then
                    dbc.ConnectionMethods.Remove(dom)
                    dbc.SaveChanges()
                End If
            End Using
            Return True
        End Function
        Public Shared Function ConnectionMethodExist(id As Integer, name As String) As Boolean
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim dom As ConnectionMethod
                If id = 0 Then
                    dom = dbc.ConnectionMethods.Where(Function(r) r.Name = name).FirstOrDefault()
                Else
                    dom = dbc.ConnectionMethods.Where(Function(r) r.Name = name And r.ConnectionMethodId <> id).FirstOrDefault()
                End If
                If IsNothing(dom) Then
                    Return False
                Else
                    Return True
                End If
            End Using
        End Function
        Public Shared Function ConnectionMethodCodeExist(id As Integer, code As Integer) As Boolean
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim dom As ConnectionMethod
                If id = 0 Then
                    dom = dbc.ConnectionMethods.Where(Function(r) r.Code = code).FirstOrDefault()
                Else
                    dom = dbc.ConnectionMethods.Where(Function(r) r.Code = code And r.ConnectionMethodId <> id).FirstOrDefault()
                End If
                If IsNothing(dom) Then
                    Return False
                Else
                    Return True
                End If
            End Using
        End Function
        Public Shared Function DeleteConnectionMethodbyName(name As String) As Boolean
            Dim dom As New Domain.ConnectionMethod()
            Try
                Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                    dom = dbc.ConnectionMethods.Where(Function(m) m.Name = name).FirstOrDefault()
                    If Not IsNothing(dom) Then
                        dbc.ConnectionMethods.Remove(dom)
                        dbc.SaveChanges()
                    End If
                End Using
            Catch ex As Exception
                Return False
            End Try

            Return True
        End Function
    End Class
End Namespace
