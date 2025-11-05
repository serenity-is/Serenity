import { Component, initComponentClass } from "../src/component"

describe("Component", () => {
    it("can be instantiated with props", () => {
        const comp = new Component({ test: 'value' });
        expect(comp.props.test).toBe('value');
    });

    it("has children in props", () => {
        const comp = new Component({ children: 'child' });
        expect(comp.props.children).toBe('child');
    });

    it("has ref in props", () => {
        const ref = { current: null };
        const comp = new Component({ ref });
        expect(comp.props.ref).toBe(ref);
    });

    it("render returns null by default", () => {
        const comp = new Component({});
        expect(comp.render()).toBe(null);
    });

    it("has isComponent static property", () => {
        expect(Component.isComponent).toBe(true);
    });

    it("can be used as JSX element", () => {
        class MyComponent extends Component<{ text: string }> {
            render() {
                return <div>{this.props.text}</div>;
            }
        }

        const element = <MyComponent text="hello" />;
        expect(element.tagName).toBe('DIV');
        expect(element.textContent).toBe('hello');
    });
});

describe("initComponentClass", () => {
    class TestComponent extends Component {
        render() {
            return document.createElement('div');
        }
    }

    it("creates instance with props and children", () => {
        const node = initComponentClass(TestComponent as any, { prop: 'value' }, 'child');
        expect(node.tagName).toBe('DIV');
    });

    it("sets ref to the component instance", () => {
        const ref = { current: null };
        initComponentClass(TestComponent as any, { ref }, []);
        expect(ref.current).toBeInstanceOf(TestComponent);
    });

    it("passes children to props", () => {
        class PropsComponent extends Component {
            storedProps: any;
            constructor(props: any) {
                super(props);
                this.storedProps = props;
            }
            render() {
                return document.createElement('p');
            }
        }

        const ref = { current: null };
        initComponentClass(PropsComponent as any, { ref }, 'test child');
        expect(ref.current.storedProps.children).toBe('test child');
    });
});

