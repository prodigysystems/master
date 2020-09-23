Imports System.Collections
Imports System.Collections.Generic
Imports OclsConnectionManager.Dto
Imports System.ComponentModel.DataAnnotations
Public Class IpAddressListViewModel
    Public Property IpAddresses() As IEnumerable(Of CollegeIpAddressDto)
    Public Property CollegeName
    Public Property CollegeId
    Public Property AvailableColleges() As IEnumerable(Of CollegeDto)
    Public Sub New()
        IpAddresses = New List(Of CollegeIpAddressDto)()
        AvailableColleges = New List(Of CollegeDto)()

    End Sub
End Class
