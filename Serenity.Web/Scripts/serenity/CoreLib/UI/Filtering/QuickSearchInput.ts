declare namespace Serenity {

    class QuickSearchInput extends Widget<QuickSearchInputOptions> {
        constructor(input: JQuery, opt: QuickSearchInputOptions);
        checkIfValueChanged(): void;
        get_field(): QuickSearchField;
        set_field(value: QuickSearchField): void;
    }

    interface QuickSearchInputOptions {
        typeDelay?: number;
        loadingParentClass?: string;
        filteredParentClass?: string;
        onSearch?: (p1: string, p2: string, p3: (p1: boolean) => void) => void;
        fields?: QuickSearchField[];
    }

}