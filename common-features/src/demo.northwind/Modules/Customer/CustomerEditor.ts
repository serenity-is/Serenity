import { EditorProps, LookupEditorBase, LookupEditorOptions } from "@serenity-is/corelib";
import { CustomerRow } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";

export class CustomerEditor<P extends LookupEditorOptions = LookupEditorOptions> extends LookupEditorBase<P, CustomerRow> {
    static override[Symbol.typeInfo] = this.registerEditor(nsDemoNorthwind);

    constructor(props: EditorProps<P>) {
        super({ async: true, ...props });
    }

    protected override getLookupKey() {
        return 'Northwind.Customer';
    }

    protected override getItemText(item, lookup) {
        return super.getItemText(item, lookup) + ' [' + item.CustomerID + ']';
    }
}
