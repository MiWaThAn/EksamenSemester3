using Domain.Entity;
using Domain.Entity.Item;
using Domain.Entity.Item.Activities;
using Domain.Entity.Item.Activities;
using Domain.Entity.Item.Registrations;
using Domain.Entity.Item.Registrations;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Domain.Entity.Person;
using Domain.Entity.Person;
using Domain.Entity.Person.Auth;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Activity = Domain.Entity.Item.Activities.Activity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        //Person
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ProjectAssignment> ProjectAssignments { get; set; }

        //auth
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<DeviceToken> DeviceTokens { get; set; }

        //Item
        public DbSet<Project> Projects { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ProjectActivity> ProjectActivities { get; set; }

        //Registrations
        public DbSet<HourRegistration> HourRegistrations { get; set; }
        public DbSet<ExpenseRegistration> ExpenseRegistrations { get; set; }
        public DbSet<WorkLog> WorkLogs { get; set; }


        //Mappings
        public DbSet<IntegrationMapping> Mappings { get; set; }
        public DbSet<IntegrationSetting> IntegrationSettings { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<SelectedEntityType> SelectedEntityTypes { get; set; }
        public DbSet<ProviderEndpoint> ProviderUrls { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Registration>(entity =>
            {
                entity.HasKey(r => r.Id);


                entity.HasOne<ProjectActivity>()
                      .WithMany(pa => pa.Registrations)
                      .HasForeignKey(r => r.ProjectActivityId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<HourRegistration>(entity =>
            {
                entity.OwnsMany(r => r.Intervals, navigationBuilder =>
                {
                    navigationBuilder.ToJson();
                })
                            .Navigation(r => r.Intervals)
            .HasField("_intervals")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
            });
            modelBuilder.Entity<Registration>()
                    .HasOne<Project>()
          .WithMany(p => p.Registrations)
          .HasForeignKey(r => r.ProjectId)
          .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<WorkLog>().HasOne<Employee>()
      .WithMany(e => e.WorkLogs)
      .HasForeignKey(r => r.EmployeeId)
      .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Project>()
                  .Navigation(p => p.Registrations)
                  .HasField("_registrations")
                  .UsePropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.Entity<ProjectActivity>()
                  .Navigation(pa => pa.Registrations)
                  .HasField("_registrations")
                  .UsePropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.Entity<Project>()
      .Navigation(p => p.Assignments)
      .HasField("_assignments")
      .UsePropertyAccessMode(PropertyAccessMode.Field);
            modelBuilder.Entity<ProjectAssignment>(entity =>
            {
                entity.HasOne<Employee>()
                      .WithMany(e => e.Assignments)
                      .HasForeignKey(a => a.EmployeeId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Project>()
                      .WithMany(p => p.Assignments)
                      .HasForeignKey(a => a.ProjectId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Employee>()
      .Navigation(p => p.Assignments)
      .HasField("_assignments")
      .UsePropertyAccessMode(PropertyAccessMode.Field);
            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(c => c.CVRNumber)
                    .HasConversion(
                        v => v.Value,
                        v => new CvrNumber(v)
                    );

                entity.Property(c => c.Email)
                    .HasConversion(
                        v => v.Value,
                        v => new EmailAddress(v)
                    );
            });
            modelBuilder.Entity<Company>()
                .HasOne(c => c.Account)
                .WithOne(a => a.Company)
                .HasForeignKey<Company>(c => c.AccountId);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Account)
                .WithOne(a => a.Employee)
                .HasForeignKey<Employee>(e => e.AccountId);

            modelBuilder.Entity<ExpenseRegistration>()
                .HasOne<Expense>()
                .WithMany()
                .HasForeignKey(er => er.ExpenseId)
                .OnDelete(DeleteBehavior.NoAction);




            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Username)
                .IsUnique();
            modelBuilder.Entity<Company>()
                .HasIndex(c => c.CVRNumber)
                .IsUnique();

            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(a => a.PhoneNumber)
                    .HasConversion(v => v.Value, v => new PhoneNumber(v));
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(c => c.CVRNumber)
                    .HasConversion(
                        v => v.Value,
                        v => new CvrNumber(v)
                    );

                entity.Property(c => c.Email)
                    .HasConversion(
                        v => v.Value,
                        v => new EmailAddress(v)
                    );
                entity.Navigation(c => c.Settings)
                .HasField("_settings")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

                entity.HasMany(c => c.Settings)
                    .WithOne()
                    .HasForeignKey(s => s.CompanyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.Email)
                    .HasConversion(
                        v => v.Value,
                        v => new EmailAddress(v)
                    );
            });

            modelBuilder.Entity<ProjectActivity>(entity =>
            {
                entity.HasKey(pa => pa.Id);

                entity.HasOne<Employee>()
                      .WithMany()
                      .HasForeignKey(pa => pa.ResponsibleEmployeeId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne<Project>()
                      .WithMany(p => p.Activities)
                      .HasForeignKey(pa => pa.ProjectId)
                      .OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<Provider>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Datasource)
                    .HasConversion(d => d.Value, v => DataSource.From(v));

                entity.Navigation(p => p.Urls)
                    .HasField("_urls")
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<ProviderEndpoint>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.EntityType)
                    .HasConversion(e => e.Value, v => IntegrationEntityType.From(v));

                entity.HasOne<Provider>()
                    .WithMany(p => p.Urls)
                    .HasForeignKey(p => p.ProviderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Account>()
            .Navigation(pa => pa.DeviceTokens)
            .HasField("_deviceTokens")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.Entity<SelectedEntityType>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.EntityType)
                    .HasConversion(e => e.Value, v => IntegrationEntityType.From(v));

                entity.HasOne<IntegrationSetting>()
                    .WithMany(s => s.EntityTypes)
                    .HasForeignKey(s => s.IntegrationSettingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<IntegrationSetting>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.HasOne(s => s.Provider)
                    .WithMany()
                    .HasForeignKey(s => s.ProviderId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.Navigation(s => s.EntityTypes)
                    .HasField("_entityTypes")
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                entity.Navigation(s => s.Mappings)
                    .HasField("_mappings")
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                entity.HasMany(s => s.Mappings)
                    .WithOne(m => m.IntegrationSetting)
                    .HasForeignKey(m => m.IntegrationSettingId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.OwnsOne(s => s.Credential, credential =>
                {
                    credential.Property(c => c.Key)
                        .HasColumnName("CredentialKey");

                        credential.Property(c => c.EncryptedValue)
                            .HasColumnName("CredentialValue");



                });


                modelBuilder.Entity<IntegrationMapping>(entity =>
                {
                    entity.HasKey(m => m.Id);

                    entity.Property(m => m.EntityType)
                        .HasConversion(e => e.Value, v => IntegrationEntityType.From(v));

                    entity.HasIndex(m => new { m.IntegrationSettingId, m.ExternalId, m.EntityType })
                        .IsUnique();
                });


            });

        }
    }
}