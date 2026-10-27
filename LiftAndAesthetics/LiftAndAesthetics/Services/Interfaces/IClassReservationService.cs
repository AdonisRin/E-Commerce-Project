using LiftAndAesthetics.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Services.Interfaces
{
    public interface IClassReservationService
    {
        Task<IEnumerable<ClassReservation>> GetUserReservationsAsync(string email);
        Task<(bool Success, string Message)> BookClassAsync(string email, int classId);
        Task<(bool Success, string Message)> CancelReservationAsync(string email, int reservationId);
    }
}