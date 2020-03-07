using Microsoft.AspNetCore.Http;
using Soccer.Web.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Models
{
    public class LeagueViewModel : LeagueEntity
    {
        [Display(Name = "Logo")]
        public IFormFile LogoFile { get; set; }
    }
}