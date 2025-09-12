import { EditorProps, LookupEditorBase, LookupEditorOptions } from "@serenity-is/corelib";
import { CustomerRow } from "../ServerTypes/Demo";

export class CustomerEditor<P extends LookupEditorOptions = LookupEditorOptions> extends LookupEditorBase<P, CustomerRow> {
    static override typeInfo = this.registerEditor("Serenity.Demo.Northwind.CustomerEditor");

    constructor(props: EditorProps<P>) {
        super({ async: true, ...props });
    }

    protected getLookupKey() {
        return 'Northwind.Customer';
    }

    protected getItemText(item, lookup) {
        return super.getItemText(item, lookup) + ' [' + item.CustomerID + ']';
    }
}
