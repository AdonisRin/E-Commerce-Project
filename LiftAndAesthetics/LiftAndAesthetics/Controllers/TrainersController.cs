using Microsoft.AspNetCore.Mvc;
using LiftAndAesthetics.Models;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization; 

namespace LiftAndAesthetics.Controllers
{
    public class TrainersController : Controller
    {
        private readonly ITrainerService _trainerService;

        public TrainersController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }

        public async Task<IActionResult> Index() => View(await _trainerService.GetAllTrainersAsync());

        public async Task<IActionResult> Details(int id)
        {
            var trainer = await _trainerService.GetTrainerByIdAsync(id);
            if (trainer == null) return NotFound();
            return View(trainer);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trainer trainer, IFormFile? photoFile)
        {
            if (ModelState.IsValid)
            {
                await _trainerService.AddTrainerAsync(trainer, photoFile);
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var trainer = await _trainerService.GetTrainerByIdAsync(id);
            if (trainer == null) return NotFound();
            return View(trainer);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Trainer trainer, IFormFile? photoFile)
        {
            if (id != trainer.TrainerId) return NotFound();

            if (ModelState.IsValid)
            {
                await _trainerService.UpdateTrainerAsync(trainer, photoFile);
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var trainer = await _trainerService.GetTrainerByIdAsync(id);
            if (trainer == null) return NotFound();
            return View(trainer);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _trainerService.DeleteTrainerAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var items = await _trainerService.GetAllTrainersAsync();
            if (!string.IsNullOrEmpty(query))
            {
                items = items.Where(t => t.Name != null && t.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var safeItems = items.Select(t => new { t.TrainerId, t.Name, t.Specialization, t.PhotoUrl, t.Motto, t.Experience, t.Availability, t.PhoneNumber });
            return Json(safeItems);
        }
    }
}