Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports OclsConnectionManager.Services
Imports OclsConnectionManager.Dto


<TestClass()> Public Class UnitTest1

    <TestMethod()> Public Sub TestMethod1()
        Dim str As String = "ANONYMOUSURL aaa"
        str += vbCrLf
        str += "TITLE Test"
        str += vbCrLf
        str += "URL http://test.com"
        str += vbCrLf
        str += "Domain testDomain"
     
        Dim extract As DirectiveExtract = EzProxyService.ExtractDirective(str, 22)

    End Sub

End Class