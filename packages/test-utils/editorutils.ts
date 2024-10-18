import { Fluent } from "@serenity-is/corelib";

export function typeText(editor: { value: string, element: Fluent }, value: string) {
    editor.element.trigger("focus");
    editor.value = value;
    editor.element.trigger("blur");
    editor.element.trigger("change");
}

export function typeNumber(editor: { value: number, element: Fluent }, value: number) {
    editor.element.trigger("focus");
    editor.value = value;
    editor.element.trigger("blur");
    editor.element.trigger("change");
}