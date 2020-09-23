Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration
Imports System.Linq
Imports System.Text
Imports OclsConnectionManager.Domain

Namespace DataAccess

    Public Class AuditLogMap
        Inherits EntityTypeConfiguration(Of AuditLog)

        Public Sub New()
            Me.HasKey(Function(t) t.AuditLogId)
            Me.ToTable("AuditLog")
            Me.Property(Function(t) t.AuditLogId).HasColumnName("AuditLogId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
            Me.Property(Function(t) t.UserId).HasColumnName("UserId")
            Me.Property(Function(t) t.EventCode).HasColumnName("EventCode").HasMaxLength(100).IsRequired()
            Me.Property(Function(t) t.KeyId).HasColumnName("KeyId")
            Me.Property(Function(t) t.EventDate).HasColumnName("EventDate").IsRequired()
            Me.Property(Function(t) t.EventDetail).HasColumnName("EventDetail").HasMaxLength(4000).IsRequired()
            Me.HasRequired(Function(t) t.User).WithMany().HasForeignKey(Function(t) t.UserId)
        End Sub

    End Class

    Public Class UserProfileMap
        Inherits EntityTypeConfiguration(Of UserProfile)

        Public Sub New()
            Me.HasKey(Function(t) t.UserId)
            Me.ToTable("UserProfile")
            Me.Property(Function(t) t.UserId).HasColumnName("UserId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
            Me.Property(Function(t) t.Username).HasColumnName("Username").HasMaxLength(56).IsRequired()
        End Sub

    End Class

    Public Class EzProxyConfigMap
        Inherits EntityTypeConfiguration(Of EzProxyConfig)

        Public Sub New()
            Me.HasKey(Function(t) t.EzProxyConfigId)
            Me.ToTable("EzProxyConfig")
            Me.Property(Function(t) t.EzProxyConfigId).HasColumnName("EzProxyConfigId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
            Me.Property(Function(t) t.ConfigContents).HasColumnName("ConfigContents")
            Me.Property(Function(t) t.CreatedByUserId).HasColumnName("CreatedByUserId")
            Me.Property(Function(t) t.CreatedDate).HasColumnName("CreatedDate")
            Me.Property(Function(t) t.EzProxyServerId).HasColumnName("EzProxyServerId").IsRequired()
            Me.HasRequired(Function(t) t.CreatedByUser).WithMany().HasForeignKey(Function(t) t.CreatedByUserId)
            Me.HasRequired(Function(t) t.EzProxyServer).WithMany().HasForeignKey(Function(t) t.EzProxyServerId)
        End Sub

    End Class


    Public Class CollegeMap
        Inherits EntityTypeConfiguration(Of College)

        Public Sub New()
            Me.HasKey(Function(t) t.CollegeId)
            Me.ToTable("College")
            Me.Property(Function(t) t.CollegeId).HasColumnName("CollegeId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
            Me.Property(Function(t) t.Name).HasColumnName("Name").HasMaxLength(200).IsRequired()
        End Sub

    End Class

    Public Class ConnectionMethodMap
        Inherits EntityTypeConfiguration(Of ConnectionMethod)

        Public Sub New()
            Me.HasKey(Function(t) t.ConnectionMethodId)
            Me.ToTable("ConnectionMethod")
            Me.Property(Function(t) t.ConnectionMethodId).HasColumnName("ConnectionMethodId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
            Me.Property(Function(t) t.Name).HasColumnName("Name").HasMaxLength(200).IsRequired()
        End Sub

    End Class

    Public Class EzProxyDatabaseMap
        Inherits EntityTypeConfiguration(Of EzProxyDatabase)

        Public Sub New()
            Me.HasKey(Function(t) t.EzProxyDatabaseId)
            Me.ToTable("EzProxyDatabase")
            Me.Property(Function(t) t.EzProxyDatabaseId).HasColumnName("EzProxyDatabaseId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
            Me.Property(Function(t) t.Name).HasColumnName("Name").HasMaxLength(200).IsRequired()
            Me.Property(Function(t) t.Title).HasColumnName("Title").HasMaxLength(1000)
            Me.Property(Function(t) t.Url).HasColumnName("Url").HasMaxLength(1000)
            Me.Property(Function(t) t.DomainName).HasColumnName("DomainName").HasMaxLength(1000)
            Me.Property(Function(t) t.Comment).HasColumnName("Comment").HasMaxLength(1000)
            Me.Property(Function(t) t.OutputOrder).HasColumnName("OutputOrder")
            Me.Property(Function(t) t.CreatedByUserId).HasColumnName("CreatedByUserId")
            Me.Property(Function(t) t.CreatedDate).HasColumnName("CreatedDate")
            Me.Property(Function(t) t.ModifiedByUserId).HasColumnName("ModifiedByUserId")
            Me.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate")
            Me.Property(Function(t) t.IsActive).HasColumnName("IsActive")
            Me.Property(Function(t) t.EzProxyServerId).HasColumnName("EzProxyServerId")
            Me.HasRequired(Function(t) t.CreatedByUser).WithMany().HasForeignKey(Function(t) t.CreatedByUserId)
            Me.HasOptional(Function(t) t.ModifiedByUser).WithMany().HasForeignKey(Function(t) t.ModifiedByUserId)
            Me.HasOptional(Function(t) t.EzProxyServer).WithMany().HasForeignKey(Function(t) t.EzProxyServerId)

        End Sub

    End Class

    Public Class EzProxyDatabaseDirectiveMap
        Inherits EntityTypeConfiguration(Of EzProxyDatabaseDirective)

        Public Sub New()
            Me.HasKey(Function(t) t.EzProxyDatabaseDirectiveId)
            Me.ToTable("EzProxyDatabaseDirective")
            Me.Property(Function(t) t.EzProxyDatabaseDirectiveId).HasColumnName("EzProxyDatabaseDirectiveId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
            Me.Property(Function(t) t.EzProxyDatabaseId).HasColumnName("EzProxyDatabaseId")
            Me.Property(Function(t) t.EzProxyDirectiveId).HasColumnName("EzProxyDirectiveId")
            Me.Property(Function(t) t.OutputAs).HasColumnName("OutputAs").HasMaxLength(4000)
            Me.Property(Function(t) t.OutputOrder).HasColumnName("OutputOrder")
            Me.Property(Function(t) t.Comment).HasColumnName("Comment").HasMaxLength(1000)
            Me.Property(Function(t) t.IsActive).HasColumnName("IsActive")
            Me.HasRequired(Function(t) t.EzProxyDatabase).WithMany(Function(t) t.Directives).HasForeignKey(Function(t) t.EzProxyDatabaseId)
            Me.HasRequired(Function(t) t.EzProxyDirective).WithMany().HasForeignKey(Function(t) t.EzProxyDirectiveId)
        End Sub

    End Class

    Public Class EzProxyDatabaseDirectiveOptionMap
        Inherits EntityTypeConfiguration(Of EzProxyDatabaseDirectiveOption)

        Public Sub New()
            Me.HasKey(Function(t) t.EzProxyDatabaseDirectiveOptionId)
            Me.ToTable("EzProxyDatabaseDirectiveOption")
            Me.Property(Function(t) t.EzProxyDatabaseDirectiveOptionId).HasColumnName("EzProxyDatabaseDirectiveOptionId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
            Me.Property(Function(t) t.EzProxyDatabaseDirectiveId).HasColumnName("EzProxyDatabaseDirectiveId")
            Me.Property(Function(t) t.EzProxyOptionId).HasColumnName("EzProxyOptionId")
            Me.Property(Function(t) t.OptionValue).HasColumnName("OptionValue").HasMaxLength(1000)
            Me.Property(Function(t) t.IsActive).HasColumnName("IsActive")
            Me.HasRequired(Function(t) t.EzProxyOption).WithMany().HasForeignKey(Function(t) t.EzProxyOptionId)
            Me.HasRequired(Function(t) t.EzProxyDatabaseDirective).WithMany(Function(t) t.Options).HasForeignKey(Function(t) t.EzProxyDatabaseDirectiveId)
        End Sub

    End Class

    Public Class EzProxyDirectiveMap
        Inherits EntityTypeConfiguration(Of EzProxyDirective)

        Public Sub New()
            Me.HasKey(Function(t) t.EzProxyDirectiveId)
            Me.ToTable("EzProxyDirective")
            Me.Property(Function(t) t.EzProxyDirectiveId).HasColumnName("EzProxyDirectiveId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
            Me.Property(Function(t) t.Name).HasColumnName("Name").IsRequired().HasMaxLength(200)
            Me.Property(Function(t) t.Description).HasColumnName("Description").HasMaxLength(1000)
            Me.Property(Function(t) t.OutputAs).HasColumnName("OutputAs").IsRequired().HasMaxLength(1000)
            Me.Property(Function(t) t.OutputOrder).HasColumnName("OutputOrder")
            Me.Property(Function(t) t.MaxOccursPerConfig).HasColumnName("MaxOccursPerConfig")
            Me.Property(Function(t) t.MaxOccursPerDatabase).HasColumnName("MaxOccursPerDatabase")
            Me.Property(Function(t) t.IsDefault).HasColumnName("IsDefault")
        End Sub

    End Class

    Public Class EzProxyOptionMap
        Inherits EntityTypeConfiguration(Of EzProxyOption)

        Public Sub New()
            Me.HasKey(Function(t) t.EzProxyOptionId)
            Me.ToTable("EzProxyOption")
            Me.Property(Function(t) t.EzProxyOptionId).HasColumnName("EzProxyOptionId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
            Me.Property(Function(t) t.EzProxyDirectiveId).HasColumnName("EzProxyDirectiveId")
            Me.Property(Function(t) t.OptionOrQualifier).HasColumnName("OptionOrQualifier").IsRequired().HasMaxLength(1)
            Me.Property(Function(t) t.Name).HasColumnName("Name").IsRequired().HasMaxLength(200)
            Me.Property(Function(t) t.Description).HasColumnName("Description").HasMaxLength(1000)
            Me.Property(Function(t) t.OutputAs).HasColumnName("OutputAs").IsRequired().HasMaxLength(200)
            Me.Property(Function(t) t.OutputOrder).HasColumnName("OutputOrder")
            Me.Property(Function(t) t.IsRequired).HasColumnName("IsRequired")
            Me.Property(Function(t) t.HasInputValue).HasColumnName("HasInputValue")
            Me.HasRequired(Function(t) t.EzProxyDirective).WithMany(Function(t) t.Options).HasForeignKey(Function(t) t.EzProxyDirectiveId)
        End Sub

    End Class

    Public Class RemoteAuthenticationMap
        Inherits EntityTypeConfiguration(Of RemoteAuthentication)

        Public Sub New()
            Me.HasKey(Function(t) t.RemoteAuthenticationId)
            Me.ToTable("RemoteAuthentication")
            Me.Property(Function(t) t.RemoteAuthenticationId).HasColumnName("RemoteAuthenticationId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
            Me.Property(Function(t) t.EzProxyDatabaseId).HasColumnName("EzProxyDatabaseId")
            Me.Property(Function(t) t.CollegeId).HasColumnName("CollegeId")
            Me.Property(Function(t) t.ConnectionMethodId).HasColumnName("ConnectionMethodId")
            Me.Property(Function(t) t.CreatedByUserId).HasColumnName("CreatedByUserId")
            Me.Property(Function(t) t.CreatedDate).HasColumnName("CreatedDate")
            Me.Property(Function(t) t.ModifiedByUserId).HasColumnName("ModifiedByUserId")
            Me.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate")
            Me.Property(Function(t) t.IsActive).HasColumnName("IsActive")
            Me.Property(Function(t) t.Url).HasColumnName("Url").HasMaxLength(1000)
            Me.Property(Function(t) t.FullUrl).HasColumnName("FullUrl").HasMaxLength(1000)
            Me.Property(Function(t) t.DomainName).HasColumnName("DomainName").HasMaxLength(1000)
            Me.Property(Function(t) t.CampusRestriction).HasColumnName("CampusRestriction").HasMaxLength(1000)
            Me.HasRequired(Function(t) t.EzProxyDatabase).WithMany().HasForeignKey(Function(t) t.EzProxyDatabaseId)
            Me.HasRequired(Function(t) t.College).WithMany().HasForeignKey(Function(t) t.CollegeId)
            Me.HasRequired(Function(t) t.ConnectionMethod).WithMany().HasForeignKey(Function(t) t.ConnectionMethodId)
            Me.HasRequired(Function(t) t.CreatedByUser).WithMany().HasForeignKey(Function(t) t.CreatedByUserId)
            Me.HasOptional(Function(t) t.ModifiedByUser).WithMany().HasForeignKey(Function(t) t.ModifiedByUserId)
        End Sub

    End Class

    Public Class CollegeIpAddressMap
        Inherits EntityTypeConfiguration(Of CollegeIpAddress)

        Public Sub New()
            Me.HasKey(Function(t) t.CollegeIpAddressId)
            Me.ToTable("CollegeIpAddress")
            Me.Property(Function(t) t.CollegeIpAddressId).HasColumnName("CollegeIpAddressId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
            Me.Property(Function(t) t.CollegeId).HasColumnName("CollegeId")
            Me.Property(Function(t) t.Campus).HasColumnName("Campus").HasMaxLength(100)
            Me.Property(Function(t) t.IpAddress).HasColumnName("IpAddress").HasMaxLength(100)
            Me.Property(Function(t) t.SubnetMask).HasColumnName("SubnetMask").HasMaxLength(100)
            Me.Property(Function(t) t.RegularExpression).HasColumnName("RegularExpression").HasMaxLength(1000)
            Me.Property(Function(t) t.CreatedByUserId).HasColumnName("CreatedByUserId")
            Me.Property(Function(t) t.CreatedDate).HasColumnName("CreatedDate")
            Me.Property(Function(t) t.ModifiedByUserId).HasColumnName("ModifiedByUserId")
            Me.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate")
            Me.Property(Function(t) t.IsActive).HasColumnName("IsActive")

            Me.HasRequired(Function(t) t.College).WithMany().HasForeignKey(Function(t) t.CollegeId)
            Me.HasRequired(Function(t) t.CreatedByUser).WithMany().HasForeignKey(Function(t) t.CreatedByUserId)
            Me.HasOptional(Function(t) t.ModifiedByUser).WithMany().HasForeignKey(Function(t) t.ModifiedByUserId)

        End Sub
    End Class

    Public Class EzProxyServerMap
        Inherits EntityTypeConfiguration(Of EzProxyServer)

        Public Sub New()
            Me.HasKey(Function(t) t.EzProxyServerId)
            Me.ToTable("EzProxyServer")
            Me.Property(Function(t) t.EzProxyServerId).HasColumnName("EzProxyServerId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
            Me.Property(Function(t) t.Name).HasColumnName("Name").IsRequired().HasMaxLength(100)
            Me.Property(Function(t) t.FileShare).HasColumnName("FileShare").IsRequired().HasMaxLength(200)
            Me.Property(Function(t) t.FileName).HasColumnName("FileName").IsRequired().HasMaxLength(200)
            Me.Property(Function(t) t.FileShareUsername).HasColumnName("FileShareUsername").IsRequired().HasMaxLength(100)
            Me.Property(Function(t) t.FileSharePassword).HasColumnName("FileSharePassword").IsRequired().HasMaxLength(100)
            Me.Property(Function(t) t.FileShareDomain).HasColumnName("FileShareDomain").IsRequired().HasMaxLength(100)
        End Sub

    End Class

End Namespace