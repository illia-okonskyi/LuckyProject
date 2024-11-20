using LuckyProject.AuthServer.Hubs;
using LuckyProject.AuthServer.Models;
using LuckyProject.Lib.Basics.LiveObjects.Sessions;
using LuckyProject.Lib.Basics.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace LuckyProject.AuthServer.Services.Sessions
{
    public class AuthSessionManager : IAuthSessionManager
    {
        #region Internals & ctor
        private readonly ILpSessionManager<
            IAuthSessionService,
            LpInMemoryCacheSessionStorage<AuthSession, AuthSessionContext>,
            AuthSession,
            AuthSessionContext> backend;
        private readonly IHubContext<
            NotificationsHub,
            INotificationsClient> notificationsHubContext;

        public AuthSessionManager(
            IOptions<AuthSessionOptions> options,
            ILpSessionManagerFactory sessionManagerFactory,
            IAuthSessionService service,
            IHubContext<NotificationsHub, INotificationsClient> notificationsHubContext)
        {
            backend = sessionManagerFactory.CreateInMemorySessionManager<
                IAuthSessionService,
                AuthSession,
                AuthSessionContext>(
                    service,
                    options.Value.ExpireInterval,
                    options.Value.ExpireInterval / 10);
            this.notificationsHubContext = notificationsHubContext;
        }
        #endregion

        #region Public interface
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await backend.StartAsync(cancellationToken);
        }

        public async Task StopAsync()
        {
            await backend.StopAsync();
        }

        public async Task<Guid> OnUserSignedInAsync(
            string clientId,
            Guid userId,
            Guid? sessionId,
            CancellationToken cancellationToken = default)
        {
            AuthSession session = null;
            if (sessionId.HasValue)
            {
                session = await backend.GetSessionAsync(sessionId.Value, cancellationToken);
            }

            if (session != null)
            {
                await backend.PingSessionAsync(session, cancellationToken);
                return session.Id;
            }

            session = await backend.CreateSessionAsync(
                new IAuthSessionService.CreateContext()
                {
                    ClientId = clientId,
                    UserId = userId,
                },
                cancellationToken);
            return session.Id;
        }

        public async Task OnNotificationsHubConnectedAsync(
            string connectionId,
            Guid sessionId,
            CancellationToken cancellationToken = default)
        {
            var session = await backend.GetSessionAsync(sessionId, cancellationToken);
            if (session == null)
            {
                return;
            }

            session.Context.NotificationsHubConnectionId = connectionId;
            await backend.UpdateSessionAsync(session, cancellationToken);
        }

        public async Task OnUserLoggedOutAppAsync(
            Guid sessionId,
            CancellationToken cancellationToken = default)
        {
            var session = await backend.GetSessionAsync(
                sessionId,
                cancellationToken);
            if (session == null)
            {
                return;
            }

            await backend.DeleteSessionAsync(session, null, cancellationToken);
        }

        public async Task OnUserLoggedOutFullOrDeletedAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var sessions = await backend.GetSessionsAsync(
                (Guid k, AuthSession s) => s.Context.UserId == userId,
                cancellationToken);
            if (sessions.Count == 0)
            {
                return;
            }

            await backend.DeleteSessionsAsync(sessions, null, cancellationToken);
        }

        public async Task OnUserChangedAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            await SendNotificationToUser(userId, new UserChangedNotification(), cancellationToken);
        }

        public async Task OnApisChangedAsync(CancellationToken cancellationToken = default)
        {
            await SendNotificationToAllSessions(new UserChangedNotification(), cancellationToken);
        }

        public async Task<bool> IsSessionAliveAndValidAsync(
            Guid sessionId,
            CancellationToken cancellationToken = default)
        {
            var session = await backend.GetSessionAsync(sessionId, cancellationToken);
            if (session == null)
            {
                return false;
            }

            await backend.PingSessionAsync(session, cancellationToken);
            return true;
        }

        public async Task SendNotificationToAllSessions(
            Notification notification,
            CancellationToken cancellationToken = default)
        {
            var sessions = await backend.GetSessionsAsync((Guid k, AuthSession s) =>
            {
                return !string.IsNullOrEmpty(s.Context.NotificationsHubConnectionId);
            },
            cancellationToken);
            if (sessions.Count == 0)
            {
                return;
            }

            await notificationsHubContext
                .Clients
                .Clients(sessions.Select(s => s.Context.NotificationsHubConnectionId).ToList())
                .Notification(notification);
        }

        public async Task SendNotificationToSession(
            Guid sessionId,
            Notification notification,
            CancellationToken cancellationToken = default)
        {
            var session = await backend.GetSessionAsync(sessionId, cancellationToken);
            if (session == null ||
                string.IsNullOrEmpty(session.Context.NotificationsHubConnectionId))
            {
                return;
            }

            await notificationsHubContext
                .Clients
                .Client(session.Context.NotificationsHubConnectionId)
                .Notification(notification);
        }

        public async Task SendNotificationToUser(
            Guid userId,
            Notification notification,
            CancellationToken cancellationToken = default)
        {
            var sessions = await backend.GetSessionsAsync((Guid k, AuthSession s) =>
            {
                return s.Context.UserId == userId &&
                    !string.IsNullOrEmpty(s.Context.NotificationsHubConnectionId);
            },
            cancellationToken);
            if (sessions.Count == 0)
            {
                return;
            }

            await notificationsHubContext
                .Clients
                .Clients(sessions.Select(s => s.Context.NotificationsHubConnectionId).ToList())
                .Notification(notification);
        }
        #endregion
    }
}
