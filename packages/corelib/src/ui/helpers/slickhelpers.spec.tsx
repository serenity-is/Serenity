import { escapeHtml } from "@serenity-is/sleekgrid";
import { Culture, addLocalText } from "../../base";
import { PropertyItemSlickConverter, SlickFormatting } from "./slickhelpers";

describe('SlickHelpers.toSlickColumn', () => {
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
            formatterType: 'Serenity.DateFormatter'
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
            formatterType: 'Serenity.DateTimeFormatter',
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

describe('SlickFormatting.itemLink', () => {
    it('should return a link wrapping the HTML element returned from the formatter', () => {
        const el = <span class="test-class">test&text</span> as HTMLElement;
        const format = SlickFormatting.itemLink("sample", "testid", () => el);
        const result = format({ escape: escapeHtml, item: { testid: 1 }, value: 'test' });
        expect(result instanceof HTMLAnchorElement).toBe(true);
        expect((result as HTMLAnchorElement).firstChild).toBe(el);
    });


    it('should return a link wrapping the fragment elements returned from the formatter', () => {
        const span = <span class="test-class">test&text</span>;
        const icon = <i class="test-icon" />;
        const format = SlickFormatting.itemLink("sample", "testid", () => <>{span}{icon}</>);
        const result = format({ escape: escapeHtml, item: { testid: 1 }, value: 'test' });
        expect(result instanceof HTMLAnchorElement).toBe(true);
        expect((result as HTMLAnchorElement).firstChild).toBe(span);
        expect((result as HTMLAnchorElement).lastChild).toBe(icon);
    });
});