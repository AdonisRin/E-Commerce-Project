using LiftAndAesthetics.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly IClassReservationService _reservationService;

        public AppointmentsController(IClassReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        public async Task<IActionResult> Index()
        {
            var reservations = await _reservationService.GetUserReservationsAsync(User.Identity?.Name ?? "");
            return View(reservations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int classId)
        {
            var result = await _reservationService.BookClassAsync(User.Identity?.Name ?? "", classId);
            
            if (result.Success) TempData["SuccessMessage"] = result.Message;
            else TempData["ErrorMessage"] = result.Message;

            return RedirectToAction(nameof(Index)); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _reservationService.CancelReservationAsync(User.Identity?.Name ?? "", id);
            
            if (result.Success) TempData["SuccessMessage"] = result.Message;
            else TempData["ErrorMessage"] = result.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}