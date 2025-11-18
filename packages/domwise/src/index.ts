/**
 * ## DomWise (@serenity-is/domwise)
 * 
 * TypeScript library for creating and managing DOM elements using JSX syntax with strong typing and IDE support.
 * 
 * - This is primarily based on  [alex-kinokon/jsx-dom](https://github.com/alex-kinokon/jsx-dom) with integrated signal support via [@preact/signals](https://github.com/preactjs/signals) and some extra types / ideas from [ryansolid/dom-expressions](https://github.com/ryansolid/dom-expressions) and [lusito/tsx-dom](https://github.com/Lusito/tsx-dom).
 * - Enables creating and manipulating DOM elements using JSX syntax with strong typing and IDE support.
 * @packageDocumentation
 */
export type * from "../types";
export { bindThis } from "./bind-this";
export { className } from "./class-name";
export { createElement, createElement as h, useImperativeHandle } from "./compat-api";
export { Component } from "./component";
export { addDisposingListener, currentLifecycleRoot, dispatchDisposingEvent, invokeDisposingListeners, removeDisposingListener } from "./disposing-listener";
export { Fragment } from "./fragment";
export * from "./hooks";
export * from "./in-namespace-uri";
export { jsx, jsx as jsxs } from "./jsx-factory";
export { MathMLNamespace } from "./mathml-consts";
export * from "./prop-hook";
export { createRef, setRef } from "./ref";
export { ShadowRootNode } from "./shadow";
export { derivedSignal, isSignalLike, observeSignal } from "./signal-util";
export * from "./signals";
export { SVGNamespace } from "./svg-consts";

