using System;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Common.Models
{
    public class UserRequest
    {
        [Required]
        public string Document { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public DateTime BornDate { get; set; }

        [Required]
        public string Sex { get; set; }

        [Required]
        public string NickName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string PasswordConfirm { get; set; }

        public int LeagueId { get; set; }
        
        public int TeamId { get; set; }

        public byte[] PictureArray { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
