using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using JF.Cloudmarks.Modules.Identity;
using JF.Cloudmarks.Modules.ViewModels.Account;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace JF.Cloudmarks.Controllers {

	[Authorize]
	public class AccountController : IdentityControllerBase {

		private readonly ISignInManager _signInManager;

		private readonly IUserManager _userManager;

		/// <summary />
		/// <param name="userManager"></param>
		/// <param name="signInManager"></param>
		public AccountController( IUserManager userManager , ISignInManager signInManager ) {
			_userManager = userManager;
			_signInManager = signInManager;
		}

		[AllowAnonymous]
		public ActionResult Login( string returnUrl ) {
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login( LoginViewModel model , string returnUrl ) {
			if ( !ModelState.IsValid ) {
				return View( model );
			}

			// This doesn't count login failures towards account lockout
			// To enable password failures to trigger account lockout, change to shouldLockout: true
			var result = await _signInManager.PasswordSignInAsync( model.Username , model.Password , model.RememberMe , false );
			switch ( result ) {
				case SignInStatus.Success:
					return RedirectToLocal( returnUrl );
				case SignInStatus.LockedOut:
					return View( "Lockout" );
				case SignInStatus.RequiresVerification:
					return RedirectToAction( "SendCode" , new { ReturnUrl = returnUrl , model.RememberMe } );
				default:
					ModelState.AddModelError( "" , "Invalid login attempt." );
					return View( model );
			}
		}

		[AllowAnonymous]
		public async Task<ActionResult> VerifyCode( string provider , string returnUrl , bool rememberMe ) {
			// Require that the user has already logged in via username/password or external login
			if ( !await _signInManager.HasBeenVerifiedAsync() ) {
				return View( "Error" );
			}
			return View( new VerifyCodeViewModel {Provider = provider , ReturnUrl = returnUrl , RememberMe = rememberMe} );
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> VerifyCode( VerifyCodeViewModel model ) {
			if ( !ModelState.IsValid ) {
				return View( model );
			}

			// The following code protects for brute force attacks against the two factor codes. 
			// If a user enters incorrect codes for a specified amount of time then the user account 
			// will be locked out for a specified amount of time. 
			// You can configure the account lockout settings in IdentityConfig
			var result =
				await _signInManager.TwoFactorSignInAsync( model.Provider , model.Code , model.RememberMe , model.RememberBrowser );
			switch ( result ) {
				case SignInStatus.Success:
					return RedirectToLocal( model.ReturnUrl );
				case SignInStatus.LockedOut:
					return View( "Lockout" );
				default:
					ModelState.AddModelError( "" , "Invalid code." );
					return View( model );
			}
		}

		[AllowAnonymous]
		public ActionResult Register() {
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register( RegisterViewModel model ) {
			if ( ModelState.IsValid ) {
				var user = new ApplicationUser {UserName = model.Username , Email = model.Email};
				var result = await _userManager.CreateAsync( user , model.Password );
				if ( result.Succeeded ) {
					await _signInManager.SignInAsync( user , false , false );

					// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
					// Send an email with this link
					// string code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
					// var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
					// await _userManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

					return RedirectToAction( "Index" , "Bookmarks" );
				}

				AddErrors( result );
			}

			// If we got this far, something failed, redisplay form
			return View( model );
		}

		[AllowAnonymous]
		public async Task<ActionResult> ConfirmEmail( string userId , string code ) {
			if ( userId == null || code == null ) {
				return View( "Error" );
			}
			var result = await _userManager.ConfirmEmailAsync( userId , code );
			return View( result.Succeeded ? "ConfirmEmail" : "Error" );
		}

		[AllowAnonymous]
		public ActionResult ForgotPassword() {
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgotPassword( ForgotPasswordViewModel model ) {
			if ( ModelState.IsValid ) {
				var user = await _userManager.FindByNameAsync( model.Email );
				if ( user == null || !( await _userManager.IsEmailConfirmedAsync( user.Id ) ) ) {
					// Don't reveal that the user does not exist or is not confirmed
					return View( "ForgotPasswordConfirmation" );
				}

				// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
				// Send an email with this link
				// string code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
				// var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
				// await _userManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
				// return RedirectToAction("ForgotPasswordConfirmation", "Account");
			}

			// If we got this far, something failed, redisplay form
			return View( model );
		}

		[AllowAnonymous]
		public ActionResult ForgotPasswordConfirmation() {
			return View();
		}

		[AllowAnonymous]
		public ActionResult ResetPassword( string code ) {
			return code == null ? View( "Error" ) : View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword( ResetPasswordViewModel model ) {
			if ( !ModelState.IsValid ) {
				return View( model );
			}
			var user = await _userManager.FindByNameAsync( model.Email );
			if ( user == null ) {
				// Don't reveal that the user does not exist
				return RedirectToAction( "ResetPasswordConfirmation" , "Account" );
			}
			var result = await _userManager.ResetPasswordAsync( user.Id , model.Code , model.Password );
			if ( result.Succeeded ) {
				return RedirectToAction( "ResetPasswordConfirmation" , "Account" );
			}
			AddErrors( result );
			return View();
		}

		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation() {
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin( string provider , string returnUrl ) {
			// Request a redirect to the external login provider
			return new ChallengeResult( provider ,
										Url.Action( "ExternalLoginCallback" , "Account" , new {ReturnUrl = returnUrl} ) );
		}

		[AllowAnonymous]
		public async Task<ActionResult> SendCode( string returnUrl , bool rememberMe ) {
			var userId = await _signInManager.GetVerifiedUserIdAsync();
			if ( userId == null ) {
				return View( "Error" );
			}
			var userFactors = await _userManager.GetValidTwoFactorProvidersAsync( userId );
			var factorOptions = userFactors.Select( purpose => new SelectListItem {Text = purpose , Value = purpose} ).ToList();
			return View( new SendCodeViewModel {Providers = factorOptions , ReturnUrl = returnUrl , RememberMe = rememberMe} );
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendCode( SendCodeViewModel model ) {
			if ( !ModelState.IsValid ) {
				return View();
			}

			// Generate the token and send it
			if ( !await _signInManager.SendTwoFactorCodeAsync( model.SelectedProvider ) ) {
				return View( "Error" );
			}
			return RedirectToAction( "VerifyCode" , new {Provider = model.SelectedProvider , model.ReturnUrl , model.RememberMe} );
		}

		[AllowAnonymous]
		public async Task<ActionResult> ExternalLoginCallback( string returnUrl ) {
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
			if ( loginInfo == null ) {
				return RedirectToAction( "Login" );
			}

			// Sign in the user with this external login provider if the user already has a login
			var result = await _signInManager.ExternalSignInAsync( loginInfo , false );
			switch ( result ) {
				case SignInStatus.Success:
					return RedirectToLocal( returnUrl );
				case SignInStatus.LockedOut:
					return View( "Lockout" );
				case SignInStatus.RequiresVerification:
					return RedirectToAction( "SendCode" , new {ReturnUrl = returnUrl , RememberMe = false} );
				default:
					// If the user does not have an account, then prompt the user to create an account
					ViewBag.ReturnUrl = returnUrl;
					ViewBag.LoginProvider = loginInfo.Login.LoginProvider;

					return View( "ExternalLoginConfirmation" , new ExternalLoginConfirmationViewModel {
						Email = loginInfo.Email ,
						Username = loginInfo.DefaultUserName
					} );
			}
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ExternalLoginConfirmation( ExternalLoginConfirmationViewModel model , string returnUrl ) {
			if ( User.Identity.IsAuthenticated ) {
				return RedirectToAction( "Index" , "Manage" );
			}

			if ( ModelState.IsValid ) {
				// Get the information about the user from the external login provider
				var info = await AuthenticationManager.GetExternalLoginInfoAsync();
				if ( info == null ) {
					return View( "ExternalLoginFailure" );
				}
				var user = new ApplicationUser { UserName = model.Username , Email = model.Email };
				var result = await _userManager.CreateAsync( user );

				if ( result.Succeeded ) {
					result = await _userManager.AddLoginAsync( user.Id , info.Login );

					if ( result.Succeeded ) {
						await _signInManager.SignInAsync( user , false , false );
						return RedirectToLocal( returnUrl );
					}
				}
				AddErrors( result );
			}

			ViewBag.ReturnUrl = returnUrl;
			return View( model );
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff() {
			AuthenticationManager.SignOut( DefaultAuthenticationTypes.ApplicationCookie );
			return RedirectToAction( "Index" , "Bookmarks" );
		}

		[AllowAnonymous]
		public ActionResult ExternalLoginFailure() {
			return View();
		}

		#region Helpers

		internal class ChallengeResult : HttpUnauthorizedResult {

			public ChallengeResult( string provider , string redirectUri )
				: this( provider , redirectUri , null ) {
			}

			public ChallengeResult( string provider , string redirectUri , string userId ) {
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }

			public string RedirectUri { get; set; }

			public string UserId { get; set; }

			public override void ExecuteResult( ControllerContext context ) {
				var properties = new AuthenticationProperties {RedirectUri = RedirectUri};
				if ( UserId != null ) {
					properties.Dictionary[IdentityConstants.XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge( properties , LoginProvider );
			}

		}

		#endregion
	}

}
