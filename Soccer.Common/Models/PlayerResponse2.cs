using System;
using System.Collections.Generic;
using System.Text;

namespace Soccer.Common.Models
{
    public class PlayerResponse2
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PicturePath { get; set; }
        public string NickName { get; set; }
        public TeamResponse Team { get; set; }
        public int Points { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public ICollection<PredictionResponse2> Predictions { get; set; }
        public string PictureFullPath => string.IsNullOrEmpty(PicturePath)
         ? "noimage"//null
         : $"http://keypress.serveftp.net:88/SoccerApi{PicturePath.Substring(1)}";

    }
}
