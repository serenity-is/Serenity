declare namespace Serenity {

    class FilterWidgetBase<TOptions> extends TemplatedWidget<TOptions> {
        constructor(div: JQuery, opt: any);
        filterStoreChanged(): void;
        get_store(): FilterStore;
        set_store(value: FilterStore): void;
    }

}