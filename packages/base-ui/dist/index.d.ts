declare let datePickerIconSvg: string;
declare function dateInputChangeHandler(e: Event): void;
declare function dateInputKeyupHandler(e: KeyboardEvent): void;
declare function flatPickrOptions(onChange: () => void): {
    clickOpens: boolean;
    allowInput: boolean;
    dateFormat: string;
    onChange: () => void;
};
declare function flatPickrTrigger(input: HTMLInputElement): HTMLElement;
declare function jQueryDatepickerZIndexWorkaround(input: HTMLInputElement, jQuery: any): void;
declare function jQueryDatepickerInitialization(jQuery: any): boolean;

export { dateInputChangeHandler, dateInputKeyupHandler, datePickerIconSvg, flatPickrOptions, flatPickrTrigger, jQueryDatepickerInitialization, jQueryDatepickerZIndexWorkaround };
