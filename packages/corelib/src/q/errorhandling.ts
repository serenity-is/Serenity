import { alertDialog } from "./dialogs";
import { htmlEncode } from "./html";
import { notifyError } from "./notify";
import { ServiceError } from "./servicetypes";

export namespace ErrorHandling {

    export function showServiceError(error: ServiceError) {
        let msg: any;
        if (error == null) {
            msg = '??ERROR??';
        }
        else {
            msg = error.Message;
            if (msg == null) {
                msg = error.Code;
            }
        }

        alertDialog(msg);
    }

    export function runtimeErrorHandler(message: string, filename?: string,
        lineno?: number, colno?: number, error?: Error) {
        try {
            var host = (window.location.host || "").toLowerCase();
            if (host.indexOf("localhost") < 0 &&
                host.indexOf("127.0.0.1") < 0)
                return;

            var errorInfo = JSON.stringify(error || {});

            message =
                '<p></p><p>Message: ' + htmlEncode(message) +
                '</p><p>File: ' + htmlEncode(filename) +
                ', Line: ' + lineno + ', Column: ' + colno +
                (errorInfo != "{}" ? '</p><p>Error: ' : "") + '</p>';

            window.setTimeout(function () {
                try {
                    notifyError(message, "SCRIPT ERROR! See browser console (F12) for details.", {
                        escapeHtml: false,
                        timeOut: 15000
                    });
                }
                catch {
                }
            });
        }
        catch {
        }
    }
}