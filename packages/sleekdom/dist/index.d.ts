/**
 * Convert a `value` to a className string.
 * `value` can be a string, an array or a `Dictionary<boolean>`.
 */
export declare function className(value: any): string;
export interface BasicClassList {
	(value: Element): void;
	readonly size: number;
	readonly value: string;
	add(...tokens: string[]): void;
	remove(...tokens: string[]): void;
	toggle(token: string, force?: boolean): void;
	contains(token: string): boolean;
}
export type ClassName = string | {
	[key: string]: boolean;
} | false | null | undefined | ClassName[];
export type ClassNames = ClassName | BasicClassList | Iterable<string> | DOMTokenList;
export interface ConfigureJSXElement {
	svg: boolean;
}
/**
 * This technically should include `DocumentFragment` as well, but a lot of web APIs expect an `Element`.
 */
export type JSXElement = HTMLElement | (ConfigureJSXElement["svg"] extends false ? never : SVGElement);
export type RefObject<T> = {
	current: T | null;
};
export type RefCallback<T> = (instance: T) => void;
export type Ref<T> = RefCallback<T> | RefObject<T> | null;
export interface SignalLike<T> {
	value: T;
	peek(): T;
	subscribe(fn: (value: T) => void): () => void;
}
export type Signalish<T> = T | SignalLike<T>;
export type ComponentChild = string | number | Iterable<ComponentChild> | Array<ComponentChild> | {
	value: ComponentChild;
	peek: () => ComponentChild;
	subscribe: (cb: (newValue: ComponentChild) => void) => void;
} | JSXElement | NodeList | ChildNode | HTMLCollection | ShadowRootContainer | DocumentFragment | Text | Comment | boolean | null | undefined;
export type ComponentChildren = ComponentChild[] | ComponentChild;
export interface CustomDomAttributes<T> {
	children?: ComponentChildren;
	dangerouslySetInnerHTML?: {
		__html: string;
	};
	ref?: Ref<T>;
	/** @deprecated This is simply ignored as it only applies to v-dom  */
	key?: string | number;
	/** compat from jsx-dom/react */
	on?: Record<string, Function>;
	onCapture?: Record<string, Function>;
	/**
	 * This is essentially a reverse "is" attribute.
	 * If you specify it, the generated tag will be tsxTag and it will receive an "is" attribute with the tag you specified in your JSX.
	 * This is needed because we can't make the is-property associate with the correct component props.
	 */
	tsxTag?: keyof HTMLElementTagNameMap | keyof SVGElementTagNameMap;
}
export interface ElementAttributes<T> {
	className?: Signalish<string | ClassNames | RemoveAttribute>;
	tabIndex?: Signalish<number | string | RemoveAttribute>;
	onClickCapture?: EventHandlerUnion<T, MouseEvent> | undefined;
	onDblClickCapture?: EventHandlerUnion<T, MouseEvent> | undefined;
	onDoubleClick?: EventHandlerUnion<T, MouseEvent> | undefined;
	onDoubleClickCapture?: EventHandlerUnion<T, MouseEvent> | undefined;
}
export interface HTMLAttributes<T> {
	contentEditable?: Signalish<EnumeratedPseudoBoolean | EnumeratedAcceptsEmpty | "plaintext-only" | "inherit" | RemoveAttribute>;
	spellCheck?: Signalish<EnumeratedPseudoBoolean | EnumeratedAcceptsEmpty | RemoveAttribute>;
	dataset?: {
		[key: string]: string;
	} | undefined;
}
export interface SVGAttributes<T> {
	tabIndex?: Signalish<number | string | RemoveAttribute>;
}
export interface AnchorHTMLAttributes<T> {
	referrerPolicy?: Signalish<HTMLReferrerPolicy | RemoveAttribute>;
}
export interface ButtonHTMLAttributes<T> {
	autoFocus?: Signalish<boolean | RemoveAttribute>;
	formNoValidate?: Signalish<boolean | RemoveAttribute>;
}
export interface InputHTMLAttributes<T> {
	maxLength?: Signalish<number | RemoveAttribute>;
	minLength?: Signalish<number | RemoveAttribute>;
	readOnly?: Signalish<boolean | RemoveAttribute>;
}
export interface LabelHTMLAttributes<T> {
	htmlFor?: Signalish<string | RemoveAttribute>;
}
export interface ComponentClass<P = {}, T extends Node = JSXElement> {
	new (props: P): ComponentClass<P, T>;
	render(): JSXElement | null;
	defaultProps?: Partial<P> | undefined;
	readonly props?: P & {
		children?: ComponentChildren;
	};
	displayName?: string | undefined;
}
export type FunctionComponent<P = {}, T extends Node = JSXElement> = (props: P & {
	children?: ComponentChildren;
}) => T | null;
export type ComponentType<P = {}, T extends Node = JSXElement> = ComponentClass<P, T> | FunctionComponent<P, T>;
export type ShadowRootContainer = {
	ref: Ref<ShadowRoot>;
	attr: {
		clonable?: boolean;
		customElementRegistry?: CustomElementRegistry;
		delegatesFocus?: boolean;
		mode: ShadowRootMode;
		serializable?: boolean;
		slotAssignment?: SlotAssignmentMode;
	};
	children: ComponentChildren;
};
/** CSSStyleDeclaration contains methods, readonly properties and an index signature, which we all need to filter out. */
export type StyleProperties = Partial<Pick<CSSStyleDeclaration, {
	[K in keyof CSSStyleDeclaration]: K extends string ? CSSStyleDeclaration[K] extends string ? K : never : never;
}[keyof CSSStyleDeclaration]>>;
//import * as csstype from "csstype";
/**
 * Originally based on JSX types for Surplus and Inferno and adapted for `dom-expressions`.
 *
 * - https://github.com/adamhaile/surplus/blob/master/index.d.ts
 * - https://github.com/infernojs/inferno/blob/master/packages/inferno/src/core/types.ts
 *
 * MathML typings coming mostly from Preact
 *
 * - https://github.com/preactjs/preact/blob/07dc9f324e58569ce66634aa03fe8949b4190358/src/jsx.d.ts#L2575
 *
 * Checked against other frameworks via the following table:
 *
 * - https://potahtml.github.io/namespace-jsx-project/index.html
 *
 * # Typings on elements
 *
 * ## Attributes
 *
 * - Typings include attributes and not properties (unless the property Is special-cased, such
 *   textContent, event handlers, etc).
 * - Attributes are lowercase to avoid confusion with properties.
 * - Attributes are used "as is" and won't be transformed in any way (such to `lowercase` or from
 *   `dashed-case` to `camelCase`).
 *
 * ## Event Handlers
 *
 * - Event handlers use `camelCase` such `onClick` and will be delegated when possible, bubbling
 *   through the component tree, not the dom tree.
 * - Native event handlers use the namespace `on:` such `on:click`, and wont be delegated. bubbling
 *   the dom tree.
 * - A global case-insensitive event handler can be added by extending `EventHandlersElement<T>`
 * - A native `on:` event handler can be added by extending `CustomEvents<T>` interface
 *
 * ## Boolean Attributes (property setter that accepts `true | false`):
 *
 * - `(bool)true` adds the attribute `<video autoplay={true}/>` or in JSX as `<video autoplay/>`
 * - `(bool)false` removes the attribute from the DOM `<video autoplay={false}/>`
 * - `=""` may be accepted for the sake of parity with html `<video autoplay=""/>`
 * - `"true" | "false"` are NOT allowed, these are strings that evaluate to `(bool)true`
 *
 * ## Enumerated Attributes (attribute accepts 1 string value out of many)
 *
 * - Accepts any of the enumerated values, such: `"perhaps" | "maybe"`
 * - When one of the possible values is empty(in html that's for the attribute to be present), then it
 *   will also accept `(bool)true` to make it consistent with boolean attributes.
 *
 * Such `popover` attribute provides `"" | "manual" | "auto" | "hint"`.
 *
 * By NOT allowing `(bool)true` we will have to write `<div popover="" />`. Therefore, To make it
 * consistent with Boolean Attributes we accept `true | "" | "manual" | "auto" | "hint"`, such as:
 * `<div popover={true} />` or in JSX `<div popover />` is allowed and equivalent to `<div
 * popover="" />`
 *
 * ## Pseudo-Boolean Attributes (enumerated attributes that happen to accept the strings `"true" | "false"`)
 *
 * - Such `<div draggable="true"/>` or `<div draggable="false"/>`. The value of the attribute is a
 *   string not a boolean.
 * - `<div draggable={true}/>` is not valid because `(bool)true` is NOT transformed to the string
 *   `"true"`. Likewise `<div draggable={false}/>` removes the attribute from the element.
 * - MDN documentation https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/draggable
 *
 * ## All Of The Above In a nutshell
 *
 * - `(bool)true` adds an empty attribute
 * - `(bool)false` removes the attribute
 * - Attributes are lowercase
 * - Event handlers are camelCase
 * - Anything else is a `string` and used "as is"
 * - Additionally, an attribute may be removed by `undefined`
 *
 * ## Using Properties
 *
 * - The namespace `prop:` could be used to directly set properties in native elements and
 *   custom-elements. `<custom-element prop:myProp={true}/>` equivalent to `el.myProp = true`
 *
 * ## Interfaces
 *
 * Events
 *
 * 1. An event handler goes in `EventHandlersElement` when:
 *
 *    - `event` is global, that's to be defined in `HTMLElement` AND `SVGElement` AND `MathMLElement`
 *    - `event` is defined in `Element` (as `HTMLElement/MathMLElement/SVGElement` -> `Element`)
 * 2. `<body>`, `<svg>`, `<framesete>` are special as these include `window` events
 * 3. Any other event is special for its own tag.
 *
 * Browser Hierarchy
 *
 * - $Element (ex HTMLDivElement <div>) -> ... -> HTMLElement -> Element -> Node
 * - $Element (all math elements are MathMLElement) MathMLElement -> Element -> Node
 * - $Element`(ex SVGMaskElement <mask>) -> ... -> SVGElement -> Element -> Node
 *
 * Attributes
 *
 *      <div> -> ... -> HTMLAttributes -> ElementAttributes
 *      <svg> -> ... -> SVGAttributes -> ElementAttributes
 *      <math> -> ... -> MathMLAttributes -> ElementAttributes
 *
 *      ElementAttributes = `Element` + `Node` attributes (aka global attributes)
 *
 *      HTMLAttributes = `HTMLElement` attributes (aka HTML global attributes)
 *      SVGAttributes = `SVGElement` attributes (aka SVG global attributes)
 *      MathMLAttributes = `MathMLElement` attributes (aka MATH global attributes)
 *
 *      CustomAttributes = Framework attributes
 */
export type DOMElement = Element;
// Event handlers
export interface EventHandler<T, E extends Event> {
	(e: E & {
		currentTarget: T;
		target: DOMElement;
	}): void;
}
//interface BoundEventHandler<
//    T,
//    E extends Event,
//    EHandler extends EventHandler<T, any> = EventHandler<T, E>
//> {
//    0: (data: any, ...e: Parameters<EHandler>) => void;
//    1: any;
//}
export type EventHandlerUnion<T, E extends Event, EHandler extends EventHandler<T, any> = EventHandler<T, E>> = EHandler; // | BoundEventHandler<T, E, EHandler>;
//interface EventHandlerWithOptions<T, E extends Event, EHandler = EventHandler<T, E>>
//    extends AddEventListenerOptions,
//    EventListenerOptions {
//    handleEvent: EHandler;
//}
//type EventHandlerWithOptionsUnion<
//    T,
//    E extends Event,
//    EHandler extends EventHandler<T, any> = EventHandler<T, E>
//> = EHandler | EventHandlerWithOptions<T, E, EHandler>;
export interface InputEventHandler<T, E extends InputEvent> {
	(e: E & {
		currentTarget: T;
		target: T extends HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement ? T : DOMElement;
	}): void;
}
export type InputEventHandlerUnion<T, E extends InputEvent> = EventHandlerUnion<T, E, InputEventHandler<T, E>>;
export interface ChangeEventHandler<T, E extends Event> {
	(e: E & {
		currentTarget: T;
		target: T extends HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement ? T : DOMElement;
	}): void;
}
export type ChangeEventHandlerUnion<T, E extends Event> = EventHandlerUnion<T, E, ChangeEventHandler<T, E>>;
export interface FocusEventHandler<T, E extends FocusEvent> {
	(e: E & {
		currentTarget: T;
		target: T extends HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement ? T : DOMElement;
	}): void;
}
export type FocusEventHandlerUnion<T, E extends FocusEvent> = EventHandlerUnion<T, E, FocusEventHandler<T, E>>;
// end event handlers
//type ClassList =
//    | Record<string, boolean>
//    | Array<string | number | boolean | null | undefined | Record<string, boolean>>;
//const SERIALIZABLE: unique symbol;
export interface SerializableAttributeValue {
	toString(): string;
}
//interface CustomAttributes<T> {
//    ref?: T | ((el: T) => void) | undefined;
//    children?: FunctionMaybe<Element | undefined>;
//    $ServerOnly?: boolean | undefined;
//}
export type Accessor<T> = () => T;
export interface Directives {
}
export interface DirectiveFunctions {
	[x: string]: (el: DOMElement, accessor: Accessor<any>) => void;
}
export interface ExplicitProperties {
}
export type DirectiveAttributes = {
	[Key in keyof Directives as `use:${Key}`]?: Directives[Key];
};
export type DirectiveFunctionAttributes<T> = {
	[K in keyof DirectiveFunctions as string extends K ? never : `use:${K}`]?: DirectiveFunctions[K] extends (el: infer E, // will be unknown if not provided
	...rest: infer R // use rest so that we can check whether it's provided or not
	) => void ? T extends E // everything extends unknown if E is unknown
	 ? R extends [
		infer A
	] // check if has accessor provided
	 ? A extends Accessor<infer V> ? V // it's an accessor
	 : never // it isn't, type error
	 : true // no accessor provided
	 : never // T is the wrong element
	 : never;
};
export type PropAttributes = {
	[Key in keyof ExplicitProperties as `prop:${Key}`]?: ExplicitProperties[Key];
};
export type OnAttributes<T> = {};
// CSS
//interface CSSProperties extends csstype.PropertiesHyphen {
//    // Override
//    [key: `-${string}`]: string | number | undefined;
//}
export type CSSProperties = StyleProperties;
// TODO: Should we allow this?
// type ClassKeys = `class:${string}`;
// type CSSKeys = Exclude<keyof csstype.PropertiesHyphen, `-${string}`>;
// type CSSAttributes = {
//   [key in CSSKeys as `style:${key}`]: csstype.PropertiesHyphen[key];
// };
// BOOLEAN
/**
 * Boolean and Pseudo-Boolean Attributes Helpers.
 *
 * Please use the helpers to describe boolean and pseudo boolean attributes to make this file and
 * also the typings easier to understand and explain.
 */
export type BooleanAttribute = true | false | "";
export type EnumeratedPseudoBoolean = "false" | "true";
export type EnumeratedAcceptsEmpty = "" | true;
export type RemoveAttribute = undefined | false;
// ARIA
// All the WAI-ARIA 1.1 attributes from https://www.w3.org/TR/wai-aria-1.1/
export interface AriaAttributes {
	/**
	 * Identifies the currently active element when DOM focus is on a composite widget, textbox,
	 * group, or application.
	 */
	"aria-activedescendant"?: Signalish<string | RemoveAttribute>;
	/**
	 * Indicates whether assistive technologies will present all, or only parts of, the changed
	 * region based on the change notifications defined by the aria-relevant attribute.
	 */
	"aria-atomic"?: Signalish<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Similar to the global aria-label. Defines a string value that labels the current element,
	 * which is intended to be converted into Braille.
	 *
	 * @see aria-label.
	 */
	"aria-braillelabel"?: Signalish<string | RemoveAttribute>;
	/**
	 * Defines a human-readable, author-localized abbreviated description for the role of an element
	 * intended to be converted into Braille. Braille is not a one-to-one transliteration of letters
	 * and numbers, but rather it includes various abbreviations, contractions, and characters that
	 * represent words (known as logograms).
	 *
	 * Instead of converting long role descriptions to Braille, the aria-brailleroledescription
	 * attribute allows for providing an abbreviated version of the aria-roledescription value,
	 * which is a human-readable, author-localized description for the role of an element, for
	 * improved user experience with braille interfaces.
	 *
	 * @see aria-roledescription.
	 */
	"aria-brailleroledescription"?: Signalish<string | RemoveAttribute>;
	/**
	 * Indicates whether inputting text could trigger display of one or more predictions of the
	 * user's intended value for an input and specifies how predictions would be presented if they
	 * are made.
	 */
	"aria-autocomplete"?: Signalish<"none" | "inline" | "list" | "both" | RemoveAttribute>;
	/**
	 * Indicates an element is being modified and that assistive technologies MAY want to wait until
	 * the modifications are complete before exposing them to the user.
	 */
	"aria-busy"?: Signalish<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Indicates the current "checked" state of checkboxes, radio buttons, and other widgets.
	 *
	 * @see aria-pressed @see aria-selected.
	 */
	"aria-checked"?: Signalish<EnumeratedPseudoBoolean | "mixed" | RemoveAttribute>;
	/**
	 * Defines the total number of columns in a table, grid, or treegrid.
	 *
	 * @see aria-colindex.
	 */
	"aria-colcount"?: Signalish<number | string | RemoveAttribute>;
	/**
	 * Defines an element's column index or position with respect to the total number of columns
	 * within a table, grid, or treegrid.
	 *
	 * @see aria-colcount @see aria-colspan.
	 */
	"aria-colindex"?: Signalish<number | string | RemoveAttribute>;
	/** Defines a human-readable text alternative of the numeric aria-colindex. */
	"aria-colindextext"?: Signalish<number | string | RemoveAttribute>;
	/**
	 * Defines the number of columns spanned by a cell or gridcell within a table, grid, or
	 * treegrid.
	 *
	 * @see aria-colindex @see aria-rowspan.
	 */
	"aria-colspan"?: Signalish<number | string | RemoveAttribute>;
	/**
	 * Identifies the element (or elements) whose contents or presence are controlled by the current
	 * element.
	 *
	 * @see aria-owns.
	 */
	"aria-controls"?: Signalish<string | RemoveAttribute>;
	/**
	 * Indicates the element that represents the current item within a container or set of related
	 * elements.
	 */
	"aria-current"?: Signalish<EnumeratedPseudoBoolean | "page" | "step" | "location" | "date" | "time" | RemoveAttribute>;
	/**
	 * Identifies the element (or elements) that describes the object.
	 *
	 * @see aria-labelledby
	 */
	"aria-describedby"?: Signalish<string | RemoveAttribute>;
	/**
	 * Defines a string value that describes or annotates the current element.
	 *
	 * @see aria-describedby
	 */
	"aria-description"?: Signalish<string | RemoveAttribute>;
	/**
	 * Identifies the element that provides a detailed, extended description for the object.
	 *
	 * @see aria-describedby.
	 */
	"aria-details"?: Signalish<string | RemoveAttribute>;
	/**
	 * Indicates that the element is perceivable but disabled, so it is not editable or otherwise
	 * operable.
	 *
	 * @see aria-hidden @see aria-readonly.
	 */
	"aria-disabled"?: Signalish<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Indicates what functions can be performed when a dragged object is released on the drop
	 * target.
	 *
	 * @deprecated In ARIA 1.1
	 */
	"aria-dropeffect"?: Signalish<"none" | "copy" | "execute" | "link" | "move" | "popup" | RemoveAttribute>;
	/**
	 * Identifies the element that provides an error message for the object.
	 *
	 * @see aria-invalid @see aria-describedby.
	 */
	"aria-errormessage"?: Signalish<string | RemoveAttribute>;
	/**
	 * Indicates whether the element, or another grouping element it controls, is currently expanded
	 * or collapsed.
	 */
	"aria-expanded"?: Signalish<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Identifies the next element (or elements) in an alternate reading order of content which, at
	 * the user's discretion, allows assistive technology to override the general default of reading
	 * in document source order.
	 */
	"aria-flowto"?: Signalish<string | RemoveAttribute>;
	/**
	 * Indicates an element's "grabbed" state in a drag-and-drop operation.
	 *
	 * @deprecated In ARIA 1.1
	 */
	"aria-grabbed"?: Signalish<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Indicates the availability and type of interactive popup element, such as menu or dialog,
	 * that can be triggered by an element.
	 */
	"aria-haspopup"?: Signalish<EnumeratedPseudoBoolean | "menu" | "listbox" | "tree" | "grid" | "dialog" | RemoveAttribute>;
	/**
	 * Indicates whether the element is exposed to an accessibility API.
	 *
	 * @see aria-disabled.
	 */
	"aria-hidden"?: Signalish<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Indicates the entered value does not conform to the format expected by the application.
	 *
	 * @see aria-errormessage.
	 */
	"aria-invalid"?: Signalish<EnumeratedPseudoBoolean | "grammar" | "spelling" | RemoveAttribute>;
	/**
	 * Indicates keyboard shortcuts that an author has implemented to activate or give focus to an
	 * element.
	 */
	"aria-keyshortcuts"?: Signalish<string | RemoveAttribute>;
	/**
	 * Defines a string value that labels the current element.
	 *
	 * @see aria-labelledby.
	 */
	"aria-label"?: Signalish<string | RemoveAttribute>;
	/**
	 * Identifies the element (or elements) that labels the current element.
	 *
	 * @see aria-describedby.
	 */
	"aria-labelledby"?: Signalish<string | RemoveAttribute>;
	/** Defines the hierarchical level of an element within a structure. */
	"aria-level"?: Signalish<number | string | RemoveAttribute>;
	/**
	 * Indicates that an element will be updated, and describes the types of updates the user
	 * agents, assistive technologies, and user can expect from the live region.
	 */
	"aria-live"?: Signalish<"off" | "assertive" | "polite" | RemoveAttribute>;
	/** Indicates whether an element is modal when displayed. */
	"aria-modal"?: Signalish<EnumeratedPseudoBoolean | RemoveAttribute>;
	/** Indicates whether a text box accepts multiple lines of input or only a single line. */
	"aria-multiline"?: Signalish<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Indicates that the user may select more than one item from the current selectable
	 * descendants.
	 */
	"aria-multiselectable"?: Signalish<EnumeratedPseudoBoolean | RemoveAttribute>;
	/** Indicates whether the element's orientation is horizontal, vertical, or unknown/ambiguous. */
	"aria-orientation"?: Signalish<"horizontal" | "vertical" | RemoveAttribute>;
	/**
	 * Identifies an element (or elements) in order to define a visual, functional, or contextual
	 * parent/child relationship between DOM elements where the DOM hierarchy cannot be used to
	 * represent the relationship.
	 *
	 * @see aria-controls.
	 */
	"aria-owns"?: Signalish<string | RemoveAttribute>;
	/**
	 * Defines a short hint (a word or short phrase) intended to aid the user with data entry when
	 * the control has no value. A hint could be a sample value or a brief description of the
	 * expected format.
	 */
	"aria-placeholder"?: Signalish<string | RemoveAttribute>;
	/**
	 * Defines an element's number or position in the current set of listitems or treeitems. Not
	 * required if all elements in the set are present in the DOM.
	 *
	 * @see aria-setsize.
	 */
	"aria-posinset"?: Signalish<number | string | RemoveAttribute>;
	/**
	 * Indicates the current "pressed" state of toggle buttons.
	 *
	 * @see aria-checked @see aria-selected.
	 */
	"aria-pressed"?: Signalish<EnumeratedPseudoBoolean | "mixed" | RemoveAttribute>;
	/**
	 * Indicates that the element is not editable, but is otherwise operable.
	 *
	 * @see aria-disabled.
	 */
	"aria-readonly"?: Signalish<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Indicates what notifications the user agent will trigger when the accessibility tree within a
	 * live region is modified.
	 *
	 * @see aria-atomic.
	 */
	"aria-relevant"?: Signalish<"additions" | "additions removals" | "additions text" | "all" | "removals" | "removals additions" | "removals text" | "text" | "text additions" | "text removals" | RemoveAttribute>;
	/** Indicates that user input is required on the element before a form may be submitted. */
	"aria-required"?: Signalish<EnumeratedPseudoBoolean | RemoveAttribute>;
	/** Defines a human-readable, author-localized description for the role of an element. */
	"aria-roledescription"?: Signalish<string | RemoveAttribute>;
	/**
	 * Defines the total number of rows in a table, grid, or treegrid.
	 *
	 * @see aria-rowindex.
	 */
	"aria-rowcount"?: Signalish<number | string | RemoveAttribute>;
	/**
	 * Defines an element's row index or position with respect to the total number of rows within a
	 * table, grid, or treegrid.
	 *
	 * @see aria-rowcount @see aria-rowspan.
	 */
	"aria-rowindex"?: Signalish<number | string | RemoveAttribute>;
	/** Defines a human-readable text alternative of aria-rowindex. */
	"aria-rowindextext"?: Signalish<number | string | RemoveAttribute>;
	/**
	 * Defines the number of rows spanned by a cell or gridcell within a table, grid, or treegrid.
	 *
	 * @see aria-rowindex @see aria-colspan.
	 */
	"aria-rowspan"?: Signalish<number | string | RemoveAttribute>;
	/**
	 * Indicates the current "selected" state of various widgets.
	 *
	 * @see aria-checked @see aria-pressed.
	 */
	"aria-selected"?: Signalish<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Defines the number of items in the current set of listitems or treeitems. Not required if all
	 * elements in the set are present in the DOM.
	 *
	 * @see aria-posinset.
	 */
	"aria-setsize"?: Signalish<number | string | RemoveAttribute>;
	/** Indicates if items in a table or grid are sorted in ascending or descending order. */
	"aria-sort"?: Signalish<"none" | "ascending" | "descending" | "other" | RemoveAttribute>;
	/** Defines the maximum allowed value for a range widget. */
	"aria-valuemax"?: Signalish<number | string | RemoveAttribute>;
	/** Defines the minimum allowed value for a range widget. */
	"aria-valuemin"?: Signalish<number | string | RemoveAttribute>;
	/**
	 * Defines the current value for a range widget.
	 *
	 * @see aria-valuetext.
	 */
	"aria-valuenow"?: Signalish<number | string | RemoveAttribute>;
	/** Defines the human readable text alternative of aria-valuenow for a range widget. */
	"aria-valuetext"?: Signalish<string | RemoveAttribute>;
	role?: Signalish<"alert" | "alertdialog" | "application" | "article" | "banner" | "button" | "cell" | "checkbox" | "columnheader" | "combobox" | "complementary" | "contentinfo" | "definition" | "dialog" | "directory" | "document" | "feed" | "figure" | "form" | "grid" | "gridcell" | "group" | "heading" | "img" | "link" | "list" | "listbox" | "listitem" | "log" | "main" | "marquee" | "math" | "menu" | "menubar" | "menuitem" | "menuitemcheckbox" | "menuitemradio" | "meter" | "navigation" | "none" | "note" | "option" | "presentation" | "progressbar" | "radio" | "radiogroup" | "region" | "row" | "rowgroup" | "rowheader" | "scrollbar" | "search" | "searchbox" | "separator" | "slider" | "spinbutton" | "status" | "switch" | "tab" | "table" | "tablist" | "tabpanel" | "term" | "textbox" | "timer" | "toolbar" | "tooltip" | "tree" | "treegrid" | "treeitem" | RemoveAttribute>;
}
// EVENTS
/**
 * `Window` events, defined for `<body>`, `<svg>`, `<frameset>` tags.
 *
 * Excluding `EventHandlersElement` events already defined as globals that all tags share, such as
 * `onblur`.
 */
export interface EventHandlersWindow<T> {
	onAfterPrint?: EventHandlerUnion<T, Event> | undefined;
	onBeforePrint?: EventHandlerUnion<T, Event> | undefined;
	onBeforeUnload?: EventHandlerUnion<T, BeforeUnloadEvent> | undefined;
	onGamepadConnected?: EventHandlerUnion<T, GamepadEvent> | undefined;
	onGamepadDisconnected?: EventHandlerUnion<T, GamepadEvent> | undefined;
	onHashchange?: EventHandlerUnion<T, HashChangeEvent> | undefined;
	onLanguageChange?: EventHandlerUnion<T, Event> | undefined;
	onMessage?: EventHandlerUnion<T, MessageEvent> | undefined;
	onMessageError?: EventHandlerUnion<T, MessageEvent> | undefined;
	onOffline?: EventHandlerUnion<T, Event> | undefined;
	onOnline?: EventHandlerUnion<T, Event> | undefined;
	onPageHide?: EventHandlerUnion<T, PageTransitionEvent> | undefined;
	// TODO `PageRevealEvent` is currently undefined on TS
	onPageReveal?: EventHandlerUnion<T, Event> | undefined;
	onPageShow?: EventHandlerUnion<T, PageTransitionEvent> | undefined;
	// TODO `PageSwapEvent` is currently undefined on TS
	onPageSwap?: EventHandlerUnion<T, Event> | undefined;
	onPopstate?: EventHandlerUnion<T, PopStateEvent> | undefined;
	onRejectionHandled?: EventHandlerUnion<T, PromiseRejectionEvent> | undefined;
	onStorage?: EventHandlerUnion<T, StorageEvent> | undefined;
	onUnhandledRejection?: EventHandlerUnion<T, PromiseRejectionEvent> | undefined;
	onUnload?: EventHandlerUnion<T, Event> | undefined;
}
/**
 * Global `EventHandlersElement`, defined for all tags.
 *
 * That's events defined and shared BY ALL of the `HTMLElement/SVGElement/MathMLElement`
 * interfaces.
 *
 * Includes events defined for the `Element` interface.
 */
export interface EventHandlersElement<T> {
	onAbort?: EventHandlerUnion<T, UIEvent> | undefined;
	onAnimationCancel?: EventHandlerUnion<T, AnimationEvent> | undefined;
	onAnimationEnd?: EventHandlerUnion<T, AnimationEvent> | undefined;
	onAnimationIteration?: EventHandlerUnion<T, AnimationEvent> | undefined;
	onAnimationStart?: EventHandlerUnion<T, AnimationEvent> | undefined;
	onAuxClick?: EventHandlerUnion<T, PointerEvent> | undefined;
	onBeforeCopy?: EventHandlerUnion<T, ClipboardEvent> | undefined;
	onBeforeCut?: EventHandlerUnion<T, ClipboardEvent> | undefined;
	onBeforeInput?: InputEventHandlerUnion<T, InputEvent> | undefined;
	onBeforeMatch?: EventHandlerUnion<T, Event> | undefined;
	onBeforePaste?: EventHandlerUnion<T, ClipboardEvent> | undefined;
	onBeforeToggle?: EventHandlerUnion<T, ToggleEvent> | undefined;
	onBeforeXRSelect?: EventHandlerUnion<T, Event> | undefined;
	onBlur?: FocusEventHandlerUnion<T, FocusEvent> | undefined;
	onCancel?: EventHandlerUnion<T, Event> | undefined;
	onCanPlay?: EventHandlerUnion<T, Event> | undefined;
	onCanPlayThrough?: EventHandlerUnion<T, Event> | undefined;
	onChange?: ChangeEventHandlerUnion<T, Event> | undefined;
	onClick?: EventHandlerUnion<T, MouseEvent> | undefined;
	onClose?: EventHandlerUnion<T, Event> | undefined;
	// TODO `CommandEvent` is currently undefined in TS
	onCommand?: EventHandlerUnion<T, Event> | undefined;
	onCompositionEnd?: EventHandlerUnion<T, CompositionEvent> | undefined;
	onCompositionStart?: EventHandlerUnion<T, CompositionEvent> | undefined;
	onCompositionUpdate?: EventHandlerUnion<T, CompositionEvent> | undefined;
	onContentVisibilityAutoStateChange?: EventHandlerUnion<T, ContentVisibilityAutoStateChangeEvent> | undefined;
	onContextLost?: EventHandlerUnion<T, Event> | undefined;
	onContextMenu?: EventHandlerUnion<T, PointerEvent> | undefined;
	onContextRestored?: EventHandlerUnion<T, Event> | undefined;
	onCopy?: EventHandlerUnion<T, ClipboardEvent> | undefined;
	onCueChange?: EventHandlerUnion<T, Event> | undefined;
	onCut?: EventHandlerUnion<T, ClipboardEvent> | undefined;
	onDblClick?: EventHandlerUnion<T, MouseEvent> | undefined;
	onDrag?: EventHandlerUnion<T, DragEvent> | undefined;
	onDragEnd?: EventHandlerUnion<T, DragEvent> | undefined;
	onDragEnter?: EventHandlerUnion<T, DragEvent> | undefined;
	onDragExit?: EventHandlerUnion<T, DragEvent> | undefined;
	onDragLeave?: EventHandlerUnion<T, DragEvent> | undefined;
	onDragOver?: EventHandlerUnion<T, DragEvent> | undefined;
	onDragStart?: EventHandlerUnion<T, DragEvent> | undefined;
	onDrop?: EventHandlerUnion<T, DragEvent> | undefined;
	onDurationChange?: EventHandlerUnion<T, Event> | undefined;
	onEmptied?: EventHandlerUnion<T, Event> | undefined;
	onEnded?: EventHandlerUnion<T, Event> | undefined;
	onError?: EventHandlerUnion<T, ErrorEvent> | undefined;
	onFocus?: FocusEventHandlerUnion<T, FocusEvent> | undefined;
	onFocusIn?: FocusEventHandlerUnion<T, FocusEvent> | undefined;
	onFocusOut?: FocusEventHandlerUnion<T, FocusEvent> | undefined;
	onFormData?: EventHandlerUnion<T, FormDataEvent> | undefined;
	onFullscreenChange?: EventHandlerUnion<T, Event> | undefined;
	onFullscreenError?: EventHandlerUnion<T, Event> | undefined;
	onGotPointerCapture?: EventHandlerUnion<T, PointerEvent> | undefined;
	onInput?: InputEventHandlerUnion<T, InputEvent> | undefined;
	onInvalid?: EventHandlerUnion<T, Event> | undefined;
	onKeyDown?: EventHandlerUnion<T, KeyboardEvent> | undefined;
	onKeyPress?: EventHandlerUnion<T, KeyboardEvent> | undefined;
	onKeyUp?: EventHandlerUnion<T, KeyboardEvent> | undefined;
	onLoad?: EventHandlerUnion<T, Event> | undefined;
	onLoadedData?: EventHandlerUnion<T, Event> | undefined;
	onLoadedMetadata?: EventHandlerUnion<T, Event> | undefined;
	onLoadStart?: EventHandlerUnion<T, Event> | undefined;
	onLostPointerCapture?: EventHandlerUnion<T, PointerEvent> | undefined;
	onMouseDown?: EventHandlerUnion<T, MouseEvent> | undefined;
	onMouseEnter?: EventHandlerUnion<T, MouseEvent> | undefined;
	onMouseLeave?: EventHandlerUnion<T, MouseEvent> | undefined;
	onMouseMove?: EventHandlerUnion<T, MouseEvent> | undefined;
	onMouseOut?: EventHandlerUnion<T, MouseEvent> | undefined;
	onMouseOver?: EventHandlerUnion<T, MouseEvent> | undefined;
	onMouseUp?: EventHandlerUnion<T, MouseEvent> | undefined;
	onPaste?: EventHandlerUnion<T, ClipboardEvent> | undefined;
	onPause?: EventHandlerUnion<T, Event> | undefined;
	onPlay?: EventHandlerUnion<T, Event> | undefined;
	onPlaying?: EventHandlerUnion<T, Event> | undefined;
	onPointerCancel?: EventHandlerUnion<T, PointerEvent> | undefined;
	onPointerDown?: EventHandlerUnion<T, PointerEvent> | undefined;
	onPointerEnter?: EventHandlerUnion<T, PointerEvent> | undefined;
	onPointerLeave?: EventHandlerUnion<T, PointerEvent> | undefined;
	onPointerMove?: EventHandlerUnion<T, PointerEvent> | undefined;
	onPointerOut?: EventHandlerUnion<T, PointerEvent> | undefined;
	onPointerOver?: EventHandlerUnion<T, PointerEvent> | undefined;
	onPointerRawUpdate?: EventHandlerUnion<T, PointerEvent> | undefined;
	onPointerUp?: EventHandlerUnion<T, PointerEvent> | undefined;
	onProgress?: EventHandlerUnion<T, ProgressEvent> | undefined;
	onRateChange?: EventHandlerUnion<T, Event> | undefined;
	onReset?: EventHandlerUnion<T, Event> | undefined;
	onResize?: EventHandlerUnion<T, UIEvent> | undefined;
	onScroll?: EventHandlerUnion<T, Event> | undefined;
	onScrollEnd?: EventHandlerUnion<T, Event> | undefined;
	// todo `SnapEvent` is currently undefined in TS
	onScrollSnapChange?: EventHandlerUnion<T, Event> | undefined;
	// todo `SnapEvent` is currently undefined in TS
	onScrollSnapChanging?: EventHandlerUnion<T, Event> | undefined;
	onSecurityPolicyViolation?: EventHandlerUnion<T, SecurityPolicyViolationEvent> | undefined;
	onSeeked?: EventHandlerUnion<T, Event> | undefined;
	onSeeking?: EventHandlerUnion<T, Event> | undefined;
	onSelect?: EventHandlerUnion<T, Event> | undefined;
	onSelectionChange?: EventHandlerUnion<T, Event> | undefined;
	onSelectStart?: EventHandlerUnion<T, Event> | undefined;
	onSlotChange?: EventHandlerUnion<T, Event> | undefined;
	onStalled?: EventHandlerUnion<T, Event> | undefined;
	onSubmit?: EventHandlerUnion<T, SubmitEvent> | undefined;
	onSuspend?: EventHandlerUnion<T, Event> | undefined;
	onTimeUpdate?: EventHandlerUnion<T, Event> | undefined;
	onToggle?: EventHandlerUnion<T, ToggleEvent> | undefined;
	onTouchCancel?: EventHandlerUnion<T, TouchEvent> | undefined;
	onTouchEnd?: EventHandlerUnion<T, TouchEvent> | undefined;
	onTouchMove?: EventHandlerUnion<T, TouchEvent> | undefined;
	onTouchStart?: EventHandlerUnion<T, TouchEvent> | undefined;
	onTransitionCancel?: EventHandlerUnion<T, TransitionEvent> | undefined;
	onTransitionEnd?: EventHandlerUnion<T, TransitionEvent> | undefined;
	onTransitionRun?: EventHandlerUnion<T, TransitionEvent> | undefined;
	onTransitionStart?: EventHandlerUnion<T, TransitionEvent> | undefined;
	onVolumeChange?: EventHandlerUnion<T, Event> | undefined;
	onWaiting?: EventHandlerUnion<T, Event> | undefined;
	onWheel?: EventHandlerUnion<T, WheelEvent> | undefined;
}
// GLOBAL ATTRIBUTES
/**
 * Global `Element` + `Node` interface keys, shared by all tags regardless of their namespace:
 *
 * 1. That's `keys` that are defined BY ALL `HTMLElement/SVGElement/MathMLElement` interfaces.
 * 2. Includes `keys` defined by `Element` and `Node` interfaces.
 */
export interface ElementAttributes<T> extends CustomDomAttributes<T>, DirectiveAttributes, DirectiveFunctionAttributes<T>, PropAttributes, OnAttributes<T>, EventHandlersElement<T>, AriaAttributes {
	// [key: ClassKeys]: boolean;
	// properties
	innerHTML?: Signalish<string>;
	textContent?: Signalish<string | number>;
	// attributes
	autofocus?: Signalish<BooleanAttribute | RemoveAttribute>;
	class?: Signalish<string | ClassNames | RemoveAttribute>;
	elementtiming?: Signalish<string | RemoveAttribute>;
	id?: Signalish<string | RemoveAttribute>;
	nonce?: Signalish<string | RemoveAttribute>;
	part?: Signalish<string | RemoveAttribute>;
	slot?: Signalish<string | RemoveAttribute>;
	style?: Signalish<CSSProperties | string | RemoveAttribute>;
	tabindex?: Signalish<number | string | RemoveAttribute>;
}
/** Global `SVGElement` interface keys only. */
export interface SVGAttributes<T> extends ElementAttributes<T> {
	id?: Signalish<string | RemoveAttribute>;
	lang?: Signalish<string | RemoveAttribute>;
	tabindex?: Signalish<number | string | RemoveAttribute>;
	xmlns?: Signalish<string | RemoveAttribute>;
}
/** Global `HTMLElement` interface keys only. */
export interface HTMLAttributes<T> extends ElementAttributes<T> {
	// properties
	innerText?: Signalish<string | number>;
	// attributes
	accesskey?: Signalish<string | RemoveAttribute>;
	autocapitalize?: Signalish<HTMLAutocapitalize | RemoveAttribute>;
	autocorrect?: Signalish<"on" | "off" | RemoveAttribute>;
	contenteditable?: Signalish<EnumeratedPseudoBoolean | EnumeratedAcceptsEmpty | "plaintext-only" | "inherit" | RemoveAttribute>;
	dir?: Signalish<HTMLDir | RemoveAttribute>;
	draggable?: Signalish<EnumeratedPseudoBoolean | RemoveAttribute>;
	enterkeyhint?: Signalish<"enter" | "done" | "go" | "next" | "previous" | "search" | "send" | RemoveAttribute>;
	exportparts?: Signalish<string | RemoveAttribute>;
	hidden?: Signalish<EnumeratedAcceptsEmpty | "hidden" | "until-found" | RemoveAttribute>;
	inert?: Signalish<BooleanAttribute | RemoveAttribute>;
	inputmode?: Signalish<"decimal" | "email" | "none" | "numeric" | "search" | "tel" | "text" | "url" | RemoveAttribute>;
	is?: Signalish<string | RemoveAttribute>;
	lang?: Signalish<string | RemoveAttribute>;
	popover?: Signalish<EnumeratedAcceptsEmpty | "manual" | "auto" | RemoveAttribute>;
	spellcheck?: Signalish<EnumeratedPseudoBoolean | EnumeratedAcceptsEmpty | RemoveAttribute>;
	title?: Signalish<string | RemoveAttribute>;
	translate?: Signalish<"yes" | "no" | RemoveAttribute>;
	/** @experimental */
	virtualkeyboardpolicy?: Signalish<EnumeratedAcceptsEmpty | "auto" | "manual" | RemoveAttribute>;
	/** @experimental */
	writingsuggestions?: Signalish<EnumeratedPseudoBoolean | RemoveAttribute>;
	// Microdata
	itemid?: Signalish<string | RemoveAttribute>;
	itemprop?: Signalish<string | RemoveAttribute>;
	itemref?: Signalish<string | RemoveAttribute>;
	itemscope?: Signalish<BooleanAttribute | RemoveAttribute>;
	itemtype?: Signalish<string | RemoveAttribute>;
	// RDFa Attributes
	about?: Signalish<string | RemoveAttribute>;
	datatype?: Signalish<string | RemoveAttribute>;
	inlist?: Signalish<any | RemoveAttribute>;
	prefix?: Signalish<string | RemoveAttribute>;
	property?: Signalish<string | RemoveAttribute>;
	resource?: Signalish<string | RemoveAttribute>;
	typeof?: Signalish<string | RemoveAttribute>;
	vocab?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	contextmenu?: Signalish<string | RemoveAttribute>;
}
// HTML
export type HTMLAutocapitalize = "off" | "none" | "on" | "sentences" | "words" | "characters";
export type HTMLAutocomplete = "additional-name" | "address-level1" | "address-level2" | "address-level3" | "address-level4" | "address-line1" | "address-line2" | "address-line3" | "bday" | "bday-day" | "bday-month" | "bday-year" | "billing" | "cc-additional-name" | "cc-csc" | "cc-exp" | "cc-exp-month" | "cc-exp-year" | "cc-family-name" | "cc-given-name" | "cc-name" | "cc-number" | "cc-type" | "country" | "country-name" | "current-password" | "email" | "family-name" | "fax" | "given-name" | "home" | "honorific-prefix" | "honorific-suffix" | "impp" | "language" | "mobile" | "name" | "new-password" | "nickname" | "off" | "on" | "organization" | "organization-title" | "pager" | "photo" | "postal-code" | "sex" | "shipping" | "street-address" | "tel" | "tel-area-code" | "tel-country-code" | "tel-extension" | "tel-local" | "tel-local-prefix" | "tel-local-suffix" | "tel-national" | "transaction-amount" | "transaction-currency" | "url" | "username" | "work" | (string & {});
export type HTMLDir = "ltr" | "rtl" | "auto";
export type HTMLFormEncType = "application/x-www-form-urlencoded" | "multipart/form-data" | "text/plain";
export type HTMLFormMethod = "post" | "get" | "dialog";
export type HTMLCrossorigin = "anonymous" | "use-credentials" | EnumeratedAcceptsEmpty;
export type HTMLReferrerPolicy = "no-referrer" | "no-referrer-when-downgrade" | "origin" | "origin-when-cross-origin" | "same-origin" | "strict-origin" | "strict-origin-when-cross-origin" | "unsafe-url";
export type HTMLIframeSandbox = "allow-downloads-without-user-activation" | "allow-downloads" | "allow-forms" | "allow-modals" | "allow-orientation-lock" | "allow-pointer-lock" | "allow-popups" | "allow-popups-to-escape-sandbox" | "allow-presentation" | "allow-same-origin" | "allow-scripts" | "allow-storage-access-by-user-activation" | "allow-top-navigation" | "allow-top-navigation-by-user-activation" | "allow-top-navigation-to-custom-protocols";
export type HTMLLinkAs = "audio" | "document" | "embed" | "fetch" | "font" | "image" | "object" | "script" | "style" | "track" | "video" | "worker";
export interface AnchorHTMLAttributes<T> extends HTMLAttributes<T> {
	download?: Signalish<string | true | RemoveAttribute>;
	href?: Signalish<string | RemoveAttribute>;
	hreflang?: Signalish<string | RemoveAttribute>;
	ping?: Signalish<string | RemoveAttribute>;
	referrerpolicy?: Signalish<HTMLReferrerPolicy | RemoveAttribute>;
	rel?: Signalish<string | RemoveAttribute>;
	target?: Signalish<"_self" | "_blank" | "_parent" | "_top" | (string & {}) | RemoveAttribute>;
	type?: Signalish<string | RemoveAttribute>;
	/** @experimental */
	attributionsrc?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	charset?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	coords?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	name?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	rev?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	shape?: Signalish<"rect" | "circle" | "poly" | "default" | RemoveAttribute>;
}
export interface AudioHTMLAttributes<T> extends MediaHTMLAttributes<T> {
}
export interface AreaHTMLAttributes<T> extends HTMLAttributes<T> {
	alt?: Signalish<string | RemoveAttribute>;
	coords?: Signalish<string | RemoveAttribute>;
	download?: Signalish<string | RemoveAttribute>;
	href?: Signalish<string | RemoveAttribute>;
	ping?: Signalish<string | RemoveAttribute>;
	referrerpolicy?: Signalish<HTMLReferrerPolicy | RemoveAttribute>;
	rel?: Signalish<string | RemoveAttribute>;
	shape?: Signalish<"rect" | "circle" | "poly" | "default" | RemoveAttribute>;
	target?: Signalish<"_self" | "_blank" | "_parent" | "_top" | (string & {}) | RemoveAttribute>;
	/** @experimental */
	attributionsrc?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	nohref?: Signalish<BooleanAttribute | RemoveAttribute>;
}
export interface BaseHTMLAttributes<T> extends HTMLAttributes<T> {
	href?: Signalish<string | RemoveAttribute>;
	target?: Signalish<"_self" | "_blank" | "_parent" | "_top" | (string & {}) | RemoveAttribute>;
}
export interface BdoHTMLAttributes<T> extends HTMLAttributes<T> {
	dir?: Signalish<"ltr" | "rtl" | RemoveAttribute>;
}
export interface BlockquoteHTMLAttributes<T> extends HTMLAttributes<T> {
	cite?: Signalish<string | RemoveAttribute>;
}
export interface BodyHTMLAttributes<T> extends HTMLAttributes<T>, EventHandlersWindow<T> {
}
export interface ButtonHTMLAttributes<T> extends HTMLAttributes<T> {
	disabled?: Signalish<BooleanAttribute | RemoveAttribute>;
	form?: Signalish<string | RemoveAttribute>;
	formaction?: Signalish<string | SerializableAttributeValue | RemoveAttribute>;
	formenctype?: Signalish<HTMLFormEncType | RemoveAttribute>;
	formmethod?: Signalish<HTMLFormMethod | RemoveAttribute>;
	formnovalidate?: Signalish<BooleanAttribute | RemoveAttribute>;
	formtarget?: Signalish<"_self" | "_blank" | "_parent" | "_top" | (string & {}) | RemoveAttribute>;
	name?: Signalish<string | RemoveAttribute>;
	popovertarget?: Signalish<string | RemoveAttribute>;
	popovertargetaction?: Signalish<"hide" | "show" | "toggle" | RemoveAttribute>;
	type?: Signalish<"submit" | "reset" | "button" | "menu" | RemoveAttribute>;
	value?: Signalish<string | RemoveAttribute>;
	/** @experimental */
	command?: Signalish<"show-modal" | "close" | "show-popover" | "hide-popover" | "toggle-popover" | (string & {}) | RemoveAttribute>;
	/** @experimental */
	commandfor?: Signalish<string | RemoveAttribute>;
}
export interface CanvasHTMLAttributes<T> extends HTMLAttributes<T> {
	height?: Signalish<number | string | RemoveAttribute>;
	width?: Signalish<number | string | RemoveAttribute>;
	/**
	 * @deprecated
	 * @non-standard
	 */
	"moz-opaque"?: Signalish<BooleanAttribute | RemoveAttribute>;
}
export interface CaptionHTMLAttributes<T> extends HTMLAttributes<T> {
	/** @deprecated */
	align?: Signalish<"left" | "center" | "right" | RemoveAttribute>;
}
export interface ColHTMLAttributes<T> extends HTMLAttributes<T> {
	span?: Signalish<number | string | RemoveAttribute>;
	/** @deprecated */
	align?: Signalish<"left" | "center" | "right" | "justify" | "char" | RemoveAttribute>;
	/** @deprecated */
	bgcolor?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	char?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	charoff?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	valign?: Signalish<"baseline" | "bottom" | "middle" | "top" | RemoveAttribute>;
	/** @deprecated */
	width?: Signalish<number | string | RemoveAttribute>;
}
export interface ColgroupHTMLAttributes<T> extends HTMLAttributes<T> {
	span?: Signalish<number | string | RemoveAttribute>;
	/** @deprecated */
	align?: Signalish<"left" | "center" | "right" | "justify" | "char" | RemoveAttribute>;
	/** @deprecated */
	bgcolor?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	char?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	charoff?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	valign?: Signalish<"baseline" | "bottom" | "middle" | "top" | RemoveAttribute>;
	/** @deprecated */
	width?: Signalish<number | string | RemoveAttribute>;
}
export interface DataHTMLAttributes<T> extends HTMLAttributes<T> {
	value?: Signalish<string | string[] | number | RemoveAttribute>;
}
export interface DetailsHtmlAttributes<T> extends HTMLAttributes<T> {
	name?: Signalish<string | RemoveAttribute>;
	open?: Signalish<BooleanAttribute | RemoveAttribute>;
}
export interface DialogHtmlAttributes<T> extends HTMLAttributes<T> {
	open?: Signalish<BooleanAttribute | RemoveAttribute>;
	/**
	 * Do not add the `tabindex` property to the `<dialog>` element as it is not interactive and
	 * does not receive focus. The dialog's contents, including the close button contained in the
	 * dialog, can receive focus and be interactive.
	 *
	 * @see https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/dialog#usage_notes
	 */
	tabindex?: never;
	/** @experimental */
	closedby?: Signalish<"any" | "closerequest" | "none" | RemoveAttribute>;
}
export interface EmbedHTMLAttributes<T> extends HTMLAttributes<T> {
	height?: Signalish<number | string | RemoveAttribute>;
	src?: Signalish<string | RemoveAttribute>;
	type?: Signalish<string | RemoveAttribute>;
	width?: Signalish<number | string | RemoveAttribute>;
	/** @deprecated */
	align?: Signalish<"left" | "right" | "justify" | "center" | RemoveAttribute>;
	/** @deprecated */
	name?: Signalish<string | RemoveAttribute>;
}
export interface FieldsetHTMLAttributes<T> extends HTMLAttributes<T> {
	disabled?: Signalish<BooleanAttribute | RemoveAttribute>;
	form?: Signalish<string | RemoveAttribute>;
	name?: Signalish<string | RemoveAttribute>;
}
export interface FormHTMLAttributes<T> extends HTMLAttributes<T> {
	"accept-charset"?: Signalish<string | RemoveAttribute>;
	action?: Signalish<string | SerializableAttributeValue | RemoveAttribute>;
	autocomplete?: Signalish<"on" | "off" | RemoveAttribute>;
	encoding?: Signalish<HTMLFormEncType | RemoveAttribute>;
	enctype?: Signalish<HTMLFormEncType | RemoveAttribute>;
	method?: Signalish<HTMLFormMethod | RemoveAttribute>;
	name?: Signalish<string | RemoveAttribute>;
	novalidate?: Signalish<BooleanAttribute | RemoveAttribute>;
	rel?: Signalish<string | RemoveAttribute>;
	target?: Signalish<"_self" | "_blank" | "_parent" | "_top" | (string & {}) | RemoveAttribute>;
	/** @deprecated */
	accept?: Signalish<string | RemoveAttribute>;
}
export interface IframeHTMLAttributes<T> extends HTMLAttributes<T> {
	allow?: Signalish<string | RemoveAttribute>;
	allowfullscreen?: Signalish<BooleanAttribute | RemoveAttribute>;
	height?: Signalish<number | string | RemoveAttribute>;
	loading?: Signalish<"eager" | "lazy" | RemoveAttribute>;
	name?: Signalish<string | RemoveAttribute>;
	referrerpolicy?: Signalish<HTMLReferrerPolicy | RemoveAttribute>;
	sandbox?: Signalish<HTMLIframeSandbox | string | RemoveAttribute>;
	src?: Signalish<string | RemoveAttribute>;
	srcdoc?: Signalish<string | RemoveAttribute>;
	width?: Signalish<number | string | RemoveAttribute>;
	/** @experimental */
	adauctionheaders?: Signalish<BooleanAttribute | RemoveAttribute>;
	/**
	 * @non-standard
	 * @experimental
	 */
	browsingtopics?: Signalish<BooleanAttribute | RemoveAttribute>;
	/** @experimental */
	credentialless?: Signalish<BooleanAttribute | RemoveAttribute>;
	/** @experimental */
	csp?: Signalish<string | RemoveAttribute>;
	/** @experimental */
	privatetoken?: Signalish<string | RemoveAttribute>;
	/** @experimental */
	sharedstoragewritable?: Signalish<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	align?: Signalish<string | RemoveAttribute>;
	/**
	 * @deprecated
	 * @non-standard
	 */
	allowpaymentrequest?: Signalish<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	allowtransparency?: Signalish<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	frameborder?: Signalish<number | string | RemoveAttribute>;
	/** @deprecated */
	longdesc?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	marginheight?: Signalish<number | string | RemoveAttribute>;
	/** @deprecated */
	marginwidth?: Signalish<number | string | RemoveAttribute>;
	/** @deprecated */
	scrolling?: Signalish<"yes" | "no" | "auto" | RemoveAttribute>;
	/** @deprecated */
	seamless?: Signalish<BooleanAttribute | RemoveAttribute>;
}
export interface ImgHTMLAttributes<T> extends HTMLAttributes<T> {
	alt?: Signalish<string | RemoveAttribute>;
	browsingtopics?: Signalish<string | RemoveAttribute>;
	crossorigin?: Signalish<HTMLCrossorigin | RemoveAttribute>;
	decoding?: Signalish<"sync" | "async" | "auto" | RemoveAttribute>;
	fetchpriority?: Signalish<"high" | "low" | "auto" | RemoveAttribute>;
	height?: Signalish<number | string | RemoveAttribute>;
	ismap?: Signalish<BooleanAttribute | RemoveAttribute>;
	loading?: Signalish<"eager" | "lazy" | RemoveAttribute>;
	referrerpolicy?: Signalish<HTMLReferrerPolicy | RemoveAttribute>;
	sizes?: Signalish<string | RemoveAttribute>;
	src?: Signalish<string | RemoveAttribute>;
	srcset?: Signalish<string | RemoveAttribute>;
	usemap?: Signalish<string | RemoveAttribute>;
	width?: Signalish<number | string | RemoveAttribute>;
	/** @experimental */
	attributionsrc?: Signalish<string | RemoveAttribute>;
	/** @experimental */
	sharedstoragewritable?: Signalish<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	align?: Signalish<"top" | "middle" | "bottom" | "left" | "right" | RemoveAttribute>;
	/** @deprecated */
	border?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	hspace?: Signalish<number | string | RemoveAttribute>;
	/** @deprecated */
	intrinsicsize?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	longdesc?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	lowsrc?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	name?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	vspace?: Signalish<number | string | RemoveAttribute>;
}
export interface InputHTMLAttributes<T> extends HTMLAttributes<T> {
	accept?: Signalish<string | RemoveAttribute>;
	alpha?: Signalish<BooleanAttribute | RemoveAttribute>;
	alt?: Signalish<string | RemoveAttribute>;
	autocomplete?: Signalish<HTMLAutocomplete | RemoveAttribute>;
	capture?: Signalish<"user" | "environment" | RemoveAttribute>;
	checked?: Signalish<BooleanAttribute | RemoveAttribute>;
	colorspace?: Signalish<string | RemoveAttribute>;
	dirname?: Signalish<string | RemoveAttribute>;
	disabled?: Signalish<BooleanAttribute | RemoveAttribute>;
	form?: Signalish<string | RemoveAttribute>;
	formaction?: Signalish<string | SerializableAttributeValue | RemoveAttribute>;
	formenctype?: Signalish<HTMLFormEncType | RemoveAttribute>;
	formmethod?: Signalish<HTMLFormMethod | RemoveAttribute>;
	formnovalidate?: Signalish<BooleanAttribute | RemoveAttribute>;
	formtarget?: Signalish<string | RemoveAttribute>;
	height?: Signalish<number | string | RemoveAttribute>;
	list?: Signalish<string | RemoveAttribute>;
	max?: Signalish<number | string | RemoveAttribute>;
	maxlength?: Signalish<number | string | RemoveAttribute>;
	min?: Signalish<number | string | RemoveAttribute>;
	minlength?: Signalish<number | string | RemoveAttribute>;
	multiple?: Signalish<BooleanAttribute | RemoveAttribute>;
	name?: Signalish<string | RemoveAttribute>;
	pattern?: Signalish<string | RemoveAttribute>;
	placeholder?: Signalish<string | RemoveAttribute>;
	popovertarget?: Signalish<string | RemoveAttribute>;
	popovertargetaction?: Signalish<"hide" | "show" | "toggle" | RemoveAttribute>;
	readonly?: Signalish<BooleanAttribute | RemoveAttribute>;
	required?: Signalish<BooleanAttribute | RemoveAttribute>;
	// https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/search#results
	results?: Signalish<number | RemoveAttribute>;
	size?: Signalish<number | string | RemoveAttribute>;
	src?: Signalish<string | RemoveAttribute>;
	step?: Signalish<number | string | RemoveAttribute>;
	type?: Signalish<"button" | "checkbox" | "color" | "date" | "datetime-local" | "email" | "file" | "hidden" | "image" | "month" | "number" | "password" | "radio" | "range" | "reset" | "search" | "submit" | "tel" | "text" | "time" | "url" | "week" | (string & {}) | RemoveAttribute>;
	value?: Signalish<string | string[] | number | RemoveAttribute>;
	width?: Signalish<number | string | RemoveAttribute>;
	/** @non-standard */
	incremental?: Signalish<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	align?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	usemap?: Signalish<string | RemoveAttribute>;
}
export interface ModHTMLAttributes<T> extends HTMLAttributes<T> {
	cite?: Signalish<string | RemoveAttribute>;
	datetime?: Signalish<string | RemoveAttribute>;
}
export interface LabelHTMLAttributes<T> extends HTMLAttributes<T> {
	for?: Signalish<string | RemoveAttribute>;
}
export interface LiHTMLAttributes<T> extends HTMLAttributes<T> {
	value?: Signalish<number | string | RemoveAttribute>;
	/** @deprecated */
	type?: Signalish<"1" | "a" | "A" | "i" | "I" | RemoveAttribute>;
}
export interface LinkHTMLAttributes<T> extends HTMLAttributes<T> {
	as?: Signalish<HTMLLinkAs | RemoveAttribute>;
	blocking?: Signalish<"render" | RemoveAttribute>;
	color?: Signalish<string | RemoveAttribute>;
	crossorigin?: Signalish<HTMLCrossorigin | RemoveAttribute>;
	disabled?: Signalish<BooleanAttribute | RemoveAttribute>;
	fetchpriority?: Signalish<"high" | "low" | "auto" | RemoveAttribute>;
	href?: Signalish<string | RemoveAttribute>;
	hreflang?: Signalish<string | RemoveAttribute>;
	imagesizes?: Signalish<string | RemoveAttribute>;
	imagesrcset?: Signalish<string | RemoveAttribute>;
	integrity?: Signalish<string | RemoveAttribute>;
	media?: Signalish<string | RemoveAttribute>;
	referrerpolicy?: Signalish<HTMLReferrerPolicy | RemoveAttribute>;
	rel?: Signalish<string | RemoveAttribute>;
	sizes?: Signalish<string | RemoveAttribute>;
	type?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	charset?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	rev?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	target?: Signalish<string | RemoveAttribute>;
}
export interface MapHTMLAttributes<T> extends HTMLAttributes<T> {
	name?: Signalish<string | RemoveAttribute>;
}
export interface MediaHTMLAttributes<T> extends HTMLAttributes<T> {
	autoplay?: Signalish<BooleanAttribute | RemoveAttribute>;
	controls?: Signalish<BooleanAttribute | RemoveAttribute>;
	controlslist?: Signalish<"nodownload" | "nofullscreen" | "noplaybackrate" | "noremoteplayback" | (string & {}) | RemoveAttribute>;
	crossorigin?: Signalish<HTMLCrossorigin | RemoveAttribute>;
	disableremoteplayback?: Signalish<BooleanAttribute | RemoveAttribute>;
	loop?: Signalish<BooleanAttribute | RemoveAttribute>;
	muted?: Signalish<BooleanAttribute | RemoveAttribute>;
	preload?: Signalish<"none" | "metadata" | "auto" | EnumeratedAcceptsEmpty | RemoveAttribute>;
	src?: Signalish<string | RemoveAttribute>;
	onEncrypted?: EventHandlerUnion<T, MediaEncryptedEvent> | undefined;
	// "on:encrypted"?: EventHandlerWithOptionsUnion<T, MediaEncryptedEvent> | undefined;
	onWaitingForKey?: EventHandlerUnion<T, Event> | undefined;
	// "on:waitingforkey"?: EventHandlerWithOptionsUnion<T, Event> | undefined;
	/** @deprecated */
	mediagroup?: Signalish<string | RemoveAttribute>;
}
export interface MenuHTMLAttributes<T> extends HTMLAttributes<T> {
	/** @deprecated */
	compact?: Signalish<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	label?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	type?: Signalish<"context" | "toolbar" | RemoveAttribute>;
}
export interface MetaHTMLAttributes<T> extends HTMLAttributes<T> {
	"http-equiv"?: Signalish<"content-security-policy" | "content-type" | "default-style" | "x-ua-compatible" | "refresh" | RemoveAttribute>;
	charset?: Signalish<string | RemoveAttribute>;
	content?: Signalish<string | RemoveAttribute>;
	media?: Signalish<string | RemoveAttribute>;
	name?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	scheme?: Signalish<string | RemoveAttribute>;
}
export interface MeterHTMLAttributes<T> extends HTMLAttributes<T> {
	form?: Signalish<string | RemoveAttribute>;
	high?: Signalish<number | string | RemoveAttribute>;
	low?: Signalish<number | string | RemoveAttribute>;
	max?: Signalish<number | string | RemoveAttribute>;
	min?: Signalish<number | string | RemoveAttribute>;
	optimum?: Signalish<number | string | RemoveAttribute>;
	value?: Signalish<string | string[] | number | RemoveAttribute>;
}
export interface QuoteHTMLAttributes<T> extends HTMLAttributes<T> {
	cite?: Signalish<string | RemoveAttribute>;
}
export interface ObjectHTMLAttributes<T> extends HTMLAttributes<T> {
	data?: Signalish<string | RemoveAttribute>;
	form?: Signalish<string | RemoveAttribute>;
	height?: Signalish<number | string | RemoveAttribute>;
	name?: Signalish<string | RemoveAttribute>;
	type?: Signalish<string | RemoveAttribute>;
	width?: Signalish<number | string | RemoveAttribute>;
	wmode?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	align?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	archive?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	border?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	classid?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	code?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	codebase?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	codetype?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	declare?: Signalish<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	hspace?: Signalish<number | string | RemoveAttribute>;
	/** @deprecated */
	standby?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	usemap?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	vspace?: Signalish<number | string | RemoveAttribute>;
	/** @deprecated */
	typemustmatch?: Signalish<BooleanAttribute | RemoveAttribute>;
}
export interface OlHTMLAttributes<T> extends HTMLAttributes<T> {
	reversed?: Signalish<BooleanAttribute | RemoveAttribute>;
	start?: Signalish<number | string | RemoveAttribute>;
	type?: Signalish<"1" | "a" | "A" | "i" | "I" | RemoveAttribute>;
	/**
	 * @deprecated
	 * @non-standard
	 */
	compact?: Signalish<BooleanAttribute | RemoveAttribute>;
}
export interface OptgroupHTMLAttributes<T> extends HTMLAttributes<T> {
	disabled?: Signalish<BooleanAttribute | RemoveAttribute>;
	label?: Signalish<string | RemoveAttribute>;
}
export interface OptionHTMLAttributes<T> extends HTMLAttributes<T> {
	disabled?: Signalish<BooleanAttribute | RemoveAttribute>;
	label?: Signalish<string | RemoveAttribute>;
	selected?: Signalish<BooleanAttribute | RemoveAttribute>;
	value?: Signalish<string | string[] | number | RemoveAttribute>;
}
export interface OutputHTMLAttributes<T> extends HTMLAttributes<T> {
	for?: Signalish<string | RemoveAttribute>;
	form?: Signalish<string | RemoveAttribute>;
	name?: Signalish<string | RemoveAttribute>;
}
export interface ProgressHTMLAttributes<T> extends HTMLAttributes<T> {
	max?: Signalish<number | string | RemoveAttribute>;
	value?: Signalish<string | string[] | number | RemoveAttribute>;
}
export interface ScriptHTMLAttributes<T> extends HTMLAttributes<T> {
	async?: Signalish<BooleanAttribute | RemoveAttribute>;
	blocking?: Signalish<"render" | RemoveAttribute>;
	crossorigin?: Signalish<HTMLCrossorigin | RemoveAttribute>;
	defer?: Signalish<BooleanAttribute | RemoveAttribute>;
	fetchpriority?: Signalish<"high" | "low" | "auto" | RemoveAttribute>;
	for?: Signalish<string | RemoveAttribute>;
	integrity?: Signalish<string | RemoveAttribute>;
	nomodule?: Signalish<BooleanAttribute | RemoveAttribute>;
	referrerpolicy?: Signalish<HTMLReferrerPolicy | RemoveAttribute>;
	src?: Signalish<string | RemoveAttribute>;
	type?: Signalish<"importmap" | "module" | "speculationrules" | (string & {}) | RemoveAttribute>;
	/** @experimental */
	attributionsrc?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	charset?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	event?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	language?: Signalish<string | RemoveAttribute>;
}
export interface SelectHTMLAttributes<T> extends HTMLAttributes<T> {
	autocomplete?: Signalish<HTMLAutocomplete | RemoveAttribute>;
	disabled?: Signalish<BooleanAttribute | RemoveAttribute>;
	form?: Signalish<string | RemoveAttribute>;
	multiple?: Signalish<BooleanAttribute | RemoveAttribute>;
	name?: Signalish<string | RemoveAttribute>;
	required?: Signalish<BooleanAttribute | RemoveAttribute>;
	size?: Signalish<number | string | RemoveAttribute>;
	value?: Signalish<string | string[] | number | RemoveAttribute>;
}
export interface HTMLSlotElementAttributes<T> extends HTMLAttributes<T> {
	name?: Signalish<string | RemoveAttribute>;
}
export interface SourceHTMLAttributes<T> extends HTMLAttributes<T> {
	height?: Signalish<number | string | RemoveAttribute>;
	media?: Signalish<string | RemoveAttribute>;
	sizes?: Signalish<string | RemoveAttribute>;
	src?: Signalish<string | RemoveAttribute>;
	srcset?: Signalish<string | RemoveAttribute>;
	type?: Signalish<string | RemoveAttribute>;
	width?: Signalish<number | string | RemoveAttribute>;
}
export interface StyleHTMLAttributes<T> extends HTMLAttributes<T> {
	blocking?: Signalish<"render" | RemoveAttribute>;
	media?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	scoped?: Signalish<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	type?: Signalish<string | RemoveAttribute>;
}
export interface TdHTMLAttributes<T> extends HTMLAttributes<T> {
	colspan?: Signalish<number | string | RemoveAttribute>;
	headers?: Signalish<string | RemoveAttribute>;
	rowspan?: Signalish<number | string | RemoveAttribute>;
	/** @deprecated */
	abbr?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	align?: Signalish<"left" | "center" | "right" | "justify" | "char" | RemoveAttribute>;
	/** @deprecated */
	axis?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	bgcolor?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	char?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	charoff?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	height?: Signalish<number | string | RemoveAttribute>;
	/** @deprecated */
	nowrap?: Signalish<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	scope?: Signalish<"col" | "row" | "rowgroup" | "colgroup" | RemoveAttribute>;
	/** @deprecated */
	valign?: Signalish<"baseline" | "bottom" | "middle" | "top" | RemoveAttribute>;
	/** @deprecated */
	width?: Signalish<number | string | RemoveAttribute>;
}
export interface TemplateHTMLAttributes<T> extends HTMLAttributes<T> {
	shadowrootclonable?: Signalish<BooleanAttribute | RemoveAttribute>;
	shadowrootdelegatesfocus?: Signalish<BooleanAttribute | RemoveAttribute>;
	shadowrootmode?: Signalish<"open" | "closed" | RemoveAttribute>;
	shadowrootcustomelementregistry?: Signalish<BooleanAttribute | RemoveAttribute>;
	/** @experimental */
	shadowrootserializable?: Signalish<BooleanAttribute | RemoveAttribute>;
}
export interface TextareaHTMLAttributes<T> extends HTMLAttributes<T> {
	autocomplete?: Signalish<HTMLAutocomplete | RemoveAttribute>;
	cols?: Signalish<number | string | RemoveAttribute>;
	dirname?: Signalish<string | RemoveAttribute>;
	disabled?: Signalish<BooleanAttribute | RemoveAttribute>;
	form?: Signalish<string | RemoveAttribute>;
	maxlength?: Signalish<number | string | RemoveAttribute>;
	minlength?: Signalish<number | string | RemoveAttribute>;
	name?: Signalish<string | RemoveAttribute>;
	placeholder?: Signalish<string | RemoveAttribute>;
	readonly?: Signalish<BooleanAttribute | RemoveAttribute>;
	required?: Signalish<BooleanAttribute | RemoveAttribute>;
	rows?: Signalish<number | string | RemoveAttribute>;
	value?: Signalish<string | string[] | number | RemoveAttribute>;
	wrap?: Signalish<"hard" | "soft" | "off" | RemoveAttribute>;
}
export interface ThHTMLAttributes<T> extends HTMLAttributes<T> {
	abbr?: Signalish<string | RemoveAttribute>;
	colspan?: Signalish<number | string | RemoveAttribute>;
	headers?: Signalish<string | RemoveAttribute>;
	rowspan?: Signalish<number | string | RemoveAttribute>;
	scope?: Signalish<"col" | "row" | "rowgroup" | "colgroup" | RemoveAttribute>;
	/** @deprecated */
	align?: Signalish<"left" | "center" | "right" | "justify" | "char" | RemoveAttribute>;
	/** @deprecated */
	axis?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	bgcolor?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	char?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	charoff?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	height?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	nowrap?: Signalish<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	valign?: Signalish<"baseline" | "bottom" | "middle" | "top" | RemoveAttribute>;
	/** @deprecated */
	width?: Signalish<number | string | RemoveAttribute>;
}
export interface TimeHTMLAttributes<T> extends HTMLAttributes<T> {
	datetime?: Signalish<string | RemoveAttribute>;
}
export interface TrackHTMLAttributes<T> extends HTMLAttributes<T> {
	default?: Signalish<BooleanAttribute | RemoveAttribute>;
	kind?: Signalish<"alternative" | "descriptions" | "main" | "main-desc" | "translation" | "commentary" | "subtitles" | "captions" | "chapters" | "metadata" | RemoveAttribute>;
	label?: Signalish<string | RemoveAttribute>;
	src?: Signalish<string | RemoveAttribute>;
	srclang?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	mediagroup?: Signalish<string | RemoveAttribute>;
}
export interface VideoHTMLAttributes<T> extends MediaHTMLAttributes<T> {
	disablepictureinpicture?: Signalish<BooleanAttribute | RemoveAttribute>;
	height?: Signalish<number | string | RemoveAttribute>;
	playsinline?: Signalish<BooleanAttribute | RemoveAttribute>;
	poster?: Signalish<string | RemoveAttribute>;
	width?: Signalish<number | string | RemoveAttribute>;
	onEnterPictureInPicture?: EventHandlerUnion<T, PictureInPictureEvent> | undefined;
	// "on:enterpictureinpicture"?: EventHandlerWithOptionsUnion<T, PictureInPictureEvent> | undefined;
	onLeavePictureInPicture?: EventHandlerUnion<T, PictureInPictureEvent> | undefined;
}
export interface WebViewHTMLAttributes<T> extends HTMLAttributes<T> {
	allowpopups?: Signalish<BooleanAttribute | RemoveAttribute>;
	disableblinkfeatures?: Signalish<string | RemoveAttribute>;
	disablewebsecurity?: Signalish<BooleanAttribute | RemoveAttribute>;
	enableblinkfeatures?: Signalish<string | RemoveAttribute>;
	httpreferrer?: Signalish<string | RemoveAttribute>;
	nodeintegration?: Signalish<BooleanAttribute | RemoveAttribute>;
	nodeintegrationinsubframes?: Signalish<BooleanAttribute | RemoveAttribute>;
	partition?: Signalish<string | RemoveAttribute>;
	plugins?: Signalish<BooleanAttribute | RemoveAttribute>;
	preload?: Signalish<string | RemoveAttribute>;
	src?: Signalish<string | RemoveAttribute>;
	useragent?: Signalish<string | RemoveAttribute>;
	webpreferences?: Signalish<string | RemoveAttribute>;
	// does this exists?
	allowfullscreen?: Signalish<BooleanAttribute | RemoveAttribute>;
	autosize?: Signalish<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	blinkfeatures?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	disableguestresize?: Signalish<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	guestinstance?: Signalish<string | RemoveAttribute>;
}
// SVG
export type SVGPreserveAspectRatio_ = "none" | "xMinYMin" | "xMidYMin" | "xMaxYMin" | "xMinYMid" | "xMidYMid" | "xMaxYMid" | "xMinYMax" | "xMidYMax" | "xMaxYMax" | "xMinYMin meet" | "xMidYMin meet" | "xMaxYMin meet" | "xMinYMid meet" | "xMidYMid meet" | "xMaxYMid meet" | "xMinYMax meet" | "xMidYMax meet" | "xMaxYMax meet" | "xMinYMin slice" | "xMidYMin slice" | "xMaxYMin slice" | "xMinYMid slice" | "xMidYMid slice" | "xMaxYMid slice" | "xMinYMax slice" | "xMidYMax slice" | "xMaxYMax slice";
export type ImagePreserveAspectRatio = SVGPreserveAspectRatio_ | "defer none" | "defer xMinYMin" | "defer xMidYMin" | "defer xMaxYMin" | "defer xMinYMid" | "defer xMidYMid" | "defer xMaxYMid" | "defer xMinYMax" | "defer xMidYMax" | "defer xMaxYMax" | "defer xMinYMin meet" | "defer xMidYMin meet" | "defer xMaxYMin meet" | "defer xMinYMid meet" | "defer xMidYMid meet" | "defer xMaxYMid meet" | "defer xMinYMax meet" | "defer xMidYMax meet" | "defer xMaxYMax meet" | "defer xMinYMin slice" | "defer xMidYMin slice" | "defer xMaxYMin slice" | "defer xMinYMid slice" | "defer xMidYMid slice" | "defer xMaxYMid slice" | "defer xMinYMax slice" | "defer xMidYMax slice" | "defer xMaxYMax slice";
export type SVGUnits = "userSpaceOnUse" | "objectBoundingBox";
export interface StylableSVGAttributes {
	class?: Signalish<string | ClassNames | RemoveAttribute>;
	style?: Signalish<CSSProperties | string | RemoveAttribute>;
}
export interface TransformableSVGAttributes {
	transform?: Signalish<string | RemoveAttribute>;
}
export interface ConditionalProcessingSVGAttributes {
	requiredExtensions?: Signalish<string | RemoveAttribute>;
	requiredFeatures?: Signalish<string | RemoveAttribute>;
	systemLanguage?: Signalish<string | RemoveAttribute>;
}
export interface ExternalResourceSVGAttributes {
	externalResourcesRequired?: Signalish<EnumeratedPseudoBoolean | RemoveAttribute>;
}
export interface AnimationTimingSVGAttributes {
	begin?: Signalish<string | RemoveAttribute>;
	dur?: Signalish<string | RemoveAttribute>;
	end?: Signalish<string | RemoveAttribute>;
	fill?: Signalish<"freeze" | "remove" | RemoveAttribute>;
	max?: Signalish<string | RemoveAttribute>;
	min?: Signalish<string | RemoveAttribute>;
	repeatCount?: Signalish<number | "indefinite" | RemoveAttribute>;
	repeatDur?: Signalish<string | RemoveAttribute>;
	restart?: Signalish<"always" | "whenNotActive" | "never" | RemoveAttribute>;
}
export interface AnimationValueSVGAttributes {
	by?: Signalish<number | string | RemoveAttribute>;
	calcMode?: Signalish<"discrete" | "linear" | "paced" | "spline" | RemoveAttribute>;
	from?: Signalish<number | string | RemoveAttribute>;
	keySplines?: Signalish<string | RemoveAttribute>;
	keyTimes?: Signalish<string | RemoveAttribute>;
	to?: Signalish<number | string | RemoveAttribute>;
	values?: Signalish<string | RemoveAttribute>;
}
export interface AnimationAdditionSVGAttributes {
	accumulate?: Signalish<"none" | "sum" | RemoveAttribute>;
	additive?: Signalish<"replace" | "sum" | RemoveAttribute>;
	attributeName?: Signalish<string | RemoveAttribute>;
}
export interface AnimationAttributeTargetSVGAttributes {
	attributeName?: Signalish<string | RemoveAttribute>;
	attributeType?: Signalish<"CSS" | "XML" | "auto" | RemoveAttribute>;
}
export interface PresentationSVGAttributes {
	"alignment-baseline"?: Signalish<"auto" | "baseline" | "before-edge" | "text-before-edge" | "middle" | "central" | "after-edge" | "text-after-edge" | "ideographic" | "alphabetic" | "hanging" | "mathematical" | "inherit" | RemoveAttribute>;
	"baseline-shift"?: Signalish<number | string | RemoveAttribute>;
	"clip-path"?: Signalish<string | RemoveAttribute>;
	"clip-rule"?: Signalish<"nonzero" | "evenodd" | "inherit" | RemoveAttribute>;
	"color-interpolation"?: Signalish<"auto" | "sRGB" | "linearRGB" | "inherit" | RemoveAttribute>;
	"color-interpolation-filters"?: Signalish<"auto" | "sRGB" | "linearRGB" | "inherit" | RemoveAttribute>;
	"color-profile"?: Signalish<string | RemoveAttribute>;
	"color-rendering"?: Signalish<"auto" | "optimizeSpeed" | "optimizeQuality" | "inherit" | RemoveAttribute>;
	"dominant-baseline"?: Signalish<"auto" | "text-bottom" | "alphabetic" | "ideographic" | "middle" | "central" | "mathematical" | "hanging" | "text-top" | "inherit" | RemoveAttribute>;
	"enable-background"?: Signalish<string | RemoveAttribute>;
	"fill-opacity"?: Signalish<number | string | "inherit" | RemoveAttribute>;
	"fill-rule"?: Signalish<"nonzero" | "evenodd" | "inherit" | RemoveAttribute>;
	"flood-color"?: Signalish<string | RemoveAttribute>;
	"flood-opacity"?: Signalish<number | string | "inherit" | RemoveAttribute>;
	"font-family"?: Signalish<string | RemoveAttribute>;
	"font-size"?: Signalish<string | RemoveAttribute>;
	"font-size-adjust"?: Signalish<number | string | RemoveAttribute>;
	"font-stretch"?: Signalish<string | RemoveAttribute>;
	"font-style"?: Signalish<"normal" | "italic" | "oblique" | "inherit" | RemoveAttribute>;
	"font-variant"?: Signalish<string | RemoveAttribute>;
	"font-weight"?: Signalish<number | string | RemoveAttribute>;
	"glyph-orientation-horizontal"?: Signalish<string | RemoveAttribute>;
	"glyph-orientation-vertical"?: Signalish<string | RemoveAttribute>;
	"image-rendering"?: Signalish<"auto" | "optimizeQuality" | "optimizeSpeed" | "inherit" | RemoveAttribute>;
	"letter-spacing"?: Signalish<number | string | RemoveAttribute>;
	"lighting-color"?: Signalish<string | RemoveAttribute>;
	"marker-end"?: Signalish<string | RemoveAttribute>;
	"marker-mid"?: Signalish<string | RemoveAttribute>;
	"marker-start"?: Signalish<string | RemoveAttribute>;
	"pointer-events"?: Signalish<"bounding-box" | "visiblePainted" | "visibleFill" | "visibleStroke" | "visible" | "painted" | "color" | "fill" | "stroke" | "all" | "none" | "inherit" | RemoveAttribute>;
	"shape-rendering"?: Signalish<"auto" | "optimizeSpeed" | "crispEdges" | "geometricPrecision" | "inherit" | RemoveAttribute>;
	"stop-color"?: Signalish<string | RemoveAttribute>;
	"stop-opacity"?: Signalish<number | string | "inherit" | RemoveAttribute>;
	"stroke-dasharray"?: Signalish<string | RemoveAttribute>;
	"stroke-dashoffset"?: Signalish<number | string | RemoveAttribute>;
	"stroke-linecap"?: Signalish<"butt" | "round" | "square" | "inherit" | RemoveAttribute>;
	"stroke-linejoin"?: Signalish<"arcs" | "bevel" | "miter" | "miter-clip" | "round" | "inherit" | RemoveAttribute>;
	"stroke-miterlimit"?: Signalish<number | string | "inherit" | RemoveAttribute>;
	"stroke-opacity"?: Signalish<number | string | "inherit" | RemoveAttribute>;
	"stroke-width"?: Signalish<number | string | RemoveAttribute>;
	"text-anchor"?: Signalish<"start" | "middle" | "end" | "inherit" | RemoveAttribute>;
	"text-decoration"?: Signalish<"none" | "underline" | "overline" | "line-through" | "blink" | "inherit" | RemoveAttribute>;
	"text-rendering"?: Signalish<"auto" | "optimizeSpeed" | "optimizeLegibility" | "geometricPrecision" | "inherit" | RemoveAttribute>;
	"unicode-bidi"?: Signalish<string | RemoveAttribute>;
	"word-spacing"?: Signalish<number | string | RemoveAttribute>;
	"writing-mode"?: Signalish<"lr-tb" | "rl-tb" | "tb-rl" | "lr" | "rl" | "tb" | "inherit" | RemoveAttribute>;
	clip?: Signalish<string | RemoveAttribute>;
	color?: Signalish<string | RemoveAttribute>;
	cursor?: Signalish<string | RemoveAttribute>;
	direction?: Signalish<"ltr" | "rtl" | "inherit" | RemoveAttribute>;
	display?: Signalish<string | RemoveAttribute>;
	fill?: Signalish<string | RemoveAttribute>;
	filter?: Signalish<string | RemoveAttribute>;
	kerning?: Signalish<string | RemoveAttribute>;
	mask?: Signalish<string | RemoveAttribute>;
	opacity?: Signalish<number | string | "inherit" | RemoveAttribute>;
	overflow?: Signalish<"visible" | "hidden" | "scroll" | "auto" | "inherit" | RemoveAttribute>;
	pathLength?: Signalish<string | number | RemoveAttribute>;
	stroke?: Signalish<string | RemoveAttribute>;
	visibility?: Signalish<"visible" | "hidden" | "collapse" | "inherit" | RemoveAttribute>;
}
export interface AnimationElementSVGAttributes<T> extends SVGAttributes<T>, ExternalResourceSVGAttributes, ConditionalProcessingSVGAttributes {
	// TODO TimeEvent is currently undefined on TS
	onBegin?: EventHandlerUnion<T, Event> | undefined;
	// "on:begin"?: EventHandlerWithOptionsUnion<T, Event> | undefined;
	// TODO TimeEvent is currently undefined on TS
	onEnd?: EventHandlerUnion<T, Event> | undefined;
	// "on:end"?: EventHandlerWithOptionsUnion<T, Event> | undefined;
	// TODO TimeEvent is currently undefined on TS
	onRepeat?: EventHandlerUnion<T, Event> | undefined;
}
export interface ContainerElementSVGAttributes<T> extends SVGAttributes<T>, ShapeElementSVGAttributes<T>, Pick<PresentationSVGAttributes, "clip-path" | "mask" | "cursor" | "opacity" | "filter" | "enable-background" | "color-interpolation" | "color-rendering"> {
}
export interface FilterPrimitiveElementSVGAttributes<T> extends SVGAttributes<T>, Pick<PresentationSVGAttributes, "color-interpolation-filters"> {
	height?: Signalish<number | string | RemoveAttribute>;
	result?: Signalish<string | RemoveAttribute>;
	width?: Signalish<number | string | RemoveAttribute>;
	x?: Signalish<number | string | RemoveAttribute>;
	y?: Signalish<number | string | RemoveAttribute>;
}
export interface SingleInputFilterSVGAttributes {
	in?: Signalish<string | RemoveAttribute>;
}
export interface DoubleInputFilterSVGAttributes {
	in?: Signalish<string | RemoveAttribute>;
	in2?: Signalish<string | RemoveAttribute>;
}
export interface FitToViewBoxSVGAttributes {
	preserveAspectRatio?: Signalish<SVGPreserveAspectRatio_ | RemoveAttribute>;
	viewBox?: Signalish<string | RemoveAttribute>;
}
export interface GradientElementSVGAttributes<T> extends SVGAttributes<T>, ExternalResourceSVGAttributes, StylableSVGAttributes {
	gradientTransform?: Signalish<string | RemoveAttribute>;
	gradientUnits?: Signalish<SVGUnits | RemoveAttribute>;
	href?: Signalish<string | RemoveAttribute>;
	spreadMethod?: Signalish<"pad" | "reflect" | "repeat" | RemoveAttribute>;
}
export interface GraphicsElementSVGAttributes<T> extends SVGAttributes<T>, Pick<PresentationSVGAttributes, "clip-rule" | "mask" | "pointer-events" | "cursor" | "opacity" | "filter" | "display" | "visibility" | "color-interpolation" | "color-rendering"> {
}
export interface LightSourceElementSVGAttributes<T> extends SVGAttributes<T> {
}
export interface NewViewportSVGAttributes<T> extends SVGAttributes<T>, Pick<PresentationSVGAttributes, "overflow" | "clip"> {
	viewBox?: Signalish<string | RemoveAttribute>;
}
export interface ShapeElementSVGAttributes<T> extends SVGAttributes<T>, Pick<PresentationSVGAttributes, "color" | "fill" | "fill-rule" | "fill-opacity" | "stroke" | "stroke-width" | "stroke-linecap" | "stroke-linejoin" | "stroke-miterlimit" | "stroke-dasharray" | "stroke-dashoffset" | "stroke-opacity" | "shape-rendering" | "pathLength"> {
}
export interface TextContentElementSVGAttributes<T> extends SVGAttributes<T>, Pick<PresentationSVGAttributes, "font-family" | "font-style" | "font-variant" | "font-weight" | "font-stretch" | "font-size" | "font-size-adjust" | "kerning" | "letter-spacing" | "word-spacing" | "text-decoration" | "glyph-orientation-horizontal" | "glyph-orientation-vertical" | "direction" | "unicode-bidi" | "text-anchor" | "dominant-baseline" | "color" | "fill" | "fill-rule" | "fill-opacity" | "stroke" | "stroke-width" | "stroke-linecap" | "stroke-linejoin" | "stroke-miterlimit" | "stroke-dasharray" | "stroke-dashoffset" | "stroke-opacity"> {
}
export interface ZoomAndPanSVGAttributes {
	/**
	 * @deprecated
	 * @non-standard
	 */
	zoomAndPan?: Signalish<"disable" | "magnify" | RemoveAttribute>;
}
export interface AnimateSVGAttributes<T> extends AnimationElementSVGAttributes<T>, AnimationAttributeTargetSVGAttributes, AnimationTimingSVGAttributes, AnimationValueSVGAttributes, AnimationAdditionSVGAttributes, Pick<PresentationSVGAttributes, "color-interpolation" | "color-rendering"> {
}
export interface AnimateMotionSVGAttributes<T> extends AnimationElementSVGAttributes<T>, AnimationTimingSVGAttributes, AnimationValueSVGAttributes, AnimationAdditionSVGAttributes {
	keyPoints?: Signalish<string | RemoveAttribute>;
	origin?: Signalish<"default" | RemoveAttribute>;
	path?: Signalish<string | RemoveAttribute>;
	rotate?: Signalish<number | string | "auto" | "auto-reverse" | RemoveAttribute>;
}
export interface AnimateTransformSVGAttributes<T> extends AnimationElementSVGAttributes<T>, AnimationAttributeTargetSVGAttributes, AnimationTimingSVGAttributes, AnimationValueSVGAttributes, AnimationAdditionSVGAttributes {
	type?: Signalish<"translate" | "scale" | "rotate" | "skewX" | "skewY" | RemoveAttribute>;
}
export interface CircleSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path"> {
	cx?: Signalish<number | string | RemoveAttribute>;
	cy?: Signalish<number | string | RemoveAttribute>;
	r?: Signalish<number | string | RemoveAttribute>;
}
export interface ClipPathSVGAttributes<T> extends SVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path"> {
	clipPathUnits?: Signalish<SVGUnits | RemoveAttribute>;
}
export interface DefsSVGAttributes<T> extends ContainerElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes {
}
export interface DescSVGAttributes<T> extends SVGAttributes<T>, StylableSVGAttributes {
}
export interface EllipseSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path"> {
	cx?: Signalish<number | string | RemoveAttribute>;
	cy?: Signalish<number | string | RemoveAttribute>;
	rx?: Signalish<number | string | RemoveAttribute>;
	ry?: Signalish<number | string | RemoveAttribute>;
}
export interface FeBlendSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, DoubleInputFilterSVGAttributes, StylableSVGAttributes {
	mode?: Signalish<"normal" | "multiply" | "screen" | "darken" | "lighten" | RemoveAttribute>;
}
export interface FeColorMatrixSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
	type?: Signalish<"matrix" | "saturate" | "hueRotate" | "luminanceToAlpha" | RemoveAttribute>;
	values?: Signalish<string | RemoveAttribute>;
}
export interface FeComponentTransferSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
}
export interface FeCompositeSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, DoubleInputFilterSVGAttributes, StylableSVGAttributes {
	k1?: Signalish<number | string | RemoveAttribute>;
	k2?: Signalish<number | string | RemoveAttribute>;
	k3?: Signalish<number | string | RemoveAttribute>;
	k4?: Signalish<number | string | RemoveAttribute>;
	operator?: Signalish<"over" | "in" | "out" | "atop" | "xor" | "arithmetic" | RemoveAttribute>;
}
export interface FeConvolveMatrixSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
	bias?: Signalish<number | string | RemoveAttribute>;
	divisor?: Signalish<number | string | RemoveAttribute>;
	edgeMode?: Signalish<"duplicate" | "wrap" | "none" | RemoveAttribute>;
	kernelMatrix?: Signalish<string | RemoveAttribute>;
	kernelUnitLength?: Signalish<number | string | RemoveAttribute>;
	order?: Signalish<number | string | RemoveAttribute>;
	preserveAlpha?: Signalish<EnumeratedPseudoBoolean | RemoveAttribute>;
	targetX?: Signalish<number | string | RemoveAttribute>;
	targetY?: Signalish<number | string | RemoveAttribute>;
}
export interface FeDiffuseLightingSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes, Pick<PresentationSVGAttributes, "color" | "lighting-color"> {
	diffuseConstant?: Signalish<number | string | RemoveAttribute>;
	kernelUnitLength?: Signalish<number | string | RemoveAttribute>;
	surfaceScale?: Signalish<number | string | RemoveAttribute>;
}
export interface FeDisplacementMapSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, DoubleInputFilterSVGAttributes, StylableSVGAttributes {
	scale?: Signalish<number | string | RemoveAttribute>;
	xChannelSelector?: Signalish<"R" | "G" | "B" | "A" | RemoveAttribute>;
	yChannelSelector?: Signalish<"R" | "G" | "B" | "A" | RemoveAttribute>;
}
export interface FeDistantLightSVGAttributes<T> extends LightSourceElementSVGAttributes<T> {
	azimuth?: Signalish<number | string | RemoveAttribute>;
	elevation?: Signalish<number | string | RemoveAttribute>;
}
export interface FeDropShadowSVGAttributes<T> extends SVGAttributes<T>, FilterPrimitiveElementSVGAttributes<T>, StylableSVGAttributes, Pick<PresentationSVGAttributes, "color" | "flood-color" | "flood-opacity"> {
	dx?: Signalish<number | string | RemoveAttribute>;
	dy?: Signalish<number | string | RemoveAttribute>;
	stdDeviation?: Signalish<number | string | RemoveAttribute>;
}
export interface FeFloodSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, StylableSVGAttributes, Pick<PresentationSVGAttributes, "color" | "flood-color" | "flood-opacity"> {
}
export interface FeFuncSVGAttributes<T> extends SVGAttributes<T> {
	amplitude?: Signalish<number | string | RemoveAttribute>;
	exponent?: Signalish<number | string | RemoveAttribute>;
	intercept?: Signalish<number | string | RemoveAttribute>;
	offset?: Signalish<number | string | RemoveAttribute>;
	slope?: Signalish<number | string | RemoveAttribute>;
	tableValues?: Signalish<string | RemoveAttribute>;
	type?: Signalish<"identity" | "table" | "discrete" | "linear" | "gamma" | RemoveAttribute>;
}
export interface FeGaussianBlurSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
	stdDeviation?: Signalish<number | string | RemoveAttribute>;
}
export interface FeImageSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, ExternalResourceSVGAttributes, StylableSVGAttributes {
	href?: Signalish<string | RemoveAttribute>;
	preserveAspectRatio?: Signalish<SVGPreserveAspectRatio_ | RemoveAttribute>;
}
export interface FeMergeSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, StylableSVGAttributes {
}
export interface FeMergeNodeSVGAttributes<T> extends SVGAttributes<T>, SingleInputFilterSVGAttributes {
}
export interface FeMorphologySVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
	operator?: Signalish<"erode" | "dilate" | RemoveAttribute>;
	radius?: Signalish<number | string | RemoveAttribute>;
}
export interface FeOffsetSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
	dx?: Signalish<number | string | RemoveAttribute>;
	dy?: Signalish<number | string | RemoveAttribute>;
}
export interface FePointLightSVGAttributes<T> extends LightSourceElementSVGAttributes<T> {
	x?: Signalish<number | string | RemoveAttribute>;
	y?: Signalish<number | string | RemoveAttribute>;
	z?: Signalish<number | string | RemoveAttribute>;
}
export interface FeSpecularLightingSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes, Pick<PresentationSVGAttributes, "color" | "lighting-color"> {
	kernelUnitLength?: Signalish<number | string | RemoveAttribute>;
	specularConstant?: Signalish<string | RemoveAttribute>;
	specularExponent?: Signalish<string | RemoveAttribute>;
	surfaceScale?: Signalish<string | RemoveAttribute>;
}
export interface FeSpotLightSVGAttributes<T> extends LightSourceElementSVGAttributes<T> {
	limitingConeAngle?: Signalish<number | string | RemoveAttribute>;
	pointsAtX?: Signalish<number | string | RemoveAttribute>;
	pointsAtY?: Signalish<number | string | RemoveAttribute>;
	pointsAtZ?: Signalish<number | string | RemoveAttribute>;
	specularExponent?: Signalish<number | string | RemoveAttribute>;
	x?: Signalish<number | string | RemoveAttribute>;
	y?: Signalish<number | string | RemoveAttribute>;
	z?: Signalish<number | string | RemoveAttribute>;
}
export interface FeTileSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
}
export interface FeTurbulanceSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, StylableSVGAttributes {
	baseFrequency?: Signalish<number | string | RemoveAttribute>;
	numOctaves?: Signalish<number | string | RemoveAttribute>;
	seed?: Signalish<number | string | RemoveAttribute>;
	stitchTiles?: Signalish<"stitch" | "noStitch" | RemoveAttribute>;
	type?: Signalish<"fractalNoise" | "turbulence" | RemoveAttribute>;
}
export interface FilterSVGAttributes<T> extends SVGAttributes<T>, ExternalResourceSVGAttributes, StylableSVGAttributes {
	filterRes?: Signalish<number | string | RemoveAttribute>;
	filterUnits?: Signalish<SVGUnits | RemoveAttribute>;
	height?: Signalish<number | string | RemoveAttribute>;
	primitiveUnits?: Signalish<SVGUnits | RemoveAttribute>;
	width?: Signalish<number | string | RemoveAttribute>;
	x?: Signalish<number | string | RemoveAttribute>;
	y?: Signalish<number | string | RemoveAttribute>;
}
export interface ForeignObjectSVGAttributes<T> extends NewViewportSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "display" | "visibility"> {
	height?: Signalish<number | string | RemoveAttribute>;
	width?: Signalish<number | string | RemoveAttribute>;
	x?: Signalish<number | string | RemoveAttribute>;
	y?: Signalish<number | string | RemoveAttribute>;
}
export interface GSVGAttributes<T> extends ContainerElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "display" | "visibility"> {
}
export interface ImageSVGAttributes<T> extends NewViewportSVGAttributes<T>, GraphicsElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "color-profile" | "image-rendering"> {
	height?: Signalish<number | string | RemoveAttribute>;
	href?: Signalish<string | RemoveAttribute>;
	preserveAspectRatio?: Signalish<ImagePreserveAspectRatio | RemoveAttribute>;
	width?: Signalish<number | string | RemoveAttribute>;
	x?: Signalish<number | string | RemoveAttribute>;
	y?: Signalish<number | string | RemoveAttribute>;
}
export interface LineSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "marker-start" | "marker-mid" | "marker-end"> {
	x1?: Signalish<number | string | RemoveAttribute>;
	x2?: Signalish<number | string | RemoveAttribute>;
	y1?: Signalish<number | string | RemoveAttribute>;
	y2?: Signalish<number | string | RemoveAttribute>;
}
export interface LinearGradientSVGAttributes<T> extends GradientElementSVGAttributes<T> {
	x1?: Signalish<number | string | RemoveAttribute>;
	x2?: Signalish<number | string | RemoveAttribute>;
	y1?: Signalish<number | string | RemoveAttribute>;
	y2?: Signalish<number | string | RemoveAttribute>;
}
export interface MarkerSVGAttributes<T> extends ContainerElementSVGAttributes<T>, ExternalResourceSVGAttributes, StylableSVGAttributes, FitToViewBoxSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "overflow" | "clip"> {
	markerHeight?: Signalish<number | string | RemoveAttribute>;
	markerUnits?: Signalish<"strokeWidth" | "userSpaceOnUse" | RemoveAttribute>;
	markerWidth?: Signalish<number | string | RemoveAttribute>;
	orient?: Signalish<string | RemoveAttribute>;
	refX?: Signalish<number | string | RemoveAttribute>;
	refY?: Signalish<number | string | RemoveAttribute>;
}
export interface MaskSVGAttributes<T> extends Omit<ContainerElementSVGAttributes<T>, "opacity" | "filter">, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path"> {
	height?: Signalish<number | string | RemoveAttribute>;
	maskContentUnits?: Signalish<SVGUnits | RemoveAttribute>;
	maskUnits?: Signalish<SVGUnits | RemoveAttribute>;
	width?: Signalish<number | string | RemoveAttribute>;
	x?: Signalish<number | string | RemoveAttribute>;
	y?: Signalish<number | string | RemoveAttribute>;
}
export interface MetadataSVGAttributes<T> extends SVGAttributes<T> {
}
export interface MPathSVGAttributes<T> extends SVGAttributes<T> {
}
export interface PathSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "marker-start" | "marker-mid" | "marker-end"> {
	d?: Signalish<string | RemoveAttribute>;
	pathLength?: Signalish<number | string | RemoveAttribute>;
}
export interface PatternSVGAttributes<T> extends ContainerElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, FitToViewBoxSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "overflow" | "clip"> {
	height?: Signalish<number | string | RemoveAttribute>;
	href?: Signalish<string | RemoveAttribute>;
	patternContentUnits?: Signalish<SVGUnits | RemoveAttribute>;
	patternTransform?: Signalish<string | RemoveAttribute>;
	patternUnits?: Signalish<SVGUnits | RemoveAttribute>;
	width?: Signalish<number | string | RemoveAttribute>;
	x?: Signalish<number | string | RemoveAttribute>;
	y?: Signalish<number | string | RemoveAttribute>;
}
export interface PolygonSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "marker-start" | "marker-mid" | "marker-end"> {
	points?: Signalish<string | RemoveAttribute>;
}
export interface PolylineSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "marker-start" | "marker-mid" | "marker-end"> {
	points?: Signalish<string | RemoveAttribute>;
}
export interface RadialGradientSVGAttributes<T> extends GradientElementSVGAttributes<T> {
	cx?: Signalish<number | string | RemoveAttribute>;
	cy?: Signalish<number | string | RemoveAttribute>;
	fx?: Signalish<number | string | RemoveAttribute>;
	fy?: Signalish<number | string | RemoveAttribute>;
	r?: Signalish<number | string | RemoveAttribute>;
}
export interface RectSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path"> {
	height?: Signalish<number | string | RemoveAttribute>;
	rx?: Signalish<number | string | RemoveAttribute>;
	ry?: Signalish<number | string | RemoveAttribute>;
	width?: Signalish<number | string | RemoveAttribute>;
	x?: Signalish<number | string | RemoveAttribute>;
	y?: Signalish<number | string | RemoveAttribute>;
}
export interface SetSVGAttributes<T> extends AnimationElementSVGAttributes<T>, StylableSVGAttributes, AnimationTimingSVGAttributes {
}
export interface StopSVGAttributes<T> extends SVGAttributes<T>, StylableSVGAttributes, Pick<PresentationSVGAttributes, "color" | "stop-color" | "stop-opacity"> {
	offset?: Signalish<number | string | RemoveAttribute>;
}
export interface SvgSVGAttributes<T> extends ContainerElementSVGAttributes<T>, NewViewportSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, FitToViewBoxSVGAttributes, ZoomAndPanSVGAttributes, PresentationSVGAttributes, EventHandlersWindow<T> {
	"xmlns:xlink"?: Signalish<string | RemoveAttribute>;
	contentScriptType?: Signalish<string | RemoveAttribute>;
	contentStyleType?: Signalish<string | RemoveAttribute>;
	height?: Signalish<number | string | RemoveAttribute>;
	width?: Signalish<number | string | RemoveAttribute>;
	x?: Signalish<number | string | RemoveAttribute>;
	y?: Signalish<number | string | RemoveAttribute>;
	/** @deprecated */
	baseProfile?: Signalish<string | RemoveAttribute>;
	/** @deprecated */
	version?: Signalish<string | RemoveAttribute>;
}
export interface SwitchSVGAttributes<T> extends ContainerElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "display" | "visibility"> {
}
export interface SymbolSVGAttributes<T> extends ContainerElementSVGAttributes<T>, NewViewportSVGAttributes<T>, ExternalResourceSVGAttributes, StylableSVGAttributes, FitToViewBoxSVGAttributes, Pick<PresentationSVGAttributes, "clip-path"> {
	height?: Signalish<number | string | RemoveAttribute>;
	preserveAspectRatio?: Signalish<SVGPreserveAspectRatio_ | RemoveAttribute>;
	refX?: Signalish<number | string | RemoveAttribute>;
	refY?: Signalish<number | string | RemoveAttribute>;
	viewBox?: Signalish<string | RemoveAttribute>;
	width?: Signalish<number | string | RemoveAttribute>;
	x?: Signalish<number | string | RemoveAttribute>;
	y?: Signalish<number | string | RemoveAttribute>;
}
export interface TextSVGAttributes<T> extends TextContentElementSVGAttributes<T>, GraphicsElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "writing-mode" | "text-rendering"> {
	dx?: Signalish<number | string | RemoveAttribute>;
	dy?: Signalish<number | string | RemoveAttribute>;
	lengthAdjust?: Signalish<"spacing" | "spacingAndGlyphs" | RemoveAttribute>;
	rotate?: Signalish<number | string | RemoveAttribute>;
	textLength?: Signalish<number | string | RemoveAttribute>;
	x?: Signalish<number | string | RemoveAttribute>;
	y?: Signalish<number | string | RemoveAttribute>;
}
export interface TextPathSVGAttributes<T> extends TextContentElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, Pick<PresentationSVGAttributes, "alignment-baseline" | "baseline-shift" | "display" | "visibility"> {
	href?: Signalish<string | RemoveAttribute>;
	method?: Signalish<"align" | "stretch" | RemoveAttribute>;
	spacing?: Signalish<"auto" | "exact" | RemoveAttribute>;
	startOffset?: Signalish<number | string | RemoveAttribute>;
}
export interface TSpanSVGAttributes<T> extends TextContentElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, Pick<PresentationSVGAttributes, "alignment-baseline" | "baseline-shift" | "display" | "visibility"> {
	dx?: Signalish<number | string | RemoveAttribute>;
	dy?: Signalish<number | string | RemoveAttribute>;
	lengthAdjust?: Signalish<"spacing" | "spacingAndGlyphs" | RemoveAttribute>;
	rotate?: Signalish<number | string | RemoveAttribute>;
	textLength?: Signalish<number | string | RemoveAttribute>;
	x?: Signalish<number | string | RemoveAttribute>;
	y?: Signalish<number | string | RemoveAttribute>;
}
/** @see https://developer.mozilla.org/en-US/docs/Web/SVG/Element/use */
export interface UseSVGAttributes<T> extends SVGAttributes<T>, StylableSVGAttributes, ConditionalProcessingSVGAttributes, GraphicsElementSVGAttributes<T>, PresentationSVGAttributes, ExternalResourceSVGAttributes, TransformableSVGAttributes {
	height?: Signalish<number | string | RemoveAttribute>;
	href?: Signalish<string | RemoveAttribute>;
	width?: Signalish<number | string | RemoveAttribute>;
	x?: Signalish<number | string | RemoveAttribute>;
	y?: Signalish<number | string | RemoveAttribute>;
}
export interface ViewSVGAttributes<T> extends SVGAttributes<T>, ExternalResourceSVGAttributes, FitToViewBoxSVGAttributes, ZoomAndPanSVGAttributes {
	viewTarget?: Signalish<string | RemoveAttribute>;
}
// TAGS
/** @type {HTMLElementTagNameMap} */
export interface HTMLElementTags {
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/a
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLAnchorElement
	 */
	a: AnchorHTMLAttributes<HTMLAnchorElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/abbr
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	abbr: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/address
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	address: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/area
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLAreaElement
	 */
	area: AreaHTMLAttributes<HTMLAreaElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/article
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	article: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/aside
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	aside: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/audio
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLAudioElement
	 */
	audio: AudioHTMLAttributes<HTMLAudioElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/b
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	b: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/base
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLBaseElement
	 */
	base: BaseHTMLAttributes<HTMLBaseElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/bdi
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	bdi: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/bdo
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	bdo: BdoHTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/blockquote
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLQuoteElement
	 */
	blockquote: BlockquoteHTMLAttributes<HTMLQuoteElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/body
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLBodyElement
	 */
	body: BodyHTMLAttributes<HTMLBodyElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/br
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLBRElement
	 */
	br: HTMLAttributes<HTMLBRElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/button
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLButtonElement
	 */
	button: ButtonHTMLAttributes<HTMLButtonElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/canvas
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLCanvasElement
	 */
	canvas: CanvasHTMLAttributes<HTMLCanvasElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/caption
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLTableCaptionElement
	 */
	caption: CaptionHTMLAttributes<HTMLTableCaptionElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/cite
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	cite: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/code
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	code: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/col
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLTableColElement
	 */
	col: ColHTMLAttributes<HTMLTableColElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/colgroup
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLTableColElement
	 */
	colgroup: ColgroupHTMLAttributes<HTMLTableColElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/data
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLDataElement
	 */
	data: DataHTMLAttributes<HTMLDataElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/datalist
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLDataListElement
	 */
	datalist: HTMLAttributes<HTMLDataListElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/dd
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	dd: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/del
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLModElement
	 */
	del: ModHTMLAttributes<HTMLModElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/details
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLDetailsElement
	 */
	details: DetailsHtmlAttributes<HTMLDetailsElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/dfn
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	dfn: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/dialog
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLDialogElement
	 */
	dialog: DialogHtmlAttributes<HTMLDialogElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/div
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLDivElement
	 */
	div: HTMLAttributes<HTMLDivElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/dl
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLDListElement
	 */
	dl: HTMLAttributes<HTMLDListElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/dt
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	dt: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/em
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	em: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/embed
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLEmbedElement
	 */
	embed: EmbedHTMLAttributes<HTMLEmbedElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/fieldset
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLFieldSetElement
	 */
	fieldset: FieldsetHTMLAttributes<HTMLFieldSetElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/figcaption
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	figcaption: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/figure
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	figure: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/footer
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	footer: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/form
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLFormElement
	 */
	form: FormHTMLAttributes<HTMLFormElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/h1
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLHeadingElement
	 */
	h1: HTMLAttributes<HTMLHeadingElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/h2
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLHeadingElement
	 */
	h2: HTMLAttributes<HTMLHeadingElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/h3
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLHeadingElement
	 */
	h3: HTMLAttributes<HTMLHeadingElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/h4
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLHeadingElement
	 */
	h4: HTMLAttributes<HTMLHeadingElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/h5
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLHeadingElement
	 */
	h5: HTMLAttributes<HTMLHeadingElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/h6
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLHeadingElement
	 */
	h6: HTMLAttributes<HTMLHeadingElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/head
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLHeadElement
	 */
	head: HTMLAttributes<HTMLHeadElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/header
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	header: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/hgroup
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	hgroup: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/hr
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLHRElement
	 */
	hr: HTMLAttributes<HTMLHRElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/html
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLHtmlElement
	 */
	html: HTMLAttributes<HTMLHtmlElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/i
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	i: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/iframe
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLIFrameElement
	 */
	iframe: IframeHTMLAttributes<HTMLIFrameElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/img
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLImageElement
	 */
	img: ImgHTMLAttributes<HTMLImageElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLInputElement
	 */
	input: InputHTMLAttributes<HTMLInputElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/ins
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLModElement
	 */
	ins: ModHTMLAttributes<HTMLModElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/kbd
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	kbd: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/label
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLLabelElement
	 */
	label: LabelHTMLAttributes<HTMLLabelElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/legend
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLLegendElement
	 */
	legend: HTMLAttributes<HTMLLegendElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/li
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLLIElement
	 */
	li: LiHTMLAttributes<HTMLLIElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/link
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLLinkElement
	 */
	link: LinkHTMLAttributes<HTMLLinkElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/main
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	main: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/map
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLMapElement
	 */
	map: MapHTMLAttributes<HTMLMapElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/mark
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	mark: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/menu
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLMenuElement
	 */
	menu: MenuHTMLAttributes<HTMLMenuElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/meta
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLMetaElement
	 */
	meta: MetaHTMLAttributes<HTMLMetaElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/meter
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLMeterElement
	 */
	meter: MeterHTMLAttributes<HTMLMeterElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/nav
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	nav: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/noscript
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	noscript: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/object
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLObjectElement
	 */
	object: ObjectHTMLAttributes<HTMLObjectElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/ol
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLOListElement
	 */
	ol: OlHTMLAttributes<HTMLOListElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/optgroup
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLOptGroupElement
	 */
	optgroup: OptgroupHTMLAttributes<HTMLOptGroupElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/option
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLOptionElement
	 */
	option: OptionHTMLAttributes<HTMLOptionElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/output
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLOutputElement
	 */
	output: OutputHTMLAttributes<HTMLOutputElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/p
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLParagraphElement
	 */
	p: HTMLAttributes<HTMLParagraphElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/picture
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLPictureElement
	 */
	picture: HTMLAttributes<HTMLPictureElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/pre
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLPreElement
	 */
	pre: HTMLAttributes<HTMLPreElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/progress
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLProgressElement
	 */
	progress: ProgressHTMLAttributes<HTMLProgressElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/q
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLQuoteElement
	 */
	q: QuoteHTMLAttributes<HTMLQuoteElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/rp
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	rp: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/rt
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	rt: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/ruby
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	ruby: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/s
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	s: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/samp
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	samp: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/script
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLScriptElement
	 */
	script: ScriptHTMLAttributes<HTMLScriptElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/search
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	search: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/section
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	section: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/select
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLSelectElement
	 */
	select: SelectHTMLAttributes<HTMLSelectElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/slot
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLSlotElement
	 */
	slot: HTMLSlotElementAttributes<HTMLSlotElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/small
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	small: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/source
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLSourceElement
	 */
	source: SourceHTMLAttributes<HTMLSourceElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/span
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLSpanElement
	 */
	span: HTMLAttributes<HTMLSpanElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/strong
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	strong: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/style
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLStyleElement
	 */
	style: StyleHTMLAttributes<HTMLStyleElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/sub
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	sub: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/summary
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	summary: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/sup
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	sup: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/table
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLTableElement
	 */
	table: HTMLAttributes<HTMLTableElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/tbody
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLTableSectionElement
	 */
	tbody: HTMLAttributes<HTMLTableSectionElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/td
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLTableCellElement
	 */
	td: TdHTMLAttributes<HTMLTableCellElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/template
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLTemplateElement
	 */
	template: TemplateHTMLAttributes<HTMLTemplateElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/textarea
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLTextAreaElement
	 */
	textarea: TextareaHTMLAttributes<HTMLTextAreaElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/tfoot
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLTableSectionElement
	 */
	tfoot: HTMLAttributes<HTMLTableSectionElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/th
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLTableCellElement
	 */
	th: ThHTMLAttributes<HTMLTableCellElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/thead
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLTableSectionElement
	 */
	thead: HTMLAttributes<HTMLTableSectionElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/time
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLTimeElement
	 */
	time: TimeHTMLAttributes<HTMLTimeElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/title
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLTitleElement
	 */
	title: HTMLAttributes<HTMLTitleElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/tr
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLTableRowElement
	 */
	tr: HTMLAttributes<HTMLTableRowElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/track
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLTrackElement
	 */
	track: TrackHTMLAttributes<HTMLTrackElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/u
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	u: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/ul
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLUListElement
	 */
	ul: HTMLAttributes<HTMLUListElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/var
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	var: HTMLAttributes<HTMLElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/video
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLVideoElement
	 */
	video: VideoHTMLAttributes<HTMLVideoElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/HTML/Element/wbr
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement
	 */
	wbr: HTMLAttributes<HTMLElement>;
	/** @url https://www.electronjs.org/docs/latest/api/webview-tag */
	webview: WebViewHTMLAttributes<HTMLElement>;
}
/** @type {SVGElementTagNameMap} */
export interface SVGElementTags {
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/animate
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGAnimateElement
	 */
	animate: AnimateSVGAttributes<SVGAnimateElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/animateMotion
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGAnimateMotionElement
	 */
	animateMotion: AnimateMotionSVGAttributes<SVGAnimateMotionElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/animateTransform
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGAnimateTransformElement
	 */
	animateTransform: AnimateTransformSVGAttributes<SVGAnimateTransformElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/circle
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGCircleElement
	 */
	circle: CircleSVGAttributes<SVGCircleElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/clipPath
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGClipPathElement
	 */
	clipPath: ClipPathSVGAttributes<SVGClipPathElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/defs
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGDefsElement
	 */
	defs: DefsSVGAttributes<SVGDefsElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/desc
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGDescElement
	 */
	desc: DescSVGAttributes<SVGDescElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/ellipse
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGEllipseElement
	 */
	ellipse: EllipseSVGAttributes<SVGEllipseElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feBlend
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEBlendElement
	 */
	feBlend: FeBlendSVGAttributes<SVGFEBlendElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feColorMatrix
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEColorMatrixElement
	 */
	feColorMatrix: FeColorMatrixSVGAttributes<SVGFEColorMatrixElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feComponentTransfer
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEComponentTransferElemen
	 */
	feComponentTransfer: FeComponentTransferSVGAttributes<SVGFEComponentTransferElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feComposite
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFECompositeElement
	 */
	feComposite: FeCompositeSVGAttributes<SVGFECompositeElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feConvolveMatrix
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEConvolveMatrixElement
	 */
	feConvolveMatrix: FeConvolveMatrixSVGAttributes<SVGFEConvolveMatrixElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feDiffuseLighting
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEDiffuseLightingElement
	 */
	feDiffuseLighting: FeDiffuseLightingSVGAttributes<SVGFEDiffuseLightingElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feDisplacementMap
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEDisplacementMapElement
	 */
	feDisplacementMap: FeDisplacementMapSVGAttributes<SVGFEDisplacementMapElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feDistantLight
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEDistantLightElement
	 */
	feDistantLight: FeDistantLightSVGAttributes<SVGFEDistantLightElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feDropShadow
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEDropShadowElement
	 */
	feDropShadow: FeDropShadowSVGAttributes<SVGFEDropShadowElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feFlood
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEFloodElement
	 */
	feFlood: FeFloodSVGAttributes<SVGFEFloodElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feFuncA
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEFuncAElement
	 */
	feFuncA: FeFuncSVGAttributes<SVGFEFuncAElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feFuncB
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEFuncBElement
	 */
	feFuncB: FeFuncSVGAttributes<SVGFEFuncBElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feFuncG
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEFuncGElement
	 */
	feFuncG: FeFuncSVGAttributes<SVGFEFuncGElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feFuncR
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEFuncRElement
	 */
	feFuncR: FeFuncSVGAttributes<SVGFEFuncRElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feGaussianBlur
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEGaussianBlurElement
	 */
	feGaussianBlur: FeGaussianBlurSVGAttributes<SVGFEGaussianBlurElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feImage
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEImageElement
	 */
	feImage: FeImageSVGAttributes<SVGFEImageElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feMerge
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEMergeElement
	 */
	feMerge: FeMergeSVGAttributes<SVGFEMergeElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feMergeNode
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEMergeNodeElement
	 */
	feMergeNode: FeMergeNodeSVGAttributes<SVGFEMergeNodeElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feMorphology
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEMorphologyElement
	 */
	feMorphology: FeMorphologySVGAttributes<SVGFEMorphologyElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feOffset
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEOffsetElement
	 */
	feOffset: FeOffsetSVGAttributes<SVGFEOffsetElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/fePointLight
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFEPointLightElement
	 */
	fePointLight: FePointLightSVGAttributes<SVGFEPointLightElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feSpecularLighting
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFESpecularLightingElement
	 */
	feSpecularLighting: FeSpecularLightingSVGAttributes<SVGFESpecularLightingElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feSpotLight
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFESpotLightElement
	 */
	feSpotLight: FeSpotLightSVGAttributes<SVGFESpotLightElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feTile
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFETileElement
	 */
	feTile: FeTileSVGAttributes<SVGFETileElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feTurbulence
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFETurbulenceElement
	 */
	feTurbulence: FeTurbulanceSVGAttributes<SVGFETurbulenceElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/filter
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGFilterElement
	 */
	filter: FilterSVGAttributes<SVGFilterElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/foreignObject
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGForeignObjectElement
	 */
	foreignObject: ForeignObjectSVGAttributes<SVGForeignObjectElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/g
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGGElement
	 */
	g: GSVGAttributes<SVGGElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/image
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGImageElement
	 */
	image: ImageSVGAttributes<SVGImageElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/line
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGLineElement
	 */
	line: LineSVGAttributes<SVGLineElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/linearGradient
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGLinearGradientElement
	 */
	linearGradient: LinearGradientSVGAttributes<SVGLinearGradientElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/marker
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGMarkerElement
	 */
	marker: MarkerSVGAttributes<SVGMarkerElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/mask
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGMaskElement
	 */
	mask: MaskSVGAttributes<SVGMaskElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/metadata
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGMetadataElement
	 */
	metadata: MetadataSVGAttributes<SVGMetadataElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/mpath
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGMPathElement
	 */
	mpath: MPathSVGAttributes<SVGMPathElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/path
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGPathElement
	 */
	path: PathSVGAttributes<SVGPathElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/pattern
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGPatternElement
	 */
	pattern: PatternSVGAttributes<SVGPatternElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/polygon
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGPolygonElement
	 */
	polygon: PolygonSVGAttributes<SVGPolygonElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/polyline
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGPolylineElement
	 */
	polyline: PolylineSVGAttributes<SVGPolylineElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/radialGradient
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGRadialGradientElement
	 */
	radialGradient: RadialGradientSVGAttributes<SVGRadialGradientElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/rect
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGRectElement
	 */
	rect: RectSVGAttributes<SVGRectElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/set
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGSetElement
	 */
	set: SetSVGAttributes<SVGSetElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/stop
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGStopElement
	 */
	stop: StopSVGAttributes<SVGStopElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/svg
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGSVGElement
	 */
	svg: SvgSVGAttributes<SVGSVGElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/switch
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGSwitchElement
	 */
	switch: SwitchSVGAttributes<SVGSwitchElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/symbol
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGSymbolElement
	 */
	symbol: SymbolSVGAttributes<SVGSymbolElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/text
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGTextElement
	 */
	text: TextSVGAttributes<SVGTextElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/textPath
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGTextPathElement
	 */
	textPath: TextPathSVGAttributes<SVGTextPathElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/tspan
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGTSpanElement
	 */
	tspan: TSpanSVGAttributes<SVGTSpanElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/use
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGUseElement
	 */
	use: UseSVGAttributes<SVGUseElement>;
	/**
	 * @url https://developer.mozilla.org/en-US/docs/Web/SVG/Element/view
	 * @url https://developer.mozilla.org/en-US/docs/Web/API/SVGViewElement
	 */
	view: ViewSVGAttributes<SVGViewElement>;
}
export type IntrinsicElementsCombined = HTMLElementTags & (ConfigureJSXElement["svg"] extends false ? void : SVGElementTags);
export interface CustomElementsHTML {
}
export declare namespace JSX {
	type Element = JSXElement;
	interface ElementAttributesProperty {
		props: unknown;
	}
	interface ElementChildrenAttribute {
		children: {};
	}
	interface IntrinsicAttributes {
	}
	interface IntrinsicClassAttributes<T> {
		ref?: Ref<T>;
	}
	interface IntrinsicElements extends IntrinsicElementsCombined, CustomElementsHTML {
	}
}
export declare function Fragment(attr: {
	children?: ComponentChildren | undefined;
}): any;
export declare function createRef<T = any>(): RefObject<T>;
declare function identity<T>(value: T): T;
export declare function useMemo<T>(factory: () => T): T;
export declare function forwardRef<T = Node, P = {}>(render: (props: P, ref: Ref<T>) => JSXElement): FunctionComponent<P & {
	ref?: Ref<T>;
}>;
export declare function useImperativeHandle<T>(ref: Ref<T>, init: () => T, _deps?: unknown): void;
export declare function createElement(tag: any, attr: any, ...children: any[]): any;
export declare const h: typeof createElement;
export declare function createFactory(tag: string | FunctionComponent<any>): any;
export declare class Component<T = any> {
	static isComponent: boolean;
	constructor(props: T & {
		children?: ComponentChildren;
		ref?: Ref<any>;
	});
	readonly props: T & {
		children?: ComponentChildren;
		ref?: Ref<any>;
	};
	render(): JSXElement | null;
}
export declare function useClassList(initialValue?: ClassNames): BasicClassList;
export declare function useText(initialValue?: string): readonly [
	Text,
	(value: string) => void
];
export declare function jsx<K extends keyof HTMLElementTagNameMap, T extends HTMLElementTagNameMap[K]>(type: K, props?: (HTMLAttributes<T> & {
	children?: ComponentChildren;
	ref?: Ref<T>;
}) | null, key?: string): T;
export declare function jsx<K extends keyof SVGElementTagNameMap, T extends SVGElementTagNameMap[K]>(type: K, props?: (SVGAttributes<T> & {
	children?: ComponentChildren;
	ref?: Ref<T>;
}) | null, key?: string): SVGElement;
export declare function jsx<T extends Element>(type: string, props?: {
	children?: ComponentChildren;
	ref?: Ref<T>;
} & ElementAttributes<T> | null, key?: string): T;
export declare function jsx<P extends {}, T extends Element>(type: ComponentType<P, T>, props?: P & {
	children?: ComponentChildren;
	ref?: Ref<T>;
} | null, key?: string): T;
export declare function jsx<T extends Element>(type: string, props?: {
	children?: ComponentChildren;
} | null, key?: string): T;
export declare function ShadowRootNode({ children, ref, ...attr }: ShadowRootInit & {
	ref?: Ref<ShadowRoot>;
	children?: ComponentChildren;
}): any;
export declare const SVGNamespace = "http://www.w3.org/2000/svg";

export {
	Fragment as StrictMode,
	createRef as useRef,
	identity as memo,
	identity as useCallback,
	jsx as jsxs,
};

export {};
