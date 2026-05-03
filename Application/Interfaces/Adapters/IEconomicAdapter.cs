using Domain.Entity.Item;
using Domain.Entity.Item.Activity;
using Domain.Entity.Item.Registrations;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Activity = Domain.Entity.Item.Activity.Activity;

namespace Application.Interfaces.Adapters
{
    public interface IEconomicAdapter
    {
        Task<IEnumerable<Project>> GetProjectsAsync(Guid companyId);
        Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId);
        Task<IEnumerable<Activity>> GetActivitiesAsync(Guid companyId);
        Task<IEnumerable<ProjectActivity>> GetProjectActivitiesAsync(Guid companyId);
        Task<IEnumerable<HourRegistration>> GetTimeRegistrationsAsync(Guid companyId);
        Task<IEnumerable<ExpenseRegistration>> GetExpenseRegistrationsAsync(Guid companyId);
        Task<IEnumerable<Registration>> GetAllRegistrationsAsync(Guid companyId);
        Task<IEnumerable<Expense>> GetAllExpenses(Guid companyId);
    }
}

//Udfordringen: Når du trækker data fra e-conomic, har du ofte ikke alle de informationer, som din domæne-constructor kræver (f.eks. et password til en medarbejder eller interne relationer).
//Brug DTO'er til at holde dataen midlertidigt, og lav en adapter, der kan konvertere disse DTO'er til dine domæne-entity'er. Dette giver dig fleksibiliteten til at håndtere manglende data og stadig opretholde en ren arkitektur.
