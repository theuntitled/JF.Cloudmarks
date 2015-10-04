using System.ComponentModel.DataAnnotations;

namespace JF.Cloudmarks.Modules.ViewModels.Account {

	public class LoginViewModel {

		[Required]
		[Display( Name = "Username" )]
		[EmailAddress]
		public string Username { get; set; }

		[Required]
		[DataType( DataType.Password )]
		[Display( Name = "Password" )]
		public string Password { get; set; }

		[Display( Name = "Remember me?" )]
		public bool RememberMe { get; set; }

	}

}
