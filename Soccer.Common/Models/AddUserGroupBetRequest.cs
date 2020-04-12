using System.ComponentModel.DataAnnotations;

namespace Soccer.Common.Models
{
    public class AddUserGroupBetRequest
    {
        [Required]
        public int PlayerId { get; set; }

        [Required]
        public int GroupBetId { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

    }
}