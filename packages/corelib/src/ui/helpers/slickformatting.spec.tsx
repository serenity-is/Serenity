import { escapeHtml } from "@serenity-is/sleekgrid";
import { SlickFormatting } from "./slickformatting";

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