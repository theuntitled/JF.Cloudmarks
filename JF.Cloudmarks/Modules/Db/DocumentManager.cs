using JF.AspNet.Identity.DocumentDB;
using JF.Azure.DocumentDB;
using JF.Cloudmarks.Modules.Identity;
using JF.Cloudmarks.Modules.Models;
using Microsoft.Azure.Documents.Client;

namespace JF.Cloudmarks.Modules.Db {

	public class DocumentManager : IdentityCollectionManager<ApplicationUser> , IDocumentManager {

		public DocumentManager( DocumentClient documentClient , string databaseId , bool createDatabaseIfNonexistent = false )
			: base( documentClient , databaseId , createDatabaseIfNonexistent ) {
		}

		public Collection<Directory> Directories { get; set; }

	}

}
