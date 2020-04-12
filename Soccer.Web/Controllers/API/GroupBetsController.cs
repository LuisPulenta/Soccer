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
    public class GroupBetsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;

        public GroupBetsController(DataContext context, IConverterHelper converterHelper, IUserHelper userHelper, IImageHelper imageHelper)
        {
            _context = context;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _imageHelper = imageHelper;
        }


        [HttpPost]
        public async Task<IActionResult> PostGroupBet([FromBody] GroupBetRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string picturePath = string.Empty;
            if (request.PictureArray != null && request.PictureArray.Length > 0)
            {
                picturePath = _imageHelper.UploadImage(request.PictureArray, "GroupBets");
            }



            var groupBet = new GroupBet
            {
                Admin = await _context.Players
                .Include(u => u.User)
                .FirstOrDefaultAsync(p => p.User.Email == request.PlayerEmail),
                CreationDate = request.CreationDate,
                Name = request.Name,
                Tournament = await _context.Tournaments.FindAsync(request.TournamentId),
                LogoPath = picturePath,
            };

            var groupBetPlayer = new GroupBetPlayer
            {

                GroupBet = groupBet,
                Player= await _context.Players
                .Include(u => u.User)
                .FirstOrDefaultAsync(p => p.User.Email == request.PlayerEmail),
                IsAccepted=true,
                IsBlocked=false,
                Points=0
            };


            _context.GroupBets.Add(groupBet);
            _context.GroupBetPlayers.Add(groupBetPlayer);
            await _context.SaveChangesAsync();
            return Ok(_converterHelper.ToGroupBetResponse(groupBet));
            //return NoContent();
        }

        [HttpPost]
        [Route("GetGroupBetsByEmail")]
        public async Task<IActionResult> GetGroupBets(EmailRequest emailRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var groupBetsPlayer = await _context.GroupBetPlayers
                .Include(p => p.Player)
                .ThenInclude(u => u.User)
                .ThenInclude(f => f.FavoriteTeam)
                .ThenInclude(l => l.League)
                .Include(g => g.GroupBet)
                .ThenInclude(t => t.Tournament)
                .Include(g => g.GroupBet)
                .ThenInclude(p => p.Admin)
                .ThenInclude(p => p.User)
                .ThenInclude(f => f.FavoriteTeam)
                .ThenInclude(l => l.League)
                .Include(g => g.GroupBet)
                .ThenInclude(p => p.GroupBetPlayers)
                .ThenInclude(p => p.Player)
                .ThenInclude(u => u.User)
                .ThenInclude(f => f.FavoriteTeam)
                .ThenInclude(l => l.League)

                .Where(o => o.Player.User.Email.ToLower() == emailRequest.Email.ToLower())
                .OrderBy(a => a.GroupBet.Name)
                .ToListAsync();

            var response = new List<GroupBetResponse>();
            foreach (var groupBetPlayer in groupBetsPlayer)
            {
                var group = new GroupBetResponse
                {
                    Id = groupBetPlayer.GroupBet.Id,
                    Admin = _converterHelper.ToPlayerResponse(groupBetPlayer.GroupBet.Admin),
                    CreationDate = groupBetPlayer.GroupBet.CreationDate,
                    LogoPath = groupBetPlayer.GroupBet.LogoPath,
                    Name = groupBetPlayer.GroupBet.Name,
                    Tournament = _converterHelper.ToTournamentResponse(await _context.Tournaments.FirstOrDefaultAsync(a => a.Id == groupBetPlayer.GroupBet.Tournament.Id)),

                    GroupBetPlayers = groupBetPlayer.GroupBet.GroupBetPlayers.Select(p => new GroupBetPlayerResponse
                    {
                        Id = p.Id,
                        IsAccepted = p.IsAccepted,
                        IsBlocked = p.IsBlocked,
                        Points = p.Points,
                        Player = _converterHelper.ToPlayerResponse(p.Player),
                    }).ToList()
                };

                response.Add(group);
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroupBet([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            var groupBet = await _context.GroupBets
                .Include(p => p.GroupBetPlayers)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (groupBet == null)
            {
                return this.NotFound();
            }

            

            _context.GroupBets.Remove(groupBet);
            await _context.SaveChangesAsync();
            return Ok("Grupo de apuestas borrado");
        }
    }
}