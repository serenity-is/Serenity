declare namespace Serenity {

    class LookupFiltering extends BaseEditorFiltering<LookupEditor> {
        getOperators(): Serenity.FilterOperator[];
    }

}