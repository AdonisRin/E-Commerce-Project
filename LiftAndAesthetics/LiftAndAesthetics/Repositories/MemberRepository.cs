using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories
{
    public class MemberRepository : RepositoryBase<Members>, IMemberRepository
    {
        public MemberRepository(LiftContext context) : base(context) { }

        public async Task<Members?> GetByEmailAsync(string email) =>
            await LiftContext.Members.FirstOrDefaultAsync(m => m.Email == email);

        public async Task<Members?> GetByCredentialsAsync(string email, string password) =>
            await LiftContext.Members.FirstOrDefaultAsync(m => m.Email == email && m.Password == password);

        public bool EmailExists(string email) => LiftContext.Members.Any(m => m.Email == email);


        public async Task<Members?> GetMemberWithDetailsAsync(int id)
        {
            return await LiftContext.Members
                .Include(m => m.UserMemberships)
                    .ThenInclude(um => um.Membership)
                .FirstOrDefaultAsync(m => m.MemberId == id);
        }

        public async Task<Members?> GetMemberWithSubscriptionsAsync(string email)
        {
            return await LiftContext.Members
                .Include(m => m.UserMemberships)
                    .ThenInclude(um => um.Membership)
                .FirstOrDefaultAsync(m => m.Email == email);
        }

        public async Task<Members?> GetMemberByUserMembershipIdAsync(int userMembershipId)
        {
            return await LiftContext.Members
                .Include(m => m.UserMemberships)
                    .ThenInclude(um => um.Membership)
                .FirstOrDefaultAsync(m => m.UserMemberships != null && m.UserMemberships.Any(um => um.Id == userMembershipId));
        }

        public void ClearTracker()
        {
            LiftContext.ChangeTracker.Clear();
        }
    }
}