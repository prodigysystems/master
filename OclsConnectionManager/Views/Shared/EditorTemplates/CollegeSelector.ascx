﻿<%@ Control Language="VB" Inherits="System.Web.Mvc.ViewUserControl(Of OclsConnectionManager.IpAddressListViewModel)" %>
<%: Html.Telerik().DropDownList().Name("CollegeName").BindTo(New SelectList(CType(ViewData("AvailableColleges"), IEnumerable(Of SelectListItem)), "Value", "Text")) %>