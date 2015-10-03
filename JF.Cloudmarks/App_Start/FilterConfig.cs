using System.Web.Mvc;

namespace JF.Cloudmarks {

	public class FilterConfig {

		public static void RegisterGlobalFilters( GlobalFilterCollection filters ) {
			filters.Add( new HandleErrorAttribute() );
		}

	}

}
