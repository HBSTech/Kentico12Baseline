using AutoMapper;
using CMS.Base;
using CMS.Helpers;
using CMS.Membership;
using Generic.Models.User;
using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using Kentico.Membership;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Web;

namespace Generic.Services.Implementation
{
    public class KenticoUserService : IUserService
    {
        readonly ISiteRepository _SiteRepo;
        readonly ISiteSettingsRepository _SiteSettingRepo;
        readonly IUserRepository _UserRepo;
        private readonly IMapper _Mapper;

        public UserManager UserManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Get<UserManager>();
            }
        }

        public KenticoUserService(ISiteRepository SiteRepo,
            ISiteSettingsRepository SiteSettingsRepo,
            IUserRepository UserRepo,
            IMapper Mapper)
        {
            _SiteRepo = SiteRepo;
            _SiteSettingRepo = SiteSettingsRepo;
            _UserRepo = UserRepo;
            _Mapper = Mapper;
        }

        public IUserInfo CreateUser(IUserInfo User, string Password, bool Enabled = false)
        {
            // Create basic user
            UserInfo NewUser = new UserInfo()
            {
                UserName = User.UserName,
                FirstName = User.FirstName,
                LastName = User.LastName,
                Email = User.Email,
                SiteIndependentPrivilegeLevel = UserPrivilegeLevelEnum.None,
                Enabled = Enabled
            };
            UserInfoProvider.SetUserInfo(NewUser);

            // Generate new password, and save any other settings
            UserInfoProvider.SetPassword(NewUser, Password);

            return NewUser;
        }

        public async Task SendRegistrationConfirmationEmailAsync(IUserInfo user, string confirmationUrl)
        {
            string token = await UserManager.GenerateEmailConfirmationTokenAsync(user.UserID);

            // Creates and sends the confirmation email to the user's address
            await UserManager.SendEmailAsync(user.UserID, "Confirm your new account",
                string.Format($"Please confirm your new account by clicking <a href=\"{confirmationUrl}?userId={user.UserGUID}&token={HttpUtility.UrlEncode(token)}\">here</a>"));
        }

        public async Task<IdentityResult> ConfirmRegistrationConfirmationToken(int UserID, string token)
        {
            return await UserManager.ConfirmEmailAsync(UserID, token);
        }

        public async Task SendPasswordResetEmailAsync(IUserInfo user, string ConfirmationLink)
        {
            string token = await UserManager.GeneratePasswordResetTokenAsync(user.UserID);

            // Creates and sends the confirmation email to the user's address
            await UserManager.SendEmailAsync(user.UserID, "Password Reset Request",
                string.Format($"A Password reset request has been generated for your account.  If you have generated this request, you may reset your password by clicking <a href=\"{ConfirmationLink}?userId={user.UserGUID}&token={HttpUtility.UrlEncode(token)}\">here</a>."));
        }

        public IdentityResult ResetPasswordFromToken(int UserID, string Token, string NewPassword)
        {
            return UserManager.ResetPassword(UserID, Token, NewPassword);
        }

        public bool ValidateUserPassword(IUserInfo user, string password)
        {
            return UserInfoProvider.ValidateUserPassword(user as UserInfo, password);
        }

        public void ResetPassword(IUserInfo user, string password)
        {
            UserInfoProvider.SetPassword(user.UserName, password, true);
        }

        public bool ValidatePasswordPolicy(string password, string SiteName)
        {
            return SecurityHelper.CheckPasswordPolicy(password, SiteName);
        }

        public IUserInfo BasicUserToIUser(BasicUser User)
        {
            return _Mapper.Map<UserInfo>(User);
        }
    }
}