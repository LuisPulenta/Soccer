using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Controllers.API
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;

        public PredictionsController(DataContext context, IConverterHelper converterHelper,IUserHelper userHelper)
        {
            _context = context;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
        }

        [HttpPost]
        [Route("GetPredictionsForUser")]
        public async Task<IActionResult> GetPredictionsForUser([FromBody] PredictionsForUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            TournamentEntity tournament = await _context.Tournaments.FindAsync(request.TournamentId);
            if (tournament == null)
            {
                return BadRequest("Este Torneo no existe.");
            }

            Player player = await _context.Players
                .Include(u => u.User.FavoriteTeam)
                .ThenInclude(l => l.League)
                .Include(pr => pr.Predictions)
                .ThenInclude(p => p.Match)
                .ThenInclude(m => m.Local)
                .ThenInclude(l => l.League)
                .Include(pr => pr.Predictions)
                .ThenInclude(p => p.Match)
                .ThenInclude(m => m.Visitor)
                .ThenInclude(l => l.League)
                .Include(pr => pr.Predictions)
                .ThenInclude(p => p.Match)
                .ThenInclude(p => p.Group)
                .ThenInclude(p => p.Tournament)
                .FirstOrDefaultAsync(u => u.User.Id == request.UserId.ToString());
            if (player == null)
            {
                return BadRequest("Este Usuario no existe.");
            }

            // Add precitions already done
            List<PredictionResponse> predictionResponses = new List<PredictionResponse>();
            foreach (PredictionEntity predictionEntity in player.Predictions)
            {
                if (predictionEntity.Match.Group.Tournament.Id == request.TournamentId)
                {
                    predictionResponses.Add(_converterHelper.ToPredictionResponse(predictionEntity));
                }
            }

            // Add precitions undone
            List<MatchEntity> matches = await _context.Matches
                .Include(m => m.Local)
                .ThenInclude(l=>l.League)
                .Include(m => m.Visitor)
                .ThenInclude(l => l.League)
                .Include(g => g.Group)
                .Include(d => d.DateName)
                .Where(m => m.Group.Tournament.Id == request.TournamentId)
                .ToListAsync();
            foreach (MatchEntity matchEntity in matches)
            {
                PredictionResponse predictionResponse = predictionResponses.FirstOrDefault(pr => pr.Match.Id == matchEntity.Id);
                if (predictionResponse == null)
                {
                    predictionResponses.Add(new PredictionResponse
                    {
                        Match = _converterHelper.ToMatchResponse(matchEntity),
                    });
                }
            }

            return Ok(predictionResponses.OrderBy(pr => pr.Id).ThenBy(pr => pr.Match.Date));
        }



        [HttpGet]
        [Route("GetPredictionsForUserInOneTournament/{id}/{id2}")]

        public async Task<IActionResult> GetPredictionsForUserInOneTournament([FromRoute] int id, [FromRoute] int id2)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            TournamentEntity tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null)
            {
                return BadRequest("Este Torneo no existe.");
            }

            Player player = await _context.Players
                .Include(u => u.User.FavoriteTeam)
                .ThenInclude(l => l.League)
                .Include(pr => pr.Predictions)
                .ThenInclude(p => p.Match)
                .ThenInclude(m => m.Local)
                .ThenInclude(l => l.League)
                .Include(pr => pr.Predictions)
                .ThenInclude(p => p.Match)
                .ThenInclude(m => m.Visitor)
                .ThenInclude(l => l.League)
                .Include(pr => pr.Predictions)
                .ThenInclude(p => p.Match)
                .ThenInclude(p => p.Group)
                .ThenInclude(p => p.Tournament)
                .FirstOrDefaultAsync(u => u.Id == id2);
            if (player == null)
            {
                return BadRequest("Este Usuario no existe.");
            }

            // Add precitions already done
            List<PredictionResponse3> predictionResponses = new List<PredictionResponse3>();
            foreach (PredictionEntity predictionEntity in player.Predictions)
            {
                if (predictionEntity.Match.Group.Tournament.Id == id)
                {
                    predictionResponses.Add(_converterHelper.ToPredictionResponse3(predictionEntity));
                }
            }
            return Ok(predictionResponses.OrderBy(pr => pr.Id).ThenBy(pr => pr.MatchDate));
        }













        [HttpPost]
        public async Task<IActionResult> PostPrediction([FromBody] PredictionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MatchEntity matchEntity = await _context.Matches.FindAsync(request.MatchId);
            if (matchEntity == null)
            {
                return BadRequest("Este partido no existe.");
            }

            if (matchEntity.IsClosed)
            {
                return BadRequest("Este partido está cerrado.");
            }

            User userEntity = await _userHelper.GetUserAsync(request.UserId);

            if (userEntity == null)
            {
                return BadRequest("Este usuario no existe.");
            }

            if (matchEntity.Date <= DateTime.UtcNow)
            {
                return BadRequest("Este partido ya empezó.");
            }

            PredictionEntity predictionEntity = await _context.Predictions
                .FirstOrDefaultAsync(p => p.Player.User.Id == request.UserId.ToString() && p.Match.Id == request.MatchId);

            if (predictionEntity == null)
            {
                //var playerResponse = JsonConvert.DeserializeObject<PlayerResponse>(Settings.Player);

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId.ToString());
                var player = await _context.Players.FirstOrDefaultAsync(u => u.User.Id == user.Id);




                predictionEntity = new PredictionEntity
                {
                    GoalsLocal = request.GoalsLocal,
                    GoalsVisitor = request.GoalsVisitor,
                    Match = matchEntity,
                    Player= player
                };

                _context.Predictions.Add(predictionEntity);
            }
            else
            {
                predictionEntity.GoalsLocal = request.GoalsLocal;
                predictionEntity.GoalsVisitor = request.GoalsVisitor;
                _context.Predictions.Update(predictionEntity);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet]
        [Route("GetPositionsByTournament/{id}/{id2}")]
        public async Task<IActionResult> GetPositionsByTournament([FromRoute] int id, [FromRoute] int id2)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TournamentEntity tournament = await _context.Tournaments
                .Include(g => g.GroupBets)
                .ThenInclude(gb => gb.GroupBetPlayers)
                .ThenInclude(p => p.Player)
                .ThenInclude(pr => pr.Predictions)
                .ThenInclude(m => m.Match)
                .ThenInclude(l => l.Local)
                .ThenInclude(le => le.League)

                .Include(g => g.GroupBets)
                .ThenInclude(gb => gb.GroupBetPlayers)
                .ThenInclude(p => p.Player)
                .ThenInclude(pr => pr.Predictions)
                .ThenInclude(m => m.Match)
                .ThenInclude(v => v.Visitor)
                .ThenInclude(l => l.League)

                 .Include(g => g.GroupBets)
                .ThenInclude(gb => gb.GroupBetPlayers)
                .ThenInclude(p => p.Player)
                .ThenInclude(pr => pr.Predictions)
                .ThenInclude(m => m.Match)
                .ThenInclude(g => g.Group)
                .ThenInclude(t => t.Tournament)

                .Include(gb => gb.GroupBets)
                .ThenInclude(a => a.Admin)
                .ThenInclude(u => u.User)
                .ThenInclude(f => f.FavoriteTeam)
                .ThenInclude(l => l.League)

                .Include(g => g.GroupBets)
                .ThenInclude(gb => gb.GroupBetPlayers)
                .ThenInclude(p => p.Player)
                .ThenInclude(pr => pr.Predictions)
                .ThenInclude(pr => pr.Player)
                .ThenInclude(u => u.User)
                .ThenInclude(f => f.FavoriteTeam)
                .ThenInclude(l => l.League)

                .FirstOrDefaultAsync(t => t.Id == id);
            if (tournament == null)
            {
                return BadRequest("Este Torneo no existe");
            }

            List<PositionResponse> positionResponses = new List<PositionResponse>();
            foreach (GroupBet groupEntity in tournament.GroupBets)

            {
                if (groupEntity.Id == id2)
                {
                    foreach (GroupBetPlayer groupBetPlayer in groupEntity.GroupBetPlayers)
                    {
                        foreach (PredictionEntity predictionEntity in groupBetPlayer.Player.Predictions)
                        {
                            if (predictionEntity.Match.Group.Tournament.Id == id)
                            {
                                PositionResponse positionResponse = positionResponses.FirstOrDefault(pr => pr.PlayerResponse.Id == predictionEntity.Player.Id);
                                if (positionResponse == null)
                                {
                                    int? pp = 0;
                                    if (predictionEntity.Points == null) { pp = 0; };
                                    if (!(predictionEntity.Points == null)) { pp = predictionEntity.Points; };
                                    positionResponses.Add(new PositionResponse
                                    {
                                        Points = pp,
                                        PlayerResponse = _converterHelper.ToPlayerResponse(predictionEntity.Player),
                                    });
                                }
                                else
                                {
                                    int? pp = 0;
                                    if (predictionEntity.Points == null) { pp = 0; };
                                    if (!(predictionEntity.Points == null)) { pp = predictionEntity.Points; };
                                    positionResponse.Points += pp;
                                }
                            }
                        }
                    }
                }
            }
            List<PositionResponse> list = positionResponses.OrderByDescending(pr => pr.Points).ToList();
            int i = 1;
            foreach (PositionResponse item in list)
            {
                item.Ranking = i;
                i++;
            }
            return Ok(list);
        }
    }
}