import './internal';

/**
 * Augmenting a sibling module: after bundling the "./internal" specifier does not
 * exist anymore, so this declaration has to be promoted to top-level - and it must
 * keep its `export` because the entry exports the augmented type.
 */
declare module './internal' {
    interface TypeB {
        /** Added by the augmentation. */
        augmented?: boolean;
    }
}

/** Augmenting an external package. */
declare module 'ext-lib' {
    interface External {
        /** Added to the external module. */
        augmentation?: boolean;
    }
}

declare global {
    /** A global declared by an inlined file. */
    var augmentedGlobal: number;
}
