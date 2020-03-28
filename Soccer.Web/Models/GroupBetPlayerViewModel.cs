using Microsoft.AspNetCore.Http;
using Soccer.Web.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Models
{
    public class GroupBetPlayerViewModel : GroupBetPlayer
    {
        public int GroupBetId{ get; set; }

       
    }
}
