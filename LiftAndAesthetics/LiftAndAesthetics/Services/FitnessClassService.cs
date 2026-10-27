using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiftAndAesthetics.Services
{
    public class FitnessClassService : IFitnessClassService
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IWebHostEnvironment _env;

        public FitnessClassService(IRepositoryWrapper repository, IWebHostEnvironment env)
        {
            _repository = repository;
            _env = env;
        }

        public async Task<IEnumerable<FitnessClass>> GetAllClassesAsync() =>
            await _repository.FitnessClass.FindAll().Include(c => c.Trainer).ToListAsync();

        public async Task<FitnessClass?> GetClassDetailsAsync(int id) =>
            await _repository.FitnessClass.FindByCondition(c => c.FitnessClassId == id).Include(c => c.Trainer).FirstOrDefaultAsync();

        public async Task<FitnessClass?> GetClassForEditAsync(int id) =>
            await _repository.FitnessClass.FindByCondition(c => c.FitnessClassId == id).FirstOrDefaultAsync();

        public async Task AddClassAsync(FitnessClass fc, IFormFile? m, IFormFile? e1, IFormFile? e2)
        {
            if (m != null) fc.MainPhotoUrl = await SaveImg(m);
            if (e1 != null) fc.ExtraPhoto1Url = await SaveImg(e1);
            if (e2 != null) fc.ExtraPhoto2Url = await SaveImg(e2);
            _repository.FitnessClass.Create(fc);
            _repository.Save();
        }

        public async Task UpdateClassAsync(FitnessClass fc, IFormFile? m, IFormFile? e1, IFormFile? e2)
        {
            if (m != null) fc.MainPhotoUrl = await SaveImg(m);
            if (e1 != null) fc.ExtraPhoto1Url = await SaveImg(e1);
            if (e2 != null) fc.ExtraPhoto2Url = await SaveImg(e2);
            _repository.FitnessClass.Update(fc);
            _repository.Save();
        }

        public async Task DeleteClassAsync(int id)
        {
            var fc = await GetClassForEditAsync(id);
            if (fc != null) { _repository.FitnessClass.Delete(fc); _repository.Save(); }
        }

        public bool ClassExists(int id) => _repository.FitnessClass.FindByCondition(c => c.FitnessClassId == id).Any();

        private async Task<string> SaveImg(IFormFile f)
        {
            string path = Path.Combine(_env.WebRootPath, "photos", Guid.NewGuid() + "_" + f.FileName);
            using (var s = new FileStream(path, FileMode.Create)) { await f.CopyToAsync(s); }
            return "/photos/" + Path.GetFileName(path);
        }
    }
}