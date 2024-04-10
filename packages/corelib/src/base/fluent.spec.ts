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

    describe('after', () => {
        let parent: HTMLElement;

        beforeEach(() => {
            parent = document.createElement('div');
            fluent.appendTo(parent);
        });

        it('should ignore null', () => {
            fluent.after(null);
            expect(element.previousSibling).toBe(null);
            expect(element.nextSibling).toBe(null);
            expect(element.parentElement).toBe(parent);
            expect(element.parentElement.childNodes.length).toBe(1);
        });

        it('should ignore empty Fluent', () => {
            fluent.before(Fluent(null));
            expect(element.previousSibling).toBe(null);
            expect(element.nextSibling).toBe(null);
            expect(element.parentElement).toBe(parent);
            expect(element.parentElement.childNodes.length).toBe(1);
        });

        it('should insert the content node after itself', () => {
            const after = document.createElement('span');
            fluent.after(after);
            expect(element.nextSibling).toBe(after);
            expect(after.parentElement).toBe(parent);
            expect(element.parentElement).toBe(parent);
            expect(element.parentElement.childNodes.length).toBe(2);
        });

        it('should insert the text after itself', () => {
            fluent.after("test");
            expect(element.nextSibling instanceof Text).toBe(true);
            expect(element.nextSibling.textContent).toBe("test");
            expect(element.parentElement).toBe(parent);
            expect(element.parentElement.childNodes.length).toBe(2);
        });

        it('can insert fragment contents', () => {
            const fragment = new DocumentFragment();
            const span = fragment.appendChild(document.createElement("span"));
            const input = fragment.appendChild(document.createElement("input"));
            fluent.after(fragment);
            expect(element.nextSibling).toBe(span);
            expect(element.nextSibling.nextSibling).toBe(input);
            expect(element.parentElement).toBe(parent);
            expect(element.parentElement.childNodes.length).toBe(3);
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

    describe('before', () => {
        let parent: HTMLElement;

        beforeEach(() => {
            parent = document.createElement('div');
            fluent.appendTo(parent);
        });

        it('should ignore null', () => {
            fluent.before(null);
            expect(element.previousSibling).toBe(null);
            expect(element.nextSibling).toBe(null);
            expect(element.parentElement).toBe(parent);
            expect(element.parentElement.childNodes.length).toBe(1);
        });

        it('should ignore empty Fluent', () => {
            fluent.before(Fluent(null));
            expect(element.previousSibling).toBe(null);
            expect(element.nextSibling).toBe(null);
            expect(element.parentElement).toBe(parent);
            expect(element.parentElement.childNodes.length).toBe(1);
        });

        it('should insert the content node before itself', () => {
            const before = document.createElement('span');
            fluent.before(before);
            expect(element.previousSibling).toBe(before);
            expect(before.parentElement).toBe(parent);
            expect(element.parentElement).toBe(parent);
            expect(element.parentElement.childNodes.length).toBe(2);
        });

        it('should insert the text before itself', () => {
            fluent.before("test");
            expect(element.previousSibling instanceof Text).toBe(true);
            expect(element.previousSibling.textContent).toBe("test");
            expect(element.parentElement).toBe(parent);
            expect(element.parentElement.childNodes.length).toBe(2);
        });

        it('can insert fragment contents', () => {
            const fragment = new DocumentFragment();
            const span = fragment.appendChild(document.createElement("span"));
            const input = fragment.appendChild(document.createElement("input"));
            fluent.before(fragment);
            expect(element.previousSibling).toBe(input);
            expect(element.previousSibling.previousSibling).toBe(span);
            expect(element.parentElement).toBe(parent);
            expect(element.parentElement.childNodes.length).toBe(3);
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

    describe("click", () => {
        it('should ignore null element', () => {
            Fluent(null).click();
        });

        it('should trigger click event with no arguments', () => {
            const onClickSpy = jest.fn();
            fluent.getNode().addEventListener("click", onClickSpy);
            fluent.click();
            expect(onClickSpy).toHaveBeenCalledTimes(1);
        });

        it("should call elements click method with no arguments", () => {
            const clickSpy = jest.spyOn(fluent.getNode(), "click");
            fluent.click();
            expect(clickSpy).toHaveBeenCalledTimes(1);
        });

        it("should ignore if element has no click method", () => {
            const onClickSpy = jest.fn();
            fluent.getNode().addEventListener("click", onClickSpy);
            fluent.getNode().click = null;
            fluent.click();
            expect(onClickSpy).not.toHaveBeenCalled();
        });

        it('should add click event handler', () => {
            const clickSpy = jest.fn();
            fluent.click(clickSpy);
            fluent.getNode().click();
            expect(clickSpy).toHaveBeenCalledTimes(1);
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

    describe('findAll', () => {
        it('should find all elements that match the specified selector within the element', () => {
            element.innerHTML = `
                    <span class="item">Item 1</span>
                    <span class="item">Item 2</span>
                    <span class="item">Item 3</span>
                `;

            const result = fluent.findAll('.item');

            expect(result.length).toBe(3);
            expect(result[0].textContent).toBe('Item 1');
            expect(result[1].textContent).toBe('Item 2');
            expect(result[2].textContent).toBe('Item 3');
        });

        it('should return an empty array if no elements match the specified selector', () => {
            element.innerHTML = `
                    <span class="item">Item 1</span>
                    <span class="item">Item 2</span>
                    <span class="item">Item 3</span>
                `;

            const result = fluent.findAll('.non-existent');

            expect(result.length).toBe(0);
        });
    });

    describe('findEach', () => {
        it('should find each element that matches the specified selector within the element and execute a callback function for each found element', () => {
            element.innerHTML = `
                    <span class="item">Item 1</span>
                    <span class="item">Item 2</span>
                    <span class="item">Item 3</span>
                `;

            const callback = jest.fn();
            fluent.findEach('.item', callback);

            expect(callback).toHaveBeenCalledTimes(3);
            expect(callback.mock.calls[0][0].getNode().textContent).toBe('Item 1');
            expect(callback.mock.calls[1][0].getNode().textContent).toBe('Item 2');
            expect(callback.mock.calls[2][0].getNode().textContent).toBe('Item 3');
        });

        it('should not execute the callback function if no elements match the specified selector', () => {
            element.innerHTML = `
                    <span class="item">Item 1</span>
                    <span class="item">Item 2</span>
                    <span class="item">Item 3</span>
                `;

            const callback = jest.fn();
            fluent.findEach('.non-existent', callback);

            expect(callback).not.toHaveBeenCalled();
        });
    });

    describe('findFirst', () => {
        it('should find the first element that matches the specified selector within the element', () => {
            element.innerHTML = `
                    <span class="item">Item 1</span>
                    <span class="item">Item 2</span>
                    <span class="item">Item 3</span>
                `;

            const result = fluent.findFirst('.item');

            expect(result.getNode().textContent).toBe('Item 1');
        });

        it('should return null if no elements match the specified selector', () => {
            element.innerHTML = `
                    <span class="item">Item 1</span>
                    <span class="item">Item 2</span>
                    <span class="item">Item 3</span>
                `;

            const result = fluent.findFirst('.non-existent');

            expect(result.getNode()).toBeNull();
        });
    });

    describe("focus", () => {
        it('should ignore null element', () => {
            Fluent(null).focus();
        });

        it("should call elements focus method", () => {
            const focusSpy = jest.spyOn(fluent.getNode(), "focus");
            fluent.focus();
            expect(focusSpy).toHaveBeenCalledTimes(1);
        });

        it("should ignore if element has no focus method", () => {
            fluent.getNode().focus = null;
            fluent.focus();
        });
    });

    describe('nextSibling', () => {

        it('should return the next sibling of the element', () => {
            document.createElement("div").appendChild(element);
            const nextElement = document.createElement('span');
            element.parentNode.insertBefore(nextElement, element.nextSibling);

            expect(fluent.nextSibling().getNode()).toBe(nextElement);
        });

        it('should not return the prev sibling of the element', () => {
            document.createElement("div").appendChild(element);
            const nextElement = document.createElement('span');
            element.parentNode.prepend(document.createElement("span"));
            element.parentNode.append(nextElement, element.nextSibling);
            expect(fluent.nextSibling().getNode()).toBe(nextElement);
        });

        it('should return the next sibling matching the selector', () => {
            document.createElement("div").appendChild(element);
            const nextElement = document.createElement('span');
            nextElement.classList.add("test");
            element.parentNode.append(document.createElement("span"));
            element.parentNode.append(nextElement);
            expect(fluent.nextSibling(".test").getNode()).toBe(nextElement);
        });

        it('should return null if no next element', () => {
            document.createElement("div").appendChild(element);
            element.parentNode.prepend(document.createElement("span"));
            expect(fluent.nextSibling().getNode()).toBe(null);
        });

        it('should return null if no next matching element', () => {
            document.createElement("div").appendChild(element);
            element.parentNode.append(document.createElement("span"));
            expect(fluent.nextSibling(".test").getNode()).toBe(null);
        });

    });

    describe('prevSibling', () => {

        it('should return the previous sibling of the element', () => {
            document.createElement("div").appendChild(element);
            const prevElement = document.createElement('span');
            element.parentNode?.insertBefore(prevElement, element);

            expect(fluent.prevSibling().getNode()).toBe(prevElement);
        });


        it('should not return the next sibling of the element', () => {
            document.createElement("div").appendChild(element);
            const prevElement = document.createElement('span');
            element.parentNode.append(document.createElement("span"));
            element.parentNode.prepend(prevElement);
            expect(fluent.prevSibling().getNode()).toBe(prevElement);
        });

        it('should return the prev sibling matching the selector', () => {
            document.createElement("div").appendChild(element);
            const prevElement = document.createElement('span');
            prevElement.classList.add("test");
            element.parentNode.prepend(document.createElement("span"));
            element.parentNode.prepend(prevElement);
            expect(fluent.prevSibling(".test").getNode()).toBe(prevElement);
        });

        it('should return null if no prev element', () => {
            document.createElement("div").appendChild(element);
            element.parentNode.prepend(document.createElement("span"));
            expect(fluent.nextSibling().getNode()).toBe(null);
        });

        it('should return null if no prev matching element', () => {
            document.createElement("div").appendChild(element);
            element.parentNode.prepend(document.createElement("span"));
            expect(fluent.prevSibling(".test").getNode()).toBe(null);
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

    describe('show', () => {

        beforeEach(() => {
            fluent = Fluent(element);
        });

        it('should ignore null element', () => {
            fluent = Fluent(null);
            const result = fluent.show();
            expect(result).toBe(fluent);
        });

        it('should set the display property to the default value', () => {
            element.style.display = 'none';

            const result = fluent.show();

            expect(element.style.display).toBe('');
            expect(result).toBe(fluent);
        });
    });

    describe('style', () => {
        it('should apply styles to the element', () => {
            const callback = (css: CSSStyleDeclaration) => {
                css.setProperty('color', 'red');
                css.setProperty('font-size', '16px');
            };

            fluent.style(callback);

            expect(element.style.color).toBe('red');
            expect(element.style.fontSize).toBe('16px');
        });

        it('should override existing styles', () => {
            element.style.color = 'blue';
            element.style.fontSize = '14px';

            const callback = (css: CSSStyleDeclaration) => {
                css.setProperty('color', 'red');
                css.setProperty('font-size', '16px');
            };

            fluent.style(callback);

            expect(element.style.color).toBe('red');
            expect(element.style.fontSize).toBe('16px');
        });

        it('should keep styles when callback is empty', () => {
            element.style.color = 'blue';
            element.style.fontSize = '14px';

            fluent.style(() => { });

            expect(element.style.color).toBe('blue');
            expect(element.style.fontSize).toBe('14px');
        });

        it('should ignore when element is null', () => {
            fluent = Fluent(null);
            const callback = jest.fn();
            fluent.style(callback);
            expect(callback).not.toHaveBeenCalled();
        });
    });

    describe('text', () => {

        it('should set the text content of the element', () => {
            fluent.text('Hello, world!');

            expect(element.textContent).toBe('Hello, world!');
        });

        it('should return the text content of the element', () => {
            element.textContent = 'Hello, world!';

            const result = fluent.text();

            expect(result).toBe('Hello, world!');
        });

        it('should return undefined for null element', () => {
            fluent = Fluent(null);

            const result = fluent.text();

            expect(result).toBeUndefined();
        });

        it('should return fluent instance for null element', () => {
            fluent = Fluent(null);

            const result = fluent.text("test");

            expect(result).toBe(fluent);
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

    describe('val', () => {
        let element: HTMLInputElement;
        let fluent: Fluent<HTMLInputElement>;

        beforeEach(() => {
            element = document.createElement('input');
            fluent = Fluent(element);
        });

        it('should return the current value of the input element', () => {
            element.value = 'Hello World';

            const result = fluent.val();

            expect(result).toBe('Hello World');
        });

        it('should set the value of the input element', () => {
            fluent.val('New Value');

            expect(element.value).toBe('New Value');
        });

        it('should chain the fluent object', () => {
            const result = fluent.val('New Value');

            expect(result).toBe(fluent);
        });
    });

});


describe('Fluent static methods', () => {

    let element: HTMLElement;

    beforeEach(() => {
        element = document.createElement('div');
    });

    afterEach(() => {
        element.remove();
    });

    describe('byId', () => {
        it('should return a Fluent object with the specified element', () => {
            const elementId = 'myElementId';
            const element = document.createElement('div');
            element.id = elementId;
            document.body.appendChild(element);

            const fluent = Fluent.byId(elementId);

            expect(fluent.length).toBe(1);
            expect(fluent.getNode()).toBe(element);
        });

        it('should return an empty Fluent object if the element does not exist', () => {
            const elementId = 'nonExistentElementId';

            const fluent = Fluent.byId(elementId);

            expect(fluent.length).toBe(0);
        });
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

    describe('findAll', () => {
        it('should find all elements that match the specified selector within the element', () => {
            document.body.append(element);
            element.innerHTML = `
                    <span class="item">Item 1</span>
                    <span class="item">Item 2</span>
                    <span class="item">Item 3</span>
                `;

            const result = Fluent.findAll('.item');

            expect(result.length).toBe(3);
            expect(result[0].textContent).toBe('Item 1');
            expect(result[1].textContent).toBe('Item 2');
            expect(result[2].textContent).toBe('Item 3');
        });

        it('should return an empty array if no elements match the specified selector', () => {
            document.body.append(element);
            element.innerHTML = `
                    <span class="item">Item 1</span>
                    <span class="item">Item 2</span>
                    <span class="item">Item 3</span>
                `;

            const result = Fluent.findAll('.non-existent');

            expect(result.length).toBe(0);
        });
    });

    describe('findEach', () => {
        it('should find each element that matches the specified selector within the element and execute a callback function for each found element', () => {
            document.body.append(element);
            element.innerHTML = `
                    <span class="item">Item 1</span>
                    <span class="item">Item 2</span>
                    <span class="item">Item 3</span>
                `;

            const callback = jest.fn();
            Fluent.findEach('.item', callback);

            expect(callback).toHaveBeenCalledTimes(3);
            expect(callback.mock.calls[0][0].getNode().textContent).toBe('Item 1');
            expect(callback.mock.calls[1][0].getNode().textContent).toBe('Item 2');
            expect(callback.mock.calls[2][0].getNode().textContent).toBe('Item 3');
        });

        it('should not execute the callback function if no elements match the specified selector', () => {
            document.body.append(element);
            element.innerHTML = `
                    <span class="item">Item 1</span>
                    <span class="item">Item 2</span>
                    <span class="item">Item 3</span>
                `;

            const callback = jest.fn();
            Fluent.findEach('.non-existent', callback);

            expect(callback).not.toHaveBeenCalled();
        });
    });

    describe('findFirst', () => {
        it('should find the first element that matches the specified selector within the element', () => {
            document.body.appendChild(element);
            element.innerHTML = `
                    <span class="item">Item 1</span>
                    <span class="item">Item 2</span>
                    <span class="item">Item 3</span>
                `;

            const result = Fluent.findFirst('.item');

            expect(result.getNode().textContent).toBe('Item 1');
        });

        it('should return null if no elements match the specified selector', () => {
            document.body.appendChild(element);
            element.innerHTML = `
                    <span class="item">Item 1</span>
                    <span class="item">Item 2</span>
                    <span class="item">Item 3</span>
                `;

            const result = Fluent.findFirst('.non-existent');

            expect(result.getNode()).toBeNull();
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

    describe("remove", () => {
        it("should ignore null and undefined", () => {
            Fluent.remove(null);
            Fluent.remove(undefined);
        });

        it("should remove the element from the DOM", () => {
            document.body.appendChild(element);
            expect(document.body.contains(element)).toBe(true);
            Fluent.remove(element);
            expect(document.body.contains(element)).toBe(false);
        });

        it("should remove event listeners", () => {
            document.body.appendChild(element);

            const listener = jest.fn();
            Fluent.on(element, "click", listener);
            
            Fluent.remove(element);

            element.click();
            expect(listener).not.toHaveBeenCalled();
        });
    });

    describe("toClassName", () => {
        it("should convert a string to a valid CSS class name", () => {
            const className = Fluent.toClassName("my-class");
            expect(className).toBe("my-class");
        });

        it("should leave a string with spaces as is", () => {
            const className = Fluent.toClassName("my class");
            expect(className).toBe("my class");
        });

        it("should convert an empty string to an empty CSS class name", () => {
            const className = Fluent.toClassName("");
            expect(className).toBe("");
        });

        it("should convert numbers to string", () => {
            const className = Fluent.toClassName(1 as any);
            expect(className).toBe("1");
        });

    });
});

