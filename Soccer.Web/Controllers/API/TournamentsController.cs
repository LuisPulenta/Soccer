﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConverterHelper _converterHelper;

        public TournamentsController(DataContext context, IConverterHelper converterHelper)
        {
            _context = context;
            _converterHelper = converterHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetTournament()
        {
            List<TournamentEntity> tournaments = await _context.Tournaments
                .Include(t => t.Groups)
                .ThenInclude(g => g.GroupDetails)
                .ThenInclude(gd => gd.Team)
                .ThenInclude(l => l.League)


                .Include(t => t.Groups)
                .ThenInclude(g => g.Matches)
                .ThenInclude(m => m.Local)
                .Include(t => t.Groups)
                .ThenInclude(g => g.Matches)
                .ThenInclude(m => m.Visitor)

                //.Include(t => t.Groups)
                //.ThenInclude(g => g.Matches)
                //.ThenInclude(m => m.Predictions)
                //.ThenInclude(p => p.Player)
                //.ThenInclude(p => p.User)

                //.Include(t => t.DateNames)
                //.ThenInclude(g => g.Matches)
                //.ThenInclude(m => m.Local)
                //.Include(t => t.DateNames)
                //.ThenInclude(g => g.Matches)
                //.ThenInclude(m => m.Visitor)

                //.Include(t => t.DateNames)
                //.ThenInclude(g => g.Matches)
                //.ThenInclude(m => m.Predictions)
                //.ThenInclude(p => p.Player)
                //.ThenInclude(p => p.User)
                .Where(t => t.IsActive)

                .ToListAsync();
            return Ok(_converterHelper.ToTournamentResponse(tournaments));
        }

    }
}