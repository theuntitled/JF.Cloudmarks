using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;

namespace JF.Cloudmarks.Modules.Models {

	public class Directory : Resource {

		public string Name { get; set; }

		public bool IsTemporary { get; set; }

		public DateTime LastUpdate { get; set; }

		public string CreatedById { get; set; }

		public string CreatedByName { get; set; }

		public List<Bookmark> Bookmarks { get; set; }

		public List<Directory> Directories { get; set; }

	}

}
