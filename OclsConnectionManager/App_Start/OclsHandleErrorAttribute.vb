Imports log4net

Public Class OclsHandleErrorAttribute
    Inherits System.Web.Mvc.HandleErrorAttribute

    Private Shared _Logger As ILog = LogManager.GetLogger(GetType(OclsHandleErrorAttribute))

    Public Overrides Sub OnException(filterContext As System.Web.Mvc.ExceptionContext)

        If filterContext.ExceptionHandled OrElse Not filterContext.HttpContext.IsCustomErrorEnabled Then
            Return
        End If

        If New HttpException(Nothing, filterContext.Exception).GetHttpCode() <> 500 Then
            Return
        End If

        If Not ExceptionType.IsInstanceOfType(filterContext.Exception) Then
            Return
        End If

        Dim controllerName As String = CType(filterContext.RouteData.Values("controller"), String)
        Dim actionName As String = CType(filterContext.RouteData.Values("action"), String)
        Dim model As HandleErrorInfo = New HandleErrorInfo(filterContext.Exception, controllerName, actionName)

        _Logger.Error(String.Format("Controller={0}, Action={1}, Message={2}", controllerName, actionName, filterContext.Exception.Message), filterContext.Exception)

        ' If the request is AJAX return JSON else view.
        If filterContext.HttpContext.Request.Headers("X-Requested-With") = "XMLHttpRequest" Then
            filterContext.Result = New JsonResult With {.JsonRequestBehavior = JsonRequestBehavior.AllowGet, .Data = New With {.error = True, .message = filterContext.Exception.Message}}
        Else
            filterContext.Result = New ViewResult With {.ViewName = View, .MasterName = Master, .ViewData = New ViewDataDictionary(Of HandleErrorInfo)(model), .TempData = filterContext.Controller.TempData}
        End If

        filterContext.ExceptionHandled = True
        filterContext.HttpContext.Response.Clear()
        filterContext.HttpContext.Response.StatusCode = 500
        filterContext.HttpContext.Response.TrySkipIisCustomErrors = True

    End Sub

End Class
