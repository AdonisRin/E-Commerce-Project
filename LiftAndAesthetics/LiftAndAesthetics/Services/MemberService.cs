using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Services
{
    public class MemberService : IMemberService
    {
        private readonly IRepositoryWrapper _repository;

        public MemberService(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Members>> GetAllMembersAsync() =>
            await _repository.Member.FindAll()
                .Include(m => m.Membership)
                .ToListAsync();

        public async Task<Members?> GetMemberByIdAsync(int id) =>
            await _repository.Member.FindByCondition(m => m.MemberId == id)
                .Include(m => m.Membership)
                .FirstOrDefaultAsync();

        public async Task<Members?> GetMemberWithDetailsAsync(int id)
        {
            return await _repository.Member.GetMemberWithDetailsAsync(id);
        }

        public async Task<Members?> GetMemberByEmailAsync(string email)
        {
            return await _repository.Member.GetByEmailAsync(email);
        }

        public async Task UpdateProfileAsync(Members member)
        {
            _repository.Member.Update(member);
            _repository.Save();
            await Task.CompletedTask;
        }

        public async Task<(bool Success, string Message)> RegisterMemberAsync(Members member)
        {
            if (_repository.Member.EmailExists(member.Email!))
                return (false, "This email is already registered!");

            _repository.Member.Create(member);
            _repository.Save();
            return (true, $"Account created for {member.Name}!");
        }

        public async Task<ClaimsPrincipal?> AuthenticateAsync(string email, string password)
        {
            var user = await _repository.Member.GetByCredentialsAsync(email, password);
            if (user == null) return null;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name ?? "User"),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }

        public async Task UpdateMemberAsync(Members member)
        {
            _repository.Member.Update(member);
            _repository.Save();
            await Task.CompletedTask;
        }

        public async Task DeleteMemberAsync(int id)
        {
            _repository.Member.ClearTracker();
            var member = await _repository.Member.FindByCondition(m => m.MemberId == id).FirstOrDefaultAsync();

            if (member != null)
            {
                _repository.Member.Delete(member);
                _repository.Save();
            }
        }

        public async Task CreateMemberAsync(Members member)
        {
            _repository.Member.Create(member);
            _repository.Save();
            await Task.CompletedTask;
        }
    }
}