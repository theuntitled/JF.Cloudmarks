using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace JF.Cloudmarks.Modules.Identity {

	public class SignInManager : SignInManager<ApplicationUser , string> , ISignInManager {

		public SignInManager( UserManager userManager , IAuthenticationManager authenticationManager )
			: base( userManager , authenticationManager ) {
		}

	}

}
