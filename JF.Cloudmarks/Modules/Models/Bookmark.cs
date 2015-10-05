using System;
using System.Collections.Generic;

namespace JF.Cloudmarks.Modules.Models {

	public class Bookmark {

		public string Uri { get; set; }

		public string Icon { get; set; }

		public string IconUrl { get; set; }

		public string Description { get; set; }

		public DateTime? Created { get; set; }

		public DateTime? LastUpdate { get; set; }

		public List<string> Tags { get; set; } = new List<string>();

	}

}
