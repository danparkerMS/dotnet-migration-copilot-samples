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
│   └── PaginatedListTests.cs (5 tests)
├── Services/
│   └── NotificationServiceTests.cs (13 tests)
├── Models/
│   ├── StudentValidationTests.cs (38 tests)
│   ├── CourseValidationTests.cs (26 tests)
│   ├── DepartmentValidationTests.cs (20 tests)
│   ├── InstructorValidationTests.cs (38 tests)
│   └── OfficeAssignmentValidationTests.cs (18 tests)
├── Controllers/
│   ├── StudentsControllerTests.cs (17 tests)
│   ├── CoursesControllerTests.cs (14 tests)
│   ├── DepartmentsControllerTests.cs (15 tests)
│   ├── InstructorsControllerTests.cs (19 tests)
│   ├── HomeControllerTests.cs (5 tests)
│   └── NotificationsControllerTests.cs (3 tests)
└── TestHelpers/
    ├── InMemoryDbContextFactory.cs
    └── TestDataBuilder.cs

ContosoUniversity.IntegrationTests/
├── ApiTests/
│   └── StudentsIntegrationTests.cs (4 tests)
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

### Models (Validation)
- ✅ **StudentValidationTests** - 38 tests
  - Required field validation (LastName, FirstMidName)
  - StringLength constraints (max 50 characters)
  - EnrollmentDate range validation (1753-9999)
  - FullName computed property
- ✅ **CourseValidationTests** - 26 tests
  - Title StringLength (3-50 characters)
  - Credits range validation (0-5)
  - TeachingMaterialImagePath max length
- ✅ **DepartmentValidationTests** - 20 tests
  - Name StringLength (3-50 characters)
  - Budget validation
  - StartDate validation
- ✅ **InstructorValidationTests** - 38 tests
  - Required field validation (LastName, FirstMidName)
  - StringLength constraints (max 50 characters)
  - HireDate range validation (1753-9999)
  - FullName computed property
- ✅ **OfficeAssignmentValidationTests** - 18 tests
  - Location StringLength (max 50 characters)
  - Null location handling

**Total Model Validation Tests: 140**

### Controllers
- ✅ **StudentsController** - 17 tests
  - Index with search, sorting, pagination
  - Details (valid/invalid/null IDs)
  - Create GET/POST
  - Edit GET
  - Delete GET/POST
- ✅ **CoursesController** - 14 tests
  - Index with courses and departments
  - Details (valid/invalid/null IDs)
  - Create GET/POST
  - Edit GET
  - Delete GET/POST
- ✅ **DepartmentsController** - 15 tests
  - Index with administrator information
  - Details (valid/invalid/null IDs)
  - Create GET/POST with instructor selection
  - Edit GET
  - Delete GET/POST
- ✅ **InstructorsController** - 19 tests
  - Index with filtering and course assignments
  - Details (valid/invalid/null IDs)
  - Create GET/POST with course assignments
  - Edit GET
  - Delete GET/POST with department handling
- ✅ **HomeController** - 5 tests
  - Index, About, Contact, Error, Unauthorized pages
- ✅ **NotificationsController** - 3 tests
  - Index dashboard
  - GetNotifications API
  - MarkAsRead API

**Total Controller Tests: 73**

### Integration Tests
- ✅ **Students API** - 4 tests
  - Index, Details, Create endpoints
  - Search functionality

**TOTAL: 199 TESTS** (195 unit + 4 integration)

## Known Issues

None! All 95 tests are passing.

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
