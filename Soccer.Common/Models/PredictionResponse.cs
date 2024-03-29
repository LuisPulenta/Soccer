﻿namespace Soccer.Common.Models
{
    public class PredictionResponse
    {
        public int Id { get; set; }
        public int? GoalsLocal { get; set; }
        public int? GoalsVisitor { get; set; }
        public int? Points { get; set; }
        public PlayerResponse Player { get; set; }
        public MatchResponse Match { get; set; }
    }
}