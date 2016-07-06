declare namespace Serenity {
    namespace SubDialogHelper {
        function bindToDataChange(dialog: any, owner: Serenity.Widget<any>, dataChange: (p1: any, p2: DataChangeInfo) => void, useTimeout?: boolean): any;
        function triggerDataChange(dialog: any): any;
        function triggerDataChange(element: JQuery): JQuery;
        function bubbleDataChange(dialog: any, owner: Serenity.Widget<any>, useTimeout?: boolean): any;
        function cascade(cascadedDialog: any, ofElement: JQuery): any;
        function cascadedDialogOffset(element: JQuery): any;
    }
}