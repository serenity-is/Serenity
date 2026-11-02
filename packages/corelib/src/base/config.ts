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
const Config = {
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
    applicationPath: '/',

    /**
     * Content Security Policy nonce to apply to dynamically created `<script>` / `<style>` tags.
     *
     * @remarks
     * Initialized from `<meta name="csp-nonce">` or the `nonce` attribute of existing
     * `<script>`/`<style>` elements via {@link resetCspNonce}; `null` when no nonce is present
     * or on the server. Helpers that inject markup should copy this value to the `nonce` attribute.
     */
    cspNonce: null as string,

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
    defaultReturnUrl: (purpose?: string) => Config.applicationPath,

    /**
     * Whether e-mail validation should allow only ASCII characters.
     *
     * @remarks
     * `true` (default) rejects non-ASCII characters in the local/domain parts;
     * set to `false` to allow Unicode/IDN addresses.
     */
    emailAllowOnlyAscii: true,

    /**
     * Callback to lazily resolve a type that is not yet in the type registry.
     *
     * @remarks
     * Useful with code-splitting / lazy chunk loading. Called with the requested type key
     * and a `kind` hint (`"dialog"`, `"editor"`, `"enum"`, `"formatter"`, `"filtering"`, …).
     * May return the type synchronously or a `Promise` resolving to it; returning `null`/`undefined`
     * signals "not found". By default, it loads the types implementing `LazyLoadInclude` that are listed
     * in the JSON block rendered by `Html.LazyTypes()`, so wrap the existing value when overriding it.
     * @param typeKey - Full type name being requested, e.g. `"MyApp.MyEditor"`.
     * @param kind - Category of the type, used to narrow search/loading.
     * @returns The resolved type, a promise for it, or `null` if unavailable.
     * @example
     * Config.lazyTypeLoader = async (typeKey) => await import(`./editors/${typeKey}`);
     */
    lazyTypeLoader: <(typeKey: string, kind: "dialog" | "editor" | "enum" | "formatter" | "filtering" | string) => any | Promise<any>>defaultLazyTypeLoader,

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
    rootNamespaces: ['Serenity'],

    /**
     * Optional handler invoked when a service call returns `NotAuthorized` / session expired.
     *
     * @remarks
     * If set, Serenity delegates the "not logged in" flow to this callback so you can
     * prompt the user, redirect to login, or refresh a token. When `null` (default),
     * the framework falls back to its built-in handling.
     * @example
     * Config.notLoggedInHandler = () => { getWindowLocation().href = "/Account/Login"; };
     */
    notLoggedInHandler: <Function>null
}

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
export function resetApplicationPath() {
    Config.applicationPath = '/';
    if (typeof document !== 'undefined') {
        const pathLink = document.querySelector('link#ApplicationPath') as HTMLLinkElement;
        if (pathLink != null) {
            Config.applicationPath = pathLink.getAttribute('href') ?? '/';
        }
    }
}

/**
 * Re-reads {@link Config.cspNonce} from the DOM.
 *
 * @remarks
 * Probes in order: `<meta name="csp-nonce">` → `<script nonce>` → `<style nonce>`.
 * Sets `null` if none found or when `document` is unavailable. Called once on module load.
 */
export function resetCspNonce() {
    Config.cspNonce = null;
    if (typeof document !== 'undefined') {
        Config.cspNonce = document.querySelector<HTMLMetaElement>('meta[name="csp-nonce"]')?.content ??
            document.querySelector<HTMLScriptElement>('script[nonce]')?.nonce ??
            document.querySelector<HTMLStyleElement>('style[nonce]')?.nonce;
    }
}

let lazyTypeModules: Map<string, string>;

/**
 * Makes the types that can be lazily loaded read again from the DOM when a type is next requested.
 *
 * @remarks
 * These are in the `<script id="LazyTypes">` JSON block rendered by `Html.LazyTypes()`, which maps
 * the urls of the modules that load the types implementing `LazyLoadInclude` to the keys of the types.
 */
export function resetLazyTypes() {
    lazyTypeModules = null;
}

function getLazyTypeModules() {
    if (lazyTypeModules)
        return lazyTypeModules;

    lazyTypeModules = new Map();
    if (typeof document !== 'undefined') {
        const json = document.querySelector('script#LazyTypes')?.textContent?.trim();
        if (json) {
            for (const [url, keys] of Object.entries<string[]>(JSON.parse(json))) {
                for (const key of keys)
                    lazyTypeModules.set(key, url);
            }
        }
    }

    return lazyTypeModules;
}

// finds the module synchronously, as the registries only try the other forms of a key when this returns nothing
function defaultLazyTypeLoader(typeKey: string): Promise<any> {
    const url = getLazyTypeModules().get(typeKey);
    if (!url)
        return null;

    return import(/* @vite-ignore */ url)
        .then(module => module.loaderByKey[typeKey]?.());
}

resetApplicationPath();
resetCspNonce();

export { Config };

