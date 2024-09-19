import { Decorators } from "../../types/decorators";
import { Widget, WidgetProps } from "../widgets/widget";

export type EditorProps<T> = WidgetProps<T> & {
    initialValue?: any;
    maxLength?: number;
    name?: string;
    placeholder?: string;
    required?: boolean;
    readOnly?: boolean;
}

@Decorators.registerType()
export class EditorWidget<P> extends Widget<EditorProps<P>> {
    static override typeInfo = Decorators.classType("Serenity.EditorWidget");

    constructor(props: EditorProps<P>) {
        super(props);
    }
}
