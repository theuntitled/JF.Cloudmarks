using System.Collections.Generic;
using Newtonsoft.Json;

namespace JF.Cloudmarks.Modules.ViewModels.Bookmark {

	public class SelectedDirectory {

		[JsonProperty( "id" )]
		public string Id { get; set; }

		[JsonProperty( "bookmarks" )]
		public List<string> Bookmarks { get; set; }

		[JsonProperty( "directories" )]
		public List<SelectedDirectory> Directories { get; set; }

	}

}
