
module Site.Bookmarks {
	
	export class DirectoryTree {
		
		rootNode: JQuery;

		protected iconSelector: string = "i.folder.icon";

		public constructor(rootNode: JQuery) {
			this.rootNode = rootNode;

			this.rootNode.find(this.iconSelector).click($.proxy(this.handleNodeClick, this));
		}

		protected handleNodeClick(event: Event): void {
			var target = $(event.target);

			var list = target.parent(".item").find(".list").first();

			if (list.is(".hidden")) {
				list.removeClass("transition hidden");

				target.removeClass("closed").addClass("open");
			} else {
				list.addClass("transition hidden");

				target.addClass("closed").removeClass("open");
			}

		}

	}

}
