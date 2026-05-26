using Application.Interfaces;
using Domain.Builders.Item;
using Domain.Builders.Item.Registration;
using Domain.Entity.Item;
using Domain.Entity.Item.Registrations;
using Domain.Services;
using MediatR;
using Shared.Item.Registrations.Commands.Expenses;
using Shared.Item.Registrations.Commands.Time;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations.Expense
{
    public class CreateExpenseHandler : IRequestHandler<CreateExpenseCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateExpenseHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var registrationBuilder = new ExpenseRegistrationBuilder();
                var expenseBuilder = new ExpenseBuilder();
                var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, cancellationToken);
                if (account == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Konto ikke fundet" };
                if(account.EmployeeId == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Konto ikke medarbejderkonto" };
                var worklog = await _unitOfWork.WorkLogs.GetActiveByEmployeeIdAsync(account.EmployeeId.Value, cancellationToken);
                if (worklog == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Du har ikke et aktivt worklog. Start et worklog for at kunne registrere en udgift." };
                var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId, cancellationToken);
                if (project == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Projektet blev ikke fundet." };
                if(request.ProjectActivityId.HasValue)
                {
                    var activity = await _unitOfWork.ProjectActivities.GetByIdAsync(request.ProjectActivityId.Value, cancellationToken);
                    if (activity == null)
                        return new BaseRegistrationResponse { Success = false, Message = "Aktiviteten blev ikke fundet." };
                    registrationBuilder.WithProjectActivity(activity);
                }
                if (request.ExpenseCategoryId.HasValue)
                {
                    var expense = await _unitOfWork.Expenses.GetByIdAsync(request.ExpenseCategoryId.Value, cancellationToken);
                    if (expense == null)
                    {
                        return new BaseRegistrationResponse { Success = false, Message = "Valgte udgift blev ikke fundet." };
                    }
                    registrationBuilder.WithExpense(expense);
                    }
                else if (!string.IsNullOrWhiteSpace(request.NewCategoryName))
                {
                    var emp = await _unitOfWork.Employees.GetByIdAsync(account.EmployeeId.Value,cancellationToken);
                    if(emp == null)
                        return new BaseRegistrationResponse { Success = false, Message = "Medarbejderen blev ikke fundet." };
                    var company = await _unitOfWork.Companies.GetByIdAsync(emp.CompanyId, cancellationToken);
                    if(company == null)
                        return new BaseRegistrationResponse { Success = false, Message = "Virksomheden blev ikke fundet." };
                    var exepense = company.CreateExpense(expenseBuilder.WithName(request.NewCategoryName));
                    await _unitOfWork.Expenses.AddAsync(exepense, cancellationToken);
                    registrationBuilder.WithExpense(exepense);
                }
                else
                {
                    return new BaseRegistrationResponse { Success = false, Message = "Du skal enten vælge en eksisterende kategori eller oprette en ny." };
                }
                registrationBuilder
                    .WithDescription(request.Description)
                    .WithProject(project)
                    .WithStatus(RegistrationStatus.Pending);
                var registration = worklog.CreateRegistration(registrationBuilder);
                await _unitOfWork.ExpenseRegistrations.AddAsync(registration);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new BaseRegistrationResponse { Success = true, Id = registration.Id };
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseRegistrationResponse { Success = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseRegistrationResponse { Success = false, Message = "Der skete en fejl under registreringen af udgiften." };
            }
        }
    }
}