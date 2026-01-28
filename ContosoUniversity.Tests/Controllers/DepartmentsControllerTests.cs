using System;
using System.Collections.Generic;
using System.Linq;
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
    public class DepartmentsControllerTests : IDisposable
    {
        private readonly SchoolContext _context;
        private readonly NotificationService _notificationService;
        private readonly DepartmentsController _controller;

        public DepartmentsControllerTests()
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

            _context.Students.AddRange(TestDataBuilder.GetTestStudents());
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            var mockConfiguration = new Mock<IConfiguration>();
            _notificationService = new NotificationService(mockConfiguration.Object);
            _controller = new DepartmentsController(_context, _notificationService);
        }

        public void Dispose()
        {
            _context?.Dispose();
            _notificationService?.Dispose();
            _controller?.Dispose();
        }

        [Fact]
        public void Index_ReturnsViewWithDepartments()
        {
            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as List<Department>;
            model.Should().NotBeNull();
            model.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Index_IncludesAdministrator()
        {
            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as List<Department>;
            model.Should().NotBeNull();
            model.Should().OnlyContain(d => d.Administrator != null);
        }

        [Fact]
        public void Details_WithValidId_ReturnsViewWithDepartment()
        {
            // Arrange
            var department = _context.Departments.First();

            // Act
            var result = _controller.Details(department.DepartmentID) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as Department;
            model.Should().NotBeNull();
            model.DepartmentID.Should().Be(department.DepartmentID);
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
        public void Details_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidId = 99999;

            // Act
            var result = _controller.Details(invalidId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void Create_Get_ReturnsViewWithInstructorSelectList()
        {
            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            Assert.NotNull(_controller.ViewBag.InstructorID);
        }

        [Fact]
        public void Create_Post_WithValidDepartment_RedirectsToIndex()
        {
            // Arrange
            var newDepartment = new Department
            {
                Name = "Computer Science",
                Budget = 500000,
                StartDate = DateTime.Today,
                InstructorID = 1
            };
            _controller.ModelState.Clear();

            // Act
            var result = _controller.Create(newDepartment) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            _context.Departments.Should().Contain(d => d.Name == "Computer Science");
        }

        [Fact]
        public void Create_Post_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var invalidDepartment = new Department(); // Missing required fields
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = _controller.Create(invalidDepartment) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().Be(invalidDepartment);
            Assert.NotNull(_controller.ViewBag.InstructorID);
        }

        [Fact]
        public void Edit_Get_WithValidId_ReturnsViewWithDepartment()
        {
            // Arrange
            var department = _context.Departments.First();

            // Act
            var result = _controller.Edit(department.DepartmentID) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as Department;
            model.Should().NotBeNull();
            model.DepartmentID.Should().Be(department.DepartmentID);
            Assert.NotNull(_controller.ViewBag.InstructorID);
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
        public void Delete_Get_WithValidId_ReturnsViewWithDepartment()
        {
            // Arrange
            var department = _context.Departments.First();

            // Act
            var result = _controller.Delete(department.DepartmentID) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as Department;
            model.Should().NotBeNull();
            model.DepartmentID.Should().Be(department.DepartmentID);
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
        public void Delete_Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidId = 99999;

            // Act
            var result = _controller.Delete(invalidId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void DeleteConfirmed_WithValidId_DeletesDepartmentAndRedirects()
        {
            // Arrange
            var department = _context.Departments.First();
            var departmentId = department.DepartmentID;

            // Act
            var result = _controller.DeleteConfirmed(departmentId) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            _context.Departments.Should().NotContain(d => d.DepartmentID == departmentId);
        }
    }
}
