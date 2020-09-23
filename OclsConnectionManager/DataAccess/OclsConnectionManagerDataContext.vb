Option Explicit On
Option Strict On

Imports System
Imports System.Data.Entity
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports OclsConnectionManager.Domain

Namespace DataAccess

    Public Class OclsConnectionManagerDataContext
        Inherits DbContext

        Public Sub New()
            MyBase.New("OclsConnectionManagerConnectionString")
            Entity.Database.SetInitializer(Of OclsConnectionManagerDataContext)(Nothing)
            MyBase.Configuration.AutoDetectChangesEnabled = False
            Me.Configuration.LazyLoadingEnabled = True
        End Sub

        Protected Overrides Sub OnModelCreating(modelBuilder As System.Data.Entity.DbModelBuilder)
            MyBase.OnModelCreating(modelBuilder)
            modelBuilder.Configurations.Add(New AuditLogMap())
            modelBuilder.Configurations.Add(New CollegeMap())
            modelBuilder.Configurations.Add(New ConnectionMethodMap())
            modelBuilder.Configurations.Add(New EzProxyConfigMap())
            modelBuilder.Configurations.Add(New EzProxyServerMap())
            modelBuilder.Configurations.Add(New EzProxyDirectiveMap())
            modelBuilder.Configurations.Add(New EzProxyOptionMap())
            modelBuilder.Configurations.Add(New EzProxyDatabaseMap())
            modelBuilder.Configurations.Add(New EzProxyDatabaseDirectiveMap())
            modelBuilder.Configurations.Add(New EzProxyDatabaseDirectiveOptionMap())
            modelBuilder.Configurations.Add(New UserProfileMap())
            modelBuilder.Configurations.Add(New RemoteAuthenticationMap())
            modelBuilder.Configurations.Add(New CollegeIpAddressMap())
        End Sub

        Public Property AuditLogs As DbSet(Of AuditLog)
        Public Property Colleges As DbSet(Of College)
        Public Property ConnectionMethods As DbSet(Of ConnectionMethod)
        Public Property EzProxyConfigs As DbSet(Of EzProxyConfig)
        Public Property EzProxyServers As DbSet(Of EzProxyServer)
        Public Property EzProxyDirectives As DbSet(Of EzProxyDirective)
        Public Property EzProxyOptions As DbSet(Of EzProxyOption)
        Public Property EzProxyDatabases As DbSet(Of EzProxyDatabase)
        Public Property Users As DbSet(Of UserProfile)
        Public Property RemoteAuthentications As DbSet(Of RemoteAuthentication)
        Public Property CollegeIpAddresses As DbSet(Of CollegeIpAddress)
        Public Property EzProxyDatabaseDirectives As DbSet(Of EzProxyDatabaseDirective)

        Public Sub AddAuditLog(ByVal user As UserProfile, ByVal eventCode As AuditLog.EventCodes, ByVal keyId As Nullable(Of Integer), ByVal detail As String)

            Dim auditLog As New AuditLog()

            With auditLog
                .User = user
                .EventCode = eventCode.ToString()
                .KeyId = keyId
                .EventDate = DateTime.Now
                .EventDetail = detail
            End With

            Me.AuditLogs.Add(auditLog)

        End Sub

    End Class

End Namespace
