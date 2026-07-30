import { BooleanEditor, DateEditor, EmailAddressEditor, initFormType, LookupEditor, PrefixedContext, StringEditor } from "@serenity-is/corelib";
import { NotesEditor } from "../../Note/NotesEditor";

export interface CustomerForm {
    CustomerID: StringEditor;
    CompanyName: StringEditor;
    ContactName: StringEditor;
    ContactTitle: StringEditor;
    Representatives: LookupEditor;
    Address: StringEditor;
    Country: LookupEditor;
    City: LookupEditor;
    Region: StringEditor;
    PostalCode: StringEditor;
    Phone: StringEditor;
    Fax: StringEditor;
    NoteList: NotesEditor;
    LastContactDate: DateEditor;
    LastContactedBy: LookupEditor;
    Email: EmailAddressEditor;
    SendBulletin: BooleanEditor;
}

export class CustomerForm extends PrefixedContext {
    static readonly formKey = 'Northwind.Customer';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!CustomerForm.init) {
            CustomerForm.init = true;

            initFormType(CustomerForm, [
                'CustomerID', StringEditor,
                'CompanyName', StringEditor,
                'ContactName', StringEditor,
                'ContactTitle', StringEditor,
                'Representatives', LookupEditor,
                'Address', StringEditor,
                'Country', LookupEditor,
                'City', LookupEditor,
                'Region', StringEditor,
                'PostalCode', StringEditor,
                'Phone', StringEditor,
                'Fax', StringEditor,
                'NoteList', NotesEditor,
                'LastContactDate', DateEditor,
                'LastContactedBy', LookupEditor,
                'Email', EmailAddressEditor,
                'SendBulletin', BooleanEditor
            ]);
        }
    }
}