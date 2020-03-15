using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class PlayersController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;

        public PlayersController(
            DataContext context,
            IUserHelper userHelper,
            ICombosHelper combosHelper,
            IConverterHelper converterHelper,
            IImageHelper imageHelper,
            IMailHelper mailHelper
)
        {
            _dataContext = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
        }

        public async Task<JsonResult> GetTeamsAsync(int leagueId)
        {
            var teams = await _dataContext.Teams
                .Where(p => p.League.Id == leagueId)
                .OrderBy(p => p.Name)
                .ToListAsync();
            return Json(teams);
        }



        // GET: Players
        public IActionResult Index()
        {
            return View(_dataContext.Players
                .Include(o => o.User)
                 .ThenInclude(t => t.FavoriteTeam)
                );
        }


        // GET: Players/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _dataContext.Players
                .Include(o => o.User)
                .ThenInclude(t => t.FavoriteTeam)
                .FirstOrDefaultAsync(m => m.Id == id.Value);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }



        // GET: Players/Create
        public IActionResult Create()
        {
            var model = new AddUserViewModel
            {
                UserTypes = _combosHelper.GetComboUserTypes(),
                Leagues = _combosHelper.GetComboLeagues(),
                Teams = _combosHelper.GetComboTeams(0),
                Sexs = _combosHelper.GetComboSexs(),
            };
            return View(model);
        }

        // POST: Players/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.FavoriteTeamId = model.TeamId;
                var user = await AddUser(model);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Este email ya existe.");
                    model.Leagues = _combosHelper.GetComboLeagues();
                    model.Teams = _combosHelper.GetComboTeams(model.LeagueId);
                    model.Sexs = _combosHelper.GetComboSexs();
                    return View(model);
                }

                var player = new Player
                {
                    User = user,
                };

                _dataContext.Players.Add(player);
                await _dataContext.SaveChangesAsync();

                var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                var tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                _mailHelper.SendMail(model.Username, "Confirmación de E-Mail", $"<h1>Confirmación de E-Mail</h1>" +
                    $"Para habilitar el Usuario, " +
                    $"por favor haga clic en el siguiente link: </br></br><a href = \"{tokenLink}\">Confirmar E-mail</a>");



                return RedirectToAction(nameof(Index));
            }
            
            model.Leagues = _combosHelper.GetComboLeagues();
            model.Teams = _combosHelper.GetComboTeams(model.LeagueId);
            model.Sexs = _combosHelper.GetComboSexs();
            return View(model);
        }


        private async Task<User> AddUser(AddUserViewModel model)
        {
            var path = string.Empty;

            if (model.ImageFile != null)
            {
                path = await _imageHelper.UploadImageAsync(model.ImageFile,"Users");
            }

            var user = new User
            {
                Document = model.Document,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                BornDate = model.BornDate,
                Sex = model.Sex,
                Picture = path,
                NickName = model.NickName,
                FavoriteTeam = await _dataContext.Teams.FindAsync(model.FavoriteTeamId),
                Points = model.Points,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Email = model.Username,
                UserName = model.Username,
            };

            var result = await _userHelper.AddUserAsync(user, model.Password);
            if (result != IdentityResult.Success)
            {
                return null;
            }
            var newUser = await _userHelper.GetUserAsync(model.Username);
            await _userHelper.AddUserToRoleAsync(newUser, "Player");
            return newUser;
        }


        // GET: Players/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _dataContext.Players
                .Include(o => o.User)
                .ThenInclude(t => t.FavoriteTeam)
                .ThenInclude(l => l.League)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (player == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Id = player.Id,
                Document = player.User.Document,
                FirstName = player.User.FirstName,
                LastName = player.User.LastName,
                Address = player.User.Address,
                PhoneNumber = player.User.PhoneNumber,
                Picture = player.User.Picture,
                BornDate = player.User.BornDate,
                Sex = player.User.Sex,
                
                NickName = player.User.NickName,
                FavoriteTeamId = player.User.FavoriteTeam.Id,
                LeagueId = player.User.FavoriteTeam.League.Id,
                TeamId = player.User.FavoriteTeam.Id,
                SexId = player.User.Sex,
                Points = player.User.Points,
                Latitude = player.User.Latitude,
                Longitude = player.User.Longitude,
                Leagues = _combosHelper.GetComboLeagues(),
                Teams = _combosHelper.GetComboTeams(player.User.FavoriteTeam.League.Id),
                Sexs = _combosHelper.GetComboSexs(),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.FavoriteTeamId = model.TeamId;

                var path = model.Picture;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";

                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\Users",
                        file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/Users/{file}";
                }


                var player = await _dataContext.Players
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == model.Id);

                player.User.Document = model.Document;
                player.User.FirstName = model.FirstName;
                player.User.LastName = model.LastName;
                player.User.Address = model.Address;
                player.User.PhoneNumber = model.PhoneNumber;
                player.User.BornDate = model.BornDate;
                player.User.Picture = path;
                player.User.Sex = model.Sex;
                player.User.NickName = model.NickName;
                player.User.FavoriteTeam = await _dataContext.Teams.FindAsync(model.FavoriteTeamId);
                player.User.Points = model.Points;
                player.User.Latitude = model.Latitude;
                player.User.Longitude = model.Longitude;

                await _userHelper.UpdateUserAsync(player.User);
                return RedirectToAction(nameof(Index));
            }
            model.Leagues = _combosHelper.GetComboLeagues();
            model.Teams = _combosHelper.GetComboTeams(0);
            model.Sexs = _combosHelper.GetComboSexs();
            return View(model);
        }


        // Owners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _dataContext.Players
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (player == null)
            {
                return NotFound();
            }

            await _userHelper.DeleteUserAsync(player.User.Email);
            _dataContext.Players.Remove(player);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction($"{nameof(Index)}");
        }



        private bool PlayersExists(int id)
        {
            return _dataContext.Players.Any(e => e.Id == id);
        }

    }
}
