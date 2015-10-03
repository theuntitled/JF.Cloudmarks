using Microsoft.AspNet.Identity;

namespace JF.Cloudmarks.Modules.Identity {

	public interface IUserValidator : IIdentityValidator<ApplicationUser> {

		bool RequireUniqueEmail { get; set; }

		bool AllowOnlyAlphanumericUserNames { get; set; }

	}

}
