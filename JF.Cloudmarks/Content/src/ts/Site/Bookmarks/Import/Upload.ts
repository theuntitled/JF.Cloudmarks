
module Site.Bookmarks.Import {

	interface IImportResponse {
		
		success: boolean;
		redirectUri?: string;

	}

	export class Upload {
		
		button: JQuery;
		dimmer: JQuery;
		progress: JQuery;
		uploadUri: string;
		formElement: JQuery;

		protected formData: FormData;
		protected xhrRequest: XMLHttpRequest;

		public constructor(formElement: JQuery, progress: JQuery, dimmer: JQuery) {
			this.progress = progress;

			this.dimmer = dimmer;
			this.formElement = formElement;
			this.uploadUri = this.formElement.attr("action");
			this.button = this.formElement.find("button[type=submit]");

			this.button.click($.proxy(this.upload, this));
		}

		protected getFileInput(): HTMLInputElement {
			return <HTMLInputElement>this.formElement.find("input[name=File]")[0];
		}

		protected upload(event: Event): void {
			event.preventDefault();

			this.formElement.transition({
				animation: "fade down",
				onComplete: () => {
					this.progress.transition({
						animation: "fade up"
					});
				}
			});

			this.xhrRequest = new XMLHttpRequest();
			this.formData = new FormData();

			this.formData.append("File", this.getFileInput().files[0]);
			this.formData.append("__RequestVerificationToken", this.formElement.find("input[name=__RequestVerificationToken]").val());
			
			this.xhrRequest.open("POST", this.uploadUri, true);
			
			this.xhrRequest.upload.onprogress = $.proxy(this.uploadOnprogress, this);
			this.xhrRequest.upload.onloadend = $.proxy(this.uploadOnloadend, this);
			this.xhrRequest.onloadend = $.proxy(this.onloadend, this);

			this.xhrRequest.send(this.formData);
		}

		protected setUploadPercent(percent: number): void {
			this.progress.attr("data-percent", percent);
			this.progress.find(".bar").width(percent + "%");
			this.progress.find(".label").text(percent + "% uploaded");
		}

		protected uploadOnprogress(event: Event): void {
			console.log("uploadOnprogress");
			this.setUploadPercent((100 / (<any>event).total) * (<any>event).loaded);
		}

		protected uploadOnloadend(event: Event): void {
			this.setUploadPercent(100);
		}

		protected onloadend(event: Event): void {
			this.dimmer.addClass("active");

			var response = <IImportResponse>JSON.parse(this.xhrRequest.responseText);

			if (response.success) {
				window.location.href = response.redirectUri;

				return;
			}
		}


	}

}
