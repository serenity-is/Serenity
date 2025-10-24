import { EventData, type EventEmitter, type IEventData } from "../core/event";
import type { ArgsGrid } from "../core/eventargs";
import type { IGrid } from "../core/igrid";

export function triggerGridEvent<TArgs extends ArgsGrid, TEventData extends IEventData = IEventData>(this: IGrid,
    evt: EventEmitter<TArgs, TEventData>, args?: TArgs, e?: TEventData) {
    e = e || new EventData() as any;
    args = args || {} as any;
    args.grid = this;
    return evt.notify(args, e, this);
}
