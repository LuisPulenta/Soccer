using Microsoft.AspNetCore.Mvc;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Soccer.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaguesController : ControllerBase
    {
        private readonly DataContext _context;

        public LeaguesController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Leagues
        [HttpGet]
        public IEnumerable<LeagueEntity> GetLeagues()
        {
            return _context.Leagues.OrderBy(pt => pt.Name);
        }
    }
}