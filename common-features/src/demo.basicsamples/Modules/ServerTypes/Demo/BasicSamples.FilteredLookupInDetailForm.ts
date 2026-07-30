import { DateEditor, initFormType, LookupEditor, PrefixedContext } from "@serenity-is/corelib";
import { CustomerEditor } from "@serenity-is/demo.northwind";
import { FilteredLookupDetailEditor } from "../../Editors/FilteredLookupInDetail/FilteredLookupInDetailPage";

export interface FilteredLookupInDetailForm {
    CustomerID: CustomerEditor;
    OrderDate: DateEditor;
    CategoryID: LookupEditor;
    DetailList: FilteredLookupDetailEditor;
}

export class FilteredLookupInDetailForm extends PrefixedContext {
    static readonly formKey = 'BasicSamples.FilteredLookupInDetail';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!FilteredLookupInDetailForm.init) {
            FilteredLookupInDetailForm.init = true;

            initFormType(FilteredLookupInDetailForm, [
                'CustomerID', CustomerEditor,
                'OrderDate', DateEditor,
                'CategoryID', LookupEditor,
                'DetailList', FilteredLookupDetailEditor
            ]);
        }
    }
}