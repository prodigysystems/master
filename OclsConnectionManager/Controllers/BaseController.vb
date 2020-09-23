Public Class BaseController
    Inherits Controller

    Protected Overrides Sub OnActionExecuting(filterContext As System.Web.Mvc.ActionExecutingContext)
        MyBase.OnActionExecuting(filterContext)
        If filterContext.ActionDescriptor.ActionName <> "Login" AndAlso filterContext.HttpContext.Session.IsNewSession Then
            Dim cookie As String = filterContext.HttpContext.Request.Headers("Cookie")
            Dim timeout As String = String.Empty
            If Not String.IsNullOrEmpty(cookie) AndAlso cookie.IndexOf("OclsConnectionManagerCookie") >= 0 Then
                timeout = "?timeout=yes"
            End If

            If filterContext.HttpContext.Request.IsAjaxRequest() Then
                filterContext.HttpContext.Response.StatusCode = 501
                filterContext.HttpContext.Response.StatusDescription = "expired"
                filterContext.Result = New EmptyResult()
            Else
                filterContext.Result = New RedirectResult(RouteConfig.GetActionUri("Account", "Login") + timeout)
            End If
        End If
    End Sub

End Class
