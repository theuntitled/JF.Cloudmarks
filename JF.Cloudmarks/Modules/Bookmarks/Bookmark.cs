using System.Collections.Generic;
using Microsoft.Azure.Documents;

namespace JF.Cloudmarks.Modules.Bookmarks {

	public class Bookmark : Resource {

		public string UserId { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public string Uri { get; set; }

		public List<string> Tags { get; set; }

	}

}
