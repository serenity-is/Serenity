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
export type SignalOrValue<T> = T | SignalLike<T>;
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
	className?: SignalOrValue<string | ClassNames | RemoveAttribute>;
	tabIndex?: SignalOrValue<number | string | RemoveAttribute>;
	onClickCapture?: EventHandlerUnion<T, MouseEvent> | undefined;
	onDblClickCapture?: EventHandlerUnion<T, MouseEvent> | undefined;
	onDoubleClick?: EventHandlerUnion<T, MouseEvent> | undefined;
	onDoubleClickCapture?: EventHandlerUnion<T, MouseEvent> | undefined;
}
export interface HTMLAttributes<T> {
	contentEditable?: SignalOrValue<EnumeratedPseudoBoolean | EnumeratedAcceptsEmpty | "plaintext-only" | "inherit" | RemoveAttribute>;
	dataset?: {
		[key: string]: string;
	} | undefined;
	spellCheck?: SignalOrValue<EnumeratedPseudoBoolean | EnumeratedAcceptsEmpty | RemoveAttribute>;
}
export interface SVGAttributes<T> {
	tabIndex?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface AnchorHTMLAttributes<T> {
	/** @deprecated use referrerpolicy */
	referrerPolicy?: SignalOrValue<HTMLReferrerPolicy | RemoveAttribute>;
}
export interface ButtonHTMLAttributes<T> {
	autoFocus?: SignalOrValue<boolean | RemoveAttribute>;
	formNoValidate?: SignalOrValue<boolean | RemoveAttribute>;
}
export interface InputHTMLAttributes<T> {
	maxLength?: SignalOrValue<string | number | RemoveAttribute>;
	minLength?: SignalOrValue<string | number | RemoveAttribute>;
	readOnly?: SignalOrValue<boolean | RemoveAttribute>;
}
export interface LabelHTMLAttributes<T> {
	htmlFor?: SignalOrValue<string | RemoveAttribute>;
}
export interface TdHTMLAttributes<T> {
	colSpan?: SignalOrValue<number | string | RemoveAttribute>;
	rowSpan?: SignalOrValue<number | string | RemoveAttribute>;
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
	"aria-activedescendant"?: SignalOrValue<string | RemoveAttribute>;
	/**
	 * Indicates whether assistive technologies will present all, or only parts of, the changed
	 * region based on the change notifications defined by the aria-relevant attribute.
	 */
	"aria-atomic"?: SignalOrValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Similar to the global aria-label. Defines a string value that labels the current element,
	 * which is intended to be converted into Braille.
	 *
	 * @see aria-label.
	 */
	"aria-braillelabel"?: SignalOrValue<string | RemoveAttribute>;
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
	"aria-brailleroledescription"?: SignalOrValue<string | RemoveAttribute>;
	/**
	 * Indicates whether inputting text could trigger display of one or more predictions of the
	 * user's intended value for an input and specifies how predictions would be presented if they
	 * are made.
	 */
	"aria-autocomplete"?: SignalOrValue<"none" | "inline" | "list" | "both" | RemoveAttribute>;
	/**
	 * Indicates an element is being modified and that assistive technologies MAY want to wait until
	 * the modifications are complete before exposing them to the user.
	 */
	"aria-busy"?: SignalOrValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Indicates the current "checked" state of checkboxes, radio buttons, and other widgets.
	 *
	 * @see aria-pressed @see aria-selected.
	 */
	"aria-checked"?: SignalOrValue<EnumeratedPseudoBoolean | "mixed" | RemoveAttribute>;
	/**
	 * Defines the total number of columns in a table, grid, or treegrid.
	 *
	 * @see aria-colindex.
	 */
	"aria-colcount"?: SignalOrValue<number | string | RemoveAttribute>;
	/**
	 * Defines an element's column index or position with respect to the total number of columns
	 * within a table, grid, or treegrid.
	 *
	 * @see aria-colcount @see aria-colspan.
	 */
	"aria-colindex"?: SignalOrValue<number | string | RemoveAttribute>;
	/** Defines a human-readable text alternative of the numeric aria-colindex. */
	"aria-colindextext"?: SignalOrValue<number | string | RemoveAttribute>;
	/**
	 * Defines the number of columns spanned by a cell or gridcell within a table, grid, or
	 * treegrid.
	 *
	 * @see aria-colindex @see aria-rowspan.
	 */
	"aria-colspan"?: SignalOrValue<number | string | RemoveAttribute>;
	/**
	 * Identifies the element (or elements) whose contents or presence are controlled by the current
	 * element.
	 *
	 * @see aria-owns.
	 */
	"aria-controls"?: SignalOrValue<string | RemoveAttribute>;
	/**
	 * Indicates the element that represents the current item within a container or set of related
	 * elements.
	 */
	"aria-current"?: SignalOrValue<EnumeratedPseudoBoolean | "page" | "step" | "location" | "date" | "time" | RemoveAttribute>;
	/**
	 * Identifies the element (or elements) that describes the object.
	 *
	 * @see aria-labelledby
	 */
	"aria-describedby"?: SignalOrValue<string | RemoveAttribute>;
	/**
	 * Defines a string value that describes or annotates the current element.
	 *
	 * @see aria-describedby
	 */
	"aria-description"?: SignalOrValue<string | RemoveAttribute>;
	/**
	 * Identifies the element that provides a detailed, extended description for the object.
	 *
	 * @see aria-describedby.
	 */
	"aria-details"?: SignalOrValue<string | RemoveAttribute>;
	/**
	 * Indicates that the element is perceivable but disabled, so it is not editable or otherwise
	 * operable.
	 *
	 * @see aria-hidden @see aria-readonly.
	 */
	"aria-disabled"?: SignalOrValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Indicates what functions can be performed when a dragged object is released on the drop
	 * target.
	 *
	 * @deprecated In ARIA 1.1
	 */
	"aria-dropeffect"?: SignalOrValue<"none" | "copy" | "execute" | "link" | "move" | "popup" | RemoveAttribute>;
	/**
	 * Identifies the element that provides an error message for the object.
	 *
	 * @see aria-invalid @see aria-describedby.
	 */
	"aria-errormessage"?: SignalOrValue<string | RemoveAttribute>;
	/**
	 * Indicates whether the element, or another grouping element it controls, is currently expanded
	 * or collapsed.
	 */
	"aria-expanded"?: SignalOrValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Identifies the next element (or elements) in an alternate reading order of content which, at
	 * the user's discretion, allows assistive technology to override the general default of reading
	 * in document source order.
	 */
	"aria-flowto"?: SignalOrValue<string | RemoveAttribute>;
	/**
	 * Indicates an element's "grabbed" state in a drag-and-drop operation.
	 *
	 * @deprecated In ARIA 1.1
	 */
	"aria-grabbed"?: SignalOrValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Indicates the availability and type of interactive popup element, such as menu or dialog,
	 * that can be triggered by an element.
	 */
	"aria-haspopup"?: SignalOrValue<EnumeratedPseudoBoolean | "menu" | "listbox" | "tree" | "grid" | "dialog" | RemoveAttribute>;
	/**
	 * Indicates whether the element is exposed to an accessibility API.
	 *
	 * @see aria-disabled.
	 */
	"aria-hidden"?: SignalOrValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Indicates the entered value does not conform to the format expected by the application.
	 *
	 * @see aria-errormessage.
	 */
	"aria-invalid"?: SignalOrValue<EnumeratedPseudoBoolean | "grammar" | "spelling" | RemoveAttribute>;
	/**
	 * Indicates keyboard shortcuts that an author has implemented to activate or give focus to an
	 * element.
	 */
	"aria-keyshortcuts"?: SignalOrValue<string | RemoveAttribute>;
	/**
	 * Defines a string value that labels the current element.
	 *
	 * @see aria-labelledby.
	 */
	"aria-label"?: SignalOrValue<string | RemoveAttribute>;
	/**
	 * Identifies the element (or elements) that labels the current element.
	 *
	 * @see aria-describedby.
	 */
	"aria-labelledby"?: SignalOrValue<string | RemoveAttribute>;
	/** Defines the hierarchical level of an element within a structure. */
	"aria-level"?: SignalOrValue<number | string | RemoveAttribute>;
	/**
	 * Indicates that an element will be updated, and describes the types of updates the user
	 * agents, assistive technologies, and user can expect from the live region.
	 */
	"aria-live"?: SignalOrValue<"off" | "assertive" | "polite" | RemoveAttribute>;
	/** Indicates whether an element is modal when displayed. */
	"aria-modal"?: SignalOrValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/** Indicates whether a text box accepts multiple lines of input or only a single line. */
	"aria-multiline"?: SignalOrValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Indicates that the user may select more than one item from the current selectable
	 * descendants.
	 */
	"aria-multiselectable"?: SignalOrValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/** Indicates whether the element's orientation is horizontal, vertical, or unknown/ambiguous. */
	"aria-orientation"?: SignalOrValue<"horizontal" | "vertical" | RemoveAttribute>;
	/**
	 * Identifies an element (or elements) in order to define a visual, functional, or contextual
	 * parent/child relationship between DOM elements where the DOM hierarchy cannot be used to
	 * represent the relationship.
	 *
	 * @see aria-controls.
	 */
	"aria-owns"?: SignalOrValue<string | RemoveAttribute>;
	/**
	 * Defines a short hint (a word or short phrase) intended to aid the user with data entry when
	 * the control has no value. A hint could be a sample value or a brief description of the
	 * expected format.
	 */
	"aria-placeholder"?: SignalOrValue<string | RemoveAttribute>;
	/**
	 * Defines an element's number or position in the current set of listitems or treeitems. Not
	 * required if all elements in the set are present in the DOM.
	 *
	 * @see aria-setsize.
	 */
	"aria-posinset"?: SignalOrValue<number | string | RemoveAttribute>;
	/**
	 * Indicates the current "pressed" state of toggle buttons.
	 *
	 * @see aria-checked @see aria-selected.
	 */
	"aria-pressed"?: SignalOrValue<EnumeratedPseudoBoolean | "mixed" | RemoveAttribute>;
	/**
	 * Indicates that the element is not editable, but is otherwise operable.
	 *
	 * @see aria-disabled.
	 */
	"aria-readonly"?: SignalOrValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Indicates what notifications the user agent will trigger when the accessibility tree within a
	 * live region is modified.
	 *
	 * @see aria-atomic.
	 */
	"aria-relevant"?: SignalOrValue<"additions" | "additions removals" | "additions text" | "all" | "removals" | "removals additions" | "removals text" | "text" | "text additions" | "text removals" | RemoveAttribute>;
	/** Indicates that user input is required on the element before a form may be submitted. */
	"aria-required"?: SignalOrValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/** Defines a human-readable, author-localized description for the role of an element. */
	"aria-roledescription"?: SignalOrValue<string | RemoveAttribute>;
	/**
	 * Defines the total number of rows in a table, grid, or treegrid.
	 *
	 * @see aria-rowindex.
	 */
	"aria-rowcount"?: SignalOrValue<number | string | RemoveAttribute>;
	/**
	 * Defines an element's row index or position with respect to the total number of rows within a
	 * table, grid, or treegrid.
	 *
	 * @see aria-rowcount @see aria-rowspan.
	 */
	"aria-rowindex"?: SignalOrValue<number | string | RemoveAttribute>;
	/** Defines a human-readable text alternative of aria-rowindex. */
	"aria-rowindextext"?: SignalOrValue<number | string | RemoveAttribute>;
	/**
	 * Defines the number of rows spanned by a cell or gridcell within a table, grid, or treegrid.
	 *
	 * @see aria-rowindex @see aria-colspan.
	 */
	"aria-rowspan"?: SignalOrValue<number | string | RemoveAttribute>;
	/**
	 * Indicates the current "selected" state of various widgets.
	 *
	 * @see aria-checked @see aria-pressed.
	 */
	"aria-selected"?: SignalOrValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Defines the number of items in the current set of listitems or treeitems. Not required if all
	 * elements in the set are present in the DOM.
	 *
	 * @see aria-posinset.
	 */
	"aria-setsize"?: SignalOrValue<number | string | RemoveAttribute>;
	/** Indicates if items in a table or grid are sorted in ascending or descending order. */
	"aria-sort"?: SignalOrValue<"none" | "ascending" | "descending" | "other" | RemoveAttribute>;
	/** Defines the maximum allowed value for a range widget. */
	"aria-valuemax"?: SignalOrValue<number | string | RemoveAttribute>;
	/** Defines the minimum allowed value for a range widget. */
	"aria-valuemin"?: SignalOrValue<number | string | RemoveAttribute>;
	/**
	 * Defines the current value for a range widget.
	 *
	 * @see aria-valuetext.
	 */
	"aria-valuenow"?: SignalOrValue<number | string | RemoveAttribute>;
	/** Defines the human readable text alternative of aria-valuenow for a range widget. */
	"aria-valuetext"?: SignalOrValue<string | RemoveAttribute>;
	role?: SignalOrValue<"alert" | "alertdialog" | "application" | "article" | "banner" | "button" | "cell" | "checkbox" | "columnheader" | "combobox" | "complementary" | "contentinfo" | "definition" | "dialog" | "directory" | "document" | "feed" | "figure" | "form" | "grid" | "gridcell" | "group" | "heading" | "img" | "link" | "list" | "listbox" | "listitem" | "log" | "main" | "marquee" | "math" | "menu" | "menubar" | "menuitem" | "menuitemcheckbox" | "menuitemradio" | "meter" | "navigation" | "none" | "note" | "option" | "presentation" | "progressbar" | "radio" | "radiogroup" | "region" | "row" | "rowgroup" | "rowheader" | "scrollbar" | "search" | "searchbox" | "separator" | "slider" | "spinbutton" | "status" | "switch" | "tab" | "table" | "tablist" | "tabpanel" | "term" | "textbox" | "timer" | "toolbar" | "tooltip" | "tree" | "treegrid" | "treeitem" | RemoveAttribute>;
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
	innerHTML?: SignalOrValue<string>;
	textContent?: SignalOrValue<string | number>;
	// attributes
	autofocus?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	class?: SignalOrValue<string | ClassNames | RemoveAttribute>;
	elementtiming?: SignalOrValue<string | RemoveAttribute>;
	id?: SignalOrValue<string | RemoveAttribute>;
	nonce?: SignalOrValue<string | RemoveAttribute>;
	part?: SignalOrValue<string | RemoveAttribute>;
	slot?: SignalOrValue<string | RemoveAttribute>;
	style?: SignalOrValue<CSSProperties | string | RemoveAttribute>;
	tabindex?: SignalOrValue<number | string | RemoveAttribute>;
}
/** Global `SVGElement` interface keys only. */
export interface SVGAttributes<T> extends ElementAttributes<T> {
	id?: SignalOrValue<string | RemoveAttribute>;
	lang?: SignalOrValue<string | RemoveAttribute>;
	tabindex?: SignalOrValue<number | string | RemoveAttribute>;
	xmlns?: SignalOrValue<string | RemoveAttribute>;
}
/** Global `HTMLElement` interface keys only. */
export interface HTMLAttributes<T> extends ElementAttributes<T> {
	// properties
	innerText?: SignalOrValue<string | number>;
	// attributes
	accesskey?: SignalOrValue<string | RemoveAttribute>;
	autocapitalize?: SignalOrValue<HTMLAutocapitalize | RemoveAttribute>;
	autocorrect?: SignalOrValue<"on" | "off" | RemoveAttribute>;
	contenteditable?: SignalOrValue<EnumeratedPseudoBoolean | EnumeratedAcceptsEmpty | "plaintext-only" | "inherit" | RemoveAttribute>;
	dir?: SignalOrValue<HTMLDir | RemoveAttribute>;
	draggable?: SignalOrValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	enterkeyhint?: SignalOrValue<"enter" | "done" | "go" | "next" | "previous" | "search" | "send" | RemoveAttribute>;
	exportparts?: SignalOrValue<string | RemoveAttribute>;
	hidden?: SignalOrValue<EnumeratedAcceptsEmpty | "hidden" | "until-found" | RemoveAttribute>;
	inert?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	inputmode?: SignalOrValue<"decimal" | "email" | "none" | "numeric" | "search" | "tel" | "text" | "url" | RemoveAttribute>;
	is?: SignalOrValue<string | RemoveAttribute>;
	lang?: SignalOrValue<string | RemoveAttribute>;
	popover?: SignalOrValue<EnumeratedAcceptsEmpty | "manual" | "auto" | RemoveAttribute>;
	spellcheck?: SignalOrValue<EnumeratedPseudoBoolean | EnumeratedAcceptsEmpty | RemoveAttribute>;
	title?: SignalOrValue<string | RemoveAttribute>;
	translate?: SignalOrValue<"yes" | "no" | RemoveAttribute>;
	/** @experimental */
	virtualkeyboardpolicy?: SignalOrValue<EnumeratedAcceptsEmpty | "auto" | "manual" | RemoveAttribute>;
	/** @experimental */
	writingsuggestions?: SignalOrValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	// Microdata
	itemid?: SignalOrValue<string | RemoveAttribute>;
	itemprop?: SignalOrValue<string | RemoveAttribute>;
	itemref?: SignalOrValue<string | RemoveAttribute>;
	itemscope?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	itemtype?: SignalOrValue<string | RemoveAttribute>;
	// RDFa Attributes
	about?: SignalOrValue<string | RemoveAttribute>;
	datatype?: SignalOrValue<string | RemoveAttribute>;
	inlist?: SignalOrValue<any | RemoveAttribute>;
	prefix?: SignalOrValue<string | RemoveAttribute>;
	property?: SignalOrValue<string | RemoveAttribute>;
	resource?: SignalOrValue<string | RemoveAttribute>;
	typeof?: SignalOrValue<string | RemoveAttribute>;
	vocab?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	contextmenu?: SignalOrValue<string | RemoveAttribute>;
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
	download?: SignalOrValue<string | true | RemoveAttribute>;
	href?: SignalOrValue<string | RemoveAttribute>;
	hreflang?: SignalOrValue<string | RemoveAttribute>;
	ping?: SignalOrValue<string | RemoveAttribute>;
	referrerpolicy?: SignalOrValue<HTMLReferrerPolicy | RemoveAttribute>;
	rel?: SignalOrValue<string | RemoveAttribute>;
	target?: SignalOrValue<"_self" | "_blank" | "_parent" | "_top" | (string & {}) | RemoveAttribute>;
	type?: SignalOrValue<string | RemoveAttribute>;
	/** @experimental */
	attributionsrc?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	charset?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	coords?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	name?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	rev?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	shape?: SignalOrValue<"rect" | "circle" | "poly" | "default" | RemoveAttribute>;
}
export interface AudioHTMLAttributes<T> extends MediaHTMLAttributes<T> {
}
export interface AreaHTMLAttributes<T> extends HTMLAttributes<T> {
	alt?: SignalOrValue<string | RemoveAttribute>;
	coords?: SignalOrValue<string | RemoveAttribute>;
	download?: SignalOrValue<string | RemoveAttribute>;
	href?: SignalOrValue<string | RemoveAttribute>;
	ping?: SignalOrValue<string | RemoveAttribute>;
	referrerpolicy?: SignalOrValue<HTMLReferrerPolicy | RemoveAttribute>;
	rel?: SignalOrValue<string | RemoveAttribute>;
	shape?: SignalOrValue<"rect" | "circle" | "poly" | "default" | RemoveAttribute>;
	target?: SignalOrValue<"_self" | "_blank" | "_parent" | "_top" | (string & {}) | RemoveAttribute>;
	/** @experimental */
	attributionsrc?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	nohref?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
}
export interface BaseHTMLAttributes<T> extends HTMLAttributes<T> {
	href?: SignalOrValue<string | RemoveAttribute>;
	target?: SignalOrValue<"_self" | "_blank" | "_parent" | "_top" | (string & {}) | RemoveAttribute>;
}
export interface BdoHTMLAttributes<T> extends HTMLAttributes<T> {
	dir?: SignalOrValue<"ltr" | "rtl" | RemoveAttribute>;
}
export interface BlockquoteHTMLAttributes<T> extends HTMLAttributes<T> {
	cite?: SignalOrValue<string | RemoveAttribute>;
}
export interface BodyHTMLAttributes<T> extends HTMLAttributes<T>, EventHandlersWindow<T> {
}
export interface ButtonHTMLAttributes<T> extends HTMLAttributes<T> {
	disabled?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	form?: SignalOrValue<string | RemoveAttribute>;
	formaction?: SignalOrValue<string | SerializableAttributeValue | RemoveAttribute>;
	formenctype?: SignalOrValue<HTMLFormEncType | RemoveAttribute>;
	formmethod?: SignalOrValue<HTMLFormMethod | RemoveAttribute>;
	formnovalidate?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	formtarget?: SignalOrValue<"_self" | "_blank" | "_parent" | "_top" | (string & {}) | RemoveAttribute>;
	name?: SignalOrValue<string | RemoveAttribute>;
	popovertarget?: SignalOrValue<string | RemoveAttribute>;
	popovertargetaction?: SignalOrValue<"hide" | "show" | "toggle" | RemoveAttribute>;
	type?: SignalOrValue<"submit" | "reset" | "button" | "menu" | RemoveAttribute>;
	value?: SignalOrValue<string | RemoveAttribute>;
	/** @experimental */
	command?: SignalOrValue<"show-modal" | "close" | "show-popover" | "hide-popover" | "toggle-popover" | (string & {}) | RemoveAttribute>;
	/** @experimental */
	commandfor?: SignalOrValue<string | RemoveAttribute>;
}
export interface CanvasHTMLAttributes<T> extends HTMLAttributes<T> {
	height?: SignalOrValue<number | string | RemoveAttribute>;
	width?: SignalOrValue<number | string | RemoveAttribute>;
	/**
	 * @deprecated
	 * @non-standard
	 */
	"moz-opaque"?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
}
export interface CaptionHTMLAttributes<T> extends HTMLAttributes<T> {
	/** @deprecated */
	align?: SignalOrValue<"left" | "center" | "right" | RemoveAttribute>;
}
export interface ColHTMLAttributes<T> extends HTMLAttributes<T> {
	span?: SignalOrValue<number | string | RemoveAttribute>;
	/** @deprecated */
	align?: SignalOrValue<"left" | "center" | "right" | "justify" | "char" | RemoveAttribute>;
	/** @deprecated */
	bgcolor?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	char?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	charoff?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	valign?: SignalOrValue<"baseline" | "bottom" | "middle" | "top" | RemoveAttribute>;
	/** @deprecated */
	width?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface ColgroupHTMLAttributes<T> extends HTMLAttributes<T> {
	span?: SignalOrValue<number | string | RemoveAttribute>;
	/** @deprecated */
	align?: SignalOrValue<"left" | "center" | "right" | "justify" | "char" | RemoveAttribute>;
	/** @deprecated */
	bgcolor?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	char?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	charoff?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	valign?: SignalOrValue<"baseline" | "bottom" | "middle" | "top" | RemoveAttribute>;
	/** @deprecated */
	width?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface DataHTMLAttributes<T> extends HTMLAttributes<T> {
	value?: SignalOrValue<string | string[] | number | RemoveAttribute>;
}
export interface DetailsHtmlAttributes<T> extends HTMLAttributes<T> {
	name?: SignalOrValue<string | RemoveAttribute>;
	open?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
}
export interface DialogHtmlAttributes<T> extends HTMLAttributes<T> {
	open?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	/**
	 * Do not add the `tabindex` property to the `<dialog>` element as it is not interactive and
	 * does not receive focus. The dialog's contents, including the close button contained in the
	 * dialog, can receive focus and be interactive.
	 *
	 * @see https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/dialog#usage_notes
	 */
	tabindex?: never;
	/** @experimental */
	closedby?: SignalOrValue<"any" | "closerequest" | "none" | RemoveAttribute>;
}
export interface EmbedHTMLAttributes<T> extends HTMLAttributes<T> {
	height?: SignalOrValue<number | string | RemoveAttribute>;
	src?: SignalOrValue<string | RemoveAttribute>;
	type?: SignalOrValue<string | RemoveAttribute>;
	width?: SignalOrValue<number | string | RemoveAttribute>;
	/** @deprecated */
	align?: SignalOrValue<"left" | "right" | "justify" | "center" | RemoveAttribute>;
	/** @deprecated */
	name?: SignalOrValue<string | RemoveAttribute>;
}
export interface FieldsetHTMLAttributes<T> extends HTMLAttributes<T> {
	disabled?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	form?: SignalOrValue<string | RemoveAttribute>;
	name?: SignalOrValue<string | RemoveAttribute>;
}
export interface FormHTMLAttributes<T> extends HTMLAttributes<T> {
	"accept-charset"?: SignalOrValue<string | RemoveAttribute>;
	action?: SignalOrValue<string | SerializableAttributeValue | RemoveAttribute>;
	autocomplete?: SignalOrValue<"on" | "off" | RemoveAttribute>;
	encoding?: SignalOrValue<HTMLFormEncType | RemoveAttribute>;
	enctype?: SignalOrValue<HTMLFormEncType | RemoveAttribute>;
	method?: SignalOrValue<HTMLFormMethod | RemoveAttribute>;
	name?: SignalOrValue<string | RemoveAttribute>;
	novalidate?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	rel?: SignalOrValue<string | RemoveAttribute>;
	target?: SignalOrValue<"_self" | "_blank" | "_parent" | "_top" | (string & {}) | RemoveAttribute>;
	/** @deprecated */
	accept?: SignalOrValue<string | RemoveAttribute>;
}
export interface IframeHTMLAttributes<T> extends HTMLAttributes<T> {
	allow?: SignalOrValue<string | RemoveAttribute>;
	allowfullscreen?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	height?: SignalOrValue<number | string | RemoveAttribute>;
	loading?: SignalOrValue<"eager" | "lazy" | RemoveAttribute>;
	name?: SignalOrValue<string | RemoveAttribute>;
	referrerpolicy?: SignalOrValue<HTMLReferrerPolicy | RemoveAttribute>;
	sandbox?: SignalOrValue<HTMLIframeSandbox | string | RemoveAttribute>;
	src?: SignalOrValue<string | RemoveAttribute>;
	srcdoc?: SignalOrValue<string | RemoveAttribute>;
	width?: SignalOrValue<number | string | RemoveAttribute>;
	/** @experimental */
	adauctionheaders?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	/**
	 * @non-standard
	 * @experimental
	 */
	browsingtopics?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	/** @experimental */
	credentialless?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	/** @experimental */
	csp?: SignalOrValue<string | RemoveAttribute>;
	/** @experimental */
	privatetoken?: SignalOrValue<string | RemoveAttribute>;
	/** @experimental */
	sharedstoragewritable?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	align?: SignalOrValue<string | RemoveAttribute>;
	/**
	 * @deprecated
	 * @non-standard
	 */
	allowpaymentrequest?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	allowtransparency?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	frameborder?: SignalOrValue<number | string | RemoveAttribute>;
	/** @deprecated */
	longdesc?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	marginheight?: SignalOrValue<number | string | RemoveAttribute>;
	/** @deprecated */
	marginwidth?: SignalOrValue<number | string | RemoveAttribute>;
	/** @deprecated */
	scrolling?: SignalOrValue<"yes" | "no" | "auto" | RemoveAttribute>;
	/** @deprecated */
	seamless?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
}
export interface ImgHTMLAttributes<T> extends HTMLAttributes<T> {
	alt?: SignalOrValue<string | RemoveAttribute>;
	browsingtopics?: SignalOrValue<string | RemoveAttribute>;
	crossorigin?: SignalOrValue<HTMLCrossorigin | RemoveAttribute>;
	decoding?: SignalOrValue<"sync" | "async" | "auto" | RemoveAttribute>;
	fetchpriority?: SignalOrValue<"high" | "low" | "auto" | RemoveAttribute>;
	height?: SignalOrValue<number | string | RemoveAttribute>;
	ismap?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	loading?: SignalOrValue<"eager" | "lazy" | RemoveAttribute>;
	referrerpolicy?: SignalOrValue<HTMLReferrerPolicy | RemoveAttribute>;
	sizes?: SignalOrValue<string | RemoveAttribute>;
	src?: SignalOrValue<string | RemoveAttribute>;
	srcset?: SignalOrValue<string | RemoveAttribute>;
	usemap?: SignalOrValue<string | RemoveAttribute>;
	width?: SignalOrValue<number | string | RemoveAttribute>;
	/** @experimental */
	attributionsrc?: SignalOrValue<string | RemoveAttribute>;
	/** @experimental */
	sharedstoragewritable?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	align?: SignalOrValue<"top" | "middle" | "bottom" | "left" | "right" | RemoveAttribute>;
	/** @deprecated */
	border?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	hspace?: SignalOrValue<number | string | RemoveAttribute>;
	/** @deprecated */
	intrinsicsize?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	longdesc?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	lowsrc?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	name?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	vspace?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface InputHTMLAttributes<T> extends HTMLAttributes<T> {
	accept?: SignalOrValue<string | RemoveAttribute>;
	alpha?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	alt?: SignalOrValue<string | RemoveAttribute>;
	autocomplete?: SignalOrValue<HTMLAutocomplete | RemoveAttribute>;
	capture?: SignalOrValue<"user" | "environment" | RemoveAttribute>;
	checked?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	colorspace?: SignalOrValue<string | RemoveAttribute>;
	dirname?: SignalOrValue<string | RemoveAttribute>;
	disabled?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	form?: SignalOrValue<string | RemoveAttribute>;
	formaction?: SignalOrValue<string | SerializableAttributeValue | RemoveAttribute>;
	formenctype?: SignalOrValue<HTMLFormEncType | RemoveAttribute>;
	formmethod?: SignalOrValue<HTMLFormMethod | RemoveAttribute>;
	formnovalidate?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	formtarget?: SignalOrValue<string | RemoveAttribute>;
	height?: SignalOrValue<number | string | RemoveAttribute>;
	list?: SignalOrValue<string | RemoveAttribute>;
	max?: SignalOrValue<number | string | RemoveAttribute>;
	maxlength?: SignalOrValue<number | string | RemoveAttribute>;
	min?: SignalOrValue<number | string | RemoveAttribute>;
	minlength?: SignalOrValue<number | string | RemoveAttribute>;
	multiple?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	name?: SignalOrValue<string | RemoveAttribute>;
	pattern?: SignalOrValue<string | RemoveAttribute>;
	placeholder?: SignalOrValue<string | RemoveAttribute>;
	popovertarget?: SignalOrValue<string | RemoveAttribute>;
	popovertargetaction?: SignalOrValue<"hide" | "show" | "toggle" | RemoveAttribute>;
	readonly?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	required?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	// https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/search#results
	results?: SignalOrValue<number | RemoveAttribute>;
	size?: SignalOrValue<number | string | RemoveAttribute>;
	src?: SignalOrValue<string | RemoveAttribute>;
	step?: SignalOrValue<number | string | RemoveAttribute>;
	type?: SignalOrValue<"button" | "checkbox" | "color" | "date" | "datetime-local" | "email" | "file" | "hidden" | "image" | "month" | "number" | "password" | "radio" | "range" | "reset" | "search" | "submit" | "tel" | "text" | "time" | "url" | "week" | (string & {}) | RemoveAttribute>;
	value?: SignalOrValue<string | string[] | number | RemoveAttribute>;
	width?: SignalOrValue<number | string | RemoveAttribute>;
	/** @non-standard */
	incremental?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	align?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	usemap?: SignalOrValue<string | RemoveAttribute>;
}
export interface ModHTMLAttributes<T> extends HTMLAttributes<T> {
	cite?: SignalOrValue<string | RemoveAttribute>;
	datetime?: SignalOrValue<string | RemoveAttribute>;
}
export interface LabelHTMLAttributes<T> extends HTMLAttributes<T> {
	for?: SignalOrValue<string | RemoveAttribute>;
}
export interface LiHTMLAttributes<T> extends HTMLAttributes<T> {
	value?: SignalOrValue<number | string | RemoveAttribute>;
	/** @deprecated */
	type?: SignalOrValue<"1" | "a" | "A" | "i" | "I" | RemoveAttribute>;
}
export interface LinkHTMLAttributes<T> extends HTMLAttributes<T> {
	as?: SignalOrValue<HTMLLinkAs | RemoveAttribute>;
	blocking?: SignalOrValue<"render" | RemoveAttribute>;
	color?: SignalOrValue<string | RemoveAttribute>;
	crossorigin?: SignalOrValue<HTMLCrossorigin | RemoveAttribute>;
	disabled?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	fetchpriority?: SignalOrValue<"high" | "low" | "auto" | RemoveAttribute>;
	href?: SignalOrValue<string | RemoveAttribute>;
	hreflang?: SignalOrValue<string | RemoveAttribute>;
	imagesizes?: SignalOrValue<string | RemoveAttribute>;
	imagesrcset?: SignalOrValue<string | RemoveAttribute>;
	integrity?: SignalOrValue<string | RemoveAttribute>;
	media?: SignalOrValue<string | RemoveAttribute>;
	referrerpolicy?: SignalOrValue<HTMLReferrerPolicy | RemoveAttribute>;
	rel?: SignalOrValue<string | RemoveAttribute>;
	sizes?: SignalOrValue<string | RemoveAttribute>;
	type?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	charset?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	rev?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	target?: SignalOrValue<string | RemoveAttribute>;
}
export interface MapHTMLAttributes<T> extends HTMLAttributes<T> {
	name?: SignalOrValue<string | RemoveAttribute>;
}
export interface MediaHTMLAttributes<T> extends HTMLAttributes<T> {
	autoplay?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	controls?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	controlslist?: SignalOrValue<"nodownload" | "nofullscreen" | "noplaybackrate" | "noremoteplayback" | (string & {}) | RemoveAttribute>;
	crossorigin?: SignalOrValue<HTMLCrossorigin | RemoveAttribute>;
	disableremoteplayback?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	loop?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	muted?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	preload?: SignalOrValue<"none" | "metadata" | "auto" | EnumeratedAcceptsEmpty | RemoveAttribute>;
	src?: SignalOrValue<string | RemoveAttribute>;
	onEncrypted?: EventHandlerUnion<T, MediaEncryptedEvent> | undefined;
	// "on:encrypted"?: EventHandlerWithOptionsUnion<T, MediaEncryptedEvent> | undefined;
	onWaitingForKey?: EventHandlerUnion<T, Event> | undefined;
	// "on:waitingforkey"?: EventHandlerWithOptionsUnion<T, Event> | undefined;
	/** @deprecated */
	mediagroup?: SignalOrValue<string | RemoveAttribute>;
}
export interface MenuHTMLAttributes<T> extends HTMLAttributes<T> {
	/** @deprecated */
	compact?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	label?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	type?: SignalOrValue<"context" | "toolbar" | RemoveAttribute>;
}
export interface MetaHTMLAttributes<T> extends HTMLAttributes<T> {
	"http-equiv"?: SignalOrValue<"content-security-policy" | "content-type" | "default-style" | "x-ua-compatible" | "refresh" | RemoveAttribute>;
	charset?: SignalOrValue<string | RemoveAttribute>;
	content?: SignalOrValue<string | RemoveAttribute>;
	media?: SignalOrValue<string | RemoveAttribute>;
	name?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	scheme?: SignalOrValue<string | RemoveAttribute>;
}
export interface MeterHTMLAttributes<T> extends HTMLAttributes<T> {
	form?: SignalOrValue<string | RemoveAttribute>;
	high?: SignalOrValue<number | string | RemoveAttribute>;
	low?: SignalOrValue<number | string | RemoveAttribute>;
	max?: SignalOrValue<number | string | RemoveAttribute>;
	min?: SignalOrValue<number | string | RemoveAttribute>;
	optimum?: SignalOrValue<number | string | RemoveAttribute>;
	value?: SignalOrValue<string | string[] | number | RemoveAttribute>;
}
export interface QuoteHTMLAttributes<T> extends HTMLAttributes<T> {
	cite?: SignalOrValue<string | RemoveAttribute>;
}
export interface ObjectHTMLAttributes<T> extends HTMLAttributes<T> {
	data?: SignalOrValue<string | RemoveAttribute>;
	form?: SignalOrValue<string | RemoveAttribute>;
	height?: SignalOrValue<number | string | RemoveAttribute>;
	name?: SignalOrValue<string | RemoveAttribute>;
	type?: SignalOrValue<string | RemoveAttribute>;
	width?: SignalOrValue<number | string | RemoveAttribute>;
	wmode?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	align?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	archive?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	border?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	classid?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	code?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	codebase?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	codetype?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	declare?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	hspace?: SignalOrValue<number | string | RemoveAttribute>;
	/** @deprecated */
	standby?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	usemap?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	vspace?: SignalOrValue<number | string | RemoveAttribute>;
	/** @deprecated */
	typemustmatch?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
}
export interface OlHTMLAttributes<T> extends HTMLAttributes<T> {
	reversed?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	start?: SignalOrValue<number | string | RemoveAttribute>;
	type?: SignalOrValue<"1" | "a" | "A" | "i" | "I" | RemoveAttribute>;
	/**
	 * @deprecated
	 * @non-standard
	 */
	compact?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
}
export interface OptgroupHTMLAttributes<T> extends HTMLAttributes<T> {
	disabled?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	label?: SignalOrValue<string | RemoveAttribute>;
}
export interface OptionHTMLAttributes<T> extends HTMLAttributes<T> {
	disabled?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	label?: SignalOrValue<string | RemoveAttribute>;
	selected?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	value?: SignalOrValue<string | string[] | number | RemoveAttribute>;
}
export interface OutputHTMLAttributes<T> extends HTMLAttributes<T> {
	for?: SignalOrValue<string | RemoveAttribute>;
	form?: SignalOrValue<string | RemoveAttribute>;
	name?: SignalOrValue<string | RemoveAttribute>;
}
export interface ProgressHTMLAttributes<T> extends HTMLAttributes<T> {
	max?: SignalOrValue<number | string | RemoveAttribute>;
	value?: SignalOrValue<string | string[] | number | RemoveAttribute>;
}
export interface ScriptHTMLAttributes<T> extends HTMLAttributes<T> {
	async?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	blocking?: SignalOrValue<"render" | RemoveAttribute>;
	crossorigin?: SignalOrValue<HTMLCrossorigin | RemoveAttribute>;
	defer?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	fetchpriority?: SignalOrValue<"high" | "low" | "auto" | RemoveAttribute>;
	for?: SignalOrValue<string | RemoveAttribute>;
	integrity?: SignalOrValue<string | RemoveAttribute>;
	nomodule?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	referrerpolicy?: SignalOrValue<HTMLReferrerPolicy | RemoveAttribute>;
	src?: SignalOrValue<string | RemoveAttribute>;
	type?: SignalOrValue<"importmap" | "module" | "speculationrules" | (string & {}) | RemoveAttribute>;
	/** @experimental */
	attributionsrc?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	charset?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	event?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	language?: SignalOrValue<string | RemoveAttribute>;
}
export interface SelectHTMLAttributes<T> extends HTMLAttributes<T> {
	autocomplete?: SignalOrValue<HTMLAutocomplete | RemoveAttribute>;
	disabled?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	form?: SignalOrValue<string | RemoveAttribute>;
	multiple?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	name?: SignalOrValue<string | RemoveAttribute>;
	required?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	size?: SignalOrValue<number | string | RemoveAttribute>;
	value?: SignalOrValue<string | string[] | number | RemoveAttribute>;
}
export interface HTMLSlotElementAttributes<T> extends HTMLAttributes<T> {
	name?: SignalOrValue<string | RemoveAttribute>;
}
export interface SourceHTMLAttributes<T> extends HTMLAttributes<T> {
	height?: SignalOrValue<number | string | RemoveAttribute>;
	media?: SignalOrValue<string | RemoveAttribute>;
	sizes?: SignalOrValue<string | RemoveAttribute>;
	src?: SignalOrValue<string | RemoveAttribute>;
	srcset?: SignalOrValue<string | RemoveAttribute>;
	type?: SignalOrValue<string | RemoveAttribute>;
	width?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface StyleHTMLAttributes<T> extends HTMLAttributes<T> {
	blocking?: SignalOrValue<"render" | RemoveAttribute>;
	media?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	scoped?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	type?: SignalOrValue<string | RemoveAttribute>;
}
export interface TdHTMLAttributes<T> extends HTMLAttributes<T> {
	colspan?: SignalOrValue<number | string | RemoveAttribute>;
	headers?: SignalOrValue<string | RemoveAttribute>;
	rowspan?: SignalOrValue<number | string | RemoveAttribute>;
	/** @deprecated */
	abbr?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	align?: SignalOrValue<"left" | "center" | "right" | "justify" | "char" | RemoveAttribute>;
	/** @deprecated */
	axis?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	bgcolor?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	char?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	charoff?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	height?: SignalOrValue<number | string | RemoveAttribute>;
	/** @deprecated */
	nowrap?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	scope?: SignalOrValue<"col" | "row" | "rowgroup" | "colgroup" | RemoveAttribute>;
	/** @deprecated */
	valign?: SignalOrValue<"baseline" | "bottom" | "middle" | "top" | RemoveAttribute>;
	/** @deprecated */
	width?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface TemplateHTMLAttributes<T> extends HTMLAttributes<T> {
	shadowrootclonable?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	shadowrootdelegatesfocus?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	shadowrootmode?: SignalOrValue<"open" | "closed" | RemoveAttribute>;
	shadowrootcustomelementregistry?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	/** @experimental */
	shadowrootserializable?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
}
export interface TextareaHTMLAttributes<T> extends HTMLAttributes<T> {
	autocomplete?: SignalOrValue<HTMLAutocomplete | RemoveAttribute>;
	cols?: SignalOrValue<number | string | RemoveAttribute>;
	dirname?: SignalOrValue<string | RemoveAttribute>;
	disabled?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	form?: SignalOrValue<string | RemoveAttribute>;
	maxlength?: SignalOrValue<number | string | RemoveAttribute>;
	minlength?: SignalOrValue<number | string | RemoveAttribute>;
	name?: SignalOrValue<string | RemoveAttribute>;
	placeholder?: SignalOrValue<string | RemoveAttribute>;
	readonly?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	required?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	rows?: SignalOrValue<number | string | RemoveAttribute>;
	value?: SignalOrValue<string | string[] | number | RemoveAttribute>;
	wrap?: SignalOrValue<"hard" | "soft" | "off" | RemoveAttribute>;
}
export interface ThHTMLAttributes<T> extends HTMLAttributes<T> {
	abbr?: SignalOrValue<string | RemoveAttribute>;
	colspan?: SignalOrValue<number | string | RemoveAttribute>;
	headers?: SignalOrValue<string | RemoveAttribute>;
	rowspan?: SignalOrValue<number | string | RemoveAttribute>;
	scope?: SignalOrValue<"col" | "row" | "rowgroup" | "colgroup" | RemoveAttribute>;
	/** @deprecated */
	align?: SignalOrValue<"left" | "center" | "right" | "justify" | "char" | RemoveAttribute>;
	/** @deprecated */
	axis?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	bgcolor?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	char?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	charoff?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	height?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	nowrap?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	valign?: SignalOrValue<"baseline" | "bottom" | "middle" | "top" | RemoveAttribute>;
	/** @deprecated */
	width?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface TimeHTMLAttributes<T> extends HTMLAttributes<T> {
	datetime?: SignalOrValue<string | RemoveAttribute>;
}
export interface TrackHTMLAttributes<T> extends HTMLAttributes<T> {
	default?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	kind?: SignalOrValue<"alternative" | "descriptions" | "main" | "main-desc" | "translation" | "commentary" | "subtitles" | "captions" | "chapters" | "metadata" | RemoveAttribute>;
	label?: SignalOrValue<string | RemoveAttribute>;
	src?: SignalOrValue<string | RemoveAttribute>;
	srclang?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	mediagroup?: SignalOrValue<string | RemoveAttribute>;
}
export interface VideoHTMLAttributes<T> extends MediaHTMLAttributes<T> {
	disablepictureinpicture?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	height?: SignalOrValue<number | string | RemoveAttribute>;
	playsinline?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	poster?: SignalOrValue<string | RemoveAttribute>;
	width?: SignalOrValue<number | string | RemoveAttribute>;
	onEnterPictureInPicture?: EventHandlerUnion<T, PictureInPictureEvent> | undefined;
	// "on:enterpictureinpicture"?: EventHandlerWithOptionsUnion<T, PictureInPictureEvent> | undefined;
	onLeavePictureInPicture?: EventHandlerUnion<T, PictureInPictureEvent> | undefined;
}
export interface WebViewHTMLAttributes<T> extends HTMLAttributes<T> {
	allowpopups?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	disableblinkfeatures?: SignalOrValue<string | RemoveAttribute>;
	disablewebsecurity?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	enableblinkfeatures?: SignalOrValue<string | RemoveAttribute>;
	httpreferrer?: SignalOrValue<string | RemoveAttribute>;
	nodeintegration?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	nodeintegrationinsubframes?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	partition?: SignalOrValue<string | RemoveAttribute>;
	plugins?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	preload?: SignalOrValue<string | RemoveAttribute>;
	src?: SignalOrValue<string | RemoveAttribute>;
	useragent?: SignalOrValue<string | RemoveAttribute>;
	webpreferences?: SignalOrValue<string | RemoveAttribute>;
	// does this exists?
	allowfullscreen?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	autosize?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	blinkfeatures?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	disableguestresize?: SignalOrValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	guestinstance?: SignalOrValue<string | RemoveAttribute>;
}
// SVG
export type SVGPreserveAspectRatioValue = "none" | "xMinYMin" | "xMidYMin" | "xMaxYMin" | "xMinYMid" | "xMidYMid" | "xMaxYMid" | "xMinYMax" | "xMidYMax" | "xMaxYMax" | "xMinYMin meet" | "xMidYMin meet" | "xMaxYMin meet" | "xMinYMid meet" | "xMidYMid meet" | "xMaxYMid meet" | "xMinYMax meet" | "xMidYMax meet" | "xMaxYMax meet" | "xMinYMin slice" | "xMidYMin slice" | "xMaxYMin slice" | "xMinYMid slice" | "xMidYMid slice" | "xMaxYMid slice" | "xMinYMax slice" | "xMidYMax slice" | "xMaxYMax slice";
export type ImagePreserveAspectRatio = SVGPreserveAspectRatioValue | "defer none" | "defer xMinYMin" | "defer xMidYMin" | "defer xMaxYMin" | "defer xMinYMid" | "defer xMidYMid" | "defer xMaxYMid" | "defer xMinYMax" | "defer xMidYMax" | "defer xMaxYMax" | "defer xMinYMin meet" | "defer xMidYMin meet" | "defer xMaxYMin meet" | "defer xMinYMid meet" | "defer xMidYMid meet" | "defer xMaxYMid meet" | "defer xMinYMax meet" | "defer xMidYMax meet" | "defer xMaxYMax meet" | "defer xMinYMin slice" | "defer xMidYMin slice" | "defer xMaxYMin slice" | "defer xMinYMid slice" | "defer xMidYMid slice" | "defer xMaxYMid slice" | "defer xMinYMax slice" | "defer xMidYMax slice" | "defer xMaxYMax slice";
export type SVGUnits = "userSpaceOnUse" | "objectBoundingBox";
export interface StylableSVGAttributes {
	class?: SignalOrValue<string | ClassNames | RemoveAttribute>;
	style?: SignalOrValue<CSSProperties | string | RemoveAttribute>;
}
export interface TransformableSVGAttributes {
	transform?: SignalOrValue<string | RemoveAttribute>;
}
export interface ConditionalProcessingSVGAttributes {
	requiredExtensions?: SignalOrValue<string | RemoveAttribute>;
	requiredFeatures?: SignalOrValue<string | RemoveAttribute>;
	systemLanguage?: SignalOrValue<string | RemoveAttribute>;
}
export interface ExternalResourceSVGAttributes {
	externalResourcesRequired?: SignalOrValue<EnumeratedPseudoBoolean | RemoveAttribute>;
}
export interface AnimationTimingSVGAttributes {
	begin?: SignalOrValue<string | RemoveAttribute>;
	dur?: SignalOrValue<string | RemoveAttribute>;
	end?: SignalOrValue<string | RemoveAttribute>;
	fill?: SignalOrValue<"freeze" | "remove" | RemoveAttribute>;
	max?: SignalOrValue<string | RemoveAttribute>;
	min?: SignalOrValue<string | RemoveAttribute>;
	repeatCount?: SignalOrValue<number | "indefinite" | RemoveAttribute>;
	repeatDur?: SignalOrValue<string | RemoveAttribute>;
	restart?: SignalOrValue<"always" | "whenNotActive" | "never" | RemoveAttribute>;
}
export interface AnimationValueSVGAttributes {
	by?: SignalOrValue<number | string | RemoveAttribute>;
	calcMode?: SignalOrValue<"discrete" | "linear" | "paced" | "spline" | RemoveAttribute>;
	from?: SignalOrValue<number | string | RemoveAttribute>;
	keySplines?: SignalOrValue<string | RemoveAttribute>;
	keyTimes?: SignalOrValue<string | RemoveAttribute>;
	to?: SignalOrValue<number | string | RemoveAttribute>;
	values?: SignalOrValue<string | RemoveAttribute>;
}
export interface AnimationAdditionSVGAttributes {
	accumulate?: SignalOrValue<"none" | "sum" | RemoveAttribute>;
	additive?: SignalOrValue<"replace" | "sum" | RemoveAttribute>;
	attributeName?: SignalOrValue<string | RemoveAttribute>;
}
export interface AnimationAttributeTargetSVGAttributes {
	attributeName?: SignalOrValue<string | RemoveAttribute>;
	attributeType?: SignalOrValue<"CSS" | "XML" | "auto" | RemoveAttribute>;
}
export interface PresentationSVGAttributes {
	"alignment-baseline"?: SignalOrValue<"auto" | "baseline" | "before-edge" | "text-before-edge" | "middle" | "central" | "after-edge" | "text-after-edge" | "ideographic" | "alphabetic" | "hanging" | "mathematical" | "inherit" | RemoveAttribute>;
	"baseline-shift"?: SignalOrValue<number | string | RemoveAttribute>;
	"clip-path"?: SignalOrValue<string | RemoveAttribute>;
	"clip-rule"?: SignalOrValue<"nonzero" | "evenodd" | "inherit" | RemoveAttribute>;
	"color-interpolation"?: SignalOrValue<"auto" | "sRGB" | "linearRGB" | "inherit" | RemoveAttribute>;
	"color-interpolation-filters"?: SignalOrValue<"auto" | "sRGB" | "linearRGB" | "inherit" | RemoveAttribute>;
	"color-profile"?: SignalOrValue<string | RemoveAttribute>;
	"color-rendering"?: SignalOrValue<"auto" | "optimizeSpeed" | "optimizeQuality" | "inherit" | RemoveAttribute>;
	"dominant-baseline"?: SignalOrValue<"auto" | "text-bottom" | "alphabetic" | "ideographic" | "middle" | "central" | "mathematical" | "hanging" | "text-top" | "inherit" | RemoveAttribute>;
	"enable-background"?: SignalOrValue<string | RemoveAttribute>;
	"fill-opacity"?: SignalOrValue<number | string | "inherit" | RemoveAttribute>;
	"fill-rule"?: SignalOrValue<"nonzero" | "evenodd" | "inherit" | RemoveAttribute>;
	"flood-color"?: SignalOrValue<string | RemoveAttribute>;
	"flood-opacity"?: SignalOrValue<number | string | "inherit" | RemoveAttribute>;
	"font-family"?: SignalOrValue<string | RemoveAttribute>;
	"font-size"?: SignalOrValue<string | RemoveAttribute>;
	"font-size-adjust"?: SignalOrValue<number | string | RemoveAttribute>;
	"font-stretch"?: SignalOrValue<string | RemoveAttribute>;
	"font-style"?: SignalOrValue<"normal" | "italic" | "oblique" | "inherit" | RemoveAttribute>;
	"font-variant"?: SignalOrValue<string | RemoveAttribute>;
	"font-weight"?: SignalOrValue<number | string | RemoveAttribute>;
	"glyph-orientation-horizontal"?: SignalOrValue<string | RemoveAttribute>;
	"glyph-orientation-vertical"?: SignalOrValue<string | RemoveAttribute>;
	"image-rendering"?: SignalOrValue<"auto" | "optimizeQuality" | "optimizeSpeed" | "inherit" | RemoveAttribute>;
	"letter-spacing"?: SignalOrValue<number | string | RemoveAttribute>;
	"lighting-color"?: SignalOrValue<string | RemoveAttribute>;
	"marker-end"?: SignalOrValue<string | RemoveAttribute>;
	"marker-mid"?: SignalOrValue<string | RemoveAttribute>;
	"marker-start"?: SignalOrValue<string | RemoveAttribute>;
	"pointer-events"?: SignalOrValue<"bounding-box" | "visiblePainted" | "visibleFill" | "visibleStroke" | "visible" | "painted" | "color" | "fill" | "stroke" | "all" | "none" | "inherit" | RemoveAttribute>;
	"shape-rendering"?: SignalOrValue<"auto" | "optimizeSpeed" | "crispEdges" | "geometricPrecision" | "inherit" | RemoveAttribute>;
	"stop-color"?: SignalOrValue<string | RemoveAttribute>;
	"stop-opacity"?: SignalOrValue<number | string | "inherit" | RemoveAttribute>;
	"stroke-dasharray"?: SignalOrValue<string | RemoveAttribute>;
	"stroke-dashoffset"?: SignalOrValue<number | string | RemoveAttribute>;
	"stroke-linecap"?: SignalOrValue<"butt" | "round" | "square" | "inherit" | RemoveAttribute>;
	"stroke-linejoin"?: SignalOrValue<"arcs" | "bevel" | "miter" | "miter-clip" | "round" | "inherit" | RemoveAttribute>;
	"stroke-miterlimit"?: SignalOrValue<number | string | "inherit" | RemoveAttribute>;
	"stroke-opacity"?: SignalOrValue<number | string | "inherit" | RemoveAttribute>;
	"stroke-width"?: SignalOrValue<number | string | RemoveAttribute>;
	"text-anchor"?: SignalOrValue<"start" | "middle" | "end" | "inherit" | RemoveAttribute>;
	"text-decoration"?: SignalOrValue<"none" | "underline" | "overline" | "line-through" | "blink" | "inherit" | RemoveAttribute>;
	"text-rendering"?: SignalOrValue<"auto" | "optimizeSpeed" | "optimizeLegibility" | "geometricPrecision" | "inherit" | RemoveAttribute>;
	"unicode-bidi"?: SignalOrValue<string | RemoveAttribute>;
	"word-spacing"?: SignalOrValue<number | string | RemoveAttribute>;
	"writing-mode"?: SignalOrValue<"lr-tb" | "rl-tb" | "tb-rl" | "lr" | "rl" | "tb" | "inherit" | RemoveAttribute>;
	clip?: SignalOrValue<string | RemoveAttribute>;
	color?: SignalOrValue<string | RemoveAttribute>;
	cursor?: SignalOrValue<string | RemoveAttribute>;
	direction?: SignalOrValue<"ltr" | "rtl" | "inherit" | RemoveAttribute>;
	display?: SignalOrValue<string | RemoveAttribute>;
	fill?: SignalOrValue<string | RemoveAttribute>;
	filter?: SignalOrValue<string | RemoveAttribute>;
	kerning?: SignalOrValue<string | RemoveAttribute>;
	mask?: SignalOrValue<string | RemoveAttribute>;
	opacity?: SignalOrValue<number | string | "inherit" | RemoveAttribute>;
	overflow?: SignalOrValue<"visible" | "hidden" | "scroll" | "auto" | "inherit" | RemoveAttribute>;
	pathLength?: SignalOrValue<string | number | RemoveAttribute>;
	stroke?: SignalOrValue<string | RemoveAttribute>;
	visibility?: SignalOrValue<"visible" | "hidden" | "collapse" | "inherit" | RemoveAttribute>;
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
	height?: SignalOrValue<number | string | RemoveAttribute>;
	result?: SignalOrValue<string | RemoveAttribute>;
	width?: SignalOrValue<number | string | RemoveAttribute>;
	x?: SignalOrValue<number | string | RemoveAttribute>;
	y?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface SingleInputFilterSVGAttributes {
	in?: SignalOrValue<string | RemoveAttribute>;
}
export interface DoubleInputFilterSVGAttributes {
	in?: SignalOrValue<string | RemoveAttribute>;
	in2?: SignalOrValue<string | RemoveAttribute>;
}
export interface FitToViewBoxSVGAttributes {
	preserveAspectRatio?: SignalOrValue<SVGPreserveAspectRatioValue | RemoveAttribute>;
	viewBox?: SignalOrValue<string | RemoveAttribute>;
}
export interface GradientElementSVGAttributes<T> extends SVGAttributes<T>, ExternalResourceSVGAttributes, StylableSVGAttributes {
	gradientTransform?: SignalOrValue<string | RemoveAttribute>;
	gradientUnits?: SignalOrValue<SVGUnits | RemoveAttribute>;
	href?: SignalOrValue<string | RemoveAttribute>;
	spreadMethod?: SignalOrValue<"pad" | "reflect" | "repeat" | RemoveAttribute>;
}
export interface GraphicsElementSVGAttributes<T> extends SVGAttributes<T>, Pick<PresentationSVGAttributes, "clip-rule" | "mask" | "pointer-events" | "cursor" | "opacity" | "filter" | "display" | "visibility" | "color-interpolation" | "color-rendering"> {
}
export interface LightSourceElementSVGAttributes<T> extends SVGAttributes<T> {
}
export interface NewViewportSVGAttributes<T> extends SVGAttributes<T>, Pick<PresentationSVGAttributes, "overflow" | "clip"> {
	viewBox?: SignalOrValue<string | RemoveAttribute>;
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
	zoomAndPan?: SignalOrValue<"disable" | "magnify" | RemoveAttribute>;
}
export interface AnimateSVGAttributes<T> extends AnimationElementSVGAttributes<T>, AnimationAttributeTargetSVGAttributes, AnimationTimingSVGAttributes, AnimationValueSVGAttributes, AnimationAdditionSVGAttributes, Pick<PresentationSVGAttributes, "color-interpolation" | "color-rendering"> {
}
export interface AnimateMotionSVGAttributes<T> extends AnimationElementSVGAttributes<T>, AnimationTimingSVGAttributes, AnimationValueSVGAttributes, AnimationAdditionSVGAttributes {
	keyPoints?: SignalOrValue<string | RemoveAttribute>;
	origin?: SignalOrValue<"default" | RemoveAttribute>;
	path?: SignalOrValue<string | RemoveAttribute>;
	rotate?: SignalOrValue<number | string | "auto" | "auto-reverse" | RemoveAttribute>;
}
export interface AnimateTransformSVGAttributes<T> extends AnimationElementSVGAttributes<T>, AnimationAttributeTargetSVGAttributes, AnimationTimingSVGAttributes, AnimationValueSVGAttributes, AnimationAdditionSVGAttributes {
	type?: SignalOrValue<"translate" | "scale" | "rotate" | "skewX" | "skewY" | RemoveAttribute>;
}
export interface CircleSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path"> {
	cx?: SignalOrValue<number | string | RemoveAttribute>;
	cy?: SignalOrValue<number | string | RemoveAttribute>;
	r?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface ClipPathSVGAttributes<T> extends SVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path"> {
	clipPathUnits?: SignalOrValue<SVGUnits | RemoveAttribute>;
}
export interface DefsSVGAttributes<T> extends ContainerElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes {
}
export interface DescSVGAttributes<T> extends SVGAttributes<T>, StylableSVGAttributes {
}
export interface EllipseSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path"> {
	cx?: SignalOrValue<number | string | RemoveAttribute>;
	cy?: SignalOrValue<number | string | RemoveAttribute>;
	rx?: SignalOrValue<number | string | RemoveAttribute>;
	ry?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface FeBlendSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, DoubleInputFilterSVGAttributes, StylableSVGAttributes {
	mode?: SignalOrValue<"normal" | "multiply" | "screen" | "darken" | "lighten" | RemoveAttribute>;
}
export interface FeColorMatrixSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
	type?: SignalOrValue<"matrix" | "saturate" | "hueRotate" | "luminanceToAlpha" | RemoveAttribute>;
	values?: SignalOrValue<string | RemoveAttribute>;
}
export interface FeComponentTransferSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
}
export interface FeCompositeSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, DoubleInputFilterSVGAttributes, StylableSVGAttributes {
	k1?: SignalOrValue<number | string | RemoveAttribute>;
	k2?: SignalOrValue<number | string | RemoveAttribute>;
	k3?: SignalOrValue<number | string | RemoveAttribute>;
	k4?: SignalOrValue<number | string | RemoveAttribute>;
	operator?: SignalOrValue<"over" | "in" | "out" | "atop" | "xor" | "arithmetic" | RemoveAttribute>;
}
export interface FeConvolveMatrixSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
	bias?: SignalOrValue<number | string | RemoveAttribute>;
	divisor?: SignalOrValue<number | string | RemoveAttribute>;
	edgeMode?: SignalOrValue<"duplicate" | "wrap" | "none" | RemoveAttribute>;
	kernelMatrix?: SignalOrValue<string | RemoveAttribute>;
	kernelUnitLength?: SignalOrValue<number | string | RemoveAttribute>;
	order?: SignalOrValue<number | string | RemoveAttribute>;
	preserveAlpha?: SignalOrValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	targetX?: SignalOrValue<number | string | RemoveAttribute>;
	targetY?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface FeDiffuseLightingSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes, Pick<PresentationSVGAttributes, "color" | "lighting-color"> {
	diffuseConstant?: SignalOrValue<number | string | RemoveAttribute>;
	kernelUnitLength?: SignalOrValue<number | string | RemoveAttribute>;
	surfaceScale?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface FeDisplacementMapSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, DoubleInputFilterSVGAttributes, StylableSVGAttributes {
	scale?: SignalOrValue<number | string | RemoveAttribute>;
	xChannelSelector?: SignalOrValue<"R" | "G" | "B" | "A" | RemoveAttribute>;
	yChannelSelector?: SignalOrValue<"R" | "G" | "B" | "A" | RemoveAttribute>;
}
export interface FeDistantLightSVGAttributes<T> extends LightSourceElementSVGAttributes<T> {
	azimuth?: SignalOrValue<number | string | RemoveAttribute>;
	elevation?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface FeDropShadowSVGAttributes<T> extends SVGAttributes<T>, FilterPrimitiveElementSVGAttributes<T>, StylableSVGAttributes, Pick<PresentationSVGAttributes, "color" | "flood-color" | "flood-opacity"> {
	dx?: SignalOrValue<number | string | RemoveAttribute>;
	dy?: SignalOrValue<number | string | RemoveAttribute>;
	stdDeviation?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface FeFloodSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, StylableSVGAttributes, Pick<PresentationSVGAttributes, "color" | "flood-color" | "flood-opacity"> {
}
export interface FeFuncSVGAttributes<T> extends SVGAttributes<T> {
	amplitude?: SignalOrValue<number | string | RemoveAttribute>;
	exponent?: SignalOrValue<number | string | RemoveAttribute>;
	intercept?: SignalOrValue<number | string | RemoveAttribute>;
	offset?: SignalOrValue<number | string | RemoveAttribute>;
	slope?: SignalOrValue<number | string | RemoveAttribute>;
	tableValues?: SignalOrValue<string | RemoveAttribute>;
	type?: SignalOrValue<"identity" | "table" | "discrete" | "linear" | "gamma" | RemoveAttribute>;
}
export interface FeGaussianBlurSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
	stdDeviation?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface FeImageSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, ExternalResourceSVGAttributes, StylableSVGAttributes {
	href?: SignalOrValue<string | RemoveAttribute>;
	preserveAspectRatio?: SignalOrValue<SVGPreserveAspectRatioValue | RemoveAttribute>;
}
export interface FeMergeSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, StylableSVGAttributes {
}
export interface FeMergeNodeSVGAttributes<T> extends SVGAttributes<T>, SingleInputFilterSVGAttributes {
}
export interface FeMorphologySVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
	operator?: SignalOrValue<"erode" | "dilate" | RemoveAttribute>;
	radius?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface FeOffsetSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
	dx?: SignalOrValue<number | string | RemoveAttribute>;
	dy?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface FePointLightSVGAttributes<T> extends LightSourceElementSVGAttributes<T> {
	x?: SignalOrValue<number | string | RemoveAttribute>;
	y?: SignalOrValue<number | string | RemoveAttribute>;
	z?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface FeSpecularLightingSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes, Pick<PresentationSVGAttributes, "color" | "lighting-color"> {
	kernelUnitLength?: SignalOrValue<number | string | RemoveAttribute>;
	specularConstant?: SignalOrValue<string | RemoveAttribute>;
	specularExponent?: SignalOrValue<string | RemoveAttribute>;
	surfaceScale?: SignalOrValue<string | RemoveAttribute>;
}
export interface FeSpotLightSVGAttributes<T> extends LightSourceElementSVGAttributes<T> {
	limitingConeAngle?: SignalOrValue<number | string | RemoveAttribute>;
	pointsAtX?: SignalOrValue<number | string | RemoveAttribute>;
	pointsAtY?: SignalOrValue<number | string | RemoveAttribute>;
	pointsAtZ?: SignalOrValue<number | string | RemoveAttribute>;
	specularExponent?: SignalOrValue<number | string | RemoveAttribute>;
	x?: SignalOrValue<number | string | RemoveAttribute>;
	y?: SignalOrValue<number | string | RemoveAttribute>;
	z?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface FeTileSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
}
export interface FeTurbulanceSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, StylableSVGAttributes {
	baseFrequency?: SignalOrValue<number | string | RemoveAttribute>;
	numOctaves?: SignalOrValue<number | string | RemoveAttribute>;
	seed?: SignalOrValue<number | string | RemoveAttribute>;
	stitchTiles?: SignalOrValue<"stitch" | "noStitch" | RemoveAttribute>;
	type?: SignalOrValue<"fractalNoise" | "turbulence" | RemoveAttribute>;
}
export interface FilterSVGAttributes<T> extends SVGAttributes<T>, ExternalResourceSVGAttributes, StylableSVGAttributes {
	filterRes?: SignalOrValue<number | string | RemoveAttribute>;
	filterUnits?: SignalOrValue<SVGUnits | RemoveAttribute>;
	height?: SignalOrValue<number | string | RemoveAttribute>;
	primitiveUnits?: SignalOrValue<SVGUnits | RemoveAttribute>;
	width?: SignalOrValue<number | string | RemoveAttribute>;
	x?: SignalOrValue<number | string | RemoveAttribute>;
	y?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface ForeignObjectSVGAttributes<T> extends NewViewportSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "display" | "visibility"> {
	height?: SignalOrValue<number | string | RemoveAttribute>;
	width?: SignalOrValue<number | string | RemoveAttribute>;
	x?: SignalOrValue<number | string | RemoveAttribute>;
	y?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface GSVGAttributes<T> extends ContainerElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "display" | "visibility"> {
}
export interface ImageSVGAttributes<T> extends NewViewportSVGAttributes<T>, GraphicsElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "color-profile" | "image-rendering"> {
	height?: SignalOrValue<number | string | RemoveAttribute>;
	href?: SignalOrValue<string | RemoveAttribute>;
	preserveAspectRatio?: SignalOrValue<ImagePreserveAspectRatio | RemoveAttribute>;
	width?: SignalOrValue<number | string | RemoveAttribute>;
	x?: SignalOrValue<number | string | RemoveAttribute>;
	y?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface LineSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "marker-start" | "marker-mid" | "marker-end"> {
	x1?: SignalOrValue<number | string | RemoveAttribute>;
	x2?: SignalOrValue<number | string | RemoveAttribute>;
	y1?: SignalOrValue<number | string | RemoveAttribute>;
	y2?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface LinearGradientSVGAttributes<T> extends GradientElementSVGAttributes<T> {
	x1?: SignalOrValue<number | string | RemoveAttribute>;
	x2?: SignalOrValue<number | string | RemoveAttribute>;
	y1?: SignalOrValue<number | string | RemoveAttribute>;
	y2?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface MarkerSVGAttributes<T> extends ContainerElementSVGAttributes<T>, ExternalResourceSVGAttributes, StylableSVGAttributes, FitToViewBoxSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "overflow" | "clip"> {
	markerHeight?: SignalOrValue<number | string | RemoveAttribute>;
	markerUnits?: SignalOrValue<"strokeWidth" | "userSpaceOnUse" | RemoveAttribute>;
	markerWidth?: SignalOrValue<number | string | RemoveAttribute>;
	orient?: SignalOrValue<string | RemoveAttribute>;
	refX?: SignalOrValue<number | string | RemoveAttribute>;
	refY?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface MaskSVGAttributes<T> extends Omit<ContainerElementSVGAttributes<T>, "opacity" | "filter">, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path"> {
	height?: SignalOrValue<number | string | RemoveAttribute>;
	maskContentUnits?: SignalOrValue<SVGUnits | RemoveAttribute>;
	maskUnits?: SignalOrValue<SVGUnits | RemoveAttribute>;
	width?: SignalOrValue<number | string | RemoveAttribute>;
	x?: SignalOrValue<number | string | RemoveAttribute>;
	y?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface MetadataSVGAttributes<T> extends SVGAttributes<T> {
}
export interface MPathSVGAttributes<T> extends SVGAttributes<T> {
}
export interface PathSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "marker-start" | "marker-mid" | "marker-end"> {
	d?: SignalOrValue<string | RemoveAttribute>;
	pathLength?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface PatternSVGAttributes<T> extends ContainerElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, FitToViewBoxSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "overflow" | "clip"> {
	height?: SignalOrValue<number | string | RemoveAttribute>;
	href?: SignalOrValue<string | RemoveAttribute>;
	patternContentUnits?: SignalOrValue<SVGUnits | RemoveAttribute>;
	patternTransform?: SignalOrValue<string | RemoveAttribute>;
	patternUnits?: SignalOrValue<SVGUnits | RemoveAttribute>;
	width?: SignalOrValue<number | string | RemoveAttribute>;
	x?: SignalOrValue<number | string | RemoveAttribute>;
	y?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface PolygonSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "marker-start" | "marker-mid" | "marker-end"> {
	points?: SignalOrValue<string | RemoveAttribute>;
}
export interface PolylineSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "marker-start" | "marker-mid" | "marker-end"> {
	points?: SignalOrValue<string | RemoveAttribute>;
}
export interface RadialGradientSVGAttributes<T> extends GradientElementSVGAttributes<T> {
	cx?: SignalOrValue<number | string | RemoveAttribute>;
	cy?: SignalOrValue<number | string | RemoveAttribute>;
	fx?: SignalOrValue<number | string | RemoveAttribute>;
	fy?: SignalOrValue<number | string | RemoveAttribute>;
	r?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface RectSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path"> {
	height?: SignalOrValue<number | string | RemoveAttribute>;
	rx?: SignalOrValue<number | string | RemoveAttribute>;
	ry?: SignalOrValue<number | string | RemoveAttribute>;
	width?: SignalOrValue<number | string | RemoveAttribute>;
	x?: SignalOrValue<number | string | RemoveAttribute>;
	y?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface SetSVGAttributes<T> extends AnimationElementSVGAttributes<T>, StylableSVGAttributes, AnimationTimingSVGAttributes {
}
export interface StopSVGAttributes<T> extends SVGAttributes<T>, StylableSVGAttributes, Pick<PresentationSVGAttributes, "color" | "stop-color" | "stop-opacity"> {
	offset?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface SvgSVGAttributes<T> extends ContainerElementSVGAttributes<T>, NewViewportSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, FitToViewBoxSVGAttributes, ZoomAndPanSVGAttributes, PresentationSVGAttributes, EventHandlersWindow<T> {
	"xmlns:xlink"?: SignalOrValue<string | RemoveAttribute>;
	contentScriptType?: SignalOrValue<string | RemoveAttribute>;
	contentStyleType?: SignalOrValue<string | RemoveAttribute>;
	height?: SignalOrValue<number | string | RemoveAttribute>;
	width?: SignalOrValue<number | string | RemoveAttribute>;
	x?: SignalOrValue<number | string | RemoveAttribute>;
	y?: SignalOrValue<number | string | RemoveAttribute>;
	/** @deprecated */
	baseProfile?: SignalOrValue<string | RemoveAttribute>;
	/** @deprecated */
	version?: SignalOrValue<string | RemoveAttribute>;
}
export interface SwitchSVGAttributes<T> extends ContainerElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "display" | "visibility"> {
}
export interface SymbolSVGAttributes<T> extends ContainerElementSVGAttributes<T>, NewViewportSVGAttributes<T>, ExternalResourceSVGAttributes, StylableSVGAttributes, FitToViewBoxSVGAttributes, Pick<PresentationSVGAttributes, "clip-path"> {
	height?: SignalOrValue<number | string | RemoveAttribute>;
	preserveAspectRatio?: SignalOrValue<SVGPreserveAspectRatioValue | RemoveAttribute>;
	refX?: SignalOrValue<number | string | RemoveAttribute>;
	refY?: SignalOrValue<number | string | RemoveAttribute>;
	viewBox?: SignalOrValue<string | RemoveAttribute>;
	width?: SignalOrValue<number | string | RemoveAttribute>;
	x?: SignalOrValue<number | string | RemoveAttribute>;
	y?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface TextSVGAttributes<T> extends TextContentElementSVGAttributes<T>, GraphicsElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "writing-mode" | "text-rendering"> {
	dx?: SignalOrValue<number | string | RemoveAttribute>;
	dy?: SignalOrValue<number | string | RemoveAttribute>;
	lengthAdjust?: SignalOrValue<"spacing" | "spacingAndGlyphs" | RemoveAttribute>;
	rotate?: SignalOrValue<number | string | RemoveAttribute>;
	textLength?: SignalOrValue<number | string | RemoveAttribute>;
	x?: SignalOrValue<number | string | RemoveAttribute>;
	y?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface TextPathSVGAttributes<T> extends TextContentElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, Pick<PresentationSVGAttributes, "alignment-baseline" | "baseline-shift" | "display" | "visibility"> {
	href?: SignalOrValue<string | RemoveAttribute>;
	method?: SignalOrValue<"align" | "stretch" | RemoveAttribute>;
	spacing?: SignalOrValue<"auto" | "exact" | RemoveAttribute>;
	startOffset?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface TSpanSVGAttributes<T> extends TextContentElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, Pick<PresentationSVGAttributes, "alignment-baseline" | "baseline-shift" | "display" | "visibility"> {
	dx?: SignalOrValue<number | string | RemoveAttribute>;
	dy?: SignalOrValue<number | string | RemoveAttribute>;
	lengthAdjust?: SignalOrValue<"spacing" | "spacingAndGlyphs" | RemoveAttribute>;
	rotate?: SignalOrValue<number | string | RemoveAttribute>;
	textLength?: SignalOrValue<number | string | RemoveAttribute>;
	x?: SignalOrValue<number | string | RemoveAttribute>;
	y?: SignalOrValue<number | string | RemoveAttribute>;
}
/** @see https://developer.mozilla.org/en-US/docs/Web/SVG/Element/use */
export interface UseSVGAttributes<T> extends SVGAttributes<T>, StylableSVGAttributes, ConditionalProcessingSVGAttributes, GraphicsElementSVGAttributes<T>, PresentationSVGAttributes, ExternalResourceSVGAttributes, TransformableSVGAttributes {
	height?: SignalOrValue<number | string | RemoveAttribute>;
	href?: SignalOrValue<string | RemoveAttribute>;
	width?: SignalOrValue<number | string | RemoveAttribute>;
	x?: SignalOrValue<number | string | RemoveAttribute>;
	y?: SignalOrValue<number | string | RemoveAttribute>;
}
export interface ViewSVGAttributes<T> extends SVGAttributes<T>, ExternalResourceSVGAttributes, FitToViewBoxSVGAttributes, ZoomAndPanSVGAttributes {
	viewTarget?: SignalOrValue<string | RemoveAttribute>;
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
export type DataKeys = `data-${string}`;
export declare function jsx<THtmlTag extends keyof HTMLElementTagNameMap, TElement extends HTMLElementTagNameMap[THtmlTag]>(type: THtmlTag, props?: (HTMLElementTags[THtmlTag] & Record<DataKeys, string | number>) | null, key?: string): TElement;
export declare function jsx<TSvgTag extends (keyof SVGElementTagNameMap & keyof SVGElementTags), TElement extends SVGElementTagNameMap[TSvgTag]>(type: TSvgTag, props?: (SVGElementTags[TSvgTag] & Record<DataKeys, string | number>) | null, key?: string): TElement;
export declare function jsx(type: string, props?: (ElementAttributes<JSXElement> & Record<DataKeys, string | number>) | null, key?: string): JSXElement;
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
