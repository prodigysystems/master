Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc
Imports System.Web.Routing

Public Class RouteConfig
    Public Shared Sub RegisterRoutes(ByVal routes As RouteCollection)
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}")

        routes.MapRoute( _
            name:="Default", _
            url:="{controller}/{action}/{id}", _
            defaults:=New With {.controller = "Account", .action = "Login", .id = UrlParameter.Optional} _
        )
    End Sub

    Public Shared Function GetActionUri(ByVal controller As String, ByVal action As String) As String
        Dim uri As String = IIf(Not IsNothing(HttpContext.Current.Request), GetAbsoluteAppPath(HttpContext.Current.Request, True), "")
        uri += String.Format("{0}/{1}", controller, action)
        Return uri
    End Function


    Private Shared Function GetAbsoluteAppPath(ByVal req As HttpRequest, ByVal preseveSchema As Boolean)
        Dim rec As String = req.ApplicationPath
        If String.IsNullOrEmpty(rec) Then
            rec = "/"
        End If

        If VirtualPathUtility.IsAbsolute(rec) = False Then
            rec = VirtualPathUtility.ToAbsolute(rec)
        End If
        If rec.EndsWith("/") = False Then
            rec += "/"
        End If
        Return rec
    End Function
End Class