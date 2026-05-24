using Application.Commands.Person.Queries;
using Application.Interfaces;
using MediatR;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Person.Handlers
{
    internal class GetDetailedProjectHandler : IRequestHandler<GetDetailedProjectQuery, DetailedProjectModel?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDetailedProjectHandler(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task<DetailedProjectModel?> Handle(GetDetailedProjectQuery request, CancellationToken ct)
        {
            // 1. Hent projektet fra databasen
            var project = await _unitOfWork.Projects.GetByIdWithDetailsAsync(request.ProjectId);

            if (project == null)
                return null;

            // 2. Hent de medarbejdere, der er tilknyttet projektet
            var relatedEmployees = await _unitOfWork.Employees.GetEmployeesRelatedToProjectAsync(request.ProjectId);

            return new DetailedProjectModel
            {
                Id = project.Id,
                ProjectName = project.Name,

                // Hvis jeres Project-entitet mangler Description eller Customer, kan du bare udkommentere linjerne
                Description = project.Description ?? "Ingen beskrivelse tilgængelig.",
                Status = project.Status.ToString() ?? "Åben",
                CustomerName = project.Customer?.Name ?? "Ikke angivet",

                // MAP LISTEN AF AKTIVITETER BASERET PÅ JERES RIGTIGE DOMÆNEMODEL:
                Activities = project.Activities?.Select(a => new ProjectActivityModel
                {
                    Id = a.Id,

                    // HER ER MAGIEN: Vi henter navnet fra den tilknyttede Activity-klasse.
                    // Hvis 'Name' brokker sig under Activity, så prøv 'a.Activity?.Title'
                    ActivityName = a.Activity?.Name ?? "Navnløs aktivitet",

                    // Vi laver et pænt aktivitetsnummer ud fra de første 5 tegn af ActivityId
                    ActivityNumber = a.ActivityId.ToString().Substring(0, 5).ToUpper(),

                    // Bruger .ToString() på jeres Status Enum, så den matcher Blazors string-forventning
                    Status = a.Status.ToString(),
                    StatusText = a.Status.ToString(),

                    // Vi sætter et standard estimat på 20 timer, eller beregner det ud fra datoerne (Slutdato minus Startdato i timer)
                    TimeEstimate = (a.EndDate - a.StartDate).TotalHours > 0
                        ? Math.Round((a.EndDate - a.StartDate).TotalHours, 1)
                        : 20.0
                }).ToList() ?? new List<ProjectActivityModel>(),

                Employees = relatedEmployees?.Select(e => new CompanyEmployeeModel
                {
                    Id = e.Id,
                    FullName = e.Name
                }).ToList() ?? new List<CompanyEmployeeModel>()
            };
        }
    }
}