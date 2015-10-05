using System;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using JF.Cloudmarks.Modules.Models;

namespace JF.Cloudmarks.Modules.Parser {

	public class NetscapeBookmarkParser : INetscapeBookmarkParser {

		protected readonly Regex DirectoryCloseRegex = new Regex( "</dl><p>" ,
																  RegexOptions.Compiled | RegexOptions.IgnoreCase );

		protected readonly Regex DirectoryNameRegex = new Regex( "<dt><h3" ,
																 RegexOptions.Compiled | RegexOptions.IgnoreCase );

		protected readonly Regex DirectoryOpenRegex = new Regex( "<dl><p>" ,
																 RegexOptions.Compiled | RegexOptions.IgnoreCase );

		protected readonly Regex LinkOpenRegex = new Regex( "<dt><a" , RegexOptions.Compiled | RegexOptions.IgnoreCase );

		public Directory ReadFile( string contents ) {
			var lines = contents.Split( new[] {"\r\n" , "\n"} , StringSplitOptions.None ).ToList();

			if ( !lines.First().Contains( "<!DOCTYPE NETSCAPE-Bookmark-file-1>" ) ) {
				throw new NotSupportedException( "The provided document is not a netscape bookmarks file." );
			}

			var startIndex = lines.FindIndex( line => line == "<DL><p>" );
			var endIndex = lines.LastIndexOf( "</DL><p>" ) + 1;

			lines = lines.Skip( startIndex ).Take( endIndex - startIndex ).Select( PrepareLine ).ToList();

			var document = HtmlNode.CreateNode( string.Join( string.Empty , lines ) );

			return ReadNode( "Root" , document.ChildNodes );
		}

		protected string PrepareLine( string line ) {
			line = line.Trim();

			line = LinkOpenRegex.Replace( line , "<a" );
			line = DirectoryNameRegex.Replace( line , "<div><h3" );
			line = DirectoryOpenRegex.Replace( line , "<dl>" );
			line = DirectoryCloseRegex.Replace( line , "</dl></div>" );

			return line;
		}

		protected Directory ReadNode( string directoryName , HtmlNodeCollection childNodes ) {
			var directory = new Directory {
				Name = directoryName
			};

			foreach ( var childNode in childNodes ) {
				if ( childNode.Name == "div" ) {
					directory.Directories.Add( ReadNode( childNode.ChildNodes[0].InnerText , childNode.ChildNodes[1].ChildNodes ) );
				}

				if ( childNode.Name == "a" ) {
					directory.Bookmarks.Add( new Bookmark {
						Description = childNode.InnerText ,
						Created = RetrieveToDateTime( childNode , "Add_date" ) ,
						Uri = childNode.GetAttributeValue( "href" , string.Empty ) ,
						Icon = childNode.GetAttributeValue( "icon" , string.Empty ) ,
						LastUpdate = RetrieveToDateTime( childNode , "last_modified" ) ,
						IconUrl = childNode.GetAttributeValue( "icon_uri" , string.Empty )
					} );
				}
			}

			return directory;
		}

		protected DateTime? RetrieveToDateTime( HtmlNode node , string attribute ) {
			var value = node.GetAttributeValue( attribute , string.Empty );

			if ( string.IsNullOrEmpty( value ) ) {
				return null;
			}

			var epoch = new DateTime( 1970 , 1 , 1 , 0 , 0 , 0 );

			return epoch.AddSeconds( double.Parse( value ) );
		}

	}

}
