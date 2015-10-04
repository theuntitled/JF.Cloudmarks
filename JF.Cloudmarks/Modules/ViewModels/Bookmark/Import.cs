using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using JF.Cloudmarks.Modules.Attributes;

namespace JF.Cloudmarks.Modules.ViewModels.Bookmark {

	public class Import {

		[Required]
		[MaxFileSize( 2 )]
		[DisplayName( "Bookmarks File" )]
		[ValidFileExtensions( Extensions = "htm,html" )]
		public HttpPostedFileBase File { get; set; }

	}

}
