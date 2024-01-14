declare function dateInputChangeHandler(e: Event): void;
declare function dateInputKeyupHandler(e: KeyboardEvent): void;
declare function flatPickrOptions(onChange: () => void): {
    clickOpens: boolean;
    allowInput: boolean;
    dateFormat: string;
    onChange: () => void;
};
declare function flatPickrTrigger(input: HTMLInputElement): HTMLElement;
declare function jQueryDatepickerZIndexWorkaround(input: HTMLInputElement): void;
declare function jQueryDatepickerInitialization(): boolean;

export { dateInputChangeHandler, dateInputKeyupHandler, flatPickrOptions, flatPickrTrigger, jQueryDatepickerInitialization, jQueryDatepickerZIndexWorkaround };
