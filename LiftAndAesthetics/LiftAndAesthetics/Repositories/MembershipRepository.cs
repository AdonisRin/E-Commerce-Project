using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories
{
    public class MembershipRepository : RepositoryBase<Membership>, IMembershipRepository
    {
        public MembershipRepository(LiftContext context) : base(context) { }

        public async Task<Members?> GetMemberByEmailAsync(string email)
        {
            return await LiftContext.Members.FirstOrDefaultAsync(m => m.Email == email);
        }

        public void UpdateMember(Members member)
        {
            LiftContext.Update(member);
        }
    }
}