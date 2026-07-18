import { Culture, addLocalText, Authorization } from "../../base";
import { DateFormatter } from "../formatters/dateformatter";
import { DateTimeFormatter } from "../formatters/datetimeformatter";
import { PropertyItemColumnConverter } from "./propertyitemcolumnconverter";

describe('PropertyItemColumnConverter', () => {
    describe('toColumns', () => {
        it('returns empty array for null items', () => {
            const result = PropertyItemColumnConverter.toColumns(null as any);
            expect(result).toEqual([]);
        });

        it('returns empty array for empty items', () => {
            const result = PropertyItemColumnConverter.toColumns([]);
            expect(result).toEqual([]);
        });

        it('converts multiple property items', () => {
            const result = PropertyItemColumnConverter.toColumns([
                { name: 'Field1', title: 'Field 1' },
                { name: 'Field2', title: 'Field 2' }
            ]);
            expect(result).toHaveLength(2);
            expect(result[0].field).toBe('Field1');
            expect(result[1].field).toBe('Field2');
        });
    });

    describe('toColumn', () => {
        it('tries to load a localText with the items name as key', () => {
            var converted = PropertyItemColumnConverter.toColumn({
                name: null,
                title: 'Test.Local.Text.Key'
            });

            expect(converted.name).toBe('Test.Local.Text.Key');

            addLocalText('Test.Local.Text.Key', 'translated');

            var converted2 = PropertyItemColumnConverter.toColumn({
                name: null,
                title: 'Test.Local.Text.Key'
            });

            expect(converted2.name).toBe('translated');
        });

        it('sets field to null and id to name when unbound is true', () => {
            const converted = PropertyItemColumnConverter.toColumn({
                name: 'UnboundField',
                title: 'Unbound',
                unbound: true
            });
            expect(converted.field).toBeNull();
            expect(converted.id).toBe('UnboundField');
        });

        it('hides column when filterOnly is true', () => {
            const converted = PropertyItemColumnConverter.toColumn({
                name: 'f1',
                title: 'Filter Only',
                filterOnly: true
            });
            expect(converted.visible).toBe(false);
            expect(converted.togglable).toBe(false);
        });

        it('hides column when readPermission is not granted', () => {
            vi.spyOn(Authorization, 'hasPermission').mockReturnValue(false);
            const converted = PropertyItemColumnConverter.toColumn({
                name: 'f1',
                title: 'Restricted',
                readPermission: 'Admin'
            });
            expect(converted.visible).toBe(false);
            expect(converted.togglable).toBe(false);
        });

        it('shows column when readPermission is granted', () => {
            vi.spyOn(Authorization, 'hasPermission').mockReturnValue(true);
            const converted = PropertyItemColumnConverter.toColumn({
                name: 'f1',
                title: 'Allowed',
                readPermission: 'User'
            });
            expect(converted.visible).toBe(true);
        });

        it('sets column properties correctly', () => {
            const converted = PropertyItemColumnConverter.toColumn({
                name: 'TestField',
                title: 'Test Field',
                cssClass: 'my-css',
                headerCssClass: 'my-header',
                focusable: false,
                sortable: false,
                showSelection: false,
                tabbable: false,
                sortOrder: 5,
                width: 200,
                minWidth: 50,
                maxWidth: 500,
                resizable: false,
                allowHide: false,
                visible: false,
                summaryType: 'Sum' as any
            });

            expect(converted.field).toBe('TestField');
            expect(converted.cssClass).toBe('my-css');
            expect(converted.headerCssClass).toBe('my-header');
            expect(converted.focusable).toBe(false);
            expect(converted.sortable).toBe(false);
            expect(converted.selectable).toBe(false);
            expect(converted.tabbable).toBe(false);
            expect(converted.sortOrder).toBe(5);
            expect(converted.width).toBe(200);
            expect(converted.minWidth).toBe(50);
            expect(converted.maxWidth).toBe(500);
            expect(converted.resizable).toBe(false);
            expect(converted.togglable).toBe(false);
            expect(converted.visible).toBe(false);
            expect(converted.summaryType).toBe('Sum');
        });

        it('uses default values for undefined properties', () => {
            const converted = PropertyItemColumnConverter.toColumn({
                name: 'f1',
                title: 'Default'
            });

            expect(converted.width).toBe(80);
            expect(converted.minWidth).toBe(30);
            expect(converted.maxWidth).toBeNull();
            expect(converted.resizable).toBe(true);
            expect(converted.sortable).toBe(true);
            expect(converted.focusable).toBe(true);
            expect(converted.visible).toBe(true);
        });

        it('sets alignment in cssClass', () => {
            const converted = PropertyItemColumnConverter.toColumn({
                name: 'f1',
                title: 'Aligned',
                alignment: 'center'
            });
            expect(converted.cssClass).toBe('align-center');
        });

        it('appends alignment to existing cssClass', () => {
            const converted = PropertyItemColumnConverter.toColumn({
                name: 'f1',
                title: 'Aligned',
                cssClass: 'existing',
                alignment: 'right'
            });
            expect(converted.cssClass).toBe('existing align-right');
        });

        it('sets frozen to start when pin is true', () => {
            const converted = PropertyItemColumnConverter.toColumn({
                name: 'f1',
                title: 'Pinned',
                pin: true as any
            });
            expect(converted.frozen).toBe('start');
        });

        it('sets frozen to end when pin is end', () => {
            const converted = PropertyItemColumnConverter.toColumn({
                name: 'f1',
                title: 'Pinned End',
                pin: 'end' as any
            });
            expect(converted.frozen).toBe('end');
        });

        it('returns early when no formatterType', () => {
            const converted = PropertyItemColumnConverter.toColumn({
                name: 'f1',
                title: 'No Formatter'
            });
            expect(converted.format).toBeUndefined();
        });

        it('should pass date formatter to slick column', () => {
            var converted = PropertyItemColumnConverter.toColumn({
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
            var converted = PropertyItemColumnConverter.toColumn({
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

        it('handles formatter with initializeColumn method', () => {
            const mockInitializeColumn = vi.fn();
            class TestFormatter {
                initializeColumn = mockInitializeColumn;
                format(ctx: any) { return 'formatted'; }
            }

            const converted = PropertyItemColumnConverter.toColumn({
                name: 'f1',
                title: 'Init Column',
                formatterType: TestFormatter as any
            });

            expect(mockInitializeColumn).toHaveBeenCalledWith(converted);
            expect(converted.format).toBeDefined();
        });

        it('handles promise-based formatter type', async () => {
            const deferred = Promise.resolve(DateFormatter);

            const converted = PropertyItemColumnConverter.toColumn({
                name: 'f1',
                title: 'Promise Formatter',
                formatterType: deferred as any
            });

            // The format function should return empty string initially
            const ctx = { row: 0, cell: 0, grid: { updateCell: vi.fn() }, value: '2021-01-01' };
            const result = converted.format!(ctx);
            expect(result).toBe('');

            // Wait for the promise to resolve
            await deferred;

            // After resolving, the format should work via formatter
            expect(converted.format).toBeDefined();
        });

        it('handles promise-based formatter without grid context', () => {
            const deferred = Promise.resolve(DateFormatter);

            const converted = PropertyItemColumnConverter.toColumn({
                name: 'f1',
                title: 'Promise No Grid',
                formatterType: deferred as any
            });

            // Format with no row/cell/grid should return empty string and not throw
            const result = converted.format!({} as any);
            expect(result).toBe('');
        });

        it('handles maxWidth of 0 by setting to null', () => {
            const converted = PropertyItemColumnConverter.toColumn({
                name: 'f1',
                title: 'Max Width Zero',
                maxWidth: 0
            });
            expect(converted.maxWidth).toBeNull();
        });

        it('handles allowHide true (togglable stays undefined)', () => {
            const converted = PropertyItemColumnConverter.toColumn({
                name: 'f1',
                title: 'Allow Hide',
                allowHide: true
            });
            expect(converted.togglable).toBeUndefined();
        });
    });
});
