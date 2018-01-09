
namespace Q {
    export namespace Culture {
        export let decimalSeparator = '.';
        export let dateSeparator = '/';
        export let dateOrder = 'dmy';
        export let dateFormat = 'dd/MM/yyyy';
        export let dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';

        export function get_groupSeparator(): string {
            return ((decimalSeparator === ',') ? '.' : ',');
        };

        var s = Q.trimToNull($('script#ScriptCulture').html());
        if (s != null) {
            var sc = $.parseJSON(s);
            if (sc.DecimalSeparator != null)
                decimalSeparator = sc.DecimalSeparator;
            if (sc.DateSeparator != null)
                dateSeparator = sc.DateSeparator;
            if (sc.DateOrder != null)
                dateOrder = sc.DateOrder;
            if (sc.DateFormat != null)
                dateFormat = sc.DateFormat;
            if (sc.DateTimeFormat != null)
                dateTimeFormat = sc.DateTimeFormat;
        }
    }
}