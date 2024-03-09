import { Fluent } from './fluent';

describe('Fluent', () => {
    let element: HTMLElement;
    let fluent: Fluent<HTMLElement>;

    beforeEach(() => {
        element = document.createElement('div');
        fluent = Fluent(element);
    });

    describe('addClass', () => {
        it('should add class to the element', () => {
            fluent.addClass('test-class');

            expect(element.classList.contains('test-class')).toBe(true);
        });

        it('should add multiple classes to the element', () => {
            fluent.addClass(['class1', 'class2']);

            expect(element.classList.contains('class1')).toBe(true);
            expect(element.classList.contains('class2')).toBe(true);
        });
    });

    describe('append', () => {
        it('should append child to the element', () => {
            const child = document.createElement('span');
            fluent.append(child);

            expect(element.lastChild).toBe(child);
        });

        it('should append string as text child to the element', () => {
            const child = '<span>Test</span>';
            fluent.append(child);

            expect((element.lastChild as HTMLElement)?.textContent).toBe(child);
        });

        it('should append Fluent instance as child to the element', () => {
            const childElement = document.createElement('span');
            const childFluent = Fluent(childElement);
            fluent.append(childFluent);

            expect(element.lastChild).toBe(childElement);
        });
    });

    describe('appendTo', () => {
        let parent: HTMLElement;

        beforeEach(() => {
            parent = document.createElement('div');
        });

        it('should append the element to the parent', () => {
            fluent.appendTo(parent);

            expect(parent.lastChild).toBe(element);
        });

        it('should remove the element if parent is null', () => {
            fluent.appendTo(null);

            expect(element.parentNode).toBe(null);
        });

        it('should remove the element if parent is undefined', () => {
            fluent.appendTo(undefined);

            expect(element.parentNode).toBe(null);
        });
    });

    describe('attr', () => {
        it('should get the attribute value', () => {
            element.setAttribute('data-test', 'value');

            const value = fluent.attr('data-test');

            expect(value).toBe('value');
        });

        it('should set the attribute value', () => {
            fluent.attr('data-test', 'value');

            expect(element.getAttribute('data-test')).toBe('value');
        });

        it('should remove the attribute if value is null', () => {
            element.setAttribute('data-test', 'value');

            fluent.attr('data-test', null);

            expect(element.hasAttribute('data-test')).toBe(false);
        });

        it('should remove the attribute if value is undefined', () => {
            element.setAttribute('data-test', 'value');

            fluent.attr('data-test', undefined);

            expect(element.hasAttribute('data-test')).toBe(false);
        });

        it('should set the attribute value to empty string if value is false', () => {
            fluent.attr('data-test', false);

            expect(element.getAttribute('data-test')).toBe(null);
        });

        it('should set the attribute value to "true" if value is true', () => {
            fluent.attr('data-test', true);

            expect(element.getAttribute('data-test')).toBe('true');
        });

        it('should convert number value to string', () => {
            fluent.attr('data-test', 123);

            expect(element.getAttribute('data-test')).toBe('123');
        });
    });

    describe('children', () => {
        beforeEach(() => {
            element.innerHTML = `<div class="child"></div><span class="child"></span><div></div>`;
        });

        it('should return all children of the element', () => {
            const children = fluent.children();

            expect(children.length).toBe(3);
            expect(children[0].classList.contains('child')).toBe(true);
            expect(children[1].classList.contains('child')).toBe(true);
        });

        it('should return children matching the selector', () => {
            const children = fluent.children('.child');

            expect(children.length).toBe(2);
            expect(children[0].classList.contains('child')).toBe(true);
            expect(children[1].classList.contains('child')).toBe(true);
        });
    });
});