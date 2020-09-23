<%@ Control Language="VB" Inherits="System.Web.Mvc.ViewUserControl(Of OclsConnectionManager.RegisterModel)" %>
<%: Html.Telerik().DropDownList().Name("Role").BindTo(New SelectList(CType(ViewData("roles"), IEnumerable(Of SelectListItem)), "Value", "Text")) %>
