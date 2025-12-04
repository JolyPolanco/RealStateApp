using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Core.Domain.Entities;
using System.Security.Claims;

namespace RealStateApp.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private static readonly Dictionary<string, string> _userConnections = new();

        public ChatHub(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections[userId] = Context.ConnectionId;
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections.Remove(userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Join a property chat room
        /// </summary>
        public async Task JoinPropertyChat(int propertyId)
        {
            var groupName = $"property_{propertyId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        /// <summary>
        /// Leave a property chat room
        /// </summary>
        public async Task LeavePropertyChat(int propertyId)
        {
            var groupName = $"property_{propertyId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        /// <summary>
        /// Send a message to a property chat
        /// </summary>
        public async Task SendMessage(int propertyId, string receiverId, string content)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId))
            {
                throw new HubException("Usuario no autenticado");
            }

            // Save message to database
            var message = new Message
            {
                PropertyId = propertyId,
                SenderUserId = senderId,
                ReceiverUserId = receiverId,
                Content = content,
                Date = DateTime.Now
            };

            await _messageRepository.AddAsync(message);

            var groupName = $"property_{propertyId}";
            
            // Get sender name
            var senderName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Usuario";

            // Broadcast to all users in the property group
            await Clients.Group(groupName).SendAsync("ReceiveMessage", new
            {
                id = message.Id,
                senderId = senderId,
                receiverId = receiverId,
                content = content,
                date = message.Date.ToString("dd/MM/yyyy HH:mm"),
                senderName = senderName,
                propertyId = propertyId
            });
        }

        /// <summary>
        /// Notify that user is typing
        /// </summary>
        public async Task NotifyTyping(int propertyId, string receiverId, bool isTyping)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId))
            {
                return;
            }

            var groupName = $"property_{propertyId}";
            var senderName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Usuario";

            await Clients.Group(groupName).SendAsync("UserTyping", new
            {
                senderId = senderId,
                senderName = senderName,
                isTyping = isTyping,
                propertyId = propertyId
            });
        }

        /// <summary>
        /// Get online status for a user
        /// </summary>
        public static bool IsUserOnline(string userId)
        {
            return _userConnections.ContainsKey(userId);
        }
    }
}
