Imports System.Collections
Imports System.Collections.Generic
Imports OclsConnectionManager.Services
Imports OclsConnectionManager.Domain

Public Class HomeController
    Inherits BaseController

    Function Index() As ActionResult
        ViewData("Message") = "Modify this template to jump-start your ASP.NET MVC application."
        Return View()
    End Function

    Function About() As ActionResult
        ViewData("Message") = "Your app description page."

        Return View()
    End Function

    Function Contact() As ActionResult
        Dim line As String
        Dim split As String()
        Dim dir As String
        Dim ezdb As New Domain.EzProxyDatabase()

        Using dc As New DataAccess.OclsConnectionManagerDataContext()
            Using sr As New System.IO.StreamReader("D:\Dev\OCLS\configClean.txt")
                While Not sr.EndOfStream
                    line = Trim(sr.ReadLine())
                    If Not String.IsNullOrEmpty(line) Then
                        split = line.Split(" ")
                        dir = UCase(split(0))
                        Select Case dir
                            Case "ANONYMOUSURL"
                                DoAnonymousUrl(line, dc, ezdb)
                            Case "COOKIE"
                                DoCookie(line, dc, ezdb)
                            Case "T", "TITLE"
                                DoTitle(line, dc, ezdb)
                            Case "U", "URL"
                                DoUrl(line, dc, ezdb)
                            Case "D", "DOMAIN"
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
                        End Select
                    Else
                        If Not ezdb Is Nothing AndAlso Not String.IsNullOrEmpty(ezdb.Name) Then
                            ezdb.IsActive = True
                            Dim outputOrder As Integer = 1
                            For Each dd As EzProxyDatabaseDirective In ezdb.Directives
                                dd.OutputOrder = outputOrder
                                outputOrder = outputOrder + 1
                            Next
                            dc.EzProxyDatabases.Add(ezdb)
                            dc.SaveChanges()
                        End If
                        ezdb = New Domain.EzProxyDatabase()
                    End If
                End While
            End Using
        End Using

        Return View()
    End Function

    Private Sub DoTitle(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

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

    Private Sub DoUrl(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

        Dim text As String = StripDirective(line)
        Dim dir As New Domain.EzProxyDatabaseDirective
        Dim opt As Domain.EzProxyDatabaseDirectiveOption
        Dim urlValue As String

        dir.EzProxyDatabase = ezdb
        dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = "URL").Single()
        dir.IsActive = True

        Dim split As String() = text.Split(" ")

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

    Private Sub DoFind(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

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

    Private Sub DoReplace(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

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

    Private Sub DoDomain(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

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

    Private Sub DoHttpHeader(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

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


    Private Sub DoProxyHostnameEdit(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

        Dim text As String = StripDirective(line)
        Dim dir As New Domain.EzProxyDatabaseDirective
        Dim opt As Domain.EzProxyDatabaseDirectiveOption

        dir.EzProxyDatabase = ezdb
        dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = "ProxyHostnameEdit").Single()
        dir.IsActive = True

        Dim split As String() = text.Split(" ")

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

    Private Sub DoDomainJavascript(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

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

    Private Sub DoHost(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

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

    Private Sub DoBooks(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

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

    Private Sub DoTokenKey(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

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

    Private Sub DoTokenSigKey(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

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

    Private Sub DoFormVariable(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

        Dim text As String = StripDirective(line)
        Dim dir As New Domain.EzProxyDatabaseDirective
        Dim opt As New Domain.EzProxyDatabaseDirectiveOption

        dir.EzProxyDatabase = ezdb
        dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = "FormVariable").Single()
        dir.IsActive = True

        Dim split As String() = text.Split("=")

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

    Private Sub DoHostJavascript(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

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

    Private Sub DoOptions(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

        Dim text As String = StripDirective(line)
        Dim dir As New Domain.EzProxyDatabaseDirective
        Dim opt As New Domain.EzProxyDatabaseDirectiveOption

        dir.EzProxyDatabase = ezdb
        dir.EzProxyDirective = dc.EzProxyDirectives.Where(Function(w) w.Name = line.Trim()).Single()
        dir.IsActive = True
        dir.OutputAs = RenderOutputAs(dir)
        ezdb.Directives.Add(dir)

    End Sub

    Private Sub DoAnonymousUrl(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

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
    Private Sub DoCookie(line As String, dc As DataAccess.OclsConnectionManagerDataContext, ezdb As Domain.EzProxyDatabase)

        Dim text As String = StripDirective(line)
        Dim dir As New Domain.EzProxyDatabaseDirective
        Dim opt As Domain.EzProxyDatabaseDirectiveOption

        Dim split1 As String() = text.Split(";")
        Dim split2 As String() = split1(0).Split("=")
        Dim split3 As String() = split1(1).Split("=")

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

    Private Function StripDirective(line As String) As String

        Dim i As Integer = line.IndexOf(" ")
        Return line.Substring(i + 1).Trim()

    End Function

    Private Function RenderOutputAs(dir As EzProxyDatabaseDirective)
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

End Class
