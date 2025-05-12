using System;
using System.Collections.Generic;

namespace Soccer.Common.Models
{
    public class GroupBetResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoPath { get; set; }
        public PlayerResponse Admin { get; set; }
        public TournamentResponse Tournament { get; set; }
        public DateTime CreationDate { get; set; }
        public ICollection<GroupBetPlayerResponse> GroupBetPlayers { get; set; }
        public int? CantPlayers { get; set; }
        public string LogoFullPath => string.IsNullOrEmpty(LogoPath)
           ? "noimage"//null
           : $"https://keypress.serveftp.net/SoccerApi{LogoPath.Substring(1)}";
    }
}