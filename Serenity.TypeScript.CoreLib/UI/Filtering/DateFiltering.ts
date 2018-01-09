declare namespace Serenity {

    class DateFiltering extends BaseEditorFiltering<DateEditor> {
        getOperators(): Serenity.FilterOperator[];
    }

}