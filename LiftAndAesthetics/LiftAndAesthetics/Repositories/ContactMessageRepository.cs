using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;

namespace LiftAndAesthetics.Repositories
{
    public class ContactMessageRepository : RepositoryBase<ContactMessage>, IContactMessageRepository
    {
        public ContactMessageRepository(LiftContext context) : base(context) { }
    }
}