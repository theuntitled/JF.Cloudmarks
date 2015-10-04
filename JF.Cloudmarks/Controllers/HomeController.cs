using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using JF.Cloudmarks.Modules.Db;
using JF.Cloudmarks.Modules.Models;
using JF.Cloudmarks.Modules.ViewModels.Bookmark;
using Microsoft.AspNet.Identity;
using Directory = JF.Cloudmarks.Modules.Models.Directory;

namespace JF.Cloudmarks.Controllers {

	[Authorize]
	[RoutePrefix( "Bookmarks" )]
	public class HomeController : Controller {

		private readonly IDocumentManager _documentManager;

		public HomeController( IDocumentManager documentManager ) {
			_documentManager = documentManager;
		}

		public ActionResult Index() {
			return View();
		}

		[Route( "import" )]
		public ActionResult Import() {
			return View();
		}

		[HttpPost]
		[Route( "import" )]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Import( Import model ) {
			if ( !ModelState.IsValid ) {
				return View();
			}

			using ( var streamReader = new StreamReader( model.File.InputStream ) ) {
				var fileContents = streamReader.ReadToEnd();

				var directory = new Directory {
					Name = "Import" ,
					IsTemporary = true ,
					LastUpdate = DateTime.Now ,
					Bookmarks = new List<Bookmark>() ,
					Directories = new List<Directory>() ,
					CreatedById = User.Identity.GetUserId() ,
					CreatedByName = User.Identity.GetUserName()
				};

				// TODO: Read file contents and push it to the database as a temporary record

				var result = await _documentManager.Directories.AddOrUpdateAsync( directory );

				return RedirectToAction( "ImportStepTwo" ,
										 new {
											 id = result.Resource.Id
										 } );
			}
		}

		[Route( "import/step-two/{id}" )]
		public async Task<ActionResult> ImportStepTwo( string id ) {
			var directory = await _documentManager.Directories.FindAsync( id );

			// TODO: Display data and allow for user reordering and tagging

			// TODO: Save the data via an API?

			return View( directory );
		}

		public ActionResult Export() {
			// TODO: Export data to an <!DOCTYPE NETSCAPE-Bookmark-file-1> file

			return View();
		}

	}

}
