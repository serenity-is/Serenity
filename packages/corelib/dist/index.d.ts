import { CellMouseEvent, Column, EventEmitter, FormatterContext, FormatterResult, GridOptions, GridSortEvent, Group, GroupItemMetadataProvider, GroupTotals, IDataView, IGroupTotals, ISleekGrid, ItemMetadata } from '@serenity-is/sleekgrid';

/**
 * Describes the currently authenticated user as resolved on the client.
 * Typically populated from the server's user definition script.
 */
export interface UserDefinition {
	/** Username / login name of the current user. */
	Username?: string;
	/** Human-readable display name of the current user. */
	DisplayName?: string;
	/**
	 * Whether the user is a super-admin with implicit access to all permissions.
	 * This is distinct from membership in an Administrators role, which may not
	 * grant every permission individually.
	 */
	IsAdmin?: boolean;
	/**
	 * Map of permission keys granted to the user (explicitly or via roles).
	 * Client-side checks should only drive UI enable/disable; always re-validate
	 * permissions on the server.
	 */
	Permissions?: {
		[key: string]: boolean;
	};
}
/**
 * Provides permission checks and user-state accessors for the current session.
 *
 * Aggregates synchronous and asynchronous helpers that are intended for UI gating only;
 * server-side authorization must still be enforced. Permission expressions may combine
 * keys with `&` (AND) and `|` (OR), e.g. `"Admin&Sales|Manager"`.
 *
 * @remarks
 * Defined as a namespace (rather than plain functions) for backward compatibility and
 * to allow consumers to override/monkey-patch members in ES-module environments.
 * `*` always grants access; `""` and `"?"` only check that a user is logged in.
 * Users with `IsAdmin` are granted every permission.
 * @example
 * if (Authorization.hasPermission("Administration:General")) {
 *     // show admin UI
 * }
 * @example
 * if (await Authorization.hasPermissionAsync("Orders:View&Orders:Approve")) {
 *     // show approve button
 * }
 */
export declare namespace Authorization {
	/**
	 * Synchronously checks whether the current user has the specified permission.
	 *
	 * @remarks
	 * Prefer {@link Authorization.hasPermissionAsync} in new code — this synchronous
	 * variant may block the UI thread if the `UserData` script has not been loaded yet
	 * (it falls back to {@link getRemoteData} which can issue a synchronous request).
	 * Use only for UI gating; always enforce permissions server-side as well.
	 * @param permission - Permission key or expression. May contain `&` (AND) and `|` (OR)
	 * operators, e.g. `"A&B|C"`. `null`/`undefined` returns `false`; `"*"` returns `true`;
	 * `""` or `"?"` returns whether the user is logged in.
	 * @returns `true` if the user has the permission (or is an admin), otherwise `false`.
	 * @example
	 * Authorization.hasPermission("Administration:General"); // true if admin or granted
	 * @example
	 * Authorization.hasPermission("A&B|C"); // true if (A and B) or C
	 */
	function hasPermission(permission: string): boolean;
	/**
	 * Asynchronously checks whether the current user has the specified permission.
	 *
	 * @remarks
	 * Preferred over {@link Authorization.hasPermission} because it awaits
	 * `UserData` via {@link getRemoteDataAsync} instead of potentially blocking
	 * the UI thread. Still UI-only — enforce permissions server-side as well.
	 * @param permission - Permission key or expression with `&`/`|` operators,
	 * e.g. `"A&B|C"`. `null`/`undefined` returns `false`; `"*"` returns `true`;
	 * `""` or `"?"` returns whether the user is logged in.
	 * @returns Promise that resolves to `true` if the user has the permission
	 * (or is an admin), otherwise `false`.
	 * @example
	 * if (await Authorization.hasPermissionAsync("Administration:General")) { ... }
	 * @example
	 * await Authorization.hasPermissionAsync("A&B|C"); // true if (A and B) or C
	 */
	function hasPermissionAsync(permission: string): Promise<boolean>;
	/**
	 * Tests whether a permission hash-set contains the specified permission or expression.
	 *
	 * @remarks
	 * Handles logical operators: `"A&B"` requires all listed permissions, `"A|B"`
	 * requires at least one. `&` binds tighter than `|`, so `"A&B|C"` is
	 * `(A AND B) OR C`. An exact key match is checked first before parsing operators.
	 * @param permissionSet - Dictionary of granted permissions (key → `true`). `null`/`undefined` yields `false`.
	 * @param permission - Permission key or expression with `&`/`|` operators. `null` returns `false`.
	 * @returns `true` if the permission is implied by the set, otherwise `false`.
	 * @example
	 * Authorization.isPermissionInSet({ "A": true, "B": true }, "A&B"); // true
	 * @example
	 * Authorization.isPermissionInSet({ "A": true }, "A|C"); // true
	 * @example
	 * Authorization.isPermissionInSet({ "A": true }, "A&B"); // false
	 */
	function isPermissionInSet(permissionSet: {
		[key: string]: boolean;
	}, permission: string): boolean;
	/**
	 * Synchronously validates that the current user has the specified permission.
	 *
	 * @remarks
	 * Shows a localized "Access Denied" notification and throws if the check fails.
	 * Prefer {@link Authorization.validatePermissionAsync} to avoid potentially
	 * blocking on `UserData` loading. Use only for UI gating.
	 * @param permission - Permission key or expression with `&`/`|` operators.
	 * @throws Error with localized "Authorization.AccessDenied" message if the user lacks the permission.
	 * @example
	 * Authorization.validatePermission("Administration:General");
	 */
	function validatePermission(permission: string): void;
	/**
	 * Asynchronously validates that the current user has the specified permission.
	 *
	 * @remarks
	 * Awaits {@link Authorization.hasPermissionAsync} and, on failure, shows a
	 * localized "Access Denied" notification before throwing.
	 * @param permission - Permission key or expression with `&`/`|` operators.
	 * @returns Promise that resolves if authorized, or rejects/throws if not.
	 * @throws Error with localized "Authorization.AccessDenied" message if the user lacks the permission.
	 * @example
	 * await Authorization.validatePermissionAsync("A&B|C");
	 */
	function validatePermissionAsync(permission: string): Promise<void>;
}
export declare namespace Authorization {
	/**
	 * Whether the current user is logged in (synchronous).
	 *
	 * @remarks
	 * Implemented as a getter over {@link Authorization.userDefinition}. Prefer
	 * {@link Authorization.isLoggedInAsync} to avoid blocking on `UserData` load.
	 * Returns `true` when `UserDefinition.Username` is truthy.
	 * @example
	 * if (Authorization.isLoggedIn) {
	 *     // user is authenticated
	 * }
	 */
	let isLoggedIn: boolean;
	/**
	 * Whether the current user is logged in (asynchronous).
	 *
	 * @remarks
	 * Awaits {@link Authorization.userDefinitionAsync} so it never blocks the UI thread.
	 * @example
	 * if (await Authorization.isLoggedInAsync) {
	 *     // user is authenticated
	 * }
	 */
	let isLoggedInAsync: Promise<boolean>;
	/**
	 * Username of the currently logged-in user (synchronous).
	 *
	 * @remarks
	 * Getter over {@link Authorization.userDefinition}`.Username`. Prefer
	 * {@link Authorization.usernameAsync} if `UserData` may not be loaded yet.
	 * Returns `undefined` when no user is logged in.
	 * @example
	 * const name = Authorization.username;
	 */
	let username: string;
	/**
	 * Username of the currently logged-in user (asynchronous).
	 *
	 * @remarks
	 * Awaits {@link Authorization.userDefinitionAsync}.
	 * @example
	 * const name = await Authorization.usernameAsync;
	 */
	let usernameAsync: Promise<string>;
	/**
	 * User definition for the currently logged-in user (synchronous).
	 *
	 * @remarks
	 * Retrieved via {@link getRemoteData}`("UserData")`. This may trigger a
	 * synchronous load if not cached — prefer {@link Authorization.userDefinitionAsync}.
	 * Returns `undefined`/`null` when not logged in.
	 * @example
	 * if (Authorization.userDefinition?.IsAdmin) {
	 *     // super-admin branch
	 * }
	 */
	let userDefinition: UserDefinition;
	/**
	 * User definition for the currently logged-in user (asynchronous).
	 *
	 * @remarks
	 * Retrieved via {@link getRemoteDataAsync}`("UserData")`.
	 * @example
	 * if ((await Authorization.userDefinitionAsync)?.IsAdmin) {
	 *     // super-admin branch
	 * }
	 */
	let userDefinitionAsync: Promise<UserDefinition>;
}
/**
 * Blocks user interaction by overlaying the page with a transparent, wait-cursor layer.
 *
 * @remarks
 * Maintains an internal reference count so nested `blockUI` / {@link blockUndo} pairs
 * are balanced — the overlay is only removed when the count returns to zero.
 * No-ops on the server (when `document` is undefined). The overlay is a fixed-position
 * `div.blockUI.blockOverlay` appended to `document.body`.
 * @param options - Optional behavior tuning.
 * @param options.zIndex - CSS `z-index` for the overlay. Defaults to `2000`.
 * @param options.useTimeout - When `true`, defers insertion via `setTimeout(…, 0)` and
 * coalesces multiple synchronous calls so only one overlay is created in the next tick.
 * When `false`/`undefined`, the overlay is inserted synchronously.
 * @example
 * blockUI(); // block immediately
 * try {\n *   await save();\n * } finally {\n *   blockUndo();\n * }
 * @example
 * blockUI({ zIndex: 3000, useTimeout: true }); // coalesced, higher z-index
 */
export declare function blockUI(options?: {
	zIndex?: number;
	useTimeout?: boolean;
}): void;
/**
 * Decrements the block-UI reference count and removes the overlay when it reaches zero.
 *
 * @remarks
 * Also cancels any pending deferred {@link blockUI} timer scheduled with `useTimeout: true`.
 * No-ops if the count is already zero. Removes the `:scope > .blockUI.blockOverlay` element
 * from `document.body` when the last blocker is undone.
 * @example
 * blockUndo(); // paired with a prior blockUI()
 */
export declare function blockUndo(): void;
/**
 * Global runtime configuration for the Serenity client framework.
 *
 * @remarks
 * Implemented as a mutable singleton object. Values are typically set once during
 * application startup (e.g. in `ScriptInit.ts`) and read throughout the app.
 * {@link resetApplicationPath} and {@link resetCspNonce} re-read values from the DOM
 * and are called automatically on module load.
 * @example
 * Config.rootNamespaces.push("MyApp");
 * Config.emailAllowOnlyAscii = false;
 */
export declare const Config: {
	/**
	 * Root path of the application, always starting and ending with `/` when read from DOM.
	 *
	 * @remarks
	 * Initialized from `<link id="ApplicationPath" href="/mysite/">` in `_LayoutHead.cshtml`;
	 * falls back to `"/"` if the element is absent or on the server. Change it at runtime
	 * or call {@link resetApplicationPath} after dynamically updating the link element.
	 * @example
	 * // app hosted at http://localhost/mysite/
	 * Config.applicationPath; // "/mysite/"
	 */
	applicationPath: string;
	/**
	 * Content Security Policy nonce to apply to dynamically created `<script>` / `<style>` tags.
	 *
	 * @remarks
	 * Initialized from `<meta name="csp-nonce">` or the `nonce` attribute of existing
	 * `<script>`/`<style>` elements via {@link resetCspNonce}; `null` when no nonce is present
	 * or on the server. Helpers that inject markup should copy this value to the `nonce` attribute.
	 */
	cspNonce: string;
	/**
	 * Returns a fallback URL to redirect to when no explicit return URL is provided.
	 *
	 * @remarks
	 * Default implementation returns {@link Config.applicationPath} regardless of purpose.
	 * Override to provide per-purpose defaults (e.g. different landing pages after login vs. logout).
	 * @param purpose - Optional hint such as `"login"` or `"logout"`.
	 * @returns The URL to use as a return target.
	 * @example
	 * Config.defaultReturnUrl = (purpose) => purpose === "logout" ? "/Goodbye" : Config.applicationPath;
	 */
	defaultReturnUrl: (purpose?: string) => string;
	/**
	 * Whether e-mail validation should allow only ASCII characters.
	 *
	 * @remarks
	 * `true` (default) rejects non-ASCII characters in the local/domain parts;
	 * set to `false` to allow Unicode/IDN addresses.
	 */
	emailAllowOnlyAscii: boolean;
	/**
	 * Optional callback to lazily resolve a type that is not yet in the type registry.
	 *
	 * @remarks
	 * Useful with code-splitting / lazy chunk loading. Called with the requested type key
	 * and a `kind` hint (`"dialog"`, `"editor"`, `"enum"`, `"formatter"`, `"filtering"`, …).
	 * May return the type synchronously or a `Promise` resolving to it; returning `null`/`undefined`
	 * signals "not found".
	 * @param typeKey - Full type name being requested, e.g. `"MyApp.MyEditor"`.
	 * @param kind - Category of the type, used to narrow search/loading.
	 * @returns The resolved type, a promise for it, or `null` if unavailable.
	 * @example
	 * Config.lazyTypeLoader = async (typeKey) => await import(`./editors/${typeKey}`);
	 */
	lazyTypeLoader: (typeKey: string, kind: "dialog" | "editor" | "enum" | "formatter" | "filtering" | string) => any | Promise<any>;
	/**
	 * Root namespaces probed when resolving short type names.
	 *
	 * @remarks
	 * When a type is requested as `"MyEditor"`, the registry first tries `"MyEditor"`,
	 * then `"Serenity.MyEditor"`, then `"<each rootNamespace>.MyEditor"`.
	 * Add your application namespace (e.g. `"MyApp"`) in `ScriptInit.ts` so short names resolve.
	 * Defaults to `["Serenity"]`.
	 * @example
	 * Config.rootNamespaces.push("MyApp");
	 */
	rootNamespaces: string[];
	/**
	 * Optional handler invoked when a service call returns `NotAuthorized` / session expired.
	 *
	 * @remarks
	 * If set, Serenity delegates the "not logged in" flow to this callback so you can
	 * prompt the user, redirect to login, or refresh a token. When `null` (default),
	 * the framework falls back to its built-in handling.
	 * @example
	 * Config.notLoggedInHandler = () => window.location.href = "/Account/Login";
	 */
	notLoggedInHandler: Function;
};
/**
 * Re-reads {@link Config.applicationPath} from the DOM.
 *
 * @remarks
 * Looks for `<link id="ApplicationPath">` and copies its `href`; falls back to `"/"`
 * if absent or when `document` is unavailable (SSR). Called once on module load;
 * call again after you change the link element at runtime.
 * @example
 * document.getElementById("ApplicationPath").href = "/newPath/";
 * resetApplicationPath();
 */
export declare function resetApplicationPath(): void;
/**
 * Re-reads {@link Config.cspNonce} from the DOM.
 *
 * @remarks
 * Probes in order: `<meta name="csp-nonce">` → `<script nonce>` → `<style nonce>`.
 * Sets `null` if none found or when `document` is unavailable. Called once on module load.
 */
export declare function resetCspNonce(): void;
/**
 * Fluent builder for Serenity criteria expressions with completion support.
 *
 * @remarks
 * Extends `Array` so an instance itself acts as a field-reference token (e.g. `["Amount"]`).
 * Create instances via {@link Criteria}`("FieldName")` rather than `new CriteriaBuilder()`.
 * Each method returns a Serenity criteria tuple/array that can be combined with
 * {@link Criteria.and}, {@link Criteria.or}, {@link Criteria.join}, or the
 * {@link parseCriteria} parser. `bw` stands for "between" (inclusive).
 * @example
 * Criteria("Age").ge(18); // [["Age"], ">=", 18]
 * @example
 * Criteria("Status").in([1, 2, 3]); // [["Status"], "in", [[1, 2, 3]]]
 */
export declare class CriteriaBuilder extends Array {
	/**
	 * Creates a BETWEEN (inclusive) criteria: `field >= from AND field <= to`.
	 *
	 * @param fromInclusive - Lower bound (inclusive).
	 * @param toInclusive - Upper bound (inclusive).
	 * @returns Composite criteria `[[field, ">=", from], "and", [field, "<=", to]]`.
	 * @example
	 * Criteria("Amount").bw(10, 20);
	 */
	bw(fromInclusive: any, toInclusive: any): Array<any>;
	/**
	 * Creates a `LIKE '%value%'` (contains) criteria.
	 *
	 * @param value - Substring to search for. Wrapped with `%` on both sides.
	 * @returns Criteria `[field, "like", "%value%"]`.
	 * @example
	 * Criteria("Name").contains("ser"); // [["Name"], "like", "%ser%"]
	 */
	contains(value: string): Array<any>;
	/**
	 * Creates a `LIKE '%value'` (ends-with) criteria.
	 *
	 * @param value - Suffix to match. Prefixed with `%`.
	 * @returns Criteria `[field, "like", "%value"]`.
	 * @example
	 * Criteria("Email").endsWith("@example.com");
	 */
	endsWith(value: string): Array<any>;
	/**
	 * Creates an equality (`=`) criteria.
	 *
	 * @param value - Value to compare for equality.
	 * @returns Criteria `[field, "=", value]`.
	 * @example
	 * Criteria("IsActive").eq(true);
	 */
	eq(value: any): Array<any>;
	/**
	 * Creates a greater-than (`>`) criteria.
	 *
	 * @param value - Lower exclusive bound.
	 * @returns Criteria `[field, ">", value]`.
	 */
	gt(value: any): Array<any>;
	/**
	 * Creates a greater-than-or-equal (`>=`) criteria.
	 *
	 * @param value - Lower inclusive bound.
	 * @returns Criteria `[field, ">=", value]`.
	 */
	ge(value: any): Array<any>;
	/**
	 * Creates an `IN` criteria.
	 *
	 * @param values - Array of allowed values. Wrapped as `[values]` per Serenity wire format.
	 * @returns Criteria `[field, "in", [values]]`.
	 * @example
	 * Criteria("Status").in([1, 2]); // [["Status"], "in", [[1, 2]]]
	 */
	in(values: any[]): Array<any>;
	/**
	 * Creates an `IS NULL` criteria.
	 *
	 * @returns Criteria `["is null", field]`.
	 */
	isNull(): Array<any>;
	/**
	 * Creates an `IS NOT NULL` criteria.
	 *
	 * @returns Criteria `["is not null", field]`.
	 */
	isNotNull(): Array<any>;
	/**
	 * Creates a less-than-or-equal (`<=`) criteria.
	 *
	 * @param value - Upper inclusive bound.
	 * @returns Criteria `[field, "<=", value]`.
	 */
	le(value: any): Array<any>;
	/**
	 * Creates a less-than (`<`) criteria.
	 *
	 * @param value - Upper exclusive bound.
	 * @returns Criteria `[field, "<", value]`.
	 */
	lt(value: any): Array<any>;
	/**
	 * Creates a not-equal (`!=`) criteria.
	 *
	 * @param value - Value that the field must not equal.
	 * @returns Criteria `[field, "!=", value]`.
	 */
	ne(value: any): Array<any>;
	/**
	 * Creates a `LIKE` criteria with the exact pattern provided.
	 *
	 * @param value - SQL LIKE pattern (use `%` / `_` wildcards as needed).
	 * @returns Criteria `[field, "like", value]`.
	 * @example
	 * Criteria("Name").like("A%");
	 */
	like(value: any): Array<any>;
	/**
	 * Creates a `LIKE 'value%'` (starts-with) criteria.
	 *
	 * @param value - Prefix to match. Suffixed with `%`.
	 * @returns Criteria `[field, "like", "value%"]`.
	 * @example
	 * Criteria("Name").startsWith("Jo"); // [["Name"], "like", "Jo%"]
	 */
	startsWith(value: string): Array<any>;
	/**
	 * Creates a `NOT IN` criteria.
	 *
	 * @param values - Array of disallowed values. Wrapped as `[values]`.
	 * @returns Criteria `[field, "not in", [values]]`.
	 */
	notIn(values: any[]): Array<any>;
	/**
	 * Creates a `NOT LIKE` criteria.
	 *
	 * @param value - SQL LIKE pattern that the field must not match.
	 * @returns Criteria `[field, "not like", value]`.
	 */
	notLike(value: any): Array<any>;
}
/**
 * Parses a criteria expression string to Serenity criteria array format.
 *
 * @remarks
 * Supports named parameters via `@name` placeholders. Operator precedence is handled
 * via a shunting-yard pass; string literals use single quotes with `''` escaping.
 * @param expression - Expression text, e.g. `"A >= @p1 and B < @p2"`.
 * @param params - Dictionary mapping parameter names to values, e.g. `{ p1: 5, p2: 4 }`.
 * @returns Serenity criteria array, e.g. `[[["A"], ">=", 5], "and", [["B"], "<", 4]]`.
 * @example
 * `parseCriteria('A >= @p1 and B < @p2', { p1: 5, p2: 4 }) // [[[a], '>=' 5], 'and', [[b], '<', 4]]`
 */
export declare function parseCriteria(expression: string, params?: any): any[];
/**
 * Parses a tagged-template criteria expression to Serenity criteria array format.
 *
 * @remarks
 * Each interpolated value becomes an auto-named `@__N` parameter, avoiding manual
 * parameter dictionaries and SQL-injection-prone concatenation.
 * @param strings - Template string fragments.
 * @param values - Interpolated values (one per placeholder).
 * @returns Serenity criteria array.
 * @example
 * let a = 5, b = 4;
 * parseCriteria`A >= ${a} and B < ${b}`; // [[["A"], ">=", 5], "and", [["B"], "<", 4]]
 */
export declare function parseCriteria(strings: TemplateStringsArray, ...values: any[]): any[];
/**
 * String constants for every operator that can appear in a Serenity criteria expression.
 *
 * @remarks
 * Values match the wire-format tokens accepted by the server (e.g. `"="`, `"like"`,
 * `"is null"`). Exposed also as {@link Criteria.Operator} for convenience.
 */
export declare enum CriteriaOperator {
	paren = "()",
	not = "not",
	isNull = "is null",
	isNotNull = "is not null",
	exists = "exists",
	and = "and",
	or = "or",
	xor = "xor",
	eq = "=",
	ne = "!=",
	gt = ">",
	ge = ">=",
	lt = "<",
	le = "<=",
	in = "in",
	notIn = "not in",
	like = "like",
	notLike = "not like"
}
/**
 * Creates a fluent {@link CriteriaBuilder} for the given field.
 *
 * @remarks
 * The returned builder extends `Array` so it doubles as a field token. A prototype
 * fixup handles environments where subclassing `Array` is unreliable.
 * @param field - Field name / property key, e.g. `"Amount"` or `"Customer.Name"`.
 * @returns A {@link CriteriaBuilder} bound to the field.
 * @example
 * Criteria("Age").ge(18);
 * @example
 * Criteria("Name").contains("acme");
 */
export declare function Criteria(field: string): CriteriaBuilder;
/**
 * Helpers for composing and inspecting Serenity criteria arrays.
 *
 * @remarks
 * Criteria are plain arrays in the form `[left, operator, right]` (binary),
 * `[operator, operand]` (unary), or nested with `"and"`/`"or"` joiners.
 * The static helpers here handle empty-value short-circuiting so callers can
 * unconditionally combine optional filters.
 */
export declare namespace Criteria {
	/** Alias for {@link CriteriaOperator} — the set of valid operator tokens. */
	const Operator: typeof CriteriaOperator;
	/**
	 * Returns `true` if the criteria is empty / falsy.
	 *
	 * @param c - Criteria array to test. `null`/`undefined` counts as empty.
	 * @returns `true` if `c` is `null`, `undefined`, `[]`, or `[""]`.
	 */
	function isEmpty(c: any[]): boolean;
	/**
	 * Joins two criteria with an operator, skipping empty sides.
	 *
	 * @param c1 - Left criteria. If empty, `c2` is returned as-is.
	 * @param op - Join operator, typically `"and"` or `"or"` / `"xor"`.
	 * @param c2 - Right criteria. If empty, `c1` is returned as-is.
	 * @returns `[c1, op, c2]` or whichever side is non-empty, if the other is empty.
	 */
	function join(c1: any[], op: string, c2: any[]): any[];
	/**
	 * Negates a criteria with the `not` operator.
	 *
	 * @param c - Criteria to negate.
	 * @returns `["not", c]`.
	 */
	function not(c: any[]): (string | any[])[];
	/**
	 * Combines two or more criteria with `and`, skipping empty entries.
	 *
	 * @param c1 - First criteria.
	 * @param c2 - Second criteria.
	 * @param rest - Additional criteria joined incrementally with `and`.
	 * @returns Combined criteria or the sole non-empty input if others are empty.
	 */
	function and(c1: any[], c2: any[], ...rest: any[][]): any[];
	/**
	 * Combines two or more criteria with `or`, skipping empty entries.
	 *
	 * @param c1 - First criteria.
	 * @param c2 - Second criteria.
	 * @param rest - Additional criteria joined incrementally with `or`.
	 * @returns Combined criteria or the sole non-empty input if others are empty.
	 */
	function or(c1: any[], c2: any[], ...rest: any[][]): any[];
	/**
	 * Wraps a criteria in parentheses (compatibility helper).
	 *
	 * @remarks
	 * Produces `["()", c]`. The server treats this as a grouping no-op but it can
	 * preserve intended precedence when criteria are serialized. Returns `c` unchanged if empty.
	 * @param c - Criteria to wrap.
	 * @returns `["()", c]` or `c` if empty.
	 */
	function paren(c: any[]): any[];
	/**
	 * Alias for {@link parseCriteria} — parses a criteria expression string or tagged template.
	 *
	 * @remarks
	 * Accepts either `"A >= @p1"` with a params object, or a tagged template
	 * `` Criteria.parse`A >= ${value}` ``. See {@link parseCriteria} for details.
	 * @example
	 * Criteria.parse("A >= @p1 and B < @p2", { p1: 5, p2: 4 });
	 * @example
	 * let a = 5, b = 4;
	 * Criteria.parse`A >= ${a} and B < ${b}`;
	 */
	const parse: typeof parseCriteria;
}
/**
 * A debounced wrapper around a function `T` with helper methods.
 *
 * @typeParam T - The original function type being debounced.
 * @remarks
 * The callable signature applies debounce timing; {@link DebouncedFunction.clear}
 * cancels a pending invocation and {@link DebouncedFunction.flush} forces it to run now.
 * @example
 * const onResize = debounce(() => layout(), 150);
 * window.addEventListener("resize", onResize);
 * onResize.clear(); // cancel pending call
 */
export interface DebouncedFunction<T extends (...args: any[]) => any> {
	/**
	 * Invokes the debounced function, applying debounce timing rules.
	 *
	 * @param args - Arguments forwarded to the original function `T`.
	 * @returns Return value of the last immediate invocation, or `undefined` if the call was deferred / never invoked.
	 */
	(...args: Parameters<T>): ReturnType<T> | undefined;
	/**
	 * Cancels any pending (not yet fired) invocation.
	 *
	 * @example
	 * const fn = debounce(save, 300);
	 * fn(); fn.clear(); // save will not run
	 */
	clear(): void;
	/**
	 * Immediately invokes the pending debounced call (if any) and returns its result.
	 *
	 * @returns Return value of the flushed invocation, or the last invocation's return value if nothing was pending, or `undefined` if never invoked.
	 * @example
	 * const fn = debounce(save, 300);
	 * fn(); fn.flush(); // save runs now instead of after 300 ms
	 */
	flush(): ReturnType<T> | undefined;
}
/**
 * Creates a debounced function that delays invoking `func` until after `wait` ms have elapsed
 * since the last time it was invoked.
 *
 * @remarks
 * When `immediate` is `false` (default), `func` is invoked on the trailing edge after the quiet period.
 * When `immediate` is `true`, `func` is invoked on the leading edge and subsequent calls within
 * `wait` ms are ignored. The returned function exposes {@link DebouncedFunction.clear} to cancel
 * a pending trailing call and {@link DebouncedFunction.flush} to run it immediately. `wait` defaults to `100` ms.
 * @typeParam T - Type of the function to debounce.
 * @param func - Function to debounce.
 * @param wait - Delay in milliseconds to wait after the last call before invoking `func`. Defaults to `100`.
 * @param immediate - If `true`, trigger on the leading edge instead of the trailing edge. Defaults to `false`.
 * @returns Debounced wrapper with `clear` and `flush` helpers.
 * @example
 * const save = debounce(() => api.save(data), 500);
 * save(); save(); // only the last call triggers after 500 ms of quiet
 * @example
 * const track = debounce(() => analytics.send(), 200, true); // leading-edge
 * @example
 * const fn = debounce(() => console.log("hi"), 300);
 * fn(); fn.clear(); // cancels
 * fn(); fn.flush(); // forces immediate invocation
 */
export declare function debounce<T extends (...args: any) => any>(func: T, wait?: number, immediate?: boolean): DebouncedFunction<T>;
/**
 * Content that can be rendered as a toast/notification message or appended to the DOM.
 * Accepts plain strings, DOM elements (HTML/SVG/MathML) and document fragments, which are
 * handled by {@link appendToNode} and the notification helpers.
 */
export type RenderableContent = string | HTMLElement | SVGElement | MathMLElement | DocumentFragment;
/**
 * HTML-encodes a value by escaping `<`, `>`, `"`, `'`, and `&`.
 * @param s - Value to encode. Non-string values are coerced to string; `null`/`undefined` yields an empty string.
 * @returns The HTML-escaped string, safe for interpolation into HTML markup.
 * @example
 * ```ts
 * htmlEncode('<a href="x">a & b</a>'); // "&lt;a href=&quot;x&quot;&gt;a &amp; b&lt;/a&gt;"
 * ```
 */
export declare function htmlEncode(s: any): string;
/**
 * Toggles one or more CSS classes on an element, supporting space-separated lists.
 * When `cls` contains spaces it is split and each token is toggled individually.
 * @param el - Target element. No-op if falsy.
 * @param cls - Single class or space-separated class list to toggle. No-op if `null`/empty.
 * @param add - Force mode: `true` to add, `false` to remove, `undefined` to toggle.
 * @remarks Delegates to `Element.classList.toggle` per token, preserving existing classes.
 */
export declare function toggleClass(el: Element, cls: string, add?: boolean): void;
/**
 * Adds one or more CSS classes to an element.
 * @param el - Target element.
 * @param cls - Class name or space-separated list of class names to add.
 * @remarks Wraps {@link toggleClass} with `add=true`; no-ops for empty/null inputs.
 */
export declare function addClass(el: Element, cls: string): void;
/**
 * Removes one or more CSS classes from an element.
 * @param el - Target element.
 * @param cls - Class name or space-separated list of class names to remove.
 * @remarks Wraps {@link toggleClass} with `add=false`; no-ops for empty/null inputs.
 */
export declare function removeClass(el: Element, cls: string): void;
/**
 * Appends heterogeneous content to a parent node.
 * Handles strings (as text nodes), `Node` instances, array-like collections (recursively),
 * promise-like values (async placeholder replaced on resolve/reject), and primitive values via `Node.append`.
 * Falsy values `null`, `undefined`, and `false` are ignored.
 * @param parent - Target parent node to append into.
 * @param child - Content to append: a single value, array-like collection, `Node`, string, or `PromiseLike`.
 * @remarks Promise children insert a comment placeholder synchronously and replace it with the resolved fragment when settled.
 */
export declare function appendToNode(parent: ParentNode, child: any): void;
/**
 * Sanitizes a URL for safe use in `href`/`src` attributes.
 * Allows `http`, `https`, `mailto`, `ftp`, `tel`, `file`, `sms`, safe relative URLs, and safe `data:` image/video/audio URLs.
 * Preserves `about:blank` and `javascript:void(0)` idioms; otherwise prefixes unsafe values with `unsafe:`.
 * @param url - URL string to sanitize; trimmed before validation.
 * @returns A safe URL string, or `unsafe:<original>` if the input fails validation.
 * @example
 * ```ts
 * sanitizeUrl("javascript:alert(1)"); // "unsafe:javascript:alert(1)"
 * sanitizeUrl("/app/page?x=1"); // "/app/page?x=1"
 * ```
 */
export declare function sanitizeUrl(url: string): string;
/**
 * Gets the read-only state of a DOM element without consulting attached widgets.
 * Considers the `readonly` CSS class, the `disabled` attribute for `select`/`radio`/`checkbox`,
 * and the `readonly` attribute for other inputs.
 * @param el - Element to inspect. Returns `null` if `el` is `null`/`undefined`.
 * @returns `true` if read-only/disabled, `false` otherwise, or `null` when `el` is absent.
 */
export declare function getElementReadOnly(el: Element): boolean | null;
/**
 * Sets the read-only appearance and attribute on one or more elements without touching attached widgets.
 * Toggles the `readonly` CSS class and sets `disabled` (for `select`/`radio`/`checkbox`) or `readonly` (for other elements).
 * @param elements - Single element or array-like collection of elements. No-op if falsy.
 * @param value - `true` to make read-only/disabled, `false` to make editable.
 */
export declare function setElementReadOnly(elements: Element | ArrayLike<Element>, value: boolean): void;
/**
 * Parses a URL query string into a key/value map.
 * @param s - Query string to parse (without leading `?` is also accepted). When `undefined`, `location.search` is used.
 * @returns An object mapping decoded keys to decoded values. Keys without `=` map to their own name; malformed percent-encodings are skipped.
 * @example
 * ```ts
 * parseQueryString("a=1&b=hello%20world"); // { a: "1", b: "hello world" }
 * ```
 */
export declare function parseQueryString(s?: string): Record<string, string>;
/**
 * Checks whether a return URL is safe for redirects.
 * A safe URL must be a relative path starting with exactly one `/`, contain no protocol (`:`), backslashes, control characters, or `//` after the leading slash, and use only `\w`, `-`, `.`, `/`, `?`, `&`, `=`, `%` characters.
 * @param url - Candidate return URL to validate.
 * @returns `true` if the URL is safe to use as a redirect target, `false` otherwise.
 */
export declare function isSafeReturnUrl(url: string): boolean;
/**
 * Retrieves the `returnUrl` from the current query string, falling back to application config.
 * @param opt - Options controlling lookup behavior.
 * @param opt.queryOnly - When `true`, only the query string is checked; skips {@link Config.defaultReturnUrl}.
 * @param opt.ignoreUnsafe - When `true`, unsafe URLs are returned as-is; otherwise unsafe values are discarded (`null`).
 * @param opt.purpose - Purpose key forwarded to `Config.defaultReturnUrl` when no query-string value is found.
 * @returns The validated return URL, the configured default, or `null`/`undefined` if none is available or the query value is unsafe.
 */
export declare function getReturnUrl(opt?: {
	/** Whether to only consider the query string. If true, the function will not check the default return URL. */
	queryOnly?: boolean;
	/** Whether to ignore unsafe URLs. If false or null (default), the function will only return safe URLs. */
	ignoreUnsafe?: boolean;
	/** The purpose of the return URL. This can be used to determine the default return URL if none is found in the query string. */
	purpose?: string;
}): string;
/**
 * Escapes a string for safe use as a CSS identifier/selector.
 * Delegates to `CSS.escape` when available; otherwise implements the CSSOM spec polyfill.
 * @param selector - Raw selector/identifier to escape.
 * @returns The escaped selector string safe for `querySelector` and CSS rules.
 */
export declare function cssEscape(selector: string): string;
/**
 * Sanitizes an HTML string by stripping dangerous elements and attributes.
 * Preference order: SleekGrid sanitizer (`sleekgrid.formatterContext()?.sanitizer` or `sleekgrid.gridDefaults.sanitizer`), `DOMPurify.sanitize` if present, otherwise a built-in `DOMParser` implementation that removes `script`/`iframe`/`object`/`embed`/`form`/`style`/`link` and event-handler / unsafe-URL attributes.
 * Falls back to {@link htmlEncode} if `DOMParser` is unavailable or parsing throws.
 * @param dirtyHtml - Untrusted HTML markup to sanitize. Falsy values return an empty string; strings without HTML tags/entities are returned as-is (fast path).
 * @returns The sanitized HTML string safe for insertion via `innerHTML`.
 * @remarks This duplicates the basic DOM sanitizer logic so corelib works standalone with or without SleekGrid loaded.
 */
export declare function sanitizeHtml(dirtyHtml: string): string;
/**
 * Bootstrap contextual utility colors usable for icon/background styling.
 * Maps directly to `bg-*` / `text-*` CSS helper classes (e.g. `bg-primary`, `text-danger`).
 */
export type UtilityColor = "primary" | "secondary" | "success" | "danger" | "warning" | "info" | "light" | "dark" | "muted" | "white";
/**
 * Text color token for {@link textColor} / {@link faIcon} / {@link fabIcon}.
 * Extends {@link UtilityColor} with additional named CSS colors (`aqua`, `blue`, `fuschia`, `gray`, `green`, `light-blue`, `lime`, `maroon`, `navy`, `olive`, `orange`, `purple`, `red`, `teal`, `yellow`) that map to `text-*` utility classes.
 */
export type TextColor = UtilityColor | "aqua" | "blue" | "fuschia" | "gray" | "green" | "light-blue" | "lime" | "maroon" | "navy" | "olive" | "orange" | "purple" | "red" | "teal" | "yellow";
/**
 * Returns the Bootstrap background utility class for a {@link UtilityColor}.
 * @param color - Utility color token (e.g. `"primary"`, `"danger"`).
 * @returns CSS class name such as `"bg-primary"`.
 * @example
 * ```ts
 * bgColor("success"); // "bg-success"
 * ```
 */
export declare function bgColor(color: UtilityColor): string;
/**
 * Returns the text utility class for a {@link TextColor}.
 * @param color - Text color token.
 * @returns CSS class name such as `"text-primary"` or `"text-teal"`.
 * @example
 * ```ts
 * textColor("warning"); // "text-warning"
 * ```
 */
export declare function textColor(color: TextColor): string;
/**
 * Builds the CSS class string for a Font Awesome (solid/regular) icon.
 * @param key - Icon key from {@link faIconKey} (without the `fa-` prefix).
 * @param color - Optional {@link TextColor} appended as a `text-*` class.
 * @returns Class string such as `"fa fa-home"` or `"fa fa-home text-danger"`.
 * @example
 * ```ts
 * faIcon("home"); // "fa fa-home"
 * faIcon("home", "primary"); // "fa fa-home text-primary"
 * ```
 */
export declare function faIcon(key: faIconKey, color?: TextColor): string;
/**
 * Builds the CSS class string for a Font Awesome Brands icon.
 * @param key - Brand icon key from {@link fabIconKey} (without the `fa-` prefix).
 * @param color - Optional {@link TextColor} appended as a `text-*` class.
 * @returns Class string such as `"fab fa-github"` or `"fab fa-github text-muted"`.
 * @example
 * ```ts
 * fabIcon("github"); // "fab fa-github"
 * ```
 */
export declare function fabIcon(key: fabIconKey, color?: TextColor): string;
/**
 * Strongly-typed union of known Font Awesome class strings (`fa fa-*` or `fab fa-*`).
 * Provides compile-time completion for recognized icon names; use {@link AnyIconClass} to allow arbitrary strings as well.
 */
export type KnownIconClass = `fa fa-${faIconKey}` | `fab fa-${fabIconKey}`;
/**
 * Icon class type that accepts either a {@link KnownIconClass} (with completions) or any custom string class.
 * The `(string & {})` trick preserves autocomplete for known values while still allowing arbitrary classes.
 */
export type AnyIconClass = KnownIconClass | (string & {});
/**
 * Flexible icon class input accepted by {@link iconClassName}: a single {@link AnyIconClass} string or an array of them.
 */
export type IconClassName = AnyIconClass | (AnyIconClass[]);
/**
 * Normalizes an {@link IconClassName} to a single space-joined class string, adding a missing `fa` prefix.
 * @param icon - Single icon class or array of classes. Falsy/empty values are returned as-is.
 * @returns Normalized CSS class string suitable for an `<i>` element.
 * @remarks If `icon` starts with `fa-` but lacks an explicit `fa` token, `"fa "` is prepended.
 * @example
 * ```ts
 * iconClassName("fa-home"); // "fa fa-home"
 * iconClassName(["fa fa-home", "text-primary"]); // "fa fa-home text-primary"
 * ```
 */
export declare function iconClassName(icon: IconClassName): string;
/**
 * Union of Font Awesome (solid/regular) icon keys (without the `fa-` prefix).
 * Generated from the Line-Awesome / Font Awesome catalog; used by {@link faIcon} and {@link KnownIconClass}.
 */
export type faIconKey = "ad" | "address-book" | "address-card" | "adjust" | "air-freshener" | "align-center" | "align-justify" | "align-left" | "align-right" | "allergies" | "ambulance" | "american-sign-language-interpreting" | "anchor" | "angle-double-down" | "angle-double-left" | "angle-double-right" | "angle-double-up" | "angle-down" | "angle-left" | "angle-right" | "angle-up" | "angry" | "ankh" | "apple-alt" | "archive" | "archway" | "arrow-alt-circle-down" | "arrow-alt-circle-left" | "arrow-alt-circle-right" | "arrow-alt-circle-up" | "arrow-circle-down" | "arrow-circle-left" | "arrow-circle-right" | "arrow-circle-up" | "arrow-down" | "arrow-left" | "arrow-right" | "arrow-up" | "arrows-alt" | "arrows-alt-h" | "arrows-alt-v" | "assistive-listening-systems" | "asterisk" | "at" | "atlas" | "atom" | "audio-description" | "award" | "baby" | "baby-carriage" | "backspace" | "backward" | "bacon" | "balance-scale" | "balance-scale-left" | "balance-scale-right" | "ban" | "band-aid" | "barcode" | "bars" | "baseball-ball" | "basketball-ball" | "bath" | "battery-empty" | "battery-full" | "battery-half" | "battery-quarter" | "battery-three-quarters" | "bed" | "beer" | "bell" | "bell-o" | "bell-slash" | "bezier-curve" | "bible" | "bicycle" | "biking" | "binoculars" | "biohazard" | "birthday-cake" | "blender" | "blender-phone" | "blind" | "blog" | "bold" | "bolt" | "bomb" | "bone" | "bong" | "book" | "book-dead" | "book-medical" | "book-open" | "book-reader" | "bookmark" | "border-all" | "border-none" | "border-style" | "bowling-ball" | "box" | "box-open" | "boxes" | "braille" | "brain" | "bread-slice" | "briefcase" | "briefcase-medical" | "broadcast-tower" | "broom" | "brush" | "bug" | "building" | "bullhorn" | "bullseye" | "burn" | "bus" | "bus-alt" | "business-time" | "calculator" | "calendar" | "calendar-alt" | "calendar-check" | "calendar-day" | "calendar-minus" | "calendar-plus" | "calendar-times" | "calendar-week" | "camera" | "camera-retro" | "campground" | "candy-cane" | "cannabis" | "capsules" | "car" | "car-alt" | "car-battery" | "car-crash" | "car-side" | "caret-down" | "caret-left" | "caret-right" | "caret-square-down" | "caret-square-left" | "caret-square-right" | "caret-square-up" | "caret-up" | "carrot" | "cart-arrow-down" | "cart-plus" | "cash-register" | "cat" | "certificate" | "chair" | "chalkboard" | "chalkboard-teacher" | "charging-station" | "chart-area" | "chart-bar" | "chart-line" | "chart-pie" | "check" | "check-circle" | "check-double" | "check-square" | "cheese" | "chess" | "chess-bishop" | "chess-board" | "chess-king" | "chess-knight" | "chess-pawn" | "chess-queen" | "chess-rook" | "chevron-circle-down" | "chevron-circle-left" | "chevron-circle-right" | "chevron-circle-up" | "chevron-down" | "chevron-left" | "chevron-right" | "chevron-up" | "child" | "church" | "circle" | "circle-notch" | "city" | "clinic-medical" | "clipboard" | "clipboard-check" | "clipboard-list" | "clock" | "clock-o" | "clone" | "closed-captioning" | "cloud" | "cloud-download-alt" | "cloud-meatball" | "cloud-moon" | "cloud-moon-rain" | "cloud-rain" | "cloud-showers-heavy" | "cloud-sun" | "cloud-sun-rain" | "cloud-upload-alt" | "cocktail" | "code" | "code-branch" | "coffee" | "cog" | "cogs" | "coins" | "columns" | "comment" | "comment-alt" | "comment-dollar" | "comment-dots" | "comment-medical" | "comment-slash" | "comments" | "comments-dollar" | "compact-disc" | "compass" | "compress" | "compress-arrows-alt" | "concierge-bell" | "cookie" | "cookie-bite" | "copy" | "copyright" | "couch" | "credit-card" | "crop" | "crop-alt" | "cross" | "crosshairs" | "crow" | "crown" | "crutch" | "cube" | "cubes" | "cut" | "database" | "deaf" | "democrat" | "desktop" | "dharmachakra" | "diagnoses" | "dice" | "dice-d20" | "dice-d6" | "dice-five" | "dice-four" | "dice-one" | "dice-six" | "dice-three" | "dice-two" | "digital-tachograph" | "directions" | "divide" | "dizzy" | "dna" | "dog" | "dollar-sign" | "dolly" | "dolly-flatbed" | "donate" | "door-closed" | "door-open" | "dot-circle" | "dove" | "download" | "drafting-compass" | "dragon" | "draw-polygon" | "drum" | "drum-steelpan" | "drumstick-bite" | "dumbbell" | "dumpster" | "dumpster-fire" | "dungeon" | "edit" | "egg" | "eject" | "ellipsis-h" | "ellipsis-v" | "envelope" | "envelope-o" | "envelope-open" | "envelope-open-text" | "envelope-square" | "equals" | "eraser" | "ethernet" | "euro-sign" | "exchange-alt" | "exclamation" | "exclamation-circle" | "exclamation-triangle" | "expand" | "expand-arrows-alt" | "external-link-alt" | "external-link-square-alt" | "eye" | "eye-dropper" | "eye-slash" | "fan" | "fast-backward" | "fast-forward" | "fax" | "feather" | "feather-alt" | "female" | "fighter-jet" | "file" | "file-alt" | "file-archive" | "file-audio" | "file-code" | "file-contract" | "file-csv" | "file-download" | "file-excel" | "file-excel-o" | "file-export" | "file-image" | "file-import" | "file-invoice" | "file-invoice-dollar" | "file-medical" | "file-medical-alt" | "file-pdf" | "file-pdf-o" | "file-powerpoint" | "file-prescription" | "file-signature" | "file-upload" | "file-text" | "file-text-o" | "file-video" | "file-word" | "fill" | "fill-drip" | "film" | "filter" | "fingerprint" | "fire" | "floppy-o" | "fire-alt" | "fire-extinguisher" | "first-aid" | "fish" | "fist-raised" | "flag" | "flag-checkered" | "flag-usa" | "flask" | "flushed" | "folder" | "folder-minus" | "folder-open" | "folder-open-o" | "folder-plus" | "font" | "football-ball" | "forward" | "frog" | "frown" | "frown-open" | "funnel-dollar" | "futbol" | "gamepad" | "gas-pump" | "gavel" | "gem" | "genderless" | "ghost" | "gift" | "gifts" | "glass-cheers" | "glass-martini" | "glass-martini-alt" | "glass-whiskey" | "glasses" | "globe" | "globe-africa" | "globe-americas" | "globe-asia" | "globe-europe" | "golf-ball" | "gopuram" | "graduation-cap" | "greater-than" | "greater-than-equal" | "grimace" | "grin" | "grin-alt" | "grin-beam" | "grin-beam-sweat" | "grin-hearts" | "grin-squint" | "grin-squint-tears" | "grin-stars" | "grin-tears" | "grin-tongue" | "grin-tongue-squint" | "grin-tongue-wink" | "grin-wink" | "grip-horizontal" | "grip-lines" | "grip-lines-vertical" | "grip-vertical" | "guitar" | "h-square" | "hamburger" | "hammer" | "hamsa" | "hand-holding" | "hand-holding-heart" | "hand-holding-usd" | "hand-lizard" | "hand-middle-finger" | "hand-paper" | "hand-peace" | "hand-point-down" | "hand-point-left" | "hand-point-right" | "hand-point-up" | "hand-pointer" | "hand-rock" | "hand-scissors" | "hand-spock" | "hands" | "hands-helping" | "handshake" | "hanukiah" | "hard-hat" | "hashtag" | "hat-cowboy" | "hat-cowboy-side" | "hat-wizard" | "haykal" | "hdd" | "heading" | "headphones" | "headphones-alt" | "headset" | "heart" | "heart-broken" | "heartbeat" | "helicopter" | "highlighter" | "hiking" | "hippo" | "history" | "hockey-puck" | "holly-berry" | "home" | "horse" | "horse-head" | "hospital" | "hospital-alt" | "hospital-symbol" | "hot-tub" | "hotdog" | "hotel" | "hourglass" | "hourglass-end" | "hourglass-half" | "hourglass-start" | "house-damage" | "hryvnia" | "i-cursor" | "ice-cream" | "icicles" | "icons" | "id-badge" | "id-card" | "id-card-alt" | "igloo" | "image" | "images" | "inbox" | "indent" | "industry" | "infinity" | "info" | "info-circle" | "italic" | "jedi" | "joint" | "journal-whills" | "kaaba" | "key" | "keyboard" | "khanda" | "kiss" | "kiss-beam" | "kiss-wink-heart" | "kiwi-bird" | "landmark" | "language" | "laptop" | "laptop-code" | "laptop-medical" | "laugh" | "laugh-beam" | "laugh-squint" | "laugh-wink" | "layer-group" | "leaf" | "lemon" | "less-than" | "less-than-equal" | "level-down-alt" | "level-up-alt" | "life-ring" | "lightbulb" | "link" | "lira-sign" | "list" | "list-alt" | "list-ol" | "list-ul" | "location-arrow" | "lock" | "lock-open" | "long-arrow-alt-down" | "long-arrow-alt-left" | "long-arrow-alt-right" | "long-arrow-alt-up" | "low-vision" | "luggage-cart" | "magic" | "magnet" | "mail-bulk" | "mail-forward" | "mail-reply" | "male" | "map" | "map-marked" | "map-marked-alt" | "map-marker" | "map-marker-alt" | "map-pin" | "map-signs" | "marker" | "mars" | "mars-double" | "mars-stroke" | "mars-stroke-h" | "mars-stroke-v" | "mask" | "medal" | "medkit" | "meh" | "meh-blank" | "meh-rolling-eyes" | "memory" | "menorah" | "mercury" | "meteor" | "microchip" | "microphone" | "microphone-alt" | "microphone-alt-slash" | "microphone-slash" | "microscope" | "minus" | "minus-circle" | "minus-square" | "mitten" | "mobile" | "mobile-alt" | "money-bill" | "money-bill-alt" | "money-bill-wave" | "money-bill-wave-alt" | "money-check" | "money-check-alt" | "monument" | "moon" | "mortar-pestle" | "mosque" | "motorcycle" | "mountain" | "mouse" | "mouse-pointer" | "mug-hot" | "music" | "network-wired" | "neuter" | "newspaper" | "not-equal" | "notes-medical" | "object-group" | "object-ungroup" | "oil-can" | "om" | "otter" | "outdent" | "pager" | "paint-brush" | "paint-roller" | "palette" | "pallet" | "paper-plane" | "paperclip" | "parachute-box" | "paragraph" | "parking" | "passport" | "pastafarianism" | "paste" | "pause" | "pause-circle" | "paw" | "peace" | "pen" | "pen-alt" | "pen-fancy" | "pen-nib" | "pen-square" | "pencil-alt" | "pencil-ruler" | "pencil-square-o" | "people-carry" | "pepper-hot" | "percent" | "percentage" | "person-booth" | "phone" | "phone-alt" | "phone-slash" | "phone-square" | "phone-square-alt" | "phone-volume" | "photo-video" | "piggy-bank" | "pills" | "pizza-slice" | "place-of-worship" | "plane" | "plane-arrival" | "plane-departure" | "play" | "play-circle" | "plug" | "plus" | "plus-circle" | "plus-square" | "podcast" | "poll" | "poll-h" | "poo" | "poo-storm" | "poop" | "portrait" | "pound-sign" | "power-off" | "pray" | "praying-hands" | "prescription" | "prescription-bottle" | "prescription-bottle-alt" | "print" | "procedures" | "project-diagram" | "puzzle-piece" | "qrcode" | "question" | "question-circle" | "quidditch" | "quote-left" | "quote-right" | "quran" | "radiation" | "radiation-alt" | "rainbow" | "random" | "receipt" | "record-vinyl" | "recycle" | "redo" | "refresh" | "redo-alt" | "registered" | "remove-format" | "reply" | "reply-all" | "republican" | "restroom" | "retweet" | "ribbon" | "ring" | "road" | "robot" | "rocket" | "route" | "rss" | "rss-square" | "ruble-sign" | "ruler" | "ruler-combined" | "ruler-horizontal" | "ruler-vertical" | "running" | "rupee-sign" | "sad-cry" | "sad-tear" | "satellite" | "satellite-dish" | "save" | "school" | "screwdriver" | "scroll" | "sd-card" | "search" | "search-dollar" | "search-location" | "search-minus" | "search-plus" | "seedling" | "server" | "shapes" | "share" | "share-alt" | "share-alt-square" | "share-square" | "shekel-sign" | "shield-alt" | "ship" | "shipping-fast" | "shoe-prints" | "shopping-bag" | "shopping-basket" | "shopping-cart" | "shower" | "shuttle-van" | "sign" | "sign-in-alt" | "sign-language" | "sign-out" | "sign-out-alt" | "signal" | "signature" | "sim-card" | "sitemap" | "skating" | "skiing" | "skiing-nordic" | "skull" | "skull-crossbones" | "slash" | "sleigh" | "sliders-h" | "smile" | "smile-beam" | "smile-wink" | "smog" | "smoking" | "smoking-ban" | "sms" | "snowboarding" | "snowflake" | "snowman" | "snowplow" | "socks" | "solar-panel" | "sort" | "sort-alpha-down" | "sort-alpha-down-alt" | "sort-alpha-up" | "sort-alpha-up-alt" | "sort-amount-down" | "sort-amount-down-alt" | "sort-amount-up" | "sort-amount-up-alt" | "sort-down" | "sort-numeric-down" | "sort-numeric-down-alt" | "sort-numeric-up" | "sort-numeric-up-alt" | "sort-up" | "spa" | "space-shuttle" | "spell-check" | "spider" | "spinner" | "splotch" | "spray-can" | "square" | "square-full" | "square-root-alt" | "stamp" | "star" | "star-and-crescent" | "star-half" | "star-half-alt" | "star-o" | "star-of-david" | "star-of-life" | "step-backward" | "step-forward" | "stethoscope" | "sticky-note" | "stop" | "stop-circle" | "stopwatch" | "store" | "store-alt" | "stream" | "street-view" | "strikethrough" | "stroopwafel" | "subscript" | "subway" | "suitcase" | "suitcase-rolling" | "sun" | "superscript" | "surprise" | "swatchbook" | "swimmer" | "swimming-pool" | "synagogue" | "sync" | "sync-alt" | "syringe" | "table" | "table-tennis" | "tablet" | "tablet-alt" | "tablets" | "tachometer-alt" | "tag" | "tags" | "tape" | "tasks" | "taxi" | "teeth" | "teeth-open" | "temperature-high" | "temperature-low" | "tenge" | "terminal" | "text-height" | "text-width" | "th" | "th-large" | "th-list" | "theater-masks" | "thermometer" | "thermometer-empty" | "thermometer-full" | "thermometer-half" | "thermometer-quarter" | "thermometer-three-quarters" | "thumbs-down" | "thumbs-up" | "thumbtack" | "ticket-alt" | "times" | "times-circle" | "tint" | "tint-slash" | "tired" | "toggle-off" | "toggle-on" | "toilet" | "toilet-paper" | "toolbox" | "tools" | "tooth" | "torah" | "torii-gate" | "tractor" | "trademark" | "traffic-light" | "train" | "tram" | "transgender" | "transgender-alt" | "trash" | "trash-alt" | "trash-o" | "trash-restore" | "trash-restore-alt" | "tree" | "trophy" | "truck" | "truck-loading" | "truck-monster" | "truck-moving" | "truck-pickup" | "tshirt" | "tty" | "tv" | "umbrella" | "umbrella-beach" | "underline" | "undo" | "undo-alt" | "universal-access" | "university" | "unlink" | "unlock" | "unlock-alt" | "upload" | "user" | "user-alt" | "user-alt-slash" | "user-astronaut" | "user-check" | "user-circle" | "user-clock" | "user-cog" | "user-edit" | "user-friends" | "user-graduate" | "user-injured" | "user-lock" | "user-md" | "user-minus" | "user-ninja" | "user-nurse" | "user-plus" | "user-secret" | "user-shield" | "user-slash" | "user-tag" | "user-tie" | "user-times" | "users" | "users-cog" | "utensil-spoon" | "utensils" | "vector-square" | "venus" | "venus-double" | "venus-mars" | "vial" | "vials" | "video" | "video-slash" | "vihara" | "voicemail" | "volleyball-ball" | "volume-down" | "volume-mute" | "volume-off" | "volume-up" | "vote-yea" | "vr-cardboard" | "walking" | "wallet" | "warehouse" | "water" | "wave-square" | "weight" | "weight-hanging" | "wheelchair" | "wifi" | "wind" | "window-close" | "window-maximize" | "window-minimize" | "window-restore" | "wine-bottle" | "wine-glass" | "wine-glass-alt" | "won-sign" | "wrench" | "x-ray" | "yen-sign" | "yin-yang";
/**
 * Union of Font Awesome Brands icon keys (without the `fa-` prefix).
 * Used by {@link fabIcon} and {@link KnownIconClass} for brand icons such as `"github"`, `"twitter"`, etc.
 */
export type fabIconKey = "500px" | "accessible-icon" | "accusoft" | "acquisitions-incorporated" | "adn" | "adobe" | "adversal" | "affiliatetheme" | "airbnb" | "algolia" | "alipay" | "amazon" | "amazon-pay" | "amilia" | "android" | "angellist" | "angrycreative" | "angular" | "app-store" | "app-store-ios" | "apper" | "apple" | "apple-pay" | "artstation" | "asymmetrik" | "atlassian" | "audible" | "autoprefixer" | "avianex" | "aviato" | "aws" | "bandcamp" | "battle-net" | "behance" | "behance-square" | "bimobject" | "bitbucket" | "bitcoin" | "bity" | "black-tie" | "blackberry" | "blogger" | "blogger-b" | "bluetooth" | "bluetooth-b" | "bootstrap" | "btc" | "buffer" | "buromobelexperte" | "buy-n-large" | "buysellads" | "canadian-maple-leaf" | "cc-amazon-pay" | "cc-amex" | "cc-apple-pay" | "cc-diners-club" | "cc-discover" | "cc-jcb" | "cc-mastercard" | "cc-paypal" | "cc-stripe" | "cc-visa" | "centercode" | "centos" | "chrome" | "chromecast" | "cloudscale" | "cloudsmith" | "cloudversify" | "codepen" | "codiepie" | "confluence" | "connectdevelop" | "contao" | "cotton-bureau" | "cpanel" | "creative-commons" | "creative-commons-by" | "creative-commons-nc" | "creative-commons-nc-eu" | "creative-commons-nc-jp" | "creative-commons-nd" | "creative-commons-pd" | "creative-commons-pd-alt" | "creative-commons-remix" | "creative-commons-sa" | "creative-commons-sampling" | "creative-commons-sampling-plus" | "creative-commons-share" | "creative-commons-zero" | "critical-role" | "css3" | "css3-alt" | "cuttlefish" | "d-and-d" | "d-and-d-beyond" | "dashcube" | "delicious" | "deploydog" | "deskpro" | "dev" | "deviantart" | "dhl" | "diaspora" | "digg" | "digital-ocean" | "discord" | "discourse" | "dochub" | "docker" | "draft2digital" | "dribbble" | "dribbble-square" | "dropbox" | "drupal" | "dyalog" | "earlybirds" | "ebay" | "edge" | "elementor" | "ello" | "ember" | "empire" | "envira" | "erlang" | "ethereum" | "etsy" | "evernote" | "expeditedssl" | "facebook" | "facebook-f" | "facebook-messenger" | "facebook-square" | "fantasy-flight-games" | "fedex" | "fedora" | "figma" | "firefox" | "first-order" | "first-order-alt" | "firstdraft" | "flickr" | "flipboard" | "fly" | "font-awesome" | "font-awesome-alt" | "font-awesome-flag" | "fonticons" | "fonticons-fi" | "fort-awesome" | "fort-awesome-alt" | "forumbee" | "foursquare" | "free-code-camp" | "freebsd" | "fulcrum" | "galactic-republic" | "galactic-senate" | "get-pocket" | "gg" | "gg-circle" | "git" | "git-alt" | "git-square" | "github" | "github-alt" | "github-square" | "gitkraken" | "gitlab" | "gitter" | "glide" | "glide-g" | "gofore" | "goodreads" | "goodreads-g" | "google" | "google-drive" | "google-play" | "google-plus" | "google-plus-g" | "google-plus-square" | "google-wallet" | "gratipay" | "grav" | "gripfire" | "grunt" | "gulp" | "hacker-news" | "hacker-news-square" | "hackerrank" | "hips" | "hire-a-helper" | "hooli" | "hornbill" | "hotjar" | "houzz" | "html5" | "hubspot" | "imdb" | "instagram" | "intercom" | "internet-explorer" | "invision" | "ioxhost" | "itch-io" | "itunes" | "itunes-note" | "java" | "jedi-order" | "jenkins" | "jira" | "joget" | "joomla" | "js" | "js-square" | "jsfiddle" | "kaggle" | "keybase" | "keycdn" | "kickstarter" | "kickstarter-k" | "korvue" | "laravel" | "lastfm" | "lastfm-square" | "leanpub" | "less" | "line" | "linkedin" | "linkedin-in" | "linode" | "linux" | "lyft" | "magento" | "mailchimp" | "mandalorian" | "markdown" | "mastodon" | "maxcdn" | "mdb" | "medapps" | "medium" | "medium-m" | "medrt" | "meetup" | "megaport" | "mendeley" | "microsoft" | "mix" | "mixcloud" | "mizuni" | "modx" | "monero" | "napster" | "neos" | "nimblr" | "node" | "node-js" | "npm" | "ns8" | "nutritionix" | "odnoklassniki" | "odnoklassniki-square" | "old-republic" | "opencart" | "openid" | "opera" | "optin-monster" | "orcid" | "osi" | "page4" | "pagelines" | "palfed" | "patreon" | "paypal" | "penny-arcade" | "periscope" | "phabricator" | "phoenix-framework" | "phoenix-squadron" | "php" | "pied-piper" | "pied-piper-alt" | "pied-piper-hat" | "pied-piper-pp" | "pinterest" | "pinterest-p" | "pinterest-square" | "playstation" | "product-hunt" | "pushed" | "python" | "qq" | "quinscape" | "quora" | "r-project" | "raspberry-pi" | "ravelry" | "react" | "reacteurope" | "readme" | "rebel" | "red-river" | "reddit" | "reddit-alien" | "reddit-square" | "redhat" | "renren" | "replyd" | "researchgate" | "resolving" | "rev" | "rocketchat" | "rockrms" | "safari" | "salesforce" | "sass" | "schlix" | "scribd" | "searchengin" | "sellcast" | "sellsy" | "servicestack" | "shirtsinbulk" | "shopware" | "simplybuilt" | "sistrix" | "sith" | "sketch" | "skyatlas" | "skype" | "slack" | "slack-hash" | "slideshare" | "snapchat" | "snapchat-ghost" | "snapchat-square" | "soundcloud" | "sourcetree" | "speakap" | "speaker-deck" | "spotify" | "squarespace" | "stack-exchange" | "stack-overflow" | "stackpath" | "staylinked" | "steam" | "steam-square" | "steam-symbol" | "sticker-mule" | "strava" | "stripe" | "stripe-s" | "studiovinari" | "stumbleupon" | "stumbleupon-circle" | "superpowers" | "supple" | "suse" | "swift" | "symfony" | "teamspeak" | "telegram" | "telegram-plane" | "tencent-weibo" | "the-red-yeti" | "themeco" | "themeisle" | "think-peaks" | "trade-federation" | "trello" | "tripadvisor" | "tumblr" | "tumblr-square" | "twitch" | "twitter" | "twitter-square" | "typo3" | "uber" | "ubuntu" | "uikit" | "umbraco" | "uniregistry" | "untappd" | "ups" | "usb" | "usps" | "ussunnah" | "vaadin" | "viacoin" | "viadeo" | "viadeo-square" | "viber" | "vimeo" | "vimeo-square" | "vimeo-v" | "vine" | "vk" | "vnv" | "vuejs" | "waze" | "weebly" | "weibo" | "weixin" | "whatsapp" | "whatsapp-square" | "whmcs" | "wikipedia-w" | "windows" | "wix" | "wizards-of-the-coast" | "wolf-pack-battalion" | "wordpress" | "wordpress-simple" | "wpbeginner" | "wpexplorer" | "wpforms" | "wpressr" | "xbox" | "xing" | "xing-square" | "y-combinator" | "yahoo" | "yammer" | "yandex" | "yandex-international" | "yarn" | "yelp" | "yoast" | "youtube" | "youtube-square" | "zhihu";
/**
 * Options that describe a single button rendered in a {@link Dialog} footer.
 * @remarks
 * Buttons are rendered as Bootstrap `btn` or jQuery UI button elements depending
 * on the active dialog provider. When {@link DialogButton.result} is set and the
 * click handler does not cancel the event, the dialog automatically closes with
 * that result code.
 */
export interface DialogButton {
	/** Visible caption rendered inside the button. Defaults to a localized value when created via helper factories. */
	text?: string;
	/** Tooltip / `title` attribute shown on hover. */
	hint?: string;
	/** Optional icon displayed before the text; resolved via {@link iconClassName}. */
	icon?: IconClassName;
	/**
	 * Click handler invoked when the button is activated.
	 * @param e - The originating mouse event.
	 * @returns `false` to prevent the automatic close, or a `Promise` that resolves to `false` to cancel asynchronously.
	 */
	click?: (e: MouseEvent) => void | false | Promise<void | false>;
	/** Additional CSS class(es) added to the button element (e.g. `"btn-primary"`, `"btn-danger"`). */
	cssClass?: string;
	/**
	 * Result code assigned to the dialog when this button is clicked.
	 * The value is stored in `dataset.dialogResult` and passed to `onClose` handlers.
	 * If set and the click handler does not call `preventDefault()` / return `false`, the dialog closes automatically.
	 */
	result?: string;
}
/**
 * Identifies the underlying UI provider that backs a {@link Dialog} instance.
 * - `"bsmodal"` — Bootstrap modal (`.modal`).
 * - `"uidialog"` — jQuery UI dialog (`.ui-dialog`).
 * - `"panel"` — Inline Serenity panel (`.s-Panel`).
 */
export type DialogProviderType = "bsmodal" | "uidialog" | "panel";
/**
 * Options that configure a {@link Dialog} instance across all providers.
 * @remarks
 * The dialog provider is chosen automatically from {@link DialogOptions.preferPanel},
 * {@link DialogOptions.preferBSModal}, and feature detection (`hasBSModal()` / `hasUIDialog()`).
 * Provider-specific options can be injected via {@link DialogOptions.providerOptions}.
 */
export interface DialogOptions {
	/** When `true`, {@link Dialog.dispose} is called automatically on close. @defaultValue `true` */
	autoDispose?: boolean;
	/** When `true`, the dialog opens immediately after construction. @defaultValue `true` */
	autoOpen?: boolean;
	/** Backdrop behavior for Bootstrap modals; `"static"` prevents closing on outside click. @defaultValue `false` */
	backdrop?: boolean | "static";
	/** Buttons rendered in the dialog footer. */
	buttons?: DialogButton[];
	/** Vertically centers a Bootstrap modal via `modal-dialog-centered`. @defaultValue `true` */
	centered?: boolean;
	/** Whether to render the header close (`×` / `btn-close`) button. @defaultValue `true` */
	closeButton?: boolean;
	/** Whether pressing <kbd>Escape</kbd> closes the dialog. Message dialogs default to `true`. */
	closeOnEscape?: boolean;
	/** Extra CSS class(es) added to the root dialog element (`.modal`, `.ui-dialog`, or `.s-Panel`). */
	dialogClass?: string;
	/** Body element or a callback that populates the freshly created body element. Array-like values are treated as the content node. */
	element?: HTMLElement | ArrayLike<HTMLElement> | ((element: HTMLElement) => void);
	/** Enables fade animation for Bootstrap modals. @defaultValue `false` for message dialogs, `true` otherwise */
	fade?: boolean;
	/** Applies a `modal-fullscreen[-{breakpoint}-down]` class. Only effective for Bootstrap modals. */
	fullScreen?: boolean | "sm-down" | "md-down" | "lg-down" | "xl-down" | "xxl-down";
	/** jQuery UI `modal` flag. Retained for backward compatibility; does not affect Bootstrap modals. */
	modal?: boolean;
	/** Callback invoked after the dialog is opened. */
	onOpen?: (e?: Event) => void;
	/** Callback invoked after the dialog is closed, receiving the result code. */
	onClose?: (result: string, e?: Event) => void;
	/** When both providers are available, prefer Bootstrap modal over jQuery UI dialog. @defaultValue `true` */
	preferBSModal?: boolean;
	/** Force inline panel mode even when modal / jQuery UI providers are available. */
	preferPanel?: boolean;
	/** Returns provider-specific options merged into the underlying call (Bootstrap modal options or jQuery UI dialog options). @param type - Resolved provider type. @param opt - The resolved dialog options. @returns Provider-specific options object. */
	providerOptions?: (type: DialogProviderType, opt: DialogOptions) => any;
	/** Makes the Bootstrap modal body scrollable via `modal-dialog-scrollable`. */
	scrollable?: boolean;
	/** Bootstrap modal size. @defaultValue `"lg"` for regular dialogs, `"md"` for message dialogs */
	size?: "sm" | "md" | "lg" | "xl";
	/** Title text shown in the dialog header. */
	title?: string;
	/** Initial width in pixels; only used by the jQuery UI dialog provider. */
	width?: number;
}
/**
 * Unified wrapper over jQuery UI dialogs, Bootstrap modals, and Serenity inline panels.
 * @remarks
 * Provider selection is automatic: `preferPanel` wins, otherwise jQuery UI vs. Bootstrap
 * is chosen via {@link hasUIDialog}, {@link hasBSModal}, and `preferBSModal`.
 * Lifecycle events (`panel*`, `dialog*`, `show.bs.modal` / `hide.bs.modal`) are normalized
 * so that {@link Dialog.onOpen} / {@link Dialog.onClose} work uniformly across providers.
 * @example
 * ```ts
 * const dlg = new Dialog({
 *   title: "Hello",
 *   element: el => el.append("Content"),
 *   buttons: [okDialogButton(), cancelDialogButton()]
 * });
 * dlg.onClose(result => console.log(result));
 * ```
 */
export declare class Dialog {
	private el;
	private dialogResult;
	/**
	 * Creates a new dialog.
	 * @param opt - Configuration for the dialog; merged over {@link Dialog.defaults}.
	 * @param create - When `false`, skips DOM creation and only binds to an existing element. Used internally by {@link Dialog.getInstance}.
	 * @remarks
	 * The concrete provider (panel / jQuery UI / Bootstrap) is resolved from availability
	 * and `preferPanel` / `preferBSModal` flags.
	 */
	constructor(opt?: DialogOptions);
	/** Default options applied to every {@link Dialog} before caller-supplied `opt` is merged. */
	static defaults: DialogOptions;
	/** Default options applied to helper message dialogs (`alertDialog`, `confirmDialog`, etc.). */
	static messageDefaults: MessageDialogOptions;
	/**
	 * Gets the dialog instance for the specified element.
	 * @param el The dialog body element (.s-Panel, .ui-dialog-content, or .modal-body) or the root element (.modal, .ui-dialog, .s-Panel)
	 * @returns The dialog instance, or null if the element is not a dialog.
	 */
	static getInstance(el: HTMLElement | ArrayLike<HTMLElement>): Dialog;
	/**
	 * Result code of the last button that closed the dialog.
	 * @remarks Mirrors `element.dataset.dialogResult`; survives disposal via the fallback field.
	 * @returns The result string (e.g. `"ok"`, `"yes"`, `"cancel"`) or `null`/`undefined` when not yet closed.
	 */
	get result(): string;
	/** Closes the dialog and reports a `null` result. @returns The dialog instance for chaining. */
	close(): this;
	/**
	 * Closes the dialog with an explicit result code.
	 * @param result - Value stored in `dataset.dialogResult` and passed to `onClose` handlers.
	 * @returns The dialog instance for chaining.
	 */
	close(result: string): this;
	/**
	 * Subscribes to the dialog close event.
	 * @param handler - Callback invoked with the dialog result and the close event. Call `preventDefault()` on the event to cancel closing when `opt.before` is `true`.
	 * @param opt - Subscription options.
	 * @param opt.before - When `true`, listens to the cancellable *before-close* event (`panelbeforeclose` / `dialogbeforeclose` / `hide.bs.modal`).
	 * @param opt.oneOff - When `true`, the handler is removed after the first invocation. Defaults to `true` unless `before` is `true`.
	 * @returns The dialog instance for chaining.
	 */
	onClose(handler: (result?: string, e?: Event) => void, opt?: {
		before?: boolean;
		oneOff?: boolean;
	}): this;
	/**
	 * Static helper that subscribes to the close event for a dialog element that may not yet be instantiated.
	 * @param el - Dialog body element (`.s-Panel`, `.ui-dialog-content`, or `.modal-body`) or an array-like wrapper.
	 * @param handler - Callback invoked with the dialog result and the close event; `preventDefault()` cancels the close when `opt.before` is `true`.
	 * @param opt - Subscription options.
	 * @param opt.before - Listen to the cancellable *before-close* event.
	 * @param opt.oneOff - Auto-remove after first invocation. Defaults to `true` unless `before` is `true`.
	 */
	static onClose(el: HTMLElement | ArrayLike<HTMLElement>, handler: (result?: string, e?: Event) => void, opt?: {
		before?: boolean;
		oneOff?: boolean;
	}): void;
	/**
	 * Subscribes to the dialog open event.
	 * @param handler - Callback invoked when the dialog is opened; `preventDefault()` cancels the open when `opt.before` is `true`.
	 * @param opt - Subscription options.
	 * @param opt.before - When `true`, listens to the cancellable *before-open* event (`panelbeforeopen` / `dialogbeforeopen` / `show.bs.modal`).
	 * @param opt.oneOff - Auto-remove after first invocation. Defaults to `true` unless `before` is `true`.
	 * @returns The dialog instance for chaining.
	 */
	onOpen(handler: (e?: Event) => void, opt?: {
		before?: boolean;
		oneOff?: boolean;
	}): this;
	/**
	 * Static helper that subscribes to the open event for a dialog element that may not yet be instantiated.
	 * @param el - Dialog body element (`.s-Panel`, `.ui-dialog-content`, or `.modal-body`) or an array-like wrapper.
	 * @param handler - Callback invoked when the dialog is opened; `preventDefault()` cancels the open when `opt.before` is `true`.
	 * @param opt - Subscription options.
	 * @param opt.before - Listen to the cancellable *before-open* event.
	 * @param opt.oneOff - Auto-remove after first invocation. Defaults to `true` unless `before` is `true`.
	 */
	static onOpen(el: HTMLElement | ArrayLike<HTMLElement>, handler: (e?: Event) => void, opt?: {
		before?: boolean;
		oneOff?: boolean;
	}): void;
	/**
	 * Opens the dialog.
	 * @remarks Dispatches the provider-specific show command (`openPanel`, `jQuery.dialog("open")`, or `bootstrap.Modal.show`).
	 * @returns The dialog instance for chaining.
	 */
	open(): this;
	/**
	 * Gets the current title text of the dialog.
	 * @returns The header title text, or `undefined` when the dialog has no header.
	 */
	title(): string;
	/**
	 * Sets the title text of the dialog.
	 * @param value - New title text to display in the header.
	 * @returns The dialog instance for chaining.
	 */
	title(value: string): this;
	/**
	 * Identifies the backing provider for this instance.
	 * @returns `"bsmodal"`, `"uidialog"`, or `"panel"`, or `null` when the element is not attached or the dialog was disposed.
	 */
	get type(): DialogProviderType;
	/**
	 * Gets the body / content node of the dialog (`.modal-body`, `.panel-body`, or `.ui-dialog-content`).
	 * @returns The content element, or the raw `el` supplied at construction.
	 */
	getContentNode(): HTMLElement;
	/**
	 * Gets the root dialog element (`.modal`, `.ui-dialog`, or `.s-Panel`).
	 * @returns The root element, or `null` when not found.
	 */
	getDialogNode(): HTMLElement;
	/**
	 * Gets the node that receives open/close lifecycle events (`.modal`, `.panel-body`, or `.ui-dialog-content`).
	 * @returns The events node, or a fallback lookup via the root element.
	 */
	getEventsNode(): HTMLElement;
	/**
	 * Gets the footer element (`.modal-footer`, `.panel-footer`, or `.ui-dialog-footer`), if present.
	 * @returns The footer element or `null` when there is none.
	 */
	getFooterNode(): HTMLElement;
	/**
	 * Gets the header element (`.modal-header`, `.panel-titlebar`, or `.ui-dialog-titlebar`), if present.
	 * @returns The header element or `null` when there is none.
	 */
	getHeaderNode(): HTMLElement;
	private onButtonClick;
	private createBSButtons;
	private createBSModal;
	private createPanel;
	private createUIDialog;
	/**
	 * Disposes the dialog, removing it from the DOM and unbinding event handlers.
	 * @remarks
	 * Handles all three providers: destroys a jQuery UI dialog, disposes a Bootstrap modal instance, or removes the panel markup. Falls back to plain DOM removal when no library is present. Safe to call multiple times.
	 */
	dispose(): void;
}
/**
 * Determines whether a Bootstrap modal provider is available.
 * @returns `true` when Bootstrap 5+ `bootstrap.Modal` or jQuery `fn.modal` (Bootstrap 3/4) is loaded.
 */
export declare function hasBSModal(): boolean;
/**
 * Determines whether the jQuery UI dialog provider is available.
 * @returns `true` when `jQuery.ui.dialog` is loaded.
 */
export declare function hasUIDialog(): boolean;
/**
 * Resolves the jQuery UI / Bootstrap button name collision.
 * @remarks
 * When both `$.fn.button` (Bootstrap) and `$.ui.button` (jQuery UI) are present, this moves Bootstrap's implementation to `$.fn.btn` via `noConflict()` so jQuery UI dialogs keep their button widget. Invoked automatically on module load.
 */
export declare function uiAndBSButtonNoConflict(): void;
/**
 * Creates an "OK" dialog button.
 * @param opt - Optional overrides for {@link DialogButton} properties. Only `text`, `cssClass`, `result`, and `click` are respected; unspecified fields fall back to localized defaults.
 * @returns A {@link DialogButton} with `text` defaulting to `DialogTexts.OkButton`, `cssClass` to `"btn-info"`, and `result` to `"ok"`.
 * @example
 * ```ts
 * new Dialog({ buttons: [okDialogButton({ click: () => save() })] });
 * ```
 */
export declare function okDialogButton(opt?: DialogButton): DialogButton;
/**
 * Creates a dialog button which, by default, has "Yes" as the caption (localized) and "yes" as the result.
 * @param opt - Optional configuration for the dialog button.
 * @returns The dialog button with the specified configuration.
 */
export declare function yesDialogButton(opt?: DialogButton): DialogButton;
/**
 * Creates a dialog button which, by default, has "No" as the caption (localized) and "no" as the result.
 * @param opt - Optional configuration for the dialog button.
 * @returns The dialog button with the specified configuration.
 */
export declare function noDialogButton(opt?: DialogButton): DialogButton;
/**
 * Creates a dialog button which, by default, has "Cancel" as the caption (localized) and "cancel" as the result.
 * @param opt - Optional configuration for the dialog button.
 * @returns The dialog button with the specified configuration.
 */
export declare function cancelDialogButton(opt?: DialogButton): DialogButton;
/**
 * Localizable text constants for dialogs.
 * @remarks
 * Each property is a getter that calls `localText("Dialogs." + key, defaultValue)` and HTML-encodes the result. Defaults are in English; override via `Texts.Dialogs.*` localizations.
 */
export declare namespace DialogTexts {
	/**
	 * Title for alert dialogs.
	 */
	const AlertTitle: string;
	/**
	 * Text for the cancel button in dialogs.
	 */
	const CancelButton: string;
	/**
	 * Text for the close button in dialogs.
	 */
	const CloseButton: string;
	/**
	 * Title for confirmation dialogs.
	 */
	const ConfirmationTitle: string;
	/**
	 * Title for information dialogs.
	 */
	const InformationTitle: string;
	/**
	 * Hint for maximizing dialogs.
	 */
	const MaximizeHint: string;
	/**
	 * Text for the "No" button in dialogs.
	 */
	const NoButton: string;
	/**
	 * Text for the "OK" button in dialogs.
	 */
	const OkButton: string;
	/**
	 * Title for the prompt dialog.
	 */
	const PromptTitle: string;
	/**
	 * Hint for restoring dialogs.
	 */
	const RestoreHint: string;
	/**
	 * Title for success dialogs.
	 */
	const SuccessTitle: string;
	/**
	 * Title for warning dialogs.
	 */
	const WarningTitle: string;
	/**
	 * Text for the "Yes" button in dialogs.
	 */
	const YesButton: string;
}
/**
 * Options for helper message dialogs (`alertDialog`, `confirmDialog`, etc.).
 * @remarks Extends {@link DialogOptions} with message-specific rendering flags.
 */
export interface MessageDialogOptions extends DialogOptions {
	/**
	 * Whether to HTML-encode string messages. @defaultValue `true`
	 * @deprecated Prefer passing a `RenderableContent` node or pre-sanitized HTML. When `false`, the string is sanitized via `sanitizeHtml`.
	 */
	htmlEncode?: boolean;
	/**
	 * Whether to preserve line breaks via `white-space: pre-wrap` on the message container. @defaultValue `true`
	 * @remarks Only applied when the message is a string; element messages manage their own styling.
	 */
	preWrap?: boolean;
}
/**
 * Displays a modal alert dialog with a single OK button.
 * @param message - Text or renderable content shown in the dialog body.
 * @param options - Additional {@link MessageDialogOptions}.
 * @returns A {@link Dialog} handle (partial when falling back to the native `alert()`), whose `result` is `"ok"`.
 * @remarks Falls back to the native `alert()` when neither Bootstrap modal nor jQuery UI dialog is available.
 * @example
 * ```ts
 * alertDialog("An error occurred!");
 * ```
 */
export declare function alertDialog(message: RenderableContent, options?: MessageDialogOptions): Partial<Dialog>;
/**
 * Additional options for {@link confirmDialog}.
 * @remarks Extends {@link MessageDialogOptions} with callbacks for the secondary buttons.
 */
export interface ConfirmDialogOptions extends MessageDialogOptions {
	/** When `true`, an extra Cancel button (`result` `"cancel"`) is rendered alongside Yes/No. */
	cancelButton?: boolean;
	/** Callback invoked when the Cancel button is clicked (only when `cancelButton` is `true`). */
	onCancel?: () => void;
	/** Callback invoked when the No button is clicked. */
	onNo?: () => void;
}
/**
 * Displays a confirmation dialog with Yes / No (and optional Cancel) buttons.
 * @param message - Text or renderable content shown in the dialog body.
 * @param onYes - Callback invoked when the Yes button is clicked.
 * @param options - Additional {@link ConfirmDialogOptions}.
 * @returns A {@link Dialog} handle (partial when falling back to the native `confirm()`), whose `result` is `"yes"`, `"no"`, or `"cancel"`.
 * @remarks Falls back to the native `confirm()` when neither Bootstrap modal nor jQuery UI dialog is available.
 * @example
 * ```ts
 * confirmDialog("Are you sure you want to delete?", () => {
 *   // do something when yes is clicked
 * });
 * ```
 */
export declare function confirmDialog(message: RenderableContent, onYes: () => void, options?: ConfirmDialogOptions): Partial<Dialog>;
/**
 * Displays an informational dialog with a single OK button.
 * @param message - Text or renderable content shown in the dialog body.
 * @param onOk - Optional callback invoked when OK is clicked.
 * @param options - Additional {@link MessageDialogOptions}.
 * @returns A {@link Dialog} handle (partial when falling back to the native `alert()`).
 * @example
 * ```ts
 * informationDialog("Operation complete", () => {
 *   // do something when OK is clicked
 * });
 * ```
 */
export declare function informationDialog(message: RenderableContent, onOk?: () => void, options?: MessageDialogOptions): Partial<Dialog>;
/**
 * Displays a success dialog with a single OK button.
 * @param message - Text or renderable content shown in the dialog body.
 * @param onOk - Optional callback invoked when OK is clicked.
 * @param options - Additional {@link MessageDialogOptions}.
 * @returns A {@link Dialog} handle (partial when falling back to the native `alert()`).
 * @example
 * ```ts
 * successDialog("Operation complete", () => {
 *   // do something when OK is clicked
 * });
 * ```
 */
export declare function successDialog(message: RenderableContent, onOk?: () => void, options?: MessageDialogOptions): Partial<Dialog>;
/**
 * Displays a warning dialog with a single OK button.
 * @param message - Text or renderable content shown in the dialog body.
 * @param options - Additional {@link MessageDialogOptions}.
 * @returns A {@link Dialog} handle (partial when falling back to the native `alert()`).
 * @example
 * ```ts
 * warningDialog("Something is odd!");
 * ```
 */
export declare function warningDialog(message: RenderableContent, options?: MessageDialogOptions): Partial<Dialog>;
/**
 * Options for {@link iframeDialog}.
 */
export interface IFrameDialogOptions {
	/** HTML string rendered inside the sandboxed `iframe` via `srcdoc`. Sanitized before injection. */
	html?: string;
}
/**
 * Displays a dialog whose content is an `iframe` rendering arbitrary HTML.
 * @param options - Configuration containing the HTML to display.
 * @param options.html - Raw HTML placed in the `iframe` `srcdoc` attribute after sanitization; wrapped in `<html>/<body>` when those tags are absent.
 * @returns A {@link Dialog} handle (partial when falling back to `alert` without modal support).
 * @remarks Falls back to `window.alert` with sanitized HTML when neither Bootstrap modal nor jQuery UI dialog is available.
 */
export declare function iframeDialog(options: IFrameDialogOptions): Partial<Dialog>;
/**
 * Gets the globally available jQuery instance, if any.
 * @remarks
 * Checks both `jQuery` and `$` globals. Returns `undefined` when jQuery is
 * not loaded or does not expose `fn`, allowing the codebase to fall back to
 * native DOM / Bootstrap 5 APIs.
 * @returns The jQuery function when available, otherwise `undefined`.
 */
export declare function getjQuery(): any;
/**
 * Determines whether Bootstrap 3 is loaded on the page.
 * @remarks
 * Inspects `jQuery.fn.modal.Constructor.VERSION`; the check is safe when
 * jQuery or the modal plugin is absent.
 * @returns `true` if Bootstrap 3 is detected, otherwise `false`.
 */
export declare function isBS3(): boolean;
/**
 * Determines whether Bootstrap 5 or later is loaded on the page.
 * @remarks
 * Uses the global `bootstrap.Modal.VERSION` when available. Explicitly
 * excludes Bootstrap 4 (major version `"4"`) so that Bootstrap 4 is treated
 * as neither BS3 nor BS5+.
 * @returns `true` if Bootstrap 5+ is detected, otherwise `false`.
 */
export declare function isBS5Plus(): boolean;
/**
 * Error payload returned by Serenity service endpoints inside a {@link ServiceResponse}.
 * The server populates at least `Code` or `Message`; other fields are optional and
 * depend on the handler / validation layer.
 */
export interface ServiceError {
	/** Machine-readable error code (e.g. `"NotLoggedIn"`, `"ValidationError"`, `"AccessDenied"`). */
	Code?: string;
	/** Optional comma-separated or serialized arguments that parameterize the error message (e.g. field names). */
	Arguments?: string;
	/** Human-readable, possibly localized, error message suitable for display. */
	Message?: string;
	/** Detailed / technical information (e.g. stack trace or inner exception) — only when diagnostics are enabled. */
	Details?: string;
	/** Correlation / error ID assigned server-side for log lookup. */
	ErrorId?: string;
}
/**
 * Base contract for every Serenity service response.
 * Successful responses omit `Error`; failed responses populate it and may omit other fields.
 */
export interface ServiceResponse {
	/** Error information when the request failed; `undefined` on success. */
	Error?: ServiceError;
}
/**
 * Marker base for all Serenity service request DTOs.
 * Concrete requests extend this to add handler-specific fields.
 */
export interface ServiceRequest {
}
/**
 * Request DTO for `Create` / `Update` service handlers.
 * @typeParam TEntity - Row / entity type being saved.
 */
export interface SaveRequest<TEntity> extends ServiceRequest {
	/** Primary key of the entity to update; omit for inserts (server generates the key). */
	EntityId?: any;
	/** Entity fields to persist. For updates only modified fields need to be sent, depending on handler. */
	Entity?: TEntity;
	/** Per-language patches for localizable rows, keyed by language ID (e.g. `{ "en": { Name: "Hello" } }`). */
	Localizations?: {
		[languageId: string]: Partial<TEntity>;
	};
}
/**
 * Response DTO for `Create` / `Update` handlers.
 */
export interface SaveResponse extends ServiceResponse {
	/** Primary key of the created / updated entity as assigned / confirmed by the server. */
	EntityId?: any;
}
/**
 * Request DTO for `Delete` handlers (soft or hard delete depending on row / handler).
 */
export interface DeleteRequest extends ServiceRequest {
	/** Primary key of the entity to delete. */
	EntityId?: any;
}
/**
 * Response DTO for `Delete` handlers. No additional fields beyond {@link ServiceResponse}.
 */
export interface DeleteResponse extends ServiceResponse {
}
/**
 * Request DTO for `Undelete` handlers (restores a soft-deleted row).
 */
export interface UndeleteRequest extends ServiceRequest {
	/** Primary key of the entity to undelete. */
	EntityId?: any;
}
/**
 * Response DTO for `Undelete` handlers. No additional fields beyond {@link ServiceResponse}.
 */
export interface UndeleteResponse extends ServiceResponse {
}
/**
 * Controls which columns are selected when listing entities.
 * Mirrors the server-side `ColumnSelection` enum and is used by `ListRequest.ColumnSelection`.
 */
export declare enum ColumnSelection {
	/** Default view columns (what the grid shows). */
	List = 0,
	/** Only key (ID) columns. */
	KeyOnly = 1,
	/** Detail view columns (more fields than `List`). */
	Details = 2,
	/** No columns (only aggregates / counts). */
	None = 3,
	/** Only the identity column. */
	IdOnly = 4,
	/** Columns needed for lookup display (usually ID + display field). */
	Lookup = 5
}
/**
 * Column-selection variant used by `Retrieve` handlers.
 * Lowercase naming is kept for backward compatibility with legacy endpoints.
 */
export declare enum RetrieveColumnSelection {
	/** Detail columns. */
	details = 0,
	/** Only key columns. */
	keyOnly = 1,
	/** List columns. */
	list = 2,
	/** No columns. */
	none = 3,
	/** Only the identity column. */
	idOnly = 4,
	/** Lookup columns. */
	lookup = 5
}
/**
 * Request DTO for `List` handlers (grid data source).
 * Supports paging, sorting, filtering and column selection.
 */
export interface ListRequest extends ServiceRequest {
	/** Number of records to skip (offset) for paging. */
	Skip?: number;
	/** Maximum number of records to take (page size). Omit or 0 for server default. */
	Take?: number;
	/** Sort expressions, e.g. `["Name ASC", "Age DESC"]`. */
	Sort?: string[];
	/** Quick-search text applied across searchable fields (or {@link ContainsField} when specified). */
	ContainsText?: string;
	/** When set, `ContainsText` is applied only to this field instead of all searchable fields. */
	ContainsField?: string;
	/** Advanced filter criteria tree (Serenity `Criteria` format: `[field, op, value]` or nested `["and", [...]]`). */
	Criteria?: any[];
	/** Simple equality filter map, e.g. `{ Status: 1 }`. Combined with `Criteria` via AND. */
	EqualityFilter?: any;
	/** When true, soft-deleted rows are included in results. */
	IncludeDeleted?: boolean;
	/** When true the server may skip computing `TotalCount` for performance. */
	ExcludeTotalCount?: boolean;
	/** Preset that controls which columns are returned. See {@link ColumnSelection}. */
	ColumnSelection?: ColumnSelection;
	/** Explicit allow-list of columns to include (overrides `ColumnSelection`). */
	IncludeColumns?: string[];
	/** Explicit deny-list of columns to exclude. */
	ExcludeColumns?: string[];
	/** Columns to export when `List` is used for export; defaults to visible columns. */
	ExportColumns?: string[];
	/** Fields to apply `DISTINCT` on (server-dependent). */
	DistinctFields?: string[];
	/** Language ID for localized field selection (e.g. `"en"`, `"tr"`). */
	Localize?: string;
}
/**
 * Response DTO for `List` handlers.
 * @typeParam TEntity - Row / entity type of the listed records.
 */
export interface ListResponse<TEntity> extends ServiceResponse {
	/** Page of entities matching the request. */
	Entities?: TEntity[];
	/** Alternative `Values` array used by some handlers that return raw values instead of entities. */
	Values?: any[];
	/** Total number of records matching the filter (before paging), unless `ExcludeTotalCount` was set. */
	TotalCount?: number;
	/** Echo of `Skip` from the request. */
	Skip?: number;
	/** Echo of `Take` from the request. */
	Take?: number;
}
/**
 * Request DTO for `Retrieve` handlers (single-entity fetch).
 */
export interface RetrieveRequest extends ServiceRequest {
	/** Primary key of the entity to retrieve. */
	EntityId?: any;
	/** Preset that controls which columns are returned. See {@link RetrieveColumnSelection}. */
	ColumnSelection?: RetrieveColumnSelection;
	/** Explicit allow-list of columns to include. */
	IncludeColumns?: string[];
	/** Explicit deny-list of columns to exclude. */
	ExcludeColumns?: string[];
}
/**
 * Response DTO for `Retrieve` handlers.
 * @typeParam TEntity - Row / entity type.
 */
export interface RetrieveResponse<TEntity> extends ServiceResponse {
	/** The retrieved entity, or `undefined` if not found (depending on handler). */
	Entity?: TEntity;
	/** Per-language values for localizable fields, keyed by language ID. */
	Localizations?: {
		[languageId: string]: Partial<TEntity>;
	};
}
/**
 * HTTP-level error details supplied to {@link ServiceOptions.onError} alongside the service error payload.
 */
export interface RequestErrorInfo {
	/** HTTP status code (e.g. `403`, `500`). */
	status?: number;
	/** HTTP status text (e.g. `"Forbidden"`). */
	statusText?: string;
	/** Raw response body text when the response could not be parsed as JSON. */
	responseText?: string;
}
/**
 * Options for {@link serviceCall} / {@link serviceRequest} / `serviceFetch`.
 * Extends the native `RequestInit` so any `fetch` option (e.g. `signal`, `cache`) can be passed through.
 * @typeParam TResponse - Expected service response type (must extend {@link ServiceResponse}).
 */
export interface ServiceOptions<TResponse extends ServiceResponse> extends RequestInit {
	/** When true (default), a `403` with a `Location` header triggers a top-window redirect. @defaultValue `true` */
	allowRedirect?: boolean;
	/** When `false` a synchronous XHR is used (blocks the UI). Prefer `true` (default). @defaultValue `true` */
	async?: boolean;
	/** When true (default) the UI is blocked with a loading indicator for the duration of the request. @defaultValue `true` */
	blockUI?: boolean;
	/** Extra HTTP headers merged with defaults (`Accept: application/json`, `Content-Type: application/json`, `X-CSRF-TOKEN` when same-origin). */
	headers?: Record<string, string>;
	/** Request DTO serialized as JSON in the POST body. */
	request?: any;
	/** Service endpoint key (e.g. `"Administration/User/List"`). Resolved via `~/Services/` when relative. Mutually exclusive with `url`. */
	service?: string;
	/** Absolute or `~/`-prefixed URL. When provided `service` is ignored. */
	url?: string;
	/** How service errors are surfaced to the user. `"alert"` shows a dialog, `"notification"` shows a toast, `"none"` suppresses default handling. */
	errorMode?: "alert" | "notification" | "none";
	/** Callback invoked after the request finishes regardless of success or failure (after `blockUI` is undone). */
	onCleanup?(): void;
	/**
	 * Custom error handler. Return `true` to indicate the error was handled and suppress default error display.
	 * @param response - Parsed service response (may be `null` for network / HTTP errors).
	 * @param info - HTTP-level error details.
	 * @returns `true` if handled (prevents double notification), `false`/`void` to allow default handling.
	 */
	onError?(response: TResponse, info?: RequestErrorInfo): void | boolean;
	/**
	 * Success callback invoked with the parsed response when `Error` is not set.
	 * @param response - Successful service response.
	 */
	onSuccess?(response: TResponse): void;
}
/**
 * Centralized error handling helpers for service and runtime errors.
 * @remarks
 * `showServiceError` is the default handler for failed service calls;
 * `runtimeErrorHandler` / `unhandledRejectionHandler` surface uncaught
 * script errors during development only (see {@link ErrorHandling.isDevelopmentMode}).
 */
export declare namespace ErrorHandling {
	/**
	 * Shows a service error to the user as an alert dialog or toast notification.
	 * @param error - Structured service error returned by the server (`ServiceError`). May be `null` when the failure is transport-level.
	 * @param errorInfo - Low-level request metadata (HTTP status, `responseText`, etc.). When `error` is falsy, this determines the fallback message.
	 * @param errorMode - Presentation mode. `"notification"` uses {@link notifyError}; any other value (or omitted) uses {@link alertDialog} / {@link iframeDialog}.
	 * @remarks
	 * - When `error` has a `Message` or `Code`, that text is shown directly.
	 * - When `error` is `null` and `errorInfo.statusText === "abort"`, the error is silently ignored.
	 * - When `responseText` is HTML and `errorMode` is `"alert"`, it is rendered in an {@link iframeDialog}; otherwise it is shown as a notification.
	 * - In production, a "See browser console (F12)" hint is appended to generic HTTP errors.
	 */
	function showServiceError(error: ServiceError, errorInfo?: RequestErrorInfo, errorMode?: "alert" | "notification"): void;
	/**
	 * Global runtime error handler suitable for `window.onerror`.
	 * @param messageOrEvent - Error message string, or the `ErrorEvent` dispatched by the browser.
	 * @param filename - Source file URL when the handler is invoked with discrete arguments (`window.onerror` signature).
	 * @param lineno - One-based line number of the error.
	 * @param colno - One-based column number of the error.
	 * @param error - The associated `Error` object, when available.
	 * @remarks
	 * Only surfaces a notification when {@link ErrorHandling.isDevelopmentMode} returns `true`; otherwise the error is ignored.
	 * The handler is wired as `window.onerror` in `ScriptInit.ts` so developers notice failures without opening the console.
	 * The displayed notification includes file, line/column, message, and stack trace when available.
	 */
	function runtimeErrorHandler(messageOrEvent: string | ErrorEvent, filename?: string, lineno?: number, colno?: number, error?: Error): void;
	/**
	 * Determines whether the current host should be treated as a development environment.
	 * @returns `true` when `window.location.hostname` is `localhost`, `127.0.0.1`, `[::1]`, or ends with `.local` / `.localhost`; `false` otherwise.
	 * @remarks
	 * Both {@link ErrorHandling.runtimeErrorHandler} and {@link ErrorHandling.unhandledRejectionHandler} gate their notifications on this check.
	 * Override by replacing `ErrorHandling.isDevelopmentMode` at startup if a different heuristic is needed.
	 */
	function isDevelopmentMode(): boolean;
	/**
	 * Handler for `unhandledrejection` events that filters expected service errors and surfaces real bugs during development.
	 * @param err - The `PromiseRejectionEvent` fired by the browser.
	 * @remarks
	 * - When `err.reason.origin === "serviceCall"` (Serenity service layer), the rejection is suppressed via `preventDefault()` and, unless `silent` or `kind !== "exception"`, logged to the console. This avoids noisy console errors for handled service failures.
	 * - For all other rejections, a notification is shown only when {@link ErrorHandling.isDevelopmentMode} returns `true`.
	 * @example
	 * ```ts
	 * window.addEventListener("unhandledrejection", ErrorHandling.unhandledRejectionHandler);
	 * ```
	 */
	function unhandledRejectionHandler(err: PromiseRejectionEvent): void;
}
/**
 * Represents a Fluent object, which is similar to jQuery but works for only one element.
 * It implements the `ArrayLike` interface and can have 0 (null) or 1 element.
 */
export interface Fluent<TElement extends HTMLElement = HTMLElement> extends ArrayLike<TElement> {
	/**
	 * Adds one or more classes to the element. Any falsy value is ignored.
	 *
	 * @param value The class or classes to add. It can be a string, boolean, or an array of strings or booleans.
	 * @returns The Fluent object itself.
	 */
	addClass(value: string | boolean | (string | boolean)[]): this;
	/**
	 * Appends content to the element.
	 *
	 * @param child The content to append. It can be a string, a Node object, or another Fluent object.
	 * @returns The Fluent object itself.
	 */
	append(child: string | Node | Fluent<any>): this;
	/**
	 * Inserts content after the element.
	 *
	 * @param content The content to insert. It can be a string, a Node object, or another Fluent object.
	 * @returns The Fluent object itself.
	 */
	after(content: string | Node | Fluent<any>): this;
	/**
	 * Appends the element to the specified parent element.
	 *
	 * @param parent The parent element to append to. It can be an Element object or another Fluent object.
	 * @returns The Fluent object itself.
	 */
	appendTo(parent: Element | Fluent<any>): this;
	/**
	 * Gets the value of the specified attribute.
	 *
	 * @param name The name of the attribute.
	 * @returns The value of the attribute.
	 */
	attr(name: string): string;
	/**
	 * Sets the value of the specified attribute.
	 *
	 * @param name The name of the attribute.
	 * @param value The value of the attribute. If the value is falsy the attribute is removed.
	 * @returns The Fluent object itself if a value is provided.
	 */
	attr(name: string, value: string | number | boolean | null | undefined): this;
	/**
	 * Inserts content before the element.
	 *
	 * @param content The content to insert. It can be a string, a Node object, or another Fluent object.
	 * @returns The Fluent object itself.
	 */
	before(content: string | Node | Fluent<any>): this;
	/**
	 * Gets the children of the element as an array (not Fluent)
	 *
	 * @param selector Optional. A CSS selector to filter the children.
	 * @returns An array of HTMLElement objects representing the children.
	 */
	children<TElement extends HTMLElement = HTMLElement>(selector?: string): TElement[];
	/**
	 * Sets (overrides) the class attribute of the element. Any falsy value is ignored.
	 *
	 * @param value The class or classes to add. It can be a string, boolean, or an array of strings or booleans.
	 * @returns The Fluent object itself.
	 */
	class(value: string | boolean | (string | boolean)[]): this;
	/**
	 * Triggers a click event on the element.
	 *
	 * @returns The Fluent object itself.
	 */
	click(): this;
	/**
	 * Adds a click event listener on the element.
	 *
	 * @param listener A callback function to execute when the click event is triggered.
	 * @returns The Fluent object itself.
	 */
	click(listener: (e: MouseEvent) => void): this;
	/**
	 * Gets the closest ancestor of the element that matches the specified selector.
	 *
	 * @param selector A CSS selector to match against.
	 * @returns A Fluent object representing the closest ancestor element.
	 */
	closest<TElement extends HTMLElement = HTMLElement>(selector: string): Fluent<TElement>;
	/**
	 * Gets or sets the value of the specified data attribute.
	 *
	 * @param name The name of the data attribute.
	 * @returns The value of the data attribute if no value is provided, or the Fluent object itself if a value is provided.
	 */
	data(name: string): string;
	data(name: string, value: string): this;
	/**
	 * Executes a callback function for the element in the Fluent object if it is not null.
	 *
	 * @param callback The callback function to execute for each element.
	 * @returns The Fluent object itself.
	 */
	each(callback: (el: TElement) => void): this;
	/**
	 * Gets the underlying HTML element.
	 *
	 * @returns The underlying HTML element.
	 */
	getNode(): TElement;
	/**
	 * Removes all child nodes from the element. It also clears event handlers attached via Fluent, and disposes any attached widgets.
	 *
	 * @returns The Fluent object itself.
	 */
	empty(): this;
	/**
	 * Finds all elements that match the specified selector within the element.
	 *
	 * @param selector A CSS selector to match against.
	 * @returns An array of elements that match the selector.
	 */
	findAll<TElement extends HTMLElement = HTMLElement>(selector: string): TElement[];
	/**
	 * Finds each element that matches the specified selector within the element and executes a callback function for each found element as a Fluent object.
	 *
	 * @param selector A CSS selector to match against.
	 * @param callback The callback function to execute for each found element. It receives a Fluent object for each element.
	 * @returns The Fluent object itself.
	 */
	findEach<TElement extends HTMLElement = HTMLElement>(selector: string, callback: (el: Fluent<TElement>, index: number) => void): this;
	/**
	 * Finds the first element that matches the specified selector within the element.
	 *
	 * @param selector A CSS selector to match against.
	 * @returns A Fluent object representing the first element that matches the selector.
	 */
	findFirst<TElement extends HTMLElement = HTMLElement>(selector: string): Fluent<TElement>;
	/**
	 * Sets focus on the element.
	 *
	 * @returns The Fluent object itself.
	 */
	focus(): this;
	/**
	 * Checks if the element has the specified class.
	 *
	 * @param klass The class to check for.
	 * @returns `true` if the element has the class, `false` otherwise.
	 */
	hasClass(klass: string): boolean;
	/**
	 * Gets the value of the hidden attribute/property.
	 *
	 * @returns The value of the hidden attribute/property
	 */
	hidden(name: string): boolean;
	/**
	 * Sets the value of the hidden property/attribute.
	 *
	 * @param value The value of the attribute. If the value is falsy the attribute is removed.
	 * @returns The Fluent object itself if a value is provided.
	 */
	hidden(value: boolean): this;
	/**
	 * Hides the element by setting its hidden property to true.
	 *
	 * @returns The Fluent object itself.
	 */
	hide(): this;
	/**
	 * Gets the widget associated with the element.
	 *
	 * @param type Optional. The constructor function of the widget.
	 * @returns The widget associated with the element.
	 */
	getWidget<TWidget>(type?: {
		new (...args: any[]): TWidget;
	}): TWidget;
	/**
	 * Inserts the element after the specified reference element.
	 *
	 * @param referenceNode The reference element to insert after. It can be an HTMLElement object or another Fluent object.
	 * @returns The Fluent object itself.
	 */
	insertAfter(referenceNode: HTMLElement | Fluent<HTMLElement>): this;
	/**
	 * Inserts the element before the specified reference element.
	 *
	 * @param referenceNode The reference element to insert before. It can be an HTMLElement object or another Fluent object.
	 * @returns The Fluent object itself.
	 */
	insertBefore(referenceNode: HTMLElement | Fluent<HTMLElement>): this;
	/**
	 * Gets an iterator for the elements in the Fluent object.
	 *
	 * @returns An iterator for the elements in the Fluent object.
	 */
	[Symbol.iterator]: TElement[];
	/**
	 * Gets the element at the specified index.
	 *
	 * @param n The index of the element.
	 * @returns The element at the specified index.
	 */
	readonly [n: number]: TElement;
	/**
	 * Gets the number of elements in the Fluent object. Can only be 1 or 0.
	 */
	readonly length: number;
	/**
	 * Removes an event listener from the element.
	 *
	 * @param type The type of the event. It can include a ".namespace" similar to jQuery.
	 * @param listener The event listener to remove.
	 * @returns The Fluent object itself.
	 */
	off<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): this;
	off(type: string): this;
	off(type: string, listener: EventListener): this;
	off(type: string, selector: string, delegationHandler: Function): this;
	/**
	 * Adds an event listener to the element. It is possible to use delegated events like jQuery.
	 *
	 * @param type The type of the event. It can include a ".namespace" similar to jQuery.
	 * @param listener The event listener to add.
	 * @returns The Fluent object itself.
	 */
	on<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): this;
	on(type: string, listener: EventListener): this;
	on(type: string, selector: string, delegationHandler: Function): this;
	/**
	 * Adds a one-time event listener to the element. It is possible to use delegated events like jQuery.
	 *
	 * @param type The type of the event. It can include a ".namespace" similar to jQuery.
	 * @param listener The event listener to add.
	 * @returns The Fluent object itself.
	 */
	one<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): this;
	one(type: string, listener: EventListener): this;
	one(type: string, selector: string, delegationHandler: Function): this;
	/**
	 * Checks if the element matches the specified selector.
	 *
	 * @param selector A CSS selector to match against.
	 * @returns `true` if the element matches the selector, `false` otherwise.
	 */
	matches(selector: string): boolean;
	/**
	 * Gets the next sibling element that matches the specified selector, or the first sibling if no selector is provided..
	 *
	 * @param selector Optional. A CSS selector to filter the next sibling.
	 * @returns A Fluent object representing the next sibling element.
	 */
	nextSibling(selector?: string): Fluent<any>;
	/**
	 * Gets the parent element of the element.
	 *
	 * @returns A Fluent object representing the parent element.
	 */
	parent<TElement extends HTMLElement = HTMLElement>(): Fluent<TElement>;
	/**
	 * Prepends content to the element.
	 *
	 * @param child The content to prepend. It can be a string, a Node object, or another Fluent object.
	 * @returns The Fluent object itself.
	 */
	prepend(child: string | Node | Fluent<any>): this;
	/**
	 * Prepends the element to the specified parent element.
	 *
	 * @param parent The parent element to prepend to. It can be an Element object or another Fluent object.
	 * @returns The Fluent object itself.
	 */
	prependTo(parent: Element | Fluent<any>): this;
	/**
	 * Gets the previous sibling element that matches the specified selector, or the first sibling if no selector is provided.
	 *
	 * @param selector Optional. A CSS selector to filter the previous sibling.
	 * @returns A Fluent object representing the previous sibling element.
	 */
	prevSibling(selector?: string): Fluent<any>;
	/**
	 * Removes the element from the DOM. It also removes event handlers and disposes widgets by calling "disposing" event handlers.
	 *
	 * @returns The Fluent object itself.
	 */
	remove(): this;
	/**
	 * Removes the specified attribute from the element.
	 *
	 * @param name The name of the attribute to remove.
	 * @returns The Fluent object itself.
	 */
	removeAttr(name: string): this;
	/**
	 * Removes one or more classes from the element. Any falsy value is ignored.
	 *
	 * @param value The class or classes to remove. It can be a string, boolean, or an array of strings or booleans.
	 * @returns The Fluent object itself.
	 */
	removeClass(value: string | boolean | (string | boolean)[]): this;
	/**
	 * Shows the element by setting its hidden property to false.
	 *
	 * @returns The Fluent object itself.
	 */
	show(): this;
	/**
	 * Executes a callback function to modify the inline style of the element.
	 *
	 * @param callback The callback function to modify the inline style.
	 * @returns The Fluent object itself.
	 */
	style(callback: (css: CSSStyleDeclaration) => void): this;
	/**
	 * Gets or sets the text content of the element.
	 *
	 * @returns The text content of the element if no value is provided, or the Fluent object itself if a value is provided.
	 */
	text(): string;
	text(value: string): this;
	/**
	 * Toggles the visibility of the element.
	 *
	 * @param flag Optional. A flag indicating whether to show or hide the element. If not provided, the visibility will be toggled.
	 * @returns The Fluent object itself.
	 */
	toggle(flag?: boolean): this;
	/**
	 * Toggles one or more classes on the element. If the class exists, it is removed; otherwise, it is added.
	 *
	 * @param value The class or classes to toggle. It can be a string, boolean, or an array of strings or booleans.
	 * @returns The Fluent object itself.
	 */
	toggleClass(value: (string | boolean | (string | boolean)[]), add?: boolean): this;
	/**
	 * Triggers a specified event on the element.
	 *
	 * @param type The type of the event to trigger.
	 * @param args Optional. An object that specifies event-specific initialization properties.
	 * @returns The Fluent object itself.
	 */
	trigger(type: string, args?: any): this;
	/**
	 * Tries to get the widget associated with the element.
	 *
	 * @param type Optional. The constructor function of the widget.
	 * @returns The widget associated with the element, or `null` if no widget is found.
	 */
	tryGetWidget<TWidget>(type?: {
		new (...args: any[]): TWidget;
	}): TWidget;
	/**
	 * Gets or sets the value of the element.
	 *
	 * @param value The value to set. If no value is provided, returns the current value of the element.
	 * @returns The value of the element if no value is provided, or the Fluent object itself if a value is provided.
	 */
	val(value: string): this;
	val(): string;
}
/**
 * Creates a {@link Fluent} wrapper from a tag name or an existing element.
 * @param tag - Tag name to create (e.g. `"div"`). Must match `^[a-zA-Z][a-zA-Z0-9\\-]*$`; otherwise an empty wrapper is returned.
 * @returns A {@link Fluent} wrapping the newly created element.
 */
export declare function Fluent<K extends keyof HTMLElementTagNameMap>(tag: K): Fluent<HTMLElementTagNameMap[K]>;
export declare namespace Fluent {
	var ready: (callback: () => void) => void;
	var byId: <TElement extends HTMLElement>(id: string) => Fluent<TElement>;
	var findAll: <TElement extends HTMLElement>(selector: string) => TElement[];
	var findEach: <TElement extends HTMLElement>(selector: string, callback: (el: Fluent<TElement>) => void) => void;
	var findFirst: <TElement extends HTMLElement>(selector: string) => Fluent<TElement>;
}
/**
 * Wraps an existing element in a {@link Fluent} instance.
 * @param element - Element to wrap; `null` / `undefined` yields an empty wrapper.
 * @returns A {@link Fluent} wrapping `element`.
 */
export declare function Fluent<TElement extends HTMLElement>(element: TElement): Fluent<TElement>;
/**
 * Wraps an `EventTarget` (typically an `HTMLElement`) in a {@link Fluent} instance.
 * @param element - Target to wrap.
 * @returns A {@link Fluent} wrapping `element`.
 */
export declare function Fluent(element: EventTarget): Fluent<HTMLElement>;
/**
 * Static helpers that operate on raw DOM elements without requiring a {@link Fluent} wrapper.
 * @remarks Supports jQuery-style namespaced and delegated events via the shared `fluent-events` module.
 */
export declare namespace Fluent {
	/**
	 * Adds an event listener, with optional delegation and namespace support.
	 * @param element - Target element to listen on.
	 * @param type - Event type; may include a `.namespace` suffix (e.g. `"click.myNs"`).
	 * @param listener - Callback to invoke when the event fires.
	 * @returns `void`.
	 */
	function on<K extends keyof HTMLElementEventMap>(element: EventTarget, type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): void;
	function on(element: EventTarget, type: string, listener: EventListener): void;
	function on(element: EventTarget, type: string, selector: string, delegationHandler: Function): void;
	/**
	 * Adds a one-time event listener that is automatically removed after the first invocation.
	 * @param element - Target element to listen on.
	 * @param type - Event type; may include a `.namespace` suffix.
	 * @param listener - Callback to invoke once when the event fires.
	 * @returns `void`.
	 */
	function one<K extends keyof HTMLElementEventMap>(element: EventTarget, type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): void;
	function one(element: EventTarget, type: string, listener: EventListener): void;
	function one(element: EventTarget, type: string, selector: string, delegationHandler: Function): void;
	/**
	 * Removes an event listener (or all listeners for a namespaced type).
	 * @param element - Target element.
	 * @param type - Event type; may include a `.namespace`. When only a namespace is handled, all matching listeners are removed.
	 * @param listener - Specific callback to remove. When omitted, all listeners for `type` are removed.
	 * @returns `void`.
	 */
	function off<K extends keyof HTMLElementEventMap>(element: EventTarget, type: K, listener?: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): void;
	function off(element: EventTarget, type: string, listener?: EventListener): void;
	function off(element: EventTarget, type: string, selector?: string, delegationHandler?: Function): void;
	/**
	 * Dispatches a synthetic event on the element.
	 * @param element - Target element to dispatch on.
	 * @param type - Event type to trigger (e.g. `"click"`, `"change"`).
	 * @param args - Optional properties merged into the created `Event` / `CustomEvent` (`detail`, `bubbles`, etc.).
	 * @returns The dispatched event. Use {@link Fluent.isDefaultPrevented} to test whether `preventDefault()` was called.
	 */
	function trigger(element: EventTarget, type: string, args?: any): Event & {
		isDefaultPrevented?(): boolean;
	};
	/**
	 * Adds one or more classes to the element.
	 * @param element - Target element.
	 * @param value - Class name(s) to add. Strings are split on whitespace; arrays are flattened; falsy entries are ignored.
	 * @returns `void`.
	 */
	function addClass(element: Element, value: string | boolean | (string | boolean)[]): void;
	/**
	 * Removes all child nodes from the element, notifying `disposing` handlers and clearing Fluent event listeners.
	 * @param element - Element to empty. No-op when `null` / `undefined`.
	 * @returns `void`.
	 */
	function empty(element: Element): void;
	/**
	 * Tests whether the element is considered visible (jQuery `:visible` semantics).
	 * @param element - Element to test.
	 * @returns `true` when the element has non-zero `offsetWidth` / `offsetHeight` or any client rects.
	 */
	function isVisibleLike(element: Element): boolean;
	/**
	 * Removes the element from the DOM, clearing Fluent event handlers and firing `disposing` notifications for the element and its descendants.
	 * @param element - Element to remove. No-op when `null` / `undefined`.
	 * @returns `void`.
	 */
	function remove(element: Element): void;
	/**
	 * Removes one or more classes from the element.
	 * @param element - Target element.
	 * @param value - Class name(s) to remove. Falsy entries are ignored.
	 * @returns `void`.
	 */
	function removeClass(element: Element, value: string | boolean | (string | boolean)[]): void;
	/**
	 * Shows or hides the element, handling `hidden`, `display:none`, and `.hidden` class.
	 * @param element - Target element.
	 * @param flag - When `true`, shows the element; when `false`, hides it; when omitted, toggles the current visibility.
	 * @returns `void`.
	 */
	function toggle(element: Element, flag?: boolean): void;
	/**
	 * Toggles one or more classes on the element.
	 * @param element - Target element.
	 * @param value - Class name(s) to toggle. Falsy entries are ignored.
	 * @param add - When `true`, forces addition; when `false`, forces removal; when omitted, each class is toggled.
	 * @returns `void`.
	 */
	function toggleClass(element: Element, value: string | boolean | (string | boolean)[], add?: boolean): void;
	/**
	 * Normalizes a class value (string, boolean flag, or nested array) to a space-separated class string.
	 * @param value - Value to normalize. Non-string primitives are stringified; booleans and `null` / `undefined` yield `""`; arrays are recursively flattened and falsy entries dropped.
	 * @returns The concatenated class string (may be empty).
	 */
	function toClassName(value: string | boolean | (string | boolean)[]): string;
	/**
	 * Tests whether the element is an input-like control (`input`, `select`, `textarea`, or `button`).
	 * @param element - Element to test.
	 * @returns `true` when the element's tag name matches an input-like tag.
	 */
	function isInputLike(element: Element): element is (HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement | HTMLButtonElement);
	/** CSS selector that matches input-like elements (`input,select,textarea,button`). */
	const inputLikeSelector = "input,select,textarea,button";
	/**
	 * Tests whether a tag name is an input-like tag.
	 * @param tag - Tag name to test (case-insensitive).
	 * @returns `true` when `tag` is `input`, `select`, `textarea`, or `button`.
	 */
	function isInputTag(tag: string): boolean;
	/**
	 * Tests whether `preventDefault()` was called on the event, supporting both native and jQuery-wrapped events.
	 * @param event - Event object, possibly with a jQuery `isDefaultPrevented()` method.
	 * @returns `true` when `defaultPrevented` is `true` or `isDefaultPrevented()` returns `true`.
	 */
	function isDefaultPrevented(event: {
		defaultPrevented?: boolean;
		isDefaultPrevented?: () => boolean;
	}): boolean;
	/**
	 * Reads a property from the event, falling back to wrapped/original event containers.
	 * @param event - Event object, potentially jQuery-wrapped (`originalEvent`, `nativeEvent`).
	 * @param prop - Property name to read.
	 * @returns The property value, or `undefined` when not found. Lookup order: `event[prop]` → `event.nativeEvent[prop]` → `event.originalEvent[prop]` / `event.nativeEvent.originalEvent[prop]` → `event.detail[prop]`.
	 */
	function eventProp(event: any, prop: string): any;
}
/**
 * Locale settings for number formatting, mirroring .NET `NumberFormatInfo`.
 * @remarks Used by {@link formatNumber}, {@link parseDecimal}, and {@link parseInteger} via {@link Culture}.
 */
export interface NumberFormat {
	/** Character used as the decimal separator (e.g. `"."` or `","`). */
	decimalSeparator: string;
	/** Character used to group thousands (e.g. `","` or `"."`). */
	groupSeparator?: string;
	/** Default number of fractional digits for `"f"` / `"n"` / `"c"` / `"p"` formats. @defaultValue `2` (Invariant). */
	decimalDigits?: number;
	/** Symbol for positive numbers (rarely displayed). @defaultValue `"+"`. */
	positiveSign?: string;
	/** Symbol for negative numbers. @defaultValue `"-"`. */
	negativeSign?: string;
	/** String rendered for `NaN` values. */
	nanSymbol?: string;
	/** Symbol appended for percent (`"p"`) formatting. @defaultValue `"%"`. */
	percentSymbol?: string;
	/** Symbol appended for currency (`"c"`) formatting. @defaultValue `"$"`. */
	currencySymbol?: string;
}
/**
 * Locale settings for date/time formatting, mirroring .NET `DateTimeFormatInfo`.
 * @remarks Consumed by {@link formatDate} and {@link parseDate} via {@link Culture}.
 */
export interface DateFormat {
	/** Character separating date parts (e.g. `"/"` or `"."`). */
	dateSeparator?: string;
	/** Default date-only format string (e.g. `"dd/MM/yyyy"`). */
	dateFormat?: string;
	/** Token order for parsing ambiguous numeric dates: `"dmy"`, `"mdy"`, or `"ymd"`. */
	dateOrder?: string;
	/** Default combined date+time format string (e.g. `"dd/MM/yyyy HH:mm:ss"`). */
	dateTimeFormat?: string;
	/** Designator for AM hours (used with `t`/`tt` tokens). @defaultValue `"AM"`. */
	amDesignator?: string;
	/** Designator for PM hours (used with `t`/`tt` tokens). @defaultValue `"PM"`. */
	pmDesignator?: string;
	/** Character separating time parts. @defaultValue `":"`. */
	timeSeparator?: string;
	/** Index of the first day of the week (`0` = Sunday, `1` = Monday). */
	firstDayOfWeek?: number;
	/** Full day names starting with Sunday — 7 entries. */
	dayNames?: string[];
	/** Abbreviated day names (e.g. `"Sun"`, `"Mon"`). — 7 entries. */
	shortDayNames?: string[];
	/** Two-letter day names (e.g. `"Su"`, `"Mo"`). — 7 entries. */
	minimizedDayNames?: string[];
	/** Full month names starting with January — 12 entries plus a trailing empty slot for compatibility. */
	monthNames?: string[];
	/** Abbreviated month names (e.g. `"Jan"`, `"Feb"`). — 12 entries plus a trailing empty slot. */
	shortMonthNames?: string[];
}
/**
 * Combined locale settings, mirroring .NET `CultureInfo`.
 * @remarks Extends both {@link NumberFormat} and {@link DateFormat} with string comparison helpers.
 */
export interface Locale extends NumberFormat, DateFormat {
	/**
	 * Locale-aware string comparator, analogous to `String.Compare`.
	 * @param a - First string to compare (may be `null`).
	 * @param b - Second string to compare (may be `null`).
	 * @returns Negative if `a < b`, positive if `a > b`, `0` if equal.
	 */
	stringCompare?: (a: string, b: string) => number;
	/**
	 * Locale-aware upper-casing function.
	 * @param a - String to convert.
	 * @returns The upper-cased string.
	 */
	toUpper?: (a: string) => string;
}
/**
 * Invariant locale with US-English / POSIX defaults, analogous to `CultureInfo.InvariantCulture`.
 * @remarks Used as the fallback for {@link Culture} and as the baseline for parsing/formatting when no culture is supplied.
 */
export declare let Invariant: Locale;
/**
 * Current culture used by all formatting and parsing helpers, analogous to `CultureInfo.CurrentCulture`.
 * @remarks
 * Initialized by {@link resetCultureSettings}. When a `<script id="ScriptCulture">` element containing a JSON object is present (rendered by `_LayoutHead.cshtml`), its values override the defaults. The `DecimalSeparator` / `GroupSeparator` keys are mapped explicitly; remaining keys are camel-cased from PascalCase.
 */
export declare let Culture: Locale;
/**
 * Resets {@link Culture} to its default values derived from {@link Invariant}.
 * @remarks
 * - Sets `dateOrder` to `"dmy"`, `dateFormat` to `"dd/MM/yyyy"`, and installs a `stringCompare` based on `String.prototype.localeCompare` with `document.documentElement.lang` when available.
 * - If a `<script id="ScriptCulture">` JSON block exists, its properties override the defaults (with special handling for `DecimalSeparator` / `GroupSeparator`).
 * - Exports in `vite8-symbol-typeinfo-workaround.md` note that no `Symbol.typeInfo` side-effects occur here.
 */
export declare function resetCultureSettings(): void;
/**
 * Formats a string by replacing `{index[:format]}` placeholders with the supplied arguments, using {@link Culture} for locale-aware value formatting.
 * @param format - Composite format string (e.g. `"Hello {0}, you have {1:n2} messages"`). `{{` / `}}` are escaped to a single brace.
 * @param prm - Values to substitute; each may be a number, `Date`, or any object with a `format(formatSpec, locale)` method. Nullish values render as empty strings.
 * @returns The formatted string.
 * @example
 * ```ts
 * stringFormat("Hello {0}, balance {1:c}", "Alice", 1234.5); // uses Culture currency symbol
 * ```
 */
export declare function stringFormat(format: string, ...prm: any[]): string;
/**
 * Locale-specific variant of {@link stringFormat}.
 * @param l - Locale whose settings are applied when formatting each argument.
 * @param format - Composite format string with `{index[:format]}` placeholders.
 * @param prm - Values to substitute. Numbers and Dates are formatted with `l`; objects with a `format` method are delegated to that method.
 * @returns The formatted string.
 */
export declare function stringFormatLocale(l: Locale, format: string, ...prm: any[]): string;
/**
 * Rounds a number to the specified number of fractional digits using "away from zero" rounding.
 * @param num - Value to round; `undefined` / `NaN` is forwarded to `Math.round` semantics.
 * @param d - Number of digits after the decimal point. @defaultValue `0` (integer rounding).
 * @returns The rounded value. `0` is normalized to `0` (not `-0`).
 * @remarks
 * Unlike `Math.round`, `1.5` rounds to `2` and `-1.5` rounds to `-2`. Implemented via exponent shifting to avoid floating-point artifacts.
 * @example
 * ```ts
 * round(1.005, 2); // 1.01
 * round(-1.5);     // -2
 * ```
 */
export declare let round: (num: number, d?: number) => number;
/**
 * Truncates a number toward zero to an integer.
 * @param n - Value to truncate; `null` / `undefined` returns `null`.
 * @returns The integer part of `n` (toward zero), or `null` for nullish input.
 * @example
 * ```ts
 * trunc(1.9);  // 1
 * trunc(-1.9); // -1
 * ```
 */
export declare let trunc: (n: number) => number;
/**
 * Formats a number using .NET-style numeric format strings and locale settings.
 * @param num - Value to format; `null` / `undefined` yields `""` and `NaN` yields `nanSymbol`.
 * @param format - Format specifier. `"g"` (general), `"d"`/`"x"`/`"e"`/`"f"`/`"n"`/`"c"`/`"p"`, or a custom pattern (`"#,##0.00"`, `"000"`, etc.). @defaultValue `"g"`.
 * @param decOrLoc - Either a {@link Locale} / {@link NumberFormat} object, or the decimal separator string for a lightweight inline locale.
 * @param grp - Group separator when `decOrLoc` is a decimal-separator string. Ignored otherwise.
 * @returns The formatted number string, applying grouping, decimal separator, and locale symbols from `decOrLoc` or {@link Culture}.
 * @remarks
 * - `"n"` / `"N"` insert grouping; `"c"`/`"p"` append `currencySymbol`/`percentSymbol` (percent multiplies by 100).
 * - Custom patterns quote literals with `'` and escape with `\`.
 * @example
 * ```ts
 * formatNumber(1234.5, "n2"); // e.g. "1,234.50" depending on Culture
 * formatNumber(0.42, "p0");   // e.g. "42%"
 * ```
 */
export declare function formatNumber(num: number, format?: string, decOrLoc?: string | NumberFormat, grp?: string): string;
/**
 * Parses a string as an integer using {@link Culture} grouping rules.
 * @param s - String to parse; `null` or whitespace yields `null`. Group separators for the current culture are stripped before validation.
 * @returns The parsed integer, `null` for empty/null input, or `NaN` when the string is not a valid integer.
 * @remarks
 * Unlike `parseInt`, only strings matching `^[+-]?\d+$` (after group-separator removal) are accepted; trailing characters cause `NaN`.
 */
export declare function parseInteger(s: string): number;
/**
 * Parses a string as a decimal number using {@link Culture} group and decimal separators.
 * @param s - String to parse; `null` or whitespace yields `null`. Group separators are stripped and the locale decimal separator is normalized to `"."` before `parseFloat`.
 * @returns The parsed number, `null` for empty/null input, or `NaN` when the string is not a valid decimal.
 * @remarks Only patterns matching `^\s*[+-]?(\d*)[decimalSep]?(\d*)\s*$` are accepted.
 */
export declare function parseDecimal(s: string): number;
/**
 * Normalizes a value to an ID suitable for entity keys.
 * @param id - Candidate ID: a number is returned as-is; a string is trimmed and, when it is a plain integer with fewer than 15 characters, parsed to a number; otherwise the trimmed string is returned. `null`, `undefined`, or whitespace yields `null`.
 * @returns The normalized ID (`number` or `string`) or `null` for empty input.
 * @example
 * ```ts
 * toId(" 42 "); // 42
 * toId("abc");  // "abc"
 * toId("");     // null
 * ```
 */
export declare function toId(id: any): any;
/**
 * Formats a `Date` (or date string) using .NET-style format tokens and locale settings.
 * @param d - Date to format, or an ISO / locale date string that is first parsed. Falsy yields `""`.
 * @param format - Format string. Special single-letter presets: `"d"` (short date), `"g"` (short datetime without seconds), `"G"` (full datetime), `"t"` (time only), `"s"` (sortable `yyyy-MM-ddTHH:mm:ss`), `"u"` (UTC sortable), `"U"` (locale datetime in UTC), `"i"`/`"id"`/`"it"` (JS `toString` variants). Prefixing with `"%"` forces a custom token (e.g. `"%M"`). When `null`, the locale's `dateFormat` is used.
 * @param locale - Locale overrides token names and separators. Defaults to {@link Culture}.
 * @returns The formatted date string, or `""` / the original string on parse failure.
 * @example
 * ```ts
 * formatDate(new Date(2019, 0, 1), "yyyy-MM-dd");                // "2019-01-01"
 * formatDate(new Date(2019, 0, 1, 12), "yyyy-MM-dd HH:mm:ss");   // "2019-01-01 12:00:00"
 * formatDate(new Date(2019, 0, 1, 12), "yyyy-MM-dd HH:mm:ss.fff"); // "2019-01-01 12:00:00.000"
 * formatDate(new Date(2019, 0, 1, 12), "yyyy-MM-dd HH:mm:ss.fff tt"); // "2019-01-01 12:00:00.000 PM"
 * ```
 */
export declare function formatDate(d: Date | string, format?: string, locale?: Locale): string;
/**
 * Formats a date as an ISO 8601 UTC timestamp (`yyyy-MM-ddTHH:mm:ss.sssZ`).
 * @param d - Date to format. `null` / `undefined` yields `""`.
 * @returns The UTC ISO string with zero-padded components, or `""` for nullish input.
 */
export declare function formatISODateTimeUTC(d: Date): string;
/**
 * Parses a string that is expected to be in ISO 8601 UTC date/time format.
 * @param s - String to parse; `null` yields `null`, empty string yields `null`, and non-ISO strings yield an invalid `Date` (`NaN`). Bare dates (`yyyy-MM-dd`, length 10) are normalized to midnight UTC.
 * @returns The parsed `Date`, `null` for null/empty input, or an invalid `Date` when the string does not match the ISO pattern.
 */
export declare function parseISODateTime(s: string): Date;
/**
 * Parses a date string in ISO 8601, locale, or JS date format.
 * @param s - String to parse; `null` / empty / whitespace yields `null`. ISO prefixes (`yyyy-MM-dd` / `yyyy-MM-ddTHH:mm:ss`) are delegated to {@link parseISODateTime}; strings containing a space and colon are split into date + time halves. Numeric parts are validated and two-digit years are expanded using a 10-year sliding window.
 * @param dateOrder - Override for ambiguous numeric dates (`"dmy"` / `"mdy"` / `"ymd"`). Defaults to {@link Culture}.`dateOrder`.
 * @returns The parsed `Date`, `null` for empty input, or an invalid `Date` (`NaN`) when the string is not a valid date.
 */
export declare function parseDate(s: string, dateOrder?: string): Date;
/**
 * Splits a date string into its numeric parts using the first detected separator.
 * @param s - String to split; trimmed before inspection. `null` / empty yields `null`.
 * @returns An array of substrings split by `"/"`, `"."`, `"-"`, or `"\"` (whichever appears first), or a single-element array when none of those separators is present.
 */
export declare function splitDateString(s: string): string[];
/**
 * Adds one or more entries to the global localization table.
 * @param obj - Either a single key (string) whose value is `pre`, or a nested object map where leaf string values are stored under dot-joined keys (recursively). Pass `null`/`undefined`/empty to no-op.
 * @param pre - Prefix prepended to each key, or the value when `obj` is a string. Defaults to `""` for object mode.
 * @remarks The table is stored on the global object under {@link localTextTableSymbol} and is shared across the application. Nested objects are flattened with `.` separators (e.g. `{ a: { b: "x" } }` with `pre="Ns."` registers `"Ns.a.b"`).
 * @example
 * ```ts
 * addLocalText({ "Db.Northwind.CustomerName": "Customer Name" });
 * addLocalText("Db.Northwind.CustomerName", "Customer Name");
 * addLocalText({ Customer: { Name: "Name" } }, "Db.Northwind.");
 * ```
 */
export declare function addLocalText(obj: string | Record<string, string | Record<string, any>>, pre?: string): void;
/**
 * Retrieves a localized string for a key, falling back gracefully.
 * @param key - Localization key to look up (e.g. `"Dialogs.YesButton"`). If falsy, an empty string or `defaultText` is returned.
 * @param defaultText - Optional fallback returned when the key is not found. When omitted, the key itself is returned.
 * @returns The localized value, `defaultText` if provided, the key itself, or `""` for nullish keys.
 * @example
 * ```ts
 * localText("Dialogs.YesButton"); // "Yes" if registered, otherwise "Dialogs.YesButton"
 * localText("Missing.Key", "Fallback"); // "Fallback"
 * ```
 */
export declare function localText(key: string, defaultText?: string): string;
/**
 * Tries to retrieve a localized string without falling back to the key.
 * @param key - Localization key to look up.
 * @returns The localized value if found, otherwise `undefined` (unlike {@link localText} which returns the key).
 */
export declare function tryGetText(key: string): string | undefined;
/**
 * Creates a typed proxy that resolves nested property access to localized strings.
 * The proxy lazily wraps each level of `tpl`; property access concatenates `pfx` with the property name and performs the lookup according to `mode`.
 * @param obj - Target object to proxy (usually `{}`). Mutated in place with hidden symbols and returned as a `Proxy`.
 * @param pfx - Prefix prepended to every key lookup (e.g. `"Db.Northwind."`). Pass `""` for no prefix.
 * @param tpl - Template object whose shape defines the available text keys; leaf values determine nesting. `null`/`undefined` leaves resolve to string lookups, objects create nested sub-proxies.
 * @param mode - Lookup strategy: `undefined` uses {@link localText} (returns key on miss), `"asTry"` uses {@link tryGetText} (returns `undefined` on miss), `"asKey"` returns the generated key without any lookup.
 * @returns A proxy over `obj` augmented with `asTry()` and `asKey()` mode-switchers. Access a leaf string like `proxy.foo.bar` to get the localized text for `"<pfx>foo.bar"`.
 * @example
 * ```ts
 * const texts = proxyTexts({}, "", { user: { name: {} } });
 * texts.user.name.first // localText("user.name.first")
 * texts.user.asTry().name.first // tryGetText("user.name.first")
 * texts.user.asKey().name.first // "user.name.first"
 * ```
 */
export declare function proxyTexts<T extends Record<string, any> = Record<string, any>>(obj: T, pfx: string, tpl: Record<string, any>, mode?: "asTry" | "asKey"): Record<string, any> & {
	asTry(): T;
	asKey(): T;
};
/**
 * List of available languages for the translation UI.
 * Each entry pairs a language identifier with its display name.
 */
export type LanguageList = {
	id: string; /** Human-readable language name. */
	text: string;
}[];
/**
 * Options passed to {@link TranslationConfig.translateTexts} to request machine/human translations.
 */
export type TranslateTextsOptions = {
	/** The source language ID */
	SourceLanguageID?: string;
	/** An array of inputs for translation */
	Inputs: {
		/** The text key to be translated */
		TextKey?: string;
		/** The target language ID */
		TargetLanguageID?: string;
		/** The source text to be translated */
		SourceText?: string;
	}[];
};
/**
 * Result returned by {@link TranslationConfig.translateTexts} containing translated entries.
 */
export type TranslateTextsResult = {
	/** An array of resulting translations */
	Translations?: {
		/** The text key that was translated */
		TextKey?: string;
		/** The target language ID */
		TargetLanguageID?: string;
		/** The translated text */
		TranslatedText?: string;
	}[];
};
/**
 * Global configuration hooks for the optional translation service integration.
 * Assign these before invoking translation features in the UI.
 */
export declare const TranslationConfig: {
	/** Retrieves the list of available languages */
	getLanguageList: () => LanguageList;
	/** A function to translate texts based on provided options */
	translateTexts: (opt: TranslateTextsOptions) => PromiseLike<TranslateTextsResult>;
};
/**
 * Alias for {@link localText}.
 * @deprecated Prefer {@link localText} directly for better discoverability and consistency.
 */
export declare const text: typeof localText;
/**
 * Legacy namespace for local-text helpers.
 * @deprecated Use the top-level {@link addLocalText} and {@link localText} functions instead.
 */
export declare namespace LT {
	/**
	 * Alias for {@link addLocalText}.
	 * @deprecated Use {@link addLocalText} directly.
	 */
	const add: typeof addLocalText;
	/**
	 * Alias for {@link localText}.
	 * @deprecated Use {@link localText} directly.
	 */
	const getDefault: typeof localText;
}
/**
 * Options used to construct a {@link Lookup}.
 * @typeParam TItem - Type of the lookup items.
 */
export interface LookupOptions<TItem> {
	/** Name of the field that holds the unique identifier for an item. Used as the key in {@link Lookup.itemById}. */
	idField?: string;
	/** Name of the field that holds the parent identifier (for hierarchical lookups). */
	parentIdField?: string;
	/** Name of the field that holds the human-readable display text for an item. */
	textField?: string;
}
/**
 * Client-side lookup data structure that holds a flat list of items and a fast key-based index.
 * Implementations are typically returned from server-side `LookupScript` endpoints and consumed by editors such as `LookupEditor`.
 * @typeParam TItem - Type of the lookup items.
 */
export interface Lookup<TItem> {
	/** Flat array of all lookup items. */
	items: TItem[];
	/** Dictionary mapping stringified {@link LookupOptions.idField} values to their corresponding items. */
	itemById: {
		[key: string]: TItem;
	};
	/** Name of the ID field (copied from {@link LookupOptions.idField}). */
	idField: string;
	/** Name of the parent-ID field for hierarchical lookups (copied from {@link LookupOptions.parentIdField}). */
	parentIdField: string;
	/** Name of the display-text field (copied from {@link LookupOptions.textField}). */
	textField: string;
}
/**
 * Concrete implementation of the {@link Lookup} interface for client-side use.
 * Maintains `items` and a `itemById` index synchronized via {@link Lookup.update}.
 * @typeParam TItem - Type of the lookup items.
 * @example
 * ```ts
 * const lookup = new Lookup<{ id: number; name: string }>({ idField: "id", textField: "name" }, items);
 * lookup.itemById["5"] // item with id 5
 * ```
 */
export declare class Lookup<TItem> {
	/** Flat array of all lookup items. */
	items: TItem[];
	/** Dictionary mapping stringified ID values to items, rebuilt on every {@link update}. */
	itemById: {
		[key: string]: TItem;
	};
	/** Name of the ID field (from constructor options). */
	idField: string;
	/** Name of the parent-ID field for hierarchical lookups (from constructor options). */
	parentIdField: string;
	/** Name of the display-text field (from constructor options). */
	textField: string;
	/**
	 * Creates a new lookup instance.
	 * @param options - Field mapping for id/parent/text. Pass `null`/`undefined` for an empty configuration (fields remain `undefined`).
	 * @param items - Optional initial item array. If provided, {@link update} is called immediately to populate `items` and `itemById`.
	 */
	constructor(options: LookupOptions<TItem>, items?: TItem[]);
	/**
	 * Replaces the lookup contents and rebuilds the `itemById` index.
	 * @param value - New item array. `null`/`undefined` clears the lookup. Primitive values (e.g. `string` numbers from a distinct query) are auto-wrapped as `{ [idField]: value, [textField]: value }`.
	 * @remarks Re-initializes both {@link Lookup.items} and {@link Lookup.itemById}. The method is declared optional (`update?`) on the interface for compatibility with plain-object lookups, but is always present on this class.
	 */
	update?(value: TItem[]): void;
}
/**
 * Options that control the toast container element.
 * Shared by individual toast calls and the global {@link Toastr} defaults.
 */
export type ToastContainerOptions = {
	/** DOM id for the container that holds toasts. Defaults to `"toast-container"`. */
	containerId?: string;
	/** CSS class applied to the container for positioning (e.g. `"toast-top-right"`). */
	positionClass?: string;
	/** CSS selector for the parent element the container is appended to. Defaults to `"body"`. */
	target?: string;
};
/**
 * Full option set for a toast notification. Extends {@link ToastContainerOptions}
 * with display, timing, styling, and lifecycle callbacks.
 */
export type ToastrOptions = ToastContainerOptions & {
	/** Show a close button, default is false. Pass an HTMLElement for a custom button element. */
	closeButton?: boolean | HTMLElement;
	/** CSS class for the close button. Defaults to `"toast-close-button"`. */
	closeClass?: string;
	/** If `true` (default) the toast stays open while hovered and closes after {@link ToastrOptions.extendedTimeOut} when the mouse leaves. */
	closeOnHover?: boolean;
	/** Timeout in ms after mouse-leave before the toast closes when {@link ToastrOptions.closeOnHover} is enabled. Defaults to `1000`. */
	extendedTimeOut?: number;
	/** @deprecated Escape message html, default is true. Pass an HTML element to message instead. */
	escapeHtml?: boolean;
	/** CSS class for the toast icon (e.g. `"toast-info"`, `"toast-error"`). */
	iconClass?: string;
	/** CSS class for the message element. Defaults to `"toast-message"`. */
	messageClass?: string;
	/** When `true` the newest toast is inserted at the top of the container. */
	newestOnTop?: boolean;
	/** CSS class for toast positioning (also on container). Defaults to `"toast-top-right"`. */
	positionClass?: string;
	/** When `true` suppresses consecutive toasts with identical messages. Defaults to `false`. */
	preventDuplicates?: boolean;
	/** When `true` the toast message element is styled with `white-space: pre-wrap`. */
	preWrap?: boolean;
	/** Enables right-to-left layout for the toast. */
	rtl?: boolean;
	/** CSS selector for the parent element that hosts the container. Defaults to `"body"`. */
	target?: string;
	/** Duration in ms the toast stays visible. Set to `0` for sticky or `-1` to disable auto-hide (extended timeout is then ignored). Defaults to `5000`. */
	timeOut?: number;
	/** CSS class for the toast element itself. Defaults to `"toast"`. */
	toastClass?: string;
	/** When `true` (default) clicking the toast dismisses it. */
	tapToDismiss?: boolean;
	/** CSS class for the title element. Defaults to `"toast-title"`. */
	titleClass?: string;
	/** Callback invoked when the toast element is clicked. */
	onclick?: (event: MouseEvent) => void;
	/** Callback invoked when the close button is clicked. */
	onCloseClick?: (event: Event) => void;
	/** Callback invoked after the toast is hidden and removed. */
	onHidden?: () => void;
	/** Callback invoked after the toast is shown. */
	onShown?: () => void;
};
/**
 * Internal descriptor for a toast notification passed to {@link Toastr.notify}.
 */
export type NotifyMap = {
	/** Toast type key (`"success"` | `"info"` | `"warning"` | `"error"`). */
	type: string;
	/** CSS class for the toast icon corresponding to the type. */
	iconClass: string;
	/** Optional title content for the toast. */
	title?: RenderableContent;
	/** Optional message content for the toast. */
	message?: RenderableContent;
};
/**
 * Toast notification manager. Provides `success` / `info` / `warning` / `error`
 * helpers, container management, and duplicate suppression. A singleton instance
 * is exported as the default export; custom instances can be constructed with
 * overriding {@link ToastrOptions}.
 */
export declare class Toastr {
	private listener;
	private toastId;
	private previousToast;
	/** Effective options for this instance (merged from defaults and constructor overrides). */
	options: ToastrOptions;
	/**
	 * Creates a new Toastr instance.
	 * @param options - Options merged over the global defaults / parent instance options.
	 */
	constructor(options?: ToastrOptions);
	/**
	 * Gets the toast container element, optionally creating it.
	 * @param options - Container options that override instance defaults when resolving `containerId` / `target` / `positionClass`.
	 * @param create - When `true` creates the container if it does not exist.
	 * @returns The container element, or `null` if not found and `create` is `false`.
	 */
	getContainer(options?: ToastContainerOptions, create?: boolean): HTMLElement;
	/**
	 * Shows an error toast.
	 * @param message - Message content for the toast.
	 * @param title - Optional title content.
	 * @param opt - Per-toast options that override instance defaults.
	 * @returns The created toast element, or `null` if suppressed as a duplicate.
	 */
	error(message?: RenderableContent, title?: RenderableContent, opt?: ToastrOptions): HTMLElement | null;
	/**
	 * Shows a warning toast.
	 * @param message - Message content for the toast.
	 * @param title - Optional title content.
	 * @param opt - Per-toast options that override instance defaults.
	 * @returns The created toast element, or `null` if suppressed as a duplicate.
	 */
	warning(message?: RenderableContent, title?: RenderableContent, opt?: ToastrOptions): HTMLElement | null;
	/**
	 * Shows a success toast.
	 * @param message - Message content for the toast.
	 * @param title - Optional title content.
	 * @param opt - Per-toast options that override instance defaults.
	 * @returns The created toast element, or `null` if suppressed as a duplicate.
	 */
	success(message?: RenderableContent, title?: RenderableContent, opt?: ToastrOptions): HTMLElement | null;
	/**
	 * Shows an info toast.
	 * @param message - Message content for the toast.
	 * @param title - Optional title content.
	 * @param opt - Per-toast options that override instance defaults.
	 * @returns The created toast element, or `null` if suppressed as a duplicate.
	 */
	info(message?: RenderableContent, title?: RenderableContent, opt?: ToastrOptions): HTMLElement | null;
	/**
	 * Subscribes to toast lifecycle events.
	 * @param callback - Function invoked with toast state on show / hide.
	 */
	subscribe(callback: (response: Toastr) => void): void;
	/**
	 * Publishes a toast lifecycle event to the subscriber.
	 * @param args - Toast state payload.
	 */
	publish(args: Toastr): void;
	private removeContainerIfEmpty;
	/**
	 * Removes a single toast element from the DOM and cleans up the container if empty.
	 * @param toastElement - The toast element to remove.
	 * @param options - Optional container options used to locate the container for cleanup.
	 */
	removeToast(toastElement: HTMLElement, options?: ToastContainerOptions): void;
	/**
	 * Clears all toasts from the container.
	 * @param options - Optional container options to resolve which container to clear.
	 */
	clear(options?: ToastContainerOptions): void;
	private notify;
}
/**
 * Default options applied to every toast notification.
 * Individual calls may override these via their `options` argument.
 * @remarks Mutate this object to change application-wide notification defaults (e.g. timeout or position).
 * @example
 * ```ts
 * defaultNotifyOptions.timeOut = 3000;
 * defaultNotifyOptions.positionClass = "toast-top-right";
 * ```
 */
export declare let defaultNotifyOptions: ToastrOptions;
/**
 * Positions the toast container relative to the topmost visible dialog, if any.
 * When a `.ui-dialog` / `.modal.in` / `.modal.show` element is found, the container is absolutely positioned just below it; otherwise any previous absolute positioning is cleared.
 * @param options - Toastr options used to locate the container (forwarded to `toastr.getContainer`).
 * @param create - Whether to create the container if it does not yet exist. Defaults to `true`.
 * @remarks No-ops if the container has no `position-toast` class, or if `document`/`document.body` is unavailable (e.g. SSR).
 */
export declare function positionToastContainer(options?: ToastrOptions, create?: boolean): void;
/**
 * Shows an error toast notification.
 * @param message - Main content of the toast. Accepts a plain string or {@link RenderableContent} (DOM nodes/fragments are handled by the underlying toastr renderer).
 * @param title - Optional title/header displayed above the message.
 * @param options - Per-call toastr overrides merged over {@link defaultNotifyOptions}. Use to customize timeout, position, or `escapeHtml` for this toast only.
 * @example
 * ```ts
 * notifyError("Failed to save record.", "Error");
 * ```
 */
export declare function notifyError(message: RenderableContent, title?: RenderableContent, options?: ToastrOptions): void;
/**
 * Shows an informational toast notification.
 * @param message - Main content of the toast.
 * @param title - Optional title displayed above the message.
 * @param options - Per-call toastr overrides merged over {@link defaultNotifyOptions}.
 * @example
 * ```ts
 * notifyInfo("Your changes were saved.");
 * ```
 */
export declare function notifyInfo(message: RenderableContent, title?: RenderableContent, options?: ToastrOptions): void;
/**
 * Shows a success toast notification.
 * @param message - Main content of the toast.
 * @param title - Optional title displayed above the message.
 * @param options - Per-call toastr overrides merged over {@link defaultNotifyOptions}.
 * @example
 * ```ts
 * notifySuccess("Record created successfully.", "Done");
 * ```
 */
export declare function notifySuccess(message: RenderableContent, title?: RenderableContent, options?: ToastrOptions): void;
/**
 * Shows a warning toast notification.
 * @param message - Main content of the toast.
 * @param title - Optional title displayed above the message.
 * @param options - Per-call toastr overrides merged over {@link defaultNotifyOptions}.
 * @example
 * ```ts
 * notifyWarning("Some fields are missing.", "Warning");
 * ```
 */
export declare function notifyWarning(message: RenderableContent, title?: RenderableContent, options?: ToastrOptions): void;
/**
 * Aggregation type used for grid / grouping summaries.
 * Controls how a column's values are aggregated in group headers or footers.
 */
export declare enum SummaryType {
	/** Summaries are disabled for the column. */
	Disabled = -1,
	/** No summary. */
	None = 0,
	/** Sum of values. */
	Sum = 1,
	/** Average of values. */
	Avg = 2,
	/** Minimum value. */
	Min = 3,
	/** Maximum value. */
	Max = 4
}
/**
 * Callback that augments an editor with additional UI alongside the editor element.
 * Used via {@link PropertyItem.editorAddons} to inject buttons or custom fragments.
 * @param props - Context supplied to the addon.
 * @param props.propertyItem - Metadata for the field the editor belongs to, if available.
 * @param props.editorElement - Root DOM element of the editor.
 * @param props.documentFragment - Optional document fragment the addon can attach to when the editor is rendered inside a fragment.
 * @returns Void; the addon is expected to manipulate the DOM directly.
 */
export type EditorAddon = (props: {
	propertyItem?: PropertyItem;
	editorElement: HTMLElement;
	documentFragment?: DocumentFragment;
}) => void;
/**
 * Describes a single field / column / form property as returned by server-side metadata.
 * Drives form generation, grid columns, editors, formatters and filtering. Each
 * property corresponds to a row field or an unbound UI field.
 */
export interface PropertyItem {
	/** Field / property key (usually the row field name). */
	name: string;
	/** Display title / column header / form label. Falls back to `name` when omitted. */
	title?: string;
	/** Tooltip / hint shown on hover or beside the label. */
	hint?: string;
	/** Placeholder text for the editor input. */
	placeholder?: string;
	/** Editor type key (e.g. `"String"`, `"Date"`) or a constructor / lazy import for a custom editor. */
	editorType?: string | {
		new (props?: any): any;
	} | PromiseLike<{
		new (props?: any): any;
	}>;
	/** Options passed to the editor constructor. */
	editorParams?: any;
	/** Addons rendered alongside the editor (e.g. buttons). Each entry specifies a type key or {@link EditorAddon} callback and optional params. */
	editorAddons?: {
		type: string | EditorAddon;
		params?: any;
	}[];
	/** Extra CSS class(es) applied to the editor element. */
	editorCssClass?: string;
	/** Category group shown as a collapsible section in forms. */
	category?: string;
	/** Whether the {@link category} section can be collapsed. */
	collapsible?: boolean;
	/** Whether the category starts collapsed. Only meaningful when {@link collapsible} is true. */
	collapsed?: boolean;
	/** Tab name the field belongs to when the form uses tabs. */
	tab?: string;
	/** CSS class applied to the grid cell / column. */
	cssClass?: string;
	/** CSS class applied to the column header. */
	headerCssClass?: string;
	/** CSS class applied to the form field container (`<div class="field">`). */
	formCssClass?: string;
	/** Maximum string length for validation. */
	maxLength?: number;
	/** Whether a value is required. */
	required?: boolean;
	/** Whether the field can be set on insert. When false the field is read-only during creation. */
	insertable?: boolean;
	/** Permission key required to set the field on insert. */
	insertPermission?: string;
	/** Hides the field in insert (create) mode. */
	hideOnInsert?: boolean;
	/** Whether the field can be updated after creation. */
	updatable?: boolean;
	/** Permission key required to update the field. */
	updatePermission?: string;
	/** Hides the field in update (edit) mode. */
	hideOnUpdate?: boolean;
	/** Whether the field is read-only in the UI (still submitted unless {@link skipOnSave} is set). */
	readOnly?: boolean;
	/** Permission key required to read / view the field. Clients may hide the field when the user lacks it. */
	readPermission?: string;
	/** When true the field is not populated on load (e.g. sensitive data). */
	skipOnLoad?: boolean;
	/** When true the field value is not sent back on save. */
	skipOnSave?: boolean;
	/** True for unbound fields that do not map to a row column (e.g. calculated UI-only fields). */
	unbound?: boolean;
	/** @deprecated use {@link skipOnSave} instead — kept for backward compatibility. */
	oneWay?: boolean;
	/** Default value applied to new records / empty editors. */
	defaultValue?: any;
	/** Whether the field supports per-language values (requires Localizations). */
	localizable?: boolean;
	/** Whether the column / field is visible by default. Hidden columns can still be shown via column picker. */
	visible?: boolean;
	/** Whether the user is allowed to hide the column via the column picker. */
	allowHide?: boolean;
	/** Whether the column can receive focus / be navigated via keyboard. */
	focusable?: boolean;
	/** Formatter type key or constructor / lazy import used to render the cell value. */
	formatterType?: string | {
		new (props?: any): {
			format(ctx: any): string;
		};
	} | PromiseLike<{
		new (props?: any): {
			format(ctx: any): string;
		};
	}>;
	/** Options passed to the formatter. */
	formatterParams?: any;
	/** Display format string (e.g. date / number format) consumed by the formatter. */
	displayFormat?: string;
	/** Horizontal alignment for the column (`"left" | "center" | "right"` or similar). */
	alignment?: string;
	/** Pin / freeze the column to the start (left) or end (right) of the grid, or `true` for start. */
	pin?: "start" | "end" | boolean;
	/** Preferred column width in pixels. */
	width?: number;
	/** True when {@link width} was explicitly set (vs. auto-calculated). */
	widthSet?: boolean;
	/** Minimum column width in pixels. */
	minWidth?: number;
	/** Maximum column width in pixels. */
	maxWidth?: number;
	/** Width of the form label for this field (e.g. `"150px"`). */
	labelWidth?: string;
	/** Whether the column is user-resizable. */
	resizable?: boolean;
	/** Whether to show a selection checkbox column behavior for this column. */
	showSelection?: boolean;
	/** Whether the column can be sorted. */
	sortable?: boolean;
	/** Default sort order index (lower values are sorted first). Negative or undefined means no default sort. */
	sortOrder?: number;
	/** Whether the column participates in tab-stop navigation. */
	tabbable?: boolean;
	/** Order index for grouping; controls group-by precedence when multiple columns are grouped. */
	groupOrder?: number;
	/** Aggregation used for group / footer summaries. See {@link SummaryType}. */
	summaryType?: SummaryType;
	/** When true the cell value is rendered as a link that opens the record's edit dialog. */
	editLink?: boolean;
	/** Row type key used for the edit link dialog (defaults to the current row type). */
	editLinkItemType?: string;
	/** Field name that provides the ID for the edit link (defaults to the identity field). */
	editLinkIdField?: string;
	/** Extra CSS class for the edit link anchor. */
	editLinkCssClass?: string;
	/** Filter editor type key that determines the filtering UI for this field (e.g. `"String"`, `"Date"`). */
	filteringType?: string;
	/** Options passed to the filtering editor. */
	filteringParams?: any;
	/** Field that provides the ID value for filtering (useful for lookup display fields). */
	filteringIdField?: string;
	/** When true the column cannot be used as a filter criterion. */
	notFilterable?: boolean;
	/** When true the field appears only in filter dialogs / panels and not in the grid itself. */
	filterOnly?: boolean;
	/** Whether the field appears in the quick-filter bar above the grid. */
	quickFilter?: boolean;
	/** Options for the quick-filter editor. */
	quickFilterParams?: any;
	/** When true a separator is rendered before this quick filter in the bar. */
	quickFilterSeparator?: boolean;
	/** Extra CSS class for the quick-filter item. */
	quickFilterCssClass?: string;
}
/**
 * Alias for {@link PropertyItem}.
 * Prefer {@link UIFieldItem} for new code; `PropertyItem` is kept for compatibility and may be phased out.
 */
export type UIFieldItem = PropertyItem;
/**
 * Metadata bundle for a form or columns set.
 * @property items - Primary fields / columns in display order.
 * @property additionalItems - Extra fields not shown by default but available via column picker or customization (e.g. audit fields).
 */
export interface PropertyItemsData {
	/** Primary fields / columns in display order. */
	items: PropertyItem[];
	/** Extra fields available on demand (e.g. via column picker); not rendered initially. */
	additionalItems: PropertyItem[];
}
/**
 * Alias for {@link PropertyItemsData}.
 * Prefer {@link UIFieldSet} for new code; `PropertyItemsData` is kept for compatibility.
 */
export type UIFieldSet = PropertyItemsData;
/**
 * Gets the known hash value for a given dynamic script name. They are usually
 * registered server-side via dynamic script manager and their latest known
 * hashes are passed to the client-side via a script element named RegisteredScripts.
 * @param name The dynamic script name
 * @param reload True to force resetting the script hash client side, e.g. for loading
 * lookups etc.
 * @returns The hash or null if no such known registration
 */
export declare function getScriptDataHash(name: string, reload?: boolean): string;
/**
 * Global hooks for script-data loading.
 * Allows tests or custom bootstrapping to intercept `fetchScriptData` / `ensureScriptDataSync`.
 * When the hook returns `undefined` the default `fetch` / XHR implementation is used.
 */
export declare const scriptDataHooks: {
	/**
	 * Override for script-data fetching.
	 * Return a value / promise to short-circuit the default loader, or `undefined` to fall back.
	 * @param name - Dynamic script name (e.g. `"Lookup.MyLookup"`, `"Form.MyForm"`).
	 * @param sync - When true the caller expects a synchronous result (legacy compat path). The hook must return data directly, not a promise.
	 * @param dynJS - When true the script was requested as a legacy `DynJS.axd` JavaScript payload rather than JSON. Only relevant when `sync` is true.
	 * @returns The script data directly (sync) or a promise of it, or `undefined` to use the default fetch.
	 */
	fetchScriptData: <TData>(name: string, sync?: boolean, dynJS?: boolean) => TData | Promise<TData>;
};
/**
 * Fetches a dynamic script payload by name via the `~/DynamicData/` endpoint.
 * Results are de-duplicated per `name + hash` while a request is in flight and the request
 * participates in global AJAX / block-UI tracking. Lookup payloads are wrapped as {@link Lookup} instances.
 * @typeParam TData - Expected shape of the returned payload.
 * @param name - Dynamic script name (e.g. `"Lookup.Administration.User"`, `"Form.MyForm"`, `"RemoteData.MyData"`).
 * @returns A promise that resolves with the parsed payload, or rejects if fetch is unavailable or the HTTP request fails.
 */
export declare function fetchScriptData<TData>(name: string): Promise<TData>;
/**
 * Returns cached script data if available, otherwise fetches it via `~/DynamicData/` and caches the result.
 * @typeParam TData - Expected payload type.
 * @param name - Dynamic script name.
 * @param reload - When true, busts the hash cache, clears the in-memory entry and forces a fresh fetch.
 * @returns A promise resolving to the script data.
 */
export declare function getScriptData<TData = any>(name: string, reload?: boolean): Promise<TData>;
/**
 * Synchronous (blocking) version of {@link getScriptData} for legacy compatibility.
 * Avoid in new code — it performs a synchronous XHR and blocks the UI thread.
 * @typeParam TData - Expected payload type.
 * @param name - Dynamic script name.
 * @param dynJS - When true loads via `~/DynJS.axd/*.js` and evaluates the returned script instead of JSON. Legacy path only.
 * @returns The script data (wrapped as {@link Lookup} for `Lookup.*` keys).
 * @throws If the hook returns a promise in sync mode or the HTTP request fails.
 */
export declare function ensureScriptDataSync<TData = any>(name: string, dynJS?: boolean): TData;
/**
 * Loads a `ColumnsScript` bundle for the given key.
 * @param key - Columns key (usually the row type name, e.g. `"Administration.User"`).
 * @returns A promise resolving to a {@link PropertyItemsData} containing `items` and `additionalItems` for grid columns.
 */
export declare function getColumnsScript(key: string): Promise<PropertyItemsData>;
/**
 * Loads a `FormScript` bundle for the given key.
 * @param key - Form key (usually the row/form type name, e.g. `"Administration.User"`).
 * @returns A promise resolving to a {@link PropertyItemsData} describing the form fields.
 */
export declare function getFormScript(key: string): Promise<PropertyItemsData>;
/**
 * Loads a lookup by key.
 * @typeParam TItem - Row type of the lookup items.
 * @param key - Lookup key as registered server-side via `[LookupScript]` (e.g. `"Administration.User"`).
 * @returns A promise resolving to the {@link Lookup} instance.
 */
export declare function getLookupAsync<TItem>(key: string): Promise<Lookup<TItem>>;
/**
 * Loads a `RemoteData` script by key.
 * @typeParam TData - Expected payload type.
 * @param key - Remote data key as registered server-side via `[RemoteDataScript]`.
 * @returns A promise resolving to the remote data payload.
 */
export declare function getRemoteDataAsync<TData = any>(key: string): Promise<TData>;
/**
 * Synchronous version of {@link getRemoteDataAsync} for legacy compatibility. Blocks the UI thread.
 * @typeParam TData - Expected payload type.
 * @param key - Remote data key.
 * @returns The remote data payload.
 */
export declare function getRemoteData<TData = any>(key: string): TData;
/**
 * Shows a suitable error message for errors occured during loading of
 * a dynamic script data.
 * @param name Name of the dynamic script
 * @param status HTTP status returned if available
 * @param statusText HTTP status text returned if available
 */
export declare function handleScriptDataError(name: string, status?: number, statusText?: string, shouldThrow?: boolean): string;
/**
 * Returns cached script data without triggering a fetch.
 * @param name - Dynamic script name.
 * @returns The cached value or `undefined` if not loaded yet.
 */
export declare function peekScriptData(name: string): any;
/**
 * Forces a reload of a lookup from the server, bypassing the client-side cache.
 * Note this only clears the browser cache entry; it does not invalidate server-side caches.
 * @typeParam TItem - Row type of the lookup items.
 * @param key - Lookup key to reload.
 * @returns A promise resolving to the freshly loaded {@link Lookup}.
 */
export declare function reloadLookupAsync<TItem = any>(key: string): Promise<Lookup<TItem>>;
/**
 * Seeds or updates the known script hashes (normally populated from the `RegisteredScripts` script tag).
 * Useful in tests or when bootstrapping hashes manually.
 * @param scripts - Map of script name to hash string. Falsy hash values are replaced with the current timestamp.
 */
export declare function setRegisteredScripts(scripts: Record<string, string>): void;
/**
 * Stores a script data value in the in-memory cache and dispatches a `scriptdatachange.<name>` DOM event.
 * @param name - Dynamic script name.
 * @param value - Value to cache (use `undefined` to clear).
 */
export declare function setScriptData(name: string, value: any): void;
/**
 * Resolves a `~/`-prefixed application-relative URL against {@link Config.applicationPath}.
 * Non-tilde URLs are returned unchanged.
 * @param url - URL to resolve; may be `null`/`undefined` or already absolute.
 * @returns The resolved absolute / root-relative URL.
 */
export declare function resolveUrl(url: string): string;
/**
 * Resolves a Serenity service endpoint to a full URL.
 * Bare service keys like `"Administration/User/List"` are prefixed with `~/Services/`;
 * already rooted (`~/`, `/`) or absolute (`://`) URLs are resolved via {@link resolveUrl} unchanged.
 * @param url - Service key or URL.
 * @returns The resolved service URL.
 */
export declare function resolveServiceUrl(url: string): string;
/**
 * Reads a cookie value by name.
 * Prefers jQuery's `$.cookie` when available, otherwise parses `document.cookie`.
 * @param name - Cookie name to look up.
 * @returns The cookie value, or `undefined` / empty string when not found.
 */
export declare function getCookie(name: string): any;
/**
 * Checks whether a URL is same-origin with the current page.
 * Used to decide whether to attach the `X-CSRF-TOKEN` header.
 * @param url - URL to test (absolute or relative; relative URLs are resolved against `window.location.origin`).
 * @returns `true` if the URL shares hostname, port and protocol with `window.location`.
 */
export declare function isSameOrigin(url: string): boolean;
/**
 * Normalizes and enriches a {@link ServiceOptions} object with defaults and derived values.
 * Applies default `method` (`POST`), `allowRedirect`/`async`/`blockUI` flags, resolves `service`/`url`,
 * and injects `Accept`, `Content-Type` and same-origin `X-CSRF-TOKEN` headers.
 * @typeParam TResponse - Expected service response type.
 * @param options - Raw service options supplied by the caller.
 * @returns The normalized options object with `url` resolved and `headers` populated.
 */
export declare function getServiceOptions<TResponse extends ServiceResponse>(options: ServiceOptions<TResponse>): ServiceOptions<TResponse>;
/**
 * Signals that an AJAX / service request has started.
 * Increments the internal active-request counter and triggers `ajaxStart` on jQuery (if present) or dispatches an `ajaxStart` DOM event. Used internally by `serviceFetch` / `serviceCall` and script-data loaders.
 */
export declare function requestStarting(): void;
/**
 * Signals that an AJAX / service request has finished.
 * Decrements the active-request counter and triggers `ajaxStop` when the count reaches zero.
 */
export declare function requestFinished(): void;
/**
 * Returns the number of currently active service / AJAX requests.
 * @returns Active request count (0 when idle).
 */
export declare function getActiveRequests(): number;
/**
 * Executes a Serenity service call.
 * Prefers `fetch` (async) when `options.async` is `true` (default); falls back to synchronous `XMLHttpRequest` when `async` is `false` (blocks the UI — avoid in new code).
 * Handles CSRF headers, `blockUI`, redirect handling, and delegates error display to the global error handler unless suppressed via `errorMode` or `onError`.
 * @typeParam TResponse - Service response type (extends {@link ServiceResponse}).
 * @param options - Service options including `service`/`url`, `request` payload and callbacks.
 * @returns A `PromiseLike` that resolves with the parsed response on success or rejects with an enriched `Error` (with `kind` and `origin === "serviceCall"`) on failure.
 */
export declare function serviceCall<TResponse extends ServiceResponse>(options: ServiceOptions<TResponse>): PromiseLike<TResponse>;
/**
 * Convenience wrapper around {@link serviceCall} that takes a service key as the first argument.
 * @typeParam TResponse - Service response type.
 * @param service - Service endpoint key (e.g. `"Administration/User/List"`) or full URL.
 * @param request - Request DTO serialized as the POST body.
 * @param onSuccess - Optional success callback invoked with the response before the promise resolves.
 * @param options - Additional {@link ServiceOptions} merged with the above (e.g. `errorMode`, `blockUI`, `signal`).
 * @returns A `PromiseLike` resolving to the service response.
 */
export declare function serviceRequest<TResponse extends ServiceResponse>(service: string, request?: any, onSuccess?: (response: TResponse) => void, options?: ServiceOptions<TResponse>): PromiseLike<TResponse>;
export type StringLiteral<T> = T extends string ? string extends T ? never : T : never;
/**
 * Type information for a registered type.
 */
export type TypeInfo<TypeName> = {
	/** Type kind, can be "class", "enum", "interface" */
	typeKind: "class" | "enum" | "interface";
	/** Registered type name */
	typeName: StringLiteral<TypeName> | (string & {});
	/** Implemented interfaces */
	interfaces?: any[];
	/** Custom attributes */
	customAttributes?: CustomAttribute[];
	/** Enum flags */
	enumFlags?: boolean;
	/** Registered flag */
	registered?: boolean;
};
export declare function getGlobalTypeRegistry(): {
	[key: string]: any;
};
export declare function ensureTypeInfo(type: any): TypeInfo<string>;
export declare function peekTypeInfo(type: any): TypeInfo<string>;
export declare function getTypeNameProp(type: any): string;
export declare function setTypeNameProp(type: any, value: string): void;
export declare const isAddRowSymbol: unique symbol;
/**
 * @deprecated Use {@link getGlobalTypeRegistry} instead. Kept for backward compatibility.
 */
export declare const getTypeRegistry: typeof getGlobalTypeRegistry;
/** Namespace prefix `"Serenity."` used when registering types with a fully-qualified name. */
export declare const nsSerenity: "Serenity.";
/** Root namespace `"Serenity"` without trailing dot. */
export declare const SerenityNS: "Serenity";
declare global {
	interface SymbolConstructor {
		readonly typeInfo: unique symbol;
	}
}
/**
 * Get the global object  (window in browsers, global in node)
 */
export declare function getGlobalObject(): any;
/**
 * Omit undefined properties from an object. Does not modify the original object.
 * This is useful when using Object.assign to avoid overwriting existing values with undefined
 * just like jQuery $.extend does.
 * @param x Object to omit undefined properties from
 * @returns New object without undefined properties
 */
export declare function omitUndefined(x: {
	[key: string]: any;
}): any;
/**
 * Type alias for any runtime type representation.
 * In Serenity this is either a constructor function (class) or a plain object (enum).
 */
export type Type = Function | Object;
/**
 * Get a nested property from an object. Can be used to get nested properties from global object for example by separating names with dots.
 * @param from Object to get the property from
 * @param name Name of the property (dot-separated for nested properties)
 * @returns Value of the property or null if not found
 */
export declare function getNested(from: any, name: string): any;
/**
 * Get a type by name from the type registry, global object or a specific target.
 * @param name Name of the type
 * @param target Target object to search in (defaults to global object)
 * @returns The type or null if not found
 */
export declare function getType(name: string, target?: any): Type;
/**
 * Get the full name of a type (including namespace if any).
 * This returns the name from typeInfo.typeName if available (e.g. registered via decorators),
 * otherwise tries to get the name from function's name property.
 * @param type Type to get the name of
 */
export declare function getTypeFullName(type: Type): string;
/**
 * Get the short name of a type (without namespace).
 * @param type Type to get the name of
 * @returns Short name of the type
 */
export declare function getTypeShortName(type: Type): string;
/**
 * Get the instance type of an object.
 * @param instance Object to get the instance type of
 * @returns The instance type or Object if not found
 */
export declare function getInstanceType(instance: any): any;
/**
 * Check if a type is assignable from another type. A type is
 * assignable from another type if they are the same or if the other type
 * is derived from it. This also works for interfaces if they are registered
 * via registerInterface function or decorators.
 * @param target Target type or interface
 * @param fromType Type to check assignability from
 * @returns true if target is assignable from type
 */
export declare function isAssignableFrom(target: any, fromType: Type): any;
/**
 * Check if an instance is of a specific type.
 * @param instance Object to check
 * @param type Type to check against
 * @returns true if instance is of type
 */
export declare function isInstanceOfType(instance: any, type: Type): any;
/**
 * Get the base type of a class or interface.
 * @param type Type to get the base type of
 * @returns The base type or null if not found
 */
export declare function getBaseType(type: any): any;
/**
 * Register a class with the type system.
 * @param type Class type to register
 * @param name Name to register the class under
 * @param intfAndAttr Optional interfaces and attributes the class implements
 */
export declare function registerClass(type: any, name: string, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): void;
/**
 * Base class for all Serenity custom attributes (metadata attached to types).
 * Attributes are stored on `typeInfo.customAttributes` and queried via
 * {@link getCustomAttribute} / {@link hasCustomAttribute}.
 */
export declare abstract class CustomAttribute {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private readonly isCustomAttribute;
}
/**
 * Attribute that overrides the lookup key under which an enum is registered in the global type registry.
 * By default the enum's full name is used as the key; this attribute allows an alternative key.
 */
export declare class EnumKeyAttribute extends CustomAttribute {
	value: string;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates a new enum-key attribute.
	 * @param value - Alternative registry key for the enum (e.g. `"MyApp.MyEnum"`).
	 */
	constructor(value: string);
}
/**
 * Register an enum with the type system.
 * @param enumType Enum type to register
 * @param name Name to register the enum under
 * @param enumKey Optional key to use for the enum
 */
export declare function registerEnum(enumType: any, name: string, enumKey?: string): void;
/**
 * Register an interface with the type system. There is no runtime representation of interfaces
 * in JavaScript, so Serenity uses classes decorated with some special symbols to emulate
 * interfaces to some degree. This is used by the type system to support isAssignableFrom and
 * isInstanceOfType functions for interfaces.
 * @param type Interface type to register
 * @param name Name to register the interface under
 * @param intf Optional interfaces the interface class implements
 */
export declare function registerInterface(type: any, name: string, intf?: InterfaceType[]): void;
/**
 * Enum utilities
 */
export declare const Enum: {
	/**
	 * Convert an enum value to a string containing enum names.
	 * @param enumType Enum type
	 * @param value Enum value
	 */
	toString: (enumType: any, value: number) => string;
	/**
	 * Returns all numeric values of an enum as an array.
	 * @param enumType - Enum object to enumerate.
	 * @returns Array of numeric enum values.
	 */
	getValues: (enumType: any) => number[];
};
/**
 * Check if a type is an enum. A type is considered an enum if it is not a function
 * and it's [Symbol.typeInfo].typeKind is "enum".
 * @param type Type to check
 * @returns True if the type is an enum
 */
export declare const isEnum: (type: any) => boolean;
/**
 * Initialize a form type. This is used in the XYZForm.ts files that are generated
 * by the Serenity server typings code generator. It defines getters that call this.w() to
 * initialize form fields on the prototype of a form class.
 * @param typ Form type to initialize
 * @param nameWidgetPairs Array of name-widget pairs
 */
export declare function initFormType(typ: Function, nameWidgetPairs: any[]): void;
/**
 * Get a proxy for form fields. This proxy returns the field name for any property
 * accessed on it. This is used in form initialization to avoid having to declare
 * a variable for the fields type. There is no actual runtime check for field names,
 * so it is only used to provide intellisense and compile-time checks.
 * @returns A readonly record of form field names and same string values
 */
export declare function fieldsProxy<TRow>(): Readonly<Record<keyof TRow, string>>;
/**
 * Check if an object is array-like. An object is considered array-like if it is
 * not null, is of type object, has a numeric length property and does not have
 * a nodeType property (to exclude DOM nodes).
 * @param obj Object to check
 * @returns True if the object is array-like
 */
export declare function isArrayLike(obj: any): obj is ArrayLike<any>;
/**
 * Check if an object is Promise-like, meaning it is either a native Promise
 * or an object with then and catch methods (like jQuery Deferred).
 * @param obj Object to check
 * @returns True if the object is Promise-like
 */
export declare function isPromiseLike(obj: any): obj is PromiseLike<any>;
/**
 * Utility type that prevents TypeScript from inferring `T` from a specific position.
 * Prefers the inferred type from other positions. TypeScript 5.4+ provides a built-in `NoInfer<T>` that can be used instead.
 * @typeParam T - Type to block inference for.
 */
export type SNoInfer<T> = [
	T
][T extends any ? 0 : never];
/**
 * Attribute that marks a class as a Serenity editor.
 * Added automatically by {@link registerEditor} / {@link editorTypeInfo}. Can also be applied manually via `classTypeInfo`.
 */
export declare class EditorAttribute extends CustomAttribute {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
}
/**
 * Marker interface for SleekGrid / DataGrid formatters.
 * Formatters implementing this interface declare a `format(ctx)` method and are registered via {@link registerFormatter} / {@link formatterTypeInfo}.
 */
export declare abstract class ISlickFormatter {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
/**
 * Register a SleekGrid formatter.
 * @param type Formatter type
 * @param name Formatter name
 * @param intfAndAttr Optional attributes and interface(s) to implement
 */
export declare function registerFormatter(type: any, name: string, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): void;
/**
 * Register an editor type. Adds EditorAttribute if not already present.
 * @param type Editor type
 * @param name Editor name
 * @param intfAndAttr Optional attributes and interface(s) to implement
 */
export declare function registerEditor(type: any, name: string, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): void;
/**
 * Attaches a custom attribute instance to a type's metadata.
 * JavaScript has no native attribute support, so Serenity stores attributes on `typeInfo.customAttributes`.
 * @param type - Target type (class / enum object) to attach the attribute to.
 * @param attr - Attribute instance to add.
 */
export declare function addCustomAttribute(type: any, attr: CustomAttribute): void;
/**
 * Get a custom attribute of a type.
 * @param type Type to get the attribute from
 * @param attrType Attribute type to get
 * @param inherit Indicates whether to search in base types
 * @returns The custom attribute or null if not found
 */
export declare function getCustomAttribute<TAttr extends CustomAttribute>(type: any, attrType: {
	new (...args: any[]): TAttr;
}, inherit?: boolean): TAttr;
/**
 * Get whether a type has a specific custom attribute.
 * @param type Type to check
 * @param attrType Attribute type to check
 * @param inherit Indicates whether to search in base types
 * @returns True if the type has the attribute
 */
export declare function hasCustomAttribute<TAttr extends CustomAttribute>(type: any, attrType: {
	new (...args: any[]): TAttr;
}, inherit?: boolean): boolean;
/**
 * Get all custom attributes of a type.
 * @param type Type to get the attributes from
 * @param attrType Attribute type to get. If not specified, all attributes are returned.
 * @param inherit Indicates whether to search in base types
 * @returns An array of custom attributes
 */
export declare function getCustomAttributes<TAttr>(type: any, attrType: {
	new (...args: any[]): TAttr;
}, inherit?: boolean): TAttr[];
/**
 * TypeInfo for a class. Used with `static override [Symbol.typeInfo] = classTypeInfo("...")` to embed the type name in declaration files (decorators are erased in `.d.ts`).
 * This is one of the helper types that are used to make the type name available in declaration files, unlike decorators that does not show in .d.ts files.
 * @typeParam TypeName - String-literal type of the fully-qualified class name.
 */
export type ClassTypeInfo<TypeName> = TypeInfo<TypeName>;
/**
 * TypeInfo for an editor class. Like {@link ClassTypeInfo} but automatically includes {@link EditorAttribute}.
 * This is one of the helper types that are used to make the type name available in declaration files, unlike decorators that does not show in .d.ts files.
 * @typeParam TypeName - String-literal editor type name.
 */
export type EditorTypeInfo<TypeName> = TypeInfo<TypeName>;
/**
 * TypeInfo for a formatter class. Like {@link ClassTypeInfo} but automatically includes {@link ISlickFormatter}.
 * This is one of the helper types that are used to make the type name available in declaration files, unlike decorators that does not show in .d.ts files.
 * @typeParam TypeName - String-literal formatter type name.
 */
export type FormatterTypeInfo<TypeName> = TypeInfo<TypeName>;
/**
 * TypeInfo for an interface. Used with `static [Symbol.typeInfo] = interfaceTypeInfo("...")`.
 * This is one of the helper types that are used to make the type name available in declaration files, unlike decorators that does not show in .d.ts files.
 * @typeParam TypeName - String-literal interface name.
 */
export type InterfaceTypeInfo<TypeName> = TypeInfo<TypeName>;
/**
 * Union of forms accepted where an attribute can be specified: an attribute instance, an attribute class (instantiated with `new`), or a factory function returning an attribute. Factories are marked with `isAttributeFactory === true`.
 */
export type AttributeSpecifier = CustomAttribute | ({
	new (): CustomAttribute;
}) | (() => CustomAttribute);
/** Interface type — a constructor function carrying an {@link InterfaceTypeInfo} via `[Symbol.typeInfo]`. */
export type InterfaceType = Function & {
	[Symbol.typeInfo]: InterfaceTypeInfo<string>;
};
/**
 * Creates {@link ClassTypeInfo} for a class. Use as `static override [Symbol.typeInfo] = classTypeInfo("MyApp.MyClass")`.
 * @typeParam TypeName - String-literal fully-qualified type name.
 * @param typeName - Fully-qualified type name (e.g. `"MyApp.MyClass"`).
 * @param intfAndAttr - Optional interfaces and attributes the class implements / carries.
 * @returns A {@link ClassTypeInfo} object to assign to `[Symbol.typeInfo]`.
 */
export declare function classTypeInfo<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): ClassTypeInfo<TypeName>;
/**
 * Creates {@link EditorTypeInfo} for an editor class. Like {@link classTypeInfo} but automatically adds {@link EditorAttribute}.
 * @typeParam TypeName - String-literal editor type name.
 * @param typeName - Fully-qualified editor name (e.g. `"MyApp.MyEditor"`).
 * @param intfAndAttr - Optional interfaces and extra attributes.
 * @returns An {@link EditorTypeInfo} to assign to `[Symbol.typeInfo]`.
 */
export declare function editorTypeInfo<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): EditorTypeInfo<TypeName>;
/**
 * Creates {@link FormatterTypeInfo} for a formatter class. Automatically includes {@link ISlickFormatter}.
 * @typeParam TypeName - String-literal formatter type name.
 * @param typeName - Fully-qualified formatter name (e.g. `"MyApp.MyFormatter"`).
 * @param intfAndAttr - Optional interfaces and attributes.
 * @returns A {@link FormatterTypeInfo} to assign to `[Symbol.typeInfo]`.
 */
export declare function formatterTypeInfo<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): FormatterTypeInfo<TypeName>;
/**
 * Creates {@link InterfaceTypeInfo} for an interface.
 * @typeParam TypeName - String-literal interface name.
 * @param typeName - Fully-qualified interface name (e.g. `"MyApp.IMyInterface"`).
 * @param intf - Optional base interfaces this interface extends.
 * @returns An {@link InterfaceTypeInfo} to assign to `[Symbol.typeInfo]`.
 */
export declare function interfaceTypeInfo<TypeName>(typeName: StringLiteral<TypeName>, intf?: InterfaceType[]): InterfaceTypeInfo<TypeName>;
/**
 * Registers a type that already has a `static [Symbol.typeInfo]` declaration.
 * Called automatically by the `static { registerType(this); }` block that follows the typeInfo declaration.
 * Validates that the typeInfo exists and has a `typeName`.
 * @param type - Class / interface object carrying `[Symbol.typeInfo]` and a `name` property.
 * @throws If `type` is null, lacks `[Symbol.typeInfo]`, or its `typeName` is empty.
 */
export declare function registerType(type: {
	[Symbol.typeInfo]: TypeInfo<any>;
	name: string;
}): void;
/**
 * Marker interface used to include column transforms in generated row metadata.
 * Implementations are generated server-side; this empty interface exists for typing only.
 */
export interface TransformInclude {
}
declare namespace servicesTexts {
	namespace Controls {
		function asKey(): typeof Controls;
		function asTry(): typeof Controls;
		namespace ImageUpload {
			function asKey(): typeof ImageUpload;
			function asTry(): typeof ImageUpload;
			const AddFileButton: string;
			const ColorboxClose: string;
			const ColorboxCurrent: string;
			const ColorboxNext: string;
			const ColorboxPrior: string;
			const DeleteButtonHint: string;
			const ExtensionBlacklisted: string;
			const ExtensionNotAllowed: string;
			const FailedScan: string;
			const ImageExtensionMismatch: string;
			const InfectedFile: string;
			const InfectedFileOrError: string;
			const MaxHeight: string;
			const MaxWidth: string;
			const MinHeight: string;
			const MinWidth: string;
			const NotAnImageFile: string;
			const NotAnImageWithExtensions: string;
			const UploadFileTooBig: string;
			const UploadFileTooSmall: string;
		}
	}
	namespace Enums {
		function asKey(): typeof Enums;
		function asTry(): typeof Enums;
		namespace ImageCheckResult {
			function asKey(): typeof ImageCheckResult;
			function asTry(): typeof ImageCheckResult;
			const DataSizeTooHigh: string;
			const HeightMismatch: string;
			const HeightTooHigh: string;
			const HeightTooLow: string;
			const ImageIsEmpty: string;
			const InvalidImage: string;
			const SizeMismatch: string;
			const StreamReadError: string;
			const UnsupportedFormat: string;
			const WidthMismatch: string;
			const WidthTooHigh: string;
			const WidthTooLow: string;
		}
	}
	namespace Validation {
		function asKey(): typeof Validation;
		function asTry(): typeof Validation;
		const ArgumentIsNull: string;
		const ArgumentOutOfRange: string;
		const EntityForeignKeyViolation: string;
		const EntityHasDeletedParent: string;
		const EntityIsNotActive: string;
		const EntityNotFound: string;
		const EntityReadAccessViolation: string;
		const EntityWriteAccessViolation: string;
		const FieldInvalidDateRange: string;
		const FieldInvalidValue: string;
		const FieldIsReadOnly: string;
		const FieldIsRequired: string;
		const Recaptcha: string;
		const RequestIsNull: string;
		const UnexpectedError: string;
	}
}
export declare const DataValidationTexts: typeof servicesTexts.Validation;
export declare const FileUploadTexts: typeof servicesTexts.Controls.ImageUpload;
export declare const ImageCheckResultTexts: typeof servicesTexts.Enums.ImageCheckResult;
declare namespace webTexts {
	namespace Controls {
		function asKey(): typeof Controls;
		function asTry(): typeof Controls;
		namespace CheckTreeEditor {
			function asKey(): typeof CheckTreeEditor;
			function asTry(): typeof CheckTreeEditor;
			const SelectAll: string;
		}
		namespace ColumnPickerDialog {
			function asKey(): typeof ColumnPickerDialog;
			function asTry(): typeof ColumnPickerDialog;
			const HiddenColumns: string;
			const HideHint: string;
			const RestoreDefaults: string;
			const ShowHint: string;
			const Title: string;
			const VisibleColumns: string;
		}
		namespace DataGrid {
			function asKey(): typeof DataGrid;
			function asTry(): typeof DataGrid;
			const NewButton: string;
		}
		namespace DateTimeEditor {
			function asKey(): typeof DateTimeEditor;
			function asTry(): typeof DateTimeEditor;
			const SetToNow: string;
		}
		namespace EntityDialog {
			function asKey(): typeof EntityDialog;
			function asTry(): typeof EntityDialog;
			const ApplyChangesButton: string;
			const CloneButton: string;
			const DeleteButton: string;
			const DeleteConfirmation: string;
			const EditButton: string;
			const EditRecordTitle: string;
			const LocalizationBack: string;
			const LocalizationButton: string;
			const LocalizationConfirmation: string;
			const NewRecordTitle: string;
			const SaveButton: string;
			const SaveSuccessMessage: string;
			const UndeleteButton: string;
			const UndeleteConfirmation: string;
			const UpdateButton: string;
			const ViewRecordTitle: string;
		}
		namespace EntityGrid {
			function asKey(): typeof EntityGrid;
			function asTry(): typeof EntityGrid;
			const IncludeDeletedToggle: string;
			const NewButton: string;
			const RefreshButton: string;
			const RowEditActionsTitle: string;
			const SaveChangesButton: string;
			const UndoChangesButton: string;
		}
		namespace FilterPanel {
			function asKey(): typeof FilterPanel;
			function asTry(): typeof FilterPanel;
			const AddFilter: string;
			const All: string;
			const And: string;
			const AndInParens: string;
			const ApplyGroups: string;
			const ChangeAndOr: string;
			const ClearGroups: string;
			const CurrentFilter: string;
			const DialogTitle: string;
			const EditFilter: string;
			const EffectiveEmpty: string;
			const EffectiveFilter: string;
			const FixErrorsMessage: string;
			const GroupBy: string;
			const InvalidDate: string;
			const InvalidNumber: string;
			const InvalidOperator: string;
			namespace OperatorFormats {
				function asKey(): typeof OperatorFormats;
				function asTry(): typeof OperatorFormats;
				const bw: string;
				const contains: string;
				const eq: string;
				const ge: string;
				const gt: string;
				const isnotnull: string;
				const isnull: string;
				const le: string;
				const lt: string;
				const ne: string;
				const startswith: string;
			}
			namespace OperatorNames {
				function asKey(): typeof OperatorNames;
				function asTry(): typeof OperatorNames;
				const bw: string;
				const contains: string;
				const eq: string;
				const ge: string;
				const gt: string;
				const isnotnull: string;
				const isnull: string;
				const le: string;
				const lt: string;
				const ne: string;
				const startswith: string;
			}
			const Or: string;
			const OrInParens: string;
			const RemoveField: string;
			const ResetButton: string;
			const ResetFilterHint: string;
			const SearchButton: string;
			const SelectField: string;
			const ThenBy: string;
			const ValueRequired: string;
		}
		namespace HtmlContentEditor {
			function asKey(): typeof HtmlContentEditor;
			function asTry(): typeof HtmlContentEditor;
			const AlignCenter: string;
			const AlignJustify: string;
			const AlignLeft: string;
			const AlignRight: string;
			const BackgroundColor: string;
			const Bold: string;
			const Copy: string;
			const Cut: string;
			const FindAndReplace: string;
			const Font: string;
			const FontSize: string;
			const Format: string;
			const HeadingLevel: string;
			const HorizontalRule: string;
			const Hyperlink: string;
			const Indent: string;
			const InlineCode: string;
			const InsertImage: string;
			const InsertTable: string;
			const Italic: string;
			const Maximize: string;
			const NormalText: string;
			const OrderedList: string;
			const Outdent: string;
			const Paste: string;
			const PasteAsPlainText: string;
			const PasteFromWord: string;
			const Redo: string;
			const RemoveFormat: string;
			const RemoveHeading: string;
			const RemoveHyperlink: string;
			const StrikeThrough: string;
			const Subscript: string;
			const Superscript: string;
			const TextColor: string;
			const ToggleHeading: string;
			const Underline: string;
			const Undo: string;
			const UnorderedList: string;
			const ViewSource: string;
		}
		namespace Pager {
			function asKey(): typeof Pager;
			function asTry(): typeof Pager;
			const DefaultLoadError: string;
			const LoadingStatus: string;
			const NoRowStatus: string;
			const Page: string;
			const PageStatus: string;
		}
		namespace PropertyGrid {
			function asKey(): typeof PropertyGrid;
			function asTry(): typeof PropertyGrid;
			const RequiredHint: string;
		}
		namespace QuickSearch {
			function asKey(): typeof QuickSearch;
			function asTry(): typeof QuickSearch;
			const FieldSelection: string;
			const Hint: string;
			const Placeholder: string;
		}
		namespace SelectEditor {
			function asKey(): typeof SelectEditor;
			function asTry(): typeof SelectEditor;
			const AjaxError: string;
			const ClickToDefine: string;
			const EmptyItemText: string;
			const InplaceAdd: string;
			const InplaceEdit: string;
			const InputTooLong: string;
			const InputTooShort: string;
			const LoadMore: string;
			const MultipleMatches: string;
			const NoMatches: string;
			const NoResultsClickToDefine: string;
			const Searching: string;
			const SelectionTooBig: string;
			const SingleMatch: string;
		}
	}
	namespace Dialogs {
		function asKey(): typeof Dialogs;
		function asTry(): typeof Dialogs;
		const AlertTitle: string;
		const CancelButton: string;
		const ConfirmationTitle: string;
		const InformationTitle: string;
		const MaximizeHint: string;
		const NoButton: string;
		const OkButton: string;
		const Prompt: string;
		const RestoreHint: string;
		const SuccessTitle: string;
		const WarningTitle: string;
		const YesButton: string;
	}
	namespace Validation {
		function asKey(): typeof Validation;
		function asTry(): typeof Validation;
		const CaptchaMismatch: string;
		const DateInvalid: string;
		const DayHourAndMin: string;
		const Decimal: string;
		const Digits: string;
		const Email: string;
		const EmailExists: string;
		const EmailMultiple: string;
		const HourAndMin: string;
		const IncorrectPassword: string;
		const Integer: string;
		const InvalidFormMessage: string;
		const MaxDate: string;
		const MaxLength: string;
		const MinDate: string;
		const MinLength: string;
		const PasswordConfirm: string;
		const Range: string;
		const Required: string;
		const UniqueConstraint: string;
		const Url: string;
		const Username: string;
		const UsernameExists: string;
		const Xss: string;
	}
}
export declare const CheckTreeEditorTexts: typeof webTexts.Controls.CheckTreeEditor;
export declare const ColumnPickerDialogTexts: typeof webTexts.Controls.ColumnPickerDialog;
export declare const DataGridTexts: typeof webTexts.Controls.DataGrid;
export declare const DateTimeEditorTexts: typeof webTexts.Controls.DateTimeEditor;
export declare const EntityDialogTexts: typeof webTexts.Controls.EntityDialog;
export declare const EntityGridTexts: typeof webTexts.Controls.EntityGrid;
export declare const FilterPanelTexts: typeof webTexts.Controls.FilterPanel;
export declare const FormValidationTexts: typeof webTexts.Validation;
export declare const HtmlContentEditorTexts: typeof webTexts.Controls.HtmlContentEditor;
export declare const PagerTexts: typeof webTexts.Controls.Pager;
export declare const PropertyGridTexts: typeof webTexts.Controls.PropertyGrid;
export declare const QuickSearchTexts: typeof webTexts.Controls.QuickSearch;
export declare const SelectEditorTexts: typeof webTexts.Controls.SelectEditor;
/**
 * Options for initializing a Bootstrap/jQuery tooltip.
 */
export interface TooltipOptions {
	/** Text shown inside the tooltip. */
	title?: string;
	/** Trigger events (e.g. `"hover focus"`, `"click"`). Defaults vary by implementation. */
	trigger?: string;
}
/**
 * Thin wrapper around Bootstrap / jQuery tooltip plugins with a fallback to the native
 * `title` attribute. Handles instance reuse, cleanup, and title updates.
 */
export declare class Tooltip {
	private el;
	/**
	 * Creates or wraps a tooltip for an element.
	 * @param el - Target element or array-like collection (first element is used).
	 * @param opt - Tooltip options; if omitted defaults are applied.
	 * @param create - When `true` (default) creates a tooltip if none exists; when `false` only wraps an existing instance.
	 */
	constructor(el: ArrayLike<HTMLElement> | HTMLElement, opt?: TooltipOptions);
	/** Default options applied when none are supplied. */
	static defaults: TooltipOptions;
	/**
	 * Disposes the underlying tooltip instance and clears internal Bootstrap state.
	 */
	dispose(): void;
	/**
	 * Disposes the tooltip after a delay.
	 * @param delay - Delay in milliseconds before disposing. Defaults to `5000`.
	 */
	delayedDispose(delay?: number): void;
	/**
	 * Hides the tooltip after a delay.
	 * @param delay - Delay in milliseconds before hiding. Defaults to `5000`.
	 */
	delayedHide(delay?: number): void;
	private static existingInstance;
	/**
	 * Gets the existing tooltip wrapper for an element, if any.
	 * @param el - Target element or array-like collection.
	 * @returns A `Tooltip` wrapper around the existing instance, or `null` if none exists.
	 */
	static getInstance(el: ArrayLike<HTMLElement> | HTMLElement): Tooltip;
	/**
	 * Whether a tooltip implementation (Bootstrap or jQuery) is available in the current environment.
	 */
	static get isAvailable(): boolean;
	/**
	 * Updates the tooltip title text and synchronizes it with the underlying implementation.
	 * @param value - New title text.
	 * @returns This instance for chaining.
	 */
	setTitle(value: string): Tooltip;
	/**
	 * Shows or hides the tooltip.
	 * @param show - `true` to show, `false` to hide.
	 * @returns This instance for chaining.
	 */
	toggle(show: boolean): Tooltip;
	/**
	 * Hides the tooltip.
	 * @returns This instance for chaining.
	 */
	hide(): Tooltip;
	/**
	 * Shows the tooltip.
	 * @returns This instance for chaining.
	 */
	show(): Tooltip;
}
/** Inspired from https://github.com/silverwind/uppie and https://github.com/GoogleChromeLabs/file-drop/blob/master/lib/filedrop.ts */
/**
 * Options controlling file selection, drag-and-drop, batching, and event callbacks for {@link Uploader}.
 */
export interface UploaderOptions {
	/** MIME / extension filter (e.g. `"image/*,.pdf"`). Falls back to the `accept` attribute of {@link UploaderOptions.input} when omitted. */
	accept?: string;
	/** When `true` (default) clears the input value after handling the change event so re-selecting the same file re-triggers the handler. */
	autoClear?: boolean;
	/** Number of files per batch when `multiple` is enabled. Defaults to `1`. Larger values upload files in groups. */
	batchSize?: number;
	/** One or more elements that act as drag-and-drop targets. */
	dropZone?: HTMLElement | ArrayLike<HTMLElement>;
	/** Called once before the first batch starts uploading. */
	allStart?: () => void;
	/** Called once after the last batch completes or fails. */
	allStop?: () => void;
	/** Called when an individual batch is about to be uploaded. */
	batchStart?: (data: {
		batch: UploaderBatch;
	}) => void;
	/** Called when an individual batch finishes uploading or fails. */
	batchStop?: (data: {
		batch: UploaderBatch;
	}) => void;
	/** Called after a batch uploads successfully. */
	batchSuccess?: (data: UploaderSuccessData) => void;
	/** Called periodically with upload progress for the current batch. */
	batchProgress?: (data: {
		batch: UploaderBatch;
		loaded: number;
		total: number;
	}) => void;
	/** Custom handler for uploading a batch. When omitted {@link Uploader.uploadBatch} is used. */
	batchHandler?: (batch: UploaderBatch, uploader: Uploader) => void | Promise<void>;
	/** Called when a change / drop / paste event occurs but no files could be resolved. */
	changeCallback?: (e: Event) => void;
	/** Error handler for upload failures. Defaults to {@link Uploader.errorHandler}. */
	errorHandler?: (data: UploaderErrorData) => void;
	/** When `true` disables MIME-type filtering against `accept`. */
	ignoreType?: boolean;
	/** File input that triggers selection. When `null`, {@link UploaderOptions.dropZone} must be provided. */
	input?: HTMLInputElement;
	/** Allows multiple file selection. Falls back to the `multiple` attribute of {@link UploaderOptions.input} when omitted. */
	multiple?: boolean;
	/** Form field name used when appending files to `FormData`. Defaults to `"files[]"`. */
	name?: string;
}
/**
 * Request configuration for {@link Uploader.uploadBatch}.
 */
export interface UploaderRequest {
	/** Extra headers to send with the upload request. */
	headers?: Record<string, string>;
	/** Expected response type. Defaults to `"json"`. */
	responseType?: "json" | "text";
	/** Endpoint URL for the upload. Defaults to `~/File/TemporaryUpload`. */
	url?: string;
}
/**
 * Represents a single upload batch queued by {@link Uploader}.
 */
export interface UploaderBatch {
	/** Originating DOM event (change / drop / paste). */
	event?: Event;
	/** Relative paths / names of files in this batch. */
	filePaths?: string[];
	/** `FormData` payload containing the batched files. */
	formData: FormData;
	/** `true` for the first batch in a multi-batch sequence. */
	isFirst?: boolean;
}
/**
 * Data passed to {@link UploaderOptions.batchSuccess} after a successful upload.
 */
export interface UploaderSuccessData {
	/** The batch that was uploaded. */
	batch: UploaderBatch;
	/** Request configuration used for the upload. */
	request: UploaderRequest;
	/** XHR load event. */
	event: ProgressEvent;
	/** The underlying `XMLHttpRequest`. */
	xhr: XMLHttpRequest;
	/** Parsed response body (JSON or text depending on {@link UploaderRequest.responseType}). */
	response: any;
}
/**
 * Data passed to error handlers when an upload fails.
 */
export interface UploaderErrorData {
	/** The batch that failed, if available. */
	batch?: UploaderBatch;
	/** XHR progress / error event, if available. */
	event?: ProgressEvent;
	/** Exception thrown during setup or handling, if any. */
	exception?: any;
	/** Request configuration used for the failed attempt. */
	request?: UploaderRequest;
	/** Parsed response body, if available. */
	response?: any;
	/** The underlying `XMLHttpRequest`, if available. */
	xhr?: XMLHttpRequest;
}
/**
 * File uploader that handles `input` change, drag-and-drop, paste, and directory
 * traversal, batching files and uploading each batch via `XMLHttpRequest`.
 * Supports MIME filtering, progress events, CSRF headers, and custom batch handling.
 */
export declare class Uploader {
	private opt;
	private batch;
	/**
	 * Creates a new uploader and wires up the configured input and drop zones.
	 * @param opt - Uploader configuration; defaults from {@link Uploader.defaults} are applied.
	 */
	constructor(opt: UploaderOptions);
	private newBatch;
	private addToBatch;
	private endBatch;
	/** Default {@link UploaderOptions} applied when constructing an instance. */
	static defaults: Partial<UploaderOptions>;
	/** Default {@link UploaderRequest} applied when {@link Uploader.uploadBatch} is called without explicit request options. */
	static requestDefaults: Partial<UploaderRequest>;
	/**
	 * Whether the uploader is configured for multiple file selection.
	 * @returns `true` if multiple files are allowed.
	 */
	isMultiple(): boolean;
	private getTypePredicate;
	private getMatchingItems;
	private watchInput;
	private watchDropZone;
	private arrayApi;
	private entriesApi;
	/**
	 * Uploads a single batch via `XMLHttpRequest`.
	 * @param batch - Batch payload containing `FormData` and file paths.
	 * @param request - Optional request overrides merged over {@link Uploader.requestDefaults}.
	 */
	uploadBatch(batch: UploaderBatch, request?: UploaderRequest): Promise<void>;
	/**
	 * Default error handler. Logs the exception, surfaces server error messages,
	 * and falls back to generic notifications or an iframe dialog for HTML responses.
	 * @param data - Error context for the failed upload.
	 */
	static errorHandler(data: UploaderErrorData): void;
}
/**
 * An `HTMLElement` that can be validated (`input`, `select`, `textarea`, or `[contenteditable]`).
 * Extends `HTMLElement` with form-associated properties used by the validation engine.
 */
export interface ValidatableElement extends HTMLElement {
	/** Owning form element, if associated. */
	form?: HTMLFormElement;
	/** Field name used as the validation key. */
	name?: string;
	/** Input type (e.g. `"text"`, `"checkbox"`, `"radio"`). */
	type?: string;
	/** Current string value of the element. */
	value?: string;
}
/** Raw value extracted from a {@link ValidatableElement} for validation. */
export type ValidationValue = string | string[] | number | boolean;
/**
 * Validation rule implementation.
 * - `boolean` return: `true` passes, `false` fails using the default message.
 * - `string` return: non-empty string fails and is used as the error message.
 * - `Promise` return: async variant with the same semantics.
 * @param value - Current field value.
 * @param element - Element being validated.
 * @param params - Optional rule parameter (e.g. min value, regex).
 * @returns Validation result or promise thereof.
 */
export type ValidationProvider = (value: ValidationValue, element: ValidatableElement, params?: any) => boolean | string | Promise<boolean | string>;
/** Map of field name to error message / flag for fields currently failing validation. */
export interface ValidationErrorMap {
	[name: string]: (string | boolean);
}
/** Single validation failure entry. */
export interface ValidationErrorItem {
	/** Localized error message to display. */
	message: string;
	/** Element that failed validation. */
	element: ValidatableElement;
	/** Name of the rule / method that failed (e.g. `"required"`, `"email"`). */
	method?: string;
}
/** Ordered list of validation failures for the current validation run. */
export type ValidationErrorList = ValidationErrorItem[];
/** Rule set for a single field: method name to parameter (e.g. `{ required: true, minlength: 3 }`). */
export type ValidationRules = Record<string, any>;
/** Map of field name to its {@link ValidationRules}. */
export interface ValidationRulesMap {
	[name: string]: ValidationRules;
}
/**
 * Event delegate for validation triggers (`onclick`, `onfocusout`, `onkeyup`, `onfocusin`).
 * @param element - Source element that raised the event.
 * @param event - DOM event.
 * @param validator - Owning validator instance.
 */
export type ValidateEventDelegate = (element: ValidatableElement, event: Event, validator: Validator) => void;
/**
 * Configuration for a {@link Validator} instance. Mirrors jQuery Validation plugin options
 * with Serenity-specific extensions.
 */
export interface ValidatorOptions {
	/** When `true` enables debug logging and prevents form submit. */
	debug?: boolean;
	/**
	 * Use this class to create error labels, to look for existing error labels and to add it to invalid elements.
	 *
	 * default: "error"
	 */
	errorClass?: string | undefined;
	/**
	 * Use this element type to create error messages and to look for existing error messages. The default, "label",
	 * has the advantage of creating a meaningful link between error message and invalid field using the for attribute (which is always used, regardless of element type).
	 *
	 * default: "label"
	 */
	errorElement?: string | undefined;
	/**
	 * Customize placement of created error labels. First argument: The created error label. Second argument: The invalid element.
	 *
	 * default: Places the error label after the invalid element
	 */
	errorPlacement?(error: HTMLElement, element: ValidatableElement, validator: Validator): void;
	/**
	 * Focus the last active or first invalid element on submit via validator.focusInvalid(). The last active element is the one
	 * that had focus when the form was submitted, avoiding stealing its focus. If there was no element focused, the first one
	 * in the form gets it, unless this option is turned off.
	 *
	 * default: true
	 */
	focusInvalid?: boolean | undefined;
	/**
	 * How to highlight invalid fields. Override to decide which fields and how to highlight.
	 *
	 * default: Adds errorClass (see the option) to the element
	 */
	highlight?(element: ValidatableElement, errorClass: string, validClass: string): void;
	/**
	 * Elements to ignore when validating, simply filtering them out. CSS not-method is used, therefore everything that is
	 * accepted by not() can be passed as this option. Inputs of type submit and reset are always ignored, so are disabled elements.
	 */
	ignore?: string | undefined;
	/**
	 * Callback for custom code when an invalid form is submitted. Called with an event object as the first argument, and the validator
	 * as in the second.
	 */
	/**
	 * Callback invoked when an invalid form is submitted.
	 * @param event - Submit / invalid-form event.
	 * @param validator - Owning validator instance.
	 */
	invalidHandler?(event: Event, validator: Validator): void;
	/**
	 * Key/value pairs defining custom messages. Key is the name of an element, value is the message to display for that element. Instead
	 * of a plain message, another map with specific messages for each rule can be used. Overrides the title attribute of an element or
	 * the default message for the method (in that order). Each message can be a String or a Callback. The callback is called in the scope
	 * of the validator, with the rule's parameters as the first argument and the element as the second, and must return a String to display
	 * as the message.
	 *
	 * default: the default message for the method used
	 */
	messages?: Record<string, string | Record<string, string>> | undefined;
	/**
	 * Optional normalizer that transforms the raw field value before validation.
	 * @param val - Raw field value.
	 * @param element - Element being validated.
	 * @returns Normalized string value.
	 */
	normalizer?: (val: ValidationValue, element: ValidatableElement) => string;
	/**
	 * Boolean or Function. Validate checkboxes and radio buttons on click. Set to false to disable.
	 *
	 * Set to a Function to decide for yourself when to run validation.
	 * A boolean true is not a valid value.
	 */
	onclick?: ValidateEventDelegate | boolean | undefined;
	/**
	 * Function. Validate elements when user focuses in. If omitted hides all other fields marked as invalid.
	 *
	 * Set to a custom Function to decide for yourself when to run validation.
	 */
	onfocusin?: ValidateEventDelegate | undefined;
	/**
	 * Boolean or Function. Validate elements (except checkboxes/radio buttons) on blur. If nothing is entered, all rules are skipped, except when the field was already marked as invalid.
	 *
	 * Set to a Function to decide for yourself when to run validation.
	 * A boolean true is not a valid value.
	 */
	onfocusout?: ValidateEventDelegate | undefined;
	/**
	 * Boolean or Function. Validate elements on keyup. As long as the field is not marked as invalid, nothing happens.
	 * Otherwise, all rules are checked on each key up event. Set to false to disable.
	 *
	 * Set to a Function to decide for yourself when to run validation.
	 * A boolean true is not a valid value.
	 */
	onkeyup?: ValidateEventDelegate | undefined;
	/**
	 * Validate the form on submit. Set to false to use only other events for validation.
	 * Set to a Function to decide for yourself when to run validation.
	 * A boolean true is not a valid value.
	 *
	 * default: true
	 */
	onsubmit?: boolean | undefined;
	/**
	 * CSS class applied to elements with a pending async validation.
	 * default: `"pending"`
	 */
	pendingClass?: string | undefined;
	/**
	 * Static rule definitions keyed by field name.
	 */
	rules?: ValidationRulesMap | undefined;
	/**
	 * Custom error display handler. Receives the current error map and list.
	 * Call `this.defaultShowErrors()` to run the built-in display logic in addition to custom handling.
	 * @param errorMap - Map of field name to error message.
	 * @param errorList - Ordered list of validation failures.
	 * @param validator - Owning validator instance.
	 */
	showErrors?(errorMap: ValidationErrorMap, errorList: ValidationErrorList, validator: Validator): void;
	/**
	 * Called when a pending async validation is aborted (e.g. due to form reset or re-validation).
	 * @param validator - Owning validator instance.
	 */
	abortHandler?(validator: Validator): void;
	/**
	 * Callback for handling the actual submit when the form is valid. Gets the form and the event object. Replaces the default submit.
	 * The right place to submit a form via Ajax after it is validated.
	 */
	submitHandler?(form: HTMLFormElement, event: Event, validator: Validator): void | boolean;
	/**
	 * String or Function. If specified, the error label is displayed to show a valid element. If a String is given, it is added as
	 * a class to the label. If a Function is given, it is called with the label and the validated input (as a DOM element).
	 * The label can be used to add a text like "ok!".
	 */
	success?: string | ((label: HTMLElement, validatedInput: ValidatableElement) => void) | undefined;
	/**
	 * Called to revert changes made by option highlight, same arguments as highlight.
	 *
	 * default: Removes the errorClass
	 */
	unhighlight?(element: ValidatableElement, errorClass: string, validClass: string, validator: Validator): void;
	/**
	 * This class is added to an element after it was validated and considered valid.
	 *
	 * default: "valid"
	 */
	validClass?: string | undefined;
}
/**
 * Form validation engine inspired by jQuery Validation and ASP.NET client validation.
 * Manages rules, messages, error display, and async pending state for a single form.
 */
export declare class Validator {
	/**
	 * Checks whether a field is optional (not required and empty).
	 * @param element - Element to test.
	 * @param value - Optional explicit value; when omitted the element's current value is used.
	 * @returns Truthy optional marker or falsy when required / non-empty; `"dependency-mismatch"` when the required rule is absent and value is empty.
	 */
	static optional(element: ValidatableElement, value?: ValidationValue): "" | "dependency-mismatch";
	/** When `true` automatically combines `min`+`max` into `range` and `minlength`+`maxlength` into `rangelength` during rule normalization. */
	static autoCreateRanges: boolean;
	/** Default options applied to every new validator instance. */
	static defaults: ValidatorOptions;
	/** Default messages keyed by validation method name. Values may be translation keys or functions. */
	static readonly messages: Record<string, string | Function>;
	/** Built-in validation methods keyed by rule name. Extend via {@link Validator.addMethod}. */
	static readonly methods: Record<string, ValidationProvider>;
	/** Effective settings for this validator instance (merged defaults + constructor options). */
	readonly settings: ValidatorOptions;
	/** Last element that received focus, used by {@link Validator.focusInvalid}. */
	lastActive: ValidatableElement;
	private cancelSubmit;
	private currentElements;
	private currentForm;
	private errorMap;
	private errorList;
	private formSubmitted;
	private submitted;
	private submitButton;
	private pendingRequest;
	private invalid;
	private pending;
	private successList;
	private toHide;
	private toShow;
	/**
	 * Creates a validator for a form and wires up submit / focus / key handlers.
	 * @param form - Form element to validate.
	 * @param options - Validator options merged over {@link Validator.defaults}.
	 */
	constructor(form: HTMLFormElement, options: ValidatorOptions);
	/**
	 * Gets the validator instance associated with a form or an element inside a form.
	 * @param element - Form element, form-associated element, or array-like collection.
	 * @returns The validator instance, or `null` if not found.
	 */
	static getInstance(element: HTMLFormElement | Node | ArrayLike<HTMLElement>): Validator;
	private init;
	/**
	 * Checks if `element` is validatable (`input`, `select`, `textarea`).
	 * @param element The element to check.
	 * @returns `true` if validatable, otherwise `false`.
	 */
	static isValidatableElement(element: EventTarget): element is ValidatableElement;
	/**
	 * Whether the element is a checkbox or radio input.
	 * @param element - Element to test.
	 * @returns `true` if checkbox or radio.
	 */
	static isCheckOrRadio(element: Node): element is HTMLInputElement;
	/**
	 * Gets the logical length of a value for `minlength` / `maxlength` checks.
	 * Handles selects, checkbox groups, and plain strings.
	 * @param value - Raw field value.
	 * @param element - Source element (used for select / checkbox groups).
	 * @returns Length of the value.
	 */
	static getLength(value: ValidationValue, element: HTMLElement): number;
	/**
	 * Whether the element is content-editable.
	 * @param element - Element to test.
	 * @returns `true` if `contenteditable` is set and not `"false"`.
	 */
	static isContentEditable(element: HTMLElement): boolean;
	/**
	 * Extracts the current value from a form element, normalizing special cases
	 * (radio / checkbox groups, number inputs, file inputs, contenteditable).
	 * @param element - Source element.
	 * @returns The extracted value (`string`, `number`, `string[]`, or `null`).
	 */
	static elementValue(element: HTMLElement): any;
	/**
	 * Validates a form or a single element using its associated validator.
	 * @param element - Form or field element (or array-like collection; first element is used).
	 * @returns `true` if valid, `false` otherwise (or `false` if no validator is found).
	 */
	static valid(element: HTMLFormElement | ValidatableElement | ArrayLike<ValidatableElement>): boolean;
	/**
	 * Gets or mutates the validation rules for an element.
	 * @param element - Target element.
	 * @param command - `"add"` to add rules, `"remove"` to remove rules, or omitted to read.
	 * @param argument - Rules to add or space-separated method names to remove.
	 * @returns The aggregated rules (or removed rules when `command` is `"remove"`).
	 */
	static rules(element: ValidatableElement, command?: "add" | "remove", argument?: any): ValidationRules;
	/**
	 * Validates the entire form, updates error state, and shows errors.
	 * @returns `true` if the form is valid.
	 */
	form(): boolean;
	/**
	 * Validates all elements in the form without updating the display.
	 * @returns `true` if all elements are valid.
	 */
	checkForm(): boolean;
	/**
	 * Validates a single element and updates error display.
	 * @param element - Element to validate.
	 * @returns `true` if the element is valid.
	 */
	element(element: ValidatableElement): boolean;
	/**
	 * Displays validation errors, merging optional additional errors into the current state.
	 * @param errors - Optional additional error map to merge before display.
	 */
	showErrors(errors?: ValidationErrorMap): void;
	/**
	 * Resets form validation state, hides errors, and clears `aria-invalid` attributes.
	 */
	resetForm(): void;
	/**
	 * Resets visual validation state for a set of elements.
	 * @param elements - Elements to reset.
	 */
	resetElements(elements: ValidatableElement[]): void;
	/**
	 * Gets the count of currently invalid fields.
	 * @returns Number of invalid entries.
	 */
	numberOfInvalids(): number;
	private static objectLength;
	/** Hides currently tracked error labels. */
	hideErrors(): void;
	/**
	 * Hides a set of error labels.
	 * @param errors - Error label elements to hide.
	 */
	hideThese(errors: HTMLElement[]): void;
	/**
	 * Whether there are currently no validation errors.
	 * @returns `true` if valid.
	 */
	valid(): boolean;
	/**
	 * Gets the number of current validation errors.
	 * @returns Error count.
	 */
	size(): number;
	/**
	 * Focuses the last active invalid element or the first invalid element.
	 * Honors `abortHandler` and `focusInvalid` settings.
	 */
	focusInvalid(): void;
	/**
	 * Finds the last active element among current errors, if it is still invalid.
	 * @returns The last active invalid element, or falsy if none.
	 */
	findLastActive(): ValidatableElement;
	/**
	 * Gets all validatable elements in the form that have rules and are not ignored.
	 * @returns Array of elements to validate.
	 */
	elements(): ValidatableElement[];
	/**
	 * Gets existing error label elements in the form.
	 * @returns Array of error label elements.
	 */
	errors(): HTMLElement[];
	/** Resets internal error tracking without touching the DOM. */
	resetInternals(): void;
	/** Resets internal state and clears the current element list. */
	reset(): void;
	/** Resets all validation state including displayed errors. */
	resetAll(): void;
	/** Prepares state for a full form validation pass. */
	prepareForm(): void;
	/**
	 * Prepares state for validating a single element.
	 * @param element - Element to prepare for.
	 */
	prepareElement(element: ValidatableElement): void;
	/**
	 * Runs all applicable validation rules for an element.
	 * @param element - Element to check.
	 * @returns `true` if valid, `false` if invalid, or `undefined` for dependency mismatch.
	 */
	check(element: ValidatableElement): boolean;
	/**
	 * Gets a custom message from HTML5 `data-msg*` attributes for an element/method.
	 * @param element - Source element.
	 * @param method - Validation method name.
	 * @returns The data message, if present.
	 */
	customDataMessage(element: ValidatableElement, method: string): string;
	/**
	 * Gets a custom message from `settings.messages` for a field/method.
	 * @param name - Field name.
	 * @param method - Validation method name.
	 * @returns The configured message, if present.
	 */
	customMessage(name: string, method: string): any;
	/**
	 * Returns the first defined argument, allowing empty strings.
	 * @param args - Values to test in order.
	 * @returns The first non-`undefined` value.
	 */
	findDefined(...args: any[]): any;
	/**
	 * Resolves the default error message for a rule, checking custom messages, data attributes, and {@link Validator.messages}.
	 * @param element - Target element.
	 * @param rule - Rule descriptor with method and parameters.
	 * @returns The resolved message string.
	 */
	defaultMessage(element: ValidatableElement, rule: {
		method: string;
		parameters?: any;
	}): any;
	/**
	 * Formats the error message for a failed rule and records it in the error map/list.
	 * @param element - Element that failed.
	 * @param rule - Rule that failed.
	 */
	formatAndAdd(element: ValidatableElement, rule: {
		method: string;
		parameters: any;
	}): void;
	/** Default error display: highlights invalid elements, shows labels, and hides stale errors. */
	defaultShowErrors(): void;
	/**
	 * Gets elements among {@link Validator.currentElements} that are currently valid.
	 * @returns Valid elements.
	 */
	validElements(): ValidatableElement[];
	/**
	 * Gets elements that are currently invalid.
	 * @returns Invalid elements.
	 */
	invalidElements(): ValidatableElement[];
	/**
	 * Creates or updates the error label for an element.
	 * @param element - Target element.
	 * @param message - Error message; when omitted shows the success state if configured.
	 */
	showLabel(element: ValidatableElement, message?: string): void;
	/**
	 * Gets error labels associated with an element via `for` attribute or `aria-describedby`.
	 * @param element - Target element.
	 * @returns Matching error label elements.
	 */
	errorsFor(element: ValidatableElement): HTMLElement[];
	/**
	 * Gets the identifier used for error label association (`name` for radio/checkbox, otherwise `id` or `name`).
	 * @param element - Target element.
	 * @returns The identifier string.
	 */
	idOrName(element: ValidatableElement): string;
	/**
	 * Resolves the actual element to validate (first member of a radio/checkbox group, filtered by `ignore`).
	 * @param element - Source element.
	 * @returns The validation target, or `undefined` if filtered out.
	 */
	validationTargetFor(element: ValidatableElement): ValidatableElement;
	/**
	 * Finds all elements in the form with the given name.
	 * @param name - Field name to search for.
	 * @returns Matching elements.
	 */
	findByName(name: string): ValidatableElement[];
	/** Handlers for depend-type checks used by {@link Validator.depend}. */
	dependTypes: {
		boolean: (param: any) => any;
		string: (param: any, element: ValidatableElement) => boolean;
		function: (param: any, element: ValidatableElement) => any;
	};
	/**
	 * Evaluates whether a dependency condition is met.
	 * @param param - Boolean, selector string, or function.
	 * @param element - Context element.
	 * @returns `true` if the dependency is satisfied.
	 */
	depend(param: any, element: ValidatableElement): any;
	/**
	 * Marks an async validation request as pending for an element.
	 * @param element - Element with a pending remote check.
	 */
	startRequest(element: ValidatableElement): void;
	/**
	 * Clears a pending async request and triggers form submit / invalid-form handling as needed.
	 * @param element - Element whose request completed.
	 * @param valid - Whether the async result was valid.
	 */
	stopRequest(element: ValidatableElement, valid: boolean): void;
	/**
	 * Aborts a pending async request for an element, if any.
	 * @param element - Element whose pending request should be aborted.
	 */
	abortRequest(element: ValidatableElement): void;
	/**
	 * Gets or creates the cached previous value for a remote validation method.
	 * @param element - Target element.
	 * @param method - Validation method name (defaults to `"remote"`).
	 * @returns The cached previous value object.
	 */
	previousValue(element: ValidatableElement, method: string): any;
	/**
	 * Cleans up event handlers and removes the validator instance from the form.
	 */
	destroy(): void;
	/** CSS-class to rule mapping (e.g. `"required"` → `{ required: true }`). */
	static classRuleSettings: Record<string, ValidationRules>;
	/**
	 * Adds validation rules associated with a CSS class.
	 * @param className - Class name or map of class names to rules.
	 * @param rules - Rules to associate when `className` is a string.
	 */
	static addClassRules(className: (string | any), rules: ValidationRules): void;
	/**
	 * Gets rules derived from the element's CSS classes.
	 * @param element - Target element.
	 * @returns Rules inferred from classes.
	 */
	static classRules(element: ValidatableElement): ValidationRules;
	/**
	 * Normalizes a single attribute rule value (e.g. coercing `min`/`max`/`step` to numbers).
	 * @param rules - Rules object to mutate.
	 * @param type - Element `type` attribute.
	 * @param method - Rule method name.
	 * @param value - Raw attribute value.
	 */
	static normalizeAttributeRule(rules: ValidationRules, type: string, method: string, value: ValidationValue): void;
	/**
	 * Gets rules derived from HTML attributes (e.g. `required`, `minlength`, `type`).
	 * @param element - Target element.
	 * @returns Attribute-derived rules.
	 */
	static attributeRules(element: ValidatableElement): ValidationRules;
	/**
	 * Gets rules derived from `data-rule-*` attributes.
	 * @param element - Target element.
	 * @returns Data-attribute rules.
	 */
	static dataRules(element: ValidatableElement): any;
	/**
	 * Gets rules from the validator's static `settings.rules` for the element's name.
	 * @param element - Target element.
	 * @returns Static rules object.
	 */
	static staticRules(element: ValidatableElement): ValidationRules;
	/**
	 * Normalizes a merged rules object: handles `depends`, coerces numeric params, and optionally auto-creates ranges.
	 * @param rules - Raw merged rules.
	 * @param element - Target element.
	 * @returns Normalized rules.
	 */
	static normalizeRules(rules: ValidationRules, element: ValidatableElement): ValidationRules;
	/**
	 * Registers a new validation method.
	 * @param name - Method / rule name.
	 * @param method - Validation function.
	 * @param message - Optional default error message for the method.
	 */
	static addMethod(name: string, method: ValidationProvider, message?: string): void;
	/**
	 * Gets the element that should be highlighted for validation feedback.
	 * Checks `data-vx-highlight`, hidden `textarea` editors, and `select2-offscreen`.
	 * @param el - Source form element.
	 * @returns The highlight target element, or `undefined` if none.
	 */
	static getHighlightTarget(el: HTMLElement): HTMLElement;
	/**
	 * Adds a custom validation callback for an element. Multiple callbacks can be registered under distinct `uniqueName` keys.
	 * @param element - Target element or array-like collection (first element is used).
	 * @param rule - Callback returning an error message string when invalid, or `null`/`undefined` when valid.
	 * @param uniqueName - Optional key to allow independent removal; defaults to `""`.
	 */
	static addCustomRule(element: HTMLElement | ArrayLike<HTMLElement>, rule: (input: ValidatableElement) => string, uniqueName?: string): void;
	private static customRuleDisposingHandler;
	/**
	 * Removes a custom validation callback previously added with {@link Validator.addCustomRule}.
	 * @param element - Target element or array-like collection.
	 * @param uniqueName - Key under which the rule was registered.
	 */
	static removeCustomRule(element: HTMLElement | ArrayLike<HTMLElement>, uniqueName: string): void;
	/** Modifier / navigation keys that should not trigger `onkeyup` re-validation. */
	static readonly excludedModifierKeys: Set<string>;
}
/** Alias for {@link Validator.addCustomRule}. */
export declare const addValidationRule: typeof Validator.addCustomRule;
/** Alias for {@link Validator.removeCustomRule}. */
export declare const removeValidationRule: typeof Validator.removeCustomRule;
/**
 * Tests whether any element in the array satisfies the predicate.
 * @param array - Array to test.
 * @param predicate - Function invoked per element; should return `true` for a match.
 * @returns `true` if at least one element matches, otherwise `false`.
 * @deprecated Prefer native `Array.prototype.some` — e.g. `array.some(predicate)`. Retained as a `Q.any` compat shim.
 * @example
 * any([1, 2, 3], x => x > 2); // true
 */
export declare function any<TItem>(array: TItem[], predicate: (x: TItem) => boolean): boolean;
/**
 * Counts elements that satisfy the predicate.
 * @param array - Array to count over.
 * @param predicate - Function invoked per element; return `true` to count the element.
 * @returns Number of matching elements.
 * @deprecated Prefer `array.filter(predicate).length` or a manual loop. Retained as a `Q.count` compat shim.
 * @example
 * count([1, 2, 3], x => x % 2 === 1); // 2
 */
export declare function count<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number;
/**
 * Returns the first element that satisfies the predicate (LINQ `First` semantics).
 * @param array - Array to search.
 * @param predicate - Function invoked per element; return `true` for the desired element.
 * @returns The first matching element.
 * @throws {Error} If no element satisfies the predicate (`"first:No element satisfies the condition."`).
 * @deprecated Prefer `array.find(predicate)` with explicit not-found handling. Retained as a `Q.first` compat shim.
 * @example
 * first([1, 2, 3], x => x > 1); // 2
 */
export declare function first<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;
/**
 * Single group produced by {@link groupBy}.
 * @typeParam TItem - Element type of the source array.
 * @example
 * const g = groupBy(users, u => u.department);
 * g.inOrder[0].key; // department key
 */
export type GroupByElement<TItem> = {
	/** Zero-based position of this group in the {@link GroupByResult.inOrder} array. */
	order: number;
	/** Group key as returned by the `getKey` callback (normalized to string). */
	key: string;
	/** Elements belonging to this group, in original encounter order. */
	items: TItem[];
	/** Index of the first element of this group in the original source array. */
	start: number;
};
/**
 * Result returned by {@link groupBy}.
 * @typeParam TItem - Element type of the source array.
 * @remarks Provides both dictionary (`byKey`) and ordered (`inOrder`) access to groups.
 */
export type GroupByResult<TItem> = {
	/** Dictionary mapping stringified key to its {@link GroupByElement}. */
	byKey: {
		[key: string]: GroupByElement<TItem>;
	};
	/** Groups in order of first encounter in the source array. */
	inOrder: GroupByElement<TItem>[];
};
/**
 * Groups an array with keys determined by specified getKey() callback.
 * Resulting object contains group objects in order and a dictionary to access by key.
 * This is similar to LINQ's ToLookup function with some additional details like start index.
 * @param items Array to group.
 * Groups an array by keys derived from each element.
 * @param items - Array to group.
 * @param getKey - Callback returning the group key for an element; `null`/`undefined` is normalized to `""`.
 * @returns A {@link GroupByResult} with `byKey` dictionary and `inOrder` array. Each group records its `order`, `key`, `items`, and `start` index.
 * @remarks Similar to LINQ `ToLookup` with extra `order`/`start` metadata. Uses `Object.create(null)` so prototype keys are safe.
 * @deprecated Kept as a `Q.groupBy` compat shim; for new code consider `Map`-based grouping or `toGrouping`.
 * @example
 * groupBy([{k:'a'}, {k:'b'}, {k:'a'}], x => x.k).inOrder.length; // 2
 */
export declare function groupBy<TItem>(items: TItem[], getKey: (x: TItem) => any): GroupByResult<TItem>;
/**
 * Returns the index of the first element satisfying the predicate.
 * @param array - Array to search.
 * @param predicate - Function invoked per element; return `true` for the target element.
 * @returns Zero-based index of the first match, or `-1` if none matches.
 * @deprecated Prefer `Array.prototype.findIndex` — `array.findIndex(predicate)`. Retained as a `Q.indexOf` compat shim (note the predicate overload differs from `Array.indexOf`).
 * @example
 * indexOf([1, 2, 3], x => x === 2); // 1
 */
export declare function indexOf<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number;
/**
 * Inserts an item into an array at the given index.
 * @param obj - Target array or array-like object with an `insert(index, item)` method.
 * @param index - Zero-based index at which to insert.
 * @param item - Item to insert.
 * @throws {Error} If `obj` is neither an array nor exposes `insert`.
 * @remarks If `obj.insert` exists it is delegated to; otherwise `Array.prototype.splice` is used. No return value.
 * @deprecated Prefer `array.splice(index, 0, item)` directly. Retained as a `Q.insert` compat shim.
 * @example
 * insert([1, 2, 3], 1, 4); // [1, 4, 2, 3]
 */
export declare function insert(obj: any, index: number, item: any): void;
/**
 * Tests whether a value is an array.
 * @remarks Thin re-export of `Array.isArray` for legacy `Q.isArray` call sites.
 * @deprecated Use `Array.isArray` directly.
 * @example
 * isArray([1, 2, 3]); // true
 * isArray({}); // false
 */
export declare const isArray: (arg: any) => arg is any[];
/**
 * Returns the single element satisfying the predicate (LINQ `Single` semantics).
 * @param array - Array to search.
 * @param predicate - Function invoked per element; exactly one element must return `true`.
 * @returns The sole matching element.
 * @throws {Error} If no element matches (`"single:No element satisfies the condition."`) or more than one matches (`"single:sequence contains more than one element."`).
 * @deprecated Retained as a `Q.single` compat shim; prefer explicit `filter` + length check for clarity.
 * @example
 * single([1, 2, 3], x => x == 2); // 2
 */
export declare function single<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;
/**
 * Dictionary mapping a stringified key to the array of items sharing that key.
 * Produced by {@link toGrouping}.
 * @typeParam TItem - Element type of the source array.
 */
export type Grouping<TItem> = {
	[key: string]: TItem[];
};
/**
 * Groups an array into a dictionary keyed by `getKey`.
 * @param items - Array to group.
 * @param getKey - Callback returning the group key for an element; `null`/`undefined` is normalized to `""`.
 * @returns A {@link Grouping} dictionary whose values are arrays of matching elements. Uses a null-prototype object.
 * @remarks Lighter alternative to {@link groupBy} when ordered metadata is not needed.
 * @deprecated Retained as a `Q.toGrouping` compat shim; new code may prefer `Map`-grouping.
 * @example
 * toGrouping([1, 2, 3], x => x % 2 == 0 ? "even" : "odd"); // { odd: [1, 3], even: [2] }
 */
export declare function toGrouping<TItem>(items: TItem[], getKey: (x: TItem) => any): Grouping<TItem>;
/**
 * Returns the first element satisfying the predicate, or `undefined` if none matches (LINQ `FirstOrDefault`).
 * @param array - Array to search.
 * @param predicate - Function invoked per element; return `true` for the desired element.
 * @returns The first matching element, or `undefined` when no match is found.
 * @deprecated Prefer `Array.prototype.find` — `array.find(predicate)`. Retained as a `Q.tryFirst` compat shim.
 * @example
 * tryFirst([1, 2, 3], x => x == 2); // 2
 * tryFirst([1, 2, 3], x => x == 4); // undefined
 */
export declare function tryFirst<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;
/**
 * Legacy `Q.alert` alias.
 * @deprecated Use {@link alertDialog} from `"@serenity-is/corelib"` instead. This re-export is retained for compat with code that imports `Q.alert` / `Serenity.alert`.
 * @see {@link alertDialog}
 */
declare const alert$1: typeof alertDialog;
/**
 * Legacy `Q.confirm` alias.
 * @deprecated Use {@link confirmDialog} instead.
 * @see {@link confirmDialog}
 */
declare const confirm$1: typeof confirmDialog;
/**
 * Legacy `Q.information` alias.
 * @deprecated Use {@link informationDialog} instead.
 * @see {@link informationDialog}
 */
export declare const information: typeof informationDialog;
/**
 * Legacy `Q.success` alias.
 * @deprecated Use {@link successDialog} instead.
 * @see {@link successDialog}
 */
export declare const success: typeof successDialog;
/**
 * Legacy `Q.warning` alias.
 * @deprecated Use {@link warningDialog} instead.
 * @see {@link warningDialog}
 */
export declare const warning: typeof warningDialog;
/**
 * Lowercases a string with Turkish-specific handling (`İ` → `i`, `I` → `ı`).
 * @param a - Input string; if falsy, returned as-is.
 * @returns Lowercased string with Turkish dotted/dotless-I mapping preserved.
 * @remarks Compat shim retained because native `String.prototype.toLocaleLowerCase('tr')` behaves differently across engines; prefer locale-aware APIs for new code.
 * @deprecated Retained for legacy `Q.turkishLocaleToLower` call sites.
 * @example
 * turkishLocaleToLower("İSTANBUL"); // "istanbul" with ı handling
 */
export declare function turkishLocaleToLower(a: string): string;
/**
 * Uppercases a string with Turkish-specific handling (`i` → `İ`, `ı` → `I`).
 * @param a - Input string; if falsy, returned as-is.
 * @returns Uppercased string with Turkish dotted/dotless-I mapping preserved.
 * @remarks Compat shim; for new code prefer `toLocaleUpperCase('tr')`.
 * @deprecated Retained for legacy `Q.turkishLocaleToUpper` call sites.
 * @example
 * turkishLocaleToUpper("istanbul"); // handles dotted i
 */
export declare function turkishLocaleToUpper(a: string): string;
/**
 * Legacy alias for {@link Culture.stringCompare}.
 * @deprecated Use `Culture.stringCompare` directly.
 * @see {@link Culture.stringCompare}
 */
export declare let turkishLocaleCompare: (a: string, b: string) => number;
/**
 * Legacy alias for {@link stringFormat} (`Q.format`).
 * @deprecated Use {@link stringFormat} directly.
 * @see {@link stringFormat}
 */
export declare let format: typeof stringFormat;
/**
 * Legacy alias for {@link stringFormatLocale}.
 * @deprecated Use {@link stringFormatLocale} directly.
 * @see {@link stringFormatLocale}
 */
export declare let localeFormat: typeof stringFormatLocale;
/**
 * Formats a duration given in minutes as `"d.hh:mm"` (days, hours, minutes).
 * @param n - Total minutes; `null`/`undefined` yields `""`, `0` yields `"0"`.
 * @returns Formatted string — e.g. `1500` → `"1.01:00"`, `90` → `"01:30"`.
 * @remarks Days are omitted when zero; minutes part `"00:00"` is omitted when zero unless days is also zero. Compat helper from `Q.formatDayHourAndMin`.
 * @example
 * formatDayHourAndMin(1500); // "1.01:00"
 * formatDayHourAndMin(0);    // "0"
 */
export declare function formatDayHourAndMin(n: number): string;
/**
 * Parses a `"hh:mm"` time string into total minutes.
 * @param value - String to parse (accepts `h:mm` or `hh:mm`; surrounding whitespace is trimmed).
 * @returns Total minutes (`h*60+m`), `null` for empty/whitespace input, or `NaN` if the format or range is invalid (hours must be 0–23, minutes 0–59, length 4–5 chars).
 * @remarks Compat helper from `Q.parseHourAndMin`.
 * @example
 * parseHourAndMin("02:30"); // 150
 * parseHourAndMin("2:05");  // 125
 */
export declare function parseHourAndMin(value: string): number;
/**
 * Parses a `"d.hh:mm"` duration string into total minutes (also accepts plain `"hh:mm"` or day count).
 * @param s - String to parse; whitespace is trimmed.
 * @returns Total minutes, `null` for empty input, or `NaN` for invalid format/range (hours 0–23, minutes 0–59).
 * @remarks Accepts `"d"` (days), `"hh:mm"`, or `"d.hh:mm"` (two-part split on `.`). Delegates the time part to {@link parseHourAndMin}. Compat helper from `Q.parseDayHourAndMin`.
 * @example
 * parseDayHourAndMin("1.01:00"); // 1500
 * parseDayHourAndMin("01:30");   // 90
 */
export declare function parseDayHourAndMin(s: string): number;
/**
 * Appends an empty (placeholder) option to a `<select>` element.
 * @param select - Target `<select>` or array-like/jQuery-like wrapper containing it.
 * @remarks Uses {@link SelectEditorTexts.EmptyItemText} as the display text and `""` as the value; delegates to {@link addOption}. Compat helper from `Q.addEmptyOption`.
 */
export declare function addEmptyOption(select: ArrayLike<HTMLElement> | HTMLSelectElement): void;
/**
 * Appends an `<option>` to a `<select>` element.
 * @param select - Target `<select>` or array-like/jQuery-like wrapper containing it.
 * @param key - Value attribute for the option (`null`/`undefined` → `""`).
 * @param text - Display text for the option (`null`/`undefined` → `""`).
 * @remarks Creates an `HTMLOptionElement` via `document.createElement("option")`. No-op if the resolved select element is falsy. Compat helper from `Q.addOption`.
 */
export declare function addOption(select: ArrayLike<HTMLElement> | HTMLSelectElement, key: string, text: string): void;
/**
 * Legacy alias for {@link htmlEncode}.
 * @deprecated Use {@link htmlEncode} directly (it also encodes quotes). Retained as `Q.attrEncode` compat shim.
 * @see {@link htmlEncode}
 */
export declare const attrEncode: typeof htmlEncode;
/**
 * Removes all child options/content from a `<select>` element.
 * @param select - Target element or array-like/jQuery-like wrapper containing it.
 * @remarks Resolves array-like wrappers via `isArrayLike` and clears with `innerHTML = ''`. No-op if the resolved element is falsy. Compat helper from `Q.clearOptions`.
 */
export declare function clearOptions(select: HTMLElement | ArrayLike<HTMLElement>): void;
/**
 * Resolves a sibling/related element by a suffix relative to a source element's id.
 * @param element - Source element or array-like/jQuery-like wrapper containing it.
 * @param relativeId - Suffix to append to the source id (with/without leading `_`) when searching.
 * @param context - Scope element for `querySelector`; defaults to the source element's root node. When omitted the search also falls back to `document.getElementById`.
 * @returns The matched `HTMLElement`, or `null` if the source is `null` or no match is found.
 * @remarks Tries `"#" + fromId + relativeId` then `"#" + fromId + "_" + relativeId`, progressively stripping trailing `"_segment"` segments from `fromId` until a match or exhaustion. Compat helper from `Q.findElementWithRelativeId`.
 * @example
 * findElementWithRelativeId(document.getElementById("Customer_Name"), "_City"); // finds #Customer_City if present
 */
export declare function findElementWithRelativeId(element: HTMLElement | ArrayLike<HTMLElement>, relativeId: string, context?: HTMLElement): HTMLElement;
/**
 * Creates a new `<div>` and appends it to `document.body`.
 * @returns The newly created and appended `HTMLDivElement`.
 * @remarks Compat helper from `Q.newBodyDiv`; prefer `document.createElement` + explicit append in new code.
 */
export declare function newBodyDiv(): HTMLDivElement;
/**
 * Returns the outer HTML markup of an element (including the element itself).
 * @param element - Target element, `Element`, or array-like/jQuery-like wrapper containing it.
 * @returns Outer HTML string. For non-Elements, clones the node into a temporary `<i>` wrapper and returns `innerHTML`; yields `""` for falsy targets.
 * @remarks Compat helper from `Q.outerHtml`; for new code prefer `element.outerHTML` directly.
 */
export declare function outerHtml(element: Element | ArrayLike<HTMLElement>): string;
/**
 * Options bag for {@link initGlobalMappings}.
 */
export interface InitGlobalMappingsOptions {
	/** Global object to install mappings on; defaults to {@link getGlobalObject}. */
	globals?: any;
	/** Core library exports to expose as `Serenity` / `Q` globals. */
	corelib?: any;
	/** `domwise` package exports to merge into `Serenity`. */
	domwise?: any;
	/** `sleekgrid` package exports to expose as `Slick` and merge into `Serenity`. */
	sleekgrid?: any;
	/** `extensions` package exports to merge into `Serenity` / `Serenity.Extensions`. */
	extensions?: any;
	/** `proextensions` package exports to merge into `Serenity`. */
	proextensions?: any;
	/** Bootstrap module (unwraps `.default` if needed) to expose as `bootstrap`. */
	bootstrap?: any;
	/** flatpickr module to expose as `flatpickr` (also calls {@link initFlatpickrLocale}). */
	flatpickr?: any;
	/** GLightbox module to expose as `GLightbox`. */
	glightbox?: any;
	/** Mousetrap module to expose as `Mousetrap`. */
	mousetrap?: any;
	/** NProgress module to expose as `NProgress` (also calls {@link initNProgress}). */
	nprogress?: any;
	/** SortableJS module to expose as `Sortable`. */
	sortable?: any;
}
/**
 * Installs legacy global namespace mappings (`Serenity`, `Slick`, `Q`, and vendor globals) for
 * compatibility with feature packages that consume globals via `tsbuild`'s `importAsGlobals`.
 * @param options - Bag of package exports / vendor modules to expose on the global object.
 * @remarks
 * - When `corelib` is provided it becomes `globals.Serenity` (or is merged via live getters if `Serenity` already exists).
 * - `sleekgrid` populates `globals.Slick` and is merged into `Serenity`; `Aggregators`/`AggregateFormatting` sub-objects and `RemoteView` are synced between `Slick` and `Serenity`.
 * - `extensions`/`proextensions`/`domwise` are merged into `Serenity` (extensions also under `Serenity.Extensions`).
 * - `bootstrap`/`mousetrap`/`sortable`/`nprogress`/`glightbox`/`flatpickr` unwrap `.default` when needed and are assigned to `bootstrap`/`Mousetrap`/`Sortable`/`NProgress`/`GLightbox`/`flatpickr` respectively.
 * - Missing or already-present targets are merged via getter/setter proxies (`copyProps`) so later assignments stay in sync.
 * @example
 * initGlobalMappings({ corelib: SerenityCore, sleekgrid: SlickGrid, globals: window });
 */
export declare function initGlobalMappings({ corelib, globals, domwise, sleekgrid, extensions, proextensions, bootstrap, flatpickr, glightbox, mousetrap, nprogress, sortable }: InitGlobalMappingsOptions): void;
/**
 * Localizes a flatpickr instance to the document language.
 * @param flatpickr - flatpickr module/instance with a `l10ns` dictionary and `localize` method; no-op if missing `l10ns`.
 * @remarks Reads `document.documentElement.lang` (falls back to `"en"`), tries the full locale (e.g. `"tr-tr"`) then the base language (e.g. `"tr"`) if available in `flatpickr.l10ns`. Called automatically by {@link initGlobalMappings} when a flatpickr module is provided.
 * @example
 * initFlatpickrLocale(flatpickr);
 */
export declare function initFlatpickrLocale(flatpickr: any): void;
/**
 * Wires NProgress to global `ajaxStart`/`ajaxStop` events (via {@link Fluent}).
 * @param nprogress - NProgress instance; defaults to `getGlobalObject().NProgress` when omitted.
 * @returns `true` once initialized (`nprogress.serenityInit` is set); `undefined`/falsy if already initialized, missing, or `document` is unavailable.
 * @remarks Starts the progress bar 200 ms after `ajaxStart` (debounced) and completes it on `ajaxStop`. No-ops if `start`/`done` are missing, already initialized, or running outside a browser. Called automatically by {@link initGlobalMappings} when an `nprogress` module is provided.
 * @example
 * initNProgress(NProgress);
 */
export declare function initNProgress(nprogress?: any): boolean;
/**
 * Returns the widget name for a type, derived from its full type name with
 * dots replaced by underscores.
 * @param type - The widget type.
 * @returns The widget name.
 */
export declare function getWidgetName(type: Function): string;
/**
 * Associates a widget with its DOM node so it can later be retrieved via
 * {@link tryGetWidget} or {@link getWidgetFrom}.
 * @param widget - The widget to associate.
 */
export declare function associateWidget(widget: {
	domNode: HTMLElement;
}): void;
/**
 * Removes the association between a widget and its DOM node.
 * @param widget - The widget to deassociate.
 */
export declare function deassociateWidget(widget: {
	domNode: HTMLElement;
}): void;
/**
 * Tries to find a widget associated with an element, optionally filtering by
 * type.
 * @param element - The element (or selector/array-like) to search.
 * @param type - Optional widget type to filter by; when omitted, the first
 *   associated widget is returned.
 * @returns The matching widget, or null if none is found.
 */
export declare function tryGetWidget<TWidget>(element: Element | ArrayLike<HTMLElement> | string, type?: {
	new (...args: any[]): TWidget;
}): TWidget;
/**
 * Finds a widget associated with an element, throwing an error if none is
 * found.
 * @param element - The element (or selector/array-like) to search.
 * @param type - Optional widget type to filter by.
 * @param context - Optional DOM node used to resolve a selector.
 * @returns The matching widget.
 */
export declare function getWidgetFrom<TWidget>(element: ArrayLike<HTMLElement> | Element | string, type?: {
	new (...args: any[]): TWidget;
}, context?: HTMLElement): TWidget;
/**
 * A helper object that resolves prefix-relative ids, with special handling for
 * the `Form`, `Tabs`, `Toolbar` and `PropertyGrid` keys.
 */
export type IdPrefixType = {
	[key: string]: string;
	Form: string;
	Tabs: string;
	Toolbar: string;
	PropertyGrid: string;
};
/**
 * Creates an id prefix helper for resolving child element ids.
 * @param prefix - The id prefix to use.
 * @returns An {@link IdPrefixType} proxy.
 */
export declare function useIdPrefix(prefix: string): IdPrefixType;
/**
 * Props accepted by all widgets, including the target element and common
 * element attributes.
 * @typeParam P - The widget's specific options type.
 */
export type WidgetProps<P> = {
	/** Optional id for the widget's DOM node. */
	id?: string;
	/** Optional CSS class(es) for the widget's DOM node. */
	class?: string;
	/** The element to bind the widget to, as an element, array-like, selector
	 *  or a callback that receives the created element. */
	element?: ((el: HTMLElement) => void) | HTMLElement | ArrayLike<HTMLElement> | string;
} & SNoInfer<P>;
/**
 * The base class for all Serenity widgets. A widget wraps a DOM node, manages
 * its lifecycle (create/destroy), associates itself with its element for later
 * lookup, and provides helpers for id prefixes, validation and rendering.
 * @typeParam P - The widget's options/props type.
 */
export declare class Widget<P = {}> {
	private static nextWidgetNumber;
	/** The widget's options/props. */
	protected readonly options: WidgetProps<P>;
	/** A unique name for this widget instance, used for event namespacing. */
	readonly uniqueName: string;
	/** The id prefix used for this widget's child element ids. */
	readonly idPrefix: string;
	/** The DOM node this widget is bound to. */
	readonly domNode: HTMLElement;
	/**
	 * Creates a widget bound to the given props, resolving the DOM node,
	 * associating the widget with it and rendering its contents.
	 * @param props - The widget props, including the target element.
	 */
	constructor(props: WidgetProps<P>);
	/**
	 * Destroys the widget, removing its association with the DOM node, its CSS
	 * classes and its event handlers.
	 */
	destroy(): void;
	/**
	 * Creates the default DOM element for a widget.
	 * @returns A new `div` element.
	 */
	static createDefaultElement(): HTMLElement;
	/**
	 * Returns a Fluent(this.domNode) object
	 */
	get element(): Fluent;
	/**
	 * Adds the widget's CSS class to its DOM node.
	 */
	protected addCssClass(): void;
	/**
	 * Determines whether rendering should be deferred until {@link init} is
	 * called.
	 * @returns True to defer rendering.
	 */
	protected deferRender(): boolean;
	/**
	 * Returns the CSS class(es) applied to the widget's DOM node.
	 * @returns The space-separated CSS class string.
	 */
	protected getCssClass(): string;
	/**
	 * Returns the widget name for a type, used for association and unique names.
	 * @param type - The widget type.
	 * @returns The widget name.
	 */
	static getWidgetName(type: Function): string;
	/**
	 * Adds a validation rule to the widget's DOM node.
	 * @param rule - The validation rule function, or a unique name when the
	 *   two-argument overload is used.
	 * @param uniqueName - A unique name for the rule, or the rule function when
	 *   the two-argument overload is used.
	 */
	addValidationRule(rule: (input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement) => string, uniqueName?: string): void;
	addValidationRule(uniqueName: string, rule: (input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement) => string): void;
	/**
	 * Finds a child element by its prefix-relative id.
	 * @param id - The id relative to the widget's id prefix.
	 * @returns A {@link Fluent} wrapper for the matching element.
	 */
	protected byId<TElement extends HTMLElement = HTMLElement>(id: string): Fluent<TElement>;
	/**
	 * Finds a child element by its prefix-relative id.
	 * @param id - The id relative to the widget's id prefix.
	 * @returns The matching element, or null if not found.
	 */
	protected findById<TElement extends HTMLElement = HTMLElement>(id: string): TElement;
	/**
	 * Returns the closest `.field` element containing the widget's DOM node.
	 * @returns A {@link Fluent} wrapper for the grid field.
	 */
	getGridField(): Fluent;
	/**
	 * Registers a `change` handler on the widget's DOM node.
	 * @param handler - The change event handler.
	 */
	change(handler: (e: Event) => void): void;
	/**
	 * Registers a `change` handler that ignores changes originating from
	 * combobox setting values.
	 * @param handler - The change event handler.
	 */
	changeSelect2(handler: (e: Event) => void): void;
	/**
	 * Creates a widget instance from the given params, appending its element to
	 * the container and invoking the init/init callbacks.
	 * @param params - The widget creation params.
	 * @returns The created widget instance.
	 */
	static create<TWidget extends Widget<P>, P>(params: CreateWidgetParams<TWidget, P>): TWidget;
	/**
	 * Returns a custom attribute applied to the widget's type.
	 * @param attrType - The attribute type to look up.
	 * @param inherit - Whether to search inherited types; defaults to true.
	 * @returns The matching attribute, or null.
	 */
	protected getCustomAttribute<TAttr extends CustomAttribute>(attrType: {
		new (...args: any[]): TAttr;
	}, inherit?: boolean): TAttr;
	/**
	 * Queues a callback to run after the widget's contents are rendered.
	 * @param callback - The callback to run after rendering.
	 */
	protected afterRender(callback: () => void): void;
	/**
	 * Initializes the widget, rendering its contents if rendering was deferred.
	 * @returns This widget instance.
	 */
	init(): this;
	/**
	 * Returns the main element for this widget or the document fragment.
	 * As widgets may get their elements from props unlike regular JSX widgets,
	 * this method should not be overridden. Override renderContents() instead.
	 */
	render(): any;
	/**
	 * Renders the widget's contents and runs any queued after-render callbacks.
	 */
	internalRenderContents(): void;
	/**
	 * Renders the widget's contents. Override this to provide custom content.
	 * @returns The rendered contents.
	 */
	protected renderContents(): any;
	/**
	 * Renders the widget from a legacy `getTemplate` string, if defined.
	 * @returns True if a legacy template was rendered.
	 */
	protected legacyTemplateRender(): boolean;
	/**
	 * Returns the widget's props/options.
	 */
	get props(): WidgetProps<P>;
	/**
	 * Runs a method synchronously or asynchronously depending on the widget's
	 * `useAsync` flag, then invokes a continuation.
	 * @param syncMethod - The synchronous method to run.
	 * @param asyncMethod - The asynchronous method to run.
	 * @param then - The continuation invoked with the result.
	 */
	protected syncOrAsyncThen<T>(syncMethod: (() => T), asyncMethod: (() => PromiseLike<T>), then: (v: T) => void): void;
	/**
	 * Returns an id prefix helper for resolving child element ids.
	 * @returns An {@link IdPrefixType} proxy for this widget's id prefix.
	 */
	protected useIdPrefix(): IdPrefixType;
	static readonly isComponent = true;
	/**
	 * Registers this type as a class with the given type name.
	 * @param typeName - The type name to register.
	 * @param intfAndAttr - Optional interfaces and attributes.
	 * @returns The class type info.
	 */
	protected static registerClass<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): ClassTypeInfo<TypeName>;
	/**
	 * Registers this type as an editor with the given type name.
	 * @param typeName - The type name to register.
	 * @param intfAndAttr - Optional interfaces and attributes.
	 * @returns The editor type info.
	 */
	protected static registerEditor<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): EditorTypeInfo<TypeName>;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
}
/** @deprecated Use {@link Widget} */
export declare const TemplatedWidget: typeof Widget;
/**
 * Parameters for {@link Widget.create}.
 * @typeParam TWidget - The widget type to create.
 * @typeParam P - The widget's options type.
 */
export interface CreateWidgetParams<TWidget extends Widget<P>, P> {
	/** The widget type to instantiate. */
	type?: {
		new (options?: P): TWidget;
		prototype: TWidget;
	};
	/** The options to pass to the widget. */
	options?: P & WidgetProps<{}>;
	/** The container to append the widget's element to. */
	container?: HTMLElement | ArrayLike<HTMLElement>;
	/** Callback invoked with the created element. */
	element?: (e: Fluent) => void;
	/** Callback invoked after the widget is initialized. */
	init?: (w: TWidget) => void;
}
/**
 * Initializes a full-height grid page from a widget class and props object.
 * Compat shim for the legacy `GridPageInit` global; wraps {@link initWidgetPage} with `#GridDiv` as the default container.
 * @param type - The widget class to instantiate.
 * @param props - Optional widget properties passed to the widget constructor.
 * @returns The root {@link HTMLElement} (`domNode`) of the initialized grid widget.
 * @deprecated Prefer calling `initWidgetPage` / `gridPageInit` directly or using the modern `Fluent` / widget APIs. Kept for backward compatibility with pre-corelib page scripts.
 */
export declare function GridPageInit<TGrid extends Widget<P>, P>({ type, props }: {
	type: CreateWidgetParams<TGrid, P>["type"];
	props?: WidgetProps<P>;
}): HTMLElement;
/**
 * Initializes a full-height panel page from a widget class and props object.
 * Compat shim for the legacy `PanelPageInit` global; wraps {@link initWidgetPage} with `#Panel` as the default container.
 * @param type - The panel widget class to instantiate.
 * @param props - Optional widget properties.
 * @returns The root {@link HTMLElement} of the initialized panel widget.
 * @deprecated Prefer `panelPageInit` or direct widget construction. Kept for backward compatibility.
 */
export declare function PanelPageInit<TPanel extends Widget<P>, P>({ type, props }: {
	type: CreateWidgetParams<TPanel, P>["type"];
	props?: WidgetProps<P>;
}): HTMLElement;
/**
 * Initializes a Serenity grid page that fills the available viewport height.
 * Compat shim for the legacy `Q.gridPageInit` / `Serenity.gridPageInit` API. Accepts either an existing widget instance or a widget class + props.
 * @param grid - An existing grid widget instance (must expose `domNode`).
 * @returns The same grid widget after full-height layout initialization.
 * @deprecated Use widget construction with {@link initFullHeightGridPage} or modern layout components. Kept for legacy page scripts.
 */
export declare function gridPageInit<TGrid extends Widget<P>, P>(grid: TGrid & {
	domNode: HTMLElement;
}): TGrid;
/**
 * Initializes a Serenity grid page that fills the available viewport height.
 * @param type - Grid widget class to instantiate.
 * @param props - Optional widget properties (supports `element` as selector or callback).
 * @returns The newly created and initialized grid widget.
 */
export declare function gridPageInit<TGrid extends Widget<P>, P>(type: CreateWidgetParams<TGrid, P>["type"], props?: WidgetProps<P>): TGrid;
/**
 * Initializes a Serenity panel page without hash-router integration.
 * Compat shim for the legacy `Q.panelPageInit` / `Serenity.panelPageInit` API. Accepts either an existing panel instance or a widget class + props.
 * @param panel - An existing panel widget instance (must expose `domNode`).
 * @returns The same panel widget after layout initialization (`noRoute: true`).
 * @deprecated Use direct widget construction with {@link initFullHeightGridPage}. Kept for legacy compatibility.
 */
export declare function panelPageInit<TGrid extends Widget<P>, P>(panel: TGrid & {
	domNode: HTMLElement;
}): TGrid;
/**
 * Initializes a Serenity panel page without hash-router integration.
 * @param type - Panel widget class to instantiate.
 * @param props - Optional widget properties.
 * @returns The newly created and initialized panel widget.
 */
export declare function panelPageInit<TGrid extends Widget<P>, P>(type: CreateWidgetParams<TGrid, P>["type"], props?: WidgetProps<P>): TGrid;
/**
 * Configures a full-height page layout for a grid or panel container.
 * Compat shim for the legacy `Q.initFullHeightGridPage`. Adds `full-height-page` / `responsive-height` classes, wires resize or `layout` events, and optionally resolves the hash router.
 * @param gridDiv - Target container: an {@link HTMLElement}, array-like collection, or an object with a `domNode` property.
 * @param opt - Layout options.
 * @param opt.noRoute - When `true`, skips the one-time {@link Router}.`resolve()` call on initial page load. Defaults to `false`.
 * @param opt.setHeight - When `true` forces height filling via {@link layoutFillHeight}; when `false` disables it; when omitted auto-detects via jQuery and element classes. Defaults to auto.
 * @deprecated Prefer CSS flex / grid layouts or `Fluent` responsive utilities. Kept for legacy full-height pages.
 */
export declare function initFullHeightGridPage(gridDiv: HTMLElement | ArrayLike<HTMLElement> | {
	domNode: HTMLElement;
}, opt?: {
	noRoute?: boolean;
	setHeight?: boolean;
}): void;
/**
 * Calculates the available height for an element to fill its parent.
 * Compat shim for `Q.layoutFillHeightValue`. Sums the outer heights of visible siblings and subtracts from the parent height, adjusting for `box-sizing`.
 * @param element - Target element or array-like collection (first element is used).
 * @returns The computed fill height in pixels (rounded from computed styles). Returns `0` if the element is not found.
 * @deprecated Use CSS flexbox or `calc()` based layouts. Kept for legacy height calculations that depend on jQuery.
 */
export declare function layoutFillHeightValue(element: HTMLElement | ArrayLike<HTMLElement>): number;
/**
 * Sets an element's height to fill the remaining vertical space in its parent.
 * Compat shim for `Q.layoutFillHeight`. Computes the value via {@link layoutFillHeightValue} and applies it as an inline `height` style.
 * @param element - Target element or array-like collection (first element is used).
 * @deprecated Prefer CSS flex / grid layouts. Kept for legacy full-height grid pages.
 */
export declare function layoutFillHeight(element: HTMLElement | ArrayLike<HTMLElement>): void;
/**
 * Determines whether the current viewport is considered a mobile view.
 * Compat helper wrapping `window.matchMedia('(max-width: 767px)')` with a fallback to `window.innerWidth < 768`.
 * @returns `true` if the viewport width is at most 767 px; otherwise `false`.
 */
export declare function isMobileView(): boolean;
/**
 * Triggers a `layout` event each time the element becomes visible.
 * Compat shim for `Q.triggerLayoutOnShow`. Uses {@link executeEverytimeWhenVisible} to fire `Fluent.trigger(element, 'layout')` on visibility transitions.
 * @param element - Target element or array-like collection (first element is used). No-op if the element is missing.
 */
export declare function triggerLayoutOnShow(element: HTMLElement | ArrayLike<HTMLElement>): void;
/**
 * Centers a jQuery UI dialog containing the given element within the viewport.
 * Compat shim for `Q.centerDialog`. Requires jQuery and jQuery UI `position`; clamps negative `left` / `top` to `0`.
 * @param el - An element inside the dialog (e.g., `.ui-dialog-content`) or the dialog element itself; array-like collections use the first element.
 * @deprecated Prefer native dialog centering or Bootstrap modal positioning. Kept for legacy jQuery UI dialogs.
 */
export declare function centerDialog(el: HTMLElement | ArrayLike<HTMLElement>): void;
/**
 * Legacy polling-based layout timer that detects size and visibility changes.
 * Compat shim for the old `Q.LayoutTimer` / `Serenity.LayoutTimer` API. Polls registered elements every ~100 ms,
 * supports optional debouncing, and fires handlers when width, height, or visibility transitions occur.
 * Prefer `ResizeObserver` or `Fluent.on(..., 'layout')` with CSS-based layouts for new code.
 * @deprecated Kept for backward compatibility with legacy `layoutFillHeight` and `triggerLayoutOnShow` callers. Use `ResizeObserver` instead.
 */
export declare namespace LayoutTimer {
	/**
	 * Captures and stores the current size of a registered element without firing its handler.
	 * Used to reset the baseline so the next poll compares against the current dimensions.
	 * @param key - Registration key returned by {@link onSizeChange} / {@link onShown} / etc.
	 */
	function store(key: number): void;
	/**
	 * Manually triggers the handler for a registration if the element is currently visible (positive width and height).
	 * Re-stores the baseline before and after invoking the handler.
	 * @param key - Registration key returned by {@link onSizeChange}.
	 */
	function trigger(key: number): void;
	/**
	 * Registers a handler invoked when the size of the element returned by `element()` changes.
	 * Polls via an internal timer; supports filtering by width / height and optional debouncing.
	 * @param element - Factory returning the target `HTMLElement` or `Window` to watch.
	 * @param handler - Callback invoked when a matching size change is detected.
	 * @param opt - Watch options.
	 * @param opt.width - When `false`, width changes are ignored. Defaults to `true`.
	 * @param opt.height - When `false`, height changes are ignored. Defaults to `true`.
	 * @param opt.debounceTimes - Number of polls to debounce before firing. `0` fires immediately. Defaults to `0`.
	 * @returns A numeric registration key that can be passed to {@link store}, {@link trigger}, or {@link off}.
	 */
	function onSizeChange(element: () => (HTMLElement | Window), handler: () => void, opt?: {
		width?: boolean;
		height?: boolean;
		debounceTimes?: number;
	}): number;
	/**
	 * Registers a handler invoked only when the width of the element changes.
	 * Convenience wrapper around {@link onSizeChange} with `height: false`.
	 * @param element - Factory returning the target `HTMLElement`.
	 * @param handler - Callback invoked on width change.
	 * @param opt - Optional debounce configuration.
	 * @param opt.debounceTimes - Number of polls to debounce before firing.
	 * @returns A numeric registration key.
	 */
	function onWidthChange(element: () => HTMLElement, handler: () => void, opt?: {
		debounceTimes?: number;
	}): number;
	/**
	 * Registers a handler invoked only when the height of the element changes.
	 * Convenience wrapper around {@link onSizeChange} with `width: false`.
	 * @param element - Factory returning the target `HTMLElement`.
	 * @param handler - Callback invoked on height change.
	 * @param opt - Optional debounce configuration.
	 * @param opt.debounceTimes - Number of polls to debounce before firing.
	 * @returns A numeric registration key.
	 */
	function onHeightChange(element: () => HTMLElement, handler: () => void, opt?: {
		debounceTimes?: number;
	}): number;
	/**
	 * Registers a handler invoked when the element becomes visible (transitions from zero to non-zero size).
	 * Wrapper around {@link onSizeChange} with both `width` and `height` set to `false` so only hidden-to-visible transitions fire.
	 * @param element - Factory returning the target `HTMLElement`.
	 * @param handler - Callback invoked when the element is shown.
	 * @param opt - Optional debounce configuration.
	 * @param opt.debounceTimes - Number of polls to debounce before firing.
	 * @returns A numeric registration key.
	 */
	function onShown(element: () => HTMLElement, handler: () => void, opt?: {
		debounceTimes?: number;
	}): number;
	/**
	 * Unregisters a handler previously registered with {@link onSizeChange} / {@link onWidthChange} / {@link onHeightChange} / {@link onShown}.
	 * Stops the internal polling timer when no registrations remain.
	 * @param key - Registration key to remove.
	 * @returns `0` for compatibility with the legacy API.
	 */
	function off(key: number): number;
}
/**
 * Executes a callback once when the element becomes visible.
 * If the element is already visible (positive `offsetWidth` / `offsetHeight`), the callback is invoked immediately and `null` is returned.
 * Otherwise registers via {@link LayoutTimer.onShown} and auto-unregisters after the first fire.
 * @param el - Target element or array-like collection (first element is used).
 * @param callback - Function to invoke when visible.
 * @returns The {@link LayoutTimer} registration key, or `null` if already visible / element missing.
 * @deprecated Prefer `IntersectionObserver` or `ResizeObserver`. Kept for legacy `triggerLayoutOnShow` compatibility.
 */
export declare function executeOnceWhenVisible(el: HTMLElement | ArrayLike<HTMLElement>, callback: Function): number | null;
/**
 * Executes a callback every time the element becomes visible.
 * Unlike {@link executeOnceWhenVisible}, the registration persists and fires on each hidden-to-visible transition.
 * @param el - Target element or array-like collection (first element is used).
 * @param callback - Function to invoke each time the element is shown.
 * @param callNowIfVisible - When `true` and the element is already visible, invokes the callback immediately before registering.
 * @returns The {@link LayoutTimer} registration key, or `null` if the element is missing.
 * @deprecated Prefer `IntersectionObserver` / `ResizeObserver`. Kept for legacy `triggerLayoutOnShow` compatibility.
 */
export declare function executeEverytimeWhenVisible(el: HTMLElement | ArrayLike<HTMLElement>, callback: Function, callNowIfVisible: boolean): number | null;
/**
 * Event payload for the `handleroute` event dispatched by {@link ClassicRouter}.
 * Extends the native {@link Event} with parsed hash-route information.
 */
export interface HandleRouteEvent extends Event {
	/** The route segment being handled for the current index (e.g., `"new"` or `"edit/5"`). */
	route: string;
	/** All route parts split by `"/+/"` from the full hash. */
	parts: string[];
	/** Zero-based index of {@link route} within {@link parts}. */
	index: number;
	/** `true` during the initial few resolves after page load; may affect handler behavior. */
	isInitial: boolean;
}
/**
 * Contract for the legacy hash-based router.
 * Compat shim for the old `Q.Router` / `Serenity.Router` singleton. The router synchronizes dialog open/close state
 * with the URL hash using `"/+/"` delimited segments and fires `handleroute` events on designated handlers.
 * @deprecated Hash-based dialog routing is legacy. Prefer explicit client-side routing or modern dialog state management. Kept for backward compatibility.
 */
export interface IClassicRouter {
	/** When `false`, all routing operations become no-ops. */
	enabled: boolean;
	/**
	 * Navigates to a new hash, optionally attempting `history.back()` when the URL matches the previous one.
	 * @param newHash - Hash string with or without leading `#`. Empty string clears the hash.
	 * @param tryBack - When `true`, uses `history.back()` if the target matches {@link ClassicRouter.oldURL}.
	 * @param silent - When `true`, suppresses the subsequent `hashchange` handling via {@link ignoreHashChange}.
	 */
	navigate(newHash: string, tryBack?: boolean, silent?: boolean): void;
	/**
	 * Replaces the current hash without adding a history entry (silent navigation).
	 * @param newHash - Target hash (with or without `#`).
	 * @param tryBack - When `true`, prefers `history.back()` if applicable.
	 */
	replace(newHash: string, tryBack?: boolean): void;
	/**
	 * Replaces only the last `"/+/"` segment of the current hash.
	 * @param newHash - Replacement for the last segment; when empty/falsy the last segment is removed.
	 * @param tryBack - When `true`, prefers `history.back()` if applicable.
	 */
	replaceLast(newHash: string, tryBack?: boolean): void;
	/**
	 * Registers a dialog open for hash tracking. The actual hash mutation is deferred until the dialog is confirmed open.
	 * @param owner - Owner element that triggered the dialog (array-like collections use the first element).
	 * @param element - Dialog content element whose `data-qroute` / `data-qprhash` attributes will be managed.
	 * @param dialogHash - Factory returning the hash segment for this dialog (e.g., `"!a1"`).
	 */
	dialog(owner: HTMLElement | ArrayLike<HTMLElement>, element: HTMLElement | ArrayLike<HTMLElement>, dialogHash: () => string): void;
	/** Regex used to heuristically detect whether a single hash segment might represent a dialog route (e.g., `new`, `edit/…`, `!…`). */
	mightBeRouteRegex: RegExp;
	/**
	 * Resolves the current (or provided) hash by closing/opening dialogs and dispatching `handleroute`.
	 * @param newHash - Hash to resolve; defaults to `window.location.hash` when omitted.
	 * @returns A status string: `"disabled"` if the router is disabled, `"skipped"` if a recent anchor click looks like a non-route hash, `"shebang"` for `!` prefixed routes, `"missinghandler"` when a handler element cannot be found, or `"calledhandler"` when a `handleroute` event was dispatched.
	 */
	resolve(newHash?: string): "disabled" | "skipped" | "shebang" | "missinghandler" | "calledhandler";
	/**
	 * Temporarily ignores the next `hashchange` event(s).
	 * @param expiration - Duration in milliseconds to ignore hash changes. Defaults to `1000`.
	 */
	ignoreHashChange(expiration?: number): void;
	/** Removes all event listeners registered by the router and releases resources. */
	destroy(): void;
}
/**
 * Legacy hash-based router that maps dialog stack to `"/+/"` delimited hash segments.
 * Compat shim for the old `Q.Router` implementation. Listens to `hashchange`, dialog open/close, and anchor clicks to keep the URL in sync with visible dialogs and to dispatch `handleroute` events.
 * @deprecated Use explicit routing or state-driven dialog management. Kept solely for backward compatibility with legacy Serenity pages.
 */
export declare class ClassicRouter implements IClassicRouter {
	private oldURL;
	private resolving;
	private autoinc;
	private ignoreHashLock;
	private ignoreHashUntil;
	private hashAnchorClickValue;
	private hashAnchorClickTime;
	enabled: boolean;
	private isEquivalentUrl;
	/** @inheritdoc */
	navigate(newHash: string, tryBack?: boolean, silent?: boolean): void;
	/** @inheritdoc */
	replace(newHash: string, tryBack?: boolean): void;
	/** @inheritdoc */
	replaceLast(newHash: string, tryBack?: boolean): void;
	private isIgnoredDialog;
	private isVisibleOrHiddenBy;
	private getVisibleOrHiddenByDialogs;
	private pendingDialogHash;
	private pendingDialogElement;
	private pendingDialogOwner;
	private pendingDialogPreHash;
	private onDialogOpen;
	/** @inheritdoc */
	dialog(owner: HTMLElement | ArrayLike<HTMLElement>, element: HTMLElement | ArrayLike<HTMLElement>, dialogHash: () => string): void;
	private resolvingPreRoute;
	private resolveIndex;
	/** @inheritdoc */
	mightBeRouteRegex: RegExp;
	/** @inheritdoc */
	resolve(newHash?: string): "disabled" | "skipped" | "shebang" | "missinghandler" | "calledhandler";
	private hashChange;
	/** @inheritdoc */
	ignoreHashChange(expiration?: number): void;
	private routerOrder;
	private boundThis;
	private onDocumentDialogOpen;
	private onDocumentClick;
	private shouldTryBack;
	private closeHandler;
	constructor();
	/** @inheritdoc */
	destroy(): void;
}
/**
 * Singleton instance of the legacy hash router.
 * Compat shim for the global `Q.Router` / `Serenity.Router`. Initialized at module load and wired to `hashchange` and dialog events.
 * @deprecated Prefer not to use hash-based dialog routing in new code. Kept for legacy pages that rely on `Router.resolve()` / `Router.navigate()`.
 */
export declare const Router: IClassicRouter;
/**
 * Legacy `ScriptData` namespace compat shim.
 * Wraps the modern `../base` script-data APIs (`getScriptData`, `setScriptData`, `ensureScriptDataSync`, etc.)
 * under the old `Q.ScriptData` / `Serenity.ScriptData` surface. All members delegate to the new APIs.
 * @deprecated Prefer importing `getScriptData`, `setScriptData`, `ensureScriptDataSync`, `peekScriptData`, etc. directly from `@serenity-is/corelib`. Kept for backward compatibility.
 */
export declare namespace ScriptData {
	/** Alias for {@link canLoadScriptData}. @deprecated Use {@link canLoadScriptData} or `peekScriptData` / `getScriptDataHash` directly. */
	const canLoad: typeof canLoadScriptData;
	/** Alias for {@link ensureScriptDataSync}. @deprecated Use `ensureScriptDataSync` directly. */
	const ensure: typeof ensureScriptDataSync;
	/** Alias for {@link setScriptData}. @deprecated Use `setScriptData` directly. */
	const set: typeof setScriptData;
	/**
	 * Binds a callback to the `scriptdatachange.<name>` document event.
	 * @param name - Dynamic script name (event namespace suffix).
	 * @param onChange - Callback invoked when script data for `name` changes.
	 * @returns An unbind function that removes the listener and clears the callback, or `void` when `document` is unavailable.
	 */
	function bindToChange(name: string, onChange: () => void): void | (() => void);
	/**
	 * Synchronously reloads a dynamic script by clearing its cache and re-ensuring it.
	 * @param name - Dynamic script name.
	 * @param dynJS - When `true`, passed through to the underlying `ensure` call (legacy flag).
	 * @returns The reloaded script data.
	 * @deprecated Prefer `getScriptData(name, true)` or `getScriptDataAsync`. Kept for legacy callers.
	 */
	function reload<TData = any>(name: string, dynJS?: boolean): TData;
	/**
	 * Asynchronously reloads a dynamic script, bypassing the cache.
	 * @param name - Dynamic script name.
	 * @returns A promise resolving to the reloaded script data.
	 * @deprecated Prefer `getScriptData(name, true)` directly.
	 */
	function reloadAsync<TData = any>(name: string): Promise<TData>;
}
/**
 * Checks whether a dynamic script with the given name is available in the cache or is a registered script name.
 * Compat shim for the legacy `Q.canLoadScriptData` global; delegates to `peekScriptData` and `getScriptDataHash`.
 * @param name - Dynamic script name (e.g., `"Lookup.Administration.User"`).
 * @returns `true` if the script is already cached or its hash is registered; otherwise `false`.
 * @deprecated Prefer `peekScriptData` / `getScriptDataHash` checks or `getScriptData` directly.
 */
export declare function canLoadScriptData(name: string): boolean;
/**
 * Synchronously retrieves a lookup by key.
 * Compat shim for `Q.getLookup`; delegates to `ScriptData.ensure('Lookup.' + key)`.
 * @param key - Lookup key (e.g., `"Administration.User"`).
 * @returns The {@link Lookup} instance for the key.
 * @deprecated Prefer `getLookupAsync` or direct `getScriptData` usage. Kept for legacy synchronous callers.
 */
export declare function getLookup<TItem>(key: string): Lookup<TItem>;
/**
 * Synchronously reloads a lookup by key.
 * Compat shim for `Q.reloadLookup`; delegates to `ScriptData.reload('Lookup.' + key)`.
 * @param key - Lookup key.
 * @returns The reloaded {@link Lookup} instance.
 * @deprecated Prefer `reloadLookupAsync` or `getScriptData(key, true)`.
 */
export declare function reloadLookup<TItem = any>(key: string): Lookup<TItem>;
/**
 * Synchronously retrieves column metadata for a row/form key.
 * Compat shim for `Q.getColumns`; delegates to `getColumnsData(key).items`.
 * @param key - Columns key (usually a row or entity type name).
 * @returns The array of {@link PropertyItem} column definitions, or an empty array if not found.
 * @deprecated Prefer `getColumnsAsync` / `getColumnsScript` for async loading. Kept for legacy synchronous callers.
 */
export declare function getColumns(key: string): PropertyItem[];
/**
 * Asynchronously retrieves column metadata for a row/form key.
 * Compat shim for `Q.getColumnsAsync`; delegates to `getColumnsScript(key)`.
 * @param key - Columns key.
 * @returns A promise resolving to the array of {@link PropertyItem} column definitions.
 */
export declare function getColumnsAsync(key: string): Promise<PropertyItem[]>;
/**
 * Synchronously retrieves the full columns data object for a key.
 * Compat shim for `Q.getColumnsData`; delegates to `ScriptData.ensure('Columns.' + key)`.
 * @param key - Columns key.
 * @returns The {@link PropertyItemsData} containing `items` and related metadata.
 * @deprecated Prefer `getColumnsDataAsync` / `getColumnsScript`.
 */
export declare function getColumnsData(key: string): PropertyItemsData;
/** Alias for {@link getColumnsScript}. Compat shim for `Q.getColumnsDataAsync`. */
export declare const getColumnsDataAsync: typeof getColumnsScript;
/**
 * Synchronously retrieves form metadata for a key.
 * Compat shim for `Q.getForm`; delegates to `getFormData(key).items`.
 * @param key - Form key (usually a form type name).
 * @returns The array of {@link PropertyItem} form field definitions, or an empty array if not found.
 * @deprecated Prefer `getFormAsync` / `getFormScript` for async loading.
 */
export declare function getForm(key: string): PropertyItem[];
/**
 * Asynchronously retrieves form metadata for a key.
 * Compat shim for `Q.getFormAsync`; delegates to `getFormScript(key)`.
 * @param key - Form key.
 * @returns A promise resolving to the array of {@link PropertyItem} form field definitions.
 */
export declare function getFormAsync(key: string): Promise<PropertyItem[]>;
/**
 * Synchronously retrieves the full form data object for a key.
 * Compat shim for `Q.getFormData`; delegates to `ScriptData.ensure('Form.' + key)`.
 * @param key - Form key.
 * @returns The {@link PropertyItemsData} containing `items` and related metadata.
 * @deprecated Prefer `getFormDataAsync` / `getFormScript`.
 */
export declare function getFormData(key: string): PropertyItemsData;
/** Alias for {@link getFormScript}. Compat shim for `Q.getFormDataAsync`. */
export declare const getFormDataAsync: typeof getFormScript;
/**
 * Sets an equality filter value on a list request.
 * Compat shim for the legacy `Q.setEquality` helper. Lazily initializes `request.EqualityFilter` if needed.
 * @param request - The {@link ListRequest} whose `EqualityFilter` map will be mutated.
 * @param field - Field name to set in the equality filter.
 * @param value - Value to assign for the field (any JSON-serializable value, or `null` to clear).
 * @deprecated Prefer setting `request.equalityFilter` / `EqualityFilter` directly or using modern list request builders. Kept for legacy compatibility.
 */
export declare function setEquality(request: ListRequest, field: string, value: any): void;
/**
 * Options for posting data to a Serenity service endpoint via a hidden form.
 * Compat shim for the legacy `Q.PostToServiceOptions` type.
 * @deprecated Prefer `fetch` / `serviceCall` APIs. Kept for legacy form-post integrations.
 */
export interface PostToServiceOptions {
	/** Absolute or app-relative URL to post to. When provided, takes precedence over {@link service}. Resolved via `resolveUrl`. */
	url?: string;
	/** Service identifier (e.g., `"Administration/User/List"`) resolved via `resolveServiceUrl` when {@link url} is not provided. */
	service?: string;
	/** Form `target` attribute (e.g., `"_blank"` or a frame name). When omitted the form posts in the current window. */
	target?: string;
	/** Request payload that will be JSON-stringified into a hidden `request` field. */
	request: any;
}
/**
 * Options for posting arbitrary parameters to a URL via a hidden form.
 * Compat shim for the legacy `Q.PostToUrlOptions` type.
 * @deprecated Prefer `fetch` or standard form handling. Kept for legacy file-export / report post flows.
 */
export interface PostToUrlOptions {
	/** Target URL to post to (app-relative or absolute). Resolved via `resolveUrl`. */
	url?: string;
	/** Form `target` attribute (e.g., `"_blank"`). */
	target?: string;
	/** Map of form field names to values; each entry becomes a hidden `<input>` in the posted form. */
	params: any;
}
/**
 * Posts a service request by creating and submitting a hidden form.
 * Compat shim for `Q.postToService`. Resolves the URL from `options.url` or `options.service`, injects a CSRF token when same-origin, and auto-removes the form after submission.
 * @param options - Post options including service/url, request payload, and optional target.
 * @deprecated Prefer `serviceCall` / `fetch` with JSON. Kept for legacy file-download and export scenarios that require form POST.
 */
export declare function postToService(options: PostToServiceOptions): void;
/**
 * Posts arbitrary parameters to a URL by creating and submitting a hidden form.
 * Compat shim for `Q.postToUrl`. Each key in `options.params` becomes a hidden input field.
 * @param options - Post options including target URL, params map, and optional target window/frame.
 * @deprecated Prefer `fetch` or programmatic form construction. Kept for legacy export / report flows.
 */
export declare function postToUrl(options: PostToUrlOptions): void;
/**
 * Determines whether a string ends with the specified suffix.
 * @deprecated Use {@link String.prototype.endsWith} directly — e.g. `s.endsWith(suffix)`.
 * @param s - The string to test.
 * @param suffix - The suffix to look for at the end of `s`.
 * @returns `true` if `s` ends with `suffix`; otherwise `false`.
 */
export declare function endsWith(s: string, suffix: string): boolean;
/**
 * Determines whether a string is `null`, `undefined`, or empty (`""`).
 * @deprecated Prefer a direct falsy check `!s` or `s == null || s.length === 0` over this shim.
 * @param s - The string to test; may be `null` or `undefined`.
 * @returns `true` if `s` is `null`/`undefined` or has zero length.
 */
export declare function isEmptyOrNull(s: string): boolean;
/**
 * Determines whether a string is `null`, `undefined`, empty, or whitespace-only.
 * @deprecated Prefer `!s?.trim()` over this shim.
 * @param s - The string to test; may be `null` or `undefined`.
 * @returns `true` if `s` is `null`/`undefined`, empty, or contains only whitespace.
 */
export declare function isTrimmedEmpty(s: string): boolean;
/**
 * Pads the string representation of `s` on the left to reach `len` characters.
 * @deprecated Use {@link String.prototype.padStart} directly — e.g. `String(s ?? "").padStart(len, ch)`.
 * @param s - The value to pad; `null`/`undefined` is treated as an empty string.
 * @param len - The desired total length after padding.
 * @param ch - The character to pad with. Defaults to a single space.
 * @returns The left-padded string; already-longer strings are returned unchanged.
 */
export declare function padLeft(s: string | number, len: number, ch?: string): any;
/**
 * Determines whether a string starts with the specified prefix.
 * @deprecated Use {@link String.prototype.startsWith} directly — e.g. `s.startsWith(prefix)`.
 * @param s - The string to test.
 * @param prefix - The prefix to look for at the start of `s`.
 * @returns `true` if `s` starts with `prefix`; otherwise `false`.
 */
export declare function startsWith(s: string, prefix: string): boolean;
/**
 * Collapses a string to a single line by replacing CR/LF and LF with spaces and trimming the result.
 * @param str - The input string; `null`/`undefined` is treated as an empty string.
 * @returns The single-line, trimmed string.
 */
export declare function toSingleLine(str: string): string;
/**
 * Removes trailing whitespace from a string.
 * @deprecated Use {@link String.prototype.trimEnd} / `trimRight` directly.
 * @param s - The input string; `null`/`undefined` yields `""`.
 * @returns The string without trailing whitespace.
 */
export declare const trimEnd: (s: string) => any;
/**
 * Removes leading whitespace from a string.
 * @deprecated Use {@link String.prototype.trimStart} / `trimLeft` directly.
 * @param s - The input string; `null`/`undefined` yields `""`.
 * @returns The string without leading whitespace.
 */
export declare const trimStart: (s: string) => any;
/**
 * Removes leading and trailing whitespace from a string.
 * @deprecated Use {@link String.prototype.trim} directly — this shim exists only for legacy `Q.trim` call sites.
 * @param s - The input string; `null`/`undefined` yields `undefined` (optional-chain semantics).
 * @returns The trimmed string, or `undefined` if `s` is `null`/`undefined`.
 */
export declare function trim(s: string): string;
/**
 * Trims leading and trailing whitespace, coercing `null`/`undefined` to an empty string.
 * @param s - The input string; `null`/`undefined` is treated as `""`.
 * @returns The trimmed string, or `""` if the input is `null`/`undefined`.
 */
export declare function trimToEmpty(s: string): string;
/**
 * Trims leading and trailing whitespace, returning `null` for empty or whitespace-only results.
 * @param s - The input string; `null`/`undefined` yields `null` directly.
 * @returns The trimmed string, or `null` if the input is `null`/`undefined` or trims to `""`.
 */
export declare function trimToNull(s: string): string;
/**
 * Replaces all occurrences of `find` in `str` with `replace`.
 * @deprecated Prefer {@link String.prototype.replaceAll} when targeting modern runtimes; this shim falls back to `split/join`.
 * @param str - The source string; `null`/`undefined` is treated as `""`.
 * @param find - The substring to search for. Must be a non-empty string.
 * @param replace - The replacement string.
 * @returns A new string with all occurrences replaced.
 */
export declare function replaceAll(str: string, find: string, replace: string): string;
/**
 * Left-pads the decimal representation of `n` with `"0"` to reach `len` characters.
 * @param n - The number to format; `null`/`undefined` yields `""`.
 * @param len - The desired total length of the resulting string.
 * @returns The zero-padded string.
 */
export declare function zeroPad(n: number, len: number): string;
/**
 * A plain-object dictionary mapping string keys to values of type `TItem`.
 * @deprecated Prefer {@link Record}`<string, TItem>` or {@link Map}`<string, TItem>` over this legacy alias.
 * @typeParam TItem - The type of each dictionary value.
 */
export type Dictionary<TItem> = {
	[key: string]: TItem;
};
/**
 * Returns the first argument if it is not `null`/`undefined`, otherwise the second argument.
 * @deprecated Use the nullish-coalescing operator `??` directly — e.g. `a ?? b`.
 * @param a - The preferred value; returned when it is not `null`/`undefined`.
 * @param b - The fallback value returned when `a` is `null`/`undefined`.
 * @returns `a` if `a != null`, otherwise `b`.
 */
export declare function coalesce(a: any, b: any): any;
/**
 * Determines whether a value is neither `null` nor `undefined`.
 * @deprecated Use `a != null` (or `a !== null && a !== undefined`) directly.
 * @param a - The value to test.
 * @returns `true` if `a` is not `null` and not `undefined`.
 */
export declare function isValue(a: any): boolean;
/**
 * Shallow-copies properties from `b` onto `a`, mutating `a` — equivalent to `Object.assign(a, b)`.
 * @deprecated Use {@link Object.assign} directly.
 * @typeParam T - The common object type.
 * @param a - The target object to extend (mutated and returned).
 * @param b - The source object whose own properties are copied onto `a`.
 * @returns The mutated target object `a`.
 */
export declare function extend<T = any>(a: T, b: T): T;
/**
 * Returns the current local date with the time component zeroed to midnight.
 * @returns A `Date` representing today at 00:00:00 in the local time zone.
 */
export declare let today: () => Date;
/**
 * Deep clones an object or value.
 * @param a The value to clone.
 * @returns A deep clone of the input value.
 */
export declare function deepClone<T = any>(a: T): T;
/**
 * Describes a single type member collected via the legacy {@link addTypeMember} / option-decorator mechanism.
 * Preserved for backward compatibility; prefer {@link Symbol.metadata} / `Symbol.typeInfo` where possible.
 */
export interface TypeMember {
	/** Member name (field or property name). */
	name: string;
	/** Bitmask indicating the member kind (field vs. property). */
	kind: TypeMemberKind;
	/** Optional attribute/metadata objects attached to the member. */
	attr?: any[];
	/** Optional getter method name for property members. */
	getter?: string;
	/** Optional setter method name for property members. */
	setter?: string;
}
/**
 * Bitmask discriminating type-member kinds stored in {@link TypeMember.kind}.
 * Values are powers of two so they can be combined and filtered with bitwise operators.
 */
export declare enum TypeMemberKind {
	/** A plain field member. */
	field = 4,
	/** A property member (with optional getter/setter). */
	property = 16
}
/** Gets type members including inherited ones. Optionally filters by member kinds.
 * @param type The type to get members for.
 * @param memberKinds Optional bitmask of TypeMemberKind to filter by.
 * @returns An array of TypeMember objects.
 * @remarks The members should be registered using addTypeMember function or option decorator.
 */
export declare function getTypeMembers(type: any, memberKinds?: TypeMemberKind): TypeMember[];
/**
 * Adds a new member to a type or updates an existing member.
 * @param type The type to add the member to.
 * @param member The member information to add.
 * @returns The added or updated member.
 */
export declare function addTypeMember(type: any, member: TypeMember): TypeMember;
/**
 * Gets all registered types.
 * @returns All registered types.
 */
export declare function getTypes(): any[];
/**
 * Removes all own enumerable properties from the given object.
 * @param d - The dictionary/object to clear. All own properties are deleted in place.
 */
export declare function clearKeys(d: any): void;
/**
 * Identity helper that preserves a property key's type, useful for type-safe `keyof` references.
 * @typeParam T - The type whose key is being referenced.
 * @param prop - A key of `T`.
 * @returns The same key, typed as `keyof T`.
 */
export declare function keyOf<T>(prop: keyof T): keyof T;
/**
 * Casts `instance` to `type`, throwing if the instance is not assignable to the target type.
 * @param instance - The value to cast; `null`/`undefined` is returned as-is.
 * @param type - The target {@link Type} to assert.
 * @returns `instance` typed as the target, if the runtime check passes.
 * @throws Error string when `instance` is not an instance of `type`.
 */
export declare function cast(instance: any, type: Type): any;
/**
 * Attempts to cast `instance` to `type`, returning `null` on failure instead of throwing.
 * @param instance - The value to cast.
 * @param type - The target {@link Type} to test against.
 * @returns `instance` if it is an instance of `type`; otherwise `null`.
 */
export declare function safeCast(instance: any, type: Type): any;
/**
 * Abort handler that disables further validation submission for the given validator.
 * Clears `settings.abortHandler` and replaces `settings.submitHandler` with a no-op that returns `false`.
 * Intended for use as `ValidatorOptions.abortHandler` during async form submission flows.
 * @param validator - The {@link Validator} instance whose settings should be reset to abort state.
 */
export declare function validatorAbortHandler(validator: Validator): void;
/**
 * Merges caller-supplied {@link ValidatorOptions} with Serenity's default validation behaviour.
 * Default handlers include: generic `errorPlacement` that targets `data-vx-id` / `.field` containers,
 * a `submitHandler` that prevents native submit, an `invalidHandler` that shows {@link FormValidationTexts.InvalidFormMessage},
 * expands collapsed categories/tabs and shows a tooltip on the first error, and a `success` handler that marks labels as `checked`.
 * Caller options override the defaults via `Object.assign`.
 * @param options - Optional overrides to merge on top of the defaults.
 * @returns A new {@link ValidatorOptions} object with defaults applied.
 */
export declare function validateOptions(options?: ValidatorOptions): ValidatorOptions;
/**
 * Legacy helper namespace for imperative form-validation flows.
 * Wraps the underlying {@link Validator} to provide `asyncSubmit` / `submit` patterns
 * that were used by Serenity dialogs before promise-based service calls became standard.
 */
export declare namespace ValidationHelper {
	/**
	 * Initiates an asynchronous submit flow: validates the form (if `validateBeforeSave` allows),
	 * then triggers a `submit` event so the validator's `submitHandler` invokes `submitHandler`.
	 * Sets `abortHandler` to allow cancellation via {@link validatorAbortHandler}.
	 * @param form - The form element or array-like wrapper containing the form.
	 * @param validateBeforeSave - Optional pre-validation callback; when it returns `false` the submit is cancelled.
	 * @param submitHandler - Callback invoked by the validator's `submitHandler` after validation passes.
	 * @returns `true` if the submit was initiated; `false` if aborted or pre-validation failed.
	 */
	function asyncSubmit(form: ArrayLike<HTMLElement> | HTMLElement, validateBeforeSave: () => boolean, submitHandler: () => void): boolean;
	/**
	 * Synchronously validates the form and, if valid, invokes `submitHandler` directly.
	 * Unlike {@link ValidationHelper.asyncSubmit}, this path calls `validator.form()` inline
	 * instead of triggering a `submit` event.
	 * @param form - The form element or array-like wrapper containing the form.
	 * @param validateBeforeSave - Optional pre-validation callback; when it returns `false` the submit is cancelled.
	 * @param submitHandler - Callback invoked when the form is valid.
	 * @returns `true` if validation passed and `submitHandler` was invoked; `false` otherwise.
	 */
	function submit(form: ArrayLike<HTMLElement> | HTMLElement, validateBeforeSave: () => boolean, submitHandler: () => void): boolean;
	/**
	 * Gets the {@link Validator} instance associated with the given element.
	 * @param elem - The form/element (or array-like wrapper) to look up the validator for.
	 * @returns The existing {@link Validator} instance, or `null`/`undefined` if none is attached.
	 */
	function getValidator(elem: ArrayLike<HTMLElement> | HTMLElement): Validator;
	/**
	 * Validates a single element using its associated {@link Validator}.
	 * No-ops if no validator is attached to the element's form.
	 * @param elem - The element (or array-like wrapper) to validate.
	 */
	function validateElement(elem: ArrayLike<HTMLElement> | HTMLElement): void;
}
/**
 * Applies legacy jQuery compatibility patches to the globally registered jQuery instance.
 *
 * Patches applied (when applicable):
 * - **jQuery UI Dialog fixes** — allows interaction with overlays such as dropdowns,
 *   datepickers, Select2, CKEditor and modals; suppresses tabbable focusing in mobile
 *   view; injects a FontAwesome close icon into the title bar.
 * - **cleanData patch** — invokes {@link invokeDisposingListeners} and any `disposing`
 *   event handlers before the original `$.cleanData`, then polyfills `$.toJSON` /
 *   `$.parseJSON` when missing.
 * - **CSRF token hook** — registers a global `beforeSend` via `$.ajaxSetup` that
 *   attaches the `CSRF-TOKEN` cookie as `X-CSRF-TOKEN` on same-origin requests.
 *
 * This is a compatibility shim for legacy pages that still load jQuery / jQuery UI.
 * New code should avoid a jQuery dependency.
 * @returns `true` if a jQuery instance was found and patches were applied; `false` if no jQuery is registered.
 */
export declare function jQueryPatch(): boolean;
/**
 * Type token for widgets/editors that expose a boolean value.
 * Implement {@link IBooleanValue.get_value} / {@link IBooleanValue.set_value} and register with the interface type system.
 */
export declare abstract class IBooleanValue {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface IBooleanValue {
	/** Gets the current boolean value. @returns Current value. */
	get_value(): boolean;
	/** Sets the boolean value. @param value - New value to assign. */
	set_value(value: boolean): void;
}
/**
 * Type token for dialog widgets. Implemented by dialogs that can be opened as modal or panel.
 */
export declare abstract class IDialog {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface IDialog {
	/**
	 * Opens the dialog.
	 * @param asPanel - When true, opens as an in-page panel instead of a modal dialog.
	 */
	dialogOpen(asPanel?: boolean): void;
}
/**
 * Type token for editors that expose a numeric (double) value.
 */
export declare abstract class IDoubleValue {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface IDoubleValue {
	/** Gets the current numeric value. @returns Current value (number or null/undefined). */
	get_value(): any;
	/** Sets the numeric value. @param value - New value to assign. */
	set_value(value: any): void;
}
/**
 * Type token for dialogs that can load an entity by id or instance.
 */
export declare abstract class IEditDialog {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface IEditDialog {
	/**
	 * Loads an entity into the dialog.
	 * @param entityOrId - Entity instance or primary key value.
	 * @param done - Callback invoked after successful load.
	 * @param fail - Optional callback invoked on failure. @param p1 - Error details.
	 * @returns Promise-like for the retrieve response.
	 */
	load(entityOrId: any, done: () => void, fail?: (p1: any) => void): PromiseLike<RetrieveResponse<any>>;
}
/**
 * Type token for editors that can write their value into a target object.
 */
export declare abstract class IGetEditValue {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface IGetEditValue {
	/**
	 * Writes the editor value into the target object.
	 * @param property - Property metadata for the field.
	 * @param target - Object to populate.
	 */
	getEditValue(property: PropertyItem, target: any): void;
}
/**
 * Type token for widgets that support read-only mode.
 */
export declare abstract class IReadOnly {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface IReadOnly {
	/** Gets whether the widget is read-only. @returns True if read-only. */
	get_readOnly(): boolean;
	/** Sets read-only state. @param value - True to make read-only, false to make editable. */
	set_readOnly(value: boolean): void;
}
/**
 * Type token for editors that can be populated from a source object.
 */
export declare abstract class ISetEditValue {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface ISetEditValue {
	/**
	 * Populates the editor from a source object.
	 * @param source - Object containing property values.
	 * @param property - Property metadata for the field.
	 */
	setEditValue(source: any, property: PropertyItem): void;
}
/**
 * Type token for editors that expose a string value.
 */
export declare abstract class IStringValue {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface IStringValue {
	/** Gets the current string value. @returns Current value. */
	get_value(): string;
	/** Sets the string value. @param value - New value to assign. */
	set_value(value: string): void;
}
/**
 * Type token for editors that support a required-field flag.
 */
export declare abstract class IValidateRequired {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface IValidateRequired {
	/** Gets whether a value is required. @returns True if required. */
	get_required(): boolean;
	/** Sets whether a value is required. @param value - True to require a value. */
	set_required(value: boolean): void;
}
/** Contract for group/total aggregators (avg/min/max/sum etc.). */
export interface IAggregator {
	/** Initializes state before a new group is processed. */
	init(): void;
	/** Accumulates a single item into the aggregator state. @param item - Row item. */
	accumulate(item: any): void;
	/** Writes computed totals into the group totals object. @param totals - Totals container keyed by aggregateKey. */
	storeResult(totals: IGroupTotals): void;
}
/** Built-in aggregator implementations. */
export declare namespace Aggregators {
	/** Average of a numeric field (ignores non-numeric / empty values). */
	class Avg implements IAggregator {
		readonly field: string;
		count: number;
		nonNullCount: number;
		sum: number;
		constructor(field: string);
		init(): void;
		accumulate(item: any): void;
		storeResult(groupTotals: IGroupTotals): void;
		static readonly summaryType = SummaryType.Avg;
		static readonly aggregateKey = "avg";
		static get displayName(): string;
	}
	/** Weighted average given a value field and a weight field. */
	class WeightedAvg implements IAggregator {
		readonly field: string;
		readonly weightedField: string;
		sum: number;
		weightedSum: number;
		constructor(field: string, weightedField: string);
		init(): void;
		accumulate(item: any): void;
		storeResult(groupTotals: any): void;
		static isValid(val: any): boolean;
		static readonly aggregateKey = "weightedAvg";
		static get displayName(): string;
	}
	/** Minimum of a field. */
	class Min implements IAggregator {
		readonly field: string;
		min: any;
		constructor(field: string);
		init(): void;
		accumulate(item: any): void;
		storeResult(groupTotals: any): void;
		static readonly summaryType = SummaryType.Min;
		static readonly aggregateKey = "min";
		static get displayName(): string;
	}
	/** Maximum of a field. */
	class Max implements IAggregator {
		readonly field: string;
		max: any;
		constructor(field: string);
		init(): void;
		accumulate(item: any): void;
		storeResult(groupTotals: any): void;
		static readonly summaryType = SummaryType.Max;
		static readonly aggregateKey = "max";
		static get displayName(): string;
	}
	/** Sum of a numeric field. */
	class Sum implements IAggregator {
		readonly field: string;
		sum: number;
		constructor(field: string);
		init(): void;
		accumulate(item: any): void;
		storeResult(groupTotals: any): void;
		static readonly summaryType = SummaryType.Sum;
		static readonly aggregateKey = "sum";
		static get displayName(): string;
	}
}
export interface IAggregatorConstructor {
	new (field: string, ...args: any[]): IAggregator;
	/**
	 * A unique key for the aggregator (like 'sum', 'avg', etc.). This is also used in the totals object
	 * as a property key to store the results of this aggregator.
	 */
	aggregateKey: string;
	/**
	 * A user-friendly display name for the aggregator (like "Sum", "Average", etc.)
	 */
	displayName?: string;
	/**
	 * Corresponding SummaryType enum value (like SummaryType.Sum, SummaryType.Avg, etc.),
	 * if any.
	 */
	summaryType?: SummaryType;
}
export declare namespace AggregatorTypeRegistry {
	/**
	 * Registers a new aggregator class.
	 * @param cls The aggregator class to register
	 */
	function register(cls: IAggregatorConstructor): void;
	/**
	 * Resets the registry by clearing all registered aggregators and re-registering the standard ones.
	 */
	function reset(): void;
	/**
	 * Tries to get an aggregator constructor by its SummaryType or unique key.
	 * @param aggKey The SummaryType enum value or the unique key of the aggregator (like 'sum', 'avg', etc.)
	 * @returns The corresponding aggregator constructor, or undefined if not found.
	 */
	function tryGet(aggKey: string | SummaryType): IAggregatorConstructor | undefined;
}
declare module "@serenity-is/sleekgrid" {
	interface Column<TItem = any> {
		summaryType?: SummaryType | string;
	}
}
/** Formatting helpers for aggregate / group-totals rows. */
export declare namespace AggregateFormatting {
	/**
	 * Formats a group totals cell (aggregate badge/value) based on the column's summaryType.
	 * @param ctx - Formatter context containing group totals item and column.
	 * @returns Rendered aggregate markup or empty string.
	 */
	function groupTotalsFormat(ctx: FormatterContext<IGroupTotals>): FormatterResult;
	/**
	 * Call this method to ensure that `gridDefaults.groupTotalsFormat` is set to `AggregateFormatting.groupTotalsFormat`.
	 * It only sets it when it is not already set to some value. This is normally called by `RemoteView` constructor.
	 */
	function initGridDefaults(): void;
}
/** Formatter function type that maps a formatter context to a result. @typeParam TItem - Row item type. */
export type Format<TItem = any> = (ctx: FormatterContext<TItem>) => FormatterResult;
declare module "@serenity-is/sleekgrid" {
	interface Column<TItem = any> {
		/** Fields that this column depends on for its formatting or values */
		referencedFields?: string[];
		/** Source PropertyItem from which this column was created */
		sourceItem?: PropertyItem;
		/** If false, the hide column action will be hidden for this column (column picker / via menu) */
		togglable?: boolean;
		/** If false, the move column actions will be hidden for this column (column picker / via menu) */
		movable?: boolean;
	}
}
/** Legacy formatter contract. Prefer {@link Format}. */
export interface Formatter {
	/** Formats a cell value. @param ctx - Formatter context with item/column/value/grid. @returns Formatted result. */
	format(ctx: FormatterContext): FormatterResult;
}
/** Configuration for a single grouping level. */
export interface GroupInfo<TItem> {
	/** Field name or getter for the group value. */
	getter?: string | ((item: TItem) => any);
	/** True if `getter` is a function. */
	getterIsAFn?: boolean;
	/**
	 * Formats the group header. Note: group value is in `ctx.item.value`, not `ctx.value`.
	 * @param ctx - Formatter context for the group row.
	 * @returns Formatter result.
	 */
	format?: (ctx: FormatterContext<Group<TItem>>) => FormatterResult;
	/** @deprecated Use `format` instead. @param group - Group object. @returns Formatted group title. */
	formatter?: (group: Group<TItem>) => string;
	/** Comparator for group ordering. @param a - First group. @param b - Second group. @returns Negative / zero / positive. */
	comparer?: (a: Group<TItem>, b: Group<TItem>) => number;
	/** Aggregators applied to this group level. */
	aggregators?: IAggregator[];
	/** Whether to aggregate child groups as well. */
	aggregateChildGroups?: boolean;
	/** Whether collapsed groups still show aggregates. */
	aggregateCollapsed?: boolean;
	/** Whether empty groups still show aggregates. */
	aggregateEmpty?: boolean;
	/** True if groups start collapsed. */
	collapsed?: boolean;
	/** True to render a totals row for this level. */
	displayTotalsRow?: boolean;
	/** True to calculate totals lazily. */
	lazyTotalsCalculation?: boolean;
	/** Predefined group values to ensure groups exist even without data. */
	predefinedValues?: any[];
}
/** Options for the slick pager control. */
export interface PagerOptions {
	/** Data view instance. */
	view?: any;
	/** Whether to show rows-per-page selector. */
	showRowsPerPage?: boolean;
	/** Current rows per page. */
	rowsPerPage?: number;
	/** Choices for rows-per-page selector. */
	rowsPerPageOptions?: number[];
	/** Callback when page changes. @param newPage - New page index (1-based). */
	onChangePage?: (newPage: number) => void;
	/** Callback when rows-per-page changes. @param n - New rows-per-page value. */
	onRowsPerPageChange?: (n: number) => void;
}
/** Aggregator configuration for view-level summaries. */
export interface SummaryOptions {
	/** Aggregators used for grand totals. */
	aggregators: IAggregator[];
}
/** Paging state for a remote/slick data view. */
export interface PagingOptions {
	/** Rows per page. */
	rowsPerPage?: number;
	/** Current page (1-based). */
	page?: number;
}
export interface ArgsRemoteView {
	dataView: IRemoteView;
}
export interface ArgsGroupToggle extends ArgsRemoteView {
	groupingKey: string;
	level: number;
}
export interface ArgsPagingInfo extends ArgsRemoteView {
	pagingInfo: PagingInfo;
}
export interface ArgsRowCountChanged extends ArgsRemoteView {
	previous: number;
	current: number;
}
export interface ArgsRowsChanged extends ArgsRemoteView {
	rows: number[];
}
export interface ArgsRowsOrCountChanged extends ArgsRemoteView {
	rowsDiff: number[];
	previousRowCount: number;
	currentRowCount: number;
	rowCountChanged: boolean;
	rowsChanged: boolean;
}
export interface ArgsRecalcRows extends ArgsRemoteView {
	oldRows: any[];
	newRows: any[];
}
/**
 * A data view that supports remote data loading, sorting, filtering, grouping, and paging.
 * Extends the functionality of SleekGrid's DataView with server-side data operations.
 */
export declare class RemoteView<TItem = any> implements IRemoteView<TItem> {
	private contentType;
	private dataType;
	private errormsg;
	private errorMessage;
	private filter;
	private filterCache;
	private filteredItems;
	private grandAggregators;
	private grandTotals;
	private groupingInfos;
	private groupItemMetadataProvider;
	private groups;
	private idProperty;
	private idxById;
	private itemMetadataCallback?;
	private items;
	private loading;
	private localSort;
	private method;
	private page;
	private populateCalls;
	private populateLocks;
	private prevRefreshHints;
	private refreshHints;
	private rows;
	private rowsById;
	private rowsPerPage;
	private sortAsc;
	private sortComparer;
	private suspend;
	private toggledGroupsByLevel;
	private totalCount;
	private totalRows;
	private updated;
	params: Record<string, any>;
	seekToPage: number;
	sortBy: string[];
	url: string;
	onAjaxCall: RemoteViewAjaxCallback<TItem>;
	onProcessData: RemoteViewProcessCallback<TItem>;
	onSubmit: CancellableViewCallback<TItem>;
	readonly onDataChanged: EventEmitter<ArgsRemoteView, {}>;
	readonly onDataLoaded: EventEmitter<ArgsRemoteView, {}>;
	readonly onDataLoading: EventEmitter<ArgsRemoteView, {}>;
	readonly onGroupCollapsed: EventEmitter<ArgsGroupToggle, {}>;
	readonly onGroupExpanded: EventEmitter<ArgsGroupToggle, {}>;
	readonly onPagingInfoChanged: EventEmitter<ArgsPagingInfo, {}>;
	readonly onRecalcRows: EventEmitter<ArgsRecalcRows, {}>;
	readonly onRowCountChanged: EventEmitter<ArgsRowCountChanged, {}>;
	readonly onRowsChanged: EventEmitter<ArgsRowsChanged, {}>;
	readonly onRowsOrCountChanged: EventEmitter<ArgsRowsOrCountChanged, {}>;
	constructor(options: RemoteViewOptions<TItem>);
	/** Default configuration for grouping information */
	static readonly groupingInfoDefaults: GroupInfo<any>;
	beginUpdate(): void;
	endUpdate(): void;
	/**
	 * Sets hints for the next refresh operation to optimize performance.
	 * @param hints Object containing refresh hints like isFilterNarrowing, isFilterExpanding, etc.
	 */
	setRefreshHints(hints: any): void;
	private updateIdxById;
	private ensureIdUniqueness;
	getItems(): TItem[];
	getIdPropertyName(): string;
	setItems(data: any[], newIdProperty?: string | boolean): void;
	setPagingOptions(args: PagingOptions): void;
	getPagingInfo(): PagingInfo;
	private getSortComparer;
	sort(comparer?: (a: any, b: any) => number, ascending?: boolean): void;
	getLocalSort(): boolean;
	setLocalSort(value: boolean): void;
	reSort(): void;
	getFilteredItems(): TItem[];
	getFilter(): RemoteViewFilter<TItem>;
	setFilter(filterFn: RemoteViewFilter<TItem>): void;
	getGrouping(): GroupInfo<TItem>[];
	setSummaryOptions(summary: SummaryOptions): void;
	getGrandTotals(): IGroupTotals;
	setGrouping(groupingInfo: GroupInfo<TItem> | GroupInfo<TItem>[]): void;
	getItemByIdx(i: number): any;
	getIdxById(id: any): number;
	private ensureRowsByIdCache;
	getRowByItem(item: any): number;
	getRowById(id: any): number;
	getItemById(id: any): TItem;
	/**
	 * Maps an array of items to their corresponding row indices.
	 * @param itemArray Array of items to map
	 * @returns Array of row indices
	 */
	mapItemsToRows(itemArray: any[]): number[];
	/**
	 * Maps an array of IDs to their corresponding row indices.
	 * @param idArray Array of item IDs to map
	 * @returns Array of row indices
	 */
	mapIdsToRows(idArray: any[]): any[];
	/**
	 * Maps an array of row indices to their corresponding item IDs.
	 * @param rowArray Array of row indices to map
	 * @returns Array of item IDs
	 */
	mapRowsToIds(rowArray: any[]): any[];
	updateItem(id: any, item: any): void;
	insertItem(insertBefore: number, item: any): void;
	addItem(item: any): void;
	deleteItem(id: any): void;
	sortedAddItem(item: any): void;
	sortedUpdateItem(id: any, item: any): void;
	private sortedIndex;
	getRows(): (TItem | Group<any> | GroupTotals<any>)[];
	getLength(): number;
	getItem(i: number): any;
	getItemMetadata(row: number): ItemMetadata<TItem>;
	private expandCollapseAllGroups;
	collapseAllGroups(level?: number): void;
	expandAllGroups(level?: number): void;
	private resolveLevelAndGroupingKey;
	private expandCollapseGroup;
	collapseGroup(constArgs: any[]): void;
	expandGroup(constArgs: any[]): void;
	getGroups(): Group<TItem>[];
	private getOrCreateGroup;
	private extractGroups;
	private calculateTotals;
	private addGroupTotals;
	private addTotals;
	private flattenGroupedRows;
	private batchFilter;
	private batchFilterWithCaching;
	private getFilteredAndPagedItems;
	private getRowDiffs;
	private recalc;
	refresh(): void;
	/***
	 * Wires the grid and the DataView together to keep row selection tied to item ids.
	 * This is useful since, without it, the grid only knows about rows, so if the items
	 * move around, the same rows stay selected instead of the selection moving along
	 * with the items.
	 *
	 * NOTE:  This doesn't work with cell selection model.
	 *
	 * @param sleekGrid The grid to sync selection with.
	 * @param preserveHidden Whether to keep selected items that go out of the
	 *     view due to them getting filtered out.
	 * @param preserveHiddenOnSelectionChange Whether to keep selected items
	 *     that are currently out of the view (see preserveHidden) as selected when selection
	 *     changes.
	 * @return An event that notifies when an internal list of selected row ids
	 *     changes.  This is useful since, in combination with the above two options, it allows
	 *     access to the full list selected row ids, and not just the ones visible to the grid.
	 */
	syncGridSelection(sleekGrid: ISleekGrid, preserveHidden?: boolean, preserveHiddenOnSelectionChange?: boolean): EventEmitter<any>;
	syncGridCellCssStyles(grid: ISleekGrid, key: string): void;
	addData(data: any): boolean;
	populate(): boolean;
	populateLock(): void;
	populateUnlock(): void;
	getGroupItemMetadataProvider(): GroupItemMetadataProvider;
	setGroupItemMetadataProvider(value: GroupItemMetadataProvider): void;
	getItemMetadataCallback(): (item: TItem, row: number) => ItemMetadata<TItem> | undefined;
	setItemMetadataCallback(value: (item: TItem, row: number) => ItemMetadata<TItem>): void;
	/** @deprecated Gets the ID property name, for compatibility */
	get idField(): string;
	private formatGroupValue;
}
/**
 * Options for configuring a RemoteView instance
 */
export interface RemoteViewOptions<TItem = any> {
	/** Automatically load data (call populate) on initialization */
	autoLoad?: boolean;
	/** HTTP content type for service requests */
	contentType?: string;
	/** Expected data type of the service response */
	dataType?: string;
	/** Error message to display when requests fail */
	errormsg?: string;
	/** Filter criteria or function to apply to the data */
	filter?: RemoteViewFilter<TItem>;
	/** Callback function to get metadata for individual items */
	getItemMetadata?: (item: TItem, row: number) => ItemMetadata<TItem>;
	/** Provider for group item metadata in grouped views */
	groupItemMetadataProvider?: GroupItemMetadataProvider;
	/** Name of the field containing unique item identifiers */
	idField?: string;
	/** Whether to perform sorting locally instead of server-side */
	localSort?: boolean;
	/** HTTP method to use for service requests */
	method?: string;
	/** Callback function invoked before AJAX calls are made */
	onAjaxCall?: RemoteViewAjaxCallback<TItem>;
	/** Callback function to process data received from the server */
	onProcessData?: RemoteViewProcessCallback<TItem>;
	/** Callback function invoked before submitting service requests */
	onSubmit?: CancellableViewCallback<TItem>;
	/** Additional parameters to include in service requests */
	params?: Record<string, object>;
	/** Number of rows to display per page (0 for no paging) */
	rowsPerPage?: number;
	/** Initial page number to seek to on first load */
	seekToPage?: number;
	/** Initial sort criteria for the data */
	sortBy?: string | string[];
	/** URL of the service endpoint for data requests */
	url?: string;
}
/**
 * Interface for an extension of IDataView that support remote data loading
 */
export interface IRemoteView<TItem = any> extends IDataView<TItem> {
	/**
	 * Adds data received from the server to the view.
	 * @param data The response data from the server
	 */
	addData?(data: any): boolean;
	/**
	 * Adds an item to the end of the items array.
	 * @param item The item to add
	 */
	addItem?(item: any): void;
	/**
	 * Begins a batch update operation. Multiple changes can be made without triggering refreshes.
	 * Call endUpdate() to complete the batch and refresh the view.
	 */
	beginUpdate(): void;
	/**
	 * Collapses all groups at the specified level, or all levels if not specified.
	 * @param level Optional level to collapse. If not specified, applies to all levels.
	 */
	collapseAllGroups?(level?: number): void;
	/**
	 * Collapses a specific group.
	 * @param varArgs Either a Slick.Group's "groupingKey" property, or a
	 * variable argument list of grouping values denoting a unique path to the row.
	 * For example, calling collapseGroup('high', '10%') will collapse the '10%' subgroup of the 'high' group.
	 */
	collapseGroup?(varArgs: any[]): void;
	/**
	 * Deletes an item by its ID.
	 * @param id The ID of the item to delete
	 */
	deleteItem?(id: any): void;
	/**
	 * Ends a batch update operation. If this is the outermost endUpdate call,
	 * refreshes the view to reflect all changes made during the batch.
	 */
	endUpdate(): void;
	/**
	 * Expands all groups at the specified level, or all levels if not specified.
	 * @param level Optional level to expand. If not specified, applies to all levels.
	 */
	expandAllGroups?(level?: number): void;
	/**
	 * Expands a specific group.
	 * @param varArgs Either a Slick.Group's "groupingKey" property, or a
	 * variable argument list of grouping values denoting a unique path to the row.
	 * For example, calling expandGroup('high', '10%') will expand the '10%' subgroup of the 'high' group.
	 */
	expandGroup?(varArgs: any[]): void;
	/**
	 * Gets the current filter function.
	 * @returns The current filter function
	 */
	getFilter?(): RemoteViewFilter<TItem>;
	/**
	 * Gets the filtered items (after applying the current filter).
	 * @returns Array of filtered items
	 */
	getFilteredItems(): any[];
	/**
	 * Gets the current grouping configuration.
	 * @returns Array of grouping information
	 */
	getGrouping?(): GroupInfo<TItem>[];
	/**
	 * Gets the group item metadata provider.
	 * @returns The metadata provider
	 */
	getGroupItemMetadataProvider?(): GroupItemMetadataProvider;
	/**
	 * Gets the current groups.
	 * @returns Array of groups
	 */
	getGroups?(): Group<TItem>[];
	/**
	 * Gets the name of the property used as the unique identifier for items.
	 * @returns The ID property name
	 */
	getIdPropertyName(): string;
	/**
	 * Gets the index of an item by its ID.
	 * @param id The ID of the item
	 * @returns The index of the item, or undefined if not found
	 */
	getIdxById(id: any): number;
	/**
	 * Gets an item by its ID.
	 * @param id The ID of the item
	 * @returns The item with the specified ID
	 */
	getItemById(id: any): TItem;
	/**
	 * Gets an item by its index in the items array.
	 * @param i The index of the item
	 * @returns The item at the specified index
	 */
	getItemByIdx(i: number): any;
	/**
	 * Gets a callback function to retrieve item metadata. This can be used to dynamically assign CSS classes or other properties to items.
	 */
	getItemMetadataCallback(): (item: TItem, row: number) => ItemMetadata<TItem> | undefined;
	/**
	 * Gets all items in the view.
	 * @returns Array of all items
	 */
	getItems(): TItem[];
	/**
	 * Gets whether local sorting is enabled.
	 * @returns true if local sorting is enabled
	 */
	getLocalSort?(): boolean;
	/**
	 * Gets the current paging information.
	 * @returns Object containing paging state information
	 */
	getPagingInfo(): PagingInfo;
	/**
	 * Gets the row index for an item by its ID.
	 * @param id The ID of the item
	 * @returns The row index of the item
	 */
	getRowById?(id: any): number;
	/**
	 * Gets the row index for an item.
	 * @param item The item to find
	 * @returns The row index of the item
	 */
	getRowByItem?(item: any): number;
	/**
	 * Gets all rows in the view (including group rows and totals rows).
	 * @returns Array of all rows
	 */
	getRows(): (TItem | Group<any> | GroupTotals<any>)[];
	/**
	 * Inserts an item at the specified position.
	 * @param insertBefore The index to insert before
	 * @param item The item to insert
	 */
	insertItem?(insertBefore: number, item: any): void;
	/** Callback invoked before making AJAX calls */
	onAjaxCall: RemoteViewAjaxCallback<TItem>;
	/** Event fired when the underlying data changes */
	readonly onDataChanged: EventEmitter<ArgsRemoteView>;
	/** Event fired when data loading completes */
	readonly onDataLoaded: EventEmitter<ArgsRemoteView>;
	/** Event fired when data loading begins */
	readonly onDataLoading: EventEmitter<ArgsRemoteView>;
	/** Event fired when a group is collapsed */
	readonly onGroupCollapsed?: EventEmitter<ArgsGroupToggle>;
	/** Event fired when a group is expanded */
	readonly onGroupExpanded?: EventEmitter<ArgsGroupToggle>;
	/** Event fired when paging information changes */
	readonly onPagingInfoChanged: EventEmitter<ArgsPagingInfo>;
	/** Callback invoked to process data received from the server */
	onProcessData: RemoteViewProcessCallback<TItem>;
	/** Event fired when rows need to be recalculated */
	readonly onRecalcRows: EventEmitter<ArgsRecalcRows>;
	/** Event fired when the row count changes */
	readonly onRowCountChanged: EventEmitter<ArgsRowCountChanged>;
	/** Event fired when rows or count change */
	readonly onRowsOrCountChanged?: EventEmitter<ArgsRowsOrCountChanged>;
	/** Callback invoked before submitting a request, can cancel the operation */
	onSubmit: CancellableViewCallback<TItem>;
	/** Additional parameters to send with service requests */
	params: Record<string, any>;
	/**
	 * Loads data from the server using the configured URL and parameters.
	 * @returns false if the operation was cancelled or no URL is configured
	 */
	populate(): boolean;
	/**
	 * Locks population to prevent automatic data loading.
	 * Use this when you want to make multiple changes without triggering loads.
	 */
	populateLock(): void;
	/**
	 * Unlocks population. If there were pending populate calls while locked, executes them.
	 */
	populateUnlock(): void;
	/**
	 * Refresh the view by recalculating the rows and notifying changes.
	 * Note that this does not re-fetch the data from the server, use populate
	 * method for that purpose.
	 */
	refresh(): void;
	/**
	 * Re-sorts the items using the current sort settings.
	 */
	reSort(): void;
	/** The page number to seek to when loading data */
	seekToPage: number;
	/**
	 * Sets the filter function to apply to items.
	 * @param filterFn The filter function to apply
	 */
	setFilter(filterFn: RemoteViewFilter<TItem>): void;
	/**
	 * Sets the grouping configuration for the view.
	 * @param groupingInfo Grouping information or array of grouping information
	 */
	setGrouping?(groupingInfo: GroupInfo<TItem> | GroupInfo<TItem>[]): void;
	/**
	 * Sets the group item metadata provider.
	 * @param value The metadata provider to set
	 */
	setGroupItemMetadataProvider?(value: GroupItemMetadataProvider): void;
	/**
	 * Sets the items in the view and optionally changes the ID property.
	 * @param data Array of items to set
	 * @param newIdProperty Optional new ID property name, or boolean to reset
	 */
	setItems(data: any[], newIdProperty?: string | boolean): void;
	/**
	 * Sets a callback function to retrieve item metadata. This can be used to dynamically assign CSS classes or other properties to items.
	 */
	setItemMetadataCallback(value: (item: TItem, row: number) => ItemMetadata<TItem>): void;
	/**
	 * Sets whether to use local sorting. When enabled, sorting is done client-side.
	 * @param value Whether to enable local sorting
	 */
	setLocalSort?(value: boolean): void;
	/**
	 * Sets paging options and triggers a data reload if options changed.
	 * @param args The paging options to set
	 */
	setPagingOptions(args: PagingOptions): void;
	/**
	 * Sets summary/aggregation options for the view.
	 * @param summary Object containing aggregators and other summary options
	 */
	setSummaryOptions?(summary: SummaryOptions): void;
	/**
	 * Sorts the items using the specified comparer function.
	 * @param comparer Optional custom comparer function
	 * @param ascending Whether to sort in ascending order (default true)
	 */
	sort?(comparer?: (a: any, b: any) => number, ascending?: boolean): void;
	/**
	 * Adds an item in sorted order.
	 * @param item The item to add
	 */
	sortedAddItem?(item: any): void;
	/**
	 * Updates an item while maintaining sorted order.
	 * @param id The ID of the item to update
	 * @param item The new item data
	 */
	sortedUpdateItem?(id: any, item: any): void;
	/** Sort expressions for the data */
	sortBy: string[];
	/**
	 * Syncs cell CSS styles between the grid and the data view.
	 */
	syncGridCellCssStyles?(grid: ISleekGrid, key: string): void;
	/***
	 * Wires the grid and the DataView together to keep row selection tied to item ids.
	 */
	syncGridSelection?(grid: ISleekGrid, preserveHidden?: boolean, preserveHiddenOnSelectionChange?: boolean): EventEmitter<any, {}>;
	/**
	 * Updates an existing item in the view.
	 * @param id The ID of the item to update
	 * @param item The new item data
	 */
	updateItem(id: any, item: any): void;
	/** The URL to fetch data from */
	url: string;
}
/**
 * Information about the current paging state of the view
 */
export interface PagingInfo {
	/** Reference to the RemoteView instance */
	dataView: IRemoteView<any>;
	/** Current error message, if any */
	error: string;
	/** Whether data is currently being loaded */
	loading: boolean;
	/** Current page number (1-based) */
	page: number;
	/** Number of rows displayed per page */
	rowsPerPage: number;
	/** Total number of items available */
	totalCount: number;
}
/**
 * Callback function that can cancel a view operation
 * @param view The RemoteView instance
 * @returns true to continue, false to cancel
 */
export type CancellableViewCallback<TItem> = (view: IRemoteView<TItem>) => boolean | void;
/**
 * Callback function for AJAX calls made by the view
 * @param view The RemoteView instance
 * @param options The service options for the AJAX call
 * @returns true to continue, false to cancel
 */
export type RemoteViewAjaxCallback<TItem> = (view: IRemoteView<TItem>, options: ServiceOptions<ListResponse<TItem>>) => boolean | void;
/**
 * Filter function for items in the view
 * @param item The item to test
 * @param view The RemoteView instance
 * @returns true if the item should be included
 */
export type RemoteViewFilter<TItem> = (item: TItem, view: IRemoteView<TItem>) => boolean;
/**
 * Callback function for processing data received from the server
 * @param data The raw data response
 * @param view The RemoteView instance
 * @returns The processed data
 */
export type RemoteViewProcessCallback<TItem> = (data: ListResponse<TItem>, view: IRemoteView<TItem>) => ListResponse<TItem>;
/**
 * Indicates whether a dialog should show a close button in its title bar.
 */
export declare class CloseButtonAttribute extends CustomAttribute {
	value: boolean;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * @param value - True to show the close button (default `true`).
	 */
	constructor(value?: boolean);
}
/**
 * Specifies the root element tag for a widget (e.g. `"div"`, `"span"`).
 */
export declare class ElementAttribute extends CustomAttribute {
	value: string;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * @param value - Element tag name.
	 */
	constructor(value: string);
}
/**
 * Indicates whether a grid should expose the advanced filter editor.
 */
export declare class AdvancedFilteringAttribute extends CustomAttribute {
	value: boolean;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/** @param value - True to enable advanced filtering (default `true`). */
	constructor(value?: boolean);
}
/**
 * Indicates that a dialog should be maximizable.
 * @remarks Requires jQuery UI and `jquery.dialogextend.js`; not applicable to Bootstrap modals.
 */
export declare class MaximizableAttribute extends CustomAttribute {
	value: boolean;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/** @param value - True to allow maximizing (default `true`). */
	constructor(value?: boolean);
}
/**
 * Indicates that the property is an option. This is no longer used as JSX
 * does not support it, but it is kept for backward compatibility.
 */
export declare class OptionAttribute extends CustomAttribute {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
}
/**
 * Indicates that a dialog should open as a side panel by default.
 */
export declare class PanelAttribute extends CustomAttribute {
	value: boolean;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/** @param value - True to prefer panel mode (default `true`). */
	constructor(value?: boolean);
}
/**
 * Indicates whether a dialog should be resizable (jQuery UI dialogs only).
 */
export declare class ResizableAttribute extends CustomAttribute {
	value: boolean;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/** @param value - True to allow resizing (default `true`). */
	constructor(value?: boolean);
}
/**
 * Indicates that the widget should render as a static panel (plain div embedded
 * in the page without title bar / modal behavior).
 */
export declare class StaticPanelAttribute extends CustomAttribute {
	value: boolean;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/** @param value - True to render as a static panel (default `true`). */
	constructor(value?: boolean);
}
/**
 * Factory helpers for common widget attributes. Each method creates an attribute instance
 * and is flagged with `isAttributeFactory` for reflection discovery.
 */
export declare namespace Attributes {
	/** Creates an {@link AdvancedFilteringAttribute}. @param value - True to enable (default `true`). */
	function advancedFiltering(value?: boolean): AdvancedFilteringAttribute;
	/** Creates a {@link CloseButtonAttribute}. @param value - True to show close button (default `true`). */
	function closeButton(value?: boolean): CloseButtonAttribute;
	/** Creates a {@link ResizableAttribute}. @param value - True to allow resizing (default `true`). */
	function resizable(value?: boolean): ResizableAttribute;
	/** Creates a {@link MaximizableAttribute}. @param value - True to allow maximizing (default `true`). */
	function maximizable(value?: boolean): MaximizableAttribute;
	/** Creates a {@link PanelAttribute}. @param value - True to prefer panel mode (default `true`). */
	function panel(value?: boolean): PanelAttribute;
	/** Creates a {@link StaticPanelAttribute}. @param value - True for static panel (default `true`). */
	function staticPanel(value?: boolean): StaticPanelAttribute;
}
/** @deprecated Use Attributes.advancedFiltering() instead */
export declare const FilterableAttribute: typeof AdvancedFilteringAttribute;
/**
 * Operation type for data change capture (used by history / audit features).
 */
export declare enum CaptureOperationType {
	/** Fired before the operation; allows cancellation or modification. */
	Before = 0,
	/** Entity deletion. */
	Delete = 1,
	/** Entity insertion. */
	Insert = 2,
	/** Entity update. */
	Update = 3
}
/**
 * Event payload broadcast when an entity is inserted / updated / deleted via a dialog or grid.
 * Listen via bubbled `datachange` events or {@link SubDialogHelper}.
 */
export interface DataChangeInfo extends Event {
	/** Operation that triggered the event (e.g. insert / update / delete). */
	operationType: string;
	/** Primary key of the affected entity, if available. */
	entityId: any;
	/** Full entity payload, if available. */
	entity: any;
}
/**
 * Legacy decorator helpers for Serenity type registration and widget attributes.
 * @deprecated Prefer direct `static [Symbol.typeInfo] = ...` and `static { registerType(this); }` patterns.
 */
export declare namespace Decorators {
	/** Legacy decorator that registers a type via `registerType`. @returns Class decorator. */
	function registerType(): (target: Function & {
		[Symbol.typeInfo]: any;
	}, _context?: any) => void;
	/** Registers a class with an optional full name and interfaces. @param nameOrIntf - Full type name or interface list. @param intf2 - Additional interfaces. @returns Class decorator. */
	function registerClass(nameOrIntf?: string | InterfaceType[], intf2?: InterfaceType[]): (target: Function, _context?: any) => void;
	/** Registers an interface. @param nameOrIntf - Full type name or interface list. @param intf2 - Additional interfaces. @returns Class decorator. */
	function registerInterface(nameOrIntf?: string | InterfaceType[], intf2?: InterfaceType[]): (target: Function, _context?: any) => void;
	/** Registers an editor class. @param nameOrIntf - Full type name or interface list. @param intf2 - Additional interfaces. @returns Class decorator. */
	function registerEditor(nameOrIntf?: string | InterfaceType[], intf2?: InterfaceType[]): (target: Function, _context?: any) => void;
	/** Registers an enum with optional keys. @param target - Enum object. @param enumKey - Legacy lookup key. @param name - Full type name. */
	function registerEnum(target: any, enumKey?: string, name?: string): void;
	/** @deprecated Use `registerEnum` instead. @param target - Enum object. @param name - Full type name. @param enumKey - Legacy lookup key. */
	function registerEnumType(target: any, name?: string, enumKey?: string): void;
	/** Registers a formatter class. @param nameOrIntf - Full type name or interface list (default `[ISlickFormatter]`). @param intf2 - Additional interfaces. @returns Class decorator. */
	function registerFormatter(nameOrIntf?: string | InterfaceType[], intf2?: InterfaceType[]): (target: Function, _context?: any) => void;
	/** Attaches an {@link EnumKeyAttribute} to an enum. @param value - Lookup key for the enum. @returns Class decorator. */
	function enumKey(value: string): (target: Function, _context?: any) => void;
	/** Marks a property/field as a reflective option (adds {@link OptionAttribute}). @returns Property decorator. */
	function option(): (target: Object, propertyKey: string) => void;
	/** Adds a {@link CloseButtonAttribute}. @param value - True to show close button (default `true`). @returns Class decorator. */
	function closeButton(value?: boolean): (target: Function, _context?: any) => void;
	/** Adds an {@link EditorAttribute}. @returns Class decorator. */
	function editor(): (target: Function, _context?: any) => void;
	/** Adds an {@link ElementAttribute}. @param value - Element tag name. @returns Class decorator. */
	function element(value: string): (target: Function, _context?: any) => void;
	/** Adds an {@link AdvancedFilteringAttribute}. @param value - True to enable (default `true`). @returns Class decorator. */
	function advancedFiltering(value?: boolean): (target: Function, _context?: any) => void;
	/** @deprecated Use `advancedFiltering` instead */
	const filterable: typeof advancedFiltering;
	/** Adds a {@link MaximizableAttribute}. @param value - True to allow maximizing (default `true`). @returns Class decorator. */
	function maximizable(value?: boolean): (target: Function, _context?: any) => void;
	/** Adds a {@link PanelAttribute}. @param value - True to prefer panel mode (default `true`). @returns Class decorator. */
	function panel(value?: boolean): (target: Function, _context?: any) => void;
	/** Adds a {@link ResizableAttribute}. @param value - True to allow resizing (default `true`). @returns Class decorator. */
	function resizable(value?: boolean): (target: Function, _context?: any) => void;
	/**
	 * Deprecated as all dialogs are responsive.
	 * @deprecated This is no longer used as all dialogs are responsive.
	 */
	function responsive(value?: boolean): (target: Function, _context?: any) => void;
	/** Adds a {@link StaticPanelAttribute}. @param value - True for static panel (default `true`). @returns Class decorator. */
	function staticPanel(value?: boolean): (target: Function, _context?: any) => void;
}
/** Constructor type for dialog widgets registered with {@link DialogTypeRegistry}. */
export type DialogType = ({
	new (props?: any): IDialog & {
		init?: () => void;
	};
});
declare abstract class BaseTypeRegistry<TType> {
	/** The kind of loading this registry performs for lazy loading */
	protected loadKind: string;
	/** Default suffix to strip from type names (e.g., "Editor", "Dialog") */
	protected defaultSuffix: string;
	/** Cache of registered types indexed by their keys */
	protected registeredTypes: {
		[key: string]: TType;
	};
	/**
	 * Creates a new type registry instance.
	 * @param options Configuration options for the registry
	 */
	constructor(options: {
		/** The kind of loading for lazy type loading */
		loadKind?: string;
		/** Default suffix to strip from type names */
		defaultSuffix?: string;
	});
	/**
	 * Gets a secondary type key for the given type.
	 * Only enums override this to provide legacy enum key support.
	 * @param type The type to get a secondary key for
	 * @returns The secondary key, or undefined if none
	 */
	protected getSecondaryTypeKey(type: any): string;
	/**
	 * Determines if the given type matches the criteria for this registry.
	 * Subclasses should override this to define which types they handle.
	 * @param type The type to check
	 * @returns True if the type matches this registry's criteria
	 */
	protected isMatchingType(type: any): boolean;
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
	protected searchSystemTypes(key: string): TType;
	/**
	 * Initializes the registry by scanning the global type registry
	 * and building the local cache of matching types.
	 */
	protected init(): void;
	/**
	 * Gets a type by key, throwing an error if not found.
	 * @param key The key to look up
	 * @returns The found type
	 * @throws When the type is not found
	 */
	get(key: string): TType;
	/**
	 * Gets a type by key, attempting lazy loading if not found.
	 * @param key The key to look up
	 * @returns The found type or a promise that resolves to it
	 * @throws When the type cannot be found or loaded
	 */
	getOrLoad(key: string): TType | PromiseLike<TType>;
	/**
	 * Clears the registry cache, forcing re-initialization on next access.
	 */
	reset(): void;
	/**
	 * Attempts to get a type by key without throwing errors.
	 * @param key The key to look up
	 * @returns The found type, or null if not found
	 */
	tryGet(key: string): TType;
	/**
	 * Attempts to get a type by key, with lazy loading support.
	 * @param key The key to look up
	 * @returns The found type, a promise that resolves to it, or null if not found
	 */
	tryGetOrLoad(key: string): TType | PromiseLike<TType>;
}
declare class DialogTypeRegistryImpl extends BaseTypeRegistry<DialogType> {
	constructor();
	protected isMatchingType(type: any): boolean;
	protected loadError(key: string): void;
}
/** Singleton registry for dialog types (keyed by full name and short name without `Dialog` suffix). */
export declare const DialogTypeRegistry: DialogTypeRegistryImpl;
/** Constructor type for editor widgets registered with {@link EditorTypeRegistry}. */
export type EditorType = {
	new (props?: WidgetProps<any>): Widget<any>;
};
declare class EditorTypeRegistryImpl extends BaseTypeRegistry<EditorType> {
	constructor();
	protected isMatchingType(type: any): boolean;
	protected loadError(key: string): void;
}
/** Singleton registry for editor types (keyed by full name and short name without `Editor` suffix). */
export declare const EditorTypeRegistry: EditorTypeRegistryImpl;
declare class EnumTypeRegistryImpl extends BaseTypeRegistry<object> {
	constructor();
	protected getSecondaryTypeKey(type: any): string;
	protected isMatchingType(type: any): boolean;
	protected loadError(key: string): void;
}
/** Singleton registry for enum types (keyed by full name and optional {@link EnumKeyAttribute} value). */
export declare const EnumTypeRegistry: EnumTypeRegistryImpl;
/** Constructor type for slick formatters registered with {@link FormatterTypeRegistry}. */
export type FormatterType = ({
	new (props?: any): Formatter;
});
declare class FormatterTypeRegistryImpl extends BaseTypeRegistry<FormatterType> {
	constructor();
	protected isMatchingType(type: any): boolean;
	protected loadError(key: string): void;
}
/** Singleton registry for formatter types (keyed by full name and short name without `Formatter` suffix). */
export declare const FormatterTypeRegistry: FormatterTypeRegistryImpl;
/**
 * Reflection-based helper that applies option objects to widgets using members
 * decorated with {@link OptionAttribute}. Handles PascalCase → camelCase mapping
 * and getter/setter (`get_*`/`set_*`) conventions.
 */
export declare namespace ReflectionOptionsSetter {
	/**
	 * Gets a property value via `get_<property>` or direct field access.
	 * @param o - Target object.
	 * @param property - Property name (PascalCase or camelCase).
	 * @returns Property value, or undefined if not found.
	 */
	function getPropertyValue(o: any, property: string): any;
	/**
	 * Sets a property value via `set_<property>` or direct field assignment.
	 * @param o - Target object.
	 * @param property - Property name (PascalCase or camelCase).
	 * @param value - Value to assign.
	 */
	function setPropertyValue(o: any, property: string, value: any): void;
	/**
	 * Applies an options bag to a widget instance by setting members
	 * decorated with {@link OptionAttribute}.
	 * @param target - Widget instance to configure.
	 * @param options - Options object (keys are matched case-insensitively via camelCase conversion).
	 */
	function set(target: any, options: any): void;
}
/**
 * Props describing a single toolbar button.
 */
export interface ToolButtonProps {
	/** Optional action name stored on the button's `data-action` attribute. */
	action?: string;
	/** The button's title (text or element). */
	title?: string | HTMLElement | SVGElement | MathMLElement | DocumentFragment;
	/** Optional tooltip hint shown on hover. */
	hint?: string;
	/** Optional CSS class(es) applied to the button. */
	cssClass?: string;
	/** Optional icon class name to display before the title. */
	icon?: IconClassName;
	/** Handler invoked when the button is clicked. */
	onClick?: (e: MouseEvent & {
		currentTarget: EventTarget & HTMLElement;
	}) => void;
	/** Callback invoked with the created button element. */
	ref?: (el: HTMLElement) => void;
	/** Whether the button is visible; may be a function evaluated on update. */
	visible?: boolean | (() => boolean);
	/** Whether the button is disabled; may be a function evaluated on update. */
	disabled?: boolean | (() => boolean);
}
/**
 * A toolbar button definition, extending {@link ToolButtonProps} with hotkey
 * and separator support.
 */
export interface ToolButton extends ToolButtonProps {
	/** Optional hotkey binding (e.g. "ctrl+s"). */
	hotkey?: string;
	/** Whether the browser's default hotkey behavior should be allowed. */
	hotkeyAllowDefault?: boolean;
	/** Optional context element to which the hotkey is bound. */
	hotkeyContext?: any;
	/** Whether (and where) a separator should be rendered before the button. */
	separator?: (false | true | "left" | "right" | "both");
}
/**
 * Creates a toolbar button element from the given props.
 * @param tb - The button props.
 * @returns The created button element.
 */
export declare function ToolbarButton(tb: ToolButtonProps): HTMLElement;
/**
 * Options for configuring a {@link Toolbar}.
 */
export interface ToolbarOptions {
	/** The buttons to render in the toolbar. */
	buttons?: ToolButton[];
	/** Optional default context element for hotkey bindings. */
	hotkeyContext?: any;
}
/**
 * A widget that renders a horizontal toolbar of buttons, supporting separators,
 * hotkeys and dynamic visibility/disabled state.
 * @typeParam P - Widget props type, constrained to {@link ToolbarOptions}.
 */
export declare class Toolbar<P extends ToolbarOptions = ToolbarOptions> extends Widget<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Renders the toolbar contents, creating button groups and buttons.
	 * @returns The rendered tool group element.
	 */
	protected renderContents(): any;
	/**
	 * Destroys the toolbar, removing click handlers and hotkey bindings.
	 */
	destroy(): void;
	/** The Mousetrap instance used for hotkey bindings, if any. */
	protected mouseTrap: any;
	/**
	 * Creates a button in the given container, handling separators and hotkeys.
	 * @param container - The container (or array-like of containers) to append to.
	 * @param tb - The button definition.
	 * @returns The created button element.
	 */
	createButton(container: ParentNode | ArrayLike<ParentNode>, tb: ToolButton): HTMLElement;
	/**
	 * Finds a button by its CSS class name.
	 * @param className - The button class name, optionally prefixed with `.`.
	 * @returns A {@link Fluent} wrapper for the matching button element.
	 */
	findButton(className: string): Fluent<HTMLElement>;
	/**
	 * Triggers an `updateInterface` event on all buttons so dynamic
	 * visibility/disabled functions are re-evaluated.
	 */
	updateInterface(): void;
}
/**
 * Base class for dialog widgets, providing dialog/modal/panel behavior,
 * validation, tabs, and toolbar integration.
 * @typeParam P - Widget props type.
 */
export declare class BaseDialog<P> extends Widget<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLDivElement;
	protected tabs: Fluent<HTMLElement>;
	protected toolbar: Toolbar;
	protected validator: any;
	protected dialog: Dialog;
	/**
	 * Creates a base dialog widget.
	 * @param props - Widget props forwarded to the base widget.
	 */
	constructor(props?: WidgetProps<P>);
	/**
	 * Cleans up tabs, toolbar, validator, and dialog resources.
	 */
	destroy(): void;
	/**
	 * Hook for subclasses to add CSS classes; the class goes to the dialog/modal/panel element.
	 */
	protected addCssClass(): void;
	/**
	 * Returns the initial dialog title.
	 * @returns The initial title text.
	 */
	protected getInitialDialogTitle(): string;
	/**
	 * Whether the dialog renders as a static panel.
	 * @returns True when static.
	 */
	protected isStaticPanel(): boolean;
	/**
	 * Returns the options used to create the underlying dialog.
	 * @returns Dialog options.
	 */
	protected getDialogOptions(): DialogOptions;
	/**
	 * Initializes the underlying dialog element.
	 */
	protected initDialog(): void;
	/**
	 * Initializes jQuery UI dialog-specific behavior.
	 */
	protected initUIDialog(): void;
	/**
	 * Opens the dialog, optionally as a panel.
	 * @param asPanel - When true, opens as a panel instead of a modal dialog.
	 */
	dialogOpen(asPanel?: boolean): void;
	/**
	 * Hook invoked when the dialog opens; focuses the first input and arranges layout.
	 */
	protected onDialogOpen(): void;
	/** Attaches a dialog/modal/panel close event handler. See Dialog.close for more info. */
	onClose(handler: (result?: string, e?: Event) => void, opt?: {
		before?: boolean;
		oneOff?: boolean;
	}): void;
	/** Attaches a dialog/modal/panel open event handler. See Dialog.open for more info. */
	onOpen(handler: (e?: Event) => void, opt?: {
		before?: boolean;
		oneOff?: boolean;
	}): void;
	/**
	 * Returns the toolbar buttons for this dialog.
	 * @returns Tool button definitions.
	 */
	protected getToolbarButtons(): ToolButton[];
	/**
	 * Initializes the toolbar from the Toolbar element.
	 */
	protected initToolbar(): void;
	/**
	 * Returns the validator options for the form.
	 * @returns Validator options.
	 */
	protected getValidatorOptions(): any;
	/**
	 * Initializes the form validator.
	 */
	protected initValidator(): void;
	/**
	 * Resets all validation state.
	 */
	protected resetValidation(): void;
	/**
	 * Validates the form.
	 * @returns True when the form is valid.
	 */
	protected validateForm(): boolean;
	/**
	 * Triggers layout on all elements that require it.
	 */
	arrange(): void;
	/**
	 * Hook invoked when the dialog closes; destroys the dialog and removes its element.
	 * @param result - The close result.
	 */
	protected onDialogClose(result?: string): void;
	/**
	 * Returns the dialog buttons for this dialog.
	 * @returns Dialog button definitions.
	 */
	protected getDialogButtons(): DialogButton[];
	/**
	 * Closes the dialog with the given result.
	 * @param result - The close result.
	 */
	dialogClose(result?: string): void;
	/**
	 * Returns the current dialog title.
	 * @returns The dialog title.
	 */
	get dialogTitle(): string;
	/** Sets the dialog title. */
	set dialogTitle(value: string);
	/**
	 * Initializes the tabs from the Tabs element.
	 */
	protected initTabs(): void;
	/**
	 * Handles responsive layout for the dialog.
	 */
	protected handleResponsive(): void;
}
/** @deprecated use BaseDialog */
export declare const TemplatedDialog: typeof BaseDialog;
/**
 * A single line in a filter panel, describing one filter condition.
 */
export interface FilterLine {
	/** Field name being filtered. */
	field?: string;
	/** Operator key. */
	operator?: string;
	/** Whether this line is OR-combined with the previous line. */
	isOr?: boolean;
	/** Whether this line opens a parenthesis group. */
	leftParen?: boolean;
	/** Whether this line closes a parenthesis group. */
	rightParen?: boolean;
	/** Validation error message, if any. */
	validationError?: string;
	/** The criteria expression for this line. */
	criteria?: any[];
	/** Display text for this line. */
	displayText?: string;
	/** Persisted editor state. */
	state?: any;
}
/**
 * Stores filter lines for a grid and builds criteria and display text from them.
 */
export declare class FilterStore {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates a filter store.
	 * @param fields - The filterable fields.
	 */
	constructor(fields: PropertyItem[]);
	/**
	 * Builds a criteria expression from a list of filter lines.
	 * @param items - The filter lines.
	 * @returns The criteria expression.
	 */
	static getCriteriaFor(items: FilterLine[]): any[];
	/**
	 * Builds the display text for a list of filter lines.
	 * @param items - The filter lines.
	 * @returns The display text.
	 */
	static getDisplayTextFor(items: FilterLine[]): string;
	private changed;
	private displayText;
	private fields;
	private fieldByName;
	private items;
	/**
	 * Returns the filterable fields.
	 * @returns The fields.
	 */
	get_fields(): PropertyItem[];
	/**
	 * Returns the fields by name.
	 * @returns The field map.
	 */
	get_fieldByName(): {
		[key: string]: PropertyItem;
	};
	/**
	 * Returns the filter lines.
	 * @returns The filter lines.
	 */
	get_items(): FilterLine[];
	/**
	 * Notifies listeners that the store changed.
	 */
	raiseChanged(): void;
	/**
	 * Subscribes a listener to store changes.
	 * @param listener - The listener.
	 */
	add_changed(listener: (store: FilterStore) => void): void;
	/**
	 * Unsubscribes a listener from store changes.
	 * @param listener - The listener.
	 */
	remove_changed(listener: (store: FilterStore) => void): void;
	/**
	 * Returns the active criteria for the current filter lines.
	 * @returns The criteria expression.
	 */
	get_activeCriteria(): any[];
	/**
	 * Returns the display text for the current filter lines.
	 * @returns The display text.
	 */
	get_displayText(): string;
}
/**
 * Abstraction for data grids that expose the root element, underlying SlickGrid
 * instance, remote view, and filter store.
 */
export interface IDataGrid {
	/**
	 * Returns the root DOM element of the grid widget.
	 * @returns The grid container element.
	 */
	getElement(): HTMLElement;
	/**
	 * Returns the underlying SlickGrid / SleekGrid instance.
	 * @returns The grid instance used for rendering and interaction.
	 */
	getGrid(): ISleekGrid;
	/**
	 * Returns the remote view that manages paging, sorting and server communication.
	 * @returns The remote view instance.
	 */
	getView(): IRemoteView<any>;
	/**
	 * Returns the filter store owned by the grid.
	 * @returns The current {@link FilterStore} instance.
	 */
	getFilterStore(): FilterStore;
}
/**
 * Describes a selectable quick search field.
 */
export interface QuickSearchField {
	/** Field name sent with the search request. */
	name: string;
	/** Display title shown in the field selector. */
	title: string;
}
/**
 * Arguments passed to quick search callbacks.
 */
export interface QuickSearchArgs {
	/** Name of the selected search field, if any. */
	field?: string;
	/** The search query text. */
	query: string;
	/** Callback to signal that the search completed; pass false when no results were found. */
	done: (found?: boolean) => void;
	/** When set, the search was already handled by a callback. */
	handled?: boolean;
}
/**
 * Options for the {@link QuickSearchInput} widget.
 */
export interface QuickSearchInputOptions {
	/** Delay in milliseconds before the search is triggered after typing stops. */
	typeDelay?: number;
	/** CSS class added to the parent element while a search is in progress. */
	loadingParentClass?: string;
	/** Optional list of fields the user can search within. */
	fields?: QuickSearchField[];
	/** CSS class added to the parent element when the search filters results. */
	filteredParentClass?: string;
	/** @deprecated Prefer search */
	onSearch?: (field: QuickSearchArgs["field"], query: QuickSearchArgs["query"], done: QuickSearchArgs["done"]) => void;
	/** Callback invoked before the search is executed. */
	beforeSearch?: (args: QuickSearchArgs) => void;
	/** Callback that performs the actual search. */
	search?: (args: QuickSearchArgs) => void;
}
/**
 * A text input that triggers a search after a short delay, with optional
 * field selection and loading/filtered visual states.
 * @typeParam P - Options type for the widget.
 */
export declare class QuickSearchInput<P extends QuickSearchInputOptions = QuickSearchInputOptions> extends Widget<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	readonly domNode: HTMLInputElement;
	private lastValue;
	private field;
	private fieldLink;
	private fieldChanged;
	private timer;
	/**
	 * Creates a quick search input widget.
	 * @param props - Widget props forwarded to the base widget.
	 */
	constructor(props: WidgetProps<P>);
	/**
	 * Checks whether the input value changed and schedules a search if so.
	 */
	protected checkIfValueChanged(): void;
	/**
	 * Returns the current trimmed input value.
	 * @returns The search query text.
	 */
	get_value(): string;
	/**
	 * Returns the currently selected search field.
	 * @returns The active {@link QuickSearchField}.
	 */
	get_field(): QuickSearchField;
	/**
	 * Sets the active search field and refreshes the placeholder.
	 * @param value - The field to select.
	 */
	set_field(value: QuickSearchField): void;
	/**
	 * Updates the field selector link text with the active field title.
	 */
	protected updateInputPlaceHolder(): void;
	/**
	 * Restores a previously persisted search state (text and field).
	 * @param value - The search text to restore.
	 * @param field - The search field to restore.
	 */
	restoreState(value: string, field: QuickSearchField): void;
	/**
	 * Executes the search for the given value, toggling loading/filtered states
	 * and invoking the configured search callbacks.
	 * @param value - The search query text.
	 */
	protected searchNow(value: string): void;
}
/**
 * Arguments passed to the column picker change callback when columns are
 * toggled, reordered, or restored to defaults.
 */
export type ColumnPickerChangeArgs = {
	/** Columns whose visibility was toggled. */
	toggledColumns: Column[];
	/** Whether columns were reordered. */
	reorderedColumns: boolean;
	/** Whether the default column order/visibility was restored. */
	restoredDefaults: boolean;
};
/**
 * Options for the {@link ColumnPickerDialog}.
 */
export interface ColumnPickerDialogOptions {
	/** Columns to display in the picker, or a function returning them. */
	columns?: Column[] | (() => Column[]);
	/** Default column order, or a function returning it. */
	defaultOrder?: string[] | (() => string[]);
	/** Default visible column ids, or a function returning them. */
	defaultVisible?: string[] | (() => string[]);
	/** Data grid the picker is associated with. */
	dataGrid?: IDataGrid;
	/** SleekGrid instance the picker operates on. */
	sleekGrid?: ISleekGrid;
	/** Callback invoked when the picker state changes. */
	onChange?: (args: ColumnPickerChangeArgs) => Promise<any>;
	/** Custom handler for toggling column visibility. */
	toggleColumns?: (columnIds: string[], show?: boolean) => Column[];
	/** Custom handler for reordering columns. */
	reorderColumns?: (columnIds: string[], setVisible?: string[]) => boolean;
}
/**
 * Dialog that lets users show/hide, reorder, and pin grid columns.
 * @typeParam P - Options type for the dialog.
 */
export declare class ColumnPickerDialog<P extends ColumnPickerDialogOptions = ColumnPickerDialogOptions> extends BaseDialog<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private list;
	private colById;
	private defaultOrder;
	private defaultVisible;
	private columns;
	private reorderColumnsHandler;
	private toggleColumnsHandler;
	private toggleAllCheckbox;
	private searchInput;
	private onChangeHandler;
	/**
	 * Creates a column picker dialog.
	 * @param opt - Options for the dialog.
	 */
	constructor(opt: P);
	/**
	 * Cleans up handlers and delegates to the base destroy.
	 */
	destroy(): void;
	/**
	 * Toggles visibility of the specified columns.
	 * @param columnIds - Column ids to toggle.
	 * @param show - Whether to show (true) or hide (false) the columns; defaults to toggling.
	 * @returns The columns whose visibility changed.
	 */
	protected toggleColumns(columnIds: string[], show?: boolean): Column[];
	/**
	 * Invokes the change callback with the given arguments.
	 * @param args - Change arguments.
	 * @returns Result of the change callback.
	 */
	protected onChange(args: ColumnPickerChangeArgs): PromiseLike<any>;
	/**
	 * Handles clicks on a column's visibility toggle.
	 * @param e - Mouse event.
	 */
	protected handleToggleClick(e: MouseEvent): void;
	/**
	 * Renders the dialog contents.
	 * @returns The rendered dialog content.
	 */
	protected renderContents(): any;
	/**
	 * Creates the quick search input in the dialog.
	 * @param div - Container element for the search bar.
	 */
	protected createSearch(div: HTMLElement): void;
	/**
	 * Reorders columns using the configured handler.
	 * @param columnIds - New column order.
	 * @param setVisible - Optional column ids to set visible.
	 * @param restoredDefaults - Whether this reorder restores defaults.
	 */
	protected reorderColumns(columnIds: string[], setVisible?: string[], restoredDefaults?: boolean): void;
	/**
	 * Restores the default column order and visibility.
	 */
	protected handleRestoreDefaults(): void;
	/**
	 * Handles clicks on the toggle-all checkbox.
	 */
	protected handleToggleAllClick(): void;
	/**
	 * Updates the toggle-all checkbox to reflect the current visibility state.
	 * @returns The new checked state of the toggle-all checkbox.
	 */
	protected updateToggleAllValue(): boolean;
	/**
	 * Filters the column list based on the search query.
	 * @param args - Quick search arguments.
	 */
	protected handleSearch({ query, done }: QuickSearchArgs): void;
	/**
	 * Creates a toolbar button that opens the column picker dialog.
	 * @param optOrDataGrid - Options or a data grid to derive options from.
	 * @returns Tool button definition.
	 */
	static createToolButton(optOrDataGrid: IDataGrid | ColumnPickerDialogOptions): ToolButton;
	/**
	 * Returns the dialog options, sized for the column picker.
	 * @returns Dialog options.
	 */
	protected getDialogOptions(): DialogOptions;
	/**
	 * Returns no dialog buttons; the picker uses its own controls.
	 * @returns Null.
	 */
	protected getDialogButtons(): DialogButton[];
	private getTitle;
	private isAlwaysHidden;
	private isTogglable;
	private isMovable;
	private getPinInfo;
	private createColumnItem;
	private handleSortableEnd;
	/**
	 * Creates the list items for all columns.
	 */
	protected createColumnItems(): void;
	/**
	 * Called when the dialog opens; builds the column list and focuses search.
	 */
	protected onDialogOpen(): void;
	/**
	 * Opens a column picker dialog with the given options.
	 * @param opt - Options for the dialog.
	 */
	static openDialog(opt: ColumnPickerDialogOptions): void;
}
declare class PubSub<TEvent = {}> {
	#private;
	subscribe(fn: (e: TEvent) => void): void;
	unsubscribe(fn: (e: TEvent) => void): void;
	notify(e: TEvent, opt?: {
		isCancelled: (e: TEvent) => boolean;
	}): void;
	clear(): void;
}
/**
 * Props for editor widgets, extending widget props with editor-specific options.
 * @typeParam T - Widget props type.
 */
export type EditorProps<T> = WidgetProps<T> & {
	/** Initial value for the editor. */
	initialValue?: any;
	/** Maximum input length. */
	maxLength?: number;
	/** Field name. */
	name?: string;
	/** Placeholder text. */
	placeholder?: string;
	/** Whether the field is required. */
	required?: boolean;
	/** Whether the editor is read-only. */
	readOnly?: boolean;
};
/**
 * Base class for editor widgets, providing read-only handling.
 * @typeParam P - Widget props type.
 */
export declare class EditorWidget<P> extends Widget<EditorProps<P>> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates an editor widget.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Returns whether the editor is read-only.
	 * @returns True when read-only.
	 */
	get readOnly(): boolean;
	/** Sets whether the editor is read-only. */
	set readOnly(value: boolean);
}
/**
 * Options for the {@link DateEditor}.
 */
export interface DateEditorOptions {
	/** Year range for the date picker (e.g. "-100:+50"). */
	yearRange?: string;
	/** Minimum allowed date as a string. */
	minValue?: string;
	/** Maximum allowed date as a string. */
	maxValue?: string;
	/** Whether to apply SQL min/max date bounds. */
	sqlMinMax?: boolean;
}
/**
 * An editor that renders a date input with a date picker.
 * @typeParam P - Widget props type.
 */
export declare class DateEditor<P extends DateEditorOptions = DateEditorOptions> extends EditorWidget<P> implements IStringValue, IReadOnly {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	readonly domNode: HTMLInputElement;
	/**
	 * Creates a date editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Sets the value to today's date.
	 * @param triggerChange - When true, triggers a change event.
	 */
	setToToday(triggerChange?: boolean): void;
	/**
	 * Cleans up the date picker instance.
	 */
	destroy(): void;
	/**
	 * Returns the current date value in "yyyy-MM-dd" format.
	 * @returns The date value, or null when empty.
	 */
	get_value(): string;
	/**
	 * Returns the current date value.
	 * @returns The date value.
	 */
	get value(): string;
	/**
	 * Sets the date value.
	 * @param value - The date value to set.
	 */
	set_value(value: string): void;
	/** Sets the date value. */
	set value(v: string);
	private get_valueAsDate;
	/**
	 * Returns the current date value as a Date.
	 * @returns The date value.
	 */
	get valueAsDate(): Date;
	private set_valueAsDate;
	/** Sets the date value as a Date. */
	set valueAsDate(v: Date);
	/**
	 * Returns whether the editor is read-only.
	 * @returns True when read-only.
	 */
	get_readOnly(): boolean;
	/**
	 * Sets whether the editor is read-only.
	 * @param value - True to enable read-only mode.
	 */
	set_readOnly(value: boolean): void;
	/**
	 * Returns the minimum allowed date value.
	 * @returns The minimum value.
	 */
	get_minValue(): string;
	/**
	 * Sets the minimum allowed date value.
	 * @param value - The minimum value.
	 */
	set_minValue(value: string): void;
	/**
	 * Returns the maximum allowed date value.
	 * @returns The maximum value.
	 */
	get_maxValue(): string;
	/**
	 * Sets the maximum allowed date value.
	 * @param value - The maximum value.
	 */
	set_maxValue(value: string): void;
	/**
	 * Returns the minimum allowed date as a Date.
	 * @returns The minimum date.
	 */
	get_minDate(): Date;
	/**
	 * Sets the minimum allowed date as a Date.
	 * @param value - The minimum date.
	 */
	set_minDate(value: Date): void;
	/**
	 * Returns the maximum allowed date as a Date.
	 * @returns The maximum date.
	 */
	get_maxDate(): Date;
	/**
	 * Sets the maximum allowed date as a Date.
	 * @param value - The maximum date.
	 */
	set_maxDate(value: Date): void;
	/**
	 * Whether SQL min/max date bounds are applied.
	 * @returns True when SQL bounds are set.
	 */
	get_sqlMinMax(): boolean;
	/**
	 * Sets whether SQL min/max date bounds are applied.
	 * @param value - True to apply SQL bounds.
	 */
	set_sqlMinMax(value: boolean): void;
	/** Handles date input change events. */
	static dateInputChange: (e: Event) => void;
	/** Handles date input keyup events. */
	static dateInputKeyup(e: KeyboardEvent): void;
	static useFlatpickr: boolean;
	/**
	 * Returns the flatpickr options for the given input.
	 * @param input - The input element.
	 * @returns Flatpickr options.
	 */
	getFlatpickrOptions(input: HTMLElement): any;
	createFlatPickrTrigger(): HTMLElement;
	static uiPickerZIndexWorkaround(el: HTMLElement | ArrayLike<HTMLElement>): void;
}
/** The combobox provider type. */
export type ComboboxType = "select2";
/** Result of a combobox formatter. */
export type ComboboxFormatResult = string | Element | DocumentFragment;
/**
 * A single item in a combobox.
 * @typeParam TSource - The source item type.
 */
export interface ComboboxItem<TSource = any> {
	/** Item id. */
	id?: string;
	/** Display text. */
	text?: string;
	/** The source item. */
	source?: TSource;
	/** Whether the item is disabled. */
	disabled?: boolean;
}
/**
 * Query passed to a combobox search callback.
 */
export interface ComboboxSearchQuery {
	/** The search term. */
	searchTerm?: string;
	/** List of ids to initialize the selection from. */
	idList?: string[];
	/** Number of items to skip. */
	skip?: number;
	/** Number of items to take. */
	take?: number;
	/** Whether to check for more results. */
	checkMore?: boolean;
	/** Whether this is an initial selection query. */
	initSelection?: boolean;
	/** Abort signal for cancelling the query. */
	signal?: AbortSignal;
}
/**
 * Result of a combobox search.
 * @typeParam TItem - The item type.
 */
export interface ComboboxSearchResult<TItem> {
	/** The matching items. */
	items: TItem[];
	/** Whether there are more results. */
	more: boolean;
}
/**
 * Options for the {@link Combobox}.
 * @typeParam TSource - The source item type.
 */
export interface ComboboxOptions<TSource = any> {
	/** Whether the selection can be cleared. */
	allowClear?: boolean;
	/** Callback that creates a search choice for arbitrary values. */
	createSearchChoice?: (s: string) => ComboboxItem<TSource>;
	/** The element to attach the combobox to. */
	element?: HTMLInputElement | HTMLSelectElement | Element[];
	/** Allow arbitrary values for items. */
	arbitraryValues?: boolean;
	/** Formatter for the selected item. */
	formatSelection?: (p1: ComboboxItem<TSource>) => ComboboxFormatResult;
	/** Formatter for result items. */
	formatResult?: (p1: ComboboxItem<TSource>) => ComboboxFormatResult;
	/** Minimum results required to show the search box. */
	minimumResultsForSearch?: number;
	/** Whether multiple items can be selected. */
	multiple?: boolean;
	/** Page size to use while loading or displaying results. */
	pageSize?: number;
	/** Placeholder text. */
	placeholder?: string;
	/** Callback to get options specific to the combobox provider type. */
	providerOptions?: (type: ComboboxType, opt: ComboboxOptions) => any;
	/** Callback that performs the search. */
	search?: (query: ComboboxSearchQuery) => (PromiseLike<ComboboxSearchResult<ComboboxItem<TSource>>> | ComboboxSearchResult<ComboboxItem<TSource>>);
	/** Type delay for searching, default is 200. */
	typeDelay?: number;
}
/**
 * A combobox widget that provides searchable selection over a set of items.
 * @typeParam TItem - The item type.
 */
export declare class Combobox<TItem = any> {
	private el;
	/** Default combobox options. */
	static defaults: ComboboxOptions;
	constructor(opt: ComboboxOptions);
	private createSelect2;
	/**
	 * Aborts any pending search query.
	 */
	abortPendingQuery(): void;
	/**
	 * Aborts any pending initial selection query.
	 */
	abortInitSelection(): void;
	/**
	 * Disposes the combobox and cleans up its resources.
	 */
	dispose(): void;
	/**
	 * Returns the combobox container element.
	 * @returns The container element.
	 */
	get container(): HTMLElement;
	/**
	 * Returns the combobox provider type.
	 * @returns The provider type, or null.
	 */
	get type(): ComboboxType;
	/**
	 * Whether the combobox allows multiple selection.
	 * @returns True when multiple.
	 */
	get isMultiple(): boolean;
	/**
	 * Returns the first selected item.
	 * @returns The selected item.
	 */
	getSelectedItem(): ComboboxItem;
	/**
	 * Returns all selected items.
	 * @returns The selected items.
	 */
	getSelectedItems(): ComboboxItem[];
	/**
	 * Returns the current value as a comma-separated string.
	 * @returns The value.
	 */
	getValue(): string;
	/**
	 * Returns the current values as an array.
	 * @returns The values.
	 */
	getValues(): string[];
	/**
	 * Sets the current value.
	 * @param value - The value to set.
	 * @param triggerChange - When true, triggers a change event.
	 */
	setValue(value: string, triggerChange?: boolean): void;
	/**
	 * Sets the current values.
	 * @param value - The values to set.
	 * @param triggerChange - When true, triggers a change event.
	 */
	setValues(value: string[], triggerChange?: boolean): void;
	/**
	 * Closes the dropdown.
	 */
	closeDropdown(): void;
	/**
	 * Opens the dropdown.
	 */
	openDropdown(): void;
	/**
	 * Returns the combobox instance attached to an element, or null.
	 * @param el - The element or collection.
	 * @returns The combobox instance, or null.
	 */
	static getInstance(el: Element | ArrayLike<Element>): Combobox;
}
/**
 * Strips diacritics from a string for accent-insensitive searching.
 * @param str - The string to process.
 * @returns The string with diacritics removed.
 */
export declare function stripDiacritics(str: string): string;
/**
 * Links a widget to a parent widget so that it reacts to the parent's changes,
 * typically used for cascading select editors.
 * @typeParam TParent - The parent widget type.
 */
export declare class CascadedWidgetLink<TParent extends Widget<any>> {
	private parentType;
	private widget;
	private parentChange;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates a cascaded widget link.
	 * @param parentType - Constructor of the parent widget type.
	 * @param widget - The child widget to link.
	 * @param parentChange - Callback invoked when the parent changes.
	 */
	constructor(parentType: {
		new (...args: any[]): TParent;
	}, widget: Widget<any>, parentChange: (p1: TParent) => void);
	private _parentID;
	private _parentNode?;
	/**
	 * Binds the link to the parent widget and subscribes to its change event.
	 * @returns The parent widget, or null if not found.
	 */
	bind(): TParent;
	/**
	 * Unbinds the link from the parent widget.
	 * @returns The parent node, or null.
	 */
	unbind(): HTMLElement | null;
	/**
	 * Returns the parent element id.
	 * @returns The parent id.
	 */
	get_parentID(): string;
	/**
	 * Sets the parent element id and rebinds the link.
	 * @param value - The parent id.
	 */
	set_parentID(value: string): void;
}
/**
 * Common options shared by combobox-based editors.
 */
export interface ComboboxCommonOptions {
	/** Whether the selection can be cleared. */
	allowClear?: boolean;
	/** Whether multiple items can be selected. */
	delimited?: boolean;
	/** Minimum results required to show the search box. */
	minimumResultsForSearch?: any;
	/** Whether multiple items can be selected. */
	multiple?: boolean;
}
/**
 * Options for cascading and filtering combobox editors.
 */
export interface ComboboxFilterOptions {
	/** Id of the parent editor to cascade from. */
	cascadeFrom?: string;
	/** Field used for cascading. */
	cascadeField?: string;
	/** Value used for cascading. */
	cascadeValue?: any;
	/** Field used for filtering. */
	filterField?: string;
	/** Value used for filtering. */
	filterValue?: any;
}
/**
 * Options for in-place add functionality in combobox editors.
 */
export interface ComboboxInplaceAddOptions {
	/** Whether in-place add is enabled. */
	inplaceAdd?: boolean;
	/** Permission required for in-place add. */
	inplaceAddPermission?: string;
	/** Dialog type used for in-place add. */
	dialogType?: string | DialogType | PromiseLike<DialogType>;
	/** Whether arbitrary values are allowed. */
	autoComplete?: boolean;
}
/**
 * Options for the {@link ComboboxEditor}.
 */
export interface ComboboxEditorOptions extends ComboboxFilterOptions, ComboboxInplaceAddOptions, ComboboxCommonOptions {
}
/**
 * Base editor that renders a searchable combobox over a set of items.
 * @typeParam P - Widget props type.
 * @typeParam TItem - The item type.
 */
export declare class ComboboxEditor<P, TItem> extends EditorWidget<P> implements ISetEditValue, IGetEditValue, IStringValue, IReadOnly {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	readonly domNode: HTMLInputElement;
	private combobox;
	private _items;
	private _itemById;
	protected lastCreateTerm: string;
	/**
	 * Creates a combobox editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Disposes the combobox and delegates to the base destroy.
	 */
	destroy(): void;
	/**
	 * Whether the editor has an asynchronous item source.
	 * @returns True when async.
	 */
	protected hasAsyncSource(): boolean;
	/**
	 * Performs an asynchronous search.
	 * @param query - The search query.
	 * @returns A promise resolving to the search result.
	 */
	protected asyncSearch(query: ComboboxSearchQuery): PromiseLike<ComboboxSearchResult<TItem>>;
	/**
	 * Returns the type delay for searching.
	 * @returns The delay in milliseconds.
	 */
	protected getTypeDelay(): any;
	/**
	 * Returns the text for the empty item.
	 * @returns The empty item text.
	 */
	protected emptyItemText(): string;
	/**
	 * Returns the page size for paged searches.
	 * @returns The page size.
	 */
	protected getPageSize(): number;
	/**
	 * Returns the id field name.
	 * @returns The id field.
	 */
	protected getIdField(): any;
	/**
	 * Returns the id of an item.
	 * @param item - The item.
	 * @returns The item id.
	 */
	protected itemId(item: TItem): string;
	/**
	 * Returns the text field name.
	 * @returns The text field.
	 */
	protected getTextField(): any;
	/**
	 * Returns the display text of an item.
	 * @param item - The item.
	 * @returns The item text.
	 */
	protected itemText(item: TItem): string;
	/**
	 * Whether an item is disabled.
	 * @param item - The item.
	 * @returns True when disabled.
	 */
	protected itemDisabled(item: TItem): boolean;
	/**
	 * Maps an item to a combobox item.
	 * @param item - The item.
	 * @returns The combobox item.
	 */
	protected mapItem(item: TItem): ComboboxItem;
	/**
	 * Maps a list of items to combobox items.
	 * @param items - The items.
	 * @returns The combobox items.
	 */
	protected mapItems(items: TItem[]): ComboboxItem[];
	/**
	 * Whether the selection can be cleared.
	 * @returns True when clear is allowed.
	 */
	protected allowClear(): boolean;
	/**
	 * Whether multiple items can be selected.
	 * @returns True when multiple.
	 */
	protected isMultiple(): boolean;
	/**
	 * Aborts any pending search query.
	 */
	protected abortPendingQuery(): void;
	/**
	 * Returns the combobox options for this editor.
	 * @returns Combobox options.
	 */
	protected getComboboxOptions(): ComboboxOptions;
	/**
	 * Returns whether the value is delimited.
	 * @returns True when delimited.
	 */
	get_delimited(): boolean;
	/**
	 * Returns the items in the editor.
	 * @returns The items.
	 */
	get items(): ComboboxItem<TItem>[];
	/** Sets the items in the editor. */
	set items(value: ComboboxItem<TItem>[]);
	protected get itemById(): {
		[key: string]: ComboboxItem<TItem>;
	};
	protected set itemById(value: {
		[key: string]: ComboboxItem<TItem>;
	});
	/**
	 * Clears all items from the editor.
	 */
	clearItems(): void;
	/**
	 * Adds an item to the editor.
	 * @param item - The item to add.
	 */
	addItem(item: ComboboxItem<TItem>): void;
	/**
	 * Adds an option to the editor.
	 * @param key - The option id.
	 * @param text - The display text.
	 * @param source - Optional source item.
	 * @param disabled - Whether the option is disabled.
	 */
	addOption(key: string, text: string, source?: any, disabled?: boolean): void;
	/**
	 * Adds the in-place create button.
	 * @param addTitle - Title for the add button.
	 * @param editTitle - Title for the edit button.
	 */
	protected addInplaceCreate(addTitle: string, editTitle: string): void;
	/**
	 * Whether in-place add is enabled.
	 * @returns True when enabled.
	 */
	protected useInplaceAdd(): boolean;
	/**
	 * Whether arbitrary values are allowed.
	 * @returns True when auto-complete is enabled.
	 */
	protected isAutoComplete(): boolean;
	/**
	 * Returns a callback that creates a search choice for a term.
	 * @param getName - Optional callback to get the name of an item.
	 * @returns The search choice callback.
	 */
	getCreateSearchChoice(getName: (z: any) => string): (s: string) => {
		id: string;
		text: string;
	};
	/**
	 * Sets the edit value from a source object.
	 * @param source - The source object.
	 * @param property - The property item.
	 */
	setEditValue(source: any, property: PropertyItem): void;
	/**
	 * Gets the edit value into a target object.
	 * @param property - The property item.
	 * @param target - The target object.
	 */
	getEditValue(property: PropertyItem, target: any): void;
	/**
	 * Returns the combobox container element.
	 * @returns The container element.
	 */
	protected getComboboxContainer(): HTMLElement;
	/**
	 * Returns the items in the editor.
	 * @returns The items.
	 */
	protected get_items(): ComboboxItem<TItem>[];
	/**
	 * Returns the item-by-id map.
	 * @returns The item map.
	 */
	protected get_itemByKey(): {
		[key: string]: ComboboxItem<TItem>;
	};
	/**
	 * Filters items by text, matching the term against the item text.
	 * @param items - The items to filter.
	 * @param getText - Callback that returns the text of an item.
	 * @param term - The search term.
	 * @returns The filtered items.
	 */
	static filterByText<TItem>(items: TItem[], getText: (item: TItem) => string, term: string): TItem[];
	/**
	 * Returns the current value.
	 * @returns The value.
	 */
	get_value(): string;
	/**
	 * Returns the current value.
	 * @returns The value.
	 */
	get value(): string;
	/**
	 * Sets the current value.
	 * @param value - The value to set.
	 */
	set_value(value: string): void;
	/** Sets the current value. */
	set value(v: string);
	/**
	 * Returns the currently selected item.
	 * @returns The selected item, or null.
	 */
	get selectedItem(): TItem;
	/**
	 * Returns the currently selected items.
	 * @returns The selected items.
	 */
	get selectedItems(): TItem[];
	/**
	 * Returns the current values.
	 * @returns The values.
	 */
	protected get_values(): string[];
	/**
	 * Returns the current values.
	 * @returns The values.
	 */
	get values(): string[];
	/**
	 * Sets the current values.
	 * @param value - The values to set.
	 */
	protected set_values(value: string[]): void;
	/** Sets the current values. */
	set values(value: string[]);
	/**
	 * Returns the display text of the current selection.
	 * @returns The text.
	 */
	protected get_text(): string;
	/**
	 * Returns the display text of the current selection.
	 * @returns The text.
	 */
	get text(): string;
	/**
	 * Returns whether the editor is read-only.
	 * @returns True when read-only.
	 */
	get_readOnly(): boolean;
	private updateInplaceReadOnly;
	/**
	 * Sets whether the editor is read-only.
	 * @param value - True to enable read-only mode.
	 */
	set_readOnly(value: boolean): void;
	/**
	 * Returns the cascade value from a parent widget.
	 * @param parent - The parent widget.
	 * @returns The cascade value.
	 */
	protected getCascadeFromValue(parent: Widget<any>): any;
	protected cascadeLink: CascadedWidgetLink<Widget<any>>;
	/**
	 * Sets the cascade-from parent id.
	 * @param value - The parent id.
	 */
	protected setCascadeFrom(value: string): void;
	/**
	 * Returns the cascade-from parent id.
	 * @returns The parent id.
	 */
	protected get_cascadeFrom(): string;
	/**
	 * Returns the cascade-from parent id.
	 * @returns The parent id.
	 */
	get cascadeFrom(): string;
	/**
	 * Sets the cascade-from parent id.
	 * @param value - The parent id.
	 */
	protected set_cascadeFrom(value: string): void;
	/** Sets the cascade-from parent id. */
	set cascadeFrom(value: string);
	/**
	 * Returns the cascade field name.
	 * @returns The cascade field.
	 */
	protected get_cascadeField(): string;
	/**
	 * Returns the cascade field name.
	 * @returns The cascade field.
	 */
	get cascadeField(): string;
	/**
	 * Sets the cascade field name.
	 * @param value - The cascade field.
	 */
	protected set_cascadeField(value: string): void;
	/** Sets the cascade field name. */
	set cascadeField(value: string);
	/**
	 * Returns the cascade value.
	 * @returns The cascade value.
	 */
	protected get_cascadeValue(): any;
	/**
	 * Returns the cascade value.
	 * @returns The cascade value.
	 */
	get cascadeValue(): any;
	/**
	 * Sets the cascade value and refreshes items.
	 * @param value - The cascade value.
	 */
	protected set_cascadeValue(value: any): void;
	/** Sets the cascade value. */
	set cascadeValue(value: any);
	/**
	 * Returns the filter field name.
	 * @returns The filter field.
	 */
	protected get_filterField(): string;
	/**
	 * Returns the filter field name.
	 * @returns The filter field.
	 */
	get filterField(): string;
	/**
	 * Sets the filter field name.
	 * @param value - The filter field.
	 */
	protected set_filterField(value: string): void;
	/** Sets the filter field name. */
	set filterField(value: string);
	/**
	 * Returns the filter value.
	 * @returns The filter value.
	 */
	protected get_filterValue(): any;
	/**
	 * Returns the filter value.
	 * @returns The filter value.
	 */
	get filterValue(): any;
	/**
	 * Sets the filter value and refreshes items.
	 * @param value - The filter value.
	 */
	protected set_filterValue(value: any): void;
	/** Sets the filter value. */
	set filterValue(value: any);
	/**
	 * Filters items by the cascade value.
	 * @param items - The items to filter.
	 * @returns The filtered items.
	 */
	protected cascadeItems(items: TItem[]): TItem[];
	/**
	 * Filters items by the filter value.
	 * @param items - The items to filter.
	 * @returns The filtered items.
	 */
	protected filterItems(items: TItem[]): TItem[];
	/**
	 * Refreshes the items in the editor.
	 */
	protected updateItems(): void;
	/**
	 * Returns the dialog type used for in-place add.
	 * @returns The dialog type.
	 */
	protected getDialogType(): DialogType | PromiseLike<DialogType>;
	/** @deprecated Override getDialogType() instead */
	protected getDialogTypeKey(): string;
	/**
	 * Creates an edit dialog for in-place add.
	 * @param callback - Callback invoked with the created dialog.
	 */
	protected createEditDialog(callback: (dlg: IEditDialog) => void): void;
	/** Callback invoked to initialize a new entity for in-place add. */
	onInitNewEntity: (entity: TItem) => void;
	/**
	 * Initializes a new entity with cascade/filter values.
	 * @param entity - The new entity.
	 */
	protected initNewEntity(entity: TItem): void;
	/**
	 * Sets the edit dialog to read-only.
	 * @param dialog - The dialog.
	 */
	protected setEditDialogReadOnly(dialog: any): void;
	/**
	 * Hook invoked when the edit dialog data changes.
	 */
	protected editDialogDataChange(): void;
	/**
	 * Sets the search term on a new entity.
	 * @param entity - The new entity.
	 * @param term - The search term.
	 * @param dialog - The edit dialog.
	 */
	protected setTermOnNewEntity(entity: TItem, term: string, dialog: any): void;
	/**
	 * Handles the in-place create button click.
	 * @param e - The click event.
	 */
	protected inplaceCreateClick(e: Event): void;
	openDropdown(): void;
	openDialogAsPanel: boolean;
}
/**
 * An editor that renders a select of items from a static list.
 * @typeParam P - Widget props type.
 */
export declare class SelectEditor<P extends SelectEditorOptions = SelectEditorOptions> extends ComboboxEditor<P, ComboboxItem> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	/**
	 * Creates a select editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Returns the items to display in the editor.
	 * @returns The list of items.
	 */
	getItems(): any[];
	/**
	 * Returns the text for the empty option.
	 * @returns The empty option text.
	 */
	protected emptyItemText(): string;
	/**
	 * Loads the configured items into the editor.
	 */
	updateItems(): void;
}
/**
 * Options for the {@link SelectEditor}.
 */
export interface SelectEditorOptions extends ComboboxCommonOptions {
	/** Items to display; each is a value or a [value, text] pair. */
	items?: any[];
	/** Text for the empty option. */
	emptyOptionText?: string;
}
/**
 * Base widget that owns a {@link FilterStore} and reacts to its changes.
 * @typeParam P - Widget props type.
 */
export declare class FilterWidgetBase<P = {}> extends Widget<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private store;
	private onFilterStoreChanged;
	/**
	 * Creates a filter widget base.
	 * @param props - Widget props.
	 */
	constructor(props: WidgetProps<P>);
	/**
	 * Cleans up the filter store subscription.
	 */
	destroy(): void;
	/**
	 * Hook invoked when the filter store changes.
	 */
	protected filterStoreChanged(): void;
	/**
	 * Returns the filter store.
	 * @returns The filter store.
	 */
	get_store(): FilterStore;
	/**
	 * Sets the filter store and subscribes to its changes.
	 * @param value - The filter store.
	 */
	set_store(value: FilterStore): void;
}
/**
 * A bar that displays the effective filter and lets users edit or reset it.
 * @typeParam P - Widget props type.
 */
export declare class FilterDisplayBar<P = {}> extends FilterWidgetBase<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Renders the filter display bar contents.
	 * @returns The rendered content.
	 */
	protected renderContents(): any;
	/**
	 * Updates the display when the filter store changes.
	 */
	protected filterStoreChanged(): void;
	/**
	 * Creates a toolbar button that opens the filter dialog.
	 * @param opt - Tool button overrides.
	 * @returns Tool button definition.
	 */
	static createToolButton(opt: Partial<ToolButtonProps>): ToolButton;
}
/**
 * Arguments for auto register callback
 */
export interface AutoRegisterArgs<P = any, T = any> {
	pluginType: {
		new (props: P): T;
	};
	/** Set to true to cancel the auto register process for this grid / dataGrid */
	cancel: boolean;
	/** The data grid instance if available */
	dataGrid?: DataGrid<any> | null;
	/** The ISleekGrid instance */
	sleekGrid?: ISleekGrid;
	/**
	 * Options that can be modified by the callback, which are passed to the plugin/mixin constructor.
	 * Note that the options set here only applies to the auto registered instance and
	 * not to any manually created instance.
	 */
	options: Partial<P>;
}
export type AutoRegisterHandler<P = any, T = any> = (args: AutoRegisterArgs<P, T>) => void;
/**
 * Minimal storage abstraction used for grid persistence.
 * Implementations may be synchronous (localStorage) or asynchronous.
 */
export interface SettingStorage {
	/**
	 * Retrieves a stored value by key.
	 * @param key - Storage key.
	 * @returns Stored value or a promise that resolves to it.
	 */
	getItem(key: string): string | Promise<string>;
	/**
	 * Persists a value under the given key.
	 * @param key - Storage key.
	 * @param value - Value to store.
	 * @returns Void or a promise that resolves when the write completes.
	 */
	setItem(key: string, value: string): void | Promise<void>;
}
/**
 * Persisted state for a single grid column.
 */
export interface PersistedGridColumn {
	/** Column identifier (matches {@link Column.id}). */
	id: string;
	/** Persisted width in pixels. */
	width?: number;
	/** Sort order and direction; positive for ascending, negative for descending. */
	sort?: number;
	/** Whether the column is visible. */
	visible?: boolean;
	/** Frozen / pinned state of the column. */
	pin?: "start" | "end" | false;
}
/**
 * Snapshot of grid state that can be persisted and later restored.
 */
export interface PersistedGridSettings {
	/** Flags that indicate which parts of the settings were persisted. */
	flags?: GridPersistenceFlags;
	/** Column state (order, width, visibility, pinning, sort). */
	columns?: PersistedGridColumn[];
	/** Advanced filter panel items. */
	filterItems?: FilterLine[];
	/** Quick filter widget states keyed by field name. */
	quickFilters?: {
		[key: string]: any;
	};
	/** Concatenated display text for active quick filters. */
	quickFilterText?: string;
	/** Field selected in the quick search input. */
	quickSearchField?: QuickSearchField;
	/** Text entered in the quick search input. */
	quickSearchText?: string;
	/** Whether the include-deleted toggle was pressed. */
	includeDeleted?: boolean;
}
/**
 * Flags controlling which parts of grid state are persisted.
 * Unspecified flags fall back to {@link defaultGridPersistenceFlags}.
 */
export interface GridPersistenceFlags {
	/** Column pinning state. Defaults to persist unless explicitly set to false. */
	columnPinning?: boolean;
	/** Column widths. Defaults to persist unless explicitly set to false. */
	columnWidths?: boolean;
	/** Column visibility. Defaults to persist unless explicitly set to false. */
	columnVisibility?: boolean;
	/** Sort columns. Defaults to persist unless explicitly set to false. */
	sortColumns?: boolean;
	/** Filter items. Defaults to persist unless explicitly set to false. */
	filterItems?: boolean;
	/** Quick filter values. Defaults to persist unless explicitly set to false. */
	quickFilters?: boolean;
	/** Quick filter display text. Only persists when explicitly set to true. */
	quickFilterText?: boolean;
	/** Quick search input text. Only persists when explicitly set to true. */
	quickSearch?: boolean;
	/** Include deleted toggle state. Defaults to persist unless explicitly set to false. */
	includeDeleted?: boolean;
}
/** Flags that disable persistence for every grid aspect. */
export declare const omitAllGridPersistenceFlags: GridPersistenceFlags;
/**
 * Event arguments for grid persistence hooks (before/after persist and restore).
 */
export interface DataGridPersistenceEvent extends DataGridEvent {
	/** Whether this is the after phase of the operation. */
	after: boolean;
	/** Flags passed by the caller. */
	flagsArgument: GridPersistenceFlags;
	/** Default flags for the grid type. */
	flagsDefault: GridPersistenceFlags;
	/** Effective flags after merging argument and defaults. */
	flagsToUse: GridPersistenceFlags;
	/** Settings being persisted or restored. */
	settings: PersistedGridSettings;
	/** True while the grid is restoring settings. */
	readonly restoring: boolean;
	/** True while the grid is persisting settings. */
	readonly persisting: boolean;
}
/**
 * Metadata that describes a row type for grid and dialog integration.
 * Implementations are resolved from the row type registry and used for
 * permissions, identity and display name resolution.
 */
export interface IRowDefinition {
	/** Permission required to delete rows. */
	readonly deletePermission?: string;
	/** Name of the identity / primary key property. */
	readonly idProperty?: string;
	/** Permission required to insert rows. */
	readonly insertPermission?: string;
	/** Name of the boolean property that marks a row as active. */
	readonly isActiveProperty?: string;
	/** Name of the boolean property that marks a row as soft-deleted. */
	readonly isDeletedProperty?: string;
	/** Local text prefix for entity texts (display names, dialogs). */
	readonly localTextPrefix?: string;
	/** Name of the property used as the display / name field. */
	readonly nameProperty?: string;
	/** Permission required to read rows. */
	readonly readPermission?: string;
	/** Permission required to update rows. */
	readonly updatePermission?: string;
}
/**
 * Arguments passed to a quick filter handler when a list request is prepared.
 * @typeParam TWidget - The widget type that backs the quick filter.
 */
export interface QuickFilterArgs<TWidget> {
	/** Field name the filter is bound to. */
	field?: string;
	/** Widget instance for the quick filter, if created. */
	widget?: TWidget;
	/** Current list request being built; handler may mutate criteria. */
	request?: ListRequest;
	/** Equality filter value derived from the widget, if any. */
	equalityFilter?: any;
	/** Canonical value of the filter. */
	value?: any;
	/** Whether the filter is currently considered active. */
	active?: boolean;
	/** When set, the framework skips default equality-filter handling. */
	handled?: boolean;
}
/**
 * Definition for a single quick filter rendered in the grid toolbar.
 * @typeParam TWidget - Widget type that provides the filter UI.
 * @typeParam P - Props/options type for the widget.
 */
export interface QuickFilter<TWidget extends Widget<P>, P> {
	/** Field name associated with the quick filter. */
	field?: string;
	/** Widget constructor used to create the filter editor. */
	type?: {
		new (options?: P): TWidget;
		prototype: TWidget;
	};
	/** Callback invoked when the list request is prepared; may mutate the request. */
	handler?: (h: QuickFilterArgs<TWidget>) => void;
	/** Title / label shown for the filter. */
	title?: string;
	/** Options passed to the widget constructor; merged with {@link WidgetProps}. */
	options?: P & WidgetProps<{}>;
	/** Optional callback to customize the filter container element. */
	element?: (e: Fluent) => void;
	/** Callback invoked after the widget instance is created for additional setup. */
	init?: (w: TWidget) => void;
	/** When true, a visual separator is rendered before this filter. */
	separator?: boolean;
	/** Extra CSS class applied to the filter item container. */
	cssClass?: string;
	/** Restores persisted filter state into the widget. */
	loadState?: (w: TWidget, state: any) => void;
	/** Persists widget state for grid settings. */
	saveState?: (w: TWidget) => any;
	/** Returns human-readable text for the active filter display. */
	displayText?: (w: TWidget, label: string) => string;
}
/**
 * An editor that renders a date-time input with a date picker and time select.
 * @typeParam P - Widget props type.
 */
export declare class DateTimeEditor<P extends DateTimeEditorOptions = DateTimeEditorOptions> extends EditorWidget<P> implements IStringValue, IReadOnly {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	readonly domNode: HTMLInputElement;
	private time;
	private lastSetValue;
	private lastSetValueGet;
	/**
	 * Creates a date-time editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Sets the value to the current date and time.
	 * @param triggerChange - When true, triggers a change event.
	 */
	setToNow(triggerChange?: boolean): void;
	/**
	 * Cleans up the date-time picker instance.
	 */
	destroy(): void;
	/**
	 * Returns the flatpickr options for this editor.
	 * @returns Flatpickr options.
	 */
	getFlatpickrOptions(): any;
	/**
	 * Creates the flatpickr trigger button.
	 * @returns The trigger element.
	 */
	createFlatPickrTrigger(): HTMLElement;
	/**
	 * Returns the current date-time value.
	 * @returns The value, or null when empty.
	 */
	get_value(): string;
	/**
	 * Returns the current date-time value.
	 * @returns The value.
	 */
	get value(): string;
	/**
	 * Sets the date-time value.
	 * @param value - The value to set.
	 */
	set_value(value: string): void;
	private getInplaceNowText;
	private getDisplayFormat;
	/** Sets the date-time value. */
	set value(v: string);
	private get_valueAsDate;
	/**
	 * Returns the current date-time value as a Date.
	 * @returns The date-time value.
	 */
	get valueAsDate(): Date;
	private set_valueAsDate;
	/** Sets the date-time value as a Date. */
	set valueAsDate(value: Date);
	/**
	 * Returns the minimum allowed date-time value.
	 * @returns The minimum value.
	 */
	get_minValue(): string;
	/**
	 * Sets the minimum allowed date-time value.
	 * @param value - The minimum value.
	 */
	set_minValue(value: string): void;
	/**
	 * Returns the maximum allowed date-time value.
	 * @returns The maximum value.
	 */
	get_maxValue(): string;
	/**
	 * Sets the maximum allowed date-time value.
	 * @param value - The maximum value.
	 */
	set_maxValue(value: string): void;
	/**
	 * Returns the minimum allowed date-time as a Date.
	 * @returns The minimum date-time.
	 */
	get_minDate(): Date;
	/**
	 * Sets the minimum allowed date-time as a Date.
	 * @param value - The minimum date-time.
	 */
	set_minDate(value: Date): void;
	/**
	 * Returns the maximum allowed date-time as a Date.
	 * @returns The maximum date-time.
	 */
	get_maxDate(): Date;
	/**
	 * Sets the maximum allowed date-time as a Date.
	 * @param value - The maximum date-time.
	 */
	set_maxDate(value: Date): void;
	/**
	 * Whether SQL min/max date-time bounds are applied.
	 * @returns True when SQL bounds are set.
	 */
	get_sqlMinMax(): boolean;
	/**
	 * Sets whether SQL min/max date-time bounds are applied.
	 * @param value - True to apply SQL bounds.
	 */
	set_sqlMinMax(value: boolean): void;
	/**
	 * Returns whether the editor is read-only.
	 * @returns True when read-only.
	 */
	get_readOnly(): boolean;
	/**
	 * Sets whether the editor is read-only.
	 * @param value - True to enable read-only mode.
	 */
	set_readOnly(value: boolean): void;
	static roundToMinutes(date: Date, minutesStep: number): Date;
	static getTimeOptions: (fromHour: number, fromMin: number, toHour: number, toMin: number, stepMins: number) => string[];
}
export interface DateTimeEditorOptions {
	startHour?: any;
	endHour?: any;
	intervalMinutes?: any;
	minValue?: string;
	maxValue?: string;
	yearRange?: string;
	useUtc?: boolean;
	seconds?: boolean;
	inputOnly?: boolean;
	sqlMinMax?: boolean;
}
/**
 * Options for the {@link QuickFilterBar} widget.
 */
export interface QuickFilterBarOptions {
	/** Quick filter definitions to render in the bar. */
	filters: QuickFilter<Widget<any>, any>[];
	/** Optional callback that returns the display title for a filter. */
	getTitle?: (filter: QuickFilter<Widget<any>, any>) => string;
	/** Prefix used for generated element ids; defaults to the widget unique name. */
	idPrefix?: string;
}
/**
 * Per-item data attached to a quick filter element for state persistence and display.
 * @typeParam TWidget - Widget type that backs the quick filter.
 */
export interface QuickFilterItemData<TWidget> {
	/** Returns the human-readable text for the active filter display. */
	displayText?: (w: TWidget, l: string) => string;
	/** Persists the widget state for grid settings. */
	saveState?: (w: TWidget) => any;
	/** Restores persisted widget state. */
	loadState?: (w: TWidget, state: any) => void;
}
/**
 * A bar that renders quick filters for a grid, including date ranges, boolean
 * toggles, and custom filter widgets, and submits their values with list requests.
 * @typeParam P - Options type for the widget.
 */
export declare class QuickFilterBar<P extends QuickFilterBarOptions = QuickFilterBarOptions> extends Widget<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates a quick filter bar and adds all configured filters.
	 * @param props - Widget props including the filter definitions.
	 */
	constructor(props: WidgetProps<P>);
	private static readonly itemDataMap;
	/**
	 * Returns the per-item data attached to a quick filter element, if any.
	 * @param filterItem - The quick filter container element.
	 * @returns The item data, or undefined if none was attached.
	 */
	static getItemData<TWidget>(filterItem: Node): QuickFilterItemData<TWidget> | undefined;
	/**
	 * Adds a visual separator to the bar.
	 */
	addSeparator(): void;
	/**
	 * Adds a quick filter widget to the bar and wires its submit handler.
	 * @param opt - Quick filter definition.
	 * @returns The created widget instance.
	 */
	add<TWidget extends Widget<any>, TOptions>(opt: QuickFilter<TWidget, TOptions>): TWidget;
	/**
	 * Adds a date range quick filter for the specified field.
	 * @param field - Field name the filter is bound to.
	 * @param title - Optional display title.
	 * @returns The created date editor.
	 */
	addDateRange(field: string, title?: string): DateEditor;
	/**
	 * Creates a date range quick filter definition for the specified field.
	 * @param field - Field name the filter is bound to.
	 * @param title - Optional display title.
	 * @returns A quick filter definition for a date range.
	 */
	static dateRange(field: string, title?: string): QuickFilter<DateEditor, DateTimeEditorOptions>;
	/**
	 * Adds a date-time range quick filter for the specified field.
	 * @param field - Field name the filter is bound to.
	 * @param title - Optional display title.
	 * @returns The created date-time editor.
	 */
	addDateTimeRange(field: string, title?: string): DateTimeEditor;
	/**
	 * Creates a date-time range quick filter definition for the specified field.
	 * @param field - Field name the filter is bound to.
	 * @param title - Optional display title.
	 * @param useUtc - Whether the editor should use UTC values.
	 * @returns A quick filter definition for a date-time range.
	 */
	static dateTimeRange(field: string, title?: string, useUtc?: boolean): QuickFilter<DateTimeEditor, DateTimeEditorOptions>;
	/**
	 * Adds a boolean quick filter for the specified field.
	 * @param field - Field name the filter is bound to.
	 * @param title - Optional display title.
	 * @param yes - Optional text for the true option.
	 * @param no - Optional text for the false option.
	 * @returns The created select editor.
	 */
	addBoolean(field: string, title?: string, yes?: string, no?: string): SelectEditor;
	/**
	 * Creates a boolean quick filter definition for the specified field.
	 * @param field - Field name the filter is bound to.
	 * @param title - Optional display title.
	 * @param yes - Optional text for the true option.
	 * @param no - Optional text for the false option.
	 * @returns A quick filter definition for a boolean value.
	 */
	static boolean(field: string, title?: string, yes?: string, no?: string): QuickFilter<SelectEditor, SelectEditorOptions>;
	/** Callback invoked when a quick filter value changes. */
	onChange: (e: Event) => void;
	private submitHandlers;
	/**
	 * Cleans up submit handlers and delegates to the base destroy.
	 */
	destroy(): void;
	/**
	 * Invokes all registered submit handlers with the given list request.
	 * @param request - The list request being prepared.
	 */
	onSubmit(request: ListRequest): void;
	/**
	 * Finds the widget instance for a quick filter by field name.
	 * @param type - Widget constructor type.
	 * @param field - Field name of the quick filter.
	 * @returns The widget instance.
	 */
	find<TWidget>(type: {
		new (...args: any[]): TWidget;
	}, field: string): TWidget;
	/**
	 * Tries to find the widget instance for a quick filter by field name.
	 * @param type - Widget constructor type.
	 * @param field - Field name of the quick filter.
	 * @returns The widget instance, or null if not found.
	 */
	tryFind<TWidget>(type: {
		new (...args: any[]): TWidget;
	}, field: string): TWidget;
}
/**
 * Base data grid widget that renders tabular data using SleekGrid, with
 * support for columns, sorting, filtering, quick filters, paging, persistence,
 * and toolbar integration.
 * @typeParam TItem - Row type displayed in the grid.
 * @typeParam P - Widget props type.
 */
export declare class DataGrid<TItem, P = {}> extends Widget<P> implements IDataGrid, IReadOnly {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private _grid;
	private _initialSettings;
	private _layoutTimer;
	/** The title element. */
	protected titleDiv: Fluent;
	/** The toolbar widget. */
	protected toolbar: Toolbar;
	/** The advanced filter bar widget. */
	protected filterBar: FilterDisplayBar;
	/** The quick filters container element. */
	protected quickFiltersDiv: Fluent;
	/** The quick filter bar widget. */
	protected quickFiltersBar: QuickFilterBar;
	/** The container element that hosts the grid. */
	protected slickContainer: Fluent;
	/** The property items data for this grid. */
	protected propertyItemsData: PropertyItemsData;
	/** Counter tracking nested settings restoration. */
	protected restoringSettings: number;
	/** The remote view used for paging and server communication. */
	view: IRemoteView<TItem>;
	/** Whether dialogs opened from this grid should be shown as panels. */
	openDialogsAsPanel: boolean;
	/** Default options shared by all data grid instances. */
	static readonly defaultOptions: {
		columnWidthDelta: number;
		columnWidthScale: number;
		enableAdvancedFiltering: (boolean | ((grid: DataGrid<any>) => boolean));
		openDialogsAsPanel: boolean;
		rowHeight: number;
		persistenceFlags: GridPersistenceFlags;
		persistenceStorage: SettingStorage;
	};
	/** Default row height used when creating grids. */
	static get defaultRowHeight(): number;
	/** Default storage used for grid persistence. */
	static get defaultPersistenceStorage(): SettingStorage;
	/** Sets the default storage used for grid persistence. */
	static set defaultPersistenceStorage(value: SettingStorage);
	/** Default column width scale applied to all grids. */
	static get defaultColumnWidthScale(): number;
	/** Sets the default column width scale applied to all grids. */
	static set defaultColumnWidthScale(value: number);
	/** Default column width delta applied to all grids. */
	static get defaultColumnWidthDelta(): number;
	/** Sets the default column width delta applied to all grids. */
	static set defaultColumnWidthDelta(value: number);
	/** Static event raised after any grid is initialized. */
	static readonly onAfterInit: PubSub<DataGridEvent>;
	/** Raised after this grid is initialized. */
	readonly onAfterInit: PubSub<DataGridEvent>;
	/** Raised to determine whether the grid can submit its view. */
	readonly onCanSubmit: PubSub<DataGridSubmitEvent>;
	/** Raised when the grid data changes. */
	readonly onDataChanged: PubSub<DataGridEvent>;
	/** Raised while filtering items in the view. */
	readonly onFiltering: PubSub<DataGridFilteringEvent<TItem>>;
	/** Raised before/after persisting or restoring grid settings. */
	readonly onPersistence: PubSub<DataGridPersistenceEvent>;
	/** Raised when the view processes a list response. */
	readonly onProcessData: PubSub<DataGridProcessEvent<TItem>>;
	/** Raised to determine whether the view submit should proceed. */
	readonly onSubmitting: PubSub<DataGridSubmitEvent>;
	/** Raised after view parameters are prepared for submission. */
	readonly onSetViewParams: PubSub<DataGridEvent>;
	/**
	 * Creates a data grid widget.
	 * @param props - Widget props forwarded to the base widget.
	 */
	constructor(props: WidgetProps<P>);
	/**
	 * Hook invoked when the grid is registered as an auto-registering plugin.
	 * @param args - Auto-registration arguments.
	 */
	protected autoRegisteringPlugin(args: AutoRegisterArgs): void;
	private layoutTimerCallback;
	/**
	 * Called once property items are available; creates the grid, filter bar,
	 * pager, quick filters, and restores persisted settings.
	 * @param itemsData - Property items and additional items for the grid.
	 */
	protected propertyItemsReady(itemsData: PropertyItemsData): void;
	/**
	 * Hook invoked after the grid is initialized and settings are restored.
	 */
	protected afterInit(): void;
	/**
	 * Whether the grid should load property items asynchronously.
	 * @returns True when async loading is used.
	 */
	protected useAsync(): boolean;
	/**
	 * Whether the grid should use the layout timer for responsive resizing.
	 * @returns True when the layout timer is used.
	 */
	protected useLayoutTimer(): boolean;
	/**
	 * Recalculates the grid layout, handling responsive height behavior.
	 */
	protected layout(): void;
	/**
	 * Returns the initial title shown above the grid.
	 * @returns The title text, or null for no title.
	 */
	protected getInitialTitle(): string;
	/**
	 * Hook for subclasses to add extra toolbar buttons or controls.
	 */
	protected createToolbarExtensions(): void;
	/**
	 * Ensures the quick filter bar exists and returns it.
	 * @returns The quick filter bar instance.
	 */
	protected ensureQuickFilterBar(): QuickFilterBar;
	/**
	 * Creates the quick filter bar with the given filters.
	 * @param filters - Quick filter definitions to render.
	 */
	protected createQuickFilters(filters?: QuickFilter<Widget<any>, any>[]): void;
	/**
	 * Returns the quick filter definitions derived from the grid columns.
	 * @returns Quick filter definitions for columns marked as quick filters.
	 */
	protected getQuickFilters(): QuickFilter<Widget<any>, any>[];
	/**
	 * Converts a property item to a quick filter definition.
	 * @param item - Property item to convert.
	 * @returns The quick filter definition, or null if not applicable.
	 */
	static propertyItemToQuickFilter(item: PropertyItem): QuickFilter<any, any> | null;
	/**
	 * Finds a quick filter widget by field name.
	 * @param type - Widget constructor type.
	 * @param field - Field name of the quick filter.
	 * @returns The widget instance.
	 */
	protected findQuickFilter<TWidget>(type: {
		new (...args: any[]): TWidget;
	}, field: string): TWidget;
	/**
	 * Tries to find a quick filter widget by field name.
	 * @param type - Widget constructor type.
	 * @param field - Field name of the quick filter.
	 * @returns The widget instance, or null if not found.
	 */
	protected tryFindQuickFilter<TWidget>(type: {
		new (...args: any[]): TWidget;
	}, field: string): TWidget;
	/**
	 * Creates the include-deleted toggle button when the row type supports it.
	 */
	protected createIncludeDeletedButton(): void;
	/**
	 * Returns the quick search fields available for this grid.
	 * @returns The quick search fields, or null for none.
	 */
	protected getQuickSearchFields(): QuickSearchField[];
	/**
	 * Creates the quick search input in the toolbar.
	 */
	protected createQuickSearchInput(): void;
	/**
	 * Cleans up event subscriptions, widgets, and the underlying grid.
	 */
	destroy(): void;
	/**
	 * Returns the CSS class for a grid row based on its active/deleted state.
	 * @param item - The row item.
	 * @param index - The row index.
	 * @returns The CSS class name, or an empty string.
	 */
	protected getItemCssClass(item: TItem, index: number): string;
	/**
	 * Returns row metadata (e.g. CSS classes) for the given item.
	 * @param item - The row item.
	 * @param index - The row index.
	 * @returns Row metadata object.
	 */
	protected getItemMetadata(item: TItem, index: number): any;
	/**
	 * Applies defaults and width adjustments to the given columns.
	 * @param columns - Columns to post-process.
	 * @returns The processed columns.
	 */
	protected postProcessColumns(columns: Column[]): Column[];
	/**
	 * Returns the width delta applied to all columns.
	 * @returns The column width delta.
	 */
	protected getColumnWidthDelta(): number;
	/**
	 * Returns the width scale applied to all columns.
	 * @returns The column width scale.
	 */
	protected getColumnWidthScale(): number;
	/**
	 * Performs the initial data population, optionally waiting until visible.
	 */
	protected initialPopulate(): void;
	/**
	 * Whether the given column can be used in the advanced filter bar.
	 * @param column - Column to check.
	 * @returns True when the column is filterable.
	 */
	protected canFilterColumn(column: Column): boolean;
	/**
	 * Initializes the filter bar store with the filterable columns.
	 */
	protected initializeFilterBar(): void;
	/**
	 * Handles filter store changes by persisting settings and refreshing.
	 */
	protected filterStoreChanged(): void;
	/**
	 * Creates initial column set for this grid. This column set is then passed
	 * to postProcessColumns to adjust widths etc, and then used as the initial
	 * columns for the slickgrid.
	 */
	protected createColumns(): Column<TItem>[];
	/**
	 * Creates the SleekGrid columns. This method calls createColumns (via getColumns for compatibility) and then post processes them.
	 * @returns The SleekGrid columns.
	 */
	protected createSleekColumns(): Column<TItem>[];
	/**
	 * Creates the underlying SleekGrid instance with the processed columns.
	 * @returns The created grid instance.
	 */
	protected createSlickGrid(): ISleekGrid<TItem> | null;
	/**
	 * Hook for subclasses to initialize the grid after creation.
	 */
	protected initSleekGrid(): void;
	/**
	 * Applies the default sort order to the grid and view.
	 */
	protected setInitialSortOrder(): void;
	/**
	 * Returns the item at the given row index.
	 * @param row - Row index.
	 * @returns The item at that row.
	 */
	itemAt(row: number): TItem;
	/**
	 * Returns the id of the given item using the grid id property.
	 * @param item - The item.
	 * @returns The item id.
	 */
	itemId(item: TItem): any;
	/**
	 * Returns the number of rows in the grid.
	 * @returns The row count.
	 */
	rowCount(): number;
	/**
	 * Returns the items currently displayed in the grid.
	 * @returns The grid items.
	 */
	getItems(): TItem[];
	/**
	 * Sets the items displayed in the grid.
	 * @param value - The items to display.
	 */
	setItems(value: TItem[]): void;
	/**
	 * Handles grid sort events by applying the sort and persisting settings.
	 * @param e - Grid sort event.
	 */
	protected handleGridSort(e: GridSortEvent): void;
	/**
	 * Handles grid cell click events by delegating to onClick.
	 * @param e - Cell mouse event.
	 */
	protected handleGridClick(e: CellMouseEvent): void;
	/**
	 * Persists settings when columns are reordered.
	 */
	protected handleGridColumnsReordered(): void;
	/**
	 * Persists settings when columns are resized.
	 */
	protected handleGridColumnsResized(): void;
	/**
	 * Subscribes to the underlying grid events.
	 */
	protected bindToSlickEvents(): void;
	/**
	 * Returns the caption for the add/new button.
	 * @returns The add button caption.
	 */
	protected getAddButtonCaption(): string;
	/**
	 * Returns the toolbar buttons for this grid.
	 * @returns Tool button definitions.
	 */
	protected getButtons(): ToolButton[];
	/**
	 * Opens an edit dialog for the given entity or id.
	 * @param entityOrId - Entity instance or identifier to edit.
	 */
	protected editItem(entityOrId: any): void;
	/**
	 * Opens an edit dialog for a specific item type.
	 * @param itemType - Item type key.
	 * @param entityOrId - Entity instance or identifier to edit.
	 */
	protected editItemOfType(itemType: string, entityOrId: any): void;
	/**
	 * Handles cell clicks, opening edit links when clicked.
	 * @param e - Click event.
	 * @param row - Row index.
	 * @param cell - Cell index.
	 */
	protected onClick(e: Event, row: number, cell: number): void;
	/**
	 * Handles view data changes by notifying subscribers and relaying out.
	 */
	protected viewDataChanged(): void;
	/**
	 * Subscribes to view events for filtering, submitting, and processing data.
	 */
	protected bindToViewEvents(): void;
	/**
	 * Filters a view item, notifying the onFiltering subscribers.
	 * @param item - The item to filter.
	 * @returns True when the item matches.
	 */
	protected handleViewFilter(item: TItem): boolean;
	/**
	 * Processes a list response, notifying the onProcessData subscribers.
	 * @param response - The list response.
	 * @returns The processed response.
	 */
	protected handleViewProcessData(response: ListResponse<TItem>): ListResponse<TItem>;
	/**
	 * Handles view submission, notifying the onSubmitting subscribers.
	 * @returns True when the submit should proceed.
	 */
	protected handleViewSubmit(): boolean;
	/**
	 * Hook for subclasses to process a list response before it is applied.
	 * @param response - The list response.
	 * @returns The processed response.
	 */
	protected onViewProcessData(response: ListResponse<TItem>): ListResponse<TItem>;
	/**
	 * Hook for subclasses to filter view items.
	 * @param item - The item to filter.
	 * @returns True when the item should be included.
	 */
	protected onViewFilter(item: TItem): boolean;
	/**
	 * Collects the fields and referenced fields of all columns into the given map.
	 * @param include - Map to populate with column field names.
	 */
	protected getIncludeColumns(include: {
		[key: string]: boolean;
	}): void;
	/**
	 * Sets the Criteria view parameter from the active filter store criteria.
	 */
	protected setCriteriaParameter(): void;
	/**
	 * Sets an equality filter on the view parameters.
	 * @param field - Field name.
	 * @param value - Equality value.
	 */
	protected setEquality(field: string, value: any): void;
	/**
	 * Sets the IncludeColumns view parameter from the grid columns.
	 */
	protected setIncludeColumnsParameter(): void;
	/**
	 * Prepares all view parameters and notifies the onSetViewParams subscribers.
	 */
	protected setViewParams(): void;
	/**
	 * Hook invoked before the view submits; prepares parameters and checks loadability.
	 * @returns True when the view can load.
	 */
	protected onViewSubmit(): boolean;
	/**
	 * Hook invoked when the grid markup is ready after data changes.
	 */
	protected markupReady(): void;
	/**
	 * Creates the container element that hosts the grid.
	 * @returns The grid container element.
	 */
	protected createSlickContainer(): Fluent;
	/**
	 * Creates the remote view used for paging and server communication.
	 * @returns The remote view instance.
	 */
	protected createView(): IRemoteView<TItem>;
	/**
	 * Returns the default sort order for the grid.
	 * @returns Array of sort descriptors.
	 */
	protected getDefaultSortBy(): any[];
	/**
	 * Whether the grid should render a pager.
	 * @returns True when paging is enabled.
	 */
	protected usePager(): boolean;
	/**
	 * Whether advanced filtering is enabled for this grid.
	 * @returns True when advanced filtering is enabled.
	 */
	protected enableAdvancedFiltering(): boolean;
	/**
	 * Whether the grid should wait until visible before populating data.
	 * @returns True when population waits for visibility.
	 */
	protected populateWhenVisible(): boolean;
	/**
	 * Creates the advanced filter bar and initializes its store.
	 */
	protected createFilterBar(): void;
	/**
	 * Returns the pager options for this grid.
	 * @returns Pager options.
	 */
	protected getPagerOptions(): PagerOptions;
	/**
	 * Creates the pager widget for this grid.
	 */
	protected createPager(): void;
	/**
	 * Returns the remote view options for this grid.
	 * @returns Remote view options.
	 */
	protected getViewOptions(): RemoteViewOptions<any>;
	/**
	 * Creates the toolbar with the given buttons.
	 * @param buttons - Tool button definitions.
	 */
	protected createToolbar(buttons: ToolButton[]): void;
	/**
	 * Returns the current grid title text.
	 * @returns The title text, or null if no title is set.
	 */
	getTitle(): string;
	/**
	 * Sets the grid title text, creating or removing the title element as needed.
	 * @param value - The title text, or null to remove the title.
	 */
	setTitle(value: string): void;
	/**
	 * Returns the item type key for this grid.
	 * @returns The item type key.
	 */
	protected getItemType(): string;
	/**
	 * Creates a formatter that renders a link to an item.
	 * @param itemType - Item type key; defaults to the grid item type.
	 * @param idField - Id field name; defaults to the grid id property.
	 * @param text - Optional text formatter.
	 * @param cssClass - Optional CSS class formatter.
	 * @param encode - Whether to HTML-encode the link text.
	 * @returns A formatter function.
	 */
	protected itemLink(itemType?: string, idField?: string, text?: Format<TItem>, cssClass?: (ctx: FormatterContext) => string, encode?: boolean): Format<TItem>;
	/** Renders an edit link for the item in current row. Returns a DocumentFragment for non-data rows, and an anchor element otherwise. */
	EditLink: (props: {
		/** formatter context (contains item, value etc) */
		context?: FormatterContext;
		/** The id of the entity to link to. If not provided it will be taken from ctx.item[idField] */
		id?: string;
		/** The name of the field in item that contains the entity id. Defaults to idProperty. Used if id is not provided. */
		idField?: string;
		/** The item type to link to. Defaults to this.getItemType() */
		itemType?: string;
		/** Extra CSS class to add to the link element besides s-EditLink. Optional. */
		cssClass?: string;
		/** The tabindex to assign to the link, default is undefined */
		tabindex?: number;
		/** @deprecated Use tabindex. */
		tabIndex?: number;
		/** The link text. If not provided it will be taken from ctx.escape(ctx.value) */
		children?: any;
	}) => any;
	/**
	 * Returns the columns key used to load property items.
	 * @returns The columns key, or null for none.
	 */
	protected getColumnsKey(): string;
	/**
	 * Returns the property items for this grid.
	 * @returns The property items.
	 */
	protected getPropertyItems(): PropertyItem[];
	/**
	 * Loads the property items data, either from script data or local items.
	 * @returns The property items data.
	 */
	protected getPropertyItemsData(): PropertyItemsData;
	/**
	 * Asynchronously loads the property items data.
	 * @returns A promise resolving to the property items data.
	 */
	protected getPropertyItemsDataAsync(): Promise<PropertyItemsData>;
	/** @deprecated override createColumns */
	protected getColumns(): Column<TItem>[];
	/**
	 * Wraps a column formatter with an edit link formatter.
	 * @param column - Column to wrap.
	 * @param item - Property item describing the edit link.
	 */
	protected wrapFormatterWithEditLink(column: Column, item: PropertyItem): void;
	/**
	 * Converts property items to grid columns, wrapping edit-link columns.
	 * @param propertyItems - Property items to convert.
	 * @returns The grid columns.
	 */
	protected propertyItemsToColumns(propertyItems: PropertyItem[]): Column[];
	/**
	 * Returns the SleekGrid options for this grid.
	 * @returns Grid options.
	 */
	protected getSlickOptions(): GridOptions;
	/**
	 * Locks the view against population.
	 */
	protected populateLock(): void;
	/**
	 * Unlocks the view population.
	 */
	protected populateUnlock(): void;
	/**
	 * Determines whether the grid can load data, notifying onCanSubmit subscribers.
	 * @returns True when the grid can load.
	 */
	protected getGridCanLoad(): boolean;
	/**
	 * Prepares submit arguments in this.view.params by calling this.view.onSubmit if available, or this.handleViewSubmit if not.
	 * Note that if getGridCanLoad returns false, the prepared arguments might be in an incomplete state.
	 * @returns True when the submit should proceed.
	 */
	prepareSubmit(): boolean;
	/**
	 * Refreshes the grid data, waiting for visibility if configured to do so.
	 */
	refresh(): void;
	/**
	 * Refreshes the grid if a refresh was requested while hidden.
	 */
	protected refreshIfNeeded(): void;
	/**
	 * Performs the actual data refresh by populating the view.
	 */
	protected internalRefresh(): void;
	private _readonly;
	/** Whether the grid is in read-only mode. */
	get readOnly(): boolean;
	/** Sets whether the grid is in read-only mode. */
	set readOnly(value: boolean);
	/**
	 * Returns whether the grid is in read-only mode.
	 * @returns True when read-only.
	 */
	get_readOnly(): boolean;
	/**
	 * Sets whether the grid is in read-only mode and updates the interface.
	 * @param value - True to enable read-only mode.
	 */
	set_readOnly(value: boolean): void;
	/**
	 * Updates the toolbar interface to reflect the current grid state.
	 */
	updateInterface(): void;
	/**
	 * Returns the row definition for this grid.
	 * @returns The row definition, or null for none.
	 */
	protected getRowDefinition(): IRowDefinition;
	private _localTextDbPrefix;
	/**
	 * Returns the local text database prefix for this grid.
	 * @returns The local text db prefix.
	 */
	protected getLocalTextDbPrefix(): string;
	/**
	 * Returns the local text prefix for this grid.
	 * @returns The local text prefix, or undefined.
	 */
	protected getLocalTextPrefix(): string;
	private _idProperty;
	/**
	 * Returns the id property name for this grid.
	 * @returns The id property name.
	 */
	protected getIdProperty(): string;
	/**
	 * Returns the is-deleted property name for this grid.
	 * @returns The is-deleted property name, or undefined.
	 */
	protected getIsDeletedProperty(): string;
	private _isActiveProperty;
	/**
	 * Returns the is-active property name for this grid.
	 * @returns The is-active property name.
	 */
	protected getIsActiveProperty(): string;
	/**
	 * Resizes the underlying grid canvas.
	 */
	protected resizeCanvas(): void;
	/**
	 * Refreshes the grid when a sub-dialog reports a data change.
	 */
	protected subDialogDataChange(): void;
	/**
	 * Adds a separator to the quick filter bar.
	 */
	protected addFilterSeparator(): void;
	/**
	 * Resolves a localized text using the grid's local text db prefix.
	 * @param getKey - Callback that builds the text key from the prefix.
	 * @returns The localized text, or null if not found.
	 */
	protected determineText(getKey: (prefix: string) => string): string;
	/**
	 * Adds a quick filter to the quick filter bar.
	 * @param opt - Quick filter definition.
	 * @returns The created widget instance.
	 */
	protected addQuickFilter<TWidget extends Widget<any>, P>(opt: QuickFilter<TWidget, P>): TWidget;
	/**
	 * Adds a date range quick filter for the specified field.
	 * @param field - Field name.
	 * @param title - Optional display title.
	 * @returns The created date editor.
	 */
	protected addDateRangeFilter(field: string, title?: string): DateEditor;
	/**
	 * Creates a date range quick filter definition.
	 * @param field - Field name.
	 * @param title - Optional display title.
	 * @returns A quick filter definition.
	 */
	protected dateRangeQuickFilter(field: string, title?: string): QuickFilter<DateEditor<DateEditorOptions>, DateTimeEditorOptions>;
	/**
	 * Adds a date-time range quick filter for the specified field.
	 * @param field - Field name.
	 * @param title - Optional display title.
	 * @returns The created date-time editor.
	 */
	protected addDateTimeRangeFilter(field: string, title?: string): DateTimeEditor<DateTimeEditorOptions>;
	/**
	 * Creates a date-time range quick filter definition.
	 * @param field - Field name.
	 * @param title - Optional display title.
	 * @returns A quick filter definition.
	 */
	protected dateTimeRangeQuickFilter(field: string, title?: string): QuickFilter<DateTimeEditor<DateTimeEditorOptions>, DateTimeEditorOptions>;
	/**
	 * Adds a boolean quick filter for the specified field.
	 * @param field - Field name.
	 * @param title - Optional display title.
	 * @param yes - Optional text for the true option.
	 * @param no - Optional text for the false option.
	 * @returns The created select editor.
	 */
	protected addBooleanFilter(field: string, title?: string, yes?: string, no?: string): SelectEditor;
	/**
	 * Creates a boolean quick filter definition.
	 * @param field - Field name.
	 * @param title - Optional display title.
	 * @param yes - Optional text for the true option.
	 * @param no - Optional text for the false option.
	 * @returns A quick filter definition.
	 */
	protected booleanQuickFilter(field: string, title?: string, yes?: string, no?: string): QuickFilter<SelectEditor<SelectEditorOptions>, SelectEditorOptions>;
	/**
	 * Invokes the quick filter submit handlers with the current view params.
	 */
	protected invokeSubmitHandlers(): void;
	/**
	 * Handles quick filter changes by persisting settings and refreshing.
	 * @param e - Change event.
	 */
	protected quickFilterChange(e: Event): void;
	/**
	 * Returns the storage used for grid persistence.
	 * @returns The persistence storage.
	 */
	protected getPersistenceStorage(): SettingStorage;
	/**
	 * Returns the key used to store grid settings.
	 * @returns The persistence key.
	 */
	protected getPersistenceKey(): string;
	/**
	 * Returns the default persistence flags for this grid.
	 * @returns Grid persistence flags.
	 */
	protected gridPersistenceFlags(): GridPersistenceFlags;
	/**
	 * Retrieves the persisted grid settings from storage.
	 * @returns The persisted settings, or a promise resolving to them.
	 */
	protected getPersistedSettings(): PersistedGridSettings | Promise<PersistedGridSettings>;
	/**
	 * Restores grid settings from the given settings or from storage.
	 * @param settings - Optional settings to restore; defaults to persisted settings.
	 * @param flags - Optional persistence flags.
	 * @returns Void or a promise that resolves when restoration completes.
	 */
	protected restoreSettings(settings?: PersistedGridSettings, flags?: GridPersistenceFlags): void | Promise<void>;
	/**
	 * Restores grid state from a persisted settings snapshot.
	 * @param settings - The settings to restore.
	 * @param flags - Optional persistence flags.
	 */
	protected restoreSettingsFrom(settings: PersistedGridSettings, flags?: GridPersistenceFlags): void;
	private _persistenceLock;
	/**
	 * Increments the persistence lock, preventing settings from being persisted.
	 */
	persistenceLock(): void;
	/**
	 * Decrements the persistence lock.
	 */
	persistenceUnlock(): void;
	/**
	 * Persists the current grid settings to storage.
	 * @param flags - Optional persistence flags.
	 * @returns Void or a promise that resolves when the write completes.
	 */
	persistSettings(flags?: GridPersistenceFlags): void | Promise<void>;
	/**
	 * Returns the current grid settings snapshot.
	 * @param flags - Optional persistence flags.
	 * @returns The current grid settings.
	 */
	getCurrentSettings(flags?: GridPersistenceFlags): PersistedGridSettings;
	/**
	 * Returns the root DOM element of the grid widget.
	 * @returns The grid container element.
	 */
	getElement(): HTMLElement;
	/**
	 * Returns the underlying SleekGrid instance.
	 * @returns The grid instance.
	 */
	getGrid(): ISleekGrid<TItem>;
	/** The underlying SleekGrid instance. */
	get sleekGrid(): ISleekGrid<TItem>;
	/** Sets the underlying SleekGrid instance. */
	protected set sleekGrid(value: ISleekGrid<TItem>);
	/** @deprecated Use sleekGrid or getGrid() */
	get slickGrid(): ISleekGrid<TItem>;
	/**
	 * Returns the remote view used for paging and server communication.
	 * @returns The remote view instance.
	 */
	getView(): IRemoteView<TItem>;
	/**
	 * Returns the filter store owned by the grid.
	 * @returns The filter store, or null if no filter bar exists.
	 */
	getFilterStore(): FilterStore;
	/** All columns including hidden ones. */
	get allColumns(): Column[];
	/** The currently visible columns. */
	get columns(): Column<TItem>[];
	/** The initial persisted settings captured at startup. */
	get initialSettings(): PersistedGridSettings;
	/** Sets the initial persisted settings. */
	protected set initialSettings(value: PersistedGridSettings);
	/** @deprecated use defaultPersistenceStorage, this one has a typo */
	static get defaultPersistanceStorage(): SettingStorage;
	/** @deprecated use defaultPersistenceStorage, this one has a typo */
	static set defaultPersistanceStorage(value: SettingStorage);
}
/**
 * Base event arguments for data grid events.
 */
export interface DataGridEvent {
	/** The data grid that raised the event. */
	dataGrid: DataGrid<any>;
}
/** Event raised when the grid data changes. */
export type DataGridChangeEvent = DataGridEvent;
/** Event raised after the grid is initialized. */
export type DataGridInitEvent = DataGridEvent;
/**
 * Event raised to determine whether the grid view submit should proceed.
 */
export interface DataGridSubmitEvent extends DataGridEvent {
	/** When true, the submit is cancelled. */
	cancel?: boolean;
}
/**
 * Event raised while filtering items in the view.
 * @typeParam TItem - Row type displayed in the grid.
 */
export interface DataGridFilteringEvent<TItem = any> extends DataGridEvent {
	/** The item being filtered. */
	item: TItem;
	/** Whether the item matches the filter; subscribers may change this. */
	isMatch: boolean;
}
/**
 * Event raised when the view processes a list response.
 * @typeParam TItem - Row type displayed in the grid.
 */
export interface DataGridProcessEvent<TItem> extends DataGridEvent {
	/** The list response being processed. */
	response: ListResponse<TItem>;
}
/**
 * Base grid for entity-bound data; integrates routing, permissions, dialogs,
 * toolbar buttons, filtering and service integration on top of {@link DataGrid}.
 * Registered via modern `static override [Symbol.typeInfo] = this.registerClass(...)`.
 * @typeParam TItem - Entity row type.
 * @typeParam P - Widget props type.
 */
export declare class EntityGrid<TItem, P = {}> extends DataGrid<TItem, P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates an entity grid and wires the route handler.
	 * @param props - Widget props forwarded to {@link DataGrid}.
	 */
	constructor(props: WidgetProps<P>);
	/**
	 * Cleans up the route-fixup handler and delegates to the base destroy.
	 */
	destroy(): void;
	/**
	 * Handles hash-based routing for new/edit dialog invocations.
	 * @param e - Route event containing the hash fragment and navigation metadata.
	 */
	protected handleRoute(e: HandleRouteEvent): void;
	/**
	 * Indicates whether a pager should be rendered; entity grids use a pager by default.
	 * @returns True when paging is enabled.
	 */
	protected usePager(): boolean;
	/**
	 * Creates standard toolbar extensions such as the include-deleted toggle and quick search.
	 */
	protected createToolbarExtensions(): void;
	/**
	 * Returns the initial title for the grid panel.
	 * @returns Localized plural display name.
	 */
	protected getInitialTitle(): string;
	/**
	 * Resolves the local text prefix for this grid; falls back to the entity type.
	 * @returns Local text prefix key.
	 */
	protected getLocalTextPrefix(): string;
	private _entityType;
	/**
	 * Returns the entity type name derived from the grid class name.
	 * @returns Entity type (e.g. "Administration.User").
	 */
	protected getEntityType(): string;
	private _displayName;
	/**
	 * Returns the localized plural display name for the entity.
	 * @returns Display name for the grid title.
	 */
	protected getDisplayName(): string;
	private _itemName;
	/**
	 * Returns the localized singular name for a single entity item.
	 * @returns Singular display name.
	 */
	protected getItemName(): string;
	/**
	 * Returns the caption for the add/new button.
	 * @returns Localized add button text.
	 */
	protected getAddButtonCaption(): string;
	/**
	 * Builds the grid toolbar buttons including add, refresh, column picker and filter bar.
	 * @returns Array of tool button definitions.
	 */
	protected getButtons(): ToolButton[];
	/**
	 * Shows or hides the filter bar depending on whether active filters exist.
	 */
	protected setFilterBarVisibility(): void;
	/**
	 * Creates the filter bar and syncs its visibility.
	 */
	protected createFilterBar(): void;
	/**
	 * Invoked when the filter store changes; refreshes the filter bar visibility.
	 */
	protected filterStoreChanged(): void;
	/**
	 * Creates a refresh toolbar button.
	 * @param noText - When true, only an icon with a hint is shown.
	 * @returns Tool button definition for refreshing the grid.
	 */
	protected newRefreshButton(noText?: boolean): ToolButton;
	/**
	 * Handles the add button click by opening a new-item dialog.
	 */
	protected addButtonClick(): void;
	/**
	 * Opens an edit dialog for an existing entity or a new instance.
	 * @param entityOrId - Entity instance or identifier to edit.
	 */
	protected editItem(entityOrId: any): void;
	/**
	 * Opens an edit dialog for a specific item type, used for polymorphic entities.
	 * @param itemType - Entity type key.
	 * @param entityOrId - Entity instance or identifier to edit.
	 */
	protected editItemOfType(itemType: string, entityOrId: any): void;
	private _service;
	/**
	 * Returns the service endpoint path for the entity (e.g. "Administration/User").
	 * @returns Service path derived from the entity type.
	 */
	protected getService(): string;
	/**
	 * Returns the service method name for listing entities.
	 * @returns Service method (defaults to "<service>/List").
	 */
	protected getServiceMethod(): string;
	/**
	 * Returns the absolute URL for the list service endpoint.
	 * @returns Resolved service URL.
	 */
	protected getServiceUrl(): string;
	/**
	 * Returns view options including the service URL.
	 * @returns Remote view options with URL populated.
	 */
	protected getViewOptions(): RemoteViewOptions;
	/**
	 * Returns the entity item type key used to resolve dialogs and row definitions.
	 * @returns Item type key.
	 */
	protected getItemType(): string;
	/**
	 * Registers hash-based routing for the specified dialog.
	 * @param itemType - Entity type key for the dialog.
	 * @param dialog - Dialog widget to route.
	 */
	protected routeDialog(itemType: string, dialog: Widget<any>): void;
	/**
	 * Returns the permission key required to insert a row.
	 * @returns Insert permission or undefined if none is configured.
	 */
	protected getInsertPermission(): string;
	/**
	 * Returns the permission key required to update a row.
	 * @returns Update permission or undefined.
	 */
	protected getUpdatePermission(): string;
	/**
	 * Returns the permission key required to delete a row.
	 * @returns Delete permission or undefined.
	 */
	protected getDeletePermission(): string;
	/**
	 * Checks whether the current user may delete rows.
	 * @returns True when deletion is allowed.
	 */
	protected hasDeletePermission(): boolean;
	/**
	 * Checks whether the current user may insert rows.
	 * @returns True when insertion is allowed.
	 */
	protected hasInsertPermission(): boolean;
	/**
	 * Checks whether the current user may update rows.
	 * @returns True when updates are allowed.
	 */
	protected hasUpdatePermission(): boolean;
	/**
	 * Propagates the grid read-only state to a newly created dialog.
	 * @param dialog - Dialog instance to configure.
	 */
	protected transferDialogReadOnly(dialog: Widget<any>): void;
	/**
	 * Initializes a dialog bound to the primary item type.
	 * @param dialog - Dialog instance to initialize.
	 */
	protected initDialog(dialog: Widget<any>): void;
	/**
	 * Initializes a dialog for the given item type, wiring data-change and routing.
	 * @param itemType - Entity type key.
	 * @param dialog - Dialog instance to initialize.
	 */
	protected initEntityDialog(itemType: string, dialog: Widget<any>): void;
	/**
	 * Creates a dialog widget for the specified item type, initializing and routing it.
	 * @param itemType - Entity type key whose dialog type will be resolved.
	 * @param callback - Optional callback invoked with the created dialog.
	 * @returns The dialog instance or a promise that resolves to it.
	 */
	protected createEntityDialog(itemType: string, callback?: (dlg: Widget<any>) => void): (Widget<any> | PromiseLike<Widget<any>>);
	/**
	 * Returns default options for the primary entity dialog.
	 * @returns Dialog options.
	 */
	protected getDialogOptions(): any;
	/**
	 * Returns dialog options for a specific item type.
	 * @param itemType - Entity type key.
	 * @returns Dialog options for that type.
	 */
	protected getDialogOptionsFor(itemType: string): any;
	/**
	 * Resolves the dialog type for the given item type.
	 * @param itemType - Entity type key.
	 * @returns Dialog constructor or a promise that resolves to it.
	 */
	protected getDialogTypeFor(itemType: string): DialogType | PromiseLike<DialogType>;
	private _dialogType;
	/**
	 * Resolves the dialog type for the primary entity; cached and may be loaded lazily.
	 * @returns Dialog constructor or a promise that resolves to it.
	 */
	protected getDialogType(): DialogType | PromiseLike<DialogType>;
}
/**
 * Pager widget for SlickGrid / SleekGrid views that provides page navigation,
 * page size selection, and status information.
 * @typeParam P - Options type for the widget.
 */
export declare class SlickPager<P extends PagerOptions = PagerOptions> extends Widget<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private currentPage;
	private totalPages;
	private pageSize;
	private stat;
	/**
	 * Creates a pager for the view specified in the options.
	 * @param props - Widget props including the view to page.
	 */
	constructor(props: WidgetProps<P>);
	/**
	 * Changes the current page based on the requested navigation action.
	 * @param ctype - Navigation action: "first", "prev", "next", "last", or "input".
	 * @returns True if the page change was handled, false otherwise.
	 */
	_changePage(ctype: string): boolean;
	/**
	 * Refreshes the pager UI from the current view paging info.
	 */
	_updatePager(): void;
}
/**
 * Adds tree / hierarchy support to a {@link DataGrid} by handling indentation,
 * expand/collapse toggles, and parent-before-child ordering.
 * Attach by constructing the mixin with the target grid and hierarchy options.
 * @typeParam TItem - Row type displayed in the grid.
 */
export declare class TreeGridMixin<TItem> {
	private options;
	/** Underlying data grid this mixin is attached to. */
	private dataGrid;
	/**
	 * Creates a tree mixin for the specified grid.
	 * @param options - Hierarchy configuration including grid reference and parent id accessor.
	 */
	constructor(options: TreeGridMixinOptions<TItem>);
	/**
	 * Toggles all rows between collapsed and expanded.
	 * If every row is collapsed, all rows are expanded and vice versa.
	 */
	toggleAll(): void;
	/** Collapses all rows in the associated grid. */
	collapseAll(): void;
	/** Expands all rows in the associated grid. */
	expandAll(): void;
	/**
	 * Reorders a set of items so that parents comes before their children.
	 * This method is required for proper tree ordering, as it is not so easy to perform with SQL.
	 * @param items list of items to be ordered
	 * @param getId a delegate to get ID of a record (must return same ID with grid identity field)
	 * @param getParentId a delegate to get parent ID of a record
	 */
	static applyTreeOrdering<TItem>(items: TItem[], getId: (item: TItem) => any, getParentId: (item: TItem) => any): TItem[];
}
/**
 * Options for {@link TreeGridMixin}.
 * @typeParam TItem - Row type displayed in the grid.
 */
export interface TreeGridMixinOptions<TItem> {
	/** Target data grid to enhance with tree behaviour. */
	grid: DataGrid<TItem, any>;
	/** Callback that returns the parent identifier for a row. */
	getParentId: (item: TItem) => any;
	/** Field / column id where the expand/collapse toggle is rendered. */
	toggleField: string;
	/** Optional callback that controls whether rows start collapsed. */
	initialCollapse?: () => boolean;
}
/**
 * Helper functions for extending jQuery UI dialogs.
 */
export declare namespace DialogExtensions {
	/**
	 * Makes a jQuery UI dialog resizable and applies optional size constraints.
	 * @param dialog - The dialog element or jQuery collection.
	 * @param w - Optional width.
	 * @param h - Optional height.
	 * @param mw - Optional minimum width.
	 * @param mh - Optional minimum height.
	 */
	function dialogResizable(dialog: HTMLElement | ArrayLike<HTMLElement>, w?: any, h?: any, mw?: any, mh?: any): void;
	/**
	 * Adds a maximize button to a jQuery UI dialog.
	 * @param dialog - The dialog element or jQuery collection.
	 */
	function dialogMaximizable(dialog: HTMLElement | ArrayLike<HTMLElement>): void;
}
/**
 * Options for the {@link UIDialogMaximizer} widget.
 */
export interface UIDialogMaximizerProps {
	/** Whether double-clicking the title bar toggles maximize. */
	dblclick?: boolean;
	/** Whether to show the maximize/restore buttons. */
	showButton?: boolean;
}
/**
 * Adds maximize / restore functionality to a jQuery UI dialog.
 * Ported from the jquery.dialogextend plugin, converted from a jQuery UI widget
 * into a plain class. Requires jQuery UI dialogs; it throws an error without them.
 */
export declare class UIDialogMaximizer extends Widget<UIDialogMaximizerProps> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/** Default options for the maximizer. */
	static readonly defaults: UIDialogMaximizerProps;
	private maximized;
	private snapshot;
	/**
	 * Creates a maximizer for the dialog containing the given element.
	 * @param props - Widget props including the dialog element.
	 */
	constructor(props: WidgetProps<UIDialogMaximizerProps>);
	private addButton;
	/**
	 * Returns the current state, e.g. "normal" or "maximized".
	 * @returns True when the dialog is maximized.
	 */
	get isMaximized(): boolean;
	private setMaximized;
	/**
	 * Maximizes the dialog to fill the window.
	 */
	maximize(): void;
	/**
	 * Restores the dialog to its previous size and position.
	 */
	restore(): void;
	private restoreSnapshot;
	private saveSnapshot;
}
/**
 * A field element rendered by the property grid, augmented with the editor
 * widget, its loading promise and the associated {@link PropertyItem}.
 */
export type PropertyFieldElement = HTMLElement & {
	/** The editor widget created for this field, once loaded. */
	editorWidget?: Widget<any>;
	/** A promise that resolves when the editor type finishes loading. */
	editorPromise?: PromiseLike<void>;
	/** The property item this field was rendered from. */
	propertyItem?: PropertyItem;
};
/**
 * Renders the caption (label) for a property field, including the required
 * marker and localized title/hint text.
 * @param props - Caption rendering props.
 * @returns The label element for the field.
 */
export declare function PropertyFieldCaption(props: {
	item: Pick<PropertyItem, "name" | "hint" | "labelWidth" | "required" | "title">;
	idPrefix?: string;
	localTextPrefix?: string;
}): HTMLLabelElement;
/**
 * Creates and initializes the editor widget for a property field, applying
 * editor params, max length, placeholder and editor addons.
 * @param props - Editor rendering props.
 */
export declare function PropertyFieldEditor(props: {
	fieldElement: PropertyFieldElement;
	item: Pick<PropertyItem, "editorCssClass" | "editorType" | "editorParams" | "maxLength" | "name" | "editorAddons" | "placeholder">;
	idPrefix?: string;
	localTextPrefix?: string;
}): void;
/**
 * Renders a line-break element when the item's form CSS class requests one at
 * the current breakpoint, or null otherwise.
 * @param props - Line-break rendering props.
 * @returns A line-break element, or null if none is needed.
 */
export declare function PropertyFieldLineBreak(props: {
	item: Pick<PropertyItem, "formCssClass">;
}): HTMLElement;
/**
 * Renders a full property field (caption plus editor) for a property item and
 * appends it to the given container.
 * @param props - Field rendering props.
 * @returns The created field element.
 */
export declare function PropertyField(props: {
	item: PropertyItem;
	container?: ParentNode;
	idPrefix?: string;
	localTextPrefix?: string;
}): PropertyFieldElement;
/**
 * Renders a category title with localized text.
 * @param props - Category title rendering props.
 * @returns The category title element.
 */
export declare function PropertyCategoryTitle(props: {
	category: string;
	localTextPrefix: string;
}): HTMLElement;
/**
 * Renders a collapsible category container holding its child fields.
 * @param props - Category rendering props.
 * @returns The category element.
 */
export declare function PropertyCategory(props: {
	category?: string;
	children?: any;
	collapsed?: boolean;
	localTextPrefix?: string;
}): HTMLElement;
/**
 * Renders a single tab item in the property tab list.
 * @param props - Tab item rendering props.
 * @returns The tab list item element.
 */
export declare function PropertyTabItem(props: {
	title: string;
	active?: boolean;
	paneId?: string;
	localTextPrefix?: string;
}): HTMLLIElement;
/**
 * Renders a single tab pane that hosts the fields of a tab.
 * @param props - Tab pane rendering props.
 * @returns The tab pane element.
 */
export declare function PropertyTabPane(props: {
	active?: boolean;
	id?: string;
	children?: any;
}): HTMLElement;
/**
 * Renders the categories container and populates it with fields for the given
 * property items, grouping them by category.
 * @param props - Categories rendering props.
 * @returns The categories element.
 */
export declare function PropertyCategories(props: {
	items: PropertyItem[];
	container?: ParentNode;
	fieldElements?: PropertyFieldElement[];
	idPrefix?: string;
	localTextPrefix?: string;
}): HTMLElement;
/**
 * Renders the tab list (nav) element for the property tabs.
 * @param props - Optional children to place inside the tab list.
 * @returns The tab list element.
 */
export declare function PropertyTabList(props?: {
	children?: any;
}): HTMLElement;
/**
 * Renders the container that holds the tab panes.
 * @returns The tab panes element.
 */
export declare function PropertyTabPanes(_?: {}): HTMLElement;
/**
 * Renders the full tabbed layout for property items that declare a `tab`,
 * grouping items without a tab into a leading untabbed section.
 * @param props - Tabs rendering props.
 * @returns A document fragment containing the tabs, or null when a container
 *   was provided and the content was appended directly to it.
 */
export declare function PropertyTabs(props: {
	items: PropertyItem[];
	container?: ParentNode;
	fieldElements?: PropertyFieldElement[];
	idPrefix?: string;
	localTextPrefix?: string;
	paneIdPrefix?: string;
}): DocumentFragment | null;
/**
 * A widget that renders a set of {@link PropertyItem}s as a form, organizing
 * them into categories and/or tabs, and manages loading/saving values to and
 * from the underlying editors.
 * @typeParam P - Widget props type, constrained to {@link PropertyGridOptions}.
 */
export declare class PropertyGrid<P extends PropertyGridOptions = PropertyGridOptions> extends Widget<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private fieldElements;
	/**
	 * Renders the property grid contents, building categories/tabs and loading
	 * the initial value.
	 * @returns The rendered contents.
	 */
	protected renderContents(): any;
	/**
	 * Destroys all field editors and clears the grid contents.
	 */
	destroy(): void;
	/**
	 * Returns the editor widgets for all rendered fields.
	 * @returns Array of editor widgets.
	 */
	get_editors(): Widget<any>[];
	/**
	 * Returns the property items for all rendered fields.
	 * @returns Array of property items.
	 */
	get_items(): PropertyItem[];
	/**
	 * Returns the id prefix used by this grid.
	 * @returns The id prefix.
	 */
	get_idPrefix(): string;
	/**
	 * Invokes a callback for each rendered field with its property item and
	 * editor widget.
	 * @param callback - Callback receiving the property item and editor widget.
	 */
	enumerateItems(callback: (p1: PropertyItem, p2: Widget<any>) => void): void;
	/**
	 * Returns the current grid mode (insert or update).
	 * @returns The current {@link PropertyGridMode}.
	 */
	get_mode(): PropertyGridMode;
	/**
	 * Sets the grid mode and refreshes the interface.
	 * @param value - The new {@link PropertyGridMode}.
	 */
	set_mode(value: PropertyGridMode): void;
	/**
	 * Loads a field's value from a source object into its editor, applying
	 * defaults in insert mode.
	 * @param source - The source object to read values from.
	 * @param fieldElement - The field element whose editor receives the value.
	 * @param mode - The grid mode, used to apply insert defaults.
	 */
	static loadFieldValue(source: any, fieldElement: PropertyFieldElement, mode?: PropertyGridMode): void;
	/**
	 * Loads values from a source object into all field editors.
	 * @param source - The source object to read values from.
	 */
	load(source: any): void;
	/**
	 * Saves a field's editor value into a target object when the item is
	 * modifiable.
	 * @param target - The target object to write values into.
	 * @param fieldElement - The field element whose editor value is saved.
	 * @param canModify - Whether the item may be modified; defaults to the
	 *   result of {@link PropertyGrid.canModifyItem}.
	 */
	static saveFieldValue(target: any, fieldElement: PropertyFieldElement, canModify?: boolean): void;
	/**
	 * Saves all field editor values into a target object.
	 * @param target - Optional target object; a new object is created if omitted.
	 * @returns The object containing the saved values.
	 */
	save(target?: any): any;
	/**
	 * Commits pending edits on all editors that support it.
	 * @returns True if all commits succeeded, false if any editor rejected.
	 */
	commitEdits(): Promise<boolean>;
	/**
	 * Gets the current values of all editors as an object.
	 */
	get value(): any;
	/**
	 * Loads values from an object into all editors.
	 * @param val - The object containing values to load.
	 */
	set value(val: any);
	/**
	 * Determines whether a property item may be modified in the given mode,
	 * taking insert/update permissions into account.
	 * @param item - The property item to check.
	 * @param mode - The grid mode; defaults to update semantics when omitted.
	 * @returns True if the item can be modified.
	 */
	static canModifyItem(item: PropertyItem, mode?: PropertyGridMode): boolean;
	/**
	 * Determines whether a property item may be modified in the current mode.
	 * @param item - The property item to check.
	 * @returns True if the item can be modified.
	 */
	protected canModifyItem(item: PropertyItem): boolean;
	/**
	 * Updates a field element's editor read-only/required state and visibility
	 * based on the item and mode.
	 * @param fieldElement - The field element to update.
	 * @param mode - The grid mode.
	 * @param canModify - Whether the item may be modified.
	 */
	static updateFieldElement(fieldElement: PropertyFieldElement, mode?: PropertyGridMode, canModify?: boolean): void;
	/**
	 * Updates a single field element in the current mode.
	 * @param fieldElement - The field element to update.
	 */
	protected updateFieldElement(fieldElement: PropertyFieldElement): void;
	/**
	 * Refreshes the read-only/required state and visibility of all fields.
	 */
	updateInterface(): void;
}
/**
 * Determines the editing mode of a {@link PropertyGrid}, which affects how
 * defaults, permissions and visibility are applied to fields.
 */
export declare enum PropertyGridMode {
	/** The grid is used for inserting a new record. */
	insert = 1,
	/** The grid is used for updating an existing record. */
	update = 2
}
/**
 * Options for configuring a {@link PropertyGrid}.
 */
export interface PropertyGridOptions {
	/** Optional id prefix used for field element ids. */
	idPrefix?: string;
	/** The property items to render as fields. */
	items: PropertyItem[];
	/** Optional local text prefix used to localize captions and hints. */
	localTextPrefix?: string;
	/** Optional initial value to load into the editors. */
	value?: any;
	/** The grid mode; defaults to {@link PropertyGridMode.insert}. */
	mode?: PropertyGridMode;
}
/**
 * Options for the {@link EntityLocalizer}.
 */
export interface EntityLocalizerOptions {
	/** Resolves an element by id within the dialog. */
	byId: (id: string) => Fluent;
	/** Id prefix used for generated elements. */
	idPrefix: string;
	/** Whether the entity is new (no id). */
	isNew: () => boolean;
	/** Returns the localization toggle button. */
	getButton: () => Fluent;
	/** Returns the current entity. */
	getEntity: () => any;
	/** Returns the list of available languages. */
	getLanguages: () => LanguageList;
	/** Returns the property grid element. */
	getPropertyGrid: () => Fluent;
	/** Returns the toolbar button elements. */
	getToolButtons: () => HTMLElement[];
	/** Options for the localization property grid. */
	pgOptions: PropertyGridOptions;
	/** Retrieves existing localizations for the entity. */
	retrieveLocalizations: () => PromiseLike<{
		[languageId: string]: any;
	}>;
	/** Validates the main form before switching modes. */
	validateForm: () => boolean;
}
/**
 * Manages the localization grid for an entity dialog, letting users edit
 * translations of localizable fields for each language.
 */
export declare class EntityLocalizer {
	protected grid: PropertyGrid;
	protected pendingValue: any;
	protected lastValue: any;
	protected targetLanguage: HTMLSelectElement;
	private options;
	/**
	 * Creates a localizer and builds the localization grid.
	 * @param opt - Options for the localizer.
	 */
	constructor(opt: EntityLocalizerOptions);
	/**
	 * Destroys the localization grid.
	 */
	destroy(): void;
	/**
	 * Clears pending and last localization values.
	 */
	clearValue(): void;
	/**
	 * Whether the localization grid is enabled (there are localizable fields).
	 * @returns True when enabled.
	 */
	isEnabled(): boolean;
	/**
	 * Whether the dialog is currently in localization mode.
	 * @returns True when in localization mode.
	 */
	protected isLocalizationMode(): boolean;
	/**
	 * Whether the localization values changed since the last save.
	 * @returns True when changed.
	 */
	protected isLocalizationModeAndChanged(): boolean;
	/**
	 * Toggles localization mode and loads/saves localization values.
	 */
	buttonClick(): void;
	/**
	 * Loads localization values into the grid.
	 */
	protected loadLocalization(): void;
	/**
	 * Copies current field values into the localization grid as hints.
	 */
	protected setLocalizationGridCurrentValues(): void;
	/**
	 * Returns the localization values from the grid, keyed by language and field.
	 * @returns The localization values.
	 */
	protected getLocalizationGridValue(): any;
	/**
	 * Adds pending localizations to a save request.
	 * @param req - The save request to modify.
	 */
	editSaveRequest(req: SaveRequest<any>): void;
	/**
	 * Returns pending localizations grouped by language.
	 * @returns The pending localizations.
	 */
	protected getPendingLocalizations(): any;
	/**
	 * Updates the UI to reflect the current localization mode.
	 */
	updateInterface(): void;
}
/**
 * Identifies how a save operation was initiated.
 */
export type SaveInitiator = "save-and-close" | "apply-changes";
/**
 * Creates a toolbar button that saves the entity and closes the dialog.
 * @param opt - Optional overrides merged into the button definition.
 * @returns Tool button definition.
 */
export declare function saveAndCloseToolButton(opt?: ToolButton): ToolButton;
/**
 * Creates a toolbar button that saves the entity and keeps the dialog open.
 * @param opt - Optional overrides merged into the button definition.
 * @returns Tool button definition.
 */
export declare function applyChangesToolButton(opt?: ToolButton): ToolButton;
/**
 * Creates a toolbar button that deletes the entity.
 * @param opt - Optional overrides merged into the button definition.
 * @returns Tool button definition.
 */
export declare function deleteToolButton(opt?: ToolButton): ToolButton;
/**
 * Creates a toolbar button that undeletes a soft-deleted entity.
 * @param opt - Optional overrides merged into the button definition.
 * @returns Tool button definition.
 */
export declare function undeleteToolButton(opt?: ToolButton): ToolButton;
/**
 * Creates a toolbar button that switches the dialog to edit mode.
 * @param opt - Optional overrides merged into the button definition.
 * @returns Tool button definition.
 */
export declare function editToolButton(opt?: ToolButton): ToolButton;
/**
 * Creates a toolbar button that toggles the localization editor.
 * @param opt - Optional overrides merged into the button definition.
 * @returns Tool button definition.
 */
export declare function localizationToolButton(opt?: ToolButton): ToolButton;
/**
 * Creates a toolbar button that clones the current entity.
 * @param opt - Optional overrides merged into the button definition.
 * @returns Tool button definition.
 */
export declare function cloneToolButton(opt?: ToolButton): ToolButton;
/**
 * Base dialog for editing entities, integrating property grids, save/delete
 * operations, localization, and toolbar buttons.
 * @typeParam TItem - Entity row type.
 * @typeParam P - Widget props type.
 */
export declare class EntityDialog<TItem, P = {}> extends BaseDialog<P> implements IEditDialog, IReadOnly {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private _entity;
	private _entityId;
	protected propertyItemsData: PropertyItemsData;
	protected propertyGrid: PropertyGrid;
	protected saveAndCloseButton: Fluent;
	protected applyChangesButton: Fluent;
	protected deleteButton: Fluent;
	protected undeleteButton: Fluent;
	protected cloneButton: Fluent;
	protected editButton: Fluent;
	protected localizer: EntityLocalizer;
	protected localizerButton: Fluent;
	/**
	 * Creates an entity dialog and loads property items.
	 * @param props - Widget props forwarded to the base dialog.
	 */
	constructor(props?: WidgetProps<P>);
	/**
	 * Called once property items are available; initializes the property grid and localizer.
	 * @param itemsData - Property items data.
	 */
	protected propertyItemsReady(itemsData: PropertyItemsData): void;
	/**
	 * Hook invoked after the dialog is initialized.
	 */
	protected afterInit(): void;
	/**
	 * Whether property items should be loaded asynchronously.
	 * @returns True when async loading is used.
	 */
	protected useAsync(): boolean;
	/**
	 * Cleans up the property grid, localizer, and toolbar buttons.
	 */
	destroy(): void;
	/**
	 * Returns the current entity.
	 * @returns The entity.
	 */
	get entity(): TItem;
	/** Sets the current entity. */
	protected set entity(value: TItem);
	/** @deprecated use entityId */
	protected get_entityId(): any;
	/**
	 * Returns the current entity id.
	 * @returns The entity id.
	 */
	get entityId(): any;
	/** Sets the current entity id. */
	protected set entityId(value: any);
	/**
	 * Returns the value of the entity name field.
	 * @returns The name field value.
	 */
	protected getEntityNameFieldValue(): any;
	/**
	 * Returns the title for the dialog based on the current mode.
	 * @returns The dialog title.
	 */
	protected getEntityTitle(): string;
	/**
	 * Updates the dialog title from the entity.
	 */
	protected updateTitle(): void;
	/**
	 * Whether the dialog is in clone mode.
	 * @returns True when cloning.
	 */
	protected isCloneMode(): boolean;
	/**
	 * Whether the dialog is editing an existing entity.
	 * @returns True when editing.
	 */
	protected isEditMode(): boolean;
	/**
	 * Whether the current entity is soft-deleted.
	 * @returns True when deleted.
	 */
	protected isDeleted(): boolean;
	/**
	 * Whether the dialog is creating a new entity.
	 * @returns True when new.
	 */
	protected isNew(): boolean;
	/**
	 * Whether the entity is new or soft-deleted.
	 * @returns True when new or deleted.
	 */
	protected isNewOrDeleted(): boolean;
	/**
	 * Returns the delete request for the current entity.
	 * @returns The delete request.
	 */
	protected getDeleteRequest(): DeleteRequest;
	/**
	 * Returns the options for the delete service call.
	 * @param callback - Callback invoked on success.
	 * @returns Service options.
	 */
	protected getDeleteOptions(callback: (response: DeleteResponse) => void): ServiceOptions<DeleteResponse>;
	/**
	 * Executes the delete service call.
	 * @param options - Service options.
	 * @param callback - Callback invoked on success.
	 * @returns A promise resolving to the delete response.
	 */
	protected deleteHandler(options: ServiceOptions<DeleteResponse>, callback: (response: DeleteResponse) => void): PromiseLike<DeleteResponse>;
	/**
	 * Returns the delete service method name.
	 * @returns The service method.
	 */
	protected getDeleteServiceMethod(): string;
	/**
	 * Deletes the current entity.
	 * @param callback - Callback invoked on success.
	 * @returns A promise resolving to the delete response.
	 */
	protected doDelete(callback: (response: DeleteResponse) => void): PromiseLike<DeleteResponse>;
	/**
	 * Hook invoked after a successful delete.
	 * @param response - The delete response.
	 */
	protected onDeleteSuccess(response: DeleteResponse): void;
	/**
	 * Returns the row definition for this dialog.
	 * @returns The row definition, or null.
	 */
	protected getRowDefinition(): IRowDefinition;
	private _entityType;
	/**
	 * Returns the entity type name derived from the dialog class name.
	 * @returns The entity type.
	 */
	protected getEntityType(): string;
	private _formKey;
	/**
	 * Returns the form key used to load property items.
	 * @returns The form key.
	 */
	protected getFormKey(): string;
	private _localTextDbPrefix;
	/**
	 * Returns the local text database prefix for this dialog.
	 * @returns The local text db prefix.
	 */
	protected getLocalTextDbPrefix(): string;
	/**
	 * Returns the local text prefix for this dialog.
	 * @returns The local text prefix.
	 */
	protected getLocalTextPrefix(): string;
	private _entitySingular;
	/**
	 * Returns the localized singular name for the entity.
	 * @returns The entity singular name.
	 */
	protected getEntitySingular(): string;
	private _nameProperty;
	/**
	 * Returns the name property for the entity.
	 * @returns The name property name.
	 */
	protected getNameProperty(): string;
	private _idProperty;
	/**
	 * Returns the id property for the entity.
	 * @returns The id property name.
	 */
	protected getIdProperty(): string;
	private _isActiveProperty;
	/**
	 * Returns the is-active property for the entity.
	 * @returns The is-active property name.
	 */
	protected getIsActiveProperty(): string;
	protected getIsDeletedProperty(): string;
	private _service;
	protected getService(): string;
	/**
	 * Loads an entity or id into the dialog.
	 * @param entityOrId - Entity instance or identifier to load.
	 * @param done - Callback invoked when loading completes.
	 * @param fail - Optional callback invoked on failure.
	 * @returns A promise resolving to the retrieve response.
	 */
	load(entityOrId: any, done: () => void, fail?: (ex: any) => void): PromiseLike<RetrieveResponse<TItem>>;
	/**
	 * Loads a new empty entity and opens the dialog.
	 * @param asPanel - When true, opens as a panel.
	 */
	loadNewAndOpenDialog(asPanel?: boolean): void;
	/**
	 * Loads an entity and opens the dialog.
	 * @param entity - The entity to load.
	 * @param asPanel - When true, opens as a panel.
	 */
	loadEntityAndOpenDialog(entity: TItem, asPanel?: boolean): void;
	/**
	 * Loads a retrieve response into the dialog.
	 * @param data - The retrieve response data.
	 */
	protected loadResponse(data: any): void;
	/**
	 * Loads an entity into the property grid.
	 * @param entity - The entity to load.
	 */
	protected loadEntity(entity: TItem): void;
	/**
	 * Hook invoked before loading an entity; clears localization state.
	 * @param entity - The entity being loaded.
	 */
	protected beforeLoadEntity(entity: TItem): void;
	/**
	 * Hook invoked after loading an entity; updates the interface and title.
	 */
	protected afterLoadEntity(): void;
	/**
	 * Loads an entity by id and opens the dialog.
	 * @param entityId - The entity id to load.
	 * @param asPanel - When true, opens as a panel.
	 * @param callback - Optional callback invoked on success.
	 * @param fail - Optional callback invoked on failure.
	 * @returns A promise resolving to the retrieve response.
	 */
	loadByIdAndOpenDialog(entityId: any, asPanel?: boolean, callback?: (response: RetrieveResponse<TItem>) => void, fail?: () => void): PromiseLike<RetrieveResponse<TItem>>;
	/**
	 * Hook invoked when data starts loading.
	 * @param data - The retrieve response data.
	 */
	protected onLoadingData(data: RetrieveResponse<TItem>): void;
	/**
	 * Returns the options for the retrieve service call.
	 * @param id - The entity id to load.
	 * @param callback - Callback invoked on success.
	 * @returns Service options.
	 */
	protected getLoadByIdOptions(id: any, callback: (response: RetrieveResponse<TItem>) => void): ServiceOptions<RetrieveResponse<TItem>>;
	/**
	 * Returns the retrieve request for an entity id.
	 * @param id - The entity id.
	 * @returns The retrieve request.
	 */
	protected getLoadByIdRequest(id: any): RetrieveRequest;
	/**
	 * Reloads the current entity by id.
	 */
	protected reloadById(): void;
	/**
	 * Returns the retrieve service method name.
	 * @returns The service method.
	 */
	protected getRetrieveServiceMethod(): string;
	/**
	 * Loads an entity by id.
	 * @param id - The entity id.
	 * @param callback - Optional callback invoked on success.
	 * @param fail - Optional callback invoked on failure.
	 * @returns A promise resolving to the retrieve response.
	 */
	loadById(id: any, callback?: (response: RetrieveResponse<TItem>) => void, fail?: () => void): PromiseLike<RetrieveResponse<TItem>>;
	/**
	 * Executes the retrieve service call.
	 * @param options - Service options.
	 * @param callback - Callback invoked on success.
	 * @param fail - Callback invoked on failure.
	 * @returns A promise resolving to the retrieve response.
	 */
	protected loadByIdHandler(options: ServiceOptions<RetrieveResponse<TItem>>, callback: (response: RetrieveResponse<TItem>) => void, fail: () => void): PromiseLike<RetrieveResponse<TItem>>;
	/**
	 * Retrieves existing localizations for the current entity.
	 * @returns A promise resolving to the localizations keyed by language.
	 */
	protected retrieveLocalizations(): Promise<Record<string, Partial<TItem>>>;
	/**
	 * Returns the options for the entity localizer.
	 * @returns Localizer options.
	 */
	protected getLocalizerOptions(): EntityLocalizerOptions;
	/**
	 * Initializes the entity localizer.
	 */
	protected initLocalizer(): void;
	/**
	 * Returns the list of languages for localization.
	 * @returns The language list.
	 */
	protected getLanguages(): LanguageList;
	/**
	 * Initializes the property grid from the PropertyGrid element.
	 */
	protected initPropertyGrid(): void;
	/**
	 * Returns the property items for this dialog.
	 * @returns The property items.
	 */
	protected getPropertyItems(): PropertyItem[];
	/**
	 * Loads the property items data, either from script data or local items.
	 * @returns The property items data.
	 */
	protected getPropertyItemsData(): PropertyItemsData;
	/**
	 * Asynchronously loads the property items data.
	 * @returns A promise resolving to the property items data.
	 */
	protected getPropertyItemsDataAsync(): Promise<PropertyItemsData>;
	/**
	 * Returns the options for the property grid.
	 * @returns Property grid options.
	 */
	protected getPropertyGridOptions(): PropertyGridOptions;
	/**
	 * Commits pending edits in the property grid.
	 * @returns True when the commit succeeds.
	 */
	protected commitEdits(): Promise<boolean>;
	/**
	 * Validates the form before saving.
	 * @returns True when the form is valid.
	 */
	protected validateBeforeSave(): boolean;
	/**
	 * Returns the create service method name.
	 * @returns The service method.
	 */
	protected getCreateServiceMethod(): string;
	/**
	 * Returns the update service method name.
	 * @returns The service method.
	 */
	protected getUpdateServiceMethod(): string;
	/**
	 * Returns the options for the save service call.
	 * @param callback - Callback invoked on success.
	 * @param initiator - How the save was initiated.
	 * @returns Service options.
	 */
	protected getSaveOptions(callback: (response: SaveResponse) => void, initiator?: SaveInitiator): ServiceOptions<SaveResponse>;
	/**
	 * Returns the entity populated from the property grid.
	 * @returns The saved entity.
	 */
	protected getSaveEntity(): TItem;
	/**
	 * Returns the save request for the current entity.
	 * @returns The save request.
	 */
	protected getSaveRequest(): SaveRequest<TItem>;
	/**
	 * Hook invoked after a successful save.
	 * @param response - The save response.
	 * @param initiator - How the save was initiated.
	 */
	protected onSaveSuccess(response: SaveResponse, initiator?: SaveInitiator): void;
	/**
	 * Submits the save after validation.
	 * @param callback - Callback invoked on success.
	 * @param initiator - How the save was initiated.
	 * @returns A promise resolving to the save response.
	 */
	protected save_submitHandler(callback: (response: SaveResponse) => void, initiator: SaveInitiator): PromiseLike<SaveResponse>;
	/**
	 * Validates and saves the entity.
	 * @param callback - Optional callback invoked on success.
	 * @param initiator - How the save was initiated.
	 * @returns A promise resolving to the save response, or false when validation fails.
	 */
	protected save(callback?: (response: SaveResponse) => void, initiator?: SaveInitiator): PromiseLike<SaveResponse> | false;
	/**
	 * Executes the save service call.
	 * @param options - Service options.
	 * @param callback - Callback invoked on success.
	 * @param initiator - How the save was initiated.
	 * @returns A promise resolving to the save response.
	 */
	protected saveHandler(options: ServiceOptions<SaveResponse>, callback: (response: SaveResponse) => void, initiator: SaveInitiator): PromiseLike<SaveResponse>;
	/**
	 * Shows a success message after saving.
	 * @param response - The save response.
	 * @param initiator - How the save was initiated.
	 */
	protected showSaveSuccessMessage(response: SaveResponse, initiator?: SaveInitiator): void;
	/**
	 * Returns the toolbar buttons for the entity dialog.
	 * @returns Tool button definitions.
	 */
	protected getToolbarButtons(): ToolButton[];
	/**
	 * Returns a clone of the current entity with identity and state fields removed.
	 * @returns The cloning entity.
	 */
	protected getCloningEntity(): TItem;
	/**
	 * Updates the interface to reflect the current mode and permissions.
	 */
	protected updateInterface(): void;
	/**
	 * Returns the undelete request for the current entity.
	 * @returns The undelete request.
	 */
	protected getUndeleteRequest(): UndeleteRequest;
	/**
	 * Returns the options for the undelete service call.
	 * @param callback - Optional callback invoked on success.
	 * @returns Service options.
	 */
	protected getUndeleteOptions(callback?: (response: UndeleteResponse) => void): ServiceOptions<UndeleteResponse>;
	/**
	 * Executes the undelete service call.
	 * @param options - Service options.
	 * @param callback - Optional callback invoked on success.
	 * @returns A promise resolving to the undelete response.
	 */
	protected undeleteHandler(options: ServiceOptions<UndeleteResponse>, callback?: (response: UndeleteResponse) => void): PromiseLike<UndeleteResponse>;
	/**
	 * Returns the undelete service method name.
	 * @returns The service method.
	 */
	protected getUndeleteServiceMethod(): string;
	/**
	 * Undeletes the current entity.
	 * @param callback - Optional callback invoked on success.
	 * @returns Void or a promise resolving to the undelete response.
	 */
	protected undelete(callback?: (response: UndeleteResponse) => void): void | PromiseLike<UndeleteResponse>;
	private _readonly;
	/** Whether the dialog is in read-only mode. */
	get readOnly(): boolean;
	/** Sets whether the dialog is in read-only mode. */
	set readOnly(value: boolean);
	/**
	 * Returns whether the dialog is in read-only mode.
	 * @returns True when read-only.
	 */
	get_readOnly(): boolean;
	/**
	 * Sets whether the dialog is in read-only mode and updates the interface.
	 * @param value - True to enable read-only mode.
	 */
	set_readOnly(value: boolean): void;
	/**
	 * Returns the insert permission for the entity.
	 * @returns The insert permission, or undefined.
	 */
	protected getInsertPermission(): string;
	/**
	 * Returns the update permission for the entity.
	 * @returns The update permission, or undefined.
	 */
	protected getUpdatePermission(): string;
	/**
	 * Returns the delete permission for the entity.
	 * @returns The delete permission, or undefined.
	 */
	protected getDeletePermission(): string;
	/**
	 * Whether the current user has delete permission.
	 * @returns True when permitted.
	 */
	protected hasDeletePermission(): boolean;
	/**
	 * Whether the current user has insert permission.
	 * @returns True when permitted.
	 */
	protected hasInsertPermission(): boolean;
	/**
	 * Whether the current user has update permission.
	 * @returns True when permitted.
	 */
	protected hasUpdatePermission(): boolean;
	/**
	 * Whether the current user has save permission (insert or update).
	 * @returns True when permitted.
	 */
	protected hasSavePermission(): boolean;
	protected editClicked: boolean;
	/**
	 * Whether the dialog is in view mode (read-only display of an existing entity).
	 * @returns True when in view mode.
	 */
	protected isViewMode(): boolean;
	/**
	 * Whether view mode is enabled for this dialog.
	 * @returns True when view mode is used.
	 */
	protected useViewMode(): boolean;
	/**
	 * Renders the dialog contents with toolbar, form, and property grid.
	 * @returns The rendered content.
	 */
	protected renderContents(): any;
	/**
	 * Returns the default language list for localization.
	 * @returns The default language list.
	 */
	static get defaultLanguageList(): string[][];
	/** Sets the default language list for localization. */
	static set defaultLanguageList(value: string[][]);
}
/**
 * A dialog that edits a single entity's properties using a property grid,
 * with OK/Cancel buttons and optional static panel behavior.
 * @typeParam TItem - Entity row type.
 * @typeParam P - Widget props type.
 */
export declare class PropertyDialog<TItem, P> extends BaseDialog<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private _entity;
	private _entityId;
	protected propertyItemsData: PropertyItemsData;
	/**
	 * Whether the dialog can be closed; false for static panels.
	 * @returns True when the dialog is closable.
	 */
	protected isClosable(): boolean;
	/**
	 * Whether the dialog renders as a static (non-closable) panel.
	 * @returns True when static.
	 */
	protected isStatic(): boolean;
	/**
	 * Creates a property dialog and loads property items.
	 * @param props - Widget props forwarded to the base dialog.
	 */
	constructor(props?: WidgetProps<P>);
	/**
	 * Called once property items are available; initializes the property grid and loads the initial entity.
	 * @param itemsData - Property items data.
	 */
	protected propertyItemsReady(itemsData: PropertyItemsData): void;
	/**
	 * Hook invoked after the dialog is initialized.
	 */
	protected afterInit(): void;
	/**
	 * Whether property items should be loaded asynchronously.
	 * @returns True when async loading is used.
	 */
	protected useAsync(): boolean;
	/**
	 * Cleans up the property grid and delegates to the base destroy.
	 */
	destroy(): void;
	/**
	 * Returns the dialog options with a narrower width.
	 * @returns Dialog options.
	 */
	protected getDialogOptions(): DialogOptions;
	/**
	 * Returns the dialog buttons; static panels have none.
	 * @returns Dialog button definitions.
	 */
	protected getDialogButtons(): DialogButton[];
	/**
	 * Handles the OK button click, validating before saving.
	 */
	protected okClick(): void;
	/**
	 * Closes the dialog with an OK result after validation passes.
	 */
	protected okClickValidated(): void;
	/**
	 * Closes the dialog with a cancel result.
	 */
	protected cancelClick(): void;
	/**
	 * Initializes the property grid from the PropertyGrid element.
	 */
	protected initPropertyGrid(): void;
	/**
	 * Returns the form key derived from the dialog class name.
	 * @returns The form key.
	 */
	protected getFormKey(): string;
	/**
	 * Returns the options for the property grid.
	 * @returns Property grid options.
	 */
	protected getPropertyGridOptions(): PropertyGridOptions;
	/**
	 * Returns the property items for this dialog.
	 * @returns The property items.
	 */
	protected getPropertyItems(): PropertyItem[];
	/**
	 * Loads the property items data, either from script data or local items.
	 * @returns The property items data.
	 */
	protected getPropertyItemsData(): PropertyItemsData;
	/**
	 * Asynchronously loads the property items data.
	 * @returns A promise resolving to the property items data.
	 */
	protected getPropertyItemsDataAsync(): Promise<PropertyItemsData>;
	/**
	 * Returns the entity populated from the property grid.
	 * @returns The saved entity.
	 */
	protected getSaveEntity(): TItem;
	/**
	 * Loads an empty entity into the property grid.
	 */
	protected loadInitialEntity(): void;
	/**
	 * Returns the current entity.
	 * @returns The entity.
	 */
	get entity(): TItem;
	/** Sets the current entity. */
	protected set entity(value: TItem);
	/**
	 * Returns the current entity id.
	 * @returns The entity id.
	 */
	get entityId(): any;
	/** Sets the current entity id. */
	protected set entityId(value: any);
	/**
	 * Validates the form before saving.
	 * @returns True when the form is valid.
	 */
	protected validateBeforeSave(): boolean;
	/**
	 * Hook for subclasses to update the dialog title.
	 */
	protected updateTitle(): void;
	protected propertyGrid: PropertyGrid;
	/**
	 * Renders the dialog contents with a form and property grid.
	 * @returns The rendered content.
	 */
	protected renderContents(): any;
}
export interface AutoNumericOptions {
	/** allowed decimal separator characters
	 * period "full stop" = '.'
	 * comma = ','
	 * @default '.'
	 */
	aDec?: string;
	/** allow to declare alternative decimal separator which is automatically replaced by aDec
	 * developed for countries the use a comma ',' as the decimal character
	 * and have keyboards\numeric pads that have a period 'full stop' as the decimal characters (Spain is an example)
	 * @default null
	 */
	altDec?: string;
	/** determine if the default value will be formatted on page ready.
	 * true = automatically formats the default value on page ready
	 * false = will not format the default value
	 * @default true
	 */
	aForm?: boolean;
	/** allowed numeric values
	 * please do not modify
	 * @default '0123456789'
	 */
	aNum?: string;
	/** allowed thousand separator characters
	 * comma = ','
	 * period "full stop" = '.'
	 * apostrophe is escaped = '\''
	 * space = ' '
	 * none = ''
	 * NOTE: do not use numeric characters
	 * @default ','
	 */
	aSep?: string;
	/** allowed currency symbol
	 * Must be in quotes aSign: '$', a space is allowed aSign: '$ '
	 * @default ''
	 */
	aSign?: string;
	/** controls decimal padding
	 * aPad: true - always Pad decimals with zeros
	 * aPad: false - does not pad with zeros.
	 * aPad: `some number` - pad decimals with zero to number different from mDec
	 * thanks to Jonas Johansson for the suggestion
	 * @default true
	 */
	aPad?: boolean;
	/** digital grouping for the thousand separator used in Format
	 * dGroup: '2', results in 99,99,99,999 common in India for values less than 1 billion and greater than -1 billion
	 * dGroup: '3', results in 999,999,999 default
	 * dGroup: '4', results in 9999,9999,9999 used in some Asian countries
	 * @default '3'
	 */
	dGroup?: string;
	/** internal */
	holder?: any;
	/** controls leading zero behavior
	 * lZero: 'allow', - allows leading zeros to be entered. Zeros will be truncated when entering additional digits. On focusout zeros will be deleted.
	 * lZero: 'deny', - allows only one leading zero on values less than one
	 * lZero: 'keep', - allows leading zeros to be entered. on fousout zeros will be retained.
	 * @default 'allow'
	 */
	lZero?: string;
	/** max number of decimal places = used to override decimal places set by the vMin & vMax values
	 * value must be enclosed in quotes example mDec: '3',
	 * This can also set the value via a call back function mDec: 'css:#
	 * @default null
	 */
	mDec?: number;
	/** method used for rounding
	 * mRound: 'S', Round-Half-Up Symmetric (default)
	 * mRound: 'A', Round-Half-Up Asymmetric
	 * mRound: 's', Round-Half-Down Symmetric (lower case s)
	 * mRound: 'a', Round-Half-Down Asymmetric (lower case a)
	 * mRound: 'B', Round-Half-Even "Bankers Rounding"
	 * mRound: 'U', Round Up "Round-Away-From-Zero"
	 * mRound: 'D', Round Down "Round-Toward-Zero" - same as truncate
	 * mRound: 'C', Round to Ceiling "Toward Positive Infinity"
	 * mRound: 'F', Round to Floor "Toward Negative Infinity"
	 * @default 'S'
	 */
	mRound?: string;
	/** places brackets on negative value -$ 999.99 to (999.99)
	 * visible only when the field does NOT have focus the left and right symbols should be enclosed in quotes and separated by a comma
	 * nBracket: null, nBracket: '(,)', nBracket: '[,]', nBracket: '<,>' or nBracket: '{,}'
	 * @default null
	 */
	nBracket?: string;
	/** placement of currency sign
	 * for prefix pSign: 'p',
	 * for suffix pSign: 's',
	 * @default 'p'
	 */
	pSign?: string;
	/** internal */
	runOnce?: boolean;
	/** maximum possible value
	 * value must be enclosed in quotes and use the period for the decimal point
	 * value must be larger than vMin
	 * @default '9999999999999.99'
	 */
	vMax?: any;
	/** minimum possible value
	 * value must be enclosed in quotes and use the period for the decimal point
	 * value must be smaller than vMax
	 * @default '0.00'
	 */
	vMin?: any;
	/** Displayed on empty string
	 * wEmpty: 'empty', - input can be blank
	 * wEmpty: 'zero', - displays zero
	 * wEmpty: 'sign', - displays the currency sign
	 * @default 'empty'
	 */
	wEmpty?: string;
}
/**
 * A jQuery-independent port of the autoNumeric library for formatting numeric inputs.
 */
export declare class AutoNumeric {
	/** Default autoNumeric options. */
	static defaults: AutoNumericOptions;
	/**
	 * Initializes autoNumeric on an input element.
	 * @param input - The input element.
	 * @param options - AutoNumeric options.
	 */
	static init(input: HTMLInputElement, options: AutoNumericOptions): void;
	/**
	 * Removes autoNumeric settings and event handlers from an input.
	 * @param input - The input element.
	 */
	static destroy(input: HTMLInputElement): void;
	/**
	 * Updates autoNumeric settings on an input.
	 * @param input - The input element.
	 * @param options - The options to update.
	 */
	static updateOptions(input: HTMLInputElement, options: AutoNumericOptions): void;
	/**
	 * Sets the formatted value of an input.
	 * @param input - The input element.
	 * @param valueIn - The value to set.
	 * @returns The formatted value.
	 */
	static setValue(input: HTMLInputElement, valueIn: number | string): string;
	/**
	 * Returns the unformatted numeric value of an input.
	 * @param input - The input element.
	 * @returns The numeric value as a string.
	 */
	static getValue(input: HTMLInputElement): string;
	/**
	 * Returns the autoNumeric settings for an input.
	 * @param input - The input element.
	 * @returns The settings.
	 */
	static getSettings(input: HTMLInputElement): AutoNumericOptions;
	/**
	 * Whether an input has an autoNumeric instance.
	 * @param input - The input element.
	 * @returns True when initialized.
	 */
	static hasInstance(input: HTMLInputElement): boolean;
	/** Setting keys that can be passed through from editor options. */
	static readonly allowedSettingKeys: Set<string>;
}
/**
 * An editor that renders a checkbox for boolean values.
 * @typeParam P - Widget props type.
 */
export declare class BooleanEditor<P = {}> extends EditorWidget<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	readonly domNode: HTMLInputElement;
	/**
	 * Returns the current boolean value.
	 * @returns True when the checkbox is checked.
	 */
	get value(): boolean;
	/**
	 * Returns the current boolean value.
	 * @returns True when the checkbox is checked.
	 */
	protected get_value(): boolean;
	/** Sets the boolean value. */
	set value(value: boolean);
	/** Sets the boolean value. */
	protected set_value(value: boolean): void;
}
/**
 * A single item in a check tree editor.
 * @typeParam TSource - The source item type.
 */
export interface CheckTreeItem<TSource> {
	/** Whether the item is selected. */
	isSelected?: boolean;
	/** Whether to hide the checkbox for this item. */
	hideCheckBox?: boolean;
	/** Whether all descendants are selected. */
	isAllDescendantsSelected?: boolean;
	/** Item id. */
	id?: string;
	/** Display text. */
	text?: string;
	/** Parent item id. */
	parentId?: string;
	/** Child items. */
	children?: CheckTreeItem<TSource>[];
	/** The source item. */
	source?: TSource;
}
/**
 * A grid-based editor that renders a hierarchical tree of checkboxes.
 * @typeParam TItem - The tree item type.
 * @typeParam P - Widget props type.
 */
export declare class CheckTreeEditor<TItem extends CheckTreeItem<TItem>, P = {}> extends DataGrid<TItem, P> implements IGetEditValue, ISetEditValue, IReadOnly {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLDivElement;
	private itemById;
	/**
	 * Creates a check tree editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Returns the id property name.
	 * @returns "id".
	 */
	protected getIdProperty(): string;
	/**
	 * Returns the tree items to display.
	 * @returns The tree items.
	 */
	protected getTreeItems(): TItem[];
	/**
	 * Loads the tree items into the view.
	 */
	protected updateItems(): void;
	/**
	 * Gets the edit value into a target object.
	 * @param property - The property item.
	 * @param target - The target object.
	 */
	getEditValue(property: PropertyItem, target: any): void;
	/**
	 * Sets the edit value from a source object.
	 * @param source - The source object.
	 * @param property - The property item.
	 */
	setEditValue(source: any, property: PropertyItem): void;
	/**
	 * Returns the toolbar buttons for the editor.
	 * @returns Tool button definitions.
	 */
	protected getButtons(): ToolButton[];
	/**
	 * Hook invoked when an item's selection changes.
	 * @param item - The item.
	 */
	protected itemSelectedChanged(item: TItem): void;
	/**
	 * Returns the text for the select-all button.
	 * @returns The select-all text.
	 */
	protected getSelectAllText(): string;
	/**
	 * Whether the tree uses a three-state hierarchy.
	 * @returns True when three-state.
	 */
	protected isThreeStateHierarchy(): boolean;
	/**
	 * Initializes the grid with tree-specific styling.
	 */
	protected initSleekGrid(): void;
	/**
	 * Filters view items for the tree hierarchy.
	 * @param item - The item to filter.
	 * @returns True when the item matches.
	 */
	protected onViewFilter(item: TItem): boolean;
	/**
	 * Returns the initial collapse state for tree rows.
	 * @returns True when collapsed.
	 */
	protected getInitialCollapse(): boolean;
	/**
	 * Processes the list response, setting tree indents.
	 * @param response - The list response.
	 * @returns The processed response.
	 */
	protected onViewProcessData(response: ListResponse<TItem>): ListResponse<TItem>;
	/**
	 * Handles cell clicks, toggling checkboxes and tree expansion.
	 * @param e - Click event.
	 * @param row - Row index.
	 * @param cell - Cell index.
	 */
	protected onClick(e: Event, row: number, cell: number): void;
	/**
	 * Updates the select-all button state.
	 */
	protected updateSelectAll(): void;
	/**
	 * Updates the selection flags for all items.
	 */
	protected updateFlags(): void;
	/**
	 * Whether all descendants of an item are selected.
	 * @param item - The item.
	 * @returns True when all descendants are selected.
	 */
	protected getDescendantsSelected(item: TItem): boolean;
	/**
	 * Sets the selection state of all descendants of an item.
	 * @param item - The item.
	 * @param selected - The selection state.
	 * @returns True when any item changed.
	 */
	protected setAllSubTreeSelected(item: TItem, selected: boolean): boolean;
	/**
	 * Whether all items are selected.
	 * @returns True when all items are selected.
	 */
	protected allItemsSelected(): boolean;
	/**
	 * Whether all descendants of an item are selected.
	 * @param item - The item.
	 * @returns True when all descendants are selected.
	 */
	protected allDescendantsSelected(item: TItem): boolean;
	/**
	 * Returns whether the value is delimited.
	 * @returns True when delimited.
	 */
	protected getDelimited(): boolean;
	/**
	 * Whether any descendant of an item is selected.
	 * @param item - The item.
	 * @returns True when any descendant is selected.
	 */
	protected anyDescendantsSelected(item: TItem): boolean;
	/**
	 * Creates the grid columns for the tree.
	 * @returns The columns.
	 */
	protected createColumns(): Column[];
	/**
	 * Returns the display text for an item.
	 * @param ctx - The formatter context.
	 * @returns The item text.
	 */
	protected getItemText(ctx: FormatterContext): FormatterResult;
	/**
	 * Returns the grid options for the editor.
	 * @returns Grid options.
	 */
	protected getSlickOptions(): GridOptions;
	/**
	 * Sorts items, moving selected items to the top.
	 */
	protected sortItems(): void;
	/**
	 * Whether selected items should be moved to the top.
	 * @returns True when moving selected items up.
	 */
	protected moveSelectedUp(): boolean;
	private _readOnly;
	/**
	 * Returns whether the editor is read-only.
	 * @returns True when read-only.
	 */
	get_readOnly(): boolean;
	/**
	 * Sets whether the editor is read-only.
	 * @param value - True to enable read-only mode.
	 */
	set_readOnly(value: boolean): void;
	private get_value;
	get value(): string[];
	private set_value;
	set value(v: string[]);
}
export interface CheckLookupEditorOptions {
	lookupKey?: string;
	checkedOnTop?: boolean;
	showSelectAll?: boolean;
	hideSearch?: boolean;
	delimited?: boolean;
	cascadeFrom?: string;
	cascadeField?: string;
	cascadeValue?: any;
	filterField?: string;
	filterValue?: any;
}
export declare class CheckLookupEditor<TItem extends CheckTreeItem<TItem> = any, P extends CheckLookupEditorOptions = CheckLookupEditorOptions> extends CheckTreeEditor<CheckTreeItem<TItem>, P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	private searchText;
	private enableUpdateItems;
	private lookupChangeOff;
	constructor(props: EditorProps<P>);
	destroy(): void;
	protected updateItems(): void;
	protected getLookupKey(): string;
	protected getButtons(): ToolButton[];
	protected createToolbarExtensions(): void;
	protected getSelectAllText(): string;
	protected cascadeItems(items: TItem[]): TItem[];
	protected filterItems(items: TItem[]): TItem[];
	protected getLookupItems(lookup: Lookup<TItem>): TItem[];
	protected getTreeItems(): {
		id: any;
		text: any;
		source: TItem;
	}[];
	protected onViewFilter(item: CheckTreeItem<TItem>): boolean;
	protected moveSelectedUp(): boolean;
	protected get_cascadeFrom(): string;
	get cascadeFrom(): string;
	protected getCascadeFromValue(parent: Widget<any>): any;
	protected cascadeLink: CascadedWidgetLink<Widget<any>>;
	protected setCascadeFrom(value: string): void;
	protected set_cascadeFrom(value: string): void;
	set cascadeFrom(value: string);
	protected get_cascadeField(): string;
	get cascadeField(): string;
	protected set_cascadeField(value: string): void;
	set cascadeField(value: string);
	protected get_cascadeValue(): any;
	get cascadeValue(): any;
	protected set_cascadeValue(value: any): void;
	set cascadeValue(value: any);
	protected get_filterField(): string;
	get filterField(): string;
	protected set_filterField(value: string): void;
	set filterField(value: string);
	protected get_filterValue(): any;
	get filterValue(): any;
	protected set_filterValue(value: any): void;
	set filterValue(value: any);
}
/**
 * An editor that renders a select of years around the current year.
 * @typeParam P - Widget props type.
 */
export declare class DateYearEditor<P extends DateYearEditorOptions = DateYearEditorOptions> extends SelectEditor<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	/**
	 * Creates a date year editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Returns the year options for the editor.
	 * @returns The list of year strings.
	 */
	getItems(): any[];
}
/**
 * Options for the {@link DateYearEditor}.
 */
export interface DateYearEditorOptions extends SelectEditorOptions {
	/** Minimum year as an absolute value or relative offset (e.g. "-10" or "+5"). */
	minYear?: string;
	/** Maximum year as an absolute value or relative offset (e.g. "+10" or "-5"). */
	maxYear?: string;
	/** Whether years are listed in descending order. */
	descending?: boolean;
}
/**
 * Options for the {@link DecimalEditor}.
 */
export interface DecimalEditorOptions {
	/** Minimum allowed value as a string. */
	minValue?: string;
	/** Maximum allowed value as a string. */
	maxValue?: string;
	/** Number of decimal places. */
	decimals?: any;
	/** Whether to pad decimals with zeros. */
	padDecimals?: any;
	/** Whether negative values are allowed. */
	allowNegatives?: boolean;
}
/**
 * An editor that renders a decimal input with AutoNumeric formatting.
 * @typeParam P - Widget props type.
 */
export declare class DecimalEditor<P extends DecimalEditorOptions = DecimalEditorOptions> extends EditorWidget<P> implements IDoubleValue {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	readonly domNode: HTMLInputElement;
	/**
	 * Creates a decimal editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Cleans up the AutoNumeric instance.
	 */
	destroy(): void;
	/**
	 * Initializes the AutoNumeric instance.
	 */
	protected initAutoNumeric(): void;
	/**
	 * Returns the AutoNumeric options for this editor.
	 * @returns AutoNumeric options.
	 */
	protected getAutoNumericOptions(): AutoNumericOptions;
	/**
	 * Returns the current decimal value.
	 * @returns The value, or null when empty.
	 */
	get_value(): number;
	/**
	 * Returns the current decimal value.
	 * @returns The value, or null when empty.
	 */
	get value(): number;
	/**
	 * Sets the decimal value.
	 * @param value - The value to set.
	 */
	set_value(value: number): void;
	/** Sets the decimal value. */
	set value(v: number);
	/**
	 * Whether the current value is valid.
	 * @returns True when valid.
	 */
	get_isValid(): boolean;
	/**
	 * Returns the default AutoNumeric options for decimal editors.
	 * @returns AutoNumeric options.
	 */
	static defaultAutoNumericOptions(): AutoNumericOptions;
}
/**
 * Utility functions for working with editor widgets.
 */
export declare namespace EditorUtils {
	/**
	 * Returns the display text of an editor's current value.
	 * @param editor - The editor widget.
	 * @returns The display text.
	 */
	function getDisplayText(editor: Widget<any>): string;
	/**
	 * Returns the current value of an editor.
	 * @param editor - The editor widget.
	 * @returns The value.
	 */
	function getValue(editor: Widget<any>): any;
	/**
	 * Saves an editor's value into a target object.
	 * @param editor - The editor widget.
	 * @param item - The property item.
	 * @param target - The target object.
	 */
	function saveValue(editor: Widget<any>, item: PropertyItem, target: any): void;
	/**
	 * Sets the value of an editor.
	 * @param editor - The editor widget.
	 * @param value - The value to set.
	 */
	function setValue(editor: Widget<any>, value: any): void;
	/**
	 * Loads a value from a source object into an editor.
	 * @param editor - The editor widget.
	 * @param item - The property item.
	 * @param source - The source object.
	 */
	function loadValue(editor: Widget<any>, item: PropertyItem, source: any): void;
	/**
	 * This functions sets readonly class and disabled (for select, radio, checkbox) or readonly attribute (for other inputs) on given elements
	 * or widgets. If a widget is passed and it has set_readOnly method it is called instead of setting readonly class or attributes.
	 * Note that if an element, instead of the widget attached to it is passed directly, this searchs for a widget attached to it.
	 * If you don't want this behavior, use setElementReadOnly method.
	 * @param elements
	 * @param value
	 */
	function setReadonly(elements: Element | Widget<any> | ArrayLike<Element | Widget>, value: boolean): void;
	/**
	 * Legacy alias for setReadonly
	 */
	const setReadOnly: typeof setReadonly;
	/**
	 * Sets the required state of an editor.
	 * @param widget - The editor widget.
	 * @param isRequired - Whether the field is required.
	 */
	function setRequired(widget: Widget<any>, isRequired: boolean): void;
	/**
	 * Sets all editors within a container to read-only.
	 * @param container - The container element.
	 * @param readOnly - Whether to enable read-only mode.
	 */
	function setContainerReadOnly(container: ArrayLike<HTMLElement> | HTMLElement, readOnly: boolean): void;
}
/**
 * An editor that renders a text input for string values.
 * @typeParam P - Widget props type.
 */
export declare class StringEditor<P = {}> extends EditorWidget<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	readonly domNode: HTMLInputElement;
	static createDefaultElement(): HTMLInputElement;
	/**
	 * Returns the current string value.
	 * @returns The input value.
	 */
	get value(): string;
	/**
	 * Returns the current string value.
	 * @returns The input value.
	 */
	protected get_value(): string;
	/** Sets the string value. */
	set value(value: string);
	/** Sets the string value. */
	protected set_value(value: string): void;
}
/**
 * An editor that renders an email address input.
 * @typeParam P - Widget props type.
 */
export declare class EmailAddressEditor<P = {}> extends StringEditor<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	/**
	 * Creates an email address editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
}
/**
 * Options for the {@link EmailEditor}.
 */
export interface EmailEditorOptions {
	/** Fixed domain appended to the user part. */
	domain?: string;
	/** Whether the domain part is read-only. */
	readOnlyDomain?: boolean;
}
/**
 * An editor that renders user and domain parts of an email address separately.
 * @typeParam P - Widget props type.
 */
export declare class EmailEditor<P extends EmailEditorOptions = EmailEditorOptions> extends EditorWidget<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	readonly domNode: HTMLInputElement;
	private readonly domain;
	/**
	 * Creates an email editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Registers the custom email validation method.
	 */
	static registerValidationMethods(): void;
	/**
	 * Returns the full email address.
	 * @returns The email value.
	 */
	get_value(): string;
	/**
	 * Returns the full email address.
	 * @returns The email value.
	 */
	get value(): string;
	/**
	 * Sets the email address, splitting it into user and domain parts.
	 * @param value - The email value.
	 */
	set_value(value: string): void;
	/** Sets the email address. */
	set value(v: string);
	/**
	 * Returns whether the editor is read-only.
	 * @returns True when read-only.
	 */
	get_readOnly(): boolean;
	/**
	 * Sets whether the editor is read-only.
	 * @param value - True to enable read-only mode.
	 */
	set_readOnly(value: boolean): void;
}
/**
 * Options for the {@link EnumEditor}.
 */
export interface EnumEditorOptions extends ComboboxCommonOptions {
	/** Key of the enum to load items from. */
	enumKey?: string;
	/** The enum type to load items from. */
	enumType?: any;
}
/**
 * An editor that renders a select of enum values.
 * @typeParam P - Widget props type.
 */
export declare class EnumEditor<P extends EnumEditorOptions = EnumEditorOptions> extends ComboboxEditor<P, ComboboxItem> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	/**
	 * Creates an enum editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Loads the enum values into the editor.
	 * @returns Void or a promise that resolves when items are loaded.
	 */
	protected updateItems(): void | PromiseLike<void>;
	/**
	 * Whether the editor allows clearing the selection.
	 * @returns True when clear is allowed.
	 */
	protected allowClear(): boolean;
}
export interface TiptapModule {
	Editor: any;
	[key: string]: any;
}
export interface TiptapToolbarHiddenOption {
	alignment?: boolean;
	alignmentJustify?: boolean;
	blockquote?: boolean;
	boldItalicUnderline?: boolean;
	inlineCode?: boolean;
	headings?: boolean;
	image?: boolean;
	link?: boolean;
	listOptions?: boolean;
	strike?: boolean;
	superSubScript?: boolean;
	undoRedo?: boolean;
}
/** The HTML editor provider to use. */
export type HtmlContentEditorProvider = "ckeditor" | "tiptap";
/**
 * Options for the {@link HtmlContentEditor}.
 */
export interface HtmlContentEditorOptions {
	/** Number of columns. */
	cols?: number;
	/** Number of rows. */
	rows?: number;
	/** The editor provider to use. */
	editorProvider?: HtmlContentEditorProvider;
}
/**
 * Configuration for the CKEditor instance.
 */
export interface CKEditorConfig {
}
/**
 * An editor that renders rich HTML content using CKEditor or Tiptap.
 * @typeParam P - Widget props type.
 */
export declare class HtmlContentEditor<P extends HtmlContentEditorOptions = HtmlContentEditorOptions> extends EditorWidget<P> implements IStringValue, IReadOnly {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	private _ckInstanceReady;
	readonly domNode: HTMLTextAreaElement;
	/** The Tiptap module loader. */
	static tiptapModule: TiptapModule | (() => (TiptapModule | Promise<TiptapModule>));
	private tiptapEditor;
	private tiptapElement;
	static createDefaultElement(): HTMLTextAreaElement;
	/** Default editor provider. */
	static defaultEditorProvider: HtmlContentEditorProvider;
	/** Default options for the editor. */
	static readonly defaultOptions: Partial<HtmlContentEditorOptions>;
	/**
	 * Creates an HTML content editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Handles the CKEditor instance-ready event.
	 * @param x - The CKEditor event.
	 */
	protected handleCKInstanceReady(x: any): void;
	/**
	 * Handles the CKEditor change event.
	 * @param e - The CKEditor event.
	 */
	protected handleCKEditorChange(e: any): void;
	/**
	 * Handles the CKEditor key event.
	 * @param e - The CKEditor event.
	 */
	protected handleCKKey(e: any): void;
	/**
	 * Returns the CKEditor language code.
	 * @returns The language code.
	 */
	protected getCKEditorLanguage(): string;
	private triggerKeyupEvent;
	/** @deprecated Override and use getCKEditorConfig() */
	protected getConfig(): CKEditorConfig;
	/**
	 * Returns the CKEditor configuration.
	 * @returns The CKEditor config.
	 */
	protected getCKEditorConfig(): CKEditorConfig;
	/**
	 * Returns the CKEditor instance for this editor.
	 * @returns The CKEditor instance.
	 */
	protected getCKEditorInstance(): any;
	/**
	 * Configures a Tiptap extension.
	 * @param extension - The extension to configure.
	 * @returns The configured extension.
	 */
	protected configureTiptapExtension(extension: any): any;
	/**
	 * Returns the Tiptap extensions for this editor.
	 * @param tiptap - The Tiptap module.
	 * @returns The extensions.
	 */
	protected getTiptapExtensions(tiptap: TiptapModule): any[];
	/**
	 * Creates the Tiptap toolbar.
	 * @param editor - The Tiptap editor.
	 * @param hidden - Hidden toolbar options.
	 * @returns The toolbar element.
	 */
	protected createTiptapToolbar(editor: any, hidden: TiptapToolbarHiddenOption): HTMLElement;
	/**
	 * Returns the hidden Tiptap toolbar options.
	 * @param editor - The Tiptap editor.
	 * @returns The hidden options.
	 */
	protected getTiptapToolbarHidden(editor: any): TiptapToolbarHiddenOption;
	/**
	 * Cleans up the editor instance.
	 */
	destroy(): void;
	/**
	 * Returns the current HTML value.
	 * @returns The HTML value.
	 */
	get_value(): string;
	/**
	 * Returns the current HTML value.
	 * @returns The HTML value.
	 */
	get value(): string;
	/**
	 * Sets the HTML value.
	 * @param value - The HTML value to set.
	 */
	set_value(value: string): void;
	/** Sets the HTML value. */
	set value(v: string);
	/**
	 * Returns whether the editor is read-only.
	 * @returns True when read-only.
	 */
	get_readOnly(): boolean;
	/**
	 * Sets whether the editor is read-only.
	 * @param value - True to enable read-only mode.
	 */
	set_readOnly(value: boolean): void;
	/** CKEditor version to load. */
	static CKEditorVer: string;
	/** Base path for CKEditor assets. */
	static CKEditorBasePath: string;
	/**
	 * Returns the base path for CKEditor assets.
	 * @returns The base path.
	 */
	static getCKEditorBasePath(): string;
	/**
	 * Includes the CKEditor script and invokes the callback when loaded.
	 * @param then - Callback invoked when CKEditor is available.
	 */
	static includeCKEditor(then: () => void): void;
	/**
	 * Returns the active editor provider.
	 * @returns The editor provider.
	 */
	get editorProvider(): HtmlContentEditorProvider;
}
/** Html content editor variant for notes with limited toolbar options, e.g. undo redo and bold / italic / underline for now */
export declare class HtmlNoteContentEditor<P extends HtmlContentEditorOptions = HtmlContentEditorOptions> extends HtmlContentEditor<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	constructor(props: EditorProps<P>);
	protected getCKEditorConfig(): CKEditorConfig;
	protected configureTiptapExtension(extension: any): any;
	protected getTiptapExtensions(tiptap: TiptapModule): any[];
}
/**
 * This is originally was intended to be a subset more compatible with reports,
 * which was necessary as most report rendering engines had limited HTML/CSS support.
 * We will revisit this if needed in future.
 */
export declare class HtmlReportContentEditor<P extends HtmlContentEditorOptions = HtmlContentEditorOptions> extends HtmlContentEditor<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	constructor(props: EditorProps<P>);
	protected getCKEditorConfig(): CKEditorConfig;
	protected getTiptapToolbarHidden(editor: any): TiptapToolbarHiddenOption;
	protected configureTiptapExtension(extension: any): any;
}
/**
 * Options for the {@link IntegerEditor}.
 */
export interface IntegerEditorOptions {
	/** Minimum allowed value. */
	minValue?: number;
	/** Maximum allowed value. */
	maxValue?: number;
	/** Whether negative values are allowed. */
	allowNegatives?: boolean;
}
/**
 * An editor that renders an integer input with AutoNumeric formatting.
 * @typeParam P - Widget props type.
 */
export declare class IntegerEditor<P extends IntegerEditorOptions = IntegerEditorOptions> extends EditorWidget<P> implements IDoubleValue {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	readonly domNode: HTMLInputElement;
	/**
	 * Creates an integer editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Cleans up the AutoNumeric instance.
	 */
	destroy(): void;
	/**
	 * Initializes the AutoNumeric instance.
	 */
	protected initAutoNumeric(): void;
	/**
	 * Returns the AutoNumeric options for this editor.
	 * @returns AutoNumeric options.
	 */
	protected getAutoNumericOptions(): AutoNumericOptions;
	/**
	 * Returns the current integer value.
	 * @returns The value, or null when empty.
	 */
	get_value(): number;
	/**
	 * Returns the current integer value.
	 * @returns The value, or null when empty.
	 */
	get value(): number;
	/**
	 * Sets the integer value.
	 * @param value - The value to set.
	 */
	set_value(value: number): void;
	/** Sets the integer value. */
	set value(v: number);
	/**
	 * Whether the current value is valid.
	 * @returns True when valid.
	 */
	get_isValid(): boolean;
}
/**
 * Options for the {@link LookupEditor}.
 */
export interface LookupEditorOptions extends ComboboxEditorOptions {
	/** Key of the lookup to load items from. */
	lookupKey?: string;
	/** Whether items are loaded asynchronously. */
	async?: boolean;
}
/**
 * Base editor that renders a combobox over lookup items.
 * @typeParam P - Widget props type.
 * @typeParam TItem - The item type.
 */
export declare abstract class LookupEditorBase<P extends LookupEditorOptions, TItem> extends ComboboxEditor<P, TItem> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	private lookupChangeOff;
	/**
	 * Creates a lookup editor base.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Whether the editor has an asynchronous item source.
	 * @returns True when async.
	 */
	hasAsyncSource(): boolean;
	/**
	 * Cleans up the lookup change subscription.
	 */
	destroy(): void;
	/**
	 * Returns the lookup key for this editor.
	 * @returns The lookup key.
	 */
	protected getLookupKey(): string;
	protected lookup: Lookup<TItem>;
	/**
	 * Asynchronously loads the lookup.
	 * @returns A promise resolving to the lookup.
	 */
	protected getLookupAsync(): PromiseLike<Lookup<TItem>>;
	/**
	 * Returns the lookup synchronously.
	 * @returns The lookup.
	 */
	protected getLookup(): Lookup<TItem>;
	/**
	 * Returns the items for the given lookup, filtered by cascade/filter values.
	 * @param lookup - The lookup.
	 * @returns The items.
	 */
	protected getItems(lookup: Lookup<TItem>): TItem[];
	/**
	 * Returns the id field name.
	 * @returns The id field.
	 */
	protected getIdField(): any;
	/**
	 * Returns the display text of an item.
	 * @param item - The item.
	 * @param lookup - The lookup.
	 * @returns The item text.
	 */
	protected getItemText(item: TItem, lookup: Lookup<TItem>): any;
	/**
	 * Maps an item to a combobox item.
	 * @param item - The item.
	 * @returns The combobox item.
	 */
	protected mapItem(item: TItem): ComboboxItem<TItem>;
	/**
	 * Whether an item is disabled.
	 * @param item - The item.
	 * @param lookup - The lookup.
	 * @returns True when disabled.
	 */
	protected getItemDisabled(item: TItem, lookup: Lookup<TItem>): boolean;
	/**
	 * Loads the lookup items into the editor.
	 */
	updateItems(): void;
	/**
	 * Performs an asynchronous search over the lookup items.
	 * @param query - The search query.
	 * @returns A promise resolving to the search result.
	 */
	protected asyncSearch(query: ComboboxSearchQuery): Promise<ComboboxSearchResult<TItem>>;
	/**
	 * Returns the dialog type key for in-place add.
	 * @returns The dialog type key.
	 */
	protected getDialogTypeKey(): string;
	/**
	 * Sets the search term on a new entity.
	 * @param entity - The new entity.
	 * @param term - The search term.
	 */
	protected setCreateTermOnNewEntity(entity: TItem, term: string): void;
	/**
	 * Reloads the lookup when the edit dialog data changes.
	 */
	protected editDialogDataChange(): void;
}
/**
 * An editor that renders a combobox over lookup items.
 * @typeParam P - Widget props type.
 */
export declare class LookupEditor<P extends LookupEditorOptions = LookupEditorOptions> extends LookupEditorBase<P, {}> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	/**
	 * Creates a lookup editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
}
/**
 * An editor that applies a mask to the input using the jQuery masked input plugin.
 * @typeParam P - Widget props type.
 */
export declare class MaskedEditor<P extends MaskedEditorOptions = MaskedEditorOptions> extends EditorWidget<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	readonly domNode: HTMLInputElement;
	/**
	 * Creates a masked editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Returns the current masked value.
	 * @returns The input value.
	 */
	get value(): string;
	/**
	 * Returns the current masked value.
	 * @returns The input value.
	 */
	protected get_value(): string;
	/** Sets the masked value. */
	set value(value: string);
	/** Sets the masked value. */
	protected set_value(value: string): void;
}
/**
 * Options for the {@link MaskedEditor}.
 */
export interface MaskedEditorOptions {
	/** The mask pattern to apply. */
	mask?: string;
	/** Placeholder character for empty mask positions. */
	placeholder?: string;
}
/**
 * An editor that renders a password input.
 * @typeParam TOptions - Widget options type.
 */
export declare class PasswordEditor<TOptions = {}> extends StringEditor<TOptions> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
}
/**
 * Options for the {@link RadioButtonEditor}.
 */
export interface RadioButtonEditorOptions {
	/** Key of the enum to load radio options from. */
	enumKey?: string;
	/** The enum type to load radio options from. */
	enumType?: any;
	/** Key of the lookup to load radio options from. */
	lookupKey?: string;
}
/**
 * An editor that renders a set of radio buttons for enum or lookup values.
 * @typeParam P - Widget props type.
 */
export declare class RadioButtonEditor<P extends RadioButtonEditorOptions = RadioButtonEditorOptions> extends EditorWidget<P> implements IReadOnly {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	private _pendingValue;
	/**
	 * Creates a radio button editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Returns the enum values for the given enum type.
	 * @param enumType - The enum type.
	 * @returns The enum values.
	 */
	protected getEnumValues(enumType: any): any[];
	/**
	 * Adds a radio button for the given value and text.
	 * @param value - The radio value.
	 * @param text - The display text.
	 */
	protected addRadio(value: string, text: string): void;
	/**
	 * Returns the currently selected radio value.
	 * @returns The selected value.
	 */
	get_value(): string;
	/**
	 * Returns the currently selected radio value.
	 * @returns The selected value.
	 */
	get value(): string;
	/**
	 * Sets the selected radio value.
	 * @param value - The value to select.
	 */
	set_value(value: string): void;
	/** Sets the selected radio value. */
	set value(v: string);
	/**
	 * Returns whether the editor is read-only.
	 * @returns True when read-only.
	 */
	get_readOnly(): boolean;
	/**
	 * Sets whether the editor is read-only.
	 * @param value - True to enable read-only mode.
	 */
	set_readOnly(value: boolean): void;
}
/**
 * Options for the {@link Recaptcha} editor.
 */
export interface RecaptchaOptions {
	/** The reCAPTCHA site key. */
	siteKey?: string;
	/** The language code for the reCAPTCHA widget. */
	language?: string;
}
/**
 * An editor that renders a Google reCAPTCHA widget and validates its response.
 * @typeParam P - Widget props type.
 */
export declare class Recaptcha<P extends RecaptchaOptions = RecaptchaOptions> extends EditorWidget<P> implements IStringValue {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	/**
	 * Creates a reCAPTCHA editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Returns the reCAPTCHA response token.
	 * @returns The response value.
	 */
	get_value(): string;
	/**
	 * Sets the reCAPTCHA value; ignored as it is managed by the widget.
	 * @param value - The value to set.
	 */
	set_value(value: string): void;
}
/**
 * Adapted from 3.5.x version of Select2 (https://github.com/select2/select2), removing jQuery dependency
 */
/** Element type that can host a Select2. */
export type Select2Element = HTMLInputElement | HTMLSelectElement;
/** Result of a Select2 formatter. */
export type Select2FormatResult = string | Element | DocumentFragment;
/**
 * Options passed to a Select2 query callback.
 */
export interface Select2QueryOptions {
	/** The element the query is for. */
	element?: Select2Element;
	/** The search term. */
	term?: string;
	/** The page number. */
	page?: number;
	/** Additional context. */
	context?: any;
	/** Callback invoked with the results. */
	callback?: (p1: Select2Result) => void;
	/** Custom matcher function. */
	matcher?: (p1: any, p2: any, p3?: any) => boolean;
}
/**
 * A single Select2 item.
 */
export interface Select2Item {
	/** Item id. */
	id?: string;
	/** Display text. */
	text?: string;
	/** The source item. */
	source?: any;
	/** Child items. */
	children?: Select2Item[];
	/** Whether the item is disabled. */
	disabled?: boolean;
	/** Whether the item is locked. */
	locked?: boolean;
}
/**
 * Result of a Select2 query.
 */
export interface Select2Result {
	/** Whether the query failed. */
	hasError?: boolean;
	/** Error information. */
	errorInfo?: any;
	/** The result items. */
	results: Select2Item[];
	/** Whether there are more results. */
	more?: boolean;
	/** Additional context. */
	context?: any;
}
/**
 * Options for ajax-based Select2 queries.
 */
export interface Select2AjaxOptions extends RequestInit {
	/** Request headers. */
	headers?: Record<string, string>;
	/** The URL or a function returning it. */
	url?: string | ((term: string, page: number, context: any) => string);
	/** Delay in milliseconds before the ajax request. */
	quietMillis?: number;
	/** Callback that builds the request data. */
	data?: (p1: string, p2: number, p3: any) => any;
	/** Callback that processes the response. */
	results?: (p1: any, p2: number, p3: any) => any;
	/** Additional request parameters. */
	params?: (() => any) | any;
	/** Callback invoked on error. */
	onError?(response: any, info?: any): void | boolean;
	/** Callback invoked on success. */
	onSuccess?(response: any): void;
}
/**
 * Options for the {@link Select2} widget.
 */
export interface Select2Options {
	/** The element to attach Select2 to. */
	element?: Select2Element;
	/** Width of the widget. */
	width?: any;
	/** Minimum input length before searching. */
	minimumInputLength?: number;
	/** Maximum input length. */
	maximumInputLength?: number;
	/** Minimum results required to show the search box. */
	minimumResultsForSearch?: number;
	/** Maximum number of selectable items. */
	maximumSelectionSize?: any;
	/** Placeholder text. */
	placeholder?: string;
	/** Placeholder option. */
	placeholderOption?: any;
	/** Separator for multiple values. */
	separator?: string;
	/** Whether the selection can be cleared. */
	allowClear?: boolean;
	/** Whether multiple items can be selected. */
	multiple?: boolean;
	/** Whether to close the dropdown on select. */
	closeOnSelect?: boolean;
	/** Whether to open the dropdown on enter. */
	openOnEnter?: boolean;
	/** Callback that returns the id of an item. */
	id?: (p1: any) => string;
	/** Custom matcher function. */
	matcher?: (p1: string, p2: string, p3: HTMLElement) => boolean;
	/** Callback that sorts results. */
	sortResults?: (p1: any, p2: HTMLElement, p3: any) => any;
	/** Formatter for ajax errors. */
	formatAjaxError?: (p1: any, p2: any) => Select2FormatResult;
	/** Formatter for the matches count. */
	formatMatches?: (matches: number) => Select2FormatResult;
	/** Formatter for selected items. */
	formatSelection?: (p1: any, p2: HTMLElement, p3: (p1: string) => string) => Select2FormatResult;
	/** Formatter for result items. */
	formatResult?: (p1: any, p2: HTMLElement, p3: any, p4: (p1: string) => string) => Select2FormatResult;
	/** Formatter for result CSS classes. */
	formatResultCssClass?: (p1: any) => string;
	/** Formatter for selection CSS classes. */
	formatSelectionCssClass?: (item: Select2Item, container: HTMLElement) => string;
	/** Formatter for no-matches text. */
	formatNoMatches?: (input: string) => Select2FormatResult;
	/** Formatter for load-more text. */
	formatLoadMore?: (pageNumber: number) => Select2FormatResult;
	/** Formatter for searching text. */
	formatSearching?: () => Select2FormatResult;
	/** Formatter for input-too-long text. */
	formatInputTooLong?: (input: string, max: number) => Select2FormatResult;
	/** Formatter for input-too-short text. */
	formatInputTooShort?: (input: string, min: number) => Select2FormatResult;
	/** Formatter for selection-too-big text. */
	formatSelectionTooBig?: (p1: number) => Select2FormatResult;
	/** Callback that creates a search choice. */
	createSearchChoice?: (p1: string) => Select2Item;
	/** Position of the create-search-choice item. */
	createSearchChoicePosition?: string | ((list: Select2Item[], item: Select2Item) => void);
	/** Callback that initializes the selection. */
	initSelection?: (p1: HTMLElement, p2: (p1: any) => void) => void;
	/** Tokenizer function. */
	tokenizer?: (p1: string, p2: any, p3: (p1: any) => any, p4: any) => string;
	/** Token separators. */
	tokenSeparators?: any;
	/** Query callback. */
	query?: (p1: Select2QueryOptions) => void;
	/** Ajax options. */
	ajax?: Select2AjaxOptions;
	/** Static data. */
	data?: any;
	/** Tags for tag mode. */
	tags?: ((string | Select2Item)[]) | (() => (string | Select2Item)[]);
	/** Container CSS. */
	containerCss?: any;
	/** Container CSS class. */
	containerCssClass?: any;
	/** Dropdown CSS. */
	dropdownCss?: any;
	/** Dropdown CSS class. */
	dropdownCssClass?: any;
	/** Whether the dropdown auto-widths. */
	dropdownAutoWidth?: boolean;
	/** Callback that returns the dropdown parent. */
	dropdownParent?: (input: HTMLElement) => HTMLElement;
	/** Callback that adapts the container CSS class. */
	adaptContainerCssClass?: (p1: string) => string;
	/** Callback that adapts the dropdown CSS class. */
	adaptDropdownCssClass?: (p1: string) => string;
	/** Callback that escapes markup. */
	escapeMarkup?: (p1: string) => string;
	/** Placeholder for the search input. */
	searchInputPlaceholder?: string;
	/** Whether to select on blur. */
	selectOnBlur?: boolean;
	/** Whether to blur on change. */
	blurOnChange?: boolean;
	/** Padding for load-more. */
	loadMorePadding?: number;
	/** Callback that returns the next search term. */
	nextSearchTerm?: (p1: any, p2: string) => string;
	/** Callback that populates results. */
	populateResults?: (container: HTMLElement, results: Select2Item[], query: Select2QueryOptions) => void;
	/** Callback that determines whether to focus the input. */
	shouldFocusInput?: (p1: any) => boolean;
}
/**
 * A searchable select widget ported from Select2.
 */
export declare class Select2 {
	private el;
	constructor(opts?: Select2Options);
	private get instance();
	/**
	 * Closes the dropdown.
	 */
	close(): void;
	/**
	 * Returns the container element.
	 * @returns The container element.
	 */
	get container(): HTMLElement;
	/**
	 * Returns the dropdown element.
	 * @returns The dropdown element.
	 */
	get dropdown(): HTMLElement;
	/**
	 * Destroys the Select2 instance.
	 */
	destroy(): void;
	/**
	 * Returns the current data.
	 * @returns The selected item(s).
	 */
	get data(): (Select2Item | Select2Item[]);
	/** Sets the current data. */
	set data(value: Select2Item | Select2Item[]);
	/**
	 * Disables the Select2 widget.
	 */
	disable(): void;
	/**
	 * Enables or disables the Select2 widget.
	 * @param enabled - Whether to enable.
	 */
	enable(enabled?: boolean): void;
	/**
	 * Focuses the search input.
	 */
	focus(): void;
	/**
	 * Whether the widget is focused.
	 * @returns True when focused.
	 */
	get isFocused(): boolean;
	/**
	 * Whether the widget allows multiple selection.
	 * @returns True when multiple.
	 */
	get isMultiple(): boolean;
	/**
	 * Whether the dropdown is open.
	 * @returns True when open.
	 */
	get opened(): boolean;
	/**
	 * Opens the dropdown.
	 * @returns True when opened.
	 */
	open(): boolean;
	/**
	 * Repositions the dropdown.
	 */
	positionDropdown(): void;
	/**
	 * Sets the read-only state.
	 * @param value - Whether to enable read-only mode.
	 */
	readonly(value?: boolean): void;
	/**
	 * Returns the search input element.
	 * @returns The search input.
	 */
	get search(): HTMLInputElement;
	/**
	 * Returns the current value.
	 * @returns The value.
	 */
	get val(): (string | string[]);
	/** Sets the current value. */
	set val(value: string | string[]);
	/**
	 * Returns the Select2 instance attached to an element, or null.
	 * @param el - The element.
	 * @returns The Select2 instance, or null.
	 */
	static getInstance(el: Select2Element): Select2;
	/** Default ajax options. */
	static readonly ajaxDefaults: Select2AjaxOptions;
	static readonly defaults: Select2Options;
	/**
	 * Highlights the matching portion of text for a search term.
	 * @param text - The text to highlight.
	 * @param term - The search term.
	 * @returns The highlighted result.
	 */
	static highlightMatch(text: string, term: string): Select2FormatResult;
	/**
	 * Strips diacritics from a string for accent-insensitive matching.
	 * @param str - The string to process.
	 * @returns The string with diacritics removed.
	 */
	static stripDiacritics(str: string): string;
}
/**
 * Options for the {@link ServiceLookupEditor}.
 */
export interface ServiceLookupEditorOptions extends ComboboxEditorOptions {
	/** Service endpoint to load items from. */
	service?: string;
	/** Id field name. */
	idField?: string;
	/** Text field name. */
	textField?: string;
	/** Page size for paged searches. */
	pageSize?: number;
	/** Minimum results required to show the search box. */
	minimumResultsForSearch?: any;
	/** Sort order for results. */
	sort?: string[];
	/** Column selection mode. */
	columnSelection?: ColumnSelection;
	/** Columns to include. */
	includeColumns?: string[];
	/** Columns to exclude. */
	excludeColumns?: string[];
	/** Whether to include deleted rows. */
	includeDeleted?: boolean;
	/** Field used for contains-text search. */
	containsField?: string;
	/** Equality filter applied to the request. */
	equalityFilter?: any;
	/** Criteria applied to the request. */
	criteria?: any[];
}
/**
 * Base editor that renders a combobox over service list results.
 * @typeParam P - Widget props type.
 * @typeParam TItem - The item type.
 */
export declare abstract class ServiceLookupEditorBase<P extends ServiceLookupEditorOptions, TItem> extends ComboboxEditor<P, TItem> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	/**
	 * Returns the dialog type key for in-place add.
	 * @returns The dialog type key.
	 */
	protected getDialogTypeKey(): string;
	/**
	 * Returns the service endpoint path.
	 * @returns The service path.
	 */
	protected getService(): string;
	/**
	 * Returns the resolved service URL.
	 * @returns The service URL.
	 */
	protected getServiceUrl(): string;
	/**
	 * Returns the columns to include in the request.
	 * @returns The include columns.
	 */
	protected getIncludeColumns(): string[];
	/**
	 * Returns the sort order for results.
	 * @returns The sort descriptors.
	 */
	protected getSort(): any[];
	/**
	 * Returns the cascade criteria for the request.
	 * @returns The cascade criteria.
	 */
	protected getCascadeCriteria(): any[];
	/**
	 * Returns the filter criteria for the request.
	 * @returns The filter criteria.
	 */
	protected getFilterCriteria(): any[];
	/**
	 * Returns the criteria for the given id list.
	 * @param idList - The id list.
	 * @returns The criteria.
	 */
	protected getIdListCriteria(idList: any[]): any[];
	/**
	 * Returns the combined criteria for the request.
	 * @param query - The search query.
	 * @returns The criteria.
	 */
	protected getCriteria(query: ComboboxSearchQuery): any[];
	/**
	 * Returns the list request for the given query.
	 * @param query - The search query.
	 * @returns The list request.
	 */
	protected getListRequest(query: ComboboxSearchQuery): ListRequest;
	/**
	 * Returns the service call options for the given query.
	 * @param query - The search query.
	 * @returns Service options.
	 */
	protected getServiceCallOptions(query: ComboboxSearchQuery): ServiceOptions<ListResponse<TItem>>;
	/**
	 * Whether the editor has an asynchronous item source.
	 * @returns True.
	 */
	protected hasAsyncSource(): boolean;
	/**
	 * Whether a search can be performed.
	 * @param byId - Whether the search is by id.
	 * @returns True when searchable.
	 */
	protected canSearch(byId: boolean): boolean;
	/**
	 * Performs an asynchronous search over the service results.
	 * @param query - The search query.
	 * @returns A promise resolving to the search result.
	 */
	protected asyncSearch(query: ComboboxSearchQuery): Promise<ComboboxSearchResult<TItem>>;
}
/**
 * An editor that renders a combobox over service list results.
 * @typeParam P - Widget props type.
 * @typeParam TItem - The item type.
 */
export declare class ServiceLookupEditor<P extends ServiceLookupEditorOptions = ServiceLookupEditorOptions, TItem = any> extends ServiceLookupEditorBase<ServiceLookupEditorOptions, TItem> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	/**
	 * Creates a service lookup editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
}
/**
 * Options for the {@link TextAreaEditor}.
 */
export interface TextAreaEditorOptions {
	/** Number of columns; 0 disables the attribute. */
	cols?: number;
	/** Number of rows; 0 disables the attribute. */
	rows?: number;
}
/**
 * An editor that renders a textarea for multi-line string values.
 * @typeParam P - Widget props type.
 */
export declare class TextAreaEditor<P extends TextAreaEditorOptions = TextAreaEditorOptions> extends EditorWidget<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLTextAreaElement;
	/**
	 * Creates a textarea editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Returns the current textarea value.
	 * @returns The value.
	 */
	get value(): string;
	/**
	 * Returns the current textarea value.
	 * @returns The value.
	 */
	protected get_value(): string;
	/** Sets the textarea value. */
	set value(value: string);
	/** Sets the textarea value. */
	protected set_value(value: string): void;
}
/**
 * Options for the {@link TimeEditorBase}.
 */
export interface TimeEditorBaseOptions {
	/** Whether to omit the empty option. */
	noEmptyOption?: boolean;
	/** Starting hour for the hour select. */
	startHour?: any;
	/** Ending hour for the hour select. */
	endHour?: any;
	/** Interval in minutes between minute options. */
	intervalMinutes?: any;
}
/**
 * Base editor for time values, providing hour and minute selects.
 * @typeParam P - Widget props type.
 */
export declare class TimeEditorBase<P extends TimeEditorBaseOptions> extends EditorWidget<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLElement;
	readonly domNode: HTMLSelectElement;
	protected minutes: Fluent;
	/**
	 * Creates a time editor base.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Returns the selected hour.
	 * @returns The hour value.
	 */
	get hour(): number;
	/**
	 * Returns the selected minute.
	 * @returns The minute value.
	 */
	get minute(): number;
	/**
	 * Returns whether the editor is read-only.
	 * @returns True when read-only.
	 */
	get_readOnly(): boolean;
	/**
	 * Sets whether the editor is read-only.
	 * @param value - True to enable read-only mode.
	 */
	set_readOnly(value: boolean): void;
	/** Returns value in HH:mm format */
	get hourAndMin(): string;
	/** Sets value in HH:mm format */
	set hourAndMin(value: string);
}
export interface TimeEditorOptions extends TimeEditorBaseOptions {
	/** Default is 1. Set to 60 to store seconds, 60000 to store ms in an integer field */
	multiplier?: number;
}
/**
 * Options for the {@link TimeEditor}.
 */
export interface TimeEditorOptions extends TimeEditorBaseOptions {
	/** Default is 1. Set to 60 to store seconds, 60000 to store ms in an integer field */
	multiplier?: number;
}
/** Note that this editor's value is number of minutes, e.g. for
 * 16:30, value will be 990. If you want to use a TimeSpan field
 * use TimeSpanEditor instead.
 */
/**
 * An editor for time values stored as a number of minutes.
 * @typeParam P - Widget props type.
 */
export declare class TimeEditor<P extends TimeEditorOptions = TimeEditorOptions> extends TimeEditorBase<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	/**
	 * Creates a time editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Returns the current time value in minutes.
	 * @returns The value, or null when empty.
	 */
	get value(): number;
	/**
	 * Returns the current time value in minutes.
	 * @returns The value, or null when empty.
	 */
	protected get_value(): number;
	/**
	 * Sets the time value in minutes.
	 * @param value - The value to set.
	 */
	set value(value: number);
	/** Sets the time value in minutes. */
	protected set_value(value: number): void;
}
/**
 * Options for the {@link TimeSpanEditor}.
 */
export interface TimeSpanEditorOptions extends TimeEditorBaseOptions {
}
/**
 * This editor is for TimeSpan fields. It uses a string value in the format "HH:mm".
 */
export declare class TimeSpanEditor<P extends TimeSpanEditorOptions = TimeSpanEditorOptions> extends TimeEditorBase<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	constructor(props: EditorProps<P>);
	/**
	 * Returns the current time span value.
	 * @returns The value in "HH:mm" format.
	 */
	protected get_value(): string;
	/** Sets the time span value. */
	protected set_value(value: string): void;
	/**
	 * Returns the current time span value.
	 * @returns The value in "HH:mm" format.
	 */
	get value(): string;
	/** Sets the time span value. */
	set value(value: string);
}
/**
 * Helper functions for file uploads, image constraints, and file display.
 */
export declare namespace UploadHelper {
	/**
	 * Adds an upload input to a container and returns a Fluent wrapper around it.
	 * @param options - The upload input options.
	 * @returns A Fluent wrapper around the created input element.
	 */
	function addUploadInput(options: UploadInputOptions): Fluent;
	/**
	 * Creates an upload input element and its associated Uploader.
	 * @param options - The upload input options.
	 * @returns An object containing the created input element and uploader.
	 */
	function createUploadInput(options: UploadInputOptions): {
		input: HTMLInputElement;
		uploader: Uploader;
	};
	/**
	 * Checks an uploaded file against the given image constraints, notifying the
	 * user of any violation.
	 * @param file - The uploaded file response.
	 * @param opt - The constraints to check against.
	 * @returns True if the file satisfies all constraints, otherwise false.
	 */
	function checkImageConstraints(file: UploadResponse, opt: FileUploadConstraints): boolean;
	/**
	 * Returns a display string combining a file name and its size.
	 * @param name - The file name.
	 * @param bytes - The file size in bytes.
	 * @returns The combined display string.
	 */
	function fileNameSizeDisplay(name: string, bytes: number): string;
	/**
	 * Formats a byte count into a human-readable size string (KB or MB).
	 * @param bytes - The file size in bytes.
	 * @returns The formatted size string.
	 */
	function fileSizeDisplay(bytes: number): string;
	/**
	 * Returns whether the given filename has a common image extension.
	 * @param filename - The filename to check.
	 * @returns True if the filename ends with a known image extension.
	 */
	function hasImageExtension(filename: string): boolean;
	/**
	 * Returns the thumbnail file name for the given filename.
	 * @param filename - The original filename.
	 * @returns The thumbnail filename.
	 */
	function thumbFileName(filename: string): string;
	/**
	 * Returns the resolved URL for a database-stored file.
	 * @param filename - The filename.
	 * @returns The resolved file URL.
	 */
	function dbFileUrl(filename: string): string;
	/**
	 * Creates a lightbox for a single upload thumbnail anchor element.
	 * It uses one of glightbox, simplelightbox or colorbox if available.
	 * Override this function to use a different lightbox library.
	 */
	function lightbox(link: HTMLElement | ArrayLike<HTMLElement>): void;
	/** @deprecated use lightbox
	 * Creates a lightbox for a single upload thumbnail anchor element.
	 * @param link - The anchor element (or array-like of elements) to open in a lightbox.
	 */
	const colorBox: typeof lightbox;
	/**
	 * Populates a container with file item elements for the given uploaded files.
	 * @param c - The container element (or array-like of elements) to populate.
	 * @param items - The uploaded files to display.
	 * @param displayOriginalName - Whether to display the original file names.
	 * @param urlPrefix - Optional URL prefix prepended to file names.
	 */
	function populateFileSymbols(c: HTMLElement | ArrayLike<HTMLElement>, items: UploadedFile[], displayOriginalName?: boolean, urlPrefix?: string): void;
}
/**
 * Represents an uploaded file.
 */
export interface UploadedFile {
	/**
	 * The stored file name.
	 */
	Filename?: string;
	/**
	 * The original file name.
	 */
	OriginalName?: string;
}
/**
 * Options for creating an upload input.
 */
export interface UploadInputOptions {
	/**
	 * The container element to add the input to.
	 */
	container?: HTMLElement | ArrayLike<HTMLElement>;
	/**
	 * The drop zone element.
	 */
	zone?: HTMLElement | ArrayLike<HTMLElement>;
	/**
	 * The progress element.
	 */
	progress?: HTMLElement | ArrayLike<HTMLElement>;
	/**
	 * The name of the input element.
	 */
	inputName?: string;
	/**
	 * Whether multiple files may be selected.
	 */
	allowMultiple?: boolean;
	/**
	 * An optional upload intent appended to the upload URL.
	 */
	uploadIntent?: string;
	/**
	 * The upload URL. Defaults to the temporary upload endpoint.
	 */
	uploadUrl?: string;
	/**
	 * Callback invoked when a file upload completes.
	 */
	fileDone?: (p1: UploadResponse, p2: string, p3: any) => void;
}
/**
 * The response returned by a file upload service.
 */
export interface UploadResponse extends ServiceResponse {
	/**
	 * The temporary file name.
	 */
	TemporaryFile: string;
	/**
	 * The file size in bytes.
	 */
	Size: number;
	/**
	 * Whether the file is an image.
	 */
	IsImage: boolean;
	/**
	 * The image width.
	 */
	Width: number;
	/**
	 * The image height.
	 */
	Height: number;
}
/**
 * Constraints for validating uploaded files.
 */
export interface FileUploadConstraints {
	/**
	 * The minimum image width.
	 */
	minWidth?: number;
	/**
	 * The maximum image width.
	 */
	maxWidth?: number;
	/**
	 * The minimum image height.
	 */
	minHeight?: number;
	/**
	 * The maximum image height.
	 */
	maxHeight?: number;
	/**
	 * The minimum file size in bytes.
	 */
	minSize?: number;
	/**
	 * The maximum file size in bytes.
	 */
	maxSize?: number;
	/**
	 * Whether non-image files are allowed.
	 */
	allowNonImage?: boolean;
	/**
	 * The name of the property holding the original file name.
	 */
	originalNameProperty?: string;
}
/**
 * Options for the {@link FileUploadEditor}.
 */
export interface FileUploadEditorOptions extends FileUploadConstraints {
	/** Whether to display the original file name. */
	displayFileName?: boolean;
	/** Upload intent for the upload service. */
	uploadIntent?: string;
	/** Upload URL. */
	uploadUrl?: string;
	/** URL prefix for file links. */
	urlPrefix?: string;
}
/**
 * Options for the {@link ImageUploadEditor}.
 */
export interface ImageUploadEditorOptions extends FileUploadEditorOptions {
}
/**
 * An editor that uploads and displays a single file.
 * @typeParam P - Widget props type.
 */
export declare class FileUploadEditor<P extends FileUploadEditorOptions = FileUploadEditorOptions> extends EditorWidget<P> implements IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	/**
	 * Creates a file upload editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Returns the upload input options.
	 * @returns Upload input options.
	 */
	protected getUploadInputOptions(): UploadInputOptions;
	/**
	 * Returns the text for the add-file button.
	 * @returns The button text.
	 */
	protected addFileButtonText(): string;
	/**
	 * Returns the toolbar buttons for the editor.
	 * @returns Tool button definitions.
	 */
	protected getToolButtons(): ToolButton[];
	/**
	 * Returns whether non-image files are allowed by default.
	 * @returns True when non-image files are allowed.
	 */
	protected getDefaultAllowNonImage(): boolean;
	/**
	 * Populates the file symbols from the current entity.
	 */
	protected populate(): void;
	/**
	 * Updates the interface to reflect the current state.
	 */
	protected updateInterface(): void;
	/**
	 * Returns whether the editor is read-only.
	 * @returns True when read-only.
	 */
	get_readOnly(): boolean;
	/**
	 * Sets whether the editor is read-only.
	 * @param value - True to enable read-only mode.
	 */
	set_readOnly(value: boolean): void;
	/**
	 * Returns whether the field is required.
	 * @returns True when required.
	 */
	get_required(): boolean;
	/**
	 * Sets whether the field is required.
	 * @param value - True when required.
	 */
	set_required(value: boolean): void;
	/**
	 * Returns the current uploaded file.
	 * @returns The uploaded file, or null.
	 */
	get_value(): UploadedFile;
	/**
	 * Returns the current uploaded file.
	 * @returns The uploaded file.
	 */
	get value(): UploadedFile;
	/**
	 * Sets the uploaded file.
	 * @param value - The uploaded file to set.
	 */
	set_value(value: UploadedFile): void;
	/** Sets the uploaded file. */
	set value(v: UploadedFile);
	/**
	 * Gets the edit value into a target object.
	 * @param property - The property item.
	 * @param target - The target object.
	 */
	getEditValue(property: PropertyItem, target: any): void;
	/**
	 * Sets the edit value from a source object.
	 * @param source - The source object.
	 * @param property - The property item.
	 */
	setEditValue(source: any, property: PropertyItem): void;
	protected entity: UploadedFile;
	protected toolbar: Toolbar;
	protected progress: HTMLElement;
	protected fileSymbols: HTMLElement;
	protected uploadInput: Fluent;
	protected hiddenInput: HTMLInputElement;
}
/**
 * An editor that uploads and displays a single image.
 * @typeParam P - Widget props type.
 */
export declare class ImageUploadEditor<P extends ImageUploadEditorOptions = ImageUploadEditorOptions> extends FileUploadEditor<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	/**
	 * Creates an image upload editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Whether non-image files are allowed.
	 * @returns False for image editors.
	 */
	protected getDefaultAllowNonImage(): boolean;
}
/**
 * Options for the {@link MultipleFileUploadEditor}.
 */
export interface MultipleFileUploadEditorOptions extends FileUploadEditorOptions {
	/** Whether to JSON-encode the value. */
	jsonEncodeValue?: boolean;
}
/**
 * An editor that uploads and displays multiple files.
 * @typeParam P - Widget props type.
 */
export declare class MultipleFileUploadEditor<P extends MultipleFileUploadEditorOptions = MultipleFileUploadEditorOptions> extends EditorWidget<P> implements IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	private entities;
	private toolbar;
	private fileSymbols;
	private uploadInput;
	protected progress: HTMLElement;
	protected hiddenInput: HTMLInputElement;
	/**
	 * Creates a multiple file upload editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
	/**
	 * Returns the upload input options.
	 * @returns Upload input options.
	 */
	protected getUploadInputOptions(): UploadInputOptions;
	/**
	 * Returns the text for the add-file button.
	 * @returns The button text.
	 */
	protected addFileButtonText(): string;
	/**
	 * Returns the toolbar buttons for the editor.
	 * @returns Tool button definitions.
	 */
	protected getToolButtons(): ToolButton[];
	/**
	 * Populates the file symbols from the current entities.
	 */
	protected populate(): void;
	/**
	 * Updates the interface to reflect the current state.
	 */
	protected updateInterface(): void;
	/**
	 * Returns whether the editor is read-only.
	 * @returns True when read-only.
	 */
	get_readOnly(): boolean;
	/**
	 * Sets whether the editor is read-only.
	 * @param value - True to enable read-only mode.
	 */
	set_readOnly(value: boolean): void;
	/**
	 * Returns whether the field is required.
	 * @returns True when required.
	 */
	get_required(): boolean;
	/**
	 * Sets whether the field is required.
	 * @param value - True when required.
	 */
	set_required(value: boolean): void;
	/**
	 * Returns the current uploaded files.
	 * @returns The uploaded files.
	 */
	get_value(): UploadedFile[];
	/**
	 * Returns the current uploaded files.
	 * @returns The uploaded files.
	 */
	get value(): UploadedFile[];
	/**
	 * Sets the uploaded files.
	 * @param value - The uploaded files to set.
	 */
	set_value(value: UploadedFile[]): void;
	/** Sets the uploaded files. */
	set value(v: UploadedFile[]);
	/**
	 * Gets the edit value into a target object.
	 * @param property - The property item.
	 * @param target - The target object.
	 */
	getEditValue(property: PropertyItem, target: any): void;
	/**
	 * Sets the edit value from a source object.
	 * @param source - The source object.
	 * @param property - The property item.
	 */
	setEditValue(source: any, property: PropertyItem): void;
	/**
	 * Whether the value is JSON-encoded.
	 * @returns True when JSON-encoded.
	 */
	get jsonEncodeValue(): boolean;
	/** Sets whether the value is JSON-encoded. */
	set jsonEncodeValue(value: boolean);
}
/**
 * An editor that uploads and displays multiple images.
 * @typeParam P - Widget props type.
 */
export declare class MultipleImageUploadEditor<P extends ImageUploadEditorOptions = ImageUploadEditorOptions> extends MultipleFileUploadEditor<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates a multiple image upload editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
}
/**
 * An editor that renders a URL input and auto-prefixes missing schemes on blur.
 * @typeParam P - Widget props type.
 */
export declare class URLEditor<P = {}> extends StringEditor<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	/**
	 * Creates a URL editor.
	 * @param props - Widget props.
	 */
	constructor(props: EditorProps<P>);
}
interface CriteriaWithText {
	/** The criteria expression. */
	criteria?: any[];
	/** The display text describing the criteria. */
	displayText?: string;
}
/**
 * Describes a filter operator (e.g. equals, contains, is null).
 */
export interface FilterOperator {
	/** Operator key. */
	key?: string;
	/** Display title. */
	title?: string;
	/** Format string used to build the display text. */
	format?: string;
}
/**
 * Constants for the built-in filter operators.
 */
export declare namespace FilterOperators {
	/** Is true operator. */
	const isTrue = "true";
	/** Is false operator. */
	const isFalse = "false";
	/** Contains operator. */
	const contains = "contains";
	/** Starts with operator. */
	const startsWith = "startswith";
	/** Equals operator. */
	const EQ = "eq";
	/** Not equals operator. */
	const NE = "ne";
	/** Greater than operator. */
	const GT = "gt";
	/** Greater than or equal operator. */
	const GE = "ge";
	/** Less than operator. */
	const LT = "lt";
	/** Less than or equal operator. */
	const LE = "le";
	/** Between operator. */
	const BW = "bw";
	/** In operator. */
	const IN = "in";
	/** Is null operator. */
	const isNull = "isnull";
	/** Is not null operator. */
	const isNotNull = "isnotnull";
	/** Maps operator keys to criteria comparison symbols. */
	const toCriteriaOperator: {
		[key: string]: string;
	};
}
/**
 * Interface for filtering handlers that build criteria and editors for a field.
 */
export declare abstract class IFiltering {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
/**
 * Interface for filtering handlers that build criteria and editors for a field.
 */
export interface IFiltering {
	/** Creates the editor for the current operator. */
	createEditor(): void;
	/** Returns the criteria and display text for the current operator. */
	getCriteria(): CriteriaWithText;
	/** Returns the operators supported by this filtering handler. */
	getOperators(): FilterOperator[];
	/** Loads persisted state into the editor. */
	loadState(state: any): void;
	/** Saves the editor state for persistence. */
	saveState(): any;
	/** Returns the field being filtered. */
	get_field(): PropertyItem;
	/** Sets the field being filtered. */
	set_field(value: PropertyItem): void;
	/** Returns the container element for the editor. */
	get_container(): HTMLElement;
	/** Sets the container element for the editor. */
	set_container(value: HTMLElement): void;
	/** Returns the current operator. */
	get_operator(): FilterOperator;
	/** Sets the current operator. */
	set_operator(value: FilterOperator): void;
}
/**
 * Interface for filtering handlers that can initialize a quick filter.
 */
export declare abstract class IQuickFiltering {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
/**
 * Interface for filtering handlers that can initialize a quick filter.
 */
export interface IQuickFiltering {
	/** Initializes a quick filter for this field. */
	initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
}
/**
 * Base class for filtering handlers that build criteria and editors for a field.
 */
export declare abstract class BaseFiltering implements IFiltering, IQuickFiltering {
	private field;
	/**
	 * Returns the field being filtered.
	 * @returns The field.
	 */
	get_field(): PropertyItem;
	/**
	 * Sets the field being filtered.
	 * @param value - The field.
	 */
	set_field(value: PropertyItem): void;
	private container;
	/**
	 * Returns the container element for the editor.
	 * @returns The container element.
	 */
	get_container(): HTMLElement;
	/**
	 * Sets the container element for the editor.
	 * @param value - The container element.
	 */
	set_container(value: HTMLElement): void;
	private operator;
	/**
	 * Returns the current operator.
	 * @returns The operator.
	 */
	get_operator(): FilterOperator;
	/**
	 * Sets the current operator.
	 * @param value - The operator.
	 */
	set_operator(value: FilterOperator): void;
	/**
	 * Returns the operators supported by this filtering handler.
	 * @returns The operators.
	 */
	abstract getOperators(): FilterOperator[];
	/**
	 * Appends the is-null and is-not-null operators when the field is nullable.
	 * @param list - The operator list.
	 * @returns The operator list.
	 */
	protected appendNullableOperators(list: FilterOperator[]): FilterOperator[];
	/**
	 * Appends the comparison operators (eq, ne, lt, le, gt, ge).
	 * @param list - The operator list.
	 * @returns The operator list.
	 */
	protected appendComparisonOperators(list: FilterOperator[]): FilterOperator[];
	/**
	 * Whether the field is nullable.
	 * @returns True when the field is not required.
	 */
	protected isNullable(): boolean;
	/**
	 * Creates the editor for the current operator.
	 */
	createEditor(): void;
	/**
	 * Returns the format string for an operator.
	 * @param op - The operator.
	 * @returns The format string.
	 */
	protected operatorFormat(op: FilterOperator): string;
	/**
	 * Returns the localized title of a field.
	 * @param field - The field.
	 * @returns The title.
	 */
	protected getTitle(field: PropertyItem): string;
	/**
	 * Builds the display text for an operator and its values.
	 * @param op - The operator.
	 * @param values - The filter values.
	 * @returns The display text.
	 */
	protected displayText(op: FilterOperator, values?: any[]): string;
	/**
	 * Returns the criteria field name.
	 * @returns The field name.
	 */
	protected getCriteriaField(): string;
	/**
	 * Returns the criteria and display text for the current operator.
	 * @returns The criteria with display text.
	 */
	getCriteria(): CriteriaWithText;
	/**
	 * Loads persisted state into the editor.
	 * @param state - The persisted state.
	 */
	loadState(state: any): void;
	/**
	 * Saves the editor state for persistence.
	 * @returns The saved state.
	 */
	saveState(): string;
	/**
	 * Returns the error thrown when a required value is missing.
	 * @returns The error.
	 */
	protected argumentNull(): Error;
	/**
	 * Validates the editor value.
	 * @param value - The value to validate.
	 * @returns The validated value.
	 */
	validateEditorValue(value: string): string;
	/**
	 * Returns the current editor value.
	 * @returns The editor value.
	 */
	getEditorValue(): string;
	/**
	 * Returns the display text of the current editor value.
	 * @returns The editor text.
	 */
	getEditorText(): string;
	/**
	 * Initializes a quick filter for this field.
	 * @param filter - The quick filter to initialize.
	 */
	initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
	protected static registerClass<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): ClassTypeInfo<TypeName>;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
}
/**
 * Base filtering handler that uses an editor widget for comparison operators.
 * @typeParam TEditor - The editor widget type.
 */
export declare abstract class BaseEditorFiltering<TEditor extends Widget<any>> extends BaseFiltering {
	editorTypeRef: any;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates a base editor filtering handler.
	 * @param editorTypeRef - Constructor of the editor widget type.
	 */
	constructor(editorTypeRef: any);
	/**
	 * Whether the current operator uses an editor.
	 * @returns True when an editor is used.
	 */
	protected useEditor(): boolean;
	protected editor: TEditor;
	/**
	 * Creates the editor for the current operator.
	 */
	createEditor(): void;
	/**
	 * Whether to use the id field for the criteria.
	 * @returns True when the id field is used.
	 */
	protected useIdField(): boolean;
	/**
	 * Returns the criteria field name, using the filtering id field when applicable.
	 * @returns The criteria field name.
	 */
	getCriteriaField(): string;
	/**
	 * Returns the options for the editor widget.
	 * @returns The editor options.
	 */
	getEditorOptions(): any;
	/**
	 * Loads persisted state into the editor.
	 * @param state - The persisted state.
	 */
	loadState(state: any): void;
	/**
	 * Saves the editor state for persistence.
	 * @returns The saved state.
	 */
	saveState(): any;
	/**
	 * Returns the current editor value.
	 * @returns The editor value.
	 */
	getEditorValue(): any;
	/**
	 * Initializes a quick filter using the editor type.
	 * @param filter - The quick filter to initialize.
	 */
	initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
}
/**
 * Filtering handler for boolean fields, supporting is-true and is-false operators.
 */
export declare class BooleanFiltering extends BaseFiltering {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Returns the operators supported by this filtering handler.
	 * @returns The operators.
	 */
	getOperators(): FilterOperator[];
}
/**
 * Filtering handler for date fields using a date editor.
 */
export declare class DateFiltering extends BaseEditorFiltering<DateEditor> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates a date filtering handler.
	 */
	constructor();
	/**
	 * Returns the operators supported by this filtering handler.
	 * @returns The operators.
	 */
	getOperators(): FilterOperator[];
}
/**
 * Filtering handler for date-time fields using a date-time editor.
 */
export declare class DateTimeFiltering extends BaseEditorFiltering<DateEditor> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates a date-time filtering handler.
	 */
	constructor();
	/**
	 * Returns the operators supported by this filtering handler.
	 * @returns The operators.
	 */
	getOperators(): FilterOperator[];
	/**
	 * Returns the criteria for the current operator, handling day-boundary comparisons.
	 * @returns The criteria with display text.
	 */
	getCriteria(): CriteriaWithText;
}
/**
 * Filtering handler for decimal fields using a decimal editor.
 */
export declare class DecimalFiltering extends BaseEditorFiltering<DecimalEditor> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates a decimal filtering handler.
	 */
	constructor();
	/**
	 * Returns the operators supported by this filtering handler.
	 * @returns The operators.
	 */
	getOperators(): FilterOperator[];
}
/**
 * A filtering handler that uses an editor type resolved from the editor type registry.
 */
export declare class EditorFiltering extends BaseEditorFiltering<Widget<any>> {
	readonly props: {
		editorType?: string;
		useRelative?: boolean;
		useLike?: boolean;
	};
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates an editor filtering handler.
	 * @param props - Options including the editor type and operator flags.
	 */
	constructor(props?: {
		editorType?: string;
		useRelative?: boolean;
		useLike?: boolean;
	});
	/** The editor type key. */
	get editorType(): string;
	/** Sets the editor type key. */
	set editorType(value: string);
	/** Whether relative comparison operators are used. */
	get useRelative(): boolean;
	/** Sets whether relative comparison operators are used. */
	set useRelative(value: boolean);
	/** Whether like operators (contains, startsWith) are used. */
	get useLike(): boolean;
	/** Sets whether like operators are used. */
	set useLike(value: boolean);
	/**
	 * Returns the operators supported by this filtering handler.
	 * @returns The operators.
	 */
	getOperators(): FilterOperator[];
	/**
	 * Whether the current operator uses an editor.
	 * @returns True when an editor is used.
	 */
	protected useEditor(): boolean;
	/**
	 * Creates the editor for the current operator.
	 */
	createEditor(): void;
	/**
	 * Whether to use the id field for the criteria.
	 * @returns True when an editor is used.
	 */
	protected useIdField(): boolean;
	/**
	 * Initializes a quick filter using the editor type.
	 * @param filter - The quick filter to initialize.
	 */
	initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
}
/**
 * Filtering handler for enum fields using an enum editor.
 */
export declare class EnumFiltering extends BaseEditorFiltering<EnumEditor> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates an enum filtering handler.
	 */
	constructor();
	/**
	 * Returns the operators supported by this filtering handler.
	 * @returns The operators.
	 */
	getOperators(): FilterOperator[];
	/**
	 * Returns the display text of the current editor value.
	 * @returns The editor text.
	 */
	getEditorText(): string;
}
/**
 * Options for the {@link FilterFieldSelect}.
 */
export interface FilterFieldSelectOptions {
	/** The fields to display in the select. */
	fields: PropertyItem[];
}
/**
 * A combobox that lets the user select a filter field.
 * @typeParam P - Widget props type.
 */
export declare class FilterFieldSelect<P extends FilterFieldSelectOptions = FilterFieldSelectOptions> extends ComboboxEditor<P, PropertyItem> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates a filter field select.
	 * @param props - Widget props.
	 */
	constructor(props: WidgetProps<P>);
	/**
	 * Returns the empty item text.
	 * @returns The empty item text.
	 */
	emptyItemText(): string;
	/**
	 * Returns the combobox options.
	 * @returns Combobox options.
	 */
	getComboboxOptions(): ComboboxOptions<any>;
}
/**
 * A combobox that lets the user select a filter operator.
 */
export declare class FilterOperatorSelect extends ComboboxEditor<any, FilterOperator> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates a filter operator select.
	 * @param props - Widget props including the source operators.
	 */
	constructor(props: WidgetProps<{
		source: FilterOperator[];
	}>);
	/**
	 * Returns the empty item text.
	 * @returns Null.
	 */
	emptyItemText(): string;
	/**
	 * Returns the combobox options.
	 * @returns Combobox options.
	 */
	getComboboxOptions(): ComboboxOptions<any>;
}
/**
 * A panel for building filter criteria with multiple filter lines.
 * @typeParam P - Widget props type.
 */
export declare class FilterPanel<P = {}> extends FilterWidgetBase<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private rowsDiv;
	private resetButton;
	private searchButton;
	/**
	 * Creates a filter panel.
	 * @param props - Widget props.
	 */
	constructor(props: WidgetProps<P>);
	private _showInitialLine;
	/**
	 * Whether an initial empty line is shown.
	 * @returns True when shown.
	 */
	get showInitialLine(): boolean;
	/** Sets whether an initial empty line is shown. */
	set showInitialLine(value: boolean);
	/**
	 * Updates the rows when the filter store changes.
	 */
	protected filterStoreChanged(): void;
	/**
	 * Rebuilds the filter rows from the store.
	 */
	updateRowsFromStore(): void;
	private _showSearchButton;
	/**
	 * Whether the search button is shown.
	 * @returns True when shown.
	 */
	get showSearchButton(): boolean;
	/** Sets whether the search button is shown. */
	set showSearchButton(value: boolean);
	/** Whether the store is updated when the panel is reset. */
	updateStoreOnReset: boolean;
	/**
	 * Renders the filter panel contents.
	 * @returns The rendered content.
	 */
	protected renderContents(): any;
	/**
	 * Handles the search button click.
	 * @param e - The click event.
	 */
	protected searchButtonClick(e: Event): void;
	/**
	 * Whether the panel has validation errors.
	 * @returns True when errors exist.
	 */
	get hasErrors(): boolean;
	/**
	 * Whether the panel has validation errors.
	 * @returns True when errors exist.
	 */
	protected get_hasErrors(): boolean;
	/**
	 * Builds filter lines from the current rows and updates the store.
	 */
	search(): void;
	/**
	 * Handles the add-button click.
	 * @param e - The click event.
	 */
	protected addButtonClick(e: Event): void;
	/**
	 * Handles the reset-button click.
	 * @param e - The click event.
	 */
	protected resetButtonClick(e: Event): void;
	/**
	 * Finds an empty row, if any.
	 * @returns The empty row element, or null.
	 */
	protected findEmptyRow(): HTMLElement;
	/**
	 * Adds an empty filter row.
	 * @param popupField - Whether to open the field dropdown.
	 * @returns The new row element.
	 */
	protected addEmptyRow(popupField: boolean): HTMLElement;
	/**
	 * Handles the field change event.
	 * @param e - The change event.
	 */
	protected onRowFieldChange(e: Event): void;
	/**
	 * Handles a field change for a row.
	 * @param row - The row element.
	 */
	protected rowFieldChange(row: HTMLElement): void;
	/**
	 * Removes the filtering handler from a row.
	 * @param row - The row element.
	 */
	protected removeFiltering(row: HTMLElement): void;
	/**
	 * Populates the operator list for a row.
	 * @param row - The row element.
	 */
	protected populateOperatorList(row: HTMLElement): void;
	/**
	 * Returns the field for a row.
	 * @param row - The row element.
	 * @returns The field, or null.
	 */
	protected getFieldFor(row: HTMLElement): PropertyItem;
	/**
	 * Returns the filtering handler for a row.
	 * @param row - The row element.
	 * @returns The filtering handler, or null.
	 */
	protected getFilteringFor(row: HTMLElement): IFiltering;
	/**
	 * Handles the operator change event.
	 * @param e - The change event.
	 */
	protected onRowOperatorChange(e: Event): void;
	/**
	 * Handles an operator change for a row.
	 * @param row - The row element.
	 */
	protected rowOperatorChange(row: HTMLElement): void;
	/**
	 * Handles the delete-row click.
	 * @param e - The click event.
	 */
	protected deleteRowClick(e: Event): void;
	/**
	 * Updates the visibility of the search and reset buttons.
	 */
	protected updateButtons(): void;
	/**
	 * Handles the and/or toggle click.
	 * @param e - The click event.
	 */
	protected andOrClick(e: Event): void;
	/**
	 * Handles the left/right parenthesis click.
	 * @param e - The click event.
	 */
	protected leftRightParenClick(e: Event): void;
	/**
	 * Updates the parenthesis indicators for all rows.
	 */
	protected updateParens(): void;
}
/**
 * A dialog that hosts a {@link FilterPanel} for editing filters.
 * @typeParam P - Widget props type.
 */
export declare class FilterDialog<P = {}> extends BaseDialog<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private filterPanel;
	/**
	 * Creates a filter dialog.
	 * @param props - Widget props.
	 */
	constructor(props: WidgetProps<P>);
	/**
	 * Returns the filter panel.
	 * @returns The filter panel.
	 */
	get_filterPanel(): FilterPanel;
	/**
	 * Renders the dialog contents with the filter panel.
	 * @returns The rendered content.
	 */
	protected renderContents(): any;
	/**
	 * Returns the dialog options.
	 * @returns Dialog options.
	 */
	protected getDialogOptions(): DialogOptions;
	/**
	 * Returns the dialog buttons.
	 * @returns Dialog button definitions.
	 */
	protected getDialogButtons(): DialogButton[];
}
declare class FilteringTypeRegistryImpl extends BaseTypeRegistry<Function> {
	/**
	 * Creates the filtering type registry.
	 */
	constructor();
	/**
	 * Whether a type is a matching filtering handler.
	 * @param type - The type to check.
	 * @returns True when assignable from IFiltering.
	 */
	protected isMatchingType(type: any): boolean;
	/**
	 * Returns the error for a missing filtering handler.
	 * @param key - The missing key.
	 * @returns The error.
	 */
	protected loadError(key: string): void;
}
/**
 * Registry for filtering handler types, resolved by filtering type key.
 */
export declare const FilteringTypeRegistry: FilteringTypeRegistryImpl;
/**
 * Filtering handler for integer fields using an integer editor.
 */
export declare class IntegerFiltering extends BaseEditorFiltering<IntegerEditor> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates an integer filtering handler.
	 */
	constructor();
	/**
	 * Returns the operators supported by this filtering handler.
	 * @returns The operators.
	 */
	getOperators(): FilterOperator[];
}
/**
 * Filtering handler for lookup fields using a lookup editor.
 */
export declare class LookupFiltering extends BaseEditorFiltering<LookupEditor> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates a lookup filtering handler.
	 */
	constructor();
	/**
	 * Returns the operators supported by this filtering handler.
	 * @returns The operators.
	 */
	getOperators(): FilterOperator[];
	/**
	 * Whether the current operator uses an editor.
	 * @returns True when eq or ne.
	 */
	protected useEditor(): boolean;
	/**
	 * Whether to use the id field for the criteria.
	 * @returns True when an editor is used.
	 */
	protected useIdField(): boolean;
	/**
	 * Returns the display text of the current editor value.
	 * @returns The editor text.
	 */
	getEditorText(): string;
}
/**
 * Filtering handler for service lookup fields using a service lookup editor.
 */
export declare class ServiceLookupFiltering extends BaseEditorFiltering<ServiceLookupEditor> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates a service lookup filtering handler.
	 */
	constructor();
	/**
	 * Returns the operators supported by this filtering handler.
	 * @returns The operators.
	 */
	getOperators(): FilterOperator[];
	/**
	 * Whether the current operator uses an editor.
	 * @returns True when eq or ne.
	 */
	protected useEditor(): boolean;
	/**
	 * Whether to use the id field for the criteria.
	 * @returns True when an editor is used.
	 */
	protected useIdField(): boolean;
	/**
	 * Returns the display text of the current editor value.
	 * @returns The editor text.
	 */
	getEditorText(): string;
}
/**
 * Filtering handler for string fields.
 */
export declare class StringFiltering extends BaseFiltering {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Returns the operators supported by this filtering handler.
	 * @returns The operators.
	 */
	getOperators(): FilterOperator[];
	/**
	 * Validates the editor value, allowing empty values.
	 * @param value - The value to validate.
	 * @returns The validated value.
	 */
	validateEditorValue(value: string): string;
}
/**
 * Renders a boolean value as localized text and/or an icon.
 * Falls back to `DialogTexts.YesButton` / `NoButton` when no explicit texts are provided.
 */
export declare class BooleanFormatter implements Formatter {
	readonly props: {
		falseText?: string;
		falseIcon?: IconClassName;
		nullText?: string;
		nullIcon?: IconClassName;
		trueText?: string;
		trueIcon?: IconClassName;
		showText?: boolean;
		showHint?: boolean;
	};
	static [Symbol.typeInfo]: FormatterTypeInfo<"Serenity.">;
	/**
	 * @param props.falseText - Text for `false` values.
	 * @param props.falseIcon - Icon class for `false` values.
	 * @param props.nullText - Text for `null` values.
	 * @param props.nullIcon - Icon class for `null` values.
	 * @param props.trueText - Text for `true` values.
	 * @param props.trueIcon - Icon class for `true` values.
	 * @param props.showText - Whether to show text alongside icon (default `true`).
	 * @param props.showHint - Whether to show text as `title` hint.
	 */
	constructor(props?: {
		falseText?: string;
		falseIcon?: IconClassName;
		nullText?: string;
		nullIcon?: IconClassName;
		trueText?: string;
		trueIcon?: IconClassName;
		showText?: boolean;
		showHint?: boolean;
	});
	/**
	 * Formats the boolean value for display.
	 * @param ctx - Formatter context with value/item/column.
	 * @returns Text, icon, or combined span per `props`.
	 */
	format(ctx: FormatterContext): FormatterResult;
	get falseText(): string;
	set falseText(value: string);
	get trueText(): string;
	set trueText(value: string);
}
/**
 * Renders a boolean as a checkbox-like visual (with optional text). In grid display
 * the checkbox is read-only; in header-filter context it falls back to text/icon.
 */
export declare class CheckboxFormatter implements Formatter {
	readonly props: {
		falseText?: string;
		falseIcon?: IconClassName;
		nullText?: string;
		nullIcon?: IconClassName;
		trueText?: string;
		trueIcon?: IconClassName;
		showText?: boolean;
		showHint?: boolean;
	};
	static [Symbol.typeInfo]: FormatterTypeInfo<"Serenity.">;
	/**
	 * @param props.falseText - Text for `false`.
	 * @param props.falseIcon - Icon for `false`.
	 * @param props.nullText - Text for `null`.
	 * @param props.nullIcon - Icon for `null`.
	 * @param props.trueText - Text for `true`.
	 * @param props.trueIcon - Icon for `true`.
	 * @param props.showText - Whether to show text (defaults to `true`, or `false` in grid cells).
	 * @param props.showHint - Whether to surface text as `title` hint.
	 */
	constructor(props?: {
		falseText?: string;
		falseIcon?: IconClassName;
		nullText?: string;
		nullIcon?: IconClassName;
		trueText?: string;
		trueIcon?: IconClassName;
		showText?: boolean;
		showHint?: boolean;
	});
	/** @param ctx - Formatter context. @returns Checkbox / icon / text markup. */
	format(ctx: FormatterContext): FormatterResult;
}
/** Formats date values using {@link formatDate} / {@link Culture.dateFormat}. */
export declare class DateFormatter implements Formatter {
	readonly props: {
		displayFormat?: string;
	};
	static [Symbol.typeInfo]: FormatterTypeInfo<"Serenity.">;
	/**
	 * @param props.displayFormat - Date format string (default `Culture.dateFormat`).
	 */
	constructor(props?: {
		displayFormat?: string;
	});
	/**
	 * Static helper to format any date-like value.
	 * @param value - Date instance or ISO string.
	 * @param format - Format string (defaults to culture format).
	 * @returns HTML-encoded formatted string.
	 */
	static format(value: any, format?: string): any;
	get displayFormat(): string;
	set displayFormat(value: string);
	/** @param ctx - Formatter context. @returns Formatted date string. */
	format(ctx: FormatterContext): string;
}
/** Variant of {@link DateFormatter} that defaults to `Culture.dateTimeFormat`. */
export declare class DateTimeFormatter extends DateFormatter {
	static [Symbol.typeInfo]: FormatterTypeInfo<"Serenity.">;
	/**
	 * @param props.displayFormat - Date-time format string (default `Culture.dateTimeFormat`).
	 */
	constructor(props?: {
		displayFormat?: string;
	});
}
/** Renders enum values as localized text via `Enums.<EnumKey>.<Name>`. */
export declare class EnumFormatter implements Formatter {
	readonly props: {
		enumKey?: string;
	};
	static [Symbol.typeInfo]: FormatterTypeInfo<"Serenity.">;
	/**
	 * @param props.enumKey - Full enum key (e.g. `"MyProject.MyEnum"`). Resolved via {@link EnumTypeRegistry}.
	 */
	constructor(props?: {
		enumKey?: string;
	});
	/** @param ctx - Formatter context containing enum value. @returns Localized enum text (or async placeholder). */
	format(ctx: FormatterContext): FormatterResult;
	get enumKey(): string;
	set enumKey(value: string);
	/**
	 * Formats an enum value given an enum type.
	 * @param enumType - Registered enum object.
	 * @param value - Enum numeric value.
	 * @returns Localized display text.
	 */
	static format(enumType: any, value: any): string;
	/**
	 * Gets localized text for an enum name.
	 * @param enumKey - Enum key (e.g. `"MyEnum"`).
	 * @param name - Member name.
	 * @returns Localized string (falls back to name).
	 */
	static getText(enumKey: string, name: string): string;
	/**
	 * Gets the member name for a value.
	 * @param enumType - Enum object.
	 * @param value - Numeric value.
	 * @returns Enum member name or empty string.
	 */
	static getName(enumType: any, value: any): string;
}
/**
 * Type token for formatters/editors that need to modify their grid column at setup time
 * (e.g. to declare `referencedFields`).
 */
export declare abstract class IInitializeColumn {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface IInitializeColumn {
	/**
	 * Called during column construction to allow the formatter to adjust column metadata.
	 * @param column - Mutable column definition to initialize.
	 */
	initializeColumn(column: Column): void;
}
/** Renders a DB file path as a download link with an icon and optional original name. */
export declare class FileDownloadFormatter implements Formatter, IInitializeColumn {
	readonly props: {
		displayFormat?: string;
		originalNameProperty?: string;
		iconClass?: string;
	};
	static [Symbol.typeInfo]: FormatterTypeInfo<"Serenity.">;
	/**
	 * @param props.displayFormat - Format string for link text (default `"{0}"`).
	 * @param props.originalNameProperty - Field holding the original file name.
	 * @param props.iconClass - Icon class for the download icon.
	 */
	constructor(props?: {
		displayFormat?: string;
		originalNameProperty?: string;
		iconClass?: string;
	});
	/** @param ctx - Formatter context. @returns Anchor element for the file. */
	format(ctx: FormatterContext): FormatterResult;
	/**
	 * Builds the download URL for a temp/upload file.
	 * @param filename - Stored file path.
	 * @returns Resolved URL under `~/upload/`.
	 */
	static dbFileUrl(filename: string): string;
	/**
	 * Declares `originalNameProperty` as a referenced field so it is fetched for formatting.
	 * @param column - Column being initialized.
	 */
	initializeColumn(column: Column): void;
	get displayFormat(): string;
	set displayFormat(value: string);
	get originalNameProperty(): string;
	set originalNameProperty(value: string);
	get iconClass(): string;
	set iconClass(value: string);
}
/**
 * Base class for Serenity formatters. Provides the static `registerFormatter` helper
 * used by formatter subclasses with `static [Symbol.typeInfo] = this.registerFormatter(...)`.
 */
export declare abstract class FormatterBase implements Formatter {
	/**
	 * Formats the cell value.
	 * @param ctx - Formatter context containing item/column/value/grid.
	 * @returns Rendered content for the cell.
	 */
	abstract format(ctx: FormatterContext): FormatterResult;
	/**
	 * Registers the formatter type under the given formal name.
	 * @typeParam TypeName - String literal for the formatter's full name.
	 * @param typeName - Full name (e.g. `"MyProject.MyFormatter"`).
	 * @param intfAndAttr - Optional interfaces / attribute specifiers.
	 * @returns The created type info.
	 */
	protected static registerFormatter<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): FormatterTypeInfo<TypeName>;
	static [Symbol.typeInfo]: FormatterTypeInfo<"Serenity.">;
}
/** Formats an integer minute count as `HH:mm` (e.g. 90 → `"01:30"`). */
export declare class MinuteFormatter implements Formatter {
	static [Symbol.typeInfo]: FormatterTypeInfo<"Serenity.">;
	/** @param ctx - Formatter context. @returns `HH:mm` string. */
	format(ctx: FormatterContext): string;
	/**
	 * Static helper to format minutes.
	 * @param value - Total minutes.
	 * @returns `HH:mm` string or empty if invalid.
	 */
	static format(value: number): string;
}
/** Formats numeric values via {@link formatNumber} (default `"0.##"`). */
export declare class NumberFormatter implements Formatter {
	readonly props: {
		displayFormat?: string;
	};
	static [Symbol.typeInfo]: FormatterTypeInfo<"Serenity.">;
	/**
	 * @param props.displayFormat - Number format string (default `"0.##"`).
	 */
	constructor(props?: {
		displayFormat?: string;
	});
	/** @param ctx - Formatter context. @returns Formatted number string. */
	format(ctx: FormatterContext): string;
	/**
	 * Static helper to format any numeric-like value.
	 * @param value - Number or numeric string.
	 * @param format - Format string (default `"0.##"`).
	 * @returns Formatted string.
	 */
	static format(value: any, format?: string): string;
	get displayFormat(): string;
	set displayFormat(value: string);
}
/** Renders a value as a hyperlink with configurable URL / display mapping. */
export declare class UrlFormatter implements Formatter, IInitializeColumn {
	readonly props: {
		displayProperty?: string;
		displayFormat?: string;
		urlProperty?: string;
		urlFormat?: string;
		target?: string;
	};
	static [Symbol.typeInfo]: FormatterTypeInfo<"Serenity.">;
	/**
	 * @param props.displayProperty - Item field used for link text (defaults to cell value).
	 * @param props.displayFormat - Format string applied to display value.
	 * @param props.urlProperty - Item field used for URL (defaults to cell value).
	 * @param props.urlFormat - Format string applied to URL value.
	 * @param props.target - Anchor target (e.g. `"_blank"`).
	 */
	constructor(props?: {
		displayProperty?: string;
		displayFormat?: string;
		urlProperty?: string;
		urlFormat?: string;
		target?: string;
	});
	/** @param ctx - Formatter context. @returns Anchor element or empty string. */
	format(ctx: FormatterContext): FormatterResult;
	/**
	 * Declares any referenced fields so they are fetched for formatting.
	 * @param column - Column being initialized.
	 */
	initializeColumn(column: Column): void;
	get displayProperty(): string;
	set displayProperty(value: string);
	get displayFormat(): string;
	set displayFormat(value: string);
	get urlProperty(): string;
	set urlProperty(value: string);
	get urlFormat(): string;
	set urlFormat(value: string);
	get target(): string;
	set target(value: string);
}
/**
 * Base class for column definitions. Exposes each column as a property of the
 * instance, keyed by its id, source item name, or field name.
 * @typeParam TRow - The type of the row data.
 */
export declare class ColumnsBase<TRow = any> {
	private __items;
	/**
	 * Creates a new ColumnsBase instance from the given column definitions.
	 * @param items - The column definitions.
	 */
	constructor(items: Column<TRow>[]);
	/**
	 * Returns the underlying column definitions array.
	 * @returns The column definitions.
	 */
	valueOf(): Column<TRow>[];
}
/**
 * Renders an edit link for a given item type and ID.
 * The link will have a CSS class based on the item type and will point to a URL fragment
 * that includes the item type and ID.
 * This is similar to SlickHelper.itemLink function, but it doesn't require a grid context
 * and does not accept FormatterResult (e.g. html string markup) as children.
 *
 * @param props - The properties for the edit link.
 * @returns An HTML anchor element representing the edit link.
 */
export declare function EditLink(props: {
	/**
	 * The ID of the item to link to.
	 */
	itemId: any;
	/**
	 * The type of the item, e.g. "Northwind.Customer".
	 */
	itemType?: string;
	/**
	 * Additional CSS class to add to the link (besides s-EditLink and s-[ItemType]Link)
	 */
	cssClass?: string;
	/**
	 * Tab index for the link. Default is null, which means no tabindex attribute.
	 */
	tabindex?: number;
	/** @deprecated Use tabindex. */
	tabIndex?: number;
	/**
	 * Child elements or text to be displayed inside the link.
	 */
	children?: any;
}): HTMLAnchorElement;
/**
 * Formatting purposes for which the edit link should be skipped (e.g. exports,
 * group headers/totals, header filters, and print).
 */
export declare const skipEditLinkFormatPurposes: Set<string>;
/**
 * Options for the {@link GridRadioSelectionMixin}.
 */
export interface GridRadioSelectionMixinOptions {
	/**
	 * A function that determines whether an item can be selected.
	 */
	selectable?: (item: any) => boolean;
}
/**
 * A mixin that adds single (radio) row selection behavior to a data grid.
 */
export declare class GridRadioSelectionMixin {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private idField;
	private include;
	private grid;
	private options;
	/**
	 * Creates a new GridRadioSelectionMixin for the given grid.
	 * @param grid - The data grid to attach the mixin to.
	 * @param options - Optional mixin options.
	 */
	constructor(grid: IDataGrid, options?: GridRadioSelectionMixinOptions);
	private isSelectable;
	/**
	 * Clears the current selection.
	 */
	clear(): void;
	/**
	 * Clears the current selection and refreshes the grid view.
	 */
	resetCheckedAndRefresh(): void;
	/**
	 * Returns the key of the currently selected item, or null if none is selected.
	 * @returns The selected key, or null.
	 */
	getSelectedKey(): string;
	/**
	 * Returns the selected key parsed as a 32-bit integer, or null if none is selected.
	 * @returns The selected key as an int32, or null.
	 */
	getSelectedAsInt32(): number | null;
	/**
	 * Returns the selected key parsed as a 64-bit integer, or null if none is selected.
	 * @returns The selected key as an int64, or null.
	 */
	getSelectedAsInt64(): number | null;
	/**
	 * Selects the item with the given key, clearing any previous selection.
	 * @param key - The key of the item to select.
	 */
	setSelectedKey(key: string): void;
	/**
	 * Creates a radio select column for the grid.
	 * @param getMixin - A function that returns the mixin instance.
	 * @param columnOptions - Optional column options to merge into the select column.
	 * @returns The select column definition.
	 */
	static createSelectColumn(getMixin: () => GridRadioSelectionMixin, columnOptions?: Partial<Column>): Column;
}
/**
 * Options for the {@link GridRowSelectionMixin}.
 */
export interface GridRowSelectionMixinOptions {
	/**
	 * A function that determines whether an item can be selected.
	 */
	selectable?: (item: any) => boolean;
}
/**
 * A mixin that adds multi (checkbox) row selection behavior to a data grid,
 * including a select-all header checkbox.
 */
export declare class GridRowSelectionMixin {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private idField;
	private include;
	private grid;
	private options;
	/**
	 * Creates a new GridRowSelectionMixin for the given grid.
	 * @param grid - The data grid to attach the mixin to.
	 * @param options - Optional mixin options.
	 */
	constructor(grid: IDataGrid, options?: GridRowSelectionMixinOptions);
	/**
	 * Detaches the mixin from the grid and cleans up event subscriptions.
	 */
	destroy(): void;
	private handleGridClick;
	private handleHeaderClick;
	/**
	 * Updates the checked state of the select-all header button based on the
	 * current selection.
	 */
	updateSelectAll(): void;
	/**
	 * Clears the current selection.
	 */
	clear(): void;
	/**
	 * Clears the current selection and refreshes the grid view.
	 */
	resetCheckedAndRefresh(): void;
	/**
	 * Selects the items with the given keys, keeping any existing selection.
	 * @param keys - The keys of the items to select.
	 */
	selectKeys(keys: string[]): void;
	/**
	 * Returns the keys of the currently selected items.
	 * @returns The selected keys.
	 */
	getSelectedKeys(): string[];
	/**
	 * Returns the selected keys parsed as 32-bit integers.
	 * @returns The selected keys as int32 values.
	 */
	getSelectedAsInt32(): number[];
	/**
	 * Returns the selected keys parsed as 64-bit integers.
	 * @returns The selected keys as int64 values.
	 */
	getSelectedAsInt64(): number[];
	/**
	 * Replaces the current selection with the items having the given keys.
	 * @param keys - The keys of the items to select.
	 */
	setSelectedKeys(keys: string[]): void;
	private isSelectable;
	/**
	 * Creates a checkbox select column for the grid, including a select-all header.
	 * @param getMixin - A function that returns the mixin instance.
	 * @param columnOptions - Optional column options to merge into the select column.
	 * @returns The select column definition.
	 */
	static createSelectColumn(getMixin: () => GridRowSelectionMixin, columnOptions?: Partial<Column>): Column;
}
/**
 * Helper functions for managing a "select all" toolbar button on a data grid.
 */
export declare namespace GridSelectAllButtonHelper {
	/**
	 * Updates the checked state of the select-all button based on whether all
	 * items in the grid are selected.
	 * @param grid - The data grid.
	 * @param getSelected - A function that returns whether an item is selected.
	 */
	function update(grid: IDataGrid, getSelected: (p1: any) => boolean): void;
	/**
	 * Defines a select-all toolbar button that selects or deselects all items.
	 * @param getGrid - A function that returns the data grid.
	 * @param getId - A function that returns the id of an item.
	 * @param getSelected - A function that returns whether an item is selected.
	 * @param setSelected - A function that sets the selected state of an item.
	 * @param text - Optional button title text. Defaults to the "Select All" text.
	 * @param onClick - Optional callback invoked after the selection is updated.
	 * @returns The toolbar button definition.
	 */
	function define(getGrid: () => IDataGrid, getId: (p1: any) => any, getSelected: (p1: any) => boolean, setSelected: (p1: any, p2: boolean) => void, text?: string, onClick?: () => void): ToolButton;
}
/**
 * Utility functions for working with data grids.
 */
export declare namespace GridUtils {
	/**
	 * Adds a toggle button to a toolbar.
	 * @param toolDiv - The toolbar element (or array-like of elements).
	 * @param cssClass - The CSS class to add to the button.
	 * @param callback - Callback invoked with the new pressed state when toggled.
	 * @param hint - The tooltip text for the button.
	 * @param initial - Optional initial pressed state.
	 */
	function addToggleButton(toolDiv: HTMLElement | ArrayLike<HTMLElement>, cssClass: string, callback: (p1: boolean) => void, hint: string, initial?: boolean): void;
	/**
	 * Adds an "include deleted" toggle button that sets the IncludeDeleted
	 * parameter on the view's submit requests.
	 * @param toolDiv - The toolbar element (or array-like of elements).
	 * @param view - The remote view.
	 * @param hint - Optional tooltip text.
	 * @param initial - Optional initial state.
	 */
	function addIncludeDeletedToggle(toolDiv: HTMLElement | ArrayLike<HTMLElement>, view: IRemoteView<any>, hint?: string, initial?: boolean): void;
	/**
	 * Adds a quick search input to a container and wires it to the given view.
	 * @param options - The quick search options.
	 * @returns The created QuickSearchInput widget.
	 */
	function addQuickSearch({ container, fields, beforeSearch, search, view }: {
		container: HTMLElement | ArrayLike<HTMLElement>;
		fields?: QuickSearchField[];
		beforeSearch?: (args: QuickSearchArgs) => void;
		search?: (args: QuickSearchArgs) => void;
		view?: IRemoteView<any>;
	}): QuickSearchInput;
	/** @deprecated use addQuickSearch with named args
	 * Adds a quick search input to a toolbar.
	 * @param toolDiv - The toolbar element (or array-like of elements).
	 * @param view - The remote view.
	 * @param fields - Optional quick search fields.
	 * @param onChange - Optional callback invoked before searching.
	 * @returns The created QuickSearchInput widget.
	 */
	function addQuickSearchInput(toolDiv: HTMLElement | ArrayLike<HTMLElement>, view: IRemoteView<any>, fields?: QuickSearchField[], onChange?: () => void): QuickSearchInput;
	/** @deprecated use addQuickSearch with named args
	 * Adds a quick search input with a custom search handler.
	 * @param container - The container element (or array-like of elements).
	 * @param search - The custom search handler.
	 * @param fields - Optional quick search fields.
	 * @returns The created QuickSearchInput widget.
	 */
	function addQuickSearchInputCustom(container: HTMLElement | ArrayLike<HTMLElement>, search: (field: QuickSearchArgs["field"], query: QuickSearchArgs["query"], done: QuickSearchArgs["done"]) => void, fields?: QuickSearchField[]): QuickSearchInput;
	/**
	 * Makes the rows of a grid reorderable by dragging.
	 * @param grid - The sleek grid.
	 * @param handleMove - Callback invoked with the moved row indexes and the insert-before index.
	 */
	function makeOrderable(grid: ISleekGrid, handleMove: (rows: number[], insertBefore: number) => void): void;
	/**
	 * Makes the rows of a data grid reorderable and persists the new order by
	 * sending update requests to the server.
	 * @typeParam TItem - The type of the row items.
	 * @typeParam TId - The type of the item id.
	 * @param dataGrid - The data grid.
	 * @param getId - A function that returns the id of an item.
	 * @param getDisplayOrder - A function that returns the display order of an item.
	 * @param service - The service URL to send update requests to.
	 * @param getUpdateRequest - A function that builds an update request for an item id and order.
	 */
	function makeOrderableWithUpdateRequest<TItem = any, TId = any>(dataGrid: IDataGrid, getId: (item: TItem) => TId, getDisplayOrder: (item: TItem) => any, service: string, getUpdateRequest: (id: TId, order: number) => SaveRequest<TItem>): void;
}
/**
 * Helper functions for lazy loading content when it becomes visible.
 */
export declare namespace LazyLoadHelper {
	/**
	 * Executes the given callback once when the element becomes visible.
	 */
	const executeOnceWhenShown: typeof executeOnceWhenVisible;
	/**
	 * Executes the given callback every time the element becomes visible.
	 */
	const executeEverytimeWhenShown: typeof executeEverytimeWhenVisible;
}
/**
 * Converts {@link PropertyItem} definitions into sleek grid column definitions.
 */
export declare namespace PropertyItemColumnConverter {
	/**
	 * Converts an array of property items into column definitions.
	 * @param items - The property items to convert.
	 * @returns The resulting column definitions.
	 */
	function toColumns(items: PropertyItem[]): Column[];
	/**
	 * Converts a single property item into a column definition.
	 * @param item - The property item to convert.
	 * @returns The resulting column definition.
	 */
	function toColumn(item: PropertyItem): Column;
}
/**
 * Formatting helpers for sleek grids.
 */
export declare namespace SlickFormatting {
	/**
	 * Returns a formatter that renders an edit link for an item.
	 * @typeParam TItem - The type of the row item.
	 * @param itemType - The type of the item, e.g. "Northwind.Customer".
	 * @param idField - The name of the field holding the item id.
	 * @param getText - A formatter that produces the link text, or null to use the raw value.
	 * @param cssClass - Optional function returning an extra CSS class for the link.
	 * @param encode - Whether to HTML-encode the text. Defaults to true.
	 * @returns The item link formatter.
	 */
	function itemLink<TItem = any>(itemType: string, idField: string, getText: Format<TItem>, cssClass?: (ctx: FormatterContext<TItem>) => string, encode?: boolean): Format<TItem>;
	/**
	 * Returns a formatter that renders a tree toggle (expand/collapse) control
	 * with indentation based on the item's hierarchy.
	 * @param getView - A function that returns the remote view.
	 * @param getId - A function that returns the id of an item.
	 * @param formatter - The formatter used to render the item content.
	 * @returns The tree toggle formatter.
	 */
	function treeToggle(getView: () => IRemoteView<any>, getId: (x: any) => any, formatter: Format): Format;
}
/**
 * Helper functions for sleek grids.
 */
export declare namespace SlickHelper {
	/**
	 * Applies default values to column definitions, such as sortability, id,
	 * and localized names.
	 * @param columns - The column definitions to update.
	 * @param localTextPrefix - Optional local text prefix used to localize column names.
	 * @returns The updated column definitions.
	 */
	function setDefaults(columns: Column[], localTextPrefix?: string): any;
}
/**
 * Helper functions for tree-structured data in grids.
 */
export declare namespace SlickTreeHelper {
	/**
	 * Returns whether an item should be visible given the collapsed state of its
	 * ancestors.
	 * @typeParam TItem - The type of the item.
	 * @param item - The item to check.
	 * @param getParent - A function that returns the parent of an item.
	 * @returns True if the item is visible, otherwise false.
	 */
	function filterCustom<TItem>(item: TItem, getParent: (x: TItem) => any): boolean;
	/**
	 * Returns whether an item should be visible by resolving its parent chain
	 * through the view.
	 * @typeParam TItem - The type of the item.
	 * @param item - The item to check.
	 * @param view - The remote view used to resolve parents.
	 * @param getParentId - A function that returns the parent id of an item.
	 * @returns True if the item is visible, otherwise false.
	 */
	function filterById<TItem>(item: TItem, view: IRemoteView<TItem>, getParentId: (x: TItem) => any): boolean;
	/**
	 * Sets the collapsed state of all given items.
	 * @typeParam TItem - The type of the item.
	 * @param items - The items to update.
	 * @param collapsed - The collapsed state to set.
	 */
	function setCollapsed<TItem>(items: TItem[], collapsed: boolean): void;
	/**
	 * Sets the collapsed state of a single item.
	 * @typeParam TItem - The type of the item.
	 * @param item - The item to update.
	 * @param collapsed - The collapsed state to set.
	 */
	function setCollapsedFlag<TItem>(item: TItem, collapsed: boolean): void;
	/**
	 * Computes and sets the indent level of each item based on its parent chain.
	 * @typeParam TItem - The type of the item.
	 * @param items - The items to update.
	 * @param getId - A function that returns the id of an item.
	 * @param getParentId - A function that returns the parent id of an item.
	 * @param setCollapsed - Optional collapsed state to set on each item.
	 */
	function setIndents<TItem>(items: TItem[], getId: (x: TItem) => any, getParentId: (x: TItem) => any, setCollapsed?: boolean): void;
	/**
	 * Handles a click on a tree toggle, expanding or collapsing the item and its
	 * descendants when the shift key is held.
	 * @typeParam TItem - The type of the item.
	 * @param e - The click event.
	 * @param row - The row index of the clicked item.
	 * @param cell - The cell index of the clicked item.
	 * @param view - The remote view.
	 * @param getId - A function that returns the id of an item.
	 */
	function toggleClick<TItem>(e: Event, row: number, cell: number, view: IRemoteView<TItem>, getId: (x: TItem) => any): void;
}
/**
 * Helper functions for coordinating data changes between dialogs and their
 * owner widgets.
 */
export declare namespace SubDialogHelper {
	/**
	 * Binds a data change handler to a dialog so it is invoked when the dialog
	 * raises a data change event.
	 * @param dialog - The dialog to bind to.
	 * @param owner - The owner widget.
	 * @param dataChange - The handler invoked on data change.
	 * @param useTimeout - Whether to invoke the handler asynchronously via a timeout.
	 * @returns The dialog.
	 */
	function bindToDataChange(dialog: any, owner: Widget<any>, dataChange: (ev: DataChangeInfo) => void, useTimeout?: boolean): any;
	/**
	 * Triggers a data change event on the given dialog.
	 * @param dialog - The dialog to trigger the event on.
	 * @returns The dialog.
	 */
	function triggerDataChange(dialog: Widget<any>): any;
	/**
	 * Triggers a data change event on the given element.
	 * @param element - The element (or array-like of elements) to trigger the event on.
	 */
	function triggerDataChanged(element: HTMLElement | ArrayLike<HTMLElement>): void;
	/**
	 * Binds a dialog's data change event so it bubbles up to the owner widget.
	 * @param dialog - The dialog to bind to.
	 * @param owner - The owner widget to bubble the event to.
	 * @param useTimeout - Whether to invoke the handler asynchronously via a timeout.
	 * @returns The dialog.
	 */
	function bubbleDataChange(dialog: any, owner: Widget<any>, useTimeout?: boolean): any;
	/**
	 * Positions a cascaded dialog relative to the element that opened it.
	 * @param cascadedDialog - The cascaded dialog to position.
	 * @param ofElement - The element (or array-like of elements) the dialog is cascaded from.
	 * @returns The cascaded dialog.
	 */
	function cascade(cascadedDialog: {
		domNode: HTMLElement;
	}, ofElement: HTMLElement | ArrayLike<HTMLElement>): any;
	/**
	 * Returns the jQuery dialog position options used to cascade a dialog from
	 * the given element.
	 * @param element - The element (or array-like of elements) to cascade from.
	 * @returns The dialog position options.
	 */
	function cascadedDialogOffset(element: HTMLElement | ArrayLike<HTMLElement>): any;
}
/**
 * Helper functions for working with tab controls, supporting both jQuery UI
 * tabs and Bootstrap-style tabs.
 */
export declare namespace TabsExtensions {
	/**
	 * Enables or disables a tab.
	 * @param tabs - The tabs element (or array-like of elements).
	 * @param tabKey - The tab key or index.
	 * @param isDisabled - Whether the tab should be disabled.
	 */
	function setDisabled(tabs: ArrayLike<HTMLElement> | HTMLElement, tabKey: string | number, isDisabled: boolean): void;
	/**
	 * Shows or hides a tab.
	 * @param tabs - The tabs element (or array-like of elements).
	 * @param tabKey - The tab key or index.
	 * @param visible - Whether the tab should be visible.
	 */
	function toggle(tabs: ArrayLike<HTMLElement> | HTMLElement, tabKey: string | number, visible: boolean): void;
	/**
	 * Returns the key of the currently active tab.
	 * @param tabs - The tabs element (or array-like of elements).
	 * @returns The active tab key.
	 */
	function activeTabKey(tabs: ArrayLike<HTMLElement> | HTMLElement): string;
	/**
	 * Returns a mapping of tab keys to their zero-based index.
	 * @param tabs - The tabs element (or array-like of elements).
	 * @returns A record mapping tab keys to indexes.
	 */
	function indexByKey(tabs: ArrayLike<HTMLElement> | HTMLElement): Record<string, number>;
	/**
	 * Selects (activates) the tab with the given key or index.
	 * @param tabs - The tabs element (or array-like of elements).
	 * @param tabKey - The tab key or index to select.
	 */
	function selectTab(tabs: HTMLElement | ArrayLike<HTMLElement>, tabKey: string | number): void;
	/**
	 * Initializes a tabs control, using jQuery UI tabs if available, otherwise
	 * emulating them with Bootstrap.
	 * @param tabs - The tabs element (or array-like of elements).
	 * @param activeChange - Optional callback invoked when the active tab changes.
	 * @returns A Fluent wrapper around the tabs element, or null if invalid.
	 */
	function initialize(tabs: HTMLElement | ArrayLike<HTMLElement>, activeChange: () => void): Fluent<HTMLElement>;
	/**
	 * Destroys a tabs control, cleaning up jQuery UI or Bootstrap tab instances.
	 * @param tabs - The tabs element (or array-like of elements).
	 */
	function destroy(tabs: HTMLElement | ArrayLike<HTMLElement>): void;
}
/**
 * Base class for panel-style widgets that manage a form, tabs and a toolbar.
 * It wires up validation, tab initialization and toolbar buttons from the
 * panel's DOM, and is the base for {@link PropertyPanel}.
 * @typeParam P - Widget props type.
 */
export declare class BasePanel<P = {}> extends Widget<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	/**
	 * Creates a panel, initializing the validator, tabs and toolbar.
	 * @param props - Widget props forwarded to {@link Widget}.
	 */
	constructor(props: WidgetProps<P>);
	/**
	 * Destroys the tabs, toolbar and validator, then delegates to the base destroy.
	 */
	destroy(): void;
	/** The initialized tabs element, if the panel has a `Tabs` div. */
	protected tabs: Fluent;
	/** The initialized toolbar, if the panel has a `Toolbar` div. */
	protected toolbar: Toolbar;
	/** The form validator, if the panel has a `Form` element. */
	protected validator: Validator;
	/** Whether this panel is rendered as a panel. */
	protected isPanel: boolean;
	/** Whether this panel is responsive. */
	protected responsive: boolean;
	/**
	 * Triggers a `layout` event on all visible `.require-layout` elements.
	 */
	arrange(): void;
	/**
	 * Returns the buttons to show in the panel toolbar.
	 * @returns Toolbar button definitions.
	 */
	protected getToolbarButtons(): ToolButton[];
	/**
	 * Returns the options used to configure the form validator.
	 * @returns Validator options object.
	 */
	protected getValidatorOptions(): any;
	/**
	 * Initializes the tabs from the `Tabs` div, if present.
	 */
	protected initTabs(): void;
	/**
	 * Initializes the toolbar from the `Toolbar` div, if present.
	 */
	protected initToolbar(): void;
	/**
	 * Initializes the form validator from the `Form` element, if present.
	 */
	protected initValidator(): void;
	/**
	 * Resets all validation state on the form validator, if present.
	 */
	protected resetValidation(): void;
	/**
	 * Validates the form, returning whether it is valid.
	 * @returns True if there is no validator or the form is valid.
	 */
	protected validateForm(): boolean;
}
/** @deprecated use {@link BasePanel} */
export declare const TemplatedPanel: typeof BasePanel;
/**
 * Provides a scoped context for resolving elements and widgets by an id prefix
 * within a given DOM node. Useful for panels and dialogs that need to look up
 * their child elements and widgets by short, prefix-relative ids.
 */
export declare class PrefixedContext {
	/** The id prefix used to resolve child element ids. */
	readonly idPrefix: string;
	/** The DOM node that acts as the scope for lookups. */
	readonly context: HTMLElement;
	/**
	 * Creates a new prefixed context.
	 * @param prefixOrWidget - Either a string id prefix, or an object exposing
	 *   `idPrefix` and `domNode` (such as a widget) from which the prefix and
	 *   context are derived.
	 * @param context - Optional DOM node to scope lookups to; defaults to the
	 *   `domNode` of `prefixOrWidget` when an object is provided.
	 */
	constructor(prefixOrWidget: string | {
		idPrefix: string;
		domNode: HTMLElement;
	}, context?: HTMLElement);
	/**
	 * Hook for subclasses to perform additional initialization.
	 */
	protected initialize(): void;
	/**
	 * Resolves an element by its prefix-relative id.
	 * @param id - The id relative to the context's id prefix.
	 * @returns A {@link Fluent} wrapper for the matching element, or an empty
	 *   Fluent object if no element matches.
	 */
	byId(id: string): Fluent;
	/**
	 * Resolves a widget by its prefix-relative id and expected type.
	 * @param id - The id relative to the context's id prefix.
	 * @param type - The widget type to look up.
	 * @returns The matching widget instance.
	 */
	w<TWidget>(id: string, type: {
		new (...args: any[]): TWidget;
	}): TWidget;
}
/**
 * A panel that hosts a {@link PropertyGrid} for editing an entity, providing
 * load/save of the entity and its id, and deriving form options from the
 * panel's type name.
 * @typeParam TItem - The entity type edited by the panel.
 * @typeParam P - Widget props type.
 */
export declare class PropertyPanel<TItem, P> extends BasePanel<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private _entity;
	private _entityId;
	/**
	 * Creates a property panel, initializing the property grid and loading the
	 * initial (empty) entity.
	 * @param props - Widget props forwarded to {@link BasePanel}.
	 */
	constructor(props: WidgetProps<P>);
	/**
	 * Destroys the property grid and validator, then delegates to the base destroy.
	 */
	destroy(): void;
	/**
	 * Initializes the property grid from the `PropertyGrid` div, if present.
	 */
	protected initPropertyGrid(): void;
	/**
	 * Loads an empty entity into the property grid.
	 */
	protected loadInitialEntity(): void;
	/**
	 * Returns the form key derived from the panel's type name, used to look up
	 * the form definition and local text prefix.
	 * @returns The form key (e.g. "MyPanel" for "MyModule.MyPanel").
	 */
	protected getFormKey(): string;
	/**
	 * Returns the options used to configure the property grid.
	 * @returns The property grid options.
	 */
	protected getPropertyGridOptions(): PropertyGridOptions;
	/**
	 * Returns the property items for the panel's form.
	 * @returns The property items to render.
	 */
	protected getPropertyItems(): PropertyItem[];
	/**
	 * Saves the current editor values into a new entity object.
	 * @returns The saved entity.
	 */
	protected getSaveEntity(): TItem;
	/**
	 * Gets the entity currently loaded in the panel.
	 */
	get entity(): TItem;
	/**
	 * Gets the id of the entity currently loaded in the panel.
	 */
	get entityId(): any;
	/**
	 * Sets the entity loaded in the panel.
	 * @param value - The entity to set; null is replaced with an empty object.
	 */
	protected set entity(value: TItem);
	/**
	 * Sets the id of the entity loaded in the panel.
	 * @param value - The entity id to set.
	 */
	protected set entityId(value: any);
	/**
	 * Validates the form before saving.
	 * @returns True if the form is valid.
	 */
	protected validateBeforeSave(): boolean;
	/** The property grid hosted by this panel. */
	protected propertyGrid: PropertyGrid;
}
export type Constructor<T> = new (...args: any[]) => T;

export {
	alert$1 as alert,
	confirm$1 as confirm,
};

export {};
