import { StringEditor } from '../editors/stringeditor';
import { type FC } from 'jsx-dom';
import $ from "@optionaldeps/jquery";
import { jsxDomWidget } from './jsx';

test('render childless element', function () {
    const element = <br />;

    expect(element.outerHTML).toBe('<br>');
});

test('render div with children', function () {
    const element = (
        <div>
            <span />
        </div>
    );

    expect(element.outerHTML).toBe('<div><span></span></div>');
});

test('render div with multiple children', function () {
    const element = (
        <div>
            <span />
            <br />
        </div>
    );

    expect(element.outerHTML).toBe('<div><span></span><br></div>');
});

test('render array of children', function () {
    const element = (
        <div>
            {[
                <span data-key={0}>0</span>,
                <span data-key={1}>1</span>,
            ]}

            <span>2</span>
        </div>
    );

    expect(element.outerHTML).toBe('<div><span data-key="0">0</span><span data-key="1">1</span><span>2</span></div>');
});

test('render number child', function () {
    const element = <span>7</span>;

    expect(element.outerHTML).toBe('<span>7</span>');
});

test('render multiple number children', function () {
    const element = (
        <span>
            {1}
            {2}
            {3}
        </span>
    );

    expect(element.outerHTML).toBe('<span>123</span>');
});

test('render string child', function () {
    const element = <span>test</span>;

    expect(element.outerHTML).toBe('<span>test</span>');
});

test('render multiple string children', function () {
    const element = (
        <span>
            {'hello'} {'world'}
        </span>
    );

    expect(element.outerHTML).toBe('<span>hello world</span>');
});

test('render div with TextNode child', function () {
    const element = <div>{document.createTextNode('Hello')}</div>;

    expect(element.outerHTML).toBe('<div>Hello</div>');
});

test('skip boolean children', function () {
    const element = (
        <span>
            {true}
            {false}
        </span>
    );

    expect(element.outerHTML).toBe('<span></span>');
});

test('skip null children', function () {
    const element = <span>{null}</span>;

    expect(element.outerHTML).toBe('<span><!----></span>');
});

test('skip undefined children', function () {
    const element = <span>{undefined}</span>;

    expect(element.outerHTML).toBe('<span></span>');
});

test('render falsey children', function () {
    const element = (
        <span>
            {0}
            {Number.NaN}
        </span>
    );

    expect(element.outerHTML).toBe('<span>0NaN</span>');
});

test('render other elements inside', function () {
    const firstElement = <a href="#first">First</a>;
    const secondElement = <a href="#second">Second</a>;
    const element = (
        <div>
            {firstElement}
            {secondElement}
        </div>
    );

    expect(element.outerHTML).toBe('<div><a href="#first">First</a><a href="#second">Second</a></div>');
});

test('render document fragments inside', function () {
    const template = document.createElement('template');
    template.innerHTML = 'Hello, <strong>World!</strong> ';
    const fragment = template.content;
    const element = <div>{fragment}</div>;

    expect(element.outerHTML).toBe('<div>Hello, <strong>World!</strong> </div>');
});

test('render svg', function () {
    const createElementNSSpy = jest.spyOn(document, 'createElementNS');

    const element = (
        <svg>
            <text x="20" y="20">
                Test
            </text>
        </svg>
    );

    expect(element).toBeTruthy();
    expect(createElementNSSpy.mock.calls.length).toBe(2);

    const xmlns = 'http://www.w3.org/2000/svg';
    expect(createElementNSSpy.mock.calls[0]).toEqual([xmlns, 'text']);
    expect(createElementNSSpy.mock.calls[1]).toEqual([xmlns, 'svg']);
    createElementNSSpy.mockClear();
});

test('render mixed html and svg', function () {
    const createElementSpy = jest.spyOn(document, 'createElement');
    const createElementNSSpy = jest.spyOn(document, 'createElementNS');

    const element = (
        <div>
            <h1>Demo</h1>

            <svg>
                <text>Test</text>
            </svg>
        </div>
    );

    expect(element).toBeDefined();
    expect(createElementSpy.mock.calls.length).toBe(2);
    expect(createElementNSSpy.mock.calls.length).toBe(2);

    expect(createElementSpy.mock.calls[0]).toEqual(['h1']);
    expect(createElementSpy.mock.calls[1]).toEqual(['div']);

    const xmlns = 'http://www.w3.org/2000/svg';
    expect(createElementNSSpy.mock.calls[0]).toEqual([xmlns, 'text']);
    expect(createElementNSSpy.mock.calls[1]).toEqual([xmlns, 'svg']);
    createElementSpy.mockClear();
    createElementNSSpy.mockClear();
});

test('create svg links with xlink namespace', function () {
    const setAttributeNS = jest.spyOn(Element.prototype, 'setAttributeNS');

    const element = (
        <svg>
            <text id="text">Test</text>
            <use xlinkHref="#text" />
            <use xlink-invalid-attribute="#text" />
        </svg>
    );

    expect(element).toBeDefined();
    expect(setAttributeNS.mock.calls.length).toBe(1);

    const xmlns = 'http://www.w3.org/1999/xlink';
    expect(setAttributeNS.mock.calls[0]).toEqual([
        xmlns,
        'xlink:href',
        '#text',
    ]);
    setAttributeNS.mockClear();
});

test('assign className', function () {
    const element = <span className="a b c" />;

    expect(element.outerHTML).toBe('<span class="a b c"></span>');
});

test('assign className via class alias', function () {
    const element = <span class="a b c" />;

    expect(element.outerHTML).toBe('<span class="a b c"></span>');
});

test('assign styles', function () {
    const style = {
        paddingTop: '10px',
        width: '200px',
        height: '200px',
        fontSize: '12px',
    };

    const element = <span {...{ style }} />;

    expect(element.outerHTML).toBe('<span style="padding-top: 10px; width: 200px; height: 200px; font-size: 12px;"></span>');
});

test('assign styles with dashed property names', function () {
    const style = {
        paddingTop: 10,
        fontSize: 12,
    };

    // ts-expect-error TODO: update the types
    const element = <span style={style} />;

    expect(element.outerHTML).toBe('<span style="padding-top: 10px; font-size: 12px;"></span>');
});

test('assign styles with css variables', function () {
    const element = <span style="--padding-top: 10; --myCamelCaseVar: red;" />;

    expect(element.outerHTML).toBe('<span style="--padding-top: 10; --myCamelCaseVar: red;"></span>');
});

test('assign other props', function () {
    const element = (
        <a href="video.mp4" id="a" referrerPolicy="no-referrer">
            Download
        </a>
    );

    expect(element.outerHTML).toBe('<a href="video.mp4" id="a" referrerpolicy="no-referrer">Download</a>');
});

test('assign htmlFor prop', function () {
    const element = <label htmlFor="name-input">Full name</label>;

    expect(element.outerHTML).toBe('<label for="name-input">Full name</label>');
});

test('assign or skip boolean props', function () {
    const input = (
        <input disabled={false} />
    );

    expect(input.outerHTML).toBe('<input>');

    const link = (
        <a download contentEditable={true}>
            Download
        </a>
    );

    expect(link.outerHTML).toBe('<a download="" contenteditable="">Download</a>');
});

test('assign booleanish false props', function () {
    const element = (
        <span contentEditable>
            <a contentEditable={false}>Download</a>
        </span>
    );
    const input = <textarea spellCheck={false} />;

    expect(element.outerHTML).toBe('<span contenteditable=""><a>Download</a></span>');
    expect(input.outerHTML).toBe('<textarea></textarea>');
});

test('skip undefined and null props', function () {
    const element = (
        // ts-expect-error Types don't allow it, but we need to test it
        <a href={undefined} title={null}>
            Download
        </a>
    );

    expect(element.outerHTML).toBe('<a>Download</a>');
});

test('escape props', function () {
    const element = <a id={'"test"'}>Download</a>;

    expect(element.outerHTML).toBe('<a id="&quot;test&quot;">Download</a>');
});

test('escape children', function () {
    const element = <div>{'<script>alert();</script>'}</div>;

    expect(element.outerHTML).toBe('<div>&lt;script&gt;alert();&lt;/script&gt;</div>');
});

test('set html', function () {
    const element = (
        <div dangerouslySetInnerHTML={{ __html: '<script>alert();</script>' }} />
    );

    expect(element.outerHTML).toBe('<div><script>alert();</script></div>');
});

test('attach event listeners', function () {
    const addEventListener = jest.spyOn(EventTarget.prototype, 'addEventListener');

    const handleClick = function () { };
    const element = (
        <a href="#" onClick={handleClick}>
            Download
        </a>
    );

    expect(element.outerHTML).toBe('<a href="#">Download</a>');

    expect(element.onclick).toBe(handleClick);
    expect(addEventListener.mock.calls.length).toEqual(0);
    addEventListener.mockClear();
});

test('attach event listeners but drop the dash after on', function () {
    const addEventListener = jest.spyOn(EventTarget.prototype, 'addEventListener');

    const handler = function () { };
    const assignProps = { onremoteinput: handler, onRemoteinput: handler };
    const element = (
        <a href="#" {...assignProps}>
            Download
        </a>
    );

    expect(element.outerHTML).toBe('<a href="#">Download</a>');

    expect(addEventListener.mock.calls.length).toBe(2);
    expect(addEventListener.mock.calls[0]).toEqual([
        'remoteinput',
        handler,
    ]);
    expect(addEventListener.mock.calls[1]).toEqual([
        'remoteinput',
        handler,
    ]);

    addEventListener.mockClear();
});

test('fragment', function () {
    const createDocumentFragment = jest.spyOn(document, 'createDocumentFragment');

    const fragment = <>test</>;

    const fragmentHtml = getfragmentHtml(fragment as any);

    expect(fragmentHtml).toBe('test');
    expect(createDocumentFragment.mock.calls.length).toBe(1);
    expect(createDocumentFragment.mock.calls[0]).toEqual([]);
});

test('fragment 2', function () {
    const fragment = (
        <>
            <h1>test</h1>
        </>
    );

    const fragmentHtml = getfragmentHtml(fragment as any);

    expect(fragmentHtml).toBe('<h1>test</h1>');
});

test('fragment 3', function () {
    const fragment = (
        <>
            <h1>heading</h1> text
        </>
    );

    const fragmentHtml = getfragmentHtml(fragment as any);

    expect(fragmentHtml).toBe('<h1>heading</h1> text');
});

test('div with inner fragment', function () {
    const element = (
        <div>
            <>
                <h1>heading</h1> text
            </>
            <span>outside fragment</span>
        </div>
    );

    expect(element.outerHTML).toBe('<div><h1>heading</h1> text<span>outside fragment</span></div>');
});

test('element created by function', function () {
    const Icon = () => <i />;

    const element = <Icon />;

    expect(element.outerHTML).toBe('<i></i>');
});

test('element created by function with existing children and attributes', function () {
    const Icon = () => <i className="sweet">Gummy <span>bears</span></i>;

    const element = <Icon />;

    expect(element.outerHTML).toBe('<i class="sweet">Gummy <span>bears</span></i>');
});

function getfragmentHtml(fragment: DocumentFragment): string {
    return Array.from(fragment.childNodes)
        .map(n => (n as HTMLElement).outerHTML || n.textContent)
        .join('');
}

//#region Make sure these typings still work.
const NoProps: FC = () => <span>foo</span>;
const OptionalProps: FC<{ foo?: string }> = ({ foo }) => <span>{foo?.length || 0}</span>;
const RequiredProps: FC<{ foo: string }> = ({ foo }) => <span>{foo.length}</span>;
const Children: FC = (_, children) => <div>{children}</div>;
const _ = <>
    some text
    <NoProps />
    <OptionalProps />
    <OptionalProps foo='bar' />
    <RequiredProps foo='bar' />
    <Children>
        <Children>
            <input type="button"
                onClick={console.log}
                style=""
                autoCapitalize='off' />
            <div style={{}} contentEditable></div>
            <svg id='svg'>
                <g>
                    <path />
                </g>
            </svg>
        </Children>
    </Children>
</>;
//#endregion

describe('jsx: intrinsic elements', () => {
    it('do not return null', () => expect(<div />).toBeDefined());

    it('can render an instance of an HTMLDivElement', () => expect(<div />).toBeInstanceOf(HTMLDivElement));

    it('can receive an object for style attribute', () => expect(
        (<div style={{ display: 'none' }} /> as HTMLDivElement).style.display
    ).toBe('none'));

    it('can receive a string for style attribute', () => expect(
        (<div style="display: none" /> as HTMLDivElement).style.display
    ).toBe('none'));

    it('can accept data attributes', () => expect(
        (<div data-foo="bar" /> as HTMLDivElement).dataset.foo
    ).toBe('bar'));

    it('can accept arbitrary attributes', () => expect(
        (<div aria-label="foo" /> as HTMLElement).getAttribute('aria-label')
    ).toBe('foo'));

    it('can get className and classList after class attribute assignment', () => {
        const div = <div class="foo"></div>;
        expect(div.classList[0]).toBe('foo');
        expect(div.className).toBe('foo');
    })

    it('can accept HTMLCollections as children', () => {
        const parent = <div class="parent">
            <div class="child"></div>
            <div class="child"></div>
            <div class="child"></div>
        </div>
        const container = <section>{parent.children}</section>
        expect(container.children.length).toBe(3);
        expect(container.children[2].className).toBe("child");
    })
});

describe('jsx: fragments', () => {
    it('return Document Fragment', () => expect((<></>)).toBeInstanceOf(DocumentFragment));

    it('preserve children', () => {
        const fragment = Array.from((<><div>1</div><div>2</div></>).children);
        expect(fragment.length).toBe(2);
        expect(fragment[0].textContent).toBe('1');
        expect(fragment[1].textContent).toBe('2');
    });
});

describe('jsx: components', () => {
    const Foo: FC<{ bar?: string }> = ({ bar, children }) => {
        return <div>{bar ?? ''} {children}</div>;
    };

    it('do not return null', () => expect(<Foo />).toBeDefined());

    it('can render an instance of an HTMLElement', () => expect(<Foo />).toBeInstanceOf(HTMLElement));

    it('can receive named props', () => expect(
        (<Foo bar='baz' />).textContent?.trim()
    ).toBe('baz'));

    it('can receive children', () => expect(
        (<Foo><div>bar</div><div>baz</div></Foo>).childElementCount
    ).toBe(2));

    it('can be nested in other elements', () => expect(
        (<div><Foo bar='baz' /></div>).textContent?.trim()
    ).toBe('baz'));

    it('can be nested in other components', () => expect(
        (<Foo><Foo bar='baz'></Foo></Foo>).textContent?.trim()
    ).toBe('baz'));
});

describe('jsx: interpolation', () => {
    const date = new Date();

    it('can render strings', () => expect(
        (<div>{'foo'}</div>).textContent
    ).toBe('foo'));

    it('can render simple arrays', () => expect(
        (<div>{['foo', 'bar']}</div>).textContent
    ).toBe('foobar'));

    it('can render mixed arrays', () => {
        const el = <div>{['foo', <span>bar</span>]}</div>;
        expect(el.childElementCount).toBe(1);
        expect(el.textContent).toBe('foobar');
    });

    it('can render numbers', () => expect(
        (<div>{100}</div>).textContent
    ).toBe('100'));

    it('can render booleans', () => expect(
        (<div>{true.toString()}</div>).textContent
    ).toBe('true'));

    it('can render Date objects', () => expect(
        (<div>{date.toUTCString()}</div>).textContent!).toBe(date.toUTCString()));
});

const StringEditor_ = jsxDomWidget(StringEditor);

describe('jsx: widget integration', () => {

    it('can create input', () => {
        var ed: StringEditor;
        window.$ = window.jQuery = $;
        var el = <StringEditor_ ref={x => ed = x} readOnly={true} />;
        expect(el.tagName).toBe('INPUT');
        expect(el.classList.contains('s-StringEditor')).toBe(true);
        expect(ed).toBeDefined();
        expect(ed.element[0]).toBe(el);
        expect(el.getAttribute('readonly')).toBe('readonly');
    });
});