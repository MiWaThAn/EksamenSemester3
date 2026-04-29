using Domain.Builders.Item;
using Domain.Builders.Person;
using Domain.Entity.Item;
using Domain.Entity.Item.Activity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Person
{
    public class Company : Account
    {
        private readonly List<Employee> _employees = new();
        public IReadOnlyCollection<Employee> Employees => _employees.AsReadOnly();
        private readonly List<Project> _projects = new();
        public IReadOnlyCollection<Project> Projects => _projects.AsReadOnly();
        private readonly List<Activity> _activities = new();
        public IReadOnlyCollection<Activity> Activities => _activities.AsReadOnly();
        private readonly List<Expense> _expenses = new();
        public IReadOnlyCollection<Expense> Expenses => _expenses.AsReadOnly();
        public string CVRNumber { get; internal set; }
        public string HashedAgreementGrantToken { get; internal set; }
        public string HashedAppSecretToken { get; internal set; }
        public string HashedEconomicAgreementNumber { get; internal set; }

        public Company(string name, string hashedPassword, string username, string? email, string? phoneNumber, string cvrNumber, string hashedAgreementGrantToken, string hashedAppSecretToken, string hashedEconomicAgreementNumber) : base(name, hashedPassword, username, email, phoneNumber)
        {
            CVRNumber = cvrNumber ?? throw new ArgumentNullException(nameof(cvrNumber));
            HashedAgreementGrantToken = hashedAgreementGrantToken ?? throw new ArgumentNullException(nameof(hashedAgreementGrantToken));
            HashedAppSecretToken = hashedAppSecretToken ?? throw new ArgumentNullException(nameof(hashedAppSecretToken));
            HashedEconomicAgreementNumber = hashedEconomicAgreementNumber ?? throw new ArgumentNullException(nameof(hashedEconomicAgreementNumber));
        }
        public Employee CreateEmployee(EmployeeBuilder builder)
        {
            var employee = builder.WithCompany(this).Build();
            if (_employees.Exists(e => e.Id == employee.Id)) throw new ArgumentException("This employee is already added to the company.");
            _employees.Add(employee);
            UpdatedAt = DateTime.UtcNow;
            return employee;
        }
        public void RemoveEmployee(Guid employeeId)
        {
            var employee = _employees.Find(e => e.Id == employeeId);
            if (employee == null) throw new ArgumentException("Employee not found for this company.");
            _employees.Remove(employee);
            UpdatedAt = DateTime.UtcNow;
        }
        public Project CreateProject(ProjectBuilder builder)
        {
            var project = builder.WithCompany(this).Build();
            if (_projects.Exists(p => p.Id == project.Id)) throw new ArgumentException("This project is already added to the company.");
            _projects.Add(project);
            UpdatedAt = DateTime.UtcNow;
            return project;
        }
        public void RemoveProject(Guid projectId)
        {
            var project = _projects.Find(p => p.Id == projectId);
            if (project == null) throw new ArgumentException("Project not found for this company.");
            _projects.Remove(project);
            UpdatedAt = DateTime.UtcNow;
        }
        public Activity CreateActivity(ActivityBuilder builder)
        {
            var activity = builder.WithCompany(this).Build();
            if (_activities.Exists(a => a.Id == activity.Id)) throw new ArgumentException("This activity is already added to the company.");
            _activities.Add(activity);
            UpdatedAt = DateTime.UtcNow;
            return activity;
        }
        public void RemoveActivity(Guid activityId)
        {
            var activity = _activities.Find(a => a.Id == activityId);
            if (activity == null) throw new ArgumentException("Activity not found for this company.");
            _activities.Remove(activity);
            UpdatedAt = DateTime.UtcNow;
        }
        public Expense CreateExpense(ExpenseBuilder builder)
        {
            var expense = builder.WithCompany(this).Build();
            if (_expenses.Exists(e => e.Id == expense.Id)) throw new ArgumentException("This expense is already added to the company.");
            _expenses.Add(expense);
            UpdatedAt = DateTime.UtcNow;
            return expense;
        }
        public void RemoveExpense(Guid expenseId)
        {
            var expense = _expenses.Find(e => e.Id == expenseId);
            if (expense == null) throw new ArgumentException("Expense not found for this company.");
            _expenses.Remove(expense);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
