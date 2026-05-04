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
        // DbSet properties for your entities go here

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
                entity.HasOne<Project>()
                    .WithMany(p => p.Registrations)
                    .OnDelete(DeleteBehavior.Restrict);

                // Do the same for Employee
                entity.HasOne<Employee>()
                    .WithMany(e => e.Registrations)
                    .OnDelete(DeleteBehavior.Cascade);


                entity.HasDiscriminator<string>("RegistrationType");
            });
        }

    }
}
