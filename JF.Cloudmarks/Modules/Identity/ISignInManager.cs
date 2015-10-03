﻿using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace JF.Cloudmarks.Modules.Identity {

	public interface ISignInManager {

		string AuthenticationType { get; set; }

		UserManager<ApplicationUser , string> UserManager { get; set; }

		IAuthenticationManager AuthenticationManager { get; set; }

		Task<ClaimsIdentity> CreateUserIdentityAsync( ApplicationUser user );

		string ConvertIdToString( string id );

		string ConvertIdFromString( string id );

		Task SignInAsync( ApplicationUser user , bool isPersistent , bool rememberBrowser );

		Task<bool> SendTwoFactorCodeAsync( string provider );

		Task<string> GetVerifiedUserIdAsync();

		Task<bool> HasBeenVerifiedAsync();

		Task<SignInStatus> TwoFactorSignInAsync( string provider , string code , bool isPersistent , bool rememberBrowser );

		Task<SignInStatus> ExternalSignInAsync( ExternalLoginInfo loginInfo , bool isPersistent );

		Task<SignInStatus> PasswordSignInAsync( string userName , string password , bool isPersistent , bool shouldLockout );

		void Dispose();

	}

}
