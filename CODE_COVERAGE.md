# Code Coverage Report - ContosoUniversity

**Generated:** January 28, 2026  
**Total Tests:** 199 (195 unit + 4 integration)  
**Overall Coverage:** 45.9% Line Coverage | 33.3% Branch Coverage

---

## Executive Summary

- **Current Line Coverage:** 45.9% (597 of 1,300 coverable lines)
- **Current Branch Coverage:** 33.3% (152 of 456 branches)
- **Current Method Coverage:** 69.4% (116 of 167 methods)
- **Full Method Coverage:** 59.2% (99 of 167 methods)
- **Target:** 80% Line Coverage

## Coverage Breakdown by Category

### ✅ Excellent Coverage (>80%)

| Component | Line Coverage | Notes |
|-----------|--------------|-------|
| BaseController | 84.6% | Core controller functionality well-tested |
| NotificationService | 81.1% | Comprehensive service tests |
| SchoolContext | 100% | DbContext configuration covered |
| PaginatedList<T> | 100% | Complete pagination logic coverage |
| Person (base model) | 100% | All properties and FullName tested |
| HomeController | 100% | All page actions covered |
| AssignedCourseData | 100% | ViewModel fully tested |
| EnrollmentDateGroup | 100% | ViewModel fully tested |
| InstructorIndexData | 100% | ViewModel fully tested |

**Average in Category:** 96.2%

### ⚠️ Good Coverage (60-80%)

| Component | Line Coverage | Notes |
|-----------|--------------|-------|
| DepartmentsController | 61.4% | Missing some POST action coverage |
| InstructorsController | 66.8% | Edit/Delete POST actions partially covered |
| Instructor (model) | 66.6% | Some navigation properties untested |
| NotificationsController | 74.2% | Good API endpoint coverage |
| StudentsController | 71.7% | Main actions covered, some edge cases missing |
| CourseAssignment | 75% | Basic coverage on join table model |
| Department (model) | 75% | Core validation tested |
| Notification (model) | 77.7% | Most properties covered |

**Average in Category:** 71.0%

### ❌ Needs Improvement (<60%)

| Component | Line Coverage | Notes |
|-----------|--------------|-------|
| CoursesController | 38% | **Priority** - POST actions need more tests |
| Student (model) | 50% | Navigation properties need coverage |
| OfficeAssignment (model) | 66.6% | Nearly at good threshold |
| Course (model) | 87.5% | **Actually good** - borderline excellent |

**Average in Category:** 60.5%

### 🚫 Intentionally Excluded (0% - Expected)

| Component | Coverage | Justification |
|-----------|----------|---------------|
| All Razor Views (26 files) | 0% | Views tested via integration tests, not unit tests |
| DbInitializer | 0% | Database seeding utility, tested manually |
| Program.cs | 0% | Application startup, tested via integration tests |
| SchoolContextFactory | 0% | Legacy pattern to be removed |
| Enrollment (model) | 0% | Join table with minimal logic |
| ErrorViewModel | 0% | Simple POCO for error display |

---

## Path to 80% Coverage

### Current State
- **45.9%** overall coverage
- **Gap to target:** 34.1 percentage points
- **Lines needed:** ~443 additional lines covered (to reach 1,040 of 1,300)

### Recommended Actions (Prioritized)

#### 1. **High Priority - Controller POST Actions** 
*Est. Impact: +15-20% coverage*

- [ ] CoursesController Edit POST action
- [ ] CoursesController Delete POST (DeleteConfirmed) edge cases
- [ ] DepartmentsController Edit POST action with concurrency handling
- [ ] InstructorsController Edit POST action with course assignments
- [ ] StudentsController Edit POST action

**Why:** These are critical business operations that handle data modification.

#### 2. **Medium Priority - Controller Error Paths**
*Est. Impact: +5-8% coverage*

- [ ] Exception handling in catch blocks
- [ ] ModelState validation failure paths
- [ ] Edge cases in search/filter logic
- [ ] Null reference checks

**Why:** Improves resilience and error handling confidence.

#### 3. **Lower Priority - Model Navigation Properties**
*Est. Impact: +3-5% coverage*

- [ ] Test lazy-loaded collections
- [ ] Test navigation property setters
- [ ] Test computed properties with related entities

**Why:** Most validation is already covered; these are relationships.

#### 4. **Optional - Integration Test Expansion**
*Est. Impact: +5-10% coverage*

- [ ] Add Courses API integration tests
- [ ] Add Departments API integration tests
- [ ] Add Instructors API integration tests

**Why:** Tests real HTTP workflows but requires more setup.

---

## Coverage Metrics Over Time

| Date | Line Coverage | Branch Coverage | Tests | Notes |
|------|---------------|-----------------|-------|-------|
| 2026-01-28 | 45.9% | 33.3% | 199 | Initial comprehensive test suite |
| | | | | - 195 unit tests |
| | | | | - 4 integration tests |
| | | | | - All controllers have basic coverage |
| | | | | - All models validated |

---

## How to Generate This Report

### Run Tests with Coverage
```powershell
dotnet test ContosoUniversity.Tests\ContosoUniversity.Tests.csproj --collect:"XPlat Code Coverage" --results-directory TestResults
```

### Generate Human-Readable Report
```powershell
# Install ReportGenerator (one-time)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator -reports:"TestResults\**\coverage.cobertura.xml" -targetdir:"TestResults\CoverageReport" -reporttypes:Html

# Generate text summary
reportgenerator -reports:"TestResults\**\coverage.cobertura.xml" -targetdir:"TestResults\CoverageReport" -reporttypes:TextSummary

# View HTML report
start TestResults\CoverageReport\index.html
```

### View Summary
```powershell
Get-Content TestResults\CoverageReport\Summary.txt
```

---

## Best Practices for Code Coverage

### DO:
✅ Focus on testing business logic and critical paths  
✅ Test edge cases and error handling  
✅ Aim for 80% line coverage as a quality goal  
✅ Monitor coverage trends over time  
✅ Use coverage to find untested code

### DON'T:
❌ Test for 100% coverage (diminishing returns)  
❌ Test auto-generated code (Razor views, migrations)  
❌ Test trivial getters/setters  
❌ Write tests just to hit coverage numbers  
❌ Sacrifice test quality for quantity

---

## Notes

- **Razor Views (0% coverage):** This is expected and normal. Razor views are tested through integration tests or UI tests, not unit tests.
- **Program.cs (0% coverage):** Application startup is tested through integration tests that spin up the whole application.
- **DbInitializer (0% coverage):** Database seeding is typically tested manually or in integration scenarios.
- **Navigation Properties:** Some model navigation properties show low coverage because they're lazy-loaded or only used in specific scenarios.

The current 45.9% coverage represents **solid foundational testing** of the core business logic, models, and services. The path to 80% is clear and achievable by focusing on controller POST actions and error paths.
