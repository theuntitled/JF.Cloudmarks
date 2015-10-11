using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using JF.Cloudmarks.Modules.Db;
using JF.Cloudmarks.Modules.Models;
using JF.Cloudmarks.Modules.Parser;
using JF.Cloudmarks.Modules.ViewModels.Bookmark;
using Microsoft.AspNet.Identity;
using Directory = JF.Cloudmarks.Modules.Models.Directory;

namespace JF.Cloudmarks.Controllers {

	[Authorize]
	[RoutePrefix( "bookmarks" )]
	public class BookmarksController : Controller {

		private readonly INetscapeBookmarkParser _bookmarkParser;

		private readonly IDocumentManager _documentManager;

		public BookmarksController( IDocumentManager documentManager , INetscapeBookmarkParser bookmarkParser ) {
			_documentManager = documentManager;
			_bookmarkParser = bookmarkParser;
		}

		#region index

		public ActionResult Index() {
			return View( GetCurrentDirectory() );
		}

		#endregion

		#region stash

		[Route( "stash" )]
		public ActionResult Stash() {
			var userId = User.Identity.GetUserId();

			var directories =
				_documentManager.Directories.AsQueryable().Where( item => item.CreatedById == userId && item.IsTemporary && !item.IsDeleted ).ToList();

			return View( directories );
		}

		[ChildActionOnly]
		public PartialViewResult StashLink() {
			var userId = User.Identity.GetUserId();

			var directoryCount =
				_documentManager.Directories.AsQueryable()
								.Where( item => item.CreatedById == userId && item.IsTemporary )
								.AsEnumerable()
								.Count();

			return PartialView( directoryCount );
		}

		#endregion

		#region import

		[Route( "import" )]
		public ActionResult Import() {
			return View();
		}

		[HttpPost]
		[Route( "import" )]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Import( Import model ) {
			if ( !ModelState.IsValid ) {
				return Json( new {
					success = false
				} );
			}

			using ( var streamReader = new StreamReader( model.File.InputStream ) ) {
				var fileContents = streamReader.ReadToEnd();

				var directory = _bookmarkParser.ReadFile( fileContents );

				directory.IsTemporary = true;
				directory.CreatedById = User.Identity.GetUserId();
				directory.CreatedByName = User.Identity.GetUserName();

				var result = await _documentManager.Directories.AddOrUpdateAsync( directory );

				return Json( new {
					success = true ,
					redirectUri = Url.Action( "ImportStepTwo" ,
											  new {
												  id = result.Resource.Id
											  } )
				} );
			}
		}

		[Route( "import/step-two/{id}" )]
		public async Task<ActionResult> ImportStepTwo( string id ) {
			var directory = await _documentManager.Directories.FindAsync( id );

			return View( directory );
		}

		[HttpPost]
		[Route( "import/step-three" )]
		public async Task<ActionResult> ImportStepThree( StepThreeRequest request ) {
			var rootDirectory = await _documentManager.Directories.FindAsync( request.Id );

			rootDirectory.Bookmarks = Match( rootDirectory.Bookmarks , request.Selection.Bookmarks );
			rootDirectory.Directories = Match( rootDirectory.Directories , request.Selection.Directories );

			var current = GetCurrentDirectory();

			if ( current == null ) {
				rootDirectory.DeletedAt = null;
				rootDirectory.IsDeleted = false;
				rootDirectory.IsTemporary = false;

				await _documentManager.Directories.AddOrUpdateAsync( rootDirectory );
			} else {
				current.Directories = Merge( current.Directories , rootDirectory.Directories );
				current.Bookmarks = Merge( current.Bookmarks , rootDirectory.Bookmarks );

				await _documentManager.Directories.AddOrUpdateAsync( current );

				await _documentManager.Directories.RemoveAsync( rootDirectory );
			}

			return Json( new {
				success = true ,
				redirectUri = Url.Action( "Index" )
			} );
		}

		#endregion

		#region delete

		[Route( "remove-permanently/{source}/{id}" )]
		public async Task<ActionResult> DeletePermanently( string source , string id ) {
			await _documentManager.Directories.RemoveAsync( id );

			switch ( source ) {
				case "stash":
					return RedirectToAction( "Stash" );
				case "recycle-bin":
					return RedirectToAction( "RecycleBin" );
			}

			return RedirectToAction( "Index" );
		}

		#endregion

		#region export

		[Route( "export" )]
		public ActionResult Export() {
			// TODO: Export data to an <!DOCTYPE NETSCAPE-Bookmark-file-1> file

			return View();
		}

		#endregion

		#region recycle

		[Route( "recycle-bin" )]
		public ActionResult RecycleBin() {
			var userId = User.Identity.GetUserId();

			var directories =
				_documentManager.Directories.AsQueryable().Where( item => item.CreatedById == userId && !item.IsTemporary && item.IsDeleted ).ToList();

			return View( directories );
		}

		[Route( "recycle/all" )]
		public async Task<ActionResult> RecycleAll() {
			var current = GetCurrentDirectory();

			current.IsDeleted = true;
			current.DeletedAt = DateTime.Now;

			await _documentManager.Directories.AddOrUpdateAsync( current );

			return RedirectToAction( "Index" );
		}

		[ChildActionOnly]
		public PartialViewResult RecycleLink() {
			var userId = User.Identity.GetUserId();

			var directoryCount =
				_documentManager.Directories.AsQueryable()
								.Where( item => item.CreatedById == userId && item.IsDeleted )
								.AsEnumerable()
								.Count();

			return PartialView( directoryCount );
		}

		#endregion

		#region internals

		private Directory GetCurrentDirectory() {
			var userId = User.Identity.GetUserId();

			return _documentManager.Directories.AsQueryable()
								.Where( item => item.CreatedById == userId && !item.IsTemporary && !item.IsDeleted )
								.AsEnumerable().FirstOrDefault();
		}

		private List<Directory> Merge( List<Directory> directories , List<Directory> selectedDirectories ) {
			foreach ( var selectedDirectory in selectedDirectories ) {
				var existingDirectory = directories.FirstOrDefault( item => item.Name == selectedDirectory.Name );

				if ( existingDirectory == null ) {
					directories.Add( selectedDirectory );

					continue;
				}

				existingDirectory.Bookmarks = Merge( existingDirectory.Bookmarks , selectedDirectory.Bookmarks );
				existingDirectory.Directories = Merge( existingDirectory.Directories , selectedDirectory.Directories );
			}

			return directories;
		}

		private List<Bookmark> Merge( List<Bookmark> bookmarks , List<Bookmark> selectedBookmarks ) {
			foreach ( var selectedBookmark in selectedBookmarks ) {
				if ( !bookmarks.Any( item => item.Uri == selectedBookmark.Uri && item.Description == selectedBookmark.Description ) ) {
					bookmarks.Add( selectedBookmark );
				}
			}

			return bookmarks;
		}

		private List<Directory> Match( List<Directory> directories , List<SelectedDirectory> selectedDirectories ) {
			var resolvedItems = new List<Directory>();

			if ( !directories.Any() ) {
				return resolvedItems;
			}

			foreach ( var directory in directories ) {
				if ( selectedDirectories.Any( item => item.Id == directory.Id ) ) {
					var selectedDirectory = selectedDirectories.First( item => item.Id == directory.Id );

					directory.Directories = Match( directory.Directories , selectedDirectory.Directories );

					directory.Bookmarks = Match( directory.Bookmarks , selectedDirectory.Bookmarks );

					resolvedItems.Add( directory );
				}
			}

			return resolvedItems;
		}

		private List<Bookmark> Match( List<Bookmark> bookmarks , List<string> selectedBookmarks ) {
			var resolvedItems = new List<Bookmark>();

			if ( !bookmarks.Any() ) {
				return resolvedItems;
			}

			resolvedItems.AddRange( bookmarks.Where( bookmark => selectedBookmarks.Any( item => item == bookmark.Id ) ) );

			return resolvedItems;
		}

		#endregion

	}

}
