using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace JF.Cloudmarks.Modules.Identity {

	public class IdentityControllerBase : Controller {

		protected IAuthenticationManager AuthenticationManager {
			get { return HttpContext.GetOwinContext().Authentication; }
		}

		protected void AddErrors( IdentityResult result ) {
			foreach ( var error in result.Errors ) {
				ModelState.AddModelError( "" , error );
			}
		}

		protected ActionResult RedirectToLocal( string returnUrl ) {
			if ( Url.IsLocalUrl( returnUrl ) ) {
				return Redirect( returnUrl );
			}

			return RedirectToAction( "Index" , "Home" );
		}

	}

}
