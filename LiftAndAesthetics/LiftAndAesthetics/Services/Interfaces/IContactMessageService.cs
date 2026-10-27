using LiftAndAesthetics.Models;

namespace LiftAndAesthetics.Services.Interfaces
{
    public interface IContactMessageService
    {
        Task<IEnumerable<ContactMessage>> GetAllMessagesAsync();
        Task AddMessageAsync(ContactMessage message);
        Task DeleteMessageAsync(int id);
    }
}