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
    Public Class IpAddressService
        Public Shared Function GetIpAddressesByCollegeId(collegeId As Integer) As IEnumerable(Of CollegeIpAddressDto)
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim domIpAddress As IEnumerable(Of Domain.CollegeIpAddress)
                If collegeId = 0 Then
                    domIpAddress = dbc.CollegeIpAddresses
                Else
                    domIpAddress = dbc.CollegeIpAddresses.Where(Function(r) r.CollegeId = collegeId)
                End If
                Return domIpAddress.OrderBy(Function(r) r.College.Name).ThenBy(Function(r) r.IpAddress).Select(Function(r) CollegeIpAddressDto.GetDto(r)).ToArray().ToList()
            End Using
        End Function

        Public Shared Function AddIpAddresse(dtoIpAddress As CollegeIpAddressDto, username As String) As Boolean
            Try
                Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                    Dim domIpAddress As New Domain.CollegeIpAddress()
                    domIpAddress.CollegeId = dtoIpAddress.CollegeId
                    domIpAddress.Campus = dtoIpAddress.Campus
                    domIpAddress.IpAddress = dtoIpAddress.IpAddress
                    domIpAddress.SubnetMask = dtoIpAddress.SubnetMask
                    domIpAddress.RegularExpression = dtoIpAddress.RegularExpression
                    domIpAddress.IsActive = dtoIpAddress.IsActive
                    domIpAddress.CreatedByUser = dbc.Users.Where(Function(u) u.UserName.Equals(username)).Single()
                    domIpAddress.CreatedDate = DateTime.Now
                    dbc.CollegeIpAddresses.Add(domIpAddress)
                    dbc.SaveChanges()
                    domIpAddress = dbc.CollegeIpAddresses.Include("College").Where(Function(m) m.CollegeIpAddressId = domIpAddress.CollegeIpAddressId).FirstOrDefault()
                    dbc.AddAuditLog(domIpAddress.CreatedByUser, AuditLog.EventCodes.CreateCollegeIpAddress, domIpAddress.CollegeIpAddressId, String.Format("Ip Address Created. College:{0}, Campus:{1}, Ip Address:{2}, Subnet Mask:{3}, Regular Expression:{4}", domIpAddress.College.Name, domIpAddress.Campus, domIpAddress.IpAddress, domIpAddress.SubnetMask, domIpAddress.RegularExpression))
                    dbc.SaveChanges()
                End Using
            Catch ex As Exception
                Return False
            End Try

            Return True
        End Function
        Public Shared Function UpdateIpAddress(dtoIpAddress As CollegeIpAddressDto, username As String) As Boolean

            Dim domIpAddress As New Domain.CollegeIpAddress()
            Try
                Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                    domIpAddress = dbc.CollegeIpAddresses.Where(Function(m) m.CollegeIpAddressId = dtoIpAddress.CollegeIpAddressId).FirstOrDefault()
                    domIpAddress.Campus = dtoIpAddress.Campus
                    dbc.Entry(domIpAddress).Property(Function(m) m.Campus).IsModified = True
                    domIpAddress.IpAddress = dtoIpAddress.IpAddress
                    dbc.Entry(domIpAddress).Property(Function(m) m.IpAddress).IsModified = True
                    domIpAddress.SubnetMask = dtoIpAddress.SubnetMask
                    dbc.Entry(domIpAddress).Property(Function(m) m.SubnetMask).IsModified = True
                    domIpAddress.RegularExpression = dtoIpAddress.RegularExpression
                    dbc.Entry(domIpAddress).Property(Function(m) m.RegularExpression).IsModified = True
                    domIpAddress.IsActive = dtoIpAddress.IsActive
                    dbc.Entry(domIpAddress).Property(Function(m) m.IsActive).IsModified = True
                    domIpAddress.ModifiedByUserId = dbc.Users.Where(Function(u) u.UserName.Equals(username)).Single().UserId
                    dbc.Entry(domIpAddress).Property(Function(m) m.ModifiedByUserId).IsModified = True
                    domIpAddress.ModifiedDate = DateTime.Now
                    dbc.Entry(domIpAddress).Property(Function(m) m.ModifiedDate).IsModified = True
                    dbc.SaveChanges()
                    dbc.AddAuditLog(domIpAddress.ModifiedByUser, AuditLog.EventCodes.UpdateCollegeIpAddress, domIpAddress.CollegeIpAddressId, String.Format("Ip Address Updated. College:{0}, Campus:{1}, Ip Address:{2}, Subnet Mask:{3}, Regular Expression:{4}", domIpAddress.College.Name, domIpAddress.Campus, domIpAddress.IpAddress, domIpAddress.SubnetMask, domIpAddress.RegularExpression))
                    dbc.SaveChanges()
                End Using
            Catch ex As Exception
                Return False
            End Try

            Return True
        End Function

        Public Shared Function DeleteIpAddresse(dtoIpAddress As CollegeIpAddressDto, username As String) As Boolean
            Dim domIpAddress As New Domain.CollegeIpAddress()
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                domIpAddress = dbc.CollegeIpAddresses.Where(Function(m) m.CollegeIpAddressId = dtoIpAddress.CollegeIpAddressId).FirstOrDefault()
                If Not IsNothing(domIpAddress) Then
                    Dim deletingUser As UserProfile = dbc.Users.Where(Function(u) u.UserName.Equals(username)).Single()
                    dbc.AddAuditLog(deletingUser, AuditLog.EventCodes.DeleteCollegeIpAddress, domIpAddress.CollegeIpAddressId, String.Format("Ip Address Deleted.  College:{0}, Campus:{1}, Ip Address:{2}, Subnet Mask:{3}, Regular Expression:{4}", domIpAddress.College.Name, domIpAddress.Campus, domIpAddress.IpAddress, domIpAddress.SubnetMask, domIpAddress.RegularExpression))
                    dbc.SaveChanges()
                    dbc.CollegeIpAddresses.Remove(domIpAddress)
                    dbc.SaveChanges()

                End If
            End Using
            Return True
        End Function
    End Class
End Namespace
