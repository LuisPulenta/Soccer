using System;
using System.Collections.Generic;
using System.Text;

namespace Soccer.Common.Models
{
    public class PredictionResponse4
    {
        public int Id { get; set; }
        public int? GoalsLocalPrediction { get; set; }
        public int? GoalsVisitorPrediction { get; set; }
        public int? GoalsLocalReal { get; set; }
        public int? GoalsVisitorReal { get; set; }
        public int? Points { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string PlayerNickName { get; set; }
        public string PlayerPicture { get; set; }
        public int MatchId { get; set; }
        public DateTime MatchDate { get; set; }
        public int TournamentId { get; set; }
        public string NameLocal { get; set; }
        public string NameVisitor { get; set; }
        public string LogoPathLocal { get; set; }
        public string LogoPathVisitor { get; set; }
        public string InitialsLocal { get; set; }
        public string InitialsVisitor { get; set; }
    }
}
