using Microsoft.AspNetCore.Mvc;
using Shared.Item.Registrations.Commands;
using Shared.Item.Registrations.Commands.Expenses;
using Shared.Item.Registrations.Commands.Time;
using Shared.Item.Registrations.Responses;
using Shared.Person.Auth.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using UI.Services.Auth;
using UI.Services.Auth.Registration;

namespace UI.Services.Registration
{
    public class RegistrationService
    {
        private readonly HttpClient _http;
        private readonly JwtAuthStateProvider _authStateProvider;
        private readonly ISecureStorage _secureStorage;
        private readonly PushRegistrationService _pushRegistrationService;
        private const string BasePath = "api/WorkLog";

        public RegistrationService(HttpClient httpClient, JwtAuthStateProvider authStateProvider, ISecureStorage secureStorage, PushRegistrationService pushRegistrationService)
        {
            _http = httpClient;
            _authStateProvider = authStateProvider;
            _secureStorage = secureStorage;
            _pushRegistrationService = pushRegistrationService;
        }

        public async Task<ServiceResult> StartWork(StartWorkCommand command, CancellationToken ct)
        {
            var url = $"{BasePath}/{command.AccountId}/{command.projectId}/{command.projectActivityId}/start-work";
            return await SendPostAsync(url, command, ct);
        }

        public async Task<ServiceResult> TakeBreak(Guid accountId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/take-break";
            return await SendPostAsync<object>(url, null, ct);
        }

        public async Task<ServiceResult> ResumeWork(Guid accountId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/resume-work";
            return await SendPostAsync<object>(url, null, ct);
        }

        public async Task<ServiceResult> SwitchActivity(Guid accountId, Guid projectId, Guid projectActivityId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/{projectId}/{projectActivityId}/switch-activity";
            return await SendPostAsync<object>(url, null, ct);
        }

        public async Task<ServiceResult> SwitchProject(Guid accountId, Guid workLogId, Guid projectId, Guid projectActivityId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/{workLogId}/{projectId}/{projectActivityId}/switch-project";
            return await SendPostAsync<object>(url, null, ct);
        }

        public async Task<ServiceResult> EndWork(Guid accountId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/end-work";
            return await SendPostAsync<object>(url, null, ct);
        }

        public async Task<ServiceResult> ClockOut(Guid accountId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/clock-out";
            return await SendPostAsync<object>(url, null, ct);
        }

        public async Task<ServiceResult> CreateTimeRegistration(Guid accountId, Guid workLogId, ManualTimeRegistrationCommand command, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/{workLogId}/create-time-registration";
            return await SendPostAsync(url, command, ct);
        }

        public async Task<ServiceResult> CreateExpenseRegistration(Guid accountId, Guid workLogId, CreateExpenseCommand command, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/{workLogId}/create-expense-registration";
            return await SendPostAsync(url, command, ct);
        }

        public async Task<ServiceResult> RemoveTimeRegistration(Guid accountId, Guid workLogId, Guid registrationId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/{workLogId}/{registrationId}/remove-time-registration";
            return await SendPostAsync<object>(url, null, ct);
        }

        public async Task<ServiceResult> RemoveExpenseRegistration(Guid accountId, Guid workLogId, Guid registrationId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/{workLogId}/{registrationId}/remove-expense-registration";
            return await SendPostAsync<object>(url, null, ct);
        }

        public async Task<ServiceResult> SubmitWorkLogForApproval(Guid accountId, Guid workLogId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/{workLogId}/submit-work-log-for-approval";
            return await SendPostAsync<object>(url, null, ct);
        }

        public async Task<ServiceResult> ApproveWorkLog(Guid accountId, Guid workLogId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/{workLogId}/approve-work-log";
            return await SendPostAsync<object>(url, null, ct);
        }

        public async Task<ServiceResult> RejectWorkLog(Guid accountId, Guid workLogId, string reason, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/{workLogId}/reject-work-log";
            return await SendPostAsync(url, new { reason }, ct);
        }

        public async Task<ServiceResult> UpdateRegistrationDescription(Guid accountId, Guid workLogId, Guid registrationId, string description, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/{workLogId}/{registrationId}/update-registration-description";
            return await SendPostAsync(url, new { description }, ct);
        }

        public async Task<ServiceResult> UpdateTimeRegistrationInterval(Guid accountId, Guid workLogId, Guid registrationId, Guid timeIntervalId, DateTime start, DateTime end, bool isBreak, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/{workLogId}/{registrationId}/{timeIntervalId}/update-time-registration-interval";
            var payload = new { start, end, isBreak };
            return await SendPostAsync(url, payload, ct);
        }

        public async Task<ServiceResult> UpdateRegistrationProject(Guid accountId, Guid workLogId, Guid registrationId, Guid newProjectId, Guid newProjectActivityId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/{workLogId}/{registrationId}/update-registration-project";
            var payload = new { NewProjectId = newProjectId, NewProjectActivityId = newProjectActivityId };
            return await SendPostAsync(url, payload, ct);
        }

        public async Task<ServiceResult> UpdateRegistrationActivity(Guid workLogId, Guid accountId, Guid registrationId, Guid newProjectActivityId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/{workLogId}/{registrationId}/update-registration-activity";
            var payload = new { NewProjectActivityId = newProjectActivityId };
            return await SendPostAsync(url, payload, ct);
        }

        public async Task<ServiceResult> UpdateRegistrationExpense(Guid accountId, Guid registrationId, Guid? newExpenseId, DateTime? date, decimal? amount, string? description, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/{registrationId}/update-registration-expense";
            var payload = new { NewExpenseId = newExpenseId, Date = date, Amount = amount, Description = description };
            return await SendPostAsync(url, payload, ct);
        }

        private async Task<ServiceResult> SendPostAsync<T>(string url, T payload, CancellationToken ct)
        {
            try
            {
                HttpResponseMessage response;
                if (payload == null)
                {
                    response = await _http.PostAsync(url, null, ct);
                }
                else
                {
                    response = await _http.PostAsJsonAsync(url, payload, ct);
                }

                if (response.IsSuccessStatusCode)
                {
                    var successData = await response.Content.ReadFromJsonAsync<ApiResponseData>(cancellationToken: ct);
                    return ServiceResult.Ok(successData?.Id);
                }

                var errorData = await response.Content.ReadFromJsonAsync<ProblemDetailsDto>(cancellationToken: ct);
                return ServiceResult.Failure(errorData?.Detail ?? "Fejl ved registrering.");
            }
            catch (Exception)
            {
                return ServiceResult.Failure("Kunne ikke oprette forbindelse til serveren.");
            }
        }
    }
}