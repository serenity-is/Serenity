
export type Handler<TArgs> = (e: JQueryEventObject, args: TArgs) => void;

export interface IEventData {
    isPropagationStopped(): boolean;
    isImmediatePropagationStopped(): boolean;
}