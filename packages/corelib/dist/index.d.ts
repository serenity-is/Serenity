import { Column, EventEmitter, FormatterContext, FormatterResult, Grid, GridOptions, Group, GroupItemMetadataProvider, GroupTotals, IDataView, IEventData, IGroupTotals, ItemMetadata } from '@serenity-is/sleekgrid';

export interface UserDefinition {
	/**
	 * Username of the logged user
	 */
	Username?: string;
	/**
	 * Display name of the logged user
	 */
	DisplayName?: string;
	/**
	 * This indicates that the user is a super "admin", e.g. assumed to have all the permissions available.
	 * It does not mean a member of Administrators, who might not have some of the permissions */
	IsAdmin?: boolean;
	/**
	 * A hashset of permission keys that the current user have, explicitly assigned or via its
	 * roles. Note that client side permission checks should only be used for UI enable/disable etc.
	 * You should not rely on client side permission checks and always re-check permissions server side.
	 */
	Permissions?: {
		[key: string]: boolean;
	};
}
/**
 * Contains permission related functions.
 *
 * ## Note
 * We use a namespace here both for compatibility and for allowing users to override
 * these functions easily in ES modules environment, which is normally hard to do.
 */
export declare namespace Authorization {
	/**
	 * Checks if the current user has the permission specified.
	 * This should only be used for UI purposes and it is strongly recommended to check permissions server side.
	 *
	 * > Please prefer the `hasPermissionAsync` variant as this may block the UI thread if the `UserData` script is not already loaded.
	 * @param permission Permission key. It may contain logical operators like A&B|C.
	 * @returns `false` for "null or undefined", true for "*", `IsLoggedIn` for "?". For other permissions,
	 * if the user has the permission or if the user has the `IsAdmin` flag (super admin) `true`, otherwise `false`.
	 */
	function hasPermission(permission: string): boolean;
	/**
	 * Checks if the current user has the permission specified.
	 * This should only be used for UI purposes and it is strongly recommended to check permissions server side.
	 *
	 * @param permission Permission key. It may contain logical operators like A&B|C.
	 * @returns `false` for "null or undefined", true for "*", `IsLoggedIn` for "?". For other permissions,
	 * if the user has the permission or if the user has the `IsAdmin` flag (super admin) `true`, otherwise `false`.
	 */
	function hasPermissionAsync(permission: string): Promise<boolean>;
	/**
	 * Checks if the hashset contains the specified permission, also handling logical "|" and "&" operators
	 * @param permissionSet Set of permissions
	 * @param permission Permission key or a permission expression containing & | operators
	 * @returns true if set contains permission
	 */
	function isPermissionInSet(permissionSet: {
		[key: string]: boolean;
	}, permission: string): boolean;
	/**
	 * Throws an error if the current user does not have the specified permission.
	 * Prefer `await validatePermissionAsync()` as this one might block the UI if the `UserData`
	 * is not already loaded.
	 * @param permission Permission key. It may contain logical operators like A&B|C.
	 */
	function validatePermission(permission: string): void;
	/**
	* Throws an error if the current user does not have the specified permission.
	* @param permission Permission key. It may contain logical operators like A&B|C.
	* @example
	* await Authorization.validatePermissionAsync("A&B|C");
	*/
	function validatePermissionAsync(permission: string): Promise<void>;
}
export declare namespace Authorization {
	/**
	 * Checks if the current user is logged in. Prefer `isLoggedInAsync` as this one might block the UI if the `UserData`
	 * is not already loaded.
	 * @returns `true` if the user is logged in, `false` otherwise.
	 * @example
	 * if (Authorization.isLoggedIn) {
	 *     // do something
	 * }
	 */
	let isLoggedIn: boolean;
	/**
	 * Checks if the current user is logged in.
	 * @returns `true` if the user is logged in, `false` otherwise.
	 * @example
	 * if (await Authorization.isLoggedInAsync) {
	 *     // do something
	 * }
	 */
	let isLoggedInAsync: Promise<boolean>;
	/** Returns the username for currently logged user. Prefer `usernameAsync` as this one might block the UI if the `UserData`
	 * is not already loaded.
	 * @returns Username for currently logged user.
	 * @example
	 * if (Authorization.username) {
	 *     // do something
	 * }
	 */
	let username: string;
	/** Returns the username for currently logged user.
	 * @returns Username for currently logged user.
	 * @example
	 * if (await Authorization.usernameAsync) {
	 *     // do something
	 * }
	 */
	let usernameAsync: Promise<string>;
	/** Returns the user data for currently logged user. Prefer `userDefinitionAsync` as this one might block the UI if the `UserData`
	 * is not already loaded.
	 * @returns User data for currently logged user.
	 * @example
	 * if (Authorization.userDefinition.IsAdmin) {
	 *     // do something
	 * }
	 */
	let userDefinition: UserDefinition;
	/** Returns the user data for currently logged user.
	 * @returns User data for currently logged user.
	 * @example
	 * if ((await Authorization.userDefinitionAsync).IsAdmin) {
	 *     // do something
	 * }
	 */
	let userDefinitionAsync: Promise<UserDefinition>;
}
/**
 * Tries to block the page
 */
export declare function blockUI(options?: {
	zIndex?: number;
	useTimeout?: boolean;
}): void;
/**
 * Unblocks the page.
 */
export declare function blockUndo(): void;
export declare const Config: {
	/**
	 * This is the root path of your application. If your application resides under http://localhost/mysite/,
	 * your root path is "/mysite/". This variable is automatically initialized by reading from a <link> element
	 * with ID "ApplicationPath" from current page, which is usually located in your _LayoutHead.cshtml file
	 */
	applicationPath: string;
	/**
	 * Gets a default return URL for the application. This is used when a return URL is not specified
	 * @param purpose Optional purpose for the return URL, for example "login" or "logout"
	 */
	defaultReturnUrl: (purpose?: string) => string;
	/**
	 * Email validation by default only allows ASCII characters. Set this to true if you want to allow unicode.
	 */
	emailAllowOnlyAscii: boolean;
	/**
	 * This is an optional callback that is used to load types lazily when they are not found in the
	 * type registry. This is useful when a type is not available in currently loaded scripts
	 * (e.g. chunks / entry modules) but is available via some other means (e.g. a separate script file).
	 * The method may return a type or a promise that resolves to a type. If either returns null,
	 * the type is considered to be not found.
	 * The method is called with the type key and an optional kind parameter, which is used to distinguish
	 * between different kinds of types (e.g. "editor" or "dialog" or "enum") usually based on calling type registry.
	 */
	lazyTypeLoader: (typeKey: string, kind: "dialog" | "editor" | "enum" | "formatter" | "filtering" | string) => any | Promise<any>;
	/**
	 * This is the list of root namespaces that may be searched for types. For example, if you specify an editor type
	 * of "MyEditor", first a class with name "MyEditor" will be searched, if not found, search will be followed by
	 * "Serenity.MyEditor" and "MyApp.MyEditor" if you added "MyApp" to the list of root namespaces.
	 *
	 * You should usually add your application root namespace to this list in ScriptInit(ialization).ts file.
	 */
	rootNamespaces: string[];
	/**
	 * This is an optional method for handling when user is not logged in. If a users session is expired
	 * and when a NotAuthorized response is received from a service call, Serenity will call this handler, so
	 * you may intercept it and notify user about this situation and ask if she wants to login again...
	 */
	notLoggedInHandler: Function;
};
export declare function resetApplicationPath(): void;
/**
 * CriteriaBuilder is a class that allows to build unary or binary criteria with completion support.
 */
export declare class CriteriaBuilder extends Array {
	/**
	 * Creates a between criteria.
	 * @param fromInclusive from value
	 * @param toInclusive to value
	 */
	bw(fromInclusive: any, toInclusive: any): Array<any>;
	/**
	 * Creates a contains criteria
	 * @param value contains value
	 */
	contains(value: string): Array<any>;
	/**
	 * Creates a endsWith criteria
	 * @param value endsWith value
	 */
	endsWith(value: string): Array<any>;
	/**
	 * Creates an equal (=) criteria
	 * @param value equal value
	 */
	eq(value: any): Array<any>;
	/**
	 * Creates a greater than criteria
	 * @param value greater than value
	 */
	gt(value: any): Array<any>;
	/**
	 * Creates a greater than or equal criteria
	 * @param value greater than or equal value
	 */
	ge(value: any): Array<any>;
	/**
	 * Creates a in criteria
	 * @param values in values
	 */
	in(values: any[]): Array<any>;
	/**
	 * Creates a IS NULL criteria
	 */
	isNull(): Array<any>;
	/**
	 * Creates a IS NOT NULL criteria
	 */
	isNotNull(): Array<any>;
	/**
	 * Creates a less than or equal to criteria
	 * @param value less than or equal to value
	 */
	le(value: any): Array<any>;
	/**
	 * Creates a less than criteria
	 * @param value less than value
	 */
	lt(value: any): Array<any>;
	/**
	 * Creates a not equal criteria
	 * @param value not equal value
	 */
	ne(value: any): Array<any>;
	/**
	 * Creates a LIKE criteria
	 * @param value like value
	 */
	like(value: any): Array<any>;
	/**
	 * Creates a STARTS WITH criteria
	 * @param value startsWith value
	 */
	startsWith(value: string): Array<any>;
	/**
	 * Creates a NOT IN criteria
	 * @param values array of NOT IN values
	 */
	notIn(values: any[]): Array<any>;
	/**
	 * Creates a NOT LIKE criteria
	 * @param value not like value
	 */
	notLike(value: any): Array<any>;
}
/**
 * Parses a criteria expression to Serenity Criteria array format.
 * The string may optionally contain parameters like `A >= @p1 and B < @p2`.
 * @param expression The criteria expression.
 * @param params The dictionary containing parameter values like { p1: 10, p2: 20 }.
 * @example
 * `parseCriteria('A >= @p1 and B < @p2', { p1: 5, p2: 4 }) // [[[a], '>=' 5], 'and', [[b], '<', 4]]`
 */
export declare function parseCriteria(expression: string, params?: any): any[];
/**
 * Parses a criteria expression to Serenity Criteria array format.
 * The expression may contain parameter placeholders like `A >= ${p1}`
 * where p1 is a variable in the scope.
 * @param strings The string fragments.
 * @param values The tagged template arguments.
 * @example
 * let a = 5, b = 4;
 * parseCriteria`A >= ${a} and B < ${b}` // [[[a], '>=' 5], 'and', [[b], '<', 4]]
 */
export declare function parseCriteria(strings: TemplateStringsArray, ...values: any[]): any[];
/**
 * Enumeration of Criteria operator keys.
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
 * Creates a new criteria builder containg the passed field name.
 * @param field The field name.
 */
export declare function Criteria(field: string): CriteriaBuilder;
export declare namespace Criteria {
	var and: (c1: any[], c2: any[], ...rest: any[][]) => any[];
	var Operator: typeof CriteriaOperator;
	var isEmpty: (c: any[]) => boolean;
	var join: (c1: any[], op: string, c2: any[]) => any[];
	var not: (c: any[]) => (string | any[])[];
	var or: (c1: any[], c2: any[], ...rest: any[][]) => any[];
	var paren: (c: any[]) => any[];
	var parse: typeof parseCriteria;
}
export interface DebouncedFunction<T extends (...args: any[]) => any> {
	/**
	 * Call the original function, but applying the debounce rules.
	 *
	 * If the debounced function can be run immediately, this calls it and returns its return
	 * value.
	 *
	 * Otherwise, it returns the return value of the last invocation, or undefined if the debounced
	 * function was not invoked yet.
	 */
	(...args: Parameters<T>): ReturnType<T> | undefined;
	/**
	 * Throw away any pending invocation of the debounced function.
	 */
	clear(): void;
	/**
	 * If there is a pending invocation of the debounced function, invoke it immediately and return
	 * its return value.
	 *
	 * Otherwise, return the value from the last invocation, or undefined if the debounced function
	 * was never invoked.
	 */
	flush(): ReturnType<T> | undefined;
}
/**
 * Returns a function, that, as long as it continues to be invoked, will not
 * be triggered. The function also has a property 'clear' that can be used
 * to clear the timer to prevent previously scheduled executions, and flush method
 * to invoke scheduled executions now if any.
 * @param wait The function will be called after it stops being called for
 * N milliseconds.
 * @param immediate If passed, trigger the function on the leading edge, instead of the trailing.
 *
 */
export declare function debounce<T extends (...args: any) => any>(func: T, wait?: number, immediate?: boolean): DebouncedFunction<T>;
/**
 * Represents the utility color options for icons corresponding to Bootstrap contextual colors like primary, secondary, success etc.
 */
export type UtilityColor = "primary" | "secondary" | "success" | "danger" | "warning" | "info" | "light" | "dark" | "muted" | "white";
/**
 * Represents the type of text color.
 * It can be one of the predefined UtilityColor values or one of the following CSS color names:
 * "aqua", "blue", "fuschia", "gray", "green", "light-blue", "lime", "maroon", "navy", "olive", "orange", "purple", "red", "teal", "yellow".
 */
export type TextColor = UtilityColor | "aqua" | "blue" | "fuschia" | "gray" | "green" | "light-blue" | "lime" | "maroon" | "navy" | "olive" | "orange" | "purple" | "red" | "teal" | "yellow";
/**
 * Returns the CSS class name for the background color based on the provided UtilityColor.
 * @param color - The UtilityColor to generate the CSS class name for.
 * @returns The CSS class name for the background color.
 */
export declare function bgColor(color: UtilityColor): string;
/**
 * Returns the CSS class for the specified text color.
 * @param color - The text color.
 * @returns The CSS class for the specified text color.
 */
export declare function textColor(color: TextColor): string;
/**
 * Returns the CSS class for a Font Awesome icon.
 * @param key - The key of the Font Awesome icon.
 * @param color - The optional color of the icon.
 * @returns The CSS class for the icon.
 */
export declare function faIcon(key: faIconKey, color?: TextColor): string;
/**
 * Generates a fully qualified class name for a Font Awesome brand icon.
 * @param key - The key of the Font Awesome brand icon.
 * @param color - The optional color of the icon.
 * @returns The fully qualified class name for the icon.
 */
export declare function fabIcon(key: fabIconKey, color?: TextColor): string;
/**
 * Represents a known icon class.
 * The icon class can be either a Font Awesome icon (`fa fa-${faIconKey}`)
 * or a Font Awesome Brands icon (`fab fa-${fabIconKey}`).
 */
export type KnownIconClass = `fa fa-${faIconKey}` | `fab fa-${fabIconKey}`;
/**
 * Represents a type that can be either a known icon class or a string.
 */
export type AnyIconClass = KnownIconClass | (string & {});
/**
 * Represents the type for an icon class name.
 * It can be either a single icon class or an array of icon classes.
 */
export type IconClassName = AnyIconClass | (AnyIconClass[]);
/**
 * Returns the CSS class name for the given icon.
 * @param icon The icon class name or an array of class names.
 * @returns The CSS class name for the icon.
 */
export declare function iconClassName(icon: IconClassName): string;
export type faIconKey = "ad" | "address-book" | "address-card" | "adjust" | "air-freshener" | "align-center" | "align-justify" | "align-left" | "align-right" | "allergies" | "ambulance" | "american-sign-language-interpreting" | "anchor" | "angle-double-down" | "angle-double-left" | "angle-double-right" | "angle-double-up" | "angle-down" | "angle-left" | "angle-right" | "angle-up" | "angry" | "ankh" | "apple-alt" | "archive" | "archway" | "arrow-alt-circle-down" | "arrow-alt-circle-left" | "arrow-alt-circle-right" | "arrow-alt-circle-up" | "arrow-circle-down" | "arrow-circle-left" | "arrow-circle-right" | "arrow-circle-up" | "arrow-down" | "arrow-left" | "arrow-right" | "arrow-up" | "arrows-alt" | "arrows-alt-h" | "arrows-alt-v" | "assistive-listening-systems" | "asterisk" | "at" | "atlas" | "atom" | "audio-description" | "award" | "baby" | "baby-carriage" | "backspace" | "backward" | "bacon" | "balance-scale" | "balance-scale-left" | "balance-scale-right" | "ban" | "band-aid" | "barcode" | "bars" | "baseball-ball" | "basketball-ball" | "bath" | "battery-empty" | "battery-full" | "battery-half" | "battery-quarter" | "battery-three-quarters" | "bed" | "beer" | "bell" | "bell-o" | "bell-slash" | "bezier-curve" | "bible" | "bicycle" | "biking" | "binoculars" | "biohazard" | "birthday-cake" | "blender" | "blender-phone" | "blind" | "blog" | "bold" | "bolt" | "bomb" | "bone" | "bong" | "book" | "book-dead" | "book-medical" | "book-open" | "book-reader" | "bookmark" | "border-all" | "border-none" | "border-style" | "bowling-ball" | "box" | "box-open" | "boxes" | "braille" | "brain" | "bread-slice" | "briefcase" | "briefcase-medical" | "broadcast-tower" | "broom" | "brush" | "bug" | "building" | "bullhorn" | "bullseye" | "burn" | "bus" | "bus-alt" | "business-time" | "calculator" | "calendar" | "calendar-alt" | "calendar-check" | "calendar-day" | "calendar-minus" | "calendar-plus" | "calendar-times" | "calendar-week" | "camera" | "camera-retro" | "campground" | "candy-cane" | "cannabis" | "capsules" | "car" | "car-alt" | "car-battery" | "car-crash" | "car-side" | "caret-down" | "caret-left" | "caret-right" | "caret-square-down" | "caret-square-left" | "caret-square-right" | "caret-square-up" | "caret-up" | "carrot" | "cart-arrow-down" | "cart-plus" | "cash-register" | "cat" | "certificate" | "chair" | "chalkboard" | "chalkboard-teacher" | "charging-station" | "chart-area" | "chart-bar" | "chart-line" | "chart-pie" | "check" | "check-circle" | "check-double" | "check-square" | "cheese" | "chess" | "chess-bishop" | "chess-board" | "chess-king" | "chess-knight" | "chess-pawn" | "chess-queen" | "chess-rook" | "chevron-circle-down" | "chevron-circle-left" | "chevron-circle-right" | "chevron-circle-up" | "chevron-down" | "chevron-left" | "chevron-right" | "chevron-up" | "child" | "church" | "circle" | "circle-notch" | "city" | "clinic-medical" | "clipboard" | "clipboard-check" | "clipboard-list" | "clock" | "clock-o" | "clone" | "closed-captioning" | "cloud" | "cloud-download-alt" | "cloud-meatball" | "cloud-moon" | "cloud-moon-rain" | "cloud-rain" | "cloud-showers-heavy" | "cloud-sun" | "cloud-sun-rain" | "cloud-upload-alt" | "cocktail" | "code" | "code-branch" | "coffee" | "cog" | "cogs" | "coins" | "columns" | "comment" | "comment-alt" | "comment-dollar" | "comment-dots" | "comment-medical" | "comment-slash" | "comments" | "comments-dollar" | "compact-disc" | "compass" | "compress" | "compress-arrows-alt" | "concierge-bell" | "cookie" | "cookie-bite" | "copy" | "copyright" | "couch" | "credit-card" | "crop" | "crop-alt" | "cross" | "crosshairs" | "crow" | "crown" | "crutch" | "cube" | "cubes" | "cut" | "database" | "deaf" | "democrat" | "desktop" | "dharmachakra" | "diagnoses" | "dice" | "dice-d20" | "dice-d6" | "dice-five" | "dice-four" | "dice-one" | "dice-six" | "dice-three" | "dice-two" | "digital-tachograph" | "directions" | "divide" | "dizzy" | "dna" | "dog" | "dollar-sign" | "dolly" | "dolly-flatbed" | "donate" | "door-closed" | "door-open" | "dot-circle" | "dove" | "download" | "drafting-compass" | "dragon" | "draw-polygon" | "drum" | "drum-steelpan" | "drumstick-bite" | "dumbbell" | "dumpster" | "dumpster-fire" | "dungeon" | "edit" | "egg" | "eject" | "ellipsis-h" | "ellipsis-v" | "envelope" | "envelope-o" | "envelope-open" | "envelope-open-text" | "envelope-square" | "equals" | "eraser" | "ethernet" | "euro-sign" | "exchange-alt" | "exclamation" | "exclamation-circle" | "exclamation-triangle" | "expand" | "expand-arrows-alt" | "external-link-alt" | "external-link-square-alt" | "eye" | "eye-dropper" | "eye-slash" | "fan" | "fast-backward" | "fast-forward" | "fax" | "feather" | "feather-alt" | "female" | "fighter-jet" | "file" | "file-alt" | "file-archive" | "file-audio" | "file-code" | "file-contract" | "file-csv" | "file-download" | "file-excel" | "file-excel-o" | "file-export" | "file-image" | "file-import" | "file-invoice" | "file-invoice-dollar" | "file-medical" | "file-medical-alt" | "file-pdf" | "file-pdf-o" | "file-powerpoint" | "file-prescription" | "file-signature" | "file-upload" | "file-text" | "file-text-o" | "file-video" | "file-word" | "fill" | "fill-drip" | "film" | "filter" | "fingerprint" | "fire" | "floppy-o" | "fire-alt" | "fire-extinguisher" | "first-aid" | "fish" | "fist-raised" | "flag" | "flag-checkered" | "flag-usa" | "flask" | "flushed" | "folder" | "folder-minus" | "folder-open" | "folder-open-o" | "folder-plus" | "font" | "football-ball" | "forward" | "frog" | "frown" | "frown-open" | "funnel-dollar" | "futbol" | "gamepad" | "gas-pump" | "gavel" | "gem" | "genderless" | "ghost" | "gift" | "gifts" | "glass-cheers" | "glass-martini" | "glass-martini-alt" | "glass-whiskey" | "glasses" | "globe" | "globe-africa" | "globe-americas" | "globe-asia" | "globe-europe" | "golf-ball" | "gopuram" | "graduation-cap" | "greater-than" | "greater-than-equal" | "grimace" | "grin" | "grin-alt" | "grin-beam" | "grin-beam-sweat" | "grin-hearts" | "grin-squint" | "grin-squint-tears" | "grin-stars" | "grin-tears" | "grin-tongue" | "grin-tongue-squint" | "grin-tongue-wink" | "grin-wink" | "grip-horizontal" | "grip-lines" | "grip-lines-vertical" | "grip-vertical" | "guitar" | "h-square" | "hamburger" | "hammer" | "hamsa" | "hand-holding" | "hand-holding-heart" | "hand-holding-usd" | "hand-lizard" | "hand-middle-finger" | "hand-paper" | "hand-peace" | "hand-point-down" | "hand-point-left" | "hand-point-right" | "hand-point-up" | "hand-pointer" | "hand-rock" | "hand-scissors" | "hand-spock" | "hands" | "hands-helping" | "handshake" | "hanukiah" | "hard-hat" | "hashtag" | "hat-cowboy" | "hat-cowboy-side" | "hat-wizard" | "haykal" | "hdd" | "heading" | "headphones" | "headphones-alt" | "headset" | "heart" | "heart-broken" | "heartbeat" | "helicopter" | "highlighter" | "hiking" | "hippo" | "history" | "hockey-puck" | "holly-berry" | "home" | "horse" | "horse-head" | "hospital" | "hospital-alt" | "hospital-symbol" | "hot-tub" | "hotdog" | "hotel" | "hourglass" | "hourglass-end" | "hourglass-half" | "hourglass-start" | "house-damage" | "hryvnia" | "i-cursor" | "ice-cream" | "icicles" | "icons" | "id-badge" | "id-card" | "id-card-alt" | "igloo" | "image" | "images" | "inbox" | "indent" | "industry" | "infinity" | "info" | "info-circle" | "italic" | "jedi" | "joint" | "journal-whills" | "kaaba" | "key" | "keyboard" | "khanda" | "kiss" | "kiss-beam" | "kiss-wink-heart" | "kiwi-bird" | "landmark" | "language" | "laptop" | "laptop-code" | "laptop-medical" | "laugh" | "laugh-beam" | "laugh-squint" | "laugh-wink" | "layer-group" | "leaf" | "lemon" | "less-than" | "less-than-equal" | "level-down-alt" | "level-up-alt" | "life-ring" | "lightbulb" | "link" | "lira-sign" | "list" | "list-alt" | "list-ol" | "list-ul" | "location-arrow" | "lock" | "lock-open" | "long-arrow-alt-down" | "long-arrow-alt-left" | "long-arrow-alt-right" | "long-arrow-alt-up" | "low-vision" | "luggage-cart" | "magic" | "magnet" | "mail-bulk" | "mail-forward" | "mail-reply" | "male" | "map" | "map-marked" | "map-marked-alt" | "map-marker" | "map-marker-alt" | "map-pin" | "map-signs" | "marker" | "mars" | "mars-double" | "mars-stroke" | "mars-stroke-h" | "mars-stroke-v" | "mask" | "medal" | "medkit" | "meh" | "meh-blank" | "meh-rolling-eyes" | "memory" | "menorah" | "mercury" | "meteor" | "microchip" | "microphone" | "microphone-alt" | "microphone-alt-slash" | "microphone-slash" | "microscope" | "minus" | "minus-circle" | "minus-square" | "mitten" | "mobile" | "mobile-alt" | "money-bill" | "money-bill-alt" | "money-bill-wave" | "money-bill-wave-alt" | "money-check" | "money-check-alt" | "monument" | "moon" | "mortar-pestle" | "mosque" | "motorcycle" | "mountain" | "mouse" | "mouse-pointer" | "mug-hot" | "music" | "network-wired" | "neuter" | "newspaper" | "not-equal" | "notes-medical" | "object-group" | "object-ungroup" | "oil-can" | "om" | "otter" | "outdent" | "pager" | "paint-brush" | "paint-roller" | "palette" | "pallet" | "paper-plane" | "paperclip" | "parachute-box" | "paragraph" | "parking" | "passport" | "pastafarianism" | "paste" | "pause" | "pause-circle" | "paw" | "peace" | "pen" | "pen-alt" | "pen-fancy" | "pen-nib" | "pen-square" | "pencil-alt" | "pencil-ruler" | "pencil-square-o" | "people-carry" | "pepper-hot" | "percent" | "percentage" | "person-booth" | "phone" | "phone-alt" | "phone-slash" | "phone-square" | "phone-square-alt" | "phone-volume" | "photo-video" | "piggy-bank" | "pills" | "pizza-slice" | "place-of-worship" | "plane" | "plane-arrival" | "plane-departure" | "play" | "play-circle" | "plug" | "plus" | "plus-circle" | "plus-square" | "podcast" | "poll" | "poll-h" | "poo" | "poo-storm" | "poop" | "portrait" | "pound-sign" | "power-off" | "pray" | "praying-hands" | "prescription" | "prescription-bottle" | "prescription-bottle-alt" | "print" | "procedures" | "project-diagram" | "puzzle-piece" | "qrcode" | "question" | "question-circle" | "quidditch" | "quote-left" | "quote-right" | "quran" | "radiation" | "radiation-alt" | "rainbow" | "random" | "receipt" | "record-vinyl" | "recycle" | "redo" | "refresh" | "redo-alt" | "registered" | "remove-format" | "reply" | "reply-all" | "republican" | "restroom" | "retweet" | "ribbon" | "ring" | "road" | "robot" | "rocket" | "route" | "rss" | "rss-square" | "ruble-sign" | "ruler" | "ruler-combined" | "ruler-horizontal" | "ruler-vertical" | "running" | "rupee-sign" | "sad-cry" | "sad-tear" | "satellite" | "satellite-dish" | "save" | "school" | "screwdriver" | "scroll" | "sd-card" | "search" | "search-dollar" | "search-location" | "search-minus" | "search-plus" | "seedling" | "server" | "shapes" | "share" | "share-alt" | "share-alt-square" | "share-square" | "shekel-sign" | "shield-alt" | "ship" | "shipping-fast" | "shoe-prints" | "shopping-bag" | "shopping-basket" | "shopping-cart" | "shower" | "shuttle-van" | "sign" | "sign-in-alt" | "sign-language" | "sign-out" | "sign-out-alt" | "signal" | "signature" | "sim-card" | "sitemap" | "skating" | "skiing" | "skiing-nordic" | "skull" | "skull-crossbones" | "slash" | "sleigh" | "sliders-h" | "smile" | "smile-beam" | "smile-wink" | "smog" | "smoking" | "smoking-ban" | "sms" | "snowboarding" | "snowflake" | "snowman" | "snowplow" | "socks" | "solar-panel" | "sort" | "sort-alpha-down" | "sort-alpha-down-alt" | "sort-alpha-up" | "sort-alpha-up-alt" | "sort-amount-down" | "sort-amount-down-alt" | "sort-amount-up" | "sort-amount-up-alt" | "sort-down" | "sort-numeric-down" | "sort-numeric-down-alt" | "sort-numeric-up" | "sort-numeric-up-alt" | "sort-up" | "spa" | "space-shuttle" | "spell-check" | "spider" | "spinner" | "splotch" | "spray-can" | "square" | "square-full" | "square-root-alt" | "stamp" | "star" | "star-and-crescent" | "star-half" | "star-half-alt" | "star-o" | "star-of-david" | "star-of-life" | "step-backward" | "step-forward" | "stethoscope" | "sticky-note" | "stop" | "stop-circle" | "stopwatch" | "store" | "store-alt" | "stream" | "street-view" | "strikethrough" | "stroopwafel" | "subscript" | "subway" | "suitcase" | "suitcase-rolling" | "sun" | "superscript" | "surprise" | "swatchbook" | "swimmer" | "swimming-pool" | "synagogue" | "sync" | "sync-alt" | "syringe" | "table" | "table-tennis" | "tablet" | "tablet-alt" | "tablets" | "tachometer-alt" | "tag" | "tags" | "tape" | "tasks" | "taxi" | "teeth" | "teeth-open" | "temperature-high" | "temperature-low" | "tenge" | "terminal" | "text-height" | "text-width" | "th" | "th-large" | "th-list" | "theater-masks" | "thermometer" | "thermometer-empty" | "thermometer-full" | "thermometer-half" | "thermometer-quarter" | "thermometer-three-quarters" | "thumbs-down" | "thumbs-up" | "thumbtack" | "ticket-alt" | "times" | "times-circle" | "tint" | "tint-slash" | "tired" | "toggle-off" | "toggle-on" | "toilet" | "toilet-paper" | "toolbox" | "tools" | "tooth" | "torah" | "torii-gate" | "tractor" | "trademark" | "traffic-light" | "train" | "tram" | "transgender" | "transgender-alt" | "trash" | "trash-alt" | "trash-o" | "trash-restore" | "trash-restore-alt" | "tree" | "trophy" | "truck" | "truck-loading" | "truck-monster" | "truck-moving" | "truck-pickup" | "tshirt" | "tty" | "tv" | "umbrella" | "umbrella-beach" | "underline" | "undo" | "undo-alt" | "universal-access" | "university" | "unlink" | "unlock" | "unlock-alt" | "upload" | "user" | "user-alt" | "user-alt-slash" | "user-astronaut" | "user-check" | "user-circle" | "user-clock" | "user-cog" | "user-edit" | "user-friends" | "user-graduate" | "user-injured" | "user-lock" | "user-md" | "user-minus" | "user-ninja" | "user-nurse" | "user-plus" | "user-secret" | "user-shield" | "user-slash" | "user-tag" | "user-tie" | "user-times" | "users" | "users-cog" | "utensil-spoon" | "utensils" | "vector-square" | "venus" | "venus-double" | "venus-mars" | "vial" | "vials" | "video" | "video-slash" | "vihara" | "voicemail" | "volleyball-ball" | "volume-down" | "volume-mute" | "volume-off" | "volume-up" | "vote-yea" | "vr-cardboard" | "walking" | "wallet" | "warehouse" | "water" | "wave-square" | "weight" | "weight-hanging" | "wheelchair" | "wifi" | "wind" | "window-close" | "window-maximize" | "window-minimize" | "window-restore" | "wine-bottle" | "wine-glass" | "wine-glass-alt" | "won-sign" | "wrench" | "x-ray" | "yen-sign" | "yin-yang";
export type fabIconKey = "500px" | "accessible-icon" | "accusoft" | "acquisitions-incorporated" | "adn" | "adobe" | "adversal" | "affiliatetheme" | "airbnb" | "algolia" | "alipay" | "amazon" | "amazon-pay" | "amilia" | "android" | "angellist" | "angrycreative" | "angular" | "app-store" | "app-store-ios" | "apper" | "apple" | "apple-pay" | "artstation" | "asymmetrik" | "atlassian" | "audible" | "autoprefixer" | "avianex" | "aviato" | "aws" | "bandcamp" | "battle-net" | "behance" | "behance-square" | "bimobject" | "bitbucket" | "bitcoin" | "bity" | "black-tie" | "blackberry" | "blogger" | "blogger-b" | "bluetooth" | "bluetooth-b" | "bootstrap" | "btc" | "buffer" | "buromobelexperte" | "buy-n-large" | "buysellads" | "canadian-maple-leaf" | "cc-amazon-pay" | "cc-amex" | "cc-apple-pay" | "cc-diners-club" | "cc-discover" | "cc-jcb" | "cc-mastercard" | "cc-paypal" | "cc-stripe" | "cc-visa" | "centercode" | "centos" | "chrome" | "chromecast" | "cloudscale" | "cloudsmith" | "cloudversify" | "codepen" | "codiepie" | "confluence" | "connectdevelop" | "contao" | "cotton-bureau" | "cpanel" | "creative-commons" | "creative-commons-by" | "creative-commons-nc" | "creative-commons-nc-eu" | "creative-commons-nc-jp" | "creative-commons-nd" | "creative-commons-pd" | "creative-commons-pd-alt" | "creative-commons-remix" | "creative-commons-sa" | "creative-commons-sampling" | "creative-commons-sampling-plus" | "creative-commons-share" | "creative-commons-zero" | "critical-role" | "css3" | "css3-alt" | "cuttlefish" | "d-and-d" | "d-and-d-beyond" | "dashcube" | "delicious" | "deploydog" | "deskpro" | "dev" | "deviantart" | "dhl" | "diaspora" | "digg" | "digital-ocean" | "discord" | "discourse" | "dochub" | "docker" | "draft2digital" | "dribbble" | "dribbble-square" | "dropbox" | "drupal" | "dyalog" | "earlybirds" | "ebay" | "edge" | "elementor" | "ello" | "ember" | "empire" | "envira" | "erlang" | "ethereum" | "etsy" | "evernote" | "expeditedssl" | "facebook" | "facebook-f" | "facebook-messenger" | "facebook-square" | "fantasy-flight-games" | "fedex" | "fedora" | "figma" | "firefox" | "first-order" | "first-order-alt" | "firstdraft" | "flickr" | "flipboard" | "fly" | "font-awesome" | "font-awesome-alt" | "font-awesome-flag" | "fonticons" | "fonticons-fi" | "fort-awesome" | "fort-awesome-alt" | "forumbee" | "foursquare" | "free-code-camp" | "freebsd" | "fulcrum" | "galactic-republic" | "galactic-senate" | "get-pocket" | "gg" | "gg-circle" | "git" | "git-alt" | "git-square" | "github" | "github-alt" | "github-square" | "gitkraken" | "gitlab" | "gitter" | "glide" | "glide-g" | "gofore" | "goodreads" | "goodreads-g" | "google" | "google-drive" | "google-play" | "google-plus" | "google-plus-g" | "google-plus-square" | "google-wallet" | "gratipay" | "grav" | "gripfire" | "grunt" | "gulp" | "hacker-news" | "hacker-news-square" | "hackerrank" | "hips" | "hire-a-helper" | "hooli" | "hornbill" | "hotjar" | "houzz" | "html5" | "hubspot" | "imdb" | "instagram" | "intercom" | "internet-explorer" | "invision" | "ioxhost" | "itch-io" | "itunes" | "itunes-note" | "java" | "jedi-order" | "jenkins" | "jira" | "joget" | "joomla" | "js" | "js-square" | "jsfiddle" | "kaggle" | "keybase" | "keycdn" | "kickstarter" | "kickstarter-k" | "korvue" | "laravel" | "lastfm" | "lastfm-square" | "leanpub" | "less" | "line" | "linkedin" | "linkedin-in" | "linode" | "linux" | "lyft" | "magento" | "mailchimp" | "mandalorian" | "markdown" | "mastodon" | "maxcdn" | "mdb" | "medapps" | "medium" | "medium-m" | "medrt" | "meetup" | "megaport" | "mendeley" | "microsoft" | "mix" | "mixcloud" | "mizuni" | "modx" | "monero" | "napster" | "neos" | "nimblr" | "node" | "node-js" | "npm" | "ns8" | "nutritionix" | "odnoklassniki" | "odnoklassniki-square" | "old-republic" | "opencart" | "openid" | "opera" | "optin-monster" | "orcid" | "osi" | "page4" | "pagelines" | "palfed" | "patreon" | "paypal" | "penny-arcade" | "periscope" | "phabricator" | "phoenix-framework" | "phoenix-squadron" | "php" | "pied-piper" | "pied-piper-alt" | "pied-piper-hat" | "pied-piper-pp" | "pinterest" | "pinterest-p" | "pinterest-square" | "playstation" | "product-hunt" | "pushed" | "python" | "qq" | "quinscape" | "quora" | "r-project" | "raspberry-pi" | "ravelry" | "react" | "reacteurope" | "readme" | "rebel" | "red-river" | "reddit" | "reddit-alien" | "reddit-square" | "redhat" | "renren" | "replyd" | "researchgate" | "resolving" | "rev" | "rocketchat" | "rockrms" | "safari" | "salesforce" | "sass" | "schlix" | "scribd" | "searchengin" | "sellcast" | "sellsy" | "servicestack" | "shirtsinbulk" | "shopware" | "simplybuilt" | "sistrix" | "sith" | "sketch" | "skyatlas" | "skype" | "slack" | "slack-hash" | "slideshare" | "snapchat" | "snapchat-ghost" | "snapchat-square" | "soundcloud" | "sourcetree" | "speakap" | "speaker-deck" | "spotify" | "squarespace" | "stack-exchange" | "stack-overflow" | "stackpath" | "staylinked" | "steam" | "steam-square" | "steam-symbol" | "sticker-mule" | "strava" | "stripe" | "stripe-s" | "studiovinari" | "stumbleupon" | "stumbleupon-circle" | "superpowers" | "supple" | "suse" | "swift" | "symfony" | "teamspeak" | "telegram" | "telegram-plane" | "tencent-weibo" | "the-red-yeti" | "themeco" | "themeisle" | "think-peaks" | "trade-federation" | "trello" | "tripadvisor" | "tumblr" | "tumblr-square" | "twitch" | "twitter" | "twitter-square" | "typo3" | "uber" | "ubuntu" | "uikit" | "umbraco" | "uniregistry" | "untappd" | "ups" | "usb" | "usps" | "ussunnah" | "vaadin" | "viacoin" | "viadeo" | "viadeo-square" | "viber" | "vimeo" | "vimeo-square" | "vimeo-v" | "vine" | "vk" | "vnv" | "vuejs" | "waze" | "weebly" | "weibo" | "weixin" | "whatsapp" | "whatsapp-square" | "whmcs" | "wikipedia-w" | "windows" | "wix" | "wizards-of-the-coast" | "wolf-pack-battalion" | "wordpress" | "wordpress-simple" | "wpbeginner" | "wpexplorer" | "wpforms" | "wpressr" | "xbox" | "xing" | "xing-square" | "y-combinator" | "yahoo" | "yammer" | "yandex" | "yandex-international" | "yarn" | "yelp" | "yoast" | "youtube" | "youtube-square" | "zhihu";
/**
 * Options for a message dialog button
 */
export interface DialogButton {
	/** Button text */
	text?: string;
	/** Button hint */
	hint?: string;
	/** Button icon */
	icon?: IconClassName;
	/** Click handler */
	click?: (e: MouseEvent) => void | false | Promise<void | false>;
	/** CSS class for button */
	cssClass?: string;
	/** The code that is returned from message dialog function when this button is clicked.
	 *  If this is set, and click event will not be defaultPrevented dialog will close.
	 */
	result?: string;
}
export type DialogProviderType = "bsmodal" | "uidialog" | "panel";
/**
 * Options that apply to all dialog types
 */
export interface DialogOptions {
	/** Auto dispose dialog on close, default is true */
	autoDispose?: boolean;
	/** True to auto open dialog */
	autoOpen?: boolean;
	/** Backdrop type, static to make it modal, e.g. can't be closed by clicking outside */
	backdrop?: boolean | "static";
	/** List of buttons to show on the dialog */
	buttons?: DialogButton[];
	/** Vertically center modal */
	centered?: boolean;
	/** Show close button, default is true */
	closeButton?: boolean;
	/** Close dialog on escape key. Default is true for message dialogs. */
	closeOnEscape?: boolean;
	/** CSS class to use for all dialog types. Is added to the top ui-dialog, panel or modal element */
	dialogClass?: string;
	/** Dialog content/body element, or callback that will populate the content element */
	element?: HTMLElement | ArrayLike<HTMLElement> | ((element: HTMLElement) => void);
	/** Enable / disable animation. Default is false for message dialogs, true for other dialogs */
	fade?: boolean;
	/** Sets one of modal-fullscreen{-...-down} classes. Only used for bootstrap modals */
	fullScreen?: boolean | "sm-down" | "md-down" | "lg-down" | "xl-down" | "xxl-down";
	/** Modal option for jQuery UI dialog compatibility only. Not to be confused with Bootstrap modal. */
	modal?: boolean;
	/** Event handler that is called when dialog is opened */
	onOpen?: (e?: Event) => void;
	/** Event handler that is called when dialog is closed */
	onClose?: (result: string, e?: Event) => void;
	/** Prefer Bootstrap modals to jQuery UI dialogs when both are available */
	preferBSModal?: boolean;
	/** Prefer Panel even when Modal / jQuery UI is available */
	preferPanel?: boolean;
	/** Callback to get options specific to the dialog provider type */
	providerOptions?: (type: DialogProviderType, opt: DialogOptions) => any;
	/** Scrollable, sets content of the modal to scrollable, only for Bootstrap */
	scrollable?: boolean;
	/** Size. Default is null for (500px) message dialogs, lg for normal dialogs */
	size?: "sm" | "md" | "lg" | "xl";
	/** Dialog title */
	title?: string;
	/** Only used for jQuery UI dialogs for backwards compatibility */
	width?: number;
}
/**
 * Wrapper for different types of dialogs, including jQuery UI, Bootstrap modals, and Panels.
 */
export declare class Dialog {
	private el;
	private dialogResult;
	/**
	 * Creates a new dialog. The type of the dialog will be determined based on
	 * the availability of jQuery UI, Bootstrap, and the options provided.
	 * @param opt Optional configuration for the dialog
	 */
	constructor(opt?: DialogOptions);
	/** Default set of dialog options */
	static defaults: DialogOptions;
	/** Default set of message dialog options */
	static messageDefaults: MessageDialogOptions;
	/**
	 * Gets the dialog instance for the specified element.
	 * @param el The dialog body element (.s-Panel, .ui-dialog-content, or .modal-body) or the root element (.modal, .ui-dialog, .s-Panel)
	 * @returns The dialog instance, or null if the element is not a dialog.
	 */
	static getInstance(el: HTMLElement | ArrayLike<HTMLElement>): Dialog;
	/** The result code of the button that is clicked. Also attached to the dialog element as data-dialog-result */
	get result(): string;
	/** Closes dialog setting the result to null */
	close(): this;
	/** Closes dialog with the result set to value */
	close(result: string): this;
	/**
	 * Adds an event handler that is called when the dialog is closed. If the opt.before is true, the handler is called before the dialog is closed and
	 * the closing can be cancelled by calling preventDefault on the event object.
	 * @param handler The event handler function
	 * @param opt Options to determine whether the handler should be called before the dialog is closed, and whether the handler should be called only once.
	 * The default for oneOff is true unless opt.before is true.
	 * @returns The dialog instance
	 */
	onClose(handler: (result?: string, e?: Event) => void, opt?: {
		before?: boolean;
		oneOff?: boolean;
	}): this;
	/**
	 * Adds an event handler that is called when the dialog is closed. If the opt.before is true, the handler is called before the dialog is closed and
	 * the closing can be cancelled by calling preventDefault on the event object. Note that if the dialog is not yet initialized, the first argument must be
	 * the body element of the dialog.
	 * @param el The dialog body element (.s-Panel, .ui-dialog-content, or .modal-body)
	 * @param handler The event handler function
	 * @param opt Options to determine whether the handler should be called before the dialog is closed, and whether the handler should be called only once.
	 * The default for oneOff is true unless opt.before is true.
	 */
	static onClose(el: HTMLElement | ArrayLike<HTMLElement>, handler: (result?: string, e?: Event) => void, opt?: {
		before?: boolean;
		oneOff?: boolean;
	}): void;
	/**
	 * Adds an event handler that is called when the dialog is opened. If the second parameter is true, the handler is called before the dialog is opened and
	 * the opening can be cancelled by calling preventDefault on the event object.
	 * Note that if the dialog is not yet initialized, the first argument must be the body element of the dialog.
	 * @param handler The event handler function
	 * @param opt Options to determine whether the handler should be called before the dialog is opened, and whether the handler should be called only once.
	 * The default for oneOff is true unless opt.before is true.
	 * @returns The dialog instance
	 */
	onOpen(handler: (e?: Event) => void, opt?: {
		before?: boolean;
		oneOff?: boolean;
	}): this;
	/**
	 * Adds an event handler that is called when the dialog is opened. If the second parameter is true, the handler is called before the dialog is opened and
	 * the opening can be cancelled by calling preventDefault on the event object. Note that if the dialog is not yet initialized, the first argument must be
	 * the body element of the dialog.
	 * @param el The dialog body element (.s-Panel, .ui-dialog-content, or .modal-body)
	 * @param handler The event handler function
	 * @param opt Options to determine whether the handler should be called before the dialog is opened, and whether the handler should be called only once.
	 * The default for oneOff is true unless opt.before is true.
	 * @returns The dialog instance
	 */
	static onOpen(el: HTMLElement | ArrayLike<HTMLElement>, handler: (e?: Event) => void, opt?: {
		before?: boolean;
		oneOff?: boolean;
	}): void;
	/** Opens the dialog */
	open(): this;
	/** Gets the title text of the dialog */
	title(): string;
	/** Sets the title text of the dialog. */
	title(value: string): this;
	/** Returns the type of the dialog, or null if no dialog on the current element or if the element is null, e.g. dialog was disposed  */
	get type(): DialogProviderType;
	/** Gets the body/content element of the dialog */
	getContentNode(): HTMLElement;
	/** Gets the dialog element of the dialog */
	getDialogNode(): HTMLElement;
	/** Gets the node that receives events for the dialog. It's .ui-dialog-content, .modal, or .panel-body */
	getEventsNode(): HTMLElement;
	/** Gets the footer element of the dialog */
	getFooterNode(): HTMLElement;
	/** Gets the header element of the dialog */
	getHeaderNode(): HTMLElement;
	private onButtonClick;
	private createBSButtons;
	private createBSModal;
	private createPanel;
	private createUIDialog;
	/**
	 * Disposes the dialog, removing it from the DOM and unbinding all event handlers.
	 */
	dispose(): void;
}
/** Returns true if Bootstrap modal is available */
export declare function hasBSModal(): boolean;
/** Returns true if jQuery UI dialog is available */
export declare function hasUIDialog(): boolean;
/** Calls Bootstrap button.noConflict method if both jQuery UI and Bootstrap buttons are available in the page */
export declare function uiAndBSButtonNoConflict(): void;
/**
 * Creates a dialog button which, by default, has "Yes" as caption (localized) and "ok" as the result.
 * @param opt - Optional configuration for the dialog button.
 * @returns The dialog button with the specified configuration.
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
 * Namespace containing localizable text constants for dialogs.
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
 * Options that apply to all message dialog types
 */
export interface MessageDialogOptions extends DialogOptions {
	/** HTML encode the message, default is true */
	htmlEncode?: boolean;
	/** Wrap the message in a `<pre>` element, so that line endings are preserved, default is true */
	preWrap?: boolean;
}
/**
 * Displays an alert dialog
 * @param message The message to display
 * @param options Additional options.
 * @see AlertOptions
 * @example
 * alertDialog("An error occured!"); }
 */
export declare function alertDialog(message: string, options?: MessageDialogOptions): Partial<Dialog>;
/** Additional options for confirm dialog */
export interface ConfirmDialogOptions extends MessageDialogOptions {
	/** True to also add a cancel button */
	cancelButton?: boolean;
	/** Event handler for cancel button click */
	onCancel?: () => void;
	/** Event handler for no button click */
	onNo?: () => void;
}
/**
 * Display a confirmation dialog
 * @param message The message to display
 * @param onYes Callback for Yes button click
 * @param options Additional options.
 * @see ConfirmOptions
 * @example
 * confirmDialog("Are you sure you want to delete?", () => {
 *     // do something when yes is clicked
 * }
 */
export declare function confirmDialog(message: string, onYes: () => void, options?: ConfirmDialogOptions): Partial<Dialog>;
/**
 * Display an information dialog
 * @param message The message to display
 * @param onOk Callback for OK button click
 * @param options Additional options.
 * @see ConfirmOptions
 * @example
 * informationDialog("Operation complete", () => {
 *     // do something when OK is clicked
 * }
 */
export declare function informationDialog(message: string, onOk?: () => void, options?: MessageDialogOptions): Partial<Dialog>;
/**
 * Display a success dialog
 * @param message The message to display
 * @param onOk Callback for OK button click
 * @param options Additional options.
 * @see MessageDialogOptions
 * @example
 * successDialog("Operation complete", () => {
 *     // do something when OK is clicked
 * }
 */
export declare function successDialog(message: string, onOk?: () => void, options?: MessageDialogOptions): Partial<Dialog>;
/**
 * Display a warning dialog
 * @param message The message to display
 * @param options Additional options.
 * @see MessageDialogOptions
 * @example
 * warningDialog("Something is odd!");
 */
export declare function warningDialog(message: string, options?: MessageDialogOptions): Partial<Dialog>;
/** Options for `iframeDialog` **/
export interface IFrameDialogOptions {
	html?: string;
}
/**
 * Display a dialog that shows an HTML block in an IFRAME, which is usually returned from server callbacks
 * @param options The options
 */
export declare function iframeDialog(options: IFrameDialogOptions): Partial<Dialog>;
export declare function getjQuery(): any;
/** Returns true if Bootstrap 3 is loaded */
export declare function isBS3(): boolean;
/** Returns true if Bootstrap 5+ is loaded */
export declare function isBS5Plus(): boolean;
export interface ServiceError {
	Code?: string;
	Arguments?: string;
	Message?: string;
	Details?: string;
	ErrorId?: string;
}
export interface ServiceResponse {
	Error?: ServiceError;
}
export interface ServiceRequest {
}
export interface SaveRequest<TEntity> extends ServiceRequest {
	EntityId?: any;
	Entity?: TEntity;
	Localizations?: {
		[languageId: string]: Partial<TEntity>;
	};
}
export interface SaveResponse extends ServiceResponse {
	EntityId?: any;
}
export interface DeleteRequest extends ServiceRequest {
	EntityId?: any;
}
export interface DeleteResponse extends ServiceResponse {
}
export interface UndeleteRequest extends ServiceRequest {
	EntityId?: any;
}
export interface UndeleteResponse extends ServiceResponse {
}
export declare enum ColumnSelection {
	List = 0,
	KeyOnly = 1,
	Details = 2,
	None = 3,
	IdOnly = 4,
	Lookup = 5
}
export declare enum RetrieveColumnSelection {
	details = 0,
	keyOnly = 1,
	list = 2,
	none = 3,
	idOnly = 4,
	lookup = 5
}
export interface ListRequest extends ServiceRequest {
	Skip?: number;
	Take?: number;
	Sort?: string[];
	ContainsText?: string;
	ContainsField?: string;
	Criteria?: any[];
	EqualityFilter?: any;
	IncludeDeleted?: boolean;
	ExcludeTotalCount?: boolean;
	ColumnSelection?: ColumnSelection;
	IncludeColumns?: string[];
	ExcludeColumns?: string[];
	ExportColumns?: string[];
	DistinctFields?: string[];
	Localize?: string;
}
export interface ListResponse<TEntity> extends ServiceResponse {
	Entities?: TEntity[];
	Values?: any[];
	TotalCount?: number;
	Skip?: number;
	Take?: number;
}
export interface RetrieveRequest extends ServiceRequest {
	EntityId?: any;
	ColumnSelection?: RetrieveColumnSelection;
	IncludeColumns?: string[];
	ExcludeColumns?: string[];
}
export interface RetrieveResponse<TEntity> extends ServiceResponse {
	Entity?: TEntity;
	Localizations?: {
		[languageId: string]: Partial<TEntity>;
	};
}
export interface RequestErrorInfo {
	status?: number;
	statusText?: string;
	responseText?: string;
}
export interface ServiceOptions<TResponse extends ServiceResponse> extends RequestInit {
	allowRedirect?: boolean;
	async?: boolean;
	blockUI?: boolean;
	headers?: Record<string, string>;
	request?: any;
	service?: string;
	url?: string;
	errorMode?: "alert" | "notification" | "none";
	onCleanup?(): void;
	/** Should return true if the error is handled (e.g. notification shown). Otherwise the error may be shown twice. */
	onError?(response: TResponse, info?: RequestErrorInfo): void | boolean;
	onSuccess?(response: TResponse): void;
}
export declare namespace ErrorHandling {
	/**
	 * Shows a service error as an alert dialog / notification. If the error
	 * is null, has no message or code, it shows a generic error message.
	 */
	function showServiceError(error: ServiceError, errorInfo?: RequestErrorInfo, errorMode?: "alert" | "notification"): void;
	/**
	 * Runtime error handler that shows a runtime error as a notification
	 * by default only in development mode (@see isDevelopmentMode)
	 * This function is assigned as window.onerror handler in
	 * ScriptInit.ts for Serenity applications so that developers
	 * can notice an error without having to check the browser console.
	 */
	function runtimeErrorHandler(message: string, filename?: string, lineno?: number, colno?: number, error?: Error): void;
	/**
	 * Determines if the current environment is development mode.
	 * The runtimeErrorHandler (window.onerror) shows error notifications only
	 * when this function returns true. The default implementation considers
	 * the environment as development mode if the host is localhost, 127.0.0.1, ::1,
	 * or a domain name that ends with .local/.localhost.
	 * @returns true if the current environment is development mode, false otherwise.
	 */
	function isDevelopmentMode(): boolean;
	/**
	 * Unhandled promise rejection error handler. It's purpose is to
	 * ignore logging serviceCall / serviceFetch errors as they have built-in
	 * error handling but browser logs it in the console, while Node crashes.
	 * Include below code in script-init/errorhandling.ts to enable:
	 * window.addEventListener("unhandledrejection", ErrorHandling.unhandledRejectionHandler);
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
	 * Hides the element by setting its display property to "none".
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
	 * Shows the element by setting its display property to empty string.
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
export declare function Fluent<K extends keyof HTMLElementTagNameMap>(tag: K): Fluent<HTMLElementTagNameMap[K]>;
export declare function Fluent<TElement extends HTMLElement>(element: TElement): Fluent<TElement>;
export declare function Fluent(element: EventTarget): Fluent<HTMLElement>;
export declare namespace Fluent {
	var ready: (callback: () => void) => void;
	var byId: <TElement extends HTMLElement>(id: string) => Fluent<TElement>;
	var findAll: <TElement extends HTMLElement>(selector: string) => TElement[];
	var findEach: <TElement extends HTMLElement>(selector: string, callback: (el: Fluent<TElement>) => void) => void;
	var findFirst: <TElement extends HTMLElement>(selector: string) => Fluent<TElement>;
}
export declare namespace Fluent {
	/**
	 * Adds an event listener to the element. It is possible to use delegated events like jQuery.
	 *
	 * @param element The target element
	 * @param type The type of the event. It can include a ".namespace" similar to jQuery.
	 * @param listener The event listener to add.
	 */
	function on<K extends keyof HTMLElementEventMap>(element: EventTarget, type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): void;
	function on(element: EventTarget, type: string, listener: EventListener): void;
	function on(element: EventTarget, type: string, selector: string, delegationHandler: Function): void;
	/**
	 * Adds a one-time event listener to the element. It is possible to use delegated events like jQuery.
	 *
	 * @param element The target element
	 * @param type The type of the event. It can include a ".namespace" similar to jQuery.
	 * @param listener The event listener to add.
	 */
	function one<K extends keyof HTMLElementEventMap>(element: EventTarget, type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): void;
	function one(element: EventTarget, type: string, listener: EventListener): void;
	function one(element: EventTarget, type: string, selector: string, delegationHandler: Function): void;
	/**
	 * Removes an event listener from the element.
	 *
	 * @param element The target element
	 * @param type The type of the event. It can include a ".namespace" similar to jQuery.
	 * @param listener The event listener to remove.
	 */
	function off<K extends keyof HTMLElementEventMap>(element: EventTarget, type: K, listener?: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): void;
	function off(element: EventTarget, type: string, listener?: EventListener): void;
	function off(element: EventTarget, type: string, selector?: string, delegationHandler?: Function): void;
	/**
	 * Triggers a specified event on the element.
	 *
	 * @param element The target element
	 * @param type The type of the event to trigger.
	 * @param args Optional. An object that specifies event-specific initialization properties.
	 * @returns The event object. Use Fluent.isDefaultPrevented the check if preventDefault is called.
	 */
	function trigger(element: EventTarget, type: string, args?: any): Event & {
		isDefaultPrevented?(): boolean;
	};
	/**
	 * Adds one or more classes to the element. Any falsy value is ignored.
	 *
	 * @param element The target element
	 * @param value The class or classes to add. It can be a string, boolean, or an array of strings or booleans.
	 */
	function addClass(element: Element, value: string | boolean | (string | boolean)[]): void;
	function empty(element: Element): void;
	/**
	 * Returns true if the element is visible like. This is for compatibility with jQuery's :visible selector.
	 * @param element The target element
	 * @returns true if the element has offsetWidth or offsetHeight or any getClientRects().length > 0
	 */
	function isVisibleLike(element: Element): boolean;
	/**
	 * Removes the element from the DOM. It also removes event handlers and disposes widgets by calling "disposing" event handlers.
	 *
	 * @param element The element to remove
	 */
	function remove(element: Element): void;
	/**
	 * Removes one or more classes from the element. Any falsy value is ignored.
	 *
	 * @param element The target element
	 * @param value The class or classes to remove. It can be a string, boolean, or an array of strings or booleans.
	 */
	function removeClass(element: Element, value: string | boolean | (string | boolean)[]): void;
	/**
	 * Toggles the visibility of the element.
	 *
	 * @param element The target element
	 * @param flag Optional. A flag indicating whether to show or hide the element. If not provided, the visibility will be toggled.
	 * @returns The Fluent object itself.
	 */
	function toggle(element: Element, flag?: boolean): void;
	/**
	 * Toggles one or more classes on the element. If the class exists, it is removed; otherwise, it is added. Falsy values are ignored.
	 *
	 * @param element The target element
	 * @param value The class or classes to toggle. It can be a string, boolean, or an array of strings or booleans.
	 */
	function toggleClass(element: Element, value: string | boolean | (string | boolean)[], add?: boolean): void;
	/**
	 * Converts the given class value or an array of class values to a CSS class name. Any falsy value is ignored.
	 * @param value The class or classes. It can be a string, boolean, or an array of strings or booleans.
	 * @returns Class name string
	 */
	function toClassName(value: string | boolean | (string | boolean)[]): string;
	/**
	 * Returns true if the element is input like. E.g. one of input, textarea, select, button. This is for compatibility with jQuery's :input selector.
	 * @param element The target element
	 * @returns true if element is an input like node
	 */
	function isInputLike(element: Element): element is (HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement | HTMLButtonElement);
	/** A CSS selector for input like tags */
	const inputLikeSelector = "input,select,textarea,button";
	/**
	 * Returns true if the tag is one of input, textarea, select, button.
	 * @param tag The tag
	 * @returns true if the element has offsetWidth or offsetHeight or any getClientRects().length > 0
	 */
	function isInputTag(tag: string): boolean;
	/**
	 * Checks if the event's preventDefault method is called. This is for compatibility with jQuery which
	 * has a non-standard isDefaultPrevented method.
	 * @param event The event object
	 * @returns True if preventDefault is called.
	 */
	function isDefaultPrevented(event: {
		defaultPrevented?: boolean;
		isDefaultPrevented?: () => boolean;
	}): boolean;
	/**
	 * Tries to read a property from the event, or event.originalEvent, or event.detail. It is designed
	 * for compatibility with the way jQuery wraps original event under originalEvent property, that
	 * causes custom properties to be not available in the event object.
	 *
	 * @param event The event object
	 * @param prop The property name
	 * @returns The property value
	 */
	function eventProp(event: any, prop: string): any;
}
/**
 * Interface for number formatting, similar to .NET's NumberFormatInfo
 */
export interface NumberFormat {
	/** Decimal separator */
	decimalSeparator: string;
	/** Group separator */
	groupSeparator?: string;
	/** Number of digits after decimal separator */
	decimalDigits?: number;
	/** Positive sign */
	positiveSign?: string;
	/** Negative sign */
	negativeSign?: string;
	/** Zero symbol */
	nanSymbol?: string;
	/** Percentage symbol */
	percentSymbol?: string;
	/** Currency symbol */
	currencySymbol?: string;
}
/** Interface for date formatting, similar to .NET's DateFormatInfo */
export interface DateFormat {
	/** Date separator */
	dateSeparator?: string;
	/** Default date format string */
	dateFormat?: string;
	/** Date order, like dmy, or ymd */
	dateOrder?: string;
	/** Default date time format string */
	dateTimeFormat?: string;
	/** AM designator */
	amDesignator?: string;
	/** PM designator */
	pmDesignator?: string;
	/** Time separator */
	timeSeparator?: string;
	/** First day of week, 0 = Sunday, 1 = Monday */
	firstDayOfWeek?: number;
	/** Array of day names */
	dayNames?: string[];
	/** Array of short day names */
	shortDayNames?: string[];
	/** Array of two letter day names */
	minimizedDayNames?: string[];
	/** Array of month names */
	monthNames?: string[];
	/** Array of short month names */
	shortMonthNames?: string[];
}
/** Interface for a locale, similar to .NET's CultureInfo */
export interface Locale extends NumberFormat, DateFormat {
	/** Locale string comparison function, similar to .NET's StringComparer */
	stringCompare?: (a: string, b: string) => number;
	/** Locale string to upper case function */
	toUpper?: (a: string) => string;
}
/** Invariant locale (e.g. CultureInfo.InvariantCulture) */
export declare let Invariant: Locale;
/**
 * Current culture, e.g. CultureInfo.CurrentCulture. This is overridden by
 * settings passed from a `<script>` element in the page with id `ScriptCulture`
 * containing a JSON object if available. This element is generally created in
 * the _LayoutHead.cshtml file for Serenity applications, so that the culture
 * settings determined server, can be passed to the client.
 */
export declare let Culture: Locale;
/**
 * Resets the culture settings to the default values.
 */
export declare function resetCultureSettings(): void;
/**
 * Formats a string with parameters similar to .NET's String.Format function
 * using current `Culture` locale settings.
 */
export declare function stringFormat(format: string, ...prm: any[]): string;
/**
 * Formats a string with parameters similar to .NET's String.Format function
 * using the locale passed as the first argument.
 */
export declare function stringFormatLocale(l: Locale, format: string, ...prm: any[]): string;
/**
 * Rounds a number to specified digits or an integer number if digits are not specified.
 * Uses away from zero rounding (e.g. 1.5 rounds to 2, -1.5 rounds to -2) unlike Math.round.
 * @param num the number to round
 * @param d the number of digits to round to. default is zero.
 * @returns the rounded number
 */
export declare let round: (num: number, d?: number) => number;
/**
 * Truncates a number to an integer number.
 */
export declare let trunc: (n: number) => number;
/**
 * Formats a number using the current `Culture` locale (or the passed locale) settings.
 * It supports format specifiers similar to .NET numeric formatting strings.
 * @param num the number to format
 * @param format the format specifier. default is 'g'.
 * See .NET numeric formatting strings documentation for more information.
 */
export declare function formatNumber(num: number, format?: string, decOrLoc?: string | NumberFormat, grp?: string): string;
/**
 * Converts a string to an integer. The difference between parseInt and parseInteger
 * is that parseInteger will return null if the string is empty or null, whereas
 * parseInt will return NaN and parseInteger will use the current culture's group
 * and decimal separators.
 * @param s the string to parse
 */
export declare function parseInteger(s: string): number;
/**
 * Converts a string to a decimal. The difference between parseFloat and parseDecimal
 * is that parseDecimal will return null if the string is empty or null, whereas
 * parseFloat will return NaN and parseDecimal will use the current culture's group
 * and decimal separators.
 * @param s the string to parse
 */
export declare function parseDecimal(s: string): number;
/**
 * Converts a string to an ID. If the string is a number, it is returned as-is.
 * If the string is empty, null or whitespace, null is returned.
 * Otherwise, it is converted to a number if possible. If the string is not a
 * valid number or longer than 14 digits, the trimmed string is returned as-is.
 * @param id the string to convert to an ID
 */
export declare function toId(id: any): any;
/**
 * Formats a date using the specified format string and optional culture.
 * Supports .NET style format strings including custom formats.
 * See .NET documentation for supported formats.
 * @param d the date to format. If null, it returns empty string.
 * @param format the format string to use. If null, it uses the current culture's default format.
 * 'G' uses the culture's datetime format.
 * 'g' uses the culture's datetime format with secs removed.
 * 'd' uses the culture's date format.
 * 't' uses the culture's time format.
 * 'u' uses the sortable ISO format with UTC time.
 * 'U' uses the culture's date format with UTC time.
 * @param locale the locale to use
 * @returns the formatted date
 * @example
 * // returns "2019-01-01"
 * formatDate(new Date(2019, 0, 1), "yyyy-MM-dd");
 * @example
 * // returns "2019-01-01 12:00:00"
 * formatDate(new Date(2019, 0, 1, 12), "yyyy-MM-dd HH:mm:ss");
 * @example
 * // returns "2019-01-01 12:00:00.000"
 * formatDate(new Date(2019, 0, 1, 12), "yyyy-MM-dd HH:mm:ss.fff");
 * @example
 * // returns "2019-01-01 12:00:00.000 AM"
 * formatDate(new Date(2019, 0, 1, 12), "yyyy-MM-dd HH:mm:ss.fff tt");
 */
export declare function formatDate(d: Date | string, format?: string, locale?: Locale): string;
/**
 * Formats a date as the ISO 8601 UTC date/time format.
 * @param d The date.
 */
export declare function formatISODateTimeUTC(d: Date): string;
/**
 * Parses a string in the ISO 8601 UTC date/time format.
 * @param s The string to parse.
 */
export declare function parseISODateTime(s: string): Date;
/**
 * Parses a string to a date. If the string is empty or whitespace, returns null.
 * Returns a NaN Date if the string is not a valid date.
 * @param s The string to parse.
 * @param dateOrder The order of the date parts in the string. Defaults to culture's default date order.
  */
export declare function parseDate(s: string, dateOrder?: string): Date;
/**
 * Splits a date string into an array of strings, each containing a single date part.
 * It can handle separators "/", ".", "-" and "\".
 * @param s The string to split.
 */
export declare function splitDateString(s: string): string[];
/**
 * Html encodes a string (encodes single and double quotes, & (ampersand), > and < characters)
 * @param s String (or number etc.) to be HTML encoded
 */
export declare function htmlEncode(s: any): string;
/**
 * Toggles the class on the element handling spaces like addClass does.
 * @param el the element
 * @param cls the class to toggle
 * @param add if true, the class will be added, if false the class will be removed, otherwise it will be toggled.
 */
export declare function toggleClass(el: Element, cls: string, add?: boolean): void;
/**
 * Adds a CSS class to the specified element.
 *
 * @param el - The element to add the class to.
 * @param cls - The CSS class to add.
 * @returns A boolean value indicating whether the class was successfully added.
 */
export declare function addClass(el: Element, cls: string): void;
/**
 * Removes a CSS class from an element.
 *
 * @param el - The element from which to remove the class.
 * @param cls - The CSS class to remove.
 * @returns A boolean indicating whether the class was successfully removed.
 */
export declare function removeClass(el: Element, cls: string): void;
/**
 * Appends content like DOM nodes, string, number or an array of these to the parent node.
 * Undefined, null, false values are ignored. Promises are awaited.
 * @param parent Target parent element
 * @param child The content
 */
export declare function appendToNode(parent: ParentNode, child: any): void;
export declare function sanitizeUrl(url: string): string;
/**
 * Gets readonly state of an element. If the element is null, returns null.
 * It does not check for attached widgets. It returns true if the element has readonly class,
 * disabled attribute (select, radio, checkbox) or readonly attribute (other inputs).
 * @param el element
 */
export declare function getElementReadOnly(el: Element): boolean | null;
/**
 * Sets readonly class and disabled (for select, radio, checkbox) or readonly attribute (for other inputs) on given element.
 * It does not check for attached widgets.
 * @param el Element
 * @param value Readonly state
 */
export declare function setElementReadOnly(elements: Element | ArrayLike<Element>, value: boolean): void;
/**
 * Parses a query string into an object.
 * @param s Query string to parse, if not specified, location.search will be used.
 * @return An object with key/value pairs from the query string.
 */
export declare function parseQueryString(s?: string): Record<string, string>;
/**
 * Checks whether a return URL is safe for redirects. Must be relative, start with a single slash,
 * and contain only allowed characters (no protocol, no backslashes, no control chars, etc).
 */
export declare function isSafeReturnUrl(url: string): boolean;
/**
 * Gets the return URL from the query string.
 * @param opt Options for getting the return URL.
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
 * Escapes a CSS selector.
 * @param selector The CSS selector to escape.
 */
export declare function cssEscape(selector: string): string;
/**
 * Adds local text entries to the localization table.
 * @param obj The object containing key/value pairs to add. If a string is provided, it will be added as a key with the prefix (second argument) as its value.
 * @param pre The prefix to add to each key. If obj is a string, this will be the value for that key.
 */
export declare function addLocalText(obj: string | Record<string, string | Record<string, any>>, pre?: string): void;
/**
 * Retrieves a localized string from the localization table.
 * @param key The key of the localized string.
 * @param defaultText The default text to return if the key is not found.
 * @returns The localized string or the default text.
 */
export declare function localText(key: string, defaultText?: string): string;
/**
 * Tries to retrieve a localized string from the localization table.
 * @param key The key of the localized string.
 * @returns The localized string or undefined if not found.
 */
export declare function tryGetText(key: string): string;
/**
 * Creates a proxy object for localized text retrieval.
 * @param obj - The target object to proxy (usually an empty object {})
 * @param pfx - The key prefix for all text lookups
 * @param tpl - Template object defining the structure (object properties become nested proxies)
 * @param mode - The lookup mode: by default it uses localText, e.g. returns the localized text or the text key if not found,
 * "asTry"=tryGetText, e.g. returns undefined if not found, "asKey"=return the text key ("Forms.Something.Abc") as is (no lookup)
 * @returns A proxy object that provides localized text access
 *
 * @example
 * const texts = proxyTexts({}, '', { user: { name: {} } });
 * texts.user.name.first // looks up "user.name.first" key, returns "user.name.first" if not found
 * texts.user.asTry().name.first // returns undefined if not found
 * texts.user.asKey().name.first // returns "user.name.first"
 */
export declare function proxyTexts<T extends Record<string, any> = Record<string, any>>(obj: T, pfx: string, tpl: Record<string, any>, mode?: "asTry" | "asKey"): Record<string, any> & {
	asTry(): T;
	asKey(): T;
};
/**
 * A list of languages with their IDs and display texts.
 */
export type LanguageList = {
	id: string;
	text: string;
}[];
/**
 * Options for translating texts.
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
 * The result of a translation operation.
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
 * Configuration for translation services.
 */
export declare const TranslationConfig: {
	/** Retrieves the list of available languages */
	getLanguageList: () => LanguageList;
	/** A function to translate texts based on provided options */
	translateTexts: (opt: TranslateTextsOptions) => PromiseLike<TranslateTextsResult>;
};
/** @deprecated prefer localText for better discoverability */
export declare const text: typeof localText;
export declare namespace LT {
	/** @deprecated Use addLocalText */
	const add: typeof addLocalText;
	/** @deprecated Use localText */
	const getDefault: typeof localText;
}
export interface LookupOptions<TItem> {
	idField?: string;
	parentIdField?: string;
	textField?: string;
}
export interface Lookup<TItem> {
	items: TItem[];
	itemById: {
		[key: string]: TItem;
	};
	idField: string;
	parentIdField: string;
	textField: string;
}
export declare class Lookup<TItem> {
	items: TItem[];
	itemById: {
		[key: string]: TItem;
	};
	idField: string;
	parentIdField: string;
	textField: string;
	constructor(options: LookupOptions<TItem>, items?: TItem[]);
	update?(value: TItem[]): void;
}
export type ToastContainerOptions = {
	containerId?: string;
	positionClass?: string;
	target?: string;
};
export type ToastrOptions = ToastContainerOptions & {
	/** Show a close button, default is false */
	closeButton?: boolean;
	/** CSS class for close button */
	closeClass?: string;
	/** If true (default) toast keeps open when hovered, and closes after extendedTimeout when mouse leaves the toast */
	closeOnHover?: boolean;
	/** If closeOnHover is true, the toast closes in extendedTimeout duration after the mouse leaves the toast. Default is 1000 */
	extendedTimeOut?: number;
	/** Escape message html, default is true */
	escapeHtml?: boolean;
	/** CSS class for icon */
	iconClass?: string;
	/** CSS class for message */
	messageClass?: string;
	/** Show newest on top */
	newestOnTop?: boolean;
	/** CSS class for toast positioning */
	positionClass?: string;
	/** Prevent duplicates of the same toast, default is false */
	preventDuplicates?: boolean;
	/** Right to left */
	rtl?: boolean;
	/** The container element id */
	target?: string;
	/** The duration for the toast to stay in the page. Set to -1 to make the toast sticky, in that case extendedTimeout is ignored. */
	timeOut?: number;
	/** CSS class for toast */
	toastClass?: string;
	/** Hides the notification when clicked, default is true */
	tapToDismiss?: boolean;
	/** CSS class for title */
	titleClass?: string;
	onclick?: (event: MouseEvent) => void;
	onCloseClick?: (event: Event) => void;
	onHidden?: () => void;
	onShown?: () => void;
};
export type NotifyMap = {
	type: string;
	iconClass: string;
	title?: string;
	message?: string;
};
export declare class Toastr {
	private listener;
	private toastId;
	private previousToast;
	options: ToastrOptions;
	constructor(options?: ToastrOptions);
	getContainer(options?: ToastContainerOptions, create?: boolean): HTMLElement;
	error(message?: string, title?: string, opt?: ToastrOptions): HTMLElement | null;
	warning(message?: string, title?: string, opt?: ToastrOptions): HTMLElement | null;
	success(message?: string, title?: string, opt?: ToastrOptions): HTMLElement | null;
	info(message?: string, title?: string, opt?: ToastrOptions): HTMLElement | null;
	subscribe(callback: (response: Toastr) => void): void;
	publish(args: Toastr): void;
	private removeContainerIfEmpty;
	removeToast(toastElement: HTMLElement, options?: ToastContainerOptions): void;
	clear(options?: ToastContainerOptions): void;
	private notify;
}
export declare let defaultNotifyOptions: ToastrOptions;
export declare function positionToastContainer(options?: ToastrOptions, create?: boolean): void;
export declare function notifyError(message: string, title?: string, options?: ToastrOptions): void;
export declare function notifyInfo(message: string, title?: string, options?: ToastrOptions): void;
export declare function notifySuccess(message: string, title?: string, options?: ToastrOptions): void;
export declare function notifyWarning(message: string, title?: string, options?: ToastrOptions): void;
export declare enum SummaryType {
	Disabled = -1,
	None = 0,
	Sum = 1,
	Avg = 2,
	Min = 3,
	Max = 4
}
export type EditorAddon = (props: {
	propertyItem?: PropertyItem;
	editorElement: HTMLElement;
	documentFragment?: DocumentFragment;
}) => void;
export interface PropertyItem {
	name: string;
	title?: string;
	hint?: string;
	placeholder?: string;
	editorType?: string | {
		new (props?: any): any;
	} | PromiseLike<{
		new (props?: any): any;
	}>;
	editorParams?: any;
	editorAddons?: {
		type: string | EditorAddon;
		params?: any;
	}[];
	editorCssClass?: string;
	category?: string;
	collapsible?: boolean;
	collapsed?: boolean;
	tab?: string;
	cssClass?: string;
	headerCssClass?: string;
	formCssClass?: string;
	maxLength?: number;
	required?: boolean;
	insertable?: boolean;
	insertPermission?: string;
	hideOnInsert?: boolean;
	updatable?: boolean;
	updatePermission?: string;
	hideOnUpdate?: boolean;
	readOnly?: boolean;
	readPermission?: string;
	oneWay?: boolean;
	defaultValue?: any;
	localizable?: boolean;
	visible?: boolean;
	allowHide?: boolean;
	formatterType?: string | {
		new (props?: any): {
			format(ctx: any): string;
		};
	} | PromiseLike<{
		new (props?: any): {
			format(ctx: any): string;
		};
	}>;
	formatterParams?: any;
	displayFormat?: string;
	alignment?: string;
	width?: number;
	widthSet?: boolean;
	minWidth?: number;
	maxWidth?: number;
	labelWidth?: string;
	resizable?: boolean;
	sortable?: boolean;
	sortOrder?: number;
	groupOrder?: number;
	summaryType?: SummaryType;
	editLink?: boolean;
	editLinkItemType?: string;
	editLinkIdField?: string;
	editLinkCssClass?: string;
	filteringType?: string;
	filteringParams?: any;
	filteringIdField?: string;
	notFilterable?: boolean;
	filterOnly?: boolean;
	quickFilter?: boolean;
	quickFilterParams?: any;
	quickFilterSeparator?: boolean;
	quickFilterCssClass?: string;
}
export interface PropertyItemsData {
	items: PropertyItem[];
	additionalItems: PropertyItem[];
}
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
 * Hook for script data related operations
 */
export declare const scriptDataHooks: {
	/**
	 * Provides a hook to override the default fetchScriptData implementation,
	 * it falls back to the default implementation if undefined is returned.
	 * It is recommended to use this hook mainly for test purposes.
	 * If the sync parameter is true (legacy/compat), then the result should be returned synchronously.
	 * DynJS parameter is true if the script is requested to be loaded via a dynamic script,
	 * and not a JSON request. This parameter is only true for the legacy/compat sync mode.
	 */
	fetchScriptData: <TData>(name: string, sync?: boolean, dynJS?: boolean) => TData | Promise<TData>;
};
/**
 * Fetches a script data with given name via ~/DynamicData endpoint
 * @param name Dynamic script name
 * @returns A promise that will return data if successfull
 */
export declare function fetchScriptData<TData>(name: string): Promise<TData>;
/**
 * Returns the script data from cache if available, or via a fetch
 * request to ~/DynamicData endpoint
 * @param name
 * @param reload Clear cache and force reload
 * @returns
 */
export declare function getScriptData<TData = any>(name: string, reload?: boolean): Promise<TData>;
/**
 * Synchronous version of getScriptData for compatibility. Avoid this one where possible,
 * as it will block the UI thread.
 * @param name
 * @param dynJS
 * @returns
 */
export declare function ensureScriptDataSync<TData = any>(name: string, dynJS?: boolean): TData;
/**
 * Gets or loads a [ColumnsScript] data
 * @param key Form key
 * @returns A property items data object containing items and additionalItems properties
 */
export declare function getColumnsScript(key: string): Promise<PropertyItemsData>;
/**
 * Gets or loads a [FormScript] data
 * @param key Form key
 * @returns A property items data object containing items and additionalItems properties
 */
export declare function getFormScript(key: string): Promise<PropertyItemsData>;
/**
 * Gets or loads a Lookup
 * @param key Lookup key
 */
export declare function getLookupAsync<TItem>(key: string): Promise<Lookup<TItem>>;
/**
 * Gets or loads a [RemoteData]
 * @param key Remote data key
 */
export declare function getRemoteDataAsync<TData = any>(key: string): Promise<TData>;
/**
 * Synchronous version of getRemoteDataAsync for compatibility
 * @param key Remote data key
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
export declare function peekScriptData(name: string): any;
/**
 * Forces reload of a lookup from the server. Note that only the
 * client side cache is cleared. This does not force reloading in the server-side.
 * @param key Lookup key
 * @returns Lookup
 */
export declare function reloadLookupAsync<TItem = any>(key: string): Promise<Lookup<TItem>>;
export declare function setRegisteredScripts(scripts: Record<string, string>): void;
export declare function setScriptData(name: string, value: any): void;
export declare function resolveUrl(url: string): string;
export declare function resolveServiceUrl(url: string): string;
export declare function getCookie(name: string): any;
export declare function isSameOrigin(url: string): boolean;
export declare function getServiceOptions<TResponse extends ServiceResponse>(options: ServiceOptions<TResponse>): ServiceOptions<TResponse>;
export declare function requestStarting(): void;
export declare function requestFinished(): void;
export declare function getActiveRequests(): number;
export declare function serviceCall<TResponse extends ServiceResponse>(options: ServiceOptions<TResponse>): PromiseLike<TResponse>;
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
	customAttributes?: any[];
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
/** @deprecated Use getGlobalTypeRegistry instead */
export declare const getTypeRegistry: typeof getGlobalTypeRegistry;
export declare const nsSerenity: "Serenity.";
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
 * Type alias for a function or object (enum).
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
 * @param intf Optional interfaces the class implements
 */
export declare function registerClass(type: any, name: string, intf?: any[]): void;
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
export declare function registerInterface(type: any, name: string, intf?: any[]): void;
/**
 * Enum utilities
 */
export declare namespace Enum {
	/**
	 * Convert an enum value to a string containing enum names.
	 * @param enumType Enum type
	 * @param value Enum value
	 */
	let toString: (enumType: any, value: number) => string;
	/**
	 * Get all numeric values of an enum as an array.
	 * @param enumType
	 * @returns
	 */
	let getValues: (enumType: any) => number[];
}
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
 * Utility type to prevent type inference in generic types.
 * TypeScript 5.4 has added a built-in NoInfer<T> type that can be used instead of this.
 */
export type SNoInfer<T> = [
	T
][T extends any ? 0 : never];
/**
 * Attribute class for editors. This is used by the editorTypeInfo function
 * and registerEditor function to add EditorAttribute to editors.
 */
export declare class EditorAttribute {
}
/**
 * Marker interface for SlickGrid formatters.
 */
export declare class ISlickFormatter {
}
/**
 * Register a SlickGrid formatter.
 * @param type Formatter type
 * @param name Formatter name
 * @param intfAndAttr Optional interface(s) to implement
 */
export declare function registerFormatter(type: any, name: string, intfAndAttr?: any[]): void;
/**
 * Register an editor type. Adds EditorAttribute if not already present.
 * @param type Editor type
 * @param name Editor name
 * @param intf Optional interface(s) to implement
 */
export declare function registerEditor(type: any, name: string, intfAndAttr?: any[]): void;
/**
 * Adds a custom attribute to a type. JavaScript does not have built-in support for attributes,
 * so Serenity uses a customAttributes array on typeInfo to store them. This is used by
 * decorators and some helper functions to add attributes to classes.
 * @param type
 * @param attr
 */
export declare function addCustomAttribute(type: any, attr: any): void;
/**
 * Get a custom attribute of a type.
 * @param type Type to get the attribute from
 * @param attrType Attribute type to get
 * @param inherit Indicates whether to search in base types
 * @returns The custom attribute or null if not found
 */
export declare function getCustomAttribute<TAttr>(type: any, attrType: {
	new (...args: any[]): TAttr;
}, inherit?: boolean): TAttr;
/**
 * Get whether a type has a specific custom attribute.
 * @param type Type to check
 * @param attrType Attribute type to check
 * @param inherit Indicates whether to search in base types
 * @returns True if the type has the attribute
 */
export declare function hasCustomAttribute<TAttr>(type: any, attrType: {
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
/** Class type information. This is used to make type name available in declaration files unlike decorators that does not show in .d.ts files. */
export type ClassTypeInfo<TypeName> = TypeInfo<TypeName>;
/** Editor type information. This is used to make type name available in declaration files unlike decorators that does not show in .d.ts files. */
export type EditorTypeInfo<TypeName> = TypeInfo<TypeName>;
/** Formatter type information. This is used to make type name available in declaration files unlike decorators that does not show in .d.ts files. */
export type FormatterTypeInfo<TypeName> = TypeInfo<TypeName>;
/** Interface type information. This is used to make type name available in declaration files unlike decorators that does not show in .d.ts files. */
export type InterfaceTypeInfo<TypeName> = TypeInfo<TypeName>;
export declare function classTypeInfo<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): ClassTypeInfo<TypeName>;
export declare function editorTypeInfo<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): EditorTypeInfo<TypeName>;
export declare function formatterTypeInfo<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): FormatterTypeInfo<TypeName>;
export declare function interfaceTypeInfo<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): InterfaceTypeInfo<TypeName>;
export declare function registerType(type: {
	[Symbol.typeInfo]: TypeInfo<any>;
	name: string;
}): void;
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
export declare const PagerTexts: typeof webTexts.Controls.Pager;
export declare const PropertyGridTexts: typeof webTexts.Controls.PropertyGrid;
export declare const QuickSearchTexts: typeof webTexts.Controls.QuickSearch;
export declare const SelectEditorTexts: typeof webTexts.Controls.SelectEditor;
export interface TooltipOptions {
	title?: string;
	trigger?: string;
}
export declare class Tooltip {
	private el;
	constructor(el: ArrayLike<HTMLElement> | HTMLElement, opt?: TooltipOptions);
	static defaults: TooltipOptions;
	dispose(): void;
	delayedDispose(delay?: number): void;
	delayedHide(delay?: number): void;
	private static existingInstance;
	static getInstance(el: ArrayLike<HTMLElement> | HTMLElement): Tooltip;
	static get isAvailable(): boolean;
	setTitle(value: string): Tooltip;
	toggle(show: boolean): Tooltip;
	hide(): Tooltip;
	show(): Tooltip;
}
/** Inspired from https://github.com/silverwind/uppie and https://github.com/GoogleChromeLabs/file-drop/blob/master/lib/filedrop.ts */
export interface UploaderOptions {
	/** Accept. If not specified, read from the passed input  */
	accept?: string;
	/** Auto clear input value after selection, so when same file selected it works. Default is true */
	autoClear?: boolean;
	/** Only used for multiple, default is 1 to upload multiple files in batches of size 1 */
	batchSize?: number;
	/** An optional list of dropzones. */
	dropZone?: HTMLElement | ArrayLike<HTMLElement>;
	/** Progress event that is called before first batch start is about to be uploaded */
	allStart?: () => void;
	/** Progress event that is called after last batch is ended uploading or failed */
	allStop?: () => void;
	/** Progress event that is called when a batch is about to be uploaded */
	batchStart?: (data: {
		batch: UploaderBatch;
	}) => void;
	/** Progress event that is called when a batch is ended uploading or failed */
	batchStop?: (data: {
		batch: UploaderBatch;
	}) => void;
	/** Called after batch is uploaded successfully */
	batchSuccess?: (data: UploaderSuccessData) => void;
	/** Progress event that is called during upload */
	batchProgress?: (data: {
		batch: UploaderBatch;
		loaded: number;
		total: number;
	}) => void;
	/** Callback to handle a batch. If not specified, a default handler is used. */
	batchHandler?: (batch: UploaderBatch, uploader: Uploader) => void | Promise<void>;
	/** Only called when a change/drop event occurs, but files can't be determined */
	changeCallback?: (e: Event) => void;
	/** Error handler, if not specified Uploader.errorHandler is used */
	errorHandler?: (data: UploaderErrorData) => void;
	/** Ignore file types, e.g. don't check accept property of input or this options */
	ignoreType?: boolean;
	/** Target input. If null, dropZone should be specified. */
	input?: HTMLInputElement;
	/** Allow multiple files. If not specified is read from the input */
	multiple?: boolean;
	/** The field name to use in FormData object. Default is files[] */
	name?: string;
}
export interface UploaderRequest {
	/** A function that will return headers to be sent with request, or static set of headers */
	headers?: Record<string, string>;
	/** Response type expected from the server. Default is json */
	responseType?: "json" | "text";
	/** URL to send the request to. Default is ~/File/TemporaryUpload */
	url?: string;
}
export interface UploaderBatch {
	event?: Event;
	filePaths?: string[];
	formData: FormData;
	isFirst?: boolean;
}
export interface UploaderSuccessData {
	batch: UploaderBatch;
	request: UploaderRequest;
	event: ProgressEvent;
	xhr: XMLHttpRequest;
	response: any;
}
export interface UploaderErrorData {
	batch?: UploaderBatch;
	event?: ProgressEvent;
	exception?: any;
	request?: UploaderRequest;
	response?: any;
	xhr?: XMLHttpRequest;
}
export declare class Uploader {
	private opt;
	private batch;
	constructor(opt: UploaderOptions);
	private newBatch;
	private addToBatch;
	private endBatch;
	static defaults: Partial<UploaderOptions>;
	static requestDefaults: Partial<UploaderRequest>;
	isMultiple(): boolean;
	private getTypePredicate;
	private getMatchingItems;
	private watchInput;
	private watchDropZone;
	private arrayApi;
	private entriesApi;
	uploadBatch(batch: UploaderBatch, request?: UploaderRequest): Promise<void>;
	static errorHandler(data: UploaderErrorData): void;
}
/**
 * An `HTMLElement` that can be validated (`input`, `select`, `textarea`, or [contenteditable).
 */
export interface ValidatableElement extends HTMLElement {
	form?: HTMLFormElement;
	name?: string;
	type?: string;
	value?: string;
}
export type ValidationValue = string | string[] | number | boolean;
/**
 * Validation plugin signature with multitype return.
 * Boolean return signifies the validation result, which uses the default validation error message read from the element attribute.
 * String return signifies failed validation, which then will be used as the validation error message.
 * Promise return signifies asynchronous plugin behavior, with same behavior as Boolean or String.
 */
export type ValidationProvider = (value: ValidationValue, element: ValidatableElement, params?: any) => boolean | string | Promise<boolean | string>;
export interface ValidationErrorMap {
	[name: string]: (string | boolean);
}
export interface ValidationErrorItem {
	message: string;
	element: ValidatableElement;
	method?: string;
}
export type ValidationErrorList = ValidationErrorItem[];
export type ValidationRules = Record<string, any>;
export interface ValidationRulesMap {
	[name: string]: ValidationRules;
}
export type ValidateEventDelegate = (element: ValidatableElement, event: Event, validator: Validator) => void;
export interface ValidatorOptions {
	/** True for logging debug info */
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
	invalidHandler?(event: Event, validator: Validator): void;
	/**
	 * Key/value pairs defining custom messages. Key is the name of an element, value the message to display for that element. Instead
	 * of a plain message, another map with specific messages for each rule can be used. Overrides the title attribute of an element or
	 * the default message for the method (in that order). Each message can be a String or a Callback. The callback is called in the scope
	 * of the validator, with the rule's parameters as the first argument and the element as the second, and must return a String to display
	 * as the message.
	 *
	 * default: the default message for the method used
	 */
	messages?: Record<string, string | Record<string, string>> | undefined;
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
	 * Pending class
	 * default: "pending"
	 */
	pendingClass?: string | undefined;
	/**
	 * A custom message display handler. Gets the map of errors as the first argument and an array of errors as the second,
	 * called in the context of the validator object. The arguments contain only those elements currently validated,
	 * which can be a single element when doing validation onblur/keyup. You can trigger (in addition to your own messages)
	 * the default behaviour by calling this.defaultShowErrors().
	 */
	rules?: ValidationRulesMap | undefined;
	/**
	 * A custom message display handler. Gets the map of errors as the first argument and an array of errors as the second,
	 * called in the context of the validator object. The arguments contain only those elements currently validated, which can
	 * be a single element when doing validation onblur/keyup. You can trigger (in addition to your own messages) the default
	 * behaviour by calling this.defaultShowErrors().
	 */
	showErrors?(errorMap: ValidationErrorMap, errorList: ValidationErrorList, validator: Validator): void;
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
export declare class Validator {
	static optional(element: ValidatableElement, value?: ValidationValue): "" | "dependency-mismatch";
	static autoCreateRanges: boolean;
	static defaults: ValidatorOptions;
	static readonly messages: Record<string, string | Function>;
	static readonly methods: Record<string, ValidationProvider>;
	readonly settings: ValidatorOptions;
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
	constructor(form: HTMLFormElement, options: ValidatorOptions);
	static getInstance(element: HTMLFormElement | Node | ArrayLike<HTMLElement>): Validator;
	private init;
	/**
	 * Checks if `element` is validatable (`input`, `select`, `textarea`).
	 * @param element The element to check.
	 * @returns `true` if validatable, otherwise `false`.
	 */
	static isValidatableElement(element: EventTarget): element is ValidatableElement;
	static isCheckOrRadio(element: Node): element is HTMLInputElement;
	static getLength(value: ValidationValue, element: HTMLElement): number;
	static isContentEditable(element: HTMLElement): boolean;
	static elementValue(element: HTMLElement): any;
	static valid(element: HTMLFormElement | ValidatableElement | ArrayLike<ValidatableElement>): boolean;
	static rules(element: ValidatableElement, command?: "add" | "remove", argument?: any): ValidationRules;
	form(): boolean;
	checkForm(): boolean;
	element(element: ValidatableElement): boolean;
	showErrors(errors?: ValidationErrorMap): void;
	resetForm(): void;
	resetElements(elements: ValidatableElement[]): void;
	numberOfInvalids(): number;
	private static objectLength;
	hideErrors(): void;
	hideThese(errors: HTMLElement[]): void;
	valid(): boolean;
	size(): number;
	focusInvalid(): void;
	findLastActive(): ValidatableElement;
	elements(): ValidatableElement[];
	errors(): HTMLElement[];
	resetInternals(): void;
	reset(): void;
	resetAll(): void;
	prepareForm(): void;
	prepareElement(element: ValidatableElement): void;
	check(element: ValidatableElement): boolean;
	customDataMessage(element: ValidatableElement, method: string): string;
	customMessage(name: string, method: string): any;
	findDefined(...args: any[]): any;
	defaultMessage(element: ValidatableElement, rule: {
		method: string;
		parameters?: any;
	}): any;
	formatAndAdd(element: ValidatableElement, rule: {
		method: string;
		parameters: any;
	}): void;
	defaultShowErrors(): void;
	validElements(): ValidatableElement[];
	invalidElements(): ValidatableElement[];
	showLabel(element: ValidatableElement, message?: string): void;
	errorsFor(element: ValidatableElement): HTMLElement[];
	idOrName(element: ValidatableElement): string;
	validationTargetFor(element: ValidatableElement): ValidatableElement;
	findByName(name: string): ValidatableElement[];
	dependTypes: {
		boolean: (param: any) => any;
		string: (param: any, element: ValidatableElement) => boolean;
		function: (param: any, element: ValidatableElement) => any;
	};
	depend(param: any, element: ValidatableElement): any;
	startRequest(element: ValidatableElement): void;
	stopRequest(element: ValidatableElement, valid: boolean): void;
	abortRequest(element: ValidatableElement): void;
	previousValue(element: ValidatableElement, method: string): any;
	destroy(): void;
	static classRuleSettings: Record<string, ValidationRules>;
	static addClassRules(className: (string | any), rules: ValidationRules): void;
	static classRules(element: ValidatableElement): ValidationRules;
	static normalizeAttributeRule(rules: ValidationRules, type: string, method: string, value: ValidationValue): void;
	static attributeRules(element: ValidatableElement): ValidationRules;
	static dataRules(element: ValidatableElement): {};
	static staticRules(element: ValidatableElement): ValidationRules;
	static normalizeRules(rules: ValidationRules, element: ValidatableElement): ValidationRules;
	static addMethod(name: string, method: ValidationProvider, message?: string): void;
	static getHighlightTarget(el: HTMLElement): HTMLElement;
	static addCustomRule(element: HTMLElement | ArrayLike<HTMLElement>, rule: (input: ValidatableElement) => string, uniqueName?: string): void;
	static removeCustomRule(element: HTMLElement | ArrayLike<HTMLElement>, uniqueName: string): void;
}
export declare const addValidationRule: typeof Validator.addCustomRule;
export declare const removeValidationRule: typeof Validator.removeCustomRule;
/**
 * Tests if any of array elements matches given predicate. Prefer Array.some() over this function (e.g. `[1, 2, 3].some(predicate)`).
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 * @returns True if any element matches.
 */
export declare function any<TItem>(array: TItem[], predicate: (x: TItem) => boolean): boolean;
/**
 * Counts number of array elements that matches a given predicate.
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 */
export declare function count<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number;
/**
 * Gets first element in an array that matches given predicate similar to LINQ's First.
 * Throws an error if no match is found.
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 * @returns First element that matches.
 */
export declare function first<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;
/**
 * A group item returned by `groupBy()`.
 */
export type GroupByElement<TItem> = {
	/** index of the item in `inOrder` array */
	order: number;
	/** key of the group */
	key: string;
	/** the items in the group */
	items: TItem[];
	/** index of the first item of this group in the original array */
	start: number;
};
/**
 * Return type of the `groupBy` function.
 */
export type GroupByResult<TItem> = {
	byKey: {
		[key: string]: GroupByElement<TItem>;
	};
	inOrder: GroupByElement<TItem>[];
};
/**
 * Groups an array with keys determined by specified getKey() callback.
 * Resulting object contains group objects in order and a dictionary to access by key.
 * This is similar to LINQ's ToLookup function with some additional details like start index.
 * @param items Array to group.
 * @param getKey Function that returns key for each item.
 * @returns GroupByResult object.
 */
export declare function groupBy<TItem>(items: TItem[], getKey: (x: TItem) => any): GroupByResult<TItem>;
/**
 * Gets index of first element in an array that matches given predicate.
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 */
export declare function indexOf<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number;
/**
 * Inserts an item to the array at specified index. Prefer Array.splice unless
 * you need to support IE.
 * @param obj Array or array like object to insert to.
 * @param index Index to insert at.
 * @param item Item to insert.
 * @throws Error if object does not support insert.
 * @example
 * insert([1, 2, 3], 1, 4); // [1, 4, 2, 3]
 * insert({ insert: (index, item) => { this.splice(index, 0, item); } }
 */
export declare function insert(obj: any, index: number, item: any): void;
/**
 * Determines if the object is an array. Prefer Array.isArray over this function (e.g. `Array.isArray(obj)`).
 * @param obj Object to test.
 * @returns True if the object is an array.
 * @example
 * isArray([1, 2, 3]); // true
 * isArray({}); // false
 */
export declare const isArray: (arg: any) => arg is any[];
/**
* Gets first element in an array that matches given predicate.
* Throws an error if no matches is found, or there are multiple matches.
* @param array Array to test.
* @param predicate Predicate to test elements.
* @returns First element that matches.
* @example
* first([1, 2, 3], x => x == 2); // 2
* first([1, 2, 3], x => x == 4); // throws error.
*/
export declare function single<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;
export type Grouping<TItem> = {
	[key: string]: TItem[];
};
/**
 * Maps an array into a dictionary with keys determined by specified getKey() callback,
 * and values that are arrays containing elements for a particular key.
 * @param items Array to map.
 * @param getKey Function that returns key for each item.
 * @returns Grouping object.
 * @example
 * toGrouping([1, 2, 3], x => x % 2 == 0 ? "even" : "odd"); // { odd: [1, 3], even: [2] }
 */
export declare function toGrouping<TItem>(items: TItem[], getKey: (x: TItem) => any): Grouping<TItem>;
/**
 * Gets first element in an array that matches given predicate (similar to LINQ's FirstOrDefault).
 * Returns null if no match is found.
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 * @returns First element that matches.
 * @example
 * tryFirst([1, 2, 3], x => x == 2); // 2
 * tryFirst([1, 2, 3], x => x == 4); // null
 */
export declare function tryFirst<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;
/** @deprecated use alertDialog */
declare const alert$1: typeof alertDialog;
/** @deprecated use confirmDialog */
declare const confirm$1: typeof confirmDialog;
/** @deprecated use informationDialog */
export declare const information: typeof informationDialog;
/** @deprecated use successDialog */
export declare const success: typeof successDialog;
/** @deprecated use warningDialog */
export declare const warning: typeof warningDialog;
/**
 * A string to lowercase function that handles special Turkish
 * characters like 'ı'. Left in for compatibility reasons.
 */
export declare function turkishLocaleToLower(a: string): string;
/**
 * A string to uppercase function that handles special Turkish
 * characters like 'ı'. Left in for compatibility reasons.
 */
export declare function turkishLocaleToUpper(a: string): string;
/**
 * This is an alias for Culture.stringCompare, left in for compatibility reasons.
 * @deprecated Use Culture.stringCompare
 */
export declare let turkishLocaleCompare: (a: string, b: string) => number;
/** @deprecated Use stringFormat */
export declare let format: typeof stringFormat;
/** @deprecated Use stringFormatLocale */
export declare let localeFormat: typeof stringFormatLocale;
/**
 * Formats a number containing number of minutes into a string in the format "d.hh:mm".
 * @param n The number of minutes.
 */
export declare function formatDayHourAndMin(n: number): string;
/**
 * Parses a time string in the format "hh:mm" into a number containing number of minutes.
 * Returns NaN if the hours not in range 0-23 or minutes not in range 0-59.
 * @param value The string to parse.
 */
export declare function parseHourAndMin(value: string): number;
/**
 * Parses a string in the format "d.hh:mm" into a number containing number of minutes.
 * Returns NaN if the hours not in range 0-23 or minutes not in range 0-59.
 * Returns NULL if the string is empty or whitespace.
 */
export declare function parseDayHourAndMin(s: string): number;
/**
 * Adds an empty option to the select.
 * @param select the select element
 */
export declare function addEmptyOption(select: ArrayLike<HTMLElement> | HTMLSelectElement): void;
/**
 * Adds an option to the select.
 */
export declare function addOption(select: ArrayLike<HTMLElement> | HTMLSelectElement, key: string, text: string): void;
/** @deprecated use htmlEncode as it also encodes quotes */
export declare const attrEncode: typeof htmlEncode;
/** Clears the options in the select element */
export declare function clearOptions(select: HTMLElement | ArrayLike<HTMLElement>): void;
/**
 * Finds the first element with the given relative id to the source element.
 * It can handle underscores in the source element id.
 * @param element the source element
 * @param relativeId the relative id to the source element
 * @param context the context element (optional)
 * @returns the element with the given relative id to the source element.
 */
export declare function findElementWithRelativeId(element: HTMLElement | ArrayLike<HTMLElement>, relativeId: string, context?: HTMLElement): HTMLElement;
/**
 * Creates a new DIV and appends it to the body.
 * @returns the new DIV element.
 */
export declare function newBodyDiv(): HTMLDivElement;
/**
 * Returns the outer HTML of the element.
 */
export declare function outerHtml(element: Element | ArrayLike<HTMLElement>): string;
export declare function getWidgetName(type: Function): string;
export declare function associateWidget(widget: {
	domNode: HTMLElement;
}): void;
export declare function deassociateWidget(widget: {
	domNode: HTMLElement;
}): void;
export declare function tryGetWidget<TWidget>(element: Element | ArrayLike<HTMLElement> | string, type?: {
	new (...args: any[]): TWidget;
}): TWidget;
export declare function getWidgetFrom<TWidget>(element: ArrayLike<HTMLElement> | Element | string, type?: {
	new (...args: any[]): TWidget;
}): TWidget;
export type IdPrefixType = {
	[key: string]: string;
	Form: string;
	Tabs: string;
	Toolbar: string;
	PropertyGrid: string;
};
export declare function useIdPrefix(prefix: string): IdPrefixType;
export type WidgetProps<P> = {
	id?: string;
	class?: string;
	element?: ((el: HTMLElement) => void) | HTMLElement | ArrayLike<HTMLElement> | string;
} & SNoInfer<P>;
export declare class Widget<P = {}> {
	private static nextWidgetNumber;
	protected readonly options: WidgetProps<P>;
	readonly uniqueName: string;
	readonly idPrefix: string;
	readonly domNode: HTMLElement;
	constructor(props: WidgetProps<P>);
	destroy(): void;
	static createDefaultElement(): HTMLElement;
	/**
	 * Returns a Fluent(this.domNode) object
	 */
	get element(): Fluent;
	protected addCssClass(): void;
	protected deferRender(): boolean;
	protected getCssClass(): string;
	static getWidgetName(type: Function): string;
	addValidationRule(rule: (input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement) => string, uniqueName?: string): void;
	addValidationRule(uniqueName: string, rule: (input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement) => string): void;
	protected byId<TElement extends HTMLElement = HTMLElement>(id: string): Fluent<TElement>;
	protected findById<TElement extends HTMLElement = HTMLElement>(id: string): TElement;
	getGridField(): Fluent;
	change(handler: (e: Event) => void): void;
	changeSelect2(handler: (e: Event) => void): void;
	static create<TWidget extends Widget<P>, P>(params: CreateWidgetParams<TWidget, P>): TWidget;
	protected getCustomAttribute<TAttr>(attrType: {
		new (...args: any[]): TAttr;
	}, inherit?: boolean): TAttr;
	protected afterRender(callback: () => void): void;
	init(): this;
	/**
	 * Returns the main element for this widget or the document fragment.
	 * As widgets may get their elements from props unlike regular JSX widgets,
	 * this method should not be overridden. Override renderContents() instead.
	 */
	render(): any;
	internalRenderContents(): void;
	protected renderContents(): any;
	protected legacyTemplateRender(): boolean;
	get props(): WidgetProps<P>;
	protected syncOrAsyncThen<T>(syncMethod: (() => T), asyncMethod: (() => PromiseLike<T>), then: (v: T) => void): void;
	protected useIdPrefix(): IdPrefixType;
	static readonly isComponent = true;
	protected static registerClass<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): ClassTypeInfo<TypeName>;
	protected static registerEditor<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): EditorTypeInfo<TypeName>;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
}
/** @deprecated Use Widget */
export declare const TemplatedWidget: typeof Widget;
export interface CreateWidgetParams<TWidget extends Widget<P>, P> {
	type?: {
		new (options?: P): TWidget;
		prototype: TWidget;
	};
	options?: P & WidgetProps<{}>;
	container?: HTMLElement | ArrayLike<HTMLElement>;
	element?: (e: Fluent) => void;
	init?: (w: TWidget) => void;
}
export declare function GridPageInit<TGrid extends Widget<P>, P>({ type, props }: {
	type: CreateWidgetParams<TGrid, P>["type"];
	props?: WidgetProps<P>;
}): HTMLElement;
export declare function PanelPageInit<TPanel extends Widget<P>, P>({ type, props }: {
	type: CreateWidgetParams<TPanel, P>["type"];
	props?: WidgetProps<P>;
}): HTMLElement;
export declare function gridPageInit<TGrid extends Widget<P>, P>(grid: TGrid & {
	domNode: HTMLElement;
}): TGrid;
export declare function gridPageInit<TGrid extends Widget<P>, P>(type: CreateWidgetParams<TGrid, P>["type"], props?: WidgetProps<P>): TGrid;
export declare function panelPageInit<TGrid extends Widget<P>, P>(panel: TGrid & {
	domNode: HTMLElement;
}): TGrid;
export declare function panelPageInit<TGrid extends Widget<P>, P>(type: CreateWidgetParams<TGrid, P>["type"], props?: WidgetProps<P>): TGrid;
export declare function initFullHeightGridPage(gridDiv: HTMLElement | ArrayLike<HTMLElement> | {
	domNode: HTMLElement;
}, opt?: {
	noRoute?: boolean;
	setHeight?: boolean;
}): void;
export declare function layoutFillHeightValue(element: HTMLElement | ArrayLike<HTMLElement>): number;
export declare function layoutFillHeight(element: HTMLElement | ArrayLike<HTMLElement>): void;
export declare function isMobileView(): boolean;
export declare function triggerLayoutOnShow(element: HTMLElement | ArrayLike<HTMLElement>): void;
export declare function centerDialog(el: HTMLElement | ArrayLike<HTMLElement>): void;
export declare namespace LayoutTimer {
	function store(key: number): void;
	function trigger(key: number): void;
	function onSizeChange(element: () => HTMLElement, handler: () => void, opt?: {
		width?: boolean;
		height?: boolean;
		debounceTimes?: number;
	}): number;
	function onWidthChange(element: () => HTMLElement, handler: () => void, opt?: {
		debounceTimes?: number;
	}): number;
	function onHeightChange(element: () => HTMLElement, handler: () => void, opt?: {
		debounceTimes?: number;
	}): number;
	function onShown(element: () => HTMLElement, handler: () => void, opt?: {
		debounceTimes?: number;
	}): number;
	function off(key: number): number;
}
export declare function executeOnceWhenVisible(el: HTMLElement | ArrayLike<HTMLElement>, callback: Function): void;
export declare function executeEverytimeWhenVisible(el: HTMLElement | ArrayLike<HTMLElement>, callback: Function, callNowIfVisible: boolean): void;
export interface HandleRouteEvent extends Event {
	route: string;
	parts: string[];
	index: number;
	isInitial: boolean;
}
export declare namespace Router {
	let enabled: boolean;
	function navigate(newHash: string, tryBack?: boolean, silent?: boolean): void;
	function replace(newHash: string, tryBack?: boolean): void;
	function replaceLast(newHash: string, tryBack?: boolean): void;
	function dialog(owner: HTMLElement | ArrayLike<HTMLElement>, element: HTMLElement | ArrayLike<HTMLElement>, dialogHash: () => string): void;
	let mightBeRouteRegex: RegExp;
	function resolve(newHash?: string): "disabled" | "skipped" | "shebang" | "missinghandler" | "calledhandler";
	function ignoreHashChange(expiration?: number): void;
}
export declare namespace ScriptData {
	const canLoad: typeof canLoadScriptData;
	const ensure: typeof ensureScriptDataSync;
	const set: typeof setScriptData;
	function bindToChange(name: string, onChange: () => void): void | (() => void);
	function reload<TData = any>(name: string, dynJS?: boolean): TData;
	function reloadAsync<TData = any>(name: string): Promise<TData>;
}
/**
 * Check if a dynamic script with provided name is available in the cache
 * or it is a registered script name
 * @param name Dynamic script name
 * @returns True if already available or registered
 */
export declare function canLoadScriptData(name: string): boolean;
export declare function getLookup<TItem>(key: string): Lookup<TItem>;
export declare function reloadLookup<TItem = any>(key: string): Lookup<TItem>;
export declare function getColumns(key: string): PropertyItem[];
export declare function getColumnsAsync(key: string): Promise<PropertyItem[]>;
export declare function getColumnsData(key: string): PropertyItemsData;
export declare const getColumnsDataAsync: typeof getColumnsScript;
export declare function getForm(key: string): PropertyItem[];
export declare function getFormAsync(key: string): Promise<PropertyItem[]>;
export declare function getFormData(key: string): PropertyItemsData;
export declare const getFormDataAsync: typeof getFormScript;
export declare function setEquality(request: ListRequest, field: string, value: any): void;
export interface PostToServiceOptions {
	url?: string;
	service?: string;
	target?: string;
	request: any;
}
export interface PostToUrlOptions {
	url?: string;
	target?: string;
	params: any;
}
export declare function postToService(options: PostToServiceOptions): void;
export declare function postToUrl(options: PostToUrlOptions): void;
/**
 * Checks if the string ends with the specified substring.
 * @deprecated Use .endsWith method of String directly
 * @param s String to check.
 * @param suffix Suffix to check.
 * @returns True if the string ends with the specified substring.
  */
export declare function endsWith(s: string, suffix: string): boolean;
/**
 * Checks if the string is empty or null. Prefer (!s) instead.
 * @param s String to check.
 * @returns True if the string is empty or null.
 */
export declare function isEmptyOrNull(s: string): boolean;
/**
 * Checks if the string is empty or null or whitespace. Prefer !s?.Trim() instead.
 * @param s String to check.
 * @returns True if the string is empty or null or whitespace.
 */
export declare function isTrimmedEmpty(s: string): boolean;
/**
 * Pads the string to the left with the specified character.
 * @param s String to pad.
 * @param len Target length of the string.
 * @param ch Character to pad with.
 * @returns Padded string.
 */
export declare function padLeft(s: string | number, len: number, ch?: string): any;
/**
 * Checks if the string starts with the prefix
 * @deprecated Use .startsWith method of String directly
 * @param s String to check.
 * @param prefix Prefix to check.
 * @returns True if the string starts with the prefix.
 */
export declare function startsWith(s: string, prefix: string): boolean;
/**
 * Converts the string to single line by removing line end characters
 * @param str String to convert.
 */
export declare function toSingleLine(str: string): string;
/**
 * Trims the whitespace characters from the end of the string
 */
export declare const trimEnd: (s: string) => any;
/**
 * Trims the whitespace characters from the start of the string
 */
export declare const trimStart: (s: string) => any;
/**
 * Trims the whitespace characters from the start and end of the string
  */
export declare function trim(s: string): string;
/**
 * Trims the whitespace characters from the start and end of the string
 * Returns empty string if the string is null or undefined.
 */
export declare function trimToEmpty(s: string): string;
/**
 * Trims the whitespace characters from the start and end of the string
 * Returns null if the string is null, undefined or whitespace.
 */
export declare function trimToNull(s: string): string;
/**
 * Replaces all occurrences of the search string with the replacement string.
 * @param str String to replace.
 * @param find String to find.
 * @param replace String to replace with.
 * @returns Replaced string.
 */
export declare function replaceAll(str: string, find: string, replace: string): string;
/**
 * Pads the start of string to make it the specified length.
 * @param n The number to pad.
 * @param len Target length of the string.
 */
export declare function zeroPad(n: number, len: number): string;
export type Dictionary<TItem> = {
	[key: string]: TItem;
};
/** @deprecated Use ?? operator */
export declare function coalesce(a: any, b: any): any;
/** @deprecated Use a != null */
export declare function isValue(a: any): boolean;
/** Extends an object with properties from another object similar to Object.assign.
 * @deprecated Use Object.assign
 */
export declare function extend<T = any>(a: T, b: T): T;
/** Returns the current date without time part */
export declare let today: () => Date;
/**
 * Deep clones an object or value.
 * @param a The value to clone.
 * @param a2 An optional second value to merge into the clone.
 * @param a3 An optional third value to merge into the clone.
 * @returns A deep clone of the input value.
 */
export declare function deepClone<T = any>(a: T): T;
/** Type member information, preserved for compatibility as used by legacy option decorator */
export interface TypeMember {
	name: string;
	kind: TypeMemberKind;
	attr?: any[];
	getter?: string;
	setter?: string;
}
/** Bitmask for type member kinds */
export declare enum TypeMemberKind {
	field = 4,
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
export declare function clearKeys(d: any): void;
export declare function keyOf<T>(prop: keyof T): keyof T;
export declare function cast(instance: any, type: Type): any;
export declare function safeCast(instance: any, type: Type): any;
export declare function validatorAbortHandler(validator: Validator): void;
export declare function validateOptions(options?: ValidatorOptions): ValidatorOptions;
export declare namespace ValidationHelper {
	function asyncSubmit(form: ArrayLike<HTMLElement> | HTMLElement, validateBeforeSave: () => boolean, submitHandler: () => void): boolean;
	function submit(form: ArrayLike<HTMLElement> | HTMLElement, validateBeforeSave: () => boolean, submitHandler: () => void): boolean;
	function getValidator(elem: ArrayLike<HTMLElement> | HTMLElement): Validator;
	function validateElement(elem: ArrayLike<HTMLElement> | HTMLElement): void;
}
export declare function jQueryPatch(): boolean;
export declare class IBooleanValue {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface IBooleanValue {
	get_value(): boolean;
	set_value(value: boolean): void;
}
export declare class IDialog {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface IDialog {
	dialogOpen(asPanel?: boolean): void;
}
export declare class IDoubleValue {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface IDoubleValue {
	get_value(): any;
	set_value(value: any): void;
}
export declare class IEditDialog {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface IEditDialog {
	load(entityOrId: any, done: () => void, fail?: (p1: any) => void): void;
}
export declare class IGetEditValue {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface IGetEditValue {
	getEditValue(property: PropertyItem, target: any): void;
}
export declare class IReadOnly {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface IReadOnly {
	get_readOnly(): boolean;
	set_readOnly(value: boolean): void;
}
export declare class ISetEditValue {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface ISetEditValue {
	setEditValue(source: any, property: PropertyItem): void;
}
export declare class IStringValue {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface IStringValue {
	get_value(): string;
	set_value(value: string): void;
}
export declare class IValidateRequired {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface IValidateRequired {
	get_required(): boolean;
	set_required(value: boolean): void;
}
export interface IAggregator {
	init(): void;
	accumulate(item: any): void;
	storeResult(totals: IGroupTotals): void;
}
export declare namespace Aggregators {
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
export declare namespace AggregateFormatting {
	function groupTotalsFormat(ctx: FormatterContext<IGroupTotals>): FormatterResult;
	/**
	 * Call this method to ensure that `gridDefaults.groupTotalsFormat` is set to `AggregateFormatting.groupTotalsFormat`.
	 * It only sets it when it is not already set to some value. This is normally called by `RemoteView` constructor.
	 */
	function initGridDefaults(): void;
}
export type Format<TItem = any> = (ctx: FormatterContext<TItem>) => FormatterResult;
declare module "@serenity-is/sleekgrid" {
	interface Column<TItem = any> {
		referencedFields?: string[];
		sourceItem?: PropertyItem;
	}
}
export interface Formatter {
	format(ctx: FormatterContext): FormatterResult;
}
export interface GroupInfo<TItem> {
	getter?: any;
	getterIsAFn?: boolean;
	formatter?: (p1: Group<TItem>) => string;
	comparer?: (a: Group<TItem>, b: Group<TItem>) => number;
	aggregators?: IAggregator[];
	aggregateChildGroups?: boolean;
	aggregateCollapsed?: boolean;
	aggregateEmpty?: boolean;
	collapsed?: boolean;
	displayTotalsRow?: boolean;
	lazyTotalsCalculation?: boolean;
	predefinedValues?: any[];
}
export interface PagerOptions {
	view?: any;
	showRowsPerPage?: boolean;
	rowsPerPage?: number;
	rowsPerPageOptions?: number[];
	onChangePage?: (newPage: number) => void;
	onRowsPerPageChange?: (n: number) => void;
}
export interface SummaryOptions {
	aggregators: IAggregator[];
}
export interface PagingOptions {
	rowsPerPage?: number;
	page?: number;
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
	/** Event fired when data loading completes */
	readonly onDataLoaded: EventEmitter<any, IEventData>;
	/** Event fired when data loading begins */
	readonly onDataLoading: EventEmitter<any, IEventData>;
	/** Event fired when a group is collapsed */
	readonly onGroupCollapsed?: EventEmitter<any, IEventData>;
	/** Event fired when a group is expanded */
	readonly onGroupExpanded?: EventEmitter<any, IEventData>;
	/** Event fired when paging information changes */
	readonly onPagingInfoChanged: EventEmitter<any, IEventData>;
	/** Callback invoked to process data received from the server */
	onProcessData: RemoteViewProcessCallback<TItem>;
	/** Event fired when rows or count change */
	readonly onRowsOrCountChanged?: EventEmitter<any, IEventData>;
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
	syncGridCellCssStyles?(grid: Grid, key: string): void;
	/***
	 * Wires the grid and the DataView together to keep row selection tied to item ids.
	 */
	syncGridSelection?(grid: Grid, preserveHidden?: boolean, preserveHiddenOnSelectionChange?: boolean): EventEmitter<any, IEventData>;
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
 * A data view that supports remote data loading, sorting, filtering, grouping, and paging.
 * Extends the functionality of SlickGrid's DataView with server-side data operations.
 *
 * @typeparam TItem The type of entities in the view
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
	/** Additional parameters to send with service requests */
	params: Record<string, any>;
	/** The page number to seek to when loading data */
	seekToPage: number;
	/** Sort expressions for the data */
	sortBy: string[];
	/** The URL to fetch data from */
	url: string;
	/** Callback invoked before making AJAX calls */
	onAjaxCall: RemoteViewAjaxCallback<TItem>;
	/** Callback invoked to process data received from the server */
	onProcessData: RemoteViewProcessCallback<TItem>;
	/** Callback invoked before submitting a request, can cancel the operation */
	onSubmit: CancellableViewCallback<TItem>;
	/** Event fired when the underlying data changes */
	readonly onDataChanged: EventEmitter<any, import("@serenity-is/sleekgrid").IEventData>;
	/** Event fired when data loading completes */
	readonly onDataLoaded: EventEmitter<any, import("@serenity-is/sleekgrid").IEventData>;
	/** Event fired when data loading begins */
	readonly onDataLoading: EventEmitter<any, import("@serenity-is/sleekgrid").IEventData>;
	/** Event fired when a group is collapsed */
	readonly onGroupCollapsed: EventEmitter<any, import("@serenity-is/sleekgrid").IEventData>;
	/** Event fired when a group is expanded */
	readonly onGroupExpanded: EventEmitter<any, import("@serenity-is/sleekgrid").IEventData>;
	/** Event fired when paging information changes */
	readonly onPagingInfoChanged: EventEmitter<any, import("@serenity-is/sleekgrid").IEventData>;
	/** Event fired when the row count changes */
	readonly onRowCountChanged: EventEmitter<any, import("@serenity-is/sleekgrid").IEventData>;
	/** Event fired when specific rows change */
	readonly onRowsChanged: EventEmitter<any, import("@serenity-is/sleekgrid").IEventData>;
	/** Event fired when rows or count change */
	readonly onRowsOrCountChanged: EventEmitter<any, import("@serenity-is/sleekgrid").IEventData>;
	constructor(options: RemoteViewOptions<TItem>);
	/** Default configuration for grouping information */
	static readonly groupingInfoDefaults: GroupInfo<any>;
	/**
	 * Begins a batch update operation. Multiple changes can be made without triggering refreshes.
	 * Call endUpdate() to complete the batch and refresh the view.
	 */
	beginUpdate(): void;
	/**
	 * Ends a batch update operation. If this is the outermost endUpdate call,
	 * refreshes the view to reflect all changes made during the batch.
	 */
	endUpdate(): void;
	/**
	 * Sets hints for the next refresh operation to optimize performance.
	 * @param hints Object containing refresh hints like isFilterNarrowing, isFilterExpanding, etc.
	 */
	setRefreshHints(hints: any): void;
	private updateIdxById;
	private ensureIdUniqueness;
	/**
	 * Gets all items in the view.
	 * @returns Array of all items
	 */
	getItems(): TItem[];
	/**
	 * Gets the name of the property used as the unique identifier for items.
	 * @returns The ID property name
	 */
	getIdPropertyName(): string;
	/**
	 * Sets the items in the view and optionally changes the ID property.
	 * @param data Array of items to set
	 * @param newIdProperty Optional new ID property name, or boolean to reset
	 */
	setItems(data: any[], newIdProperty?: string | boolean): void;
	/**
	 * Sets paging options and triggers a data reload if options changed.
	 * @param args The paging options to set
	 */
	setPagingOptions(args: PagingOptions): void;
	/**
	 * Gets the current paging information.
	 * @returns Object containing paging state information
	 */
	getPagingInfo(): PagingInfo;
	private getSortComparer;
	/**
	 * Sorts the items using the specified comparer function.
	 * @param comparer Optional custom comparer function
	 * @param ascending Whether to sort in ascending order (default true)
	 */
	sort(comparer?: (a: any, b: any) => number, ascending?: boolean): void;
	/**
	 * Gets whether local sorting is enabled.
	 * @returns true if local sorting is enabled
	 */
	getLocalSort(): boolean;
	/**
	 * Sets whether to use local sorting. When enabled, sorting is done client-side.
	 * @param value Whether to enable local sorting
	 */
	setLocalSort(value: boolean): void;
	/**
	 * Re-sorts the items using the current sort settings.
	 */
	reSort(): void;
	/**
	 * Gets the filtered items (after applying the current filter).
	 * @returns Array of filtered items
	 */
	getFilteredItems(): any;
	/**
	 * Gets the current filter function.
	 * @returns The current filter function
	 */
	getFilter(): RemoteViewFilter<TItem>;
	/**
	 * Sets the filter function to apply to items.
	 * @param filterFn The filter function to apply
	 */
	setFilter(filterFn: RemoteViewFilter<TItem>): void;
	/**
	 * Gets the current grouping configuration.
	 * @returns Array of grouping information
	 */
	getGrouping(): GroupInfo<TItem>[];
	/**
	 * Sets summary/aggregation options for the view.
	 * @param summary Object containing aggregators and other summary options
	 */
	setSummaryOptions(summary: SummaryOptions): void;
	/**
	 * Gets the grand totals for all aggregated data.
	 * @returns Object containing grand totals
	 */
	getGrandTotals(): IGroupTotals;
	/**
	 * Sets the grouping configuration for the view.
	 * @param groupingInfo Grouping information or array of grouping information
	 */
	setGrouping(groupingInfo: GroupInfo<TItem> | GroupInfo<TItem>[]): void;
	/**
	 * Gets an item by its index in the items array.
	 * @param i The index of the item
	 * @returns The item at the specified index
	 */
	getItemByIdx(i: number): any;
	/**
	 * Gets the index of an item by its ID.
	 * @param id The ID of the item
	 * @returns The index of the item, or undefined if not found
	 */
	getIdxById(id: any): number;
	private ensureRowsByIdCache;
	/**
	 * Gets the row index for an item.
	 * @param item The item to find
	 * @returns The row index of the item
	 */
	getRowByItem(item: any): number;
	/**
	 * Gets the row index for an item by its ID.
	 * @param id The ID of the item
	 * @returns The row index of the item
	 */
	getRowById(id: any): number;
	/**
	 * Gets an item by its ID.
	 * @param id The ID of the item
	 * @returns The item with the specified ID
	 */
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
	/**
	 * Updates an existing item in the view.
	 * @param id The ID of the item to update
	 * @param item The new item data
	 */
	updateItem(id: any, item: any): void;
	/**
	 * Inserts an item at the specified position.
	 * @param insertBefore The index to insert before
	 * @param item The item to insert
	 */
	insertItem(insertBefore: number, item: any): void;
	/**
	 * Adds an item to the end of the items array.
	 * @param item The item to add
	 */
	addItem(item: any): void;
	/**
	 * Deletes an item by its ID.
	 * @param id The ID of the item to delete
	 */
	deleteItem(id: any): void;
	/**
	 * Adds an item in sorted order.
	 * @param item The item to add
	 */
	sortedAddItem(item: any): void;
	/**
	 * Updates an item while maintaining sorted order.
	 * @param id The ID of the item to update
	 * @param item The new item data
	 */
	sortedUpdateItem(id: any, item: any): void;
	private sortedIndex;
	/**
	 * Gets all rows in the view (including group rows and totals rows).
	 * @returns Array of all rows
	 */
	getRows(): (TItem | Group<any> | GroupTotals<any>)[];
	/**
	 * Gets the total number of rows in the view.
	 * @returns The number of rows
	 */
	getLength(): number;
	/**
	 * Gets the item at the specified row index.
	 * @param i The row index
	 * @returns The item at the row index
	 */
	getItem(i: number): any;
	/**
	 * Gets metadata for the item at the specified row index.
	 * @param row The row index
	 * @returns Metadata object or null
	 */
	getItemMetadata(row: number): any;
	private expandCollapseAllGroups;
	/**
	 * Collapses all groups at the specified level, or all levels if not specified.
	 * @param level Optional level to collapse. If not specified, applies to all levels.
	 */
	collapseAllGroups(level?: number): void;
	/**
	 * Expands all groups at the specified level, or all levels if not specified.
	 * @param level Optional level to expand. If not specified, applies to all levels.
	 */
	expandAllGroups(level?: number): void;
	private resolveLevelAndGroupingKey;
	private expandCollapseGroup;
	/**
	 * Collapses a specific group.
	 * @param varArgs Either a Slick.Group's "groupingKey" property, or a
	 * variable argument list of grouping values denoting a unique path to the row.
	 * For example, calling collapseGroup('high', '10%') will collapse the '10%' subgroup of the 'high' group.
	 */
	collapseGroup(varArgs: any[]): void;
	/**
	 * Expands a specific group.
	 * @param varArgs Either a Slick.Group's "groupingKey" property, or a
	 * variable argument list of grouping values denoting a unique path to the row.
	 * For example, calling expandGroup('high', '10%') will expand the '10%' subgroup of the 'high' group.
	 */
	expandGroup(varArgs: any[]): void;
	/**
	 * Gets the current groups.
	 * @returns Array of groups
	 */
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
	/**
	 * Refresh the view by recalculating the rows and notifying changes.
	 * Note that this does not re-fetch the data from the server, use populate
	 * method for that purpose.
	 */
	refresh(): void;
	/***
	 * Wires the grid and the DataView together to keep row selection tied to item ids.
	 * This is useful since, without it, the grid only knows about rows, so if the items
	 * move around, the same rows stay selected instead of the selection moving along
	 * with the items.
	 *
	 * NOTE:  This doesn't work with cell selection model.
	 *
	 * @param grid The grid to sync selection with.
	 * @param preserveHidden Whether to keep selected items that go out of the
	 *     view due to them getting filtered out.
	 * @param preserveHiddenOnSelectionChange Whether to keep selected items
	 *     that are currently out of the view (see preserveHidden) as selected when selection
	 *     changes.
	 * @return An event that notifies when an internal list of selected row ids
	 *     changes.  This is useful since, in combination with the above two options, it allows
	 *     access to the full list selected row ids, and not just the ones visible to the grid.
	 */
	syncGridSelection(grid: Grid, preserveHidden?: boolean, preserveHiddenOnSelectionChange?: boolean): EventEmitter<any, import("@serenity-is/sleekgrid").IEventData>;
	/**
	 * Syncs cell CSS styles between the grid and the data view.
	 * @param grid The grid to sync styles with
	 * @param key The style key to sync
	 */
	syncGridCellCssStyles(grid: Grid, key: string): void;
	/**
	 * Adds data received from the server to the view.
	 * @param data The response data from the server
	 */
	addData(data: any): boolean;
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
	 * Gets the group item metadata provider.
	 * @returns The metadata provider
	 */
	getGroupItemMetadataProvider(): GroupItemMetadataProvider;
	/**
	 * Sets the group item metadata provider.
	 * @param value The metadata provider to set
	 */
	setGroupItemMetadataProvider(value: GroupItemMetadataProvider): void;
	/** @deprecated Gets the ID property name, for compatibility */
	get idField(): string;
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
 * Indicates the enum key of an enum type (by default the name of the enum type is used as key)
 */
export declare class EnumKeyAttribute {
	value: string;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor(value: string);
}
/**
 * Indicates if a dialog should have a close button in its title bar (default true)
 */
export declare class CloseButtonAttribute {
	value: boolean;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor(value?: boolean);
}
/**
 * Indicates the element type of a widget like "div", "span" etc.
 */
export declare class ElementAttribute {
	value: string;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor(value: string);
}
/**
 * Indicates if a grid should have an advanced filter editor
 */
export declare class FilterableAttribute {
	value: boolean;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor(value?: boolean);
}
/**
 * Indicates that a dialog or panel should be maximizable.
 * Requires jquery ui dialogs and jquery.dialogextend.js.
 * It does not work with current bootstrap modals.
 */
export declare class MaximizableAttribute {
	value: boolean;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor(value?: boolean);
}
/**
 * Indicates that the property is an option. This is no longer used as JSX
 * does not support it, but it is kept for backward compatibility.
 */
export declare class OptionAttribute {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
}
/**
 * Indicates if a dialog should be opened as a panel
 */
export declare class PanelAttribute {
	value: boolean;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor(value?: boolean);
}
/**
 * Indicates if a dialog should be resizable, only for jquery ui dialogs.
 */
export declare class ResizableAttribute {
	value: boolean;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor(value?: boolean);
}
/**
 * Indicates if a dialog should be a static panel, which is not a dialog at all,
 * but a simple div element embedded in the page.
 * It does not have a title bar, close button or modal behavior.
 * It is just a way to show a form inside a page, without any dialog stuff.
 */
export declare class StaticPanelAttribute {
	value: boolean;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor(value?: boolean);
}
export declare enum CaptureOperationType {
	Before = 0,
	Delete = 1,
	Insert = 2,
	Update = 3
}
export interface DataChangeInfo extends Event {
	operationType: string;
	entityId: any;
	entity: any;
}
export declare namespace Decorators {
	function registerType(): (target: Function & {
		[Symbol.typeInfo]: any;
	}, _context?: any) => void;
	function registerClass(nameOrIntf?: string | any[], intf2?: any[]): (target: Function, _context?: any) => void;
	function registerInterface(nameOrIntf?: string | any[], intf2?: any[]): (target: Function, _context?: any) => void;
	function registerEditor(nameOrIntf?: string | any[], intf2?: any[]): (target: Function, _context?: any) => void;
	function registerEnum(target: any, enumKey?: string, name?: string): void;
	function registerEnumType(target: any, name?: string, enumKey?: string): void;
	function registerFormatter(nameOrIntf?: string | any[], intf2?: any[]): (target: Function, _context?: any) => void;
	function enumKey(value: string): (target: Function, _context?: any) => void;
	function option(): (target: Object, propertyKey: string) => void;
	function closeButton(value?: boolean): (target: Function, _context?: any) => void;
	function editor(): (target: Function, _context?: any) => void;
	function element(value: string): (target: Function, _context?: any) => void;
	function filterable(value?: boolean): (target: Function, _context?: any) => void;
	function maximizable(value?: boolean): (target: Function, _context?: any) => void;
	function panel(value?: boolean): (target: Function, _context?: any) => void;
	function resizable(value?: boolean): (target: Function, _context?: any) => void;
	/**
	 * Deprecated as all dialogs are responsive.
	 * @deprecated This is no longer used as all dialogs are responsive.
	 */
	function responsive(value?: boolean): (target: Function, _context?: any) => void;
	function staticPanel(value?: boolean): (target: Function, _context?: any) => void;
}
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
export declare const DialogTypeRegistry: DialogTypeRegistryImpl;
export type EditorType = {
	new (props?: WidgetProps<any>): Widget<any>;
};
declare class EditorTypeRegistryImpl extends BaseTypeRegistry<EditorType> {
	constructor();
	protected isMatchingType(type: any): boolean;
	protected loadError(key: string): void;
}
export declare const EditorTypeRegistry: EditorTypeRegistryImpl;
declare class EnumTypeRegistryImpl extends BaseTypeRegistry<object> {
	constructor();
	protected getSecondaryTypeKey(type: any): string;
	protected isMatchingType(type: any): boolean;
	protected loadError(key: string): void;
}
export declare const EnumTypeRegistry: EnumTypeRegistryImpl;
export type FormatterType = ({
	new (props?: any): Formatter;
});
declare class FormatterTypeRegistryImpl extends BaseTypeRegistry<FormatterType> {
	constructor();
	protected isMatchingType(type: any): boolean;
	protected loadError(key: string): void;
}
export declare const FormatterTypeRegistry: FormatterTypeRegistryImpl;
export declare namespace ReflectionUtils {
	function getPropertyValue(o: any, property: string): any;
	function setPropertyValue(o: any, property: string, value: any): void;
	function makeCamelCase(s: string): string;
}
export interface ToolButtonProps {
	action?: string;
	title?: string | HTMLElement | SVGElement | DocumentFragment;
	hint?: string;
	cssClass?: string;
	icon?: IconClassName;
	onClick?: (e: MouseEvent & {
		currentTarget: EventTarget & HTMLElement;
	}) => void;
	ref?: (el: HTMLElement) => void;
	visible?: boolean | (() => boolean);
	disabled?: boolean | (() => boolean);
}
export interface ToolButton extends ToolButtonProps {
	hotkey?: string;
	hotkeyAllowDefault?: boolean;
	hotkeyContext?: any;
	separator?: (false | true | "left" | "right" | "both");
}
export declare function ToolbarButton(tb: ToolButtonProps): HTMLElement;
export interface ToolbarOptions {
	buttons?: ToolButton[];
	hotkeyContext?: any;
}
export declare class Toolbar<P extends ToolbarOptions = ToolbarOptions> extends Widget<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	protected renderContents(): any;
	destroy(): void;
	protected mouseTrap: any;
	createButton(container: ParentNode | ArrayLike<ParentNode>, tb: ToolButton): HTMLElement;
	findButton(className: string): Fluent<HTMLElement>;
	updateInterface(): void;
}
export declare class BaseDialog<P> extends Widget<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLDivElement;
	protected tabs: Fluent<HTMLElement>;
	protected toolbar: Toolbar;
	protected validator: any;
	protected dialog: Dialog;
	constructor(props?: WidgetProps<P>);
	destroy(): void;
	protected addCssClass(): void;
	protected getInitialDialogTitle(): string;
	protected isStaticPanel(): boolean;
	protected getDialogOptions(): DialogOptions;
	protected initDialog(): void;
	protected initUIDialog(): void;
	dialogOpen(asPanel?: boolean): void;
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
	protected getToolbarButtons(): ToolButton[];
	protected initToolbar(): void;
	protected getValidatorOptions(): any;
	protected initValidator(): void;
	protected resetValidation(): void;
	protected validateForm(): boolean;
	arrange(): void;
	protected onDialogClose(result?: string): void;
	protected getDialogButtons(): DialogButton[];
	dialogClose(result?: string): void;
	get dialogTitle(): string;
	set dialogTitle(value: string);
	protected initTabs(): void;
	protected handleResponsive(): void;
}
/** @deprecated use BaseDialog */
export declare const TemplatedDialog: typeof BaseDialog;
export interface FilterLine {
	field?: string;
	operator?: string;
	isOr?: boolean;
	leftParen?: boolean;
	rightParen?: boolean;
	validationError?: string;
	criteria?: any[];
	displayText?: string;
	state?: any;
}
export declare class FilterStore {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor(fields: PropertyItem[]);
	static getCriteriaFor(items: FilterLine[]): any[];
	static getDisplayTextFor(items: FilterLine[]): string;
	private changed;
	private displayText;
	private fields;
	private fieldByName;
	private items;
	get_fields(): PropertyItem[];
	get_fieldByName(): {
		[key: string]: PropertyItem;
	};
	get_items(): FilterLine[];
	raiseChanged(): void;
	add_changed(value: (e: Event, a: any) => void): void;
	remove_changed(value: (e: Event, a: any) => void): void;
	get_activeCriteria(): any[];
	get_displayText(): string;
}
export declare function delegateCombine(delegate1: any, delegate2: any): any;
export declare function delegateRemove(delegate1: any, delegate2: any): any;
export declare function delegateContains(targets: any[], object: any, method: any): boolean;
export interface IDataGrid {
	getElement(): HTMLElement;
	getGrid(): Grid;
	getView(): IRemoteView<any>;
	getFilterStore(): FilterStore;
}
export declare class ColumnPickerDialog<P = {}> extends BaseDialog<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private ulVisible;
	private ulHidden;
	private colById;
	allColumns: Column[];
	visibleColumns: string[];
	defaultColumns: string[];
	done: () => void;
	protected renderContents(): any;
	static createToolButton(grid: IDataGrid): ToolButton;
	protected getDialogButtons(): DialogButton[];
	protected getDialogOptions(): DialogOptions;
	private getTitle;
	private allowHide;
	private createLI;
	private updateListStates;
	protected setupColumns(): void;
	protected onDialogOpen(): void;
}
export type EditorProps<T> = WidgetProps<T> & {
	initialValue?: any;
	maxLength?: number;
	name?: string;
	placeholder?: string;
	required?: boolean;
	readOnly?: boolean;
};
export declare class EditorWidget<P> extends Widget<EditorProps<P>> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor(props: EditorProps<P>);
	get readOnly(): boolean;
	set readOnly(value: boolean);
}
export interface DateEditorOptions {
	yearRange?: string;
	minValue?: string;
	maxValue?: string;
	sqlMinMax?: boolean;
}
export declare class DateEditor<P extends DateEditorOptions = DateEditorOptions> extends EditorWidget<P> implements IStringValue, IReadOnly {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	readonly domNode: HTMLInputElement;
	constructor(props: EditorProps<P>);
	setToToday(triggerChange?: boolean): void;
	destroy(): void;
	get_value(): string;
	get value(): string;
	set_value(value: string): void;
	set value(v: string);
	private get_valueAsDate;
	get valueAsDate(): Date;
	private set_valueAsDate;
	set valueAsDate(v: Date);
	get_readOnly(): boolean;
	set_readOnly(value: boolean): void;
	get_minValue(): string;
	set_minValue(value: string): void;
	get_maxValue(): string;
	set_maxValue(value: string): void;
	get_minDate(): Date;
	set_minDate(value: Date): void;
	get_maxDate(): Date;
	set_maxDate(value: Date): void;
	get_sqlMinMax(): boolean;
	set_sqlMinMax(value: boolean): void;
	static dateInputChange: (e: Event) => void;
	static dateInputKeyup(e: KeyboardEvent): void;
	static useFlatpickr: boolean;
	getFlatpickrOptions(input: HTMLElement): any;
	createFlatPickrTrigger(): HTMLElement;
	static uiPickerZIndexWorkaround(el: HTMLElement | ArrayLike<HTMLElement>): void;
}
export type ComboboxType = "select2";
export type ComboboxFormatResult = string | Element | DocumentFragment;
export interface ComboboxItem<TSource = any> {
	id?: string;
	text?: string;
	source?: TSource;
	disabled?: boolean;
}
export interface ComboboxSearchQuery {
	searchTerm?: string;
	idList?: string[];
	skip?: number;
	take?: number;
	checkMore?: boolean;
	initSelection?: boolean;
	signal?: AbortSignal;
}
export interface ComboboxSearchResult<TItem> {
	items: TItem[];
	more: boolean;
}
export interface ComboboxOptions<TSource = any> {
	allowClear?: boolean;
	createSearchChoice?: (s: string) => ComboboxItem<TSource>;
	element?: HTMLInputElement | HTMLSelectElement | Element[];
	/** Allow arbitrary values for items */
	arbitraryValues?: boolean;
	formatSelection?: (p1: ComboboxItem<TSource>) => ComboboxFormatResult;
	formatResult?: (p1: ComboboxItem<TSource>) => ComboboxFormatResult;
	minimumResultsForSearch?: number;
	multiple?: boolean;
	/** Page size to use while loading or displaying results */
	pageSize?: number;
	placeholder?: string;
	/** Callback to get options specific to the combobox provider type */
	providerOptions?: (type: ComboboxType, opt: ComboboxOptions) => any;
	search?: (query: ComboboxSearchQuery) => (PromiseLike<ComboboxSearchResult<ComboboxItem<TSource>>> | ComboboxSearchResult<ComboboxItem<TSource>>);
	/** Type delay for searching, default is 200 */
	typeDelay?: number;
}
export declare class Combobox<TItem = any> {
	private el;
	static defaults: ComboboxOptions;
	constructor(opt: ComboboxOptions);
	private createSelect2;
	abortPendingQuery(): void;
	abortInitSelection(): void;
	dispose(): void;
	get container(): HTMLElement;
	get type(): ComboboxType;
	get isMultiple(): boolean;
	getSelectedItem(): ComboboxItem;
	getSelectedItems(): ComboboxItem[];
	getValue(): string;
	getValues(): string[];
	setValue(value: string, triggerChange?: boolean): void;
	setValues(value: string[], triggerChange?: boolean): void;
	closeDropdown(): void;
	openDropdown(): void;
	static getInstance(el: Element | ArrayLike<Element>): Combobox;
}
export declare function stripDiacritics(str: string): string;
export declare class CascadedWidgetLink<TParent extends Widget<any>> {
	private parentType;
	private widget;
	private parentChange;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor(parentType: {
		new (...args: any[]): TParent;
	}, widget: Widget<any>, parentChange: (p1: TParent) => void);
	private _parentID;
	bind(): TParent;
	unbind(): TParent;
	get_parentID(): string;
	set_parentID(value: string): void;
}
export interface ComboboxCommonOptions {
	allowClear?: boolean;
	delimited?: boolean;
	minimumResultsForSearch?: any;
	multiple?: boolean;
}
export interface ComboboxFilterOptions {
	cascadeFrom?: string;
	cascadeField?: string;
	cascadeValue?: any;
	filterField?: string;
	filterValue?: any;
}
export interface ComboboxInplaceAddOptions {
	inplaceAdd?: boolean;
	inplaceAddPermission?: string;
	dialogType?: string;
	autoComplete?: boolean;
}
export interface ComboboxEditorOptions extends ComboboxFilterOptions, ComboboxInplaceAddOptions, ComboboxCommonOptions {
}
export declare class ComboboxEditor<P, TItem> extends EditorWidget<P> implements ISetEditValue, IGetEditValue, IStringValue, IReadOnly {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	readonly domNode: HTMLInputElement;
	private combobox;
	private _items;
	private _itemById;
	protected lastCreateTerm: string;
	constructor(props: EditorProps<P>);
	destroy(): void;
	protected hasAsyncSource(): boolean;
	protected asyncSearch(query: ComboboxSearchQuery): PromiseLike<ComboboxSearchResult<TItem>>;
	protected getTypeDelay(): any;
	protected emptyItemText(): string;
	protected getPageSize(): number;
	protected getIdField(): any;
	protected itemId(item: TItem): string;
	protected getTextField(): any;
	protected itemText(item: TItem): string;
	protected itemDisabled(item: TItem): boolean;
	protected mapItem(item: TItem): ComboboxItem;
	protected mapItems(items: TItem[]): ComboboxItem[];
	protected allowClear(): boolean;
	protected isMultiple(): boolean;
	protected abortPendingQuery(): void;
	protected getComboboxOptions(): ComboboxOptions;
	get_delimited(): boolean;
	get items(): ComboboxItem<TItem>[];
	set items(value: ComboboxItem<TItem>[]);
	protected get itemById(): {
		[key: string]: ComboboxItem<TItem>;
	};
	protected set itemById(value: {
		[key: string]: ComboboxItem<TItem>;
	});
	clearItems(): void;
	addItem(item: ComboboxItem<TItem>): void;
	addOption(key: string, text: string, source?: any, disabled?: boolean): void;
	protected addInplaceCreate(addTitle: string, editTitle: string): void;
	protected useInplaceAdd(): boolean;
	protected isAutoComplete(): boolean;
	getCreateSearchChoice(getName: (z: any) => string): (s: string) => {
		id: string;
		text: string;
	};
	setEditValue(source: any, property: PropertyItem): void;
	getEditValue(property: PropertyItem, target: any): void;
	protected getComboboxContainer(): HTMLElement;
	protected get_items(): ComboboxItem<TItem>[];
	protected get_itemByKey(): {
		[key: string]: ComboboxItem<TItem>;
	};
	static filterByText<TItem>(items: TItem[], getText: (item: TItem) => string, term: string): TItem[];
	get_value(): string;
	get value(): string;
	set_value(value: string): void;
	set value(v: string);
	get selectedItem(): TItem;
	get selectedItems(): TItem[];
	protected get_values(): string[];
	get values(): string[];
	protected set_values(value: string[]): void;
	set values(value: string[]);
	protected get_text(): string;
	get text(): string;
	get_readOnly(): boolean;
	private updateInplaceReadOnly;
	set_readOnly(value: boolean): void;
	protected getCascadeFromValue(parent: Widget<any>): any;
	protected cascadeLink: CascadedWidgetLink<Widget<any>>;
	protected setCascadeFrom(value: string): void;
	protected get_cascadeFrom(): string;
	get cascadeFrom(): string;
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
	protected cascadeItems(items: TItem[]): TItem[];
	protected filterItems(items: TItem[]): TItem[];
	protected updateItems(): void;
	protected getDialogTypeKey(): string;
	protected createEditDialog(callback: (dlg: IEditDialog) => void): void;
	onInitNewEntity: (entity: TItem) => void;
	protected initNewEntity(entity: TItem): void;
	protected setEditDialogReadOnly(dialog: any): void;
	protected editDialogDataChange(): void;
	protected setTermOnNewEntity(entity: TItem, term: string, dialog: any): void;
	protected inplaceCreateClick(e: Event): void;
	openDropdown(): void;
	openDialogAsPanel: boolean;
}
export declare class SelectEditor<P extends SelectEditorOptions = SelectEditorOptions> extends ComboboxEditor<P, ComboboxItem> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	constructor(props: EditorProps<P>);
	getItems(): any[];
	protected emptyItemText(): string;
	updateItems(): void;
}
export interface SelectEditorOptions extends ComboboxCommonOptions {
	items?: any[];
	emptyOptionText?: string;
}
export declare class FilterWidgetBase<P = {}> extends Widget<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private store;
	private onFilterStoreChanged;
	constructor(props: WidgetProps<P>);
	destroy(): void;
	protected filterStoreChanged(): void;
	get_store(): FilterStore;
	set_store(value: FilterStore): void;
}
export declare class FilterDisplayBar<P = {}> extends FilterWidgetBase<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	protected renderContents(): any;
	protected filterStoreChanged(): void;
}
export interface QuickSearchField {
	name: string;
	title: string;
}
export interface QuickSearchInputOptions {
	typeDelay?: number;
	loadingParentClass?: string;
	filteredParentClass?: string;
	onSearch?: (p1: string, p2: string, p3: (p1: boolean) => void) => void;
	fields?: QuickSearchField[];
}
export declare class QuickSearchInput<P extends QuickSearchInputOptions = QuickSearchInputOptions> extends Widget<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	readonly domNode: HTMLInputElement;
	private lastValue;
	private field;
	private fieldLink;
	private fieldChanged;
	private timer;
	constructor(props: WidgetProps<P>);
	protected checkIfValueChanged(): void;
	get_value(): string;
	get_field(): QuickSearchField;
	set_field(value: QuickSearchField): void;
	protected updateInputPlaceHolder(): void;
	restoreState(value: string, field: QuickSearchField): void;
	protected searchNow(value: string): void;
}
export interface SettingStorage {
	getItem(key: string): string | Promise<string>;
	setItem(key: string, value: string): void | Promise<void>;
}
export interface PersistedGridColumn {
	id: string;
	width?: number;
	sort?: number;
	visible?: boolean;
}
export interface PersistedGridSettings {
	columns?: PersistedGridColumn[];
	filterItems?: FilterLine[];
	quickFilters?: {
		[key: string]: any;
	};
	quickFilterText?: string;
	quickSearchField?: QuickSearchField;
	quickSearchText?: string;
	includeDeleted?: boolean;
}
export interface GridPersistanceFlags {
	columnWidths?: boolean;
	columnVisibility?: boolean;
	sortColumns?: boolean;
	filterItems?: boolean;
	quickFilters?: boolean;
	quickFilterText?: boolean;
	quickSearch?: boolean;
	includeDeleted?: boolean;
}
export declare const omitAllGridPersistenceFlags: GridPersistanceFlags;
export interface IRowDefinition {
	readonly deletePermission?: string;
	readonly idProperty?: string;
	readonly insertPermission?: string;
	readonly isActiveProperty?: string;
	readonly isDeletedProperty?: string;
	readonly localTextPrefix?: string;
	readonly nameProperty?: string;
	readonly readPermission?: string;
	readonly updatePermission?: string;
}
export interface QuickFilterArgs<TWidget> {
	field?: string;
	widget?: TWidget;
	request?: ListRequest;
	equalityFilter?: any;
	value?: any;
	active?: boolean;
	handled?: boolean;
}
export interface QuickFilter<TWidget extends Widget<P>, P> {
	field?: string;
	type?: {
		new (options?: P): TWidget;
		prototype: TWidget;
	};
	handler?: (h: QuickFilterArgs<TWidget>) => void;
	title?: string;
	options?: P & WidgetProps<{}>;
	element?: (e: Fluent) => void;
	init?: (w: TWidget) => void;
	separator?: boolean;
	cssClass?: string;
	loadState?: (w: TWidget, state: any) => void;
	saveState?: (w: TWidget) => any;
	displayText?: (w: TWidget, label: string) => string;
}
export declare class DateTimeEditor<P extends DateTimeEditorOptions = DateTimeEditorOptions> extends EditorWidget<P> implements IStringValue, IReadOnly {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	readonly domNode: HTMLInputElement;
	private time;
	private lastSetValue;
	private lastSetValueGet;
	constructor(props: EditorProps<P>);
	setToNow(triggerChange?: boolean): void;
	destroy(): void;
	getFlatpickrOptions(): any;
	createFlatPickrTrigger(): HTMLElement;
	get_value(): string;
	get value(): string;
	set_value(value: string): void;
	private getInplaceNowText;
	private getDisplayFormat;
	set value(v: string);
	private get_valueAsDate;
	get valueAsDate(): Date;
	private set_valueAsDate;
	set valueAsDate(value: Date);
	get_minValue(): string;
	set_minValue(value: string): void;
	get_maxValue(): string;
	set_maxValue(value: string): void;
	get_minDate(): Date;
	set_minDate(value: Date): void;
	get_maxDate(): Date;
	set_maxDate(value: Date): void;
	get_sqlMinMax(): boolean;
	set_sqlMinMax(value: boolean): void;
	get_readOnly(): boolean;
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
}
export interface QuickFilterBarOptions {
	filters: QuickFilter<Widget<any>, any>[];
	getTitle?: (filter: QuickFilter<Widget<any>, any>) => string;
	idPrefix?: string;
}
export declare class QuickFilterBar<P extends QuickFilterBarOptions = QuickFilterBarOptions> extends Widget<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor(props: WidgetProps<P>);
	addSeparator(): void;
	add<TWidget extends Widget<any>, TOptions>(opt: QuickFilter<TWidget, TOptions>): TWidget;
	addDateRange(field: string, title?: string): DateEditor;
	static dateRange(field: string, title?: string): QuickFilter<DateEditor, DateTimeEditorOptions>;
	addDateTimeRange(field: string, title?: string): DateTimeEditor;
	static dateTimeRange(field: string, title?: string, useUtc?: boolean): QuickFilter<DateTimeEditor, DateTimeEditorOptions>;
	addBoolean(field: string, title?: string, yes?: string, no?: string): SelectEditor;
	static boolean(field: string, title?: string, yes?: string, no?: string): QuickFilter<SelectEditor, SelectEditorOptions>;
	onChange: (e: Event) => void;
	private submitHandlers;
	destroy(): void;
	onSubmit(request: ListRequest): void;
	protected add_submitHandlers(action: (request: ListRequest) => void): void;
	protected remove_submitHandlers(action: (request: ListRequest) => void): void;
	protected clear_submitHandlers(): void;
	find<TWidget>(type: {
		new (...args: any[]): TWidget;
	}, field: string): TWidget;
	tryFind<TWidget>(type: {
		new (...args: any[]): TWidget;
	}, field: string): TWidget;
}
export declare class DataGrid<TItem, P = {}> extends Widget<P> implements IDataGrid, IReadOnly {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private _isDisabled;
	private _layoutTimer;
	protected titleDiv: Fluent;
	protected toolbar: Toolbar;
	protected filterBar: FilterDisplayBar;
	protected quickFiltersDiv: Fluent;
	protected quickFiltersBar: QuickFilterBar;
	protected slickContainer: Fluent;
	protected allColumns: Column[];
	protected propertyItemsData: PropertyItemsData;
	protected initialSettings: PersistedGridSettings;
	protected restoringSettings: number;
	view: IRemoteView<TItem>;
	slickGrid: Grid;
	openDialogsAsPanel: boolean;
	static defaultRowHeight: number;
	static defaultHeaderHeight: number;
	static defaultPersistanceStorage: SettingStorage;
	static defaultColumnWidthScale: number;
	static defaultColumnWidthDelta: number;
	constructor(props: WidgetProps<P>);
	protected propertyItemsReady(itemsData: PropertyItemsData): void;
	protected afterInit(): void;
	protected useAsync(): boolean;
	protected useLayoutTimer(): boolean;
	protected layout(): void;
	protected getInitialTitle(): string;
	protected createToolbarExtensions(): void;
	protected ensureQuickFilterBar(): QuickFilterBar;
	protected createQuickFilters(filters?: QuickFilter<Widget<any>, any>[]): void;
	protected getQuickFilters(): QuickFilter<Widget<any>, any>[];
	static propertyItemToQuickFilter(item: PropertyItem): QuickFilter<any, any> | null;
	protected findQuickFilter<TWidget>(type: {
		new (...args: any[]): TWidget;
	}, field: string): TWidget;
	protected tryFindQuickFilter<TWidget>(type: {
		new (...args: any[]): TWidget;
	}, field: string): TWidget;
	protected createIncludeDeletedButton(): void;
	protected getQuickSearchFields(): QuickSearchField[];
	protected createQuickSearchInput(): void;
	destroy(): void;
	protected getItemCssClass(item: TItem, index: number): string;
	protected getItemMetadata(item: TItem, index: number): any;
	protected postProcessColumns(columns: Column[]): Column[];
	protected getColumnWidthDelta(): number;
	protected getColumnWidthScale(): number;
	protected initialPopulate(): void;
	protected canFilterColumn(column: Column): boolean;
	protected initializeFilterBar(): void;
	protected createSlickGrid(): Grid;
	protected setInitialSortOrder(): void;
	itemAt(row: number): TItem;
	itemId(item: TItem): any;
	rowCount(): number;
	getItems(): TItem[];
	setItems(value: TItem[]): void;
	protected bindToSlickEvents(): void;
	protected getAddButtonCaption(): string;
	protected getButtons(): ToolButton[];
	protected editItem(entityOrId: any): void;
	protected editItemOfType(itemType: string, entityOrId: any): void;
	protected onClick(e: Event, row: number, cell: number): void;
	protected viewDataChanged(e: any, rows: TItem[]): void;
	protected bindToViewEvents(): void;
	protected onViewProcessData(response: ListResponse<TItem>): ListResponse<TItem>;
	protected onViewFilter(item: TItem): boolean;
	protected getIncludeColumns(include: {
		[key: string]: boolean;
	}): void;
	protected setCriteriaParameter(): void;
	protected setEquality(field: string, value: any): void;
	protected setIncludeColumnsParameter(): void;
	protected onViewSubmit(): boolean;
	protected markupReady(): void;
	protected createSlickContainer(): Fluent;
	protected createView(): IRemoteView<TItem>;
	protected getDefaultSortBy(): any[];
	protected usePager(): boolean;
	protected enableFiltering(): boolean;
	protected populateWhenVisible(): boolean;
	protected createFilterBar(): void;
	protected getPagerOptions(): PagerOptions;
	protected createPager(): void;
	protected getViewOptions(): RemoteViewOptions<any>;
	protected createToolbar(buttons: ToolButton[]): void;
	getTitle(): string;
	setTitle(value: string): void;
	protected getItemType(): string;
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
		/** Tab index to add to the link element. Optional. */
		tabIndex?: number;
		/** The link text. If not provided it will be taken from ctx.escape(ctx.value) */
		children?: any;
	}) => any;
	protected getColumnsKey(): string;
	protected getPropertyItems(): PropertyItem[];
	protected getPropertyItemsData(): PropertyItemsData;
	protected getPropertyItemsDataAsync(): Promise<PropertyItemsData>;
	protected getColumns(): Column<TItem>[];
	protected wrapFormatterWithEditLink(column: Column, item: PropertyItem): void;
	protected propertyItemsToSlickColumns(propertyItems: PropertyItem[]): Column[];
	protected getSlickOptions(): GridOptions;
	protected populateLock(): void;
	protected populateUnlock(): void;
	protected getGridCanLoad(): boolean;
	refresh(): void;
	protected refreshIfNeeded(): void;
	protected internalRefresh(): void;
	setIsDisabled(value: boolean): void;
	private _readonly;
	get readOnly(): boolean;
	set readOnly(value: boolean);
	get_readOnly(): boolean;
	set_readOnly(value: boolean): void;
	updateInterface(): void;
	protected getRowDefinition(): IRowDefinition;
	private _localTextDbPrefix;
	protected getLocalTextDbPrefix(): string;
	protected getLocalTextPrefix(): string;
	private _idProperty;
	protected getIdProperty(): string;
	protected getIsDeletedProperty(): string;
	private _isActiveProperty;
	protected getIsActiveProperty(): string;
	protected updateDisabledState(): void;
	protected resizeCanvas(): void;
	protected subDialogDataChange(): void;
	protected addFilterSeparator(): void;
	protected determineText(getKey: (prefix: string) => string): string;
	protected addQuickFilter<TWidget extends Widget<any>, P>(opt: QuickFilter<TWidget, P>): TWidget;
	protected addDateRangeFilter(field: string, title?: string): DateEditor;
	protected dateRangeQuickFilter(field: string, title?: string): QuickFilter<DateEditor<DateEditorOptions>, DateTimeEditorOptions>;
	protected addDateTimeRangeFilter(field: string, title?: string): DateTimeEditor<DateTimeEditorOptions>;
	protected dateTimeRangeQuickFilter(field: string, title?: string): QuickFilter<DateTimeEditor<DateTimeEditorOptions>, DateTimeEditorOptions>;
	protected addBooleanFilter(field: string, title?: string, yes?: string, no?: string): SelectEditor;
	protected booleanQuickFilter(field: string, title?: string, yes?: string, no?: string): QuickFilter<SelectEditor<SelectEditorOptions>, SelectEditorOptions>;
	protected invokeSubmitHandlers(): void;
	protected quickFilterChange(e: Event): void;
	protected getPersistanceStorage(): SettingStorage;
	protected getPersistanceKey(): string;
	protected gridPersistanceFlags(): GridPersistanceFlags;
	protected canShowColumn(column: Column): boolean;
	protected getPersistedSettings(): PersistedGridSettings | Promise<PersistedGridSettings>;
	protected restoreSettings(settings?: PersistedGridSettings, flags?: GridPersistanceFlags): void | Promise<void>;
	protected restoreSettingsFrom(settings: PersistedGridSettings, flags?: GridPersistanceFlags): void;
	persistSettings(flags?: GridPersistanceFlags): void | Promise<void>;
	getCurrentSettings(flags?: GridPersistanceFlags): PersistedGridSettings;
	getElement(): HTMLElement;
	getGrid(): Grid;
	getView(): IRemoteView<TItem>;
	getFilterStore(): FilterStore;
}
export declare class EntityGrid<TItem, P = {}> extends DataGrid<TItem, P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor(props: WidgetProps<P>);
	destroy(): void;
	protected handleRoute(e: HandleRouteEvent): void;
	protected usePager(): boolean;
	protected createToolbarExtensions(): void;
	protected getInitialTitle(): string;
	protected getLocalTextPrefix(): string;
	private _entityType;
	protected getEntityType(): string;
	private _displayName;
	protected getDisplayName(): string;
	private _itemName;
	protected getItemName(): string;
	protected getAddButtonCaption(): string;
	protected getButtons(): ToolButton[];
	protected newRefreshButton(noText?: boolean): ToolButton;
	protected addButtonClick(): void;
	protected editItem(entityOrId: any): void;
	protected editItemOfType(itemType: string, entityOrId: any): void;
	private _service;
	protected getService(): string;
	protected getServiceMethod(): string;
	protected getServiceUrl(): string;
	protected getViewOptions(): RemoteViewOptions;
	protected getItemType(): string;
	protected routeDialog(itemType: string, dialog: Widget<any>): void;
	protected getInsertPermission(): string;
	protected hasInsertPermission(): boolean;
	protected transferDialogReadOnly(dialog: Widget<any>): void;
	protected initDialog(dialog: Widget<any>): void;
	protected initEntityDialog(itemType: string, dialog: Widget<any>): void;
	protected createEntityDialog(itemType: string, callback?: (dlg: Widget<any>) => void): (Widget<any> | PromiseLike<Widget<any>>);
	protected getDialogOptions(): any;
	protected getDialogOptionsFor(itemType: string): any;
	protected getDialogTypeFor(itemType: string): DialogType | PromiseLike<DialogType>;
	private _dialogType;
	protected getDialogType(): DialogType | PromiseLike<DialogType>;
}
export declare class SlickPager<P extends PagerOptions = PagerOptions> extends Widget<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private currentPage;
	private totalPages;
	private pageSize;
	private stat;
	constructor(props: WidgetProps<P>);
	_changePage(ctype: string): boolean;
	_updatePager(): void;
}
/**
 * A mixin that can be applied to a DataGrid for tree functionality
 */
export declare class TreeGridMixin<TItem> {
	private options;
	private dataGrid;
	constructor(options: TreeGridMixinOptions<TItem>);
	/**
	 * Expands / collapses all rows in a grid automatically
	 */
	toggleAll(): void;
	collapseAll(): void;
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
export interface TreeGridMixinOptions<TItem> {
	grid: DataGrid<TItem, any>;
	getParentId: (item: TItem) => any;
	toggleField: string;
	initialCollapse?: () => boolean;
}
export declare namespace DialogExtensions {
	function dialogResizable(dialog: HTMLElement | ArrayLike<HTMLElement>, w?: any, h?: any, mw?: any, mh?: any): void;
	function dialogMaximizable(dialog: HTMLElement | ArrayLike<HTMLElement>): void;
}
export type PropertyFieldElement = HTMLElement & {
	editorWidget?: Widget<any>;
	editorPromise?: PromiseLike<void>;
	propertyItem?: PropertyItem;
};
export declare function PropertyFieldCaption(props: {
	item: Pick<PropertyItem, "name" | "hint" | "labelWidth" | "required" | "title">;
	idPrefix?: string;
	localTextPrefix?: string;
}): HTMLLabelElement;
export declare function PropertyFieldEditor(props: {
	fieldElement: PropertyFieldElement;
	item: Pick<PropertyItem, "editorCssClass" | "editorType" | "editorParams" | "maxLength" | "name" | "editorAddons" | "placeholder">;
	idPrefix?: string;
	localTextPrefix?: string;
}): void;
export declare function PropertyFieldLineBreak(props: {
	item: Pick<PropertyItem, "formCssClass">;
}): HTMLElement;
export declare function PropertyField(props: {
	item: PropertyItem;
	container?: ParentNode;
	idPrefix?: string;
	localTextPrefix?: string;
}): PropertyFieldElement;
export declare function PropertyCategoryTitle(props: {
	category: string;
	localTextPrefix: string;
}): HTMLElement;
export declare function PropertyCategory(props: {
	category?: string;
	children?: any;
	collapsed?: boolean;
	localTextPrefix?: string;
}): HTMLElement;
export declare function PropertyTabItem(props: {
	title: string;
	active?: boolean;
	paneId?: string;
	localTextPrefix?: string;
}): HTMLLIElement;
export declare function PropertyTabPane(props: {
	active?: boolean;
	id?: string;
	children?: any;
}): HTMLElement;
export declare function PropertyCategories(props: {
	items: PropertyItem[];
	container?: ParentNode;
	fieldElements?: PropertyFieldElement[];
	idPrefix?: string;
	localTextPrefix?: string;
}): HTMLElement;
export declare function PropertyTabList(props?: {
	children?: any;
}): HTMLElement;
export declare function PropertyTabPanes(_?: {}): HTMLElement;
export declare function PropertyTabs(props: {
	items: PropertyItem[];
	container?: ParentNode;
	fieldElements?: PropertyFieldElement[];
	idPrefix?: string;
	localTextPrefix?: string;
	paneIdPrefix?: string;
}): DocumentFragment | null;
export declare class PropertyGrid<P extends PropertyGridOptions = PropertyGridOptions> extends Widget<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private fieldElements;
	protected renderContents(): any;
	destroy(): void;
	get_editors(): Widget<any>[];
	get_items(): PropertyItem[];
	get_idPrefix(): string;
	enumerateItems(callback: (p1: PropertyItem, p2: Widget<any>) => void): void;
	get_mode(): PropertyGridMode;
	set_mode(value: PropertyGridMode): void;
	static loadFieldValue(source: any, fieldElement: PropertyFieldElement, mode?: PropertyGridMode): void;
	load(source: any): void;
	static saveFieldValue(target: any, fieldElement: PropertyFieldElement, canModify?: boolean): void;
	save(target?: any): any;
	get value(): any;
	set value(val: any);
	static canModifyItem(item: PropertyItem, mode?: PropertyGridMode): boolean;
	protected canModifyItem(item: PropertyItem): boolean;
	static updateFieldElement(fieldElement: PropertyFieldElement, mode?: PropertyGridMode, canModify?: boolean): void;
	protected updateFieldElement(fieldElement: PropertyFieldElement): void;
	updateInterface(): void;
}
export declare enum PropertyGridMode {
	insert = 1,
	update = 2
}
export interface PropertyGridOptions {
	idPrefix?: string;
	items: PropertyItem[];
	localTextPrefix?: string;
	value?: any;
	mode?: PropertyGridMode;
}
export interface EntityLocalizerOptions {
	byId: (id: string) => Fluent;
	idPrefix: string;
	isNew: () => boolean;
	getButton: () => Fluent;
	getEntity: () => any;
	getLanguages: () => LanguageList;
	getPropertyGrid: () => Fluent;
	getToolButtons: () => HTMLElement[];
	pgOptions: PropertyGridOptions;
	retrieveLocalizations: () => PromiseLike<{
		[languageId: string]: any;
	}>;
	validateForm: () => boolean;
}
export declare class EntityLocalizer {
	protected grid: PropertyGrid;
	protected pendingValue: any;
	protected lastValue: any;
	protected targetLanguage: HTMLSelectElement;
	private options;
	constructor(opt: EntityLocalizerOptions);
	destroy(): void;
	clearValue(): void;
	isEnabled(): boolean;
	protected isLocalizationMode(): boolean;
	protected isLocalizationModeAndChanged(): boolean;
	buttonClick(): void;
	protected loadLocalization(): void;
	protected setLocalizationGridCurrentValues(): void;
	protected getLocalizationGridValue(): any;
	editSaveRequest(req: SaveRequest<any>): void;
	protected getPendingLocalizations(): any;
	updateInterface(): void;
}
export type SaveInitiator = "save-and-close" | "apply-changes";
export declare function saveAndCloseToolButton(opt?: ToolButton): ToolButton;
export declare function applyChangesToolButton(opt?: ToolButton): ToolButton;
export declare function deleteToolButton(opt?: ToolButton): ToolButton;
export declare function undeleteToolButton(opt?: ToolButton): ToolButton;
export declare function editToolButton(opt?: ToolButton): ToolButton;
export declare function localizationToolButton(opt?: ToolButton): ToolButton;
export declare function cloneToolButton(opt?: ToolButton): ToolButton;
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
	constructor(props?: WidgetProps<P>);
	protected propertyItemsReady(itemsData: PropertyItemsData): void;
	protected afterInit(): void;
	protected useAsync(): boolean;
	destroy(): void;
	get entity(): TItem;
	protected set entity(value: TItem);
	/** @deprecated use entityId */
	protected get_entityId(): any;
	get entityId(): any;
	protected set entityId(value: any);
	protected getEntityNameFieldValue(): any;
	protected getEntityTitle(): string;
	protected updateTitle(): void;
	protected isCloneMode(): boolean;
	protected isEditMode(): boolean;
	protected isDeleted(): boolean;
	protected isNew(): boolean;
	protected isNewOrDeleted(): boolean;
	protected getDeleteOptions(callback: (response: DeleteResponse) => void): ServiceOptions<DeleteResponse>;
	protected deleteHandler(options: ServiceOptions<DeleteResponse>, callback: (response: DeleteResponse) => void): void;
	protected getDeleteServiceMethod(): string;
	protected doDelete(callback: (response: DeleteResponse) => void): void;
	protected onDeleteSuccess(response: DeleteResponse): void;
	protected getRowDefinition(): IRowDefinition;
	private _entityType;
	protected getEntityType(): string;
	private _formKey;
	protected getFormKey(): string;
	private _localTextDbPrefix;
	protected getLocalTextDbPrefix(): string;
	protected getLocalTextPrefix(): string;
	private _entitySingular;
	protected getEntitySingular(): string;
	private _nameProperty;
	protected getNameProperty(): string;
	private _idProperty;
	protected getIdProperty(): string;
	private _isActiveProperty;
	protected getIsActiveProperty(): string;
	protected getIsDeletedProperty(): string;
	private _service;
	protected getService(): string;
	load(entityOrId: any, done: () => void, fail?: (ex: any) => void): void;
	loadNewAndOpenDialog(asPanel?: boolean): void;
	loadEntityAndOpenDialog(entity: TItem, asPanel?: boolean): void;
	protected loadResponse(data: any): void;
	protected loadEntity(entity: TItem): void;
	protected beforeLoadEntity(entity: TItem): void;
	protected afterLoadEntity(): void;
	loadByIdAndOpenDialog(entityId: any, asPanel?: boolean, callback?: (response: RetrieveResponse<TItem>) => void, fail?: () => void): void;
	protected onLoadingData(data: RetrieveResponse<TItem>): void;
	protected getLoadByIdOptions(id: any, callback: (response: RetrieveResponse<TItem>) => void): ServiceOptions<RetrieveResponse<TItem>>;
	protected getLoadByIdRequest(id: any): RetrieveRequest;
	protected reloadById(): void;
	protected getRetrieveServiceMethod(): string;
	loadById(id: any, callback?: (response: RetrieveResponse<TItem>) => void, fail?: () => void): void;
	protected loadByIdHandler(options: ServiceOptions<RetrieveResponse<TItem>>, callback: (response: RetrieveResponse<TItem>) => void, fail: () => void): void;
	protected retrieveLocalizations(): Promise<Record<string, Partial<TItem>>>;
	protected getLocalizerOptions(): EntityLocalizerOptions;
	protected initLocalizer(): void;
	protected getLanguages(): LanguageList;
	protected initPropertyGrid(): void;
	protected getPropertyItems(): PropertyItem[];
	protected getPropertyItemsData(): PropertyItemsData;
	protected getPropertyItemsDataAsync(): Promise<PropertyItemsData>;
	protected getPropertyGridOptions(): PropertyGridOptions;
	protected validateBeforeSave(): boolean;
	protected getCreateServiceMethod(): string;
	protected getUpdateServiceMethod(): string;
	protected getSaveOptions(callback: (response: SaveResponse) => void, initiator?: SaveInitiator): ServiceOptions<SaveResponse>;
	protected getSaveEntity(): TItem;
	protected getSaveRequest(): SaveRequest<TItem>;
	protected onSaveSuccess(response: SaveResponse, initiator?: SaveInitiator): void;
	protected save_submitHandler(callback: (response: SaveResponse) => void, initiator: SaveInitiator): void;
	protected save(callback?: (response: SaveResponse) => void, initiator?: SaveInitiator): void | boolean;
	protected saveHandler(options: ServiceOptions<SaveResponse>, callback: (response: SaveResponse) => void, initiator: SaveInitiator): void;
	protected showSaveSuccessMessage(response: SaveResponse, initiator?: SaveInitiator): void;
	protected getToolbarButtons(): ToolButton[];
	protected getCloningEntity(): TItem;
	protected updateInterface(): void;
	protected getUndeleteOptions(callback?: (response: UndeleteResponse) => void): ServiceOptions<UndeleteResponse>;
	protected undeleteHandler(options: ServiceOptions<UndeleteResponse>, callback: (response: UndeleteResponse) => void): void;
	protected getUndeleteServiceMethod(): string;
	protected undelete(callback?: (response: UndeleteResponse) => void): void;
	private _readonly;
	get readOnly(): boolean;
	set readOnly(value: boolean);
	get_readOnly(): boolean;
	set_readOnly(value: boolean): void;
	protected getInsertPermission(): string;
	protected getUpdatePermission(): string;
	protected getDeletePermission(): string;
	protected hasDeletePermission(): boolean;
	protected hasInsertPermission(): boolean;
	protected hasUpdatePermission(): boolean;
	protected hasSavePermission(): boolean;
	protected editClicked: boolean;
	protected isViewMode(): boolean;
	protected useViewMode(): boolean;
	protected renderContents(): any;
	static get defaultLanguageList(): string[][];
	static set defaultLanguageList(value: string[][]);
}
export declare class PropertyDialog<TItem, P> extends BaseDialog<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private _entity;
	private _entityId;
	protected propertyItemsData: PropertyItemsData;
	protected isClosable(): boolean;
	protected isStatic(): boolean;
	constructor(props?: WidgetProps<P>);
	protected propertyItemsReady(itemsData: PropertyItemsData): void;
	protected afterInit(): void;
	protected useAsync(): boolean;
	destroy(): void;
	protected getDialogOptions(): DialogOptions;
	protected getDialogButtons(): DialogButton[];
	protected okClick(): void;
	protected okClickValidated(): void;
	protected cancelClick(): void;
	protected initPropertyGrid(): void;
	protected getFormKey(): string;
	protected getPropertyGridOptions(): PropertyGridOptions;
	protected getPropertyItems(): PropertyItem[];
	protected getPropertyItemsData(): PropertyItemsData;
	protected getPropertyItemsDataAsync(): Promise<PropertyItemsData>;
	protected getSaveEntity(): TItem;
	protected loadInitialEntity(): void;
	get entity(): TItem;
	protected set entity(value: TItem);
	get entityId(): any;
	protected set entityId(value: any);
	protected validateBeforeSave(): boolean;
	protected updateTitle(): void;
	protected propertyGrid: PropertyGrid;
	protected renderContents(): any;
}
export interface AutoNumericOptions {
	aDec?: string;
	allowedAutoStrip?: RegExp;
	allowLeading?: boolean;
	altDec?: string;
	aForm?: boolean;
	aNum?: string;
	aNeg?: string;
	aSep?: string;
	aSign?: string;
	aNegRegAutoStrip?: string;
	aPad?: boolean;
	dGroup?: string;
	/** internal */
	holder?: any;
	lZero?: string;
	mDec?: number;
	mInt?: number;
	mRound?: string;
	nBracket?: string;
	numRegAutoStrip?: RegExp;
	oEvent?: any;
	pSign?: string;
	/** internal */
	runOnce?: boolean;
	skipFirstAutoStrip?: RegExp;
	skipLastAutoStrip?: RegExp;
	tagList?: string[];
	vMax?: any;
	vMin?: any;
	wEmpty?: string;
}
export declare class AutoNumeric {
	static init(input: HTMLInputElement, options: AutoNumericOptions): void;
	/** method to remove settings and stop autoNumeric() */
	static destroy(input: HTMLInputElement): void;
	/** method to update settings - can call as many times */
	static updateOptions(input: HTMLInputElement, options: AutoNumericOptions): void;
	/** returns a formatted strings for "input:text" fields Uses jQuery's .val() method*/
	static setValue(input: HTMLInputElement, valueIn: number | string): string;
	/** method to get the unformatted value from a specific input field, returns a numeric value */
	static getValue(input: HTMLInputElement): string;
	/** returns the settings object for those who need to look under the hood */
	static getSettings(input: HTMLInputElement): AutoNumericOptions;
	static hasInstance(input: HTMLInputElement): boolean;
}
export declare class BooleanEditor<P = {}> extends EditorWidget<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	readonly domNode: HTMLInputElement;
	get value(): boolean;
	protected get_value(): boolean;
	set value(value: boolean);
	protected set_value(value: boolean): void;
}
export interface CheckTreeItem<TSource> {
	isSelected?: boolean;
	hideCheckBox?: boolean;
	isAllDescendantsSelected?: boolean;
	id?: string;
	text?: string;
	parentId?: string;
	children?: CheckTreeItem<TSource>[];
	source?: TSource;
}
export declare class CheckTreeEditor<TItem extends CheckTreeItem<TItem>, P = {}> extends DataGrid<TItem, P> implements IGetEditValue, ISetEditValue, IReadOnly {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLDivElement;
	private itemById;
	constructor(props: EditorProps<P>);
	protected getIdProperty(): string;
	protected getTreeItems(): TItem[];
	protected updateItems(): void;
	getEditValue(property: PropertyItem, target: any): void;
	setEditValue(source: any, property: PropertyItem): void;
	protected getButtons(): ToolButton[];
	protected itemSelectedChanged(item: TItem): void;
	protected getSelectAllText(): string;
	protected isThreeStateHierarchy(): boolean;
	protected createSlickGrid(): Grid;
	protected onViewFilter(item: TItem): boolean;
	protected getInitialCollapse(): boolean;
	protected onViewProcessData(response: ListResponse<TItem>): ListResponse<TItem>;
	protected onClick(e: Event, row: number, cell: number): void;
	protected updateSelectAll(): void;
	protected updateFlags(): void;
	protected getDescendantsSelected(item: TItem): boolean;
	protected setAllSubTreeSelected(item: TItem, selected: boolean): boolean;
	protected allItemsSelected(): boolean;
	protected allDescendantsSelected(item: TItem): boolean;
	protected getDelimited(): boolean;
	protected anyDescendantsSelected(item: TItem): boolean;
	protected getColumns(): Column[];
	protected getItemText(ctx: FormatterContext): string;
	protected getSlickOptions(): GridOptions;
	protected sortItems(): void;
	protected moveSelectedUp(): boolean;
	private _readOnly;
	get_readOnly(): boolean;
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
	private lookupChangeUnbind;
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
	protected getTreeItems(): CheckTreeItem<TItem>[];
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
export declare class DateYearEditor<P extends DateYearEditorOptions = DateYearEditorOptions> extends SelectEditor<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	constructor(props: EditorProps<P>);
	getItems(): any[];
}
export interface DateYearEditorOptions extends SelectEditorOptions {
	minYear?: string;
	maxYear?: string;
	descending?: boolean;
}
export interface DecimalEditorOptions {
	minValue?: string;
	maxValue?: string;
	decimals?: any;
	padDecimals?: any;
	allowNegatives?: boolean;
}
export declare class DecimalEditor<P extends DecimalEditorOptions = DecimalEditorOptions> extends EditorWidget<P> implements IDoubleValue {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	readonly domNode: HTMLInputElement;
	constructor(props: EditorProps<P>);
	destroy(): void;
	protected initAutoNumeric(): void;
	protected getAutoNumericOptions(): any;
	get_value(): number;
	get value(): number;
	set_value(value: number): void;
	set value(v: number);
	get_isValid(): boolean;
	static defaultAutoNumericOptions(): AutoNumericOptions;
}
export declare namespace EditorUtils {
	function getDisplayText(editor: Widget<any>): string;
	function getValue(editor: Widget<any>): any;
	function saveValue(editor: Widget<any>, item: PropertyItem, target: any): void;
	function setValue(editor: Widget<any>, value: any): void;
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
	function setRequired(widget: Widget<any>, isRequired: boolean): void;
	function setContainerReadOnly(container: ArrayLike<HTMLElement> | HTMLElement, readOnly: boolean): void;
}
export declare class StringEditor<P = {}> extends EditorWidget<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	readonly domNode: HTMLInputElement;
	static createDefaultElement(): HTMLInputElement;
	get value(): string;
	protected get_value(): string;
	set value(value: string);
	protected set_value(value: string): void;
}
export declare class EmailAddressEditor<P = {}> extends StringEditor<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	constructor(props: EditorProps<P>);
}
export interface EmailEditorOptions {
	domain?: string;
	readOnlyDomain?: boolean;
}
export declare class EmailEditor<P extends EmailEditorOptions = EmailEditorOptions> extends EditorWidget<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	readonly domNode: HTMLInputElement;
	private readonly domain;
	constructor(props: EditorProps<P>);
	static registerValidationMethods(): void;
	get_value(): string;
	get value(): string;
	set_value(value: string): void;
	set value(v: string);
	get_readOnly(): boolean;
	set_readOnly(value: boolean): void;
}
export interface EnumEditorOptions extends ComboboxCommonOptions {
	enumKey?: string;
	enumType?: any;
}
export declare class EnumEditor<P extends EnumEditorOptions = EnumEditorOptions> extends ComboboxEditor<P, ComboboxItem> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	constructor(props: EditorProps<P>);
	protected updateItems(): void | PromiseLike<void>;
	protected allowClear(): boolean;
}
export interface HtmlContentEditorOptions {
	cols?: number;
	rows?: number;
}
export interface CKEditorConfig {
}
export declare class HtmlContentEditor<P extends HtmlContentEditorOptions = HtmlContentEditorOptions> extends EditorWidget<P> implements IStringValue, IReadOnly {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	private _instanceReady;
	readonly domNode: HTMLTextAreaElement;
	static createDefaultElement(): HTMLTextAreaElement;
	constructor(props: EditorProps<P>);
	protected instanceReady(x: any): void;
	protected getLanguage(): string;
	protected getConfig(): CKEditorConfig;
	protected getEditorInstance(): any;
	destroy(): void;
	get_value(): string;
	get value(): string;
	set_value(value: string): void;
	set value(v: string);
	get_readOnly(): boolean;
	set_readOnly(value: boolean): void;
	static CKEditorVer: string;
	static CKEditorBasePath: string;
	static getCKEditorBasePath(): string;
	static includeCKEditor(then: () => void): void;
}
export declare class HtmlNoteContentEditor<P extends HtmlContentEditorOptions = HtmlContentEditorOptions> extends HtmlContentEditor<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	protected getConfig(): CKEditorConfig;
}
export declare class HtmlReportContentEditor<P extends HtmlContentEditorOptions = HtmlContentEditorOptions> extends HtmlContentEditor<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	protected getConfig(): CKEditorConfig;
}
export interface IntegerEditorOptions {
	minValue?: number;
	maxValue?: number;
	allowNegatives?: boolean;
}
export declare class IntegerEditor<P extends IntegerEditorOptions = IntegerEditorOptions> extends EditorWidget<P> implements IDoubleValue {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	readonly domNode: HTMLInputElement;
	constructor(props: EditorProps<P>);
	destroy(): void;
	protected initAutoNumeric(): void;
	protected getAutoNumericOptions(): any;
	get_value(): number;
	get value(): number;
	set_value(value: number): void;
	set value(v: number);
	get_isValid(): boolean;
}
export interface LookupEditorOptions extends ComboboxEditorOptions {
	lookupKey?: string;
	async?: boolean;
}
export declare abstract class LookupEditorBase<P extends LookupEditorOptions, TItem> extends ComboboxEditor<P, TItem> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	private lookupChangeUnbind;
	constructor(props: EditorProps<P>);
	hasAsyncSource(): boolean;
	destroy(): void;
	protected getLookupKey(): string;
	protected lookup: Lookup<TItem>;
	protected getLookupAsync(): PromiseLike<Lookup<TItem>>;
	protected getLookup(): Lookup<TItem>;
	protected getItems(lookup: Lookup<TItem>): TItem[];
	protected getIdField(): any;
	protected getItemText(item: TItem, lookup: Lookup<TItem>): any;
	protected mapItem(item: TItem): ComboboxItem<TItem>;
	protected getItemDisabled(item: TItem, lookup: Lookup<TItem>): boolean;
	updateItems(): void;
	protected asyncSearch(query: ComboboxSearchQuery): Promise<ComboboxSearchResult<TItem>>;
	protected getDialogTypeKey(): string;
	protected setCreateTermOnNewEntity(entity: TItem, term: string): void;
	protected editDialogDataChange(): void;
}
export declare class LookupEditor<P extends LookupEditorOptions = LookupEditorOptions> extends LookupEditorBase<P, {}> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	constructor(props: EditorProps<P>);
}
export declare class MaskedEditor<P extends MaskedEditorOptions = MaskedEditorOptions> extends EditorWidget<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
	readonly domNode: HTMLInputElement;
	constructor(props: EditorProps<P>);
	get value(): string;
	protected get_value(): string;
	set value(value: string);
	protected set_value(value: string): void;
}
export interface MaskedEditorOptions {
	mask?: string;
	placeholder?: string;
}
export declare class PasswordEditor<TOptions = {}> extends StringEditor<TOptions> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLInputElement;
}
export interface RadioButtonEditorOptions {
	enumKey?: string;
	enumType?: any;
	lookupKey?: string;
}
export declare class RadioButtonEditor<P extends RadioButtonEditorOptions = RadioButtonEditorOptions> extends EditorWidget<P> implements IReadOnly {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	constructor(props: EditorProps<P>);
	protected addRadio(value: string, text: string): void;
	get_value(): string;
	get value(): string;
	set_value(value: string): void;
	set value(v: string);
	get_readOnly(): boolean;
	set_readOnly(value: boolean): void;
}
export interface RecaptchaOptions {
	siteKey?: string;
	language?: string;
}
export declare class Recaptcha<P extends RecaptchaOptions = RecaptchaOptions> extends EditorWidget<P> implements IStringValue {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	constructor(props: EditorProps<P>);
	get_value(): string;
	set_value(value: string): void;
}
/**
 * Adapted from 3.5.x version of Select2 (https://github.com/select2/select2), removing jQuery dependency
 */
export type Select2Element = HTMLInputElement | HTMLSelectElement;
export type Select2FormatResult = string | Element | DocumentFragment;
export interface Select2QueryOptions {
	element?: Select2Element;
	term?: string;
	page?: number;
	context?: any;
	callback?: (p1: Select2Result) => void;
	matcher?: (p1: any, p2: any, p3?: any) => boolean;
}
export interface Select2Item {
	id?: string;
	text?: string;
	source?: any;
	children?: Select2Item[];
	disabled?: boolean;
	locked?: boolean;
}
export interface Select2Result {
	hasError?: boolean;
	errorInfo?: any;
	results: Select2Item[];
	more?: boolean;
	context?: any;
}
export interface Select2AjaxOptions extends RequestInit {
	headers?: Record<string, string>;
	url?: string | ((term: string, page: number, context: any) => string);
	quietMillis?: number;
	data?: (p1: string, p2: number, p3: any) => any;
	results?: (p1: any, p2: number, p3: any) => any;
	params?: (() => any) | any;
	onError?(response: any, info?: any): void | boolean;
	onSuccess?(response: any): void;
}
export interface Select2Options {
	element?: Select2Element;
	width?: any;
	minimumInputLength?: number;
	maximumInputLength?: number;
	minimumResultsForSearch?: number;
	maximumSelectionSize?: any;
	placeholder?: string;
	placeholderOption?: any;
	separator?: string;
	allowClear?: boolean;
	multiple?: boolean;
	closeOnSelect?: boolean;
	openOnEnter?: boolean;
	id?: (p1: any) => string;
	matcher?: (p1: string, p2: string, p3: HTMLElement) => boolean;
	sortResults?: (p1: any, p2: HTMLElement, p3: any) => any;
	formatAjaxError?: (p1: any, p2: any) => Select2FormatResult;
	formatMatches?: (matches: number) => Select2FormatResult;
	formatSelection?: (p1: any, p2: HTMLElement, p3: (p1: string) => string) => Select2FormatResult;
	formatResult?: (p1: any, p2: HTMLElement, p3: any, p4: (p1: string) => string) => Select2FormatResult;
	formatResultCssClass?: (p1: any) => string;
	formatSelectionCssClass?: (item: Select2Item, container: HTMLElement) => string;
	formatNoMatches?: (input: string) => Select2FormatResult;
	formatLoadMore?: (pageNumber: number) => Select2FormatResult;
	formatSearching?: () => Select2FormatResult;
	formatInputTooLong?: (input: string, max: number) => Select2FormatResult;
	formatInputTooShort?: (input: string, min: number) => Select2FormatResult;
	formatSelectionTooBig?: (p1: number) => Select2FormatResult;
	createSearchChoice?: (p1: string) => Select2Item;
	createSearchChoicePosition?: string | ((list: Select2Item[], item: Select2Item) => void);
	initSelection?: (p1: HTMLElement, p2: (p1: any) => void) => void;
	tokenizer?: (p1: string, p2: any, p3: (p1: any) => any, p4: any) => string;
	tokenSeparators?: any;
	query?: (p1: Select2QueryOptions) => void;
	ajax?: Select2AjaxOptions;
	data?: any;
	tags?: ((string | Select2Item)[]) | (() => (string | Select2Item)[]);
	containerCss?: any;
	containerCssClass?: any;
	dropdownCss?: any;
	dropdownCssClass?: any;
	dropdownAutoWidth?: boolean;
	dropdownParent?: (input: HTMLElement) => HTMLElement;
	adaptContainerCssClass?: (p1: string) => string;
	adaptDropdownCssClass?: (p1: string) => string;
	escapeMarkup?: (p1: string) => string;
	searchInputPlaceholder?: string;
	selectOnBlur?: boolean;
	blurOnChange?: boolean;
	loadMorePadding?: number;
	nextSearchTerm?: (p1: any, p2: string) => string;
	populateResults?: (container: HTMLElement, results: Select2Item[], query: Select2QueryOptions) => void;
	shouldFocusInput?: (p1: any) => boolean;
}
export declare class Select2 {
	private el;
	constructor(opts?: Select2Options);
	private get instance();
	close(): void;
	get container(): HTMLElement;
	get dropdown(): HTMLElement;
	destroy(): void;
	get data(): (Select2Item | Select2Item[]);
	set data(value: Select2Item | Select2Item[]);
	disable(): void;
	enable(enabled?: boolean): void;
	focus(): void;
	get isFocused(): boolean;
	get isMultiple(): boolean;
	get opened(): boolean;
	open(): boolean;
	positionDropdown(): void;
	readonly(value?: boolean): void;
	get search(): HTMLInputElement;
	get val(): (string | string[]);
	set val(value: string | string[]);
	static getInstance(el: Select2Element): Select2;
	static readonly ajaxDefaults: Select2AjaxOptions;
	static readonly defaults: Select2Options;
	static stripDiacritics(str: string): string;
}
export interface ServiceLookupEditorOptions extends ComboboxEditorOptions {
	service?: string;
	idField?: string;
	textField?: string;
	pageSize?: number;
	minimumResultsForSearch?: any;
	sort?: string[];
	columnSelection?: ColumnSelection;
	includeColumns?: string[];
	excludeColumns?: string[];
	includeDeleted?: boolean;
	containsField?: string;
	equalityFilter?: any;
	criteria?: any[];
}
export declare abstract class ServiceLookupEditorBase<P extends ServiceLookupEditorOptions, TItem> extends ComboboxEditor<P, TItem> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	protected getDialogTypeKey(): string;
	protected getService(): string;
	protected getServiceUrl(): string;
	protected getIncludeColumns(): string[];
	protected getSort(): any[];
	protected getCascadeCriteria(): any[];
	protected getFilterCriteria(): any[];
	protected getIdListCriteria(idList: any[]): any[];
	protected getCriteria(query: ComboboxSearchQuery): any[];
	protected getListRequest(query: ComboboxSearchQuery): ListRequest;
	protected getServiceCallOptions(query: ComboboxSearchQuery): ServiceOptions<ListResponse<TItem>>;
	protected hasAsyncSource(): boolean;
	protected canSearch(byId: boolean): boolean;
	protected asyncSearch(query: ComboboxSearchQuery): Promise<ComboboxSearchResult<TItem>>;
}
export declare class ServiceLookupEditor<P extends ServiceLookupEditorOptions = ServiceLookupEditorOptions, TItem = any> extends ServiceLookupEditorBase<ServiceLookupEditorOptions, TItem> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	constructor(props: EditorProps<P>);
}
export interface TextAreaEditorOptions {
	cols?: number;
	rows?: number;
}
export declare class TextAreaEditor<P extends TextAreaEditorOptions = TextAreaEditorOptions> extends EditorWidget<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLTextAreaElement;
	constructor(props: EditorProps<P>);
	get value(): string;
	protected get_value(): string;
	set value(value: string);
	protected set_value(value: string): void;
}
export interface TimeEditorBaseOptions {
	noEmptyOption?: boolean;
	startHour?: any;
	endHour?: any;
	intervalMinutes?: any;
}
export declare class TimeEditorBase<P extends TimeEditorBaseOptions> extends EditorWidget<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	static createDefaultElement(): HTMLElement;
	readonly domNode: HTMLSelectElement;
	protected minutes: Fluent;
	constructor(props: EditorProps<P>);
	get hour(): number;
	get minute(): number;
	get_readOnly(): boolean;
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
/** Note that this editor's value is number of minutes, e.g. for
 * 16:30, value will be 990. If you want to use a TimeSpan field
 * use TimeSpanEditor instead.
 */
export declare class TimeEditor<P extends TimeEditorOptions = TimeEditorOptions> extends TimeEditorBase<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	constructor(props: EditorProps<P>);
	get value(): number;
	protected get_value(): number;
	set value(value: number);
	protected set_value(value: number): void;
}
export interface TimeSpanEditorOptions extends TimeEditorBaseOptions {
}
/**
 * This editor is for TimeSpan fields. It uses a string value in the format "HH:mm".
 */
export declare class TimeSpanEditor<P extends TimeSpanEditorOptions = TimeSpanEditorOptions> extends TimeEditorBase<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	constructor(props: EditorProps<P>);
	protected get_value(): string;
	protected set_value(value: string): void;
	get value(): string;
	set value(value: string);
}
export declare namespace UploadHelper {
	function addUploadInput(options: UploadInputOptions): Fluent;
	function checkImageConstraints(file: UploadResponse, opt: FileUploadConstraints): boolean;
	function fileNameSizeDisplay(name: string, bytes: number): string;
	function fileSizeDisplay(bytes: number): string;
	function hasImageExtension(filename: string): boolean;
	function thumbFileName(filename: string): string;
	function dbFileUrl(filename: string): string;
	/**
	 * Creates a lightbox for a single upload thumbnail anchor element.
	 * It uses one of glightbox, simplelightbox or colorbox if available.
	 * Override this function to use a different lightbox library.
	 */
	function lightbox(link: HTMLElement | ArrayLike<HTMLElement>): void;
	/** @deprecated use lightbox */
	const colorBox: typeof lightbox;
	function populateFileSymbols(c: HTMLElement | ArrayLike<HTMLElement>, items: UploadedFile[], displayOriginalName?: boolean, urlPrefix?: string): void;
}
export interface UploadedFile {
	Filename?: string;
	OriginalName?: string;
}
export interface UploadInputOptions {
	container?: HTMLElement | ArrayLike<HTMLElement>;
	zone?: HTMLElement | ArrayLike<HTMLElement>;
	progress?: HTMLElement | ArrayLike<HTMLElement>;
	inputName?: string;
	allowMultiple?: boolean;
	uploadIntent?: string;
	uploadUrl?: string;
	fileDone?: (p1: UploadResponse, p2: string, p3: any) => void;
}
export interface UploadResponse extends ServiceResponse {
	TemporaryFile: string;
	Size: number;
	IsImage: boolean;
	Width: number;
	Height: number;
}
export interface FileUploadConstraints {
	minWidth?: number;
	maxWidth?: number;
	minHeight?: number;
	maxHeight?: number;
	minSize?: number;
	maxSize?: number;
	allowNonImage?: boolean;
	originalNameProperty?: string;
}
export interface FileUploadEditorOptions extends FileUploadConstraints {
	displayFileName?: boolean;
	uploadIntent?: string;
	uploadUrl?: string;
	urlPrefix?: string;
}
export interface ImageUploadEditorOptions extends FileUploadEditorOptions {
}
export declare class FileUploadEditor<P extends FileUploadEditorOptions = FileUploadEditorOptions> extends EditorWidget<P> implements IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	constructor(props: EditorProps<P>);
	protected getUploadInputOptions(): UploadInputOptions;
	protected addFileButtonText(): string;
	protected getToolButtons(): ToolButton[];
	protected populate(): void;
	protected updateInterface(): void;
	get_readOnly(): boolean;
	set_readOnly(value: boolean): void;
	get_required(): boolean;
	set_required(value: boolean): void;
	get_value(): UploadedFile;
	get value(): UploadedFile;
	set_value(value: UploadedFile): void;
	set value(v: UploadedFile);
	getEditValue(property: PropertyItem, target: any): void;
	setEditValue(source: any, property: PropertyItem): void;
	protected entity: UploadedFile;
	protected toolbar: Toolbar;
	protected progress: HTMLElement;
	protected fileSymbols: HTMLElement;
	protected uploadInput: Fluent;
	protected hiddenInput: HTMLInputElement;
}
export declare class ImageUploadEditor<P extends ImageUploadEditorOptions = ImageUploadEditorOptions> extends FileUploadEditor<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	constructor(props: EditorProps<P>);
}
export interface MultipleFileUploadEditorOptions extends FileUploadEditorOptions {
	jsonEncodeValue?: boolean;
}
export declare class MultipleFileUploadEditor<P extends MultipleFileUploadEditorOptions = MultipleFileUploadEditorOptions> extends EditorWidget<P> implements IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	private entities;
	private toolbar;
	private fileSymbols;
	private uploadInput;
	protected progress: HTMLElement;
	protected hiddenInput: HTMLInputElement;
	constructor(props: EditorProps<P>);
	protected getUploadInputOptions(): UploadInputOptions;
	protected addFileButtonText(): string;
	protected getToolButtons(): ToolButton[];
	protected populate(): void;
	protected updateInterface(): void;
	get_readOnly(): boolean;
	set_readOnly(value: boolean): void;
	get_required(): boolean;
	set_required(value: boolean): void;
	get_value(): UploadedFile[];
	get value(): UploadedFile[];
	set_value(value: UploadedFile[]): void;
	set value(v: UploadedFile[]);
	getEditValue(property: PropertyItem, target: any): void;
	setEditValue(source: any, property: PropertyItem): void;
	get jsonEncodeValue(): boolean;
	set jsonEncodeValue(value: boolean);
}
export declare class MultipleImageUploadEditor<P extends ImageUploadEditorOptions = ImageUploadEditorOptions> extends MultipleFileUploadEditor<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor(props: EditorProps<P>);
}
export declare class URLEditor<P = {}> extends StringEditor<P> {
	static [Symbol.typeInfo]: EditorTypeInfo<"Serenity.">;
	constructor(props: EditorProps<P>);
}
interface CriteriaWithText {
	criteria?: any[];
	displayText?: string;
}
export interface FilterOperator {
	key?: string;
	title?: string;
	format?: string;
}
export declare namespace FilterOperators {
	const isTrue = "true";
	const isFalse = "false";
	const contains = "contains";
	const startsWith = "startswith";
	const EQ = "eq";
	const NE = "ne";
	const GT = "gt";
	const GE = "ge";
	const LT = "lt";
	const LE = "le";
	const BW = "bw";
	const IN = "in";
	const isNull = "isnull";
	const isNotNull = "isnotnull";
	const toCriteriaOperator: {
		[key: string]: string;
	};
}
export interface IFiltering {
	createEditor(): void;
	getCriteria(): CriteriaWithText;
	getOperators(): FilterOperator[];
	loadState(state: any): void;
	saveState(): any;
	get_field(): PropertyItem;
	set_field(value: PropertyItem): void;
	get_container(): HTMLElement;
	set_container(value: HTMLElement): void;
	get_operator(): FilterOperator;
	set_operator(value: FilterOperator): void;
}
export declare class IFiltering {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export interface IQuickFiltering {
	initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
}
export declare class IQuickFiltering {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export declare abstract class BaseFiltering implements IFiltering, IQuickFiltering {
	private field;
	get_field(): PropertyItem;
	set_field(value: PropertyItem): void;
	private container;
	get_container(): HTMLElement;
	set_container(value: HTMLElement): void;
	private operator;
	get_operator(): FilterOperator;
	set_operator(value: FilterOperator): void;
	abstract getOperators(): FilterOperator[];
	protected appendNullableOperators(list: FilterOperator[]): FilterOperator[];
	protected appendComparisonOperators(list: FilterOperator[]): FilterOperator[];
	protected isNullable(): boolean;
	createEditor(): void;
	protected operatorFormat(op: FilterOperator): string;
	protected getTitle(field: PropertyItem): string;
	protected displayText(op: FilterOperator, values?: any[]): string;
	protected getCriteriaField(): string;
	getCriteria(): CriteriaWithText;
	loadState(state: any): void;
	saveState(): string;
	protected argumentNull(): Error;
	validateEditorValue(value: string): string;
	getEditorValue(): string;
	getEditorText(): string;
	initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
	protected static registerClass<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): ClassTypeInfo<TypeName>;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
}
export declare abstract class BaseEditorFiltering<TEditor extends Widget<any>> extends BaseFiltering {
	editorTypeRef: any;
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor(editorTypeRef: any);
	protected useEditor(): boolean;
	protected editor: TEditor;
	createEditor(): void;
	protected useIdField(): boolean;
	getCriteriaField(): string;
	getEditorOptions(): any;
	loadState(state: any): void;
	saveState(): any;
	getEditorValue(): any;
	initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
}
export declare class BooleanFiltering extends BaseFiltering {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	getOperators(): FilterOperator[];
}
export declare class DateFiltering extends BaseEditorFiltering<DateEditor> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor();
	getOperators(): FilterOperator[];
}
export declare class DateTimeFiltering extends BaseEditorFiltering<DateEditor> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor();
	getOperators(): FilterOperator[];
	getCriteria(): CriteriaWithText;
}
export declare class DecimalFiltering extends BaseEditorFiltering<DecimalEditor> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor();
	getOperators(): FilterOperator[];
}
export declare class EditorFiltering extends BaseEditorFiltering<Widget<any>> {
	readonly props: {
		editorType?: string;
		useRelative?: boolean;
		useLike?: boolean;
	};
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor(props?: {
		editorType?: string;
		useRelative?: boolean;
		useLike?: boolean;
	});
	get editorType(): string;
	set editorType(value: string);
	get useRelative(): boolean;
	set useRelative(value: boolean);
	get useLike(): boolean;
	set useLike(value: boolean);
	getOperators(): FilterOperator[];
	protected useEditor(): boolean;
	getEditorOptions(): any;
	createEditor(): void;
	protected useIdField(): boolean;
	initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
}
export declare class EnumFiltering extends BaseEditorFiltering<EnumEditor> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor();
	getOperators(): FilterOperator[];
	getEditorText(): string;
}
export interface FilterFieldSelectOptions {
	fields: PropertyItem[];
}
export declare class FilterPanel<P = {}> extends FilterWidgetBase<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private rowsDiv;
	private resetButton;
	private searchButton;
	constructor(props: WidgetProps<P>);
	private _showInitialLine;
	get showInitialLine(): boolean;
	set showInitialLine(value: boolean);
	protected filterStoreChanged(): void;
	updateRowsFromStore(): void;
	private _showSearchButton;
	get showSearchButton(): boolean;
	set showSearchButton(value: boolean);
	updateStoreOnReset: boolean;
	protected renderContents(): any;
	protected searchButtonClick(e: Event): void;
	get_hasErrors(): boolean;
	search(): void;
	protected addButtonClick(e: Event): void;
	protected resetButtonClick(e: Event): void;
	protected findEmptyRow(): HTMLElement;
	protected addEmptyRow(popupField: boolean): HTMLElement;
	protected onRowFieldChange(e: Event): void;
	protected rowFieldChange(row: HTMLElement): void;
	protected removeFiltering(row: HTMLElement): void;
	protected populateOperatorList(row: HTMLElement): void;
	protected getFieldFor(row: HTMLElement): PropertyItem;
	protected getFilteringFor(row: HTMLElement): IFiltering;
	protected onRowOperatorChange(e: Event): void;
	protected rowOperatorChange(row: HTMLElement): void;
	protected deleteRowClick(e: Event): void;
	protected updateButtons(): void;
	protected andOrClick(e: Event): void;
	protected leftRightParenClick(e: Event): void;
	protected updateParens(): void;
}
export declare class FilterDialog<P = {}> extends BaseDialog<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private filterPanel;
	constructor(props: WidgetProps<P>);
	get_filterPanel(): FilterPanel;
	protected renderContents(): any;
	protected getDialogOptions(): DialogOptions;
	protected getDialogButtons(): DialogButton[];
}
declare class FilteringTypeRegistryImpl extends BaseTypeRegistry<Function> {
	constructor();
	protected isMatchingType(type: any): boolean;
	protected loadError(key: string): void;
}
export declare const FilteringTypeRegistry: FilteringTypeRegistryImpl;
export declare class IntegerFiltering extends BaseEditorFiltering<IntegerEditor> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor();
	getOperators(): FilterOperator[];
}
export declare class LookupFiltering extends BaseEditorFiltering<LookupEditor> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor();
	getOperators(): FilterOperator[];
	protected useEditor(): boolean;
	protected useIdField(): boolean;
	getEditorText(): string;
}
export declare class ServiceLookupFiltering extends BaseEditorFiltering<ServiceLookupEditor> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor();
	getOperators(): FilterOperator[];
	protected useEditor(): boolean;
	protected useIdField(): boolean;
	getEditorText(): string;
}
export declare class StringFiltering extends BaseFiltering {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	getOperators(): FilterOperator[];
	validateEditorValue(value: string): string;
}
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
	format(ctx: FormatterContext): FormatterResult;
	get falseText(): string;
	set falseText(value: string);
	get trueText(): string;
	set trueText(value: string);
}
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
	format(ctx: FormatterContext): FormatterResult;
}
export declare class DateFormatter implements Formatter {
	readonly props: {
		displayFormat?: string;
	};
	static [Symbol.typeInfo]: FormatterTypeInfo<"Serenity.">;
	constructor(props?: {
		displayFormat?: string;
	});
	static format(value: any, format?: string): any;
	get displayFormat(): string;
	set displayFormat(value: string);
	format(ctx: FormatterContext): string;
}
export declare class DateTimeFormatter extends DateFormatter {
	static [Symbol.typeInfo]: FormatterTypeInfo<"Serenity.">;
	constructor(props?: {
		displayFormat?: string;
	});
}
export declare class EnumFormatter implements Formatter {
	readonly props: {
		enumKey?: string;
	};
	static [Symbol.typeInfo]: FormatterTypeInfo<"Serenity.">;
	constructor(props?: {
		enumKey?: string;
	});
	format(ctx: FormatterContext): FormatterResult;
	get enumKey(): string;
	set enumKey(value: string);
	static format(enumType: any, value: any): string;
	static getText(enumKey: string, name: string): string;
	static getName(enumType: any, value: any): string;
}
export interface IInitializeColumn {
	initializeColumn(column: Column): void;
}
export declare class IInitializeColumn {
	static [Symbol.typeInfo]: InterfaceTypeInfo<"Serenity.">;
}
export declare class FileDownloadFormatter implements Formatter, IInitializeColumn {
	readonly props: {
		displayFormat?: string;
		originalNameProperty?: string;
		iconClass?: string;
	};
	static [Symbol.typeInfo]: FormatterTypeInfo<"Serenity.">;
	constructor(props?: {
		displayFormat?: string;
		originalNameProperty?: string;
		iconClass?: string;
	});
	format(ctx: FormatterContext): string;
	static dbFileUrl(filename: string): string;
	initializeColumn(column: Column): void;
	get displayFormat(): string;
	set displayFormat(value: string);
	get originalNameProperty(): string;
	set originalNameProperty(value: string);
	get iconClass(): string;
	set iconClass(value: string);
}
export declare abstract class FormatterBase implements Formatter {
	abstract format(ctx: FormatterContext): FormatterResult;
	protected static registerFormatter<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): FormatterTypeInfo<TypeName>;
	static [Symbol.typeInfo]: FormatterTypeInfo<"Serenity.">;
}
export declare class MinuteFormatter implements Formatter {
	static [Symbol.typeInfo]: FormatterTypeInfo<"Serenity.">;
	format(ctx: FormatterContext): string;
	static format(value: number): string;
}
export declare class NumberFormatter implements Formatter {
	readonly props: {
		displayFormat?: string;
	};
	static [Symbol.typeInfo]: FormatterTypeInfo<"Serenity.">;
	constructor(props?: {
		displayFormat?: string;
	});
	format(ctx: FormatterContext): string;
	static format(value: any, format?: string): string;
	get displayFormat(): string;
	set displayFormat(value: string);
}
export declare class UrlFormatter implements Formatter, IInitializeColumn {
	readonly props: {
		displayProperty?: string;
		displayFormat?: string;
		urlProperty?: string;
		urlFormat?: string;
		target?: string;
	};
	static [Symbol.typeInfo]: FormatterTypeInfo<"Serenity.">;
	constructor(props?: {
		displayProperty?: string;
		displayFormat?: string;
		urlProperty?: string;
		urlFormat?: string;
		target?: string;
	});
	format(ctx: FormatterContext): string;
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
export declare class ColumnsBase<TRow = any> {
	constructor(items: Column<TRow>[]);
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
	tabIndex?: number;
	/**
	 * Child elements or text to be displayed inside the link.
	 */
	children?: any;
}): HTMLAnchorElement;
export interface GridRadioSelectionMixinOptions {
	selectable?: (item: any) => boolean;
}
export declare class GridRadioSelectionMixin {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private idField;
	private include;
	private grid;
	private options;
	constructor(grid: IDataGrid, options?: GridRadioSelectionMixinOptions);
	private isSelectable;
	clear(): void;
	resetCheckedAndRefresh(): void;
	getSelectedKey(): string;
	getSelectedAsInt32(): number;
	getSelectedAsInt64(): number;
	setSelectedKey(key: string): void;
	static createSelectColumn(getMixin: () => GridRadioSelectionMixin): Column;
}
export interface GridRowSelectionMixinOptions {
	selectable?: (item: any) => boolean;
}
export declare class GridRowSelectionMixin {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private idField;
	private include;
	private grid;
	private options;
	constructor(grid: IDataGrid, options?: GridRowSelectionMixinOptions);
	updateSelectAll(): void;
	clear(): void;
	resetCheckedAndRefresh(): void;
	selectKeys(keys: string[]): void;
	getSelectedKeys(): string[];
	getSelectedAsInt32(): number[];
	getSelectedAsInt64(): number[];
	setSelectedKeys(keys: string[]): void;
	private isSelectable;
	static createSelectColumn(getMixin: () => GridRowSelectionMixin): Column;
}
export declare namespace GridSelectAllButtonHelper {
	function update(grid: IDataGrid, getSelected: (p1: any) => boolean): void;
	function define(getGrid: () => IDataGrid, getId: (p1: any) => any, getSelected: (p1: any) => boolean, setSelected: (p1: any, p2: boolean) => void, text?: string, onClick?: () => void): ToolButton;
}
export declare namespace GridUtils {
	function addToggleButton(toolDiv: HTMLElement | ArrayLike<HTMLElement>, cssClass: string, callback: (p1: boolean) => void, hint: string, initial?: boolean): void;
	function addIncludeDeletedToggle(toolDiv: HTMLElement | ArrayLike<HTMLElement>, view: IRemoteView<any>, hint?: string, initial?: boolean): void;
	function addQuickSearchInput(toolDiv: HTMLElement | ArrayLike<HTMLElement>, view: IRemoteView<any>, fields?: QuickSearchField[], onChange?: () => void): QuickSearchInput;
	function addQuickSearchInputCustom(container: HTMLElement | ArrayLike<HTMLElement>, onSearch: (p1: string, p2: string, done: (p3: boolean) => void) => void, fields?: QuickSearchField[]): QuickSearchInput;
	function makeOrderable(grid: Grid, handleMove: (rows: number[], insertBefore: number) => void): void;
	function makeOrderableWithUpdateRequest<TItem = any, TId = any>(grid: IDataGrid, getId: (item: TItem) => TId, getDisplayOrder: (item: TItem) => any, service: string, getUpdateRequest: (id: TId, order: number) => SaveRequest<TItem>): void;
}
export declare namespace LazyLoadHelper {
	const executeOnceWhenShown: typeof executeOnceWhenVisible;
	const executeEverytimeWhenShown: typeof executeEverytimeWhenVisible;
}
export declare namespace PropertyItemSlickConverter {
	function toSlickColumns(items: PropertyItem[]): Column[];
	function toSlickColumn(item: PropertyItem): Column;
}
export declare namespace SlickFormatting {
	function itemLink<TItem = any>(itemType: string, idField: string, getText: Format<TItem>, cssClass?: (ctx: FormatterContext<TItem>) => string, encode?: boolean): Format<TItem>;
	function treeToggle(getView: () => IRemoteView<any>, getId: (x: any) => any, formatter: Format): Format;
}
export declare namespace SlickHelper {
	function setDefaults(columns: Column[], localTextPrefix?: string): any;
}
export declare namespace SlickTreeHelper {
	function filterCustom<TItem>(item: TItem, getParent: (x: TItem) => any): boolean;
	function filterById<TItem>(item: TItem, view: IRemoteView<TItem>, getParentId: (x: TItem) => any): boolean;
	function setCollapsed<TItem>(items: TItem[], collapsed: boolean): void;
	function setCollapsedFlag<TItem>(item: TItem, collapsed: boolean): void;
	function setIndents<TItem>(items: TItem[], getId: (x: TItem) => any, getParentId: (x: TItem) => any, setCollapsed?: boolean): void;
	function toggleClick<TItem>(e: Event, row: number, cell: number, view: IRemoteView<TItem>, getId: (x: TItem) => any): void;
}
export declare namespace SubDialogHelper {
	function bindToDataChange(dialog: any, owner: Widget<any>, dataChange: (ev: DataChangeInfo) => void, useTimeout?: boolean): any;
	function triggerDataChange(dialog: Widget<any>): any;
	function triggerDataChanged(element: HTMLElement | ArrayLike<HTMLElement>): void;
	function bubbleDataChange(dialog: any, owner: Widget<any>, useTimeout?: boolean): any;
	function cascade(cascadedDialog: {
		domNode: HTMLElement;
	}, ofElement: HTMLElement | ArrayLike<HTMLElement>): any;
	function cascadedDialogOffset(element: HTMLElement | ArrayLike<HTMLElement>): any;
}
export declare namespace TabsExtensions {
	function setDisabled(tabs: ArrayLike<HTMLElement> | HTMLElement, tabKey: string, isDisabled: boolean): void;
	function toggle(tabs: ArrayLike<HTMLElement> | HTMLElement, tabKey: string, visible: boolean): void;
	function activeTabKey(tabs: ArrayLike<HTMLElement> | HTMLElement): string;
	function indexByKey(tabs: ArrayLike<HTMLElement> | HTMLElement): Record<string, number>;
	function selectTab(tabs: HTMLElement | ArrayLike<HTMLElement>, tabKey: string | number): void;
	function initialize(tabs: HTMLElement | ArrayLike<HTMLElement>, activeChange: () => void): Fluent<HTMLElement>;
	function destroy(tabs: HTMLElement | ArrayLike<HTMLElement>): void;
}
export declare class BasePanel<P = {}> extends Widget<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	constructor(props: WidgetProps<P>);
	destroy(): void;
	protected tabs: Fluent;
	protected toolbar: Toolbar;
	protected validator: Validator;
	protected isPanel: boolean;
	protected responsive: boolean;
	arrange(): void;
	protected getToolbarButtons(): ToolButton[];
	protected getValidatorOptions(): any;
	protected initTabs(): void;
	protected initToolbar(): void;
	protected initValidator(): void;
	protected resetValidation(): void;
	protected validateForm(): boolean;
}
/** @deprecated use BasePanel */
export declare const TemplatedPanel: typeof BasePanel;
export declare class PrefixedContext {
	readonly idPrefix: string;
	constructor(idPrefix: string);
	byId(id: string): Fluent;
	w<TWidget>(id: string, type: {
		new (...args: any[]): TWidget;
	}): TWidget;
}
export declare class PropertyPanel<TItem, P> extends BasePanel<P> {
	static [Symbol.typeInfo]: ClassTypeInfo<"Serenity.">;
	private _entity;
	private _entityId;
	constructor(props: WidgetProps<P>);
	destroy(): void;
	protected initPropertyGrid(): void;
	protected loadInitialEntity(): void;
	protected getFormKey(): string;
	protected getPropertyGridOptions(): PropertyGridOptions;
	protected getPropertyItems(): PropertyItem[];
	protected getSaveEntity(): TItem;
	get entity(): TItem;
	get entityId(): any;
	protected set entity(value: TItem);
	protected set entityId(value: any);
	protected validateBeforeSave(): boolean;
	protected propertyGrid: PropertyGrid;
}
export declare namespace ReflectionOptionsSetter {
	function set(target: any, options: any): void;
}
export type Constructor<T> = new (...args: any[]) => T;

export {
	alert$1 as alert,
	confirm$1 as confirm,
};

export {};
