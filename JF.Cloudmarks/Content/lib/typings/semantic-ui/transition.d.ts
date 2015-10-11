
declare module SemanticUI {

	export interface TransitionOptions {

		animation?: string;
		interval?: number;
		reverse?: string;
		displayType?: string;
		duration?: string;
		useFailSafe?: boolean;
		allowRepeats?: boolean;
		queue?: boolean;

		onShow?(callback: () => void): void;
		onHide?(callback: () => void): void;
		onStart?(callback: () => void): void;
		onComplete?(callback: () => void): void;

	}

}

interface JQueryStatic {

	transition(options?: SemanticUI.TransitionOptions): JQuery;
	
}

interface JQuery {

	transition(options?: SemanticUI.TransitionOptions): JQuery;
	
}
