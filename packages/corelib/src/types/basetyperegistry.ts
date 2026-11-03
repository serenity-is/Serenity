import { Config, getGlobalTypeRegistry, getType, isPromiseLike } from "../base";

/**
 * Base class for type registries that manage registration and lookup of user-defined types.
 * Provides common functionality for finding types by key, lazy loading, and root namespace searching.
 *
 * Subclasses should override {@link isMatchingType} to define which types they handle,
 * and {@link loadError} to provide appropriate error messages.
 */
export abstract class BaseTypeRegistry<TType> {
    /** The kind of loading this registry performs for lazy loading */
    declare protected loadKind: string;
    /** Default suffix to strip from type names (e.g., "Editor", "Dialog") */
    declare protected defaultSuffix: string;
    /** Cache of registered types indexed by their keys */
    declare protected registeredTypes: { [key: string]: TType };

    /**
     * Creates a new type registry instance.
     * @param options Configuration options for the registry
     */
    constructor(options: {
        /** The kind of loading for lazy type loading */
        loadKind?: string,
        /** Default suffix to strip from type names */
        defaultSuffix?: string
    }) {
        this.loadKind = options.loadKind;
        this.defaultSuffix = options.defaultSuffix;
    }

    /**
     * Gets a secondary type key for the given type.
     * Only enums override this to provide legacy enum key support.
     * @param type The type to get a secondary key for
     * @returns The secondary key, or undefined if none
     */
    protected getSecondaryTypeKey(type: any): string {
        return void 0;
    }

    /**
     * Determines if the given type matches the criteria for this registry.
     * Subclasses should override this to define which types they handle.
     * @param type The type to check
     * @returns True if the type matches this registry's criteria
     */
    protected isMatchingType(type: any): boolean {
        return true;
    }

    /**
     * Called when a type cannot be found or loaded.
     * Subclasses must implement this to provide appropriate error messages.
     * @param key The key that could not be found
     */
    protected abstract loadError(key: string): void;

    /**
     * Searches for a type in the global type registry and root namespaces.
     * @param key The key to search for
     * @returns The found type, or null if not found
     */
    protected searchSystemTypes(key: string): TType {
        let type = getType(key) as TType;
        if (type != null && this.isMatchingType(type))
            return type;

        for (const ns of Config.rootNamespaces) {
            const k = ns + "." + key;
            type = this.registeredTypes[k];
            if (type)
                return type;

            type = getType(ns + '.' + key) as TType;
            if (type != null && this.isMatchingType(type))
                return type;
        }
    }

    /**
     * Initializes the registry by scanning the global type registry
     * and building the local cache of matching types.
     */
    protected init() {
        this.registeredTypes = {};
        for (const [fullName, type] of Object.entries(getGlobalTypeRegistry())) {
            if (!this.isMatchingType(type))
                continue;

            this.registeredTypes[fullName] = type;

            const akey = this.getSecondaryTypeKey?.(type);
            if (akey && akey !== fullName)
                this.registeredTypes[akey] = type;
        }

        if (this.defaultSuffix) {
            for (const key of Object.keys(this.registeredTypes)) {
                if (key.endsWith(this.defaultSuffix)) {
                    const p = key.substring(0, key.length - this.defaultSuffix.length);
                    if (p && !this.registeredTypes[p])
                        this.registeredTypes[p] = this.registeredTypes[key];
                }
            }
        }
    }

    /**
     * Gets a type by key, throwing an error if not found.
     * @param key The key to look up
     * @returns The found type
     * @throws When the type is not found
     */
    get(key: string): TType {
        const type = this.tryGet(key);
        if (type)
            return type;

        this.loadError(key);
    }

    /**
     * Gets a type by key, attempting lazy loading if not found.
     * @param key The key to look up
     * @returns The found type or a promise that resolves to it
     * @throws When the type cannot be found or loaded
     */
    getOrLoad(key: string): TType | PromiseLike<TType> {
        const type = this.tryGetOrLoad(key);
        if (type) {
            if (isPromiseLike(type)) {
                return type.then(t => {
                    if (!t)
                        this.loadError(key);
                    return t;
                });
            }

            return type;
        }

        this.loadError(key);
    }

    /**
     * Clears the registry cache, forcing re-initialization on next access.
     */
    reset() {
        this.registeredTypes = null;
    }

    /**
     * Attempts to get a type by key without throwing errors.
     * @param key The key to look up
     * @returns The found type, or null if not found
     */
    tryGet(key: string): TType {
        if (!key)
            return null;

        if (this.registeredTypes == null)
            this.init();

        let type = this.registeredTypes[key];
        if (type)
            return type;

        type = this.searchSystemTypes(key);

        if (type == null && this.defaultSuffix && !key.endsWith(this.defaultSuffix))
            type = this.registeredTypes[key + this.defaultSuffix] ?? this.searchSystemTypes(key + this.defaultSuffix);

        if (type) {
            this.registeredTypes[key] = type;

            const akey = this.getSecondaryTypeKey?.(type);
            if (akey && key != akey)
                this.registeredTypes[akey] = type;

            if (this.defaultSuffix && key.endsWith(this.defaultSuffix)) {
                const p = key.substring(0, key.length - this.defaultSuffix.length);
                if (p && !this.registeredTypes[p])
                    this.registeredTypes[p] = this.registeredTypes[key];
            }

            return type;
        }

        return type;
    }

    /**
     * Gets the forms of a key a lazy type can be registered with, in the order they are tried.
     * @param key The key to look up
     */
    private getLazyKeys(key: string): string[] {
        const keys = this.defaultSuffix ? [key, key + this.defaultSuffix] : [key];
        for (const ns of Config.rootNamespaces) {
            keys.push(ns + "." + key);
            if (this.defaultSuffix)
                keys.push(ns + "." + key + this.defaultSuffix);
        }
        return keys;
    }

    /**
     * Checks whether a type seems to be available, either as it is already registered or as `Config.lazyTypeLoader`
     * says it can load it, without loading it. A lazy type may still turn out to be of another kind once it is loaded.
     * @param key The key to look up
     * @returns True if the type seems to be available
     */
    isAvailable(key: string): boolean {
        if (this.tryGet(key))
            return true;

        return !!key && !!Config.lazyTypeLoader &&
            this.getLazyKeys(key).some(lazyKey => !!Config.lazyTypeLoader(lazyKey, this.loadKind as any, true));
    }

    /**
     * Attempts to get a type by key, with lazy loading support.
     * @param key The key to look up
     * @returns The found type, a promise that resolves to it, or null if not found
     */
    tryGetOrLoad(key: string): TType | PromiseLike<TType> {
        let type = this.tryGet(key);
        if (type)
            return type;

        if (key && Config.lazyTypeLoader) {
            let promise: any;
            for (const lazyKey of this.getLazyKeys(key)) {
                if (promise = Config.lazyTypeLoader(lazyKey, this.loadKind as any))
                    break;
            }

            if (isPromiseLike(promise)) {
                return promise.then(t => {
                    if (t && this.isMatchingType(t)) {
                        this.registeredTypes[key] = t;
                        return t;
                    }
                    return null;
                });
            }

            if (promise && this.isMatchingType(promise)) {
                this.registeredTypes[key] = promise;
                return promise;
            }

            return null;
        }

        return type;
    }
}