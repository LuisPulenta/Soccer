using System;
using Soccer.Common.Enum;
using Soccer.Web.Data.Entities;

namespace Soccer.Web.Data.Entities
{
    public class PlayerGroupBetRequestEntity
    {
        public int Id { get; set; }

        public Player ProposalPlayer { get; set; }

        public Player RequiredPlayer { get; set; }

        public GroupBet GroupBet { get; set; }

        public PlayerGroupBetStatus Status { get; set; }

        public Guid Token { get; set; }
    }
}