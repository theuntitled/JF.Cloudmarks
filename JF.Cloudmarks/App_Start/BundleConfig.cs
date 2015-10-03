using System.Web.Optimization;

namespace JF.Cloudmarks {

	public class BundleConfig {

		public static void RegisterBundles( BundleCollection bundles ) {
			bundles.Add( new ScriptBundle( "~/bundles/scripts" ).Include( "~/Content/dist/site.js" ) );

			bundles.Add( new StyleBundle( "~/bundles/styles" ).Include( "~/Content/dist/site.css" ) );
		}

	}

}
