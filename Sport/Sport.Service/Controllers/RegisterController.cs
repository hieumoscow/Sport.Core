using System;
using System.Data.Entity;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Sport.Service.Models;
using Sport.Service.Results;
using Sport.Service.Strings;
using Sport.Service.Utilities.Auth;
using Sport.Service.ViewModels;

namespace Sport.Service.Controllers
{
    [Authorize]
    [RoutePrefix("api/register")]
    public class RegisterController : ApiController
    {
        private ApplicationUserManager _userManager;

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }
        public RegisterController()
        {
        }
        public RegisterController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous]
        public async Task<IHttpActionResult> Post(RegisterBindingModel request)
        {
            if (!ModelState.IsValid)
            {
                return ApiResult.Result(null, ModelState, null);
            }
            using (var context = new ApplicationDbContext())
            {
                var u = await context.Users.SingleOrDefaultAsync(x => x.PhoneNumber.EndsWith(request.PhoneNumber));
                if (u != null)
                {
                    ModelState.AddModelError(nameof(request.PhoneNumber), Resources.Register_PhoneExisted);
                }
            }
            var user = await UserManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                ModelState.AddModelError(nameof(request.Email), Resources.Register_EmailExisted);
                return ApiResult.Result(null, ModelState, null);
            }

            user = new ApplicationUser()
            {
                UserName = request.Email,
                Email = request.Email,
                Firstname = request.Firstname,
                Dob = request.Dob,
                Gender = request.Gender,
                PhoneNumber = request.PhoneNumber,               
            };

            IdentityResult result = await UserManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {

                return ApiResult.Result(result, null, null);
            }



            if (string.IsNullOrEmpty(user.Id))
            {
                user = await UserManager.FindByEmailAsync(request.Email);
            }

            var phone = await AuthHelper.SendConfirmSms(UserManager, user, request.PhoneNumber);

            if (!phone)
            {
                ModelState.AddModelError(nameof(request.PhoneNumber), Resources.Register_SmsFailed);
                await UserManager.DeleteAsync(user);
                return ApiResult.Result(null, ModelState, null);
            }

            //await UserManager.AddToRoleAsync(user.Id, request.Role);
            await AuthHelper.SendConfirmEmail(this, UserManager, user);
            return new ApiResult(await AuthHelper.SignIn(UserManager, user));
        }

        [HttpPost, Route("verify")]
        public async Task<IHttpActionResult> PhoneConfirm(PhoneConfirmBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return ApiResult.Result(null, ModelState, null);
            }
            var user = await UserManager.FindByIdAsync(model.UserId);
            if (user == null)
                throw new Exception(Resources.Register_UserNotFound);
            if (string.IsNullOrEmpty(model.NewPhoneNumber))
            {
                using (var context = new ApplicationDbContext())
                {
                    if ((await context.Users.FirstOrDefaultAsync(x => x.PhoneNumber.Equals(model.NewPhoneNumber))) != null)
                    {
                        return ApiResult.Exception(new Exception(Resources.Register_PhoneExisted));
                    }
                    user.PhoneNumber = model.NewPhoneNumber;
                    await context.SaveChangesAsync();
                }
                
            }
            var result = await UserManager.ChangePhoneNumberAsync(user.Id, model.NewPhoneNumber, model.Code);
            if (result == IdentityResult.Success)
            {
                return new ApiResult(await AuthHelper.SignIn(UserManager, user));
            }
            return ApiResult.Exception(new Exception("Mã xác thực không đúng"));
        }

        [HttpPost, Route("phone")]
        public async Task<IHttpActionResult> PhoneChange(PhoneConfirmBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return ApiResult.Result(null, ModelState, null);
            }
            if (!string.IsNullOrEmpty(model.NewPhoneNumber))
            {
                using (var context = new ApplicationDbContext())
                {
                    var u = await context.Users.SingleOrDefaultAsync(x => x.PhoneNumber.EndsWith(model.NewPhoneNumber));
                    if (u != null)
                    {
                        ModelState.AddModelError(nameof(model.NewPhoneNumber), Resources.Register_PhoneExisted);
                    }
                }
            }
            var user = await UserManager.FindByIdAsync(model.UserId);
            if (user == null)
                throw new Exception(Resources.Register_UserNotFound);
            if (string.IsNullOrEmpty(model.NewPhoneNumber))
            {
                if (user.PhoneNumberConfirmed)
                {
                    throw new Exception(Resources.Register_PhoneVerified);
                }
                else
                {
                    await AuthHelper.SendConfirmSms(UserManager, user, user.PhoneNumber);
                }
            }
            else
            {
                await AuthHelper.SendConfirmSms(UserManager, user, model.NewPhoneNumber);
            }
            return ApiResult.Ok();
        }
    }
}