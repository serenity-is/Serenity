import type { Handler, IEventData, } from "./Event";
import type { Column } from "./Column";
import type { GridOptions } from "./GridOptions";

declare global {
    namespace Slick {
        export interface AutoTooltipsOptions {
            enableForHeaderCells?: boolean;
            enableForCells?: boolean;
            maxToolTipLength?: number;
        }

        export class AutoTooltips {
            constructor(options: AutoTooltipsOptions);
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

        export interface PagerOptions {
            view?: any;
            showRowsPerPage?: boolean;
            rowsPerPage?: number;
            rowsPerPageOptions?: number[],
            onChangePage?: (newPage: number) => void;
            onRowsPerPageChange?: (n: number) => void;
        }
    
        export interface RowMoveManagerOptions {
            cancelEditOnDrag: boolean;
        }

        export class RowMoveManager {
            constructor(options: RowMoveManagerOptions);
            onBeforeMoveRows: Event;
            onMoveRows: Event;
        }

        export class RowSelectionModel {
        }

        namespace Data {
            export class GroupItemMetadataProvider implements GroupItemMetadataProvider {
                constructor();
                getGroupRowMetadata(item: any): any;
                getTotalsRowMetadata(item: any): any;
            }            
        }
    }
}