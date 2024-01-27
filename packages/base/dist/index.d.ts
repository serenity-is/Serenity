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
    click?: (e: MouseEvent) => void | false | Promise<void | false>;
    /** CSS class for button */
    cssClass?: string;
    /** The code that is returned from message dialog function when this button is clicked.
     *  If this is set, and click event will not be defaultPrevented dialog will close.
     */
    result?: string;
}
type DialogType = "bsmodal" | "uidialog" | "panel";
/**
 * Options that apply to all dialog types
 */
interface DialogOptions {
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
    providerOptions?: (type: DialogType, opt: DialogOptions) => any;
    /** Scrollable, sets content of the modal to scrollable, only for Bootstrap */
    scrollable?: boolean;
    /** Size. Default is null for (500px) message dialogs, lg for normal dialogs */
    size?: "sm" | "md" | "lg" | "xl";
    /** Dialog title */
    title?: string;
    /** Only used for jQuery UI dialogs for backwards compatibility */
    width?: number;
}
declare class Dialog {
    private el;
    private dialogResult;
    constructor(opt?: DialogOptions);
    static defaults: DialogOptions;
    static messageDefaults: MessageDialogOptions;
    static getInstance(el: HTMLElement | ArrayLike<HTMLElement>): Dialog;
    /** The result code of the button that is clicked. Also attached to the dialog element as data-dialog-result */
    get result(): string;
    /** Closes dialog setting the result to null */
    close(): this;
    /** Closes dialog with the result set to value */
    close(result: string): this;
    onClose(handler: (result?: string, e?: Event) => void, before?: boolean): void;
    onOpen(handler: (e?: Event) => void, before?: boolean): this;
    /** Closes dialog */
    open(): this;
    /** Gets the title text of the dialog */
    title(): string;
    /** Sets the title text of the dialog. */
    title(value: string): this;
    get type(): DialogType;
    /** Gets the body/content element of the dialog */
    getContentNode(): HTMLElement;
    /** Gets the dialog element of the dialog */
    getDialogNode(): HTMLElement;
    /** Gets the node that receives events for the dialog. It's .ui-dialog-content, .modal, or .s-Panel */
    getEventsNode(): HTMLElement;
    /** Gets the footer element of the dialog */
    getFooterNode(): HTMLElement;
    /** Gets the header element of the dialog */
    getHeaderNode(): HTMLElement;
    private onButtonClick;
    private createBSButtons;
    createBSModal(opt: DialogOptions): void;
    private createPanel;
    createUIDialog(opt: DialogOptions): void;
    dispose(): void;
}
declare function hasBSModal(): boolean;
declare function hasUIDialog(): boolean;
declare function okDialogButton(opt?: DialogButton): DialogButton;
declare function yesDialogButton(opt?: DialogButton): DialogButton;
declare function noDialogButton(opt?: DialogButton): DialogButton;
declare function cancelDialogButton(opt?: DialogButton): DialogButton;
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
/**
 * Options that apply to all message dialog types
 */
interface MessageDialogOptions extends DialogOptions {
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
declare function alertDialog(message: string, options?: MessageDialogOptions): Partial<Dialog>;
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
declare function confirmDialog(message: string, onYes: () => void, options?: ConfirmDialogOptions): Partial<Dialog>;
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
declare function informationDialog(message: string, onOk?: () => void, options?: MessageDialogOptions): Partial<Dialog>;
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
declare function successDialog(message: string, onOk?: () => void, options?: MessageDialogOptions): Partial<Dialog>;
/**
 * Display a warning dialog
 * @param message The message to display
 * @param options Additional options.
 * @see MessageDialogOptions
 * @example
 * warningDialog("Something is odd!");
 */
declare function warningDialog(message: string, options?: MessageDialogOptions): Partial<Dialog>;
/** Options for `iframeDialog` **/
interface IFrameDialogOptions {
    html?: string;
}
/**
 * Display a dialog that shows an HTML block in an IFRAME, which is usually returned from server callbacks
 * @param options The options
 */
declare function iframeDialog(options: IFrameDialogOptions): Partial<Dialog>;

declare function getjQuery(): any;
/** Returns true if Bootstrap 3 is loaded */
declare function isBS3(): boolean;
/** Returns true if Bootstrap 5+ is loaded */
declare function isBS5Plus(): boolean;

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
    findFirst<TElement extends HTMLElement = HTMLElement>(selector: string): Fluent<TElement>;
    findAll<TElement extends HTMLElement = HTMLElement>(selector: string): TElement[];
    hasClass(klass: string): boolean;
    hide(): this;
    insertAfter(referenceNode: HTMLElement | Fluent<HTMLElement>): this;
    insertBefore(referenceNode: HTMLElement | Fluent<HTMLElement>): this;
    [Symbol.iterator]: TElement[];
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
    var ready: (callback: () => void) => void;
}
declare namespace Fluent {
    const off: typeof EventHandler.off;
    const on: typeof EventHandler.on;
    const one: typeof EventHandler.on;
    const trigger: typeof EventHandler.trigger;
    function addClass(el: Element, value: string | boolean | (string | boolean)[]): void;
    function empty(el: Element): void;
    /** For compatibility with jQuery's :visible selector, e.g. has offsetWidth or offsetHeight or any client rect */
    function isVisibleLike(el: Element): boolean;
    function remove(el: Element): any;
    function removeClass(el: Element, value: string | boolean | (string | boolean)[]): void;
    function toggle(el: Element, flag?: boolean): void;
    function toggleClass(el: Element, value: string | boolean | (string | boolean)[], add?: boolean): void;
    function toClassName(value: string | boolean | (string | boolean)[]): string;
    function isInputLike(node: Element): node is (HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement | HTMLButtonElement);
    const inputLikeSelector = "input,select,textarea,button";
    function isInputTag(tag: string): boolean;
}
declare function H<K extends keyof HTMLElementTagNameMap>(tag: K): Fluent<HTMLElementTagNameMap[K]>;

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
declare function omitUndefined(x: {
    [key: string]: any;
}): any;
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

interface TooltipOptions {
    title?: string;
    trigger?: string;
}
declare class Tooltip {
    private el;
    constructor(el: ArrayLike<HTMLElement> | HTMLElement, opt?: TooltipOptions);
    static defaults: TooltipOptions;
    dispose(): void;
    delayedDispose(delay?: number): void;
    delayedHide(delay?: number): void;
    private static existingInstance;
    static getInstance(el: ArrayLike<HTMLElement> | HTMLElement): Tooltip;
    setTitle(value: string): Tooltip;
    toggle(show: boolean): Tooltip;
    hide(): Tooltip;
    show(): Tooltip;
}

/** Inspired from https://github.com/silverwind/uppie and https://github.com/GoogleChromeLabs/file-drop/blob/master/lib/filedrop.ts */
interface UploaderOptions {
    /** Accept. If not specified, read from the passed input  */
    accept?: string;
    /** Auto clear input value after selection, so when same file selected it works. Default is true */
    autoClear?: boolean;
    /** Only used for multiple, default is 1 to upload multiple files in batches of size 1 */
    batchSize?: number;
    /** An optional list of dropzones. */
    dropZone?: HTMLElement | ArrayLike<HTMLElement>;
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
interface UploaderRequest {
    /** A function that will return headers to be sent with request, or static set of headers */
    headers?: Record<string, string>;
    /** Response type expected from the server. Default is json */
    responseType?: "json" | "text";
    /** URL to send the request to. Default is ~/File/TemporaryUpload */
    url?: string;
}
interface UploaderBatch {
    event?: Event;
    filePaths?: string[];
    formData: FormData;
}
interface UploaderSuccessData {
    batch: UploaderBatch;
    request: UploaderRequest;
    event: ProgressEvent;
    xhr: XMLHttpRequest;
    response: any;
}
interface UploaderErrorData {
    batch?: UploaderBatch;
    event?: ProgressEvent;
    exception?: any;
    request?: UploaderRequest;
    response?: any;
    xhr?: XMLHttpRequest;
}
declare class Uploader {
    private opt;
    private batch;
    constructor(opt: UploaderOptions);
    private newBatch;
    private addToBatch;
    private endBatch;
    static defaults: Partial<UploaderOptions>;
    static requestDefaults: Partial<UploaderRequest>;
    private isMultiple;
    private getTypePredicate;
    private getMatchingItems;
    private watchInput;
    private watchDropZone;
    private arrayApi;
    private entriesApi;
    uploadBatch(batch: UploaderBatch, request?: UploaderRequest): Promise<void>;
    static errorHandler: (data: UploaderErrorData) => void;
}

/*!
 * Serenity validator implementation inspired from:
 * jQuery Validation Plugin, https://jqueryvalidation.org/)
 * - and -
 * https://raw.githubusercontent.com/haacked/aspnet-client-validation
 */
/**
 * An `HTMLElement` that can be validated (`input`, `select`, `textarea`, or [contenteditable).
 */
interface ValidatableElement extends HTMLElement {
    form?: HTMLFormElement;
    name?: string;
    type?: string;
    value?: string;
}
type ValidationValue = string | string[] | number | boolean;
/**
 * Validation plugin signature with multitype return.
 * Boolean return signifies the validation result, which uses the default validation error message read from the element attribute.
 * String return signifies failed validation, which then will be used as the validation error message.
 * Promise return signifies asynchronous plugin behavior, with same behavior as Boolean or String.
 */
type ValidationProvider = (value: ValidationValue, element: ValidatableElement, params?: any) => boolean | string | Promise<boolean | string>;
interface ValidationErrorMap {
    [name: string]: (string | boolean);
}
interface ValidationErrorItem {
    message: string;
    element: ValidatableElement;
    method?: string;
}
type ValidationErrorList = ValidationErrorItem[];
type ValidationRules = Record<string, any>;
interface ValidationRulesMap {
    [name: string]: ValidationRules;
}
type ValidateEventDelegate = (element: ValidatableElement, event: Event, validator: Validator) => void;
interface ValidatorOptions {
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
    messages?: Record<string, string> | undefined;
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
    submitHandler?(form: HTMLFormElement, event: Event, validator: Validator): void;
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
declare class Validator {
    static optional(element: ValidatableElement): "" | "dependency-mismatch";
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
    static elementValue(element: HTMLElement): string | number | string[];
    static valid(element: HTMLFormElement | ValidatableElement | ArrayLike<ValidatableElement>): boolean;
    static rules(element: ValidatableElement, command?: "add" | "remove", argument?: any): Record<string, any>;
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
}
declare function addValidationRule(element: HTMLElement | ArrayLike<HTMLElement>, rule: (input: ValidatableElement) => string, uniqueName?: string): void;
declare function removeValidationRule(element: HTMLElement | ArrayLike<HTMLElement>, uniqueName: string): void;

export { type AnyIconClass, type ClassTypeInfo, ColumnSelection, Config, type ConfirmDialogOptions, Criteria, CriteriaBuilder, CriteriaOperator, Culture, type DateFormat, type DebouncedFunction, type DeleteRequest, type DeleteResponse, Dialog, type DialogButton, type DialogOptions, DialogTexts, type DialogType, EditorAttribute, type EditorTypeInfo, Enum, ErrorHandling, Fluent, type FormatterTypeInfo, H, type IFrameDialogOptions, ISlickFormatter, type IconClassName, type InterfaceTypeInfo, Invariant, type KnownIconClass, type ListRequest, type ListResponse, type Locale, Lookup, type LookupOptions, type MessageDialogOptions, type NoInfer, type NotifyMap, type NumberFormat, type PropertyItem, type PropertyItemsData, type RequestErrorInfo, RetrieveColumnSelection, type RetrieveLocalizationRequest, type RetrieveLocalizationResponse, type RetrieveRequest, type RetrieveResponse, type SaveRequest, type SaveRequestWithAttachment, type SaveResponse, type SaveWithLocalizationRequest, type ServiceError, type ServiceOptions, type ServiceRequest, type ServiceResponse, type StringLiteral, SummaryType, type TextColor, type ToastContainerOptions, Toastr, type ToastrOptions, Tooltip, type TooltipOptions, type Type, type UndeleteRequest, type UndeleteResponse, Uploader, type UploaderBatch, type UploaderErrorData, type UploaderOptions, type UploaderRequest, type UploaderSuccessData, type UtilityColor, type ValidatableElement, type ValidationProvider, type ValidationValue, Validator, type ValidatorOptions, addClass, addCustomAttribute, addLocalText, addValidationRule, alertDialog, bgColor, blockUI, blockUndo, cancelDialogButton, classTypeInfo, compareStringFactory, confirmDialog, debounce, defaultNotifyOptions, editorTypeInfo, faIcon, type faIconKey, fabIcon, type fabIconKey, fetchScriptData, fieldsProxy, formatDate, formatISODateTimeUTC, formatNumber, formatterTypeInfo, getActiveRequests, getBaseType, getColumnsScript, getCookie, getCustomAttribute, getCustomAttributes, getFormScript, getGlobalObject, getInstanceType, getLookupAsync, getNested, getRemoteDataAsync, getScriptData, getScriptDataHash, getType, getTypeFullName, getTypeNameProp, getTypeRegistry, getTypeShortName, getjQuery, handleScriptDataError, hasBSModal, hasCustomAttribute, hasUIDialog, htmlEncode, iconClassName, iframeDialog, informationDialog, initFormType, interfaceTypeInfo, isArrayLike, isAssignableFrom, isBS3, isBS5Plus, isEnum, isInstanceOfType, isPromiseLike, isSameOrigin, localText, noDialogButton, notifyError, notifyInfo, notifySuccess, notifyWarning, okDialogButton, omitUndefined, parseCriteria, parseDate, parseDecimal, parseISODateTime, parseInteger, peekScriptData, positionToastContainer, proxyTexts, registerClass, registerEditor, registerEnum, registerFormatter, registerInterface, registerType, reloadLookupAsync, removeClass, removeValidationRule, requestFinished, requestStarting, resolveServiceUrl, resolveUrl, round, serviceCall, serviceRequest, setRegisteredScripts, setScriptData, setTypeNameProp, splitDateString, stringFormat, stringFormatLocale, successDialog, textColor, toId, toggleClass, trunc, tryGetText, typeInfoProperty, warningDialog, yesDialogButton };
