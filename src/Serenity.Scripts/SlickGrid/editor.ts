import type { Column } from "./column";
import type { IEventData } from "./event";
import type { Grid } from "./grid";
import type { Position } from "./types";

export interface EditorOptions {
    grid: Grid;
    gridPosition?: Position;
    position?: Position;
    column?: Column;
    container?: HTMLElement;
    item?: any;
    event: IEventData;
    commitChanges?: () => void,
    cancelChanges?: () => void
}

export interface Editor {
    new(options: EditorOptions): Editor;
    destroy(): void;
    applyValue(item: any, value: any): void;
    focus(): void;
    isValueChanged(): boolean;
    keyCaptureList?: number[];
    loadValue(value: any): void;
    serializeValue(): any;
    position?(pos: Position): void;
    preClick?(): void;
    hide?(): void;
    show?(): void;
    suppressClearOnEdit?: boolean;
    validate?(): ValidationResult;
}

export interface EditorFactory {
    getEditor(column: Column): Editor;
}

export interface EditCommand {
    row: number;
    cell: number;
    editor: Editor;
    serializedValue: any;
    prevSerializedValue: any;
    execute: () => void;
    undo: () => void;
}

export interface ValidationResult {
    valid: boolean;
    msg?: string;
}