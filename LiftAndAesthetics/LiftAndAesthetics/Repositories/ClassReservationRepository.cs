using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories
{
    public class ClassReservationRepository : RepositoryBase<ClassReservation>, IClassReservationRepository
    {
        public ClassReservationRepository(LiftContext context) : base(context) { }

        public async Task<IEnumerable<ClassReservation>> GetReservationsByUserAsync(int memberId)
        {
            return await FindByCondition(r => r.MemberId == memberId)
                .Include(r => r.FitnessClass) 
                .OrderByDescending(r => r.ReservationDate)
                .ToListAsync();
        }

        public async Task<bool> HasUserBookedClassAsync(int memberId, int classId)
        {
            return await FindByCondition(r => r.MemberId == memberId && r.FitnessClassId == classId).AnyAsync();
        }
    }
}