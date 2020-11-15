
declare namespace Slick
{
    type Handler<TArgs> = (e: JQueryEventObject, args: TArgs) => void;
    
    interface IEventData {
        isPropagationStopped(): boolean;
        isImmediatePropagationStopped(): boolean;
    }

    class EventData {
        constructor();
        isPropagationStopped(): boolean;
        isImmediatePropagationStopped(): boolean;
    }

    class Event<TArgs = any> {
        subscribe(handler: Handler<TArgs>): void;
        unsubscribe(handler: Handler<TArgs>): void;
        notify(args?: TArgs, e?: IEventData, scope?: any): void;
        clear(): void;
    }

    class EventHandler<TArgs = any> {
        subscribe<TArgs>(event: Event<TArgs>, handler: Handler<TArgs>): EventHandler<TArgs>;
        unsubscribe<TArgs>(event: Event<TArgs>, handler: Handler<TArgs>): EventHandler<TArgs>;
        unsubscribeAll(): EventHandler<TArgs>;
    }
}