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
            ? "http://keypress.serveftp.net:88/SoccerApi/Images/leagues/noimage.png"
           : $"http://keypress.serveftp.net:88/SoccerApi{LogoPath.Substring(1)}";
    }
}