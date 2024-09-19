import { Fluent, ListRequest, getCookie, isSameOrigin, resolveServiceUrl, resolveUrl } from "../base";

export function setEquality(request: ListRequest, field: string, value: any) {
    if (request.EqualityFilter == null) {
        request.EqualityFilter = {};
    }
    request.EqualityFilter[field] = value;
}

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

export function parseQueryString(s?: string): Record<string, string> {
    let qs: string;
    if (s === undefined)
        qs = location.search.substring(1, location.search.length);
    else
        qs = s || '';
    let result: Record<string, string> = {};
    let parts = qs.split('&');
    for (let i = 0; i < parts.length; i++) {
        let pair = parts[i].split('=');
        let name = decodeURIComponent(pair[0]);
        result[name] = (pair.length >= 2 ? decodeURIComponent(pair[1]) : name);
    }
    return result;
}

function postToCommon(url: string, form: HTMLFormElement, target: string) {

    if (isSameOrigin(resolveUrl(url))) {
        var csrfToken = getCookie('CSRF-TOKEN');
        if (csrfToken) {
            form.appendChild(<input type="hidden" name="__RequestVerificationToken" value={csrfToken} />);
        }
    }

    form.method = "POST";
    form.action = url;
    form.appendChild(<input type="submit" />);
    document.body.appendChild(form);
    target && (form.target = target);
    form.submit();
    window.setTimeout(function () {
        form.remove();
    }, 0);
}

export function postToService(options: PostToServiceOptions) {
    postToCommon(options.url ? (resolveUrl(options.url)) : resolveServiceUrl(options.service), <form>
        <input type="hidden" name="request" value={JSON.stringify(options.request)} />
    </form> as HTMLFormElement, options.target);
}

export function postToUrl(options: PostToUrlOptions) {
    postToCommon(resolveUrl(options.url), <form>
        {options.params && Object.keys(options.params).map(k =>
            <input type="hidden" name={k} value={options.params[k]} />
        )}
    </form> as HTMLFormElement, options.target);
}
