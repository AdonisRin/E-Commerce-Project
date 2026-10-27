using System;
using System.ComponentModel.DataAnnotations;

namespace LiftAndAesthetics.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SenderEmail { get; set; } = string.Empty;

        [Required]
        public string ReceiverEmail { get; set; } = string.Empty; 

        [Required]
        public string Content { get; set; } = string.Empty;

        public string? AttachmentUrl { get; set; } 

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false;
        public bool IsEdited { get; set; } = false;
    }
}