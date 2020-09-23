Imports System.Web
Imports System.Web.Mvc

Public Class FilterConfig
    Public Shared Sub RegisterGlobalFilters(ByVal filters As GlobalFilterCollection)
        filters.Add(New OclsHandleErrorAttribute With {.ExceptionType = GetType(System.Exception), .View = "Error"})

    End Sub
End Class