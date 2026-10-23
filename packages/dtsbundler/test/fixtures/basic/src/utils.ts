import { External } from 'ext-lib';

/**
 * A helper declared in the utils module. It collides with
 * {@link Helper} from the internal module.
 */
export interface Helper {
    /** Extra info. */
    extra: string;
}

/**
 * The main type of this fixture.
 */
export interface TypeA {
    helper: Helper;
    symbol?: typeof helperSymbol;
    ns?: typeof InternalNamespace;
    base?: InternalBase;
    ext?: External;
    generated?: GeneratedLike;
    name: string;
}

/** Never included in the bundle (not reachable from the entry). */
export function makeA(): TypeA {
    throw new Error('not implemented');
}

/** Not exported by the entry, but referenced by {@link TypeA}. */
export const helperSymbol: unique symbol = Symbol('helper');

/** A namespace that is not exported by the entry. */
export namespace InternalNamespace {
    /** A member of the namespace. */
    export const value: number = 1;
}

export abstract class InternalBase {
    abstract readonly id: string;
}

/**
 * Generated code often spells external references as `import("pkg").Type`, which
 * would duplicate the import statement the bundle already emits.
 */
export interface GeneratedLike {
    detail: import('ext-lib').ExternalDetail;
    nested: Array<import('ext-lib').ExternalDetail>;
    inlined: import('./internal').Helper2Probe;
}
