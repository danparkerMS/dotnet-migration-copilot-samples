using System;
using ContosoUniversity.Controllers;
using ContosoUniversity.Data;
using ContosoUniversity.Services;
using ContosoUniversity.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ContosoUniversity.Tests.Controllers
{
    public class HomeControllerTests : IDisposable
    {
        private readonly SchoolContext _context;
        private readonly NotificationService _notificationService;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
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
            _controller = new HomeController(_context, _notificationService);
        }

        public void Dispose()
        {
            _context?.Dispose();
            _notificationService?.Dispose();
            _controller?.Dispose();
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void About_ReturnsViewWithEnrollmentStats()
        {
            // Act
            var result = _controller.About() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewData.Should().NotBeNull();
            // The About action groups students by enrollment date
            result.ViewData.Model.Should().NotBeNull();
        }

        [Fact]
        public void Contact_ReturnsViewResult()
        {
            // Act
            var result = _controller.Contact();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void Error_ReturnsViewResult()
        {
            // Act
            var result = _controller.Error();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void Unauthorized_ReturnsViewResult()
        {
            // Act
            var result = _controller.Unauthorized();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
    }
}
