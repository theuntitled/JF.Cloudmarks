using System.Configuration;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace JF.Cloudmarks.Modules.Identity.MessageServices {

	public class EmailService : IEmailService {

		private readonly SmtpClient _smtpClient;

		public EmailService( SmtpClient smtpClient ) {
			_smtpClient = smtpClient;
		}

		public Task SendAsync( IdentityMessage message ) {
			var mailMessage = new MailMessage {
				Body = message.Body ,
				Subject = message.Subject ,
				IsBodyHtml = true ,
				From =
					new MailAddress(
						ConfigurationManager.AppSettings.Get( "DefaultMailFromAddress" ) ,
						ConfigurationManager.AppSettings.Get( "DefaultMailFromName" ) ,
						Encoding.UTF8 )
			};

			mailMessage.To.Add( message.Destination );

			_smtpClient.Send( mailMessage );

			return Task.FromResult( 0 );
		}

	}

}
