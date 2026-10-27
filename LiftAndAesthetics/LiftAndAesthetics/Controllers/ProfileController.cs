using Microsoft.AspNetCore.Mvc;
using LiftAndAesthetics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Threading.Tasks;
using LiftAndAesthetics.Services.Interfaces;

namespace LiftAndAesthetics.Controllers
{
    [Authorize] 
    public class ProfileController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager; 
        private readonly IWebHostEnvironment _env;

        public ProfileController(IMemberService memberService, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IWebHostEnvironment env)
        {
            _memberService = memberService;
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var email = User.Identity.Name;
            var member = await _memberService.GetMemberByEmailAsync(email);
            if (member == null) return NotFound();

            if (member.Height.HasValue && member.Weight.HasValue && member.Height > 0)
            {
                double heightInMeters = member.Height.Value / 100.0;
                double bmi = member.Weight.Value / (heightInMeters * heightInMeters);
                ViewBag.BMI = Math.Round(bmi, 2);
            }

            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInfo(string name, int? age, int? height, double? weight, string? address, IFormFile? profilePicture)
        {
            var email = User.Identity.Name;
            var member = await _memberService.GetMemberByEmailAsync(email);
            if (member == null) return NotFound();

            member.Name = name;
            member.Age = age;
            member.Height = height;
            member.Weight = weight;
            member.Address = address; 

            if (profilePicture != null && profilePicture.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "photos", "profiles");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + profilePicture.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(fileStream);
                }

                member.ProfilePictureUrl = "/photos/profiles/" + uniqueFileName;
            }

            await _memberService.UpdateProfileAsync(member);

            TempData["SuccessMessage"] = "Profile info updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user != null && !string.IsNullOrEmpty(currentPassword) && !string.IsNullOrEmpty(newPassword))
            {
                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Password changed securely!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to change password. Make sure current password is correct.";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            var email = User.Identity.Name;

            var member = await _memberService.GetMemberByEmailAsync(email);
            if (member != null)
            {
                await _memberService.DeleteMemberAsync(member.MemberId);
            }

            var identityUser = await _userManager.FindByEmailAsync(email);
            if (identityUser != null)
            {
                await _userManager.DeleteAsync(identityUser);

                await _signInManager.SignOutAsync();
            }

            TempData["SuccessMessage"] = "Account permanently deleted. We're sorry to see you go!";
            return RedirectToAction("Index", "Home");
        }
    }
}