import { Widget, WidgetProps } from "../ui/widgets/widget";

export type EditorType = { new(props?: WidgetProps<any>): Widget<any> }
