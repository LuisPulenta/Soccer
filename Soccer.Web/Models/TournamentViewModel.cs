﻿using Microsoft.AspNetCore.Http;
using Soccer.Web.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Models
{
    public class TournamentViewModel : TournamentEntity
    {
        [Display(Name = "Logo")]
        public IFormFile LogoFile { get; set; }

        public string LogoFullPath => string.IsNullOrEmpty(LogoPath)
             ? "noimage"//null
             : $"https://keypress.serveftp.net/SoccerApi{LogoPath.Substring(1)}";
    }
}