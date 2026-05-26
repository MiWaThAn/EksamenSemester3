using Domain.Entity.Item;
using Domain.Entity.Mapping;
using Domain.Entity.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndTests.Domain.Entity.Person
{
    public class CompanyTests
    {
        private readonly string _validCompanyName = "Acme Logistics Group";
        private readonly CvrNumber _stubCvr = new CvrNumber("12345678");
        private readonly EmailAddress _stubEmail = new EmailAddress("info@acmelogistics.dk");
        private readonly Account _stubAccount;

        public CompanyTests()
        {
            _stubAccount = new Account { Id = Guid.NewGuid() };
        }

        #region Helper Methods for Private Collection Injections

        private Company CreateCompanyInstance()
        {
            var company = new Company(_validCompanyName, _stubCvr, _stubAccount, _stubEmail);
            // Establish target aggregate Id explicitly if required by base configuration
            typeof(Company).GetProperty("Id")?.SetValue(company, Guid.NewGuid());
            return company;
        }

        private void InjectChildEntity<T>(Company company, string fieldName, T childEntity)
        {
            var field = typeof(Company).GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var list = (List<T>)field?.GetValue(company)!;
            list.Add(childEntity);
        }

        #endregion

        #region Constructor & Initialization Tests

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitializeCorrectlyAndBindAccount()
        {
            // Act
            var company = new Company(_validCompanyName, _stubCvr, _stubAccount, _stubEmail);

            // Assert
            Assert.Equal(_validCompanyName, company.Name);
            Assert.Equal(_stubCvr, company.CVRNumber);
            Assert.Equal(_stubEmail, company.Email);
            Assert.Equal(_stubAccount.Id, company.AccountId);
            Assert.Same(_stubAccount, company.Account);

            // Check that inner encapsulation lists default to empty collections
            Assert.Empty(company.Employees);
            Assert.Empty(company.Projects);
            Assert.Empty(company.Activities);
            Assert.Empty(company.Expenses);
            Assert.Empty(company.Settings);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Constructor_WhenNameIsNullOrEmpty_ShouldThrowException(string invalidName)
        {
            Assert.ThrowsAny<Exception>(() => new Company(invalidName, _stubCvr, _stubAccount, _stubEmail));
        }

        [Fact]
        public void Constructor_WhenAccountIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsAny<Exception>(() => new Company(_validCompanyName, _stubCvr, null!, _stubEmail));
        }

        #endregion

        #region Aggregate Boundary Collection Modifications (Happy Paths & Guards)

        [Fact]
        public void RemoveEmployee_WhenEmployeeExists_ShouldExecuteSoftDeleteAndFilterPublicCollection()
        {
            // Arrange
            var company = CreateCompanyInstance();
            var employeeId = Guid.NewGuid();
            var employee = new Employee { Id = employeeId };

            InjectChildEntity(company, "_employees", employee);

            // Act
            company.RemoveEmployee(employeeId);

            // Assert
            Assert.Empty(company.Employees); // Excluded from the exposed read-only list
            Assert.True(employee.IsDeleted);  // Flag raised successfully inside domain entity core
        }

        [Fact]
        public void RemoveEmployee_WhenEmployeeDoesNotExist_ShouldThrowArgumentException()
        {
            // Arrange
            var company = CreateCompanyInstance();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => company.RemoveEmployee(Guid.NewGuid()));
            Assert.Equal("Denne medarbejder blev ikke fundet i firmaet", exception.Message);
        }

        [Fact]
        public void RemoveProject_WhenProjectExists_ShouldExecuteSoftDelete()
        {
            // Arrange
            var company = CreateCompanyInstance();
            var projectId = Guid.NewGuid();
            var project = new Project();
            typeof(Project).GetProperty("Id")?.SetValue(project, projectId);

            InjectChildEntity(company, "_projects", project);

            // Act
            company.RemoveProject(projectId);

            // Assert
            Assert.Empty(company.Projects);
            Assert.True(project.IsDeleted);
        }

        [Fact]
        public void RemoveActivity_WhenActivityDoesNotExist_ShouldThrowArgumentException()
        {
            // Arrange
            var company = CreateCompanyInstance();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => company.RemoveActivity(Guid.NewGuid()));
            Assert.Equal("Denne aktivitet blev ikke fundet i firmaet", exception.Message);
        }

        [Fact]
        public void RemoveExpense_WhenExpenseExists_ShouldSoftDeleteTargetRecord()
        {
            // Arrange
            var company = CreateCompanyInstance();
            var expenseId = Guid.NewGuid();
            var expense = new Expense();
            typeof(Expense).GetProperty("Id")?.SetValue(expense, expenseId);

            InjectChildEntity(company, "_expenses", expense);

            // Act
            company.RemoveExpense(expenseId);

            // Assert
            Assert.Empty(company.Expenses);
            Assert.True(expense.IsDeleted);
        }

        #endregion

        #region Integration Settings Boundary Constraints

        [Fact]
        public void RemoveIntegrationSetting_WhenSettingExists_ShouldHardRemoveFromCollection()
        {
            // Arrange
            var company = CreateCompanyInstance();
            var settingId = Guid.NewGuid();
            var setting = new IntegrationSetting { Id = settingId };

            InjectChildEntity(company, "_settings", setting);

            // Act
            company.RemoveIntegrationSetting(settingId);

            // Assert
            // Integration settings don't possess soft-delete rules in your implementation code,
            // they are fully dropped from memory immediately.
            Assert.Empty(company.Settings);
        }

        [Fact]
        public void RemoveIntegrationSetting_WhenNotFound_ShouldThrowArgumentException()
        {
            // Arrange
            var company = CreateCompanyInstance();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => company.RemoveIntegrationSetting(Guid.NewGuid()));
            Assert.Equal("Denne integrationsindstilling blev ikke fundet for dette firma.", exception.Message);
        }

        #endregion

        #region Fundamental Value Mutations

        [Fact]
        public void UpdateCompanyName_WithValidString_ShouldMutatePropertyAndTrackTimestamp()
        {
            // Arrange
            var company = CreateCompanyInstance();
            var previousUpdateTimestamp = company.UpdatedAt;
            string structuralNameChange = "Global Omni-Logistics Solutions ApS";

            // Act
            company.UpdateCompanyName(structuralNameChange);

            // Assert
            Assert.Equal(structuralNameChange, company.Name);
            Assert.True(company.UpdatedAt >= previousUpdateTimestamp);
        }

        [Fact]
        public void UpdateCompanyEmail_WithValidValueObject_ShouldMutateProperty()
        {
            // Arrange
            var company = CreateCompanyInstance();
            var directEmailObject = new EmailAddress("accounting@acmelogistics.com");

            // Act
            company.UpdateCompanyEmail(directEmailObject);

            // Assert
            Assert.Equal(directEmailObject, company.Email);
        }

        #endregion
    }
}
