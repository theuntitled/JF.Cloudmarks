using System.Collections.Generic;
using JF.AspNet.Identity.DocumentDB;

namespace JF.Cloudmarks.Modules.Identity {

	internal class ApplicationRoleStore : IdentityRoleStore {

		public ApplicationRoleStore() {
			Roles = new List<string> {
				"Administrator"
			};
		}

	}

}
