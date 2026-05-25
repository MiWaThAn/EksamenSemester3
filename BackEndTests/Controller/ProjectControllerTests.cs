using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Xunit;

public class ProjectControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ProjectControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetMyCompanyProjects_ReturnsUnauthorized_WhenNotLoggedIn()
    {
        // Act
        var response = await _client.GetAsync("/api/project/project-company");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/project/company/project/00000000-0000-0000-0000-000000000000")]
    [InlineData("/api/project/employee/projects")]
    public async Task GetEndpoints_ReturnsUnauthorized_WhenNotLoggedIn(string url)
    {
        var response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    [Fact]
    public async Task GetCompanyProjects_ReturnsForbidden_WhenAccessingOtherCompany()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Generer et token for en bruger, der tilhører Company 1
        var myCompanyId = "00000000-0000-0000-0000-000000000001";
        var token = GenerateTestToken(myCompanyId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Vi prøver at tilgå projektet med ID 3 (som i SeedData tilhører Company 9999...)
        var forbiddenProjectId = "00000000-0000-0000-0000-000000000003";

        // Act
        var response = await client.GetAsync($"/api/project/company/project/{forbiddenProjectId}");

        // For at kunne se fejlen hvis den stadig laver 500:
        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            Assert.Fail($"API fejlede med 500: {errorBody}");
        }

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetCompanyProjects_ReturnsOk_WhenAccessingOwnCompany()
    {
        // Arrange
        var myCompanyId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        // Act
        var response = await _client.GetAsync($"/api/project/company/{myCompanyId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    [Fact]
    public async Task GetDetailedProject_ReturnsNotFound_WhenProjectDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/project/company/project/{nonExistentId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDetailedProject_ReturnsOk_WhenProjectExists()
    {
        // Du skal bruge et ID fra en projekt, der rent faktisk findes i din test-DB
        var validId = Guid.Parse("YOUR-VALID-PROJECT-ID-HERE");
        var response = await _client.GetAsync($"/api/project/company/project/{validId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetEmployeeProjects_ReturnsOk_WhenCalledByEmployee()
    {
        var response = await _client.GetAsync("/api/project/employee/projects");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMyCompanyProjects_ReturnsOk_WhenLoggedIn()
    {
        var response = await _client.GetAsync("/api/project/project-company");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // Hjælpemetode til at bygge et token baseret på din appsettings.json
    private string GenerateTestToken(string companyId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        // Nøglen fra din appsettings.json
        var key = Encoding.UTF8.GetBytes("nOdmCjtFHDG/byDmKM5wJ+DOW/snYVaqfaywAMB8FKqzPOaW30PCcEVnPKHRFuIQIG0qOK2wIyoVq+rdDfoMuw==");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim("CompanyId", companyId) // Det her claim skal din controller tjekke imod!
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = "FAST",
            Audience = "Håndværker",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}