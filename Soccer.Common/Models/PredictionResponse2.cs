namespace Soccer.Common.Models
{
    public class PredictionResponse2
    {
        public int Id { get; set; }
        public int? GoalsLocal { get; set; }
        public int? GoalsVisitor { get; set; }
        public int? Points { get; set; }
        public int PlayerId { get; set; }
        public int MatchId { get; set; }
        public int TournamentId { get; set; }
        public string NameLocal { get; set; }
        public string NameVisitor { get; set; }
    }
}