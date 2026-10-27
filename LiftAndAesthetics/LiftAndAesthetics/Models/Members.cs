using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiftAndAesthetics.Models
{
    public class Members
    {
        [Key]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Please enter your name.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please create a password.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public string Role { get; set; } = "User";

        public decimal WalletBalance { get; set; } = 6767.00m;

        public int? MembershipId { get; set; }
        [ForeignKey("MembershipId")]
        public virtual Membership? Membership { get; set; }
        public DateTime? MembershipExpiryDate { get; set; }

        public virtual ICollection<UserMembership>? UserMemberships { get; set; }
        public int? Age { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public int? Height { get; set; }
        public double? Weight { get; set; }
        public string? Address { get; set; }
    }
}