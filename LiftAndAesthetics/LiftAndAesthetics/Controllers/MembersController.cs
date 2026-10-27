using Microsoft.AspNetCore.Mvc;
using LiftAndAesthetics.Models;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace LiftAndAesthetics.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IMembershipService _membershipService;
        private readonly UserManager<IdentityUser> _userManager;

        public MembersController(IMemberService memberService, IMembershipService membershipService, UserManager<IdentityUser> userManager)
        {
            _memberService = memberService;
            _membershipService = membershipService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var members = await _memberService.GetAllMembersAsync();
            return View(members);
        }

        public async Task<IActionResult> Details(int id)
        {
            var member = await _memberService.GetMemberWithDetailsAsync(id);
            if (member == null) return NotFound();

            return View(member);
        }

        public async Task<IActionResult> Create()
        {
            var memberships = await _membershipService.GetAllMembershipsAsync();
            ViewBag.MembershipId = new SelectList(memberships, "MembershipId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Members member, string? UserRole)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(member.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "This email is already registered in the system!");
                    var mems = await _membershipService.GetAllMembershipsAsync();
                    ViewBag.MembershipId = new SelectList(mems, "MembershipId", "Name", member.MembershipId);
                    return View(member);
                }

                var identityUser = new IdentityUser { UserName = member.Email, Email = member.Email };
                var result = await _userManager.CreateAsync(identityUser, member.Password);

                if (result.Succeeded)
                {
                    var roleToAssign = !string.IsNullOrEmpty(UserRole) ? UserRole : (member.Role ?? "User");
                    await _userManager.AddToRoleAsync(identityUser, roleToAssign);
                    member.Role = roleToAssign;

                    member.Password = identityUser.PasswordHash;

                    if (member.MembershipId != null)
                    {
                        member.MembershipExpiryDate = DateTime.Now.AddDays(30);
                    }

                    await _memberService.CreateMemberAsync(member);

                    TempData["SuccessMessage"] = "Member securely created and password hashed!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("Password", error.Description);
                    }
                }
            }

            var memberships = await _membershipService.GetAllMembershipsAsync();
            ViewBag.MembershipId = new SelectList(memberships, "MembershipId", "Name", member.MembershipId);
            return View(member);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var member = await _memberService.GetMemberByIdAsync(id);
            if (member == null) return NotFound();

            var memberships = await _membershipService.GetAllMembershipsAsync();
            ViewBag.MembershipId = new SelectList(memberships, "MembershipId", "Name", member.MembershipId);
            ViewBag.Roles = new SelectList(new List<string> { "User", "Admin" }, member.Role);

            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Members member, string UserRole)
        {
            if (id != member.MemberId) return NotFound();

            var existingMember = await _memberService.GetMemberByIdAsync(id);
            if (existingMember == null) return NotFound();

            if (ModelState.IsValid)
            {
                if (member.MembershipId != null)
                {
                    if (existingMember.MembershipExpiryDate == null || existingMember.MembershipExpiryDate < DateTime.Now)
                    {
                        existingMember.MembershipExpiryDate = DateTime.Now.AddDays(30);
                    }
                }
                else
                {
                    existingMember.MembershipExpiryDate = null;
                }

                existingMember.Name = member.Name;
                existingMember.MembershipId = member.MembershipId;

                if (!string.IsNullOrEmpty(UserRole))
                {
                    existingMember.Role = UserRole;
                }

                await _memberService.UpdateMemberAsync(existingMember);

                var identityUser = await _userManager.FindByEmailAsync(existingMember.Email);
                if (identityUser != null)
                {
                    var currentRoles = await _userManager.GetRolesAsync(identityUser);
                    await _userManager.RemoveFromRolesAsync(identityUser, currentRoles);

                    await _userManager.AddToRoleAsync(identityUser, existingMember.Role);
                }

                TempData["SuccessMessage"] = "Member updated! (Note: The user must re-login to see new admin/user rights).";
                return RedirectToAction(nameof(Index));
            }

            var memberships = await _membershipService.GetAllMembershipsAsync();
            ViewBag.MembershipId = new SelectList(memberships, "MembershipId", "Name", member.MembershipId);
            ViewBag.Roles = new SelectList(new List<string> { "User", "Admin" }, member.Role);

            return View(member);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var member = await _memberService.GetMemberByIdAsync(id);
            if (member == null) return NotFound();
            return View(member);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _memberService.DeleteMemberAsync(id);
            TempData["SuccessMessage"] = "Member account deleted!";
            return RedirectToAction(nameof(Index));
        }
    }
}