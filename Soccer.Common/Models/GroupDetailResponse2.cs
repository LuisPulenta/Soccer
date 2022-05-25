namespace Soccer.Common.Models
{
    public class GroupDetailResponse2
    {
        public int Id { get; set; }
        public int MatchesPlayed { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesTied { get; set; }
        public int MatchesLost { get; set; }
        public int Points => MatchesWon * 3 + MatchesTied;
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference => GoalsFor - GoalsAgainst;
        public TeamResponse Team { get; set; }
    }
}