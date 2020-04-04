using System;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Common.Models
{
    public class GroupBetRequest
    {
        [Required]
        public string Name { get; set; }

        public int TournamentId { get; set; }

        public byte[] PictureArray { get; set; }

        public string PlayerEmail { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
