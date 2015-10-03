using System.ComponentModel.DataAnnotations;

namespace JF.Cloudmarks.Modules.ViewModels.Account {

	public class ForgotPasswordViewModel {

		[Required]
		[EmailAddress]
		[Display( Name = "Email" )]
		public string Email { get; set; }

	}

}
