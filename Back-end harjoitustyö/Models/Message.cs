using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Back_end_harjoitustyö.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int SenderId { get; set; }

        [ForeignKey(nameof(SenderId))]
        public User? Sender { get; set; }

        public int? ReceiverId { get; set; }

        [ForeignKey(nameof(ReceiverId))]
        public User? Receiver { get; set; }

        public int? PreviousMessageId { get; set; }

        [ForeignKey(nameof(PreviousMessageId))]
        public Message? PreviousMessage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}