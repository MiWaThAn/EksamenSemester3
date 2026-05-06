using Domain.Entity.Item;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Domain.Entity.Person;
using System.Diagnostics;
using Domain.Entity.Item.Activities;
using Activity = Domain.Entity.Item.Activities.Activity;
using Domain.Entity.Item.Registrations;
using Domain.Entity.Mapping;

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
        public DbSet<Account> Accounts { get; set; }

        //Item
        public DbSet<Project> Projects { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ProjectActivity> ProjectActivities { get; set; }

        //Registrations
        public DbSet<HourRegistration> HourRegistrations { get; set; }
        public DbSet<ExpenseRegistration> ExpenseRegistrations { get; set; }


        //Mappings
        public DbSet<IntegrationMapping> Mappings { get; set; }
        public DbSet<IntegrationSetting> IntegrationSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Registration>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.HasOne<Project>()
                      .WithMany(p => p.Registrations)
                      .HasForeignKey(r => r.ProjectId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne<ProjectActivity>()
                      .WithMany(pa => pa.Registrations)
                      .HasForeignKey(r => r.ProjectActivityId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne<Employee>()
                      .WithMany(e => e.Registrations)
                      .HasForeignKey(r => r.EmployeeId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Project>()
                  .Navigation(p => p.Registrations)
                  .HasField("_registrations")
                  .UsePropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.Entity<ProjectActivity>()
                  .Navigation(pa => pa.Registrations)
                  .HasField("_registrations")
                  .UsePropertyAccessMode(PropertyAccessMode.Field);

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
        

        modelBuilder.Entity<ExpenseRegistration>()
                .HasOne<Expense>()
                .WithMany()
                .HasForeignKey(e => e.ExpenseId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ExpenseRegistration>()
                .HasOne<Expense>()
                .WithMany()
                .HasForeignKey(er => er.ExpenseId);
            modelBuilder.Entity<Company>()
                .HasOne(c => c.Account)
                .WithOne(a => a.Company)
                .HasForeignKey<Company>(c => c.AccountId);

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

        }

    }
}
