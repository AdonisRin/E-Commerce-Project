using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiftAndAesthetics.Models;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LiftAndAesthetics.Controllers
{
    public class ContactMessagesController : Controller
    {
        private readonly IContactMessageService _messageService;

        public ContactMessagesController(IContactMessageService messageService)
        {
            _messageService = messageService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _messageService.GetAllMessagesAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] ContactMessage contactMessage)
        {
            if (ModelState.IsValid)
            {
                await _messageService.AddMessageAsync(contactMessage);
                return Json(new { success = true, message = "Thank you! Your message was sent successfully." });
            }
            return Json(new { success = false, message = "Please fill in all fields correctly." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _messageService.DeleteMessageAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}