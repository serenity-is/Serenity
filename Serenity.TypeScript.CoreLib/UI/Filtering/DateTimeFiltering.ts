declare namespace Serenity {

    class DateTimeFiltering extends BaseEditorFiltering<DateEditor> {
        getOperators(): Serenity.FilterOperator[];
    }

}