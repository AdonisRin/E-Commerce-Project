// CHAT FEATURE DISABLED FOR DEMO
/*
using Microsoft.AspNetCore.Mvc;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private const string AdminEmail = "admin@lift.com";

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<IActionResult> Index(string? userEmail)
        {
            var currentUser = User.Identity?.Name ?? "";

            if (User.IsInRole("Admin"))
            {
                if (string.IsNullOrEmpty(userEmail))
                {
                    var users = await _chatService.GetAdminUserListAsync();
                    return View("AdminInbox", users);
                }
                ViewBag.ChatWith = userEmail;

                await _chatService.MarkMessagesAsReadAsync(AdminEmail, userEmail);

                var messages = await _chatService.GetMessagesAsync(AdminEmail, userEmail);
                return View(messages);
            }

            ViewBag.ChatWith = AdminEmail;

            await _chatService.MarkMessagesAsReadAsync(currentUser, AdminEmail);

            var userMessages = await _chatService.GetMessagesAsync(currentUser, AdminEmail);
            return View(userMessages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(string receiverEmail, string content, IFormFile? attachment)
        {
            await _chatService.SendMessageAsync(User.Identity?.Name ?? "", receiverEmail, content, attachment);
            return RedirectToAction(nameof(Index), new { userEmail = receiverEmail });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string receiverEmail)
        {
            bool isAdmin = User.IsInRole("Admin");
            await _chatService.DeleteMessageAsync(id, User.Identity?.Name ?? "", isAdmin);

            return RedirectToAction(nameof(Index), new { userEmail = receiverEmail });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string newContent, string receiverEmail)
        {
            await _chatService.EditMessageAsync(id, newContent, User.Identity?.Name ?? "");

            return RedirectToAction(nameof(Index), new { userEmail = receiverEmail });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConversation(string userEmail)
        {
            await _chatService.DeleteConversationAsync(userEmail);
            TempData["SuccessMessage"] = $"Chat history with {userEmail} has been wiped.";
            return RedirectToAction(nameof(Index));
        }
    }
}
*/