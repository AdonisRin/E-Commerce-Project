using LiftAndAesthetics.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Services.Interfaces
{
    public interface INutritionService
    {
        Task<IEnumerable<NutritionPlan>> GetAllPlansAsync();
        Task<NutritionPlan?> GetPlanByIdAsync(int id);
        Task AddPlanAsync(NutritionPlan plan, IFormFile? imageFile); 
        Task UpdatePlanAsync(NutritionPlan plan, IFormFile? imageFile); 
        Task DeletePlanAsync(int id);
    }
}