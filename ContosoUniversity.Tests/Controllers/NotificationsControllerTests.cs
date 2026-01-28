using System;
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
    public class NotificationsControllerTests : IDisposable
    {
        private readonly SchoolContext _context;
        private readonly NotificationService _notificationService;
        private readonly NotificationsController _controller;

        public NotificationsControllerTests()
        {
            var dbName = Guid.NewGuid().ToString();
            _context = InMemoryDbContextFactory.Create(dbName);

            var mockConfiguration = new Mock<IConfiguration>();
            _notificationService = new NotificationService(mockConfiguration.Object);
            _controller = new NotificationsController(_context, _notificationService);
        }

        public void Dispose()
        {
            _context?.Dispose();
            _notificationService?.Dispose();
            _controller?.Dispose();
        }

        [Fact]
        public void Index_ReturnsView()
        {
            // Act
            var result = _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void GetNotifications_ReturnsJsonWithPendingNotifications()
        {
            // Arrange
            _notificationService.SendNotification("Student", "1", EntityOperation.CREATE);
            _notificationService.SendNotification("Course", "2", EntityOperation.UPDATE);

            // Act
            var result = _controller.GetNotifications() as JsonResult;

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void MarkAsRead_WithValidId_ReturnsJsonResult()
        {
            // Arrange
            var notificationId = 1;

            // Act
            var result = _controller.MarkAsRead(notificationId);

            // Assert
            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public void MarkAsRead_WithInvalidId_ReturnsJsonResult()
        {
            // Arrange
            var invalidId = 99999;

            // Act
            var result = _controller.MarkAsRead(invalidId);

            // Assert
            result.Should().BeOfType<JsonResult>();
        }
    }
}
