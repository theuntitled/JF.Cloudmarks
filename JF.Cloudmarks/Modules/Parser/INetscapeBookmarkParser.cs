using JF.Cloudmarks.Modules.Models;

namespace JF.Cloudmarks.Modules.Parser {

	public interface INetscapeBookmarkParser {

		Directory ReadFile( string contents );

	}

}
