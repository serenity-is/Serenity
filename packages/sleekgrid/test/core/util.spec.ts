import { addClass, basicDOMSanitizer, disableSelection, escapeHtml, removeClass } from "../../src/core/util";
import { jsx as H } from "@serenity-is/domwise";

describe('addClass', () => {
    it('should not do anything if classes to add is null or undefined', () => {
        const element: HTMLDivElement = document.createElement('div');

        addClass(element, null);
        addClass(element, undefined);

        expect(element.classList.length).toBe(0);
    });

    it('should add class to the element', () => {
        const element: HTMLDivElement = document.createElement('div');

        addClass(element, 'test');

        expect(element.classList.length).toBe(1);
        expect(element.classList.contains('test')).toBe(true);
    });

    it('should add multiple classes to the element', () => {
        const element: HTMLDivElement = document.createElement('div');

        addClass(element, 'test1 test2');

        expect(element.classList.length).toBe(2);
        expect(element.classList.contains('test1')).toBe(true);
        expect(element.classList.contains('test2')).toBe(true);
    });

    it('should not add duplicate classes to the element', () => {
        const element: HTMLDivElement = document.createElement('div');

        addClass(element, 'test');
        addClass(element, 'test');

        expect(element.classList.length).toBe(1);
        expect(element.classList.contains('test')).toBe(true);
    });

    it('should not add empty space if parameter contains more than one space', () => {
        const element: HTMLDivElement = document.createElement('div');

        addClass(element, 'test1  test2');

        expect(element.classList.length).toBe(2);
        expect(element.classList.contains('test1')).toBe(true);
        expect(element.classList.contains('test2')).toBe(true);
    });
});

describe('removeClass', () => {
    it('should not do anything if classes to remove is null or undefined', () => {
        const element: HTMLDivElement = document.createElement('div');
        element.classList.add('test');

        removeClass(element, null);
        removeClass(element, undefined);

        expect(element.classList.length).toBe(1);
        expect(element.classList.contains('test')).toBe(true);
    });

    it('should remove class from the element', () => {
        const element: HTMLDivElement = document.createElement('div');
        element.classList.add('test');

        expect(element.classList.length).toBe(1);

        removeClass(element, 'test');

        expect(element.classList.length).toBe(0);
        expect(element.classList.contains('test')).toBe(false);
    });

    it('should remove multiple classes from the element', () => {
        const element: HTMLDivElement = document.createElement('div');
        element.classList.add('test1');
        element.classList.add('test2');

        expect(element.classList.length).toBe(2);

        removeClass(element, 'test1 test2');

        expect(element.classList.length).toBe(0);
        expect(element.classList.contains('test1')).toBe(false);
        expect(element.classList.contains('test2')).toBe(false);
    });
});

describe('H', () => {
    it('should create a div element', () => {
        const element: HTMLDivElement = H('div');

        expect(element.tagName).toBe('DIV');
    });

    it('should create a div element with class', () => {
        const element: HTMLDivElement = H('div', { class: 'test' });

        expect(element.tagName).toBe('DIV');
        expect(element.classList.contains('test')).toBe(true);
    });

    it('should create a div element with id', () => {
        const element: HTMLDivElement = H('div', { id: 'test' });

        expect(element.tagName).toBe('DIV');
        expect(element.id).toBe('test');
    });

    it('should create a div element with id and class', () => {
        const element: HTMLDivElement = H('div', { id: 'test', class: 'test' });

        expect(element.tagName).toBe('DIV');
        expect(element.id).toBe('test');
        expect(element.classList.contains('test')).toBe(true);
    });

    it('should create a div element with id, class and attributes', () => {
        const element: HTMLDivElement = H('div', { id: 'test', class: 'test', 'data-test': 'test' });

        expect(element.tagName).toBe('DIV');
        expect(element.id).toBe('test');
        expect(element.classList.contains('test')).toBe(true);
        expect(element.getAttribute('data-test')).toBe('test');
    });

    it('should create a div with children', () => {
        const element: HTMLDivElement = H('div', { children: H('span') });

        expect(element.tagName).toBe('DIV');
        expect(element.childElementCount).toBe(1);
        expect(element.children[0].tagName).toBe('SPAN');
    });

    it('should create a div with children and attributes', () => {
        const element: HTMLDivElement = H('div', { 'data-test': 'test', children: H('span') });

        expect(element.tagName).toBe('DIV');
        expect(element.childElementCount).toBe(1);
        expect(element.children[0].tagName).toBe('SPAN');
        expect(element.getAttribute('data-test')).toBe('test');
    });

    it('should create a div with children which have attributes and classes and id', () => {
        const element: HTMLDivElement = H('div', { 'data-test': 'test', children: H('span', { class: 'test', id: 'test' }) });

        expect(element.tagName).toBe('DIV');
        expect(element.getAttribute('data-test')).toBe('test');
        expect(element.childElementCount).toBe(1);
        expect(element.children[0].tagName).toBe('SPAN');
        expect(element.children[0].classList.contains('test')).toBe(true);
        expect(element.children[0].id).toBe('test');
    });

    it('should leave attribute value empty on div if value is true', () => {
        const element: HTMLDivElement = H('div', { 'data-test': true as any });

        expect(element.getAttribute('data-test')).toBe('');
    });

    it('should not add attribute if value is false', () => {
        const element: HTMLDivElement = H('div', { 'data-test': false as any });

        expect(element.hasAttribute('data-test')).toBe(false);
    });

    it('should not add attribute if value is null', () => {
        const element: HTMLDivElement = H('div', { 'data-test': null });

        expect(element.hasAttribute('data-test')).toBe(false);
    });

    it('should call ref method with the element reference', () => {
        var divRef: HTMLSpanElement;
        var spanRef: HTMLSpanElement;
        var element = H('div', {
            ref: el => divRef = el, children:
                H('span', { ref: el => spanRef = el })
        });
        expect(divRef).toBeDefined();
        expect(divRef === element).toBe(true);
        expect(spanRef).toBeDefined();
        expect(spanRef.tagName).toBe('SPAN');
    });

    it('converts className attribute to class', () => {
        var element = H('div', { className: 'test' });
        expect(element).toBeDefined();
        expect(element.className).toBe('test');
    });

    it('can set className property via class', () => {
        var element = H('div', { class: 'test' });
        expect(element).toBeDefined();
        expect(element.className).toBe('test');
    });

});

describe('disableSelection', () => {
    it('should not do anything if element is null or undefined', () => {
        disableSelection(null);
        disableSelection(undefined);
    });

    it('should disable selection on the element', () => {
        const element: HTMLDivElement = document.createElement('div');
        var func: Function;
        element.addEventListener = (_: any, listener: any) => func = listener;

        disableSelection(element);

        expect(element.style.userSelect).toBe('none');
        expect(func).toBeDefined();
        expect(func()).toBe(false);
    });
});

describe('escape', () => {
    it('should encode & as &amp;', () => {
        expect(escapeHtml('&')).toBe('&amp;');
    });

    it('should encode < as &lt;', () => {
        expect(escapeHtml('<')).toBe('&lt;');
    });

    it('should encode > as &gt;', () => {
        expect(escapeHtml('>')).toBe('&gt;');
    });

    it('should encode " as &quot;', () => {
        expect(escapeHtml('"')).toBe('&quot;');
    });

    it('should encode multiple & as &amp;', () => {
        expect(escapeHtml('&&')).toBe('&amp;&amp;');
    });

    it('should encode multiple < as &lt;', () => {
        expect(escapeHtml('<<')).toBe('&lt;&lt;');
    });

    it('should encode multiple > as &gt;', () => {
        expect(escapeHtml('>>')).toBe('&gt;&gt;');
    });

    it('should encode multiple " as &quot;', () => {
        expect(escapeHtml('""')).toBe('&quot;&quot;');
    });

    it('should encode all characters', () => {
        expect(escapeHtml('&<>"')).toBe('&amp;&lt;&gt;&quot;');
    });

    it('should return empty string if parameter is null or undefined', () => {
        expect(escapeHtml(null)).toBe('');
        expect(escapeHtml(undefined)).toBe('');
    });

    it('should convert any type to a string', () => {
        expect(escapeHtml(1)).toBe('1');
        expect(escapeHtml(true)).toBe('true');
        expect(escapeHtml({})).toBe('[object Object]');
    });

    it('uses this.value if no argument passed', () => {
        expect(escapeHtml.apply({ value: "&><" }, [])).toBe("&amp;&gt;&lt;");
    });
});

describe("basicDOMSanitizer", () => {
    it("should return empty string for null/undefined input", () => {
        expect(basicDOMSanitizer(null)).toBe('');
        expect(basicDOMSanitizer(undefined)).toBe('');
        expect(basicDOMSanitizer('')).toBe('');
    });

    it("should use fast path for plain text without HTML characters", () => {
        // These should go through the fast path and return as-is
        expect(basicDOMSanitizer('Hello World')).toBe('Hello World');
        expect(basicDOMSanitizer('123456')).toBe('123456');
        expect(basicDOMSanitizer('a-z A-Z 0-9')).toBe('a-z A-Z 0-9');
        expect(basicDOMSanitizer('Special chars: @#$%^*()')).toBe('Special chars: @#$%^*()');
        expect(basicDOMSanitizer('Unicode: 你好 🌟')).toBe('Unicode: 你好 🌟');
    });

    it("should preserve safe HTML content", () => {
        const safe = '<div class="container"><p>Hello <strong>world</strong>!</p></div>';
        expect(basicDOMSanitizer(safe)).toBe(safe);
    });

    it("should remove script tags completely", () => {
        const dirty = '<div>Safe</div><script>alert("XSS")</script><p>Content</p>';
        const expected = '<div>Safe</div><p>Content</p>';
        expect(basicDOMSanitizer(dirty)).toBe(expected);
    });

    it("should remove iframe elements", () => {
        const dirty = '<p>Before</p><iframe src="malicious.com"></iframe><p>After</p>';
        const expected = '<p>Before</p><p>After</p>';
        expect(basicDOMSanitizer(dirty)).toBe(expected);
    });

    it("should remove dangerous elements", () => {
        const dangerousElements = ['object', 'embed', 'form', 'input', 'button', 'textarea', 'select', 'style', 'link'];
        dangerousElements.forEach(tag => {
            const dirty = `<p>Safe</p><${tag}>dangerous</${tag}><p>Content</p>`;
            const result = basicDOMSanitizer(dirty);
            expect(result).not.toContain(`<${tag}>`);
            expect(result).not.toContain(`</${tag}>`);
            expect(result).toContain('<p>Safe</p>');
            expect(result).toContain('<p>Content</p>');
        });
    });

    it("should remove event handler attributes", () => {
        const dirty = '<div onclick="alert(1)" onload="evil()" onmouseover="bad()">Content</div>';
        const expected = '<div>Content</div>';
        expect(basicDOMSanitizer(dirty)).toBe(expected);
    });

    it("should remove href attributes with javascript URLs", () => {
        const dirty = '<a href="javascript:alert(1)">Bad Link</a>';
        const expected = '<a>Bad Link</a>';
        expect(basicDOMSanitizer(dirty)).toBe(expected);
    });

    it("should remove src attributes with javascript URLs", () => {
        const dirty = '<img src="javascript:alert(1)">';
        const expected = '<img>';
        expect(basicDOMSanitizer(dirty)).toBe(expected);
    });

    it("should remove data URL attributes", () => {
        const dirty = '<img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==">';
        const expected = '<img>';
        expect(basicDOMSanitizer(dirty)).toBe(expected);
    });

    it("should preserve safe href attributes", () => {
        const safe = '<a href="https://example.com">Safe Link</a>';
        expect(basicDOMSanitizer(safe)).toBe(safe);
    });

    it("should preserve safe src attributes", () => {
        const safe = '<img src="https://example.com/image.jpg" alt="Safe Image">';
        expect(basicDOMSanitizer(safe)).toBe(safe);
    });

    it("should remove attributes containing javascript anywhere", () => {
        const dirty = '<div data-js="javascript:void(0)" custom="some-javascript-code">Content</div>';
        const expected = '<div>Content</div>';
        expect(basicDOMSanitizer(dirty)).toBe(expected);
    });

    it("should handle complex nested dangerous content", () => {
        const dirty = `
            <div class="safe">
                <p>Safe paragraph</p>
                <script>alert('XSS1');</script>
                <a href="javascript:alert('XSS2')">Bad Link</a>
                <iframe src="evil.com">
                    <form action="hack.php">
                        <input type="hidden" name="csrf" value="stolen">
                        <button onclick="stealData()">Submit</button>
                    </form>
                </iframe>
                <img src="data:text/html,<script>alert('XSS3')</script>">
                <span>More safe content</span>
            </div>
        `;
        const result = basicDOMSanitizer(dirty);

        // Check that dangerous elements are removed
        expect(result).not.toContain('<script>');
        expect(result).not.toContain('<iframe>');
        expect(result).not.toContain('<form>');
        expect(result).not.toContain('<input>');
        expect(result).not.toContain('<button>');
        expect(result).not.toContain('javascript:');
        expect(result).not.toContain('data:');

        // Check that safe content is preserved
        expect(result).toContain('<div class="safe">');
        expect(result).toContain('<p>Safe paragraph</p>');
        expect(result).toContain('<span>More safe content</span>');
        expect(result).toContain('Bad Link'); // Text should remain even if href is removed
    });

    it("should handle malformed HTML gracefully", () => {
        const malformed = '<div><script>evil</script><p>Unclosed<div><a href="javascript:bad">Bad</a>';
        // Should not crash and should sanitize what's parseable
        const result = basicDOMSanitizer(malformed);
        expect(result).not.toContain('<script>');
        expect(result).not.toContain('javascript:');
        expect(result).toContain('Unclosed');
    });

    it("should handle HTML entities correctly", () => {
        const withEntities = '<p>&lt;script&gt;alert(1)&lt;/script&gt;</p>';
        expect(basicDOMSanitizer(withEntities)).toBe(withEntities);
    });

    it("should handle mixed case tags and attributes", () => {
        const mixedCase = '<DIV><SCRIPT>alert(1)</SCRIPT><A HREF="javascript:bad()">Link</A></DIV>';
        const expected = '<div><a>Link</a></div>';
        expect(basicDOMSanitizer(mixedCase)).toBe(expected);
    });

    it("should handle xlink:href attributes", () => {
        const dirty = '<svg><a xlink:href="javascript:alert(1)">Bad SVG Link</a></svg>';
        const expected = '<svg><a>Bad SVG Link</a></svg>';
        expect(basicDOMSanitizer(dirty)).toBe(expected);
    });

    it("should preserve safe xlink:href attributes", () => {
        const safe = '<svg><a xlink:href="#anchor">Safe SVG Link</a></svg>';
        expect(basicDOMSanitizer(safe)).toBe(safe);
    });

    it("should validate URLs with improved pattern", () => {
        // Should allow safe protocols
        expect(basicDOMSanitizer('<a href="https://example.com">HTTPS</a>')).toBe('<a href="https://example.com">HTTPS</a>');
        expect(basicDOMSanitizer('<a href="http://example.com">HTTP</a>')).toBe('<a href="http://example.com">HTTP</a>');
        expect(basicDOMSanitizer('<a href="mailto:test@example.com">Email</a>')).toBe('<a href="mailto:test@example.com">Email</a>');
        expect(basicDOMSanitizer('<a href="tel:+1234567890">Phone</a>')).toBe('<a href="tel:+1234567890">Phone</a>');

        // Should allow relative URLs
        expect(basicDOMSanitizer('<a href="/path">Relative Path</a>')).toBe('<a href="/path">Relative Path</a>');
        expect(basicDOMSanitizer('<a href="?query=value">Query</a>')).toBe('<a href="?query=value">Query</a>');
        expect(basicDOMSanitizer('<a href="#anchor">Anchor</a>')).toBe('<a href="#anchor">Anchor</a>');

        // Should block dangerous protocols
        expect(basicDOMSanitizer('<a href="javascript:alert(1)">JS</a>')).toBe('<a>JS</a>');
        expect(basicDOMSanitizer('<a href="data:text/html,<script>alert(1)</script>">Data</a>')).toBe('<a>Data</a>');
        expect(basicDOMSanitizer('<a href="vbscript:msgbox(1)">VBScript</a>')).toBe('<a>VBScript</a>');
    });

    it("should preserve whitespace between elements", () => {
        // Test whitespace preservation between safe elements
        const withSpace = '<i class="fa fa-something"></i> Sometext';
        expect(basicDOMSanitizer(withSpace)).toBe(withSpace);

        // Test multiple spaces
        const multipleSpaces = '<span>Before</span>   <span>After</span>';
        expect(basicDOMSanitizer(multipleSpaces)).toBe(multipleSpaces);

        // Test newlines
        const withNewlines = '<div>Line 1</div>\n<div>Line 2</div>';
        expect(basicDOMSanitizer(withNewlines)).toBe(withNewlines);

        // Test whitespace around dangerous elements (should preserve whitespace when elements are removed)
        const dangerousWithSpace = '<i class="fa fa-something"></i> <script>evil()</script> Sometext';
        const expectedDangerous = '<i class="fa fa-something"></i>  Sometext';
        expect(basicDOMSanitizer(dangerousWithSpace)).toBe(expectedDangerous);

        // Note: DOMParser normalizes invalid self-closing tags for non-void elements
        // This is expected HTML parsing behavior, not a bug in the sanitizer
        // Example: '<i class="fa fa-something" /> Sometext' becomes '<i class="fa fa-something"> Sometext</i>'
        // because <i> is not a void element in HTML5
    });

});

