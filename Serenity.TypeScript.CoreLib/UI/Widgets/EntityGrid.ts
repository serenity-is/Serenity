declare namespace Serenity {
    class EntityGrid<TItem, TOptions> extends DataGrid<TItem, TOptions> {
        constructor(container: JQuery, options?: TOptions);
        protected addButtonClick(): void;
        protected createEntityDialog(itemType: string, callback?: (dlg: Widget<any>) => void): Widget<any>;
        protected getDialogOptions(): any;
        protected getDialogOptionsFor(itemType: string): any;
        protected getDialogType(): { new (...args: any[]): Widget<any> };
        protected getDialogTypeFor(itemType: string): { new (...args: any[]): Widget<any> };
        protected getDisplayName(): string;
        protected getItemName(): string;
        protected getEntityType(): string;
        protected getService(): string;
        protected initDialog(dialog: Widget<any>): void;
        protected initEntityDialog(itemType: string, dialog: Widget<any>): void;
        protected newRefreshButton(noText?: boolean): ToolButton;
    }
}