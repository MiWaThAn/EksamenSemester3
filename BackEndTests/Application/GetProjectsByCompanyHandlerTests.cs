using Application.Commands.Person.Handlers;
using Application.Commands.Person.Queries;
using Application.Interfaces;
using Domain.Builders.Item;
using Domain.Entity.Item;
using Domain.Entity.Person;
using Moq;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BackEndTests.Application
{
    public class GetProjectsByCompanyHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetProjectsByCompanyHandler _handler;

        public GetProjectsByCompanyHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new GetProjectsByCompanyHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_CompanyDoesNotExistOrHasNoProjects_ReturnsEmptyEnumerable()
        {
            // Arrange
            var companyId = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.Companies.GetWithProjectsAsync(companyId))
                          .ReturnsAsync((Company?)null);

            var query = new GetProjectsByCompanyQuery(companyId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsMappedCompanyProjects()
        {
            // Arrange
            var companyId = Guid.NewGuid();

            var dummyCompany = new Company();
            dummyCompany.Id = companyId;

            var builder1 = new ProjectBuilder().WithName("Byggeplads Nord");
            var builder2 = new ProjectBuilder().WithName("Renovering Center");

            var proj1 = dummyCompany.CreateProject(builder1);
            var proj2 = dummyCompany.CreateProject(builder2);

            proj1.Id = Guid.NewGuid();
            proj2.Id = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.Companies.GetWithProjectsAsync(companyId))
                          .ReturnsAsync(dummyCompany);

            var query = new GetProjectsByCompanyQuery(companyId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            var projectList = result.ToList();
            Assert.Equal(2, projectList.Count);

            Assert.Equal(proj1.Id, projectList[0].Id);
            Assert.Equal("Byggeplads Nord", projectList[0].ProjectName);
            Assert.False(projectList[0].IsSelected);
            Assert.Equal(0, projectList[0].NotificationCount);

            Assert.Equal(proj2.Id, projectList[1].Id);
            Assert.Equal("Renovering Center", projectList[1].ProjectName);
            Assert.False(projectList[1].IsSelected);
            Assert.Equal(0, projectList[1].NotificationCount);
        }
    }
}