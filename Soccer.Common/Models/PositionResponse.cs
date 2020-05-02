using Soccer.Common.Models;
using System.Collections.Generic;

namespace Soccer.Common.Models
{
    public class PositionResponse
    {
        public PlayerResponse PlayerResponse { get; set; }

        public int? Points { get; set; }

        public int Ranking { get; set; }

    }
}