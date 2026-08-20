import { Fluent, nsSerenity, setElementReadOnly, toId } from "../../base";
import { addOption, zeroPad } from "../../compat";
import { IDoubleValue, IReadOnly, IStringValue } from "../../interfaces";
import { EditorProps, EditorWidget } from "./editorwidget";

/**
 * Options for the {@link TimeEditorBase}.
 */
export interface TimeEditorBaseOptions {
    /** Whether to omit the empty option. */
    noEmptyOption?: boolean;
    /** Starting hour for the hour select. */
    startHour?: any;
    /** Ending hour for the hour select. */
    endHour?: any;
    /** Interval in minutes between minute options. */
    intervalMinutes?: any;
}

/**
 * Base editor for time values, providing hour and minute selects.
 * @typeParam P - Widget props type.
 */
export class TimeEditorBase<P extends TimeEditorBaseOptions> extends EditorWidget<P> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity);

    /** Creates the default select element for the time editor.
     * @returns The hour select element. */
    static override createDefaultElement(): HTMLElement { return document.createElement("select"); }
    /** The hour select element that backs the editor. */
    declare readonly domNode: HTMLSelectElement;

    declare protected minutes: Fluent;

    /**
     * Creates a time editor base.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);

        let input = this.element;
        input.addClass('editor hour');

        if (!this.options.noEmptyOption) {
            addOption(input, '', '--');
        }

        for (let h = (this.options.startHour || 0); h <= (this.options.endHour || 23); h++) {
            addOption(input, "" + h, zeroPad(h, 2));
        }

        const select = document.createElement("select");
        select.classList.add("editor", "minute");
        this.minutes = Fluent(select).insertAfter(input);
        this.minutes.on("change", () => Fluent.trigger(this.domNode, "change"));

        for (var m = 0; m <= 59; m += (this.options.intervalMinutes || 5)) {
            addOption(this.minutes, "" + m, zeroPad(m, 2));
        }
    }

    /**
     * Returns the selected hour.
     * @returns The hour value.
     */
    get hour(): number {
        return toId(this.domNode.value);
    }

    /**
     * Returns the selected minute.
     * @returns The minute value.
     */
    get minute(): number {
        return toId(this.minutes.val());
    }

    /**
     * Returns whether the editor is read-only.
     * @returns True when read-only.
     */
    get_readOnly(): boolean {
        return this.domNode.classList.contains('readonly');
    }

    /**
     * Sets whether the editor is read-only.
     * @param value - True to enable read-only mode.
     */
    set_readOnly(value: boolean): void {
        if (value !== this.get_readOnly()) {
            setElementReadOnly([this.domNode, this.minutes.getNode()], value);
        }
    }

    /** Returns the combined time value in HH:mm format.
     * @returns The time string, or null when empty. */
    public get hourAndMin(): string {
        var hour = this.hour;
        var minute = this.minute;
        if (hour == null || minute == null) {
            return null;
        }
        return `${zeroPad(hour, 2)}:${zeroPad(minute, 2)}`;
    }

    /** Sets the combined time value in HH:mm format.
     * @param value - The time string to set. */
    public set hourAndMin(value: string) {
        if (value == null || value === "") {
            if (this.options.noEmptyOption) {
                this.domNode.value = this.options.startHour;
                this.minutes.val('0');
            }
            else {
                this.domNode.value = '';
                this.minutes.val('0');
            }
            return;
        }

        var parts = value.split(':');
        this.domNode.value = "" + parseInt(parts[0], 10);
        this.minutes.val("" + parseInt(parts[1], 10));
    }
}


/**
 * Options for the {@link TimeEditor}.
 */
export interface TimeEditorOptions extends TimeEditorBaseOptions {
    /** Default is 1. Set to 60 to store seconds, 60000 to store ms in an integer field */
    multiplier?: number;
}

/**
 * Options for the {@link TimeEditor}.
 */
export interface TimeEditorOptions extends TimeEditorBaseOptions {
    /** Default is 1. Set to 60 to store seconds, 60000 to store ms in an integer field */
    multiplier?: number;
}

/** Note that this editor's value is number of minutes, e.g. for
 * 16:30, value will be 990. If you want to use a TimeSpan field
 * use TimeSpanEditor instead.
 */
/**
 * An editor for time values stored as a number of minutes.
 * @typeParam P - Widget props type.
 */
export class TimeEditor<P extends TimeEditorOptions = TimeEditorOptions> extends TimeEditorBase<P> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IDoubleValue, IReadOnly]);

    /**
     * Creates a time editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);
        this.domNode.classList.add("s-TimeEditor");
        this.minutes.addClass("s-TimeEditor");
    }

    /**
     * Returns the current time value in minutes.
     * @returns The value, or null when empty.
     */
    public get value(): number {
        var hour = this.hour;
        var minute = this.minute;
        if (hour == null || minute == null) {
            return null;
        }
        return (hour * 60 + minute) * (this.options.multiplier || 1);
    }

    /**
     * Returns the current time value in minutes.
     * @returns The value, or null when empty.
     */
    protected get_value(): number {
        return this.value;
    }

    /**
     * Sets the time value in minutes.
     * @param value - The value to set.
     */
    public set value(value: number) {
        if (value == null || (value as any) === "" || isNaN(value)) {
            if (this.options.noEmptyOption) {
                this.domNode.value = this.options.startHour;
                this.minutes.val('0');
            }
            else {
                this.domNode.value = '';
                this.minutes.val('0');
            }
        }
        else {
            value /= (this.options.multiplier || 1);
            var hour = Math.floor(value / 60);
            this.domNode.value = "" + hour;
            this.minutes.val("" + (value % 60));
        }
    }

    /** Sets the time value in minutes. */
    protected set_value(value: number): void {
        this.value = value;
    }
}

/**
 * Options for the {@link TimeSpanEditor}.
 */
export interface TimeSpanEditorOptions extends TimeEditorBaseOptions {
}

/**
 * This editor is for TimeSpan fields. It uses a string value in the format "HH:mm".
 */
export class TimeSpanEditor<P extends TimeSpanEditorOptions = TimeSpanEditorOptions> extends TimeEditorBase<P> {

    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IStringValue, IReadOnly]);

    /**
     * Creates a time span editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);
        this.domNode.classList.add("s-TimeSpanEditor");
        this.minutes.addClass("s-TimeSpanEditor");
    }

    /**
     * Returns the current time span value.
     * @returns The value in "HH:mm" format.
     */
    protected get_value(): string {
        return this.value;
    }

    /** Sets the time span value.
     * @param value - The time span value to set. */
    protected set_value(value: string): void {
        this.value = value;
    }

    /**
     * Returns the current time span value.
     * @returns The value in "HH:mm" format.
     */
    public get value(): string {
        return this.hourAndMin;
    }

    /** Sets the time span value.
     * @param value - The time span string to set. */
    public set value(value: string) {
        this.hourAndMin = value;
    }
}