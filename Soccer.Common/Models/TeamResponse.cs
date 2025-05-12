namespace Soccer.Common.Models
{
    public class TeamResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Initials { get; set; }
        public string LogoPath { get; set; }
        public int LeagueId { get; set; }
        public string LeagueName { get; set; }
        public string LogoFullPath => string.IsNullOrEmpty(LogoPath)
           ? "https://keypress.serveftp.net/SoccerApi/Images/teams/noimage.png"
           : $"https://keypress.serveftp.net/SoccerApi{LogoPath.Substring(1)}";
    }
}