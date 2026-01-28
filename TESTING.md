# Testing Guide for ContosoUniversity

## Overview
This document describes the testing infrastructure for the ContosoUniversity application, including how to run tests, write new tests, and understand the testing conventions.

## Test Projects

### ContosoUniversity.Tests
Unit tests for testing individual components in isolation.
- **Framework**: xUnit
- **Mocking**: Moq
- **Assertions**: FluentAssertions  
- **Database**: EF Core In-Memory

### ContosoUniversity.IntegrationTests
Integration tests for testing complete workflows and API endpoints.
- **Framework**: xUnit
- **Testing Host**: WebApplicationFactory
- **Database**: EF Core In-Memory

## Running Tests

### Run All Tests
```powershell
dotnet test
```

### Run Specific Test Project
```powershell
dotnet test ContosoUniversity.Tests\ContosoUniversity.Tests.csproj
dotnet test ContosoUniversity.IntegrationTests\ContosoUniversity.IntegrationTests.csproj
```

### Run with Code Coverage
```powershell
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

### Run Specific Test
```powershell
dotnet test --filter "FullyQualifiedName~NotificationServiceTests"
```

## Test Organization

```
ContosoUniversity.Tests/
├── Data/
│   └── PaginatedListTests.cs
├── Services/
│   └── NotificationServiceTests.cs
├── Controllers/
│   └── (Future controller tests)
└── TestHelpers/
    ├── InMemoryDbContextFactory.cs
    └── TestDataBuilder.cs

ContosoUniversity.IntegrationTests/
├── ApiTests/
│   └── StudentsIntegrationTests.cs
└── TestHelpers/
    ├── ContosoWebApplicationFactory.cs
    └── TestDataBuilder.cs
```

## Writing Unit Tests

### Test Naming Convention
```csharp
[MethodName]_[Scenario]_[ExpectedResult]
```

**Examples:**
- `SendNotification_WithValidData_CreatesNotification`
- `Create_EmptySource_ReturnsEmptyPaginatedList`

### AAA Pattern
All tests follow the Arrange-Act-Assert pattern:

```csharp
[Fact]
public void SendNotification_WithValidData_CreatesNotification()
{
    // Arrange
    var entityType = "Student";
    var entityId = "123";
    var operation = EntityOperation.CREATE;

    // Act
    _notificationService.SendNotification(entityType, entityId, operation);
    var notifications = _notificationService.GetNotifications();

    // Assert
    notifications.Should().HaveCount(1);
    notification.EntityType.Should().Be(entityType);
}
```

### Using Test Data Builders

```csharp
// Create test entities
var student = TestDataBuilder.CreateStudent("John", "Doe");
var department = TestDataBuilder.CreateDepartment("Engineering");

// Get collections
var students = TestDataBuilder.GetTestStudents();
var courses = TestDataBuilder.GetTestCourses();
```

### Using In-Memory Database

```csharp
[Fact]
public void Example_Test_WithDatabase()
{
    // Arrange
    using var context = InMemoryDbContextFactory.CreateWithData();
    
    // Act
    var students = context.Students.ToList();
    
    // Assert
    students.Should().HaveCount(3);
}
```

## Writing Integration Tests

### Basic Structure

```csharp
public class StudentsIntegrationTests : IClassFixture<ContosoWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public StudentsIntegrationTests(ContosoWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_Students_Index_ReturnsSuccess()
    {
        // Act
        var response = await _client.GetAsync("/Students");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

## Test Helpers

### InMemoryDbContextFactory
Creates in-memory database contexts for testing:

```csharp
// Empty database
var context = InMemoryDbContextFactory.Create();

// Pre-seeded with test data
var context = InMemoryDbContextFactory.CreateWithData();

// Named database (for sharing across tests)
var context = InMemoryDbContextFactory.Create("MyTestDb");
```

### TestDataBuilder
Creates consistent test data:

```csharp
var student = TestDataBuilder.CreateStudent("John", "Doe");
var students = TestDataBuilder.GetTestStudents(); // Returns 3 sample students
var courses = TestDataBuilder.GetTestCourses();
var departments = TestDataBuilder.GetTestDepartments();
```

### ContosoWebApplicationFactory
Configures the application for integration testing with an in-memory database.

## Testing Best Practices

### DO:
✅ Test one thing per test
✅ Use descriptive test names
✅ Follow AAA pattern
✅ Use FluentAssertions for readable assertions
✅ Keep tests independent (no shared state)
✅ Use test data builders for consistency

### DON'T:
❌ Test framework code (e.g., EF Core internals)
❌ Make tests depend on each other
❌ Use magic numbers without explanation
❌ Test implementation details
❌ Write tests that depend on external resources

## Current Test Coverage

### Services
- ✅ **NotificationService** - 13 tests
  - Send/Receive/Get notifications
  - Message generation
  - Queue behavior
  - User tracking

### Data
- ✅ **PaginatedList** - 5 tests
  - Pagination logic
  - Page boundaries
  - Empty results

### Integration Tests
- 🔨 **Students API** - 4 tests (infrastructure in progress)
  - Index, Details, Create endpoints
  - Search functionality

## Known Issues

### Integration Tests
The WebApplicationFactory currently has a conflict between SQL Server and In-Memory database providers. This is being resolved. Unit tests are fully functional.

## Adding New Tests

### 1. Create Test Class
```csharp
namespace ContosoUniversity.Tests.Services
{
    public class MyNewServiceTests
    {
        [Fact]
        public void MyMethod_Scenario_ExpectedResult()
        {
            // Arrange
            // Act
            // Assert
        }
    }
}
```

### 2. Add Test Data If Needed
Update `TestDataBuilder` with any new test data factories.

### 3. Run Tests
```powershell
dotnet test
```

## Continuous Integration

Tests are designed to run in CI/CD pipelines:
- Fast execution (all unit tests < 5 seconds)
- No external dependencies
- Consistent results
- Clear failure messages

## Future Enhancements

- [ ] Complete integration test implementation
- [ ] Add controller unit tests
- [ ] Add model validation tests
- [ ] Implement performance benchmarks
- [ ] Add code coverage reporting to CI/CD
- [ ] Add mutation testing (Stryker.NET)
- [ ] Add UI tests with Playwright

## Resources

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Moq Documentation](https://github.com/moq/moq4)
- [EF Core Testing](https://learn.microsoft.com/en-us/ef/core/testing/)
