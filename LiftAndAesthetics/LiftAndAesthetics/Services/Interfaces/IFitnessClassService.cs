using LiftAndAesthetics.Models;
using Microsoft.AspNetCore.Http;

namespace LiftAndAesthetics.Services.Interfaces
{
    public interface IFitnessClassService
    {
        Task<IEnumerable<FitnessClass>> GetAllClassesAsync();
        Task<FitnessClass?> GetClassDetailsAsync(int id);
        Task<FitnessClass?> GetClassForEditAsync(int id);
        Task AddClassAsync(FitnessClass fitnessClass, IFormFile? main, IFormFile? extra1, IFormFile? extra2);
        Task UpdateClassAsync(FitnessClass fitnessClass, IFormFile? main, IFormFile? extra1, IFormFile? extra2);
        Task DeleteClassAsync(int id);
        bool ClassExists(int id);
    }
}