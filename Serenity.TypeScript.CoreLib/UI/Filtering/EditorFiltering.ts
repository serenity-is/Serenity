declare namespace Serenity {

    class EditorFiltering extends BaseEditorFiltering<Serenity.Widget<any>> {
        get_editorType(): string;
        set_editorType(value: string): void;
        get_useRelative(): boolean;
        set_useRelative(value: boolean): void;
        get_useLike(): boolean;
        set_useLike(value: boolean): void;
    }

}