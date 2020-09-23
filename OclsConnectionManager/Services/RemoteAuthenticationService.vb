Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports OclsConnectionManager.DataAccess
Imports OclsConnectionManager.Domain
Imports OclsConnectionManager.Dto

Namespace Services

    Public Class RemoteAuthenticationService
        Private Sub New()
        End Sub

        Public Shared Function GetRemoteAuthenticationsList(databaseName As String, url As String, useFullUrl As Boolean, domainName As String, collegeId As String) As IEnumerable(Of RemoteAuthenticationDto)

            Dim colId As Integer = Integer.Parse(collegeId)

            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim ras As IEnumerable(Of Domain.RemoteAuthentication) = dbc.RemoteAuthentications.Where(
                    Function(r) (String.IsNullOrEmpty(databaseName) Or r.EzProxyDatabase.Name.Contains(databaseName)) And
                                (colId = 0 Or r.CollegeId = colId) And
                                (String.IsNullOrEmpty(url) Or (Not (r.EzProxyDatabase.Directives.Where(Function(d) (d.EzProxyDirective.Name.ToLower() = "url") And d.OutputAs.ToLower().Contains(url.ToLower())).FirstOrDefault()) Is Nothing) Or (useFullUrl AndAlso r.FullUrl.ToLower().Contains(url.ToLower()))) And
                                (String.IsNullOrEmpty(domainName) Or Not (r.EzProxyDatabase.Directives.Where(Function(d) (d.EzProxyDirective.Name.ToLower() = "domain") And d.OutputAs.ToLower().Contains(domainName.ToLower())).FirstOrDefault()) Is Nothing)
                               )

                Return ras.OrderBy(Function(r) r.College.Name).ThenBy(Function(r) r.EzProxyDatabase.Name).Select(Function(r) RemoteAuthenticationDto.GetDto(r)).ToArray().ToList()
            End Using

        End Function

        Public Shared Function GetDatabaseDirective(dbId As Integer) As String
            Dim rs As String = ""
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim div As Domain.EzProxyDatabaseDirective = dbc.EzProxyDatabaseDirectives.Where(Function(r) r.EzProxyDirective.Name.ToLower() = "url" And r.EzProxyDatabaseId = dbId).FirstOrDefault()
                If Not IsNothing(div) Then
                    Dim directiveOption As Domain.EzProxyDatabaseDirectiveOption = div.Options.Where(Function(o) o.EzProxyOption.Name = "url").FirstOrDefault()
                    rs += directiveOption.OptionValue + ","
                End If
                div = dbc.EzProxyDatabaseDirectives.Where(Function(r) r.EzProxyDirective.Name.ToLower() = "domain" And r.EzProxyDatabaseId = dbId).FirstOrDefault()
                If Not IsNothing(div) Then
                    Dim directiveOption As Domain.EzProxyDatabaseDirectiveOption = div.Options.Where(Function(o) o.EzProxyOption.Name = "wilddomain").FirstOrDefault()
                    rs += directiveOption.OptionValue
                End If
            End Using
            Return rs

        End Function

        Public Shared Function GetRemoteAuthentication(remoteAuthenticationId As Integer) As RemoteAuthenticationDto

            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim ra As RemoteAuthentication = dbc.RemoteAuthentications.Where(Function(r) r.RemoteAuthenticationId = remoteAuthenticationId).SingleOrDefault()
                Return RemoteAuthenticationDto.GetDto(ra)
            End Using

        End Function

        Public Shared Function GetAvailableColleges() As IEnumerable(Of CollegeDto)
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim domColleges As IEnumerable(Of Domain.College) = dbc.Colleges
                Return domColleges.Select(Function(r) CollegeDto.GetDto(r)).ToArray().ToList()
            End Using

        End Function

        Public Shared Function GetAvailableConnectionMethods() As IEnumerable(Of ConnectionMethodDto)
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim domConnectionmethods As IEnumerable(Of Domain.ConnectionMethod) = dbc.ConnectionMethods
                Return domConnectionmethods.Select(Function(r) ConnectionMethodDto.GetDto(r)).ToArray().ToList()
            End Using
        End Function

        Public Shared Function AddRemoteAuthentication(dtoRA As RemoteAuthenticationDto, username As String) As Boolean
            Dim ret As Boolean = RemoteAuthenticationExist(dtoRA.RemoteAuthenticationId, dtoRA.EzProxyDatabaseId, dtoRA.CollegeId, dtoRA.ConnectionMethodId)
            If ret = True Then
                Return False
            End If
            Using trans As New System.Transactions.TransactionScope()
                Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                    ret = AddRemoteAuthenticationCore(dtoRA, username)

                End Using
                trans.Complete()
            End Using

            Return True
        End Function

        Private Shared Function AddRemoteAuthenticationCore(dtoRA As RemoteAuthenticationDto, username As String) As Boolean

            Dim domRA As New Domain.RemoteAuthentication()
            Dim domDB As Domain.EzProxyDatabase

            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                domDB = dbc.EzProxyDatabases.Where(Function(d) d.EzProxyDatabaseId = dtoRA.EzProxyDatabaseId).Single()
                domRA.EzProxyDatabaseId = dtoRA.EzProxyDatabaseId
                domRA.CollegeId = (dtoRA.CollegeId)
                domRA.ConnectionMethodId = (dtoRA.ConnectionMethodId)
                domRA.CreatedByUser = dbc.Users.Where(Function(u) u.UserName.Equals(username)).Single()
                domRA.CreatedDate = DateTime.Now
                domRA.IsActive = dtoRA.IsActive
                domRA.CampusRestriction = dtoRA.CampusRestriction
                domRA.FullUrl = dtoRA.FullUrl

                If domDB.Url Is Nothing OrElse Not domDB.Url.Equals(dtoRA.Url) Then
                    domRA.Url = dtoRA.Url
                End If

                If domDB.DomainName Is Nothing OrElse Not domDB.DomainName.Equals(dtoRA.DomainName) Then
                    domRA.DomainName = dtoRA.DomainName
                End If

                domRA = CType(dbc.Set(GetType(OclsConnectionManager.Domain.RemoteAuthentication)).Add(domRA), OclsConnectionManager.Domain.RemoteAuthentication)
                dbc.SaveChanges()
                domRA = dbc.RemoteAuthentications.Include("College").Include("ConnectionMethod").Where(Function(d) d.RemoteAuthenticationId = domRA.RemoteAuthenticationId).Single()
                dbc.AddAuditLog(domRA.CreatedByUser, AuditLog.EventCodes.CreateRemoteAuthentication, domRA.RemoteAuthenticationId, String.Format("Remote Authentication Created.  College:{4}, Campus Restriction:{5}, Database Name:{0}, URL:{1}, Domain Name:{2}, Method:{3}", domRA.EzProxyDatabase.Name, domRA.EzProxyDatabase.Url, domRA.EzProxyDatabase.DomainName, domRA.ConnectionMethod.Name, domRA.College.Name, domRA.CampusRestriction.Trim()))
                dbc.SaveChanges()

            End Using

            Return True

        End Function

        Public Shared Function UpdateRemoteAuthentication(dtoRA As RemoteAuthenticationDto, username As String) As Boolean
            Dim ret As Boolean = RemoteAuthenticationExist(dtoRA.RemoteAuthenticationId, dtoRA.EzProxyDatabaseId, dtoRA.CollegeId, dtoRA.ConnectionMethodId)
            If ret = True Then
                Return False
            End If
            Dim domRA As New Domain.RemoteAuthentication()
            Using trans As New System.Transactions.TransactionScope()

                Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                    domRA = dbc.RemoteAuthentications.Where(Function(m) m.RemoteAuthenticationId = dtoRA.RemoteAuthenticationId).FirstOrDefault()
                    domRA.ConnectionMethodId = dtoRA.ConnectionMethodId
                    dbc.Entry(domRA).Property(Function(m) m.ConnectionMethodId).IsModified = True
                    domRA.EzProxyDatabaseId = dtoRA.EzProxyDatabaseId
                    dbc.Entry(domRA).Property(Function(m) m.EzProxyDatabaseId).IsModified = True
                    domRA.CollegeId = dtoRA.CollegeId
                    dbc.Entry(domRA).Property(Function(m) m.CollegeId).IsModified = True
                    domRA.ModifiedByUserId = dbc.Users.Where(Function(u) u.UserName.Equals(username)).Single().UserId
                    dbc.Entry(domRA).Property(Function(m) m.ModifiedByUserId).IsModified = True
                    domRA.ModifiedDate = DateTime.Now
                    dbc.Entry(domRA).Property(Function(m) m.ModifiedDate).IsModified = True
                    domRA.IsActive = dtoRA.IsActive
                    dbc.Entry(domRA).Property(Function(m) m.IsActive).IsModified = True

                    domRA.FullUrl = dtoRA.FullUrl
                    dbc.Entry(domRA).Property(Function(m) m.FullUrl).IsModified = True

                    'If Not domRA.EzProxyDatabase.Url.Equals(dtoRA.Url) Then 'original code was throwing nullreference exception when url field in ezproxydatabase was null
                    If domRA.EzProxyDatabase.Url Is Nothing Or
                        (domRA.EzProxyDatabase.Url IsNot Nothing AndAlso Not domRA.EzProxyDatabase.Url.Equals(dtoRA.Url)) Then
                        domRA.Url = dtoRA.Url
                        dbc.Entry(domRA).Property(Function(m) m.Url).IsModified = True
                    Else
                        domRA.Url = Nothing
                        dbc.Entry(domRA).Property(Function(m) m.Url).IsModified = True
                    End If

                    'If Not domRA.EzProxyDatabase.DomainName.Equals(dtoRA.DomainName) Then 'original code was throwing nullreference exception when domainname field in ezproxydatabase was null
                    If domRA.EzProxyDatabase.DomainName Is Nothing Or
                        (domRA.EzProxyDatabase.DomainName IsNot Nothing AndAlso Not domRA.EzProxyDatabase.DomainName.Equals(dtoRA.DomainName)) Then
                        domRA.DomainName = dtoRA.DomainName
                        dbc.Entry(domRA).Property(Function(m) m.DomainName).IsModified = True
                    Else
                        domRA.DomainName = Nothing
                        dbc.Entry(domRA).Property(Function(m) m.DomainName).IsModified = True
                    End If

                    domRA.CampusRestriction = dtoRA.CampusRestriction
                    dbc.Entry(domRA).Property(Function(m) m.CampusRestriction).IsModified = True

                    dbc.SaveChanges()
                    dbc.AddAuditLog(domRA.ModifiedByUser, AuditLog.EventCodes.UpdateRemoteAuthentication, domRA.RemoteAuthenticationId, String.Format("Remote Authentication Updated.   College:{4}, Campus Restriction:{5}, Database Name:{0}, URL:{1}, Domain Name:{2}, Method:{3}", domRA.EzProxyDatabase.Name, domRA.EzProxyDatabase.Url, domRA.EzProxyDatabase.DomainName, domRA.ConnectionMethod.Name, domRA.College.Name, domRA.CampusRestriction.Trim()))
                    dbc.SaveChanges()
                End Using
                trans.Complete()
            End Using

            Return True
        End Function

        Public Shared Function DeleteRemoteAuthentication(id As Integer, username As String) As Boolean
            Dim ret As Boolean = False
            Using trans As New System.Transactions.TransactionScope()

                ret = DeleteRemoteAuthenticationCore(id, username)
                trans.Complete()
            End Using

            Return ret
        End Function

        Private Shared Function DeleteRemoteAuthenticationCore(id As Integer, username As String) As Boolean

            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim domRA As New Domain.RemoteAuthentication()
                domRA = dbc.RemoteAuthentications.Where(Function(m) m.RemoteAuthenticationId = id).FirstOrDefault()
                If Not IsNothing(domRA) Then
                    Dim deletingUser As UserProfile = dbc.Users.Where(Function(u) u.UserName.Equals(username)).Single()

                    ''''Added/Commented on September 07, 2016 to prevent object reference error when triming CampusRestriction string - Starts
                    If Not String.IsNullOrEmpty(domRA.CampusRestriction) Then
                        domRA.CampusRestriction = domRA.CampusRestriction.Trim()
                    End If
                    ''dbc.AddAuditLog(deletingUser, AuditLog.EventCodes.DeleteRemoteAuthentication, id, String.Format("Remote Authentication Deleted.    College:{4}, Campus Restriction:{5}, Database Name:{0}, URL:{1}, Domain Name:{2}, Method:{3}", domRA.EzProxyDatabase.Name, domRA.EzProxyDatabase.Url, domRA.EzProxyDatabase.DomainName, domRA.ConnectionMethod.Name, domRA.College.Name, domRA.CampusRestriction.Trim()))
                    dbc.AddAuditLog(deletingUser, AuditLog.EventCodes.DeleteRemoteAuthentication, id, String.Format("Remote Authentication Deleted.    College:{4}, Campus Restriction:{5}, Database Name:{0}, URL:{1}, Domain Name:{2}, Method:{3}", domRA.EzProxyDatabase.Name, domRA.EzProxyDatabase.Url, domRA.EzProxyDatabase.DomainName, domRA.ConnectionMethod.Name, domRA.College.Name, domRA.CampusRestriction))
                    ''''Added/Commented on September 07, 2016 to prevent object reference error when triming CampusRestriction string - Ends

                    dbc.SaveChanges()
                    dbc.RemoteAuthentications.Remove(domRA)
                    dbc.SaveChanges()

                End If
            End Using
            Return True
        End Function

        Public Shared Function RemoteAuthenticationExist(raId As Integer, dbId As Integer, collegeId As Integer, connectionMethodId As Integer) As Boolean

            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim ra As RemoteAuthentication = dbc.RemoteAuthentications.Where(Function(r) r.CollegeId = collegeId And r.ConnectionMethodId = connectionMethodId And r.EzProxyDatabaseId = dbId).SingleOrDefault()
                If IsNothing(ra) Then
                    Return False
                Else
                    If raId = ra.RemoteAuthenticationId Then
                        Return False
                    End If
                    Return True
                End If
            End Using

        End Function
        Public Shared Function GetConnectionMethodIdByName(name As String) As Integer

            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim ra As ConnectionMethod = dbc.ConnectionMethods.Where(Function(r) r.Name = name).SingleOrDefault()
                If IsNothing(ra) Then
                    Return 0
                Else
                    Return ra.ConnectionMethodId
                End If
            End Using

        End Function

    End Class

End Namespace