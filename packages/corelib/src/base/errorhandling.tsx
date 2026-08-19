import { alertDialog, iframeDialog } from "./dialogs";
import { stringFormat } from "./formatting";
import { htmlEncode } from "./html";
import { localText } from "./localtext";
import { notifyError } from "./notify";
import { RequestErrorInfo, ServiceError } from "./servicetypes";

/**
 * Centralized error handling helpers for service and runtime errors.
 * @remarks
 * `showServiceError` is the default handler for failed service calls;
 * `runtimeErrorHandler` / `unhandledRejectionHandler` surface uncaught
 * script errors during development only (see {@link ErrorHandling.isDevelopmentMode}).
 */
export namespace ErrorHandling {

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
    export function showServiceError(error: ServiceError, errorInfo?: RequestErrorInfo, errorMode?: 'alert' | 'notification') {

        const showMessage = errorMode == 'notification' ? notifyError : alertDialog;

        if (error || !errorInfo) {
            showMessage(error?.Message ?? error?.Code ?? localText("Services.GenericErrorMessage", "An error occurred while processing your request."));
            return;
        }

        const seeBrowserConsole = !ErrorHandling.isDevelopmentMode() ? (" " + localText("Services.SeeBrowserConsole", "See browser console (F12) for more information.")) : "";

        if (!errorInfo.responseText) {
            if (!errorInfo.status) {
                if (errorInfo.statusText != "abort")
                    showMessage(localText("Services.UnknownConnectionError", "An error occured while connecting to the server.") + seeBrowserConsole);
            }
            else if (errorInfo.status == 500)
                showMessage(localText("Services.InternalServerError", "Internal Server Error (500).") + seeBrowserConsole);
            else
                showMessage(stringFormat(localText("Services.HttpError", "HTTP Error {0}."), errorInfo.status) + seeBrowserConsole);
        }
        else if (errorMode == 'notification')
            notifyError(errorInfo.responseText);
        else
            iframeDialog({ html: errorInfo.responseText });
    }

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
    export function runtimeErrorHandler(messageOrEvent: string | ErrorEvent, filename?: string,
        lineno?: number, colno?: number, error?: Error) {
        try {
            if (!ErrorHandling.isDevelopmentMode())
                return;

            const isEvent = typeof messageOrEvent === "object" && "preventDefault" in (messageOrEvent as any);
            error = isEvent ? (messageOrEvent as ErrorEvent).error : error;

            reportScriptError({
                error,
                filename: isEvent ? (messageOrEvent as ErrorEvent).filename : filename,
                lineno: isEvent ? (messageOrEvent as ErrorEvent).lineno : lineno,
                colno: isEvent ? (messageOrEvent as ErrorEvent).colno : colno,
                message: isEvent ? (messageOrEvent as ErrorEvent).message : messageOrEvent as string
            });
        }
        catch {
        }
    }

    function reportScriptError({ error, filename, lineno, colno, message, unhandledRejection }: {
        error: any,
        message?: string,
        filename?: string,
        lineno?: number,
        colno?: number,
        unhandledRejection?: boolean
    }) {
        const errorInfo = error?.stack ?? error?.toString();

        const msg = <div>
            <p>Message: {message ?? error?.message ?? error?.toString() ?? "Unknown error"}</p>
            {filename != null && <p>File: {filename}</p>}
            {(lineno ?? colno) != null && <p>Line: {lineno}, Column: {colno}</p>}
            {errorInfo && <p style={{ maxHeight: "180px", overflowY: "auto", whiteSpace: "pre-wrap" }}>{errorInfo}</p>}
        </div>

        window.setTimeout(function () {
            try {
                notifyError(msg, (unhandledRejection ? "UNCAUGHT ERROR! (in promise)!" : "SCRIPT ERROR!") + " See browser console (F12) for details.", {
                    timeOut: 15000
                });
            }
            catch {
            }
        }, 0);
    }

    /**
     * Determines whether the current host should be treated as a development environment.
     * @returns `true` when `window.location.hostname` is `localhost`, `127.0.0.1`, `[::1]`, or ends with `.local` / `.localhost`; `false` otherwise.
     * @remarks
     * Both {@link ErrorHandling.runtimeErrorHandler} and {@link ErrorHandling.unhandledRejectionHandler} gate their notifications on this check.
     * Override by replacing `ErrorHandling.isDevelopmentMode` at startup if a different heuristic is needed.
     */
    export function isDevelopmentMode() {
        const hostname = (window.location.hostname ?? "").toLowerCase();
        return (hostname === "localhost" ||
            hostname === "127.0.0.1" ||
            hostname === "[::1]" ||
            hostname.endsWith(".local") ||
            hostname.endsWith(".localhost"));
    }

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
    export function unhandledRejectionHandler(err: PromiseRejectionEvent) {
        try {
            if (!err || !err.reason) {
                return;
            }

            const reason = err.reason;
            if (reason?.origin == "serviceCall") {
                err.preventDefault();

                if (!reason.silent &&
                    (reason.kind ?? "exception") === "exception") {
                    console.error(err);
                }

                return;
            }

            if (isDevelopmentMode()) {
                reportScriptError({
                    error: reason instanceof Error ? reason : null,
                    message: reason instanceof Error ? reason.message : (typeof reason === "string" ? reason : null),
                    unhandledRejection: true
                });
            }
        }
        catch {
        }
    }
}