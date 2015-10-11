
module Site.Bookmarks.Import {

	export interface INode {

		id: string;
		bookmarks: string[];
		directories: INode[];

	}

	export class Save {

		id: string;
		submitUri: string;
		formElement: JQuery;
		submitButton: JQuery;

		handleCheckboxChange: boolean;

		public constructor(formElement: JQuery, submitUri: string, id: string) {
			this.id = id;
			this.submitUri = submitUri;
			this.formElement = formElement;

			this.formElement.find(".ui.checkbox").checkbox();

			this.handleCheckboxChange = true;

			this.formElement.find("input[type=checkbox]").change((event: Event) => {
				this.onCheckboxChange(event);
			});

			this.submitButton = this.formElement.find("button[data-save]");

			this.submitButton.click($.proxy(this.handleSubmit, this));
		}

		protected onCheckboxChange(event: Event): void {
			const target = $(event.target);
			if (!this.handleCheckboxChange) {
				return;
			}
			let method = "uncheck";
			if (target.is(":checked")) {
				method = "check";

				this.handleCheckboxChange = false;

				$.each(target.parents(".item"), (index: string, value: string) => {
					$(value).find(".checkbox").first().checkbox("check");
				});

				this.handleCheckboxChange = true;
			}

			if (!target.is(".dir")) {
				return;
			}

			target.parents(".item").first().find(".list").first().find(".checkbox").checkbox(method);
		}

		protected scanDirectory(node: JQuery, id: string): INode {
			var data: INode = {
				id: id,
				bookmarks: [],
				directories: []
			};

			$.each(node.children("[data-type=directory]"), (index: string, value: HTMLElement) => {
				var target = $(value);

				if (this.isChecked(target)) {
					data.directories.push(this.scanDirectory(target.find(".list").first(), this.getId(target)));
				}
			});

			$.each(node.children("[data-type=bookmark]"), (index: string, value: string) => {
				var target = $(value);

				if (this.isChecked(target)) {
					data.bookmarks.push(this.getId(target));
				}
			});

			return data;
		}

		protected getCheckbox(element: JQuery): JQuery {
			return element.find("input[type=checkbox]").first();
		}

		protected getId(element: JQuery): string {
			return this.getCheckbox(element).data("id");
		}

		protected isChecked(element: JQuery): boolean {
			return this.getCheckbox(element).is(":checked");
		}

		protected handleSubmit(event: Event): void {
			event.preventDefault();

			this.submitButton.addClass("loading");

			$.ajax(this.submitUri, {
				method: "POST",
				data: {
					id: this.id,
					selection: this.scanDirectory(this.formElement.find("[data-tree]"), null)
				},
				success: (response: any) => {
					if (response.success) {
						window.location.href = response.redirectUri;
					}
				}
			});
		}

	}

}