using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Services
{
    public class ClassReservationService : IClassReservationService
    {
        private readonly IRepositoryWrapper _repository;

        public ClassReservationService(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ClassReservation>> GetUserReservationsAsync(string email)
        {
            var user = await _repository.Member.GetByEmailAsync(email);
            if (user == null) return new List<ClassReservation>();

            return await _repository.ClassReservation.GetReservationsByUserAsync(user.MemberId);
        }

        public async Task<(bool Success, string Message)> BookClassAsync(string email, int classId)
        {
            var user = await _repository.Member.GetByEmailAsync(email);
            if (user == null) return (false, "Please login to book a class.");

            var fitnessClass = await _repository.FitnessClass.FindByCondition(c => c.FitnessClassId == classId).FirstOrDefaultAsync();
            if (fitnessClass == null) return (false, "Class not found.");

            if (await _repository.ClassReservation.HasUserBookedClassAsync(user.MemberId, classId))
                return (false, "You are already booked for this class! Hit the gym!");

            var reservation = new ClassReservation
            {
                MemberId = user.MemberId,
                FitnessClassId = classId,
                ReservationDate = DateTime.Now
            };

            _repository.ClassReservation.Create(reservation);
            await _repository.SaveAsync();

            return (true, $"Boom! You successfully booked a spot for {fitnessClass.ClassName}.");
        }

        public async Task<(bool Success, string Message)> CancelReservationAsync(string email, int reservationId)
        {
            var user = await _repository.Member.GetByEmailAsync(email);
            if (user == null) return (false, "User not found.");

            var reservation = await _repository.ClassReservation.FindByCondition(r => r.ClassReservationId == reservationId).FirstOrDefaultAsync();
            if (reservation == null || reservation.MemberId != user.MemberId)
                return (false, "Unauthorized or reservation not found.");

            _repository.ClassReservation.Delete(reservation);
            await _repository.SaveAsync();

            return (true, "Reservation cancelled. No excuses next time!");
        }
    }
}