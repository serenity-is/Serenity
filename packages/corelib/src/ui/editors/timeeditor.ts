import { Decorators } from "../../decorators";
import { IDoubleValue, IReadOnly } from "../../interfaces";
import { addOption, toId } from "@serenity-is/corelib/q";
import { Widget } from "../widgets/widget";
import { EditorUtils } from "./editorutils";

export interface TimeEditorOptions {
    noEmptyOption?: boolean;
    startHour?: any;
    endHour?: any;
    intervalMinutes?: any;
}

@Decorators.registerEditor('Serenity.TimeEditor', [IDoubleValue, IReadOnly])
@Decorators.element("<select />")
export class TimeEditor extends Widget<TimeEditorOptions> {

    private minutes: JQuery;

    constructor(input: JQuery, opt?: TimeEditorOptions) {
        super(input, opt);

        input.addClass('editor s-TimeEditor hour');

        if (!this.options.noEmptyOption) {
            addOption(input, '', '--');
        }

        for (var h = (this.options.startHour || 0); h <= (this.options.endHour || 23); h++) {
            addOption(input, h.toString(), ((h < 10) ? ('0' + h) : h.toString()));
        }

        this.minutes = $('<select/>').addClass('editor s-TimeEditor minute').insertAfter(input);
        this.minutes.change(() => this.element.trigger("change"));

        for (var m = 0; m <= 59; m += (this.options.intervalMinutes || 5)) {
            addOption(this.minutes, m.toString(), ((m < 10) ? ('0' + m) : m.toString()));
        }
    }

    public get value(): number {
        var hour = toId(this.element.val());
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
                this.element.val(this.options.startHour);
                this.minutes.val('0');
            }
            else {
                this.element.val('');
                this.minutes.val('0');
            }
        }
        else {
            this.element.val(Math.floor(value / 60).toString());
            this.minutes.val(value % 60);
        }
    }

    protected set_value(value: number): void {
        this.value = value;
    }

    get_readOnly(): boolean {
        return this.element.hasClass('readonly');
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