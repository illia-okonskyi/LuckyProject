using LuckyProject.AuthServer.Models;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace LuckyProject.AuthServer.Services.Sessions
{
    public interface IAuthSessionManager
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync();
        Task<Guid> OnUserSignedInAsync(
            string clientId,
            Guid userId,
            Guid? sessionId,
            CancellationToken cancellationToken = default);
        Task OnNotificationsHubConnectedAsync(
            string connectionId,
            Guid sessionId,
            CancellationToken cancellationToken = default);

        Task OnUserLoggedOutAppAsync(
            Guid sessionId,
            CancellationToken cancellationToken = default);
        Task OnUserLoggedOutFullOrDeletedAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
        Task OnUserChangedAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
        Task OnApisChangedAsync(CancellationToken cancellationToken = default);
        Task<bool> IsSessionAliveAndValidAsync(
            Guid sessionId,
            CancellationToken cancellationToken = default);

        Task SendNotificationToAllSessions(
            Notification notification,
            CancellationToken cancellationToken = default);
        Task SendNotificationToSession(
            Guid sessionId,
            Notification notification,
            CancellationToken cancellationToken = default);
        Task SendNotificationToUser(
            Guid userId,
            Notification notification,
            CancellationToken cancellationToken = default);
    }
}
