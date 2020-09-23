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
Imports System.Data.Entity.Validation

Namespace Services

    Public Class EzProxyService


        ''''Added on July 27, 2016 to truncate value of Title, Url, and Domain fields in EZDatabase table while importing directives
        Public Const EzDatabaseFieldMaxLength As Integer = 1000

        Private Sub New()
        End Sub

        Public Shared Function GetDatabases(ezproxyServerId As Integer, Optional databaseName As String = "", Optional urlSearchTerm As String = "", Optional domainName As String = "", Optional keywordName As String = "") As IEnumerable(Of EzProxyDatabaseDto)

            databaseName = databaseName.Trim()
            urlSearchTerm = urlSearchTerm.Trim()
            domainName = domainName.Trim()
            keywordName = keywordName.Trim()

            Using dbc As New DataAccess.OclsConnectionManagerDataContext()

                Dim domains As IEnumerable(Of EzProxyDatabase) =
                    (From d In dbc.EzProxyDatabases.Include("EzProxyServer")
                     Join r In dbc.EzProxyDatabaseDirectives On d.EzProxyDatabaseId Equals r.EzProxyDatabaseId
                     From o In r.Options
                     Where (String.IsNullOrEmpty(databaseName) OrElse d.Name.Contains(databaseName)) AndAlso
                           (String.IsNullOrEmpty(urlSearchTerm) OrElse d.Url.Contains(urlSearchTerm)) AndAlso
                           (String.IsNullOrEmpty(domainName) OrElse d.DomainName.Contains(domainName)) AndAlso
                           (String.IsNullOrEmpty(keywordName) OrElse (d.Comment.Contains(keywordName) OrElse o.OptionValue.Contains(keywordName))) AndAlso
                           (ezproxyServerId = 0 OrElse d.EzProxyServerId = ezproxyServerId OrElse Not d.EzProxyServerId.HasValue)
                     Order By d.Name
                     Select d).Distinct()

                Return domains.Select(Function(d) Dto.EzProxyDatabaseDto.GetDto(d, False)).ToArray().ToList().Distinct()

            End Using

        End Function

        Public Shared Function GetDatabase(ezProxyDatabaseId As Integer) As EzProxyDatabaseDto

            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim domain As EzProxyDatabase = dbc.EzProxyDatabases.Where(Function(w) w.EzProxyDatabaseId = ezProxyDatabaseId).FirstOrDefault()
                If Not domain Is Nothing Then
                    If Not IsNothing(domain.Directives) Then
                        domain.Directives = domain.Directives.OrderBy(Function(d) d.OutputOrder).ToArray().ToList()
                    End If

                    Return EzProxyDatabaseDto.GetDto(domain, True)
                Else
                    Return New Dto.EzProxyDatabaseDto() With {.Directives = New List(Of Dto.EzProxyDatabaseDirectiveDto)}
                End If
            End Using

        End Function

        Public Shared Function CreateDatabase(database As EzProxyDatabaseDto, username As String) As Boolean

            Try

                Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                    Dim existEDB As Domain.EzProxyDatabase = dbc.EzProxyDatabases.Where(Function(m) m.Name = database.Name).FirstOrDefault()
                    If Not IsNothing(existEDB) Then
                        Return False
                    End If

                    If Not database Is Nothing Then

                        Dim dbm As OclsConnectionManager.Domain.EzProxyDatabase = ApplyChanges(New Domain.EzProxyDatabase, database)

                        If Not dbm Is Nothing Then

                            Dim directivesDomain As New List(Of Domain.EzProxyDatabaseDirective)

                            If Not database.Directives Is Nothing Then

                                Dim maxOutputOrder As Integer = 0

                                If database.Directives.Count() > 0 Then
                                    maxOutputOrder = database.Directives.Max(Function(d) d.OutputOrder)
                                End If

                                For Each dbDir As OclsConnectionManager.Dto.EzProxyDatabaseDirectiveDto In database.Directives

                                    maxOutputOrder += 10
                                    dbDir.EzProxyDatabaseId = database.EzProxyDatabaseId

                                    Dim domainDatabaseDirective As New Domain.EzProxyDatabaseDirective
                                    domainDatabaseDirective.EzProxyDatabaseId = database.EzProxyDatabaseId
                                    domainDatabaseDirective.EzProxyDirectiveId = dbDir.EzProxyDirectiveId
                                    domainDatabaseDirective.OutputAs = dbDir.OutputAs
                                    domainDatabaseDirective.EzProxyDirective = dbc.EzProxyDirectives.Where(Function(d) d.EzProxyDirectiveId = dbDir.EzProxyDirectiveId).FirstOrDefault()
                                    domainDatabaseDirective.OutputOrder = maxOutputOrder

                                    Dim directiveOptionsDomain As New List(Of Domain.EzProxyDatabaseDirectiveOption)

                                    If Not IsNothing(dbDir.Options) Then

                                        For Each dbDirOption As OclsConnectionManager.Dto.EzProxyDatabaseDirectiveOptionDto In dbDir.Options

                                            Dim dbDirectiveOptionDomain As New Domain.EzProxyDatabaseDirectiveOption

                                            dbDirectiveOptionDomain.EzProxyDatabaseDirective = domainDatabaseDirective

                                            dbDirectiveOptionDomain.EzProxyOption = dbc.EzProxyOptions.Where(Function(d) d.EzProxyOptionId = dbDirOption.EzProxyOptionId).FirstOrDefault()

                                            dbDirectiveOptionDomain = ApplyDirectiveOptionChanges(dbDirectiveOptionDomain, dbDirOption)

                                            Dim objDbDirOption As System.Data.Entity.Infrastructure.DbEntityEntry = dbc.Entry(dbDirectiveOptionDomain)

                                            dbDirectiveOptionDomain = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabaseDirectiveOption)).Add(dbDirectiveOptionDomain), OclsConnectionManager.Domain.EzProxyDatabaseDirectiveOption)
                                            objDbDirOption.State = EntityState.Added

                                            directiveOptionsDomain.Add(dbDirectiveOptionDomain)

                                        Next

                                    End If

                                    If Not IsNothing(domainDatabaseDirective) Then

                                        domainDatabaseDirective = ApplyDirectiveChanges(domainDatabaseDirective, dbDir, False)
                                        domainDatabaseDirective.Options = directiveOptionsDomain

                                        Dim objDbDirective As System.Data.Entity.Infrastructure.DbEntityEntry = dbc.Entry(domainDatabaseDirective)

                                        If domainDatabaseDirective.EzProxyDatabaseDirectiveId > 0 Then
                                            domainDatabaseDirective = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabaseDirective)).Attach(domainDatabaseDirective), OclsConnectionManager.Domain.EzProxyDatabaseDirective)
                                            objDbDirective.State = EntityState.Modified
                                        Else
                                            domainDatabaseDirective = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabaseDirective)).Add(domainDatabaseDirective), OclsConnectionManager.Domain.EzProxyDatabaseDirective)
                                            objDbDirective.State = EntityState.Added
                                        End If

                                    End If

                                    directivesDomain.Add(domainDatabaseDirective)
                                Next

                            End If

                            dbm.Directives = directivesDomain
                            dbm.CreatedByUser = dbc.Users.Where(Function(u) u.UserName.Equals(username)).Single()


                            dbm = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabase)).Add(dbm), OclsConnectionManager.Domain.EzProxyDatabase)
                            dbc.SaveChanges()

                            dbc.AddAuditLog(dbm.CreatedByUser, AuditLog.EventCodes.CreateEzProxyDatabase, dbm.EzProxyDatabaseId, String.Format("EZProxy Stanza Created.  Name:{0}, Title:{1}", dbm.Name, dbm.Title))
                            dbc.SaveChanges()

                            database = EzProxyDatabaseDto.GetDto(dbm, True)
                        End If
                    End If
                End Using
            Catch dbEx As DbEntityValidationException
                Dim message As String = String.Empty
                For Each validationErrors As DbEntityValidationResult In dbEx.EntityValidationErrors
                    For Each validationError As DbValidationError In validationErrors.ValidationErrors
                        message += vbNewLine + String.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage)
                        ' the current instance as InnerException
                    Next
                Next

                Throw

            Catch ex As Exception
                Throw

            End Try

            Return True

        End Function

        Public Shared Function GetDirectiveByName(directiveName As String) As OclsConnectionManager.Domain.EzProxyDirective
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Return dbc.EzProxyDirectives.Where(Function(w) Not w.Name Is Nothing And Not directiveName Is Nothing And w.Name.ToLower().Trim() = directiveName.ToLower().Trim()).FirstOrDefault()
            End Using

        End Function

        Private Shared Function ApplyChanges(domain As OclsConnectionManager.Domain.EzProxyDatabase, dto As OclsConnectionManager.Dto.EzProxyDatabaseDto) As OclsConnectionManager.Domain.EzProxyDatabase

            If Not domain Is Nothing Then
                If domain.EzProxyDatabaseId = 0 Then
                    domain.Directives = New List(Of Domain.EzProxyDatabaseDirective)
                    domain.CreatedDate = Date.Now
                Else
                    domain.ModifiedDate = Date.Now
                End If

                domain.Title = dto.Title
                domain.DomainName = dto.DomainName
                domain.Url = dto.Url
                domain.Name = dto.Name
                domain.Comment = dto.Comment
                domain.OutputOrder = dto.OutputOrder
                domain.IsActive = dto.IsActive

                If dto.EzProxyServerId > 0 Then
                    domain.EzProxyServerId = dto.EzProxyServerId
                Else
                    domain.EzProxyServerId = Nothing
                End If
            End If

            Return domain

        End Function

        Public Shared Function UpdateDatabase(database As EzProxyDatabaseDto, username As String) As EzProxyDatabaseDto

            Using dbc As New DataAccess.OclsConnectionManagerDataContext()

                If Not database Is Nothing Then

                    Dim ezProxyDatabase As OclsConnectionManager.Domain.EzProxyDatabase = dbc.EzProxyDatabases.Where(Function(d) d.EzProxyDatabaseId = database.EzProxyDatabaseId).FirstOrDefault()

                    If Not ezProxyDatabase Is Nothing Then

                        If Not database.Directives Is Nothing Then

                            Dim databaseDirectiveIdsToBeDeleted As List(Of Integer) = Nothing

                            If Not IsNothing(ezProxyDatabase.Directives) Then
                                databaseDirectiveIdsToBeDeleted = ezProxyDatabase.Directives.Select(Function(d) d.EzProxyDatabaseDirectiveId).ToList().Except(database.Directives.Select(Function(d) d.EzProxyDatabaseDirectiveId)).ToList()
                            End If

                            Dim directivesDomain As New List(Of Domain.EzProxyDatabaseDirective)
                            Dim maxOutputOrder As Integer = 0

                            If database.Directives.Count() > 0 Then
                                maxOutputOrder = database.Directives.Max(Function(d) d.OutputOrder)
                            End If

                            For Each dbDir As OclsConnectionManager.Dto.EzProxyDatabaseDirectiveDto In database.Directives


                                dbDir.EzProxyDatabaseId = database.EzProxyDatabaseId

                                Dim domainDatabaseDirective As Domain.EzProxyDatabaseDirective = ezProxyDatabase.Directives.Where(Function(d) d.EzProxyDatabaseDirectiveId = dbDir.EzProxyDatabaseDirectiveId).FirstOrDefault()

                                If domainDatabaseDirective Is Nothing OrElse dbDir.EzProxyDatabaseDirectiveId = 0 Then
                                    maxOutputOrder += 10
                                    domainDatabaseDirective = New Domain.EzProxyDatabaseDirective()
                                    domainDatabaseDirective.EzProxyDatabaseId = database.EzProxyDatabaseId
                                    domainDatabaseDirective.EzProxyDirectiveId = dbDir.EzProxyDirectiveId
                                    domainDatabaseDirective.EzProxyDirective = dbc.EzProxyDirectives.Where(Function(d) d.EzProxyDirectiveId = dbDir.EzProxyDirectiveId).FirstOrDefault()
                                    domainDatabaseDirective.OutputOrder = maxOutputOrder
                                End If

                                domainDatabaseDirective.OutputAs = dbDir.OutputAs
                                domainDatabaseDirective.IsActive = dbDir.IsActive

                                Dim directiveOptionsDomain As New List(Of Domain.EzProxyDatabaseDirectiveOption)

                                If Not IsNothing(dbDir.Options) Then

                                    For Each dirOptionDto As Dto.EzProxyDatabaseDirectiveOptionDto In dbDir.Options

                                        Dim dbDirectiveOptionDomain As Domain.EzProxyDatabaseDirectiveOption = Nothing

                                        If Not IsNothing(domainDatabaseDirective) Then
                                            If dirOptionDto.EzProxyDatabaseDirectiveOptionId > 0 Then
                                                dbDirectiveOptionDomain = domainDatabaseDirective.Options.Where(Function(d) d.EzProxyDatabaseDirectiveOptionId = dirOptionDto.EzProxyDatabaseDirectiveOptionId).FirstOrDefault()
                                            End If
                                        End If

                                        Dim objDbDirOption As System.Data.Entity.Infrastructure.DbEntityEntry = Nothing

                                        If IsNothing(dbDirectiveOptionDomain) Then

                                            If dirOptionDto.IsActive Then

                                                dbDirectiveOptionDomain = New Domain.EzProxyDatabaseDirectiveOption()
                                                dbDirectiveOptionDomain.EzProxyDatabaseDirective = domainDatabaseDirective
                                                dbDirectiveOptionDomain = ApplyDirectiveOptionChanges(dbDirectiveOptionDomain, dirOptionDto)
                                                dbDirectiveOptionDomain.EzProxyOption = dbc.EzProxyOptions.Where(Function(d) d.EzProxyOptionId = dirOptionDto.EzProxyOptionId).FirstOrDefault()

                                                objDbDirOption = dbc.Entry(dbDirectiveOptionDomain)
                                                dbDirectiveOptionDomain = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabaseDirectiveOption)).Add(dbDirectiveOptionDomain), OclsConnectionManager.Domain.EzProxyDatabaseDirectiveOption)
                                                objDbDirOption.State = EntityState.Added

                                                directiveOptionsDomain.Add(dbDirectiveOptionDomain)

                                            Else
                                                ''''No Action
                                            End If
                                        Else
                                            If dirOptionDto.IsActive Then
                                                dbDirectiveOptionDomain.EzProxyDatabaseDirective = domainDatabaseDirective
                                                dbDirectiveOptionDomain = ApplyDirectiveOptionChanges(dbDirectiveOptionDomain, dirOptionDto)
                                                dbDirectiveOptionDomain.EzProxyOption = dbc.EzProxyOptions.Where(Function(d) d.EzProxyOptionId = dirOptionDto.EzProxyOptionId).FirstOrDefault()

                                                objDbDirOption = dbc.Entry(dbDirectiveOptionDomain)
                                                dbDirectiveOptionDomain = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabaseDirectiveOption)).Attach(dbDirectiveOptionDomain), OclsConnectionManager.Domain.EzProxyDatabaseDirectiveOption)
                                                objDbDirOption.State = EntityState.Modified

                                                directiveOptionsDomain.Add(dbDirectiveOptionDomain)

                                            Else
                                                objDbDirOption = dbc.Entry(dbDirectiveOptionDomain)
                                                dbDirectiveOptionDomain = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabaseDirectiveOption)).Remove(dbDirectiveOptionDomain), OclsConnectionManager.Domain.EzProxyDatabaseDirectiveOption)
                                                objDbDirOption.State = EntityState.Deleted

                                            End If

                                        End If

                                    Next

                                End If

                                If Not IsNothing(domainDatabaseDirective) Then
                                    If (domainDatabaseDirective.EzProxyDatabaseDirectiveId <> 0) Then
                                        domainDatabaseDirective = ApplyDirectiveChanges(domainDatabaseDirective, dbDir, False)
                                    End If
                                    domainDatabaseDirective.Options = directiveOptionsDomain

                                    Dim objDbDirective As System.Data.Entity.Infrastructure.DbEntityEntry = dbc.Entry(domainDatabaseDirective)

                                    If domainDatabaseDirective.EzProxyDatabaseDirectiveId > 0 Then
                                        domainDatabaseDirective = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabaseDirective)).Attach(domainDatabaseDirective), OclsConnectionManager.Domain.EzProxyDatabaseDirective)
                                        objDbDirective.State = EntityState.Modified
                                    Else
                                        domainDatabaseDirective = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabaseDirective)).Add(domainDatabaseDirective), OclsConnectionManager.Domain.EzProxyDatabaseDirective)
                                        objDbDirective.State = EntityState.Added
                                    End If

                                End If

                                directivesDomain.Add(domainDatabaseDirective)

                            Next

                            If Not IsNothing(databaseDirectiveIdsToBeDeleted) Then

                                If databaseDirectiveIdsToBeDeleted.Count > 0 Then

                                    For Each databaseDirectiveId In databaseDirectiveIdsToBeDeleted
                                        If databaseDirectiveId > 0 Then
                                            DeleteDatabaseDirective(databaseDirectiveId)
                                        End If

                                    Next
                                End If
                            End If

                            ezProxyDatabase = ApplyChanges(ezProxyDatabase, database)
                            ezProxyDatabase.Directives = directivesDomain

                            Dim obj As System.Data.Entity.Infrastructure.DbEntityEntry = dbc.Entry(ezProxyDatabase)
                            ezProxyDatabase = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabase)).Attach(ezProxyDatabase), OclsConnectionManager.Domain.EzProxyDatabase)
                            ' ezProxyDatabase.ModifiedByUser = dbc.Users.Where(Function(u) u.UserName.Equals(username)).Single()
                            ezProxyDatabase.ModifiedByUserId = dbc.Users.Where(Function(u) u.UserName.Equals(username)).Single().UserId
                            obj.State = EntityState.Modified
                            dbc.SaveChanges()

                            dbc.AddAuditLog(ezProxyDatabase.ModifiedByUser, AuditLog.EventCodes.UpdateEzProxyDatabase, ezProxyDatabase.EzProxyDatabaseId, String.Format("EZProxy Stanza Updated.  Name:{0}, Title:{1}", ezProxyDatabase.Name, ezProxyDatabase.Title))
                            dbc.SaveChanges()

                        End If
                        Return Dto.EzProxyDatabaseDto.GetDto(ezProxyDatabase, True)
                    Else
                        Return database
                    End If
                Else
                    Return database
                End If

            End Using

            Return database

        End Function

        Public Shared Function UpdateDatabaseFieldsOnly(database As EzProxyDatabaseDto, username As String) As EzProxyDatabaseDto

            Using dbc As New DataAccess.OclsConnectionManagerDataContext()

                If Not database Is Nothing Then

                    Dim ezProxyDatabase As OclsConnectionManager.Domain.EzProxyDatabase = dbc.EzProxyDatabases.Where(Function(d) d.EzProxyDatabaseId = database.EzProxyDatabaseId).FirstOrDefault()

                    If Not ezProxyDatabase Is Nothing Then

                        ezProxyDatabase = ApplyChanges(ezProxyDatabase, database)

                        Dim objDbDirective As System.Data.Entity.Infrastructure.DbEntityEntry = dbc.Entry(ezProxyDatabase)

                        ezProxyDatabase = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabase)).Attach(ezProxyDatabase), OclsConnectionManager.Domain.EzProxyDatabase)

                        objDbDirective.State = EntityState.Modified

                        dbc.SaveChanges()

                        Return Dto.EzProxyDatabaseDto.GetDto(ezProxyDatabase, True)
                    Else
                        Return database
                    End If
                Else
                    Return database
                End If
            End Using
            Return database

        End Function


        Public Shared Function DeleteDatabase(database As EzProxyDatabaseDto, username As String) As Boolean
            Dim retValue As Integer = 0
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                If Not database Is Nothing Then
                    Try
                        Dim ezProxyDatabase As OclsConnectionManager.Domain.EzProxyDatabase = dbc.EzProxyDatabases.Where(Function(d) d.EzProxyDatabaseId = database.EzProxyDatabaseId).FirstOrDefault()

                        If Not ezProxyDatabase Is Nothing Then
                            Dim deletingUser As UserProfile = dbc.Users.Where(Function(u) u.UserName.Equals(username)).Single()
                            dbc.AddAuditLog(deletingUser, AuditLog.EventCodes.DeleteEzProxyDatabase, ezProxyDatabase.EzProxyDatabaseId, String.Format("EZProxy Stanza Deleted.  Name:{0}, Title:{1}", ezProxyDatabase.Name, ezProxyDatabase.Title))
                            ezProxyDatabase = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabase)).Remove(ezProxyDatabase), OclsConnectionManager.Domain.EzProxyDatabase)
                            retValue = dbc.SaveChanges()
                        End If
                    Catch ex As Exception
                        retValue = 0
                    End Try

                End If
            End Using

            If retValue > 0 Then
                Return True
            Else
                Return False
            End If

        End Function


        Public Shared Function GetDirectives(ezProxyDatabaseId As Integer) As List(Of EzProxyDatabaseDirectiveDto)
            Dim db As OclsConnectionManager.Dto.EzProxyDatabaseDto = GetDatabase(ezProxyDatabaseId)

            If Not db Is Nothing Then
                Return db.Directives.ToList()
            Else
                Return New List(Of EzProxyDatabaseDirectiveDto)
            End If

        End Function

        Shared Function GetDirectiveOptions(ezProxyDatabaseId As Integer, ezProxyDirectiveId As Integer) As List(Of EzProxyOptionDto)

            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim ezProxyDirective As OclsConnectionManager.Domain.EzProxyDirective = dbc.EzProxyDirectives.Where(Function(d) d.EzProxyDirectiveId = ezProxyDirectiveId).FirstOrDefault()

                If Not ezProxyDirective Is Nothing Then
                    Dim ezProxyDirectiveDto As OclsConnectionManager.Dto.EzProxyDirectiveDto = EzProxyDirectiveDto.GetDto(ezProxyDirective, True)
                    Return ezProxyDirectiveDto.Options.ToList()
                Else
                    Return New List(Of EzProxyOptionDto)
                End If
            End Using
        End Function

        Public Shared Function GetDatabaseDirective(ezProxyDatabaseDirectiveId As Integer, ezProxyDatabaseId As Integer, ezProxyDirectiveId As Integer) As EzProxyDatabaseDirectiveDto

            Using dbc As New DataAccess.OclsConnectionManagerDataContext()

                Dim ezProxyDirectiveDomain As OclsConnectionManager.Domain.EzProxyDatabaseDirective = Nothing

                If ezProxyDatabaseDirectiveId > 0 Then
                    ezProxyDirectiveDomain = dbc.EzProxyDatabaseDirectives.Where(Function(d) d.EzProxyDatabaseDirectiveId = ezProxyDatabaseDirectiveId).FirstOrDefault()
                End If

                Dim ids As New List(Of Integer)

                If Not IsNothing(ezProxyDirectiveDomain) Then
                    ids.Add(ezProxyDirectiveDomain.EzProxyDirectiveId)
                End If

                Dim lstProxies As New List(Of Dto.EzProxyDatabaseDirectiveOptionDto)

                Dim allProxyOptions As List(Of Domain.EzProxyOption) = dbc.EzProxyOptions.Where(Function(d) d.EzProxyDirectiveId = ezProxyDirectiveId).OrderBy(Function(d) d.OutputOrder).ToList()

                If Not IsNothing(allProxyOptions) Then

                    For Each opts As Domain.EzProxyOption In allProxyOptions

                        If Not String.IsNullOrEmpty(opts.OptionOrQualifier) Then

                            If opts.OptionOrQualifier.ToLower().StartsWith("q") Then
                                opts.OptionOrQualifier = "Qualifier"
                            End If

                            If opts.OptionOrQualifier.ToLower().StartsWith("o") Then
                                opts.OptionOrQualifier = "Option"
                            End If

                        End If

                        Dim ezProxyDatabaseDirectiveOptionDomain As Domain.EzProxyDatabaseDirectiveOption = Nothing

                        If (ids.Contains(opts.EzProxyDirectiveId)) Then

                            Dim ezProxyDatabaseDirective As Domain.EzProxyDatabaseDirective = Nothing

                            If ezProxyDatabaseDirectiveId > 0 Then
                                ezProxyDatabaseDirective = dbc.EzProxyDatabaseDirectives.Where(Function(f) f.EzProxyDatabaseDirectiveId = ezProxyDatabaseDirectiveId).FirstOrDefault()
                            Else
                                ezProxyDatabaseDirective = dbc.EzProxyDatabaseDirectives.Where(Function(f) f.EzProxyDirectiveId = opts.EzProxyDirectiveId And f.EzProxyDatabaseId = ezProxyDatabaseId).FirstOrDefault()
                            End If

                            If Not ezProxyDatabaseDirective Is Nothing Then

                                ezProxyDatabaseDirectiveOptionDomain = ezProxyDatabaseDirective.Options.Where(Function(o) o.EzProxyDatabaseDirectiveId = ezProxyDatabaseDirective.EzProxyDatabaseDirectiveId And o.EzProxyOptionId = opts.EzProxyOptionId).FirstOrDefault()

                                If IsNothing(ezProxyDatabaseDirectiveOptionDomain) Then
                                    ezProxyDatabaseDirectiveOptionDomain = New Domain.EzProxyDatabaseDirectiveOption
                                    ezProxyDatabaseDirectiveOptionDomain.EzProxyDatabaseDirective = ezProxyDatabaseDirective
                                    ezProxyDatabaseDirectiveOptionDomain.EzProxyOption = opts
                                    ezProxyDatabaseDirectiveOptionDomain.IsActive = False
                                    ezProxyDatabaseDirectiveOptionDomain.OptionValue = String.Empty
                                End If
                            Else
                                ezProxyDatabaseDirectiveOptionDomain = New Domain.EzProxyDatabaseDirectiveOption
                                ezProxyDatabaseDirectiveOptionDomain.EzProxyDatabaseDirective = ezProxyDatabaseDirective
                                ezProxyDatabaseDirectiveOptionDomain.EzProxyOption = opts
                                ezProxyDatabaseDirectiveOptionDomain.IsActive = False
                                ezProxyDatabaseDirectiveOptionDomain.OptionValue = String.Empty
                            End If
                        Else
                            ezProxyDatabaseDirectiveOptionDomain = New Domain.EzProxyDatabaseDirectiveOption
                            ezProxyDatabaseDirectiveOptionDomain.EzProxyDatabaseDirective = New Domain.EzProxyDatabaseDirective With {.EzProxyDirectiveId = ezProxyDirectiveId, .EzProxyDatabaseId = ezProxyDatabaseId}
                            ezProxyDatabaseDirectiveOptionDomain.EzProxyOption = opts
                            ezProxyDatabaseDirectiveOptionDomain.IsActive = False
                            ezProxyDatabaseDirectiveOptionDomain.OptionValue = String.Empty
                        End If

                        If Not ezProxyDatabaseDirectiveOptionDomain Is Nothing Then
                            lstProxies.Add(OclsConnectionManager.Dto.EzProxyDatabaseDirectiveOptionDto.GetDto(ezProxyDatabaseDirectiveOptionDomain))
                        End If
                    Next
                End If

                Dim ezProxyDatabaseDirectiveDto As Dto.EzProxyDatabaseDirectiveDto = Nothing

                If Not ezProxyDirectiveDomain Is Nothing Then
                    ezProxyDatabaseDirectiveDto = EzProxyDatabaseDirective.GetDto(ezProxyDirectiveDomain, True)
                End If

                If Not ezProxyDatabaseDirectiveDto Is Nothing Then

                    ezProxyDatabaseDirectiveDto.Options = lstProxies

                    Return ezProxyDatabaseDirectiveDto
                Else
                    Return New Dto.EzProxyDatabaseDirectiveDto With {.Options = lstProxies}

                End If
            End Using

        End Function


        Shared Function UpdateDatabaseDirective(ezProxyDatabaseDirectiveDto As EzProxyDatabaseDirectiveDto) As EzProxyDatabaseDirectiveDto

            Try
                If Not ezProxyDatabaseDirectiveDto Is Nothing Then

                    Using dbc As New DataAccess.OclsConnectionManagerDataContext()

                        Dim ezProxyDatabaseDirectiveDomain As OclsConnectionManager.Domain.EzProxyDatabaseDirective = dbc.EzProxyDatabaseDirectives _
                    .Where(Function(d) d.EzProxyDatabaseDirectiveId = ezProxyDatabaseDirectiveDto.EzProxyDatabaseDirectiveId).FirstOrDefault()

                        Dim maxOutputOrder As Integer = 0

                        If IsNothing(ezProxyDatabaseDirectiveDomain) Then

                            ezProxyDatabaseDirectiveDomain = ApplyDirectiveChanges(New Domain.EzProxyDatabaseDirective() With {.Options = New List(Of Domain.EzProxyDatabaseDirectiveOption)}, ezProxyDatabaseDirectiveDto, False)
                            ezProxyDatabaseDirectiveDomain.EzProxyDirective = dbc.EzProxyDirectives.Where(Function(d) d.EzProxyDirectiveId = ezProxyDatabaseDirectiveDto.EzProxyDirectiveId).FirstOrDefault()

                            Dim ezProxyDatabase As Domain.EzProxyDatabase = dbc.EzProxyDatabases.Where(Function(d) d.EzProxyDatabaseId = ezProxyDatabaseDirectiveDto.EzProxyDatabaseId).FirstOrDefault()

                            If Not IsNothing(ezProxyDatabase) Then

                                If Not IsNothing(ezProxyDatabase.Directives) Then

                                    If ezProxyDatabase.Directives.Count() > 0 Then
                                        maxOutputOrder = ezProxyDatabase.Directives.Max(Function(d) d.OutputOrder)
                                    End If

                                End If

                                ezProxyDatabaseDirectiveDomain.OutputOrder = maxOutputOrder + 10

                            End If

                        End If

                        If Not ezProxyDatabaseDirectiveDomain Is Nothing Then

                            If Not ezProxyDatabaseDirectiveDomain.Options Is Nothing Then

                                If Not IsNothing(ezProxyDatabaseDirectiveDto.Options) Then

                                    Dim directiveOptionIdsToBeDeleted As List(Of Integer) = ezProxyDatabaseDirectiveDto.Options.Select(Function(d) d.EzProxyDatabaseDirectiveOptionId).ToList().Except(ezProxyDatabaseDirectiveDomain.Options.Select(Function(d) d.EzProxyDatabaseDirectiveOptionId)).ToList()

                                    Dim directiveOptionsDomain As New List(Of Domain.EzProxyDatabaseDirectiveOption)

                                    For Each dirOptionDto As Dto.EzProxyDatabaseDirectiveOptionDto In ezProxyDatabaseDirectiveDto.Options

                                        Dim dbDirectiveOptionDomain As Domain.EzProxyDatabaseDirectiveOption = Nothing

                                        If Not IsNothing(ezProxyDatabaseDirectiveDomain) Then
                                            If dirOptionDto.EzProxyDatabaseDirectiveOptionId > 0 Then
                                                dbDirectiveOptionDomain = ezProxyDatabaseDirectiveDomain.Options.Where(Function(d) d.EzProxyDatabaseDirectiveOptionId = dirOptionDto.EzProxyDatabaseDirectiveOptionId).FirstOrDefault()
                                            End If
                                        End If

                                        Dim objDbDirOption As System.Data.Entity.Infrastructure.DbEntityEntry = Nothing

                                        If IsNothing(dbDirectiveOptionDomain) Then

                                            If dirOptionDto.IsActive Then

                                                dbDirectiveOptionDomain = New Domain.EzProxyDatabaseDirectiveOption()

                                                dbDirectiveOptionDomain.EzProxyDatabaseDirective = ezProxyDatabaseDirectiveDomain
                                                dbDirectiveOptionDomain = ApplyDirectiveOptionChanges(dbDirectiveOptionDomain, dirOptionDto)
                                                dbDirectiveOptionDomain.EzProxyOption = dbc.EzProxyOptions.Where(Function(d) d.EzProxyOptionId = dirOptionDto.EzProxyOptionId).FirstOrDefault()

                                                objDbDirOption = dbc.Entry(dbDirectiveOptionDomain)
                                                dbDirectiveOptionDomain = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabaseDirectiveOption)).Add(dbDirectiveOptionDomain), OclsConnectionManager.Domain.EzProxyDatabaseDirectiveOption)
                                                objDbDirOption.State = EntityState.Added

                                                directiveOptionsDomain.Add(dbDirectiveOptionDomain)

                                            Else
                                                ''''No Action
                                            End If
                                        Else
                                            If dirOptionDto.IsActive Then
                                                dbDirectiveOptionDomain.EzProxyDatabaseDirective = ezProxyDatabaseDirectiveDomain
                                                dbDirectiveOptionDomain = ApplyDirectiveOptionChanges(dbDirectiveOptionDomain, dirOptionDto)
                                                dbDirectiveOptionDomain.EzProxyOption = dbc.EzProxyOptions.Where(Function(d) d.EzProxyOptionId = dirOptionDto.EzProxyOptionId).FirstOrDefault()

                                                objDbDirOption = dbc.Entry(dbDirectiveOptionDomain)
                                                dbDirectiveOptionDomain = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabaseDirectiveOption)).Attach(dbDirectiveOptionDomain), OclsConnectionManager.Domain.EzProxyDatabaseDirectiveOption)
                                                objDbDirOption.State = EntityState.Modified

                                                directiveOptionsDomain.Add(dbDirectiveOptionDomain)

                                            Else
                                                objDbDirOption = dbc.Entry(dbDirectiveOptionDomain)
                                                dbDirectiveOptionDomain = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabaseDirectiveOption)).Remove(dbDirectiveOptionDomain), OclsConnectionManager.Domain.EzProxyDatabaseDirectiveOption)
                                                objDbDirOption.State = EntityState.Deleted

                                            End If

                                        End If

                                    Next

                                    If Not IsNothing(ezProxyDatabaseDirectiveDomain) Then

                                        ezProxyDatabaseDirectiveDomain = ApplyDirectiveChanges(ezProxyDatabaseDirectiveDomain, ezProxyDatabaseDirectiveDto, False)
                                        ezProxyDatabaseDirectiveDomain.Options = directiveOptionsDomain

                                        Dim objDbDirective As System.Data.Entity.Infrastructure.DbEntityEntry = dbc.Entry(ezProxyDatabaseDirectiveDomain)

                                        If ezProxyDatabaseDirectiveDomain.EzProxyDatabaseDirectiveId > 0 Then
                                            ezProxyDatabaseDirectiveDomain = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabaseDirective)).Attach(ezProxyDatabaseDirectiveDomain), OclsConnectionManager.Domain.EzProxyDatabaseDirective)
                                            objDbDirective.State = EntityState.Modified
                                        Else
                                            ezProxyDatabaseDirectiveDomain = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabaseDirective)).Add(ezProxyDatabaseDirectiveDomain), OclsConnectionManager.Domain.EzProxyDatabaseDirective)
                                            objDbDirective.State = EntityState.Added
                                        End If

                                    End If

                                End If

                                dbc.SaveChanges()

                            End If

                            Return Dto.EzProxyDatabaseDirectiveDto.GetDto(ezProxyDatabaseDirectiveDomain)
                        Else
                            Return Nothing
                        End If
                    End Using
                Else
                    Return Nothing
                End If

            Catch dbEx As DbEntityValidationException
                Dim message As String = String.Empty
                For Each validationErrors As DbEntityValidationResult In dbEx.EntityValidationErrors
                    For Each validationError As DbValidationError In validationErrors.ValidationErrors
                        message += vbNewLine + String.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage)
                    Next
                Next

                Throw

            Catch ex As Exception
                Throw
            End Try

        End Function

        Private Shared Function ApplyDirectiveOptionChanges(domain As OclsConnectionManager.Domain.EzProxyDatabaseDirectiveOption, dto As OclsConnectionManager.Dto.EzProxyDatabaseDirectiveOptionDto) As OclsConnectionManager.Domain.EzProxyDatabaseDirectiveOption
            If Not domain Is Nothing Then

                domain.IsActive = dto.IsActive
                domain.OptionValue = dto.OptionValue
                domain.EzProxyOptionId = dto.EzProxyOptionId

            End If

            Return domain
        End Function

        Private Shared Function ApplyDirectiveChanges(domain As OclsConnectionManager.Domain.EzProxyDatabaseDirective, dto As OclsConnectionManager.Dto.EzProxyDatabaseDirectiveDto, Optional processOptions As Boolean = True) As OclsConnectionManager.Domain.EzProxyDatabaseDirective

            If Not domain Is Nothing Then

                If processOptions Then

                    If domain.EzProxyDatabaseId = 0 AndAlso domain.EzProxyDirectiveId = 0 Then
                        domain.Options = New List(Of Domain.EzProxyDatabaseDirectiveOption)
                    Else
                        Dim newOptionList As New List(Of Domain.EzProxyDatabaseDirectiveOption)

                        If Not dto.Options Is Nothing Then

                            For Each dirOption As Dto.EzProxyDatabaseDirectiveOptionDto In dto.Options

                                Dim intEzProxyDatabaseDirectiveId As Integer = dirOption.EzProxyDatabaseDirectiveId
                                Dim intEzProxyOptionId As Integer = dirOption.EzProxyOptionId
                                Dim intEzProxyDatabaseDirectiveOptionId As Integer = dirOption.EzProxyDatabaseDirectiveOptionId

                                Dim dtoOption As Domain.EzProxyDatabaseDirectiveOption = domain.Options.Where(Function(d) _
                                                                                                              d.EzProxyDatabaseDirectiveId = intEzProxyDatabaseDirectiveId _
                                                                                                              And d.EzProxyDatabaseDirectiveOptionId = intEzProxyDatabaseDirectiveOptionId).FirstOrDefault()
                                If Not dtoOption Is Nothing Then
                                    newOptionList.Add(ApplyDirectiveOptionChanges(dtoOption, dirOption))
                                End If

                            Next

                        End If
                        domain.Options.Clear()
                        domain.Options = newOptionList
                    End If

                End If

                domain.Comment = dto.Comment
                domain.IsActive = dto.IsActive
                domain.EzProxyDatabaseId = dto.EzProxyDatabaseId
                domain.EzProxyDirectiveId = dto.EzProxyDirectiveId
                domain.IsActive = dto.IsActive
                domain.OutputAs = dto.OutputAs
                domain.OutputOrder = dto.OutputOrder

            End If

            Return domain
        End Function

        Public Shared Function DeleteDatabaseDirective(EzProxyDatabaseDirectiveId As Integer) As Boolean
            Dim retValue As Integer
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                If EzProxyDatabaseDirectiveId > 0 Then

                    Dim ezProxyDatabaseDirective As OclsConnectionManager.Domain.EzProxyDatabaseDirective = dbc.EzProxyDatabaseDirectives.Where(Function(d) d.EzProxyDatabaseDirectiveId = EzProxyDatabaseDirectiveId).FirstOrDefault()

                    If Not ezProxyDatabaseDirective Is Nothing Then
                        ezProxyDatabaseDirective = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabaseDirective)).Remove(ezProxyDatabaseDirective), OclsConnectionManager.Domain.EzProxyDatabaseDirective)
                        retValue = dbc.SaveChanges()
                    End If

                End If
            End Using

            If retValue > 0 Then
                Return True
            Else
                Return False
            End If

        End Function


        Public Shared Function GetDatabaseDirective(ezProxyDatabaseDirectiveId As Integer) As EzProxyDatabaseDirectiveDto

            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                If ezProxyDatabaseDirectiveId > 0 Then
                    Dim ezProxyDatabaseDirectiveDto As Domain.EzProxyDatabaseDirective = dbc.EzProxyDatabaseDirectives.Where(Function(d) d.EzProxyDatabaseDirectiveId = ezProxyDatabaseDirectiveId).FirstOrDefault()
                    If Not ezProxyDatabaseDirectiveDto Is Nothing Then
                        Return Dto.EzProxyDatabaseDirectiveDto.GetDto(ezProxyDatabaseDirectiveDto)
                    Else
                        Return New EzProxyDatabaseDirectiveDto
                    End If

                Else
                    Return New EzProxyDatabaseDirectiveDto
                End If
            End Using

        End Function

        Public Shared Function GetAllDirectives() As List(Of EzProxyDirectiveDto)
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim domains As List(Of Domain.EzProxyDirective) = dbc.EzProxyDirectives.ToList()
                Return domains.Select(Function(d) Dto.EzProxyDirectiveDto.GetDto(d, False)).ToArray().ToList()
            End Using
        End Function

        Public Shared Function GetAllServers() As List(Of EzProxyServerDto)
            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                Dim domains As List(Of Domain.EzProxyServer) = dbc.EzProxyServers.ToList()
                Return domains.Select(Function(d) Dto.EzProxyServerDto.GetDto(d)).ToArray().ToList()
            End Using
        End Function

        Private Shared Function DeleteDirective(dirs As EzProxyDatabaseDirective) As Boolean
            Dim retValue As Integer

            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                If Not dirs Is Nothing Then

                    Dim ezProxyDatabaseDirective As OclsConnectionManager.Domain.EzProxyDatabaseDirective = dbc.EzProxyDatabaseDirectives.Where(Function(d) d.EzProxyDatabaseDirectiveId = dirs.EzProxyDatabaseDirectiveId).FirstOrDefault()

                    If Not ezProxyDatabaseDirective Is Nothing Then

                        ezProxyDatabaseDirective = CType(dbc.Set(GetType(OclsConnectionManager.Domain.EzProxyDatabaseDirective)).Remove(ezProxyDatabaseDirective), OclsConnectionManager.Domain.EzProxyDatabaseDirective)

                        retValue = dbc.SaveChanges()

                    End If
                End If
            End Using

            Return retValue > 0
        End Function

        Public Shared Function PreviewEzProxyConfig(username As String, ezProxyServerId As Integer) As String

            Return GenerateEzProxyConfig(username, ezProxyServerId)

        End Function

        Public Shared Function SaveEzProxyConfig(username As String, ezProxyServerId As Integer) As String

            Dim result As String = GenerateEzProxyConfig(username, ezProxyServerId)
            Using context As New DataAccess.OclsConnectionManagerDataContext()
                Dim currentUser As UserProfile = context.Users.Where(Function(u) u.UserName.Equals(username)).Single()
                context.EzProxyConfigs.Add(New EzProxyConfig() With
                    {
                        .CreatedByUser = currentUser,
                        .CreatedDate = DateTime.Now,
                        .EzProxyServerId = ezProxyServerId,
                        .ConfigContents = result.ToString()
                    })
                context.SaveChanges()
            End Using

            Return result.ToString()

        End Function

        Public Shared Function GenerateEzProxyConfig(username As String, ezProxyServerId As Integer) As String

            Dim result As New StringBuilder()
            Dim databases As IEnumerable(Of EzProxyDatabase)

            Using context As New DataAccess.OclsConnectionManagerDataContext()
                databases = context.EzProxyDatabases.Where(Function(d) d.IsActive AndAlso (d.EzProxyServerId Is Nothing OrElse d.EzProxyServerId.Value = ezProxyServerId)).OrderBy(Function(d) If(d.OutputOrder, Integer.MaxValue))
                result.AppendLine("# Autogenerated EZproxy configuration file.")
                result.AppendLine("# Generated by " + username + " on " + DateTime.Now.ToLongDateString + " " + DateTime.Now.ToLongTimeString)
                result.AppendLine("#")

                For Each database As EzProxyDatabase In databases
                    result.AppendLine("# " + database.Name.Replace(vbCrLf, ""))
                    For Each directive As EzProxyDatabaseDirective In database.Directives.Where(Function(d) d.IsActive).OrderBy(Function(d) d.OutputOrder)
                        result.AppendLine(directive.OutputAs)
                    Next

                    result.AppendLine()

                Next

            End Using

            Return result.ToString()

        End Function

        Shared Function GetDirective(ezProxyDirectiveId As Integer) As EzProxyDirectiveDto

            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                If ezProxyDirectiveId > 0 Then
                    Dim ezProxyDatabaseDirectiveDto As Domain.EzProxyDirective = dbc.EzProxyDirectives.Where(Function(d) d.EzProxyDirectiveId = ezProxyDirectiveId).FirstOrDefault()
                    If Not ezProxyDatabaseDirectiveDto Is Nothing Then
                        Return Dto.EzProxyDirectiveDto.GetDto(ezProxyDatabaseDirectiveDto, False)
                    Else
                        Return New EzProxyDirectiveDto
                    End If

                Else
                    Return New EzProxyDirectiveDto
                End If
            End Using
        End Function

        Public Shared Function GetOutputAsValue(databaseDirectiveOption As Dto.EzProxyDatabaseDirectiveOptionDto) As String

            Dim returnString As String = String.Empty

            If Not IsNothing(databaseDirectiveOption) Then

                If databaseDirectiveOption.HasInputValue Then

                    If databaseDirectiveOption.OutputAs.Contains("{0}") Then
                        returnString += String.Format(databaseDirectiveOption.OutputAs, databaseDirectiveOption.OptionValue)
                    Else
                        returnString += databaseDirectiveOption.OutputAs + " " + databaseDirectiveOption.OptionValue
                    End If
                Else
                    returnString += databaseDirectiveOption.OutputAs + " "
                End If

                If returnString.Contains("<br/>") Then
                    returnString = returnString.Replace("<br/>", vbCrLf)
                End If

                If returnString.Contains("&nbsp;") Then
                    returnString = returnString.Replace("&nbsp;", " ")
                End If

            End If

            Return returnString

        End Function

        Public Shared Function GetDirectiveOption(directiveId As Integer, optionName As String) As EzProxyOptionDto

            Using dbc As New DataAccess.OclsConnectionManagerDataContext()
                If directiveId <= 0 Then
                    Return Nothing
                Else
                    If String.IsNullOrEmpty(optionName) Then
                        Return Nothing
                    Else
                        Dim ezProxyOptionDomain As Domain.EzProxyOption = dbc.EzProxyOptions.Where(Function(d) d.EzProxyDirectiveId = directiveId And d.Name.ToLower().Trim() = optionName.ToLower().Trim()).FirstOrDefault()

                        If IsNothing(ezProxyOptionDomain) Then
                            Return Nothing
                        Else
                            Return Dto.EzProxyOptionDto.GetDto(ezProxyOptionDomain)
                        End If
                    End If
                End If
            End Using

        End Function

        Private Shared Sub DoTitle(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

            Dim text As String = StripDirective(line)
            Dim dir As New Domain.EzProxyDatabaseDirective
            Dim opt As New Domain.EzProxyDatabaseDirectiveOption

            dir.EzProxyDatabase = ezdb
            dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = "Title").Single()
            dir.IsActive = True

            If text = "Criterion Pic" Then
                Console.WriteLine("Stop")
            End If

            If text.Contains("-hide") Then
                text = text.Replace("-hide", "").Trim()
                opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "-Hide" And w.EzProxyDirective.Name = "Title").Single()
                opt.EzProxyDatabaseDirective = dir
                opt.IsActive = True
                dir.Options.Add(opt)
            End If

            opt = New Domain.EzProxyDatabaseDirectiveOption()

            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "text" And w.EzProxyDirective.Name = "Title").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = text.Trim()

            ezdb.Name = text

            dir.Options.Add(opt)
            dir.OutputAs = RenderOutputAs(dir)
            ezdb.Directives.Add(dir)

        End Sub

        Private Shared Sub DoUrl(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

            Dim text As String = StripDirective(line)
            Dim dir As New Domain.EzProxyDatabaseDirective
            Dim opt As Domain.EzProxyDatabaseDirectiveOption
            Dim urlValue As String

            dir.EzProxyDatabase = ezdb
            dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = "URL").Single()
            dir.IsActive = True

            Dim split As String() = text.Split(" ".ToCharArray())

            If split.Length > 1 Then
                opt = New Domain.EzProxyDatabaseDirectiveOption
                opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "-Form" And w.EzProxyDirective.Name = "URL").Single()
                opt.EzProxyDatabaseDirective = dir
                opt.IsActive = True
                opt.OptionValue = split(0).Replace("-Form=", "").Trim()
                dir.Options.Add(opt)

                opt = New Domain.EzProxyDatabaseDirectiveOption
                opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "-RewriteHost" And w.EzProxyDirective.Name = "URL").Single()
                opt.EzProxyDatabaseDirective = dir
                opt.IsActive = True
                dir.Options.Add(opt)

                opt = New Domain.EzProxyDatabaseDirectiveOption
                opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "label" And w.EzProxyDirective.Name = "URL").Single()
                opt.EzProxyDatabaseDirective = dir
                opt.IsActive = True
                opt.OptionValue = split(2).Trim()
                dir.Options.Add(opt)

                urlValue = split(3).Trim()
            Else
                urlValue = text.Trim()
            End If

            opt = New Domain.EzProxyDatabaseDirectiveOption
            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "url" And w.EzProxyDirective.Name = "URL").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = urlValue
            dir.Options.Add(opt)
            dir.OutputAs = RenderOutputAs(dir)
            ezdb.Directives.Add(dir)

        End Sub

        Private Shared Sub DoFind(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

            Dim text As String = StripDirective(line)
            Dim dir As New Domain.EzProxyDatabaseDirective
            Dim opt As New Domain.EzProxyDatabaseDirectiveOption

            dir.EzProxyDatabase = ezdb
            dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = "Find & Replace").Single()
            dir.EzProxyDatabaseDirectiveId = dir.EzProxyDirective.EzProxyDirectiveId
            dir.IsActive = True

            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "findstring" And w.EzProxyDirective.Name = "Find & Replace").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = text.Trim()
            dir.Options.Add(opt)
            ezdb.Directives.Add(dir)

        End Sub

        Private Shared Sub DoReplace(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

            Dim text As String = StripDirective(line)
            Dim dir As Domain.EzProxyDatabaseDirective
            Dim opt As New Domain.EzProxyDatabaseDirectiveOption

            dir = ezdb.Directives.Last()
            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "replacestring" And w.EzProxyDirective.Name = "Find & Replace").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = text.Trim()
            dir.Options.Add(opt)
            dir.OutputAs = RenderOutputAs(dir)

        End Sub

        Private Shared Sub DoDomain(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

            Dim text As String = StripDirective(line)
            Dim dir As New Domain.EzProxyDatabaseDirective
            Dim opt As New Domain.EzProxyDatabaseDirectiveOption

            dir.EzProxyDatabase = ezdb
            dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = "Domain").Single()
            dir.IsActive = True

            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "wilddomain" And w.EzProxyDirective.Name = "Domain").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = text.Trim()
            dir.Options.Add(opt)
            dir.OutputAs = RenderOutputAs(dir)
            ezdb.Directives.Add(dir)

        End Sub

        Private Shared Sub DoHttpHeader(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

            Dim text As String = StripDirective(line)
            Dim dir As New Domain.EzProxyDatabaseDirective
            Dim opt As New Domain.EzProxyDatabaseDirectiveOption

            dir.EzProxyDatabase = ezdb
            dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = "HTTPHeader").Single()
            dir.IsActive = True

            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "wildheader" And w.EzProxyDirective.Name = "HTTPHeader").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = text.Trim()
            dir.Options.Add(opt)
            dir.OutputAs = RenderOutputAs(dir)
            ezdb.Directives.Add(dir)

        End Sub


        Private Shared Sub DoProxyHostnameEdit(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

            Dim text As String = StripDirective(line)
            Dim dir As New Domain.EzProxyDatabaseDirective
            Dim opt As Domain.EzProxyDatabaseDirectiveOption

            dir.EzProxyDatabase = ezdb
            dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = "ProxyHostnameEdit").Single()
            dir.IsActive = True

            Dim split As String() = text.Split(" ".ToCharArray())

            opt = New Domain.EzProxyDatabaseDirectiveOption()
            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "find" And w.EzProxyDirective.Name = "ProxyHostnameEdit").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = split(0).Trim()
            dir.Options.Add(opt)

            opt = New Domain.EzProxyDatabaseDirectiveOption()
            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "replace" And w.EzProxyDirective.Name = "ProxyHostnameEdit").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = split(1).Trim()
            dir.Options.Add(opt)

            dir.OutputAs = RenderOutputAs(dir)
            ezdb.Directives.Add(dir)

        End Sub

        Private Shared Sub DoDomainJavascript(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

            Dim text As String = StripDirective(line)
            Dim dir As New Domain.EzProxyDatabaseDirective
            Dim opt As New Domain.EzProxyDatabaseDirectiveOption

            dir.EzProxyDatabase = ezdb
            dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = "Domain Javascript").Single()
            dir.IsActive = True

            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "wilddomain" And w.EzProxyDirective.Name = "Domain Javascript").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = text.Trim()
            dir.Options.Add(opt)
            dir.OutputAs = RenderOutputAs(dir)
            ezdb.Directives.Add(dir)

        End Sub

        Private Shared Sub DoHost(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

            Dim text As String = StripDirective(line)
            Dim dir As New Domain.EzProxyDatabaseDirective
            Dim opt As New Domain.EzProxyDatabaseDirectiveOption

            dir.EzProxyDatabase = ezdb
            dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = "Host").Single()
            dir.IsActive = True

            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "host" And w.EzProxyDirective.Name = "Host").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = text.Trim()
            dir.Options.Add(opt)
            dir.OutputAs = RenderOutputAs(dir)
            ezdb.Directives.Add(dir)

        End Sub

        Private Shared Sub DoBooks(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

            Dim text As String = StripDirective(line)
            Dim dir As New Domain.EzProxyDatabaseDirective
            Dim opt As New Domain.EzProxyDatabaseDirectiveOption

            dir.EzProxyDatabase = ezdb
            dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = "Books24x7Site").Single()
            dir.IsActive = True

            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "site" And w.EzProxyDirective.Name = "Books24x7Site").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = text.Trim()
            dir.Options.Add(opt)
            dir.OutputAs = RenderOutputAs(dir)
            ezdb.Directives.Add(dir)

        End Sub

        Private Shared Sub DoTokenKey(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

            Dim text As String = StripDirective(line)
            Dim dir As New Domain.EzProxyDatabaseDirective
            Dim opt As New Domain.EzProxyDatabaseDirectiveOption

            dir.EzProxyDatabase = ezdb
            dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = "TokenKey").Single()
            dir.IsActive = True

            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "key" And w.EzProxyDirective.Name = "TokenKey").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = text.Trim()
            dir.Options.Add(opt)
            dir.OutputAs = RenderOutputAs(dir)
            ezdb.Directives.Add(dir)

        End Sub

        Private Shared Sub DoTokenSigKey(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

            Dim text As String = StripDirective(line)
            Dim dir As New Domain.EzProxyDatabaseDirective
            Dim opt As New Domain.EzProxyDatabaseDirectiveOption

            dir.EzProxyDatabase = ezdb
            dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = "TokenSignatureKey").Single()
            dir.IsActive = True

            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "signkey" And w.EzProxyDirective.Name = "TokenSignatureKey").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = text.Trim()
            dir.Options.Add(opt)
            dir.OutputAs = RenderOutputAs(dir)
            ezdb.Directives.Add(dir)

        End Sub

        Private Shared Sub DoFormVariable(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

            Dim text As String = StripDirective(line)
            Dim dir As New Domain.EzProxyDatabaseDirective
            Dim opt As New Domain.EzProxyDatabaseDirectiveOption

            dir.EzProxyDatabase = ezdb
            dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = "FormVariable").Single()
            dir.IsActive = True

            Dim split As String() = text.Split("=".ToCharArray())

            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "name" And w.EzProxyDirective.Name = "FormVariable").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = split(0).Trim()
            dir.Options.Add(opt)

            opt = New Domain.EzProxyDatabaseDirectiveOption
            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "value" And w.EzProxyDirective.Name = "FormVariable").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = split(1).Trim()
            dir.Options.Add(opt)

            dir.OutputAs = RenderOutputAs(dir)
            ezdb.Directives.Add(dir)

        End Sub

        Private Shared Sub DoHostJavascript(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

            Dim text As String = StripDirective(line)
            Dim dir As New Domain.EzProxyDatabaseDirective
            Dim opt As New Domain.EzProxyDatabaseDirectiveOption

            dir.EzProxyDatabase = ezdb
            dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = "Host Javascript").Single()
            dir.IsActive = True

            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "host" And w.EzProxyDirective.Name = "Host Javascript").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = text.Trim()
            dir.Options.Add(opt)
            dir.OutputAs = RenderOutputAs(dir)
            ezdb.Directives.Add(dir)

        End Sub

        Private Shared Sub DoOptions(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

            Dim text As String = StripDirective(line)
            Dim dir As New Domain.EzProxyDatabaseDirective
            Dim opt As New Domain.EzProxyDatabaseDirectiveOption

            dir.EzProxyDatabase = ezdb
            dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = line.Trim()).Single()
            dir.IsActive = True
            dir.OutputAs = RenderOutputAs(dir)
            ezdb.Directives.Add(dir)

        End Sub

        Private Shared Sub DoAnonymousUrl(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

            Dim text As String = StripDirective(line)
            Dim dir As New Domain.EzProxyDatabaseDirective
            Dim opt As New Domain.EzProxyDatabaseDirectiveOption

            dir.EzProxyDatabase = ezdb
            dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = "AnonymousURL").Single()
            dir.IsActive = True

            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "wildurl" And w.EzProxyDirective.Name = "AnonymousURL").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = text.Trim()
            dir.Options.Add(opt)
            dir.OutputAs = RenderOutputAs(dir)
            ezdb.Directives.Add(dir)

        End Sub

        'Cookie BROWSER_SUPPORTS_COOKIES=1; domain=.sciencedirect.com
        Private Shared Sub DoCookie(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

            Dim text As String = StripDirective(line)
            Dim dir As New Domain.EzProxyDatabaseDirective
            Dim opt As Domain.EzProxyDatabaseDirectiveOption

            Dim split1 As String() = text.Split(";".ToCharArray())
            Dim split2 As String() = split1(0).Split("=".ToCharArray())
            Dim split3 As String() = split1(1).Split("=".ToCharArray())

            dir.EzProxyDatabase = ezdb
            dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = "Cookie").Single()
            dir.IsActive = True

            opt = New Domain.EzProxyDatabaseDirectiveOption()
            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "name" And w.EzProxyDirective.Name = "Cookie").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = split2(0).Trim()
            dir.Options.Add(opt)

            opt = New Domain.EzProxyDatabaseDirectiveOption()
            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "value" And w.EzProxyDirective.Name = "Cookie").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = split2(1).Trim()
            dir.Options.Add(opt)

            opt = New Domain.EzProxyDatabaseDirectiveOption()
            opt.EzProxyOption = dc.EzProxyOptions.Where(Function(w) w.Name = "domain" And w.EzProxyDirective.Name = "Cookie").Single()
            opt.EzProxyDatabaseDirective = dir
            opt.IsActive = True
            opt.OptionValue = split3(1).Trim()
            dir.Options.Add(opt)

            dir.OutputAs = RenderOutputAs(dir)
            ezdb.Directives.Add(dir)

        End Sub

        Private Shared Function StripDirective(line As String) As String

            Dim i As Integer = line.IndexOf(" ")
            Return line.Substring(i + 1).Trim()

        End Function

        Private Shared Function RenderOutputAs(dir As EzProxyDatabaseDirective) As String
            Dim sb As New StringBuilder()

            sb.Append(dir.EzProxyDirective.OutputAs)

            Dim v = From o In dir.Options Order By o.EzProxyOption.OutputOrder

            For Each opt As EzProxyDatabaseDirectiveOption In v
                sb.Append(String.Format(opt.EzProxyOption.OutputAs, opt.OptionValue))
            Next

            sb.Replace("&nbsp;", " ")
            sb.Replace("<br/>", vbCrLf)

            Return sb.ToString()
        End Function

        Public Shared Function ExtractDirective(directiveString As String, ezProxyDatabaseId As Integer) As Dto.DirectiveExtract
            Dim split As String()
            Dim dir As String
            Dim dirValue As String
            Dim ezdb As New Domain.EzProxyDatabase()
            Dim extObj As New Dto.DirectiveExtract()
            Dim errors As List(Of String) = New List(Of String)()
            Dim dtoDatabase As Dto.EzProxyDatabaseDto = GetDatabase(ezProxyDatabaseId)
            Using dc As New DataAccess.OclsConnectionManagerDataContext()
                Dim records As String() = directiveString.Split(vbCrLf.ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries)
                Dim lineNum As Integer = 0
                Try
                    For Each line As String In records
                        lineNum += 1
                        line = Trim(line)
                        If String.IsNullOrEmpty(line) Then
                            errors.Add(String.Format("line {0}  is empty", lineNum))
                            Continue For
                        End If
                        split = line.Split(CChar(" "))
                        If split.Length < 2 Then
                            errors.Add(String.Format("line {0}:  Name-{1} does not have value", lineNum, line))
                            Continue For
                        End If
                        dir = Trim(UCase(split(0)))
                        dirValue = Trim(split(1))
                        If String.IsNullOrEmpty(dir) Or String.IsNullOrEmpty(dirValue) Then
                            errors.Add(String.Format("line {0}:  Name-{1} or Value-{2} is empty", lineNum, dir, dirValue))
                            Continue For
                        End If
                        Select Case dir
                            Case "ANONYMOUSURL"
                                DoAnonymousUrl(line, dc, ezdb)
                            Case "COOKIE"
                                DoCookie(line, dc, ezdb)
                            Case "T", "TITLE"
                                line = line.Substring(0, EzDatabaseFieldMaxLength)
                                DoTitle(line, dc, ezdb)
                            Case "U", "URL"
                                line = line.Substring(0, EzDatabaseFieldMaxLength)
                                DoUrl(line, dc, ezdb)
                            Case "D", "DOMAIN"
                                line = line.Substring(0, EzDatabaseFieldMaxLength)
                                DoDomain(line, dc, ezdb)
                            Case "DJ", "DOMAINJAVASCRIPT"
                                DoDomainJavascript(line, dc, ezdb)
                            Case "HJ", "HOSTJAVASCRIPT"
                                DoHostJavascript(line, dc, ezdb)
                            Case "HTTPHEADER"
                                DoHttpHeader(line, dc, ezdb)
                            Case "PROXYHOSTNAMEEDIT"
                                DoProxyHostnameEdit(line, dc, ezdb)
                            Case "OPTION"
                                DoOptions(line, dc, ezdb)
                            Case "H", "HOST"
                                DoHost(line, dc, ezdb)
                            Case "BOOKS24X7SITE"
                                DoBooks(line, dc, ezdb)
                            Case "TOKENKEY"
                                DoTokenKey(line, dc, ezdb)
                            Case "TOKENSIGNATUREKEY"
                                DoTokenSigKey(line, dc, ezdb)
                            Case "FIND"
                                DoFind(line, dc, ezdb)
                            Case "REPLACE"
                                DoReplace(line, dc, ezdb)
                            Case "FORMVARIABLE"
                                DoFormVariable(line, dc, ezdb)
                            Case Else
                                errors.Add(String.Format("line {0} Directive {1} is invalid", lineNum, dir))
                        End Select
                    Next

                    Dim dtDirList As List(Of Dto.EzProxyDatabaseDirectiveDto) = dtoDatabase.Directives.ToList()
                    For Each domDir As Domain.EzProxyDatabaseDirective In ezdb.Directives
                        If domDir.EzProxyDirectiveId = 0 Then
                            domDir.EzProxyDirectiveId = domDir.EzProxyDirective.EzProxyDirectiveId
                        End If
                        If domDir.EzProxyDatabaseId = 0 And dtoDatabase.EzProxyDatabaseId <> 0 Then
                            domDir.EzProxyDatabaseId = dtoDatabase.EzProxyDatabaseId
                        End If
                        dtDirList.Add(Dto.EzProxyDatabaseDirectiveDto.GetDto(domDir))
                    Next

                    dtoDatabase.Directives = dtDirList
                    If String.IsNullOrEmpty(dtoDatabase.Name) Then
                        Dim dto As Dto.EzProxyDatabaseDirectiveDto = dtDirList.Where(Function(w) w.Name = "Title").FirstOrDefault()
                        If Not IsNothing(dto) Then
                            Dim opt As Dto.EzProxyDatabaseDirectiveOptionDto = dto.Options.Where(Function(w) w.Name = "text").Single()
                            If Not IsNothing(opt) Then
                                dtoDatabase.Name = opt.OptionValue
                            End If
                        End If
                    End If
                    If String.IsNullOrEmpty(dtoDatabase.Title) Then
                        dtoDatabase.Title = dtoDatabase.Name
                    End If

                    If String.IsNullOrEmpty(dtoDatabase.Url) Then
                        Dim dto As Dto.EzProxyDatabaseDirectiveDto = dtDirList.Where(Function(w) w.Name = "URL").FirstOrDefault()

                        If Not IsNothing(dto) Then
                            Dim opt As Dto.EzProxyDatabaseDirectiveOptionDto = dto.Options.Where(Function(w) w.Name = "url").Single()
                            If Not IsNothing(opt) Then
                                dtoDatabase.Url = opt.OptionValue
                            End If
                        End If
                    End If

                    If String.IsNullOrEmpty(dtoDatabase.DomainName) Then
                        Dim dto As Dto.EzProxyDatabaseDirectiveDto = dtDirList.Where(Function(w) w.Name = "Domain").FirstOrDefault()
                        If Not IsNothing(dto) Then
                            Dim opt As Dto.EzProxyDatabaseDirectiveOptionDto = dto.Options.Where(Function(w) w.Name = "wilddomain").Single()
                            If Not IsNothing(opt) Then
                                dtoDatabase.DomainName = opt.OptionValue
                            End If
                        End If
                    End If

                    extObj.EzProxyDatabase = dtoDatabase
                    extObj.Errors = errors
                    extObj.IsSuccessful = True
                Catch ex As Exception
                    extObj.IsSuccessful = False
                End Try
            End Using
            Return extObj
        End Function
    End Class


End Namespace