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
export type RemoveIndex<T> = {
	[K in keyof T as string extends K ? never : number extends K ? never : K]: T[K];
};
export type ExcludeMethods<T> = Pick<T, {
	[K in keyof T]: T[K] extends Function ? never : K;
}[keyof T]>;
export type StyleAttributes = Partial<ExcludeMethods<RemoveIndex<Omit<CSSStyleDeclaration, "length" | "parentRules">>>>;
/** CSSStyleDeclaration contains methods, readonly properties and an index signature, which we all need to filter out. */
export type StyleProperties = Partial<Pick<CSSStyleDeclaration, {
	[K in keyof CSSStyleDeclaration]: K extends string ? CSSStyleDeclaration[K] extends string ? K : never : never;
}[keyof CSSStyleDeclaration]>>;
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
export type ComponentChild = string | number | Iterable<ComponentChild> | Array<ComponentChild> | {
	value: ComponentChild;
	peek: () => ComponentChild;
	subscribe: (cb: (newValue: ComponentChild) => void) => void;
} | JSXElement | NodeList | ChildNode | HTMLCollection | ShadowRootContainer | DocumentFragment | Text | Comment | boolean | null | undefined;
export type ComponentChildren = ComponentChild[] | ComponentChild;
export interface ComponentClass<P = {}, T extends Node = JSXElement> {
	new (props: P): ComponentClass<P, T>;
	render(): JSXElement | null;
	defaultProps?: Partial<P> | undefined;
	readonly props?: P & {
		children?: ComponentChildren;
	};
	displayName?: string | undefined;
}
export interface BaseProps {
	children?: ComponentChildren;
}
export type FunctionComponent<P = BaseProps, T extends Node = JSXElement> = (props: P & {
	children?: ComponentChildren;
}) => T | null;
export type ComponentType<P = {}, T extends Node = JSXElement> = ComponentClass<P, T> | FunctionComponent<P, T>;
export type ComponentAttributes = {
	[s: string]: string | number | boolean | undefined | null | StyleAttributes | EventListenerOrEventListenerObject;
};
export interface SignalLike<T> {
	value: T;
	peek(): T;
	subscribe(fn: (value: T) => void): () => void;
}
export type Signalish<T> = T | SignalLike<T>;
export interface HTMLAttributes<T> {
	accept?: Signalish<string>;
	acceptCharset?: Signalish<string>;
	accessKey?: Signalish<string>;
	action?: Signalish<string>;
	allow?: Signalish<string>;
	allowFullscreen?: Signalish<boolean>;
	alt?: Signalish<string>;
	as?: Signalish<string>;
	async?: Signalish<boolean>;
	autocapitalize?: Signalish<string>;
	autocomplete?: Signalish<string>;
	autocorrect?: Signalish<string>;
	autofocus?: Signalish<boolean>;
	autoplay?: Signalish<boolean>;
	capture?: Signalish<boolean | string>;
	cellPadding?: Signalish<number | string>;
	cellSpacing?: Signalish<number | string>;
	charset?: Signalish<string>;
	checked?: Signalish<boolean>;
	class?: Signalish<ClassNames>;
	className?: Signalish<ClassNames>;
	cols?: Signalish<number>;
	colSpan?: Signalish<number>;
	colspan?: Signalish<number>;
	content?: Signalish<string>;
	contentEditable?: Signalish<boolean>;
	controls?: Signalish<boolean>;
	coords?: Signalish<string>;
	crossOrigin?: Signalish<string>;
	data?: Signalish<string>;
	dataset?: Signalish<{
		[key: string]: string;
	} | undefined>;
	dateTime?: Signalish<string>;
	default?: Signalish<boolean>;
	defer?: Signalish<boolean>;
	dir?: Signalish<"auto" | "rtl" | "ltr">;
	disabled?: Signalish<boolean>;
	disableRemotePlayback?: Signalish<boolean>;
	download?: Signalish<string | boolean>;
	decoding?: Signalish<"sync" | "async" | "auto">;
	draggable?: Signalish<"true" | "false">;
	enctype?: Signalish<string>;
	enterKeyHint?: Signalish<"enter" | "done" | "go" | "next" | "previous" | "search" | "send">;
	form?: Signalish<string>;
	formAction?: Signalish<string>;
	formEnctype?: Signalish<string>;
	formMethod?: Signalish<string>;
	formNoValidate?: Signalish<boolean>;
	formTarget?: Signalish<string>;
	frameBorder?: Signalish<number | string>;
	headers?: Signalish<string>;
	height?: Signalish<number | string>;
	hidden?: Signalish<boolean>;
	high?: Signalish<number>;
	href?: Signalish<string>;
	hreflang?: Signalish<string>;
	for?: Signalish<string>;
	htmlFor?: Signalish<string>;
	httpEquiv?: Signalish<string>;
	id?: Signalish<string>;
	innerText?: Signalish<string | undefined>;
	inputMode?: Signalish<string>;
	integrity?: Signalish<string>;
	is?: Signalish<string>;
	kind?: Signalish<string>;
	label?: Signalish<string>;
	lang?: Signalish<string>;
	list?: Signalish<string>;
	loading?: Signalish<"eager" | "lazy">;
	loop?: Signalish<boolean>;
	low?: Signalish<number>;
	marginHeight?: Signalish<number>;
	marginWidth?: Signalish<number>;
	max?: Signalish<number | string>;
	maxLength?: Signalish<number>;
	media?: Signalish<string>;
	method?: Signalish<string>;
	min?: Signalish<number | string>;
	minLength?: Signalish<number>;
	multiple?: Signalish<boolean>;
	muted?: Signalish<boolean>;
	name?: Signalish<string>;
	namespaceURI?: Signalish<string | undefined>;
	nonce?: Signalish<string>;
	noValidate?: Signalish<boolean>;
	open?: Signalish<boolean>;
	optimum?: Signalish<number>;
	pattern?: Signalish<string>;
	ping?: Signalish<string>;
	placeholder?: Signalish<string>;
	playsInline?: Signalish<boolean>;
	poster?: Signalish<string>;
	preload?: Signalish<string>;
	readOnly?: Signalish<boolean>;
	referrerPolicy?: Signalish<"no-referrer" | "no-referrer-when-downgrade" | "origin" | "origin-when-cross-origin" | "same-origin" | "strict-origin" | "strict-origin-when-cross-origin" | "unsafe-url">;
	rel?: Signalish<string>;
	required?: Signalish<boolean>;
	role?: Signalish<string>;
	rows?: Signalish<number>;
	rowSpan?: Signalish<number>;
	rowspan?: Signalish<number>;
	sandbox?: Signalish<string>;
	scope?: Signalish<string>;
	scrolling?: Signalish<string>;
	selected?: Signalish<boolean>;
	shape?: Signalish<string>;
	size?: Signalish<number>;
	sizes?: Signalish<string>;
	slot?: Signalish<string>;
	span?: Signalish<number>;
	spellcheck?: Signalish<boolean>;
	spellCheck?: Signalish<boolean>;
	src?: Signalish<string>;
	srcdoc?: Signalish<string>;
	srclang?: Signalish<string>;
	srcset?: Signalish<string>;
	start?: Signalish<number>;
	step?: Signalish<number | string>;
	style?: Signalish<string | StyleProperties>;
	summary?: Signalish<string>;
	tabIndex?: Signalish<number>;
	tabindex?: Signalish<number>;
	target?: Signalish<string>;
	textContent?: Signalish<string | undefined>;
	title?: Signalish<string>;
	type?: Signalish<string>;
	useMap?: Signalish<string>;
	value?: Signalish<string | string[] | number>;
	volume?: Signalish<string | number>;
	width?: Signalish<number | string>;
	wrap?: Signalish<string>;
}
export interface SVGAttributes<T> extends HTMLAttributes<T> {
	accentHeight?: number | string;
	accumulate?: "none" | "sum";
	additive?: "replace" | "sum";
	alignmentBaseline?: "auto" | "baseline" | "before-edge" | "text-before-edge" | "middle" | "central" | "after-edge" | "text-after-edge" | "ideographic" | "alphabetic" | "hanging" | "mathematical" | "inherit";
	allowReorder?: "no" | "yes";
	alphabetic?: number | string;
	amplitude?: number | string;
	arabicForm?: "initial" | "medial" | "terminal" | "isolated";
	ascent?: number | string;
	attributeName?: string;
	attributeType?: string;
	autoReverse?: boolean | "true" | "false";
	azimuth?: number | string;
	baseFrequency?: number | string;
	baselineShift?: number | string;
	baseProfile?: number | string;
	bbox?: number | string;
	begin?: number | string;
	bias?: number | string;
	by?: number | string;
	calcMode?: number | string;
	capHeight?: number | string;
	clip?: number | string;
	clipPath?: string;
	clipPathUnits?: number | string;
	clipRule?: number | string;
	colorInterpolation?: number | string;
	colorInterpolationFilters?: "auto" | "sRGB" | "linearRGB" | "inherit";
	colorProfile?: number | string;
	colorRendering?: number | string;
	contentScriptType?: number | string;
	contentStyleType?: number | string;
	cursor?: number | string;
	cx?: number | string;
	cy?: number | string;
	d?: string;
	decelerate?: number | string;
	descent?: number | string;
	diffuseConstant?: number | string;
	direction?: number | string;
	display?: number | string;
	divisor?: number | string;
	dominantBaseline?: number | string;
	dur?: number | string;
	dx?: number | string;
	dy?: number | string;
	edgeMode?: number | string;
	elevation?: number | string;
	enableBackground?: number | string;
	end?: number | string;
	exponent?: number | string;
	externalResourcesRequired?: boolean | "true" | "false";
	fill?: string;
	fillOpacity?: number | string;
	fillRule?: "nonzero" | "evenodd" | "inherit";
	filter?: string;
	filterRes?: number | string;
	filterUnits?: number | string;
	floodColor?: number | string;
	floodOpacity?: number | string;
	focusable?: boolean | "true" | "false" | "auto";
	fontFamily?: string;
	fontSize?: number | string;
	fontSizeAdjust?: number | string;
	fontStretch?: number | string;
	fontStyle?: number | string;
	fontVariant?: number | string;
	fontWeight?: number | string;
	format?: number | string;
	fr?: number | string;
	from?: number | string;
	fx?: number | string;
	fy?: number | string;
	g1?: number | string;
	g2?: number | string;
	glyphName?: number | string;
	glyphOrientationHorizontal?: number | string;
	glyphOrientationVertical?: number | string;
	glyphRef?: number | string;
	gradientTransform?: string;
	gradientUnits?: string;
	hanging?: number | string;
	horizAdvX?: number | string;
	horizOriginX?: number | string;
	href?: string;
	ideographic?: number | string;
	imageRendering?: number | string;
	in?: string;
	in2?: number | string;
	intercept?: number | string;
	k?: number | string;
	k1?: number | string;
	k2?: number | string;
	k3?: number | string;
	k4?: number | string;
	kernelMatrix?: number | string;
	kernelUnitLength?: number | string;
	kerning?: number | string;
	keyPoints?: number | string;
	keySplines?: number | string;
	keyTimes?: number | string;
	lengthAdjust?: number | string;
	letterSpacing?: number | string;
	lightingColor?: number | string;
	limitingConeAngle?: number | string;
	local?: number | string;
	markerEnd?: string;
	markerHeight?: number | string;
	markerMid?: string;
	markerStart?: string;
	markerUnits?: number | string;
	markerWidth?: number | string;
	mask?: string;
	maskContentUnits?: number | string;
	maskUnits?: number | string;
	mathematical?: number | string;
	mode?: number | string;
	numOctaves?: number | string;
	offset?: number | string;
	opacity?: number | string;
	operator?: number | string;
	order?: number | string;
	orient?: number | string;
	orientation?: number | string;
	origin?: number | string;
	overflow?: number | string;
	overlinePosition?: number | string;
	overlineThickness?: number | string;
	paintOrder?: number | string;
	panose1?: number | string;
	path?: string;
	pathLength?: number | string;
	patternContentUnits?: string;
	patternTransform?: number | string;
	patternUnits?: string;
	pointerEvents?: number | string;
	points?: string;
	pointsAtX?: number | string;
	pointsAtY?: number | string;
	pointsAtZ?: number | string;
	preserveAlpha?: boolean | "true" | "false";
	preserveAspectRatio?: string;
	primitiveUnits?: number | string;
	r?: number | string;
	radius?: number | string;
	refX?: number | string;
	refY?: number | string;
	renderingIntent?: number | string;
	repeatCount?: number | string;
	repeatDur?: number | string;
	requiredExtensions?: number | string;
	requiredFeatures?: number | string;
	restart?: number | string;
	result?: string;
	rotate?: number | string;
	rx?: number | string;
	ry?: number | string;
	scale?: number | string;
	seed?: number | string;
	shapeRendering?: number | string;
	slope?: number | string;
	spacing?: number | string;
	specularConstant?: number | string;
	specularExponent?: number | string;
	speed?: number | string;
	spreadMethod?: string;
	startOffset?: number | string;
	stdDeviation?: number | string;
	stemh?: number | string;
	stemv?: number | string;
	stitchTiles?: number | string;
	stopColor?: string;
	stopOpacity?: number | string;
	strikethroughPosition?: number | string;
	strikethroughThickness?: number | string;
	string?: number | string;
	stroke?: string;
	strokeDasharray?: string | number;
	strokeDashoffset?: string | number;
	strokeLinecap?: "butt" | "round" | "square" | "inherit";
	strokeLinejoin?: "miter" | "round" | "bevel" | "inherit";
	strokeMiterlimit?: number | string;
	strokeOpacity?: number | string;
	strokeWidth?: number | string;
	surfaceScale?: number | string;
	systemLanguage?: number | string;
	tableValues?: number | string;
	targetX?: number | string;
	targetY?: number | string;
	textAnchor?: string;
	textDecoration?: number | string;
	textLength?: number | string;
	textRendering?: number | string;
	to?: number | string;
	transform?: string;
	u1?: number | string;
	u2?: number | string;
	underlinePosition?: number | string;
	underlineThickness?: number | string;
	unicode?: number | string;
	unicodeBidi?: number | string;
	unicodeRange?: number | string;
	unitsPerEm?: number | string;
	vAlphabetic?: number | string;
	values?: string;
	vectorEffect?: number | string;
	version?: string;
	vertAdvY?: number | string;
	vertOriginX?: number | string;
	vertOriginY?: number | string;
	vHanging?: number | string;
	vIdeographic?: number | string;
	viewBox?: string;
	viewTarget?: number | string;
	visibility?: number | string;
	vMathematical?: number | string;
	widths?: number | string;
	wordSpacing?: number | string;
	writingMode?: number | string;
	x?: number | string;
	x1?: number | string;
	x2?: number | string;
	xChannelSelector?: string;
	xHeight?: number | string;
	xlinkActuate?: string;
	xlinkArcrole?: string;
	xlinkHref?: string;
	xlinkRole?: string;
	xlinkShow?: string;
	xlinkTitle?: string;
	xlinkType?: string;
	xmlBase?: string;
	xmlLang?: string;
	xmlns?: string;
	xmlnsXlink?: string;
	xmlSpace?: string;
	y?: number | string;
	y1?: number | string;
	y2?: number | string;
	yChannelSelector?: string;
	z?: number | string;
	zoomAndPan?: string;
}
export type SpecialKeys<T extends Element> = T extends HTMLLabelElement | HTMLOutputElement ? "for" | "class" | "is" : "class" | "is" | "spellCheck";
/** Figure out which of the attributes exist for a specific element */
export type ElementAttributes<TElement extends Element, TAttributes extends HTMLAttributes<TElement> | SVGAttributes<TElement>> = {
	[TKey in (keyof TElement & keyof TAttributes) | SpecialKeys<TElement>]?: TAttributes[TKey];
};
export type PropertiesOfFix<TFixes, TName> = TName extends keyof TFixes ? TFixes[TName] : unknown;
/**
 * Some tags properties can't be inferred correctly. To fix these properties, this manual override is defined.
 * Since it's an interface, users can even override them from outside.
 */
export interface HTMLTagFixes {
	meta: {
		charset?: string;
		property?: string;
	};
}
/** Figure out which of the HTML attributes exist for a specific element */
export type HTMLElementAttributes<TName extends keyof HTMLElementTagNameMap> = ElementAttributes<HTMLElementTagNameMap[TName], HTMLAttributes<HTMLElementTagNameMap[TName]>> & PropertiesOfFix<HTMLTagFixes, TName>;
/** Figure out which of the SVG attributes exist for a specific element */
export type SVGElementAttributes<TName extends keyof SVGElementTagNameMap> = ElementAttributes<SVGElementTagNameMap[TName], SVGAttributes<SVGElementTagNameMap[TName]>>;
export type SVGOnlyElementKeys = Exclude<keyof SVGElementTagNameMap, SVGAndHTMLElementKeys>;
export type SVGAndHTMLElementKeys = keyof SVGElementTagNameMap & keyof HTMLElementTagNameMap;
export type Simplify<T> = {
	[KeyType in keyof T]: T[KeyType];
} & {};
/** Generic event handler type with `currentTarget` and `this` correctly typed */
export type EventHandler<TTarget extends EventTarget, TEvent extends Event> = (this: TTarget, event: Omit<TEvent, "currentTarget"> & {
	readonly currentTarget: TTarget;
}) => void;
export type WithEventCapture<T extends Record<string, any>> = Simplify<T & {
	[TKey in Extract<keyof T, string> as `${TKey}Capture`]?: T[TKey];
}>;
export interface EventAttributesBase<T extends EventTarget> {
	onLoad?: EventHandler<T, Event>;
	onError?: EventHandler<T, ErrorEvent>;
	onCopy?: EventHandler<T, ClipboardEvent>;
	onCut?: EventHandler<T, ClipboardEvent>;
	onPaste?: EventHandler<T, ClipboardEvent>;
	onCompositionEnd?: EventHandler<T, CompositionEvent>;
	onCompositionStart?: EventHandler<T, CompositionEvent>;
	onCompositionUpdate?: EventHandler<T, CompositionEvent>;
	onToggle?: EventHandler<T, Event>;
	onFocus?: EventHandler<T, FocusEvent>;
	onFocusIn?: EventHandler<T, FocusEvent>;
	onFocusOut?: EventHandler<T, FocusEvent>;
	onBlur?: EventHandler<T, FocusEvent>;
	onChange?: EventHandler<T, Event>;
	onInput?: EventHandler<T, Event>;
	onBeforeInput?: EventHandler<T, InputEvent>;
	onSearch?: EventHandler<T, Event>;
	onSubmit?: EventHandler<T, SubmitEvent>;
	onInvalid?: EventHandler<T, Event>;
	onReset?: EventHandler<T, Event>;
	onFormData?: EventHandler<T, FormDataEvent>;
	onKeyDown?: EventHandler<T, KeyboardEvent>;
	onKeyPress?: EventHandler<T, KeyboardEvent>;
	onKeyUp?: EventHandler<T, KeyboardEvent>;
	onAbort?: EventHandler<T, UIEvent>;
	onCanPlay?: EventHandler<T, Event>;
	onCanPlayThrough?: EventHandler<T, Event>;
	onDurationChange?: EventHandler<T, Event>;
	onEmptied?: EventHandler<T, Event>;
	onEncrypted?: EventHandler<T, Event>;
	onEnded?: EventHandler<T, Event>;
	onLoadedData?: EventHandler<T, Event>;
	onLoadedMetadata?: EventHandler<T, Event>;
	onLoadStart?: EventHandler<T, Event>;
	onPause?: EventHandler<T, Event>;
	onPlay?: EventHandler<T, Event>;
	onPlaying?: EventHandler<T, Event>;
	onProgress?: EventHandler<T, ProgressEvent>;
	onRateChange?: EventHandler<T, Event>;
	onSeeked?: EventHandler<T, Event>;
	onSeeking?: EventHandler<T, Event>;
	onStalled?: EventHandler<T, Event>;
	onSuspend?: EventHandler<T, Event>;
	onTimeUpdate?: EventHandler<T, Event>;
	onVolumeChange?: EventHandler<T, Event>;
	onWaiting?: EventHandler<T, Event>;
	onClick?: EventHandler<T, MouseEvent>;
	onContextMenu?: EventHandler<T, MouseEvent>;
	onDblClick?: EventHandler<T, MouseEvent>;
	onDoubleClick?: EventHandler<T, MouseEvent>;
	onAuxClick?: EventHandler<T, MouseEvent>;
	onDrag?: EventHandler<T, DragEvent>;
	onDragEnd?: EventHandler<T, DragEvent>;
	onDragEnter?: EventHandler<T, DragEvent>;
	onDragLeave?: EventHandler<T, DragEvent>;
	onDragOver?: EventHandler<T, DragEvent>;
	onDragStart?: EventHandler<T, DragEvent>;
	onDrop?: EventHandler<T, DragEvent>;
	onMouseDown?: EventHandler<T, MouseEvent>;
	onMouseEnter?: EventHandler<T, MouseEvent>;
	onMouseLeave?: EventHandler<T, MouseEvent>;
	onMouseMove?: EventHandler<T, MouseEvent>;
	onMouseOut?: EventHandler<T, MouseEvent>;
	onMouseOver?: EventHandler<T, MouseEvent>;
	onMouseUp?: EventHandler<T, MouseEvent>;
	onSelect?: EventHandler<T, Event>;
	onSelectionChange?: EventHandler<T, Event>;
	onSelectStart?: EventHandler<T, Event>;
	onBeforeToggle?: EventHandler<T, Event>;
	onDeviceMotion?: EventHandler<T, DeviceMotionEvent>;
	onDeviceOrientation?: EventHandler<T, DeviceOrientationEvent>;
	onGamepadConnected?: EventHandler<T, GamepadEvent>;
	onGamepadDisconnected?: EventHandler<T, GamepadEvent>;
	onTouchCancel?: EventHandler<T, TouchEvent>;
	onTouchEnd?: EventHandler<T, TouchEvent>;
	onTouchMove?: EventHandler<T, TouchEvent>;
	onTouchStart?: EventHandler<T, TouchEvent>;
	onPointerOver?: EventHandler<T, PointerEvent>;
	onPointerEnter?: EventHandler<T, PointerEvent>;
	onPointerDown?: EventHandler<T, PointerEvent>;
	onPointerMove?: EventHandler<T, PointerEvent>;
	onPointerUp?: EventHandler<T, PointerEvent>;
	onPointerCancel?: EventHandler<T, PointerEvent>;
	onPointerOut?: EventHandler<T, PointerEvent>;
	onPointerLeave?: EventHandler<T, PointerEvent>;
	onGotPointerCapture?: EventHandler<T, PointerEvent>;
	onLostPointerCapture?: EventHandler<T, PointerEvent>;
	onPointerLockChange?: EventHandler<T, Event>;
	onPointerLockError?: EventHandler<T, Event>;
	onScroll?: EventHandler<T, Event>;
	onScrollEnd?: EventHandler<T, Event>;
	onResize?: EventHandler<T, UIEvent>;
	onOrientationChange?: EventHandler<T, Event>;
	onFullscreenChange?: EventHandler<T, Event>;
	onFullscreenError?: EventHandler<T, Event>;
	onVisibilityChange?: EventHandler<T, Event>;
	onCueChange?: EventHandler<T, Event>;
	onWheel?: EventHandler<T, WheelEvent>;
	onAnimationStart?: EventHandler<T, AnimationEvent>;
	onAnimationEnd?: EventHandler<T, AnimationEvent>;
	onAnimationIteration?: EventHandler<T, AnimationEvent>;
	onAnimationCancel?: EventHandler<T, AnimationEvent>;
	onTransitionCancel?: EventHandler<T, TransitionEvent>;
	onTransitionEnd?: EventHandler<T, TransitionEvent>;
	onTransitionRun?: EventHandler<T, TransitionEvent>;
	onTransitionStart?: EventHandler<T, TransitionEvent>;
	onAfterPrint?: EventHandler<T, Event>;
	onBeforePrint?: EventHandler<T, Event>;
	onHashChange?: EventHandler<T, HashChangeEvent>;
	onPopState?: EventHandler<T, PopStateEvent>;
	onPageHide?: EventHandler<T, PageTransitionEvent>;
	onPageShow?: EventHandler<T, PageTransitionEvent>;
	onReadyStateChange?: EventHandler<T, Event>;
	onUnload?: EventHandler<T, Event>;
	onBeforeUnload?: EventHandler<T, BeforeUnloadEvent>;
	onCancel?: EventHandler<T, Event>;
	onClose?: EventHandler<T, Event>;
	onSlotChange?: EventHandler<T, Event>;
	onLanguageChange?: EventHandler<T, Event>;
	onMessage?: EventHandler<T, MessageEvent>;
	onMessageError?: EventHandler<T, MessageEvent>;
	onOffline?: EventHandler<T, Event>;
	onOnline?: EventHandler<T, Event>;
	onRejectionHandled?: EventHandler<T, PromiseRejectionEvent>;
	onUnhandledRejection?: EventHandler<T, PromiseRejectionEvent>;
	onSecurityPolicyViolation?: EventHandler<T, SecurityPolicyViolationEvent>;
	onStorage?: EventHandler<T, StorageEvent>;
}
export type EventAttributes<T extends EventTarget> = WithEventCapture<EventAttributesBase<T>>;
export interface CustomElementsHTML {
}
export interface HTMLComponentProps<T extends Element> extends BaseProps {
	dangerouslySetInnerHTML?: {
		__html: string;
	};
	/** @deprecated This is simply ignored as it only applies to v-dom  */
	key?: string | number;
	on?: Record<string, Function>;
	onCapture?: Record<string, Function>;
	/**
	 * This is essentially a reverse "is" attribute.
	 * If you specify it, the generated tag will be tsxTag and it will receive an "is" attribute with the tag you specified in your JSX.
	 * This is needed because we can't make the is-property associate with the correct component props.
	 */
	tsxTag?: keyof HTMLElementTagNameMap | keyof SVGElementTagNameMap;
	ref?: Ref<T>;
}
export type DOMAttributes<T extends Element> = EventAttributes<T> & HTMLComponentProps<T>;
export type IntrinsicElementsHTML = {
	[TKey in keyof HTMLElementTagNameMap]?: HTMLElementAttributes<TKey> & HTMLComponentProps<HTMLElementTagNameMap[TKey]> & EventAttributes<HTMLElementTagNameMap[TKey]>;
};
export type IntrinsicElementsSVG = {
	[TKey in SVGOnlyElementKeys]?: SVGElementAttributes<TKey> & HTMLComponentProps<SVGElementTagNameMap[TKey]> & EventAttributes<SVGElementTagNameMap[TKey]>;
};
export type IntrinsicElementsCombined = IntrinsicElementsHTML & (ConfigureJSXElement["svg"] extends false ? void : IntrinsicElementsSVG);
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
} & DOMAttributes<T> | null, key?: string): T;
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
