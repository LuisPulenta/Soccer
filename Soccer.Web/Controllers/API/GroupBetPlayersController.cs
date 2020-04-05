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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Controllers.API
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupBetPlayersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;

        public GroupBetPlayersController(DataContext context, IConverterHelper converterHelper, IUserHelper userHelper, IImageHelper imageHelper)
        {
            _context = context;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _imageHelper = imageHelper;
        }


        [HttpPost]
        public async Task<IActionResult> PostGroupBetPlayer([FromBody] GroupBetPlayerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var groupBetPlayer = new GroupBetPlayer
            {
                Player= await _context.Players
                .Include(u => u.User)
                .FirstOrDefaultAsync(p => p.Id == request.PlayerId),
                IsAccepted=request.IsAccepted,
                IsBlocked=request.IsBlocked,
                Points=0,
                GroupBet= await _context.GroupBets
                .FirstOrDefaultAsync(p => p.Id == request.GroupBetId),


            };

            _context.GroupBetPlayers.Add(groupBetPlayer);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        


        [HttpPost]
        [Route("GetPlayersForGroupBet")]
        public async Task<IActionResult> GetPlayersForGroupBet([FromBody] PlayersForGroupBetRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            GroupBet groupBet = await _context.GroupBets.FindAsync(request.GroupBetId);
            if (groupBet == null)
            {
                return BadRequest("Este Grupo de Apuestas no existe.");
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
                .FirstOrDefaultAsync(u => u.User.Id == request.ToString());
            if (player == null)
            {
                return BadRequest("Este Usurio no existe.");
            }

            // Add precitions already done
            List<PredictionResponse> predictionResponses = new List<PredictionResponse>();
            //foreach (PredictionEntity predictionEntity in player.Predictions)
            //{
            //    if (predictionEntity.Match.Group.Tournament.Id == request.TournamentId)
            //    {
            //        predictionResponses.Add(_converterHelper.ToPredictionResponse(predictionEntity));
            //    }
            //}

            //// Add precitions undone
            //List<MatchEntity> matches = await _context.Matches
            //    .Include(m => m.Local)
            //    .Include(m => m.Visitor)
            //    .Where(m => m.Group.Tournament.Id == request.TournamentId)
            //    .ToListAsync();
            //foreach (MatchEntity matchEntity in matches)
            //{
            //    PredictionResponse predictionResponse = predictionResponses.FirstOrDefault(pr => pr.Match.Id == matchEntity.Id);
            //    if (predictionResponse == null)
            //    {
            //        predictionResponses.Add(new PredictionResponse
            //        {
            //            Match = _converterHelper.ToMatchResponse(matchEntity),
            //        });
            //    }
            //}

            return Ok(predictionResponses.OrderBy(pr => pr.Id).ThenBy(pr => pr.Match.Date));
        }


    }
}