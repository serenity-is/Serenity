import { Handler, IEventData, } from "./EventHandler";
import { Column } from "./Column";
import { GridOptions } from "./GridOptions";

declare global {
    export namespace Slick {
        export class AutoTooltips {
            constructor(options: AutoTooltipsOptions);
        }
        
        export interface AutoTooltipsOptions {
            enableForHeaderCells?: boolean;
            enableForCells?: boolean;
            maxToolTipLength?: number;
        }

        export class Event<TArgs = any> {
            subscribe(handler: Handler<TArgs>): void;
            unsubscribe(handler: Handler<TArgs>): void;
            notify(args?: TArgs, e?: IEventData, scope?: any): void;
            clear(): void;
        }

        export class EventData {
            constructor();
            isPropagationStopped(): boolean;
            isImmediatePropagationStopped(): boolean;
        }        
       
        export class EventHandler<TArgs = any> {
            subscribe<TArgs>(event: Event<TArgs>, handler: Handler<TArgs>): EventHandler<TArgs>;
            unsubscribe<TArgs>(event: Event<TArgs>, handler: Handler<TArgs>): EventHandler<TArgs>;
            unsubscribeAll(): EventHandler<TArgs>;
        }

        export class Grid {
            constructor(container: JQuery, data: any, columns: Column[], options: GridOptions);
        }

        export class RowSelectionModel {
        }

        export class RowMoveManager {
            constructor(options: RowMoveManagerOptions);
            onBeforeMoveRows: Slick.Event;
            onMoveRows: Slick.Event;
        }
        
        export interface RowMoveManagerOptions {
            cancelEditOnDrag: boolean;
        }

        export namespace Data {
            export interface GroupItemMetadataProvider {
                getGroupRowMetadata(item: any): any;
                getTotalsRowMetadata(item: any): any;
            }
            
            export class GroupItemMetadataProvider implements GroupItemMetadataProvider {
                constructor();
                getGroupRowMetadata(item: any): any;
                getTotalsRowMetadata(item: any): any;
            }
        }
    }
}