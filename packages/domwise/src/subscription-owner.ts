import { addDisposingListener, invokeDisposingListeners } from "./disposing-listener";

const scopedOwners = new WeakMap<EventTarget, Map<string, EventTarget>>();

/**
 * Returns a stable `EventTarget` used to scope the signal subscriptions of a
 * single property on a node.
 *
 * Subscriptions created with this owner as their `lifecycleNode` can be torn
 * down together via {@link disposeScopedSubscriptions}, and are also disposed
 * automatically when the owning node is disposed. This lets props like `style`
 * and `class` drop their per-key signal bindings when the whole prop value is
 * replaced, without affecting the node's other subscriptions.
 *
 * @param node - The element that owns the property.
 * @param prop - Property name (e.g. `"style"` or `"class"`).
 * @returns The scoped owner for that property.
 */
export function getScopedOwner(node: EventTarget, prop: string): EventTarget {
    let owners = scopedOwners.get(node);
    if (!owners) {
        owners = new Map();
        scopedOwners.set(node, owners);
    }
    let owner = owners.get(prop);
    if (!owner) {
        const created = new EventTarget();
        owners.set(prop, created);
        owner = created;
        addDisposingListener(node, () => invokeDisposingListeners(created), "domwise:owner:" + prop);
    }
    return owner;
}

/**
 * Disposes all signal subscriptions currently scoped to `prop` on `node`,
 * leaving the owner and subscriptions of other properties untouched.
 * @param node - The element that owns the property.
 * @param prop - Property name (e.g. `"style"` or `"class"`).
 */
export function disposeScopedSubscriptions(node: EventTarget, prop: string): void {
    const owner = scopedOwners.get(node)?.get(prop);
    if (owner)
        invokeDisposingListeners(owner);
}
