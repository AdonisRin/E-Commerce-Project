using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiftAndAesthetics.Models
{
    public class ClassReservation
    {
        [Key]
        public int ClassReservationId { get; set; }

        public DateTime ReservationDate { get; set; } = DateTime.Now;

        [Required]
        public int MemberId { get; set; }
        [ForeignKey("MemberId")]
        public Members? Member { get; set; }

        [Required]
        public int FitnessClassId { get; set; }
        [ForeignKey("FitnessClassId")]
        public FitnessClass? FitnessClass { get; set; }
    }
}