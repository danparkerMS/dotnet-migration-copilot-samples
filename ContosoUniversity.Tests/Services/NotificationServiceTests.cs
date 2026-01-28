using ContosoUniversity.Models;
using ContosoUniversity.Services;
using ContosoUniversity.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ContosoUniversity.Tests.Services
{
    public class NotificationServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly NotificationService _notificationService;

        public NotificationServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _notificationService = new NotificationService(_mockConfiguration.Object);
        }

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
            var notification = notifications.First();
            notification.EntityType.Should().Be(entityType);
            notification.EntityId.Should().Be(entityId);
            notification.Operation.Should().Be(operation.ToString());
            notification.IsRead.Should().BeFalse();
        }

        [Fact]
        public void SendNotification_WithDisplayName_IncludesDisplayNameInMessage()
        {
            // Arrange
            var entityType = "Student";
            var entityId = "123";
            var displayName = "John Doe";
            var operation = EntityOperation.CREATE;

            // Act
            _notificationService.SendNotification(entityType, entityId, displayName, operation);
            var notifications = _notificationService.GetNotifications();

            // Assert
            var notification = notifications.First();
            notification.Message.Should().Contain(displayName);
            notification.Message.Should().Contain("created");
        }

        [Fact]
        public void SendNotification_CreateOperation_GeneratesCorrectMessage()
        {
            // Arrange
            var operation = EntityOperation.CREATE;

            // Act
            _notificationService.SendNotification("Student", "1", operation);
            var notification = _notificationService.GetNotifications().First();

            // Assert
            notification.Message.Should().Contain("created");
        }

        [Fact]
        public void SendNotification_UpdateOperation_GeneratesCorrectMessage()
        {
            // Arrange
            var operation = EntityOperation.UPDATE;

            // Act
            _notificationService.SendNotification("Student", "1", operation);
            var notification = _notificationService.GetNotifications().First();

            // Assert
            notification.Message.Should().Contain("updated");
        }

        [Fact]
        public void SendNotification_DeleteOperation_GeneratesCorrectMessage()
        {
            // Arrange
            var operation = EntityOperation.DELETE;

            // Act
            _notificationService.SendNotification("Student", "1", operation);
            var notification = _notificationService.GetNotifications().First();

            // Assert
            notification.Message.Should().Contain("deleted");
        }

        [Fact]
        public void ReceiveNotification_WithExistingNotifications_ReturnsAndRemovesNotification()
        {
            // Arrange
            _notificationService.SendNotification("Student", "1", EntityOperation.CREATE);

            // Act
            var notification = _notificationService.ReceiveNotification();

            // Assert
            notification.Should().NotBeNull();
            notification.EntityType.Should().Be("Student");
            
            // Verify it was removed
            var remainingNotifications = _notificationService.GetNotifications();
            remainingNotifications.Should().BeEmpty();
        }

        [Fact]
        public void ReceiveNotification_WithNoNotifications_ReturnsNull()
        {
            // Act
            var notification = _notificationService.ReceiveNotification();

            // Assert
            notification.Should().BeNull();
        }

        [Fact]
        public void GetNotifications_ReturnsOrderedByCreatedAtDescending()
        {
            // Arrange
            _notificationService.SendNotification("Student", "1", EntityOperation.CREATE);
            Thread.Sleep(10); // Ensure different timestamps
            _notificationService.SendNotification("Course", "2", EntityOperation.UPDATE);
            Thread.Sleep(10);
            _notificationService.SendNotification("Instructor", "3", EntityOperation.DELETE);

            // Act
            var notifications = _notificationService.GetNotifications();

            // Assert
            notifications.Should().HaveCount(3);
            notifications[0].EntityType.Should().Be("Instructor"); // Most recent
            notifications[1].EntityType.Should().Be("Course");
            notifications[2].EntityType.Should().Be("Student"); // Oldest
        }

        [Fact]
        public void GetNotifications_WithMaxCount_ReturnsLimitedResults()
        {
            // Arrange
            for (int i = 0; i < 100; i++)
            {
                _notificationService.SendNotification("Student", i.ToString(), EntityOperation.CREATE);
            }

            // Act
            var notifications = _notificationService.GetNotifications(maxCount: 10);

            // Assert
            notifications.Should().HaveCount(10);
        }

        [Fact]
        public void SendNotification_WithUserName_SetsCreatedBy()
        {
            // Arrange
            var userName = "testuser@example.com";

            // Act
            _notificationService.SendNotification("Student", "1", EntityOperation.CREATE, userName);
            var notification = _notificationService.GetNotifications().First();

            // Assert
            notification.CreatedBy.Should().Be(userName);
        }

        [Fact]
        public void SendNotification_WithoutUserName_SetsCreatedByToSystem()
        {
            // Act
            _notificationService.SendNotification("Student", "1", EntityOperation.CREATE);
            var notification = _notificationService.GetNotifications().First();

            // Assert
            notification.CreatedBy.Should().Be("System");
        }

        [Fact]
        public void Multiple_SendReceive_MaintainsCorrectQueueBehavior()
        {
            // Arrange & Act
            _notificationService.SendNotification("Student", "1", EntityOperation.CREATE);
            _notificationService.SendNotification("Course", "2", EntityOperation.UPDATE);
            
            var first = _notificationService.ReceiveNotification();
            _notificationService.SendNotification("Instructor", "3", EntityOperation.DELETE);
            var second = _notificationService.ReceiveNotification();

            // Assert
            first.EntityType.Should().Be("Student"); // FIFO
            second.EntityType.Should().Be("Course");
            
            var remaining = _notificationService.GetNotifications();
            remaining.Should().HaveCount(1);
            remaining[0].EntityType.Should().Be("Instructor");
        }
    }
}
