using Microsoft.AspNetCore.Mvc;
using LiftAndAesthetics.Models;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;

namespace LiftAndAesthetics.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IMemberService _memberService;

        public ReviewsController(IReviewService reviewService, IMemberService memberService)
        {
            _reviewService = reviewService;
            _memberService = memberService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string category = "All")
        {
            var reviews = category == "All"
                ? await _reviewService.GetAllReviewsAsync()
                : await _reviewService.GetReviewsByCategoryAsync(category);

            ViewBag.CurrentCategory = category;
            return View(reviews);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review review)
        {
            var user = await _memberService.GetMemberByEmailAsync(User.Identity.Name);
            if (user == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                review.MemberId = user.MemberId; 
                review.CreatedAt = DateTime.Now;

                await _reviewService.CreateReviewAsync(review);
                TempData["SuccessMessage"] = "Review posted successfully! Thank you for your feedback.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to post review. Please check your inputs.";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int rating, string comment)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null) return NotFound();

            var currentUser = await _memberService.GetMemberByEmailAsync(User.Identity.Name);

            if (currentUser == null || review.MemberId != currentUser.MemberId) return Unauthorized();

            review.Rating = rating;
            review.Comment = comment;

            await _reviewService.UpdateReviewAsync(review);
            TempData["SuccessMessage"] = "Review updated.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null) return NotFound();

            var currentUser = await _memberService.GetMemberByEmailAsync(User.Identity.Name);

            if ((currentUser == null || review.MemberId != currentUser.MemberId) && !User.IsInRole("Admin"))
                return Unauthorized();

            await _reviewService.DeleteReviewAsync(id);
            TempData["SuccessMessage"] = "Review deleted.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int id, string adminReply)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null) return NotFound();

            review.AdminReply = adminReply;
            review.ReplyDate = DateTime.Now;

            await _reviewService.UpdateReviewAsync(review);
            TempData["SuccessMessage"] = "Reply sent to user.";

            return RedirectToAction(nameof(Index));
        }
    }
}