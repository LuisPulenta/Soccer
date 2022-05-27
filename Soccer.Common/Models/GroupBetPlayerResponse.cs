using System.Collections.Generic;

namespace Soccer.Common.Models
{
    public class GroupBetPlayerResponse
    {
        public int Id { get; set; }
        public GroupBetResponse GroupBet { get; set; }
        public PlayerResponse Player { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsBlocked { get; set; }
        public int Points { get; set; }
    }
}