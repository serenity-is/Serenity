declare namespace Serenity {

    class CheckTreeEditor<TItem, TOptions> extends DataGrid<TItem, TOptions> {
        constructor(input: JQuery, opt?: TOptions);
        protected getTreeItems(): TItem[];
        protected updateItems(): void;
        protected itemSelectedChanged(item: TItem): void;
        protected getSelectAllText(): string;
        protected isThreeStateHierarchy(): boolean;
        protected getInitialCollapse(): boolean;
        protected updateSelectAll(): void;
        protected updateFlags(): void;
        protected getDescendantsSelected(item: TItem): boolean;
        protected allDescendantsSelected(item: TItem): boolean;
        protected getItemText(ctx: Slick.FormatterContext): string;
        protected sortItems(): void;
        protected moveSelectedUp(): boolean;
        public value: string[];
    }

}