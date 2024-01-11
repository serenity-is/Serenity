import { GroupTotals, Column, FormatterContext, Group, GroupItemMetadataProvider, EventEmitter, Grid, GridOptions } from '@serenity-is/sleekgrid';

/**
 * Tries to block the page
 */
declare function blockUI(options?: {
    zIndex?: number;
    useTimeout?: boolean;
}): void;
/**
 * Unblocks the page.
 */
declare function blockUndo(): void;

declare var Config: {
    /**
     * This is the root path of your application. If your application resides under http://localhost/mysite/,
     * your root path is "/mysite/". This variable is automatically initialized by reading from a <link> element
     * with ID "ApplicationPath" from current page, which is usually located in your _LayoutHead.cshtml file
     */
    applicationPath: string;
    /**
     * Email validation by default only allows ASCII characters. Set this to true if you want to allow unicode.
     */
    emailAllowOnlyAscii: boolean;
    /**
     * @Obsolete defaulted to false before for backward compatibility, now it is true by default
     */
    responsiveDialogs: boolean;
    /**
     * Set this to true, to prefer bootstrap modals over jQuery UI dialogs by default for general dialogs
     */
    bootstrapDialogs: boolean;
    /**
     * Set this to true, to prefer bootstrap modals over jQuery UI dialogs by default for message dialogs
     */
    bootstrapMessages: boolean;
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

/**
 * CriteriaBuilder is a class that allows to build unary or binary criteria with completion support.
 */
declare class CriteriaBuilder extends Array {
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
 * parseCriteria('A >= @p1 and B < @p2', { p1: 5, p2: 4 }) // [[[a], '>=' 5], 'and', [[b], '<', 4]]
 */
declare function parseCriteria(expression: string, params?: any): any[];
/**
 * Parses a criteria expression to Serenity Criteria array format.
 * The expression may contain parameter placeholders like `A >= ${p1}`
 * where p1 is a variable in the scope.
 * @param strings The string fragments.
 * @param values The tagged template arguments.
 * @example
 * var a = 5, b = 4;
 * parseCriteria`A >= ${a} and B < ${b}` // [[[a], '>=' 5], 'and', [[b], '<', 4]]
 */
declare function parseCriteria(strings: TemplateStringsArray, ...values: any[]): any[];
/**
 * Enumeration of Criteria operator keys.
 */
declare enum CriteriaOperator {
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
declare function Criteria(field: string): CriteriaBuilder;
declare namespace Criteria {
    var and: (c1: any[], c2: any[], ...rest: any[][]) => any[];
    var Operator: typeof CriteriaOperator;
    var isEmpty: (c: any[]) => boolean;
    var join: (c1: any[], op: string, c2: any[]) => any[];
    var not: (c: any[]) => (string | any[])[];
    var or: (c1: any[], c2: any[], ...rest: any[][]) => any[];
    var paren: (c: any[]) => any[];
    var parse: typeof parseCriteria;
}

interface DebouncedFunction<T extends (...args: any[]) => any> {
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
 * @source underscore.js
 */
declare function debounce<T extends (...args: any) => any>(func: T, wait?: number, immediate?: boolean): DebouncedFunction<T>;

type UtilityColor = "primary" | "secondary" | "success" | "danger" | "warning" | "info" | "light" | "dark" | "muted" | "white";
type TextColor = UtilityColor | "aqua" | "blue" | "fuschia" | "gray" | "green" | "light-blue" | "lime" | "maroon" | "navy" | "olive" | "orange" | "purple" | "red" | "teal" | "yellow";
declare function bgColor(color: UtilityColor): string;
declare function textColor(color: TextColor): string;
declare function faIcon(key: faIconKey, color?: TextColor): string;
declare function fabIcon(key: fabIconKey, color?: TextColor): string;
type KnownIconClass = `fa fa-${faIconKey}` | `fab fa-${fabIconKey}`;
type AnyIconClass = KnownIconClass | (string & {});
type IconClassName = AnyIconClass | (AnyIconClass[]);
declare function iconClassName(icon: IconClassName): string;
type faIconKey = "ad" | "address-book" | "address-card" | "adjust" | "air-freshener" | "align-center" | "align-justify" | "align-left" | "align-right" | "allergies" | "ambulance" | "american-sign-language-interpreting" | "anchor" | "angle-double-down" | "angle-double-left" | "angle-double-right" | "angle-double-up" | "angle-down" | "angle-left" | "angle-right" | "angle-up" | "angry" | "ankh" | "apple-alt" | "archive" | "archway" | "arrow-alt-circle-down" | "arrow-alt-circle-left" | "arrow-alt-circle-right" | "arrow-alt-circle-up" | "arrow-circle-down" | "arrow-circle-left" | "arrow-circle-right" | "arrow-circle-up" | "arrow-down" | "arrow-left" | "arrow-right" | "arrow-up" | "arrows-alt" | "arrows-alt-h" | "arrows-alt-v" | "assistive-listening-systems" | "asterisk" | "at" | "atlas" | "atom" | "audio-description" | "award" | "baby" | "baby-carriage" | "backspace" | "backward" | "bacon" | "balance-scale" | "balance-scale-left" | "balance-scale-right" | "ban" | "band-aid" | "barcode" | "bars" | "baseball-ball" | "basketball-ball" | "bath" | "battery-empty" | "battery-full" | "battery-half" | "battery-quarter" | "battery-three-quarters" | "bed" | "beer" | "bell" | "bell-o" | "bell-slash" | "bezier-curve" | "bible" | "bicycle" | "biking" | "binoculars" | "biohazard" | "birthday-cake" | "blender" | "blender-phone" | "blind" | "blog" | "bold" | "bolt" | "bomb" | "bone" | "bong" | "book" | "book-dead" | "book-medical" | "book-open" | "book-reader" | "bookmark" | "border-all" | "border-none" | "border-style" | "bowling-ball" | "box" | "box-open" | "boxes" | "braille" | "brain" | "bread-slice" | "briefcase" | "briefcase-medical" | "broadcast-tower" | "broom" | "brush" | "bug" | "building" | "bullhorn" | "bullseye" | "burn" | "bus" | "bus-alt" | "business-time" | "calculator" | "calendar" | "calendar-alt" | "calendar-check" | "calendar-day" | "calendar-minus" | "calendar-plus" | "calendar-times" | "calendar-week" | "camera" | "camera-retro" | "campground" | "candy-cane" | "cannabis" | "capsules" | "car" | "car-alt" | "car-battery" | "car-crash" | "car-side" | "caret-down" | "caret-left" | "caret-right" | "caret-square-down" | "caret-square-left" | "caret-square-right" | "caret-square-up" | "caret-up" | "carrot" | "cart-arrow-down" | "cart-plus" | "cash-register" | "cat" | "certificate" | "chair" | "chalkboard" | "chalkboard-teacher" | "charging-station" | "chart-area" | "chart-bar" | "chart-line" | "chart-pie" | "check" | "check-circle" | "check-double" | "check-square" | "cheese" | "chess" | "chess-bishop" | "chess-board" | "chess-king" | "chess-knight" | "chess-pawn" | "chess-queen" | "chess-rook" | "chevron-circle-down" | "chevron-circle-left" | "chevron-circle-right" | "chevron-circle-up" | "chevron-down" | "chevron-left" | "chevron-right" | "chevron-up" | "child" | "church" | "circle" | "circle-notch" | "city" | "clinic-medical" | "clipboard" | "clipboard-check" | "clipboard-list" | "clock" | "clock-o" | "clone" | "closed-captioning" | "cloud" | "cloud-download-alt" | "cloud-meatball" | "cloud-moon" | "cloud-moon-rain" | "cloud-rain" | "cloud-showers-heavy" | "cloud-sun" | "cloud-sun-rain" | "cloud-upload-alt" | "cocktail" | "code" | "code-branch" | "coffee" | "cog" | "cogs" | "coins" | "columns" | "comment" | "comment-alt" | "comment-dollar" | "comment-dots" | "comment-medical" | "comment-slash" | "comments" | "comments-dollar" | "compact-disc" | "compass" | "compress" | "compress-arrows-alt" | "concierge-bell" | "cookie" | "cookie-bite" | "copy" | "copyright" | "couch" | "credit-card" | "crop" | "crop-alt" | "cross" | "crosshairs" | "crow" | "crown" | "crutch" | "cube" | "cubes" | "cut" | "database" | "deaf" | "democrat" | "desktop" | "dharmachakra" | "diagnoses" | "dice" | "dice-d20" | "dice-d6" | "dice-five" | "dice-four" | "dice-one" | "dice-six" | "dice-three" | "dice-two" | "digital-tachograph" | "directions" | "divide" | "dizzy" | "dna" | "dog" | "dollar-sign" | "dolly" | "dolly-flatbed" | "donate" | "door-closed" | "door-open" | "dot-circle" | "dove" | "download" | "drafting-compass" | "dragon" | "draw-polygon" | "drum" | "drum-steelpan" | "drumstick-bite" | "dumbbell" | "dumpster" | "dumpster-fire" | "dungeon" | "edit" | "egg" | "eject" | "ellipsis-h" | "ellipsis-v" | "envelope" | "envelope-o" | "envelope-open" | "envelope-open-text" | "envelope-square" | "equals" | "eraser" | "ethernet" | "euro-sign" | "exchange-alt" | "exclamation" | "exclamation-circle" | "exclamation-triangle" | "expand" | "expand-arrows-alt" | "external-link-alt" | "external-link-square-alt" | "eye" | "eye-dropper" | "eye-slash" | "fan" | "fast-backward" | "fast-forward" | "fax" | "feather" | "feather-alt" | "female" | "fighter-jet" | "file" | "file-alt" | "file-archive" | "file-audio" | "file-code" | "file-contract" | "file-csv" | "file-download" | "file-excel" | "file-excel-o" | "file-export" | "file-image" | "file-import" | "file-invoice" | "file-invoice-dollar" | "file-medical" | "file-medical-alt" | "file-pdf" | "file-pdf-o" | "file-powerpoint" | "file-prescription" | "file-signature" | "file-upload" | "file-text" | "file-text-o" | "file-video" | "file-word" | "fill" | "fill-drip" | "film" | "filter" | "fingerprint" | "fire" | "floppy-o" | "fire-alt" | "fire-extinguisher" | "first-aid" | "fish" | "fist-raised" | "flag" | "flag-checkered" | "flag-usa" | "flask" | "flushed" | "folder" | "folder-minus" | "folder-open" | "folder-open-o" | "folder-plus" | "font" | "football-ball" | "forward" | "frog" | "frown" | "frown-open" | "funnel-dollar" | "futbol" | "gamepad" | "gas-pump" | "gavel" | "gem" | "genderless" | "ghost" | "gift" | "gifts" | "glass-cheers" | "glass-martini" | "glass-martini-alt" | "glass-whiskey" | "glasses" | "globe" | "globe-africa" | "globe-americas" | "globe-asia" | "globe-europe" | "golf-ball" | "gopuram" | "graduation-cap" | "greater-than" | "greater-than-equal" | "grimace" | "grin" | "grin-alt" | "grin-beam" | "grin-beam-sweat" | "grin-hearts" | "grin-squint" | "grin-squint-tears" | "grin-stars" | "grin-tears" | "grin-tongue" | "grin-tongue-squint" | "grin-tongue-wink" | "grin-wink" | "grip-horizontal" | "grip-lines" | "grip-lines-vertical" | "grip-vertical" | "guitar" | "h-square" | "hamburger" | "hammer" | "hamsa" | "hand-holding" | "hand-holding-heart" | "hand-holding-usd" | "hand-lizard" | "hand-middle-finger" | "hand-paper" | "hand-peace" | "hand-point-down" | "hand-point-left" | "hand-point-right" | "hand-point-up" | "hand-pointer" | "hand-rock" | "hand-scissors" | "hand-spock" | "hands" | "hands-helping" | "handshake" | "hanukiah" | "hard-hat" | "hashtag" | "hat-cowboy" | "hat-cowboy-side" | "hat-wizard" | "haykal" | "hdd" | "heading" | "headphones" | "headphones-alt" | "headset" | "heart" | "heart-broken" | "heartbeat" | "helicopter" | "highlighter" | "hiking" | "hippo" | "history" | "hockey-puck" | "holly-berry" | "home" | "horse" | "horse-head" | "hospital" | "hospital-alt" | "hospital-symbol" | "hot-tub" | "hotdog" | "hotel" | "hourglass" | "hourglass-end" | "hourglass-half" | "hourglass-start" | "house-damage" | "hryvnia" | "i-cursor" | "ice-cream" | "icicles" | "icons" | "id-badge" | "id-card" | "id-card-alt" | "igloo" | "image" | "images" | "inbox" | "indent" | "industry" | "infinity" | "info" | "info-circle" | "italic" | "jedi" | "joint" | "journal-whills" | "kaaba" | "key" | "keyboard" | "khanda" | "kiss" | "kiss-beam" | "kiss-wink-heart" | "kiwi-bird" | "landmark" | "language" | "laptop" | "laptop-code" | "laptop-medical" | "laugh" | "laugh-beam" | "laugh-squint" | "laugh-wink" | "layer-group" | "leaf" | "lemon" | "less-than" | "less-than-equal" | "level-down-alt" | "level-up-alt" | "life-ring" | "lightbulb" | "link" | "lira-sign" | "list" | "list-alt" | "list-ol" | "list-ul" | "location-arrow" | "lock" | "lock-open" | "long-arrow-alt-down" | "long-arrow-alt-left" | "long-arrow-alt-right" | "long-arrow-alt-up" | "low-vision" | "luggage-cart" | "magic" | "magnet" | "mail-bulk" | "mail-forward" | "mail-reply" | "male" | "map" | "map-marked" | "map-marked-alt" | "map-marker" | "map-marker-alt" | "map-pin" | "map-signs" | "marker" | "mars" | "mars-double" | "mars-stroke" | "mars-stroke-h" | "mars-stroke-v" | "mask" | "medal" | "medkit" | "meh" | "meh-blank" | "meh-rolling-eyes" | "memory" | "menorah" | "mercury" | "meteor" | "microchip" | "microphone" | "microphone-alt" | "microphone-alt-slash" | "microphone-slash" | "microscope" | "minus" | "minus-circle" | "minus-square" | "mitten" | "mobile" | "mobile-alt" | "money-bill" | "money-bill-alt" | "money-bill-wave" | "money-bill-wave-alt" | "money-check" | "money-check-alt" | "monument" | "moon" | "mortar-pestle" | "mosque" | "motorcycle" | "mountain" | "mouse" | "mouse-pointer" | "mug-hot" | "music" | "network-wired" | "neuter" | "newspaper" | "not-equal" | "notes-medical" | "object-group" | "object-ungroup" | "oil-can" | "om" | "otter" | "outdent" | "pager" | "paint-brush" | "paint-roller" | "palette" | "pallet" | "paper-plane" | "paperclip" | "parachute-box" | "paragraph" | "parking" | "passport" | "pastafarianism" | "paste" | "pause" | "pause-circle" | "paw" | "peace" | "pen" | "pen-alt" | "pen-fancy" | "pen-nib" | "pen-square" | "pencil-alt" | "pencil-ruler" | "pencil-square-o" | "people-carry" | "pepper-hot" | "percent" | "percentage" | "person-booth" | "phone" | "phone-alt" | "phone-slash" | "phone-square" | "phone-square-alt" | "phone-volume" | "photo-video" | "piggy-bank" | "pills" | "pizza-slice" | "place-of-worship" | "plane" | "plane-arrival" | "plane-departure" | "play" | "play-circle" | "plug" | "plus" | "plus-circle" | "plus-square" | "podcast" | "poll" | "poll-h" | "poo" | "poo-storm" | "poop" | "portrait" | "pound-sign" | "power-off" | "pray" | "praying-hands" | "prescription" | "prescription-bottle" | "prescription-bottle-alt" | "print" | "procedures" | "project-diagram" | "puzzle-piece" | "qrcode" | "question" | "question-circle" | "quidditch" | "quote-left" | "quote-right" | "quran" | "radiation" | "radiation-alt" | "rainbow" | "random" | "receipt" | "record-vinyl" | "recycle" | "redo" | "refresh" | "redo-alt" | "registered" | "remove-format" | "reply" | "reply-all" | "republican" | "restroom" | "retweet" | "ribbon" | "ring" | "road" | "robot" | "rocket" | "route" | "rss" | "rss-square" | "ruble-sign" | "ruler" | "ruler-combined" | "ruler-horizontal" | "ruler-vertical" | "running" | "rupee-sign" | "sad-cry" | "sad-tear" | "satellite" | "satellite-dish" | "save" | "school" | "screwdriver" | "scroll" | "sd-card" | "search" | "search-dollar" | "search-location" | "search-minus" | "search-plus" | "seedling" | "server" | "shapes" | "share" | "share-alt" | "share-alt-square" | "share-square" | "shekel-sign" | "shield-alt" | "ship" | "shipping-fast" | "shoe-prints" | "shopping-bag" | "shopping-basket" | "shopping-cart" | "shower" | "shuttle-van" | "sign" | "sign-in-alt" | "sign-language" | "sign-out" | "sign-out-alt" | "signal" | "signature" | "sim-card" | "sitemap" | "skating" | "skiing" | "skiing-nordic" | "skull" | "skull-crossbones" | "slash" | "sleigh" | "sliders-h" | "smile" | "smile-beam" | "smile-wink" | "smog" | "smoking" | "smoking-ban" | "sms" | "snowboarding" | "snowflake" | "snowman" | "snowplow" | "socks" | "solar-panel" | "sort" | "sort-alpha-down" | "sort-alpha-down-alt" | "sort-alpha-up" | "sort-alpha-up-alt" | "sort-amount-down" | "sort-amount-down-alt" | "sort-amount-up" | "sort-amount-up-alt" | "sort-down" | "sort-numeric-down" | "sort-numeric-down-alt" | "sort-numeric-up" | "sort-numeric-up-alt" | "sort-up" | "spa" | "space-shuttle" | "spell-check" | "spider" | "spinner" | "splotch" | "spray-can" | "square" | "square-full" | "square-root-alt" | "stamp" | "star" | "star-and-crescent" | "star-half" | "star-half-alt" | "star-o" | "star-of-david" | "star-of-life" | "step-backward" | "step-forward" | "stethoscope" | "sticky-note" | "stop" | "stop-circle" | "stopwatch" | "store" | "store-alt" | "stream" | "street-view" | "strikethrough" | "stroopwafel" | "subscript" | "subway" | "suitcase" | "suitcase-rolling" | "sun" | "superscript" | "surprise" | "swatchbook" | "swimmer" | "swimming-pool" | "synagogue" | "sync" | "sync-alt" | "syringe" | "table" | "table-tennis" | "tablet" | "tablet-alt" | "tablets" | "tachometer-alt" | "tag" | "tags" | "tape" | "tasks" | "taxi" | "teeth" | "teeth-open" | "temperature-high" | "temperature-low" | "tenge" | "terminal" | "text-height" | "text-width" | "th" | "th-large" | "th-list" | "theater-masks" | "thermometer" | "thermometer-empty" | "thermometer-full" | "thermometer-half" | "thermometer-quarter" | "thermometer-three-quarters" | "thumbs-down" | "thumbs-up" | "thumbtack" | "ticket-alt" | "times" | "times-circle" | "tint" | "tint-slash" | "tired" | "toggle-off" | "toggle-on" | "toilet" | "toilet-paper" | "toolbox" | "tools" | "tooth" | "torah" | "torii-gate" | "tractor" | "trademark" | "traffic-light" | "train" | "tram" | "transgender" | "transgender-alt" | "trash" | "trash-alt" | "trash-o" | "trash-restore" | "trash-restore-alt" | "tree" | "trophy" | "truck" | "truck-loading" | "truck-monster" | "truck-moving" | "truck-pickup" | "tshirt" | "tty" | "tv" | "umbrella" | "umbrella-beach" | "underline" | "undo" | "undo-alt" | "universal-access" | "university" | "unlink" | "unlock" | "unlock-alt" | "upload" | "user" | "user-alt" | "user-alt-slash" | "user-astronaut" | "user-check" | "user-circle" | "user-clock" | "user-cog" | "user-edit" | "user-friends" | "user-graduate" | "user-injured" | "user-lock" | "user-md" | "user-minus" | "user-ninja" | "user-nurse" | "user-plus" | "user-secret" | "user-shield" | "user-slash" | "user-tag" | "user-tie" | "user-times" | "users" | "users-cog" | "utensil-spoon" | "utensils" | "vector-square" | "venus" | "venus-double" | "venus-mars" | "vial" | "vials" | "video" | "video-slash" | "vihara" | "voicemail" | "volleyball-ball" | "volume-down" | "volume-mute" | "volume-off" | "volume-up" | "vote-yea" | "vr-cardboard" | "walking" | "wallet" | "warehouse" | "water" | "wave-square" | "weight" | "weight-hanging" | "wheelchair" | "wifi" | "wind" | "window-close" | "window-maximize" | "window-minimize" | "window-restore" | "wine-bottle" | "wine-glass" | "wine-glass-alt" | "won-sign" | "wrench" | "x-ray" | "yen-sign" | "yin-yang";
type fabIconKey = "500px" | "accessible-icon" | "accusoft" | "acquisitions-incorporated" | "adn" | "adobe" | "adversal" | "affiliatetheme" | "airbnb" | "algolia" | "alipay" | "amazon" | "amazon-pay" | "amilia" | "android" | "angellist" | "angrycreative" | "angular" | "app-store" | "app-store-ios" | "apper" | "apple" | "apple-pay" | "artstation" | "asymmetrik" | "atlassian" | "audible" | "autoprefixer" | "avianex" | "aviato" | "aws" | "bandcamp" | "battle-net" | "behance" | "behance-square" | "bimobject" | "bitbucket" | "bitcoin" | "bity" | "black-tie" | "blackberry" | "blogger" | "blogger-b" | "bluetooth" | "bluetooth-b" | "bootstrap" | "btc" | "buffer" | "buromobelexperte" | "buy-n-large" | "buysellads" | "canadian-maple-leaf" | "cc-amazon-pay" | "cc-amex" | "cc-apple-pay" | "cc-diners-club" | "cc-discover" | "cc-jcb" | "cc-mastercard" | "cc-paypal" | "cc-stripe" | "cc-visa" | "centercode" | "centos" | "chrome" | "chromecast" | "cloudscale" | "cloudsmith" | "cloudversify" | "codepen" | "codiepie" | "confluence" | "connectdevelop" | "contao" | "cotton-bureau" | "cpanel" | "creative-commons" | "creative-commons-by" | "creative-commons-nc" | "creative-commons-nc-eu" | "creative-commons-nc-jp" | "creative-commons-nd" | "creative-commons-pd" | "creative-commons-pd-alt" | "creative-commons-remix" | "creative-commons-sa" | "creative-commons-sampling" | "creative-commons-sampling-plus" | "creative-commons-share" | "creative-commons-zero" | "critical-role" | "css3" | "css3-alt" | "cuttlefish" | "d-and-d" | "d-and-d-beyond" | "dashcube" | "delicious" | "deploydog" | "deskpro" | "dev" | "deviantart" | "dhl" | "diaspora" | "digg" | "digital-ocean" | "discord" | "discourse" | "dochub" | "docker" | "draft2digital" | "dribbble" | "dribbble-square" | "dropbox" | "drupal" | "dyalog" | "earlybirds" | "ebay" | "edge" | "elementor" | "ello" | "ember" | "empire" | "envira" | "erlang" | "ethereum" | "etsy" | "evernote" | "expeditedssl" | "facebook" | "facebook-f" | "facebook-messenger" | "facebook-square" | "fantasy-flight-games" | "fedex" | "fedora" | "figma" | "firefox" | "first-order" | "first-order-alt" | "firstdraft" | "flickr" | "flipboard" | "fly" | "font-awesome" | "font-awesome-alt" | "font-awesome-flag" | "fonticons" | "fonticons-fi" | "fort-awesome" | "fort-awesome-alt" | "forumbee" | "foursquare" | "free-code-camp" | "freebsd" | "fulcrum" | "galactic-republic" | "galactic-senate" | "get-pocket" | "gg" | "gg-circle" | "git" | "git-alt" | "git-square" | "github" | "github-alt" | "github-square" | "gitkraken" | "gitlab" | "gitter" | "glide" | "glide-g" | "gofore" | "goodreads" | "goodreads-g" | "google" | "google-drive" | "google-play" | "google-plus" | "google-plus-g" | "google-plus-square" | "google-wallet" | "gratipay" | "grav" | "gripfire" | "grunt" | "gulp" | "hacker-news" | "hacker-news-square" | "hackerrank" | "hips" | "hire-a-helper" | "hooli" | "hornbill" | "hotjar" | "houzz" | "html5" | "hubspot" | "imdb" | "instagram" | "intercom" | "internet-explorer" | "invision" | "ioxhost" | "itch-io" | "itunes" | "itunes-note" | "java" | "jedi-order" | "jenkins" | "jira" | "joget" | "joomla" | "js" | "js-square" | "jsfiddle" | "kaggle" | "keybase" | "keycdn" | "kickstarter" | "kickstarter-k" | "korvue" | "laravel" | "lastfm" | "lastfm-square" | "leanpub" | "less" | "line" | "linkedin" | "linkedin-in" | "linode" | "linux" | "lyft" | "magento" | "mailchimp" | "mandalorian" | "markdown" | "mastodon" | "maxcdn" | "mdb" | "medapps" | "medium" | "medium-m" | "medrt" | "meetup" | "megaport" | "mendeley" | "microsoft" | "mix" | "mixcloud" | "mizuni" | "modx" | "monero" | "napster" | "neos" | "nimblr" | "node" | "node-js" | "npm" | "ns8" | "nutritionix" | "odnoklassniki" | "odnoklassniki-square" | "old-republic" | "opencart" | "openid" | "opera" | "optin-monster" | "orcid" | "osi" | "page4" | "pagelines" | "palfed" | "patreon" | "paypal" | "penny-arcade" | "periscope" | "phabricator" | "phoenix-framework" | "phoenix-squadron" | "php" | "pied-piper" | "pied-piper-alt" | "pied-piper-hat" | "pied-piper-pp" | "pinterest" | "pinterest-p" | "pinterest-square" | "playstation" | "product-hunt" | "pushed" | "python" | "qq" | "quinscape" | "quora" | "r-project" | "raspberry-pi" | "ravelry" | "react" | "reacteurope" | "readme" | "rebel" | "red-river" | "reddit" | "reddit-alien" | "reddit-square" | "redhat" | "renren" | "replyd" | "researchgate" | "resolving" | "rev" | "rocketchat" | "rockrms" | "safari" | "salesforce" | "sass" | "schlix" | "scribd" | "searchengin" | "sellcast" | "sellsy" | "servicestack" | "shirtsinbulk" | "shopware" | "simplybuilt" | "sistrix" | "sith" | "sketch" | "skyatlas" | "skype" | "slack" | "slack-hash" | "slideshare" | "snapchat" | "snapchat-ghost" | "snapchat-square" | "soundcloud" | "sourcetree" | "speakap" | "speaker-deck" | "spotify" | "squarespace" | "stack-exchange" | "stack-overflow" | "stackpath" | "staylinked" | "steam" | "steam-square" | "steam-symbol" | "sticker-mule" | "strava" | "stripe" | "stripe-s" | "studiovinari" | "stumbleupon" | "stumbleupon-circle" | "superpowers" | "supple" | "suse" | "swift" | "symfony" | "teamspeak" | "telegram" | "telegram-plane" | "tencent-weibo" | "the-red-yeti" | "themeco" | "themeisle" | "think-peaks" | "trade-federation" | "trello" | "tripadvisor" | "tumblr" | "tumblr-square" | "twitch" | "twitter" | "twitter-square" | "typo3" | "uber" | "ubuntu" | "uikit" | "umbraco" | "uniregistry" | "untappd" | "ups" | "usb" | "usps" | "ussunnah" | "vaadin" | "viacoin" | "viadeo" | "viadeo-square" | "viber" | "vimeo" | "vimeo-square" | "vimeo-v" | "vine" | "vk" | "vnv" | "vuejs" | "waze" | "weebly" | "weibo" | "weixin" | "whatsapp" | "whatsapp-square" | "whmcs" | "wikipedia-w" | "windows" | "wix" | "wizards-of-the-coast" | "wolf-pack-battalion" | "wordpress" | "wordpress-simple" | "wpbeginner" | "wpexplorer" | "wpforms" | "wpressr" | "xbox" | "xing" | "xing-square" | "y-combinator" | "yahoo" | "yammer" | "yandex" | "yandex-international" | "yarn" | "yelp" | "yoast" | "youtube" | "youtube-square" | "zhihu";

type DialogType = "bs3" | "bs4" | "bs5" | "jqueryui" | "panel";
/**
 * Options that apply to all dialog types
 */
interface DialogOptions {
    /** True to open dialog as panel */
    asPanel?: boolean;
    /** True to auto open. Ignored for message dialogs. */
    autoOpen?: boolean;
    /** Prefer Bootstrap modals to jQuery UI dialogs when both are available */
    bootstrap?: boolean;
    /** List of buttons to show on the dialog */
    buttons?: DialogButton[];
    /** Show close button, default is true */
    closeButton?: boolean;
    /** CSS class to use for all dialog types. Is added to the top ui-dialog, panel or modal element */
    dialogClass?: string;
    /** Dialog content element, or callback that will populate the content */
    element?: HTMLElement | ((element: HTMLElement) => void);
    /** Is modal dialog, default is true, only used for jQuery UI */
    modal?: boolean;
    /** Additional CSS class to use only for BS modals, like modal-lg etc. */
    modalClass?: string;
    /** Event handler that is called when dialog is opened */
    onOpen?: () => void;
    /** Event handler that is called when dialog is closed */
    onClose?: (result: string) => void;
    /** Callback to get options specific to the dialog provider type */
    providerOptions?: (type: DialogType, opt: DialogOptions) => any;
    /** Dialog title */
    title?: string;
    /** Dialog width. Only used for jQuery UI dialogs */
    width?: number;
}
interface ICommonDialog {
    /** Gets dialog provider type used */
    readonly type: DialogType;
    /** Opens the dialog */
    open(): void;
    /** Closes the dialog */
    close(result?: string): void;
    /** Sets the title of the dialog */
    title: string;
    /** Dispose the dialog instance */
    dispose(): void;
    /** The result code of the button that is clicked */
    readonly result?: string;
}
/**
 * Options for a message dialog button
 */
interface DialogButton {
    /** Button text */
    text?: string;
    /** Button hint */
    hint?: string;
    /** Button icon */
    icon?: IconClassName;
    /** Click handler */
    click?: (e: MouseEvent) => void;
    /** CSS class for button */
    cssClass?: string;
    /** HTML encode button text. Default is true. */
    htmlEncode?: boolean;
    /** The code that is returned from message dialog function when this button is clicked.
     *  If this is set, and click event will not be defaultPrevented dialog will close.
     */
    result?: string;
}
/**
 * Options that apply to all message dialog types
 */
interface MessageDialogOptions extends DialogOptions {
    /** HTML encode the message, default is true */
    htmlEncode?: boolean;
    /** Wrap the message in a `<pre>` element, so that line endings are preserved, default is true */
    preWrap?: boolean;
}
/** Returns true if Bootstrap 3 is loaded */
declare function isBS3(): boolean;
/** Returns true if Bootstrap 5+ is loaded */
declare function isBS5Plus(): boolean;
declare namespace DialogTexts {
    const AlertTitle: string;
    const CancelButton: string;
    const CloseButton: string;
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
declare function createCommonDialog(options: DialogOptions): ICommonDialog;
/** Converts a `DialogButton` declaration to Bootstrap button element
 * @param x Dialog button declaration
 * @returns Bootstrap button element
*/
declare function dialogButtonToBS(x: DialogButton): HTMLButtonElement;
/** Converts a `DialogButton` declaration to jQuery UI button type
 * @param x Dialog button declaration
 * @returns jQuery UI button type
 */
declare function dialogButtonToUI(x: DialogButton): any;
declare function okDialogButton(opt?: DialogButton): DialogButton;
declare function yesDialogButton(opt?: DialogButton): DialogButton;
declare function noDialogButton(opt?: DialogButton): DialogButton;
declare function cancelDialogButton(opt?: DialogButton): DialogButton;
/**
 * Displays an alert dialog
 * @param message The message to display
 * @param options Additional options.
 * @see AlertOptions
 * @example
 * alertDialog("An error occured!"); }
 */
declare function alertDialog(message: string, options?: MessageDialogOptions): Partial<ICommonDialog>;
/** Additional options for confirm dialog */
interface ConfirmDialogOptions extends MessageDialogOptions {
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
declare function confirmDialog(message: string, onYes: () => void, options?: ConfirmDialogOptions): Partial<ICommonDialog>;
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
declare function informationDialog(message: string, onOk?: () => void, options?: MessageDialogOptions): Partial<ICommonDialog>;
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
declare function successDialog(message: string, onOk?: () => void, options?: MessageDialogOptions): Partial<ICommonDialog>;
/**
 * Display a warning dialog
 * @param message The message to display
 * @param options Additional options.
 * @see MessageDialogOptions
 * @example
 * warningDialog("Something is odd!");
 */
declare function warningDialog(message: string, options?: MessageDialogOptions): Partial<ICommonDialog>;
/** Options for `iframeDialog` **/
interface IFrameDialogOptions {
    html?: string;
}
/**
 * Display a dialog that shows an HTML block in an IFRAME, which is usually returned from server callbacks
 * @param options The options
 */
declare function iframeDialog(options: IFrameDialogOptions): Partial<ICommonDialog>;
/**
 * Closes a panel, triggering panelbeforeclose and panelclose events on the panel element.
 * If the panelbeforeclose prevents the default, the operation is cancelled.
 * @param element The panel element
 * @param e  The event triggering the close
 */
declare function closePanel(el: (HTMLElement | ArrayLike<HTMLElement>)): void;
/**
 * Opens a panel, triggering panelbeforeopen and panelopen events on the panel element,
 * and panelopening and panelopened events on the window.
 * If the panelbeforeopen prevents the default, the operation is cancelled.
 * @param element The panel element
 * @param uniqueName A unique name for the panel. If not specified, the panel id is used. If the panel has no id, a timestamp is used.
 * @param e The event triggering the open
 */
declare function openPanel(element: HTMLElement | ArrayLike<HTMLElement>, uniqueName?: string): void;
/** Returns .s-Panel, .modal, .ui-dialog */
declare function getDialogRootElement(element: HTMLElement | ArrayLike<HTMLElement>): HTMLElement;
/** Returns .panel-body, .modal-body, .ui-dialog-content */
declare function getDialogBodyElement(element: HTMLElement | ArrayLike<HTMLElement>): HTMLElement;
/** Returns .s-Panel, .modal, .ui-dialog-content */
declare function getDialogEventTarget(element: HTMLElement | ArrayLike<HTMLElement>): HTMLElement;
/** Tries to close a ui-dialog, panel or modal */
declare function closeDialog(element: HTMLElement | ArrayLike<HTMLElement>): void;
declare function attachToDialogBeforeCloseEvent(element: HTMLElement | ArrayLike<HTMLElement>, handler: (e: Event) => void): void;
declare function attachToDialogCloseEvent(element: HTMLElement | ArrayLike<HTMLElement>, handler: (e: Event) => void): void;

interface ServiceError {
    Code?: string;
    Arguments?: string;
    Message?: string;
    Details?: string;
    ErrorId?: string;
}
interface ServiceResponse {
    Error?: ServiceError;
}
interface ServiceRequest {
}
interface SaveRequest<TEntity> extends ServiceRequest {
    EntityId?: any;
    Entity?: TEntity;
    Localizations?: any;
}
interface SaveRequestWithAttachment<TEntity> extends SaveRequest<TEntity> {
    Attachments?: any[];
}
interface SaveResponse extends ServiceResponse {
    EntityId?: any;
}
interface SaveWithLocalizationRequest<TEntity> extends SaveRequest<TEntity> {
    Localizations?: {
        [key: string]: TEntity;
    };
}
interface DeleteRequest extends ServiceRequest {
    EntityId?: any;
}
interface DeleteResponse extends ServiceResponse {
}
interface UndeleteRequest extends ServiceRequest {
    EntityId?: any;
}
interface UndeleteResponse extends ServiceResponse {
}
declare enum ColumnSelection {
    List = 0,
    KeyOnly = 1,
    Details = 2,
    None = 3,
    IdOnly = 4,
    Lookup = 5
}
declare enum RetrieveColumnSelection {
    details = 0,
    keyOnly = 1,
    list = 2,
    none = 3,
    idOnly = 4,
    lookup = 5
}
interface ListRequest extends ServiceRequest {
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
}
interface ListResponse<TEntity> extends ServiceResponse {
    Entities?: TEntity[];
    Values?: any[];
    TotalCount?: number;
    Skip?: number;
    Take?: number;
}
interface RetrieveRequest extends ServiceRequest {
    EntityId?: any;
    ColumnSelection?: RetrieveColumnSelection;
    IncludeColumns?: string[];
    ExcludeColumns?: string[];
}
interface RetrieveResponse<TEntity> extends ServiceResponse {
    Entity?: TEntity;
}
interface RetrieveLocalizationRequest extends RetrieveRequest {
}
interface RetrieveLocalizationResponse<TEntity> extends ServiceResponse {
    Entities?: {
        [key: string]: TEntity;
    };
}
interface RequestErrorInfo {
    status?: number;
    statusText?: string;
    responseText?: string;
}
interface ServiceOptions<TResponse extends ServiceResponse> extends RequestInit {
    allowRedirect?: boolean;
    async?: boolean;
    blockUI?: boolean;
    headers?: Record<string, string>;
    request?: any;
    service?: string;
    url?: string;
    onCleanup?(): void;
    /** Should return true if the error is handled (e.g. notification shown). Otherwise the error may be shown twice. */
    onError?(response: TResponse, info?: RequestErrorInfo): void | boolean;
    onSuccess?(response: TResponse): void;
}

declare namespace ErrorHandling {
    /**
     * Shows a service error as an alert dialog. If the error
     * is null, has no message or code, it shows "??ERROR??".
     */
    function showServiceError(error: ServiceError, errorInfo?: RequestErrorInfo): void;
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
}

interface LookupOptions<TItem> {
    idField?: string;
    parentIdField?: string;
    textField?: string;
}
interface Lookup<TItem> {
    items: TItem[];
    itemById: {
        [key: string]: TItem;
    };
    idField: string;
    parentIdField: string;
    textField: string;
}
declare class Lookup<TItem> {
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

declare enum SummaryType {
    Disabled = -1,
    None = 0,
    Sum = 1,
    Avg = 2,
    Min = 3,
    Max = 4
}
interface PropertyItem {
    name?: string;
    title?: string;
    hint?: string;
    placeholder?: string;
    editorType?: string;
    editorParams?: any;
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
    formatterType?: string;
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
interface PropertyItemsData {
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
declare function getScriptDataHash(name: string, reload?: boolean): string;
/**
 * Fetches a script data with given name via ~/DynamicData endpoint
 * @param name Dynamic script name
 * @returns A promise that will return data if successfull
 */
declare function fetchScriptData<TData>(name: string): Promise<TData>;
/**
 * Returns the script data from cache if available, or via a fetch
 * request to ~/DynamicData endpoint
 * @param name
 * @param reload Clear cache and force reload
 * @returns
 */
declare function getScriptData<TData = any>(name: string, reload?: boolean): Promise<TData>;
/**
 * Gets or loads a [ColumnsScript] data
 * @param key Form key
 * @returns A property items data object containing items and additionalItems properties
 */
declare function getColumnsScript(key: string): Promise<PropertyItemsData>;
/**
 * Gets or loads a [FormScript] data
 * @param key Form key
 * @returns A property items data object containing items and additionalItems properties
 */
declare function getFormScript(key: string): Promise<PropertyItemsData>;
/**
 * Gets or loads a Lookup
 * @param key Lookup key
 */
declare function getLookupAsync<TItem>(key: string): Promise<Lookup<TItem>>;
/**
 * Gets or loads a [RemoteData]
 * @param key Remote data key
 */
declare function getRemoteDataAsync<TData = any>(key: string): Promise<TData>;
/**
 * Shows a suitable error message for errors occured during loading of
 * a dynamic script data.
 * @param name Name of the dynamic script
 * @param status HTTP status returned if available
 * @param statusText HTTP status text returned if available
 */
declare function handleScriptDataError(name: string, status?: number, statusText?: string, shouldThrow?: boolean): string;
declare function peekScriptData(name: string): any;
/**
 * Forces reload of a lookup from the server. Note that only the
 * client side cache is cleared. This does not force reloading in the server-side.
 * @param key Lookup key
 * @returns Lookup
 */
declare function reloadLookupAsync<TItem = any>(key: string): Promise<Lookup<TItem>>;
declare function setRegisteredScripts(scripts: any[]): void;
declare function setScriptData(name: string, value: any): void;

/**
 * Interface for number formatting, similar to .NET's NumberFormatInfo
 */
interface NumberFormat {
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
interface DateFormat {
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
interface Locale extends NumberFormat, DateFormat {
    /** Locale string comparison function, similar to .NET's StringComparer */
    stringCompare?: (a: string, b: string) => number;
    /** Locale string to upper case function */
    toUpper?: (a: string) => string;
}
/** Invariant locale (e.g. CultureInfo.InvariantCulture) */
declare let Invariant: Locale;
/**
 * Factory for a function that compares two strings, based on a character order
 * passed in the `order` argument.
 */
declare function compareStringFactory(order: string): ((a: string, b: string) => number);
/**
 * Current culture, e.g. CultureInfo.CurrentCulture. This is overridden by
 * settings passed from a `<script>` element in the page with id `ScriptCulture`
 * containing a JSON object if available. This element is generally created in
 * the _LayoutHead.cshtml file for Serenity applications, so that the culture
 * settings determined server, can be passed to the client.
 */
declare let Culture: Locale;
/**
 * Formats a string with parameters similar to .NET's String.Format function
 * using current `Culture` locale settings.
 */
declare function stringFormat(format: string, ...prm: any[]): string;
/**
 * Formats a string with parameters similar to .NET's String.Format function
 * using the locale passed as the first argument.
 */
declare function stringFormatLocale(l: Locale, format: string, ...prm: any[]): string;
/**
 * Rounds a number to specified digits or an integer number if digits are not specified.
 * @param n the number to round
 * @param d the number of digits to round to. default is zero.
 * @param rounding whether to use banker's rounding
 * @returns the rounded number
 */
declare let round: (n: number, d?: number, rounding?: boolean) => number;
/**
 * Truncates a number to an integer number.
 */
declare let trunc: (n: number) => number;
/**
 * Formats a number using the current `Culture` locale (or the passed locale) settings.
 * It supports format specifiers similar to .NET numeric formatting strings.
 * @param num the number to format
 * @param format the format specifier. default is 'g'.
 * See .NET numeric formatting strings documentation for more information.
 */
declare function formatNumber(num: number, format?: string, decOrLoc?: string | NumberFormat, grp?: string): string;
/**
 * Converts a string to an integer. The difference between parseInt and parseInteger
 * is that parseInteger will return null if the string is empty or null, whereas
 * parseInt will return NaN and parseInteger will use the current culture's group
 * and decimal separators.
 * @param s the string to parse
 */
declare function parseInteger(s: string): number;
/**
 * Converts a string to a decimal. The difference between parseFloat and parseDecimal
 * is that parseDecimal will return null if the string is empty or null, whereas
 * parseFloat will return NaN and parseDecimal will use the current culture's group
 * and decimal separators.
 * @param s the string to parse
 */
declare function parseDecimal(s: string): number;
/**
 * Converts a string to an ID. If the string is a number, it is returned as-is.
 * If the string is empty, null or whitespace, null is returned.
 * Otherwise, it is converted to a number if possible. If the string is not a
 * valid number or longer than 14 digits, the trimmed string is returned as-is.
 * @param id the string to convert to an ID
 */
declare function toId(id: any): any;
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
declare function formatDate(d: Date | string, format?: string, locale?: Locale): string;
/**
 * Formats a date as the ISO 8601 UTC date/time format.
 * @param n The number of minutes.
 */
declare function formatISODateTimeUTC(d: Date): string;
/**
 * Parses a string in the ISO 8601 UTC date/time format.
 * @param s The string to parse.
 */
declare function parseISODateTime(s: string): Date;
/**
 * Parses a string to a date. If the string is empty or whitespace, returns null.
 * Returns a NaN Date if the string is not a valid date.
 * @param s The string to parse.
 * @param dateOrder The order of the date parts in the string. Defaults to culture's default date order.
  */
declare function parseDate(s: string, dateOrder?: string): Date;
/**
 * Splits a date string into an array of strings, each containing a single date part.
 * It can handle separators "/", ".", "-" and "\".
 * @param s The string to split.
 */
declare function splitDateString(s: string): string[];

declare namespace EventHandler {
    function on<K extends keyof HTMLElementEventMap>(element: EventTarget, type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): void;
    function on(element: EventTarget, type: string, listener: EventListener): void;
    function on(element: EventTarget, type: string, selector: string, delegationHandler: Function): void;
    function one<K extends keyof HTMLElementEventMap>(element: EventTarget, type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): void;
    function one(element: EventTarget, type: string, listener: EventListener): void;
    function one(element: EventTarget, type: string, selector: string, delegationHandler: Function): void;
    function off<K extends keyof HTMLElementEventMap>(element: EventTarget, type: K, listener?: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): void;
    function off(element: EventTarget, type: string, listener?: EventListener): void;
    function off(element: EventTarget, type: string, selector?: string, delegationHandler?: Function): void;
    function trigger(element: EventTarget, type: string, args?: any): Event & {
        isDefaultPrevented?(): boolean;
    };
}

/**
 * Html encodes a string (encodes single and double quotes, & (ampersand), > and < characters)
 * @param s String (or number etc.) to be HTML encoded
 */
declare function htmlEncode(s: any): string;
/**
 * Toggles the class on the element handling spaces like addClass does.
 * @param el the element
 * @param cls the class to toggle
 * @param add if true, the class will be added, if false the class will be removed, otherwise it will be toggled.
 */
declare function toggleClass(el: Element, cls: string, add?: boolean): void;
declare function addClass(el: Element, cls: string): void;
declare function removeClass(el: Element, cls: string): void;
declare function toClassName(value: string | boolean | (string | boolean)[]): string;
declare function isInputLike(node: Element): node is (HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement | HTMLButtonElement);
declare const inputLikeSelector = "input,select,textarea,button";
declare function isInputTag(tag: string): boolean;
interface Fluent<TElement extends HTMLElement = HTMLElement> extends ArrayLike<TElement> {
    addClass(value: string | boolean | (string | boolean)[]): this;
    append(child: string | Node | Fluent<any>): this;
    appendTo(parent: Element | Fluent<any>): this;
    attr(name: string): string;
    attr(name: string, value: string | number | boolean | null | undefined): this;
    children(selector?: string): HTMLElement[];
    closest(selector: string): Fluent<HTMLElement>;
    data(name: string): string;
    data(name: string, value: string): this;
    getNode(): TElement;
    empty(): this;
    findFirst(selector: string): Fluent<HTMLElement>;
    findAll(selector: string): HTMLElement[];
    hasClass(klass: string): boolean;
    hide(): this;
    html(): string;
    html(value: string): this;
    insertAfter(referenceNode: HTMLElement | Fluent<HTMLElement>): this;
    insertBefore(referenceNode: HTMLElement | Fluent<HTMLElement>): this;
    readonly [n: number]: TElement;
    readonly length: number;
    off<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): this;
    off(type: string, listener: EventListener): this;
    off(type: string, selector: string, delegationHandler: Function): this;
    on<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): this;
    on(type: string, listener: EventListener): this;
    on(type: string, selector: string, delegationHandler: Function): this;
    one<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): this;
    one(type: string, listener: EventListener): this;
    one(type: string, selector: string, delegationHandler: Function): this;
    parent(): Fluent<HTMLElement>;
    prepend(child: string | Node | Fluent<any>): this;
    prependTo(parent: Element | Fluent<any>): this;
    remove(): this;
    removeAttr(name: string): this;
    removeClass(value: string | boolean | (string | boolean)[]): this;
    show(): this;
    text(): string;
    text(value: string): this;
    toggle(flag?: boolean): this;
    toggleClass(value: (string | boolean | (string | boolean)[]), add?: boolean): this;
    trigger(type: string, args?: any): this;
    val(value: string): this;
    val(): string;
}
declare function Fluent<K extends keyof HTMLElementTagNameMap>(tag: K): Fluent<HTMLElementTagNameMap[K]>;
declare function Fluent<TElement extends HTMLElement>(element: TElement): Fluent<TElement>;
declare function Fluent(element: EventTarget): Fluent<HTMLElement>;
declare namespace Fluent {
    var off: typeof EventHandler.off;
    var on: typeof EventHandler.on;
    var one: typeof EventHandler.on;
    var trigger: typeof EventHandler.trigger;
    var ready: (callback: () => void) => void;
}
declare function H<K extends keyof HTMLElementTagNameMap>(tag: K): Fluent<HTMLElementTagNameMap[K]>;

declare function addLocalText(obj: string | Record<string, string | Record<string, any>> | string, pre?: string): void;
declare function localText(key: string, defaultText?: string): string;
declare function tryGetText(key: string): string;
declare function proxyTexts(o: Record<string, any>, p: string, t: Record<string, any>): Object;

type ToastContainerOptions = {
    containerId?: string;
    positionClass?: string;
    target?: string;
};
type ToastrOptions = ToastContainerOptions & {
    tapToDismiss?: boolean;
    toastClass?: string;
    showDuration?: number;
    onShown?: () => void;
    hideDuration?: number;
    onHidden?: () => void;
    closeMethod?: boolean;
    closeDuration?: number | false;
    closeEasing?: boolean;
    closeOnHover?: boolean;
    extendedTimeOut?: number;
    iconClass?: string;
    positionClass?: string;
    timeOut?: number;
    titleClass?: string;
    messageClass?: string;
    escapeHtml?: boolean;
    target?: string;
    closeHtml?: string;
    closeClass?: string;
    newestOnTop?: boolean;
    preventDuplicates?: boolean;
    onclick?: (event: MouseEvent) => void;
    onCloseClick?: (event: Event) => void;
    closeButton?: boolean;
    rtl?: boolean;
};
type NotifyMap = {
    type: string;
    iconClass: string;
    title?: string;
    message?: string;
};
declare class Toastr {
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

declare let defaultNotifyOptions: ToastrOptions;
declare function positionToastContainer(options?: ToastrOptions, create?: boolean): void;
declare function notifyError(message: string, title?: string, options?: ToastrOptions): void;
declare function notifyInfo(message: string, title?: string, options?: ToastrOptions): void;
declare function notifySuccess(message: string, title?: string, options?: ToastrOptions): void;
declare function notifyWarning(message: string, title?: string, options?: ToastrOptions): void;

declare function resolveUrl(url: string): string;
declare function resolveServiceUrl(url: string): string;
declare function getCookie(name: string): any;
declare function isSameOrigin(url: string): boolean;
declare function requestStarting(): void;
declare function requestFinished(): void;
declare function getActiveRequests(): number;
declare function serviceCall<TResponse extends ServiceResponse>(options: ServiceOptions<TResponse>): PromiseLike<TResponse>;
declare function serviceRequest<TResponse extends ServiceResponse>(service: string, request?: any, onSuccess?: (response: TResponse) => void, options?: ServiceOptions<TResponse>): PromiseLike<TResponse>;

declare const typeInfoProperty = "typeInfo";
type StringLiteral<T> = T extends string ? string extends T ? never : T : never;
type TypeInfo<T> = {
    typeKind: "class" | "enum" | "interface" | "editor" | "formatter";
    typeName: StringLiteral<T> | (string & {});
    interfaces?: any[];
    customAttributes?: any[];
    enumFlags?: boolean;
    registered?: boolean;
};
declare function getTypeRegistry(): any;
declare function getTypeNameProp(type: any): string;
declare function setTypeNameProp(type: any, value: string): void;

declare function getGlobalObject(): any;
type Type = Function | Object;
declare function getNested(from: any, name: string): any;
declare function getType(name: string, target?: any): Type;
declare function getTypeFullName(type: Type): string;
declare function getTypeShortName(type: Type): string;
declare function getInstanceType(instance: any): any;
declare function isAssignableFrom(target: any, type: Type): any;
declare function isInstanceOfType(instance: any, type: Type): any;
declare function getBaseType(type: any): any;
declare function registerClass(type: any, name: string, intf?: any[]): void;
declare function registerEnum(type: any, name: string, enumKey?: string): void;
declare function registerInterface(type: any, name: string, intf?: any[]): void;
declare namespace Enum {
    let toString: (enumType: any, value: number) => string;
    let getValues: (enumType: any) => any[];
}
declare const isEnum: (type: any) => boolean;
declare function initFormType(typ: Function, nameWidgetPairs: any[]): void;
declare function fieldsProxy<TRow>(): Readonly<Record<keyof TRow, string>>;
declare function isArrayLike(obj: any): obj is ArrayLike<any>;
declare function isPromiseLike(obj: any): obj is PromiseLike<any>;
declare function getjQuery(): any;
type NoInfer<T> = [T][T extends any ? 0 : never];
declare class EditorAttribute {
}
declare class ISlickFormatter {
}
declare function registerFormatter(type: any, name: string, intf?: any[]): void;
declare function registerEditor(type: any, name: string, intf?: any[]): void;
declare function addCustomAttribute(type: any, attr: any): void;
declare function getCustomAttribute<TAttr>(type: any, attrType: {
    new (...args: any[]): TAttr;
}, inherit?: boolean): TAttr;
declare function hasCustomAttribute<TAttr>(type: any, attrType: {
    new (...args: any[]): TAttr;
}, inherit?: boolean): boolean;
declare function getCustomAttributes<TAttr>(type: any, attrType: {
    new (...args: any[]): TAttr;
}, inherit?: boolean): TAttr[];
type ClassTypeInfo<T> = TypeInfo<T>;
type EditorTypeInfo<T> = TypeInfo<T>;
type FormatterTypeInfo<T> = TypeInfo<T>;
type InterfaceTypeInfo<T> = TypeInfo<T>;
declare function classTypeInfo<T>(typeName: StringLiteral<T>, interfaces?: any[]): ClassTypeInfo<T>;
declare function editorTypeInfo<T>(typeName: StringLiteral<T>, interfaces?: any[]): EditorTypeInfo<T>;
declare function formatterTypeInfo<T>(typeName: StringLiteral<T>, interfaces?: any[]): FormatterTypeInfo<T>;
declare function interfaceTypeInfo<T>(typeName: StringLiteral<T>, interfaces?: any[]): InterfaceTypeInfo<T>;
declare function registerType(type: {
    [typeInfoProperty]: TypeInfo<any>;
    name: string;
}): void;

/**
 * Tests if any of array elements matches given predicate. Prefer Array.some() over this function (e.g. `[1, 2, 3].some(predicate)`).
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 * @returns True if any element matches.
 */
declare function any<TItem>(array: TItem[], predicate: (x: TItem) => boolean): boolean;
/**
 * Counts number of array elements that matches a given predicate.
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 */
declare function count<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number;
/**
 * Gets first element in an array that matches given predicate similar to LINQ's First.
 * Throws an error if no match is found.
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 * @returns First element that matches.
 */
declare function first<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;
/**
 * A group item returned by `groupBy()`.
 */
type GroupByElement<TItem> = {
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
type GroupByResult<TItem> = {
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
declare function groupBy<TItem>(items: TItem[], getKey: (x: TItem) => any): GroupByResult<TItem>;
/**
 * Gets index of first element in an array that matches given predicate.
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 */
declare function indexOf<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number;
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
declare function insert(obj: any, index: number, item: any): void;
/**
 * Determines if the object is an array. Prefer Array.isArray over this function (e.g. `Array.isArray(obj)`).
 * @param obj Object to test.
 * @returns True if the object is an array.
 * @example
 * isArray([1, 2, 3]); // true
 * isArray({}); // false
 */
declare const isArray: (arg: any) => arg is any[];
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
declare function single<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;
type Grouping<TItem> = {
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
declare function toGrouping<TItem>(items: TItem[], getKey: (x: TItem) => any): Grouping<TItem>;
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
declare function tryFirst<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;

interface UserDefinition {
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
declare namespace Authorization {
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
declare namespace Authorization {
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

/** @deprecated use alertDialog */
declare const alert: typeof alertDialog;
/** @deprecated use confirmDialog */
declare const confirm: typeof confirmDialog;
/** @deprecated use informationDialog */
declare const information: typeof informationDialog;
/** @deprecated use successDialog */
declare const success: typeof successDialog;
/** @deprecated use warningDialog */
declare const warning: typeof warningDialog;

/**
 * A string to lowercase function that handles special Turkish
 * characters like 'ı'. Left in for compatibility reasons.
 */
declare function turkishLocaleToLower(a: string): string;
/**
 * A string to uppercase function that handles special Turkish
 * characters like 'ı'. Left in for compatibility reasons.
 */
declare function turkishLocaleToUpper(a: string): string;
/**
 * This is an alias for Culture.stringCompare, left in for compatibility reasons.
 */
declare let turkishLocaleCompare: (a: string, b: string) => number;
/** @deprecated Use stringFormat */
declare let format: typeof stringFormat;
/** @deprecated Use stringFormatLocale */
declare let localeFormat: typeof stringFormatLocale;
/**
 * Formats a number containing number of minutes into a string in the format "d.hh:mm".
 * @param n The number of minutes.
 */
declare function formatDayHourAndMin(n: number): string;
/**
 * Parses a time string in the format "hh:mm" into a number containing number of minutes.
 * Returns NaN if the hours not in range 0-23 or minutes not in range 0-59.
 * @param s The string to parse.
 */
declare function parseHourAndMin(value: string): number;
/**
 * Parses a string in the format "d.hh:mm" into a number containing number of minutes.
 * Returns NaN if the hours not in range 0-23 or minutes not in range 0-59.
 * Returns NULL if the string is empty or whitespace.
 */
declare function parseDayHourAndMin(s: string): number;

/**
 * Adds an empty option to the select.
 * @param select the select element
 */
declare function addEmptyOption(select: ArrayLike<HTMLElement> | HTMLSelectElement): void;
/**
 * Adds an option to the select.
 */
declare function addOption(select: ArrayLike<HTMLElement> | HTMLSelectElement, key: string, text: string): void;
/** @deprecated use htmlEncode as it also encodes quotes */
declare const attrEncode: typeof htmlEncode;
/** Clears the options in the select element */
declare function clearOptions(select: HTMLElement | ArrayLike<HTMLElement>): void;
/**
 * Finds the first element with the given relative id to the source element.
 * It can handle underscores in the source element id.
 * @param element the source element
 * @param relativeId the relative id to the source element
 * @param context the context element (optional)
 * @returns the element with the given relative id to the source element.
 */
declare function findElementWithRelativeId(element: HTMLElement | ArrayLike<HTMLElement>, relativeId: string, context?: HTMLElement): HTMLElement;
/**
 * Creates a new DIV and appends it to the body.
 * @returns the new DIV element.
 */
declare function newBodyDiv(): HTMLDivElement;
/**
 * Returns the outer HTML of the element.
 */
declare function outerHtml(element: Element | ArrayLike<HTMLElement>): string;
/**
 * Appends child at first argument to given node at second argument.
 * From https://github.com/alex-kinokon/jsx-dom.
 * @param child Child element or elements
 * @param node Target parent element
 */
declare function appendChild(child: any, node: HTMLElement): void;

declare function getWidgetName(type: Function): string;
declare function associateWidget(widget: {
    domNode: HTMLElement;
}): void;
declare function deassociateWidget(widget: {
    domNode: HTMLElement;
}): void;
declare function tryGetWidget<TWidget>(element: Element | ArrayLike<HTMLElement> | string, type?: {
    new (...args: any[]): TWidget;
}): TWidget;
declare function getWidgetFrom<TWidget>(element: ArrayLike<HTMLElement> | Element | string, type?: {
    new (...args: any[]): TWidget;
}): TWidget;
declare module "@serenity-is/base" {
    interface Fluent<TElement extends HTMLElement> {
        getWidget<TWidget>(type?: {
            new (...args: any[]): TWidget;
        }): TWidget;
        tryGetWidget<TWidget>(type?: {
            new (...args: any[]): TWidget;
        }): TWidget;
    }
}
type IdPrefixType = {
    [key: string]: string;
    Form: string;
    Tabs: string;
    Toolbar: string;
    PropertyGrid: string;
};
declare function useIdPrefix(prefix: string): IdPrefixType;
type WidgetProps<P> = {
    id?: string;
    class?: string;
    element?: ((el: HTMLElement) => void) | HTMLElement | ArrayLike<HTMLElement> | string;
} & NoInfer<P>;
type EditorProps<T> = WidgetProps<T> & {
    initialValue?: any;
    maxLength?: number;
    name?: string;
    placeholder?: string;
    required?: boolean;
    readOnly?: boolean;
};

declare class Widget<P = {}> {
    static typeInfo: ClassTypeInfo<"Serenity.Widget">;
    private static nextWidgetNumber;
    protected readonly options: WidgetProps<P>;
    protected readonly uniqueName: string;
    readonly idPrefix: string;
    readonly domNode: HTMLElement;
    constructor(props: WidgetProps<P>);
    destroy(): void;
    static createDefaultElement(): HTMLElement;
    /**
     * @deprecated
     * Prefer domNode as this one returns a Fluent object emulating jQuery
     */
    get element(): Fluent;
    protected addCssClass(): void;
    protected getCssClass(): string;
    static getWidgetName(type: Function): string;
    addValidationRule(rule: (input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement) => string, uniqueName?: string): void;
    addValidationRule(uniqueName: string, rule: (input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement) => string): void;
    getFieldElement(): HTMLElement;
    getGridField(): Fluent;
    change(handler: (e: Event) => void): void;
    changeSelect2(handler: (e: Event) => void): void;
    static create<TWidget extends Widget<P>, P>(params: CreateWidgetParams<TWidget, P>): TWidget;
    protected getCustomAttribute<TAttr>(attrType: {
        new (...args: any[]): TAttr;
    }, inherit?: boolean): TAttr;
    protected internalInit(): void;
    init(): this;
    /**
     * Returns the main element for this widget or the document fragment.
     * As widgets may get their elements from props unlike regular JSX widgets,
     * this method should not be overridden. Override renderContents() instead.
     */
    render(): HTMLElement | DocumentFragment;
    protected internalRenderContents(): void;
    protected renderContents(): any | void;
    get props(): WidgetProps<P>;
    protected syncOrAsyncThen<T>(syncMethod: (() => T), asyncMethod: (() => PromiseLike<T>), then: (v: T) => void): void;
    protected useIdPrefix(): IdPrefixType;
}
declare class EditorWidget<P> extends Widget<EditorProps<P>> {
    static typeInfo: ClassTypeInfo<"Serenity.EditorWidget">;
    constructor(props: EditorProps<P>);
}
interface CreateWidgetParams<TWidget extends Widget<P>, P> {
    type?: {
        new (options?: P): TWidget;
        prototype: TWidget;
    };
    options?: P & WidgetProps<{}>;
    container?: HTMLElement | ArrayLike<HTMLElement>;
    element?: (e: Fluent) => void;
    init?: (w: TWidget) => void;
}

declare function GridPageInit<TGrid extends Widget<P>, P>({ type, props }: {
    type: CreateWidgetParams<TGrid, P>["type"];
    props?: WidgetProps<P>;
}): HTMLElement;
declare function PanelPageInit<TPanel extends Widget<P>, P>({ type, props }: {
    type: CreateWidgetParams<TPanel, P>["type"];
    props?: WidgetProps<P>;
}): HTMLElement;
declare function gridPageInit<TGrid extends Widget<P>, P>(grid: TGrid & {
    domNode: HTMLElement;
}): TGrid;
declare function gridPageInit<TGrid extends Widget<P>, P>(type: CreateWidgetParams<TGrid, P>["type"], props?: WidgetProps<P>): TGrid;
declare function panelPageInit<TGrid extends Widget<P>, P>(panel: TGrid & {
    domNode: HTMLElement;
}): TGrid;
declare function panelPageInit<TGrid extends Widget<P>, P>(type: CreateWidgetParams<TGrid, P>["type"], props?: WidgetProps<P>): TGrid;
declare function initFullHeightGridPage(gridDiv: HTMLElement | ArrayLike<HTMLElement> | {
    domNode: HTMLElement;
}, opt?: {
    noRoute?: boolean;
    setHeight?: boolean;
}): void;
declare function layoutFillHeightValue(element: HTMLElement | ArrayLike<HTMLElement>): number;
declare function layoutFillHeight(element: HTMLElement | ArrayLike<HTMLElement>): void;
declare function isMobileView(): boolean;
declare function triggerLayoutOnShow(element: HTMLElement | ArrayLike<HTMLElement>): void;
declare function centerDialog(el: HTMLElement | ArrayLike<HTMLElement>): void;

declare namespace LayoutTimer {
    function store(key: number): void;
    function trigger(key: number): void;
    function onSizeChange(element: () => HTMLElement, handler: () => void, width?: boolean, height?: boolean): number;
    function onWidthChange(element: () => HTMLElement, handler: () => void): number;
    function onHeightChange(element: () => HTMLElement, handler: () => void): number;
    function onShown(element: () => HTMLElement, handler: () => void): number;
    function off(key: number): number;
}
declare function executeOnceWhenVisible(el: HTMLElement | ArrayLike<HTMLElement>, callback: Function): void;
declare function executeEverytimeWhenVisible(el: HTMLElement | ArrayLike<HTMLElement>, callback: Function, callNowIfVisible: boolean): void;

/** @deprecated prefer localText for better discoverability */
declare const text: typeof localText;
declare function dbText(prefix: string): ((key: string) => string);
declare function prefixedText(prefix: string): (text: string, key: string | ((p?: string) => string)) => string;
declare function dbTryText(prefix: string): ((key: string) => string);
declare namespace LT {
    /** @deprecated Use addLocalText */
    const add: typeof addLocalText;
    /** @deprecated Use localText */
    const getDefault: typeof localText;
}

interface HandleRouteEvent extends Event {
    route: string;
    parts: string[];
    index: number;
}
declare namespace Router {
    let enabled: boolean;
    function navigate(hash: string, tryBack?: boolean, silent?: boolean): void;
    function replace(hash: string, tryBack?: boolean): void;
    function replaceLast(hash: string, tryBack?: boolean): void;
    function dialog(owner: HTMLElement | ArrayLike<HTMLElement>, element: HTMLElement | ArrayLike<HTMLElement>, hash: () => string): void;
    function resolve(hash?: string): void;
}

declare namespace ScriptData {
    function bindToChange(name: string, onChange: () => void): void | (() => void);
    const canLoad: typeof canLoadScriptData;
    function ensure<TData = any>(name: string, dynJS?: boolean): TData;
    function reload<TData = any>(name: string, dynJS?: boolean): TData;
    function reloadAsync<TData = any>(name: string): Promise<TData>;
    const set: typeof setScriptData;
}
/**
 * Check if a dynamic script with provided name is available in the cache
 * or it is a registered script name
 * @param name Dynamic script name
 * @returns True if already available or registered
 */
declare function canLoadScriptData(name: string): boolean;
declare function getRemoteData<TData = any>(key: string): TData;
declare function getLookup<TItem>(key: string): Lookup<TItem>;
declare function reloadLookup<TItem = any>(key: string): Lookup<TItem>;
declare function getColumns(key: string): PropertyItem[];
declare function getColumnsAsync(key: string): Promise<PropertyItem[]>;
declare function getColumnsData(key: string): PropertyItemsData;
declare const getColumnsDataAsync: typeof getColumnsScript;
declare function getForm(key: string): PropertyItem[];
declare function getFormAsync(key: string): Promise<PropertyItem[]>;
declare function getFormData(key: string): PropertyItemsData;
declare const getFormDataAsync: typeof getFormScript;
declare function getTemplate(key: string): string;

declare function setEquality(request: ListRequest, field: string, value: any): void;
interface PostToServiceOptions {
    url?: string;
    service?: string;
    target?: string;
    request: any;
}
interface PostToUrlOptions {
    url?: string;
    target?: string;
    params: any;
}
declare function parseQueryString(s?: string): {};
declare function postToService(options: PostToServiceOptions): void;
declare function postToUrl(options: PostToUrlOptions): void;

/**
 * Checks if the string ends with the specified substring.
 * @deprecated Use .endsWith method of String directly
 * @param s String to check.
 * @param suffix Suffix to check.
 * @returns True if the string ends with the specified substring.
  */
declare function endsWith(s: string, suffix: string): boolean;
/**
 * Checks if the string is empty or null. Prefer (!s) instead.
 * @param s String to check.
 * @returns True if the string is empty or null.
 */
declare function isEmptyOrNull(s: string): boolean;
/**
 * Checks if the string is empty or null or whitespace. Prefer !s?.Trim() instead.
 * @param s String to check.
 * @returns True if the string is empty or null or whitespace.
 */
declare function isTrimmedEmpty(s: string): boolean;
/**
 * Pads the string to the left with the specified character.
 * @param s String to pad.
 * @param len Target length of the string.
 * @param ch Character to pad with.
 * @returns Padded string.
 */
declare function padLeft(s: string | number, len: number, ch?: string): any;
/**
 * Checks if the string starts with the prefix
 * @deprecated Use .startsWith method of String directly
 * @param s String to check.
 * @param prefix Prefix to check.
 * @returns True if the string starts with the prefix.
 */
declare function startsWith(s: string, prefix: string): boolean;
/**
 * Converts the string to single line by removing line end characters
 * @param str String to convert.
 */
declare function toSingleLine(str: string): string;
/**
 * Trims the whitespace characters from the end of the string
 */
declare var trimEnd: (s: string) => any;
/**
 * Trims the whitespace characters from the start of the string
 */
declare var trimStart: (s: string) => any;
/**
 * Trims the whitespace characters from the start and end of the string
 * This returns empty string even when the string is null or undefined.
 */
declare function trim(s: string): string;
/**
 * Trims the whitespace characters from the start and end of the string
 * Returns empty string if the string is null or undefined.
 */
declare function trimToEmpty(s: string): string;
/**
 * Trims the whitespace characters from the start and end of the string
 * Returns null if the string is null, undefined or whitespace.
 */
declare function trimToNull(s: string): string;
/**
 * Replaces all occurrences of the search string with the replacement string.
 * @param str String to replace.
 * @param find String to find.
 * @param replace String to replace with.
 * @returns Replaced string.
 */
declare function replaceAll(str: string, find: string, replace: string): string;
/**
 * Pads the start of string to make it the specified length.
 * @param s String to pad.
 * @param len Target length of the string.
 */
declare function zeroPad(n: number, len: number): string;

type Dictionary<TItem> = {
    [key: string]: TItem;
};
/** @deprecated Use ?? operator */
declare function coalesce(a: any, b: any): any;
/** @deprecated Use a != null */
declare function isValue(a: any): boolean;
declare let today: () => Date;
declare function extend<T = any>(a: T, b: T): T;
declare function deepClone<T = any>(a: T, a2?: any, a3?: any): T;
interface TypeMember {
    name: string;
    type: MemberType;
    attr?: any[];
    getter?: string;
    setter?: string;
}
declare enum MemberType {
    field = 4,
    property = 16
}
declare function getMembers(type: any, memberTypes: MemberType): TypeMember[];
declare function addTypeMember(type: any, member: TypeMember): TypeMember;
declare function getTypes(from?: any): any[];
declare function clearKeys(d: any): void;
declare function keyOf<T>(prop: keyof T): keyof T;
declare function cast(instance: any, type: Type): any;
declare function safeCast(instance: any, type: Type): any;
declare function initializeTypes(root: any, pre: string, limit: number): void;
declare class Exception extends Error {
    constructor(message: string);
}
declare class ArgumentNullException extends Exception {
    constructor(paramName: string, message?: string);
}
declare class InvalidCastException extends Exception {
    constructor(message: string);
}

declare function validatorAbortHandler(validator: any): void;
declare function validateOptions(options?: any): any;

declare function loadValidationErrorMessages(): boolean;
declare function getHighlightTarget(el: HTMLElement): HTMLElement;
declare function baseValidateOptions(): any;
declare function validateForm(form: HTMLElement | ArrayLike<HTMLElement>, opt: any): any;
declare function addValidationRule(element: HTMLElement | ArrayLike<HTMLElement>, rule: (input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement) => string, uniqueName?: string): void;
declare function removeValidationRule(element: HTMLElement | ArrayLike<HTMLElement>, uniqueName: string): void;

declare namespace Aggregators {
    function Avg(field: string): void;
    function WeightedAvg(field: string, weightedField: string): void;
    function Min(field: string): void;
    function Max(field: string): void;
    function Sum(field: string): void;
}
declare namespace AggregateFormatting {
    function formatMarkup<TItem = any>(totals: GroupTotals, column: Column<TItem>, aggType: string): string;
    function formatValue(column: Column, value: number): string;
    function groupTotalsFormatter<TItem = any>(totals: GroupTotals, column: Column<TItem>): string;
}

type Format<TItem = any> = (ctx: FormatterContext<TItem>) => string;
declare module "@serenity-is/sleekgrid" {
    interface Column<TItem = any> {
        referencedFields?: string[];
        sourceItem?: PropertyItem;
    }
}
interface Formatter {
    format(ctx: FormatterContext): string;
}
interface GroupInfo<TItem> {
    getter?: any;
    formatter?: (p1: Group<TItem>) => string;
    comparer?: (a: Group<TItem>, b: Group<TItem>) => number;
    aggregators?: any[];
    aggregateCollapsed?: boolean;
    lazyTotalsCalculation?: boolean;
}
interface PagerOptions {
    view?: any;
    showRowsPerPage?: boolean;
    rowsPerPage?: number;
    rowsPerPageOptions?: number[];
    onChangePage?: (newPage: number) => void;
    onRowsPerPageChange?: (n: number) => void;
}
interface SummaryOptions {
    aggregators: any[];
}
interface PagingOptions {
    rowsPerPage?: number;
    page?: number;
}

interface RemoteViewOptions {
    autoLoad?: boolean;
    idField?: string;
    contentType?: string;
    dataType?: string;
    filter?: any;
    params?: any;
    onSubmit?: CancellableViewCallback<any>;
    url?: string;
    localSort?: boolean;
    sortBy?: any;
    rowsPerPage?: number;
    seekToPage?: number;
    onProcessData?: RemoteViewProcessCallback<any>;
    method?: string;
    inlineFilters?: boolean;
    groupItemMetadataProvider?: GroupItemMetadataProvider;
    onAjaxCall?: RemoteViewAjaxCallback<any>;
    getItemMetadata?: (p1?: any, p2?: number) => any;
    errorMsg?: string;
}
interface PagingInfo {
    rowsPerPage: number;
    page: number;
    totalCount: number;
    loading: boolean;
    error: string;
    dataView: RemoteView<any>;
}
type CancellableViewCallback<TEntity> = (view: RemoteView<TEntity>) => boolean | void;
type RemoteViewAjaxCallback<TEntity> = (view: RemoteView<TEntity>, options: ServiceOptions<ListResponse<TEntity>>) => boolean | void;
type RemoteViewFilter<TEntity> = (item: TEntity, view: RemoteView<TEntity>) => boolean;
type RemoteViewProcessCallback<TEntity> = (data: ListResponse<TEntity>, view: RemoteView<TEntity>) => ListResponse<TEntity>;
interface RemoteView<TEntity> {
    onSubmit: CancellableViewCallback<TEntity>;
    onDataChanged: EventEmitter;
    onDataLoading: EventEmitter;
    onDataLoaded: EventEmitter;
    onPagingInfoChanged: EventEmitter;
    onRowCountChanged: EventEmitter;
    onRowsChanged: EventEmitter;
    onRowsOrCountChanged: EventEmitter;
    getPagingInfo(): PagingInfo;
    onGroupExpanded: EventEmitter;
    onGroupCollapsed: EventEmitter;
    onAjaxCall: RemoteViewAjaxCallback<TEntity>;
    onProcessData: RemoteViewProcessCallback<TEntity>;
    addData(data: ListResponse<TEntity>): void;
    beginUpdate(): void;
    endUpdate(): void;
    deleteItem(id: any): void;
    getItems(): TEntity[];
    setFilter(filter: RemoteViewFilter<TEntity>): void;
    getFilter(): RemoteViewFilter<TEntity>;
    getFilteredItems(): any;
    getGroupItemMetadataProvider(): GroupItemMetadataProvider;
    setGroupItemMetadataProvider(value: GroupItemMetadataProvider): void;
    fastSort: any;
    setItems(items: any[], newIdProperty?: boolean | string): void;
    getIdPropertyName(): string;
    getItemById(id: any): TEntity;
    getGrandTotals(): any;
    getGrouping(): GroupInfo<TEntity>[];
    getGroups(): any[];
    getRowById(id: any): number;
    getRowByItem(item: any): number;
    getRows(): any[];
    mapItemsToRows(itemArray: any[]): any[];
    mapRowsToIds(rowArray: number[]): any[];
    mapIdsToRows(idAray: any[]): number[];
    setFilterArgs(args: any): void;
    setRefreshHints(hints: any[]): void;
    insertItem(insertBefore: number, item: any): void;
    sortedAddItem(item: any): void;
    sortedUpdateItem(id: any, item: any): void;
    syncGridSelection(grid: any, preserveHidden?: boolean, preserveHiddenOnSelectionChange?: boolean): void;
    syncGridCellCssStyles(grid: any, key: string): void;
    getItemMetadata(i: number): any;
    updateItem(id: any, item: TEntity): void;
    addItem(item: TEntity): void;
    getIdxById(id: any): any;
    getItemByIdx(index: number): any;
    setGrouping(groupInfo: GroupInfo<TEntity>[]): void;
    collapseAllGroups(level: number): void;
    expandAllGroups(level: number): void;
    expandGroup(keys: any[]): void;
    collapseGroup(keys: any[]): void;
    setSummaryOptions(options: SummaryOptions): void;
    setPagingOptions(options: PagingOptions): void;
    refresh(): void;
    populate(): void;
    populateLock(): void;
    populateUnlock(): void;
    getItem(row: number): any;
    getLength(): number;
    rowsPerPage: number;
    errormsg: string;
    params: any;
    getLocalSort(): boolean;
    setLocalSort(value: boolean): void;
    sort(comparer?: (a: any, b: any) => number, ascending?: boolean): void;
    reSort(): void;
    sortBy: string[];
    url: string;
    method: string;
    idField: string;
    seekToPage?: number;
}
declare class RemoteView<TEntity> {
    constructor(options: RemoteViewOptions);
}

declare global {
    namespace Select2 {
        namespace util {
            function stripDiacritics(input: string): string;
        }
    }
    interface Select2QueryOptions {
        element?: ArrayLike<HTMLElement>;
        term?: string;
        page?: number;
        context?: any;
        callback?: (p1: Select2Result) => void;
    }
    interface Select2Item {
        id: string;
        text: string;
        source?: any;
        disabled?: boolean;
    }
    interface Select2Result {
        results: any;
        more: boolean;
        context?: any;
    }
    interface Select2AjaxOptions {
        transport?: any;
        url?: any;
        dataType?: string;
        quietMillis?: number;
        cache?: boolean;
        jsonpCallback?: any;
        data?: (p1: string, p2: number, p3: any) => any;
        results?: (p1: any, p2: number, p3: any) => any;
        params?: any;
    }
    interface Select2Options {
        width?: any;
        minimumInputLength?: number;
        maximumInputLength?: number;
        minimumResultsForSearch?: number;
        maximumSelectionSize?: any;
        placeHolder?: string;
        placeHolderOption?: any;
        separator?: string;
        allowClear?: boolean;
        multiple?: boolean;
        closeOnSelect?: boolean;
        openOnEnter?: boolean;
        id?: (p1: any) => string;
        matcher?: (p1: string, p2: string, p3: ArrayLike<HTMLElement>) => boolean;
        sortResults?: (p1: any, p2: ArrayLike<HTMLElement>, p3: any) => any;
        formatSelection?: (p1: any, p2: ArrayLike<HTMLElement>, p3: (p1: string) => string) => string;
        formatResult?: (p1: any, p2: ArrayLike<HTMLElement>, p3: any, p4: (p1: string) => string) => string;
        formatResultCssClass?: (p1: any) => string;
        formatNoMatches?: (p1: string) => string;
        formatSearching?: () => string;
        formatInputTooShort?: (p1: string, p2: number) => string;
        formatSelectionTooBig?: (p1: string) => string;
        createSearchChoice?: (p1: string) => any;
        createSearchChoicePosition?: string;
        initSelection?: (p1: ArrayLike<HTMLElement>, p2: (p1: any) => void) => void;
        tokenizer?: (p1: string, p2: any, p3: (p1: any) => any, p4: any) => string;
        tokenSeparators?: any;
        query?: (p1: Select2QueryOptions) => void;
        ajax?: Select2AjaxOptions;
        data?: any;
        tags?: any;
        containerCss?: any;
        containerCssClass?: any;
        dropdownCss?: any;
        dropdownCssClass?: any;
        dropdownAutoWidth?: boolean;
        adaptContainerCssClass?: (p1: string) => string;
        adaptDropdownCssClass?: (p1: string) => string;
        escapeMarkup?: (p1: string) => string;
        selectOnBlur?: boolean;
        loadMorePadding?: number;
        nextSearchTerm?: (p1: any, p2: string) => string;
    }
    interface Select2Data {
        text?: string;
    }
    interface JQuery {
        select2(options: Select2Options): JQuery;
        select2(cmd: 'focus' | 'open'): JQuery;
        select2(cmd: 'destroy'): void;
        select2(cmd: 'val'): any;
        select2(cmd: 'val', value: string | string[]): JQuery;
        select2(cmd: 'data'): Select2Data;
    }
}

declare class IBooleanValue {
}
interface IBooleanValue {
    get_value(): boolean;
    set_value(value: boolean): void;
}

declare class IDoubleValue {
}
interface IDoubleValue {
    get_value(): any;
    set_value(value: any): void;
}

declare class IDialog {
}
interface IDialog {
    dialogOpen(asPanel?: boolean): void;
}

declare class IEditDialog {
}
interface IEditDialog {
    load(entityOrId: any, done: () => void, fail?: (p1: any) => void): void;
}

declare class IGetEditValue {
}
interface IGetEditValue {
    getEditValue(property: PropertyItem, target: any): void;
}

interface IReadOnly {
    get_readOnly(): boolean;
    set_readOnly(value: boolean): void;
}
declare class IReadOnly {
}

declare class ISetEditValue {
}
interface ISetEditValue {
    setEditValue(source: any, property: PropertyItem): void;
}

declare class IStringValue {
}
interface IStringValue {
    get_value(): string;
    set_value(value: string): void;
}

interface IValidateRequired {
    get_required(): boolean;
    set_required(value: boolean): void;
}
declare class IValidateRequired {
}

declare class EnumKeyAttribute {
    value: string;
    constructor(value: string);
}
declare class DisplayNameAttribute {
    displayName: string;
    constructor(displayName: string);
}
declare class CategoryAttribute {
    category: string;
    constructor(category: string);
}
declare class ColumnsKeyAttribute {
    value: string;
    constructor(value: string);
}
declare class CssClassAttribute {
    cssClass: string;
    constructor(cssClass: string);
}
declare class DefaultValueAttribute {
    value: any;
    constructor(value: any);
}
declare class DialogTypeAttribute {
    value: any;
    constructor(value: any);
}
declare class EditorOptionAttribute {
    key: string;
    value: any;
    constructor(key: string, value: any);
}
declare class EditorTypeAttributeBase {
    editorType: string;
    constructor(editorType: string);
    setParams(editorParams: any): void;
}
declare class EditorTypeAttribute extends EditorTypeAttributeBase {
    constructor(editorType: string);
}
declare class ElementAttribute {
    value: string;
    constructor(value: string);
}
declare class EntityTypeAttribute {
    value: string;
    constructor(value: string);
}
declare class FilterableAttribute {
    value: boolean;
    constructor(value?: boolean);
}
declare class FlexifyAttribute {
    value: boolean;
    constructor(value?: boolean);
}
declare class FormKeyAttribute {
    value: string;
    constructor(value: string);
}
declare class GeneratedCodeAttribute {
    origin?: string;
    constructor(origin?: string);
}
declare class HiddenAttribute {
    constructor();
}
declare class HintAttribute {
    hint: string;
    constructor(hint: string);
}
declare class IdPropertyAttribute {
    value: string;
    constructor(value: string);
}
declare class InsertableAttribute {
    value: boolean;
    constructor(value?: boolean);
}
declare class IsActivePropertyAttribute {
    value: string;
    constructor(value: string);
}
declare class ItemNameAttribute {
    value: string;
    constructor(value: string);
}
declare class LocalTextPrefixAttribute {
    value: string;
    constructor(value: string);
}
declare class MaximizableAttribute {
    value: boolean;
    constructor(value?: boolean);
}
declare class MaxLengthAttribute {
    maxLength: number;
    constructor(maxLength: number);
}
declare class NamePropertyAttribute {
    value: string;
    constructor(value: string);
}
declare class OneWayAttribute {
}
declare class OptionAttribute {
}
declare class OptionsTypeAttribute {
    value: Function;
    constructor(value: Function);
}
declare class PanelAttribute {
    value: boolean;
    constructor(value?: boolean);
}
declare class PlaceholderAttribute {
    value: string;
    constructor(value: string);
}
declare class ReadOnlyAttribute {
    value: boolean;
    constructor(value?: boolean);
}
declare class RequiredAttribute {
    isRequired: boolean;
    constructor(isRequired?: boolean);
}
declare class ResizableAttribute {
    value: boolean;
    constructor(value?: boolean);
}
declare class ResponsiveAttribute {
    value: boolean;
    constructor(value?: boolean);
}
declare class ServiceAttribute {
    value: string;
    constructor(value: string);
}
declare class UpdatableAttribute {
    value: boolean;
    constructor(value?: boolean);
}

declare enum CaptureOperationType {
    Before = 0,
    Delete = 1,
    Insert = 2,
    Update = 3
}

interface DataChangeInfo extends Event {
    operationType: string;
    entityId: any;
    entity: any;
}

declare namespace Decorators {
    const classType: typeof classTypeInfo;
    const editorType: typeof editorTypeInfo;
    const interfaceType: typeof interfaceTypeInfo;
    const formatterType: typeof formatterTypeInfo;
    function registerType(): (target: Function & {
        [typeInfoProperty]: any;
    }) => void;
    function registerClass(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
    function registerInterface(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
    function registerEditor(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
    function registerEnum(target: any, enumKey?: string, name?: string): void;
    function registerEnumType(target: any, name?: string, enumKey?: string): void;
    function registerFormatter(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
    function enumKey(value: string): (target: Function) => void;
    function option(): (target: Object, propertyKey: string) => void;
    function dialogType(value: any): (target: Function) => void;
    function editor(): (target: Function) => void;
    function element(value: string): (target: Function) => void;
    function filterable(value?: boolean): (target: Function) => void;
    function itemName(value: string): (target: Function) => void;
    function maximizable(value?: boolean): (target: Function) => void;
    function optionsType(value: Function): (target: Function) => void;
    function panel(value?: boolean): (target: Function) => void;
    function resizable(value?: boolean): (target: Function) => void;
    function responsive(value?: boolean): (target: Function) => void;
    function service(value: string): (target: Function) => void;
}

declare namespace DialogTypeRegistry {
    function get(key: string): any;
    function reset(): void;
    function tryGet(key: string): any;
}

declare namespace EditorTypeRegistry {
    function get(key: string): any;
    function reset(): void;
    function tryGet(key: string): any;
}

declare namespace EnumTypeRegistry {
    function get(key: string): Function;
    function reset(): void;
    function tryGet(key: string): any;
}

declare namespace ReflectionUtils {
    function getPropertyValue(o: any, property: string): any;
    function setPropertyValue(o: any, property: string, value: any): void;
    function makeCamelCase(s: string): string;
}

interface IRowDefinition {
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

declare namespace LazyLoadHelper {
    const executeOnceWhenShown: typeof executeOnceWhenVisible;
    const executeEverytimeWhenShown: typeof executeEverytimeWhenVisible;
}

declare class PrefixedContext {
    readonly idPrefix: string;
    constructor(idPrefix: string);
    byId(id: string): Fluent;
    w<TWidget>(id: string, type: {
        new (...args: any[]): TWidget;
    }): TWidget;
}

declare global {
    interface JQuery {
        getWidget<TWidget>(widgetType: {
            new (...args: any[]): TWidget;
        }): TWidget;
        tryGetWidget<TWidget>(widgetType?: {
            new (...args: any[]): TWidget;
        }): TWidget;
        validate(...args: any[]): any;
        valid(): boolean;
    }
}

interface ToolButtonProps {
    action?: string;
    title?: string;
    hint?: string;
    cssClass?: string;
    icon?: IconClassName;
    onClick?: any;
    htmlEncode?: any;
    hotkey?: string;
    hotkeyAllowDefault?: boolean;
    hotkeyContext?: any;
    separator?: (false | true | 'left' | 'right' | 'both');
    visible?: boolean | (() => boolean);
    disabled?: boolean | (() => boolean);
}
interface ToolButton extends ToolButtonProps {
    hotkey?: string;
    hotkeyAllowDefault?: boolean;
    hotkeyContext?: any;
    separator?: (false | true | 'left' | 'right' | 'both');
}
declare function ToolbarButton(tb: ToolButtonProps): HTMLElement;
interface PopupMenuButtonOptions {
    menu?: HTMLElement | ArrayLike<HTMLElement>;
    onPopup?: () => void;
    positionMy?: string;
    positionAt?: string;
}
declare class PopupMenuButton<P extends PopupMenuButtonOptions = PopupMenuButtonOptions> extends Widget<P> {
    constructor(props: WidgetProps<P>);
    destroy(): void;
}
interface PopupToolButtonOptions extends PopupMenuButtonOptions {
}
declare class PopupToolButton<P extends PopupToolButtonOptions = PopupToolButtonOptions> extends PopupMenuButton<P> {
    constructor(props: WidgetProps<P>);
}
interface ToolbarOptions {
    buttons?: ToolButton[];
    hotkeyContext?: any;
}
declare class Toolbar<P extends ToolbarOptions = ToolbarOptions> extends Widget<P> {
    protected renderContents(): HTMLDivElement;
    destroy(): void;
    protected mouseTrap: any;
    createButton(container: Fluent, tb: ToolButton): void;
    findButton(className: string): Fluent<HTMLElement>;
    updateInterface(): void;
}

declare class TemplatedWidget<P> extends Widget<P> {
    protected byId(id: string): Fluent;
    protected findById(id: string): HTMLElement;
    protected getTemplate(): string;
    protected renderContents(): void;
}

declare class TemplatedDialog<P> extends TemplatedWidget<P> {
    static createDefaultElement(): HTMLDivElement;
    protected tabs: any;
    protected toolbar: Toolbar;
    protected validator: any;
    protected commonDialog: ICommonDialog;
    constructor(props?: WidgetProps<P>);
    private get isMarkedAsPanel();
    destroy(): void;
    protected addCssClass(): void;
    protected getInitialDialogTitle(): string;
    protected getDialogOptions(asPanel?: boolean): DialogOptions;
    /** Gets jQuery UI Dialog options */
    protected getUIDialogOptions(): any;
    protected preferBSModal(): boolean;
    protected getModalOptions(): any;
    protected initDialog(): void;
    protected initUIDialog(): void;
    dialogOpen(asPanel?: boolean): void;
    protected onDialogOpen(): void;
    protected getToolbarButtons(): ToolButton[];
    protected initToolbar(): void;
    protected getValidatorOptions(): any;
    protected initValidator(): void;
    protected resetValidation(): void;
    protected validateForm(): boolean;
    arrange(): void;
    protected onDialogClose(result?: string): void;
    protected getDialogButtons(): DialogButton[];
    dialogClose(): void;
    get dialogTitle(): string;
    set dialogTitle(value: string);
    protected initTabs(): void;
    protected handleResponsive(): void;
}

declare class TemplatedPanel<P = {}> extends TemplatedWidget<P> {
    constructor(props: WidgetProps<P>);
    destroy(): void;
    protected tabs: Fluent;
    protected toolbar: Toolbar;
    protected validator: any;
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

declare namespace ValidationHelper {
    function asyncSubmit(form: ArrayLike<HTMLElement> | HTMLElement, validateBeforeSave: () => boolean, submitHandler: () => void): boolean;
    function submit(form: ArrayLike<HTMLElement> | HTMLElement, validateBeforeSave: () => boolean, submitHandler: () => void): boolean;
    function getValidator(elem: ArrayLike<HTMLElement> | HTMLElement): any;
    function validateElement(elem: ArrayLike<HTMLElement> | HTMLElement): void;
}

declare class CascadedWidgetLink<TParent extends Widget<any>> {
    private parentType;
    private widget;
    private parentChange;
    constructor(parentType: {
        new (...args: any[]): TParent;
    }, widget: Widget<any>, parentChange: (p1: TParent) => void);
    private _parentID;
    bind(): TParent;
    unbind(): TParent;
    get_parentID(): string;
    set_parentID(value: string): void;
}

declare namespace TabsExtensions {
    function setDisabled(tabs: ArrayLike<HTMLElement> | HTMLElement, tabKey: string, isDisabled: boolean): void;
    function toggle(tabs: ArrayLike<HTMLElement> | HTMLElement, tabKey: string, visible: boolean): void;
    function activeTabKey(tabs: ArrayLike<HTMLElement> | HTMLElement): any;
    function indexByKey(tabs: ArrayLike<HTMLElement> | HTMLElement): Record<string, number>;
    function selectTab(tabs: HTMLElement | ArrayLike<HTMLElement>, tabKey: string): void;
}

declare namespace ReflectionOptionsSetter {
    function set(target: any, options: any): void;
}

declare class PropertyGrid<P extends PropertyGridOptions = PropertyGridOptions> extends Widget<P> {
    private editors;
    private items;
    protected renderContents(): void;
    destroy(): void;
    private createItems;
    private createCategoryDiv;
    private categoryLinkClick;
    private determineText;
    private createField;
    private getCategoryOrder;
    private createCategoryLinks;
    get_editors(): Widget<any>[];
    get_items(): PropertyItem[];
    get_idPrefix(): string;
    get_mode(): PropertyGridMode;
    set_mode(value: PropertyGridMode): void;
    private static setMaxLength;
    load(source: any): void;
    save(target?: any): any;
    get value(): any;
    set value(val: any);
    private canModifyItem;
    updateInterface(): void;
    enumerateItems(callback: (p1: PropertyItem, p2: Widget<any>) => void): void;
}
declare enum PropertyGridMode {
    insert = 1,
    update = 2
}
interface PropertyGridOptions {
    idPrefix?: string;
    items: PropertyItem[];
    useCategories?: boolean;
    categoryOrder?: string;
    defaultCategory?: string;
    localTextPrefix?: string;
    mode?: PropertyGridMode;
}

declare class PropertyPanel<TItem, P> extends TemplatedPanel<P> {
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
    protected get_entity(): TItem;
    protected get_entityId(): any;
    protected set_entity(value: TItem): void;
    protected set_entityId(value: any): void;
    protected validateBeforeSave(): boolean;
    protected propertyGrid: PropertyGrid;
}

declare namespace SubDialogHelper {
    function bindToDataChange(dialog: any, owner: Widget<any>, dataChange: (ev: DataChangeInfo) => void, useTimeout?: boolean): any;
    function triggerDataChange(dialog: Widget<any>): any;
    function triggerDataChanged(element: HTMLElement | ArrayLike<HTMLElement>): void;
    function bubbleDataChange(dialog: any, owner: Widget<any>, useTimeout?: boolean): any;
    function cascade(cascadedDialog: {
        domNode: HTMLElement;
    }, ofElement: HTMLElement | ArrayLike<HTMLElement>): any;
    function cascadedDialogOffset(element: HTMLElement | ArrayLike<HTMLElement>): any;
}

declare namespace DialogExtensions {
    function dialogResizable(dialog: HTMLElement | ArrayLike<HTMLElement>, w?: any, h?: any, mw?: any, mh?: any): void;
    function dialogMaximizable(dialog: HTMLElement | ArrayLike<HTMLElement>): void;
}

declare class PropertyDialog<TItem, P> extends TemplatedDialog<P> {
    protected entity: TItem;
    protected entityId: any;
    protected propertyItemsData: PropertyItemsData;
    constructor(props?: WidgetProps<P>);
    protected propertyItemsReady(itemsData: PropertyItemsData): void;
    protected afterInit(): void;
    protected useAsync(): boolean;
    destroy(): void;
    protected getUIDialogOptions(): any;
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
    protected get_entity(): TItem;
    protected set_entity(value: TItem): void;
    protected get_entityId(): any;
    protected set_entityId(value: any): void;
    protected validateBeforeSave(): boolean;
    protected updateTitle(): void;
    protected propertyGrid: PropertyGrid;
    protected getTemplate(): string;
}

declare namespace EditorUtils {
    function getDisplayText(editor: Widget<any>): string;
    function getValue(editor: Widget<any>): any;
    function saveValue(editor: Widget<any>, item: PropertyItem, target: any): void;
    function setValue(editor: Widget<any>, value: any): void;
    function loadValue(editor: Widget<any>, item: PropertyItem, source: any): void;
    function setReadonly(elements: Element | ArrayLike<Element>, isReadOnly: boolean): void;
    function setReadOnly(widget: Widget<any>, isReadOnly: boolean): void;
    function setRequired(widget: Widget<any>, isRequired: boolean): void;
    function setContainerReadOnly(container: ArrayLike<HTMLElement> | HTMLElement, readOnly: boolean): void;
}

declare class StringEditor<P = {}> extends EditorWidget<P> {
    static typeInfo: EditorTypeInfo<"Serenity.StringEditor">;
    readonly domNode: HTMLInputElement;
    static createDefaultElement(): HTMLInputElement;
    get value(): string;
    protected get_value(): string;
    set value(value: string);
    protected set_value(value: string): void;
}

declare class PasswordEditor<TOptions = {}> extends StringEditor<TOptions> {
    static typeInfo: EditorTypeInfo<"Serenity.PasswordEditor">;
    static createDefaultElement(): HTMLInputElement;
}

interface TextAreaEditorOptions {
    cols?: number;
    rows?: number;
}
declare class TextAreaEditor<P extends TextAreaEditorOptions = TextAreaEditorOptions> extends EditorWidget<P> {
    static createDefaultElement(): HTMLTextAreaElement;
    constructor(props: EditorProps<P>);
    get value(): string;
    protected get_value(): string;
    set value(value: string);
    protected set_value(value: string): void;
}

declare class BooleanEditor<P = {}> extends EditorWidget<P> {
    static createDefaultElement(): HTMLInputElement;
    readonly domNode: HTMLInputElement;
    get value(): boolean;
    protected get_value(): boolean;
    set value(value: boolean);
    protected set_value(value: boolean): void;
}

interface DecimalEditorOptions {
    minValue?: string;
    maxValue?: string;
    decimals?: any;
    padDecimals?: any;
    allowNegatives?: boolean;
}
declare class DecimalEditor<P extends DecimalEditorOptions = DecimalEditorOptions> extends EditorWidget<P> implements IDoubleValue {
    static createDefaultElement(): HTMLInputElement;
    readonly domNode: HTMLInputElement;
    constructor(props: EditorProps<P>);
    get_value(): number;
    get value(): number;
    set_value(value: number): void;
    set value(v: number);
    get_isValid(): boolean;
    static defaultAutoNumericOptions(): any;
}

interface IntegerEditorOptions {
    minValue?: number;
    maxValue?: number;
    allowNegatives?: boolean;
}
declare class IntegerEditor<P extends IntegerEditorOptions = IntegerEditorOptions> extends EditorWidget<P> implements IDoubleValue {
    static createDefaultElement(): HTMLInputElement;
    readonly domNode: HTMLInputElement;
    constructor(props: EditorProps<P>);
    get_value(): number;
    get value(): number;
    set_value(value: number): void;
    set value(v: number);
    get_isValid(): boolean;
}

declare const datePickerIconSvg: string;
interface DateEditorOptions {
    yearRange?: string;
    minValue?: string;
    sqlMinMax?: boolean;
}
declare class DateEditor<P extends DateEditorOptions = DateEditorOptions> extends EditorWidget<P> implements IStringValue, IReadOnly {
    static createDefaultElement(): HTMLInputElement;
    readonly domNode: HTMLInputElement;
    private minValue;
    private maxValue;
    constructor(props: EditorProps<P>);
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
    yearRange: string;
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
    static flatPickrOptions(input: HTMLElement): {
        clickOpens: boolean;
        allowInput: boolean;
        dateFormat: string;
        onChange: () => void;
    };
    static flatPickrTrigger(input: HTMLInputElement): HTMLElement;
    static uiPickerZIndexWorkaround(el: HTMLElement | ArrayLike<HTMLElement>): void;
}

declare class DateTimeEditor<P extends DateTimeEditorOptions = DateTimeEditorOptions> extends EditorWidget<P> implements IStringValue, IReadOnly {
    static createDefaultElement(): HTMLInputElement;
    readonly domNode: HTMLInputElement;
    private minValue;
    private maxValue;
    private time;
    private lastSetValue;
    private lastSetValueGet;
    constructor(props: EditorProps<P>);
    getFlatpickrOptions(): any;
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
interface DateTimeEditorOptions {
    startHour?: any;
    endHour?: any;
    intervalMinutes?: any;
    yearRange?: string;
    useUtc?: boolean;
    seconds?: boolean;
    inputOnly?: boolean;
}

interface TimeEditorOptions {
    noEmptyOption?: boolean;
    startHour?: any;
    endHour?: any;
    intervalMinutes?: any;
}
declare class TimeEditor<P extends TimeEditorOptions = TimeEditorOptions> extends EditorWidget<P> {
    static createDefaultElement(): HTMLElement;
    readonly domNode: HTMLSelectElement;
    private minutes;
    constructor(props: EditorProps<P>);
    get value(): number;
    protected get_value(): number;
    set value(value: number);
    protected set_value(value: number): void;
    get_readOnly(): boolean;
    set_readOnly(value: boolean): void;
}

interface EmailEditorOptions {
    domain?: string;
    readOnlyDomain?: boolean;
}
declare class EmailEditor<P extends EmailEditorOptions = EmailEditorOptions> extends EditorWidget<P> {
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

declare class EmailAddressEditor<P = {}> extends StringEditor<P> {
    static createDefaultElement(): HTMLInputElement;
    constructor(props: EditorProps<P>);
}

declare class URLEditor<P = {}> extends StringEditor<P> {
    constructor(props: EditorProps<P>);
}

interface RadioButtonEditorOptions {
    enumKey?: string;
    enumType?: any;
    lookupKey?: string;
}
declare class RadioButtonEditor<P extends RadioButtonEditorOptions = RadioButtonEditorOptions> extends EditorWidget<P> implements IReadOnly {
    constructor(props: EditorProps<P>);
    protected addRadio(value: string, text: string): void;
    get_value(): string;
    get value(): string;
    set_value(value: string): void;
    set value(v: string);
    get_readOnly(): boolean;
    set_readOnly(value: boolean): void;
}

interface Select2CommonOptions {
    allowClear?: boolean;
    delimited?: boolean;
    minimumResultsForSearch?: any;
    multiple?: boolean;
}
interface Select2FilterOptions {
    cascadeFrom?: string;
    cascadeField?: string;
    cascadeValue?: any;
    filterField?: string;
    filterValue?: any;
}
interface Select2InplaceAddOptions {
    inplaceAdd?: boolean;
    inplaceAddPermission?: string;
    dialogType?: string;
    autoComplete?: boolean;
}
interface Select2EditorOptions extends Select2FilterOptions, Select2InplaceAddOptions, Select2CommonOptions {
}
interface Select2SearchPromise extends PromiseLike<any> {
    abort?(): void;
    catch?(callback: () => void): void;
    fail?(callback: () => void): void;
}
interface Select2SearchQuery {
    searchTerm?: string;
    idList?: string[];
    skip?: number;
    take?: number;
    checkMore?: boolean;
}
interface Select2SearchResult<TItem> {
    items: TItem[];
    more: boolean;
}
declare class Select2Editor<P, TItem> extends Widget<P> implements ISetEditValue, IGetEditValue, IStringValue, IReadOnly {
    static createDefaultElement(): HTMLInputElement;
    readonly domNode: HTMLInputElement;
    private _items;
    private _itemById;
    protected lastCreateTerm: string;
    constructor(props: EditorProps<P>);
    destroy(): void;
    protected hasAsyncSource(): boolean;
    protected asyncSearch(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): Select2SearchPromise;
    protected getTypeDelay(): any;
    protected emptyItemText(): string;
    protected getPageSize(): number;
    protected getIdField(): any;
    protected itemId(item: TItem): string;
    protected getTextField(): any;
    protected itemText(item: TItem): string;
    protected itemDisabled(item: TItem): boolean;
    protected mapItem(item: TItem): Select2Item;
    protected mapItems(items: TItem[]): Select2Item[];
    protected allowClear(): boolean;
    protected isMultiple(): boolean;
    private initSelectionPromise;
    private queryPromise;
    private typeTimeout;
    protected abortPendingQuery(): void;
    protected getSelect2Options(): Select2Options;
    get_delimited(): boolean;
    get items(): Select2Item[];
    set items(value: Select2Item[]);
    protected get itemById(): {
        [key: string]: Select2Item;
    };
    protected set itemById(value: {
        [key: string]: Select2Item;
    });
    clearItems(): void;
    addItem(item: Select2Item): void;
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
    protected get_select2Container(): Fluent;
    protected get_items(): Select2Item[];
    protected get_itemByKey(): {
        [key: string]: Select2Item;
    };
    static filterByText<TItem>(items: TItem[], getText: (item: TItem) => string, term: string): TItem[];
    get_value(): any;
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
    get readOnly(): boolean;
    private updateInplaceReadOnly;
    set_readOnly(value: boolean): void;
    set readOnly(value: boolean);
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
    protected setTermOnNewEntity(entity: TItem, term: string): void;
    protected inplaceCreateClick(e: Event): void;
    openDialogAsPanel: boolean;
}
declare function select2LocaleInitialization(): boolean;

declare class SelectEditor<P extends SelectEditorOptions = SelectEditorOptions> extends Select2Editor<P, Select2Item> {
    constructor(props: EditorProps<P>);
    getItems(): any[];
    protected emptyItemText(): string;
    updateItems(): void;
}
interface SelectEditorOptions extends Select2CommonOptions {
    items?: any[];
    emptyOptionText?: string;
}

declare class DateYearEditor<P extends DateYearEditorOptions = DateYearEditorOptions> extends SelectEditor<P> {
    constructor(props: EditorProps<P>);
    getItems(): any[];
}
interface DateYearEditorOptions extends SelectEditorOptions {
    minYear?: string;
    maxYear?: string;
    descending?: boolean;
}

interface EnumEditorOptions extends Select2CommonOptions {
    enumKey?: string;
    enumType?: any;
}
declare class EnumEditor<P extends EnumEditorOptions = EnumEditorOptions> extends Select2Editor<P, Select2Item> {
    constructor(props: EditorProps<P>);
    protected updateItems(): void;
    protected allowClear(): boolean;
}

interface LookupEditorOptions extends Select2EditorOptions {
    lookupKey?: string;
    async?: boolean;
}
declare abstract class LookupEditorBase<P extends LookupEditorOptions, TItem> extends Select2Editor<P, TItem> {
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
    protected mapItem(item: TItem): Select2Item;
    protected getItemDisabled(item: TItem, lookup: Lookup<TItem>): boolean;
    updateItems(): void;
    protected asyncSearch(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): Select2SearchPromise;
    protected getDialogTypeKey(): string;
    protected setCreateTermOnNewEntity(entity: TItem, term: string): void;
    protected editDialogDataChange(): void;
}
declare class LookupEditor<P extends LookupEditorOptions = LookupEditorOptions> extends LookupEditorBase<P, {}> {
    constructor(props: EditorProps<P>);
}

interface ServiceLookupEditorOptions extends Select2EditorOptions {
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
declare abstract class ServiceLookupEditorBase<P extends ServiceLookupEditorOptions, TItem> extends Select2Editor<P, TItem> {
    protected getDialogTypeKey(): string;
    protected getService(): string;
    protected getServiceUrl(): string;
    protected getIncludeColumns(): string[];
    protected getSort(): any[];
    protected getCascadeCriteria(): any[];
    protected getFilterCriteria(): any[];
    protected getIdListCriteria(idList: any[]): any[];
    protected getCriteria(query: Select2SearchQuery): any[];
    protected getListRequest(query: Select2SearchQuery): ListRequest;
    protected getServiceCallOptions(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): ServiceOptions<ListResponse<TItem>>;
    protected hasAsyncSource(): boolean;
    protected asyncSearch(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): Select2SearchPromise;
}
declare class ServiceLookupEditor<P extends ServiceLookupEditorOptions = ServiceLookupEditorOptions, TItem = any> extends ServiceLookupEditorBase<ServiceLookupEditorOptions, TItem> {
    constructor(props: EditorProps<P>);
}

interface HtmlContentEditorOptions {
    cols?: number;
    rows?: number;
}
interface CKEditorConfig {
}
declare class HtmlContentEditor<P extends HtmlContentEditorOptions = HtmlContentEditorOptions> extends EditorWidget<P> implements IStringValue, IReadOnly {
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
    static includeCKEditor(): void;
}
declare class HtmlNoteContentEditor<P extends HtmlContentEditorOptions = HtmlContentEditorOptions> extends HtmlContentEditor<P> {
    protected getConfig(): CKEditorConfig;
}
declare class HtmlReportContentEditor<P extends HtmlContentEditorOptions = HtmlContentEditorOptions> extends HtmlContentEditor<P> {
    protected getConfig(): CKEditorConfig;
}

declare class MaskedEditor<P extends MaskedEditorOptions = MaskedEditorOptions> extends EditorWidget<P> {
    static createDefaultElement(): HTMLInputElement;
    readonly domNode: HTMLInputElement;
    constructor(props: EditorProps<P>);
    get value(): string;
    protected get_value(): string;
    set value(value: string);
    protected set_value(value: string): void;
}
interface MaskedEditorOptions {
    mask?: string;
    placeholder?: string;
}

interface RecaptchaOptions {
    siteKey?: string;
    language?: string;
}
declare class Recaptcha<P extends RecaptchaOptions = RecaptchaOptions> extends EditorWidget<P> implements IStringValue {
    constructor(props: EditorProps<P>);
    get_value(): string;
    set_value(value: string): void;
}

declare namespace UploadHelper {
    function addUploadInput(options: UploadInputOptions): Fluent;
    function checkImageConstraints(file: UploadResponse, opt: FileUploadConstraints): boolean;
    function fileNameSizeDisplay(name: string, bytes: number): string;
    function fileSizeDisplay(bytes: number): string;
    function hasImageExtension(filename: string): boolean;
    function thumbFileName(filename: string): string;
    function dbFileUrl(filename: string): string;
    function colorBox(link: HTMLElement | ArrayLike<HTMLElement>, options: any): void;
    function populateFileSymbols(c: HTMLElement | ArrayLike<HTMLElement>, items: UploadedFile[], displayOriginalName?: boolean, urlPrefix?: string): void;
}
interface UploadedFile {
    Filename?: string;
    OriginalName?: string;
}
interface UploadInputOptions {
    container?: HTMLElement | ArrayLike<HTMLElement>;
    zone?: HTMLElement | ArrayLike<HTMLElement>;
    progress?: HTMLElement | ArrayLike<HTMLElement>;
    inputName?: string;
    allowMultiple?: boolean;
    uploadIntent?: string;
    uploadUrl?: string;
    fileDone?: (p1: UploadResponse, p2: string, p3: any) => void;
}
interface UploadResponse extends ServiceResponse {
    TemporaryFile: string;
    Size: number;
    IsImage: boolean;
    Width: number;
    Height: number;
}
interface FileUploadConstraints {
    minWidth?: number;
    maxWidth?: number;
    minHeight?: number;
    maxHeight?: number;
    minSize?: number;
    maxSize?: number;
    allowNonImage?: boolean;
    originalNameProperty?: string;
}

interface FileUploadEditorOptions extends FileUploadConstraints {
    displayFileName?: boolean;
    uploadIntent?: string;
    uploadUrl?: string;
    urlPrefix?: string;
}
interface ImageUploadEditorOptions extends FileUploadEditorOptions {
}
declare class FileUploadEditor<P extends FileUploadEditorOptions = FileUploadEditorOptions> extends EditorWidget<P> implements IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired {
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
    protected progress: Fluent;
    protected fileSymbols: Fluent;
    protected uploadInput: Fluent;
    protected hiddenInput: Fluent;
}
declare class ImageUploadEditor<P extends ImageUploadEditorOptions = ImageUploadEditorOptions> extends FileUploadEditor<P> {
    constructor(props: EditorProps<P>);
}
declare class MultipleFileUploadEditor<P extends FileUploadEditorOptions = FileUploadEditorOptions> extends EditorWidget<P> implements IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired {
    private entities;
    private toolbar;
    private fileSymbols;
    private uploadInput;
    protected progress: Fluent;
    protected hiddenInput: Fluent;
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
    jsonEncodeValue: boolean;
}
declare class MultipleImageUploadEditor<P extends ImageUploadEditorOptions = ImageUploadEditorOptions> extends MultipleFileUploadEditor<P> {
    constructor(props: EditorProps<P>);
}

interface QuickFilterArgs<TWidget> {
    field?: string;
    widget?: TWidget;
    request?: ListRequest;
    equalityFilter?: any;
    value?: any;
    active?: boolean;
    handled?: boolean;
}
interface QuickFilter<TWidget extends Widget<P>, P> {
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

interface QuickFilterBarOptions {
    filters: QuickFilter<Widget<any>, any>[];
    getTitle?: (filter: QuickFilter<Widget<any>, any>) => string;
    idPrefix?: string;
}
declare class QuickFilterBar<P extends QuickFilterBarOptions = QuickFilterBarOptions> extends Widget<P> {
    constructor(props: WidgetProps<P>);
    addSeparator(): void;
    add<TWidget extends Widget<any>, TOptions>(opt: QuickFilter<TWidget, TOptions>): TWidget;
    addDateRange(field: string, title?: string): DateEditor;
    static dateRange(field: string, title?: string): QuickFilter<DateEditor, DateTimeEditorOptions>;
    addDateTimeRange(field: string, title?: string): DateTimeEditor<DateTimeEditorOptions>;
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

interface QuickSearchField {
    name: string;
    title: string;
}
interface QuickSearchInputOptions {
    typeDelay?: number;
    loadingParentClass?: string;
    filteredParentClass?: string;
    onSearch?: (p1: string, p2: string, p3: (p1: boolean) => void) => void;
    fields?: QuickSearchField[];
}
declare class QuickSearchInput<P extends QuickSearchInputOptions = QuickSearchInputOptions> extends Widget<P> {
    static createDefaultElement(): HTMLInputElement;
    readonly domNode: HTMLInputElement;
    private lastValue;
    private field;
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

interface FilterOperator {
    key?: string;
    title?: string;
    format?: string;
}
declare namespace FilterOperators {
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

interface FilterLine {
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

declare class FilterStore {
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
declare function delegateCombine(delegate1: any, delegate2: any): any;
declare function delegateRemove(delegate1: any, delegate2: any): any;
declare function delegateContains(targets: any[], object: any, method: any): boolean;

interface IFiltering {
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
declare class IFiltering {
}
interface CriteriaWithText {
    criteria?: any[];
    displayText?: string;
}
interface IQuickFiltering {
    initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
}
declare class IQuickFiltering {
}
declare abstract class BaseFiltering implements IFiltering, IQuickFiltering {
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
}
declare abstract class BaseEditorFiltering<TEditor extends Widget<any>> extends BaseFiltering {
    editorType: any;
    constructor(editorType: any);
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
declare class DateFiltering extends BaseEditorFiltering<DateEditor> {
    constructor();
    getOperators(): FilterOperator[];
}
declare class BooleanFiltering extends BaseFiltering {
    getOperators(): FilterOperator[];
}
declare class DateTimeFiltering extends BaseEditorFiltering<DateEditor> {
    constructor();
    getOperators(): FilterOperator[];
    getCriteria(): CriteriaWithText;
}
declare class DecimalFiltering extends BaseEditorFiltering<DecimalEditor> {
    constructor();
    getOperators(): FilterOperator[];
}
declare class EditorFiltering extends BaseEditorFiltering<Widget<any>> {
    constructor();
    editorType: string;
    useRelative: boolean;
    useLike: boolean;
    getOperators(): FilterOperator[];
    protected useEditor(): boolean;
    getEditorOptions(): any;
    createEditor(): void;
    protected useIdField(): boolean;
    initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
}
declare class EnumFiltering extends BaseEditorFiltering<EnumEditor> {
    constructor();
    getOperators(): FilterOperator[];
}
declare class IntegerFiltering extends BaseEditorFiltering<IntegerEditor> {
    constructor();
    getOperators(): FilterOperator[];
}
declare class LookupFiltering extends BaseEditorFiltering<LookupEditor> {
    constructor();
    getOperators(): FilterOperator[];
    protected useEditor(): boolean;
    protected useIdField(): boolean;
    getEditorText(): string;
}
declare class ServiceLookupFiltering extends BaseEditorFiltering<ServiceLookupEditor> {
    constructor();
    getOperators(): FilterOperator[];
    protected useEditor(): boolean;
    protected useIdField(): boolean;
    getEditorText(): string;
}
declare class StringFiltering extends BaseFiltering {
    getOperators(): FilterOperator[];
    validateEditorValue(value: string): string;
}
declare namespace FilteringTypeRegistry {
    function get(key: string): Function;
}

declare class FilterWidgetBase<P = {}> extends TemplatedWidget<P> {
    private store;
    private onFilterStoreChanged;
    constructor(props: WidgetProps<P>);
    destroy(): void;
    protected filterStoreChanged(): void;
    get_store(): FilterStore;
    set_store(value: FilterStore): void;
}

interface FilterFieldSelectOptions {
    fields: PropertyItem[];
}
declare class FilterPanel<P = {}> extends FilterWidgetBase<P> {
    private rowsDiv;
    constructor(props: WidgetProps<P>);
    private showInitialLine;
    get_showInitialLine(): boolean;
    set_showInitialLine(value: boolean): void;
    protected filterStoreChanged(): void;
    updateRowsFromStore(): void;
    private showSearchButton;
    get_showSearchButton(): boolean;
    set_showSearchButton(value: boolean): void;
    private updateStoreOnReset;
    get_updateStoreOnReset(): boolean;
    set_updateStoreOnReset(value: boolean): void;
    protected getTemplate(): string;
    protected initButtons(): void;
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

declare class FilterDialog<P = {}> extends TemplatedDialog<P> {
    private filterPanel;
    constructor(props: WidgetProps<P>);
    get_filterPanel(): FilterPanel;
    protected getTemplate(): string;
    protected getDialogButtons(): {
        text: string;
        click: () => void;
    }[];
}

declare class FilterDisplayBar<P = {}> extends FilterWidgetBase<P> {
    protected renderContents(): HTMLDivElement;
    protected filterStoreChanged(): void;
}

declare class SlickPager<P extends PagerOptions = PagerOptions> extends Widget<P> {
    private currentPage;
    private totalPages;
    private pageSize;
    private stat;
    constructor(props: WidgetProps<P>);
    _changePage(ctype: string): boolean;
    _updatePager(): void;
}

interface IDataGrid {
    getElement(): HTMLElement;
    getGrid(): Grid;
    getView(): RemoteView<any>;
    getFilterStore(): FilterStore;
}

interface GridRowSelectionMixinOptions {
    selectable?: (item: any) => boolean;
}
declare class GridRowSelectionMixin {
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
interface GridRadioSelectionMixinOptions {
    selectable?: (item: any) => boolean;
}
declare class GridRadioSelectionMixin {
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
declare namespace GridSelectAllButtonHelper {
    function update(grid: IDataGrid, getSelected: (p1: any) => boolean): void;
    function define(getGrid: () => IDataGrid, getId: (p1: any) => any, getSelected: (p1: any) => boolean, setSelected: (p1: any, p2: boolean) => void, text?: string, onClick?: () => void): ToolButton;
}
declare namespace GridUtils {
    function addToggleButton(toolDiv: HTMLElement | ArrayLike<HTMLElement>, cssClass: string, callback: (p1: boolean) => void, hint: string, initial?: boolean): void;
    function addIncludeDeletedToggle(toolDiv: HTMLElement | ArrayLike<HTMLElement>, view: RemoteView<any>, hint?: string, initial?: boolean): void;
    function addQuickSearchInput(toolDiv: HTMLElement | ArrayLike<HTMLElement>, view: RemoteView<any>, fields?: QuickSearchField[], onChange?: () => void): QuickSearchInput;
    function addQuickSearchInputCustom(container: HTMLElement | ArrayLike<HTMLElement>, onSearch: (p1: string, p2: string, done: (p3: boolean) => void) => void, fields?: QuickSearchField[]): QuickSearchInput;
    function makeOrderable(grid: Grid, handleMove: (rows: number[], insertBefore: number) => void): void;
    function makeOrderableWithUpdateRequest<TItem = any, TId = any>(grid: IDataGrid, getId: (item: TItem) => TId, getDisplayOrder: (item: TItem) => any, service: string, getUpdateRequest: (id: TId, order: number) => SaveRequest<TItem>): void;
}
declare namespace PropertyItemSlickConverter {
    function toSlickColumns(items: PropertyItem[]): Column[];
    function toSlickColumn(item: PropertyItem): Column;
}
declare namespace SlickFormatting {
    function getEnumText(enumKey: string, name: string): string;
    function treeToggle(getView: () => RemoteView<any>, getId: (x: any) => any, formatter: Format): Format;
    function date(format?: string): Format;
    function dateTime(format?: string): Format;
    function checkBox(): Format;
    function number(format: string): Format;
    function getItemType(link: HTMLElement | ArrayLike<HTMLElement>): string;
    function getItemId(link: HTMLElement | ArrayLike<HTMLElement>): string;
    function itemLinkText(itemType: string, id: any, text: any, extraClass: string, encode: boolean): string;
    function itemLink<TItem = any>(itemType: string, idField: string, getText: Format<TItem>, cssClass?: Format<TItem>, encode?: boolean): Format<TItem>;
}
declare namespace SlickHelper {
    function setDefaults(columns: Column[], localTextPrefix?: string): any;
}
declare namespace SlickTreeHelper {
    function filterCustom<TItem>(item: TItem, getParent: (x: TItem) => any): boolean;
    function filterById<TItem>(item: TItem, view: RemoteView<TItem>, getParentId: (x: TItem) => any): boolean;
    function setCollapsed<TItem>(items: TItem[], collapsed: boolean): void;
    function setCollapsedFlag<TItem>(item: TItem, collapsed: boolean): void;
    function setIndents<TItem>(items: TItem[], getId: (x: TItem) => any, getParentId: (x: TItem) => any, setCollapsed?: boolean): void;
    function toggleClick<TItem>(e: Event, row: number, cell: number, view: RemoteView<TItem>, getId: (x: TItem) => any): void;
}
declare class ColumnsBase<TRow = any> {
    constructor(items: Column<TRow>[]);
    valueOf(): Column<TRow>[];
}

interface IInitializeColumn {
    initializeColumn(column: Column): void;
}
declare class IInitializeColumn {
}
declare class BooleanFormatter implements Formatter {
    format(ctx: FormatterContext): string;
    falseText: string;
    trueText: string;
}
declare class CheckboxFormatter implements Formatter {
    static typeInfo: FormatterTypeInfo<"Serenity.CheckboxFormatter">;
    format(ctx: FormatterContext): string;
}
declare class DateFormatter implements Formatter {
    constructor();
    static format(value: any, format?: string): any;
    displayFormat: string;
    format(ctx: FormatterContext): string;
}
declare class DateTimeFormatter extends DateFormatter {
    constructor();
}
declare class EnumFormatter implements Formatter {
    format(ctx: FormatterContext): string;
    enumKey: string;
    static format(enumType: any, value: any): string;
    static getText(enumKey: string, name: string): string;
    static getName(enumType: any, value: any): string;
}
declare class FileDownloadFormatter implements Formatter, IInitializeColumn {
    format(ctx: FormatterContext): string;
    static dbFileUrl(filename: string): string;
    initializeColumn(column: Column): void;
    displayFormat: string;
    originalNameProperty: string;
    iconClass: string;
}
declare class MinuteFormatter implements Formatter {
    format(ctx: FormatterContext): string;
    static format(value: number): string;
}
declare class NumberFormatter {
    format(ctx: FormatterContext): string;
    static format(value: any, format?: string): string;
    displayFormat: string;
}
declare class UrlFormatter implements Formatter, IInitializeColumn {
    format(ctx: FormatterContext): string;
    initializeColumn(column: Column): void;
    displayProperty: string;
    displayFormat: string;
    urlProperty: string;
    urlFormat: string;
    target: string;
}

declare namespace FormatterTypeRegistry {
    function get(key: string): any;
    function reset(): void;
    function tryGet(key: string): any;
}

interface SettingStorage {
    getItem(key: string): string | Promise<string>;
    setItem(key: string, value: string): void | Promise<void>;
}
interface PersistedGridColumn {
    id: string;
    width?: number;
    sort?: number;
    visible?: boolean;
}
interface PersistedGridSettings {
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
interface GridPersistanceFlags {
    columnWidths?: boolean;
    columnVisibility?: boolean;
    sortColumns?: boolean;
    filterItems?: boolean;
    quickFilters?: boolean;
    quickFilterText?: boolean;
    quickSearch?: boolean;
    includeDeleted?: boolean;
}
declare class DataGrid<TItem, P = {}> extends Widget<P> implements IDataGrid, IReadOnly {
    private _isDisabled;
    private _layoutTimer;
    private _slickGridOnSort;
    private _slickGridOnClick;
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
    view: RemoteView<TItem>;
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
    static propertyItemToQuickFilter(item: PropertyItem): any;
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
    protected createView(): RemoteView<TItem>;
    protected getDefaultSortBy(): any[];
    protected usePager(): boolean;
    protected enableFiltering(): boolean;
    protected populateWhenVisible(): boolean;
    protected createFilterBar(): void;
    protected getPagerOptions(): PagerOptions;
    protected createPager(): void;
    protected getViewOptions(): RemoteViewOptions;
    protected createToolbar(buttons: ToolButton[]): void;
    getTitle(): string;
    setTitle(value: string): void;
    protected getItemType(): string;
    protected itemLink(itemType?: string, idField?: string, text?: (ctx: FormatterContext) => string, cssClass?: (ctx: FormatterContext) => string, encode?: boolean): Format<TItem>;
    protected getColumnsKey(): string;
    protected getPropertyItems(): PropertyItem[];
    protected getPropertyItemsData(): PropertyItemsData;
    protected getPropertyItemsDataAsync(): Promise<PropertyItemsData>;
    protected getColumns(): Column<TItem>[];
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
    protected persistSettings(flags?: GridPersistanceFlags): void | Promise<void>;
    protected getCurrentSettings(flags?: GridPersistanceFlags): PersistedGridSettings;
    getElement(): HTMLElement;
    getGrid(): Grid;
    getView(): RemoteView<TItem>;
    getFilterStore(): FilterStore;
}

declare class ColumnPickerDialog<P = {}> extends TemplatedDialog<P> {
    private ulVisible;
    private ulHidden;
    private colById;
    allColumns: Column[];
    visibleColumns: string[];
    defaultColumns: string[];
    done: () => void;
    constructor(props: WidgetProps<P>);
    protected renderContents(): Fluent<HTMLDivElement>;
    static createToolButton(grid: IDataGrid): ToolButton;
    protected getDialogButtons(): DialogButton[];
    protected getUIDialogOptions(): any;
    private getTitle;
    private allowHide;
    private createLI;
    private updateListStates;
    protected setupColumns(): void;
    protected onDialogOpen(): void;
}

/**
 * A mixin that can be applied to a DataGrid for tree functionality
 */
declare class TreeGridMixin<TItem> {
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
interface TreeGridMixinOptions<TItem> {
    grid: DataGrid<TItem, any>;
    getParentId: (item: TItem) => any;
    toggleField: string;
    initialCollapse?: () => boolean;
}

interface CheckTreeItem<TSource> {
    isSelected?: boolean;
    hideCheckBox?: boolean;
    isAllDescendantsSelected?: boolean;
    id?: string;
    text?: string;
    parentId?: string;
    children?: CheckTreeItem<TSource>[];
    source?: TSource;
}
declare class CheckTreeEditor<TItem extends CheckTreeItem<TItem>, P = {}> extends DataGrid<TItem, P> implements IGetEditValue, ISetEditValue, IReadOnly {
    static createDefaultElement(): HTMLDivElement;
    private byId;
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
interface CheckLookupEditorOptions {
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
declare class CheckLookupEditor<TItem extends CheckTreeItem<TItem> = any, P extends CheckLookupEditorOptions = CheckLookupEditorOptions> extends CheckTreeEditor<CheckTreeItem<TItem>, P> {
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

declare class EntityGrid<TItem, P = {}> extends DataGrid<TItem, P> {
    constructor(props: WidgetProps<P>);
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
    protected getViewOptions(): RemoteViewOptions;
    protected getItemType(): string;
    protected routeDialog(itemType: string, dialog: Widget<any>): void;
    protected getInsertPermission(): string;
    protected hasInsertPermission(): boolean;
    protected transferDialogReadOnly(dialog: Widget<any>): void;
    protected initDialog(dialog: Widget<any>): void;
    protected initEntityDialog(itemType: string, dialog: Widget<any>): void;
    protected createEntityDialog(itemType: string, callback?: (dlg: Widget<any>) => void): Widget<any>;
    protected getDialogOptions(): any;
    protected getDialogOptionsFor(itemType: string): any;
    protected getDialogTypeFor(itemType: string): {
        new (...args: any[]): Widget<any>;
    };
    private _dialogType;
    protected getDialogType(): {
        new (...args: any[]): Widget<any>;
    };
}

declare class EntityDialog<TItem, P = {}> extends TemplatedDialog<P> implements IEditDialog, IReadOnly {
    protected entity: TItem;
    protected entityId: any;
    protected propertyItemsData: PropertyItemsData;
    protected propertyGrid: PropertyGrid;
    protected toolbar: Toolbar;
    protected saveAndCloseButton: Fluent;
    protected applyChangesButton: Fluent;
    protected deleteButton: Fluent;
    protected undeleteButton: Fluent;
    protected cloneButton: Fluent;
    protected editButton: Fluent;
    protected localizationGrid: PropertyGrid;
    protected localizationButton: Fluent;
    protected localizationPendingValue: any;
    protected localizationLastValue: any;
    static defaultLanguageList: () => string[][];
    constructor(props?: WidgetProps<P>);
    protected propertyItemsReady(itemsData: PropertyItemsData): void;
    protected afterInit(): void;
    protected useAsync(): boolean;
    destroy(): void;
    protected get_entity(): TItem;
    protected set_entity(entity: any): void;
    protected get_entityId(): any;
    protected set_entityId(value: any): void;
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
    load(entityOrId: any, done: () => void, fail?: (ex: Exception) => void): void;
    loadNewAndOpenDialog(asPanel?: boolean): void;
    loadEntityAndOpenDialog(entity: TItem, asPanel?: boolean): void;
    protected loadResponse(data: any): void;
    protected loadEntity(entity: TItem): void;
    protected beforeLoadEntity(entity: TItem): void;
    protected afterLoadEntity(): void;
    loadByIdAndOpenDialog(entityId: any, asPanel?: boolean): void;
    protected onLoadingData(data: RetrieveResponse<TItem>): void;
    protected getLoadByIdOptions(id: any, callback: (response: RetrieveResponse<TItem>) => void): ServiceOptions<RetrieveResponse<TItem>>;
    protected getLoadByIdRequest(id: any): RetrieveRequest;
    protected reloadById(): void;
    loadById(id: any, callback?: (response: RetrieveResponse<TItem>) => void, fail?: () => void): void;
    protected loadByIdHandler(options: ServiceOptions<RetrieveResponse<TItem>>, callback: (response: RetrieveResponse<TItem>) => void, fail: () => void): void;
    protected initLocalizationGrid(): void;
    protected initLocalizationGridCommon(pgOptions: PropertyGridOptions): void;
    protected isLocalizationMode(): boolean;
    protected isLocalizationModeAndChanged(): boolean;
    protected localizationButtonClick(): void;
    protected getLanguages(): any[];
    private getLangs;
    protected loadLocalization(): void;
    protected setLocalizationGridCurrentValues(): void;
    protected getLocalizationGridValue(): any;
    protected getPendingLocalizations(): any;
    protected initPropertyGrid(): void;
    protected getPropertyItems(): PropertyItem[];
    protected getPropertyItemsData(): PropertyItemsData;
    protected getPropertyItemsDataAsync(): Promise<PropertyItemsData>;
    protected getPropertyGridOptions(): PropertyGridOptions;
    protected validateBeforeSave(): boolean;
    protected getSaveOptions(callback: (response: SaveResponse) => void): ServiceOptions<SaveResponse>;
    protected getSaveEntity(): TItem;
    protected getSaveRequest(): SaveRequest<TItem>;
    protected onSaveSuccess(response: SaveResponse): void;
    protected save_submitHandler(callback: (response: SaveResponse) => void): void;
    protected save(callback?: (response: SaveResponse) => void): void | boolean;
    protected saveHandler(options: ServiceOptions<SaveResponse>, callback: (response: SaveResponse) => void): void;
    protected initToolbar(): void;
    protected showSaveSuccessMessage(response: SaveResponse): void;
    protected getToolbarButtons(): ToolButton[];
    protected getCloningEntity(): TItem;
    protected updateInterface(): void;
    protected getUndeleteOptions(callback?: (response: UndeleteResponse) => void): ServiceOptions<UndeleteResponse>;
    protected undeleteHandler(options: ServiceOptions<UndeleteResponse>, callback: (response: UndeleteResponse) => void): void;
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
    protected getTemplate(): string;
}

/**
 * ## Serenity Core Library
 *
 * This is the package containing core TypeScript classes and functions used in Serenity applications.
 *
 * It should be installed by default in your projects created from `Serene` or `StartSharp` template:
 *
 * ```json
 * {
 *   "dependencies": {
 *     // ...
 *     "@serenity-is/corelib": "latest"
 *   }
 * }
 * ```
 *
 * The version number for this package should be equal or as close as possible to Serenity NuGet package versions in your project file.
 *
 * > When using classic namespaces instead of the ESM modules, the types and functions in this module are directly available from the global `Serenity` and `Q` namespaces.
 * > e.g. `Serenity.EntityGrid`
 * @packageDocumentation
 */

type Constructor<T> = new (...args: any[]) => T;

export { AggregateFormatting, Aggregators, type AnyIconClass, ArgumentNullException, Authorization, BaseEditorFiltering, BaseFiltering, BooleanEditor, BooleanFiltering, BooleanFormatter, type CKEditorConfig, type CancellableViewCallback, CaptureOperationType, CascadedWidgetLink, CategoryAttribute, CheckLookupEditor, type CheckLookupEditorOptions, CheckTreeEditor, type CheckTreeItem, CheckboxFormatter, type ClassTypeInfo, ColumnPickerDialog, ColumnSelection, ColumnsBase, ColumnsKeyAttribute, Config, type ConfirmDialogOptions, type Constructor, type CreateWidgetParams, Criteria, CriteriaBuilder, CriteriaOperator, type CriteriaWithText, CssClassAttribute, Culture, type DataChangeInfo, DataGrid, DateEditor, type DateEditorOptions, DateFiltering, type DateFormat, DateFormatter, DateTimeEditor, type DateTimeEditorOptions, DateTimeFiltering, DateTimeFormatter, DateYearEditor, type DateYearEditorOptions, type DebouncedFunction, DecimalEditor, type DecimalEditorOptions, DecimalFiltering, Decorators, DefaultValueAttribute, type DeleteRequest, type DeleteResponse, type DialogButton, DialogExtensions, type DialogOptions, DialogTexts, type DialogType, DialogTypeAttribute, DialogTypeRegistry, type Dictionary, DisplayNameAttribute, EditorAttribute, EditorFiltering, EditorOptionAttribute, type EditorProps, EditorTypeAttribute, EditorTypeAttributeBase, type EditorTypeInfo, EditorTypeRegistry, EditorUtils, EditorWidget, ElementAttribute, EmailAddressEditor, EmailEditor, type EmailEditorOptions, EntityDialog, EntityGrid, EntityTypeAttribute, Enum, EnumEditor, type EnumEditorOptions, EnumFiltering, EnumFormatter, EnumKeyAttribute, EnumTypeRegistry, ErrorHandling, Exception, FileDownloadFormatter, type FileUploadConstraints, FileUploadEditor, type FileUploadEditorOptions, FilterDialog, FilterDisplayBar, type FilterFieldSelectOptions, type FilterLine, type FilterOperator, FilterOperators, FilterPanel, FilterStore, FilterWidgetBase, FilterableAttribute, FilteringTypeRegistry, FlexifyAttribute, Fluent, FormKeyAttribute, type Format, type Formatter, type FormatterTypeInfo, FormatterTypeRegistry, GeneratedCodeAttribute, GridPageInit, type GridPersistanceFlags, GridRadioSelectionMixin, type GridRadioSelectionMixinOptions, GridRowSelectionMixin, type GridRowSelectionMixinOptions, GridSelectAllButtonHelper, GridUtils, type GroupByElement, type GroupByResult, type GroupInfo, type Grouping, H, type HandleRouteEvent, HiddenAttribute, HintAttribute, HtmlContentEditor, type HtmlContentEditorOptions, HtmlNoteContentEditor, HtmlReportContentEditor, IBooleanValue, type ICommonDialog, type IDataGrid, IDialog, IDoubleValue, IEditDialog, IFiltering, type IFrameDialogOptions, IGetEditValue, IInitializeColumn, IQuickFiltering, IReadOnly, type IRowDefinition, ISetEditValue, ISlickFormatter, IStringValue, IValidateRequired, type IconClassName, type IdPrefixType, IdPropertyAttribute, ImageUploadEditor, type ImageUploadEditorOptions, InsertableAttribute, IntegerEditor, type IntegerEditorOptions, IntegerFiltering, type InterfaceTypeInfo, InvalidCastException, Invariant, IsActivePropertyAttribute, ItemNameAttribute, type KnownIconClass, LT, LayoutTimer, LazyLoadHelper, type ListRequest, type ListResponse, LocalTextPrefixAttribute, type Locale, Lookup, LookupEditor, LookupEditorBase, type LookupEditorOptions, LookupFiltering, type LookupOptions, MaskedEditor, type MaskedEditorOptions, MaxLengthAttribute, MaximizableAttribute, MemberType, type MessageDialogOptions, MinuteFormatter, MultipleFileUploadEditor, MultipleImageUploadEditor, NamePropertyAttribute, type NoInfer, type NotifyMap, type NumberFormat, NumberFormatter, OneWayAttribute, OptionAttribute, OptionsTypeAttribute, type PagerOptions, type PagingInfo, type PagingOptions, PanelAttribute, PanelPageInit, PasswordEditor, type PersistedGridColumn, type PersistedGridSettings, PlaceholderAttribute, PopupMenuButton, type PopupMenuButtonOptions, PopupToolButton, type PopupToolButtonOptions, type PostToServiceOptions, type PostToUrlOptions, PrefixedContext, PropertyDialog, PropertyGrid, PropertyGridMode, type PropertyGridOptions, type PropertyItem, PropertyItemSlickConverter, type PropertyItemsData, PropertyPanel, type QuickFilter, type QuickFilterArgs, QuickFilterBar, type QuickFilterBarOptions, type QuickSearchField, QuickSearchInput, type QuickSearchInputOptions, RadioButtonEditor, type RadioButtonEditorOptions, ReadOnlyAttribute, Recaptcha, type RecaptchaOptions, ReflectionOptionsSetter, ReflectionUtils, RemoteView, type RemoteViewAjaxCallback, type RemoteViewFilter, type RemoteViewOptions, type RemoteViewProcessCallback, type RequestErrorInfo, RequiredAttribute, ResizableAttribute, ResponsiveAttribute, RetrieveColumnSelection, type RetrieveLocalizationRequest, type RetrieveLocalizationResponse, type RetrieveRequest, type RetrieveResponse, Router, type SaveRequest, type SaveRequestWithAttachment, type SaveResponse, type SaveWithLocalizationRequest, ScriptData, type Select2CommonOptions, Select2Editor, type Select2EditorOptions, type Select2FilterOptions, type Select2InplaceAddOptions, type Select2SearchPromise, type Select2SearchQuery, type Select2SearchResult, SelectEditor, type SelectEditorOptions, ServiceAttribute, type ServiceError, ServiceLookupEditor, ServiceLookupEditorBase, type ServiceLookupEditorOptions, ServiceLookupFiltering, type ServiceOptions, type ServiceRequest, type ServiceResponse, type SettingStorage, SlickFormatting, SlickHelper, SlickPager, SlickTreeHelper, StringEditor, StringFiltering, type StringLiteral, SubDialogHelper, type SummaryOptions, SummaryType, TabsExtensions, TemplatedDialog, TemplatedPanel, TemplatedWidget, TextAreaEditor, type TextAreaEditorOptions, type TextColor, TimeEditor, type TimeEditorOptions, type ToastContainerOptions, Toastr, type ToastrOptions, type ToolButton, type ToolButtonProps, Toolbar, ToolbarButton, type ToolbarOptions, TreeGridMixin, type TreeGridMixinOptions, type Type, type TypeMember, URLEditor, type UndeleteRequest, type UndeleteResponse, UpdatableAttribute, UploadHelper, type UploadInputOptions, type UploadResponse, type UploadedFile, UrlFormatter, type UserDefinition, type UtilityColor, ValidationHelper, Widget, type WidgetProps, addClass, addCustomAttribute, addEmptyOption, addLocalText, addOption, addTypeMember, addValidationRule, alert, alertDialog, any, appendChild, associateWidget, attachToDialogBeforeCloseEvent, attachToDialogCloseEvent, attrEncode, baseValidateOptions, bgColor, blockUI, blockUndo, canLoadScriptData, cancelDialogButton, cast, centerDialog, classTypeInfo, clearKeys, clearOptions, closeDialog, closePanel, coalesce, compareStringFactory, confirm, confirmDialog, count, createCommonDialog, datePickerIconSvg, dbText, dbTryText, deassociateWidget, debounce, deepClone, defaultNotifyOptions, delegateCombine, delegateContains, delegateRemove, dialogButtonToBS, dialogButtonToUI, editorTypeInfo, endsWith, executeEverytimeWhenVisible, executeOnceWhenVisible, extend, faIcon, type faIconKey, fabIcon, type fabIconKey, fetchScriptData, fieldsProxy, findElementWithRelativeId, first, format, formatDate, formatDayHourAndMin, formatISODateTimeUTC, formatNumber, formatterTypeInfo, getActiveRequests, getBaseType, getColumns, getColumnsAsync, getColumnsData, getColumnsDataAsync, getColumnsScript, getCookie, getCustomAttribute, getCustomAttributes, getDialogBodyElement, getDialogEventTarget, getDialogRootElement, getForm, getFormAsync, getFormData, getFormDataAsync, getFormScript, getGlobalObject, getHighlightTarget, getInstanceType, getLookup, getLookupAsync, getMembers, getNested, getRemoteData, getRemoteDataAsync, getScriptData, getScriptDataHash, getTemplate, getType, getTypeFullName, getTypeNameProp, getTypeRegistry, getTypeShortName, getTypes, getWidgetFrom, getWidgetName, getjQuery, gridPageInit, groupBy, handleScriptDataError, hasCustomAttribute, htmlEncode, iconClassName, iframeDialog, indexOf, information, informationDialog, initFormType, initFullHeightGridPage, initializeTypes, inputLikeSelector, insert, interfaceTypeInfo, isArray, isArrayLike, isAssignableFrom, isBS3, isBS5Plus, isEmptyOrNull, isEnum, isInputLike, isInputTag, isInstanceOfType, isMobileView, isPromiseLike, isSameOrigin, isTrimmedEmpty, isValue, keyOf, layoutFillHeight, layoutFillHeightValue, loadValidationErrorMessages, localText, localeFormat, newBodyDiv, noDialogButton, notifyError, notifyInfo, notifySuccess, notifyWarning, okDialogButton, openPanel, outerHtml, padLeft, panelPageInit, parseCriteria, parseDate, parseDayHourAndMin, parseDecimal, parseHourAndMin, parseISODateTime, parseInteger, parseQueryString, peekScriptData, positionToastContainer, postToService, postToUrl, prefixedText, proxyTexts, registerClass, registerEditor, registerEnum, registerFormatter, registerInterface, registerType, reloadLookup, reloadLookupAsync, removeClass, removeValidationRule, replaceAll, requestFinished, requestStarting, resolveServiceUrl, resolveUrl, round, safeCast, select2LocaleInitialization, serviceCall, serviceRequest, setEquality, setRegisteredScripts, setScriptData, setTypeNameProp, single, splitDateString, startsWith, stringFormat, stringFormatLocale, success, successDialog, text, textColor, toClassName, toGrouping, toId, toSingleLine, today, toggleClass, triggerLayoutOnShow, trim, trimEnd, trimStart, trimToEmpty, trimToNull, trunc, tryFirst, tryGetText, tryGetWidget, turkishLocaleCompare, turkishLocaleToLower, turkishLocaleToUpper, typeInfoProperty, useIdPrefix, validateForm, validateOptions, validatorAbortHandler, warning, warningDialog, yesDialogButton, zeroPad };
