using System;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Data.Entities
{
    public class Version
    {
        [Key]
        public int Id { get; set; }
        public string NroVersion { get; set; }
        public DateTime Fecha { get; set; }
    }
}