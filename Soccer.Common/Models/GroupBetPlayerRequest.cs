namespace Soccer.Common.Models
{
    public class GroupBetPlayerRequest
    {
        public int Id { get; set; }
        public int GroupBetId { get; set; }
        public int PlayerId { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsBlocked { get; set; }
        public int Points { get; set; }
    }
}
