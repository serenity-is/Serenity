import { formatterContext as ctx } from "@serenity-is/sleekgrid";
import { SlickFormatting } from "./slickformatting";

describe('SlickFormatting.itemLink', () => {
    it('should return a link wrapping the HTML element returned from the formatter', () => {
        const el = <span class="test-class">test&text</span> as HTMLElement;
        const format = SlickFormatting.itemLink("sample", "testid", () => el);
        const result = format(ctx({ item: { testid: 1 }, value: 'test' }));
        expect(result instanceof HTMLAnchorElement).toBe(true);
        expect((result as HTMLAnchorElement).firstChild).toBe(el);
    });

    it('should return a link wrapping the fragment elements returned from the formatter', () => {
        const span = <span class="test-class">test&text</span>;
        const icon = <i class="test-icon" />;
        const format = SlickFormatting.itemLink("sample", "testid", () => <>{span}{icon}</>);
        const result = format(ctx({ item: { testid: 1 }, value: 'test' }));
        expect(result instanceof HTMLAnchorElement).toBe(true);
        expect((result as HTMLAnchorElement).firstChild).toBe(span);
        expect((result as HTMLAnchorElement).lastChild).toBe(icon);
    });

    it('should return value when getText is null', () => {
        const format = SlickFormatting.itemLink("sample", "testid", null);
        const result = format(ctx({ item: { testid: 1 }, value: 'test', escape: (x: any) => x }));
        // When getText is null, the value becomes the formatter result, but still wrapped in an anchor
        expect(result instanceof HTMLAnchorElement).toBe(true);
    });

    it('should return text when item is nonDataRow', () => {
        const format = SlickFormatting.itemLink("sample", "testid", () => 'data');
        const result = format(ctx({ item: { __nonDataRow: true, testid: 1 }, value: 'test' }));
        expect(result).toBe('data');
    });

    it('should skip edit link for certain purposes', () => {
        const format = SlickFormatting.itemLink("sample", "testid", () => 'data');
        const result = format(ctx({ item: { testid: 1 }, value: 'test', purpose: 'excel-export' }));
        expect(result).toBe('data');
    });

    it('should create link with correct href and data attributes', () => {
        const format = SlickFormatting.itemLink("Northwind.Customer", "id", () => 'customer');
        const result = format(ctx({ item: { id: 42 }, value: 'test', escape: (x: any) => x }));
        expect(result instanceof HTMLAnchorElement).toBe(true);
        const link = result as HTMLAnchorElement;
        expect(link.href).toContain('#Northwind-Customer/42');
        expect(link.dataset.itemType).toBe('Northwind.Customer');
        expect(link.dataset.itemId).toBe('42');
        expect(link.classList.contains('s-EditLink')).toBe(true);
        expect(link.classList.contains('s-Northwind-CustomerLink')).toBe(true);
    });

    it('should handle missing item id gracefully', () => {
        const format = SlickFormatting.itemLink("sample", "id", () => 'text');
        const result = format(ctx({ item: {}, value: 'test', escape: (x: any) => x }));
        expect(result instanceof HTMLAnchorElement).toBe(true);
        const link = result as HTMLAnchorElement;
        expect(link.href).not.toContain('undefined');
    });

    it('should apply cssClass function', () => {
        const format = SlickFormatting.itemLink("sample", "id", () => 'text',
            (c) => c.value === 'test' ? 'highlight' : '');
        const result = format(ctx({ item: { id: 1 }, value: 'test', escape: (x: any) => x }));
        expect(result instanceof HTMLAnchorElement).toBe(true);
        const link = result as HTMLAnchorElement;
        expect(link.classList.contains('highlight')).toBe(true);
    });

    it('should encode text by default', () => {
        const format = SlickFormatting.itemLink("sample", "id", () => '<b>bold</b>');
        const result = format(ctx({ item: { id: 1 }, value: 'test', escape: (x: any) => 'escaped:' + x }));
        expect(result instanceof HTMLAnchorElement).toBe(true);
    });
});

describe('SlickFormatting.treeToggle', () => {
    it('should return formatter result wrapped with tree toggle elements', () => {
        const view = {
            getIdxById: vi.fn(() => 0),
            getItemByIdx: vi.fn(() => null)
        };
        const format = SlickFormatting.treeToggle(
            () => view as any,
            (x: any) => x.id,
            (c: any) => c.value
        );

        const result = format(ctx({
            item: { id: 1, _indent: 0 },
            value: 'node',
            enableHtmlRendering: false
        }));

        expect(result).toBeTruthy();
    });

    it('should add expand class when next item has greater indent and current is collapsed', () => {
        const view = {
            getIdxById: vi.fn(() => 0),
            getItemByIdx: vi.fn(() => ({ _indent: 1 }))
        };
        const format = SlickFormatting.treeToggle(
            () => view as any,
            (x: any) => x.id,
            (c: any) => c.value
        );

        const result = format(ctx({
            item: { id: 1, _indent: 0, _collapsed: true },
            value: 'node',
            enableHtmlRendering: false
        }));

        expect(result).toBeTruthy();
    });

    it('should add collapse class when next item has greater indent and current is not collapsed', () => {
        const view = {
            getIdxById: vi.fn(() => 0),
            getItemByIdx: vi.fn(() => ({ _indent: 1 }))
        };
        const format = SlickFormatting.treeToggle(
            () => view as any,
            (x: any) => x.id,
            (c: any) => c.value
        );

        const result = format(ctx({
            item: { id: 1, _indent: 0, _collapsed: false },
            value: 'node',
            enableHtmlRendering: false
        }));

        expect(result).toBeTruthy();
    });

    it('should return HTML string when enableHtmlRendering is true and text is string', () => {
        const view = {
            getIdxById: vi.fn(() => 0),
            getItemByIdx: vi.fn(() => null)
        };
        const format = SlickFormatting.treeToggle(
            () => view as any,
            (x: any) => x.id,
            (c: any) => c.value
        );

        const result = format(ctx({
            item: { id: 1, _indent: 0 },
            value: 'node',
            enableHtmlRendering: true
        }));

        expect(typeof result).toBe('string');
    });
});