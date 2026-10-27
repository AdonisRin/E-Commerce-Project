using LiftAndAesthetics.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<Members>> GetAllMembersAsync();
        Task<Members?> GetMemberByIdAsync(int id);
        Task<Members?> GetMemberWithDetailsAsync(int id);

        Task<Members?> GetMemberByEmailAsync(string email);
        Task UpdateProfileAsync(Members member);

        Task CreateMemberAsync(Members member);
        Task<(bool Success, string Message)> RegisterMemberAsync(Members member);
        Task<ClaimsPrincipal?> AuthenticateAsync(string email, string password);
        Task UpdateMemberAsync(Members member);
        Task DeleteMemberAsync(int id);
    }
}