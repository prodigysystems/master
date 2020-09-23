' Note: For instructions on enabling IIS6 or IIS7 classic mode, 
' visit http://go.microsoft.com/?LinkId=9394802
Imports System.Web.Http
Imports System.Web.Optimization
Imports log4net

Public Class MvcApplication
    Inherits System.Web.HttpApplication

    Private Shared _Logger As ILog = LogManager.GetLogger(GetType(MvcApplication))

    Sub Application_Start()
        AreaRegistration.RegisterAllAreas()
        WebApiConfig.Register(GlobalConfiguration.Configuration)
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters)
        RouteConfig.RegisterRoutes(RouteTable.Routes)
        BundleConfig.RegisterBundles(BundleTable.Bundles)
        AuthConfig.RegisterAuth()
        log4net.Config.XmlConfigurator.Configure()
        System.Web.Helpers.AntiForgeryConfig.SuppressIdentityHeuristicChecks = True
    End Sub

    Sub Session_Start()

        FormsAuthentication.SignOut()

    End Sub

    Sub Application_Error(sender As Object, e As EventArgs)
        Dim ex = Server.GetLastError()
        _Logger.Error(ex)
        Server.ClearError()
    End Sub

End Class
