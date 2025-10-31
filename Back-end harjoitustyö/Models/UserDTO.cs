using System.ComponentModel.DataAnnotations;

namespace Back_end_harjoitustyö.Models
{
    public class UserDTO
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public DateTime LastLogin { get; set; }
    }

    public class UserCreateDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}