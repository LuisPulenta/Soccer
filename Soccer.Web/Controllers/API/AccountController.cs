using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Soccer.Common.Enums;
using Soccer.Common.Models;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Soccer.Web.Controllers.API
{
    [Route("api/[Controller]")]
    public class AccountController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public AccountController(
            DataContext dataContext,
            IUserHelper userHelper,
            IMailHelper mailHelper,
            IImageHelper imageHelper,
            IConverterHelper converterHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }

        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request",
                    Result = ModelState
                });
            }

            User user = await _userHelper.GetUserAsync(request.Email);
            if (user != null)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Este Usuario ya existe"
                });
            }

            string picturePath = string.Empty;
            if (request.PictureArray != null && request.PictureArray.Length > 0)
            {
                picturePath = _imageHelper.UploadImage(request.PictureArray, "Users");
            }

            user = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                NickName = request.NickName,
                LastName = request.LastName,
                UserName = request.Email,
                Picture = picturePath,
                FavoriteTeam = await _dataContext.Teams.FindAsync(request.TeamId)
            };

            IdentityResult result = await _userHelper.AddUserAsync(user, request.Password);
            if (result != IdentityResult.Success)
            {
                return BadRequest(result.Errors.FirstOrDefault().Description);
            }

            User userNew = await _userHelper.GetUserAsync(request.Email);

            await _userHelper.AddUserToRoleAsync(userNew, UserType.Player.ToString());
            _dataContext.Players.Add(new Player { User = userNew });
            await _dataContext.SaveChangesAsync();

            string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            string tokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userid = user.Id,
                token = myToken
            }, protocol: HttpContext.Request.Scheme);


            _mailHelper.SendMail("Soporte Soccer",user.Email, "Confirmación de Email",
                  $"<table style = 'max-width: 800px; padding: 10px; margin:0 auto; border-collapse: collapse;'>" +
                  $"  <tr>" +
                  $"    <td style = 'background-color: #3658a8; text-align: center; padding: 0'>" +
                                     $"  <td style = 'padding: 0'>" +
                  $"<tr>" +
                  $" <td style = 'background-color: #ecf0f1'>" +
                  $"      <div style = 'color: #3658a8; margin: 4% 10% 2%; text-align: justify;font-family: sans-serif'>" +
                  $"            <h1 style = 'color: #e67e22; margin: 0 0 7px' >Soporte Soccer </h1>" +
                  $"                    <p style = 'margin: 2px; font-size: 15px'>" +
                  $"                      Para completar el registro de su Usuario usted debe confirmar la dirección de Email haciendo clic en el botón del final de este mail.<br>" +
                  $"      <ul style = 'font-size: 15px;  margin: 10px 0'>" +
                  $"  <div style = 'width: 100%; text-align: center'>" +
                  $"    <h2 style = 'color: #e67e22; margin: 0 0 5px' >Confirmación de Email</h2>" +
                  $"    Para habilitar el usuario, por favor hacer clic en el siguiente enlace: </ br ></ br > " +
                  $"    <p style = 'color: #b3b3b3; font-size: 12px; text-align: center;margin: 10px 0 0' >   </p>" +
                  $"    <a style ='text-decoration: none; border-radius: 5px; padding: 5px 5px; color: white; background-color: #3658a8' href = \"{tokenLink}\">Confirmar Email</a>" +
                  $"    <p style = 'color: #b3b3b3; font-size: 12px; text-align: center;margin: 10px 0 0' >Soporte Soccer</p>" +
                  $"  </div>" +
                  $" </td >" +
                  $"</tr>" +
                  $"</table>");

            return Ok(new Response
            {
                IsSuccess = true,
                Message = "Un mail de confirmación fue enviado. Ingrese a su cuenta para confirmar su mail e ingrese a la App."
            });
        }

        [HttpPost]
        [Route("RecoverPassword")]
        public async Task<IActionResult> RecoverPassword([FromBody] EmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request"
                });
            }

            var user = await _userHelper.GetUserAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Este mail no pertenece a ningún Usuario."
                });
            }

            var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
            var link = Url.Action("ResetPassword", "Account", new { token = myToken }, protocol: HttpContext.Request.Scheme);

            _mailHelper.SendMail("Soporte Soccer", user.Email, "Resetear Password",
                  $"<table style = 'max-width: 800px; padding: 10px; margin:0 auto; border-collapse: collapse;'>" +
                  $"  <tr>" +
                  $"    <td style = 'background-color: #3658a8; text-align: center; padding: 0'>" +
                                     $"  <td style = 'padding: 0'>" +
                  $"<tr>" +
                  $" <td style = 'background-color: #ecf0f1'>" +
                  $"      <div style = 'color: #3658a8; margin: 4% 10% 2%; text-align: justify;font-family: sans-serif'>" +
                  $"            <h1 style = 'color: #e67e22; margin: 0 0 7px' >Soporte Soccer </h1>" +
                  $"                    <p style = 'margin: 2px; font-size: 15px'>" +
                  $"                      Para establecer una nueva contraseña usted debe hacer clic en el botón del final de este mail.<br>" +
                  $"      <ul style = 'font-size: 15px;  margin: 10px 0'>" +
                  $"  <div style = 'width: 100%; text-align: center'>" +
                  $"    <h2 style = 'color: #e67e22; margin: 0 0 5px' >Resetear Password</h2>" +
                  $"    Para establecer una nueva contraseña, por favor hacer clic en el siguiente enlace: </ br ></ br > " +
                  $"    <p style = 'color: #b3b3b3; font-size: 12px; text-align: center;margin: 10px 0 0' >   </p>" +
                  $"    <a style ='text-decoration: none; border-radius: 5px; padding: 5px 5px; color: white; background-color: #3658a8' href = \"{link}\">Resetear Password</a>" +
                  $"    <p style = 'color: #b3b3b3; font-size: 12px; text-align: center;margin: 10px 0 0' >Soporte Soccer</p>" +
                  $"  </div>" +
                  $" </td >" +
                  $"</tr>" +
                  $"</table>");

            return Ok(new Response
            {
                IsSuccess = true,
                Message = "Se le envió un mail con instrucciones para resetear el Password."
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        public async Task<IActionResult> PutAccount([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }



            User userEntity = await _userHelper.GetUserAsync(request.Email);
            if (userEntity == null)
            {
                return BadRequest("Este Usuario no existe.");
            }

            string picturePath = userEntity.Picture;
            if (request.PictureArray != null && request.PictureArray.Length > 0)
            {
                picturePath = _imageHelper.UploadImage(request.PictureArray, "Users");
            }

            userEntity.FirstName = request.FirstName;
            userEntity.LastName = request.LastName;
            userEntity.NickName = request.NickName;
            userEntity.FavoriteTeam = await _dataContext.Teams.FindAsync(request.TeamId);
            userEntity.Picture = picturePath;
            
            IdentityResult respose = await _userHelper.UpdateUserAsync(userEntity);
            if (!respose.Succeeded)
            {
                return BadRequest(respose.Errors.FirstOrDefault().Description);
            }

            return NoContent();
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request",
                    Result = ModelState
                });
            }


            User user = await _userHelper.GetUserAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Usuario no encontrado"
                });
            }

            IdentityResult result = await _userHelper.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = result.Errors.FirstOrDefault().Description
                });
            }

            return Ok(new Response
            {
                IsSuccess = true,
                Message = "El password fue cambiado con éxito."
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail([FromBody] EmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            User userEntity = await _userHelper.GetUserAsync(request.Email);
            if (userEntity == null)
            {
                return NotFound("Este Usuario no existe.");
            }

            var player = _dataContext.Players
                .FirstOrDefault(o => o.User.Id == userEntity.Id);
            return Ok(_converterHelper.ToPlayerResponse(player));
        }
    }
}