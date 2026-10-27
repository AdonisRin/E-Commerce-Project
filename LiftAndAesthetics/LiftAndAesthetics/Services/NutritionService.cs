using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Services
{
    public class NutritionService : INutritionService
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NutritionService(IRepositoryWrapper repository, IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
        }

        private async Task<string?> ProcessUploadedFile(IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "photos");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
                return "/photos/" + uniqueFileName;
            }
            return null;
        }

        public async Task<IEnumerable<NutritionPlan>> GetAllPlansAsync()
        {
            return await _repository.Nutrition.FindAll().ToListAsync();
        }

        public async Task<NutritionPlan?> GetPlanByIdAsync(int id)
        {
            return await _repository.Nutrition.FindByCondition(n => n.PlanId == id).FirstOrDefaultAsync();
        }

        public async Task AddPlanAsync(NutritionPlan plan, IFormFile? imageFile)
        {
            var imagePath = await ProcessUploadedFile(imageFile);
            if (imagePath != null) plan.ImageUrl = imagePath;

            _repository.Nutrition.Create(plan);
            _repository.Save();
            await Task.CompletedTask;
        }

        public async Task UpdatePlanAsync(NutritionPlan plan, IFormFile? imageFile)
        {
            var imagePath = await ProcessUploadedFile(imageFile);
            if (imagePath != null) plan.ImageUrl = imagePath;

            _repository.Nutrition.Update(plan);
            _repository.Save();
            await Task.CompletedTask;
        }

        public async Task DeletePlanAsync(int id)
        {
            var plan = await GetPlanByIdAsync(id);
            if (plan != null)
            {
                _repository.Nutrition.Delete(plan);
                _repository.Save();
            }
        }
    }
}