import { alertDialog, iframeDialog } from "./dialogs";
import { stringFormat } from "./formatting";
import { htmlEncode } from "./html";
import { localText } from "./localtext";
import { notifyError } from "./notify";
import { RequestErrorInfo, ServiceError } from "./servicetypes";

export namespace ErrorHandling {

    /**
     * Shows a service error as an alert dialog / notification. If the error
     * is null, has no message or code, it shows a generic error message.
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
     * Runtime error handler that shows a runtime error as a notification
     * by default only in development mode (@see isDevelopmentMode)
     * This function is assigned as window.onerror handler in 
     * ScriptInit.ts for Serenity applications so that developers
     * can notice an error without having to check the browser console.
     */
    export function runtimeErrorHandler(messageOrEvent: string | ErrorEvent, filename?: string,
        lineno?: number, colno?: number, error?: Error) {
        try {
            if (!ErrorHandling.isDevelopmentMode())
                return;

            const isEvent = messageOrEvent != null && "preventDefault" in (messageOrEvent as any);
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
            {filename != null && <p>File: {filename ?? error?.filename ?? "Unknown file"}</p>}
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
     * Determines if the current environment is development mode.
     * The runtimeErrorHandler (window.onerror) shows error notifications only
     * when this function returns true. The default implementation considers 
     * the environment as development mode if the host is localhost, 127.0.0.1, ::1,
     * or a domain name that ends with .local/.localhost.
     * @returns true if the current environment is development mode, false otherwise. 
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
     * Unhandled promise rejection error handler. It's purpose is to
     * ignore logging serviceCall / serviceFetch errors as they have built-in
     * error handling but browser logs it in the console, while Node crashes.
     * Include below code in script-init/errorhandling.ts to enable:
     * window.addEventListener("unhandledrejection", ErrorHandling.unhandledRejectionHandler);
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