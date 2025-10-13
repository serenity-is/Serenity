import type { ClassNames } from "./classname";
import type { StyleProperties } from "./style-attributes";

export interface HTMLAttributes<T> {
    accept?: string;
    acceptCharset?: string;
    accessKey?: string;
    action?: string;
    allow?: string;
    allowFullscreen?: boolean;
    alt?: string;
    as?: string;
    async?: boolean;
    autocapitalize?: string;
    autocomplete?: string;
    autocorrect?: string;
    autofocus?: boolean;
    autoplay?: boolean;
    capture?: boolean | string;
    cellPadding?: number | string;
    cellSpacing?: number | string;
    charset?: string;
    checked?: boolean;
    class?: ClassNames;
    className?: ClassNames;
    cols?: number;
    colSpan?: number;
    colspan?: number;
    content?: string;
    contentEditable?: boolean;
    controls?: boolean;
    coords?: string;
    crossOrigin?: string;
    data?: string;
    dataset?: { [key: string]: string } | undefined
    dateTime?: string;
    default?: boolean;
    defer?: boolean;
    dir?: "auto" | "rtl" | "ltr";
    disabled?: boolean;
    disableRemotePlayback?: boolean;
    download?: string | boolean;
    decoding?: "sync" | "async" | "auto";
    draggable?: "true" | "false";
    enctype?: string;
    enterKeyHint?: "enter" | "done" | "go" | "next" | "previous" | "search" | "send";
    form?: string;
    formAction?: string;
    formEnctype?: string;
    formMethod?: string;
    formNoValidate?: boolean;
    formTarget?: string;
    frameBorder?: number | string;
    headers?: string;
    height?: number | string;
    hidden?: boolean;
    high?: number;
    href?: string;
    hreflang?: string;
    for?: string;
    htmlFor?: string;
    httpEquiv?: string;
    id?: string;
    innerText?: string | undefined
    inputMode?: string;
    integrity?: string;
    is?: string;
    kind?: string;
    label?: string;
    lang?: string;
    list?: string;
    loading?: "eager" | "lazy";
    loop?: boolean;
    low?: number;
    marginHeight?: number;
    marginWidth?: number;
    max?: number | string;
    maxLength?: number;
    media?: string;
    method?: string;
    min?: number | string;
    minLength?: number;
    multiple?: boolean;
    muted?: boolean;
    name?: string;
    namespaceURI?: string | undefined;
    nonce?: string;
    noValidate?: boolean;
    open?: boolean;
    optimum?: number;
    pattern?: string;
    ping?: string;
    placeholder?: string;
    playsInline?: boolean;
    poster?: string;
    preload?: string;
    readOnly?: boolean;
    referrerPolicy?:
    | "no-referrer"
    | "no-referrer-when-downgrade"
    | "origin"
    | "origin-when-cross-origin"
    | "same-origin"
    | "strict-origin"
    | "strict-origin-when-cross-origin"
    | "unsafe-url";
    rel?: string;
    required?: boolean;
    role?: string;
    rows?: number;
    rowSpan?: number;
    rowspan?: number;
    sandbox?: string;
    scope?: string;
    scrolling?: string;
    selected?: boolean;
    shape?: string;
    size?: number;
    sizes?: string;
    slot?: string;
    span?: number;
    spellcheck?: boolean;
    spellCheck?: boolean;
    src?: string;
    srcdoc?: string;
    srclang?: string;
    srcset?: string;
    start?: number;
    step?: number | string;
    style?: string | StyleProperties;
    summary?: string;
    tabIndex?: number;
    tabindex?: number;
    target?: string;
    textContent?: string | undefined
    title?: string;
    type?: string;
    useMap?: string;
    value?: string | string[] | number;
    volume?: string | number;
    width?: number | string;
    wrap?: string;
}

