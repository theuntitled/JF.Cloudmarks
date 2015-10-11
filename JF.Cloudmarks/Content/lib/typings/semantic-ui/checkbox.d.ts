
declare module SemanticUI {

	export interface CheckboxOptions {

		uncheckable?: string | boolean;
		fireOnInit?: boolean;

		onChange?(callback: () => void): void;
		onChecked?(callback: () => void): void;
		onIndeterminate?(callback: () => void): void;
		onDeterminate?(callback: () => void): void;
		onUnchecked?(callback: () => void): void;
		beforeChecked?(callback: () => void): void;
		beforeIndeterminate?(callback: () => void): void;
		beforeDeterminate?(callback: () => void): void;
		beforeUnchecked?(callback: () => void): void;
		onEnable?(callback: () => void): void;
		onDisable?(callback: () => void): void;

		name?: string;
		namespace?: string;
		debug?: boolean;
		performance?: boolean;
		verbose?: boolean;

		selector?: {

			input?: string;
			label?: string;

		};

		className?: {

			checked?: string;
			disabled?: string;
			radio?: string;
			readOnly?: string;

		};

		errors?: {

			method?: string;

		};

	}

}

interface JQueryStatic {

	checkbox(method: string): JQuery;
	checkbox(options?: SemanticUI.CheckboxOptions): JQuery;

}

interface JQuery {

	checkbox(method: string): JQuery;
	checkbox(options?: SemanticUI.CheckboxOptions): JQuery;

}
