import {
    absBox,
    getInnerWidth,
    getMaxSupportedCssHeight
} from '../../src/grid/style-utils';

describe('getInnerWidth', () => {
    const style = {
        boxSizing: 'content-box',
        ['border-left-width']: '1px',
        ['border-right-width']: '2px',
        ['padding-left']: '4px',
        ['padding-right']: '8px',
        width: '100px',
        getPropertyValue: (property: string) => style[property]
    };
    const totalHBoxDelta = 1 + 2 + 4 + 8;

    let oldGetComputedStyle: any;
    beforeAll(() => {
        oldGetComputedStyle = window.getComputedStyle;
        window.getComputedStyle = () => style as any;
    });

    afterAll(() => {
        window.getComputedStyle = oldGetComputedStyle;
    });

    it('should return width if box-sizing is not border-box', () => {
        const element = document.createElement('div');
        style.boxSizing = 'content-box';

        expect(getInnerWidth(element)).toBe(100);
    });

    it('should return width - horizontal padding + border if box-sizing is border-box', () => {
        const element = document.createElement('div');
        style.boxSizing = 'border-box';

        expect(getInnerWidth(element)).toBe(100 - totalHBoxDelta);
    });

    it('should return 0 if width is not set', () => {
        const element = document.createElement('div');
        style.width = '';

        expect(getInnerWidth(element)).toBe(0);
    });

    it('should return 0 if width is not a number', () => {
        const element = document.createElement('div');
        style.width = 'foo';

        expect(getInnerWidth(element)).toBe(0);
    });

    it('should return 0 if width is negative', () => {
        const element = document.createElement('div');
        style.width = '-1px';

        expect(getInnerWidth(element)).toBe(0);
    });
});

describe('getMaxSupportedCssHeight', () => {
    it('should return 4000000 for gecko', () => {
        const oldNavigator = window.navigator;
        Object.defineProperty(window, 'navigator', {
            value: {
                userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:95.0) Gecko/20100101 Firefox/95.0',
            },
            configurable: true,
            writable: true
        });

        expect(getMaxSupportedCssHeight(true)).toBe(4000000);

        Object.defineProperty(window, 'navigator', {
            value: oldNavigator,
            configurable: true,
            writable: true
        });
    });

    it('should return 32000000 for other browsers', () => {
        const userAgents = [
            'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36', // Chrome Generic Windows 10
            'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.3', // Chrome Generic macOs
            'Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36', // Chrome Generic Linux
            'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/16.1 Safari/605.1.15', // Safari 16.1 macOs
        ];

        userAgents.forEach(userAgent => {
            const oldNavigator = window.navigator;
            Object.defineProperty(window, 'navigator', {
                value: {
                    userAgent
                },
                configurable: true,
                writable: true
            });

            expect(userAgent + ": " + getMaxSupportedCssHeight(true)).toBe(userAgent + ": " + 32000000); // concat to make it easier to debug

            Object.defineProperty(window, 'navigator', {
                value: oldNavigator,
                configurable: true,
                writable: true
            });
        });
    });
});

describe('absBox', () => {
    let htmlProtoGetSet = ["offsetLeft", "offsetTop", "offsetWidth", "offsetHeight", "offsetParent", "scrollHeight", "scrollWidth", "scrollLeft", "scrollTop", "clientHeight"];
    const oldProperties: PropertyDescriptor[] = []
    beforeAll(() => {
        htmlProtoGetSet.forEach(prop => {
            oldProperties[prop] = Object.getOwnPropertyDescriptor(HTMLElement.prototype, prop); // probably will be undefined due to jsdom

            Object.defineProperty(window.HTMLElement.prototype, prop, {
                configurable: true,
                get: function () {
                    return this[`_${prop}`] ?? 0;
                },
                set(val) {
                    this[`_${prop}`] = val;
                }
            });
        });
    });

    afterAll(() => {
        htmlProtoGetSet.forEach(prop => {
            window.HTMLElement.prototype[prop] = oldProperties[prop];
        });
    });

    it('should return the correct box', () => {
        const element = document.createElement('div');

        element["_offsetTop"] = 11;
        element["_offsetHeight"] = 88;
        element["_offsetLeft"] = 22;
        element["_offsetWidth"] = 44;

        const box = absBox(element);
        expect(box.top).toBe(11);
        expect(box.height).toBe(88);
        expect(box.left).toBe(22);
        expect(box.width).toBe(44);

        expect(box.bottom).toBe(99); // 11 (top) + 88 (height)
        expect(box.right).toBe(66); // 22 (left) + 44 (width)

        expect(box.visible).toBe(true);
    });

    it('should return not visible when the box bottom is smaller than parents scrollTop', () => {
        const element = document.createElement('div');

        element["_offsetTop"] = 10;
        element["_offsetHeight"] = 10;

        const parentElement = document.createElement('div');
        parentElement.appendChild(element);
        parentElement["_scrollTop"] = 21;
        parentElement["_scrollHeight"] = 20;

        const box = absBox(element);
        expect(box.visible).toBe(false);
    });

    it('should return visible when the box bottom is bigger to parents scrollTop', () => {
        const element = document.createElement('div');

        element["_offsetTop"] = 10;
        element["_offsetHeight"] = 10;

        const parentElement = document.createElement('div');
        parentElement.appendChild(element);
        parentElement["_scrollTop"] = 9;
        parentElement["_clientHeight"] = 100;

        const box = absBox(element);
        expect(box.visible).toBe(true);
    });

    it('should return not visible when the box top is bigger than parents scrollTop + clientHeight', () => {
        const element = document.createElement('div');

        element["_offsetTop"] = 30;
        element["_offsetHeight"] = 10;

        const parentElement = document.createElement('div');
        parentElement.appendChild(element);
        parentElement["_scrollTop"] = 21;
        parentElement["_scrollHeight"] = 20;

        const box = absBox(element);
        expect(box.visible).toBe(false);
    });

    it('should return visible when the box top is smaller than parents scrollTop + clientHeight', () => {
        const element = document.createElement('div');

        element["_offsetTop"] = 20;
        element["_offsetHeight"] = 10;

        const parentElement = document.createElement('div');
        parentElement.appendChild(element);
        parentElement["_scrollTop"] = 21;
        parentElement["_clientHeight"] = 100;

        const box = absBox(element);
        expect(box.visible).toBe(true);
    });

    it('should return not visible when the box right is smaller than parents scrollLeft', () => {
        const element = document.createElement('div');

        element["_offsetLeft"] = 10;
        element["_offsetWidth"] = 10;

        const parentElement = document.createElement('div');
        parentElement.appendChild(element);
        parentElement["_scrollLeft"] = 21;
        parentElement["_scrollWidth"] = 20;

        const box = absBox(element);
        expect(box.visible).toBe(false);
    });

    it('should return visible when the box right is bigger to parents scrollLeft', () => {
        const element = document.createElement('div');

        element["_offsetLeft"] = 10;
        element["_offsetWidth"] = 10;

        const parentElement = document.createElement('div');
        parentElement.appendChild(element);
        parentElement["_scrollLeft"] = 9;
        parentElement["_clientWidth"] = 100;

        const box = absBox(element);
        expect(box.visible).toBe(true);
    });

    it('should return not visible when the box left is bigger than parents scrollLeft + clientWidth', () => {
        const element = document.createElement('div');

        element["_offsetLeft"] = 30;
        element["_offsetWidth"] = 10;

        const parentElement = document.createElement('div');
        parentElement.appendChild(element);
        parentElement["_scrollLeft"] = 21;
        parentElement["_scrollWidth"] = 20;

        const box = absBox(element);
        expect(box.visible).toBe(false);
    });

    it('should return visible when the box left is smaller than parents scrollLeft + clientWidth', () => {
        const element = document.createElement('div');

        element["_offsetLeft"] = 20;
        element["_offsetWidth"] = 10;

        const parentElement = document.createElement('div');
        parentElement.appendChild(element);
        parentElement["_scrollLeft"] = 21;
        parentElement["_clientWidth"] = 100;

        const box = absBox(element);
        expect(box.visible).toBe(true);
    });

    it('should return the correct box when the parent element is scrolled', () => {
        const element = document.createElement('div');

        element["_offsetTop"] = 10;
        element["_offsetHeight"] = 20;
        element["_offsetLeft"] = 40;
        element["_offsetWidth"] = 80;

        const parentElement = document.createElement('div');
        parentElement.appendChild(element);
        parentElement["_scrollTop"] = 10;
        parentElement["_scrollLeft"] = 20;
        parentElement["_clientHeight"] = 80;
        parentElement["_clientWidth"] = 160;

        const box = absBox(element);
        expect(box.top).toBe(0); // 10 (element offsetTop) - 10 (parent scrollTop)
        expect(box.height).toBe(20);
        expect(box.left).toBe(20); // 40 (element offsetLeft) - 20 (parent scrollLeft)
        expect(box.width).toBe(80);

        expect(box.bottom).toBe(20);
        expect(box.right).toBe(100);

        expect(box.visible).toBe(true);
    });

    it('should return the correct box when the parent element is scrolled and we have an offsetParent', () => {
        const element = document.createElement('div');

        element["_offsetTop"] = 10;
        element["_offsetHeight"] = 20;
        element["_offsetLeft"] = 40;
        element["_offsetWidth"] = 80;

        const parentElement = document.createElement('div');
        parentElement.appendChild(element);
        parentElement["_scrollTop"] = 10;
        parentElement["_scrollLeft"] = 20;
        parentElement["_clientHeight"] = 80;
        parentElement["_clientWidth"] = 160;

        const offsetParent = document.createElement('td');
        offsetParent.appendChild(parentElement);
        element["_offsetParent"] = offsetParent;
        offsetParent["_scrollTop"] = 3;
        offsetParent["_scrollLeft"] = 3;
        offsetParent["_offsetTop"] = 15;
        offsetParent["_offsetLeft"] = 25;
        offsetParent["_clientHeight"] = 45;
        offsetParent["_clientWidth"] = 85;

        const box = absBox(element);
        expect(box.top).toBe(12); // 10 (element offsetTop) - 10 (parent offsetTop) - 3 (offsetParent scrollTop) + 15 (offsetParent offsetTop)
        expect(box.height).toBe(20);
        expect(box.left).toBe(42); // 40 (element offsetLeft) - 20 (parent offsetLeft) - 3 (offsetParent scrollLeft) + 25 (offsetParent offsetLeft)
        expect(box.width).toBe(80);

        expect(box.bottom).toBe(32);
        expect(box.right).toBe(122);

        expect(box.visible).toBe(true);
    });
});
