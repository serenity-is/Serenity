import { ComponentChildren, JSXElement, SignalLike, SignalOrValue } from '@serenity-is/sleekdom';

export type IfCondition<TWhen> = SignalOrValue<TWhen | undefined | null>;
export declare function Show<TWhen>(props: {
	when: IfCondition<TWhen>;
	fallback?: ComponentChildren;
	children: ComponentChildren | ((when: IfCondition<TWhen>) => ComponentChildren);
}): JSXElement;
export interface SignalOptions<T> {
	watched?: (this: SignalLike<T>) => void;
	unwatched?: (this: SignalLike<T>) => void;
	name?: string;
}
export interface EffectOptions {
	name?: string;
}
export type ReadonlySignalLike<T> = Readonly<SignalLike<T>>;
export type EffectFn = ((this: {
	dispose: () => void;
}) => void | (() => void)) | (() => void | (() => void));
export declare const signal: {
	<T>(value: T, options?: SignalOptions<T>): SignalLike<T>;
	<T = undefined>(): SignalLike<T | undefined>;
};
export declare const computed: (<T>(fn: () => T, options?: SignalOptions<T>) => ReadonlySignalLike<T>);
export declare const effect: ((fn: EffectFn, options?: EffectOptions) => () => void);
export declare const batch: (<T>(fn: () => T) => T);
export declare const untracked: (<T>(fn: () => T) => T);

export {};
