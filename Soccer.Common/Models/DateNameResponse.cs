using System;
using System.Collections.Generic;

namespace Soccer.Common.Models
{
    public class DateNameResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TournamentResponse Tournament { get; set; }
        public ICollection<MatchResponse> Matches { get; set; }
    }
}
