/** A helper that comes from the internal module. */
export interface Helper {
    /** The identifier. */
    id: number;
}

/** A type that contains a {@link Helper}. */
export interface TypeB {
    helper: Helper;
}

/** Referenced only through an `import("./internal").X` type in another module. */
export interface Helper2Probe {
    probe: true;
}
