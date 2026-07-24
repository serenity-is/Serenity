# DomWise (@serenity-is/domwise)

[![npm version](https://img.shields.io/npm/v/@serenity-is/domwise.svg?style=flat-square)](https://www.npmjs.com/package/@serenity-is/domwise)
[![npm downloads](https://img.shields.io/npm/dm/@serenity-is/domwise.svg?style=flat-square)](https://www.npmjs.com/package/@serenity-is/domwise)
![gzip size](https://img.badgesize.io/https:/cdn.jsdelivr.net/npm/@serenity-is/domwise/dist/index.js?compression=gzip)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](./LICENSE.md)

> **Use JSX syntax to create and manage DOM elements** — with full TypeScript support, reactive signal bindings, and seamless integration with the Serenity widget system.

DomWise is a lightweight, high-performance JSX library that compiles your JSX templates directly into real DOM nodes — no virtual DOM, no diffing, no reconciliation overhead. Designed from the ground up for the [Serenity](https://serenity.is) application framework, it embraces direct DOM manipulation and works harmoniously with widget lifecycles, avoiding the conflicts that plague VDOM libraries when mixed with imperative DOM updates.

Serenity's widget model was originally built around jQuery UI widgets that attach to existing elements, manipulate the DOM directly, and manage their own lifecycle through `init()` / `destroy()` methods. As the framework transitions away from jQuery toward a modern JSX-based programming model, DomWise provides the ideal foundation — it creates real DOM nodes that widgets can interact with natively, without the abstraction layer of a virtual DOM that would fight against imperative DOM operations.

Built on the foundations of [jsx-dom](https://github.com/alex-kinokon/jsx-dom), [dom-expressions](https://github.com/ryansolid/dom-expressions), and [tsx-dom](https://github.com/Lusito/tsx-dom), DomWise adds first-class reactive programming via [@preact/signals-core](https://github.com/preactjs/signals), a powerful conditional `Show` component, and a comprehensive lifecycle management system that integrates with Serenity's widget dispose patterns.

See [NOTICE.md](./NOTICE.md) for licensing information about used libraries.

---

## Table of Contents

- [Installation](#installation)
- [Usage in Serene/StartSharp Applications](#usage-in-serene-startsharp-applications)
- [Quick Start](#quick-start)
- [Why Not Virtual DOM?](#why-not-virtual-dom)
- [JSX Syntax](#jsx-syntax)
  - [Creating Elements](#creating-elements)
  - [Class](#class)
  - [Style](#style)
  - [Events](#events)
  - [Children](#children)
  - [Refs](#refs)
  - [Dataset](#dataset)
  - [Special Attributes](#special-attributes)
- [Reactive Signals](#reactive-signals)
  - [Attribute Bindings](#attribute-bindings)
  - [Class Bindings](#class-bindings)
  - [Style Bindings](#style-bindings)
  - [Content Bindings](#content-bindings)
  - [Signal API](#signal-api)
  - [Signal Utilities](#signal-utilities)
- [Components](#components)
  - [Function Components](#function-components)
  - [Class Components](#class-components)
- [Flow Control](#flow-control)
  - [Show Component](#show-component)
- [Fragments](#fragments)
- [Shadow DOM](#shadow-dom)
- [SVG & Namespaces](#svg--namespaces)
- [Hooks](#hooks)
  - [useClassList](#useclasslist)
  - [usePropBinding](#usepropbinding)
  - [useText](#usetext)
  - [useRef](#useref)
  - [useSignal](#usesignal)
  - [useUpdatableComputed](#useupdatablecomputed)
- [Lifecycle & Disposal](#lifecycle--disposal)
  - [Disposing Listeners](#disposing-listeners)
  - [Lifecycle Root](#lifecycle-root)
- [Serenity Widget Integration](#serenity-widget-integration)
  - [Widget Rendering Model: renderContents vs render](#widget-rendering-model-rendercontents-vs-render)
  - [Once-Only Rendering](#once-only-rendering)
  - [Direct DOM Manipulation](#direct-dom-manipulation)
  - [Widget Lifecycle Integration](#widget-lifecycle-integration)
- [Utilities](#utilities)
  - [className](#classname)
  - [bindThis](#bindthis)
  - [inSVGNamespace / inMathMLNamespace](#insvgnamespace--inmathmlnamespace)
- [React Compatibility API](#react-compatibility-api)
- [TypeScript Configuration](#typescript-configuration)
- [Browser Support](#browser-support)
- [License](#license)

---

## Installation

```bash
npm install @serenity-is/domwise
```

```bash
yarn add @serenity-is/domwise
```

```bash
pnpm add @serenity-is/domwise
```

---

## Usage in Serene/StartSharp Applications

In Serene and StartSharp template applications, you typically do not need to install `@serenity-is/domwise` via npm manually. The package is brought in automatically through the NuGet package dependency chain:

1. Your `.csproj` file references `Serenity.Corelib` (and optionally other Serenity packages).
2. `Serenity.Corelib` has a NuGet dependency on `Serenity.DomWise`, which ships the DomWise JavaScript and TypeScript files as embedded static web assets under its `dist/` directory.
3. When you run `npm install` or `pnpm install`, the `preinstall` / `pnpm:devPreinstall` script defined in your `package.json` executes `dotnet build -target:RestoreNodeTypes`.
4. The `RestoreNodeTypes` MSBuild target, defined in `Serenity.Net.Web.targets` (shipped via the `Serenity.Net.Web` NuGet package), scans all referenced NuGet and project packages for their `dist/` directories and copies the files to your project's `node_modules/.dotnet/` folder.
5. The same target also automatically inserts or updates the corresponding entries in your `package.json` dependencies section, pointing them to the local `node_modules/.dotnet/` paths.

After running `npm install`, your `package.json` will contain entries like:

```json
"@serenity-is/domwise": "./node_modules/.dotnet/serenity.domwise"
```

This system ensures that your npm dependencies are always kept in sync with the NuGet package versions you have referenced — without any manual version management. It also accommodates Serenity packages (such as `Serenity.Extensions`, `Serenity.Pro.Extensions`) that do not have published npm registry counterparts.

> **Note:** The direct `npm install @serenity-is/domwise` shown in the previous section is only needed when using DomWise outside of a Serene/StartSharp project, or if you require a standalone installation.

---

## Quick Start

```tsx
import { jsx } from "@serenity-is/domwise";

document.body.appendChild(
  <div id="greeting" class="alert">
    Hello World
  </div>
);
```

With reactive signals:

```tsx
import { signal } from "@serenity-is/domwise";

const count = signal(0);

document.body.appendChild(
  <div>
    <p>Count: {count}</p>
    <button onClick={() => count.value++}>Increment</button>
  </div>
);
```

Conditional rendering:

```tsx
import { signal, Show } from "@serenity-is/domwise";

const isLoggedIn = signal(false);

document.body.appendChild(
  <div>
    <Show when={isLoggedIn} fallback={<button onClick={() => isLoggedIn.value = true}>Log in</button>}>
      <button onClick={() => isLoggedIn.value = false}>Log out</button>
    </Show>
  </div>
);
```

---

## Why Not Virtual DOM?

DomWise compiles JSX directly into real DOM nodes — no virtual DOM, no diffing, no reconciliation. This design is a deliberate choice driven by Serenity's widget architecture.

Serenity's widget model was originally designed over 10 years ago around jQuery UI widgets. Widgets attach themselves to existing DOM elements, manipulate the DOM directly, and manage their own lifecycle through `init()` / `destroy()` methods. When the framework began transitioning away from jQuery toward a JSX-based programming model, VDOM libraries like React were evaluated but their virtual DOM reconciliation conflicted with widgets that modify the DOM imperatively.


While VDOM has its own set of advantages, they do not outweigh the issues that may arise when dealing with code or external components that manipulate the DOM directly.

A JSX library that creates real DOM elements like DomWise — avoids these issues entirely:

- **No reconciliation conflicts** — Widgets can freely append, remove, or modify DOM nodes without worrying about a VDOM diffing algorithm reverting their changes.
- **Seamless migration** — Existing widgets that call `appendChild`, modify `innerHTML`, attach event listeners, or integrate with third-party libraries (Select2, Flatpickr, SortableJS, etc.) continue to work without wrappers or workarounds.
- **Predictable lifecycle** — Elements are created once and live until explicitly removed. There is no re-render cycle that might reconstruct or detach widget-bound elements.
- **Framework flexibility** — While DomWise is the primary programming model, you can freely use React, Preact, Vue, or any other framework in parts of your application without cross-framework reconciliation issues. For example, the StartSharp dashboard page uses Preact for its Chat widget while other widgets on the same page use DomWise.

### From jsx-dom to DomWise

The Serenity ecosystem initially adopted [jsx-dom](https://github.com/alex-kinokon/jsx-dom) as its JSX runtime. While jsx-dom served well, it lacked features needed for deeper framework integration:

- **Reactive signals** — jsx-dom had no built-in support for reactive programming patterns.
- **Control over development** — We needed the ability to quickly add features and fix discovered issues without waiting for upstream releases.

DomWise builds on the foundations of jsx-dom, [dom-expressions](https://github.com/ryansolid/dom-expressions), and [tsx-dom](https://github.com/Lusito/tsx-dom), adding:

- First-class [@preact/signals-core](https://github.com/preactjs/signals) integration — signals work natively as attribute values, class/style bindings, and children, with automatic DOM updates.
- A comprehensive lifecycle management system that integrates with Serenity's widget disposal patterns.
- Lowercased HTML attributes (e.g., `tabindex`, `readonly`, `for`) to match actual HTML, making it easier to copy-paste Bootstrap and other HTML snippets.
- A `Show` component for declarative conditional rendering.
- Hooks like `useClassList`, `usePropBinding`, `useText`, and `useUpdatableComputed`.

---

## JSX Syntax

### Creating Elements

DomWise compiles JSX directly into real DOM nodes using the `jsx` factory function. Set your `jsxImportSource` to `@serenity-is/domwise` in your `tsconfig.json` (see [TypeScript Configuration](#typescript-configuration)).

```tsx
// Basic elements
<div>Hello</div>
<span class="label">Name:</span>
<input type="text" value="default" />

// Self-closing
<br />
<hr />
<img src="photo.jpg" />

// Nested elements
<div class="container">
  <header>
    <h1>Title</h1>
  </header>
  <main>
    <p>Content</p>
  </main>
</div>
```

**Factory functions:**

DomWise provides two JSX factory functions with different signatures:

| Function | Children parameter |
|----------|--------------------|
| `jsx` / `jsxs` | Children are passed as `props.children` (a property of the second argument) |
| `createElement` / `h` | Children are passed as additional arguments after the props (rest params) |

- **`jsx`** — The primary factory used by the automatic JSX runtime (`jsxImportSource`). Children go inside the `props` object: `jsx("div", { class: "foo", children: "Hello" })`.
- **`createElement`** (aliased as `h`) — A classic/familiar API similar to `React.createElement`. Children are extra arguments: `createElement("div", { class: "foo" }, "Hello")`. If the `attr` argument is a string or array, it is treated as the first child and `attr` defaults to `{}`. If `attr.children` is present and no additional children were given, `attr.children` is used.

### Class

The `class` attribute (also `className`) accepts:

- **A string**: simple class name(s)
- **An object**: `{ [key: string]: boolean }` — keys with truthy values are included
- **An array**: any combination of the above, deeply nested
- **A signal**: reactive class bindings (see [Class Bindings](#class-bindings))

Falsy values (`false`, `null`, `undefined`, `true`) are ignored per React semantics.

```tsx
// String
<div class="greeting" />
<div className="greeting" />

// Object — keys with truthy values are added
<div class={{ hidden: isHidden, active: true, disabled: false }} />

// Array — falsy values filtered out
<div class={[condition && "active", "base", ["nested", ["deep"]]]} />

// Mix
<div class={["card", { selected: isSelected, "has-focus": focused }]} />
```

### Style

The `style` attribute accepts:

- **A string**: inline CSS
- **An object**: CSS property names in camelCase
- **An array**: any combination of the above, deeply nested
- **A signal**: reactive style bindings (see [Style Bindings](#style-bindings))

Unitless properties (e.g. `opacity`, `zIndex`, `flex`, `lineHeight`) follow React conventions — numeric values are not auto-suffixed with `px`. CSS custom properties (`--*`) are set via `style.setProperty()`.

```tsx
// String
<div style="color: red; font-size: 14px;" />

// Object
<div style={{ color: "red", fontSize: "14px", opacity: 0.5 }} />

// Array (merged left-to-right)
<div style={[{ color: "red" }, { fontSize: "14px" }]} />

// CSS custom properties
<div style={{ "--my-color": "blue" }} />
```

### Events

Event listeners are attached directly as DOM properties (e.g. `node.onclick = handler`). Standard events use lowercase names, custom events preserve their original casing.

```tsx
// Standard events
<button onClick={e => handleClick(e)}>Click me</button>
<input onChange={e => validate(e.target.value)} />
<div onMouseOver={e => highlight(e)} />

// Custom events
<my-element onMyCustomEvent={e => console.log(e.detail)} />

// Capture phase events
<button onClickCapture={e => handleCapture(e)} />

// Bulk event registration via `on` and `onCapture` attributes
<div on={{ click: handleClick, contextmenu: handleContextMenu }} />
<div onCapture={{ keydown: handleKeyDown }} />
```

### Children

Children can be strings, numbers, DOM nodes, arrays, or signals. `false`, `null`, `undefined`, and `true` are ignored.

```tsx
// Text children
<div>Hello World</div>

// Number children
<div>Score: {42}</div>

// Mixed
<div>{["Total: ", 20]}</div>

// Nested arrays are flattened
<div>{[["a", "b"], ["c", "d"]]}</div>

// NodeList, HTMLCollection, Iterables
<div>{document.querySelectorAll("span")}</div>

// Explicit children attribute (when no JSX children)
<div children={["Total: ", 20]} />
```

### Refs

Refs provide access to the underlying DOM node after creation. Use `createRef` to create a ref object, or pass a callback.

```tsx
import { createRef, useRef } from "@serenity-is/domwise";

// Ref object (like React.createRef)
const inputRef = createRef<HTMLInputElement>();
render(<input ref={inputRef} />);
inputRef.current.focus();

// Callback ref
<input ref={node => node?.focus()} />

// useRef hook (alias for createRef)
const divRef = useRef<HTMLDivElement>();
```

Refs also work with class components — they receive the component instance rather than a DOM node.

### Dataset

The `dataset` attribute accepts an object of key-value pairs that are set on `element.dataset`.

```tsx
<div dataset={{ user: "guest", theme: "dark", isLoggedIn: false }} />
// Result: <div data-user="guest" data-theme="dark" data-is-logged-in="false">
```

When used with signals or prop bindings, previous dataset keys are cleaned up on change:

```tsx
const data = signal({ type: "alert", level: "warn" });
<div dataset={data} />
// Changing to { type: "info" } removes "level", adds "type: info"
```

### Special Attributes

| Attribute | Description |
|-----------|-------------|
| `ref` | Captures the DOM node or component instance |
| `namespaceURI` | Specifies the XML namespace for the element |
| `dangerouslySetInnerHTML` | Sets `innerHTML` from `{ __html: string }` |
| `textContent` | Sets `textContent` directly |
| `innerText` | Alias for `textContent` |
| `value` | Sets `value` property on `<input>`, `<textarea>`, `<select>` (with multi-select array support) |
| `dataset` | Sets `dataset` properties in bulk |
| `on` / `onCapture` | Bulk event registration via object maps |

---

## Reactive Signals

DomWise integrates [@preact/signals-core](https://github.com/preactjs/signals) for fine-grained reactivity. Any signal can be used directly in JSX attributes, class bindings, style bindings, and as children — and the DOM updates automatically when the signal changes.

### Attribute Bindings

Pass a signal as an attribute value and the DOM property updates reactively:

```tsx
const title = signal("Hello");

<div title={title} />
// <div title="Hello">

title.value = "Updated";
// <div title="Updated">
```

### Class Bindings

Signals work seamlessly with all class binding forms:

```tsx
const active = signal(true);
const className = signal(["item", "highlighted"]);

// Signal as the entire class value
<div class={className} />

// Signal as individual object entries
<div class={{ active, disabled: false }} />

// Signal in arrays
<div class={[active, "base", className]} />

// Mix of signal and non-signal
<div class={{ active: active, disabled: false, highlighted: true }} />
```

When signal values change, only the affected classes are toggled — manually added classes via `classList.add()` are preserved.

### Style Bindings

Signals can drive individual style properties or the entire style object:

```tsx
const color = signal("red");
const size = signal(16);
const styleObj = signal({ color: "red", backgroundColor: "blue" });

// Individual property
<div style={{ color }} />
<div style={{ color, fontSize: size }} />

// Full style object
<div style={styleObj} />

// Clearing with `false`
styleObj.value = false; // removes all previously set properties
```

When a signal style value changes, only the affected properties are updated — unrelated properties set manually or by other signals are untouched.

### Content Bindings

Signals as children create reactive text nodes that update in place:

```tsx
const name = signal("World");

<div>Hello {name}!</div>
// <div>Hello World!</div>

name.value = "DomWise";
// <div>Hello DomWise!</div>
```

Signal children inside fragments are also supported:

```tsx
const value = signal("mid");

<div>
  <><span>before</span>{value}<span>after</span></>
</div>
```

### Signal API

DomWise re-exports the full `@preact/signals-core` API:

```tsx
import { signal, computed, effect, batch, untracked } from "@serenity-is/domwise";

const count = signal(0);
const doubled = computed(() => count.value * 2);

effect(() => {
  console.log("Count:", count.value);
});

batch(() => {
  count.value = 1;
  count.value = 2; // Only one notification
});
```

Additional signal helpers:

```tsx
import { useSignal, useUpdatableComputed } from "@serenity-is/domwise";

// Create a writable signal
const sig = useSignal(42);

// Manually updatable computed — only recalculates when update() is called
const { computed: cmp, update } = useUpdatableComputed();
const derived = cmp(() => expensiveCalculation(data));
update(); // Forces recalculation
```

### Signal Utilities

```tsx
import { isSignalLike, isWritableSignal, isReadonlySignal, observeSignal, derivedSignal } from "@serenity-is/domwise";

isSignalLike(obj);       // true if obj has subscribe(), peek(), and value
isWritableSignal(obj);   // true if signal has a writable value
isReadonlySignal(obj);   // true if signal is read-only (computed)

// Observe signal changes with lifecycle management
const dispose = observeSignal(mySignal, (args) => {
  console.log(args.newValue, args.prevValue, args.isInitial);
  args.lifecycleNode = someElement; // ties subscription to element's disposal
});

// Derive a signal that transforms another signal
const derived = derivedSignal(source, value => transform(value));
```

---

## Components

### Function Components

Write components as plain functions that return JSX. Props are passed as an object, with `children` in `props.children`. `defaultProps` are supported.

```tsx
function Greeting(props: { firstName?: string; lastName: string }) {
  return (
    <div>
      Hello, {props.firstName} {props.lastName}!
    </div>
  );
}
Greeting.defaultProps = { firstName: "John" };

document.body.appendChild(<Greeting lastName="Appleseed" />);
// <div>Hello, John Appleseed!</div>
```

### Class Components

Class components extend `Component` and implement a `render()` method. They receive props (including `children` and `ref`) in the constructor.

```tsx
import { Component } from "@serenity-is/domwise";

interface MyButtonProps {
  label: string;
  variant?: "primary" | "secondary";
}

class MyButton extends Component<MyButtonProps> {
  render() {
    return (
      <button class={`btn btn-${this.props.variant ?? "primary"}`}>
        {this.props.label}
      </button>
    );
  }
}

// Usage
<MyButton label="Submit" variant="primary" />

// With ref (receives the component instance, not a DOM node)
const btnRef = createRef<MyButton>();
<MyButton ref={btnRef} label="Click" />
```

---

## Flow Control

### Show Component

The `Show` component provides declarative conditional rendering, similar to SolidJS's `<Show>`. When the `when` prop is a signal, it reactively swaps between the main content and the fallback.

```tsx
import { Show } from "@serenity-is/domwise";

// Static condition
<Show when={isAuthenticated} fallback={<LoginForm />}>
  <Dashboard />
</Show>

// Reactive with a signal
const loading = signal(true);
<Show when={loading} fallback={<div>Content loaded</div>}>
  <div class="spinner">Loading...</div>
</Show>

loading.value = false;
// Automatically switches to "Content loaded"

// Function children/footer for lazy evaluation
<Show when={user} fallback={() => <AnonymousView />}>
  {(user) => <UserProfile user={user} />}
</Show>

// Cleanup: subscriptions are auto-disposed when the shown element is disposed
```

---

## Fragments

Use `<></>` or `Fragment` to group multiple children without a wrapper element:

```tsx
import { Fragment } from "@serenity-is/domwise";

// Short syntax
<>
  <li>Item 1</li>
  <li>Item 2</li>
</>

// Explicit Fragment
<Fragment>
  <div>First</div>
  <div>Second</div>
</Fragment>
```

Fragments compile to `DocumentFragment` nodes. Signal children inside fragments are tracked independently.

---

## Shadow DOM

Attach a shadow root to any element with the `ShadowRootNode` component:

```tsx
import { ShadowRootNode } from "@serenity-is/domwise";

<div>
  <ShadowRootNode mode="open">
    <style>{`:host { font-family: sans-serif; }`}</style>
    <button>Shadow button</button>
  </ShadowRootNode>
</div>

// With ref to access the shadow root
const shadowRef = createRef<ShadowRoot>();
<ShadowRootNode mode="closed" ref={shadowRef}>
  <p>Encapsulated content</p>
</ShadowRootNode>
```

Supports all `ShadowRootInit` options: `mode`, `delegatesFocus`, `clonable`, `serializable`, `slotAssignment`.

---

## SVG & Namespaces

SVG and MathML tags are automatically created in the correct namespace. For custom namespaces, use the `namespaceURI` attribute.

```tsx
<svg width="150" height="100" viewBox="0 0 3 2">
  <rect width="1" height="2" x="0" fill="#008d46" />
  <rect width="1" height="2" x="1" fill="#ffffff" />
  <rect width="1" height="2" x="2" fill="#d2232c" />
</svg>
```

For dynamic namespace scoping:

```tsx
import { inSVGNamespace, inMathMLNamespace, inHTMLNamespace, SVGNamespace, MathMLNamespace } from "@serenity-is/domwise";

// Create SVG elements outside of an <svg> context
const result = inSVGNamespace(() => (
  <a namespaceURI={SVGNamespace}>SVG anchor</a>
));

// Temporarily switch to HTML namespace
inHTMLNamespace(() => {
  // Elements here are created in the HTML namespace
});
```

Supported SVG tags: `svg`, `animate`, `circle`, `clipPath`, `defs`, `desc`, `ellipse`, `feBlend`, `feColorMatrix`, `feComponentTransfer`, `feComposite`, `feConvolveMatrix`, `feDiffuseLighting`, `feDisplacementMap`, `feDistantLight`, `feDropShadow`, `feFlood`, `feFuncA`, `feFuncB`, `feFuncG`, `feFuncR`, `feGaussianBlur`, `feImage`, `feMerge`, `feMergeNode`, `feMorphology`, `feOffset`, `fePointLight`, `feSpecularLighting`, `feSpotLight`, `feTile`, `feTurbulence`, `filter`, `foreignObject`, `g`, `image`, `line`, `linearGradient`, `marker`, `mask`, `metadata`, `mpath`, `path`, `pattern`, `polygon`, `polyline`, `radialGradient`, `rect`, `stop`, `switch`, `symbol`, `text`, `textPath`, `tspan`, `use`, `view`.

---

## Hooks

### useClassList

A reactive class list that automatically syncs with the DOM when used as the `class` attribute.

```tsx
import { useClassList } from "@serenity-is/domwise";

function Component() {
  const cls = useClassList(["base", { ready: false }]);

  setTimeout(() => {
    cls.add("loaded");
    cls.toggle("ready");
  }, 2000);

  return <div class={cls}>Status</div>;
}

// API
cls.add("foo", "bar");       // Add classes
cls.remove("foo");            // Remove classes
cls.toggle("active", true);   // Toggle with optional force
cls.contains("active");       // Check if class exists
cls.value;                     // Get class string
cls.size;                      // Get number of classes
cls();                         // Get the underlying DOMTokenList
```

When the element is disposed, the class list is automatically cleaned up.

### usePropBinding

A two-way prop binding that lets you read and write an attribute value while keeping the DOM in sync.

```tsx
import { usePropBinding } from "@serenity-is/domwise";

function InputField() {
  const value = usePropBinding("initial");

  return <input value={value} />;
}

// Reading
console.log(value()); // "initial"

// Writing — updates both the internal value and the DOM
value("new value");
console.log(value()); // "new value"
```

The binding is tied to the element's lifecycle — when the element is disposed, the binding stops updating the DOM.

### useText

Creates a `Text` node and a setter function for efficient text updates without touching the parent element.

```tsx
import { useText } from "@serenity-is/domwise";

function StatusIndicator() {
  const [text, setText] = useText("Loading...");

  fetch("/api/data").then(() => {
    setText("Done!");
  });

  return <div>Status: {text}</div>;
}
```

### useRef

Alias for `createRef`. Returns a `{ current: null }` ref object.

```tsx
import { useRef } from "@serenity-is/domwise";

const myRef = useRef<HTMLDivElement>();
<div ref={myRef} />;
```

### useSignal

Creates a writable signal with the given initial value. Convenience wrapper around `signal()`.

```tsx
import { useSignal } from "@serenity-is/domwise";

const count = useSignal(0);
count.value++; // 1
```

### useUpdatableComputed

Creates computed signals that only recalculate when `update()` is explicitly called — useful for batching dependent recalculations.

```tsx
import { useUpdatableComputed } from "@serenity-is/domwise";

const { computed, update } = useUpdatableComputed();

// Create a computed that depends on external state
let externalData = { x: 1, y: 2 };
const sum = computed(() => externalData.x + externalData.y);

console.log(sum.value); // 3

externalData = { x: 5, y: 10 };
console.log(sum.value); // 3 (still cached)

update();
console.log(sum.value); // 15 (recalculated)
```

---

## Lifecycle & Disposal

DomWise provides a comprehensive lifecycle management system that integrates with Serenity's widget disposal patterns. When a DOM element is removed or a widget is destroyed, all associated signal subscriptions, event listeners, and hooks are automatically cleaned up.

### Disposing Listeners

Register cleanup callbacks that fire when an element receives a `"disposing"` custom event:

```tsx
import { addDisposingListener, removeDisposingListener, dispatchDisposingEvent, invokeDisposingListeners } from "@serenity-is/domwise";

// Register a cleanup handler
const el = document.createElement("div");
addDisposingListener(el, () => {
  console.log("Element disposed — clean up resources");
});

// Dispatch the disposing event manually
dispatchDisposingEvent(el);

// Or invoke listeners directly (without dispatching an event)
invokeDisposingListeners(el);

// With options: clean up descendants and/or exclude self
invokeDisposingListeners(el, { descendants: true });
invokeDisposingListeners(el, { excludeSelf: true });

// Remove a specific listener
removeDisposingListener(el, handler);

// Remove by registration key
addDisposingListener(el, handler, "myKey");
removeDisposingListener(el, null, "myKey");
```

### Lifecycle Root

The lifecycle root tracks the current disposal context, enabling automatic cleanup of signal subscriptions when the parent element is disposed.

```tsx
import { currentLifecycleRoot } from "@serenity-is/domwise";

// Get the current lifecycle root (set automatically during JSX creation)
const root = currentLifecycleRoot();
```

---

## Serenity Widget Integration

DomWise is the foundation of Serenity's JSX rendering pipeline. It works hand-in-hand with the `Widget` base class from `@serenity-is/corelib` to provide a predictable, imperative-friendly component model.

### Widget Rendering Model: `renderContents` vs `render`

Unlike React class components where `render()` is called on every state/prop change, Serenity widgets follow a simpler, once-only rendering model:

- **`render()`** — Returns the widget's root DOM node. This method is called once by the JSX runtime when the widget is instantiated in JSX (e.g., `<MyWidget />`). It calls `init()`, which triggers `internalRenderContents()`.

- **`renderContents()`** — Called exactly once during widget initialization to populate the widget's DOM node with its initial content. Override this method to provide the JSX content for your widget:

```tsx
class MyWidget extends Widget<any> {
  protected override renderContents() {
    return (
      <div class="my-widget">
        <button onClick={e => this.handleClick(e)}>Click me</button>
      </div>
    );
  }
}
```

The `afterRender` internal queue ensures `renderContents` is only invoked once — the queue is deleted after the first call, preventing subsequent executions. This is fundamentally different from React's `render()`, which is called on every state or prop change.

### Once-Only Rendering

Serenity widgets do not re-render automatically. `renderContents()` fires once during initialization, and that's it. This design choice aligns with Serenity's imperative heritage:

- **Direct DOM updates** — After `renderContents`, widgets update their DOM by directly manipulating elements (changing text content, toggling classes, adding/removing children). No VDOM diffing is needed.
- **Reactive signals** — For scenarios where automatic DOM updates are desired, use signals. DomWise's signal bindings update the DOM in-place without triggering a full re-render:

```tsx
import { signal, Widget } from "@serenity-is/corelib";

class CounterWidget extends Widget<any> {
  private count = signal(0);

  protected override renderContents() {
    return (
      <div>
        <p>Count: {this.count}</p>
        <button onClick={() => this.count.value++}>Increment</button>
      </div>
    );
  }
}
```

- **Imperative refreshes** — Methods like `grid.refresh()` or manual property updates followed by direct DOM changes are the norm. Widgets that need to rebuild their content can clear `domNode.innerHTML` and re-append elements.

### Direct DOM Manipulation

Because there is no VDOM reconciliation, you can freely mix JSX-rendered content with imperative DOM operations:

```tsx
class HybridWidget extends Widget<any> {
  private imperativePart: HTMLDivElement;

  protected override renderContents() {
    // JSX for the initial structure
    return (
      <div class="hybrid">
        <div class="jsx-part">Created via JSX</div>
        <div ref={el => this.imperativePart = el} class="imperative-part"></div>
      </div>
    );
  }

  someMethod() {
    // Direct DOM manipulation — perfectly safe, no VDOM conflicts
    const child = document.createElement('span');
    child.textContent = 'Added imperatively';
    this.imperativePart.appendChild(child);
  }
}
```

This pattern is essential when working with third-party libraries (Select2, Flatpickr, SortableJS, etc.) that directly manipulate the DOM. In a VDOM environment, these libraries would fight against the framework's reconciliation engine; with DomWise, they coexist naturally.

### Widget Lifecycle Integration

DomWise's lifecycle system automatically hooks into Serenity's widget disposal mechanism. When a widget's `destroy()` method is called, DomWise:

1. Fires the `"disposing"` custom event on the widget's `domNode`.
2. Cleans up all signal subscriptions associated with the element.
3. Removes event listeners registered via Fluent or DomWise's event system.
4. Recursively disposes child widgets through the widget association system.

This ensures no memory leaks when widgets are dynamically created and destroyed, even when signals and event handlers are involved. The `addDisposingListener` and `removeDisposingListener` functions from DomWise are used internally by the `Widget` base class to register its `destroy()` method.

---

## Utilities

### className

Converts various value types to a class name string. Supports strings, objects, arrays (nested), and iterables.

```tsx
import { className } from "@serenity-is/domwise";

className("foo");                       // "foo"
className({ active: true, hidden: false }); // "active"
className(["a", "b", null, false]);     // "a b"
className([["a", "b"], ["c"]]);         // "a b c"
className(new Set(["x", "y"]));         // "x y"
```

### bindThis

Creates a proxy that automatically binds method calls to the given object — lazily, with caching.
Primarily intended for use in classes when attaching event handlers.

Instead of `someElement.addEventListener("click", this.onClick.bind(this))` or an arrow function
`(e) => this.onClick(e)` — both of which hurt performance and complicate `removeEventListener`
because the bound function must be stored — you can write:

```tsx
import { bindThis } from "@serenity-is/domwise";

class MyWidget {
  boundThis = bindThis(this);

  constructor() {
    // subscribe with boundThis — no need to store the bound function
    someElement.addEventListener("click", this.boundThis.onClick);
  }

  onClick(e: MouseEvent) {
    // `this` is always the widget instance
    console.log("clicked", this);
  }

  dispose() {
    // unsubscribe with the original method — no need to call bindThis again
    someElement.removeEventListener("click", this.onClick);
  }
}
```

The returned proxy lazily binds methods on first access and caches the bound function in the
target object. Subsequent accesses return the same cached function, making it safe to use with
`removeEventListener` by passing the **original** method (e.g. `this.onClick`). Calling `bindThis`
a second time on the same object returns the same proxy.

```tsx
const bound = bindThis(widget);
bound === bindThis(widget); // true
```

Arrow functions and own properties are returned directly without binding.

### inSVGNamespace / inMathMLNamespace

Temporarily set the namespace context for creating elements outside their natural parent:

```tsx
import { inSVGNamespace, inMathMLNamespace, inHTMLNamespace } from "@serenity-is/domwise";

inSVGNamespace(() => {
  // All JSX elements here are created in the SVG namespace
});

inMathMLNamespace(() => {
  // All JSX elements here are created in the MathML namespace
});

inHTMLNamespace(() => {h, useImperativeHandle } from "@serenity-is/domwise";

// Classic createElement API (like React.createElement)
// Children are passed as additional arguments (rest params).
const el = createElement("div", { class: "foo" }, "child1", "child2");

// h is an alias for createElement
const el2 = h("span", { class: "bar" }, "text");

// If attr is a string or array, it is treated as the first child:
const el3 = createElement("div", "only child

---

## React Compatibility API

For projects migrating from or integrating with React codebases:

```tsx
import { createElement, useImperativeHandle } from "@serenity-is/domwise";

// Classic createElement API (like React.createElement)
const el = createElement("div", { class: "foo" }, "child1", "child2");

// useImperativeHandle (simplified — sets ref.current immediately)
useImperativeHandle(myRef, () => ({ focus: () => console.log("focused") }));
```

---

## TypeScript Configuration

For automatic JSX transform (recommended), configure your `tsconfig.json`:

```json
{
  "compilerOptions": {
    "jsx": "react-jsx",
    "jsxImportSource": "@serenity-is/domwise"
  }
}
```

This allows you to use JSX without importing anything. If you prefer the classic transform:

```json
{
  "compilerOptions": {
    "jsx": "react",
    "jsxFactory": "jsx"
  }
}
```

Then import `jsx` (or `h`) from the package:

```tsx
import { jsx } from "@serenity-is/domwise";
```

The package also provides a `/jsx-runtime` entry point for automatic runtime resolution, and a `/jsx-dev-runtime` entry point for development mode.

---

## Browser Support

All modern browsers that support ES2019+ features. Internet Explorer is not supported.

---

## License

**MIT** — see [LICENSE.md](./LICENSE.md).

This package includes derived work from [jsx-dom](https://github.com/alex-kinokon/jsx-dom) (BSD 3-Clause), [dom-expressions](https://github.com/ryansolid/dom-expressions) (MIT), and [tsx-dom](https://github.com/Lusito/tsx-dom) (MIT). See [NOTICE.md](./NOTICE.md) for full license attributions.