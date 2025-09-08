import { Culture, addLocalText, getTypeFullName } from "../../base";
import { DateFormatter, DateTimeFormatter } from "../formatters/formatters";
import { PropertyItemSlickConverter } from "./propertyitemslickconverter";

describe('PropertyItemSlickConverter.toSlickColumn', () => {
    it('tries to load a localText with the items name as key', () => {
        var converted = PropertyItemSlickConverter.toSlickColumn({
            name: null,
            title: 'Test.Local.Text.Key'
        });

        expect(converted.name).toBe('Test.Local.Text.Key');

        addLocalText('Test.Local.Text.Key', 'translated');

        var converted2 = PropertyItemSlickConverter.toSlickColumn({
            name: null,
            title: 'Test.Local.Text.Key'
        });

        expect(converted2.name).toBe('translated');
    });

    it('should pass date formatter to slick formatter', () => {
        var converted = PropertyItemSlickConverter.toSlickColumn({
            name: null,
            title: 'Test.Local.Text.Key',
            formatterType: DateFormatter,
        });

        Culture.dateSeparator = '/';
        Culture.dateOrder = 'dmy';
        Culture.dateFormat = 'dd/MM/yyyy';
        Culture.dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';

        expect(converted.format).toBeDefined();
        var formattedDate = converted.format.call(null, { value: '2021-01-01T00:00:00' });
        expect(formattedDate).toBe('01/01/2021');
    });


    it('should pass date time formatter parameters to slick formatter', () => {
        var converted = PropertyItemSlickConverter.toSlickColumn({
            name: null,
            title: 'Test.Local.Text.Key',
            formatterType: DateTimeFormatter,
            formatterParams: {
                displayFormat: 'g'
            }
        });

        Culture.dateSeparator = '/';
        Culture.dateOrder = 'dmy';
        Culture.dateFormat = 'dd/MM/yyyy';
        Culture.dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';

        expect(converted.format).toBeDefined();
        var formattedDate = converted.format.call(null, { value: '2021-01-01T00:00:00' });
        expect(formattedDate).toBe('01/01/2021 00:00');
    });
});
