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
           ? "noimage"//null
           : $"http://keypress.serveftp.net:88/SoccerApi{LogoPath.Substring(1)}";
    }
}