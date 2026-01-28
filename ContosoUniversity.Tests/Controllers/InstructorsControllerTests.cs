using System;
using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.Controllers;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;
using ContosoUniversity.Services;
using ContosoUniversity.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ContosoUniversity.Tests.Controllers
{
    public class InstructorsControllerTests : IDisposable
    {
        private readonly SchoolContext _context;
        private readonly NotificationService _notificationService;
        private readonly InstructorsController _controller;

        public InstructorsControllerTests()
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
            _controller = new InstructorsController(_context, _notificationService);
        }

        public void Dispose()
        {
            _context?.Dispose();
            _notificationService?.Dispose();
            _controller?.Dispose();
        }

        [Fact]
        public void Index_WithNoParameters_ReturnsViewWithInstructors()
        {
            // Act
            var result = _controller.Index(null, null) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as InstructorIndexData;
            model.Should().NotBeNull();
            model.Instructors.Should().NotBeNull();
            model.Instructors.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public void Index_WithInstructorId_SetsViewBagAndPopulatesCourses()
        {
            // Arrange
            var instructor = _context.Instructors.First();

            // Act
            var result = _controller.Index(instructor.ID, null) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as InstructorIndexData;
            model.Should().NotBeNull();
            ((int)_controller.ViewBag.InstructorID).Should().Be(instructor.ID);
            model.Courses.Should().NotBeNull();
        }

        [Fact]
        public void Index_WithCourseId_SetsViewBagInstructorIdAndCourseId()
        {
            // Arrange
            var instructor = _context.Instructors.First();
            var course = _context.Courses.First();
            
            // Add a course assignment to the instructor
            var courseAssignment = new CourseAssignment 
            { 
                InstructorID = instructor.ID, 
                CourseID = course.CourseID 
            };
            _context.CourseAssignments.Add(courseAssignment);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            // Act
            var result = _controller.Index(instructor.ID, course.CourseID) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as InstructorIndexData;
            model.Should().NotBeNull();
            ((int)_controller.ViewBag.InstructorID).Should().Be(instructor.ID);
            ((int)_controller.ViewBag.CourseID).Should().Be(course.CourseID);
        }

        [Fact]
        public void Index_OrdersInstructorsByLastName()
        {
            // Act
            var result = _controller.Index(null, null) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as InstructorIndexData;
            model.Should().NotBeNull();
            var instructors = model.Instructors.ToList();
            instructors.Should().BeInAscendingOrder(i => i.LastName);
        }

        [Fact]
        public void Details_WithValidId_ReturnsViewWithInstructor()
        {
            // Arrange
            var instructor = _context.Instructors.First();

            // Act
            var result = _controller.Details(instructor.ID) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as Instructor;
            model.Should().NotBeNull();
            model.ID.Should().Be(instructor.ID);
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
        public void Create_Get_ReturnsViewWithNewInstructor()
        {
            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as Instructor;
            model.Should().NotBeNull();
            model.CourseAssignments.Should().NotBeNull();
            model.CourseAssignments.Should().BeEmpty();
            Assert.NotNull(_controller.ViewBag.Courses);
        }

        [Fact]
        public void Create_Post_WithValidInstructor_RedirectsToIndex()
        {
            // Arrange
            var newInstructor = new Instructor
            {
                FirstMidName = "New",
                LastName = "Instructor",
                HireDate = DateTime.Today
            };
            _controller.ModelState.Clear();

            // Act
            var result = _controller.Create(newInstructor, null) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            _context.Instructors.Should().Contain(i => i.LastName == "Instructor" && i.FirstMidName == "New");
        }

        [Fact]
        public void Create_Post_WithSelectedCourses_AssignsCourses()
        {
            // Arrange
            var newInstructor = new Instructor
            {
                FirstMidName = "Test",
                LastName = "Teacher",
                HireDate = DateTime.Today
            };
            var course = _context.Courses.First();
            var selectedCourses = new[] { course.CourseID.ToString() };
            _controller.ModelState.Clear();

            // Act
            var result = _controller.Create(newInstructor, selectedCourses) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            var createdInstructor = _context.Instructors
                .FirstOrDefault(i => i.LastName == "Teacher" && i.FirstMidName == "Test");
            createdInstructor.Should().NotBeNull();
            createdInstructor.CourseAssignments.Should().NotBeNull();
            createdInstructor.CourseAssignments.Should().HaveCount(1);
            createdInstructor.CourseAssignments.First().CourseID.Should().Be(course.CourseID);
        }

        [Fact]
        public void Create_Post_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var invalidInstructor = new Instructor
            {
                CourseAssignments = new List<CourseAssignment>()
            };
            _controller.ModelState.AddModelError("LastName", "Required");

            // Act
            var result = _controller.Create(invalidInstructor, null) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().Be(invalidInstructor);
            Assert.NotNull(_controller.ViewBag.Courses);
        }

        [Fact]
        public void Edit_Get_WithValidId_ReturnsViewWithInstructor()
        {
            // Arrange
            var instructor = _context.Instructors.First();

            // Act
            var result = _controller.Edit(instructor.ID) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as Instructor;
            model.Should().NotBeNull();
            model.ID.Should().Be(instructor.ID);
            Assert.NotNull(_controller.ViewBag.Courses);
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
        public void Edit_Get_WithInvalidId_ThrowsInvalidOperationException()
        {
            // Arrange
            var invalidId = 99999;

            // Act & Assert
            var act = () => _controller.Edit(invalidId);
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Sequence contains no elements");
        }

        [Fact]
        public void Delete_Get_WithValidId_ReturnsViewWithInstructor()
        {
            // Arrange
            var instructor = _context.Instructors.First();

            // Act
            var result = _controller.Delete(instructor.ID) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as Instructor;
            model.Should().NotBeNull();
            model.ID.Should().Be(instructor.ID);
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
        public void DeleteConfirmed_WithValidId_DeletesInstructorAndRedirects()
        {
            // Arrange
            var instructor = _context.Instructors.First();
            var instructorId = instructor.ID;

            // Act
            var result = _controller.DeleteConfirmed(instructorId) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            _context.Instructors.Should().NotContain(i => i.ID == instructorId);
        }

        [Fact]
        public void DeleteConfirmed_RemovesDepartmentAssociation()
        {
            // Arrange
            var instructor = _context.Instructors.First();
            var department = _context.Departments.First(d => d.InstructorID == instructor.ID);
            var instructorId = instructor.ID;

            // Act
            var result = _controller.DeleteConfirmed(instructorId) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            _context.Instructors.Should().NotContain(i => i.ID == instructorId);
            
            // Reload department to check if InstructorID is null
            _context.ChangeTracker.Clear();
            var updatedDepartment = _context.Departments.Find(department.DepartmentID);
            updatedDepartment.Should().NotBeNull();
            updatedDepartment.InstructorID.Should().BeNull();
        }
    }
}
