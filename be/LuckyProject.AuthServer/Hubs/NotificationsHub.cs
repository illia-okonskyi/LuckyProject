using LuckyProject.AuthServer.Models;
using LuckyProject.AuthServer.Services.Sessions;
using LuckyProject.Lib.Web.Constants;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace LuckyProject.AuthServer.Hubs
{
    public interface INotificationsClient
    {
        Task Notification(Notification notification);
    }

    [EnableCors(LpWebConstants.Cors.DynamicPolicyName)]
    public class NotificationsHub : Hub<INotificationsClient>
    {
        #region Internals & ctor
        private readonly IAuthSessionManager authSessionManager;

        public NotificationsHub(IAuthSessionManager authSessionManager)
        {
            this.authSessionManager = authSessionManager;
        }
        #endregion

        #region Authorize
        public class AuthorizeRequest
        {
            public Guid SessionId { get; set; }
        }

        [HubMethodName("Authorize")]
        public async Task AuthorizeAsync(AuthorizeRequest request)
        {
            if (!await authSessionManager.IsSessionAliveAndValidAsync(request.SessionId))
            {
                await Clients.Caller.Notification(new AuthorizeResultNotification(false));
            }

            await authSessionManager.OnNotificationsHubConnectedAsync(
                Context.ConnectionId,
                request.SessionId);
            await Clients.Caller.Notification(new AuthorizeResultNotification(true));
        }
        #endregion
    }
}
