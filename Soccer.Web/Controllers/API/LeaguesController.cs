using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Soccer.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaguesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConverterHelper _converterHelper;

        public LeaguesController(DataContext context, IConverterHelper converterHelper)
        {
            _context = context;
            _converterHelper = converterHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetLeagues()
        {
            List<LeagueEntity> leagues = await _context.Leagues
                .Include(t => t.Teams)
                .ToListAsync();
            return Ok(_converterHelper.ToLeagueResponse(leagues));
        }

    }
}