using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories
{
    public class ChatRepository : RepositoryBase<ChatMessage>, IChatRepository
    {
        public ChatRepository(LiftContext context) : base(context) { }

        public async Task<IEnumerable<ChatMessage>> GetConversationAsync(string user1, string user2)
        {
            return await FindByCondition(m =>
                (m.SenderEmail == user1 && m.ReceiverEmail == user2) ||
                (m.SenderEmail == user2 && m.ReceiverEmail == user1))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetAdminConversationsAsync()
        {
            var senders = await FindAll().Select(m => m.SenderEmail).Distinct().ToListAsync();
            var receivers = await FindAll().Select(m => m.ReceiverEmail).Distinct().ToListAsync();

            return senders.Union(receivers).Where(e => e != "admin@lift.com").ToList();
        }
    }
}