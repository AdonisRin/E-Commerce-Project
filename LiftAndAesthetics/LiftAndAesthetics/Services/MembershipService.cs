using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IRepositoryWrapper _repository;

        public MembershipService(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Membership>> GetAllMembershipsAsync() =>
            await _repository.Membership.FindAll().Include(m => m.Members).ToListAsync();

        public async Task<Membership?> GetMembershipByIdAsync(int id) =>
            await _repository.Membership.FindByCondition(m => m.MembershipId == id).FirstOrDefaultAsync();

        public async Task AddMembershipAsync(Membership m) { _repository.Membership.Create(m); _repository.Save(); await Task.CompletedTask; }
        public async Task UpdateMembershipAsync(Membership m) { _repository.Membership.Update(m); _repository.Save(); await Task.CompletedTask; }

        public async Task DeleteMembershipAsync(int id)
        {
            var m = await GetMembershipByIdAsync(id);
            if (m != null) { _repository.Membership.Delete(m); _repository.Save(); }
        }

        public async Task<(int? MembershipId, DateTime? ExpiryDate)> GetUserMembershipStatusAsync(string email)
        {
            var user = await _repository.Member.GetByEmailAsync(email);
            return (user?.MembershipId, user?.MembershipExpiryDate);
        }

        public async Task<(bool Success, string Message)> SubscribeAsync(string email, int membershipId)
        {
            var user = await _repository.Member.GetMemberWithSubscriptionsAsync(email);
            var membership = await GetMembershipByIdAsync(membershipId);

            if (user == null || membership == null)
                return (false, "Error: User profile or Plan not found.");

            if (user.WalletBalance < membership.Price)
                return (false, "Insufficient funds! You need more LEI in your wallet.");

            DateTime now = DateTime.Now;
            DateTime startDate = now;

            if (user.UserMemberships != null && user.UserMemberships.Any())
            {
                var maxEndDate = user.UserMemberships.Max(um => um.EndDate);
                if (maxEndDate > now)
                {
                    startDate = maxEndDate;
                }
            }

            var newSubscription = new UserMembership
            {
                MemberId = user.MemberId,
                MembershipId = membership.MembershipId,
                StartDate = startDate,
                EndDate = startDate.AddDays(30)
            };

            user.WalletBalance -= membership.Price;

            if (startDate.Date == now.Date)
            {
                user.MembershipId = membership.MembershipId;
                user.MembershipExpiryDate = newSubscription.EndDate;
            }

            if (user.UserMemberships == null) user.UserMemberships = new List<UserMembership>();
            user.UserMemberships.Add(newSubscription);

            _repository.Member.Update(user);
            _repository.Save();

            if (startDate > now)
                return (true, $"Success! {membership.Name} was queued. It will automatically start on {startDate:dd MMM yyyy}.");
            else
                return (true, $"Success! You are now a {membership.Name} member.");
        }

        public async Task<(bool Success, string Message)> RefundMembershipAsync(int userMembershipId)
        {
            var user = await _repository.Member.GetMemberByUserMembershipIdAsync(userMembershipId);

            if (user == null) return (false, "Record not found.");

            var um = user.UserMemberships!.FirstOrDefault(u => u.Id == userMembershipId);
            if (um == null) return (false, "Record not found.");

            DateTime now = DateTime.Now;
            if (um.EndDate <= now) return (false, "Cannot refund an expired plan.");

            int daysToRefund = 0;
            if (um.StartDate > now)
            {
                daysToRefund = 30;
            }
            else
            {
                daysToRefund = (um.EndDate - now).Days;
            }

            decimal refundAmount = (um.Membership!.Price / 30m) * daysToRefund;

            user.WalletBalance += refundAmount;

            if (user.MembershipId == um.MembershipId && user.MembershipExpiryDate == um.EndDate)
            {
                user.MembershipId = null;
                user.MembershipExpiryDate = null;
            }

            user.UserMemberships.Remove(um);

            _repository.Member.Update(user);
            _repository.Save();

            return (true, $"Refund successful! Returned {refundAmount:0.00} LEI for {daysToRefund} remaining days.");
        }
    }
}