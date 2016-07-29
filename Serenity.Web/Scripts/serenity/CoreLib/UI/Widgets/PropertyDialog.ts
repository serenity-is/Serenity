declare namespace Serenity {
    class PropertyDialog<TItem, TOptions> extends TemplatedDialog<TOptions> {
        constructor(options?: TOptions);
        protected entity: TItem;
        protected entityId: any;
        protected getFormKey(): string;
        protected getPropertyGridOptions(): PropertyGridOptions;
        protected getPropertyGridOptionsAsync(): PromiseLike<PropertyGridOptions>;
        protected getPropertyItems(): PropertyItem[];
        protected getPropertyItemsAsync(): PromiseLike<PropertyItem[]>;
        protected getSaveEntity(): TItem;
        protected initializeAsync(): PromiseLike<void>;
        protected loadInitialEntity(): void;
        protected set_entity(entity: TItem): void;
        protected set_entityId(value: any): void;
        protected validateBeforeSave(): boolean;
        protected propertyGrid: Serenity.PropertyGrid;
    }
}