import { IDialog } from "../interfaces/idialog";

/** Constructor type for dialog widgets registered with {@link DialogTypeRegistry}. */
export type DialogType = ({ new(props?: any): IDialog & { init?: () => void } });

