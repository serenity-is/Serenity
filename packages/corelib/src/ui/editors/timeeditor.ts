import { Fluent, setElementReadOnly, toId } from "../../base";
import { IDoubleValue, IReadOnly, IStringValue } from "../../interfaces";
import { addOption, zeroPad } from "../../compat";
import { Decorators } from "../../types/decorators";
import { EditorProps, EditorWidget } from "./editorwidget";

export interface TimeEditorBaseOptions {
    noEmptyOption?: boolean;
    startHour?: any;
    endHour?: any;
    intervalMinutes?: any;
}

export class TimeEditorBase<P extends TimeEditorBaseOptions> extends EditorWidget<P> {

    static override createDefaultElement(): HTMLElement { return document.createElement("select"); }
    declare readonly domNode: HTMLSelectElement;

    declare protected minutes: Fluent;

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

    get hour(): number {
        return toId(this.domNode.value);
    }

    get minute(): number {
        return toId(this.minutes.val());
    }

    get_readOnly(): boolean {
        return this.domNode.classList.contains('readonly');
    }

    set_readOnly(value: boolean): void {
        if (value !== this.get_readOnly()) {
            setElementReadOnly([this.domNode, this.minutes.getNode()], value);
        }
    }

    /** Returns value in HH:mm format */
    public get hourAndMin(): string {
        var hour = this.hour;
        var minute = this.minute;
        if (hour == null || minute == null) {
            return null;
        }
        return `${zeroPad(hour, 2)}:${zeroPad(minute, 2)}`;
    }

    /** Sets value in HH:mm format */
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


export interface TimeEditorOptions extends TimeEditorBaseOptions {
    /** Default is 1. Set to 60 to store seconds, 60000 to store ms in an integer field */
    multiplier?: number;
}

/** Note that this editor's value is number of minutes, e.g. for
 * 16:30, value will be 990. If you want to use a TimeSpan field
 * use TimeSpanEditor instead.
 */
@Decorators.registerEditor('Serenity.TimeEditor', [IDoubleValue, IReadOnly])
export class TimeEditor<P extends TimeEditorOptions = TimeEditorOptions> extends TimeEditorBase<P> {
    
    constructor(props: EditorProps<P>) {
        super(props);
        this.domNode.classList.add("s-TimeEditor");
        this.minutes.addClass("s-TimeEditor");
    }

    public get value(): number {
        var hour = this.hour;
        var minute = this.minute;
        if (hour == null || minute == null) {
            return null;
        }
        return (hour * 60 + minute) * (this.options.multiplier || 1);;
    }

    protected get_value(): number {
        return this.value;
    }

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

    protected set_value(value: number): void {
        this.value = value;
    }
}

export interface TimeSpanEditorOptions extends TimeEditorBaseOptions {
}

/**
 * This editor is for TimeSpan fields. It uses a string value in the format "HH:mm".
 */
@Decorators.registerEditor('Serenity.TimeSpanEditor', [IStringValue, IReadOnly])
export class TimeSpanEditor<P extends TimeSpanEditorOptions = TimeSpanEditorOptions> extends TimeEditorBase<P> {

    constructor(props: EditorProps<P>) {
        super(props);
        this.domNode.classList.add("s-TimeSpanEditor");
        this.minutes.addClass("s-TimeSpanEditor");
    }

    protected get_value(): string {
        return this.value;
    }

    protected set_value(value: string): void {
        this.value = value;
    }

    public get value(): string {
        return this.hourAndMin;
    }

    public set value(value: string) {
        this.hourAndMin = value;
    }
}