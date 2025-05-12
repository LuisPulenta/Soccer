using System.Collections.Generic;

namespace Soccer.Common.Models
{
    public class LeagueResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoPath { get; set; }
        public ICollection<TeamResponse> Teams { get; set; }
        public string LogoFullPath => string.IsNullOrEmpty(LogoPath)
            ? "https://keypress.serveftp.net/SoccerApi/Images/leagues/noimage.png"
           : $"https://keypress.serveftp.net/SoccerApi{LogoPath.Substring(1)}";
    }
}