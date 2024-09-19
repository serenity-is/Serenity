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
                    showMessage(localText("Services.UnknownConnectionEror", "An error occured while connecting to the server.") + seeBrowserConsole);
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
    export function runtimeErrorHandler(message: string, filename?: string,
        lineno?: number, colno?: number, error?: Error) {
        try {
            if (!ErrorHandling.isDevelopmentMode())
                return;
            var errorInfo = error?.stack ?? error?.toString();

            message =
                '<p></p><p>Message: ' + htmlEncode(message) +
                '</p><p>File: ' + htmlEncode(filename) +
                ', Line: ' + lineno + ', Column: ' + colno +
                (errorInfo ? ('</p><p>' + htmlEncode(errorInfo)) : "") + '</p>';

            window.setTimeout(function () {
                try {
                    notifyError(message, "SCRIPT ERROR! See browser console (F12) for details.", {
                        escapeHtml: false,
                        timeOut: 15000
                    });
                }
                catch {
                }
            }, 0);
        }
        catch {
        }
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
        var hostname = (window.location.hostname ?? "").toLowerCase();
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
            if (!err || !err.reason)
                return;

            const reason = err.reason;
            if (reason.origin == "serviceCall") {
                err.preventDefault();

                if (!reason.silent &&
                    (reason.kind ?? "exception") === "exception") {
                    console.error(err);
                }
            }
        }
        catch {
        }
    }
}