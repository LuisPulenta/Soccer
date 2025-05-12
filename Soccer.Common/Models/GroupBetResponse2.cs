using System;
using System.Collections.Generic;

namespace Soccer.Common.Models
{
    public class GroupBetResponse2
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoPath { get; set; }
        public String AdminName { get; set; }
        public String AdminNickName { get; set; }
        public String AdminPicture { get; set; }
        public String AdminTeam { get; set; }
        public String TournamentName { get; set; }
        public int TournamentId { get; set; }
        public DateTime CreationDate { get; set; }
        public ICollection<GroupBetPlayerResponse2> GroupBetPlayers { get; set; }
        public int? CantPlayers { get; set; }
        public string LogoFullPath => string.IsNullOrEmpty(LogoPath)
           ? "noimage"//null
           : $"https://keypress.serveftp.net/SoccerApi{LogoPath.Substring(1)}";
    }
}