using LiftAndAesthetics.Models;
using Microsoft.AspNetCore.Http;

namespace LiftAndAesthetics.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<Trainer>> GetAllTrainersAsync();
        Task<Trainer?> GetTrainerByIdAsync(int id);
        Task AddTrainerAsync(Trainer trainer, IFormFile? photoFile);
        Task UpdateTrainerAsync(Trainer trainer, IFormFile? photoFile);
        Task DeleteTrainerAsync(int id);
        bool TrainerExists(int id);
    }
}