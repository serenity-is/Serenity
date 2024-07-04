import { IDialog } from "../interfaces/idialog";

export type DialogType = ({ new(props?: any): IDialog & { init?: () => void } });

