using Microsoft.AspNetCore.Http;
using Soccer.Web.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Models
{
    public class GroupBetViewModel : GroupBet
    {
        [Display(Name = "Logo")]
        public IFormFile LogoFile { get; set; }

        public string LogoFullPath => string.IsNullOrEmpty(LogoPath)
             ? "noimage"//null
             : $"http://keypress.serveftp.net:88/SoccerApi{LogoPath.Substring(1)}";
    }
}