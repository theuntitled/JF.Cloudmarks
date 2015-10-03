using System.ComponentModel.DataAnnotations;

namespace JF.Cloudmarks.Modules.ViewModels.Account {

	public class ForgotViewModel {

		[Required]
		[Display( Name = "Email" )]
		public string Email { get; set; }

	}

}
