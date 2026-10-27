using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LiftAndAesthetics.Models
{
    public class Membership
    {
        public int MembershipId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        public string? Features { get; set; }

        public ICollection<Members>? Members { get; set; }

        public ICollection<UserMembership>? UserMemberships { get; set; }
    }
}