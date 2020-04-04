using System.Collections.Generic;

namespace Soccer.Common.Models
{
    public class PlayerGroupBetResponse
    {
        public int Id { get; set; }
        public ICollection<GroupBetResponse> GroupBets { get; set; }
    }
}
