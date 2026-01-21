using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ContosoUniversity.Services
{
    public class NotificationService
    {
        private readonly ConcurrentQueue<Notification> _notifications;
        private readonly IConfiguration _configuration;

        public NotificationService(IConfiguration configuration)
        {
            _notifications = new ConcurrentQueue<Notification>();
            _configuration = configuration;
        }

        public void SendNotification(string entityType, string entityId, EntityOperation operation, string userName = null)
        {
            SendNotification(entityType, entityId, null, operation, userName);
        }

        public void SendNotification(string entityType, string entityId, string entityDisplayName, EntityOperation operation, string userName = null)
        {
            try
            {
                var notification = new Notification
                {
                    EntityType = entityType,
                    EntityId = entityId,
                    Operation = operation.ToString(),
                    Message = GenerateMessage(entityType, entityId, entityDisplayName, operation),
                    CreatedAt = DateTime.Now,
                    CreatedBy = userName ?? "System",
                    IsRead = false
                };

                // Add to in-memory queue (replaces MSMQ)
                _notifications.Enqueue(notification);
            }
            catch (Exception ex)
            {
                // Log error but don't break the main operation
                System.Diagnostics.Debug.WriteLine($"Failed to send notification: {ex.Message}");
            }
        }

        public Notification ReceiveNotification()
        {
            try
            {
                if (_notifications.TryDequeue(out var notification))
                {
                    return notification;
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to receive notification: {ex.Message}");
                return null;
            }
        }

        public List<Notification> GetNotifications(int maxCount = 50)
        {
            try
            {
                var notifications = new List<Notification>();
                var tempList = new List<Notification>();
                
                // Get up to maxCount notifications without removing them
                int count = 0;
                foreach (var notification in _notifications)
                {
                    if (count >= maxCount) break;
                    notifications.Add(notification);
                    count++;
                }

                return notifications.OrderByDescending(n => n.CreatedAt).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting notifications: {ex.Message}");
                return new List<Notification>();
            }
        }

        public void MarkAsRead(int notificationId)
        {
            // In a real implementation, you might want to store notifications in database as well
            // for persistence and tracking read status
        }

        private string GenerateMessage(string entityType, string entityId, string entityDisplayName, EntityOperation operation)
        {
            var displayText = !string.IsNullOrWhiteSpace(entityDisplayName) 
                ? $"{entityType} '{entityDisplayName}'" 
                : $"{entityType} (ID: {entityId})";

            switch (operation)
            {
                case EntityOperation.CREATE:
                    return $"New {displayText} has been created";
                case EntityOperation.UPDATE:
                    return $"{displayText} has been updated";
                case EntityOperation.DELETE:
                    return $"{displayText} has been deleted";
                default:
                    return $"{displayText} operation: {operation}";
            }
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }
}
