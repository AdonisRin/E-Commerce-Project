using LiftAndAesthetics.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories.Interfaces
{
    public interface IClassReservationRepository : IRepositoryBase<ClassReservation>
    {
        Task<IEnumerable<ClassReservation>> GetReservationsByUserAsync(int memberId);
        Task<bool> HasUserBookedClassAsync(int memberId, int classId);
    }
}