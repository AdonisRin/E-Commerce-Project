using LiftAndAesthetics.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Services.Interfaces
{
    public interface IChatService
    {
        Task<IEnumerable<ChatMessage>> GetMessagesAsync(string currentUserEmail, string otherUserEmail);
        Task<IEnumerable<string>> GetAdminUserListAsync();
        Task SendMessageAsync(string senderEmail, string receiverEmail, string content, IFormFile? attachment);
        Task<int> GetUnreadCountAsync(string userEmail);
        Task MarkMessagesAsReadAsync(string receiverEmail, string senderEmail);
        Task<bool> DeleteMessageAsync(int messageId, string userEmail, bool isAdmin);
        Task<bool> EditMessageAsync(int messageId, string newContent, string userEmail);
        Task DeleteConversationAsync(string userEmail);
    }
}