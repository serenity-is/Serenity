declare module Q {
    function zeroPad(n: number, digits: number): string;
    function formatDayHourAndMin(n: number): string;
    function formatISODateTimeUTC(d: Date): string;
    function formatNumber(n: number, fmt: string, dec?: string, grp?: string): string;
    function parseDate(s: string, dateOrder?: string): any;
    function parseDecimal(s: string): number;
    function splitDateString(s: string): string[];
    function parseISODateTime(s: string): Date;
    function parseHourAndMin(value: string): any;
    function parseDayHourAndMin(s: string): number;
    function parseQueryString(s: string): {};
    function turkishLocaleCompare(a: string, b: string): number;
    function turkishLocaleToUpper(a: string): string;
    interface CommonDialogOptions extends JQueryUI.DialogOptions {
        onOpen?: () => void;
        onClose?: () => void;
        htmlEncode?: boolean;
    }
    interface AlertOptions extends CommonDialogOptions {
        okButton?: string;
    }
    interface ConfirmOptions extends CommonDialogOptions {
        yesButton?: string;
        noButton?: string;
        cancelButton?: string;
        onCancel?: () => void;
        onNo?: () => void;
    }
    function alert(message: string, options?: AlertOptions): void;
    function confirm(message: string, onYes: () => void, options?: ConfirmOptions): void;
    function warning(message: string, options?: AlertOptions): void;
    function information(message: string, onOk: () => void, options?: ConfirmOptions): void;
    interface IFrameDialogOptions {
        html?: string;
    }
    function iframeDialog(options: IFrameDialogOptions): void;
    function toId(id: any): any;
    function validatorAbortHandler(validator: any): void;
    function validateOptions(options: any): any;
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
    function postToService(options: PostToServiceOptions): void;
    function postToUrl(options: PostToUrlOptions): void;
    function showInFrame(code: any): void;
    function deepClone(arg1: any, arg2: any): any;
}
