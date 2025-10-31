using System.ComponentModel.DataAnnotations;

namespace Back_end_harjoitustyö.Models
{
    public class MessageDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int SenderId { get; set; }

        public int? ReceiverId { get; set; }

        public int? PreviousMessageId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}