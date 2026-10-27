using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Services
{
    public class ChatService : IChatService
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IWebHostEnvironment _env;

        public ChatService(IRepositoryWrapper repository, IWebHostEnvironment env)
        {
            _repository = repository;
            _env = env;
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesAsync(string currentUser, string otherUser)
            => await _repository.Chat.GetConversationAsync(currentUser, otherUser);

        public async Task<IEnumerable<string>> GetAdminUserListAsync()
            => await _repository.Chat.GetAdminConversationsAsync();

        public async Task SendMessageAsync(string sender, string receiver, string content, IFormFile? attachment)
        {
            string finalContent = string.IsNullOrWhiteSpace(content)
                                  ? (attachment != null ? " Attachment" : "...")
                                  : content;

            var message = new ChatMessage
            {
                SenderEmail = sender,
                ReceiverEmail = receiver,
                Content = finalContent,
                Timestamp = DateTime.Now
            };

            if (attachment != null && attachment.Length > 0)
            {
                string folder = Path.Combine(_env.WebRootPath, "photos", "chat");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid().ToString() + "_" + attachment.FileName;
                using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
                {
                    await attachment.CopyToAsync(stream);
                }
                message.AttachmentUrl = "/photos/chat/" + fileName;
            }

            _repository.Chat.Create(message);
            await _repository.SaveAsync();
        }


        public async Task<int> GetUnreadCountAsync(string userEmail)
        {
            var unread = await _repository.Chat
                .FindByCondition(m => m.ReceiverEmail == userEmail && m.IsRead == false)
                .ToListAsync();
            return unread.Count;
        }

        public async Task MarkMessagesAsReadAsync(string receiverEmail, string senderEmail)
        {
            var unreadMessages = await _repository.Chat
                .FindByCondition(m => m.ReceiverEmail == receiverEmail && m.SenderEmail == senderEmail && m.IsRead == false)
                .ToListAsync();

            if (unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                {
                    msg.IsRead = true;
                    _repository.Chat.Update(msg);
                }
                await _repository.SaveAsync();
            }

        }

        public async Task<bool> DeleteMessageAsync(int messageId, string userEmail, bool isAdmin)
        {
            var msg = await _repository.Chat.FindByCondition(m => m.Id == messageId).FirstOrDefaultAsync();
            if (msg == null) return false;

            if (msg.SenderEmail == userEmail || isAdmin)
            {
                _repository.Chat.Delete(msg);
                await _repository.SaveAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> EditMessageAsync(int messageId, string newContent, string userEmail)
        {
            var msg = await _repository.Chat.FindByCondition(m => m.Id == messageId).FirstOrDefaultAsync();
            if (msg == null) return false;

            if (msg.SenderEmail == userEmail && !string.IsNullOrWhiteSpace(newContent))
            {
                msg.Content = newContent;
                msg.IsEdited = true;
                _repository.Chat.Update(msg);
                await _repository.SaveAsync();
                return true;
            }
            return false;
        }

        public async Task DeleteConversationAsync(string userEmail)
        {
            var messages = await _repository.Chat
                .FindByCondition(c => c.SenderEmail == userEmail || c.ReceiverEmail == userEmail)
                .ToListAsync();

            if (messages.Any())
            {
                foreach (var msg in messages)
                {
                    _repository.Chat.Delete(msg);
                }
                await _repository.SaveAsync();
            }
        }
    }
}