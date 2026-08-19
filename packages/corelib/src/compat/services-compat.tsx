import { ListRequest, getCookie, isSameOrigin, resolveServiceUrl, resolveUrl } from "../base";

/**
 * Sets an equality filter value on a list request.
 * Compat shim for the legacy `Q.setEquality` helper. Lazily initializes `request.EqualityFilter` if needed.
 * @param request - The {@link ListRequest} whose `EqualityFilter` map will be mutated.
 * @param field - Field name to set in the equality filter.
 * @param value - Value to assign for the field (any JSON-serializable value, or `null` to clear).
 * @deprecated Prefer setting `request.equalityFilter` / `EqualityFilter` directly or using modern list request builders. Kept for legacy compatibility.
 */
export function setEquality(request: ListRequest, field: string, value: any) {
    if (request.EqualityFilter == null) {
        request.EqualityFilter = {};
    }
    request.EqualityFilter[field] = value;
}

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

function postToCommon(url: string, form: HTMLFormElement, target: string) {

    if (isSameOrigin(resolveUrl(url))) {
        const csrfToken = getCookie('CSRF-TOKEN');
        if (csrfToken) {
            form.appendChild(<input type="hidden" name="__RequestVerificationToken" value={csrfToken} />);
        }
    }

    form.method = "POST";
    form.action = url;
    form.appendChild(<input type="submit" />);
    target && (form.target = target);
    document.body.appendChild(form);
    form.submit();
    window.setTimeout(function () {
        form.remove();
    }, 0);
}

/**
 * Posts a service request by creating and submitting a hidden form.
 * Compat shim for `Q.postToService`. Resolves the URL from `options.url` or `options.service`, injects a CSRF token when same-origin, and auto-removes the form after submission.
 * @param options - Post options including service/url, request payload, and optional target.
 * @deprecated Prefer `serviceCall` / `fetch` with JSON. Kept for legacy file-download and export scenarios that require form POST.
 */
export function postToService(options: PostToServiceOptions) {
    postToCommon(options.url ? (resolveUrl(options.url)) : resolveServiceUrl(options.service), <form>
        <input type="hidden" name="request" value={JSON.stringify(options.request)} />
    </form> as HTMLFormElement, options.target);
}

/**
 * Posts arbitrary parameters to a URL by creating and submitting a hidden form.
 * Compat shim for `Q.postToUrl`. Each key in `options.params` becomes a hidden input field.
 * @param options - Post options including target URL, params map, and optional target window/frame.
 * @deprecated Prefer `fetch` or programmatic form construction. Kept for legacy export / report flows.
 */
export function postToUrl(options: PostToUrlOptions) {
    postToCommon(resolveUrl(options.url), <form>
        {options.params && Object.keys(options.params).map(k =>
            <input type="hidden" name={k} value={options.params[k]} />
        )}
    </form> as HTMLFormElement, options.target);
}
