import {
    applyFormatterResultToCellNode,
    CompatFormatter,
    convertCompatFormatter,
    defaultColumnFormat,
    formatterContext as ctx,
    FormatterContext
} from "../../src/core/formatting";
import { gridDefaults } from "../../src/core/gridoptions";

describe('defaultFormatter with enableHtmlRendering: true', () => {
    let previousHtmlRenderingSetting: boolean;
    beforeEach(() => {
        previousHtmlRenderingSetting = gridDefaults.enableHtmlRendering;
        gridDefaults.enableHtmlRendering = true;
    });
    afterEach(() => gridDefaults.enableHtmlRendering = previousHtmlRenderingSetting);
    try {
        it('should encode & as &amp;', () => {
            expect(defaultColumnFormat(ctx({ value: '&' }))).toBe('&amp;');
        });

        it('should encode < as &lt;', () => {
            expect(defaultColumnFormat(ctx({ value: '<' }))).toBe('&lt;');
        });

        it('should encode > as &gt;', () => {
            expect(defaultColumnFormat(ctx({ value: '>' }))).toBe('&gt;');
        });

        it('should encode multiple & as &amp;', () => {
            expect(defaultColumnFormat(ctx({ value: '&&' }))).toBe('&amp;&amp;');
        });

        it('should encode multiple < as &lt;', () => {
            expect(defaultColumnFormat(ctx({ value: '<<' }))).toBe('&lt;&lt;');
        });

        it('should encode multiple > as &gt;', () => {
            expect(defaultColumnFormat(ctx({ value: '>>' }))).toBe('&gt;&gt;');
        });

        it('should encode all characters', () => {
            expect(defaultColumnFormat(ctx({ value: '&<>' }))).toBe('&amp;&lt;&gt;');
        });

        it('should return empty string if parameter is null or undefined', () => {
            expect(defaultColumnFormat(ctx({ value: null }))).toBe('');
            expect(defaultColumnFormat(ctx({ value: undefined }))).toBe('');
        });

        it('should convert any type to a string', () => {
            expect(defaultColumnFormat(ctx({ value: 1 }))).toBe('1');
            expect(defaultColumnFormat(ctx({ value: true }))).toBe('true');
            expect(defaultColumnFormat(ctx({ value: {} }))).toBe('[object Object]');
        });
    } finally {
        gridDefaults.enableHtmlRendering = previousHtmlRenderingSetting;
    }
});

describe('defaultFormatter with enableHtmlRendering: false', () => {
    let previousHtmlRenderingSetting: boolean;
    beforeEach(() => {
        previousHtmlRenderingSetting = gridDefaults.enableHtmlRendering;
        gridDefaults.enableHtmlRendering = false;
    });
    afterEach(() => gridDefaults.enableHtmlRendering = previousHtmlRenderingSetting);
    try {
        it('should not encode html characters', () => {
            expect(defaultColumnFormat(ctx({ value: '&<>!"\'' }))).toBe('&<>!"\'');
        });

        it('should return empty string if parameter is null or undefined', () => {
            expect(defaultColumnFormat(ctx({ value: null }))).toBe('');
            expect(defaultColumnFormat(ctx({ value: undefined }))).toBe('');
        });

        it('should convert any type to a string', () => {
            expect(defaultColumnFormat(ctx({ value: 1 }))).toBe('1');
            expect(defaultColumnFormat(ctx({ value: true }))).toBe('true');
            expect(defaultColumnFormat(ctx({ value: {} }))).toBe('[object Object]');
        });
    } finally {
        gridDefaults.enableHtmlRendering = previousHtmlRenderingSetting;
    }
});

describe('applyFormatterResultToCellNode', () => {
    it('should remove old fmtatt from element dataset', () => {
        const cellNode = document.createElement('div');
        cellNode.dataset.fmtatt = 'a,b,c';

        expect(cellNode.dataset.fmtatt).not.toBe(undefined);

        applyFormatterResultToCellNode(ctx(), '', cellNode);

        expect(cellNode.dataset.fmtatt).toBe(undefined);
    });

    it('should remove old fmtatt values from element attributes', () => {
        const cellNode = document.createElement('div');
        cellNode.dataset.fmtatt = 'a,b,c';
        cellNode.setAttribute('a', '1');
        cellNode.setAttribute('b', '2');
        cellNode.setAttribute('c', '3');

        applyFormatterResultToCellNode(ctx(), '', cellNode);

        expect(cellNode.dataset.fmtatt).toBe(undefined);
        expect(cellNode.getAttribute('a')).toBe(null);
        expect(cellNode.getAttribute('b')).toBe(null);
        expect(cellNode.getAttribute('c')).toBe(null);
    });

    it('should remove old fmtcls from element dataset', () => {
        const cellNode = document.createElement('div');
        cellNode.dataset.fmtcls = 'a,b,c';

        expect(cellNode.dataset.fmtcls).not.toBe(undefined);

        applyFormatterResultToCellNode(ctx(), '', cellNode);

        expect(cellNode.dataset.fmtcls).toBe(undefined);
    });

    it('should remove old fmtcls from element dataset', () => {
        const cellNode = document.createElement('div');
        cellNode.dataset.fmtcls = 'a,b,c';

        expect(cellNode.dataset.fmtcls).not.toBe(undefined);

        applyFormatterResultToCellNode(ctx({ addClass: "" }), '', cellNode);

        expect(cellNode.dataset.fmtcls).toBe(undefined);
    });

    it('should remove old fmtcls values from element classes if result contains addClass', () => {
        const cellNode = document.createElement('div');
        cellNode.dataset.fmtcls = 'a b c';
        cellNode.classList.add('a', 'b', 'c');

        applyFormatterResultToCellNode(ctx(), '', cellNode);

        expect(cellNode.dataset.fmtcls).toBe(undefined);
        expect(cellNode.classList.contains('a')).toBe(false);
        expect(cellNode.classList.contains('b')).toBe(false);
        expect(cellNode.classList.contains('c')).toBe(false);
    });

    it('should remove tooltip from element if result doesnt contain tooltip', () => {
        const cellNode = document.createElement('div');
        cellNode.setAttribute('tooltip', 'a');

        applyFormatterResultToCellNode(ctx(), '', cellNode);

        expect(cellNode.getAttribute('tooltip')).toBe(null);
    });

    it('should set tooltip on element if result contains tooltip', () => {
        const cellNode = document.createElement('div');

        applyFormatterResultToCellNode(ctx({
            tooltip: 'test'
        }), undefined, cellNode);

        expect(cellNode.getAttribute('tooltip')).toBe('test');
    });

    it('should set html of the element if fmtResult is string', () => {
        const cellNode = document.createElement('div');

        applyFormatterResultToCellNode(ctx(), 'test', cellNode);

        expect(cellNode.innerHTML).toBe('test');
    });

    it('should set html of the element to the html in the object', () => {
        const cellNode = document.createElement('div');

        applyFormatterResultToCellNode(ctx(), 'test<span>test</span>', cellNode);

        expect(cellNode.childElementCount).toBe(1);
        expect(cellNode.innerHTML).toBe('test<span>test</span>');
    });

    it('should apply attributes using addAttrs in the object', () => {
        const cellNode = document.createElement('div');

        applyFormatterResultToCellNode(ctx({
            addAttrs: {
                a: '1',
                b: '2',
                c: '3'
            },
        }), undefined, cellNode);

        expect(cellNode.getAttribute('a')).toBe('1');
        expect(cellNode.getAttribute('b')).toBe('2');
        expect(cellNode.getAttribute('c')).toBe('3');
    });

    it('should set fmtatt in the dataset using addAttrs in the object', () => {
        const cellNode = document.createElement('div');

        applyFormatterResultToCellNode(ctx({
            addAttrs: {
                a: '1',
                b: '2',
                c: '3'
            },
        }), undefined, cellNode);

        expect(cellNode.dataset.fmtatt).toBe('a,b,c');
    });

    it('should apply tooltip', () => {
        const cellNode = document.createElement('div');

        applyFormatterResultToCellNode(ctx({
            tooltip: 'test'
        }), undefined, cellNode);

        expect(cellNode.getAttribute('tooltip')).toBe('test');
    });

    it('should empty innerHTML if object is null or undefined', () => {
        const cellNode = document.createElement('div');
        cellNode.innerHTML = 'test';

        applyFormatterResultToCellNode(ctx(), null, cellNode);
        expect(cellNode.innerHTML).toBe('');

        cellNode.innerHTML = 'test';
        applyFormatterResultToCellNode(ctx(), undefined, cellNode);
        expect(cellNode.innerHTML).toBe('');
    });

    it('should apply classes using addClass in the object', () => {
        const cellNode = document.createElement('div');

        applyFormatterResultToCellNode(ctx({
            addClass: 'a b c',
        }), undefined, cellNode);

        expect(cellNode.classList.contains('a')).toBe(true);
        expect(cellNode.classList.contains('b')).toBe(true);
        expect(cellNode.classList.contains('c')).toBe(true);
    });

    it('should set fmtcls in the dataset using addClass in the object', () => {
        const cellNode = document.createElement('div');

        applyFormatterResultToCellNode(ctx({
            addClass: 'a b c',
        }), undefined, cellNode);

        expect(cellNode.dataset.fmtcls).toBe('a b c');
    });
});

describe('convertCompatFormatter', () => {
    it('should return null if compat formatter is null', () => {
        expect(convertCompatFormatter(null)).toBe(null);
    });

    it('should convert a compat formatter that returns an CompatFormatterResult to ColumnFormatter', () => {
        const compatFormatterRetObj: CompatFormatter = (_value, _row, _column, _dataContext, _grid) => ({
            text: 'test',
            addClasses: 'd e f',
            toolTip: 'test'
        });

        const formatter = convertCompatFormatter(compatFormatterRetObj);

        expect(formatter).not.toBe(compatFormatterRetObj);
        expect(formatter).toBeInstanceOf(Function);

        const fmtCtx: FormatterContext = ctx();
        const result = formatter(fmtCtx);

        expect(result).toBe('test');
        expect(fmtCtx.addClass).toBe('d e f');
        expect(fmtCtx.tooltip).toBe('test');
    });

    it('should convert compat formatter that returns string to ColumnFormatter', () => {
        const compatFormatterRetStr: CompatFormatter = (_value, _row, _column, _dataContext, _grid) => 'test';

        const formatter = convertCompatFormatter(compatFormatterRetStr);

        expect(formatter).not.toBe(compatFormatterRetStr);
        expect(formatter).toBeInstanceOf(Function);

        const fmtCtx: FormatterContext = ctx();

        const result = formatter(fmtCtx);

        expect(result).toBe('test');
        expect(fmtCtx.addClass).toBe(undefined);
        expect(fmtCtx.tooltip).toBe(undefined);
    });
});
