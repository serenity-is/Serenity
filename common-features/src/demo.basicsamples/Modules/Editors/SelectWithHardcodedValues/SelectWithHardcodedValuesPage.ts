import { HardcodedValuesForm } from "../../ServerTypes/Demo";
import { ComboboxEditor, Decorators, EditorProps, PropertyDialog, WidgetProps, notifySuccess } from "@serenity-is/corelib";

export default function pageInit() {
    var dlg = new HardcodedValuesDialog({});
    dlg.dialogOpen();
    dlg.element.findFirst('.field.SomeValue .editor').tryGetWidget(ComboboxEditor)?.openDropdown();

    // let's also create it in our page, for demonstration purposes this time we directly create
    new HardcodedValuesEditor({
        element: el => document.querySelector("#UsingWidgetCreate label").insertAdjacentElement("afterend", el)
    });

    // here we directly create it on a element found via selector
    new HardcodedValuesEditor({ element: "#CreatingOnInput input" });
}

/**
 * Our select editor with hardcoded values.
 * 
 * When you define a new editor type, make sure you build
 * and transform templates for it to be available 
 * in server side forms, e.g. [HardCodedValuesEditor]
 */
@Decorators.registerEditor('Serenity.Demo.BasicSamples.HardcodedValuesEditor')
export class HardcodedValuesEditor<P = {}> extends ComboboxEditor<P, any> {

    constructor(props: EditorProps<P>) {
        super(props);

        // add option accepts a key (id) value and display text value
        this.addOption("key1", "Text 1");
        this.addOption("key2", "Text 2");

        // you may also use addItem which accepts a Select2Item parameter
        this.addItem({
            id: "key3",
            text: "Text 3"
        });

        // don't let selecting this one (disabled)
        this.addItem({
            id: "key4",
            text: "Text 4",
            disabled: true
        });
    }
}

@Decorators.registerClass('Serenity.Demo.BasicSamples.HardcodedValuesDialog')
export class HardcodedValuesDialog<P = {}> extends PropertyDialog<any, P> {
    protected getFormKey() { return HardcodedValuesForm.formKey; }

    protected form = new HardcodedValuesForm(this.idPrefix);

    constructor(props: WidgetProps<P>) {
        super(props);

        this.dialogTitle = "Please select some value";

        this.form.SomeValue.changeSelect2(() => {
            notifySuccess("You selected item with key: " + this.form.SomeValue.value);
        });
    }

    protected getDialogOptions() {
        var opt = super.getDialogOptions();
        opt.modal = false;
        return opt;
    }
}