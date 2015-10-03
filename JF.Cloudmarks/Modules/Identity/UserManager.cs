using System;
using System.Security.Claims;
using System.Threading.Tasks;
using JF.AspNet.Identity.DocumentDB;
using JF.Cloudmarks.Modules.Identity.MessageServices;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace JF.Cloudmarks.Modules.Identity {

	public class UserManager : UserManager<ApplicationUser> , IUserManager {

		public UserManager(
			IDocumentDBUserStore<ApplicationUser> store ,
			IEmailService emailService ) : base( store ) {
			// Configure validation logic for usernames
			UserValidator = new UserValidator<ApplicationUser>( this ) {
				RequireUniqueEmail = true ,
				AllowOnlyAlphanumericUserNames = false
			};

			// Configure validation logic for passwords
			PasswordValidator = new PasswordValidator {
				RequiredLength = 6 ,
				RequireDigit = true ,
				RequireLowercase = true ,
				RequireUppercase = false ,
				RequireNonLetterOrDigit = true
			};

			// Configure user lockout defaults
			UserLockoutEnabledByDefault = true;
			MaxFailedAccessAttemptsBeforeLockout = 5;
			DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes( 60 );

			// Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
			// You can write your own provider and plug it in here.
			/*RegisterTwoFactorProvider(
				"Phone Code" ,
				new PhoneNumberTokenProvider<ApplicationUser> {
					MessageFormat = "Your security code is {0}"
				} );*/

			RegisterTwoFactorProvider(
				"Username Code" ,
				new EmailTokenProvider<ApplicationUser> {
					Subject = "Security Code" ,
					BodyFormat = "Your security code is {0}"
				} );

			// SmsService = smsService;
			EmailService = emailService;

			if ( DefaultDataProtectionProvider != null ) {
				UserTokenProvider =
					new DataProtectorTokenProvider<ApplicationUser>( DefaultDataProtectionProvider.Create( "ASP.NET Identity" ) );
			}
		}

		public static IDataProtectionProvider DefaultDataProtectionProvider { get; set; }

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(
			ApplicationUser user ,
			string authenticationType = DefaultAuthenticationTypes.ApplicationCookie ) {
			var identity = await CreateIdentityAsync( user , authenticationType );

			identity.AddClaim( new Claim( ClaimTypes.Email , user.Email ) );

			return identity;
		}

	}

}
