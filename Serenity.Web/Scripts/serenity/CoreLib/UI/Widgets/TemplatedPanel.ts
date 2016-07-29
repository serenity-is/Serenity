declare namespace Serenity {
    class TemplatedPanel<TOptions> extends TemplatedWidget<TOptions> {
        constructor(container: JQuery, options?: TOptions);
        protected tabs: JQuery;
        protected toolbar: Serenity.Toolbar;
        protected validator: JQueryValidation.Validator;
        protected arrange(): void;
        protected isPanel: boolean;
        protected responsive: boolean;
        protected arrange(): void;
        protected getToolbarButtons(): ToolButton[];
        protected getValidatorOptions(): JQueryValidation.ValidationOptions;
        protected initTabs(): void;
        protected initToolbar(): void;
        protected initValidator(): void;
        protected resetValidation(): void;
        protected validateForm(): boolean;
    }
}