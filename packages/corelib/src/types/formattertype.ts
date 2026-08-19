import { Formatter } from "../slick/slicktypes";

/** Constructor type for slick formatters registered with {@link FormatterTypeRegistry}. */
export type FormatterType = ({ new(props?: any): Formatter });
