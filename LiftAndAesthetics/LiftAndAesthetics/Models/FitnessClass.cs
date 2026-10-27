using System;
using System.Collections.Generic;

namespace LiftAndAesthetics.Models
{
    public class FitnessClass
    {
        public int FitnessClassId { get; set; }
        public string? ClassName { get; set; }
        public DateTime ScheduleTime { get; set; }

        public string? Description { get; set; }
        public string? VideoUrl { get; set; }
        public string? MainPhotoUrl { get; set; }
        public string? ExtraPhoto1Url { get; set; }
        public string? ExtraPhoto2Url { get; set; }

        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        public ICollection<ClassReservation>? ClassReservations { get; set; }
    }
}