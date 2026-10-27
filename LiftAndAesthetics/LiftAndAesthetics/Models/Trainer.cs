    using System.Collections.Generic;

    namespace LiftAndAesthetics.Models
    {
        public class Trainer
        {
            public int TrainerId { get; set; }
            public string? Name { get; set; }
            public string? Specialization { get; set; }

            public string? PhotoUrl { get; set; } 
            public string? Motto { get; set; }
            public string? Experience { get; set; } 
            public string? Availability { get; set; }
            public string? PhoneNumber { get; set; }

            public ICollection<FitnessClass>? FitnessClasses { get; set; }
        }
    }