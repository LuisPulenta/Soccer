using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                .Include(u => u.Predictions)
                .ThenInclude(p => p.Match)
                .ThenInclude(m => m.Local)
                .ThenInclude(l => l.League)
                .Include(u => u.Predictions)
                .ThenInclude(p => p.Match)
                .ThenInclude(m => m.Visitor)
                .ThenInclude(l => l.League)
                .Include(u => u.Predictions)
                .ThenInclude(p => p.Match)
                .ThenInclude(p => p.Group)
                .ThenInclude(p => p.Tournament)
                .FirstOrDefaultAsync(u => u.User.Id == request.UserId.ToString());
            if (player == null)
            {
                return BadRequest("Este Usurio no existe.");
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
                .Include(m => m.Visitor)
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
                predictionEntity = new PredictionEntity
                {
                    GoalsLocal = request.GoalsLocal,
                    GoalsVisitor = request.GoalsVisitor,
                    Match = matchEntity,
                    Player=_converterHelper.ToPlayer(userEntity)
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPositionsByTournament([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TournamentEntity tournament = await _context.Tournaments
                .Include(t => t.Groups)
                .ThenInclude(g => g.Matches)
                .ThenInclude(m => m.Predictions)
                .ThenInclude(p => p.Player)
                .ThenInclude(us => us.User)
                .ThenInclude(u => u.FavoriteTeam)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tournament == null)
            {
                return BadRequest("Este Torneo no existe");
            }

            List<PositionResponse> positionResponses = new List<PositionResponse>();
            foreach (GroupEntity groupEntity in tournament.Groups)
            {
                foreach (MatchEntity matchEntity in groupEntity.Matches)
                {
                    foreach (PredictionEntity predictionEntity in matchEntity.Predictions)
                    {
                        PositionResponse positionResponse = positionResponses.FirstOrDefault(pr => pr.PlayerResponse.Id == predictionEntity.Player.Id);
                        if (positionResponse == null)
                        {
                            positionResponses.Add(new PositionResponse
                            {
                                Points = predictionEntity.Points,
                                PlayerResponse = _converterHelper.ToPlayerResponse(predictionEntity.Player),
                            });
                        }
                        else
                        {
                            positionResponse.Points += predictionEntity.Points;
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
