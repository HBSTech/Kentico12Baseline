using System;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Kentico.Membership;
using CMS.EventLog;
using System.Web;
using Generic.Services.Interfaces;
using Generic.Repositories.Interfaces;
using CMS.Membership;
using Authorization.Kentico.MVC;
using Generic.Models;
using Generic.Models.User;

namespace Controllers
{
    public class AccountController : Controller
    {
        readonly IUserRepository _UserRepo;
        readonly IUserService _UserService;

        /// <summary>
        /// Provides access to the Kentico.Membership.SignInManager instance.
        /// </summary>
        public SignInManager SignInManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<SignInManager>();
            }
        }

        /// <summary>
        /// Provides access to the Kentico.Membership.UserManager instance.
        /// </summary>
        public UserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<UserManager>();
            }
        }

        public User CurrentUser
        {
            get
            {
                return UserManager.FindByName(User.Identity.Name);
            }
        }

        /// <summary>
        /// Provides access to the Microsoft.Owin.Security.IAuthenticationManager instance.
        /// </summary>
        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public AccountController(IUserRepository UserRepo,
            IUserService UserService)
        {
            _UserRepo = UserRepo;
            _UserService = UserService;
        }

        #region "Registration"

        [HttpGet]
        public ActionResult Register()
        {
            return View(new NewAccountViewModel());
        }

        /// <summary>
        /// Registers the User, uses Email confirmation
        /// </summary>
        /// <param name="UserAccountModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Register(NewAccountViewModel UserAccountModel)
        {
            // Ensure valid
            if (!ModelState.IsValid)
            {
                return View(UserAccountModel);
            }

            // Create a basic Kentico User and assign the portal ID
            var NewUser = _UserService.CreateUser(_UserService.BasicUserToIUser(UserAccountModel.User), UserAccountModel.Password);

            // Send confirmation email with registration link
            await _UserService.SendRegistrationConfirmationEmailAsync(NewUser as UserInfo, Url.Action("Confirmation", "Account", new object { }, protocol: Request.Url.Scheme));

            // Displays a view asking the visitor to check their email and confirm the new account
            return View("CheckYourEmail");
        }

        /// <summary>
        /// Action for confirming new user accounts. Handles the links that users click in confirmation emails.
        /// </summary>
        public ActionResult Confirmation(Guid? userId, string token)
        {
            IdentityResult confirmResult;

            try
            {
                if (!userId.HasValue)
                {
                    throw new InvalidOperationException("No user Identity Provided");
                }
                // Convert Guid to ID
                int? UserIntID = _UserRepo.UserGuidToID(userId.Value);

                // Verifies the confirmation parameters and enables the user account if successful
                confirmResult = _UserService.ConfirmRegistrationConfirmationToken(UserIntID.Value, token).Result;

                if (confirmResult.Succeeded)
                {
                    // If the verification was successful, displays a view informing the user that their account was activated
                    return View();
                }
            }
            catch (InvalidOperationException)
            {
                // An InvalidOperationException occurs if a user with the given ID is not found
                confirmResult = IdentityResult.Failed("User not found.");
            }

            // Returns a view informing the user that the email confirmation failed
            return View("ConfirmationFailed");
        }

        #endregion

        #region "Sign in/out"

        /// <summary>
        /// Basic action that displays the sign-in form.
        /// </summary>
        [HttpGet]
        public ActionResult SignIn()
        {
            if (!AuthenticationManager.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return View("Logout");
            }
        }

        /// <summary>
        /// Handles authentication when the sign-in form is submitted. Accepts parameters posted from the sign-in form via the SignInViewModel.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignIn(LoginViewModel model, string returnUrl)
        {
            // Validates the received user credentials based on the view model
            if (!ModelState.IsValid)
            {
                // Displays the sign-in form if the user credentials are invalid
                return View();
            }

            // Attempts to authenticate the user against the Kentico database
            SignInStatus signInResult = SignInStatus.Failure;
            try
            {
                var ActualUser = _UserRepo.GetUserByEmail(model.UserName, new string[] { "Username" });
                signInResult = await SignInManager.PasswordSignInAsync(ActualUser?.UserName, model.Password, model.StaySignedIn, false);
            }
            catch (Exception ex)
            {
                // Logs an error into the Kentico event log if the authentication fails
                EventLogProvider.LogException("SignInManager", "SignIn", ex);
            }

            // If the authentication was not successful, displays the sign-in form with an "Authentication failed" message
            if (signInResult != SignInStatus.Success)
            {
                ModelState.AddModelError(string.Empty, "Authentication failed");
                return View();
            }

            // If the authentication was successful, redirects to the return URL when possible or to a different default action
            string decodedReturnUrl = Server.UrlDecode(returnUrl);
            if (!string.IsNullOrEmpty(decodedReturnUrl) && Url.IsLocalUrl(decodedReturnUrl))
            {
                return Redirect(decodedReturnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Action for signing out users. The Authorize attribute allows the action only for users who are already signed in.
        /// </summary>
        [HttpGet]
        [KenticoAuthorize(UserAuthenticationRequired = true)]
        public ActionResult SignOut()
        {
            // Signs out the current user
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            // Redirects to a different action after the sign-out
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region "Forgot Password"

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View(new ForgotPasswordRequest());
        }

        /// <summary>
        /// For security, will always show that it sent an email to that user, even if it didn't find it.  The email will contain the link to reset the password
        /// </summary>
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordRequest Request)
        {
            if (!ModelState.IsValid)
            {
                return View(Request);
            }

            var User = _UserRepo.GetUserByEmail(Request.EmailAddress);
            if (User != null)
            {
                _UserService.SendPasswordResetEmailAsync(User, Url.Action("ForgottenPasswordReset", "Account", null, HttpContext.Request.Url.Scheme));
            }

            return View("ForgotPasswordSent");
        }

        /// <summary>
        /// Retrieves the UserGUID and the Token and presents the password reset.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ForgottenPasswordReset(Guid? userId, string token)
        {
            return View(new ResetForgotPassword()
            {
                UserID = userId.HasValue ? userId.Value : Guid.Empty,
                Token = token
            });
        }

        /// <summary>
        /// Validates and resets the password
        /// </summary>
        [HttpPost]
        public ActionResult ForgottenPasswordReset(ResetForgotPassword ForgottenPasswordResetModel)
        {
            if (!ModelState.IsValid)
            {
                return View(ForgottenPasswordResetModel);
            }

            int? UserID = _UserRepo.UserGuidToID(ForgottenPasswordResetModel.UserID);
            var Results = _UserService.ResetPasswordFromToken(UserID.Value, ForgottenPasswordResetModel.Token, ForgottenPasswordResetModel.Password);
            if (Results.Succeeded)
            {
                return View("ForgottenPasswordResetSuccessful");
            }
            else
            {
                return View("ForgottenPasswordResetFailure");
            }
        }

        #endregion

        #region "Password Reset"

        /// <summary>
        /// Password Reset, must be authenticated to reset password this way.
        /// </summary>        
        [HttpGet]
        [KenticoAuthorize(UserAuthenticationRequired = true)]
        public ActionResult ResetPassword()
        {
            return View(new ResetPasswordViewModel());
        }

        /// <summary>
        /// Password Reset, must be authenticated to reset password this way.
        /// </summary>
        [HttpPost]
        [KenticoAuthorize(UserAuthenticationRequired = true)]
        public ActionResult ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordViewModel);
            }

            var CurrentUser = _UserRepo.GetUserByUsername(User.Identity.Name);

            // Everything valid, reset password
            _UserService.ResetPassword(CurrentUser, resetPasswordViewModel.Password);

            return View("ResetPasswordSuccessful");
        }

        #endregion

        #region "My Account"

        /// <summary>
        /// Can enable and create a My Account View.
        /// </summary>
        /*[HttpGet]
        [KenticoAuthorize(UserAuthenticationRequired = true)]
        public ActionResult MyAccount()
        {
            return View();
        }*/

        #endregion
    }
}