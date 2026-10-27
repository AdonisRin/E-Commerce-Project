using LiftAndAesthetics.Models;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories.Interfaces
{
    public interface IMemberRepository : IRepositoryBase<Members>
    {
        Task<Members?> GetByEmailAsync(string email);
        Task<Members?> GetByCredentialsAsync(string email, string password);
        bool EmailExists(string email);

        Task<Members?> GetMemberWithDetailsAsync(int id);
        Task<Members?> GetMemberWithSubscriptionsAsync(string email);
        Task<Members?> GetMemberByUserMembershipIdAsync(int userMembershipId);
        void ClearTracker();
    }
}