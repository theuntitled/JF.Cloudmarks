using System.Web.Mvc;

namespace JF.Cloudmarks.Controllers {

	[Authorize]
	public class HomeController : Controller {

		public ActionResult Index() {
			return View();
		}

	}

}
