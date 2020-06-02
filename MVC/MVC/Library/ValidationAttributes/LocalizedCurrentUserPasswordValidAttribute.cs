using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using HBS.LocalizedValidationAttributes.Kentico.MVC;
using System.Web;
using System.Web.Mvc;

namespace Generic.Attributes
{
    /// <summary>
    /// Checks if the password matches the current user's password.  Used in Password reset.
    /// </summary>
    public class LocalizedCurrentUserPasswordValidAttribute : LocalizedValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if(value is string)
            {
                string Password = value.ToString();
                IUserService _UserService = DependencyResolver.Current.GetService<IUserService>();
                IUserRepository _UserRepo = DependencyResolver.Current.GetService<IUserRepository>();
                var CurrentUser = _UserRepo.GetUserByUsername(HttpContext.Current.User.Identity.Name);
                return _UserService.ValidateUserPassword(CurrentUser, Password);
            }
            else
            {
                return false;
            }
        }
    }
}