using Soccer.Common.Enums;
using System;

namespace Soccer.Common.Models
{
    public class PlayerResponse
    {
        public int Id { get; set; }
        public string Document { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime BornDate { get; set; }
        public string Sex { get; set; }
        public string PicturePath { get; set; }
        public string NickName { get; set; }
        public TeamResponse Team { get; set; }
        public int Points { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public UserType UserType { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string FullNameWithDocument => $"{FirstName} {LastName} - {Document}";
    }
}