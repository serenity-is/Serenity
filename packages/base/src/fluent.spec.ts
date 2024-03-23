import { Fluent } from './fluent';

describe('Fluent instance methods', () => {
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

        it('ignores existing classes', () => {
            element.classList.add("test");
            fluent.addClass(['class1', 'class2', 'test']);

            expect(element.classList.contains('class1')).toBe(true);
            expect(element.classList.contains('class2')).toBe(true);
            expect(element.classList.contains('test')).toBe(true);
        });

        it('ignores falsy classes', () => {
            fluent.addClass(['class1', false && 'class2']);

            expect(element.classList.contains('class1')).toBe(true);
            expect(element.classList.contains('class2')).toBe(false);
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

    describe('empty', () => {
        it('ignores if el is null', () => {
            const f = Fluent(null);
            const result = f.empty();
            expect(f).toBe(result);
        });

        it('should return the Fluent instance', () => {
            const result = fluent.empty();
            expect(result).toBe(fluent);
        });

        it('should remove all child elements', () => {
            const child1 = document.createElement('span');
            const child2 = document.createElement('span');
            child1.append(child2);
            fluent.append(child1);

            fluent.empty();

            expect(fluent.getNode().children.length).toBe(0);
        });

        it('uses jQuery.remove if available', () => {
            const empty = jest.fn();
            (window as any).jQuery = jest.fn().mockReturnValue({ empty });
            try {
                fluent.append(document.createElement('span'));

                fluent.empty();

                expect(empty).toHaveBeenCalledTimes(1);
                expect(fluent.getNode().children.length).toBe(1);
            }
            finally {
                delete (window as any).jQuery;
            }
        });
    });

    describe('removeClass', () => {
        beforeEach(() => {
            element.classList.add('test-class');
            element.classList.add('another-class');
        });

        it('should remove a single class from the element', () => {
            fluent.removeClass('test-class');

            expect(element.classList.contains('test-class')).toBe(false);
            expect(element.classList.contains('another-class')).toBe(true);
        });

        it('should remove multiple classes from the element via an array', () => {
            fluent.removeClass(['test-class', 'another-class']);

            expect(element.classList.contains('test-class')).toBe(false);
            expect(element.classList.contains('another-class')).toBe(false);
        });

        it('ignores false and null values in an array', () => {
            fluent.removeClass([false && 'test-class', true && 'another-class', null]);

            expect(element.classList.contains('test-class')).toBe(true);
            expect(element.classList.contains('another-class')).toBe(false);
        });

        it('should remove multiple classes from the element via space-separated string', () => {
            fluent.removeClass('test-class another-class');

            expect(element.classList.contains('test-class')).toBe(false);
            expect(element.classList.contains('another-class')).toBe(false);
        });

        it('should not throw an error if the class does not exist', () => {
            fluent.removeClass('non-existent-class');

            expect(element.classList.contains('test-class')).toBe(true);
            expect(element.classList.contains('another-class')).toBe(true);
        });
    });


    describe('toggle', () => {

        beforeEach(() => {
            document.body.appendChild(element);
        });

        afterEach(() => {
            document.body.removeChild(element);
        });

        it('should hide the element if it is visible', () => {
            fluent.toggle();

            expect(element.style.display).toBe('none');
        });

        it('should show the element if it is hidden', () => {
            element.style.display = 'none';

            fluent.toggle();

            expect(element.style.display).toBe('');
        });

        it('should hide the element if the flag is false', () => {
            fluent.toggle(false);

            expect(element.style.display).toBe('none');
        });

        it('should show the element if the flag is true', () => {
            element.style.display = 'none';

            fluent.toggle(true);

            expect(element.style.display).toBe('');
        });

        it('should hide the element if the flag is not provided and it is visible', () => {
            element.style.display = 'block';

            fluent.toggle();

            expect(element.style.display).toBe('none');
        });

        it('should show the element if the flag is not provided and it is hidden', () => {
            element.style.display = 'none';

            fluent.toggle();

            expect(element.style.display).toBe('');
        });
    });

    describe('toggleClass', () => {
        beforeEach(() => {
            element.classList.add('test-class');
            element.classList.add('another-class');
        });

        it('should add the class if it does not exist', () => {
            fluent.toggleClass('new-class');

            expect(element.classList.contains('new-class')).toBe(true);
        });

        it('should remove the class if it exists', () => {
            fluent.toggleClass('test-class');

            expect(element.classList.contains('test-class')).toBe(false);
        });

        it('should add the class if the add parameter is true', () => {
            fluent.toggleClass('test-class', true);

            expect(element.classList.contains('test-class')).toBe(true);
        });

        it('should remove the class if the add parameter is false', () => {
            fluent.toggleClass('test-class', false);

            expect(element.classList.contains('test-class')).toBe(false);
        });

        it('should add multiple classes if provided as an array', () => {
            fluent.toggleClass(['new-class', 'another-new-class']);

            expect(element.classList.contains('new-class')).toBe(true);
            expect(element.classList.contains('another-new-class')).toBe(true);
        });

        it('should remove multiple classes if provided as an array', () => {
            fluent.toggleClass(['test-class', 'another-class']);

            expect(element.classList.contains('test-class')).toBe(false);
            expect(element.classList.contains('another-class')).toBe(false);
        });

        it('should add multiple classes if provided as a space-separated string', () => {
            fluent.toggleClass('new-class another-new-class');

            expect(element.classList.contains('new-class')).toBe(true);
            expect(element.classList.contains('another-new-class')).toBe(true);
        });

        it('should remove multiple classes if provided as a space-separated string', () => {
            fluent.toggleClass('test-class another-class');

            expect(element.classList.contains('test-class')).toBe(false);
            expect(element.classList.contains('another-class')).toBe(false);
        });

        it('should ignore false values', () => {
            fluent.toggleClass(['test-class', false && 'another-class']);

            expect(element.classList.contains('test-class')).toBe(false);
            expect(element.classList.contains('another-class')).toBe(true);
        });
    });

});


describe('Fluent static methods', () => {

    let element: HTMLElement;

    beforeEach(() => {
        element = document.createElement('div');
    });

    describe('empty', () => {
        it('ignores if el is null', () => {
            Fluent.empty(null);
        });

        it('should remove all child elements', () => {
            const child1 = document.createElement('span');
            const child2 = document.createElement('span');
            child1.append(child2);
            element.appendChild(child1);

            Fluent.empty(element);

            expect(element.children.length).toBe(0);
        });

        it('uses jQuery.remove if available', () => {
            const empty = jest.fn();
            (window as any).jQuery = jest.fn().mockReturnValue({ empty });
            try {
                element.appendChild(document.createElement('span'));

                Fluent.empty(element);

                expect(empty).toHaveBeenCalledTimes(1);
                expect(element.children.length).toBe(1);
            }
            finally {
                delete (window as any).jQuery;
            }
        });
    });

    describe('eventProp', () => {
        it('should return the value of the specified property from the event object', () => {
            const event = { value: 'Hello, World!' };
            const result = Fluent.eventProp(event, 'value');
            expect(result).toBe('Hello, World!');
        });

        it('should return undefined if the specified property does not exist in the event object', () => {
            const event = { value: 'Hello, World!' };
            const result = Fluent.eventProp(event, 'invalidProperty');
            expect(result).toBeUndefined();
        });

        it('should return undefined if the event object is null', () => {
            const event = null as any;
            const result = Fluent.eventProp(event, 'value');
            expect(result).toBeUndefined();
        });

        it('should return undefined if the event object is undefined', () => {
            const event = undefined as any;
            const result = Fluent.eventProp(event, 'value');
            expect(result).toBeUndefined();
        });

        it('should return value from the originalEvent if it is an object with that prop', () => {
            const event = { originalEvent: { value: 5 } }
            const result = Fluent.eventProp(event, 'value');
            expect(result).toBe(5);
        });

        it('should return value from the detail if it is an object with that prop', () => {
            const event = { detail: { value: 5 } }
            const result = Fluent.eventProp(event, 'value');
            expect(result).toBe(5);
        });
    });

    describe('isInputLike', () => {
        it('should return true for input element', () => {
            const inputElement = document.createElement('input');
            const result = Fluent.isInputLike(inputElement);
            expect(result).toBe(true);
        });

        it('should return true for select element', () => {
            const selectElement = document.createElement('select');
            const result = Fluent.isInputLike(selectElement);
            expect(result).toBe(true);
        });

        it('should return true for textarea element', () => {
            const textareaElement = document.createElement('textarea');
            const result = Fluent.isInputLike(textareaElement);
            expect(result).toBe(true);
        });

        it('should return true for button element', () => {
            const buttonElement = document.createElement('button');
            const result = Fluent.isInputLike(buttonElement);
            expect(result).toBe(true);
        });

        it('should return false for div element', () => {
            const divElement = document.createElement('div');
            const result = Fluent.isInputLike(divElement);
            expect(result).toBe(false);
        });

        it('should return false for null element', () => {
            expect(Fluent.isInputLike(null)).toBe(false);
        });

    });

    describe("isInputTag", () => {
        it("should return true for input tag", () => {
            const result = Fluent.isInputTag("input");
            expect(result).toBe(true);
        });

        it("should return true for select tag", () => {
            const result = Fluent.isInputTag("select");
            expect(result).toBe(true);
        });

        it("should return true for textarea tag", () => {
            const result = Fluent.isInputTag("textarea");
            expect(result).toBe(true);
        });

        it("should return true for button tag", () => {
            const result = Fluent.isInputTag("button");
            expect(result).toBe(true);
        });

        it("should return false for div tag", () => {
            const result = Fluent.isInputTag("div");
            expect(result).toBe(false);
        });
    });

    describe("isVisibleLike", () => {
        it("returns false if el is null", () => {
            expect(Fluent.isVisibleLike(null)).toBe(false);
            expect(Fluent.isVisibleLike(undefined)).toBe(false);
        });

        it("returns false if offsetWidth, offsetHeight and getClientRects().length is 0", () => {
            Object.defineProperties(element, {
                offsetWidth: { get: () => 0 },
                offsetHeight: { get: () => 0 },
                getClientRects: { get: () => (function () { return { length: 0 } }) }
            });
            expect(Fluent.isVisibleLike(element)).toBe(false);
        });

        it("returns true if offsetWidth > 0", () => {
            Object.defineProperties(element, {
                offsetWidth: { get: () => 1 },
                offsetHeight: { get: () => 0 },
                getClientRects: { get: () => (function () { return { length: 0 } }) }
            });
            expect(Fluent.isVisibleLike(element)).toBe(true);
        });

        it("returns true if offsetHeight > 0", () => {
            Object.defineProperties(element, {
                offsetWidth: { get: () => 0 },
                offsetHeight: { get: () => 1 },
                getClientRects: { get: () => (function () { return { length: 0 } }) }
            });
            expect(Fluent.isVisibleLike(element)).toBe(true);
        });

        it("returns true if clientRects.length > 0", () => {
            Object.defineProperties(element, {
                offsetWidth: { get: () => 0 },
                offsetHeight: { get: () => 0 },
                getClientRects: { get: () => (function () { return { length: 1 } }) }
            });
            expect(Fluent.isVisibleLike(element)).toBe(true);
        });
    });
});

