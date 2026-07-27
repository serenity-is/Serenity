/**
 * ## DomWise (@serenity-is/domwise)
 * 
 * > **Use JSX syntax to create and manage DOM elements** — with full TypeScript support, reactive signal bindings, and seamless integration with the Serenity widget system.
 *
 * DomWise is a lightweight, high-performance JSX library that compiles your JSX templates directly into real DOM nodes — no virtual DOM, no diffing, no reconciliation overhead. Designed from the ground up for the [Serenity](https://serenity.is) application framework, it embraces direct DOM manipulation and works harmoniously with widget lifecycles, avoiding the conflicts that plague VDOM libraries when mixed with imperative DOM updates.
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

