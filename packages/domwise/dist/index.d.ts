/**
 * Base interface for JSX prop hooks. A prop hook is a callable object that can
 * be assigned as a value to a JSX attribute to reactively bind to an element.
 * @typeParam TNode - The type of the DOM node the hook binds to.
 */
export interface PropHook<TNode extends Element = Element> {
}
/**
 * A class list manager created by `useClassList`. It wraps a `DOMTokenList`
 * and provides a subset of the native `classList` API (`add`, `remove`,
 * `toggle`, `contains`, `size`, `value`). It can also be used as a JSX prop
 * hook to reactively bind the `class` attribute; see {@link useClassList}.
 */
export interface BasicClassList extends PropHook<Element> {
	/** Returns the underlying `DOMTokenList` (detached before binding, live after). */
	(): DOMTokenList;
	/** Number of tokens in the list. */
	readonly size: number;
	/** Space-separated string of all tokens (mirrors `DOMTokenList.value`). */
	readonly value: string;
	/**
	 * Adds one or more tokens to the list. Duplicate tokens are ignored.
	 * @param tokens - Class names to add.
	 */
	add(...tokens: string[]): void;
	/**
	 * Removes one or more tokens from the list.
	 * @param tokens - Class names to remove.
	 */
	remove(...tokens: string[]): void;
	/**
	 * Toggles a token, optionally forcing the presence or absence.
	 * @param token - Class name to toggle.
	 * @param force - When provided, forces add (`true`) or remove (`false`).
	 */
	toggle(token: string, force?: boolean): void;
	/**
	 * Checks whether the list contains the given token.
	 * @param token - Class name to test.
	 * @returns `true` if the token is present.
	 */
	contains(token: string): boolean;
}
export type ClassName = string | {
	[key: string]: boolean;
} | false | null | undefined | ClassName[];
/**
 * A value that can be used as a `class` attribute: a string, an array of class
 * names, an iterable, a dictionary of boolean flags, or a `DOMTokenList`.
 */
export type ClassNames = ClassName | Iterable<string> | DOMTokenList;
/**
 * A mutable reference container with a `current` property.
 * @typeParam T - The type of the referenced value.
 */
export type RefObject<T> = {
	current: T | null;
};
/**
 * A callback invoked with the referenced instance.
 * @typeParam T - The type of the referenced instance.
 */
export type RefCallback<T> = (instance: T) => void;
/**
 * A reference to a DOM node or component instance: either a `RefObject`, a ref
 * callback, or `null`.
 * @typeParam T - The type of the referenced value.
 */
export type Ref<T> = RefCallback<T> | RefObject<T> | null;
/**
 * A function that disposes an effect or subscription.
 */
export type EffectDisposer = (() => void) | null;
/**
 * A read-only signal-like value that can be subscribed to and peeked.
 * Compatible with `@preact/signals-core` and any duck-typed signal that
 * exposes `value`, `peek`, and `subscribe`.
 * @typeParam T - The type of the signal's value.
 */
export interface SignalLike<T> {
	/** Current value; reading may track a dependency when inside an effect/computed. */
	get value(): T;
	/**
	 * Returns the current value without creating a dependency.
	 * @returns The current value.
	 */
	peek(): T;
	/**
	 * Subscribes to value changes.
	 * @param fn - Callback invoked with each new value (and typically immediately with the current value).
	 * @returns A disposer that unsubscribes, or `null` if unsubscription is not supported.
	 */
	subscribe(fn: (value: T) => void): EffectDisposer;
}
/**
 * A writable signal whose `value` can be set.
 * @typeParam T - The type of the signal's value.
 */
export interface Signal<T> extends SignalLike<T> {
	set value(value: T);
}
/**
 * A read-only (computed) signal.
 * @typeParam T - The type of the computed value.
 */
export interface Computed<T> extends SignalLike<T> {
}
/**
 * A value or a signal-like value that can be used interchangeably.
 * @typeParam T - The type of the value.
 */
export type SignalOrValue<T> = T | SignalLike<T>;
/**
 * A two-way prop binding hook created by `usePropBinding`. It acts as a
 * getter when called with no arguments and a setter when called with a value;
 * when attached as a prop hook it synchronizes that value to the bound element
 * attribute.
 * @typeParam T - The type of the bound value.
 * @typeParam TElement - The type of the element the binding is attached to.
 */
export interface PropBinding<T = any, TElement extends Element = Element> extends PropHook<TElement> {
	/**
	 * Gets the current bound value.
	 * @returns The current value.
	 */
	(): T;
	/**
	 * Sets the bound value and synchronizes it to the attached element (if any).
	 * @param value - New value to store and propagate to the DOM.
	 * @returns The value that was set.
	 */
	(value: T): T;
}
/**
 * A value that can be assigned to a JSX attribute: a plain value, a prop hook,
 * or a signal-like value.
 * @typeParam T - The type of the attribute value.
 */
export type PropValue<T> = T | PropHook<Element> | SignalLike<T>;
/**
 * Custom DOM attributes supported by DomWise on all elements.
 * Includes `children`, `ref`, `dangerouslySetInnerHTML`, and jsx-dom/react-compatible
 * `on` / `onCapture` event maps.
 * @typeParam T - The type of the DOM element.
 */
export interface CustomDomAttributes<T> {
	/** Child nodes / JSX children for the element. */
	children?: ComponentChildren;
	/** Raw HTML to assign via `innerHTML`. Use with caution — content is not escaped. */
	dangerouslySetInnerHTML?: {
		__html: string;
	};
	/** Ref object or callback that receives the created DOM node. */
	ref?: Ref<T>;
	/** Compatibility event map for `on*` handlers (jsx-dom / React style). */
	on?: Record<string, Function>;
	/** Compatibility event map for capture-phase handlers. */
	onCapture?: Record<string, Function>;
}
export interface ElementAttributes<T> {
	className?: ElementAttributes<T>["class"];
	tabIndex?: PropValue<number | string | RemoveAttribute>;
	namespaceURI?: string | undefined;
	onClickCapture?: EventHandlerUnion<T, MouseEvent> | undefined;
	onDblClickCapture?: EventHandlerUnion<T, MouseEvent> | undefined;
	onDoubleClick?: EventHandlerUnion<T, MouseEvent> | undefined;
	onDoubleClickCapture?: EventHandlerUnion<T, MouseEvent> | undefined;
}
interface HTMLAttributes<T> {
	contentEditable?: PropValue<EnumeratedPseudoBoolean | EnumeratedAcceptsEmpty | "plaintext-only" | "inherit" | RemoveAttribute>;
	dataset?: {
		[key: string]: string;
	} | undefined;
	spellCheck?: PropValue<EnumeratedPseudoBoolean | EnumeratedAcceptsEmpty | RemoveAttribute>;
}
interface SVGAttributes<T> {
	tabIndex?: PropValue<number | string | RemoveAttribute>;
}
interface AnchorHTMLAttributes<T> {
	/** @deprecated use referrerpolicy */
	referrerPolicy?: PropValue<HTMLReferrerPolicy | RemoveAttribute>;
}
interface ButtonHTMLAttributes<T> {
	autoFocus?: PropValue<boolean | RemoveAttribute>;
	formNoValidate?: PropValue<boolean | RemoveAttribute>;
}
interface FormHTMLAttributes<T> extends HTMLAttributes<T> {
	autoComplete?: PropValue<"on" | "off" | RemoveAttribute>;
}
interface InputHTMLAttributes<T> {
	autoComplete?: PropValue<HTMLAutocomplete | RemoveAttribute>;
	maxLength?: PropValue<string | number | RemoveAttribute>;
	minLength?: PropValue<string | number | RemoveAttribute>;
	readOnly?: PropValue<boolean | RemoveAttribute>;
}
interface LabelHTMLAttributes<T> {
	htmlFor?: PropValue<string | RemoveAttribute>;
}
interface TdHTMLAttributes<T> {
	colSpan?: PropValue<number | string | RemoveAttribute>;
	rowSpan?: PropValue<number | string | RemoveAttribute>;
}
export type RemoveIndex<T> = {
	[K in keyof T as string extends K ? never : number extends K ? never : K]: T[K];
};
// eslint-disable-next-line @typescript-eslint/no-unsafe-function-type
export type ExcludeMethods<T> = Pick<T, {
	[K in keyof T]: T[K] extends Function ? never : K;
}[keyof T]>;
/**
 * Style properties that can be assigned to the `style` attribute, with methods,
 * readonly properties, and the index signature filtered out of
 * `CSSStyleDeclaration`.
 */
export type StyleAttributes = Partial<ExcludeMethods<RemoveIndex<Omit<CSSStyleDeclaration, "length" | "parentRules">>>>;
/** CSSStyleDeclaration contains methods, readonly properties and an index signature, which we all need to filter out. */
export type StylePropertiesBase = Partial<Pick<CSSStyleDeclaration, {
	[K in keyof CSSStyleDeclaration]: K extends string ? CSSStyleDeclaration[K] extends string ? K : never : never;
}[keyof CSSStyleDeclaration]>>;
/**
 * Style properties for the `style` JSX attribute, where each CSS property may
 * be a plain value or a signal-like value that updates reactively.
 */
export type StyleProperties = {
	[K in keyof StylePropertiesBase]: SignalOrValue<StylePropertiesBase[K]>;
};
type DOMElement = Element;
interface EventHandler<T, E extends Event> {
	(e: E & {
		currentTarget: T;
		target: DOMElement;
	}): void;
}
type EventHandlerUnion<T, E extends Event, EHandler extends EventHandler<T, any> = EventHandler<T, E>> = EHandler;
interface InputEventHandler<T, E extends InputEvent> {
	(e: E & {
		currentTarget: T;
		target: T extends HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement ? T : DOMElement;
	}): void;
}
type InputEventHandlerUnion<T, E extends InputEvent> = EventHandlerUnion<T, E, InputEventHandler<T, E>>;
interface ChangeEventHandler<T, E extends Event> {
	(e: E & {
		currentTarget: T;
		target: T extends HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement ? T : DOMElement;
	}): void;
}
type ChangeEventHandlerUnion<T, E extends Event> = EventHandlerUnion<T, E, ChangeEventHandler<T, E>>;
interface FocusEventHandler<T, E extends FocusEvent> {
	(e: E & {
		currentTarget: T;
		target: T extends HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement ? T : DOMElement;
	}): void;
}
type FocusEventHandlerUnion<T, E extends FocusEvent> = EventHandlerUnion<T, E, FocusEventHandler<T, E>>;
interface SerializableAttributeValue {
	toString(): string;
}
type Binding<T> = () => T;
interface Directives {
}
interface DirectiveFunctions {
	[x: string]: (el: DOMElement, accessor: Binding<any>) => void;
}
interface ExplicitProperties {
}
type DirectiveAttributes = {
	[Key in keyof Directives as `use:${Key}`]?: Directives[Key];
};
type DirectiveFunctionAttributes<T> = {
	[K in keyof DirectiveFunctions as string extends K ? never : `use:${K}`]?: DirectiveFunctions[K] extends (el: infer E, // will be unknown if not provided
	...rest: infer R // use rest so that we can check whether it's provided or not
	) => void ? T extends E // everything extends unknown if E is unknown
	 ? R extends [
		infer A
	] // check if has accessor provided
	 ? A extends Binding<infer V> ? V // it's an accessor
	 : never // it isn't, type error
	 : true // no accessor provided
	 : never // T is the wrong element
	 : never;
};
type PropAttributes = {
	[Key in keyof ExplicitProperties as `prop:${Key}`]?: ExplicitProperties[Key];
};
type OnAttributes<T> = {};
type CSSProperties = StyleProperties;
type BooleanAttribute = true | false | "";
type EnumeratedPseudoBoolean = "false" | "true";
type EnumeratedAcceptsEmpty = "" | true;
type RemoveAttribute = undefined | false;
interface AriaAttributes {
	/**
	 * Identifies the currently active element when DOM focus is on a composite widget, textbox,
	 * group, or application.
	 */
	"aria-activedescendant"?: PropValue<string | RemoveAttribute>;
	/**
	 * Indicates whether assistive technologies will present all, or only parts of, the changed
	 * region based on the change notifications defined by the aria-relevant attribute.
	 */
	"aria-atomic"?: PropValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Similar to the global aria-label. Defines a string value that labels the current element,
	 * which is intended to be converted into Braille.
	 *
	 * @see aria-label.
	 */
	"aria-braillelabel"?: PropValue<string | RemoveAttribute>;
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
	"aria-brailleroledescription"?: PropValue<string | RemoveAttribute>;
	/**
	 * Indicates whether inputting text could trigger display of one or more predictions of the
	 * user's intended value for an input and specifies how predictions would be presented if they
	 * are made.
	 */
	"aria-autocomplete"?: PropValue<"none" | "inline" | "list" | "both" | RemoveAttribute>;
	/**
	 * Indicates an element is being modified and that assistive technologies MAY want to wait until
	 * the modifications are complete before exposing them to the user.
	 */
	"aria-busy"?: PropValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Indicates the current "checked" state of checkboxes, radio buttons, and other widgets.
	 *
	 * @see aria-pressed @see aria-selected.
	 */
	"aria-checked"?: PropValue<EnumeratedPseudoBoolean | "mixed" | RemoveAttribute>;
	/**
	 * Defines the total number of columns in a table, grid, or treegrid.
	 *
	 * @see aria-colindex.
	 */
	"aria-colcount"?: PropValue<number | string | RemoveAttribute>;
	/**
	 * Defines an element's column index or position with respect to the total number of columns
	 * within a table, grid, or treegrid.
	 *
	 * @see aria-colcount @see aria-colspan.
	 */
	"aria-colindex"?: PropValue<number | string | RemoveAttribute>;
	/** Defines a human-readable text alternative of the numeric aria-colindex. */
	"aria-colindextext"?: PropValue<number | string | RemoveAttribute>;
	/**
	 * Defines the number of columns spanned by a cell or gridcell within a table, grid, or
	 * treegrid.
	 *
	 * @see aria-colindex @see aria-rowspan.
	 */
	"aria-colspan"?: PropValue<number | string | RemoveAttribute>;
	/**
	 * Identifies the element (or elements) whose contents or presence are controlled by the current
	 * element.
	 *
	 * @see aria-owns.
	 */
	"aria-controls"?: PropValue<string | RemoveAttribute>;
	/**
	 * Indicates the element that represents the current item within a container or set of related
	 * elements.
	 */
	"aria-current"?: PropValue<EnumeratedPseudoBoolean | "page" | "step" | "location" | "date" | "time" | RemoveAttribute>;
	/**
	 * Identifies the element (or elements) that describes the object.
	 *
	 * @see aria-labelledby
	 */
	"aria-describedby"?: PropValue<string | RemoveAttribute>;
	/**
	 * Defines a string value that describes or annotates the current element.
	 *
	 * @see aria-describedby
	 */
	"aria-description"?: PropValue<string | RemoveAttribute>;
	/**
	 * Identifies the element that provides a detailed, extended description for the object.
	 *
	 * @see aria-describedby.
	 */
	"aria-details"?: PropValue<string | RemoveAttribute>;
	/**
	 * Indicates that the element is perceivable but disabled, so it is not editable or otherwise
	 * operable.
	 *
	 * @see aria-hidden @see aria-readonly.
	 */
	"aria-disabled"?: PropValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Indicates what functions can be performed when a dragged object is released on the drop
	 * target.
	 *
	 * @deprecated In ARIA 1.1
	 */
	"aria-dropeffect"?: PropValue<"none" | "copy" | "execute" | "link" | "move" | "popup" | RemoveAttribute>;
	/**
	 * Identifies the element that provides an error message for the object.
	 *
	 * @see aria-invalid @see aria-describedby.
	 */
	"aria-errormessage"?: PropValue<string | RemoveAttribute>;
	/**
	 * Indicates whether the element, or another grouping element it controls, is currently expanded
	 * or collapsed.
	 */
	"aria-expanded"?: PropValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Identifies the next element (or elements) in an alternate reading order of content which, at
	 * the user's discretion, allows assistive technology to override the general default of reading
	 * in document source order.
	 */
	"aria-flowto"?: PropValue<string | RemoveAttribute>;
	/**
	 * Indicates an element's "grabbed" state in a drag-and-drop operation.
	 *
	 * @deprecated In ARIA 1.1
	 */
	"aria-grabbed"?: PropValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Indicates the availability and type of interactive popup element, such as menu or dialog,
	 * that can be triggered by an element.
	 */
	"aria-haspopup"?: PropValue<EnumeratedPseudoBoolean | "menu" | "listbox" | "tree" | "grid" | "dialog" | RemoveAttribute>;
	/**
	 * Indicates whether the element is exposed to an accessibility API.
	 *
	 * @see aria-disabled.
	 */
	"aria-hidden"?: PropValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Indicates the entered value does not conform to the format expected by the application.
	 *
	 * @see aria-errormessage.
	 */
	"aria-invalid"?: PropValue<EnumeratedPseudoBoolean | "grammar" | "spelling" | RemoveAttribute>;
	/**
	 * Indicates keyboard shortcuts that an author has implemented to activate or give focus to an
	 * element.
	 */
	"aria-keyshortcuts"?: PropValue<string | RemoveAttribute>;
	/**
	 * Defines a string value that labels the current element.
	 *
	 * @see aria-labelledby.
	 */
	"aria-label"?: PropValue<string | RemoveAttribute>;
	/**
	 * Identifies the element (or elements) that labels the current element.
	 *
	 * @see aria-describedby.
	 */
	"aria-labelledby"?: PropValue<string | RemoveAttribute>;
	/** Defines the hierarchical level of an element within a structure. */
	"aria-level"?: PropValue<number | string | RemoveAttribute>;
	/**
	 * Indicates that an element will be updated, and describes the types of updates the user
	 * agents, assistive technologies, and user can expect from the live region.
	 */
	"aria-live"?: PropValue<"off" | "assertive" | "polite" | RemoveAttribute>;
	/** Indicates whether an element is modal when displayed. */
	"aria-modal"?: PropValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/** Indicates whether a text box accepts multiple lines of input or only a single line. */
	"aria-multiline"?: PropValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Indicates that the user may select more than one item from the current selectable
	 * descendants.
	 */
	"aria-multiselectable"?: PropValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/** Indicates whether the element's orientation is horizontal, vertical, or unknown/ambiguous. */
	"aria-orientation"?: PropValue<"horizontal" | "vertical" | RemoveAttribute>;
	/**
	 * Identifies an element (or elements) in order to define a visual, functional, or contextual
	 * parent/child relationship between DOM elements where the DOM hierarchy cannot be used to
	 * represent the relationship.
	 *
	 * @see aria-controls.
	 */
	"aria-owns"?: PropValue<string | RemoveAttribute>;
	/**
	 * Defines a short hint (a word or short phrase) intended to aid the user with data entry when
	 * the control has no value. A hint could be a sample value or a brief description of the
	 * expected format.
	 */
	"aria-placeholder"?: PropValue<string | RemoveAttribute>;
	/**
	 * Defines an element's number or position in the current set of listitems or treeitems. Not
	 * required if all elements in the set are present in the DOM.
	 *
	 * @see aria-setsize.
	 */
	"aria-posinset"?: PropValue<number | string | RemoveAttribute>;
	/**
	 * Indicates the current "pressed" state of toggle buttons.
	 *
	 * @see aria-checked @see aria-selected.
	 */
	"aria-pressed"?: PropValue<EnumeratedPseudoBoolean | "mixed" | RemoveAttribute>;
	/**
	 * Indicates that the element is not editable, but is otherwise operable.
	 *
	 * @see aria-disabled.
	 */
	"aria-readonly"?: PropValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Indicates what notifications the user agent will trigger when the accessibility tree within a
	 * live region is modified.
	 *
	 * @see aria-atomic.
	 */
	"aria-relevant"?: PropValue<"additions" | "additions removals" | "additions text" | "all" | "removals" | "removals additions" | "removals text" | "text" | "text additions" | "text removals" | RemoveAttribute>;
	/** Indicates that user input is required on the element before a form may be submitted. */
	"aria-required"?: PropValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/** Defines a human-readable, author-localized description for the role of an element. */
	"aria-roledescription"?: PropValue<string | RemoveAttribute>;
	/**
	 * Defines the total number of rows in a table, grid, or treegrid.
	 *
	 * @see aria-rowindex.
	 */
	"aria-rowcount"?: PropValue<number | string | RemoveAttribute>;
	/**
	 * Defines an element's row index or position with respect to the total number of rows within a
	 * table, grid, or treegrid.
	 *
	 * @see aria-rowcount @see aria-rowspan.
	 */
	"aria-rowindex"?: PropValue<number | string | RemoveAttribute>;
	/** Defines a human-readable text alternative of aria-rowindex. */
	"aria-rowindextext"?: PropValue<number | string | RemoveAttribute>;
	/**
	 * Defines the number of rows spanned by a cell or gridcell within a table, grid, or treegrid.
	 *
	 * @see aria-rowindex @see aria-colspan.
	 */
	"aria-rowspan"?: PropValue<number | string | RemoveAttribute>;
	/**
	 * Indicates the current "selected" state of various widgets.
	 *
	 * @see aria-checked @see aria-pressed.
	 */
	"aria-selected"?: PropValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	/**
	 * Defines the number of items in the current set of listitems or treeitems. Not required if all
	 * elements in the set are present in the DOM.
	 *
	 * @see aria-posinset.
	 */
	"aria-setsize"?: PropValue<number | string | RemoveAttribute>;
	/** Indicates if items in a table or grid are sorted in ascending or descending order. */
	"aria-sort"?: PropValue<"none" | "ascending" | "descending" | "other" | RemoveAttribute>;
	/** Defines the maximum allowed value for a range widget. */
	"aria-valuemax"?: PropValue<number | string | RemoveAttribute>;
	/** Defines the minimum allowed value for a range widget. */
	"aria-valuemin"?: PropValue<number | string | RemoveAttribute>;
	/**
	 * Defines the current value for a range widget.
	 *
	 * @see aria-valuetext.
	 */
	"aria-valuenow"?: PropValue<number | string | RemoveAttribute>;
	/** Defines the human readable text alternative of aria-valuenow for a range widget. */
	"aria-valuetext"?: PropValue<string | RemoveAttribute>;
	role?: PropValue<"alert" | "alertdialog" | "application" | "article" | "banner" | "button" | "cell" | "checkbox" | "columnheader" | "combobox" | "complementary" | "contentinfo" | "definition" | "dialog" | "directory" | "document" | "feed" | "figure" | "form" | "grid" | "gridcell" | "group" | "heading" | "img" | "link" | "list" | "listbox" | "listitem" | "log" | "main" | "marquee" | "math" | "menu" | "menubar" | "menuitem" | "menuitemcheckbox" | "menuitemradio" | "meter" | "navigation" | "none" | "note" | "option" | "presentation" | "progressbar" | "radio" | "radiogroup" | "region" | "row" | "rowgroup" | "rowheader" | "scrollbar" | "search" | "searchbox" | "separator" | "slider" | "spinbutton" | "status" | "switch" | "tab" | "table" | "tablist" | "tabpanel" | "term" | "textbox" | "timer" | "toolbar" | "tooltip" | "tree" | "treegrid" | "treeitem" | RemoveAttribute>;
}
interface EventHandlersWindow<T> {
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
interface EventHandlersElement<T> {
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
	onScrollSnapChange?: EventHandlerUnion<T, Event> | undefined;
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
	textContent?: PropValue<string | number>;
	// attributes
	autofocus?: PropValue<BooleanAttribute | RemoveAttribute>;
	class?: PropValue<string | ClassNames | RemoveAttribute> | {
		[key: string]: PropValue<boolean | RemoveAttribute>;
	};
	elementtiming?: PropValue<string | RemoveAttribute>;
	id?: PropValue<string | RemoveAttribute>;
	nonce?: PropValue<string | RemoveAttribute>;
	part?: PropValue<string | RemoveAttribute>;
	slot?: PropValue<string | RemoveAttribute>;
	style?: PropValue<CSSProperties | string | RemoveAttribute>;
	tabindex?: PropValue<number | string | RemoveAttribute>;
}
interface SVGAttributes<T> extends ElementAttributes<T> {
	id?: PropValue<string | RemoveAttribute>;
	lang?: PropValue<string | RemoveAttribute>;
	tabindex?: PropValue<number | string | RemoveAttribute>;
	xmlns?: PropValue<string | RemoveAttribute>;
}
interface MathMLAttributes<T> extends ElementAttributes<T> {
	dir?: PropValue<HTMLDir | RemoveAttribute>;
	displaystyle?: PropValue<BooleanAttribute | RemoveAttribute>;
	scriptlevel?: PropValue<string | RemoveAttribute>;
	xmlns?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	href?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	mathbackground?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	mathcolor?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	mathsize?: PropValue<string | RemoveAttribute>;
}
interface HTMLAttributes<T> extends ElementAttributes<T> {
	// properties
	innerText?: PropValue<string | number>;
	// attributes
	accesskey?: PropValue<string | RemoveAttribute>;
	autocapitalize?: PropValue<HTMLAutocapitalize | RemoveAttribute>;
	autocorrect?: PropValue<"on" | "off" | RemoveAttribute>;
	contenteditable?: PropValue<EnumeratedPseudoBoolean | EnumeratedAcceptsEmpty | "plaintext-only" | "inherit" | RemoveAttribute>;
	dir?: PropValue<HTMLDir | RemoveAttribute>;
	draggable?: PropValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	enterkeyhint?: PropValue<"enter" | "done" | "go" | "next" | "previous" | "search" | "send" | RemoveAttribute>;
	exportparts?: PropValue<string | RemoveAttribute>;
	hidden?: PropValue<EnumeratedAcceptsEmpty | "hidden" | "until-found" | RemoveAttribute>;
	inert?: PropValue<BooleanAttribute | RemoveAttribute>;
	inputmode?: PropValue<"decimal" | "email" | "none" | "numeric" | "search" | "tel" | "text" | "url" | "value" | RemoveAttribute>;
	is?: PropValue<string | RemoveAttribute>;
	lang?: PropValue<string | RemoveAttribute>;
	popover?: PropValue<EnumeratedAcceptsEmpty | "manual" | "auto" | RemoveAttribute>;
	spellcheck?: PropValue<EnumeratedPseudoBoolean | EnumeratedAcceptsEmpty | RemoveAttribute>;
	title?: PropValue<string | RemoveAttribute>;
	translate?: PropValue<"yes" | "no" | RemoveAttribute>;
	/** @experimental */
	virtualkeyboardpolicy?: PropValue<EnumeratedAcceptsEmpty | "auto" | "manual" | RemoveAttribute>;
	/** @experimental */
	writingsuggestions?: PropValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	// Microdata
	itemid?: PropValue<string | RemoveAttribute>;
	itemprop?: PropValue<string | RemoveAttribute>;
	itemref?: PropValue<string | RemoveAttribute>;
	itemscope?: PropValue<BooleanAttribute | RemoveAttribute>;
	itemtype?: PropValue<string | RemoveAttribute>;
	// RDFa Attributes
	about?: PropValue<string | RemoveAttribute>;
	datatype?: PropValue<string | RemoveAttribute>;
	inlist?: PropValue<any | RemoveAttribute>;
	prefix?: PropValue<string | RemoveAttribute>;
	property?: PropValue<string | RemoveAttribute>;
	resource?: PropValue<string | RemoveAttribute>;
	typeof?: PropValue<string | RemoveAttribute>;
	vocab?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	contextmenu?: PropValue<string | RemoveAttribute>;
}
type HTMLAutocapitalize = "off" | "none" | "on" | "sentences" | "words" | "characters";
type HTMLAutocomplete = "additional-name" | "address-level1" | "address-level2" | "address-level3" | "address-level4" | "address-line1" | "address-line2" | "address-line3" | "bday" | "bday-day" | "bday-month" | "bday-year" | "billing" | "cc-additional-name" | "cc-csc" | "cc-exp" | "cc-exp-month" | "cc-exp-year" | "cc-family-name" | "cc-given-name" | "cc-name" | "cc-number" | "cc-type" | "country" | "country-name" | "current-password" | "email" | "family-name" | "fax" | "given-name" | "home" | "honorific-prefix" | "honorific-suffix" | "impp" | "language" | "mobile" | "name" | "new-password" | "nickname" | "off" | "on" | "organization" | "organization-title" | "pager" | "photo" | "postal-code" | "sex" | "shipping" | "street-address" | "tel" | "tel-area-code" | "tel-country-code" | "tel-extension" | "tel-local" | "tel-local-prefix" | "tel-local-suffix" | "tel-national" | "transaction-amount" | "transaction-currency" | "url" | "username" | "work" | (string & {});
type HTMLDir = "ltr" | "rtl" | "auto";
type HTMLFormEncType = "application/x-www-form-urlencoded" | "multipart/form-data" | "text/plain";
type HTMLFormMethod = "post" | "get" | "dialog";
type HTMLCrossorigin = "anonymous" | "use-credentials" | EnumeratedAcceptsEmpty;
type HTMLReferrerPolicy = "no-referrer" | "no-referrer-when-downgrade" | "origin" | "origin-when-cross-origin" | "same-origin" | "strict-origin" | "strict-origin-when-cross-origin" | "unsafe-url";
type HTMLIframeSandbox = "allow-downloads-without-user-activation" | "allow-downloads" | "allow-forms" | "allow-modals" | "allow-orientation-lock" | "allow-pointer-lock" | "allow-popups" | "allow-popups-to-escape-sandbox" | "allow-presentation" | "allow-same-origin" | "allow-scripts" | "allow-storage-access-by-user-activation" | "allow-top-navigation" | "allow-top-navigation-by-user-activation" | "allow-top-navigation-to-custom-protocols";
type HTMLLinkAs = "audio" | "document" | "embed" | "fetch" | "font" | "image" | "object" | "script" | "style" | "track" | "video" | "worker";
interface AnchorHTMLAttributes<T> extends HTMLAttributes<T> {
	download?: PropValue<string | true | RemoveAttribute>;
	href?: PropValue<string | RemoveAttribute>;
	hreflang?: PropValue<string | RemoveAttribute>;
	ping?: PropValue<string | RemoveAttribute>;
	referrerpolicy?: PropValue<HTMLReferrerPolicy | RemoveAttribute>;
	rel?: PropValue<string | RemoveAttribute>;
	target?: PropValue<"_self" | "_blank" | "_parent" | "_top" | (string & {}) | RemoveAttribute>;
	type?: PropValue<string | RemoveAttribute>;
	/** @experimental */
	attributionsrc?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	charset?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	coords?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	name?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	rev?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	shape?: PropValue<"rect" | "circle" | "poly" | "default" | RemoveAttribute>;
}
interface AudioHTMLAttributes<T> extends MediaHTMLAttributes<T> {
}
interface AreaHTMLAttributes<T> extends HTMLAttributes<T> {
	alt?: PropValue<string | RemoveAttribute>;
	coords?: PropValue<string | RemoveAttribute>;
	download?: PropValue<string | RemoveAttribute>;
	href?: PropValue<string | RemoveAttribute>;
	ping?: PropValue<string | RemoveAttribute>;
	referrerpolicy?: PropValue<HTMLReferrerPolicy | RemoveAttribute>;
	rel?: PropValue<string | RemoveAttribute>;
	shape?: PropValue<"rect" | "circle" | "poly" | "default" | RemoveAttribute>;
	target?: PropValue<"_self" | "_blank" | "_parent" | "_top" | (string & {}) | RemoveAttribute>;
	/** @experimental */
	attributionsrc?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	nohref?: PropValue<BooleanAttribute | RemoveAttribute>;
}
interface BaseHTMLAttributes<T> extends HTMLAttributes<T> {
	href?: PropValue<string | RemoveAttribute>;
	target?: PropValue<"_self" | "_blank" | "_parent" | "_top" | (string & {}) | RemoveAttribute>;
}
interface BdoHTMLAttributes<T> extends HTMLAttributes<T> {
	dir?: PropValue<"ltr" | "rtl" | RemoveAttribute>;
}
interface BlockquoteHTMLAttributes<T> extends HTMLAttributes<T> {
	cite?: PropValue<string | RemoveAttribute>;
}
interface BodyHTMLAttributes<T> extends HTMLAttributes<T>, EventHandlersWindow<T> {
}
interface ButtonHTMLAttributes<T> extends HTMLAttributes<T> {
	disabled?: PropValue<BooleanAttribute | RemoveAttribute>;
	form?: PropValue<string | RemoveAttribute>;
	formaction?: PropValue<string | SerializableAttributeValue | RemoveAttribute>;
	formenctype?: PropValue<HTMLFormEncType | RemoveAttribute>;
	formmethod?: PropValue<HTMLFormMethod | RemoveAttribute>;
	formnovalidate?: PropValue<BooleanAttribute | RemoveAttribute>;
	formtarget?: PropValue<"_self" | "_blank" | "_parent" | "_top" | (string & {}) | RemoveAttribute>;
	name?: PropValue<string | RemoveAttribute>;
	popovertarget?: PropValue<string | RemoveAttribute>;
	popovertargetaction?: PropValue<"hide" | "show" | "toggle" | RemoveAttribute>;
	type?: PropValue<"submit" | "reset" | "button" | "menu" | RemoveAttribute>;
	value?: PropValue<string | RemoveAttribute>;
	/** @experimental */
	command?: PropValue<"show-modal" | "close" | "show-popover" | "hide-popover" | "toggle-popover" | (string & {}) | RemoveAttribute>;
	/** @experimental */
	commandfor?: PropValue<string | RemoveAttribute>;
}
interface CanvasHTMLAttributes<T> extends HTMLAttributes<T> {
	height?: PropValue<number | string | RemoveAttribute>;
	width?: PropValue<number | string | RemoveAttribute>;
	/** @deprecated @non-standard */
	"moz-opaque"?: PropValue<BooleanAttribute | RemoveAttribute>;
}
interface CaptionHTMLAttributes<T> extends HTMLAttributes<T> {
	/** @deprecated */
	align?: PropValue<"left" | "center" | "right" | RemoveAttribute>;
}
interface ColHTMLAttributes<T> extends HTMLAttributes<T> {
	span?: PropValue<number | string | RemoveAttribute>;
	/** @deprecated */
	align?: PropValue<"left" | "center" | "right" | "justify" | "char" | RemoveAttribute>;
	/** @deprecated */
	bgcolor?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	char?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	charoff?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	valign?: PropValue<"baseline" | "bottom" | "middle" | "top" | RemoveAttribute>;
	/** @deprecated */
	width?: PropValue<number | string | RemoveAttribute>;
}
interface ColgroupHTMLAttributes<T> extends HTMLAttributes<T> {
	span?: PropValue<number | string | RemoveAttribute>;
	/** @deprecated */
	align?: PropValue<"left" | "center" | "right" | "justify" | "char" | RemoveAttribute>;
	/** @deprecated */
	bgcolor?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	char?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	charoff?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	valign?: PropValue<"baseline" | "bottom" | "middle" | "top" | RemoveAttribute>;
	/** @deprecated */
	width?: PropValue<number | string | RemoveAttribute>;
}
interface DataHTMLAttributes<T> extends HTMLAttributes<T> {
	value?: PropValue<string | string[] | number | RemoveAttribute>;
}
interface DetailsHtmlAttributes<T> extends HTMLAttributes<T> {
	name?: PropValue<string | RemoveAttribute>;
	open?: PropValue<BooleanAttribute | RemoveAttribute>;
}
interface DialogHtmlAttributes<T> extends HTMLAttributes<T> {
	open?: PropValue<BooleanAttribute | RemoveAttribute>;
	/**
	 * Do not add the `tabindex` property to the `<dialog>` element as it is not interactive and
	 * does not receive focus. The dialog's contents, including the close button contained in the
	 * dialog, can receive focus and be interactive.
	 *
	 * @see https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/dialog#usage_notes
	 */
	tabindex?: never;
	/** @experimental */
	closedby?: PropValue<"any" | "closerequest" | "none" | RemoveAttribute>;
}
interface EmbedHTMLAttributes<T> extends HTMLAttributes<T> {
	height?: PropValue<number | string | RemoveAttribute>;
	src?: PropValue<string | RemoveAttribute>;
	type?: PropValue<string | RemoveAttribute>;
	width?: PropValue<number | string | RemoveAttribute>;
	/** @deprecated */
	align?: PropValue<"left" | "right" | "justify" | "center" | RemoveAttribute>;
	/** @deprecated */
	name?: PropValue<string | RemoveAttribute>;
}
interface FieldsetHTMLAttributes<T> extends HTMLAttributes<T> {
	disabled?: PropValue<BooleanAttribute | RemoveAttribute>;
	form?: PropValue<string | RemoveAttribute>;
	name?: PropValue<string | RemoveAttribute>;
}
interface FormHTMLAttributes<T> extends HTMLAttributes<T> {
	"accept-charset"?: PropValue<string | RemoveAttribute>;
	action?: PropValue<string | SerializableAttributeValue | RemoveAttribute>;
	autocomplete?: PropValue<"on" | "off" | RemoveAttribute>;
	encoding?: PropValue<HTMLFormEncType | RemoveAttribute>;
	enctype?: PropValue<HTMLFormEncType | RemoveAttribute>;
	method?: PropValue<HTMLFormMethod | RemoveAttribute>;
	name?: PropValue<string | RemoveAttribute>;
	novalidate?: PropValue<BooleanAttribute | RemoveAttribute>;
	rel?: PropValue<string | RemoveAttribute>;
	target?: PropValue<"_self" | "_blank" | "_parent" | "_top" | (string & {}) | RemoveAttribute>;
	/** @deprecated */
	accept?: PropValue<string | RemoveAttribute>;
}
interface IframeHTMLAttributes<T> extends HTMLAttributes<T> {
	allow?: PropValue<string | RemoveAttribute>;
	allowfullscreen?: PropValue<BooleanAttribute | RemoveAttribute>;
	height?: PropValue<number | string | RemoveAttribute>;
	loading?: PropValue<"eager" | "lazy" | RemoveAttribute>;
	name?: PropValue<string | RemoveAttribute>;
	referrerpolicy?: PropValue<HTMLReferrerPolicy | RemoveAttribute>;
	sandbox?: PropValue<HTMLIframeSandbox | string | RemoveAttribute>;
	src?: PropValue<string | RemoveAttribute>;
	srcdoc?: PropValue<string | RemoveAttribute>;
	width?: PropValue<number | string | RemoveAttribute>;
	/** @experimental */
	adauctionheaders?: PropValue<BooleanAttribute | RemoveAttribute>;
	/**
	 * @non-standard
	 * @experimental
	 */
	browsingtopics?: PropValue<BooleanAttribute | RemoveAttribute>;
	/** @experimental */
	credentialless?: PropValue<BooleanAttribute | RemoveAttribute>;
	/** @experimental */
	csp?: PropValue<string | RemoveAttribute>;
	/** @experimental */
	privatetoken?: PropValue<string | RemoveAttribute>;
	/** @experimental */
	sharedstoragewritable?: PropValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	align?: PropValue<string | RemoveAttribute>;
	/** @deprecated @non-standard */
	allowpaymentrequest?: PropValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	allowtransparency?: PropValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	frameborder?: PropValue<number | string | RemoveAttribute>;
	/** @deprecated */
	longdesc?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	marginheight?: PropValue<number | string | RemoveAttribute>;
	/** @deprecated */
	marginwidth?: PropValue<number | string | RemoveAttribute>;
	/** @deprecated */
	scrolling?: PropValue<"yes" | "no" | "auto" | RemoveAttribute>;
	/** @deprecated */
	seamless?: PropValue<BooleanAttribute | RemoveAttribute>;
}
interface ImgHTMLAttributes<T> extends HTMLAttributes<T> {
	alt?: PropValue<string | RemoveAttribute>;
	browsingtopics?: PropValue<string | RemoveAttribute>;
	crossorigin?: PropValue<HTMLCrossorigin | RemoveAttribute>;
	decoding?: PropValue<"sync" | "async" | "auto" | RemoveAttribute>;
	fetchpriority?: PropValue<"high" | "low" | "auto" | RemoveAttribute>;
	height?: PropValue<number | string | RemoveAttribute>;
	ismap?: PropValue<BooleanAttribute | RemoveAttribute>;
	loading?: PropValue<"eager" | "lazy" | RemoveAttribute>;
	referrerpolicy?: PropValue<HTMLReferrerPolicy | RemoveAttribute>;
	sizes?: PropValue<string | RemoveAttribute>;
	src?: PropValue<string | RemoveAttribute>;
	srcset?: PropValue<string | RemoveAttribute>;
	usemap?: PropValue<string | RemoveAttribute>;
	width?: PropValue<number | string | RemoveAttribute>;
	/** @experimental */
	attributionsrc?: PropValue<string | RemoveAttribute>;
	/** @experimental */
	sharedstoragewritable?: PropValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	align?: PropValue<"top" | "middle" | "bottom" | "left" | "right" | RemoveAttribute>;
	/** @deprecated */
	border?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	hspace?: PropValue<number | string | RemoveAttribute>;
	/** @deprecated */
	intrinsicsize?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	longdesc?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	lowsrc?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	name?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	vspace?: PropValue<number | string | RemoveAttribute>;
}
interface InputHTMLAttributes<T> extends HTMLAttributes<T> {
	accept?: PropValue<string | RemoveAttribute>;
	alpha?: PropValue<BooleanAttribute | RemoveAttribute>;
	alt?: PropValue<string | RemoveAttribute>;
	autocomplete?: PropValue<HTMLAutocomplete | RemoveAttribute>;
	capture?: PropValue<"user" | "environment" | RemoveAttribute>;
	checked?: PropValue<BooleanAttribute | RemoveAttribute>;
	colorspace?: PropValue<string | RemoveAttribute>;
	dirname?: PropValue<string | RemoveAttribute>;
	disabled?: PropValue<BooleanAttribute | RemoveAttribute>;
	form?: PropValue<string | RemoveAttribute>;
	formaction?: PropValue<string | SerializableAttributeValue | RemoveAttribute>;
	formenctype?: PropValue<HTMLFormEncType | RemoveAttribute>;
	formmethod?: PropValue<HTMLFormMethod | RemoveAttribute>;
	formnovalidate?: PropValue<BooleanAttribute | RemoveAttribute>;
	formtarget?: PropValue<string | RemoveAttribute>;
	height?: PropValue<number | string | RemoveAttribute>;
	list?: PropValue<string | RemoveAttribute>;
	max?: PropValue<number | string | RemoveAttribute>;
	maxlength?: PropValue<number | string | RemoveAttribute>;
	min?: PropValue<number | string | RemoveAttribute>;
	minlength?: PropValue<number | string | RemoveAttribute>;
	multiple?: PropValue<BooleanAttribute | RemoveAttribute>;
	name?: PropValue<string | RemoveAttribute>;
	pattern?: PropValue<string | RemoveAttribute>;
	placeholder?: PropValue<string | RemoveAttribute>;
	popovertarget?: PropValue<string | RemoveAttribute>;
	popovertargetaction?: PropValue<"hide" | "show" | "toggle" | RemoveAttribute>;
	readonly?: PropValue<BooleanAttribute | RemoveAttribute>;
	required?: PropValue<BooleanAttribute | RemoveAttribute>;
	// https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/search#results
	results?: PropValue<number | RemoveAttribute>;
	size?: PropValue<number | string | RemoveAttribute>;
	src?: PropValue<string | RemoveAttribute>;
	step?: PropValue<number | string | RemoveAttribute>;
	type?: PropValue<"button" | "checkbox" | "color" | "date" | "datetime-local" | "email" | "file" | "hidden" | "image" | "month" | "number" | "password" | "radio" | "range" | "reset" | "search" | "submit" | "tel" | "text" | "time" | "url" | "week" | (string & {}) | RemoveAttribute>;
	value?: PropValue<string | string[] | number | RemoveAttribute>;
	width?: PropValue<number | string | RemoveAttribute>;
	/** @non-standard */
	incremental?: PropValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	align?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	usemap?: PropValue<string | RemoveAttribute>;
}
interface ModHTMLAttributes<T> extends HTMLAttributes<T> {
	cite?: PropValue<string | RemoveAttribute>;
	datetime?: PropValue<string | RemoveAttribute>;
}
interface LabelHTMLAttributes<T> extends HTMLAttributes<T> {
	for?: PropValue<string | RemoveAttribute>;
}
interface LiHTMLAttributes<T> extends HTMLAttributes<T> {
	value?: PropValue<number | string | RemoveAttribute>;
	/** @deprecated */
	type?: PropValue<"1" | "a" | "A" | "i" | "I" | RemoveAttribute>;
}
interface LinkHTMLAttributes<T> extends HTMLAttributes<T> {
	as?: PropValue<HTMLLinkAs | RemoveAttribute>;
	blocking?: PropValue<"render" | RemoveAttribute>;
	color?: PropValue<string | RemoveAttribute>;
	crossorigin?: PropValue<HTMLCrossorigin | RemoveAttribute>;
	disabled?: PropValue<BooleanAttribute | RemoveAttribute>;
	fetchpriority?: PropValue<"high" | "low" | "auto" | RemoveAttribute>;
	href?: PropValue<string | RemoveAttribute>;
	hreflang?: PropValue<string | RemoveAttribute>;
	imagesizes?: PropValue<string | RemoveAttribute>;
	imagesrcset?: PropValue<string | RemoveAttribute>;
	integrity?: PropValue<string | RemoveAttribute>;
	media?: PropValue<string | RemoveAttribute>;
	referrerpolicy?: PropValue<HTMLReferrerPolicy | RemoveAttribute>;
	rel?: PropValue<string | RemoveAttribute>;
	sizes?: PropValue<string | RemoveAttribute>;
	type?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	charset?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	rev?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	target?: PropValue<string | RemoveAttribute>;
}
interface MapHTMLAttributes<T> extends HTMLAttributes<T> {
	name?: PropValue<string | RemoveAttribute>;
}
interface MediaHTMLAttributes<T> extends HTMLAttributes<T> {
	autoplay?: PropValue<BooleanAttribute | RemoveAttribute>;
	controls?: PropValue<BooleanAttribute | RemoveAttribute>;
	controlslist?: PropValue<"nodownload" | "nofullscreen" | "noplaybackrate" | "noremoteplayback" | (string & {}) | RemoveAttribute>;
	crossorigin?: PropValue<HTMLCrossorigin | RemoveAttribute>;
	disableremoteplayback?: PropValue<BooleanAttribute | RemoveAttribute>;
	loop?: PropValue<BooleanAttribute | RemoveAttribute>;
	muted?: PropValue<BooleanAttribute | RemoveAttribute>;
	preload?: PropValue<"none" | "metadata" | "auto" | EnumeratedAcceptsEmpty | RemoveAttribute>;
	src?: PropValue<string | RemoveAttribute>;
	onEncrypted?: EventHandlerUnion<T, MediaEncryptedEvent> | undefined;
	// "on:encrypted"?: EventHandlerWithOptionsUnion<T, MediaEncryptedEvent> | undefined;
	onWaitingForKey?: EventHandlerUnion<T, Event> | undefined;
	// "on:waitingforkey"?: EventHandlerWithOptionsUnion<T, Event> | undefined;
	/** @deprecated */
	mediagroup?: PropValue<string | RemoveAttribute>;
}
interface MenuHTMLAttributes<T> extends HTMLAttributes<T> {
	/** @deprecated */
	compact?: PropValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	label?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	type?: PropValue<"context" | "toolbar" | RemoveAttribute>;
}
interface MetaHTMLAttributes<T> extends HTMLAttributes<T> {
	"http-equiv"?: PropValue<"content-security-policy" | "content-type" | "default-style" | "x-ua-compatible" | "refresh" | RemoveAttribute>;
	charset?: PropValue<string | RemoveAttribute>;
	content?: PropValue<string | RemoveAttribute>;
	media?: PropValue<string | RemoveAttribute>;
	name?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	scheme?: PropValue<string | RemoveAttribute>;
}
interface MeterHTMLAttributes<T> extends HTMLAttributes<T> {
	form?: PropValue<string | RemoveAttribute>;
	high?: PropValue<number | string | RemoveAttribute>;
	low?: PropValue<number | string | RemoveAttribute>;
	max?: PropValue<number | string | RemoveAttribute>;
	min?: PropValue<number | string | RemoveAttribute>;
	optimum?: PropValue<number | string | RemoveAttribute>;
	value?: PropValue<string | string[] | number | RemoveAttribute>;
}
interface QuoteHTMLAttributes<T> extends HTMLAttributes<T> {
	cite?: PropValue<string | RemoveAttribute>;
}
interface ObjectHTMLAttributes<T> extends HTMLAttributes<T> {
	data?: PropValue<string | RemoveAttribute>;
	form?: PropValue<string | RemoveAttribute>;
	height?: PropValue<number | string | RemoveAttribute>;
	name?: PropValue<string | RemoveAttribute>;
	type?: PropValue<string | RemoveAttribute>;
	width?: PropValue<number | string | RemoveAttribute>;
	wmode?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	align?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	archive?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	border?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	classid?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	code?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	codebase?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	codetype?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	declare?: PropValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	hspace?: PropValue<number | string | RemoveAttribute>;
	/** @deprecated */
	standby?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	usemap?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	vspace?: PropValue<number | string | RemoveAttribute>;
	/** @deprecated */
	typemustmatch?: PropValue<BooleanAttribute | RemoveAttribute>;
}
interface OlHTMLAttributes<T> extends HTMLAttributes<T> {
	reversed?: PropValue<BooleanAttribute | RemoveAttribute>;
	start?: PropValue<number | string | RemoveAttribute>;
	type?: PropValue<"1" | "a" | "A" | "i" | "I" | RemoveAttribute>;
	/** @deprecated @non-standard */
	compact?: PropValue<BooleanAttribute | RemoveAttribute>;
}
interface OptgroupHTMLAttributes<T> extends HTMLAttributes<T> {
	disabled?: PropValue<BooleanAttribute | RemoveAttribute>;
	label?: PropValue<string | RemoveAttribute>;
}
interface OptionHTMLAttributes<T> extends HTMLAttributes<T> {
	disabled?: PropValue<BooleanAttribute | RemoveAttribute>;
	label?: PropValue<string | RemoveAttribute>;
	selected?: PropValue<BooleanAttribute | RemoveAttribute>;
	value?: PropValue<string | string[] | number | RemoveAttribute>;
}
interface OutputHTMLAttributes<T> extends HTMLAttributes<T> {
	for?: PropValue<string | RemoveAttribute>;
	form?: PropValue<string | RemoveAttribute>;
	name?: PropValue<string | RemoveAttribute>;
}
interface ProgressHTMLAttributes<T> extends HTMLAttributes<T> {
	max?: PropValue<number | string | RemoveAttribute>;
	value?: PropValue<string | string[] | number | RemoveAttribute>;
}
interface ScriptHTMLAttributes<T> extends HTMLAttributes<T> {
	async?: PropValue<BooleanAttribute | RemoveAttribute>;
	blocking?: PropValue<"render" | RemoveAttribute>;
	crossorigin?: PropValue<HTMLCrossorigin | RemoveAttribute>;
	defer?: PropValue<BooleanAttribute | RemoveAttribute>;
	fetchpriority?: PropValue<"high" | "low" | "auto" | RemoveAttribute>;
	for?: PropValue<string | RemoveAttribute>;
	integrity?: PropValue<string | RemoveAttribute>;
	nomodule?: PropValue<BooleanAttribute | RemoveAttribute>;
	referrerpolicy?: PropValue<HTMLReferrerPolicy | RemoveAttribute>;
	src?: PropValue<string | RemoveAttribute>;
	type?: PropValue<"importmap" | "module" | "speculationrules" | (string & {}) | RemoveAttribute>;
	/** @experimental */
	attributionsrc?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	charset?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	event?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	language?: PropValue<string | RemoveAttribute>;
}
interface SelectHTMLAttributes<T> extends HTMLAttributes<T> {
	autocomplete?: PropValue<HTMLAutocomplete | RemoveAttribute>;
	disabled?: PropValue<BooleanAttribute | RemoveAttribute>;
	form?: PropValue<string | RemoveAttribute>;
	multiple?: PropValue<BooleanAttribute | RemoveAttribute>;
	name?: PropValue<string | RemoveAttribute>;
	required?: PropValue<BooleanAttribute | RemoveAttribute>;
	size?: PropValue<number | string | RemoveAttribute>;
	value?: PropValue<string | string[] | number | RemoveAttribute>;
}
interface HTMLSlotElementAttributes<T> extends HTMLAttributes<T> {
	name?: PropValue<string | RemoveAttribute>;
}
interface SourceHTMLAttributes<T> extends HTMLAttributes<T> {
	height?: PropValue<number | string | RemoveAttribute>;
	media?: PropValue<string | RemoveAttribute>;
	sizes?: PropValue<string | RemoveAttribute>;
	src?: PropValue<string | RemoveAttribute>;
	srcset?: PropValue<string | RemoveAttribute>;
	type?: PropValue<string | RemoveAttribute>;
	width?: PropValue<number | string | RemoveAttribute>;
}
interface StyleHTMLAttributes<T> extends HTMLAttributes<T> {
	blocking?: PropValue<"render" | RemoveAttribute>;
	media?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	scoped?: PropValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	type?: PropValue<string | RemoveAttribute>;
}
interface TdHTMLAttributes<T> extends HTMLAttributes<T> {
	colspan?: PropValue<number | string | RemoveAttribute>;
	headers?: PropValue<string | RemoveAttribute>;
	rowspan?: PropValue<number | string | RemoveAttribute>;
	/** @deprecated */
	abbr?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	align?: PropValue<"left" | "center" | "right" | "justify" | "char" | RemoveAttribute>;
	/** @deprecated */
	axis?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	bgcolor?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	char?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	charoff?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	height?: PropValue<number | string | RemoveAttribute>;
	/** @deprecated */
	nowrap?: PropValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	scope?: PropValue<"col" | "row" | "rowgroup" | "colgroup" | RemoveAttribute>;
	/** @deprecated */
	valign?: PropValue<"baseline" | "bottom" | "middle" | "top" | RemoveAttribute>;
	/** @deprecated */
	width?: PropValue<number | string | RemoveAttribute>;
}
interface TemplateHTMLAttributes<T> extends HTMLAttributes<T> {
	shadowrootclonable?: PropValue<BooleanAttribute | RemoveAttribute>;
	shadowrootdelegatesfocus?: PropValue<BooleanAttribute | RemoveAttribute>;
	shadowrootmode?: PropValue<"open" | "closed" | RemoveAttribute>;
	shadowrootcustomelementregistry?: PropValue<BooleanAttribute | RemoveAttribute>;
	/** @experimental */
	shadowrootserializable?: PropValue<BooleanAttribute | RemoveAttribute>;
}
interface TextareaHTMLAttributes<T> extends HTMLAttributes<T> {
	autocomplete?: PropValue<HTMLAutocomplete | RemoveAttribute>;
	cols?: PropValue<number | string | RemoveAttribute>;
	dirname?: PropValue<string | RemoveAttribute>;
	disabled?: PropValue<BooleanAttribute | RemoveAttribute>;
	form?: PropValue<string | RemoveAttribute>;
	maxlength?: PropValue<number | string | RemoveAttribute>;
	minlength?: PropValue<number | string | RemoveAttribute>;
	name?: PropValue<string | RemoveAttribute>;
	placeholder?: PropValue<string | RemoveAttribute>;
	readonly?: PropValue<BooleanAttribute | RemoveAttribute>;
	required?: PropValue<BooleanAttribute | RemoveAttribute>;
	rows?: PropValue<number | string | RemoveAttribute>;
	value?: PropValue<string | string[] | number | RemoveAttribute>;
	wrap?: PropValue<"hard" | "soft" | "off" | RemoveAttribute>;
}
interface ThHTMLAttributes<T> extends HTMLAttributes<T> {
	abbr?: PropValue<string | RemoveAttribute>;
	colspan?: PropValue<number | string | RemoveAttribute>;
	headers?: PropValue<string | RemoveAttribute>;
	rowspan?: PropValue<number | string | RemoveAttribute>;
	scope?: PropValue<"col" | "row" | "rowgroup" | "colgroup" | RemoveAttribute>;
	/** @deprecated */
	align?: PropValue<"left" | "center" | "right" | "justify" | "char" | RemoveAttribute>;
	/** @deprecated */
	axis?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	bgcolor?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	char?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	charoff?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	height?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	nowrap?: PropValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	valign?: PropValue<"baseline" | "bottom" | "middle" | "top" | RemoveAttribute>;
	/** @deprecated */
	width?: PropValue<number | string | RemoveAttribute>;
}
interface TimeHTMLAttributes<T> extends HTMLAttributes<T> {
	datetime?: PropValue<string | RemoveAttribute>;
}
interface TrackHTMLAttributes<T> extends HTMLAttributes<T> {
	default?: PropValue<BooleanAttribute | RemoveAttribute>;
	kind?: PropValue<"alternative" | "descriptions" | "main" | "main-desc" | "translation" | "commentary" | "subtitles" | "captions" | "chapters" | "metadata" | RemoveAttribute>;
	label?: PropValue<string | RemoveAttribute>;
	src?: PropValue<string | RemoveAttribute>;
	srclang?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	mediagroup?: PropValue<string | RemoveAttribute>;
}
interface VideoHTMLAttributes<T> extends MediaHTMLAttributes<T> {
	disablepictureinpicture?: PropValue<BooleanAttribute | RemoveAttribute>;
	height?: PropValue<number | string | RemoveAttribute>;
	playsinline?: PropValue<BooleanAttribute | RemoveAttribute>;
	poster?: PropValue<string | RemoveAttribute>;
	width?: PropValue<number | string | RemoveAttribute>;
	onEnterPictureInPicture?: EventHandlerUnion<T, PictureInPictureEvent> | undefined;
	// "on:enterpictureinpicture"?: EventHandlerWithOptionsUnion<T, PictureInPictureEvent> | undefined;
	onLeavePictureInPicture?: EventHandlerUnion<T, PictureInPictureEvent> | undefined;
}
interface WebViewHTMLAttributes<T> extends HTMLAttributes<T> {
	allowpopups?: PropValue<BooleanAttribute | RemoveAttribute>;
	disableblinkfeatures?: PropValue<string | RemoveAttribute>;
	disablewebsecurity?: PropValue<BooleanAttribute | RemoveAttribute>;
	enableblinkfeatures?: PropValue<string | RemoveAttribute>;
	httpreferrer?: PropValue<string | RemoveAttribute>;
	nodeintegration?: PropValue<BooleanAttribute | RemoveAttribute>;
	nodeintegrationinsubframes?: PropValue<BooleanAttribute | RemoveAttribute>;
	partition?: PropValue<string | RemoveAttribute>;
	plugins?: PropValue<BooleanAttribute | RemoveAttribute>;
	preload?: PropValue<string | RemoveAttribute>;
	src?: PropValue<string | RemoveAttribute>;
	useragent?: PropValue<string | RemoveAttribute>;
	webpreferences?: PropValue<string | RemoveAttribute>;
	// does this exists?
	allowfullscreen?: PropValue<BooleanAttribute | RemoveAttribute>;
	autosize?: PropValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	blinkfeatures?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	disableguestresize?: PropValue<BooleanAttribute | RemoveAttribute>;
	/** @deprecated */
	guestinstance?: PropValue<string | RemoveAttribute>;
}
type SVGPreserveAspectRatioValue = "none" | "xMinYMin" | "xMidYMin" | "xMaxYMin" | "xMinYMid" | "xMidYMid" | "xMaxYMid" | "xMinYMax" | "xMidYMax" | "xMaxYMax" | "xMinYMin meet" | "xMidYMin meet" | "xMaxYMin meet" | "xMinYMid meet" | "xMidYMid meet" | "xMaxYMid meet" | "xMinYMax meet" | "xMidYMax meet" | "xMaxYMax meet" | "xMinYMin slice" | "xMidYMin slice" | "xMaxYMin slice" | "xMinYMid slice" | "xMidYMid slice" | "xMaxYMid slice" | "xMinYMax slice" | "xMidYMax slice" | "xMaxYMax slice";
type ImagePreserveAspectRatio = SVGPreserveAspectRatioValue | "defer none" | "defer xMinYMin" | "defer xMidYMin" | "defer xMaxYMin" | "defer xMinYMid" | "defer xMidYMid" | "defer xMaxYMid" | "defer xMinYMax" | "defer xMidYMax" | "defer xMaxYMax" | "defer xMinYMin meet" | "defer xMidYMin meet" | "defer xMaxYMin meet" | "defer xMinYMid meet" | "defer xMidYMid meet" | "defer xMaxYMid meet" | "defer xMinYMax meet" | "defer xMidYMax meet" | "defer xMaxYMax meet" | "defer xMinYMin slice" | "defer xMidYMin slice" | "defer xMaxYMin slice" | "defer xMinYMid slice" | "defer xMidYMid slice" | "defer xMaxYMid slice" | "defer xMinYMax slice" | "defer xMidYMax slice" | "defer xMaxYMax slice";
type SVGUnits = "userSpaceOnUse" | "objectBoundingBox";
interface StylableSVGAttributes {
	class?: ElementAttributes<Element>["class"];
	style?: PropValue<CSSProperties | string | RemoveAttribute>;
}
interface TransformableSVGAttributes {
	transform?: PropValue<string | RemoveAttribute>;
}
interface ConditionalProcessingSVGAttributes {
	requiredExtensions?: PropValue<string | RemoveAttribute>;
	requiredFeatures?: PropValue<string | RemoveAttribute>;
	systemLanguage?: PropValue<string | RemoveAttribute>;
}
interface ExternalResourceSVGAttributes {
	externalResourcesRequired?: PropValue<EnumeratedPseudoBoolean | RemoveAttribute>;
}
interface AnimationTimingSVGAttributes {
	begin?: PropValue<string | RemoveAttribute>;
	dur?: PropValue<string | RemoveAttribute>;
	end?: PropValue<string | RemoveAttribute>;
	fill?: PropValue<"freeze" | "remove" | RemoveAttribute>;
	max?: PropValue<string | RemoveAttribute>;
	min?: PropValue<string | RemoveAttribute>;
	repeatCount?: PropValue<number | "indefinite" | RemoveAttribute>;
	repeatDur?: PropValue<string | RemoveAttribute>;
	restart?: PropValue<"always" | "whenNotActive" | "never" | RemoveAttribute>;
}
interface AnimationValueSVGAttributes {
	by?: PropValue<number | string | RemoveAttribute>;
	calcMode?: PropValue<"discrete" | "linear" | "paced" | "spline" | RemoveAttribute>;
	from?: PropValue<number | string | RemoveAttribute>;
	keySplines?: PropValue<string | RemoveAttribute>;
	keyTimes?: PropValue<string | RemoveAttribute>;
	to?: PropValue<number | string | RemoveAttribute>;
	values?: PropValue<string | RemoveAttribute>;
}
interface AnimationAdditionSVGAttributes {
	accumulate?: PropValue<"none" | "sum" | RemoveAttribute>;
	additive?: PropValue<"replace" | "sum" | RemoveAttribute>;
	attributeName?: PropValue<string | RemoveAttribute>;
}
interface AnimationAttributeTargetSVGAttributes {
	attributeName?: PropValue<string | RemoveAttribute>;
	attributeType?: PropValue<"CSS" | "XML" | "auto" | RemoveAttribute>;
}
interface PresentationSVGAttributes {
	"alignment-baseline"?: PropValue<"auto" | "baseline" | "before-edge" | "text-before-edge" | "middle" | "central" | "after-edge" | "text-after-edge" | "ideographic" | "alphabetic" | "hanging" | "mathematical" | "inherit" | RemoveAttribute>;
	"baseline-shift"?: PropValue<number | string | RemoveAttribute>;
	"clip-path"?: PropValue<string | RemoveAttribute>;
	"clip-rule"?: PropValue<"nonzero" | "evenodd" | "inherit" | RemoveAttribute>;
	"color-interpolation"?: PropValue<"auto" | "sRGB" | "linearRGB" | "inherit" | RemoveAttribute>;
	"color-interpolation-filters"?: PropValue<"auto" | "sRGB" | "linearRGB" | "inherit" | RemoveAttribute>;
	"color-profile"?: PropValue<string | RemoveAttribute>;
	"color-rendering"?: PropValue<"auto" | "optimizeSpeed" | "optimizeQuality" | "inherit" | RemoveAttribute>;
	"dominant-baseline"?: PropValue<"auto" | "text-bottom" | "alphabetic" | "ideographic" | "middle" | "central" | "mathematical" | "hanging" | "text-top" | "inherit" | RemoveAttribute>;
	"enable-background"?: PropValue<string | RemoveAttribute>;
	"fill-opacity"?: PropValue<number | string | "inherit" | RemoveAttribute>;
	"fill-rule"?: PropValue<"nonzero" | "evenodd" | "inherit" | RemoveAttribute>;
	"flood-color"?: PropValue<string | RemoveAttribute>;
	"flood-opacity"?: PropValue<number | string | "inherit" | RemoveAttribute>;
	"font-family"?: PropValue<string | RemoveAttribute>;
	"font-size"?: PropValue<string | RemoveAttribute>;
	"font-size-adjust"?: PropValue<number | string | RemoveAttribute>;
	"font-stretch"?: PropValue<string | RemoveAttribute>;
	"font-style"?: PropValue<"normal" | "italic" | "oblique" | "inherit" | RemoveAttribute>;
	"font-variant"?: PropValue<string | RemoveAttribute>;
	"font-weight"?: PropValue<number | string | RemoveAttribute>;
	"glyph-orientation-horizontal"?: PropValue<string | RemoveAttribute>;
	"glyph-orientation-vertical"?: PropValue<string | RemoveAttribute>;
	"image-rendering"?: PropValue<"auto" | "optimizeQuality" | "optimizeSpeed" | "inherit" | RemoveAttribute>;
	"letter-spacing"?: PropValue<number | string | RemoveAttribute>;
	"lighting-color"?: PropValue<string | RemoveAttribute>;
	"marker-end"?: PropValue<string | RemoveAttribute>;
	"marker-mid"?: PropValue<string | RemoveAttribute>;
	"marker-start"?: PropValue<string | RemoveAttribute>;
	"pointer-events"?: PropValue<"bounding-box" | "visiblePainted" | "visibleFill" | "visibleStroke" | "visible" | "painted" | "color" | "fill" | "stroke" | "all" | "none" | "inherit" | RemoveAttribute>;
	"shape-rendering"?: PropValue<"auto" | "optimizeSpeed" | "crispEdges" | "geometricPrecision" | "inherit" | RemoveAttribute>;
	"stop-color"?: PropValue<string | RemoveAttribute>;
	"stop-opacity"?: PropValue<number | string | "inherit" | RemoveAttribute>;
	"stroke-dasharray"?: PropValue<string | RemoveAttribute>;
	"stroke-dashoffset"?: PropValue<number | string | RemoveAttribute>;
	"stroke-linecap"?: PropValue<"butt" | "round" | "square" | "inherit" | RemoveAttribute>;
	"stroke-linejoin"?: PropValue<"arcs" | "bevel" | "miter" | "miter-clip" | "round" | "inherit" | RemoveAttribute>;
	"stroke-miterlimit"?: PropValue<number | string | "inherit" | RemoveAttribute>;
	"stroke-opacity"?: PropValue<number | string | "inherit" | RemoveAttribute>;
	"stroke-width"?: PropValue<number | string | RemoveAttribute>;
	"text-anchor"?: PropValue<"start" | "middle" | "end" | "inherit" | RemoveAttribute>;
	"text-decoration"?: PropValue<"none" | "underline" | "overline" | "line-through" | "blink" | "inherit" | RemoveAttribute>;
	"text-rendering"?: PropValue<"auto" | "optimizeSpeed" | "optimizeLegibility" | "geometricPrecision" | "inherit" | RemoveAttribute>;
	"unicode-bidi"?: PropValue<string | RemoveAttribute>;
	"word-spacing"?: PropValue<number | string | RemoveAttribute>;
	"writing-mode"?: PropValue<"lr-tb" | "rl-tb" | "tb-rl" | "lr" | "rl" | "tb" | "inherit" | RemoveAttribute>;
	clip?: PropValue<string | RemoveAttribute>;
	color?: PropValue<string | RemoveAttribute>;
	cursor?: PropValue<string | RemoveAttribute>;
	direction?: PropValue<"ltr" | "rtl" | "inherit" | RemoveAttribute>;
	display?: PropValue<string | RemoveAttribute>;
	fill?: PropValue<string | RemoveAttribute>;
	filter?: PropValue<string | RemoveAttribute>;
	kerning?: PropValue<string | RemoveAttribute>;
	mask?: PropValue<string | RemoveAttribute>;
	opacity?: PropValue<number | string | "inherit" | RemoveAttribute>;
	overflow?: PropValue<"visible" | "hidden" | "scroll" | "auto" | "inherit" | RemoveAttribute>;
	pathLength?: PropValue<string | number | RemoveAttribute>;
	stroke?: PropValue<string | RemoveAttribute>;
	visibility?: PropValue<"visible" | "hidden" | "collapse" | "inherit" | RemoveAttribute>;
}
interface AnimationElementSVGAttributes<T> extends SVGAttributes<T>, ExternalResourceSVGAttributes, ConditionalProcessingSVGAttributes {
	// TODO TimeEvent is currently undefined on TS
	onBegin?: EventHandlerUnion<T, Event> | undefined;
	// "on:begin"?: EventHandlerWithOptionsUnion<T, Event> | undefined;
	// TODO TimeEvent is currently undefined on TS
	onEnd?: EventHandlerUnion<T, Event> | undefined;
	// "on:end"?: EventHandlerWithOptionsUnion<T, Event> | undefined;
	// TODO TimeEvent is currently undefined on TS
	onRepeat?: EventHandlerUnion<T, Event> | undefined;
}
interface ContainerElementSVGAttributes<T> extends SVGAttributes<T>, ShapeElementSVGAttributes<T>, Pick<PresentationSVGAttributes, "clip-path" | "mask" | "cursor" | "opacity" | "filter" | "enable-background" | "color-interpolation" | "color-rendering"> {
}
interface FilterPrimitiveElementSVGAttributes<T> extends SVGAttributes<T>, Pick<PresentationSVGAttributes, "color-interpolation-filters"> {
	height?: PropValue<number | string | RemoveAttribute>;
	result?: PropValue<string | RemoveAttribute>;
	width?: PropValue<number | string | RemoveAttribute>;
	x?: PropValue<number | string | RemoveAttribute>;
	y?: PropValue<number | string | RemoveAttribute>;
}
interface SingleInputFilterSVGAttributes {
	in?: PropValue<string | RemoveAttribute>;
}
interface DoubleInputFilterSVGAttributes {
	in?: PropValue<string | RemoveAttribute>;
	in2?: PropValue<string | RemoveAttribute>;
}
interface FitToViewBoxSVGAttributes {
	preserveAspectRatio?: PropValue<SVGPreserveAspectRatioValue | RemoveAttribute>;
	viewBox?: PropValue<string | RemoveAttribute>;
}
interface GradientElementSVGAttributes<T> extends SVGAttributes<T>, ExternalResourceSVGAttributes, StylableSVGAttributes {
	gradientTransform?: PropValue<string | RemoveAttribute>;
	gradientUnits?: PropValue<SVGUnits | RemoveAttribute>;
	href?: PropValue<string | RemoveAttribute>;
	spreadMethod?: PropValue<"pad" | "reflect" | "repeat" | RemoveAttribute>;
}
interface GraphicsElementSVGAttributes<T> extends SVGAttributes<T>, Pick<PresentationSVGAttributes, "clip-rule" | "mask" | "pointer-events" | "cursor" | "opacity" | "filter" | "display" | "visibility" | "color-interpolation" | "color-rendering"> {
}
interface LightSourceElementSVGAttributes<T> extends SVGAttributes<T> {
}
interface NewViewportSVGAttributes<T> extends SVGAttributes<T>, Pick<PresentationSVGAttributes, "overflow" | "clip"> {
	viewBox?: PropValue<string | RemoveAttribute>;
}
interface ShapeElementSVGAttributes<T> extends SVGAttributes<T>, Pick<PresentationSVGAttributes, "color" | "fill" | "fill-rule" | "fill-opacity" | "stroke" | "stroke-width" | "stroke-linecap" | "stroke-linejoin" | "stroke-miterlimit" | "stroke-dasharray" | "stroke-dashoffset" | "stroke-opacity" | "shape-rendering" | "pathLength"> {
}
interface TextContentElementSVGAttributes<T> extends SVGAttributes<T>, Pick<PresentationSVGAttributes, "font-family" | "font-style" | "font-variant" | "font-weight" | "font-stretch" | "font-size" | "font-size-adjust" | "kerning" | "letter-spacing" | "word-spacing" | "text-decoration" | "glyph-orientation-horizontal" | "glyph-orientation-vertical" | "direction" | "unicode-bidi" | "text-anchor" | "dominant-baseline" | "color" | "fill" | "fill-rule" | "fill-opacity" | "stroke" | "stroke-width" | "stroke-linecap" | "stroke-linejoin" | "stroke-miterlimit" | "stroke-dasharray" | "stroke-dashoffset" | "stroke-opacity"> {
}
interface ZoomAndPanSVGAttributes {
	/** @deprecated @non-standard */
	zoomAndPan?: PropValue<"disable" | "magnify" | RemoveAttribute>;
}
interface AnimateSVGAttributes<T> extends AnimationElementSVGAttributes<T>, AnimationAttributeTargetSVGAttributes, AnimationTimingSVGAttributes, AnimationValueSVGAttributes, AnimationAdditionSVGAttributes, Pick<PresentationSVGAttributes, "color-interpolation" | "color-rendering"> {
}
interface AnimateMotionSVGAttributes<T> extends AnimationElementSVGAttributes<T>, AnimationTimingSVGAttributes, AnimationValueSVGAttributes, AnimationAdditionSVGAttributes {
	keyPoints?: PropValue<string | RemoveAttribute>;
	origin?: PropValue<"default" | RemoveAttribute>;
	path?: PropValue<string | RemoveAttribute>;
	rotate?: PropValue<number | string | "auto" | "auto-reverse" | RemoveAttribute>;
}
interface AnimateTransformSVGAttributes<T> extends AnimationElementSVGAttributes<T>, AnimationAttributeTargetSVGAttributes, AnimationTimingSVGAttributes, AnimationValueSVGAttributes, AnimationAdditionSVGAttributes {
	type?: PropValue<"translate" | "scale" | "rotate" | "skewX" | "skewY" | RemoveAttribute>;
}
interface CircleSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path"> {
	cx?: PropValue<number | string | RemoveAttribute>;
	cy?: PropValue<number | string | RemoveAttribute>;
	r?: PropValue<number | string | RemoveAttribute>;
}
interface ClipPathSVGAttributes<T> extends SVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path"> {
	clipPathUnits?: PropValue<SVGUnits | RemoveAttribute>;
}
interface DefsSVGAttributes<T> extends ContainerElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes {
}
interface DescSVGAttributes<T> extends SVGAttributes<T>, StylableSVGAttributes {
}
interface EllipseSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path"> {
	cx?: PropValue<number | string | RemoveAttribute>;
	cy?: PropValue<number | string | RemoveAttribute>;
	rx?: PropValue<number | string | RemoveAttribute>;
	ry?: PropValue<number | string | RemoveAttribute>;
}
interface FeBlendSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, DoubleInputFilterSVGAttributes, StylableSVGAttributes {
	mode?: PropValue<"normal" | "multiply" | "screen" | "darken" | "lighten" | RemoveAttribute>;
}
interface FeColorMatrixSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
	type?: PropValue<"matrix" | "saturate" | "hueRotate" | "luminanceToAlpha" | RemoveAttribute>;
	values?: PropValue<string | RemoveAttribute>;
}
interface FeComponentTransferSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
}
interface FeCompositeSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, DoubleInputFilterSVGAttributes, StylableSVGAttributes {
	k1?: PropValue<number | string | RemoveAttribute>;
	k2?: PropValue<number | string | RemoveAttribute>;
	k3?: PropValue<number | string | RemoveAttribute>;
	k4?: PropValue<number | string | RemoveAttribute>;
	operator?: PropValue<"over" | "in" | "out" | "atop" | "xor" | "arithmetic" | RemoveAttribute>;
}
interface FeConvolveMatrixSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
	bias?: PropValue<number | string | RemoveAttribute>;
	divisor?: PropValue<number | string | RemoveAttribute>;
	edgeMode?: PropValue<"duplicate" | "wrap" | "none" | RemoveAttribute>;
	kernelMatrix?: PropValue<string | RemoveAttribute>;
	kernelUnitLength?: PropValue<number | string | RemoveAttribute>;
	order?: PropValue<number | string | RemoveAttribute>;
	preserveAlpha?: PropValue<EnumeratedPseudoBoolean | RemoveAttribute>;
	targetX?: PropValue<number | string | RemoveAttribute>;
	targetY?: PropValue<number | string | RemoveAttribute>;
}
interface FeDiffuseLightingSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes, Pick<PresentationSVGAttributes, "color" | "lighting-color"> {
	diffuseConstant?: PropValue<number | string | RemoveAttribute>;
	kernelUnitLength?: PropValue<number | string | RemoveAttribute>;
	surfaceScale?: PropValue<number | string | RemoveAttribute>;
}
interface FeDisplacementMapSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, DoubleInputFilterSVGAttributes, StylableSVGAttributes {
	scale?: PropValue<number | string | RemoveAttribute>;
	xChannelSelector?: PropValue<"R" | "G" | "B" | "A" | RemoveAttribute>;
	yChannelSelector?: PropValue<"R" | "G" | "B" | "A" | RemoveAttribute>;
}
interface FeDistantLightSVGAttributes<T> extends LightSourceElementSVGAttributes<T> {
	azimuth?: PropValue<number | string | RemoveAttribute>;
	elevation?: PropValue<number | string | RemoveAttribute>;
}
interface FeDropShadowSVGAttributes<T> extends SVGAttributes<T>, FilterPrimitiveElementSVGAttributes<T>, StylableSVGAttributes, Pick<PresentationSVGAttributes, "color" | "flood-color" | "flood-opacity"> {
	dx?: PropValue<number | string | RemoveAttribute>;
	dy?: PropValue<number | string | RemoveAttribute>;
	stdDeviation?: PropValue<number | string | RemoveAttribute>;
}
interface FeFloodSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, StylableSVGAttributes, Pick<PresentationSVGAttributes, "color" | "flood-color" | "flood-opacity"> {
}
interface FeFuncSVGAttributes<T> extends SVGAttributes<T> {
	amplitude?: PropValue<number | string | RemoveAttribute>;
	exponent?: PropValue<number | string | RemoveAttribute>;
	intercept?: PropValue<number | string | RemoveAttribute>;
	offset?: PropValue<number | string | RemoveAttribute>;
	slope?: PropValue<number | string | RemoveAttribute>;
	tableValues?: PropValue<string | RemoveAttribute>;
	type?: PropValue<"identity" | "table" | "discrete" | "linear" | "gamma" | RemoveAttribute>;
}
interface FeGaussianBlurSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
	stdDeviation?: PropValue<number | string | RemoveAttribute>;
}
interface FeImageSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, ExternalResourceSVGAttributes, StylableSVGAttributes {
	href?: PropValue<string | RemoveAttribute>;
	preserveAspectRatio?: PropValue<SVGPreserveAspectRatioValue | RemoveAttribute>;
}
interface FeMergeSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, StylableSVGAttributes {
}
interface FeMergeNodeSVGAttributes<T> extends SVGAttributes<T>, SingleInputFilterSVGAttributes {
}
interface FeMorphologySVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
	operator?: PropValue<"erode" | "dilate" | RemoveAttribute>;
	radius?: PropValue<number | string | RemoveAttribute>;
}
interface FeOffsetSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
	dx?: PropValue<number | string | RemoveAttribute>;
	dy?: PropValue<number | string | RemoveAttribute>;
}
interface FePointLightSVGAttributes<T> extends LightSourceElementSVGAttributes<T> {
	x?: PropValue<number | string | RemoveAttribute>;
	y?: PropValue<number | string | RemoveAttribute>;
	z?: PropValue<number | string | RemoveAttribute>;
}
interface FeSpecularLightingSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes, Pick<PresentationSVGAttributes, "color" | "lighting-color"> {
	kernelUnitLength?: PropValue<number | string | RemoveAttribute>;
	specularConstant?: PropValue<string | RemoveAttribute>;
	specularExponent?: PropValue<string | RemoveAttribute>;
	surfaceScale?: PropValue<string | RemoveAttribute>;
}
interface FeSpotLightSVGAttributes<T> extends LightSourceElementSVGAttributes<T> {
	limitingConeAngle?: PropValue<number | string | RemoveAttribute>;
	pointsAtX?: PropValue<number | string | RemoveAttribute>;
	pointsAtY?: PropValue<number | string | RemoveAttribute>;
	pointsAtZ?: PropValue<number | string | RemoveAttribute>;
	specularExponent?: PropValue<number | string | RemoveAttribute>;
	x?: PropValue<number | string | RemoveAttribute>;
	y?: PropValue<number | string | RemoveAttribute>;
	z?: PropValue<number | string | RemoveAttribute>;
}
interface FeTileSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, SingleInputFilterSVGAttributes, StylableSVGAttributes {
}
interface FeTurbulanceSVGAttributes<T> extends FilterPrimitiveElementSVGAttributes<T>, StylableSVGAttributes {
	baseFrequency?: PropValue<number | string | RemoveAttribute>;
	numOctaves?: PropValue<number | string | RemoveAttribute>;
	seed?: PropValue<number | string | RemoveAttribute>;
	stitchTiles?: PropValue<"stitch" | "noStitch" | RemoveAttribute>;
	type?: PropValue<"fractalNoise" | "turbulence" | RemoveAttribute>;
}
interface FilterSVGAttributes<T> extends SVGAttributes<T>, ExternalResourceSVGAttributes, StylableSVGAttributes {
	filterRes?: PropValue<number | string | RemoveAttribute>;
	filterUnits?: PropValue<SVGUnits | RemoveAttribute>;
	height?: PropValue<number | string | RemoveAttribute>;
	primitiveUnits?: PropValue<SVGUnits | RemoveAttribute>;
	width?: PropValue<number | string | RemoveAttribute>;
	x?: PropValue<number | string | RemoveAttribute>;
	y?: PropValue<number | string | RemoveAttribute>;
}
interface ForeignObjectSVGAttributes<T> extends NewViewportSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "display" | "visibility"> {
	height?: PropValue<number | string | RemoveAttribute>;
	width?: PropValue<number | string | RemoveAttribute>;
	x?: PropValue<number | string | RemoveAttribute>;
	y?: PropValue<number | string | RemoveAttribute>;
}
interface GSVGAttributes<T> extends ContainerElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "display" | "visibility"> {
}
interface ImageSVGAttributes<T> extends NewViewportSVGAttributes<T>, GraphicsElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "color-profile" | "image-rendering"> {
	height?: PropValue<number | string | RemoveAttribute>;
	href?: PropValue<string | RemoveAttribute>;
	preserveAspectRatio?: PropValue<ImagePreserveAspectRatio | RemoveAttribute>;
	width?: PropValue<number | string | RemoveAttribute>;
	x?: PropValue<number | string | RemoveAttribute>;
	y?: PropValue<number | string | RemoveAttribute>;
}
interface LineSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "marker-start" | "marker-mid" | "marker-end"> {
	x1?: PropValue<number | string | RemoveAttribute>;
	x2?: PropValue<number | string | RemoveAttribute>;
	y1?: PropValue<number | string | RemoveAttribute>;
	y2?: PropValue<number | string | RemoveAttribute>;
}
interface LinearGradientSVGAttributes<T> extends GradientElementSVGAttributes<T> {
	x1?: PropValue<number | string | RemoveAttribute>;
	x2?: PropValue<number | string | RemoveAttribute>;
	y1?: PropValue<number | string | RemoveAttribute>;
	y2?: PropValue<number | string | RemoveAttribute>;
}
interface MarkerSVGAttributes<T> extends ContainerElementSVGAttributes<T>, ExternalResourceSVGAttributes, StylableSVGAttributes, FitToViewBoxSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "overflow" | "clip"> {
	markerHeight?: PropValue<number | string | RemoveAttribute>;
	markerUnits?: PropValue<"strokeWidth" | "userSpaceOnUse" | RemoveAttribute>;
	markerWidth?: PropValue<number | string | RemoveAttribute>;
	orient?: PropValue<string | RemoveAttribute>;
	refX?: PropValue<number | string | RemoveAttribute>;
	refY?: PropValue<number | string | RemoveAttribute>;
}
interface MaskSVGAttributes<T> extends Omit<ContainerElementSVGAttributes<T>, "opacity" | "filter">, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path"> {
	height?: PropValue<number | string | RemoveAttribute>;
	maskContentUnits?: PropValue<SVGUnits | RemoveAttribute>;
	maskUnits?: PropValue<SVGUnits | RemoveAttribute>;
	width?: PropValue<number | string | RemoveAttribute>;
	x?: PropValue<number | string | RemoveAttribute>;
	y?: PropValue<number | string | RemoveAttribute>;
}
interface MetadataSVGAttributes<T> extends SVGAttributes<T> {
}
interface MPathSVGAttributes<T> extends SVGAttributes<T> {
}
interface PathSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "marker-start" | "marker-mid" | "marker-end"> {
	d?: PropValue<string | RemoveAttribute>;
	pathLength?: PropValue<number | string | RemoveAttribute>;
}
interface PatternSVGAttributes<T> extends ContainerElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, FitToViewBoxSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "overflow" | "clip"> {
	height?: PropValue<number | string | RemoveAttribute>;
	href?: PropValue<string | RemoveAttribute>;
	patternContentUnits?: PropValue<SVGUnits | RemoveAttribute>;
	patternTransform?: PropValue<string | RemoveAttribute>;
	patternUnits?: PropValue<SVGUnits | RemoveAttribute>;
	width?: PropValue<number | string | RemoveAttribute>;
	x?: PropValue<number | string | RemoveAttribute>;
	y?: PropValue<number | string | RemoveAttribute>;
}
interface PolygonSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "marker-start" | "marker-mid" | "marker-end"> {
	points?: PropValue<string | RemoveAttribute>;
}
interface PolylineSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "marker-start" | "marker-mid" | "marker-end"> {
	points?: PropValue<string | RemoveAttribute>;
}
interface RadialGradientSVGAttributes<T> extends GradientElementSVGAttributes<T> {
	cx?: PropValue<number | string | RemoveAttribute>;
	cy?: PropValue<number | string | RemoveAttribute>;
	fx?: PropValue<number | string | RemoveAttribute>;
	fy?: PropValue<number | string | RemoveAttribute>;
	r?: PropValue<number | string | RemoveAttribute>;
}
interface RectSVGAttributes<T> extends GraphicsElementSVGAttributes<T>, ShapeElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path"> {
	height?: PropValue<number | string | RemoveAttribute>;
	rx?: PropValue<number | string | RemoveAttribute>;
	ry?: PropValue<number | string | RemoveAttribute>;
	width?: PropValue<number | string | RemoveAttribute>;
	x?: PropValue<number | string | RemoveAttribute>;
	y?: PropValue<number | string | RemoveAttribute>;
}
interface SetSVGAttributes<T> extends AnimationElementSVGAttributes<T>, StylableSVGAttributes, AnimationTimingSVGAttributes {
}
interface StopSVGAttributes<T> extends SVGAttributes<T>, StylableSVGAttributes, Pick<PresentationSVGAttributes, "color" | "stop-color" | "stop-opacity"> {
	offset?: PropValue<number | string | RemoveAttribute>;
}
interface SvgSVGAttributes<T> extends ContainerElementSVGAttributes<T>, NewViewportSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, FitToViewBoxSVGAttributes, ZoomAndPanSVGAttributes, PresentationSVGAttributes, EventHandlersWindow<T> {
	"xmlns:xlink"?: PropValue<string | RemoveAttribute>;
	contentScriptType?: PropValue<string | RemoveAttribute>;
	contentStyleType?: PropValue<string | RemoveAttribute>;
	height?: PropValue<number | string | RemoveAttribute>;
	width?: PropValue<number | string | RemoveAttribute>;
	x?: PropValue<number | string | RemoveAttribute>;
	y?: PropValue<number | string | RemoveAttribute>;
	/** @deprecated */
	baseProfile?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	version?: PropValue<string | RemoveAttribute>;
}
interface SwitchSVGAttributes<T> extends ContainerElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "display" | "visibility"> {
}
interface SymbolSVGAttributes<T> extends ContainerElementSVGAttributes<T>, NewViewportSVGAttributes<T>, ExternalResourceSVGAttributes, StylableSVGAttributes, FitToViewBoxSVGAttributes, Pick<PresentationSVGAttributes, "clip-path"> {
	height?: PropValue<number | string | RemoveAttribute>;
	preserveAspectRatio?: PropValue<SVGPreserveAspectRatioValue | RemoveAttribute>;
	refX?: PropValue<number | string | RemoveAttribute>;
	refY?: PropValue<number | string | RemoveAttribute>;
	viewBox?: PropValue<string | RemoveAttribute>;
	width?: PropValue<number | string | RemoveAttribute>;
	x?: PropValue<number | string | RemoveAttribute>;
	y?: PropValue<number | string | RemoveAttribute>;
}
interface TextSVGAttributes<T> extends TextContentElementSVGAttributes<T>, GraphicsElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, TransformableSVGAttributes, Pick<PresentationSVGAttributes, "clip-path" | "writing-mode" | "text-rendering"> {
	dx?: PropValue<number | string | RemoveAttribute>;
	dy?: PropValue<number | string | RemoveAttribute>;
	lengthAdjust?: PropValue<"spacing" | "spacingAndGlyphs" | RemoveAttribute>;
	rotate?: PropValue<number | string | RemoveAttribute>;
	textLength?: PropValue<number | string | RemoveAttribute>;
	x?: PropValue<number | string | RemoveAttribute>;
	y?: PropValue<number | string | RemoveAttribute>;
}
interface TextPathSVGAttributes<T> extends TextContentElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, Pick<PresentationSVGAttributes, "alignment-baseline" | "baseline-shift" | "display" | "visibility"> {
	href?: PropValue<string | RemoveAttribute>;
	method?: PropValue<"align" | "stretch" | RemoveAttribute>;
	spacing?: PropValue<"auto" | "exact" | RemoveAttribute>;
	startOffset?: PropValue<number | string | RemoveAttribute>;
}
interface TSpanSVGAttributes<T> extends TextContentElementSVGAttributes<T>, ConditionalProcessingSVGAttributes, ExternalResourceSVGAttributes, StylableSVGAttributes, Pick<PresentationSVGAttributes, "alignment-baseline" | "baseline-shift" | "display" | "visibility"> {
	dx?: PropValue<number | string | RemoveAttribute>;
	dy?: PropValue<number | string | RemoveAttribute>;
	lengthAdjust?: PropValue<"spacing" | "spacingAndGlyphs" | RemoveAttribute>;
	rotate?: PropValue<number | string | RemoveAttribute>;
	textLength?: PropValue<number | string | RemoveAttribute>;
	x?: PropValue<number | string | RemoveAttribute>;
	y?: PropValue<number | string | RemoveAttribute>;
}
interface UseSVGAttributes<T> extends SVGAttributes<T>, StylableSVGAttributes, ConditionalProcessingSVGAttributes, GraphicsElementSVGAttributes<T>, PresentationSVGAttributes, ExternalResourceSVGAttributes, TransformableSVGAttributes {
	height?: PropValue<number | string | RemoveAttribute>;
	href?: PropValue<string | RemoveAttribute>;
	width?: PropValue<number | string | RemoveAttribute>;
	x?: PropValue<number | string | RemoveAttribute>;
	y?: PropValue<number | string | RemoveAttribute>;
}
interface ViewSVGAttributes<T> extends SVGAttributes<T>, ExternalResourceSVGAttributes, FitToViewBoxSVGAttributes, ZoomAndPanSVGAttributes {
	viewTarget?: PropValue<string | RemoveAttribute>;
}
interface MathMLAnnotationElementAttributes<T> extends MathMLAttributes<T> {
	encoding?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	src?: PropValue<string | RemoveAttribute>;
}
interface MathMLAnnotationXmlElementAttributes<T> extends MathMLAttributes<T> {
	encoding?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	src?: PropValue<string | RemoveAttribute>;
}
interface MathMLMactionElementAttributes<T> extends MathMLAttributes<T> {
	/** @deprecated @non-standard */
	actiontype?: PropValue<"statusline" | "toggle" | RemoveAttribute>;
	/** @deprecated @non-standard */
	selection?: PropValue<string | RemoveAttribute>;
}
interface MathMLMathElementAttributes<T> extends MathMLAttributes<T> {
	display?: PropValue<"block" | "inline" | RemoveAttribute>;
}
interface MathMLMerrorElementAttributes<T> extends MathMLAttributes<T> {
}
interface MathMLMfracElementAttributes<T> extends MathMLAttributes<T> {
	linethickness?: PropValue<string | RemoveAttribute>;
	/** @deprecated @non-standard */
	denomalign?: PropValue<"center" | "left" | "right" | RemoveAttribute>;
	/** @deprecated @non-standard */
	numalign?: PropValue<"center" | "left" | "right" | RemoveAttribute>;
}
interface MathMLMiElementAttributes<T> extends MathMLAttributes<T> {
	mathvariant?: PropValue<"normal" | RemoveAttribute>;
}
interface MathMLMmultiscriptsElementAttributes<T> extends MathMLAttributes<T> {
	/** @deprecated @non-standard */
	subscriptshift?: PropValue<string | RemoveAttribute>;
	/** @deprecated @non-standard */
	superscriptshift?: PropValue<string | RemoveAttribute>;
}
interface MathMLMnElementAttributes<T> extends MathMLAttributes<T> {
}
interface MathMLMoElementAttributes<T> extends MathMLAttributes<T> {
	fence?: PropValue<BooleanAttribute | RemoveAttribute>;
	form?: PropValue<"prefix" | "infix" | "postfix" | RemoveAttribute>;
	largeop?: PropValue<BooleanAttribute | RemoveAttribute>;
	lspace?: PropValue<string | RemoveAttribute>;
	maxsize?: PropValue<string | RemoveAttribute>;
	minsize?: PropValue<string | RemoveAttribute>;
	movablelimits?: PropValue<BooleanAttribute | RemoveAttribute>;
	rspace?: PropValue<string | RemoveAttribute>;
	separator?: PropValue<BooleanAttribute | RemoveAttribute>;
	stretchy?: PropValue<BooleanAttribute | RemoveAttribute>;
	symmetric?: PropValue<BooleanAttribute | RemoveAttribute>;
	/** @non-standard */
	accent?: PropValue<BooleanAttribute | RemoveAttribute>;
}
interface MathMLMoverElementAttributes<T> extends MathMLAttributes<T> {
	accent?: PropValue<BooleanAttribute | RemoveAttribute>;
}
interface MathMLMpaddedElementAttributes<T> extends MathMLAttributes<T> {
	depth?: PropValue<string | RemoveAttribute>;
	height?: PropValue<string | RemoveAttribute>;
	lspace?: PropValue<string | RemoveAttribute>;
	voffset?: PropValue<string | RemoveAttribute>;
	width?: PropValue<string | RemoveAttribute>;
}
interface MathMLMphantomElementAttributes<T> extends MathMLAttributes<T> {
}
interface MathMLMprescriptsElementAttributes<T> extends MathMLAttributes<T> {
}
interface MathMLMrootElementAttributes<T> extends MathMLAttributes<T> {
}
interface MathMLMrowElementAttributes<T> extends MathMLAttributes<T> {
}
interface MathMLMsElementAttributes<T> extends MathMLAttributes<T> {
	/** @deprecated */
	lquote?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	rquote?: PropValue<string | RemoveAttribute>;
}
interface MathMLMspaceElementAttributes<T> extends MathMLAttributes<T> {
	depth?: PropValue<string | RemoveAttribute>;
	height?: PropValue<string | RemoveAttribute>;
	width?: PropValue<string | RemoveAttribute>;
}
interface MathMLMsqrtElementAttributes<T> extends MathMLAttributes<T> {
}
interface MathMLMstyleElementAttributes<T> extends MathMLAttributes<T> {
	/** @deprecated @non-standard */
	background?: PropValue<string | RemoveAttribute>;
	/** @deprecated @non-standard */
	color?: PropValue<string | RemoveAttribute>;
	/** @deprecated @non-standard */
	fontsize?: PropValue<string | RemoveAttribute>;
	/** @deprecated @non-standard */
	fontstyle?: PropValue<string | RemoveAttribute>;
	/** @deprecated @non-standard */
	fontweight?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	scriptminsize?: PropValue<string | RemoveAttribute>;
	/** @deprecated */
	scriptsizemultiplier?: PropValue<string | RemoveAttribute>;
}
interface MathMLMsubElementAttributes<T> extends MathMLAttributes<T> {
	/** @deprecated @non-standard */
	subscriptshift?: PropValue<string | RemoveAttribute>;
}
interface MathMLMsubsupElementAttributes<T> extends MathMLAttributes<T> {
	/** @deprecated @non-standard */
	subscriptshift?: PropValue<string | RemoveAttribute>;
	/** @deprecated @non-standard */
	superscriptshift?: PropValue<string | RemoveAttribute>;
}
interface MathMLMsupElementAttributes<T> extends MathMLAttributes<T> {
	/** @deprecated @non-standard */
	superscriptshift?: PropValue<string | RemoveAttribute>;
}
interface MathMLMtableElementAttributes<T> extends MathMLAttributes<T> {
	/** @non-standard */
	align?: PropValue<"axis" | "baseline" | "bottom" | "center" | "top" | RemoveAttribute>;
	/** @non-standard */
	columnalign?: PropValue<"center" | "left" | "right" | RemoveAttribute>;
	/** @non-standard */
	columnlines?: PropValue<"dashed" | "none" | "solid" | RemoveAttribute>;
	/** @non-standard */
	columnspacing?: PropValue<string | RemoveAttribute>;
	/** @non-standard */
	frame?: PropValue<"dashed" | "none" | "solid" | RemoveAttribute>;
	/** @non-standard */
	framespacing?: PropValue<string | RemoveAttribute>;
	/** @non-standard */
	rowalign?: PropValue<"axis" | "baseline" | "bottom" | "center" | "top" | RemoveAttribute>;
	/** @non-standard */
	rowlines?: PropValue<"dashed" | "none" | "solid" | RemoveAttribute>;
	/** @non-standard */
	rowspacing?: PropValue<string | RemoveAttribute>;
	/** @non-standard */
	width?: PropValue<string | RemoveAttribute>;
}
interface MathMLMtdElementAttributes<T> extends MathMLAttributes<T> {
	columnspan?: PropValue<number | string | RemoveAttribute>;
	rowspan?: PropValue<number | string | RemoveAttribute>;
	/** @non-standard */
	columnalign?: PropValue<"center" | "left" | "right" | RemoveAttribute>;
	/** @non-standard */
	rowalign?: PropValue<"axis" | "baseline" | "bottom" | "center" | "top" | RemoveAttribute>;
}
interface MathMLMtextElementAttributes<T> extends MathMLAttributes<T> {
}
interface MathMLMtrElementAttributes<T> extends MathMLAttributes<T> {
	/** @non-standard */
	columnalign?: PropValue<"center" | "left" | "right" | RemoveAttribute>;
	/** @non-standard */
	rowalign?: PropValue<"axis" | "baseline" | "bottom" | "center" | "top" | RemoveAttribute>;
}
interface MathMLMunderElementAttributes<T> extends MathMLAttributes<T> {
	accentunder?: PropValue<BooleanAttribute | RemoveAttribute>;
}
interface MathMLMunderoverElementAttributes<T> extends MathMLAttributes<T> {
	accent?: PropValue<BooleanAttribute | RemoveAttribute>;
	accentunder?: PropValue<BooleanAttribute | RemoveAttribute>;
}
interface MathMLSemanticsElementAttributes<T> extends MathMLAttributes<T> {
}
interface MathMLMencloseElementAttributes<T> extends MathMLAttributes<T> {
	/** @non-standard */
	notation?: PropValue<string | RemoveAttribute>;
}
interface MathMLMfencedElementAttributes<T> extends MathMLAttributes<T> {
	close?: PropValue<string | RemoveAttribute>;
	open?: PropValue<string | RemoveAttribute>;
	separators?: PropValue<string | RemoveAttribute>;
}
// TAGS
/** @type {HTMLElementTagNameMap} */
export interface HTMLElementTags {
	a: AnchorHTMLAttributes<HTMLAnchorElement>;
	abbr: HTMLAttributes<HTMLElement>;
	address: HTMLAttributes<HTMLElement>;
	area: AreaHTMLAttributes<HTMLAreaElement>;
	article: HTMLAttributes<HTMLElement>;
	aside: HTMLAttributes<HTMLElement>;
	audio: AudioHTMLAttributes<HTMLAudioElement>;
	b: HTMLAttributes<HTMLElement>;
	base: BaseHTMLAttributes<HTMLBaseElement>;
	bdi: HTMLAttributes<HTMLElement>;
	bdo: BdoHTMLAttributes<HTMLElement>;
	blockquote: BlockquoteHTMLAttributes<HTMLQuoteElement>;
	body: BodyHTMLAttributes<HTMLBodyElement>;
	br: HTMLAttributes<HTMLBRElement>;
	button: ButtonHTMLAttributes<HTMLButtonElement>;
	canvas: CanvasHTMLAttributes<HTMLCanvasElement>;
	caption: CaptionHTMLAttributes<HTMLTableCaptionElement>;
	cite: HTMLAttributes<HTMLElement>;
	code: HTMLAttributes<HTMLElement>;
	col: ColHTMLAttributes<HTMLTableColElement>;
	colgroup: ColgroupHTMLAttributes<HTMLTableColElement>;
	data: DataHTMLAttributes<HTMLDataElement>;
	datalist: HTMLAttributes<HTMLDataListElement>;
	dd: HTMLAttributes<HTMLElement>;
	del: ModHTMLAttributes<HTMLModElement>;
	details: DetailsHtmlAttributes<HTMLDetailsElement>;
	dfn: HTMLAttributes<HTMLElement>;
	dialog: DialogHtmlAttributes<HTMLDialogElement>;
	div: HTMLAttributes<HTMLDivElement>;
	dl: HTMLAttributes<HTMLDListElement>;
	dt: HTMLAttributes<HTMLElement>;
	em: HTMLAttributes<HTMLElement>;
	embed: EmbedHTMLAttributes<HTMLEmbedElement>;
	fieldset: FieldsetHTMLAttributes<HTMLFieldSetElement>;
	figcaption: HTMLAttributes<HTMLElement>;
	figure: HTMLAttributes<HTMLElement>;
	footer: HTMLAttributes<HTMLElement>;
	form: FormHTMLAttributes<HTMLFormElement>;
	h1: HTMLAttributes<HTMLHeadingElement>;
	h2: HTMLAttributes<HTMLHeadingElement>;
	h3: HTMLAttributes<HTMLHeadingElement>;
	h4: HTMLAttributes<HTMLHeadingElement>;
	h5: HTMLAttributes<HTMLHeadingElement>;
	h6: HTMLAttributes<HTMLHeadingElement>;
	head: HTMLAttributes<HTMLHeadElement>;
	header: HTMLAttributes<HTMLElement>;
	hgroup: HTMLAttributes<HTMLElement>;
	hr: HTMLAttributes<HTMLHRElement>;
	html: HTMLAttributes<HTMLHtmlElement>;
	i: HTMLAttributes<HTMLElement>;
	iframe: IframeHTMLAttributes<HTMLIFrameElement>;
	img: ImgHTMLAttributes<HTMLImageElement>;
	input: InputHTMLAttributes<HTMLInputElement>;
	ins: ModHTMLAttributes<HTMLModElement>;
	kbd: HTMLAttributes<HTMLElement>;
	label: LabelHTMLAttributes<HTMLLabelElement>;
	legend: HTMLAttributes<HTMLLegendElement>;
	li: LiHTMLAttributes<HTMLLIElement>;
	link: LinkHTMLAttributes<HTMLLinkElement>;
	main: HTMLAttributes<HTMLElement>;
	map: MapHTMLAttributes<HTMLMapElement>;
	mark: HTMLAttributes<HTMLElement>;
	menu: MenuHTMLAttributes<HTMLMenuElement>;
	meta: MetaHTMLAttributes<HTMLMetaElement>;
	meter: MeterHTMLAttributes<HTMLMeterElement>;
	nav: HTMLAttributes<HTMLElement>;
	noscript: HTMLAttributes<HTMLElement>;
	object: ObjectHTMLAttributes<HTMLObjectElement>;
	ol: OlHTMLAttributes<HTMLOListElement>;
	optgroup: OptgroupHTMLAttributes<HTMLOptGroupElement>;
	option: OptionHTMLAttributes<HTMLOptionElement>;
	output: OutputHTMLAttributes<HTMLOutputElement>;
	p: HTMLAttributes<HTMLParagraphElement>;
	picture: HTMLAttributes<HTMLPictureElement>;
	pre: HTMLAttributes<HTMLPreElement>;
	progress: ProgressHTMLAttributes<HTMLProgressElement>;
	q: QuoteHTMLAttributes<HTMLQuoteElement>;
	rp: HTMLAttributes<HTMLElement>;
	rt: HTMLAttributes<HTMLElement>;
	ruby: HTMLAttributes<HTMLElement>;
	s: HTMLAttributes<HTMLElement>;
	samp: HTMLAttributes<HTMLElement>;
	script: ScriptHTMLAttributes<HTMLScriptElement>;
	search: HTMLAttributes<HTMLElement>;
	section: HTMLAttributes<HTMLElement>;
	select: SelectHTMLAttributes<HTMLSelectElement>;
	slot: HTMLSlotElementAttributes<HTMLSlotElement>;
	small: HTMLAttributes<HTMLElement>;
	source: SourceHTMLAttributes<HTMLSourceElement>;
	span: HTMLAttributes<HTMLSpanElement>;
	strong: HTMLAttributes<HTMLElement>;
	style: StyleHTMLAttributes<HTMLStyleElement>;
	sub: HTMLAttributes<HTMLElement>;
	summary: HTMLAttributes<HTMLElement>;
	sup: HTMLAttributes<HTMLElement>;
	table: HTMLAttributes<HTMLTableElement>;
	tbody: HTMLAttributes<HTMLTableSectionElement>;
	td: TdHTMLAttributes<HTMLTableCellElement>;
	template: TemplateHTMLAttributes<HTMLTemplateElement>;
	textarea: TextareaHTMLAttributes<HTMLTextAreaElement>;
	tfoot: HTMLAttributes<HTMLTableSectionElement>;
	th: ThHTMLAttributes<HTMLTableCellElement>;
	thead: HTMLAttributes<HTMLTableSectionElement>;
	time: TimeHTMLAttributes<HTMLTimeElement>;
	title: HTMLAttributes<HTMLTitleElement>;
	tr: HTMLAttributes<HTMLTableRowElement>;
	track: TrackHTMLAttributes<HTMLTrackElement>;
	u: HTMLAttributes<HTMLElement>;
	ul: HTMLAttributes<HTMLUListElement>;
	var: HTMLAttributes<HTMLElement>;
	video: VideoHTMLAttributes<HTMLVideoElement>;
	wbr: HTMLAttributes<HTMLElement>;
	/** @url https://www.electronjs.org/docs/latest/api/webview-tag */
	webview: WebViewHTMLAttributes<HTMLElement>;
}
/** @type {SVGElementTagNameMap} */
export interface SVGElementTags {
	animate: AnimateSVGAttributes<SVGAnimateElement>;
	animateMotion: AnimateMotionSVGAttributes<SVGAnimateMotionElement>;
	animateTransform: AnimateTransformSVGAttributes<SVGAnimateTransformElement>;
	circle: CircleSVGAttributes<SVGCircleElement>;
	clipPath: ClipPathSVGAttributes<SVGClipPathElement>;
	defs: DefsSVGAttributes<SVGDefsElement>;
	desc: DescSVGAttributes<SVGDescElement>;
	ellipse: EllipseSVGAttributes<SVGEllipseElement>;
	feBlend: FeBlendSVGAttributes<SVGFEBlendElement>;
	feColorMatrix: FeColorMatrixSVGAttributes<SVGFEColorMatrixElement>;
	feComponentTransfer: FeComponentTransferSVGAttributes<SVGFEComponentTransferElement>;
	feComposite: FeCompositeSVGAttributes<SVGFECompositeElement>;
	feConvolveMatrix: FeConvolveMatrixSVGAttributes<SVGFEConvolveMatrixElement>;
	feDiffuseLighting: FeDiffuseLightingSVGAttributes<SVGFEDiffuseLightingElement>;
	feDisplacementMap: FeDisplacementMapSVGAttributes<SVGFEDisplacementMapElement>;
	feDistantLight: FeDistantLightSVGAttributes<SVGFEDistantLightElement>;
	feDropShadow: FeDropShadowSVGAttributes<SVGFEDropShadowElement>;
	feFlood: FeFloodSVGAttributes<SVGFEFloodElement>;
	feFuncA: FeFuncSVGAttributes<SVGFEFuncAElement>;
	feFuncB: FeFuncSVGAttributes<SVGFEFuncBElement>;
	feFuncG: FeFuncSVGAttributes<SVGFEFuncGElement>;
	feFuncR: FeFuncSVGAttributes<SVGFEFuncRElement>;
	feGaussianBlur: FeGaussianBlurSVGAttributes<SVGFEGaussianBlurElement>;
	feImage: FeImageSVGAttributes<SVGFEImageElement>;
	feMerge: FeMergeSVGAttributes<SVGFEMergeElement>;
	feMergeNode: FeMergeNodeSVGAttributes<SVGFEMergeNodeElement>;
	feMorphology: FeMorphologySVGAttributes<SVGFEMorphologyElement>;
	feOffset: FeOffsetSVGAttributes<SVGFEOffsetElement>;
	fePointLight: FePointLightSVGAttributes<SVGFEPointLightElement>;
	feSpecularLighting: FeSpecularLightingSVGAttributes<SVGFESpecularLightingElement>;
	feSpotLight: FeSpotLightSVGAttributes<SVGFESpotLightElement>;
	feTile: FeTileSVGAttributes<SVGFETileElement>;
	feTurbulence: FeTurbulanceSVGAttributes<SVGFETurbulenceElement>;
	filter: FilterSVGAttributes<SVGFilterElement>;
	foreignObject: ForeignObjectSVGAttributes<SVGForeignObjectElement>;
	g: GSVGAttributes<SVGGElement>;
	image: ImageSVGAttributes<SVGImageElement>;
	line: LineSVGAttributes<SVGLineElement>;
	linearGradient: LinearGradientSVGAttributes<SVGLinearGradientElement>;
	marker: MarkerSVGAttributes<SVGMarkerElement>;
	mask: MaskSVGAttributes<SVGMaskElement>;
	metadata: MetadataSVGAttributes<SVGMetadataElement>;
	mpath: MPathSVGAttributes<SVGMPathElement>;
	path: PathSVGAttributes<SVGPathElement>;
	pattern: PatternSVGAttributes<SVGPatternElement>;
	polygon: PolygonSVGAttributes<SVGPolygonElement>;
	polyline: PolylineSVGAttributes<SVGPolylineElement>;
	radialGradient: RadialGradientSVGAttributes<SVGRadialGradientElement>;
	rect: RectSVGAttributes<SVGRectElement>;
	set: SetSVGAttributes<SVGSetElement>;
	stop: StopSVGAttributes<SVGStopElement>;
	svg: SvgSVGAttributes<SVGSVGElement>;
	switch: SwitchSVGAttributes<SVGSwitchElement>;
	symbol: SymbolSVGAttributes<SVGSymbolElement>;
	text: TextSVGAttributes<SVGTextElement>;
	textPath: TextPathSVGAttributes<SVGTextPathElement>;
	tspan: TSpanSVGAttributes<SVGTSpanElement>;
	use: UseSVGAttributes<SVGUseElement>;
	view: ViewSVGAttributes<SVGViewElement>;
}
export interface MathMLElementTags {
	annotation: MathMLAnnotationElementAttributes<MathMLElement>;
	"annotation-xml": MathMLAnnotationXmlElementAttributes<MathMLElement>;
	math: MathMLMathElementAttributes<MathMLElement>;
	merror: MathMLMerrorElementAttributes<MathMLElement>;
	mfrac: MathMLMfracElementAttributes<MathMLElement>;
	mi: MathMLMiElementAttributes<MathMLElement>;
	mmultiscripts: MathMLMmultiscriptsElementAttributes<MathMLElement>;
	mn: MathMLMnElementAttributes<MathMLElement>;
	mo: MathMLMoElementAttributes<MathMLElement>;
	mover: MathMLMoverElementAttributes<MathMLElement>;
	mpadded: MathMLMpaddedElementAttributes<MathMLElement>;
	mphantom: MathMLMphantomElementAttributes<MathMLElement>;
	mprescripts: MathMLMprescriptsElementAttributes<MathMLElement>;
	mroot: MathMLMrootElementAttributes<MathMLElement>;
	mrow: MathMLMrowElementAttributes<MathMLElement>;
	ms: MathMLMsElementAttributes<MathMLElement>;
	mspace: MathMLMspaceElementAttributes<MathMLElement>;
	msqrt: MathMLMsqrtElementAttributes<MathMLElement>;
	mstyle: MathMLMstyleElementAttributes<MathMLElement>;
	msub: MathMLMsubElementAttributes<MathMLElement>;
	msubsup: MathMLMsubsupElementAttributes<MathMLElement>;
	msup: MathMLMsupElementAttributes<MathMLElement>;
	mtable: MathMLMtableElementAttributes<MathMLElement>;
	mtd: MathMLMtdElementAttributes<MathMLElement>;
	mtext: MathMLMtextElementAttributes<MathMLElement>;
	mtr: MathMLMtrElementAttributes<MathMLElement>;
	munder: MathMLMunderElementAttributes<MathMLElement>;
	munderover: MathMLMunderoverElementAttributes<MathMLElement>;
	semantics: MathMLSemanticsElementAttributes<MathMLElement>;
	/** @non-standard */
	menclose: MathMLMencloseElementAttributes<MathMLElement>;
	/** @deprecated */
	maction: MathMLMactionElementAttributes<MathMLElement>;
	/** @deprecated @non-standard */
	mfenced: MathMLMfencedElementAttributes<MathMLElement>;
}
/**
 * The DOM node type returned by JSX expressions.
 *
 * Union of `HTMLElement` plus `SVGElement`/`MathMLElement` when those
 * namespaces are enabled in {@link JSX.ConfigureElement}. Technically this
 * could also include `DocumentFragment`, but many DOM APIs expect `Element`,
 * so fragments are typed separately where needed.
 */
export type JSXElement = HTMLElement | (JSX.ConfigureElement["svg"] extends false ? never : SVGElement) | (JSX.ConfigureElement["mathml"] extends false ? never : MathMLElement);
/**
 * A single child that can be rendered inside a JSX element.
 *
 * Includes primitives (`string`/`number`), DOM nodes, iterables/arrays of
 * children, signal-like wrappers, shadow root containers, and the ignorable
 * `boolean`/`null`/`undefined` values (filtered similarly to React).
 */
export type ComponentChild = string | number | Iterable<ComponentChild> | Array<ComponentChild> | {
	value: ComponentChild;
	peek: () => ComponentChild;
	subscribe: (cb: (newValue: ComponentChild) => void) => void;
} | JSXElement | NodeList | ChildNode | HTMLCollection | ShadowRootContainer | DocumentFragment | Text | Comment | boolean | null | undefined;
/**
 * The children of a JSX element: a single child or an array of children.
 */
export type ComponentChildren = ComponentChild[] | ComponentChild;
export namespace JSX {
	// eslint-disable-next-line @typescript-eslint/no-empty-object-type
	/** Augment to declare custom HTML element tag/type mappings. */
	export interface CustomElementsHTML {
	}
	/** Toggles for optional JSX element namespaces. Set to `false` to exclude SVG/MathML from `JSXElement` / `IntrinsicElements`. */
	export interface ConfigureElement {
		/** When `false`, SVG elements are excluded from the JSX element union. */
		svg: boolean;
		/** When `false`, MathML elements are excluded from the JSX element union. */
		mathml: boolean;
	}
	type Element = JSXElement;
	interface ElementAttributesProperty {
		props: unknown;
	}
	interface ElementChildrenAttribute {
		children: {};
	}
	interface IntrinsicClassAttributes<T> {
		ref?: Ref<T>;
	}
	type IntrinsicElementsCombined = HTMLElementTags & (ConfigureElement["svg"] extends false ? void : SVGElementTags) & (ConfigureElement["mathml"] extends false ? void : MathMLElementTags);
	// eslint-disable-next-line @typescript-eslint/no-empty-object-type
	export interface CustomElementsHTML {
	}
	interface IntrinsicElements extends IntrinsicElementsCombined, CustomElementsHTML {
	}
}
/**
 * A class-based JSX component. Extend `Component` or implement this interface
 * and override `render` to return a `JSXElement`.
 * @typeParam P - The type of the component's props.
 * @typeParam T - The type of the DOM node the component renders.
 */
export interface ComponentClass<P = {}, T extends Node = JSXElement> {
	/**
	 * Constructs the component with the given props.
	 * @param props - Props including optional `children`.
	 */
	new (props: P): ComponentClass<P, T>;
	/**
	 * Renders the component.
	 * @returns The rendered `JSXElement` or `null`.
	 */
	render(): JSXElement | null;
	/** Optional default prop values merged in by the JSX factory before construction. */
	defaultProps?: Partial<P> | undefined;
	/** Props passed to the instance, including optional `children`. */
	readonly props?: P & {
		children?: ComponentChildren;
	};
	/** Optional display name used in devtools / error messages. */
	displayName?: string | undefined;
}
/**
 * A function-based JSX component that receives props (including `children`)
 * and returns a `JSXElement` or `null`.
 * @typeParam P - The type of the component's props.
 * @typeParam T - The type of the DOM node the component renders.
 */
export type FunctionComponent<P = {}, T extends Node = JSXElement> = (props: P & {
	children?: ComponentChildren;
}) => T | null;
/**
 * A JSX component: either a class-based or a function-based component.
 * @typeParam P - The type of the component's props.
 * @typeParam T - The type of the DOM node the component renders.
 */
export type ComponentType<P = {}, T extends Node = JSXElement> = ComponentClass<P, T> | FunctionComponent<P, T>;
/**
 * A virtual descriptor for a `ShadowRoot` created by `ShadowRootNode`.
 * Recognized by the JSX factory to create a shadow root on the parent element
 * via `attachShadow`. See {@link ShadowRootNode}.
 */
export type ShadowRootContainer = {
	/** Optional ref that receives the created `ShadowRoot`. */
	ref: Ref<ShadowRoot>;
	/** `ShadowRootInit` options forwarded to `attachShadow`. */
	attr: {
		/** Whether the shadow root should be clonable. */
		clonable?: boolean;
		/** Custom element registry for the shadow tree. */
		customElementRegistry?: CustomElementRegistry;
		/** Whether focus should delegate to the shadow host. */
		delegatesFocus?: boolean;
		/** Shadow root mode (`"open"` or `"closed"`). */
		mode: ShadowRootMode;
		/** Whether the shadow root is serializable. */
		serializable?: boolean;
		/** Slot assignment mode (`"manual"` or `"named"`). */
		slotAssignment?: SlotAssignmentMode;
	};
	/** Children rendered inside the shadow root. */
	children: ComponentChildren;
};
/**
 * Creates a proxy that automatically binds method calls to the given object.
 * Intended for use in classes, e.g. when attaching event handlers.
 * Instead of `someElement.addEventListener("click", this.onClick.bind(this))`
 * or an arrow function `(e) => this.onClick(e)` (both of which hurt performance
 * and complicate `removeEventListener` because the bound function must be stored),
 * you can write:
 *
 * ```ts
 * const boundThis = bindThis(this);
 * someElement.addEventListener("click", boundThis.onClick);
 * // later, in dispose:
 * someElement.removeEventListener("click", this.onClick);
 * ```
 *
 * The returned proxy lazily binds methods on first access and caches the bound
 * function in the target object. Subsequent accesses return the same cached
 * function, making it safe to use with `removeEventListener` by passing the
 * **original** method (e.g. `this.onClick`). There is no need to call `bindThis`
 * again in dispose — calling it a second time returns the same proxy.
 * Non-function properties are returned as-is.
 *
 * @param obj - The object whose methods should be auto-bound.
 * @returns A proxy wrapping the object with auto-bound method access.
 */
export declare function bindThis<T>(obj: T): T;
/**
 * Converts a heterogeneous class value to a normalized space-separated class string.
 *
 * Handles strings, nested arrays (recursively flattened), any iterable (e.g. `Set`),
 * and dictionary objects where only keys with truthy values are included.
 * Falsy primitives, `true`/`false`, `null` and `undefined` produce an empty string.
 *
 * @param value - The class value to normalize. May be a string, array, iterable,
 * dictionary (`Record<string, boolean>`), primitive, or nested combination thereof.
 * @returns A space-separated class name string, or `""` when the input yields no classes.
 * @example
 * ```ts
 * className("foo bar") // => "foo bar"
 * className(["foo", ["bar", { baz: true, qux: false }]]) // => "foo bar baz"
 * className(new Set(["a", "b"])) // => "a b"
 * ```
 */
export declare function className(value: any): string;
/**
 * Creates a JSX element using the classic (non-automatic) JSX factory signature.
 *
 * Children are passed as variadic rest arguments after `attr`. For compatibility,
 * if `attr` itself is a string or array it is treated as the first child and
 * `attr` is replaced with `{}`. If `attr.children` is set and no explicit
 * `children` were supplied, the `children` property is extracted from `attr`.
 *
 * Prefer {@link jsx} when using the automatic JSX runtime (`"jsx": "automatic"`).
 *
 * @param tag - HTML/SVG tag name or a component function/class.
 * @param attr - Attributes/props for the element, or the first child when a
 * string or array. May be `null`/`undefined` when no attributes are needed.
 * @param children - Child elements passed as rest arguments.
 * @returns The created JSX DOM node.
 */
export declare function createElement(tag: any, attr: any, ...children: any[]): JSXElement;
/**
 * Compatibility helper similar to React's `useImperativeHandle`.
 *
 * Evaluates `init()` and forwards the result to `ref` via {@link setRef}.
 * Prefer calling {@link setRef} directly in new code.
 *
 * @typeParam T - Type of the value exposed through the ref.
 * @param ref - Target `RefObject` or ref callback to update.
 * @param init - Factory that produces the value to assign to the ref.
 */
export declare function useImperativeHandle<T>(ref: Ref<T>, init: () => T): void;
/**
 * Base class for class-based JSX components.
 *
 * Extend this class and override {@link render} to return a {@link JSXElement}.
 * Props (including optional `children` and `ref`) are available via {@link props}.
 *
 * @typeParam T - The type of the component's props (excluding `children` and `ref` which are added automatically).
 * @example
 * ```tsx
 * class Greeting extends Component<{ name: string }> {
 *   render() {
 *     return <div>Hello, {this.props.name}!</div>;
 *   }
 * }
 * // usage: <Greeting name="World" />
 * ```
 */
export declare class Component<T = any> {
	/**
	 * Marker used by the JSX factory to distinguish class components from function components.
	 * Do not modify.
	 */
	static isComponent: boolean;
	/**
	 * Creates a component instance.
	 * @param props - Props passed to the component, including optional `children` and `ref`.
	 */
	constructor(props: T & {
		children?: ComponentChildren;
		ref?: Ref<any>;
	});
	/** Props passed to this component instance, including optional `children` and `ref`. */
	readonly props: T & {
		children?: ComponentChildren;
		ref?: Ref<any>;
	};
	/**
	 * Renders the component's output.
	 * Override in subclasses to return a DOM node, fragment, or `null`.
	 * @returns The rendered {@link JSXElement}, or `null` to render nothing.
	 */
	render(): JSXElement | null;
}
/**
 * Dispatches a `disposing` event on the target element, causing any
 * listeners registered via {@link addDisposingListener} to be invoked.
 * No-ops when `target` is falsy or `CustomEvent` is unavailable.
 * @param target - Event target to dispatch the event on.
 * @param opt - Optional event configuration.
 * @param opt.bubbles - Whether the event should bubble. Defaults to `false`.
 * @param opt.cancelable - Whether the event is cancelable. Defaults to `false`.
 */
export declare function dispatchDisposingEvent(target: EventTarget, opt?: {
	bubbles?: boolean;
	cancelable?: boolean;
}): void;
/**
 * Synchronously invokes all disposing listeners registered for `node` and
 * removes the internal `disposing` DOM listener from the target.
 *
 * This does **not** dispatch a `disposing` DOM event; use
 * {@link dispatchDisposingEvent} for that. Listener errors are swallowed.
 *
 * @param node - Target whose disposing listeners should be invoked. No-op when falsy.
 * @param opt - Optional behavior flags.
 * @param opt.descendants - When `true`, also invokes listeners registered on descendant elements/text/comment nodes found via `createNodeIterator`.
 * @param opt.excludeSelf - When `true`, skips listeners registered directly on `node` itself (only descendants are invoked, in combination with `descendants`).
 */
export declare function invokeDisposingListeners(node: EventTarget, opt?: {
	descendants?: boolean;
	excludeSelf?: boolean;
}): void;
/**
 * Registers a disposing listener for an element.
 *
 * The `handler` is not added as a direct DOM event listener; instead it is
 * stored in an internal `WeakMap` and invoked when a `disposing` event is
 * dispatched on `target` (via {@link dispatchDisposingEvent} or
 * {@link invokeDisposingListeners}). The first registration on a given target
 * also installs a one-shot `disposing` event listener to drive the callback
 * list. Duplicate `handler` references are ignored (with optional `regKey`
 * tracking), so calling this multiple times with the same callback is safe.
 *
 * @typeParam T - Type of the target event target.
 * @param target - Element/event target to attach the listener to. No-op when `null`/`undefined`.
 * @param handler - Callback invoked with the element when it is disposing. No-op when `null`/`undefined`.
 * @param regKey - Optional registration key used to de-duplicate or later remove this listener.
 * @returns The `target` that was passed in, for chaining.
 * @throws {Error} When the same `handler` is already registered with a different `regKey`.
 */
export declare function addDisposingListener<T extends EventTarget>(target: T | null | undefined, handler: ((el: T) => void) | undefined | null, regKey?: string): T | null | undefined;
/**
 * Removes a previously registered disposing listener from an element.
 *
 * This removes the entry from the internal disposing-listener registry, not a
 * direct DOM `EventListener`. A listener matches when either its `handler`
 * reference equals the stored callback or its `regKey` equals the stored key.
 * When the last listener is removed the underlying `disposing` DOM listener
 * is also detached from the target.
 *
 * @typeParam T - Type of the target event target.
 * @param target - Element/event target to remove the listener from. No-op when `null`/`undefined`.
 * @param handler - Callback whose registration should be removed. If `null`/`undefined`, matching falls back to `regKey`.
 * @param regKey - Optional registration key to match against.
 * @returns The `target` that was passed in, for chaining.
 */
export declare function removeDisposingListener<T extends EventTarget>(target: T | null | undefined, handler: (() => void) | undefined | null, regKey?: string | undefined | null): T | null | undefined;
/**
 * Gets or sets the current JSX lifecycle root element used to scope signal subscriptions.
 *
 * The lifecycle root is the `EventTarget` whose `disposing` event will dispose
 * effects created during JSX construction (e.g. via `observeSignal` with
 * `useLifecycleRoot: true`).
 *
 * @param args - When provided, the first element is installed as the new lifecycle root.
 * When called with no arguments the current root (or `null` if none) is returned.
 * @returns The current lifecycle root, or the previous root when a new one is being set. Returns `null` if none is set.
 */
export declare function currentLifecycleRoot(...args: Element[]): Element | null;
/**
 * Creates a `DocumentFragment` containing the given children.
 *
 * Intended as the JSX fragment factory (i.e. the target for the `<></>` syntax
 * when `jsxFragment` is set to `Fragment`). Accepts the standard
 * `{ children }` props bag produced by the JSX transform.
 *
 * @param attr - Props bag with optional `children` to append to the fragment.
 * @returns A `DocumentFragment` containing the appended children.
 */
export declare function Fragment(attr: {
	children?: ComponentChildren | undefined;
}): any;
/**
 * Creates a new sealed `RefObject` whose `current` is initially `null`.
 *
 * The returned object is `Object.seal`ed so that no new properties can be
 * added, matching the `React.createRef` contract.
 *
 * @typeParam T - Type of the value held by the ref.
 * @returns A sealed `RefObject<T>` with `current` set to `null`.
 * @example
 * ```tsx
 * const inputRef = createRef<HTMLInputElement>();
 * return <input ref={inputRef} />;
 * // later: inputRef.current?.focus();
 * ```
 */
export declare function createRef<T = any>(): RefObject<T>;
/**
 * Assigns a value to a ref, handling both object and callback forms.
 *
 * - If `ref` is a `RefObject`, its `current` property is set to `current`.
 * - If `ref` is a function, it is invoked with `current`.
 * - If `ref` is `null`/`undefined` or neither form, no action is taken.
 *
 * @typeParam T - Type of the node/value being assigned.
 * @param ref - Target `RefObject`, callback, or `null`/`undefined`.
 * @param current - Value to assign to the ref.
 */
export declare function setRef<T = Node>(ref: Ref<T> | undefined, current: T): void;
/**
 * Creates a `classList`-like manager that can be used as a JSX prop hook for the `class` attribute.
 *
 * The returned {@link BasicClassList} is a callable that also implements
 * `add`/`remove`/`toggle`/`contains` plus `value`/`size`, mirroring the native
 * `DOMTokenList` API. When assigned to `class` (e.g. `<div class={cls} />`),
 * the hook binds to the element's `classList` and keeps it in sync; the
 * binding is cleaned up automatically when the element is disposed.
 * Before binding, an optional `initialValue` is used to seed a detached
 * `classList` so that `add`/`remove` calls prior to attachment are preserved.
 *
 * @param initialValue - Optional initial class value (string, array, dictionary, iterable, or `DOMTokenList`).
 * @returns A `BasicClassList` instance that is both callable and a prop hook.
 * @example
 * ```tsx
 * const cls = useClassList("foo");
 * cls.add("bar");
 * return <div class={cls} />;
 * ```
 */
export declare function useClassList(initialValue?: ClassNames): BasicClassList;
/**
 * Creates a two-way prop binding hook that synchronizes a value to a single element attribute.
 *
 * The returned {@link PropBinding} is a callable getter/setter that also
 * implements the prop-hook protocol. When the binding is assigned to a JSX
 * attribute (e.g. `<input value={binding} />`), it attaches to that element
 * and calls `assignProp` on every subsequent `binding(newValue)`. The hook
 * may only be bound once — reusing the same binding on a different element
 * or attribute throws.
 *
 * @typeParam T - Type of the bound prop value.
 * @param initialValue - Optional initial value stored prior to element attachment.
 * @returns A `PropBinding<T>` callable that reads the current value when called
 * with no arguments, and writes a new value when called with an argument.
 * @example
 * ```tsx
 * const value = usePropBinding("hello");
 * return <><input value={value} /><button onClick={() => value("world")} /></>;
 * ```
 */
export declare function usePropBinding<T>(initialValue?: T | null | undefined | false): PropBinding<T>;
/**
 * Creates a `Text` node and a setter to update its content.
 *
 * The node's `toString()` is overridden to return `textContent`, so the
 * returned `Text` can be interpolated directly as a JSX child and will
 * render its string value.
 *
 * @param initialValue - Optional initial text content. If omitted the node starts empty.
 * @returns A readonly tuple `[textNode, setText]` where `setText` assigns `textContent`.
 * @example
 * ```tsx
 * const [label, setLabel] = useText("hello");
 * return <><span>{label}</span><button onClick={() => setLabel("world")} /></>;
 * ```
 */
export declare function useText(initialValue?: string): readonly [
	text: Text,
	setText: (value: string) => void
];
/**
 * Gets or sets the ambient JSX namespace URI used for `createElement`/`jsx`.
 *
 * Stored on `globalThis` under `Serenity.jsxNamespaceURI`. When the active
 * namespace is `"http://www.w3.org/2000/svg"` (or MathML), elements created
 * without an explicit `namespaceURI` prop are created via `createElementNS`.
 *
 * @param value - When arguments are supplied, the namespace is set to this
 * value (use `null` to reset to the HTML namespace). When called with no
 * arguments the current value is simply returned.
 * @returns The current namespace URI (no-arg call), or the previous value
 * (setter call). May be `null`/`undefined` when no override is active.
 */
export declare function currentNamespaceURI(value?: string | null | undefined): string | null | undefined;
/**
 * Executes a children factory within a scoped namespace URI.
 *
 * Temporarily sets {@link currentNamespaceURI} to `namespaceURI` for the
 * duration of `children()`, then restores the previous value (even if the
 * factory throws). This lets you create SVG/MathML subtrees imperatively
 * without setting `namespaceURI` on every element.
 *
 * @param namespaceURI - Namespace URI to activate, or `null` for the HTML namespace.
 * @param children - Factory that produces the children to render in the given namespace.
 * @returns The children returned by the factory.
 * @example
 * ```tsx
 * const icon = inNamespaceURI(SVGNamespace, () => <><circle r={10} /><path d="M0 0" /></>);
 * ```
 */
export declare function inNamespaceURI(namespaceURI: string | null, children: () => ComponentChildren): ComponentChildren;
/**
 * Executes a children factory within the SVG namespace (`http://www.w3.org/2000/svg`).
 * Sugar over {@link inNamespaceURI} with {@link SVGNamespace}.
 * @param fn - Factory that returns children to create as SVG elements.
 * @returns The children produced by the factory.
 */
export declare function inSVGNamespace(fn: () => ComponentChildren): ComponentChildren;
/**
 * Executes a children factory within the MathML namespace (`http://www.w3.org/1998/Math/MathML`).
 * Sugar over {@link inNamespaceURI} with {@link MathMLNamespace}.
 * @param fn - Factory that returns children to create as MathML elements.
 * @returns The children produced by the factory.
 */
export declare function inMathMLNamespace(fn: () => ComponentChildren): ComponentChildren;
/**
 * Executes a children factory within the HTML namespace (clears any active SVG/MathML override).
 * Sugar over {@link inNamespaceURI} with `null`.
 * @param fn - Factory that returns children to create as HTML elements.
 * @returns The children produced by the factory.
 */
export declare function inHTMLNamespace(fn: () => ComponentChildren): ComponentChildren;
type DataKeys = `data-${string}`;
/**
 * Creates a JSX element. This is the automatic JSX factory used by the
 * compiler (imported as `jsx` and `jsxs`). Handles HTML/SVG/MathML elements
 * and custom function or class components.
 *
 * When `type` is a string, a real DOM element is created (with namespace
 * auto-detection for SVG/MathML), props are assigned via `assignProps`, and
 * children are appended. `select[value]` signals are resolved and applied.
 * When `type` is a function/class, it is invoked or instantiated as a
 * component and the resulting node is returned. `defaultProps` are respected
 * and `ref` is forwarded via {@link setRef}.
 *
 * Unlike {@link createElement} / `h`, children are expected inside `props`
 * (`props.children`) rather than as rest arguments.
 *
 * @param type - HTML/SVG/MathML tag name or a component function/class.
 * @param props - Attributes/props for the element. Children are read from `props.children`; may be `null`.
 * @returns The created DOM node (or component render result).
 */
export declare function jsx<THtmlTag extends (keyof HTMLElementTagNameMap & keyof HTMLElementTags), TElement extends HTMLElementTagNameMap[THtmlTag]>(type: THtmlTag, props?: (HTMLElementTags[THtmlTag] & Record<DataKeys, string | number>) | null): TElement;
export declare function jsx<TSVGTag extends (keyof SVGElementTagNameMap & keyof SVGElementTags), TElement extends SVGElementTagNameMap[TSVGTag]>(type: TSVGTag, props?: (SVGElementTags[TSVGTag] & Record<DataKeys, string | number>) | null): TElement;
export declare function jsx(type: string, props?: (ElementAttributes<JSXElement> & Record<DataKeys, string | number>) | null): JSXElement;
export declare function jsx<P extends {}, TElement extends JSXElement = JSXElement>(type: ComponentType<P, TElement>, props?: P & {
	children?: ComponentChildren;
	ref?: Ref<TElement>;
} | null): TElement;
/** The MathML namespace URI (`http://www.w3.org/1998/Math/MathML`). */
export declare const MathMLNamespace = "http://www.w3.org/1998/Math/MathML";
/**
 * Well-known symbol (`Symbol.for("Serenity.initPropHook")`) that marks a value
 * as a JSX prop hook.
 *
 * When a prop value that carries this symbol is assigned as a JSX attribute,
 * the renderer invokes `value[initPropHookSymbol](node, propName)` to let the
 * hook bind itself to the DOM node (e.g. to observe signals or install
 * class-list synchronization). See also the {@link PropHook} interface.
 */
export declare const initPropHookSymbol: unique symbol;
export interface PropHook<TNode extends Element = Element> {
	[initPropHookSymbol](node: TNode, propName: string): void;
}
/**
 * Creates a virtual `ShadowRoot` descriptor recognized by the JSX factory.
 *
 * When a `ShadowRootContainer` produced by this function appears among a
 * parent element's children (e.g. `<div><ShadowRootNode mode="open">…</ShadowRootNode></div>`),
 * the factory calls `parent.attachShadow(init)` and appends the `children`
 * into the resulting `ShadowRoot`. An optional `ref` is forwarded to the
 * created `ShadowRoot`.
 *
 * @param options - Shadow root init options (`mode`, `delegatesFocus`, etc.) plus optional `ref` and `children`.
 * @param options.mode - Shadow root mode (`"open"` or `"closed"`).
 * @param options.delegatesFocus - Whether focus delegation is enabled.
 * @param options.slotAssignment - How slots are assigned (`"manual"` or `"named"`).
 * @param options.clonable - Whether the shadow root is clonable.
 * @param options.serializable - Whether the shadow root is serializable.
 * @param options.customElementRegistry - Custom element registry for the shadow tree.
 * @param options.ref - Optional ref to receive the created `ShadowRoot`.
 * @param options.children - Children to render inside the shadow root.
 * @returns A virtual node descriptor that the JSX factory consumes to create the shadow root.
 * @example
 * ```tsx
 * <div><ShadowRootNode mode="open"><span>inside shadow</span></ShadowRootNode></div>
 * ```
 */
export declare function ShadowRootNode({ children, ref, ...attr }: ShadowRootInit & {
	ref?: Ref<ShadowRoot>;
	children?: ComponentChildren;
}): any;
/**
 * A type guard that checks if an object is signal-like, meaning it has `subscribe` and `peek` methods,
 * and a `value` property.
 * @param obj - The object to check.
 * @returns `true` if the object is signal-like.
 */
export declare function isSignalLike<T = any>(obj: any): obj is SignalLike<T>;
type SignalObserveArgs<T> = {
	/** `true` on the initial synchronous invocation right after subscription; `false` thereafter. */
	isInitial: boolean;
	/** Value from the previous callback invocation. `undefined` on the initial call. */
	prevValue: T | undefined;
	/** Current value of the signal for this invocation. `undefined` if unavailable. */
	newValue: T | undefined;
	/** `true` when `newValue !== prevValue`; always `false` on the initial call. */
	hasChanged: boolean;
	/** The observed signal instance. */
	readonly signal: SignalLike<T>;
	/**
	 * Disposer for the underlying subscription. Only non-null when the signal
	 * library supports unsubscription; assign `null`/`undefined` to clear it.
	 * Reassigning also updates the lifecycle-bound disposing listeners.
	 */
	effectDisposer: EffectDisposer | null | undefined;
	/**
	 * Lifecycle root captured at subscription time when `useLifecycleRoot` was
	 * `true`; otherwise `undefined`. See {@link currentLifecycleRoot}.
	 */
	readonly lifecycleRoot: EventTarget | null | undefined;
	/**
	 * Lifecycle node that owns the subscription — the disposer is registered
	 * as a disposing listener on this node. Getter returns the current node.
	 */
	get lifecycleNode(): EventTarget | null | undefined;
	/**
	 * Sets the lifecycle node that owns the subscription. Changing it moves
	 * the disposing listener registration from the old node to the new one.
	 */
	set lifecycleNode(value: EventTarget | null | undefined);
};
type ObserveSignalCallback<T> = (args: SignalObserveArgs<T>) => void;
/**
 * Subscribes to a signal and invokes `callback` immediately and on every subsequent change.
 *
 * On subscription a {@link SignalObserveArgs} object is created and `callback` is
 * invoked synchronously with `isInitial: true`. Future notifications update
 * `newValue`/`prevValue`/`hasChanged` and invoke `callback` again. The
 * returned disposer (when non-null) can be used to unsubscribe; it is also
 * automatically registered as a disposing listener on `lifecycleNode` /
 * `lifecycleRoot` so it is cleaned up when the owning DOM node is disposed.
 *
 * @typeParam T - Type of the signal's value.
 * @param signal - Signal-like object to observe (must have `subscribe`/`peek`/`value`).
 * @param callback - Function called initially and on each change.
 * @param opt - Optional lifecycle wiring.
 * @param opt.useLifecycleRoot - When `true`, captures {@link currentLifecycleRoot} at call time as the lifecycle root.
 * @param opt.lifecycleNode - Explicit node whose `disposing` event will dispose the subscription.
 * @returns A disposer function for the subscription, or `null`/`undefined` if the signal does not expose one.
 */
export declare function observeSignal<T>(signal: SignalLike<T>, callback: ObserveSignalCallback<T>, opt?: {
	/**
	 * When `true`, the current lifecycle root (see {@link currentLifecycleRoot})
	 * at subscription time is recorded as {@link SignalObserveArgs.lifecycleRoot}.
	 */
	useLifecycleRoot?: boolean;
	/**
	 * Optional DOM node whose `disposing` event will automatically dispose the
	 * subscription via {@link addDisposingListener}.
	 */
	lifecycleNode?: EventTarget;
}): EffectDisposer | null | undefined;
interface DerivedSignalLike<T> extends SignalLike<T> {
	/** Optional disposer that unsubscribes the derived signal from its source. */
	derivedDisposer?: () => void;
}
/**
 * Creates a derived (computed) signal from a source signal and a transform.
 *
 * When the source signal changes, the derived value is re-computed via `fn`.
 * If the source signal's constructor appears to be a computed-capable type,
 * a new instance of that constructor wrapping `() => fn(input.value)` is
 * attempted; otherwise a lightweight {@link PrimitiveComputed} fallback is
 * used. The returned signal exposes a `derivedDisposer` that unsubscribes
 * from the source.
 *
 * @typeParam TDerived - Type of the derived/computed value.
 * @typeParam TInput - Type of the source signal's value.
 * @param input - Source signal to derive from. Must be signal-like.
 * @param fn - Transform applied to the source value to produce the derived value.
 * @returns A `DerivedSignalLike<TDerived>` whose `value` tracks `fn(input.value)`.
 * @throws {Error} When `input` is not signal-like.
 */
export declare function derivedSignal<TDerived, TInput = any>(input: SignalLike<TInput>, fn: (value: TInput) => TDerived): DerivedSignalLike<TDerived>;
/**
 * Options for creating a signal via {@link signal} / {@link computed}.
 * Re-exported from `@preact/signals-core`.
 * @typeParam T - Type of the signal's value.
 */
export interface SignalOptions<T> {
	/** Called when the signal gains its first subscriber. */
	watched?: (this: SignalLike<T>) => void;
	/** Called when the signal loses its last subscriber. */
	unwatched?: (this: SignalLike<T>) => void;
	/** Optional debug name for the signal. */
	name?: string;
}
/**
 * Options for creating an effect via {@link effect}.
 */
export interface EffectOptions {
	/** Optional debug name for the effect. */
	name?: string;
}
type EffectFn = ((this: {
	dispose: () => void;
}) => void | (() => void)) | (() => void | (() => void));
/**
 * Creates a new writable signal with an optional initial value.
 * Re-exported from `@preact/signals-core` with typed overloads.
 * @typeParam T - Type of the signal's value.
 * @param value - Optional initial value for the signal. When omitted the signal starts as `undefined`.
 * @param options - Optional signal options (`watched`, `unwatched`, `name`).
 * @returns A writable `Signal<T>`.
 * @example
 * ```ts
 * const count = signal(0);
 * count.value++;
 * ```
 */
export declare const signal: {
	<T>(value: T, options?: SignalOptions<T>): Signal<T>;
	<T = undefined>(): Signal<T | undefined>;
};
/**
 * Creates a computed (derived) signal that re-computes when its dependencies change.
 * Re-exported from `@preact/signals-core`.
 * @typeParam T - The type of the computed value.
 * @param fn - A computation function that returns the derived value.
 * @param options - Optional signal options.
 * @returns A read-only `Computed<T>` signal.
 */
export declare const computed: (<T>(fn: () => T, options?: SignalOptions<T>) => Computed<T>);
/**
 * Creates an effect that runs whenever its signal dependencies change.
 * Re-exported from `@preact/signals-core`.
 * @param fn - The effect function. May optionally return a cleanup callback.
 * @param options - Optional effect options (`name`).
 * @returns A disposer function to stop the effect.
 */
export declare const effect: ((fn: EffectFn, options?: EffectOptions) => () => void);
/**
 * Batches multiple signal updates into a single notification.
 * Re-exported from `@preact/signals-core`.
 * @typeParam T - The return type of the batch function.
 * @param fn - A function that performs batched signal updates.
 * @returns The return value of `fn`.
 */
export declare const batch: (<T>(fn: () => T) => T);
/**
 * Reads signal values without creating a dependency tracking context.
 * Re-exported from `@preact/signals-core`.
 * @typeParam T - The return type of the function.
 * @param fn - A function that reads signals without tracking them.
 * @returns The return value of `fn`.
 */
export declare const untracked: (<T>(fn: () => T) => T);
/**
 * Creates a writable signal with the given initial value.
 * Convenience wrapper around {@link signal}.
 * @typeParam T - Type of the signal's value.
 * @param initialValue - Initial value for the signal.
 * @returns A `Signal<T>` instance.
 * @example
 * ```ts
 * const name = useSignal("Alice");
 * name.value = "Bob";
 * ```
 */
export declare function useSignal<T>(initialValue: T): Signal<T>;
/**
 * Creates a factory for computed signals that can be manually invalidated in batch.
 *
 * Computed signals produced by the returned `computed` wrapper depend on an
 * internal `updater` signal; calling `update()` bumps that signal so every
 * derived computed re-evaluates on its next read, without wiring each one to
 * a separate source.
 *
 * @returns An object with:
 *  - `computed` — factory that wraps a computation so it tracks the shared updater.
 *  - `update` — bumps the updater, invalidating all computeds created from this factory.
 * @example
 * ```ts
 * const { computed: uc, update } = useUpdatableComputed();
 * const derived = uc(() => expensiveRead());
 * // later: after external state changes
 * update();
 * ```
 */
export declare function useUpdatableComputed(): {
	computed: <T>(fn: () => T) => Computed<T>;
	update: () => void;
};
/** The SVG namespace URI (`http://www.w3.org/2000/svg`). */
export declare const SVGNamespace = "http://www.w3.org/2000/svg";

export {
	createElement as h,
	createRef as useRef,
	jsx as jsxs,
};

export {};
