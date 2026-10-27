using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiftAndAesthetics.Services
{
    public class TrainerService : ITrainerService
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TrainerService(IRepositoryWrapper repository, IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<Trainer>> GetAllTrainersAsync() =>
            await _repository.Trainer.FindAll().ToListAsync();

        public async Task<Trainer?> GetTrainerByIdAsync(int id) =>
            await _repository.Trainer.FindByCondition(t => t.TrainerId == id).FirstOrDefaultAsync();

        public async Task AddTrainerAsync(Trainer trainer, IFormFile? photoFile)
        {
            if (photoFile != null) trainer.PhotoUrl = await UploadFile(photoFile);
            _repository.Trainer.Create(trainer);
            _repository.Save();
        }

        public async Task UpdateTrainerAsync(Trainer trainer, IFormFile? photoFile)
        {
            if (photoFile != null) trainer.PhotoUrl = await UploadFile(photoFile);
            _repository.Trainer.Update(trainer);
            _repository.Save();
        }

        public async Task DeleteTrainerAsync(int id)
        {
            var trainer = await GetTrainerByIdAsync(id);
            if (trainer != null)
            {
                _repository.Trainer.Delete(trainer);
                _repository.Save();
            }
        }

        public bool TrainerExists(int id) =>
            _repository.Trainer.FindByCondition(t => t.TrainerId == id).Any();

        private async Task<string> UploadFile(IFormFile file)
        {
            string folder = Path.Combine(_webHostEnvironment.WebRootPath, "photos");
            string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(folder, fileName);
            using (var fs = new FileStream(filePath, FileMode.Create)) { await file.CopyToAsync(fs); }
            return "/photos/" + fileName;
        }
    }
}