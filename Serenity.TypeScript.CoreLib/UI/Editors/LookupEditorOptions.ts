declare namespace Serenity {
    interface LookupEditorOptions {
        lookupKey?: string;
        minimumResultsForSearch?: any;
        autoComplete?: boolean;
        inplaceAdd?: boolean;
        inplaceAddPermission?: string;
        dialogType?: string;
        cascadeFrom?: string;
        cascadeField?: string;
        cascadeValue?: any;
        filterField?: string;
        filterValue?: any;
        multiple?: boolean;
        delimited?: boolean;
    }
}