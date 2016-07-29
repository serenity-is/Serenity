declare namespace Serenity {
    class PropertyPanel<TItem, TOptions> extends TemplatedPanel<TOptions> {
        constructor(container: JQuery, options?: TOptions);
        protected getFormKey(): string;
        protected getPropertyGridOptions(): PropertyGridOptions;
        protected getPropertyGridOptionsAsync(): PromiseLike<PropertyGridOptions>;
        protected getPropertyItems(): PropertyItem[];
        protected getPropertyItemsAsync(): PromiseLike<PropertyItem[]>;
        protected getSaveEntity(): TItem;
        protected get_entity(): TItem;
        protected get_entityId(): any;
        protected initializeAsync(): PromiseLike<void>;
        protected loadInitialEntity(): void;
        protected set_entity(entity: TItem): void;
        protected set_entityId(value: any): void;
        protected validateBeforeSave(): boolean;
        protected propertyGrid: Serenity.PropertyGrid;
    }
}