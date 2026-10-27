using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiftAndAesthetics.Models
{
    public class UserMembership
    {
        [Key]
        public int Id { get; set; }

        public int MemberId { get; set; }
        [ForeignKey("MemberId")]
        public virtual Members? Member { get; set; }

        public int MembershipId { get; set; }
        [ForeignKey("MembershipId")]
        public virtual Membership? Membership { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}