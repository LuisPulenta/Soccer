using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using Soccer.Web.Models;

namespace Soccer.Web.Controllers
{
    public class LeaguesController : Controller
    {
        private readonly DataContext _context;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public LeaguesController(DataContext context,
            IImageHelper imageHelper,
            IConverterHelper converterHelper)
        {
            _context = context;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }

        // GET: Leagues
        public async Task<IActionResult> Index()
        {
            return View(await _context.Leagues
                 .Include(t => t.Teams)
                 .OrderBy(t => t.Name).ToListAsync());
        }

        // GET: Leagues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leagueEntity = await _context.Leagues
                .Include(t=>t.Teams)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leagueEntity == null)
            {
                return NotFound();
            }

            return View(leagueEntity);
        }

        // GET: Leagues/Create
        public IActionResult Create()
        {
            return View();
        }


        // POST: Leagues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeagueViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (model.LogoFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.LogoFile, "Leagues");
                }

                var league = _converterHelper.ToLeagueEntity(model, path, true);
                _context.Add(league);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Esta Liga ya existe");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    }
                }
            }

                return View(model);
        }



        // GET: Leagues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            LeagueEntity leagueEntity = await _context.Leagues.FindAsync(id);
            if (leagueEntity == null)
            {
                return NotFound();
            }

            LeagueViewModel model = _converterHelper.ToLeagueViewModel(leagueEntity);
            return View(model);
        }


        // POST: Leagues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LeagueViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (ModelState.IsValid)
                {
                    var path = model.LogoPath;

                    if (model.LogoFile != null)
                    {
                        path = await _imageHelper.UploadImageAsync(model.LogoFile, "Leagues");
                    }

                    LeagueEntity league = _converterHelper.ToLeagueEntity(model, path, false);
                    _context.Update(league);
                    try
                    {
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException.Message.Contains("duplicate"))
                        {
                            ModelState.AddModelError(string.Empty, "Esta Liga ya existe");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                        }
                    }
                }
                }
            return View(model);
        }




        // POST: Leagues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leagueEntity = await _context.Leagues
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leagueEntity == null)
            {
                return NotFound();
            }

            _context.Leagues.Remove(leagueEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CreateTeam(int? id)
        {
            if (id == null)

            {

                return NotFound();
            }

            var league = await _context.Leagues.FindAsync(id.Value);
            if (league == null)
            {
                return NotFound();
            }

            var model = new TeamViewModel
            {
                LeagueId = league.Id
                //Leagues = _combosHelper.GetComboLeagues()
            };


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam(TeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;
                model.Initials = model.Initials.ToUpper();
                model.League = await _context.Leagues
                .FirstOrDefaultAsync(p => p.Id == model.LeagueId);
                if (model.LogoFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.LogoFile,"Teams");
                }

                var team = _converterHelper.ToTeamEntity(model, path, true);
                _context.Teams.Add(team);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction($"Details/{model.Id}");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Este Equipo ya existe");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    }

                }
            }

            //model.Leagues = _combosHelper.GetComboLeagues();
            return View(model);
        }


        public async Task<IActionResult> EditTeam(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(p => p.League)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(_converterHelper.ToTeamViewModel(team));
        }

        [HttpPost]
        public async Task<IActionResult> EditTeam(TeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = model.LogoPath;
                model.Initials = model.Initials.ToUpper();
                model.League = await _context.Leagues
                .FirstOrDefaultAsync(p => p.Id == model.LeagueId);

                if (model.LogoFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.LogoFile, "Teams");
                }

                var team = _converterHelper.ToTeamEntity(model, path, false);
                _context.Teams.Update(team);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction($"Details/{model.League.Id}");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Este Equipo ya existe");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    }
                }
            }
            //model.Leagues = _combosHelper.GetComboLeagues();
            return View(model);
        }

        public async Task<IActionResult> DetailsTeam(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(p => p.League)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        public async Task<IActionResult> DeleteTeam(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(p => p.League)
                .FirstOrDefaultAsync(pi => pi.Id == id.Value);
            if (team == null)
            {
                return NotFound();
            }


            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return RedirectToAction($"{nameof(Details)}/{team.League.Id}");
        }
    }
}
