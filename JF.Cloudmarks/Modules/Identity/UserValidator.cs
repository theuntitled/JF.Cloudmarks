using Microsoft.AspNet.Identity;

namespace JF.Cloudmarks.Modules.Identity {

	public class UserValidator : UserValidator<ApplicationUser , string> , IUserValidator {

		public UserValidator( IUserManager manager ) : base( manager as UserManager ) {
		}

	}

}
