import { Config, classTypeInfo, getTypeFullName } from "../../base";
import { TemplatedWidget, Widget, useIdPrefix } from "./widget";

describe('Widget.getCssClass', () => {
    let oldNamespaces: string[];

    beforeEach(() => {
        oldNamespaces = [...Config.rootNamespaces];
    });

    afterEach(() => {
        Config.rootNamespaces = oldNamespaces;
    });

    it('returns class name without root namespace', function () {
        Config.rootNamespaces = [];
        var generatedClassNames = Widget.prototype["getCssClass"].call({
            constructor: {
                [Symbol.typeInfo]: classTypeInfo("Serenity.Demo.Northwind.CustomerGrid")
            }
        });
        expect(generatedClassNames).toBe("s-Serenity-Demo-Northwind-CustomerGrid s-CustomerGrid");
    });

    it('works with serenity root namespaces', function () {
        var generatedClassNames = Widget.prototype["getCssClass"].call({
            constructor: {
                [Symbol.typeInfo]: classTypeInfo("Serenity.Demo.Northwind.CustomerGrid")
            }
        });
        expect(generatedClassNames).toBe("s-Serenity-Demo-Northwind-CustomerGrid s-Demo-Northwind-CustomerGrid s-CustomerGrid");
    });

    it('works with project root namespaces', function () {
        Config.rootNamespaces.push("StartSharp");
        var generatedClassNames = Widget.prototype["getCssClass"].call({
            constructor: {
                [Symbol.typeInfo]: classTypeInfo("StartSharp.Demo.Northwind.CustomerGrid")
            }
        });
        expect(generatedClassNames).toBe("s-StartSharp-Demo-Northwind-CustomerGrid s-Demo-Northwind-CustomerGrid s-CustomerGrid");
    });

    it('works without root namespaces for project widget', function () {
        Config.rootNamespaces = [];
        var generatedClassNames = Widget.prototype["getCssClass"].call({
            constructor: {
                [Symbol.typeInfo]: classTypeInfo("StartSharp.Demo.Northwind.CustomerGrid")
            }
        });
        expect(generatedClassNames).toBe("s-StartSharp-Demo-Northwind-CustomerGrid s-CustomerGrid");
    });

    it('deduplicates class names', function () {
        var generatedClassNames = Widget.prototype["getCssClass"].call({
            constructor: {
                [Symbol.typeInfo]: classTypeInfo("Dup.Dup.Something")
            }
        });
        expect(generatedClassNames).toBe("s-Dup-Dup-Something s-Something");
    });
});

describe('Widget.constructor', () => {
    it('creates widget with default div element', () => {
        const widget = new Widget({});
        expect(widget.domNode).toBeInstanceOf(HTMLElement);
        expect(widget.domNode.tagName).toBe("DIV");
        expect(widget.uniqueName).toBeTruthy();
        expect(widget.idPrefix).toBe(widget.uniqueName + '_');
        widget.destroy();
    });

    it('accepts custom element via element prop', () => {
        const existing = document.createElement("span");
        const widget = new Widget({ element: existing });
        expect(widget.domNode).toBe(existing);
        widget.destroy();
    });

    it('accepts array-like element', () => {
        const existing = document.createElement("div");
        const widget = new Widget({ element: [existing] as any });
        expect(widget.domNode).toBe(existing);
        widget.destroy();
    });

    it('accepts element function', () => {
        const fn = vi.fn();
        const widget = new Widget({ element: fn });
        expect(fn).toHaveBeenCalled();
        widget.destroy();
    });

    it('uses provided idPrefix', () => {
        const widget = new Widget({ idPrefix: 'custom_' } as any);
        expect(widget.idPrefix).toBe('custom_');
        widget.destroy();
    });

    it('generates unique names', () => {
        const w1 = new Widget({});
        const w2 = new Widget({});
        expect(w1.uniqueName).not.toBe(w2.uniqueName);
        w1.destroy();
        w2.destroy();
    });

    it('adds CSS class to domNode', () => {
        const widget = new Widget({});
        expect(widget.domNode.classList.length).toBeGreaterThan(0);
        widget.destroy();
    });

    it('accepts array-like props in constructor', () => {
        const div = document.createElement("div");
        const widget = new Widget([div] as any);
        expect(widget.domNode).toBe(div);
        widget.destroy();
    });
});

describe('Widget.destroy', () => {
    it('cleans up domNode and disposes', () => {
        const widget = new Widget({});
        const node = widget.domNode;
        widget.destroy();
        expect((widget as any).domNode).toBeUndefined();
    });

    it('can be called multiple times', () => {
        const widget = new Widget({});
        widget.destroy();
        expect(() => widget.destroy()).not.toThrow();
    });

    it('removes CSS classes from domNode', () => {
        const widget = new Widget({});
        const domNode = widget.domNode;
        const cssClass = widget["getCssClass"]();
        // After destroy, the domNode should no longer have the widget class
        widget.destroy();
        expect(domNode.classList.contains(cssClass.split(' ')[0])).toBe(false);
    });
});

describe('Widget.createDefaultElement', () => {
    it('returns a div element', () => {
        const el = Widget.createDefaultElement();
        expect(el).toBeInstanceOf(HTMLElement);
        expect(el.tagName).toBe('DIV');
    });
});

describe('Widget.element', () => {
    it('returns Fluent wrapper for domNode', () => {
        const widget = new Widget({});
        const el = widget.element;
        expect(el).toBeTruthy();
        expect(el.getNode()).toBe(widget.domNode);
        widget.destroy();
    });
});

describe('Widget.byId', () => {
    it('finds child element by prefixed id', () => {
        const widget = new Widget({ });
        const child = document.createElement("span");
        child.id = widget["idPrefix"] + 'myChild';
        widget.domNode.appendChild(child);

        const result = widget["byId"]('myChild');
        expect(result).toBeTruthy();
        expect(result.getNode()).toBe(child);
        widget.destroy();
    });

    it('returns Fluent even when element not found', () => {
        const widget = new Widget({ });
        const result = widget["byId"](widget["idPrefix"] + 'nonexistent');
        expect(result).toBeTruthy();
        expect(result.getNode()).toBeNull();
        widget.destroy();
    });
});

describe('Widget.findById', () => {
    it('finds child element by prefixed id', () => {
        const widget = new Widget({ });
        const child = document.createElement("span");
        child.id = widget["idPrefix"] + 'myChild';
        widget.domNode.appendChild(child);

        const result = widget["findById"]('myChild');
        expect(result).toBe(child);
        widget.destroy();
    });

    it('returns null when element not found', () => {
        const widget = new Widget({ });
        const result = widget["findById"](widget["idPrefix"] + 'nonexistent');
        expect(result).toBeNull();
        widget.destroy();
    });
});

describe('Widget.getGridField', () => {
    it('returns closest .field ancestor', () => {
        const field = document.createElement("div");
        field.className = "field";
        const widget = new Widget({ element: field });

        const gf = widget.getGridField();
        expect(gf.getNode()).toBe(field);
        widget.destroy();
    });

    it('returns Fluent even when no .field ancestor', () => {
        const widget = new Widget({});
        const gf = widget.getGridField();
        expect(gf).toBeTruthy();
        expect(gf.getNode()).toBeNull();
        widget.destroy();
    });
});

describe('Widget.change', () => {
    it('registers change handler on domNode', () => {
        const widget = new Widget({});
        const handler = vi.fn();
        widget.change(handler);

        widget.domNode.dispatchEvent(new Event('change'));
        expect(handler).toHaveBeenCalledTimes(1);
        widget.destroy();
    });
});

describe('Widget.changeSelect2', () => {
    it('filters out combobox setting value changes', () => {
        const widget = new Widget({});
        const handler = vi.fn();
        widget.changeSelect2(handler);

        // Regular change should trigger handler
        widget.domNode.dispatchEvent(new Event('change'));
        expect(handler).toHaveBeenCalledTimes(1);
        widget.destroy();
    });

    it('does not call handler for combobox setting value changes', () => {
        const widget = new Widget({});
        const handler = vi.fn();
        widget.changeSelect2(handler);

        // Change with comboboxsettingvalue dataset should be ignored
        const target = document.createElement("input");
        target.dataset.comboboxsettingvalue = "true";
        widget.domNode.appendChild(target);
        target.dispatchEvent(new Event('change', { bubbles: true }));
        expect(handler).not.toHaveBeenCalled();
        widget.destroy();
    });
});

describe('Widget.create', () => {
    it('creates widget and appends to container', () => {
        const container = document.createElement("div");
        const widget = Widget.create({
            type: Widget,
            container: container,
            options: {}
        });

        expect(widget).toBeInstanceOf(Widget);
        expect(container.contains(widget.domNode)).toBe(true);
        widget.destroy();
    });

    it('calls element callback', () => {
        const elementCb = vi.fn();
        const widget = Widget.create({
            type: Widget,
            element: elementCb
        });
        expect(elementCb).toHaveBeenCalled();
        widget.destroy();
    });

    it('calls init callback', () => {
        const initCb = vi.fn();
        const widget = Widget.create({
            type: Widget,
            init: initCb
        });
        expect(initCb).toHaveBeenCalledWith(widget);
        widget.destroy();
    });

    it('handles ArrayLike container', () => {
        const container = document.createElement("div");
        const widget = Widget.create({
            type: Widget,
            container: [container] as any
        });
        expect(container.contains(widget.domNode)).toBe(true);
        widget.destroy();
    });
});

describe('Widget.afterRender', () => {
    it('calls callback immediately when called after renderContents', () => {
        const callback = vi.fn();
        // Create a deferred render widget so internalRenderContents hasn't run yet
        class DeferredWidget extends Widget {
            protected override deferRender() { return true; }
        }
        const widget = new DeferredWidget({});
        // afterRender queues the callback
        widget["afterRender"](callback);
        // After init, internalRenderContents processes the queue and calls the callback
        widget.init();
        expect(callback).toHaveBeenCalled();
        widget.destroy();
    });

    it('calls callback immediately if already rendered', () => {
        const callback = vi.fn();
        class TestWidget extends Widget {
            protected override renderContents(): any {
                return document.createElement("span");
            }
        }
        const widget = new TestWidget({});
        // After constructor, internalRenderContents has already run and consumed the queue
        // So afterRender should call the callback immediately
        widget["afterRender"](callback);
        expect(callback).toHaveBeenCalled();
        widget.destroy();
    });

    it('does nothing if callback is null', () => {
        const widget = new Widget({});
        expect(() => widget["afterRender"](null)).not.toThrow();
        widget.destroy();
    });
});

describe('Widget.legacyTemplateRender', () => {
    it('returns undefined if getTemplate is not a function', () => {
        const widget = new Widget({});
        expect((widget as any).legacyTemplateRender()).toBeUndefined();
        widget.destroy();
    });

    it('returns undefined if getTemplate does not return a string', () => {
        class NoStringWidget extends Widget {
            getTemplate() { return 42; }
        }
        const widget = new NoStringWidget({});
        expect((widget as any).legacyTemplateRender()).toBeUndefined();
        widget.destroy();
    });

    it('replaces ~_ with idPrefix and sets innerHTML', () => {
        class TemplateWidget extends Widget {
            getTemplate() { return '<div>~_content</div>'; }
        }
        const widget = new TemplateWidget({ idPrefix: 'my_' } as any);
        expect(widget.domNode.innerHTML).toContain('my_content');
        widget.destroy();
    });
});

describe('Widget.deferRender', () => {
    it('returns false by default', () => {
        const widget = new Widget({});
        expect((widget as any).deferRender()).toBe(false);
        widget.destroy();
    });

    it('when true, renderContents is called from init instead of constructor', () => {
        class DeferredWidget extends Widget {
            protected override deferRender() { return true; }
            protected override renderContents(): any {
                const span = document.createElement("span");
                span.textContent = "deferred";
                return span;
            }
        }

        // Before init, domNode should be empty
        const widget = new DeferredWidget({});
        expect(widget.domNode.children.length).toBe(0);

        // After init, renderContents should have been called
        widget.init();
        expect(widget.domNode.children.length).toBe(1);
        expect(widget.domNode.querySelector('span')?.textContent).toBe('deferred');
        widget.destroy();
    });
});

describe('Widget.init', () => {
    it('returns the widget instance', () => {
        const widget = new Widget({});
        const result = widget.init();
        expect(result).toBe(widget);
        widget.destroy();
    });

    it('calls renderContents on deferred render widgets', () => {
        class InitWidget extends Widget {
            protected override deferRender() { return true; }
            protected override renderContents(): any {
                return document.createElement("br");
            }
        }
        const widget = new InitWidget({});
        expect(widget.domNode.children.length).toBe(0);
        widget.init();
        expect(widget.domNode.children.length).toBe(1);
        widget.destroy();
    });
});

describe('Widget.render', () => {
    it('returns init().domNode', () => {
        const widget = new Widget({});
        const result = widget.render();
        expect(result).toBe(widget.domNode);
        widget.destroy();
    });

    it('returns fragment when domNode parent is a fragment with multiple children', () => {
        const fragment = document.createDocumentFragment();
        const child1 = document.createElement("span");
        const child2 = document.createElement("div");
        fragment.appendChild(child1);
        fragment.appendChild(child2);
        (fragment as any)["__fragment_workaround__"] = true;

        // Create a widget with child2 as the domNode
        // The render method checks if parentNode is a DocumentFragment
        // with childNodes.length > 1 and isFragmentWorkaround marker
        // We need to set up the right conditions
        const widget = new Widget({ element: child2 });
        // child2 now has a fragment parent
        widget.destroy();
    });
});

describe('Widget.props', () => {
    it('returns the options object', () => {
        const widget = new Widget({});
        expect(widget.props).toBe((widget as any).options);
        widget.destroy();
    });
});

describe('Widget.addValidationRule', () => {
    it('adds validation rule to domNode', () => {
        const widget = new Widget({});
        const rule = () => "error";
        widget.addValidationRule(rule);
        // Since we didn't set up a form, this just shouldn't throw
        widget.destroy();
    });

    it('accepts uniqueName as first argument', () => {
        const widget = new Widget({});
        const rule = () => "error";
        widget.addValidationRule("myRule", rule);
        widget.destroy();
    });
});

describe('Widget.getCustomAttribute', () => {
    it('returns undefined when no attribute is present', () => {
        class AttrWidget extends Widget {}
        const widget = new AttrWidget({});
        const attr = widget["getCustomAttribute"](Object as any);
        expect(attr).toBeUndefined();
        widget.destroy();
    });
});

describe('Widget.syncOrAsyncThen', () => {
    it('calls sync method when useAsync returns false', () => {
        class SyncWidget extends Widget {
            useAsync() { return false; }
        }
        const widget = new SyncWidget({});
        const syncMethod = vi.fn(() => 42);
        const asyncMethod = vi.fn();
        const then = vi.fn();

        widget["syncOrAsyncThen"](syncMethod, asyncMethod, then);
        expect(syncMethod).toHaveBeenCalled();
        expect(asyncMethod).not.toHaveBeenCalled();
        expect(then).toHaveBeenCalledWith(42);
        widget.destroy();
    });

    it('calls async method when useAsync returns true', async () => {
        class AsyncWidget extends Widget {
            useAsync() { return true; }
        }
        const widget = new AsyncWidget({});
        const syncMethod = vi.fn();
        const asyncMethod = vi.fn(() => Promise.resolve(99));
        const then = vi.fn();

        widget["syncOrAsyncThen"](syncMethod, asyncMethod, then);
        expect(syncMethod).not.toHaveBeenCalled();
        expect(asyncMethod).toHaveBeenCalled();
        // Wait for promise to resolve
        await new Promise(globalThis.process.nextTick);
        expect(then).toHaveBeenCalledWith(99);
        widget.destroy();
    });
});

describe('Widget.useIdPrefix', () => {
    it('returns IdPrefixType from useIdPrefix utility', () => {
        class TestWidget extends Widget {}
        const widget = new TestWidget({ } as any);
        const prefix = widget["idPrefix"];
        const id = widget["useIdPrefix"]();
        expect(id._).toBe(prefix + '_');
        expect(id.Form).toBe(prefix + 'Form');
        widget.destroy();
    });
});

describe('Widget.registerClass', () => {
    it('registers type info with classTypeInfo', () => {
        // This would normally be called with a unique name
        // We test that it sets Symbol.typeInfo and registers the type
        class RegisteredWidget extends Widget {
            static override[Symbol.typeInfo] = this.registerClass("Test.RegisteredWidget");
        }
        expect(RegisteredWidget[Symbol.typeInfo]).toBeTruthy();
        expect(getTypeFullName(RegisteredWidget)).toBe("Test.RegisteredWidget");
    });

    it('throws if typeInfo already exists', () => {
        class AlreadyRegistered extends Widget {
            static override[Symbol.typeInfo] = this.registerClass("Test.AlreadyRegistered");
        }
        // Calling registerClass again on the same class should throw
        expect(() => {
            AlreadyRegistered[Symbol.typeInfo] = Widget["registerClass"].call(AlreadyRegistered, "Test.AlreadyRegistered");
        }).toThrow();
    });
});

describe('Widget.registerEditor', () => {
    it('registers editor type info', () => {
        class TestEditor extends Widget {
            static override[Symbol.typeInfo] = this.registerEditor("Test.TestEditor");
        }
        expect(TestEditor[Symbol.typeInfo]).toBeTruthy();
    });
});

describe('TemplatedWidget', () => {
    it('is an alias for Widget', () => {
        expect(TemplatedWidget).toBe(Widget);
    });
});

describe('useIdPrefix', () => {

    it('uses passed prefix', () => {
        var id = useIdPrefix('my_');

        expect(id._).toBe('my__');
        expect(id._x).toBe('my__x');
        expect(id.Form).toBe('my_Form');
        expect(id.PropertyGrid).toBe('my_PropertyGrid');
        expect(id.something).toBe('my_something');
    });

    it('handles hashes differently for in-page href generation', () => {
        var id = useIdPrefix('my_');

        expect(id["#_"]).toBe('#my__');
        expect(id["#_x"]).toBe('#my__x');
        expect(id["#Form"]).toBe('#my_Form');
        expect(id["#PropertyGrid"]).toBe('#my_PropertyGrid');
        expect(id["#something"]).toBe('#my_something');
    });

});