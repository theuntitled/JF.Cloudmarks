using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace JF.Cloudmarks.Modules.Identity.MessageServices {

	public class SmsService : ISmsService {

		public Task SendAsync( IdentityMessage message ) {
			throw new NotImplementedException();
			// var twilio = new TwilioRestClient(
			// 	ConfigurationManager.AppSettings.Get( "TwilioSid" ) ,
			// 	ConfigurationManager.AppSettings.Get( "TwilioToken" ) );
			// 
			// var result = twilio.SendMessage(
			// 	ConfigurationManager.AppSettings.Get( "TwilioFromPhone" ) ,
			// 	message.Destination ,
			// 	message.Body );
			// 
			// // Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
			// Trace.TraceInformation( result.Status );
			// 
			// // Twilio doesn't currently have an async API, so return success.
			// return Task.FromResult( 0 );
		}

	}

}
