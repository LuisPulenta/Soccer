using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using Soccer.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Controllers
{
    [Authorize(Roles = "Manager")]
    public class TournamentsController : Controller
    {
        private readonly DataContext _context;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IMatchHelper _matchHelper;

        public TournamentsController(DataContext context,
                                     IConverterHelper converterHelper,
                                     IImageHelper imageHelper,
                                     ICombosHelper combosHelper,
                                     IMatchHelper matchHelper)
        {
            _context = context;
            _imageHelper = imageHelper;
            _combosHelper = combosHelper;
            _matchHelper = matchHelper;
            _converterHelper = converterHelper;
        }

        public async Task<JsonResult> GetTeamsAsync(int leagueId)
        {
            var teams = await _context.Teams
                .Where(p => p.League.Id == leagueId)
                .OrderBy(p => p.Name)
                .ToListAsync();
            return Json(teams);
        }

        

        public async Task<JsonResult> GetTeamsAsync2(int groupDetailId)
        {

            GroupEntity Group = await _context.Groups.FindAsync(groupDetailId);

            var teams = _context.GroupDetails
            .Include(gd => gd.Team)
            .Where(gd => gd.Group.Id == groupDetailId);


            var response = new List<TeamEntity>();
            foreach (var te in teams)
            {
                var team = new TeamEntity
                {
                    Id = te.Team.Id,
                    Name = te.Team.Name
                };
                response.Add(team);
            }
            return Json(response);
        }







        public async Task<IActionResult> Index()
        {
            return View(await _context
                .Tournaments
                .Include(t => t.Groups)
                .Include(d=>d.DateNames)
                .OrderBy(t => t.StartDate)
                .ToListAsync());
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TournamentViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = string.Empty;
                model.Groups = new List<GroupEntity>
                { };
                model.DateNames = new List<DateNameEntity>
                { };


                if (model.LogoFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.LogoFile, "Tournaments");
                }
                model.LogoPath = path;
                var tournament = _converterHelper.ToTournamentEntity(model, path, true);
                _context.Add(tournament);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TournamentEntity tournamentEntity = await _context.Tournaments.FindAsync(id);
            if (tournamentEntity == null)
            {
                return NotFound();
            }

            TournamentViewModel model = _converterHelper.ToTournamentViewModel(tournamentEntity);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TournamentViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = model.LogoPath;

                if (model.LogoFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.LogoFile, "Tournaments");
                }

                TournamentEntity tournamentEntity = _converterHelper.ToTournamentEntity(model, path, false);
                _context.Update(tournamentEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction($"{nameof(Details)}/{model.Id}");

            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentEntity = await _context.Tournaments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tournamentEntity == null)
            {
                return NotFound();
            }

            _context.Tournaments.Remove(tournamentEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentEntity = await _context.Tournaments
                .Include(d => d.DateNames)
                .ThenInclude(t => t.Matches)
                .ThenInclude(t => t.Local)
                .Include(d => d.DateNames)
                .ThenInclude(t => t.Matches)
                .ThenInclude(t => t.Visitor)
                .Include(t => t.Groups)
                .ThenInclude(t => t.GroupDetails)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tournamentEntity == null)
            {
                return NotFound();
            }

            return View(tournamentEntity);
        }

        public async Task<IActionResult> AddGroup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentEntity = await _context.Tournaments.FindAsync(id);
            if (tournamentEntity == null)
            {
                return NotFound();
            }

            var model = new GroupViewModel
            {
                Tournament = tournamentEntity,
                TournamentId = tournamentEntity.Id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGroup(GroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var groupEntity = await _converterHelper.ToGroupEntityAsync(model, true);
                _context.Add(groupEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction($"{nameof(Details)}/{model.TournamentId}");
            }

            return View(model);
        }


        public async Task<IActionResult> EditGroup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupEntity = await _context.Groups
                .Include(g => g.Tournament)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (groupEntity == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToGroupViewModel(groupEntity);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGroup(GroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var groupEntity = await _converterHelper.ToGroupEntityAsync(model, false);
                _context.Update(groupEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction($"{nameof(Details)}/{model.TournamentId}");
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteGroup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupEntity = await _context.Groups
                .Include(g => g.Tournament)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (groupEntity == null)
            {
                return NotFound();
            }

            _context.Groups.Remove(groupEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction($"{nameof(Details)}/{groupEntity.Tournament.Id}");
        }

        public async Task<IActionResult> DetailsGroup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupEntity = await _context.Groups
                .Include(g => g.Tournament)
                .ThenInclude(d => d.DateNames)
                .ThenInclude(m => m.Matches)
                .ThenInclude(l => l.Local)
                .Include(g => g.Tournament)
                .ThenInclude(d => d.DateNames)
                .ThenInclude(m => m.Matches)
                .ThenInclude(l => l.Visitor)
                .Include(g => g.GroupDetails)
                .ThenInclude(gd => gd.Team)
                .Include(m=>m.Matches)
                .ThenInclude(l => l.Local)
                 .Include(m => m.Matches)
                 .ThenInclude(l => l.Visitor)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (groupEntity == null)
            {
                return NotFound();
            }

            return View(groupEntity);
        }

        public async Task<IActionResult> DetailsDateName(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dateNameEntity = await _context.DateNames
                .Include(g => g.Tournament)
                .Include(m => m.Matches)
                .ThenInclude(l => l.Local)
                .Include(m => m.Matches)
                .ThenInclude(l => l.Visitor)
                .Include(m => m.Matches)
                .ThenInclude(l => l.Group)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (dateNameEntity == null)
            {
                return NotFound();
            }

            return View(dateNameEntity);
        }





        public async Task<IActionResult> AddGroupDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupEntity = await _context.Groups.FindAsync(id);
            if (groupEntity == null)
            {
                return NotFound();
            }

            var model = new GroupDetailViewModel
            {
                Group = groupEntity,
                GroupId = groupEntity.Id,
                Leagues = _combosHelper.GetComboLeagues(),
                Teams = _combosHelper.GetComboTeams(0)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGroupDetail(GroupDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var groupDetailEntity = await _converterHelper.ToGroupDetailEntityAsync(model, true);
                _context.Add(groupDetailEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsGroup)}/{model.GroupId}");
            }
            model.Group = await _context.Groups.FindAsync(model.GroupId);
            model.Leagues= _combosHelper.GetComboLeagues();
            model.Teams = _combosHelper.GetComboTeams(model.LeagueId);
            return View(model);
        }

        public async Task<IActionResult> AddMatch(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupEntity = await _context.Groups
            .Include(gd => gd.Tournament)
            .FirstOrDefaultAsync(gd => gd.Id == id);

            if (groupEntity == null)
            {
                return NotFound();
            }

            var model = new MatchViewModel
            {
                Date = DateTime.Today,
                Group = groupEntity,
                GroupId = groupEntity.Id,
                Teams = _combosHelper.GetComboTeams2(groupEntity.Id),
                DateNames= _combosHelper.GetComboDateNames(groupEntity.Tournament.Id)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMatch(MatchViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.DateName= await _context.DateNames.FirstOrDefaultAsync(gd => gd.Id == model.DateNameId);
                if (model.LocalId != model.VisitorId)
                {
                    var matchEntity = await _converterHelper.ToMatchEntityAsync(model, true);
                    _context.Add(matchEntity);
                    await _context.SaveChangesAsync();
                    return RedirectToAction($"{nameof(DetailsGroup)}/{model.GroupId}");
                }

                ModelState.AddModelError(string.Empty, "El local y el visitante deben ser equipos diferentes.");
            }

            model.Group = await _context.Groups.FindAsync(model.GroupId);
            model.Teams = _combosHelper.GetComboTeams(model.GroupId);
            model.DateNames = _combosHelper.GetComboDateNames(model.Group.Tournament.Id);
            return View(model);
        }

        public async Task<IActionResult> AddMatch2(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var dateNameEntity = await _context.DateNames
            .Include(gd => gd.Tournament)
            .FirstOrDefaultAsync(gd => gd.Id == id);

            if (dateNameEntity == null)
            {
                return NotFound();
            }

            var model = new MatchViewModel2
            {
                DateNameId= dateNameEntity.Id,
                Date = DateTime.Today,
                DateName=dateNameEntity,
                Groups = _combosHelper.GetComboGroups(dateNameEntity.Tournament.Id),
                Teams = _combosHelper.GetComboTeams2(0)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMatch2(MatchViewModel2 model)
        {
            if (ModelState.IsValid)
            {
                model.Group = await _context.Groups
                    .Include(t=>t.Tournament)
                    .FirstOrDefaultAsync(gd => gd.Id == model.GroupId);
                model.DateName = await _context.DateNames.FirstOrDefaultAsync(gd => gd.Id == model.DateNameId);
                model.Local = await _context.Teams.FirstOrDefaultAsync(gd => gd.Id == model.LocalId);
                model.Visitor = await _context.Teams.FirstOrDefaultAsync(gd => gd.Id == model.VisitorId);
                    if (model.LocalId != model.VisitorId)
                    {
                        var matchEntity = await _converterHelper.ToMatchEntityAsync2(model, true);
                        _context.Add(matchEntity);
                        await _context.SaveChangesAsync();
                        return RedirectToAction($"{nameof(DetailsDateName)}/{model.DateNameId}");
                    }

                    ModelState.AddModelError(string.Empty, "El local y el visitante deben ser equipos diferentes.");
            }

            //model.Group = await _context.Groups.FindAsync(model.GroupId);
            model.Teams = _combosHelper.GetComboTeams2(model.GroupId);
            model.Groups = _combosHelper.GetComboGroups(model.Group.Tournament.Id);
            return View(model);
        }

        public async Task<IActionResult> EditGroupDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupDetailEntity = await _context.GroupDetails
                .Include(gd => gd.Group)
                .ThenInclude(t=>t.Tournament)
                .Include(gd => gd.Team)
                .ThenInclude(l=>l.League)
                .FirstOrDefaultAsync(gd => gd.Id == id);
            if (groupDetailEntity == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToGroupDetailViewModel(groupDetailEntity);
            model.Teams= _combosHelper.GetComboTeams(model.LeagueId);
            return View(model);
        }

[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGroupDetail(GroupDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var groupDetailEntity = await _converterHelper.ToGroupDetailEntityAsync(model, false);
                _context.Update(groupDetailEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsGroup)}/{model.GroupId}");
            }

            return View(model);
        }

        public async Task<IActionResult> EditMatch(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matchEntity = await _context.Matches
                .Include(m => m.Group)
                .Include(m => m.Local)
                .Include(m => m.Visitor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (matchEntity == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToMatchViewModel(matchEntity);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMatch(MatchViewModel model)
        {
            if (ModelState.IsValid)
            {
                var matchEntity = await _converterHelper.ToMatchEntityAsync(model, false);
                _context.Update(matchEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsGroup)}/{model.GroupId}");
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteGroupDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupDetailEntity = await _context.GroupDetails
                .Include(gd => gd.Group)
                .FirstOrDefaultAsync(gd => gd.Id == id);
            if (groupDetailEntity == null)
            {
                return NotFound();
            }

            _context.GroupDetails.Remove(groupDetailEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction($"{nameof(DetailsGroup)}/{groupDetailEntity.Group.Id}");
        }

        public async Task<IActionResult> DeleteMatch(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matchEntity = await _context.Matches
                .Include(m => m.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (matchEntity == null)
            {
                return NotFound();
            }

            _context.Matches.Remove(matchEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction($"{nameof(DetailsGroup)}/{matchEntity.Group.Id}");
        }

        public async Task<IActionResult> DeleteMatch2(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matchEntity = await _context.Matches
                .Include(m => m.Group)
                .Include(d=>d.DateName)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (matchEntity == null)
            {
                return NotFound();
            }

            _context.Matches.Remove(matchEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction($"{nameof(DetailsDateName)}/{matchEntity.DateName.Id}");
        }

        public async Task<IActionResult> AddDateName(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentEntity = await _context.Tournaments.FindAsync(id);
            if (tournamentEntity == null)
            {
                return NotFound();
            }

            var model = new DateNameViewModel
            {
                Tournament = tournamentEntity,
                TournamentId = tournamentEntity.Id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDateName(DateNameViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dateNameEntity = await _converterHelper.ToDateNameEntityAsync(model, true);
                _context.Add(dateNameEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction($"{nameof(Details)}/{model.TournamentId}");
            }

            return View(model);
        }

        public async Task<IActionResult> EditDateName(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dateNameEntity = await _context.DateNames
                .Include(g => g.Tournament)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (dateNameEntity == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToDateNameViewModel(dateNameEntity);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDateName(DateNameViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dateNameEntity = await _converterHelper.ToDateNameEntityAsync(model, false);
                _context.Update(dateNameEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction($"{nameof(Details)}/{model.TournamentId}");
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteDateName(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dateNameEntity = await _context.DateNames
                .Include(g => g.Tournament)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dateNameEntity == null)
            {
                return NotFound();
            }

            _context.DateNames.Remove(dateNameEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction($"{nameof(Details)}/{dateNameEntity.Tournament.Id}");
        }

        public async Task<IActionResult> EditMatchDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matchEntity = await _context.Matches
                .Include(m => m.Group)
                .ThenInclude(t => t.Tournament)
                .Include(m => m.Local)
                .Include(m => m.Visitor)
                .Include(m => m.DateName)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (matchEntity == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToMatchViewModel(matchEntity);
            model.Teams = _combosHelper.GetComboTeams2(model.GroupId);
            model.DateNameId = model.DateName.Id;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMatchDetail(MatchViewModel model)
        {
            model.DateName = await _context.DateNames.FirstOrDefaultAsync(gd => gd.Id == model.DateNameId);
            model.Local = await _context.Teams.FirstOrDefaultAsync(gd => gd.Id == model.LocalId);
            model.Visitor = await _context.Teams.FirstOrDefaultAsync(gd => gd.Id == model.VisitorId);
            model.Group = await _context.Groups
                .Include(t => t.Tournament)
                .FirstOrDefaultAsync(gd => gd.Id == model.GroupId);
            var matchEntity = await _converterHelper.ToMatchEntityAsync(model, false);

            if (model.LocalId != model.VisitorId)
            {
                _context.Update(matchEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsGroup)}/{model.GroupId}");
            }

            ModelState.AddModelError(string.Empty, "El local y el visitante deben ser equipos diferentes.");
            model.Teams = _combosHelper.GetComboTeams2(model.GroupId);
            
            return View(model);
        }







        public async Task<IActionResult> EditMatchDetail2(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matchEntity = await _context.Matches
                .Include(m => m.Group)
                .ThenInclude(t=>t.Tournament)
                .Include(m => m.Local)
                .Include(m => m.Visitor)
                .Include(m => m.DateName)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (matchEntity == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToMatchViewModel2(matchEntity);
            model.Teams = _combosHelper.GetComboTeams2(model.GroupId);
            model.DateNameId = model.DateName.Id;
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMatchDetail2(MatchViewModel2 model)
        {
            model.DateName = await _context.DateNames.FirstOrDefaultAsync(gd => gd.Id == model.DateNameId);
            model.Local = await _context.Teams.FirstOrDefaultAsync(gd => gd.Id == model.LocalId);
            model.Visitor = await _context.Teams.FirstOrDefaultAsync(gd => gd.Id == model.VisitorId);
            model.Group = await _context.Groups
                .Include(t => t.Tournament)
                .FirstOrDefaultAsync(gd => gd.Id == model.GroupId);
            var matchEntity = await _converterHelper.ToMatchEntityAsync2(model, false);

            if (model.LocalId != model.VisitorId)
            {
                _context.Update(matchEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsDateName)}/{model.DateNameId}");
            }

            ModelState.AddModelError(string.Empty, "El local y el visitante deben ser equipos diferentes.");
            model.Teams = _combosHelper.GetComboTeams2(model.GroupId);
            model.Groups = _combosHelper.GetComboGroups(model.Group.Tournament.Id);
            return View(model);
        }

        public async Task<IActionResult> CloseMatch(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matchEntity = await _context.Matches
                .Include(m => m.Group)
                .Include(m => m.Local)
                .Include(m => m.Visitor)
                .Include(d => d.DateName)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (matchEntity == null)
            {
                return NotFound();
            }

            var model = new CloseMatchViewModel
            {
                Group = matchEntity.Group,
                GroupId = matchEntity.Group.Id,
                Local = matchEntity.Local,
                LocalId = matchEntity.Local.Id,
                MatchId = matchEntity.Id,
                Visitor = matchEntity.Visitor,
                VisitorId = matchEntity.Visitor.Id,
                DateName= matchEntity.DateName,
                DateNameId = matchEntity.DateName.Id

            };

            return View(model);
        }

        public async Task<IActionResult> CloseMatch2(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matchEntity = await _context.Matches
                .Include(m => m.Group)
                .Include(m => m.Local)
                .Include(m => m.Visitor)
                .Include(d => d.DateName)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (matchEntity == null)
            {
                return NotFound();
            }

            var model = new CloseMatchViewModel
            {
                Group = matchEntity.Group,
                GroupId = matchEntity.Group.Id,
                Local = matchEntity.Local,
                LocalId = matchEntity.Local.Id,
                MatchId = matchEntity.Id,
                Visitor = matchEntity.Visitor,
                VisitorId = matchEntity.Visitor.Id,
                DateName = matchEntity.DateName,
                DateNameId = matchEntity.DateName.Id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseMatch(CloseMatchViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _matchHelper.CloseMatchAsync(model.MatchId, model.GoalsLocal.Value, model.GoalsVisitor.Value);
                return RedirectToAction($"{nameof(DetailsGroup)}/{model.GroupId}");
            }

            model.Group = await _context.Groups.FindAsync(model.GroupId);
            model.Local = await _context.Teams.FindAsync(model.LocalId);
            model.Visitor = await _context.Teams.FindAsync(model.VisitorId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseMatch2(CloseMatchViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _matchHelper.CloseMatchAsync(model.MatchId, model.GoalsLocal.Value, model.GoalsVisitor.Value);
                return RedirectToAction($"{nameof(DetailsDateName)}/{model.DateNameId}");
            }

            model.DateName = await _context.DateNames.FindAsync(model.DateNameId);
            model.Local = await _context.Teams.FindAsync(model.LocalId);
            model.Visitor = await _context.Teams.FindAsync(model.VisitorId);
            return View(model);
        }

    }
}