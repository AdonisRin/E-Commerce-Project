using LiftAndAesthetics.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Services.Interfaces
{
    public interface IMembershipService
    {
        Task<IEnumerable<Membership>> GetAllMembershipsAsync();
        Task<Membership?> GetMembershipByIdAsync(int id);
        Task AddMembershipAsync(Membership membership);
        Task UpdateMembershipAsync(Membership membership);
        Task DeleteMembershipAsync(int id);

        Task<(bool Success, string Message)> SubscribeAsync(string email, int membershipId);
        Task<(int? MembershipId, DateTime? ExpiryDate)> GetUserMembershipStatusAsync(string email);
        Task<(bool Success, string Message)> RefundMembershipAsync(int userMembershipId);
    }
}