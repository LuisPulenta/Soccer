using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using Soccer.Web.Models;

namespace Soccer.Web.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagersController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ICombosHelper _combosHelper;
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;

        public ManagersController(
            DataContext dataContext,
            ICombosHelper combosHelper,
            IUserHelper userHelper,
            IImageHelper imageHelper,
            IMailHelper mailHelper)
        {
            _dataContext = dataContext;
            _combosHelper = combosHelper;
            _userHelper = userHelper;
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



        public IActionResult Index()
        {
            return View(_dataContext.Managers.Include(m => m.User));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manager = await _dataContext.Managers
                .Include(o => o.User)
                .ThenInclude(t => t.FavoriteTeam)
                .FirstOrDefaultAsync(m => m.Id == id.Value);
            if (manager == null)
            {
                return NotFound();
            }

            return View(manager);
        }

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

                var manager = new Manager
                {
                    User = user,
                };

                _dataContext.Managers.Add(manager);
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

        private async Task<User> AddUser(AddUserViewModel view)
        {
            var path = string.Empty;

            if (view.ImageFile != null)
            {
                path = await _imageHelper.UploadImageAsync(view.ImageFile, "Users");
            }
            var user = new User
            {
                Document = view.Document,
                FirstName = view.FirstName,
                LastName = view.LastName,
                Address = view.Address,
                PhoneNumber = view.PhoneNumber,
                BornDate = view.BornDate,
                Sex = view.Sex,
                Picture = path,
                NickName = view.NickName,
                FavoriteTeam = await _dataContext.Teams.FindAsync(view.FavoriteTeamId),
                Points = view.Points,
                Latitude = view.Latitude,
                Longitude = view.Longitude,
                Email = view.Username,
                UserName = view.Username,
            };

            var result = await _userHelper.AddUserAsync(user, view.Password);
            if (result != IdentityResult.Success)
            {
                return null;
            }

            var newUser = await _userHelper.GetUserAsync(view.Username);
            await _userHelper.AddUserToRoleAsync(newUser, "Manager");
            await _dataContext.SaveChangesAsync();
            return newUser;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manager = await _dataContext.Managers
                .Include(o => o.User)
                .ThenInclude(t => t.FavoriteTeam)
                .ThenInclude(l => l.League)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (manager == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Id = manager.Id,
                Document = manager.User.Document,
                FirstName = manager.User.FirstName,
                LastName = manager.User.LastName,
                Address = manager.User.Address,
                PhoneNumber = manager.User.PhoneNumber,
                Picture = manager.User.Picture,
                BornDate = manager.User.BornDate,
                Sex = manager.User.Sex,

                NickName = manager.User.NickName,
                FavoriteTeamId = manager.User.FavoriteTeam.Id,
                LeagueId = manager.User.FavoriteTeam.League.Id,
                TeamId = manager.User.FavoriteTeam.Id,
                SexId = manager.User.Sex,
                Points = manager.User.Points,
                Latitude = manager.User.Latitude,
                Longitude = manager.User.Longitude,
                Leagues = _combosHelper.GetComboLeagues(),
                Teams = _combosHelper.GetComboTeams(manager.User.FavoriteTeam.League.Id),
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


                var manager = await _dataContext.Managers
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == model.Id);

                manager.User.Document = model.Document;
                manager.User.FirstName = model.FirstName;
                manager.User.LastName = model.LastName;
                manager.User.Address = model.Address;
                manager.User.PhoneNumber = model.PhoneNumber;
                manager.User.BornDate = model.BornDate;
                manager.User.Picture = path;
                manager.User.Sex = model.Sex;
                manager.User.NickName = model.NickName;
                manager.User.FavoriteTeam = await _dataContext.Teams.FindAsync(model.FavoriteTeamId);
                manager.User.Points = model.Points;
                manager.User.Latitude = model.Latitude;
                manager.User.Longitude = model.Longitude;

                await _userHelper.UpdateUserAsync(manager.User);
                return RedirectToAction(nameof(Index));
            }
            model.Leagues = _combosHelper.GetComboLeagues();
            model.Teams = _combosHelper.GetComboTeams(0);
            model.Sexs = _combosHelper.GetComboSexs();
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manager = await _dataContext.Managers
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (manager == null)
            {
                return NotFound();
            }

            await _userHelper.DeleteUserAsync(manager.User.Email);
            _dataContext.Managers.Remove(manager);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction($"{nameof(Index)}");
        }

        private bool ManagerExists(int id)
        {
            return _dataContext.Managers.Any(e => e.Id == id);
        }
    }
}
