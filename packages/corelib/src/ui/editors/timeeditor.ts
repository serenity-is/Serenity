import { Fluent, toId } from "../../base";
import { IDoubleValue, IReadOnly, IStringValue } from "../../interfaces";
import { addOption } from "../../q";
import { Decorators } from "../../types/decorators";
import { EditorProps, EditorWidget } from "./editorwidget";
import { EditorUtils } from "./editorutils";

export interface TimeEditorOptions {
    noEmptyOption?: boolean;
    startHour?: any;
    endHour?: any;
    intervalMinutes?: any;
}

export class TimeEditorBase<P extends TimeEditorOptions = TimeEditorOptions> extends EditorWidget<P> {

    static override createDefaultElement(): HTMLElement { return document.createElement("select"); }
    declare readonly domNode: HTMLSelectElement;

    protected minutes: Fluent;

    constructor(props: EditorProps<P>) {
        super(props);

        let input = this.element;
        input.addClass('editor s-TimeEditor hour');

        if (!this.options.noEmptyOption) {
            addOption(input, '', '--');
        }

        for (var h = (this.options.startHour || 0); h <= (this.options.endHour || 23); h++) {
            var hour = ((h < 10) ? (`0${h}`) : h.toString());
            addOption(input, hour, hour);
        }

        this.minutes = Fluent("select").class('editor s-TimeEditor minute').insertAfter(input);
        this.minutes.on("change", () => Fluent.trigger(this.domNode, "change"));

        for (var m = 0; m <= 59; m += (this.options.intervalMinutes || 5)) {
            var minute = ((m < 10) ? (`0${m}`) : m.toString());
            addOption(this.minutes, minute, minute);
        }
    }

    get_readOnly(): boolean {
        return this.domNode.classList.contains('readonly');
    }

    set_readOnly(value: boolean): void {

        if (value !== this.get_readOnly()) {
            if (value) {
                this.element.addClass('readonly').attr('readonly', 'readonly');
            }
            else {
                this.element.removeClass('readonly').removeAttr('readonly');
            }
            EditorUtils.setReadonly(this.minutes, value);
        }
    }
}

@Decorators.registerEditor('Serenity.TimeEditor', [IDoubleValue, IReadOnly])
export class TimeEditor<P extends TimeEditorOptions = TimeEditorOptions> extends TimeEditorBase<P> {
    public get value(): number {
        var hour = toId(this.domNode.value);
        var minute = toId(this.minutes.val());
        if (hour == null || minute == null) {
            return null;
        }
        return hour * 60 + minute;
    }

    protected get_value(): number {
        return this.value;
    }

    public set value(value: number) {
        if (!value) {
            if (this.options.noEmptyOption) {
                this.domNode.value = this.options.startHour;
                this.minutes.val('00');
            }
            else {
                this.domNode.value = '';
                this.minutes.val('00');
            }
        }
        else {
            var hour = Math.floor(value / 60);
            if (hour < 10)
                this.domNode.value = `0${hour}`;
            else
                this.domNode.value = hour.toString();

            this.minutes.val("" + (value % 60));
        }
    }

    protected set_value(value: number): void {
        this.value = value;
    }
}

@Decorators.registerEditor('Serenity.TimeSpanEditor', [IStringValue, IReadOnly])
export class TimeSpanEditor<P extends TimeEditorOptions = TimeEditorOptions> extends TimeEditorBase<P> {

    protected get_value(): string {
        return this.value;
    }

    protected set_value(value: string): void {
        this.value = value;
    }
    public get value(): string {
        var hour = toId(this.domNode.value);
        var minute = toId(this.minutes.val());
        if (hour == null || minute == null) {
            return null;
        }
        return `${hour}:${minute}:00.00`;
    }
    public set value(value: string) {
        if (!value) {
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
            var parts = value.split(':');
            this.domNode.value = parts[0];
            this.minutes.val(parts[1]);
        }
    }
}