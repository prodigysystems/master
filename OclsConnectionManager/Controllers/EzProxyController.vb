Imports System.Configuration
Imports System.IO

Imports System.Net
Imports Telerik.Web.Mvc

Public Class EzProxyController
    Inherits BaseController

    Function Index() As ActionResult

        If Not Session.Item("CurrentDirectives") Is Nothing Then
            Session.Remove("CurrentDirectives")
        End If

        Dim model As New EzProxyDatabaseListViewModel()

        model.AllDirectives = Services.EzProxyService.GetAllDirectives()

        Dim ServerListForEdit As List(Of SelectListItem) = New List(Of SelectListItem)
        Dim ServerListForSave As List(Of SelectListItem) = New List(Of SelectListItem)
        Dim servers As IEnumerable(Of Dto.EzProxyServerDto) = Services.EzProxyService.GetAllServers()

        For Each s In servers
            ServerListForEdit.Add(New SelectListItem With {.Text = s.Name, .Value = s.EzProxyServerId.ToString()})
            ServerListForSave.Add(New SelectListItem With {.Text = s.Name, .Value = s.EzProxyServerId.ToString()})
        Next

        ServerListForEdit.Insert(0, New SelectListItem With {.Text = "- All servers -", .Value = "0"})
        ServerListForSave.Insert(0, New SelectListItem With {.Text = "- Select one EZproxy server -", .Value = "0"})
        ViewData("AvailableServersForEdit") = ServerListForEdit
        ViewData("AvailableServersForSave") = ServerListForSave

        If Not IsNothing(model.AllDirectives) Then
            model.AllDirectives.Insert(0, New Dto.EzProxyDirectiveDto() With {.EzProxyDirectiveId = 0, .Name = String.Empty})
        End If

        ViewData("AllDirectives") = New SelectList(model.AllDirectives, "EzProxyDirectiveId", "Name")
        Return (View(model))

    End Function

    <GridAction()>
    Function SearchDatabases(ezproxyServerId As Integer, Optional databaseName As String = "", Optional urlSearchTerm As String = "", Optional domainName As String = "", Optional keywordName As String = "") As ActionResult
        Dim databases As IEnumerable(Of Dto.EzProxyDatabaseDto) = Services.EzProxyService.GetDatabases(ezproxyServerId, databaseName, urlSearchTerm, domainName, keywordName)
        Return View(New GridModel(databases))
    End Function

    <HttpPost()>
    Function AddDatabase(database As Dto.EzProxyDatabaseDto) As JsonResult
        Dim ret As Boolean = False
        If ModelState.IsValid Then

            If Not IsNothing(database) Then
                If Not IsNothing(database.Directives) Then
                    database.Directives = New List(Of Dto.EzProxyDatabaseDirectiveDto)
                End If
                ret = InsertDatabase(database)
            End If

        End If
        Return Json(ret)
    End Function

    <HttpPost()>
    Function EditDatabase(database As Dto.EzProxyDatabaseDto) As JsonResult
        If ModelState.IsValid Then
            If Not IsNothing(database) Then
                If Not IsNothing(database.Directives) Then
                    database.Directives = New List(Of Dto.EzProxyDatabaseDirectiveDto)
                End If
                UpdateDatabase(database)
            End If
        Else
            ''Dim errors As List(Of String) = ModelState.SelectMany(Function(d) d.Value.Errors.Where(Function(er) er.ErrorMessage)).ToList()
            Dim errorMessages As String = String.Join("; ", ModelState.Values.SelectMany(Function(x) x.Errors).Select(Function(x) x.ErrorMessage))
        End If
        Return Json(New With {.value = True})
    End Function

    Function InsertDatabase(database As Dto.EzProxyDatabaseDto) As Boolean
        Dim ret As Boolean = False

        If ModelState.IsValid Then

            Dim allDirectives As List(Of Dto.EzProxyDatabaseDirectiveDto) = Nothing

            If Not Session.Item("CurrentDirectives") Is Nothing Then
                allDirectives = CType(Session.Item("CurrentDirectives"), List(Of Dto.EzProxyDatabaseDirectiveDto))
            End If

            If IsNothing(allDirectives) Then
                allDirectives = New List(Of Dto.EzProxyDatabaseDirectiveDto)
            End If

            If Not IsNothing(database) Then
                database.Directives = allDirectives
            End If

            ret = Services.EzProxyService.CreateDatabase(database, User.Identity.Name)

            If Not IsNothing(Session.Item("CurrentDirectives")) AndAlso ret Then
                Session.Remove("CurrentDirectives")
            End If

        End If

        Return ret
    End Function

    <GridAction()>
    Function UpdateDatabase(database As Dto.EzProxyDatabaseDto) As ActionResult

        If ModelState.IsValid Then

            Dim allDirectives As List(Of Dto.EzProxyDatabaseDirectiveDto) = Nothing

            If Not Session.Item("CurrentDirectives") Is Nothing Then
                allDirectives = CType(Session.Item("CurrentDirectives"), List(Of Dto.EzProxyDatabaseDirectiveDto))
            End If

            If Not database Is Nothing Then

                If Not allDirectives Is Nothing Then
                    database.Directives = allDirectives
                End If

            End If

            database = Services.EzProxyService.UpdateDatabase(database, User.Identity.Name)

            If Not IsNothing(Session.Item("CurrentDirectives")) AndAlso Not IsNothing(database) Then
                Session.Remove("CurrentDirectives")
            End If

        End If
        Return View(New GridModel(Services.EzProxyService.GetDatabases(0, "")))
    End Function

    <GridAction()>
    Function DeleteDatabase(id As Integer) As ActionResult

        Dim database As Dto.EzProxyDatabaseDto = Services.EzProxyService.GetDatabase(id)

        If Not database Is Nothing Then
            If Services.EzProxyService.DeleteDatabase(database, User.Identity.Name) Then

                If Not Session.Item("CurrentDirectives") Is Nothing Then
                    Session.Remove("CurrentDirectives")
                End If

            End If
        End If

        Return View(New GridModel(Services.EzProxyService.GetDatabases(0, "")))
    End Function

    <HttpPost()>
    Function DeleteDatabaseCustom(id As Integer) As JsonResult

        Dim database As Dto.EzProxyDatabaseDto = Services.EzProxyService.GetDatabase(id)
        Dim ret As Boolean = False
        If Not database Is Nothing Then
            If Services.EzProxyService.DeleteDatabase(database, User.Identity.Name) Then

                If Not Session.Item("CurrentDirectives") Is Nothing Then
                    Session.Remove("CurrentDirectives")
                End If
                ret = True
            End If
        End If

        Return Json(ret)
    End Function

    <GridAction()>
    Function GetDatabase(ezProxyDatabaseId As Integer) As ActionResult

        Dim database As Dto.EzProxyDatabaseDto = Services.EzProxyService.GetDatabase(ezProxyDatabaseId)
        Return View(New GridModel(database.Directives))

    End Function

    <GridAction()>
    Function GetDatabaseDirectives(ezProxyDatabaseId As Integer) As ActionResult

        Dim database As Dto.EzProxyDatabaseDto = Services.EzProxyService.GetDatabase(ezProxyDatabaseId)

        If Not database Is Nothing Then

            If database.EzProxyDatabaseId > 0 Then

                Dim allDirectives As New List(Of Dto.EzProxyDatabaseDirectiveDto)

                If Not Session.Item("CurrentDirectives") Is Nothing Then
                    allDirectives = CType(Session.Item("CurrentDirectives"), List(Of Dto.EzProxyDatabaseDirectiveDto))
                    If Not allDirectives Is Nothing Then
                        If allDirectives.Where(Function(d) d.EzProxyDatabaseId = database.EzProxyDatabaseId).Count() = 0 Then
                            Session.Remove("CurrentDirectives")
                        End If
                    Else
                        allDirectives = database.Directives
                    End If
                Else
                    allDirectives = database.Directives
                End If

                Session.Item("CurrentDirectives") = allDirectives

                database.Directives = CType(Session.Item("CurrentDirectives"), List(Of Dto.EzProxyDatabaseDirectiveDto))

            Else
                If Not Session.Item("CurrentDirectives") Is Nothing Then
                    database.Directives = CType(Session.Item("CurrentDirectives"), List(Of Dto.EzProxyDatabaseDirectiveDto))
                End If
            End If
        End If

        Return View(New GridModel(database.Directives))

    End Function

    <GridAction()>
    Function InsertDatabaseDirective(ezProxyDatabaseDirectiveDto As OclsConnectionManager.Dto.EzProxyDatabaseDirectiveDto, EzProxyDatabaseId As Integer, EzProxyDirectiveId As Integer) As ActionResult

        Return GetDatabaseDirectives(EzProxyDatabaseId)
    End Function

    <GridAction()>
    Function UpdateDatabaseDirective(ezProxyDatabaseDirectiveDto As OclsConnectionManager.Dto.EzProxyDatabaseDirectiveDto, EzProxyDatabaseId As Integer, EzProxyDirectiveId As Integer) As ActionResult

        Return GetDatabaseDirectives(EzProxyDatabaseId)
    End Function


    <HttpPost()>
    Function DeleteDatabaseDirective(DatabaseId As Integer, DirectiveId As Integer, DatabaseDirectiveId As Integer, Optional directiveName As String = "") As JsonResult

        Dim ezProxyDatabaseDirectiveId As Integer = DatabaseDirectiveId

        Dim allDirectives As New List(Of Dto.EzProxyDatabaseDirectiveDto)

        If Not Session.Item("CurrentDirectives") Is Nothing Then
            allDirectives = CType(Session.Item("CurrentDirectives"), List(Of Dto.EzProxyDatabaseDirectiveDto))
        End If

        Dim ezProxyDatabaseDirectiveDto As Dto.EzProxyDatabaseDirectiveDto = Nothing

        Dim IsSuccessFul As Boolean = False

        If ezProxyDatabaseDirectiveId > 0 Then
            ezProxyDatabaseDirectiveDto = Services.EzProxyService.GetDatabaseDirective(ezProxyDatabaseDirectiveId)

            If Not IsNothing(ezProxyDatabaseDirectiveDto) Then

                IsSuccessFul = Services.EzProxyService.DeleteDatabaseDirective(ezProxyDatabaseDirectiveId)

                If Not IsNothing(allDirectives) Then
                    ezProxyDatabaseDirectiveDto = allDirectives.Where(Function(d) d.EzProxyDatabaseDirectiveId = ezProxyDatabaseDirectiveId).FirstOrDefault()

                    If Not IsNothing(ezProxyDatabaseDirectiveDto) Then
                        allDirectives.Remove(ezProxyDatabaseDirectiveDto)
                    End If

                End If
            End If
        Else
            If Not IsNothing(allDirectives) Then

                ezProxyDatabaseDirectiveDto = allDirectives.Where(Function(d) Not String.IsNullOrEmpty(d.Name) AndAlso d.Name = directiveName AndAlso _
                                                                      d.EzProxyDirectiveId = DirectiveId AndAlso d.EzProxyDatabaseDirectiveId = ezProxyDatabaseDirectiveId).FirstOrDefault()

                If Not IsNothing(ezProxyDatabaseDirectiveDto) Then
                    IsSuccessFul = allDirectives.Remove(ezProxyDatabaseDirectiveDto)
                End If
            End If

        End If

        If Not IsNothing(allDirectives) Then
            Session.Item("CurrentDirectives") = allDirectives
        End If

        Return Json(New With {.value = IsSuccessFul, .message = "Successfully deleted."})

    End Function

    <GridAction()>
    Function GetDatabaseDirectiveOptions(ezProxyDatabaseId As Integer, ezProxyDirectiveId As Integer, ezProxyDatabaseDirectiveId As Integer) As ActionResult

        Dim eEzProxyDatabaseDirectiveDto As Dto.EzProxyDatabaseDirectiveDto = Services.EzProxyService.GetDatabaseDirective(ezProxyDatabaseDirectiveId, ezProxyDatabaseId, ezProxyDirectiveId)

        ''If ezProxyDatabaseId = 0 And ezProxyDirectiveId > 0 Then

        If ezProxyDirectiveId > 0 Then

            Dim allDirectives As New List(Of Dto.EzProxyDatabaseDirectiveDto)

            If Not Session.Item("CurrentDirectives") Is Nothing Then
                allDirectives = CType(Session.Item("CurrentDirectives"), List(Of Dto.EzProxyDatabaseDirectiveDto))
            End If

            If Not IsNothing(allDirectives) Then

                Dim directive As Dto.EzProxyDirectiveDto = Services.EzProxyService.GetDirective(ezProxyDirectiveId)

                If Not IsNothing(directive) Then

                    Dim currentDirective As Dto.EzProxyDatabaseDirectiveDto = allDirectives.Where(Function(d) Not String.IsNullOrEmpty(d.Name) AndAlso d.Name.ToLower().Trim() = directive.Name.ToLower().Trim() AndAlso d.EzProxyDatabaseDirectiveId = ezProxyDatabaseDirectiveId).FirstOrDefault()

                    If Not IsNothing(currentDirective) AndAlso Not IsNothing(currentDirective.Options) AndAlso Not IsNothing(eEzProxyDatabaseDirectiveDto) AndAlso Not IsNothing(eEzProxyDatabaseDirectiveDto.Options) Then

                        For Each opt As Dto.EzProxyDatabaseDirectiveOptionDto In eEzProxyDatabaseDirectiveDto.Options

                            Dim currentOption As Dto.EzProxyDatabaseDirectiveOptionDto = currentDirective.Options.Where(Function(d) d.EzProxyDirectiveId = opt.EzProxyDirectiveId And d.EzProxyOptionId = opt.EzProxyOptionId).FirstOrDefault()

                            If Not IsNothing(currentOption) Then

                                If Not String.IsNullOrEmpty(currentOption.OptionValue) Then
                                    opt.OptionValue = currentOption.OptionValue.Trim()
                                End If

                                opt.IsActive = currentOption.IsActive

                            End If

                        Next
                    End If
                End If

            End If
        End If

        Return View(New GridModel(eEzProxyDatabaseDirectiveDto.Options))

    End Function

    <GridAction()>
    Function InsertDatabaseDirectiveOption(ezProxyDatabaseDirectiveId As Integer, EzProxyDatabaseId As Integer, EzProxyDirectiveId As Integer) As ActionResult

        Dim eEzProxyDatabaseDirectiveDto As Dto.EzProxyDatabaseDirectiveDto = Services.EzProxyService.GetDatabaseDirective(ezProxyDatabaseDirectiveId, EzProxyDatabaseId, EzProxyDirectiveId)
        Return View(New GridModel(eEzProxyDatabaseDirectiveDto.Options))
    End Function

    <GridAction()>
    Function UpdateDatabaseDirectiveOption(ezProxyDatabaseDirectiveId As Integer, EzProxyDatabaseId As Integer, EzProxyDirectiveId As Integer) As ActionResult
        Dim eEzProxyDatabaseDirectiveDto As Dto.EzProxyDatabaseDirectiveDto = Services.EzProxyService.GetDatabaseDirective(ezProxyDatabaseDirectiveId, EzProxyDatabaseId, EzProxyDirectiveId)
        Return View(New GridModel(eEzProxyDatabaseDirectiveDto.Options))
    End Function

    <GridAction()>
    Function DeleteDatabaseDirectiveOption(ezProxyDatabaseDirectiveId As Integer, EzProxyDatabaseId As Integer, EzProxyDirectiveId As Integer) As ActionResult

        Dim eEzProxyDatabaseDirectiveDto As Dto.EzProxyDatabaseDirectiveDto = Services.EzProxyService.GetDatabaseDirective(ezProxyDatabaseDirectiveId, EzProxyDatabaseId, EzProxyDirectiveId)
        Return View(New GridModel(eEzProxyDatabaseDirectiveDto.Options))
    End Function

    <HttpPost()>
    Public Function AddDatabaseDirectiveOption(ezProxyDatabaseDirectiveDto As Dto.EzProxyDatabaseDirectiveDto) As JsonResult

        Dim isValidated As Boolean = False

        If Not IsNothing(ezProxyDatabaseDirectiveDto) AndAlso ezProxyDatabaseDirectiveDto.EzProxyDirectiveId > 0 Then

            Dim strOutputAsValue As String = String.Empty

            If Not ezProxyDatabaseDirectiveDto.Options Is Nothing Then

                For Each opt As Dto.EzProxyDatabaseDirectiveOptionDto In ezProxyDatabaseDirectiveDto.Options

                    If opt.HasInputValue Then

                        If opt.IsRequired Then

                            If Not String.IsNullOrEmpty(opt.OptionValue) Then

                                isValidated = True

                                If Not String.IsNullOrEmpty(opt.OutputAs) Then

                                    If opt.OutputAs.Contains("{0}") Then
                                        strOutputAsValue += String.Format(opt.OutputAs, opt.OptionValue.Trim())
                                    Else
                                        strOutputAsValue += opt.OutputAs + " " + opt.OptionValue.Trim()
                                    End If
                                End If

                            Else
                                isValidated = False
                                Exit For
                            End If
                        Else
                            If opt.IsActive Then

                                If Not String.IsNullOrEmpty(opt.OptionValue) Then

                                    If Not String.IsNullOrEmpty(opt.OutputAs) Then

                                        isValidated = True

                                        If opt.OutputAs.Contains("{0}") Then
                                            strOutputAsValue += String.Format(opt.OutputAs, opt.OptionValue.Trim())
                                        Else
                                            strOutputAsValue += opt.OutputAs + " " + opt.OptionValue.Trim()
                                        End If

                                    End If

                                End If

                            End If

                        End If
                    Else
                        If opt.IsActive Then
                            isValidated = True
                            strOutputAsValue += opt.OutputAs
                        End If
                    End If

                Next

                ezProxyDatabaseDirectiveDto.OutputAs = strOutputAsValue.Replace("&nbsp;", " ").Replace("<br/>", vbCrLf)

            Else
                isValidated = True
            End If

            If isValidated = False Then
                Return Json(New With {.value = isValidated})
            Else
                Dim allDirectives As List(Of Dto.EzProxyDatabaseDirectiveDto) = Nothing

                If Not Session.Item("CurrentDirectives") Is Nothing Then
                    allDirectives = CType(Session.Item("CurrentDirectives"), List(Of Dto.EzProxyDatabaseDirectiveDto))
                End If

                If allDirectives Is Nothing Then
                    allDirectives = New List(Of Dto.EzProxyDatabaseDirectiveDto)
                End If

                Dim directive As Domain.EzProxyDirective = Services.EzProxyService.GetDirectiveByName(ezProxyDatabaseDirectiveDto.Name)
                If Not IsNothing(directive) Then
                    ezProxyDatabaseDirectiveDto.OutputAs = (directive.OutputAs & strOutputAsValue).Replace("&nbsp;", " ").Replace("<br/>", vbCrLf)
                End If

                Dim oldEzProxyDatabaseDirectiveDto As Dto.EzProxyDatabaseDirectiveDto = Nothing
                If ezProxyDatabaseDirectiveDto.EzProxyDatabaseDirectiveId > 0 Then
                    oldEzProxyDatabaseDirectiveDto = allDirectives.Where(Function(d) d.EzProxyDatabaseDirectiveId = ezProxyDatabaseDirectiveDto.EzProxyDatabaseDirectiveId).FirstOrDefault()
                Else

                    oldEzProxyDatabaseDirectiveDto = allDirectives.Where(Function(d) d.EzProxyDirectiveId = ezProxyDatabaseDirectiveDto.EzProxyDirectiveId And _
                                                        d.EzProxyDatabaseId = ezProxyDatabaseDirectiveDto.EzProxyDatabaseId And d.EzProxyDatabaseDirectiveId = ezProxyDatabaseDirectiveDto.EzProxyDatabaseDirectiveId).FirstOrDefault()
                End If

                If Not oldEzProxyDatabaseDirectiveDto Is Nothing Then

                    Dim idx As Integer = allDirectives.IndexOf(oldEzProxyDatabaseDirectiveDto)

                    If (ezProxyDatabaseDirectiveDto.EzProxyDatabaseId > 0) Then
                        ezProxyDatabaseDirectiveDto = Services.EzProxyService.UpdateDatabaseDirective(ezProxyDatabaseDirectiveDto)
                    End If

                    If Not IsNothing(ezProxyDatabaseDirectiveDto) And idx > -1 Then

                        If Not IsNothing(ezProxyDatabaseDirectiveDto) AndAlso (ezProxyDatabaseDirectiveDto.EzProxyDatabaseId > 0) Then
                            UpdateDatabaseFields(ezProxyDatabaseDirectiveDto, allDirectives)
                        End If

                        Dim updatedDirective As Dto.EzProxyDatabaseDirectiveDto = ezProxyDatabaseDirectiveDto
                        allDirectives.RemoveAt(idx)

                        allDirectives.Insert(idx, updatedDirective)

                    End If
                Else

                    If ezProxyDatabaseDirectiveDto.EzProxyDatabaseId > 0 Then
                        ezProxyDatabaseDirectiveDto = Services.EzProxyService.UpdateDatabaseDirective(ezProxyDatabaseDirectiveDto)
                        If Not IsNothing(ezProxyDatabaseDirectiveDto) Then
                            UpdateDatabaseFields(ezProxyDatabaseDirectiveDto, allDirectives)
                        End If
                    End If

                    If Not ezProxyDatabaseDirectiveDto Is Nothing Then

                        If ezProxyDatabaseDirectiveDto.EzProxyDatabaseId = 0 AndAlso ezProxyDatabaseDirectiveDto.EzProxyDatabaseDirectiveId = 0 Then
                            ezProxyDatabaseDirectiveDto.EzProxyDatabaseDirectiveId = GetNewTemporaryEzProxyDatabaseDirectiveId(allDirectives)
                        End If

                        If ezProxyDatabaseDirectiveDto.OutputOrder = 0 Then
                            ezProxyDatabaseDirectiveDto.OutputOrder = GetNewTemporaryEzProxyDatabaseDirectiveOutputOrder(allDirectives)
                        End If

                        allDirectives.Add(ezProxyDatabaseDirectiveDto)
                    End If

                End If

                Session.Item("CurrentDirectives") = allDirectives

            End If

        End If

        Return Json(New With {.value = isValidated})

    End Function

    <HttpPost()>
    Public Function EditDatabaseDirectiveOption(ezProxyDatabaseDirectiveDto As Dto.EzProxyDatabaseDirectiveDto) As JsonResult

        Dim isValidated As Boolean = False

        If Not IsNothing(ezProxyDatabaseDirectiveDto) AndAlso ezProxyDatabaseDirectiveDto.EzProxyDirectiveId > 0 Then

            Dim strOutputAsValue As String = String.Empty

            If Not ezProxyDatabaseDirectiveDto.Options Is Nothing Then

                For Each opt As Dto.EzProxyDatabaseDirectiveOptionDto In ezProxyDatabaseDirectiveDto.Options

                    If opt.HasInputValue Then

                        If opt.IsRequired Then

                            If Not String.IsNullOrEmpty(opt.OptionValue) Then

                                isValidated = True

                                If Not String.IsNullOrEmpty(opt.OutputAs) Then

                                    If opt.OutputAs.Contains("{0}") Then
                                        strOutputAsValue += String.Format(opt.OutputAs, opt.OptionValue.Trim())
                                    Else
                                        strOutputAsValue += opt.OutputAs + " " + opt.OptionValue.Trim()
                                    End If
                                End If
                            Else
                                isValidated = False
                                Exit For
                            End If
                        Else
                            If opt.IsActive Then

                                If Not String.IsNullOrEmpty(opt.OptionValue) Then

                                    If Not String.IsNullOrEmpty(opt.OutputAs) Then

                                        isValidated = True

                                        If opt.OutputAs.Contains("{0}") Then
                                            strOutputAsValue += String.Format(opt.OutputAs, opt.OptionValue.Trim())
                                        Else
                                            strOutputAsValue += opt.OutputAs + " " + opt.OptionValue.Trim()
                                        End If

                                    End If
                                End If

                            End If

                        End If
                    Else
                        If opt.IsActive Then
                            isValidated = True
                            strOutputAsValue += opt.OutputAs
                        End If
                    End If

                Next

                ezProxyDatabaseDirectiveDto.OutputAs = strOutputAsValue.Replace("&nbsp;", " ").Replace("<br/>", vbCrLf)
            Else
                isValidated = True
            End If


            If isValidated = False Then

                Return Json(New With {.value = isValidated})

            Else

                Dim allDirectives As List(Of Dto.EzProxyDatabaseDirectiveDto) = Nothing

                If Not Session.Item("CurrentDirectives") Is Nothing Then
                    allDirectives = CType(Session.Item("CurrentDirectives"), List(Of Dto.EzProxyDatabaseDirectiveDto))
                End If

                If allDirectives Is Nothing Then
                    allDirectives = New List(Of Dto.EzProxyDatabaseDirectiveDto)
                End If

                'Dim directive As Domain.EzProxyDirective = Services.EzProxyService.GetDirectiveByName(ezProxyDatabaseDirectiveDto.Name)
                Dim directive As Dto.EzProxyDirectiveDto = Services.EzProxyService.GetDirective(ezProxyDatabaseDirectiveDto.EzProxyDirectiveId)
                If Not IsNothing(directive) Then
                    ezProxyDatabaseDirectiveDto.OutputAs = (directive.OutputAs & strOutputAsValue).Replace("&nbsp;", " ").Replace("<br/>", vbCrLf)
                End If

                Dim oldEzProxyDatabaseDirectiveDto As Dto.EzProxyDatabaseDirectiveDto = Nothing
                If ezProxyDatabaseDirectiveDto.EzProxyDatabaseDirectiveId > 0 Then
                    oldEzProxyDatabaseDirectiveDto = allDirectives.Where(Function(d) d.EzProxyDatabaseDirectiveId = ezProxyDatabaseDirectiveDto.EzProxyDatabaseDirectiveId).FirstOrDefault()
                Else
                    oldEzProxyDatabaseDirectiveDto = allDirectives.Where(Function(d) d.EzProxyDirectiveId = ezProxyDatabaseDirectiveDto.EzProxyDirectiveId And _
                                                        d.EzProxyDatabaseId = ezProxyDatabaseDirectiveDto.EzProxyDatabaseId And d.EzProxyDatabaseDirectiveId = ezProxyDatabaseDirectiveDto.EzProxyDatabaseDirectiveId).FirstOrDefault()
                End If


                If Not oldEzProxyDatabaseDirectiveDto Is Nothing Then

                    Dim idx As Integer = allDirectives.IndexOf(oldEzProxyDatabaseDirectiveDto)

                    If ezProxyDatabaseDirectiveDto.EzProxyDatabaseId > 0 Then
                        ezProxyDatabaseDirectiveDto = Services.EzProxyService.UpdateDatabaseDirective(ezProxyDatabaseDirectiveDto)
                    End If

                    If Not IsNothing(ezProxyDatabaseDirectiveDto) And idx > -1 Then

                        If Not IsNothing(ezProxyDatabaseDirectiveDto) AndAlso ezProxyDatabaseDirectiveDto.EzProxyDatabaseId > 0 Then
                            UpdateDatabaseFields(ezProxyDatabaseDirectiveDto, allDirectives)
                        End If

                        Dim updatedDirective As Dto.EzProxyDatabaseDirectiveDto = ezProxyDatabaseDirectiveDto
                        allDirectives.RemoveAt(idx)

                        allDirectives.Insert(idx, updatedDirective)

                    End If

                Else

                    If ezProxyDatabaseDirectiveDto.EzProxyDatabaseId > 0 Then
                        ezProxyDatabaseDirectiveDto = Services.EzProxyService.UpdateDatabaseDirective(ezProxyDatabaseDirectiveDto)
                        If Not IsNothing(ezProxyDatabaseDirectiveDto) Then
                            UpdateDatabaseFields(ezProxyDatabaseDirectiveDto, allDirectives)
                        End If
                    End If

                    If Not ezProxyDatabaseDirectiveDto Is Nothing Then
                        If ezProxyDatabaseDirectiveDto.EzProxyDatabaseId = 0 AndAlso ezProxyDatabaseDirectiveDto.EzProxyDatabaseDirectiveId = 0 Then
                            ezProxyDatabaseDirectiveDto.EzProxyDatabaseDirectiveId = GetNewTemporaryEzProxyDatabaseDirectiveId(allDirectives)
                        End If

                        If ezProxyDatabaseDirectiveDto.OutputOrder = 0 Then
                            ezProxyDatabaseDirectiveDto.OutputOrder = GetNewTemporaryEzProxyDatabaseDirectiveOutputOrder(allDirectives)
                        End If

                        allDirectives.Add(ezProxyDatabaseDirectiveDto)
                    End If
                End If

                Session.Item("CurrentDirectives") = allDirectives

            End If

        End If

        Return Json(New With {.value = isValidated})

    End Function

    Public Function ShowDatabasePreview(databaseId As Integer) As ActionResult

        Dim result As String = GenerateDatabasePreview(User.Identity.Name).ToString()

        Return File(System.Text.Encoding.UTF8.GetBytes(result), "application/text")
    End Function

    Public Function ShowPreview(ezProxyServerId As Integer) As ActionResult

        Dim result As String = Services.EzProxyService.PreviewEzProxyConfig(User.Identity.Name, ezProxyServerId)
        Return File(System.Text.Encoding.UTF8.GetBytes(result), "application/text")

    End Function

    Public Function SaveConfiguration(ezProxyServerId As Integer) As ActionResult

        Dim server As Dto.EzProxyServerDto = Services.EzProxyService.GetAllServers().FirstOrDefault(Function(s) s.EzProxyServerId = ezProxyServerId)
        Dim share As String = ConfigurationManager.AppSettings("EZproxyConfigShare")
        Dim result As String = String.Empty
        Dim credentials As NetworkCredential
        Dim networkConnection As NetworkConnection = Nothing
        Dim filename As String

        If server Is Nothing Then
            Throw New ApplicationException("Invalid ezProxyServerId " & ezProxyServerId & " provided.")
        End If

        Try
            result = Services.EzProxyService.SaveEzProxyConfig(User.Identity.Name, ezProxyServerId)

            If String.IsNullOrEmpty(server.FileShare) Then
                Throw New ApplicationException("FileShare has not been specified.")
            End If

            If String.IsNullOrEmpty(server.FileName) Then
                Throw New ApplicationException("FileName has not been specified.")
            End If

            If String.IsNullOrEmpty(server.FileShareUsername) Then
                Throw New ApplicationException("FileShareUsername has not been specified.")
            End If

            If String.IsNullOrEmpty(server.FileSharePassword) Then
                Throw New ApplicationException("FileSharePassword has not been specified.")
            End If

            If String.IsNullOrEmpty(server.FileShareDomain) Then
                Throw New ApplicationException("FileShareDomain has not been specified.")
            End If

            filename = Path.Combine(server.FileShare, server.FileName)
            credentials = New NetworkCredential(server.FileShareUsername, server.FileSharePassword, server.FileShareDomain)
            networkConnection = New NetworkConnection(server.FileShare, credentials)
            System.IO.File.WriteAllText(filename, result)
        Finally
            If Not networkConnection Is Nothing Then
                networkConnection.Dispose()
            End If
        End Try

        Return File(System.Text.Encoding.UTF8.GetBytes(result), "application/text")

    End Function

    Public Function ImportDatabaseDirectives(DatabaseDirectivesFile As IEnumerable(Of HttpPostedFileBase), Optional ezProxyDatabaseId As Integer = 0) As ActionResult

        Dim oEzProxyDatabaseDto As Dto.EzProxyDatabaseDto = Services.EzProxyService.GetDatabase(ezProxyDatabaseId)
        Dim extract As Dto.DirectiveExtract = Nothing

        If Not DatabaseDirectivesFile Is Nothing AndAlso DatabaseDirectivesFile.Count > 0 Then

            For Each directiveFile As HttpPostedFileBase In DatabaseDirectivesFile

                Dim configFileName As String = Path.GetFileName(directiveFile.FileName)
                Dim configFileUploadPath As String = Server.MapPath(FileUploadPath)

                If Not System.IO.Directory.Exists(configFileUploadPath) Then
                    System.IO.Directory.CreateDirectory(configFileUploadPath)
                End If

                directiveFile.SaveAs(configFileUploadPath & Path.DirectorySeparatorChar & configFileName)

                If System.IO.File.Exists(configFileUploadPath & Path.DirectorySeparatorChar & configFileName) Then
                    Try
                        ''''Use fileContents string to process all directives for the database
                        Dim fileContents As String = System.IO.File.ReadAllText(configFileUploadPath & Path.DirectorySeparatorChar & configFileName)
                        Dim i As Integer = 0
                        extract = Services.EzProxyService.ExtractDirective(fileContents, ezProxyDatabaseId)

                        If Not IsNothing(extract) AndAlso Not IsNothing(extract.EzProxyDatabase) AndAlso Not IsNothing(extract.EzProxyDatabase.Directives) Then

                            ''''Check if there any directives in user's current Session
                            If Not IsNothing(Session.Item("CurrentDirectives")) Then

                                Dim existingDirectives As List(Of Dto.EzProxyDatabaseDirectiveDto) = CType(Session.Item("CurrentDirectives"), List(Of Dto.EzProxyDatabaseDirectiveDto))
                                If Not IsNothing(existingDirectives) Then

                                    ''''Select all previously saved directive Ids
                                    Dim existingDirectiveIds As Integer() = existingDirectives.Where(Function(t) t.EzProxyDatabaseDirectiveId > 0).Select(Function(ex) ex.EzProxyDatabaseDirectiveId).ToArray()

                                    ''''Select only newly added (imported) directives
                                    Dim newlyAddedDirectives As List(Of Dto.EzProxyDatabaseDirectiveDto) = (From dir In extract.EzProxyDatabase.Directives
                                                                                                            Where Not existingDirectiveIds.Contains(dir.EzProxyDatabaseDirectiveId)
                                                                                                            Select dir).ToList()

                                    ''''Update all EzProxyDatabaseDirectiveId for newly added directives
                                    Dim maxMinusNumber As Integer = 0
                                    If existingDirectives.Select(Function(d) d.EzProxyDatabaseDirectiveId).Count() > 0 Then
                                        maxMinusNumber = existingDirectives.Select(Function(d) d.EzProxyDatabaseDirectiveId).Min()
                                    End If

                                    If (maxMinusNumber < 0) Then
                                        For Each newDir As Dto.EzProxyDatabaseDirectiveDto In newlyAddedDirectives
                                            maxMinusNumber = Math.Abs(maxMinusNumber) + 10
                                            newDir.EzProxyDatabaseDirectiveId = -(maxMinusNumber)
                                        Next
                                    End If

                                    ''''Add new directives to list of all directives
                                    existingDirectives.AddRange(newlyAddedDirectives)

                                    ''''Update list of directives in current session
                                    Session.Item("CurrentDirectives") = existingDirectives
                                Else
                                    Session.Item("CurrentDirectives") = extract.EzProxyDatabase.Directives.ToList()
                                End If
                            Else
                                Session.Item("CurrentDirectives") = extract.EzProxyDatabase.Directives.ToList()
                            End If

                            ''''Update OutputOrder for the directives, Specified parameter is nothing as It will get the list of directives from Session.
                            UpdateEzProxyDatabaseDirectiveOutputOrders(Nothing)

                            ''''Reset database fields
                            extract.EzProxyDatabase.Url = GetDefaultDirectiveValue("url", "url")
                            extract.EzProxyDatabase.Title = GetDefaultDirectiveValue("title", "text")
                            extract.EzProxyDatabase.DomainName = GetDefaultDirectiveValue("domain", "wilddomain")

                            If Not IsNothing(Session.Item("CurrentDirectives")) Then
                                extract.EzProxyDatabase.Directives = CType(Session.Item("CurrentDirectives"), List(Of Dto.EzProxyDatabaseDirectiveDto))
                            End If

                        End If

                    Catch ex As Exception

                    Finally

                    End Try
                End If

            Next
        End If

        Return Json(New With {.value = extract.IsSuccessful, .EzProxyDatabase = extract.EzProxyDatabase, .DirectiveExtraction = extract})

    End Function

    Public ReadOnly Property FileUploadPath As String
        Get
            Return ConfigurationManager.AppSettings("UploadFilePath")
        End Get
    End Property

    Public Function RemoveDatabaseDirectives(DatabaseDirectivesFile As IEnumerable(Of HttpPostedFileBase)) As ActionResult

        Return Json(New With {.value = True, .message = "File successfully removed."})

    End Function

    <HttpPost()>
    Public Function ClearPreviousSession() As JsonResult
        If Not Session.Item("CurrentDirectives") Is Nothing Then
            Session.Remove("CurrentDirectives")
        End If
        Return Json(New With {.value = True})

    End Function

    <HttpPost()>
    Public Function UpdateDirective(ezProxyDatabaseDirectiveDto As Dto.EzProxyDatabaseDirectiveDto) As JsonResult

        Dim success As Boolean = False
        Dim directives As New List(Of Dto.EzProxyDatabaseDirectiveDto)

        If Not IsNothing(ezProxyDatabaseDirectiveDto) Then

            If Not String.IsNullOrEmpty(ezProxyDatabaseDirectiveDto.Name) Then

                success = True

                If IsNothing(Session.Item("CurrentDirectives")) Then
                    Session.Item("CurrentDirectives") = New List(Of Dto.EzProxyDatabaseDirectiveDto)
                End If

                If Not Session.Item("CurrentDirectives") Is Nothing Then

                    directives = ResetDirective(ezProxyDatabaseDirectiveDto, CType(Session.Item("CurrentDirectives"), List(Of Dto.EzProxyDatabaseDirectiveDto)))
                    Session.Item("CurrentDirectives") = directives

                End If

            End If

        End If
        Return Json(New With {.value = success, .directives = directives})

    End Function


    Private Function ResetDirective(ezProxyDatabaseDirectiveDto As Dto.EzProxyDatabaseDirectiveDto, ByRef directives As List(Of Dto.EzProxyDatabaseDirectiveDto)) As List(Of Dto.EzProxyDatabaseDirectiveDto)

        If IsNothing(directives) Then
            Return directives
        Else
            If IsNothing(ezProxyDatabaseDirectiveDto) Or String.IsNullOrEmpty(ezProxyDatabaseDirectiveDto.Name) Then
                Return directives
            Else

                Dim urlDir As Dto.EzProxyDatabaseDirectiveDto = directives.Where(Function(d) Not String.IsNullOrEmpty(d.Name) AndAlso d.Name.ToLower().Trim() = ezProxyDatabaseDirectiveDto.Name.ToLower().Trim()).FirstOrDefault()

                If Not String.IsNullOrEmpty(ezProxyDatabaseDirectiveDto.OutputAs) Then

                    Dim directive As Domain.EzProxyDirective = Services.EzProxyService.GetDirectiveByName(ezProxyDatabaseDirectiveDto.Name)

                    Dim directiveId As Integer = 0

                    Dim directiveValue As String = String.Empty

                    directiveValue = ezProxyDatabaseDirectiveDto.OutputAs.Trim()

                    If Not IsNothing(directive) Then
                        directiveId = directive.EzProxyDirectiveId
                        ezProxyDatabaseDirectiveDto.EzProxyDirectiveId = directiveId

                        If ezProxyDatabaseDirectiveDto.EzProxyDatabaseId = 0 AndAlso ezProxyDatabaseDirectiveDto.EzProxyDatabaseDirectiveId = 0 Then
                            ezProxyDatabaseDirectiveDto.EzProxyDatabaseDirectiveId = GetNewTemporaryEzProxyDatabaseDirectiveId(directives)
                        End If

                        If ezProxyDatabaseDirectiveDto.OutputOrder = 0 Then
                            ezProxyDatabaseDirectiveDto.OutputOrder = GetNewTemporaryEzProxyDatabaseDirectiveOutputOrder(directives)
                        End If
                    End If

                    Dim requiredOptionForUrlDirective As Dto.EzProxyOptionDto = Nothing

                    Select Case ezProxyDatabaseDirectiveDto.Name.ToLower().Trim()

                        Case "url"
                            requiredOptionForUrlDirective = Services.EzProxyService.GetDirectiveOption(directiveId, "url")

                        Case "domain"
                            requiredOptionForUrlDirective = Services.EzProxyService.GetDirectiveOption(directiveId, "wilddomain")

                        Case "title"
                            requiredOptionForUrlDirective = Services.EzProxyService.GetDirectiveOption(directiveId, "text")

                    End Select

                    Dim ezProxyDatabaseDirectiveOptionDto As Dto.EzProxyDatabaseDirectiveOptionDto = Nothing

                    If Not IsNothing(requiredOptionForUrlDirective) Then

                        ezProxyDatabaseDirectiveOptionDto = New Dto.EzProxyDatabaseDirectiveOptionDto
                        ezProxyDatabaseDirectiveOptionDto.EzProxyDirectiveId = directiveId
                        ezProxyDatabaseDirectiveOptionDto.EzProxyOptionId = requiredOptionForUrlDirective.EzProxyOptionId

                        ezProxyDatabaseDirectiveOptionDto.IsActive = True
                        ezProxyDatabaseDirectiveOptionDto.IsRequired = requiredOptionForUrlDirective.IsRequired
                        ezProxyDatabaseDirectiveOptionDto.HasInputValue = requiredOptionForUrlDirective.HasInputValue
                        ezProxyDatabaseDirectiveOptionDto.Name = requiredOptionForUrlDirective.Name
                        ezProxyDatabaseDirectiveOptionDto.OptionType = requiredOptionForUrlDirective.OptionOrQualifier
                        ezProxyDatabaseDirectiveOptionDto.OutputAs = requiredOptionForUrlDirective.OutputAs
                        ezProxyDatabaseDirectiveOptionDto.OptionValue = String.Format(requiredOptionForUrlDirective.OutputAs, directiveValue)

                        If Not String.IsNullOrEmpty(ezProxyDatabaseDirectiveOptionDto.OptionValue) Then
                            ezProxyDatabaseDirectiveOptionDto.OptionValue = ezProxyDatabaseDirectiveOptionDto.OptionValue.Replace("&nbsp;", " ")
                        End If

                    End If

                    Dim ezProxyDatabaseDirectiveOptionDtos As New List(Of Dto.EzProxyDatabaseDirectiveOptionDto)

                    If IsNothing(urlDir) Then
                        urlDir = New Dto.EzProxyDatabaseDirectiveDto() With {.OutputOrder = ezProxyDatabaseDirectiveDto.OutputOrder, .EzProxyDirectiveId = directiveId, .EzProxyDatabaseDirectiveId = ezProxyDatabaseDirectiveDto.EzProxyDatabaseDirectiveId, .EzProxyDatabaseId = ezProxyDatabaseDirectiveDto.EzProxyDatabaseId, .Name = ezProxyDatabaseDirectiveDto.Name, .IsActive = True}

                        ezProxyDatabaseDirectiveOptionDtos.Add(ezProxyDatabaseDirectiveOptionDto)
                        urlDir.Options = ezProxyDatabaseDirectiveOptionDtos
                    Else
                        If Not IsNothing(requiredOptionForUrlDirective) Then
                            ezProxyDatabaseDirectiveOptionDto = urlDir.Options.Where(Function(d) d.EzProxyDirectiveId = directiveId And d.EzProxyOptionId = requiredOptionForUrlDirective.EzProxyOptionId).FirstOrDefault()

                            If Not IsNothing(ezProxyDatabaseDirectiveOptionDto) Then

                                ezProxyDatabaseDirectiveOptionDto.OptionValue = String.Format(requiredOptionForUrlDirective.OutputAs, directiveValue)

                                If Not String.IsNullOrEmpty(ezProxyDatabaseDirectiveOptionDto.OptionValue) Then
                                    ezProxyDatabaseDirectiveOptionDto.OptionValue = ezProxyDatabaseDirectiveOptionDto.OptionValue.Replace("&nbsp;", " ")
                                End If

                            End If

                        End If

                    End If

                    If Not IsNothing(urlDir) Then

                        Dim strOutputAsValue As String = directive.OutputAs

                        For Each opt As Dto.EzProxyDatabaseDirectiveOptionDto In urlDir.Options

                            If opt.HasInputValue Then

                                If opt.IsRequired Then

                                    If Not String.IsNullOrEmpty(opt.OptionValue) Then

                                        If Not String.IsNullOrEmpty(opt.OutputAs) Then

                                            If opt.OutputAs.Contains("{0}") Then
                                                strOutputAsValue += String.Format(opt.OutputAs, opt.OptionValue.Trim())
                                            Else
                                                strOutputAsValue += opt.OutputAs + " " + opt.OptionValue.Trim()
                                            End If
                                        End If

                                    End If
                                Else

                                    If opt.IsActive Then

                                        If Not String.IsNullOrEmpty(opt.OptionValue) Then

                                            If Not String.IsNullOrEmpty(opt.OutputAs) Then

                                                If opt.OutputAs.Contains("{0}") Then
                                                    strOutputAsValue += String.Format(opt.OutputAs, opt.OptionValue.Trim())
                                                Else
                                                    strOutputAsValue += opt.OutputAs + " " + opt.OptionValue.Trim()
                                                End If

                                            End If

                                        End If

                                    End If

                                End If
                            Else
                                If opt.IsActive Then

                                    strOutputAsValue += opt.OutputAs

                                End If
                            End If

                        Next

                        urlDir.OutputAs = strOutputAsValue.Replace("&nbsp;", " ").Replace("<br/>", vbCrLf)

                    End If


                    If Not String.IsNullOrEmpty(directiveValue) Then

                        If Not IsNothing(urlDir) Then

                            Dim idx As Integer = directives.IndexOf(urlDir)

                            Dim newUrlDir As Dto.EzProxyDatabaseDirectiveDto = urlDir
                            newUrlDir.IsActive = True
                            newUrlDir.EzProxyDatabaseDirectiveId = urlDir.EzProxyDatabaseDirectiveId
                            newUrlDir.EzProxyDatabaseId = urlDir.EzProxyDatabaseId
                            newUrlDir.EzProxyDirectiveId = urlDir.EzProxyDirectiveId
                            newUrlDir.Options = urlDir.Options
                            newUrlDir.OutputAs = urlDir.OutputAs

                            If newUrlDir.EzProxyDatabaseId > 0 Then
                                newUrlDir = Services.EzProxyService.UpdateDatabaseDirective(newUrlDir)
                                If Not IsNothing(newUrlDir) Then
                                    UpdateDatabaseFields(newUrlDir, directives)
                                End If

                            End If

                            If idx > -1 Then

                                If Not IsNothing(newUrlDir) Then
                                    directives.RemoveAt(idx)
                                    directives.Insert(idx, newUrlDir)
                                End If
                            Else

                                If Not IsNothing(newUrlDir) Then
                                    directives.Add(newUrlDir)
                                End If

                            End If

                        End If
                    Else
                        If Not IsNothing(urlDir) Then
                            DeleteDatabaseDirective(urlDir.EzProxyDatabaseId, urlDir.EzProxyDirectiveId, urlDir.EzProxyDatabaseDirectiveId, urlDir.Name)
                        End If
                    End If
                Else
                    If Not IsNothing(urlDir) Then
                        DeleteDatabaseDirective(urlDir.EzProxyDatabaseId, urlDir.EzProxyDirectiveId, urlDir.EzProxyDatabaseDirectiveId, urlDir.Name)
                    End If
                End If

            End If
        End If
        Return directives
    End Function


    Private Sub UpdateDatabaseFields(oldEzProxyDatabaseDirectiveDto As Dto.EzProxyDatabaseDirectiveDto, ByRef allDirectives As List(Of Dto.EzProxyDatabaseDirectiveDto))

        Dim defaultDir As Dto.EzProxyDatabaseDirectiveDto = Nothing

        Dim defultDirNames As String() = {"url", "title", "domain"}

        If defultDirNames.Contains(oldEzProxyDatabaseDirectiveDto.Name.ToLower().Trim()) Then

            defaultDir = allDirectives.Where(Function(d) Not String.IsNullOrEmpty(d.Name) AndAlso d.Name.ToLower().Trim() = oldEzProxyDatabaseDirectiveDto.Name.ToLower().Trim()).FirstOrDefault()

            If Not IsNothing(defaultDir) Then

                If defaultDir.EzProxyDatabaseDirectiveId = oldEzProxyDatabaseDirectiveDto.EzProxyDatabaseDirectiveId Then

                    Dim database As Dto.EzProxyDatabaseDto = Services.EzProxyService.GetDatabase(oldEzProxyDatabaseDirectiveDto.EzProxyDatabaseId)

                    If Not IsNothing(database) Then

                        Dim requiredOptionForUrlDirective As Dto.EzProxyOptionDto = Nothing

                        Select Case oldEzProxyDatabaseDirectiveDto.Name.ToLower().Trim()

                            Case "url"
                                requiredOptionForUrlDirective = Services.EzProxyService.GetDirectiveOption(oldEzProxyDatabaseDirectiveDto.EzProxyDirectiveId, "url")

                            Case "domain"
                                requiredOptionForUrlDirective = Services.EzProxyService.GetDirectiveOption(oldEzProxyDatabaseDirectiveDto.EzProxyDirectiveId, "wilddomain")

                            Case "title"
                                requiredOptionForUrlDirective = Services.EzProxyService.GetDirectiveOption(oldEzProxyDatabaseDirectiveDto.EzProxyDirectiveId, "text")

                        End Select

                        Dim optionValueOfDirective As Dto.EzProxyDatabaseDirectiveOptionDto = Nothing

                        If Not IsNothing(requiredOptionForUrlDirective) Then
                            optionValueOfDirective = oldEzProxyDatabaseDirectiveDto.Options.Where(Function(d) Not String.IsNullOrEmpty(d.Name) AndAlso d.Name.ToLower().Trim() = requiredOptionForUrlDirective.Name.ToLower().Trim()).FirstOrDefault()

                            If Not IsNothing(optionValueOfDirective) Then

                                Dim updatedValue As String = optionValueOfDirective.OptionValue

                                If Not String.IsNullOrEmpty(optionValueOfDirective.OptionValue) Then
                                    updatedValue = optionValueOfDirective.OptionValue.Trim()
                                End If

                                Select Case oldEzProxyDatabaseDirectiveDto.Name.ToLower().Trim()

                                    Case "url"
                                        database.Url = updatedValue

                                    Case "domain"
                                        database.DomainName = updatedValue

                                    Case "title"
                                        database.Title = updatedValue

                                End Select

                                Services.EzProxyService.UpdateDatabaseFieldsOnly(database, User.Identity.Name)

                            End If
                        End If

                    End If

                End If

            End If

        End If
    End Sub


    <HttpPost()>
    Public Function GetDatabaseValues(databaseId As Integer) As JsonResult

        Dim isSuccess As Boolean = False

        Dim db As Dto.EzProxyDatabaseDto = Services.EzProxyService.GetDatabase(databaseId)

        If Not IsNothing(db) Then

            isSuccess = True

            db.Url = GetDefaultDirectiveValue("url", "url")
            db.Title = GetDefaultDirectiveValue("title", "text")
            db.DomainName = GetDefaultDirectiveValue("domain", "wilddomain")

        End If

        Return Json(New With {.value = isSuccess, .database = db})

    End Function


    Private Function GetDefaultDirectiveValue(directiveName As String, optionName As String) As String

        Dim defaultValue As String = String.Empty

        If Not String.IsNullOrEmpty(directiveName) AndAlso Not String.IsNullOrEmpty(optionName) Then
            Dim allDirectives As List(Of Dto.EzProxyDatabaseDirectiveDto) = Nothing

            If Not Session.Item("CurrentDirectives") Is Nothing Then
                allDirectives = CType(Session.Item("CurrentDirectives"), List(Of Dto.EzProxyDatabaseDirectiveDto))
            End If

            If allDirectives Is Nothing Then
                allDirectives = New List(Of Dto.EzProxyDatabaseDirectiveDto)
            End If

            Dim defaultDir As Dto.EzProxyDatabaseDirectiveDto = allDirectives.Where(Function(d) Not String.IsNullOrEmpty(d.Name) AndAlso d.Name.ToLower().Trim() = directiveName.ToLower().Trim()).FirstOrDefault()

            If Not IsNothing(defaultDir) AndAlso Not IsNothing(defaultDir.Options) Then

                Dim requiredOption As Dto.EzProxyDatabaseDirectiveOptionDto = defaultDir.Options.Where(Function(d) Not String.IsNullOrEmpty(d.Name) AndAlso d.Name.ToLower().Trim() = optionName.ToLower().Trim()).FirstOrDefault()

                If Not IsNothing(requiredOption) AndAlso Not String.IsNullOrEmpty(requiredOption.OptionValue) Then
                    defaultValue = requiredOption.OptionValue.Trim()
                End If

            End If

        End If

        Return defaultValue
    End Function


    Private Function GenerateDatabasePreview(username As String) As StringBuilder

        Dim result As New StringBuilder()

        If Not Session.Item("CurrentDirectives") Is Nothing Then

            Dim allDirectives As List(Of Dto.EzProxyDatabaseDirectiveDto) = CType(Session.Item("CurrentDirectives"), List(Of Dto.EzProxyDatabaseDirectiveDto))

            If allDirectives Is Nothing Then
                allDirectives = New List(Of Dto.EzProxyDatabaseDirectiveDto)
            End If

            For Each directive As Dto.EzProxyDatabaseDirectiveDto In allDirectives.Where(Function(d) d.IsActive).OrderBy(Function(d) d.OutputOrder)
                result.AppendLine(directive.OutputAs)
            Next

            result.AppendLine()

        End If

        Return result

    End Function


    ''''Function added on May 05, 2014 to allow user to add the same directive more than once

    Private Function GetNewTemporaryEzProxyDatabaseDirectiveId(ByRef allDirectives As List(Of Dto.EzProxyDatabaseDirectiveDto)) As Integer

        ''''Start sequence with -1 
        Dim tempEzProxyDatabaseDirectiveId = -1

        If IsNothing(allDirectives) Then
            If Not Session.Item("CurrentDirectives") Is Nothing Then
                allDirectives = CType(Session.Item("CurrentDirectives"), List(Of Dto.EzProxyDatabaseDirectiveDto))
            End If
        End If

        If Not IsNothing(allDirectives) AndAlso allDirectives.Count() > 0 AndAlso allDirectives.Where(Function(d) d.EzProxyDatabaseId = 0).Count() > 0 Then
            tempEzProxyDatabaseDirectiveId = allDirectives.Where(Function(d) d.EzProxyDatabaseId = 0).Max(Function(d) Math.Abs(d.EzProxyDatabaseDirectiveId))
        End If

        If tempEzProxyDatabaseDirectiveId > 0 Then
            tempEzProxyDatabaseDirectiveId = -(tempEzProxyDatabaseDirectiveId + 1)
        End If

        Return tempEzProxyDatabaseDirectiveId

    End Function

    Private Function GetNewTemporaryEzProxyDatabaseDirectiveOutputOrder(allDirectives As List(Of Dto.EzProxyDatabaseDirectiveDto)) As Integer

        ''''Start sequence with 1 
        Dim tempEzProxyDatabaseDirectiveOutputOrder As Integer? = 0

        If IsNothing(allDirectives) Then
            If Not Session.Item("CurrentDirectives") Is Nothing Then
                allDirectives = CType(Session.Item("CurrentDirectives"), List(Of Dto.EzProxyDatabaseDirectiveDto))
            End If
        End If

        ''If Not IsNothing(allDirectives) AndAlso allDirectives.Count() > 0 AndAlso allDirectives.Where(Function(d) d.OutputOrder = 0).Count() > 0 Then
        If Not IsNothing(allDirectives) AndAlso allDirectives.Count() > 0 Then
            tempEzProxyDatabaseDirectiveOutputOrder = allDirectives.Max(Function(d) d.OutputOrder)
        End If

        If tempEzProxyDatabaseDirectiveOutputOrder = 0 Then
            tempEzProxyDatabaseDirectiveOutputOrder = 1
        Else
            tempEzProxyDatabaseDirectiveOutputOrder = (tempEzProxyDatabaseDirectiveOutputOrder + 10)
        End If

        Return tempEzProxyDatabaseDirectiveOutputOrder
    End Function

    Private Function UpdateEzProxyDatabaseDirectiveOutputOrders(allDirectives As List(Of Dto.EzProxyDatabaseDirectiveDto)) As Integer

        If IsNothing(allDirectives) Then
            If Not Session.Item("CurrentDirectives") Is Nothing Then
                allDirectives = CType(Session.Item("CurrentDirectives"), List(Of Dto.EzProxyDatabaseDirectiveDto))
            End If
        End If

        If Not IsNothing(allDirectives) Then

            For Each dir As Dto.EzProxyDatabaseDirectiveDto In allDirectives.Where(Function(d) d.OutputOrder = 0)
                dir.OutputOrder = GetNewTemporaryEzProxyDatabaseDirectiveOutputOrder(allDirectives)
            Next

        End If

    End Function

End Class
