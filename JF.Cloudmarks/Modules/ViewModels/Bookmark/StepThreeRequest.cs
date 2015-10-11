using Newtonsoft.Json;

namespace JF.Cloudmarks.Modules.ViewModels.Bookmark {

	public class StepThreeRequest {

		[JsonProperty( "id" )]
		public string Id { get; set; }

		[JsonProperty( "selection" )]
		public SelectedDirectory Selection { get; set; }

	}

}
