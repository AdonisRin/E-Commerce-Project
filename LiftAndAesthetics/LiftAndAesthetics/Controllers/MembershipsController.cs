using Microsoft.AspNetCore.Mvc;
using LiftAndAesthetics.Models;
using LiftAndAesthetics.Services.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;

namespace LiftAndAesthetics.Controllers
{
    public class MembershipsController : Controller
    {
        private readonly IMembershipService _membershipService;

        public MembershipsController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        public async Task<IActionResult> Index()
        {
            var memberships = await _membershipService.GetAllMembershipsAsync();
            var mostPopularId = memberships.OrderByDescending(m => m?.Members?.Count ?? 0).FirstOrDefault()?.MembershipId;
            ViewBag.MostPopularId = mostPopularId;

            if (User.Identity.IsAuthenticated)
            {
                var status = await _membershipService.GetUserMembershipStatusAsync(User.Identity.Name);
                ViewBag.CurrentPlanId = status.MembershipId ?? 0;

                if (status.ExpiryDate.HasValue && status.ExpiryDate.Value > DateTime.Now)
                    ViewBag.DaysLeft = (status.ExpiryDate.Value - DateTime.Now).Days;
                else
                    ViewBag.DaysLeft = 0;
            }

            return View(memberships);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(int membershipId)
        {
            var email = User.Identity.Name;
            var result = await _membershipService.SubscribeAsync(email, membershipId);

            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Membership membership)
        {
            if (ModelState.IsValid)
            {
                await _membershipService.AddMembershipAsync(membership);
                return RedirectToAction(nameof(Index));
            }
            return View(membership);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var membership = await _membershipService.GetMembershipByIdAsync(id);
            if (membership == null) return NotFound();
            return View(membership);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Membership membership)
        {
            if (id != membership.MembershipId) return NotFound();

            if (ModelState.IsValid)
            {
                await _membershipService.UpdateMembershipAsync(membership);
                return RedirectToAction(nameof(Index));
            }
            return View(membership);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var membership = await _membershipService.GetMembershipByIdAsync(id);
            if (membership == null) return NotFound();
            return View(membership);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _membershipService.DeleteMembershipAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var items = await _membershipService.GetAllMembershipsAsync();
            if (!string.IsNullOrEmpty(query))
            {
                items = items.Where(m => m.Name != null && m.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var safeItems = items.Select(m => new {
                membershipId = m.MembershipId,
                name = m.Name,
                price = m.Price,
                description = m.Description,
                features = m.Features
            });

            return Json(safeItems);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Refund(int userMembershipId)
        {
            var result = await _membershipService.RefundMembershipAsync(userMembershipId);

            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;

            return RedirectToAction("Index", "Members");
        }
    }
}