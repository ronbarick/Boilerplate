using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Project.Domain.Interfaces;

namespace Project.WebApi.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly ICurrentUser _currentUser;

        public NotificationHub(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        public override async Task OnConnectedAsync()
        {
            if (_currentUser.IsAuthenticated)
            {
                // Add to user group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{_currentUser.Id}");
                
                // Add to tenant group if exists
                if (_currentUser.TenantId.HasValue)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Tenant_{_currentUser.TenantId}");
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (_currentUser.IsAuthenticated)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{_currentUser.Id}");
                if (_currentUser.TenantId.HasValue)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Tenant_{_currentUser.TenantId}");
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
