using Microsoft.AspNetCore.Mvc;
using Soccer.Web.Helpers;
using Soccer.Web.Models;
using System.Linq;
using System.Threading.Tasks;
using Soccer.Web.Data.Entities;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Soccer.Common.Enums;
using Soccer.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.IO;

namespace Soccer.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;
        private readonly DataContext _context;

        public AccountController(IUserHelper userHelper, ICombosHelper combosHelper, IImageHelper imageHelper,
    IConfiguration configuration, IMailHelper mailHelper, DataContext context)
        {
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _imageHelper = imageHelper;
            _configuration = configuration;
            _mailHelper = mailHelper;
            _context = context;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError(string.Empty, "Usuario o contraseña inválidos.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        public async Task<JsonResult> GetTeamsAsync(int leagueId)
        {
            var teams = await _context.Teams
                .Where(p => p.League.Id == leagueId)
                .OrderBy(p => p.Name)
                .ToListAsync();
            return Json(teams);
        }

        public IActionResult Register()
        {
            var model = new AddUserViewModel
            {
                Sexs = _combosHelper.GetComboSexs(),
                UserTypes = _combosHelper.GetComboUserTypes(),
                Leagues = _combosHelper.GetComboLeagues(),
                Teams = _combosHelper.GetComboTeams(0),
                UserTypeId=1
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
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

                _context.Players.Add(player);
                await _context.SaveChangesAsync();

                var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                var tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                _mailHelper.SendMail(model.Username, "Confirmación de E-Mail", $"<h1>Confirmación de E-Mail</h1>" +
                    $"Para habilitar el Usuario, " +
                    $"por favor haga clic en el siguiente link: </br></br><a href = \"{tokenLink}\">Confirmar E-mail</a>");

                ViewBag.Message = "Las instrucciones para habilitar su Usuario han sido enviadas por mail.";
                return View(model);
                
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
                path = await _imageHelper.UploadImageAsync(model.ImageFile, "Users");
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
                FavoriteTeam = await _context.Teams.FindAsync(model.FavoriteTeamId),
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


        public async Task<IActionResult> ChangeUser()
        {
            var player = await _context.Players
                .Include(o => o.User)
                .ThenInclude(t=> t.FavoriteTeam)
                .ThenInclude(l => l.League)
                .FirstOrDefaultAsync(o => o.User.Email.ToLower().Equals(User.Identity.Name.ToLower()));
            if (player == null)
            {
                return NotFound();
            }

            var SI = String.Empty;
            if (player.User.Sex=="Hombre")
            {
                SI = "1";
            }
            if (player.User.Sex == "Mujer")
            {
                SI = "2";
            }
            EditUserViewModel model = new EditUserViewModel
            {
                Address = player.User.Address,
                Document = player.User.Document,
                FirstName = player.User.FirstName,
                LastName = player.User.LastName,
                PhoneNumber = player.User.PhoneNumber,
                Picture = player.User.Picture,
                BornDate = player.User.BornDate,
                Latitude = player.User.Latitude,
                Longitude = player.User.Longitude,
                NickName = player.User.NickName,
                Points = player.User.Points,
                Sex = player.User.Sex,
                SexId=SI,
                UserTypeId=1,
                Sexs = _combosHelper.GetComboSexs(),
                Leagues=_combosHelper.GetComboLeagues(),
                LeagueId=player.User.FavoriteTeam.League.Id,
                Teams=_combosHelper.GetComboTeams(player.User.FavoriteTeam.League.Id),
                TeamId = player.User.FavoriteTeam.Id,
                FavoriteTeamId = player.User.FavoriteTeam.Id,
                Id =player.Id,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var player = await _context.Players
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == model.Id);


               

               



                string path = model.Picture;

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






                player.User.Document = model.Document;
                player.User.FirstName = model.FirstName;
                player.User.LastName = model.LastName;
                player.User.Address = model.Address;
                player.User.PhoneNumber = model.PhoneNumber;
                player.User.Picture = path;
                player.User.Address = model.Address;
                player.User.BornDate = model.BornDate;
                player.User.Picture = path;
                player.User.Latitude = model.Latitude;
                player.User.Longitude = model.Longitude;
                player.User.NickName = model.NickName;
                player.User.Points = model.Points;
                player.User.Sex = model.Sex;
                player.User.FavoriteTeam = await _context.Teams.FindAsync(model.FavoriteTeamId);

                await _userHelper.UpdateUserAsync(player.User);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserAsync(User.Identity.Name);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Este usuario no existe.");
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserAsync(model.Username);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15),
                            signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return Created(string.Empty, results);
                    }
                }
            }

            return BadRequest();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            User user = await _userHelper.GetUserAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound();
            }

            Microsoft.AspNetCore.Identity.IdentityResult result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "El Email no corresponde a un usuario registrado.");
                    return View(model);
                }

                string myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                string link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);
                _mailHelper.SendMail(model.Email, "Soccer Recupero de Password", $"<h1>Soccer Recupero de Password</h1>" +
                    $"Para recuperar el Password haga clic en este link: </br></br>" +
                    $"<a href = \"{link}\">Resetear Password</a>");
                ViewBag.Message = "Las instrucciones para recuperar su password han sido enviadas por mail.";
                return View();

            }

            return View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            User user = await _userHelper.GetUserAsync(model.UserName);
            if (user != null)
            {
                IdentityResult result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Password reseteado correctamente.";
                    return View();
                }

                ViewBag.Message = "Error mientras se reseteaba el password.";
                return View(model);
            }

            ViewBag.Message = "Usuario no encontrado.";
            return View(model);
        }

        public async Task<IActionResult> ConfirmUserGroup(int requestId, string token)
        {
            if (requestId == 0 || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            PlayerGroupBetRequestEntity playerGroupBetRequest = await _context.PlayerGroupBetRequests
                .Include(ugr => ugr.ProposalPlayer)
                .ThenInclude(us=>us.User)
                .Include(ugr => ugr.RequiredPlayer)
                .ThenInclude(us => us.User)
                .FirstOrDefaultAsync(ugr => ugr.Id == requestId &&
                                            ugr.Token == new Guid(token));
            if (playerGroupBetRequest == null)
            {
                return NotFound();
            }

            //await AddGroupBetPlayerAsync(playerGroupBetRequest.ProposalPlayer);
            

            //playerGroupBetRequest.Status = UserGroupStatus.Accepted;
            //_context.UserGroupRequests.Update(playerGroupBetRequest);
            //await _context.SaveChangesAsync();
            return View();
        }

        //private async Task AddGroupBetPlayerAsync(Player proposalPlayer)
        //{
        //    UserGroupEntity userGroup = await _context.UserGroups
        //        .Include(ug => ug.Users)
        //        .ThenInclude(u => u.User)
        //        .FirstOrDefaultAsync(ug => ug.User.Id == proposalUser.Id);
        //    if (userGroup != null)
        //    {
        //        UserGroupDetailEntity user = userGroup.Users.FirstOrDefault(u => u.User.Id == requiredUser.Id);
        //        if (user == null)
        //        {
        //            userGroup.Users.Add(new UserGroupDetailEntity { User = requiredUser });
        //        }

        //        _context.UserGroups.Update(userGroup);
        //    }
        //    else
        //    {
        //        _context.UserGroups.Add(new UserGroupEntity
        //        {
        //            User = proposalUser,
        //            Users = new List<UserGroupDetailEntity>
        //    {
        //        new UserGroupDetailEntity { User = requiredUser }
        //    }
        //        });
        //    }
        //}

        //public async Task<IActionResult> RejectUserGroup(int requestId, string token)
        //{
        //    if (requestId == 0 || string.IsNullOrEmpty(token))
        //    {
        //        return NotFound();
        //    }

        //    UserGroupRequestEntity userGroupRequest = await _context
        //        .UserGroupRequests.FirstOrDefaultAsync(ugr => ugr.Id == requestId &&
        //                                                ugr.Token == new Guid(token));
        //    if (userGroupRequest == null)
        //    {
        //        return NotFound();
        //    }

        //    userGroupRequest.Status = UserGroupStatus.Rejected;
        //    _context.UserGroupRequests.Update(userGroupRequest);
        //    await _context.SaveChangesAsync();
        //    return View();
        //}


    }
}