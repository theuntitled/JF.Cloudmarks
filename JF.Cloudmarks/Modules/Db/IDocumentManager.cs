using JF.AspNet.Identity.DocumentDB;
using JF.Azure.DocumentDB;
using JF.Cloudmarks.Modules.Identity;
using JF.Cloudmarks.Modules.Models;

namespace JF.Cloudmarks.Modules.Db {

	public interface IDocumentManager : IIdentityCollectionManager<ApplicationUser> {

		Collection<Directory> Directories { get; set; }

	}

}
