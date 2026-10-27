using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Services
{
    public class ContactMessageService : IContactMessageService
    {
        private readonly IRepositoryWrapper _repository;

        public ContactMessageService(IRepositoryWrapper repository) => _repository = repository;

        public async Task<IEnumerable<ContactMessage>> GetAllMessagesAsync() =>
            await _repository.ContactMessage.FindAll().OrderByDescending(m => m.SentDate).ToListAsync();

        public async Task AddMessageAsync(ContactMessage m)
        {
            _repository.ContactMessage.Create(m);
            _repository.Save();
            await Task.CompletedTask;
        }

        public async Task DeleteMessageAsync(int id)
        {
            var m = await _repository.ContactMessage.FindByCondition(x => x.ContactMessageId == id).FirstOrDefaultAsync();
            if (m != null) { _repository.ContactMessage.Delete(m); _repository.Save(); }
        }
    }
}