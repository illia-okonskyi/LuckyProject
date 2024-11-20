using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenIddict.EntityFrameworkCore.Models;
using System;

namespace LuckyProject.AuthServer.DbLayer
{
    public class AuthServerDbContext : IdentityDbContext<AuthServerUser, AuthServerRole, Guid>
    {
        #region ctor & base config
        private readonly string connectionString;

        public AuthServerDbContext(
            IOptions<AuthServerDbContextOptions> dbContextOptions,
            DbContextOptions options)
            : base(options)
        {
            connectionString = dbContextOptions?.Value?.ConnectionString
                ?? "Data Source=:memory:; Foreign Keys=True";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite(connectionString);
            optionsBuilder.UseOpenIddict();
            // TODO: Remove for production release
            optionsBuilder.EnableSensitiveDataLogging();
        }
        #endregion

        #region DbSets
        public virtual DbSet<AuthServerPermission> Permissions { get; set; }
        public virtual DbSet<AuthServerBinaryPermissionValue> BinaryPermissionValue
        {
            get;
            set;
        }

        public virtual DbSet<AuthServerLevelPermissionValue> LevelPermissionsValue
        {
            get;
            set;
        }
        public virtual DbSet<AuthServerPasskeyPermissionValue> PasskeyPermissionsValue
        {
            get;
            set;
        }
        public virtual DbSet<AuthServerRolePermission> RolePermissions { get; set; }
        public virtual DbSet<AuthServerCorsConfig> CorsConfig {  get; set; }
        public virtual DbSet<OpenIddictEntityFrameworkCoreApplication> OpenIddictApplications
        {
            get;
            set;
        }
        public virtual DbSet<LpApi> Apis { get; set; }
        public virtual DbSet<AuthServerInitialSeed> InitialSeed { get; set; }
        #endregion

        #region OnModelCreating
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AuthServerPermission>()
                .ToTable("Permissions")
                .HasKey(m => m.Id);
            builder.Entity<AuthServerPermission>().Property(m => m.FullName)
                .HasMaxLength(64)
                .IsRequired(true);
            builder.Entity<AuthServerPermission>().Property(m => m.Description)
                .HasMaxLength(128)
                .IsRequired(true);

            builder.Entity<AuthServerBinaryPermissionValue>()
                .ToTable("PermissionsBinaryValue")
                .HasKey(m => m.Id);
            builder.Entity<AuthServerBinaryPermissionValue>()
                .Property(m => m.Allow)
                .HasMaxLength(3)
                .IsRequired(true);

            builder.Entity<AuthServerLevelPermissionValue>()
                .ToTable("PermissionsLevelValue")
                .HasKey(m => m.Id);

            builder.Entity<AuthServerPasskeyPermissionValue>()
                .ToTable("PermissionsPasskeyValue")
                .HasKey(m => m.Id);
            builder.Entity<AuthServerPasskeyPermissionValue>()
                .Property(m => m.Passkey)
                .HasMaxLength(64)
                .IsRequired(true);

            builder.Entity<AuthServerPermission>()
                .HasOne(m => m.BinaryValue)
                .WithOne(m => m.Permission)
                .HasForeignKey<AuthServerBinaryPermissionValue>(m => m.PermissionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<AuthServerPermission>()
                .HasOne(m => m.LevelValue)
                .WithOne(m => m.Permission)
                .HasForeignKey<AuthServerLevelPermissionValue>(m => m.PermissionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<AuthServerPermission>()
                .HasMany(m => m.PasskeyValue)
                .WithOne(m => m.Permission)
                .HasForeignKey(m => m.PermissionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AuthServerRolePermission>()
                .ToTable("RolePermissions")
                .HasKey(m => new { m.RoleId, m.PermissionId });

            builder.Entity<AuthServerRole>()
                .HasMany(m => m.RolePermissions)
                .WithOne(m => m.Role)
                .HasForeignKey(m => m.RoleId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AuthServerPermission>()
                .HasMany(m => m.RolePermissions)
                .WithOne(m => m.Permission)
                .HasForeignKey(m => m.PermissionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AuthServerRole>().Property(m => m.Name)
                .HasMaxLength(128)
                .IsRequired(true);
            builder.Entity<AuthServerRole>().Property(m => m.NormalizedName)
                .HasMaxLength(128)
                .IsRequired(true);
            builder.Entity<AuthServerRole>().Property(m => m.Description)
                .HasMaxLength(128)
                .IsRequired(true);

            builder.Entity<AuthServerUser>()
                .Property(m => m.UserName)
                .HasMaxLength(64)
                .IsRequired(true);
            builder.Entity<AuthServerUser>()
                .Property(m => m.NormalizedUserName)
                .HasMaxLength(64)
                .IsRequired(true);
            builder.Entity<AuthServerUser>()
                .Property(m => m.Email)
                .HasMaxLength(15)
                .IsRequired(true);
            builder.Entity<AuthServerUser>()
                .Property(m => m.NormalizedEmail)
                .HasMaxLength(128)
                .IsRequired(true);
            builder.Entity<AuthServerUser>()
                .Property(m => m.PhoneNumber)
                .HasMaxLength(16)
                .IsRequired(true);
            builder.Entity<AuthServerUser>()
                .Property(m => m.FullName)
                .HasMaxLength(128)
                .IsRequired(true);
            builder.Entity<AuthServerUser>()
                .Property(m => m.TelegramUserName)
                .HasMaxLength(128)
                .IsRequired(false);
            builder.Entity<AuthServerUser>()
                .Property(m => m.PreferredLocale)
                .HasMaxLength(8)
                .IsRequired(true);

            builder.Entity<AuthServerInitialSeed>()
                .ToTable("InitialSeed")
                .HasKey(m => m.Id);

            builder.Entity<AuthServerCorsConfig>()
                .ToTable("CorsConfig")
                .HasKey(m => new { m.ClientId, m.Origin });

            builder.Entity<AuthServerCorsConfig>()
                .Property(m => m.ClientId)
                .HasMaxLength(100);
            builder.Entity<AuthServerCorsConfig>()
                .Property(m => m.Origin)
                .HasMaxLength(128);

            builder.Entity<AuthServerCorsConfig>()
                .HasOne<OpenIddictEntityFrameworkCoreApplication>()
                .WithMany()
                .HasPrincipalKey(m => m.ClientId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AuthServerUser>()
                .HasOne<OpenIddictEntityFrameworkCoreApplication>()
                .WithOne()
                .HasForeignKey<AuthServerUser>(m => m.MachineClientId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<LpApi>()
                .ToTable("Apis")
                .HasKey(m => m.Id);
            builder.Entity<LpApi>()
                .Property(m => m.Name)
                .HasMaxLength(64)
                .IsRequired(true);
            builder.Entity<LpApi>()
                .Property(m => m.Description)
                .HasMaxLength(256)
                .IsRequired(true);
            builder.Entity<LpApi>()
                .Property(m => m.Endpoint)
                .HasMaxLength(256)
                .IsRequired(true);
            builder.Entity<LpApi>()
                .Property(m => m.CallbackUrl)
                .HasMaxLength(256)
                .IsRequired(true);
            builder.Entity<LpApi>()
                .HasOne<OpenIddictEntityFrameworkCoreApplication>()
                .WithOne()
                .HasForeignKey<LpApi>(m => m.MachineClientId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<LpApi>()
                .HasOne(m => m.MachineUser)
                .WithOne(m => m.Api)
                .HasForeignKey<LpApi>(m => m.MachineUserId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<LpApi>()
                .HasMany(m => m.Permissions)
                .WithOne(m => m.Api)
                .HasForeignKey(m => m.ApiId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        }
        #endregion
    }
}
