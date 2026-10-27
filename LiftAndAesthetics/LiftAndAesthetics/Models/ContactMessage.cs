using System;
using System.ComponentModel.DataAnnotations;

namespace LiftAndAesthetics.Models
{
    public class ContactMessage
    {
        public int ContactMessageId { get; set; }

        [Required(ErrorMessage = "Please enter your name.")]
        public string? SenderName { get; set; }

        [Required(ErrorMessage = "Please enter an email address.")]
        [EmailAddress(ErrorMessage = "Please enter a VALID email address (e.g. name@domain.com).")]
        public string? SenderEmail { get; set; }

        [Required(ErrorMessage = "Please enter a message.")]
        public string? MessageContent { get; set; }

        public DateTime SentDate { get; set; } = DateTime.Now;
    }
}