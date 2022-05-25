using Soccer.Common.Enums;
using System;

namespace Soccer.Common.Models
{
    public class PlayerResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PicturePath { get; set; }
        public string NickName { get; set; }
        public TeamResponse Team { get; set; }
        public int Points { get; set; }
        public string Email { get; set; }
        public UserType UserType { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string PictureFullPath => string.IsNullOrEmpty(PicturePath)
         ? "http://keypress.serveftp.net:88/SoccerApi/Images/users/nouser.png"
         : $"http://keypress.serveftp.net:88/SoccerApi{PicturePath.Substring(1)}";
    
    }
}