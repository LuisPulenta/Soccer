using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Common.Enum;
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
        private readonly IMailHelper _mailHelper;

        public GroupBetsController(DataContext context, IConverterHelper converterHelper, IUserHelper userHelper, IImageHelper imageHelper, IMailHelper mailHelper)
        {
            _context = context;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
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

                .Include(g => g.GroupBet)
                .ThenInclude(p => p.GroupBetPlayers)
                .ThenInclude(p => p.Player)
                .ThenInclude(p => p.Predictions)
                .ThenInclude(p => p.Match)
                .ThenInclude(p => p.Local)
                .ThenInclude(p => p.League)

                .Include(g => g.GroupBet)
                .ThenInclude(p => p.GroupBetPlayers)
                .ThenInclude(p => p.Player)
                .ThenInclude(p => p.Predictions)
                .ThenInclude(p => p.Match)
                .ThenInclude(p => p.Visitor)
                .ThenInclude(p => p.League)

                .Include(g => g.GroupBet)
                .ThenInclude(p => p.GroupBetPlayers)
                .ThenInclude(p => p.Player)
                .ThenInclude(p => p.Predictions)
                .ThenInclude(p => p.Match)
                .ThenInclude(p => p.Group)
                .ThenInclude(p => p.Tournament)

                .Where(o => o.Player.User.Email.ToLower() == emailRequest.Email.ToLower() && o.GroupBet.Tournament.IsActive)
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
                        Player = new PlayerResponse2
                        {
                            FirstName = p.Player.User.FirstName,
                            LastName = p.Player.User.LastName,
                            NickName = p.Player.User.NickName,
                            PicturePath = p.Player.User.Picture,
                            Id = p.Player.Id,
                            Points = p.Player.User.Points,
                            Team = new TeamResponse
                            {
                                Id = p.Player.User.FavoriteTeam.Id,
                                Initials = p.Player.User.FavoriteTeam.Initials,
                                LeagueId = p.Player.User.FavoriteTeam.League.Id,
                                LeagueName = p.Player.User.FavoriteTeam.League.Name,
                                Name = p.Player.User.FavoriteTeam.Name,
                                LogoPath = p.Player.User.FavoriteTeam.LogoPath,
                            },
                            UserId = p.Player.User.Id,
                            Predictions = p.Player.Predictions.Select(h => new PredictionResponse2
                            {
                                Id = h.Id,
                                GoalsLocal = h.GoalsLocal,
                                GoalsVisitor = h.GoalsVisitor,
                                Points = h.Points,
                                MatchId = h.Match.Id,
                                PlayerId = h.Player.Id,
                                TournamentId=h.Match.Group.Tournament.Id,
                                NameLocal = h.Match.Local.Name,
                                NameVisitor = h.Match.Visitor.Name
                            }).ToList()
                        },

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


        [HttpPost]
        [Route("Invitar")]
        public async Task<IActionResult> PostUserGroupBet([FromBody] AddUserGroupBetRequest request)
        {
            if (!ModelState.IsValid)
            {                return BadRequest(ModelState);
            }

            Player proposalPlayer = await _context.Players
                .Include(u=>u.User)
                .FirstOrDefaultAsync(p => p.Id == request.PlayerId);

            if (proposalPlayer.User == null)
            {
                return BadRequest("Este Usuario no existe.");
            }

            Player requiredPlayer2 = _converterHelper.ToPlayer(await _userHelper.GetUserAsync(request.Email));


            if (requiredPlayer2.User == null)
            {
                return BadRequest("Este Usuario no existe en la App.");
            }

            Player requiredPlayer = await _context.Players
                .Include(u => u.User)
                .FirstOrDefaultAsync(p => p.User.Id == requiredPlayer2.User.Id);

            GroupBet groupBet = await _context.GroupBets
                .Include(u => u.GroupBetPlayers)
                .FirstOrDefaultAsync(p => p.Id == request.GroupBetId);


           


                GroupBetPlayer groupBetPlayer = await _context.GroupBetPlayers
                .Include(ug => ug.Player)
                .ThenInclude(u => u.User)
                .FirstOrDefaultAsync(ug => ug.Player.Id == requiredPlayer.Id && ug.GroupBet.Id == request.GroupBetId);
            if (groupBetPlayer != null)
            {
                {
                    return BadRequest("Este Usuario ya pertenece al Grupo.");
                }
            }

            PlayerGroupBetRequestEntity playerGroupBetRequestEntity = await _context.PlayerGroupBetRequests
                .FirstOrDefaultAsync(ug => ug.RequiredPlayer.Id == requiredPlayer.Id && ug.GroupBet.Id == request.GroupBetId && ug.Status== PlayerGroupBetStatus.Pending);
            
            if (playerGroupBetRequestEntity != null)

            {
                {
                    return BadRequest("Este Usuario ya tiene una invitación que está pendiente.");
                }
            }

            PlayerGroupBetRequestEntity playerGroupBetRequest = new PlayerGroupBetRequestEntity
            {
                ProposalPlayer = proposalPlayer,
                RequiredPlayer = requiredPlayer,
                GroupBet= groupBet,
                Status = PlayerGroupBetStatus.Pending,
                Token = Guid.NewGuid()
            };

            try
            {
                _context.PlayerGroupBetRequests.Add(playerGroupBetRequest);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            string linkConfirm = Url.Action("ConfirmUserGroup", "Account", new
            {
                requestId = playerGroupBetRequest.Id,
                token = playerGroupBetRequest.Token
            }, protocol: HttpContext.Request.Scheme);

            string linkReject = Url.Action("RejectUserGroup", "Account", new
            {
                requestId = playerGroupBetRequest.Id,
                token = playerGroupBetRequest.Token
            }, protocol: HttpContext.Request.Scheme);

            Response response = _mailHelper.SendMail(request.Email, "Solicitud de unirse a un Grupo", $"<h1>Solicitud de unirse a un Grupo</h1>" +
                $"El Usuario: {proposalPlayer.User.FullName} ({proposalPlayer.User.Email}), ha solicitado que sea miembro de su grupo de usuarios {groupBet.Name} en la aplicación FULBO PULENTA. " +
                $"</hr></br></br>Si desea aceptar, haga clic aquí: <a href = \"{linkConfirm}\">Confirmar</a>" +
                $"</hr></br></br> . Si desea rechazar, haga clic aquí: <a href = \"{linkReject}\">Rechazar</a>");

            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }

            return Ok("Se ha enviado un correo electrónico al usuario con su solicitud, esperamos a que responda pronto!");
        }



    }
}