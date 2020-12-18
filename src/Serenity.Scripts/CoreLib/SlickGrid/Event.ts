
export type Handler<TArgs> = (e: JQueryEventObject, args: TArgs) => void;

export interface IEventData {
    isPropagationStopped(): boolean;
    isImmediatePropagationStopped(): boolean;
}

export interface Event<TArgs = any> {
    subscribe(handler: Handler<TArgs>): void;
    unsubscribe(handler: Handler<TArgs>): void;
    notify(args?: TArgs, e?: IEventData, scope?: any): void;
    clear(): void;
}

export interface EventHandler<TArgs = any> {
    subscribe<TArgs>(event: Event<TArgs>, handler: Handler<TArgs>): EventHandler<TArgs>;
    unsubscribe<TArgs>(event: Event<TArgs>, handler: Handler<TArgs>): EventHandler<TArgs>;
    unsubscribeAll(): EventHandler<TArgs>;
}   

export interface EventData {
    isPropagationStopped(): boolean;
    isImmediatePropagationStopped(): boolean;
}
