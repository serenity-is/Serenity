import { CustomerRow } from "../ServerTypes/Demo";
import { Decorators, EditorProps, LookupEditorBase, LookupEditorOptions } from "@serenity-is/corelib";

@Decorators.registerEditor('Serenity.Demo.Northwind.CustomerEditor')
export class CustomerEditor<P extends LookupEditorOptions = LookupEditorOptions> extends LookupEditorBase<P, CustomerRow> {

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
