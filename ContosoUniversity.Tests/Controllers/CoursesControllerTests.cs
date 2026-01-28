using System;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Controllers;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Services;
using ContosoUniversity.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ContosoUniversity.Tests.Controllers
{
    public class CoursesControllerTests : IDisposable
    {
        private readonly SchoolContext _context;
        private readonly NotificationService _notificationService;
        private readonly CoursesController _controller;

        public CoursesControllerTests()
        {
            // Use a unique database name for each test class instance to avoid conflicts
            var dbName = Guid.NewGuid().ToString();
            _context = InMemoryDbContextFactory.Create(dbName);
            
            // Seed test data with ChangeTracker.Clear() between operations
            _context.Instructors.AddRange(TestDataBuilder.GetTestInstructors());
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            _context.Departments.AddRange(TestDataBuilder.GetTestDepartments());
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            _context.Courses.AddRange(TestDataBuilder.GetTestCourses());
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            var mockConfiguration = new Mock<IConfiguration>();
            _notificationService = new NotificationService(mockConfiguration.Object);
            _controller = new CoursesController(_context, _notificationService);
        }

        public void Dispose()
        {
            _context?.Dispose();
            _notificationService?.Dispose();
            _controller?.Dispose();
        }

        [Fact]
        public void Index_ReturnsViewWithCourses()
        {
            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as System.Collections.Generic.List<Course>;
            model.Should().NotBeNull();
            model.Count.Should().BeGreaterThan(0);
            model.Should().OnlyContain(c => c.Department != null, "courses should be loaded with Department");
        }

        [Fact]
        public void Details_WithValidId_ReturnsViewWithCourse()
        {
            // Arrange
            var course = _context.Courses.First();

            // Act
            var result = _controller.Details(course.CourseID) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as Course;
            model.Should().NotBeNull();
            model.CourseID.Should().Be(course.CourseID);
            model.Department.Should().NotBeNull("Department should be loaded");
        }

        [Fact]
        public void Details_WithNullId_ReturnsBadRequest()
        {
            // Act
            var result = _controller.Details(null);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            var statusResult = result as StatusCodeResult;
            statusResult.StatusCode.Should().Be(400);
        }

        [Fact]
        public void Details_WithInvalidId_ThrowsInvalidOperationException()
        {
            // Arrange
            var invalidId = 99999;

            // Act & Assert
            // The controller uses .Single() which throws when no element is found
            var act = () => _controller.Details(invalidId);
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Create_Get_ReturnsViewWithNewCourse()
        {
            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result!.Model as Course;
            model.Should().NotBeNull();
            Assert.NotNull(_controller.ViewBag.DepartmentID);
        }

        [Fact]
        public async Task Create_Post_WithValidCourse_RedirectsToIndex()
        {
            // Arrange
            var department = _context.Departments.First();
            var newCourse = new Course
            {
                CourseID = 9999,
                Title = "New Test Course",
                Credits = 3,
                DepartmentID = department.DepartmentID
            };
            _controller.ModelState.Clear();

            // Act
            var result = await _controller.Create(newCourse, null) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be("Index");
            _context.Courses.Should().Contain(c => c.CourseID == 9999 && c.Title == "New Test Course");
        }

        [Fact]
        public async Task Create_Post_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var invalidCourse = new Course(); // Missing required fields
            _controller.ModelState.AddModelError("Title", "Required");

            // Act
            var result = await _controller.Create(invalidCourse, null) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.Model.Should().Be(invalidCourse);
            Assert.NotNull(_controller.ViewBag.DepartmentID);
        }

        [Fact]
        public void Edit_Get_WithValidId_ReturnsViewWithCourse()
        {
            // Arrange
            var course = _context.Courses.First();

            // Act
            var result = _controller.Edit(course.CourseID) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result!.Model as Course;
            model.Should().NotBeNull();
            model!.CourseID.Should().Be(course.CourseID);
            Assert.NotNull(_controller.ViewBag.DepartmentID);
        }

        [Fact]
        public void Edit_Get_WithNullId_ReturnsBadRequest()
        {
            // Act
            var result = _controller.Edit((int?)null);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            var statusResult = result as StatusCodeResult;
            statusResult.StatusCode.Should().Be(400);
        }

        [Fact]
        public void Edit_Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidId = 99999;

            // Act
            var result = _controller.Edit(invalidId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void Delete_Get_WithValidId_ReturnsViewWithCourse()
        {
            // Arrange
            var course = _context.Courses.First();

            // Act
            var result = _controller.Delete(course.CourseID) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as Course;
            model.Should().NotBeNull();
            model.CourseID.Should().Be(course.CourseID);
            model.Department.Should().NotBeNull("Department should be loaded");
        }

        [Fact]
        public void Delete_Get_WithNullId_ReturnsBadRequest()
        {
            // Act
            var result = _controller.Delete((int?)null);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            var statusResult = result as StatusCodeResult;
            statusResult.StatusCode.Should().Be(400);
        }

        [Fact]
        public void Delete_Get_WithInvalidId_ThrowsInvalidOperationException()
        {
            // Arrange
            var invalidId = 99999;

            // Act & Assert
            // The controller uses .Single() which throws when no element is found
            var act = () => _controller.Delete(invalidId);
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void DeleteConfirmed_WithValidId_DeletesCourseAndRedirects()
        {
            // Arrange
            var course = _context.Courses.First();
            var courseId = course.CourseID;

            // Act
            var result = _controller.DeleteConfirmed(courseId) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            _context.Courses.Should().NotContain(c => c.CourseID == courseId);
        }
    }
}
