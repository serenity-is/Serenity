import { Widget, WidgetProps } from "../ui/widgets/widget";

/** Constructor type for editor widgets registered with {@link EditorTypeRegistry}. */
export type EditorType = { new(props?: WidgetProps<any>): Widget<any> }
