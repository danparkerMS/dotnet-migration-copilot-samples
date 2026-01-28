using ContosoUniversity.IntegrationTests.TestHelpers;
using FluentAssertions;
using System.Net;
using Xunit;

namespace ContosoUniversity.IntegrationTests.ApiTests
{
    public class StudentsIntegrationTests : IClassFixture<ContosoWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public StudentsIntegrationTests(ContosoWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GET_Students_Index_ReturnsSuccessAndContent()
        {
            // Act
            var response = await _client.GetAsync("/Students");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Students");
        }

        [Fact]
        public async Task GET_Students_Details_WithValidId_ReturnsSuccessAndStudentData()
        {
            // Arrange
            var studentId = 1; // From test data

            // Act
            var response = await _client.GetAsync($"/Students/Details/{studentId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Carson");
            content.Should().Contain("Alexander");
        }

        [Fact]
        public async Task GET_Students_Create_ReturnsCreateForm()
        {
            // Act
            var response = await _client.GetAsync("/Students/Create");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("First Name");
            content.Should().Contain("Last Name");
        }

        [Fact]
        public async Task GET_Students_Index_WithSearchString_FiltersResults()
        {
            // Act
            var response = await _client.GetAsync("/Students?searchString=Alexander");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Alexander");
        }
    }
}
