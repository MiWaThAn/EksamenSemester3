using Domain.Builders.Item;
using Domain.Builders.Mapping;
using Domain.Builders.Person;
using Domain.Entity.Item;
using Domain.Entity.Item.Activities;
using Domain.Entity.Item.Registrations;
using Domain.Entity.Mapping;
using Domain.Guards;
using Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.Person
{
    /// <summary>
    /// Denne klasse ligger på grænsen af at være en fed aggregate root/gud klasse da den indeholder lister og forretnings logik over ansatte, projekter, aktiviteter, og udgifter.
    /// Kunne godt overveje at flytte nogen af de her foretnings regler ned i nogen services eller lignende for at gøre klassen mere single responsibility
    /// Kunne også godt gøre brug af bounded context og seperate projekter for at adskille de forskellige områder af domænet og undgå at have en stor klasse
    /// Men jeg tror det går fint så længe vi holder det her på et rimeligt niveau og ikke tilføjer alt formeget kompleks logik.
    /// 
    /// </summary>
    public class Company : Base
    {
        //Navn og email for notifikationer og information
        public string Name { get; internal set; }
        public EmailAddress Email { get; internal set; }
        //Cvr til registrering så vi ved at firmaer der opretter sig er unikke
        public CvrNumber CVRNumber { get; internal set; }

        //Et firma kan ikke eksiterer uden en konto, så AccountId er ikke nullable.
        //Det er den konto der har admin rettigheder over firmaet og dets ansatte og projekter.
        public Guid AccountId { get; internal set; }
        public Account Account { get; internal set; }

        //Lister over ansatte, projekter, aktiviteter, og udgifter.
        //Disse lister håndteres internt i klassen for at sikre konsistens og integritet,
        //og de eksponeres som read-only for at forhindre uautoriserede ændringer udefra.
        //Readonly tvinger os til at bruge medtoderne i klassen for at tilføje og fjerne elementer fra listerne,
        //hvilket giver os mulighed for at implementere forretningslogik og validering i disse metoder.
        private readonly List<Employee> _employees = new();
        public IReadOnlyCollection<Employee> Employees => _employees.Where(e=>!e.IsDeleted).ToList().AsReadOnly();
        private readonly List<Project> _projects = new();
        public IReadOnlyCollection<Project> Projects => _projects.Where(p=>!p.IsDeleted).ToList().AsReadOnly();
        private readonly List<Activity> _activities = new();
        public IReadOnlyCollection<Activity> Activities => _activities.Where(a=>!a.IsDeleted).ToList().AsReadOnly();
        private readonly List<Expense> _expenses = new();
        public IReadOnlyCollection<Expense> Expenses => _expenses.Where(e=>!e.IsDeleted).ToList().AsReadOnly();

        //Lister over integration settings, som kan bruges til at gemme API nøgler og lignende for integrationer med eksterne systemer som e-conomic, Microsoft Graph, Slack, osv.
        //Dette gør det muligt for firmaet at have fleksible og udvidelige integrationer uden at skulle ændre på selve datamodellen for firmaet.
        //Det tillader også firmaet at have flere integrationer med forskellige systemer samtidig, og at håndtere disse integrationer på en struktureret måde.
        //Dette er gjordt for skalerbarhed og for at imødekomme fremtidige behov for integrationer, som kan være en vigtig del af et moderne tidsregistreringssystem.
        //Hver integration setting har en provider (f.eks. "e-conomic") og en key (f.eks. "APIKey"), som sammen unikt identificerer en integration setting for et firma.
        //Denne liste kan også være tom hvis firmaet ikke har nogen integrationer, og det er op til firmaet at tilføje integration settings efter behov.
        //Dvs. at et firma kan lave en konto og bruge appen uden nogensinde at tilføje en integration setting (ved at lave medarbejderer kun gemt lokalt på vores database)
        private readonly List<IntegrationSetting> _settings = new();
        public IReadOnlyCollection<IntegrationSetting> Settings => _settings.AsReadOnly();

        public Company() : base()
        {

        }
        internal Company(string name, CvrNumber cvrNumber, Account account, EmailAddress email) : base()
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(account, nameof(account));
            Name = name;
            CVRNumber = cvrNumber;
            Account = account;
            AccountId = account.Id;
            Email = email;
        }
        public Employee CreateEmployee(EmployeeBuilder builder)
        {
            var employee = builder.WithCompany(this).Build();
            if (_employees.Exists(e => e.Id == employee.Id)) throw new ArgumentException("Denne medarbejder er allerede i firmaet");
            _employees.Add(employee);
            UpdatedAt = DateTime.UtcNow;
            return employee;
        }
        public void RemoveEmployee(Guid employeeId)
        {
            var employee = _employees.Find(e => e.Id == employeeId && !e.IsDeleted);
            if (employee == null) throw new ArgumentException("Denne medarbejder blev ikke fundet i firmaet");
            employee.SoftDelete();
            UpdatedAt = DateTime.UtcNow;
        }
        public Project CreateProject(ProjectBuilder builder)
        {
            var project = builder.WithCompany(this).Build();
            if (_projects.Exists(p => p.Id == project.Id)) throw new ArgumentException("Dette projekt er allerede tilføjet til firmaet");
            _projects.Add(project);
            UpdatedAt = DateTime.UtcNow;
            return project;
        }
        public void RemoveProject(Guid projectId)
        {
            var project = _projects.Find(p => p.Id == projectId && !p.IsDeleted);
            if (project == null) throw new ArgumentException("Dette projekt blev ikke fundet i firmaet");
            project.SoftDelete();
            UpdatedAt = DateTime.UtcNow;
        }
        public Activity CreateActivity(ActivityBuilder builder)
        {
            var activity = builder.WithCompany(this).Build();
            if (_activities.Exists(a => a.Id == activity.Id)) throw new ArgumentException("Denne aktivitet er allerede tilføjet til firmaet");
            _activities.Add(activity);
            UpdatedAt = DateTime.UtcNow;
            return activity;
        }
        public void RemoveActivity(Guid activityId)
        {
            var activity = _activities.Find(a => a.Id == activityId && !a.IsDeleted);
            if (activity == null) throw new ArgumentException("Denne aktivitet blev ikke fundet i firmaet");
            activity.SoftDelete();
            UpdatedAt = DateTime.UtcNow;
        }
        public Expense CreateExpense(ExpenseBuilder builder)
        {
            var expense = builder.WithCompany(this).Build();
            if (_expenses.Exists(e => e.Id == expense.Id)) throw new ArgumentException("Denne omkostning er allerede tilføjet til firmaet");
            _expenses.Add(expense);
            UpdatedAt = DateTime.UtcNow;
            return expense;
        }
        public void RemoveExpense(Guid expenseId)
        {
            var expense = _expenses.Find(e => e.Id == expenseId && !e.IsDeleted);
            if (expense == null) throw new ArgumentException("Denne omkostning blev ikke fundet i firmaet");
            expense.SoftDelete();
            UpdatedAt = DateTime.UtcNow;
        }
        public IntegrationSetting CreateIntegrationSetting(IntegrationSettingBuilder builder)
        {
            var setting = builder.WithCompany(this).Build();
            if(_settings.Exists(s => s.Provider == setting.Provider && s.Credential.Key == setting.Credential.Key)) throw new ArgumentException("En integrationsindstilling med samme udbyder og nøgle findes allerede for dette firma.");
            if (_settings.Exists(s => s.Id == setting.Id)) throw new ArgumentException("Denne integrationsindstilling er allerede tilføjet til firmaet.");
            _settings.Add(setting);
            UpdatedAt = DateTime.UtcNow;
            return setting;
        }
        public void RemoveIntegrationSetting(Guid settingId)
        {
            var setting = _settings.Find(s => s.Id == settingId);
            if (setting == null) throw new ArgumentException("Denne integrationsindstilling blev ikke fundet for dette firma.");
            _settings.Remove(setting);
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateCompanyName(string newName)
        {
            Guard.AgainstNullOrEmpty(newName, nameof(newName));
            Name = newName;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateCompanyEmail(EmailAddress newEmail)
        {
            Email = newEmail;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
