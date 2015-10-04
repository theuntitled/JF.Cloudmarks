using System.ComponentModel.DataAnnotations;

namespace JF.Cloudmarks.Modules.ViewModels.Account {

	public class ExternalLoginConfirmationViewModel {

		[Required]
		[Display( Name = "Email" )]
		public string Email { get; set; }

		[Required]
		[Display( Name = "Username" )]
		public string Username { get; set; }

	}

}
