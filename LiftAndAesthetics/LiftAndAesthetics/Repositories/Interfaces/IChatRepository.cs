using LiftAndAesthetics.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories.Interfaces
{
    public interface IChatRepository : IRepositoryBase<ChatMessage>
    {
        Task<IEnumerable<ChatMessage>> GetConversationAsync(string user1, string user2);
        Task<IEnumerable<string>> GetAdminConversationsAsync();
    }
}