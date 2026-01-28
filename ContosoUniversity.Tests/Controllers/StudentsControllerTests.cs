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
    public class StudentsControllerTests : IDisposable
    {
        private readonly SchoolContext _context;
        private readonly NotificationService _notificationService;
        private readonly StudentsController _controller;

        public StudentsControllerTests()
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
            _controller = new StudentsController(_context, _notificationService);
        }

        public void Dispose()
        {
            _context?.Dispose();
            _notificationService?.Dispose();
            _controller?.Dispose();
        }

        [Fact]
        public void Index_WithNoParameters_ReturnsViewWithStudents()
        {
            // Act
            var result = _controller.Index(null, null, null, null) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as PaginatedList<Student>;
            model.Should().NotBeNull();
            model.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Index_WithSearchString_FiltersStudents()
        {
            // Arrange
            var searchString = "Alexander";

            // Act
            var result = _controller.Index(null, null, searchString, null) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as PaginatedList<Student>;
            model.Should().NotBeNull();
            model.Should().OnlyContain(s => s.LastName.Contains(searchString) || s.FirstMidName.Contains(searchString));
        }

        [Fact]
        public void Index_WithNameDescSort_SortsByLastNameDescending()
        {
            // Arrange
            var sortOrder = "name_desc";

            // Act
            var result = _controller.Index(sortOrder, null, null, null) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as PaginatedList<Student>;
            model.Should().NotBeNull();
            var students = model.ToList();
            students.Should().BeInDescendingOrder(s => s.LastName);
        }

        [Fact]
        public void Index_WithDateSort_SortsByEnrollmentDateAscending()
        {
            // Arrange
            var sortOrder = "Date";

            // Act
            var result = _controller.Index(sortOrder, null, null, null) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as PaginatedList<Student>;
            model.Should().NotBeNull();
            var students = model.ToList();
            students.Should().BeInAscendingOrder(s => s.EnrollmentDate);
        }

        [Fact]
        public void Index_SetsViewBagProperties()
        {
            // Arrange
            var sortOrder = "name_desc";
            var searchString = "Test";

            // Act
            var result = _controller.Index(sortOrder, null, searchString, null) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            ((string)_controller.ViewBag.CurrentSort).Should().Be(sortOrder);
            ((string)_controller.ViewBag.CurrentFilter).Should().Be(searchString);
            // ViewBag properties are dynamic, so we can't use FluentAssertions on them directly
            Assert.NotNull(_controller.ViewBag.NameSortParm);
            Assert.NotNull(_controller.ViewBag.DateSortParm);
        }

        [Fact]
        public void Details_WithValidId_ReturnsViewWithStudent()
        {
            // Arrange
            var student = _context.Students.First();

            // Act
            var result = _controller.Details(student.ID) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as Student;
            model.Should().NotBeNull();
            model.ID.Should().Be(student.ID);
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
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Sequence contains no elements");
        }

        [Fact]
        public void Create_Get_ReturnsViewWithNewStudent()
        {
            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as Student;
            model.Should().NotBeNull();
            model.EnrollmentDate.Should().Be(DateTime.Today);
        }

        [Fact]
        public void Create_Post_WithValidStudent_RedirectsToIndex()
        {
            // Arrange
            var newStudent = new Student
            {
                FirstMidName = "New",
                LastName = "Student",
                EnrollmentDate = DateTime.Today
            };
            _controller.ModelState.Clear();

            // Act
            var result = _controller.Create(newStudent) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            _context.Students.Should().Contain(s => s.LastName == "Student" && s.FirstMidName == "New");
        }

        [Fact]
        public void Create_Post_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var invalidStudent = new Student(); // Missing required fields
            _controller.ModelState.AddModelError("LastName", "Required");

            // Act
            var result = _controller.Create(invalidStudent) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().Be(invalidStudent);
        }

        [Fact]
        public void Edit_Get_WithValidId_ReturnsViewWithStudent()
        {
            // Arrange
            var student = _context.Students.First();

            // Act
            var result = _controller.Edit(student.ID) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as Student;
            model.Should().NotBeNull();
            model.ID.Should().Be(student.ID);
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
        public void Delete_Get_WithValidId_ReturnsViewWithStudent()
        {
            // Arrange
            var student = _context.Students.First();

            // Act
            var result = _controller.Delete(student.ID) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as Student;
            model.Should().NotBeNull();
            model.ID.Should().Be(student.ID);
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
        public void DeleteConfirmed_WithValidId_DeletesStudentAndRedirects()
        {
            // Arrange
            var student = _context.Students.First();
            var studentId = student.ID;

            // Act
            var result = _controller.DeleteConfirmed(studentId) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            _context.Students.Should().NotContain(s => s.ID == studentId);
        }
    }
}
