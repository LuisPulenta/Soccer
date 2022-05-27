using Microsoft.AspNetCore.Mvc;
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
                //.Include(t => t.Groups)
                //.ThenInclude(g => g.GroupDetails)
                //.ThenInclude(gd => gd.Team)
                //.ThenInclude(l => l.League)

                //.Include(t => t.Groups)
                //.ThenInclude(g => g.Matches)
                //.ThenInclude(m => m.Local)

                //.Include(t => t.Groups)
                //.ThenInclude(g => g.Matches)
                //.ThenInclude(m => m.Visitor)

                //.Include(t => t.Groups)
                //.ThenInclude(g => g.Matches)
                //.ThenInclude(m => m.DateName)

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

        [HttpPost]
        [Route("GetGroups/{codigo}")]
        public async Task<IActionResult> GetGroups(int codigo)
        {
            List<GroupEntity> groups = await _context.Groups
                .Where(t => t.Tournament.Id == codigo)
                .OrderBy (c=>c.Name)

                .ToListAsync();
            return Ok(_converterHelper.ToGroupResponse(groups));
        }

        [HttpPost]
        [Route("GetGroupDetails/{codigo}")]
        public async Task<IActionResult> GetGroupDetails(int codigo)
        {
            List<GroupDetailEntity> groupDetails = await _context.GroupDetails
                .Include(t=>t.Team)
                .ThenInclude(l=>l.League)
                .Where(t => t.Group.Id == codigo)
                .OrderByDescending(c => c.Points).ThenBy(c => c.GoalDifference).ThenBy(c => c.GoalsAgainst)

                .ToListAsync();

            var res = _converterHelper.ToGroupDetailResponse(groupDetails);

            return Ok(res.Result);
        }

        [HttpPost]
        [Route("GetMatches/{codigo}")]
        public async Task<IActionResult> GetMatches(int codigo)
        {
            List<MatchEntity> matches = await _context.Matches
                .Include(t => t.Group)
                .Include(t => t.Local)
                .ThenInclude(l => l.League)
                .Include(t => t.Visitor)
                .ThenInclude(l => l.League)
                .Include(t => t.DateName)
                .Where(t => t.Group.Id == codigo)
                .OrderByDescending(c => c.DateName)

                .ToListAsync();

            var res = _converterHelper.ToMatchResponse2(matches);

            return Ok(res.Result);
        }

        [HttpPost]
        [Route("GetMyGroups/{codigo}")]
        public async Task<IActionResult> GetMyGroups(string codigo)
        {
            List<GroupBetPlayer> groupsBet = await _context.GroupBetPlayers
                .Include(t => t.GroupBet)
                .ThenInclude(t=>t.GroupBetPlayers)
                .Include(t => t.GroupBet)
                .ThenInclude(t => t.Tournament)
                .Include(t => t.GroupBet)
                .ThenInclude(t => t.Admin)
                .ThenInclude(t => t.User)
                .ThenInclude(t => t.FavoriteTeam)
                .Where(t => t.Player.User.Id == codigo && t.GroupBet.Tournament.IsActive)
                .ToListAsync();

            var res = _converterHelper.ToGroupBetResponse2(groupsBet);
            return Ok(res.Result);
        }
    }
}