using System.Collections.Generic;

namespace Soccer.Web.Data.Entities
{
    public class Player
    {
        public int Id { get; set; }

        public User User { get; set; }

        public ICollection<PredictionEntity> Predictions { get; set; }

    }
}