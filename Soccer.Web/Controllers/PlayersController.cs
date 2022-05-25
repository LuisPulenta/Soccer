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

                _mailHelper.SendMail("Soporte Soccer", user.Email, "Confirmación de Email",
                 $"<table style = 'max-width: 800px; padding: 10px; margin:0 auto; border-collapse: collapse;'>" +
                 $"  <tr>" +
                 $"    <td style = 'background-color: #3658a8; text-align: center; padding: 0'>" +
                                    $"  <td style = 'padding: 0'>" +
                 $"<tr>" +
                 $" <td style = 'background-color: #ecf0f1'>" +
                 $"      <div style = 'color: #3658a8; margin: 4% 10% 2%; text-align: justify;font-family: sans-serif'>" +
                 $"            <h1 style = 'color: #e67e22; margin: 0 0 7px' > Soccer </h1>" +
                 $"                    <p style = 'margin: 2px; font-size: 15px'>" +
                 $"                      Para completar el registro de su Usuario usted debe <br>" +
                 $"      <ul style = 'font-size: 15px;  margin: 10px 0'>" +
                 $"        <li> confirmar la dirección de Email haciendo clic en el botón del final de este mail.</li>" +

                 $"  <div style = 'width: 100%;margin:5px 0; display: inline-block;text-align: center'>" +
                 $"  </div>" +
                 $"  <div style = 'width: 100%; text-align: center'>" +
                 $"    <h2 style = 'color: #e67e22; margin: 0 0 5px' >Confirmación de Email</h2>" +
                 $"    Para habilitar el usuario, por favor hacer clic en el siguiente enlace: </ br ></ br > " +
                 $"    <a style ='text-decoration: none; border-radius: 5px; padding: 5px 5px; color: white; background-color: #3658a8' href = \"{tokenLink}\">Confirmar Email</a>" +
                 $"    <p style = 'color: #b3b3b3; font-size: 12px; text-align: center;margin: 10px 0 0' > Soccer</p>" +
                 $"  </div>" +
                 $" </td >" +
                 $"</tr>" +
                 $"</table>");



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
                FirstName = model.FirstName,
                LastName = model.LastName,
                Picture = path,
                NickName = model.NickName,
                FavoriteTeam = await _dataContext.Teams.FindAsync(model.FavoriteTeamId),
                Points = model.Points,
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
                FirstName = player.User.FirstName,
                LastName = player.User.LastName,
                PhoneNumber = player.User.PhoneNumber,
                Picture = player.User.Picture,
                NickName = player.User.NickName,
                FavoriteTeamId = player.User.FavoriteTeam.Id,
                LeagueId = player.User.FavoriteTeam.League.Id,
                TeamId = player.User.FavoriteTeam.Id,
                Points = player.User.Points,
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

                player.User.FirstName = model.FirstName;
                player.User.LastName = model.LastName;
                player.User.PhoneNumber = model.PhoneNumber;
                player.User.Picture = path;
                player.User.NickName = model.NickName;
                player.User.FavoriteTeam = await _dataContext.Teams.FindAsync(model.FavoriteTeamId);
                player.User.Points = model.Points;

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
