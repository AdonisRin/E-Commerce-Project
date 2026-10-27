using Microsoft.AspNetCore.Mvc;
using LiftAndAesthetics.Models;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;

namespace LiftAndAesthetics.Controllers
{
    public class FitnessClassesController : Controller
    {
        private readonly IFitnessClassService _classService;
        private readonly ITrainerService _trainerService;

        public FitnessClassesController(IFitnessClassService classService, ITrainerService trainerService)
        {
            _classService = classService;
            _trainerService = trainerService;
        }

        public async Task<IActionResult> Index() => View(await _classService.GetAllClassesAsync());

        public async Task<IActionResult> Details(int id)
        {
            var fitnessClass = await _classService.GetClassDetailsAsync(id);
            if (fitnessClass == null) return NotFound();
            return View(fitnessClass);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var trainers = await _trainerService.GetAllTrainersAsync();
            ViewData["TrainerId"] = new SelectList(trainers, "TrainerId", "Name");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FitnessClass fitnessClass, IFormFile? mainPhoto, IFormFile? extraPhoto1, IFormFile? extraPhoto2)
        {
            if (ModelState.IsValid)
            {
                await _classService.AddClassAsync(fitnessClass, mainPhoto, extraPhoto1, extraPhoto2);
                return RedirectToAction(nameof(Index));
            }
            var trainers = await _trainerService.GetAllTrainersAsync();
            ViewData["TrainerId"] = new SelectList(trainers, "TrainerId", "Name", fitnessClass.TrainerId);
            return View(fitnessClass);
        }

        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var fitnessClass = await _classService.GetClassForEditAsync(id);
            if (fitnessClass == null) return NotFound();

            var trainers = await _trainerService.GetAllTrainersAsync();
            ViewData["TrainerId"] = new SelectList(trainers, "TrainerId", "Name", fitnessClass.TrainerId);
            return View(fitnessClass);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FitnessClass fitnessClass, IFormFile? mainPhoto, IFormFile? extraPhoto1, IFormFile? extraPhoto2)
        {
            if (id != fitnessClass.FitnessClassId) return NotFound();

            if (ModelState.IsValid)
            {
                await _classService.UpdateClassAsync(fitnessClass, mainPhoto, extraPhoto1, extraPhoto2);
                return RedirectToAction(nameof(Index));
            }
            var trainers = await _trainerService.GetAllTrainersAsync();
            ViewData["TrainerId"] = new SelectList(trainers, "TrainerId", "Name", fitnessClass.TrainerId);
            return View(fitnessClass);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var fitnessClass = await _classService.GetClassDetailsAsync(id);
            if (fitnessClass == null) return NotFound();
            return View(fitnessClass);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _classService.DeleteClassAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var items = await _classService.GetAllClassesAsync();
            if (!string.IsNullOrEmpty(query))
            {
                items = items.Where(c => c.ClassName != null && c.ClassName.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var safeItems = items.Select(c => new { c.FitnessClassId, c.ClassName, c.Description, c.VideoUrl, c.MainPhotoUrl, c.ExtraPhoto1Url, c.ExtraPhoto2Url, Trainer = c.Trainer != null ? new { c.Trainer.Name } : null });
            return Json(safeItems);
        }
    }
}