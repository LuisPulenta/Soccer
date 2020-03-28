using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using Soccer.Web.Models;

namespace Soccer.Web.Controllers
{
    [Authorize(Roles = "Manager")]
    public class GroupBetsController : Controller
    {
        private readonly DataContext _context;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public GroupBetsController(DataContext context,
            IImageHelper imageHelper,
            IConverterHelper converterHelper)
        {
            _context = context;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }

        // GET: GroupBets
        public async Task<IActionResult> Index()
        {
            return View(await _context.GroupBets
                 .Include(t => t.GroupBetPlayers)
                 .OrderBy(t => t.Name).ToListAsync());
        }

        // GET: GroupBets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var GroupBetEntity = await _context.GroupBets
                .Include(t => t.GroupBetPlayers)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (GroupBetEntity == null)
            {
                return NotFound();
            }

            return View(GroupBetEntity);
        }

        // GET: GroupBets/Create
        public IActionResult Create()
        {
            return View();
        }


        // POST: GroupBets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GroupBetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (model.LogoFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.LogoFile, "GroupBets");
                }

                var GroupBet = _converterHelper.ToGroupBet(model, path, true);
                _context.Add(GroupBet);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Este Grupo ya existe");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    }
                }
            }

            return View(model);
        }



        // GET: GroupBets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GroupBet GroupBetEntity = await _context.GroupBets.FindAsync(id);
            if (GroupBetEntity == null)
            {
                return NotFound();
            }

            GroupBetViewModel model = _converterHelper.ToGroupBetViewModel(GroupBetEntity);
            return View(model);
        }


        // POST: GroupBets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GroupBetViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (ModelState.IsValid)
                {
                    var path = model.LogoPath;

                    if (model.LogoFile != null)
                    {
                        path = await _imageHelper.UploadImageAsync(model.LogoFile, "GroupBets");
                    }

                    GroupBet GroupBet = _converterHelper.ToGroupBet(model, path, false);
                    _context.Update(GroupBet);
                    try
                    {
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException.Message.Contains("duplicate"))
                        {
                            ModelState.AddModelError(string.Empty, "Este Grupo ya existe");
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




        // POST: GroupBets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var GroupBetEntity = await _context.GroupBets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (GroupBetEntity == null)
            {
                return NotFound();
            }

            _context.GroupBets.Remove(GroupBetEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CreateGroupBetPlayer(int? id)
        {
            if (id == null)

            {

                return NotFound();
            }

            var GroupBet = await _context.GroupBets.FindAsync(id.Value);
            if (GroupBet == null)
            {
                return NotFound();
            }

            var model = new GroupBetPlayerViewModel
            {
                GroupBetId = GroupBet.Id
                //GroupBets = _combosHelper.GetComboGroupBets()
            };


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroupBetPlayer(GroupBetPlayerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;
                
                model.GroupBet = await _context.GroupBets
                .FirstOrDefaultAsync(p => p.Id == model.GroupBetId);
                

                //var GroupBetPlayer = _converterHelper.ToGroupBetPlayerEntity(model, path, true);
                //_context.GroupBetPlayers.Add(GroupBetPlayer);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction($"Details/{model.Id}");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Este Jugador ya existe");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    }

                }
            }

            //model.GroupBets = _combosHelper.GetComboGroupBets();
            return View(model);
        }


        //public async Task<IActionResult> EditGroupBetPlayer(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var GroupBetPlayer = await _context.GroupBetPlayers
        //        .Include(p => p.GroupBet)
        //        .FirstOrDefaultAsync(p => p.Id == id);
        //    if (GroupBetPlayer == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(_converterHelper.ToGroupBetPlayerViewModel(GroupBetPlayer));
        //}

        //[HttpPost]
        //public async Task<IActionResult> EditGroupBetPlayer(GroupBetPlayerViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var path = model.LogoPath;
        //        model.Initials = model.Initials.ToUpper();
        //        model.GroupBet = await _context.GroupBets
        //        .FirstOrDefaultAsync(p => p.Id == model.GroupBetId);

        //        if (model.LogoFile != null)
        //        {
        //            path = await _imageHelper.UploadImageAsync(model.LogoFile, "GroupBetPlayers");
        //        }

        //        var GroupBetPlayer = _converterHelper.ToGroupBetPlayerEntity(model, path, false);
        //        _context.GroupBetPlayers.Update(GroupBetPlayer);
        //        try
        //        {
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction($"Details/{model.GroupBet.Id}");
        //        }
        //        catch (Exception ex)
        //        {
        //            if (ex.InnerException.Message.Contains("duplicate"))
        //            {
        //                ModelState.AddModelError(string.Empty, "Este Equipo ya existe");
        //            }
        //            else
        //            {
        //                ModelState.AddModelError(string.Empty, ex.InnerException.Message);
        //            }
        //        }
        //    }
        //    //model.GroupBets = _combosHelper.GetComboGroupBets();
        //    return View(model);
        //}

        public async Task<IActionResult> DetailsGroupBetPlayer(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var GroupBetPlayer = await _context.GroupBetPlayers
                .Include(p => p.GroupBet)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (GroupBetPlayer == null)
            {
                return NotFound();
            }

            return View(GroupBetPlayer);
        }

        public async Task<IActionResult> DeleteGroupBetPlayer(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var GroupBetPlayer = await _context.GroupBetPlayers
                .Include(p => p.GroupBet)
                .FirstOrDefaultAsync(pi => pi.Id == id.Value);
            if (GroupBetPlayer == null)
            {
                return NotFound();
            }


            _context.GroupBetPlayers.Remove(GroupBetPlayer);
            await _context.SaveChangesAsync();
            return RedirectToAction($"{nameof(Details)}/{GroupBetPlayer.GroupBet.Id}");
        }
    }
}
