
declare namespace Slick {
    class RowMoveManager {
        constructor(options: Slick.RowMoveManagerOptions);
        onBeforeMoveRows: Slick.Event;
        onMoveRows: Slick.Event;
    }

    interface RowMoveManagerOptions {
        cancelEditOnDrag: boolean;
    }
}