type Simplify<T> = { [KeyType in keyof T]: T[KeyType] } & {};

/** Generic event handler type with `currentTarget` and `this` correctly typed */
export type EventHandler<TTarget extends EventTarget, TEvent extends Event> =
    (this: TTarget, event: Omit<TEvent, "currentTarget"> & { readonly currentTarget: TTarget }) => void;

export type WithEventOptions<T extends Record<string, any>> = Simplify<{
    [TKey in Extract<keyof T, string> as `${TKey}Capture`]?: T[TKey] | { listener: T[TKey]; options: AddEventListenerOptions };
}>;

export interface EventAttributesBase<T extends EventTarget> {
    // Image Events
    onLoad?: EventHandler<T, Event>;
    onError?: EventHandler<T, ErrorEvent>;

    // Clipboard Events
    onCopy?: EventHandler<T, ClipboardEvent>;
    onCut?: EventHandler<T, ClipboardEvent>;
    onPaste?: EventHandler<T, ClipboardEvent>;

    // Composition Events
    onCompositionEnd?: EventHandler<T, CompositionEvent>;
    onCompositionStart?: EventHandler<T, CompositionEvent>;
    onCompositionUpdate?: EventHandler<T, CompositionEvent>;

    // Details Events
    onToggle?: EventHandler<T, Event>;

    // Focus Events
    onFocus?: EventHandler<T, FocusEvent>;
    onFocusIn?: EventHandler<T, FocusEvent>;
    onFocusOut?: EventHandler<T, FocusEvent>;
    onBlur?: EventHandler<T, FocusEvent>;

    // Form Events
    onChange?: EventHandler<T, Event>;
    onInput?: EventHandler<T, Event>;
    onBeforeInput?: EventHandler<T, InputEvent>;
    onSearch?: EventHandler<T, Event>;
    onSubmit?: EventHandler<T, SubmitEvent>;
    onInvalid?: EventHandler<T, Event>;
    onReset?: EventHandler<T, Event>;
    onFormData?: EventHandler<T, FormDataEvent>;

    // Keyboard Events
    onKeyDown?: EventHandler<T, KeyboardEvent>;
    onKeyPress?: EventHandler<T, KeyboardEvent>;
    onKeyUp?: EventHandler<T, KeyboardEvent>;

    // Media Events
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

    // Mouse Events
    onClick?: EventHandler<T, MouseEvent>;
    onContextMenu?: EventHandler<T, MouseEvent>;
    onDblClick?: EventHandler<T, MouseEvent>;
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

    // Selection Events
    onSelect?: EventHandler<T, Event>;
    onSelectionChange?: EventHandler<T, Event>;
    onSelectStart?: EventHandler<T, Event>;
    onBeforeToggle?: EventHandler<T, Event>;

    // Sensor Events
    onDeviceMotion?: EventHandler<T, DeviceMotionEvent>;
    onDeviceOrientation?: EventHandler<T, DeviceOrientationEvent>;

    // Gamepad Events
    onGamepadConnected?: EventHandler<T, GamepadEvent>;
    onGamepadDisconnected?: EventHandler<T, GamepadEvent>;

    // Touch Events
    onTouchCancel?: EventHandler<T, TouchEvent>;
    onTouchEnd?: EventHandler<T, TouchEvent>;
    onTouchMove?: EventHandler<T, TouchEvent>;
    onTouchStart?: EventHandler<T, TouchEvent>;

    // Pointer Events
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

    // UI Events
    onScroll?: EventHandler<T, Event>;
    onScrollEnd?: EventHandler<T, Event>;
    onResize?: EventHandler<T, UIEvent>;
    onOrientationChange?: EventHandler<T, Event>;
    onFullscreenChange?: EventHandler<T, Event>;
    onFullscreenError?: EventHandler<T, Event>;
    onVisibilityChange?: EventHandler<T, Event>;
    onCueChange?: EventHandler<T, Event>;

    // Wheel Events
    onWheel?: EventHandler<T, WheelEvent>;

    // Animation Events
    onAnimationStart?: EventHandler<T, AnimationEvent>;
    onAnimationEnd?: EventHandler<T, AnimationEvent>;
    onAnimationIteration?: EventHandler<T, AnimationEvent>;
    onAnimationCancel?: EventHandler<T, AnimationEvent>;

    // Transition Events
    onTransitionCancel?: EventHandler<T, TransitionEvent>;
    onTransitionEnd?: EventHandler<T, TransitionEvent>;
    onTransitionRun?: EventHandler<T, TransitionEvent>;
    onTransitionStart?: EventHandler<T, TransitionEvent>;

    // Print Events
    onAfterPrint?: EventHandler<T, Event>;
    onBeforePrint?: EventHandler<T, Event>;

    // History Events
    onHashChange?: EventHandler<T, HashChangeEvent>;
    onPopState?: EventHandler<T, PopStateEvent>;
    onPageHide?: EventHandler<T, PageTransitionEvent>;
    onPageShow?: EventHandler<T, PageTransitionEvent>;

    // State Events
    onReadyStateChange?: EventHandler<T, Event>;
    onUnload?: EventHandler<T, Event>;
    onBeforeUnload?: EventHandler<T, BeforeUnloadEvent>;

    // Dialog Events
    onCancel?: EventHandler<T, Event>;
    onClose?: EventHandler<T, Event>;

    // Slot Events
    onSlotChange?: EventHandler<T, Event>;

    // Language Events
    onLanguageChange?: EventHandler<T, Event>;

    // Message Events
    onMessage?: EventHandler<T, MessageEvent>;
    onMessageError?: EventHandler<T, MessageEvent>;

    // Network Access Events
    onOffline?: EventHandler<T, Event>;
    onOnline?: EventHandler<T, Event>;

    // Error Events
    onRejectionHandled?: EventHandler<T, PromiseRejectionEvent>;
    onUnhandledRejection?: EventHandler<T, PromiseRejectionEvent>;
    onSecurityPolicyViolation?: EventHandler<T, SecurityPolicyViolationEvent>;

    // Storage Events
    onStorage?: EventHandler<T, StorageEvent>;
}

export type EventAttributes<T extends EventTarget> = WithEventOptions<EventAttributesBase<T>>;
