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

            modelBuilder.Entity<Registration>()
                .HasOne<Employee>()
                .WithMany() 
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Registration>()
                .HasOne<Project>()
                .WithMany()
                .HasForeignKey(r => r.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Registration>()
                .HasOne<Activity>()
                .WithMany()
                .HasForeignKey(r => r.ActivityId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ExpenseRegistration>()
                .HasOne<Expense>()
                .WithMany()
                .HasForeignKey(e => e.ExpenseId)
                .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
