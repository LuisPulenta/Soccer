using System;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Common.Models
{
    public class UserRequest
    {
        public string Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string NickName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string PasswordConfirm { get; set; }

        public int LeagueId { get; set; }
        
        public int TeamId { get; set; }

        public byte[] PictureArray { get; set; }
    }
}
