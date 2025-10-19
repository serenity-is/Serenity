import { ComponentChildren, JSXElement, SignalLike, SignalOrValue } from '@serenity-is/sleekdom';

export declare function IfElse<TWhen>(props: {
	when: SignalOrValue<TWhen | undefined | null>;
	else?: ComponentChildren;
	children: ComponentChildren | ((when: SignalOrValue<TWhen | undefined | null>) => ComponentChildren);
}): JSXElement;
export interface Signal<T> extends SignalLike<T> {
}
export interface SignalOptions<T> {
	watched?: (this: Signal<T>) => void;
	unwatched?: (this: Signal<T>) => void;
	name?: string;
}
export interface EffectOptions {
	name?: string;
}
export type ReadonlySignal<T> = Readonly<Signal<T>>;
export type EffectFn = ((this: {
	dispose: () => void;
}) => void | (() => void)) | (() => void | (() => void));
export declare const signal: {
	<T>(value: T, options?: SignalOptions<T>): Signal<T>;
	<T = undefined>(): Signal<T | undefined>;
};
export declare const computed: (<T>(fn: () => T, options?: SignalOptions<T>) => ReadonlySignal<T>);
export declare const effect: ((fn: EffectFn, options?: EffectOptions) => () => void);
export declare const batch: (<T>(fn: () => T) => T);
export declare const untracked: (<T>(fn: () => T) => T);

export {};
