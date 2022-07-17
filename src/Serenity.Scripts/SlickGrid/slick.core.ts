/***
 * Contains core SlickGrid classes.
 * @module Core
 * @namespace Slick
 */
namespace Slick {
    /***
     * An event object for passing data to event handlers and letting them control propagation.
     * <p>This is pretty much identical to how W3C and jQuery implement events.</p>
     * @class EventData
     * @constructor
     */
    export class EventData {
        private _isPropagationStopped = false;
        private _isImmediatePropagationStopped = false;

        /***
         * Stops event from propagating up the DOM tree.
         * @method stopPropagation
         */
        stopPropagation() {
            this._isPropagationStopped = true;
        }

        /***
         * Returns whether stopPropagation was called on this event object.
         * @method isPropagationStopped
         * @return {Boolean}
         */
        isPropagationStopped(): boolean {
            return this._isPropagationStopped;
        }

        /***
         * Prevents the rest of the handlers from being executed.
         * @method stopImmediatePropagation
         */
        stopImmediatePropagation() {
            this._isImmediatePropagationStopped = true;
        }

        /***
         * Returns whether stopImmediatePropagation was called on this event object.\
         * @method isImmediatePropagationStopped
         * @return {Boolean}
         */
        isImmediatePropagationStopped(): boolean {
            return this._isImmediatePropagationStopped;
        }
    }

    export type Handler<TArgs> = (e: JQueryEventObject, args: TArgs) => void;

    /***
     * A simple publisher-subscriber implementation.
     * @class Event
     * @constructor
     */
    export class Event<TArgs = any> {

        private _handlers: Function[] = [];

        /***
         * Adds an event handler to be called when the event is fired.
         * <p>Event handler will receive two arguments - an <code>EventData</code> and the <code>data</code>
         * object the event was fired with.<p>
         * @method subscribe
         * @param fn {Function} Event handler.
         */
        subscribe(fn: Handler<TArgs>) {
            this._handlers.push(fn);
        };

        /***
         * Removes an event handler added with <code>subscribe(fn)</code>.
         * @method unsubscribe
         * @param fn {Function} Event handler to be removed.
         */
        unsubscribe(fn: Handler<TArgs>) {
            for (var i = this._handlers.length - 1; i >= 0; i--) {
                if (this._handlers[i] === fn) {
                    this._handlers.splice(i, 1);
                }
            }
        }

        /***
         * Fires an event notifying all subscribers.
         * @method notify
         * @param args {Object} Additional data object to be passed to all handlers.
         * @param e {EventData}
         *      Optional.
         *      An <code>EventData</code> object to be passed to all handlers.
         *      For DOM events, an existing W3C/jQuery event object can be passed in.
         * @param scope {Object}
         *      Optional.
         *      The scope ("this") within which the handler will be executed.
         *      If not specified, the scope will be set to the <code>Event</code> instance.
         */
        notify(args: any, e: EventData, scope: object) {
            e = e || new EventData();
            scope = scope || this;

            var returnValue;
            for (var i = 0; i < this._handlers.length && !(e.isPropagationStopped() || e.isImmediatePropagationStopped()); i++) {
                returnValue = this._handlers[i].call(scope, e, args);
            }

            return returnValue;
        }

        clear(fn: Handler<TArgs>) {
            this._handlers = [];
        }
    }

    interface EventHandlerEntry<TArgs = any> {
        event: Event<TArgs>;
        handler: Handler<TArgs>;
    }

    export class EventHandler<TArgs = any>  {
        private _handlers: EventHandlerEntry<TArgs>[] = [];

        subscribe(event: Event<TArgs>, handler: Handler<TArgs>): EventHandler<TArgs> {
            this._handlers.push({
                event: event,
                handler: handler
            });
            event.subscribe(handler);

            return this;  // allow chaining
        };

        unsubscribe(event: Event<TArgs>, handler: Handler<TArgs>): EventHandler<TArgs> {
            var i = this._handlers.length;
            while (i--) {
                if (this._handlers[i].event === event &&
                    this._handlers[i].handler === handler) {
                    this._handlers.splice(i, 1);
                    event.unsubscribe(handler);
                    return;
                }
            }

            return this;  // allow chaining
        };

        unsubscribeAll(): EventHandler<TArgs> {
            var i = this._handlers.length;
            while (i--) {
                this._handlers[i].event.unsubscribe(this._handlers[i].handler);
            }
            this._handlers = [];

            return this;  // allow chaining
        }
    }

    /***
       * A structure containing a range of cells.
       * @class Range
       * @constructor
       * @param fromRow {Integer} Starting row.
       * @param fromCell {Integer} Starting cell.
       * @param toRow {Integer} Optional. Ending row. Defaults to <code>fromRow</code>.
       * @param toCell {Integer} Optional. Ending cell. Defaults to <code>fromCell</code>.
       */
    export class Range {

        public fromRow: number;
        public fromCell: number;
        public toRow: number;
        public toCell: number;

        constructor(fromRow: number, fromCell: number, toRow: number, toCell: number) {
            if (toRow === undefined && toCell === undefined) {
                toRow = fromRow;
                toCell = fromCell;
            }

            this.fromRow = Math.min(fromRow, toRow);
            this.fromCell = Math.min(fromCell, toCell);
            this.toRow = Math.max(fromRow, toRow);
            this.toCell = Math.max(fromCell, toCell);
        }

        /***
         * Returns whether a range represents a single row.
         * @method isSingleRow
         * @return {Boolean}
         */
        isSingleRow(): boolean {
            return this.fromRow == this.toRow;
        };

        /***
         * Returns whether a range represents a single cell.
         * @method isSingleCell
         * @return {Boolean}
         */
        isSingleCell(): boolean {
            return this.fromRow == this.toRow && this.fromCell == this.toCell;
        };

        /***
         * Returns whether a range contains a given cell.
         * @method contains
         * @param row {Integer}
         * @param cell {Integer}
         * @return {Boolean}
         */
        contains(row: number, cell: number): boolean {
            return row >= this.fromRow && row <= this.toRow &&
                cell >= this.fromCell && cell <= this.toCell;
        };

        /***
         * Returns a readable representation of a range.
         * @method toString
         * @return {String}
         */
        toString(): string {
            if (this.isSingleCell()) {
                return "(" + this.fromRow + ":" + this.fromCell + ")";
            }
            else {
                return "(" + this.fromRow + ":" + this.fromCell + " - " + this.toRow + ":" + this.toCell + ")";
            }
        }
    }

    /***
     * A base class that all special / non-data rows (like Group and GroupTotals) derive from.
     * @class NonDataRow
     * @constructor
     */
    export class NonDataRow {
        __nonDataRow: boolean = true;
    }

    /***
     * Information about a group of rows.
     * @class Group
     * @extends Sleek.NonDataRow
     * @constructor
     */
    export class Group<TEntity = any> extends NonDataRow {
        readonly __group = true;

        /**
         * Grouping level, starting with 0.
         * @property level
         * @type {Number}
         */
        level: number = 0;

        /***
         * Number of rows in the group.
         * @property count
         * @type {Integer}
         */
        count: number = 0;

        /***
         * Grouping value.
         * @property value
         * @type {Object}
         */
        value: any;

        /***
         * Formatted display value of the group.
         * @property title
         * @type {String}
         */
        title: string;

        /***
         * Whether a group is collapsed.
         * @property collapsed
         * @type {Boolean}
         */
        collapsed: boolean = false;

        /***
         * GroupTotals, if any.
         * @property totals
         * @type {GroupTotals}
         */
        totals: GroupTotals<TEntity>;

        /**
         * Rows that are part of the group.
         * @property rows
         * @type {Array}
         */
        rows: TEntity[] = [];

        /**
         * Sub-groups that are part of the group.
         * @property groups
         * @type {Array}
         */
        groups: Group<TEntity>[];

        /**
         * A unique key used to identify the group.  This key can be used in calls to DataView
         * collapseGroup() or expandGroup().
         * @property groupingKey
         * @type {Object}
         */
        groupingKey: string;
    }

    /***
     * Information about group totals.
     * An instance of GroupTotals will be created for each totals row and passed to the aggregators
     * so that they can store arbitrary data in it.  That data can later be accessed by group totals
     * formatters during the display.
     * @class GroupTotals
     * @extends Sleek.NonDataRow
     * @constructor
     */
    export class GroupTotals<TEntity = any> extends NonDataRow {

        readonly __groupTotals = true;

        /***
         * Parent Group.
         * @param group
         * @type {Group}
         */
        group: Group<TEntity>;

        /***
         * Whether the totals have been fully initialized / calculated.
         * Will be set to false for lazy-calculated group totals.
         * @param initialized
         * @type {Boolean}
         */
        initialized: boolean = false;

        /** 
         * Contains sum
         */
        sum?: number;

        /** 
         * Contains avg
         */
        avg?: number;

        /** 
         * Contains min
         */
        min?: any;

        /** 
         * Contains max
         */
        max?: any;        
    }

    export interface EditController {
        commitCurrentEdit(): boolean;
        cancelCurrentEdit(): boolean;
    }

    /***
     * A locking helper to track the active edit controller and ensure that only a single controller
     * can be active at a time.  This prevents a whole class of state and validation synchronization
     * issues.  An edit controller (such as SleekGrid) can query if an active edit is in progress
     * and attempt a commit or cancel before proceeding.
     * @class EditorLock
     * @constructor
     */
    export class EditorLock {
        private activeEditController: EditController;

        /***
         * Returns true if a specified edit controller is active (has the edit lock).
         * If the parameter is not specified, returns true if any edit controller is active.
         * @method isActive
         * @param editController {EditController}
         * @return {Boolean}
         */
        isActive(editController?: EditController): boolean {
            return (editController ? this.activeEditController === editController : this.activeEditController !== null);
        };

        /***
         * Sets the specified edit controller as the active edit controller (acquire edit lock).
         * If another edit controller is already active, and exception will be thrown.
         * @method activate
         * @param editController {EditController} edit controller acquiring the lock
         */
        activate(editController: EditController) {
            if (editController === this.activeEditController) { // already activated?
                return;
            }
            if (this.activeEditController !== null) {
                throw "SleekGrid.EditorLock.activate: an editController is still active, can't activate another editController";
            }
            if (!editController.commitCurrentEdit) {
                throw "SleekGrid.EditorLock.activate: editController must implement .commitCurrentEdit()";
            }
            if (!editController.cancelCurrentEdit) {
                throw "SleekGrid.EditorLock.activate: editController must implement .cancelCurrentEdit()";
            }
            this.activeEditController = editController;
        };

        /***
         * Unsets the specified edit controller as the active edit controller (release edit lock).
         * If the specified edit controller is not the active one, an exception will be thrown.
         * @method deactivate
         * @param editController {EditController} edit controller releasing the lock
         */
        deactivate(editController: EditController) {
            if (this.activeEditController !== editController) {
                throw "SleekGrid.EditorLock.deactivate: specified editController is not the currently active one";
            }
            this.activeEditController = null;
        };

        /***
         * Attempts to commit the current edit by calling "commitCurrentEdit" method on the active edit
         * controller and returns whether the commit attempt was successful (commit may fail due to validation
         * errors, etc.).  Edit controller's "commitCurrentEdit" must return true if the commit has succeeded
         * and false otherwise.  If no edit controller is active, returns true.
         * @method commitCurrentEdit
         * @return {Boolean}
         */
        commitCurrentEdit(): boolean {
            return (this.activeEditController ? this.activeEditController.commitCurrentEdit() : true);
        };

        /***
         * Attempts to cancel the current edit by calling "cancelCurrentEdit" method on the active edit
         * controller and returns whether the edit was successfully cancelled.  If no edit controller is
         * active, returns true.
         * @method cancelCurrentEdit
         * @return {Boolean}
         */
        cancelCurrentEdit(): boolean {
            return (this.activeEditController ? this.activeEditController.cancelCurrentEdit() : true);
        };
    }

    /***
     * A global singleton editor lock.
     * @class GlobalEditorLock
     * @static
     * @constructor
     */
    export const GlobalEditorLock = new EditorLock();

    export const keyCode = {
        BACKSPACE: 8,
        DELETE: 46,
        DOWN: 40,
        END: 35,
        ENTER: 13,
        ESCAPE: 27,
        HOME: 36,
        INSERT: 45,
        LEFT: 37,
        PAGEDOWN: 34,
        PAGEUP: 33,
        RIGHT: 39,
        TAB: 9,
        UP: 38
    }
}