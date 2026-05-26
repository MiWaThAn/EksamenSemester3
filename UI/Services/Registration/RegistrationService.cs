using Microsoft.AspNetCore.Mvc;
using Shared.Item.ProjectActivity;
using Shared.Item.Registrations.Commands;
using Shared.Item.Registrations.Commands.Expenses;
using Shared.Item.Registrations.Commands.Time;
using Shared.Item.Registrations.DTOs;
using Shared.Item.Registrations.Responses;
using Shared.Person.Auth.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UI.Services.Registration
{
    public class RegistrationService
    {
        private readonly HttpClient _http;
        private const string BasePath = "api/worklog";

        public RegistrationService(HttpClient httpClient)
        {
            _http = httpClient;
        }

        //QUERIES (GET)

        public async Task<WorkLogDto?> GetActiveWorkLog(Guid accountId, CancellationToken ct)
        {
            try
            {
                return await _http.GetFromJsonAsync<WorkLogDto>($"{BasePath}/active/{accountId}", ct);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<WorkLogDto>?> GetWorkLogHistory(Guid accountId, CancellationToken ct)
        {
            return await _http.GetFromJsonAsync<IEnumerable<WorkLogDto>>($"{BasePath}/history/{accountId}", ct);
        }

        public async Task<WorkLogDto?> GetWorkLogById(Guid workLogId, CancellationToken ct)
        {
            return await _http.GetFromJsonAsync<WorkLogDto>($"{BasePath}/{workLogId}", ct);
        }

        public async Task<IEnumerable<WorkLogDto>?> GetPendingWorkLogs(Guid accountId, CancellationToken ct)
        {
            return await _http.GetFromJsonAsync<IEnumerable<WorkLogDto>>($"{BasePath}/pending-approval/{accountId}", ct);
        }

        public async Task<IEnumerable<ProjectActivityDto>?> GetProjectActivities(Guid ProjectId, CancellationToken ct)
        {
            return await _http.GetFromJsonAsync<IEnumerable<ProjectActivityDto>>($"api/projects/{ProjectId}/activities/for-project", ct);
        }

        public async Task<IEnumerable<ProjectDto>?> GetProjects(CancellationToken ct)
        {
            return await _http.GetFromJsonAsync<IEnumerable<ProjectDto>>($"api/project/employees-company-projects", ct);
        }

        //COMMANDS

        public async Task<ServiceResult> StartWork(StartWorkCommand command, CancellationToken ct)
        {
            var url = $"{BasePath}/{command.AccountId}/start-work/{command.projectId}/{command.projectActivityId}";
            return await SendRequestAsync(HttpMethod.Post, url, command, ct);
        }

        public async Task<ServiceResult> TakeBreak(Guid accountId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/take-break";
            return await SendRequestAsync<object>(HttpMethod.Post, url, null, ct);
        }

        public async Task<ServiceResult> ResumeWork(Guid accountId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/resume-work";
            var result = await SendRequestAsync<object>(HttpMethod.Post, url, null, ct);
            return result;
        }

        public async Task<ServiceResult> SwitchActivity(Guid accountId, Guid projectId, Guid projectActivityId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/switch-activity/{projectId}/{projectActivityId}";
            return await SendRequestAsync<object>(HttpMethod.Post, url, null, ct);
        }

        public async Task<ServiceResult> SwitchProject(Guid accountId, Guid workLogId, Guid projectId, Guid projectActivityId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/switch-project/{workLogId}/{projectId}/{projectActivityId}";
            return await SendRequestAsync<object>(HttpMethod.Post, url, null, ct);
        }

        public async Task<ServiceResult> EndWork(Guid accountId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/end-work";
            return await SendRequestAsync<object>(HttpMethod.Post, url, null, ct);
        }

        public async Task<ServiceResult> ClockOut(Guid accountId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/clock-out";
            return await SendRequestAsync<object>(HttpMethod.Post, url, null, ct);
        }

        public async Task<ServiceResult> CreateTimeRegistration(Guid accountId, Guid workLogId, ManualTimeRegistrationCommand command, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/worklog/{workLogId}/time-registration";
            return await SendRequestAsync(HttpMethod.Post, url, command, ct);
        }

        public async Task<ServiceResult> CreateExpenseRegistration(Guid accountId, Guid workLogId, CreateExpenseCommand command, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/worklog/{workLogId}/expense-registration";
            return await SendRequestAsync(HttpMethod.Post, url, command, ct);
        }

        public async Task<ServiceResult> RemoveTimeRegistration(Guid accountId, Guid workLogId, Guid registrationId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/worklog/{workLogId}/time-registration/{registrationId}";
            return await SendRequestAsync<object>(HttpMethod.Delete, url, null, ct);
        }

        public async Task<ServiceResult> RemoveExpenseRegistration(Guid accountId, Guid workLogId, Guid registrationId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/worklog/{workLogId}/expense-registration/{registrationId}";
            return await SendRequestAsync<object>(HttpMethod.Delete, url, null, ct);
        }

        public async Task<ServiceResult> SubmitWorkLogForApproval(Guid accountId, Guid workLogId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/worklog/{workLogId}/submit";
            return await SendRequestAsync<object>(HttpMethod.Post, url, null, ct);
        }

        public async Task<ServiceResult> ApproveWorkLog(Guid accountId, Guid workLogId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/worklog/{workLogId}/approve";
            return await SendRequestAsync<object>(HttpMethod.Post, url, null, ct);
        }

        public async Task<ServiceResult> RejectWorkLog(Guid accountId, Guid workLogId, string reason, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/worklog/{workLogId}/reject";
            return await SendRequestAsync(HttpMethod.Post, url, new { reason }, ct);
        }

        public async Task<ServiceResult> UpdateRegistrationDescription(Guid accountId, Guid workLogId, Guid registrationId, string description, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/worklog/{workLogId}/registration/{registrationId}/description";
            return await SendRequestAsync(HttpMethod.Put, url, new { description }, ct);
        }

        public async Task<ServiceResult> UpdateTimeRegistrationInterval(Guid accountId, Guid workLogId, Guid registrationId, Guid timeIntervalId, DateTime start, DateTime end, bool isBreak, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/worklog/{workLogId}/registration/{registrationId}/interval/{timeIntervalId}";
            var payload = new { start, end, isBreak };
            return await SendRequestAsync(HttpMethod.Put, url, payload, ct);
        }

        public async Task<ServiceResult> UpdateRegistrationProject(Guid accountId, Guid workLogId, Guid registrationId, Guid newProjectId, Guid newProjectActivityId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/worklog/{workLogId}/registration/{registrationId}/project";
            var payload = new { NewProjectId = newProjectId, NewProjectActivityId = newProjectActivityId };
            return await SendRequestAsync(HttpMethod.Put, url, payload, ct);
        }

        public async Task<ServiceResult> UpdateRegistrationActivity(Guid workLogId, Guid accountId, Guid registrationId, Guid newProjectActivityId, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/worklog/{workLogId}/registration/{registrationId}/activity";
            var payload = new { NewProjectActivityId = newProjectActivityId };
            return await SendRequestAsync(HttpMethod.Put, url, payload, ct);
        }

        public async Task<ServiceResult> UpdateRegistrationExpense(Guid accountId, Guid registrationId, Guid? newExpenseId, DateTime? date, decimal? amount, string? description, CancellationToken ct)
        {
            var url = $"{BasePath}/{accountId}/registration/{registrationId}/expense";
            var payload = new { NewExpenseId = newExpenseId, Date = date, Amount = amount, Description = description };
            return await SendRequestAsync(HttpMethod.Put, url, payload, ct);
        }

//HELPER METHOD

        private async Task<ServiceResult> SendRequestAsync<T>(HttpMethod method, string url, T payload, CancellationToken ct)
        {
            try
            {
                using var request = new HttpRequestMessage(method, url);

                if (payload != null)
                {
                    request.Content = JsonContent.Create(payload);
                }

                var response = await _http.SendAsync(request, ct);

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