import { Decorators, gridPageInit } from "@serenity-is/corelib";
import { CustomerDialog, CustomerGrid, CustomerService } from "@serenity-is/demo.northwind";
import "@serenity-is/demo.northwind/dist/index.css";

export default () => gridPageInit(SerialAutoNumberGrid);

/**
 * Subclass of CustomerGrid to override dialog type to SerialAutoNumberDialog
 */
@Decorators.registerClass('Serenity.Demo.BasicSamples.SerialAutoNumberGrid')
export class SerialAutoNumberGrid extends CustomerGrid {
    protected getDialogType() { return SerialAutoNumberDialog; }
}

@Decorators.registerClass('Serenity.Demo.BasicSamples.SerialAutoNumberDialog')
export class SerialAutoNumberDialog extends CustomerDialog {

    constructor(props: {}) {
        super(props);

        this.form.CustomerID.element.on('keyup', (e: KeyboardEvent) => {
            // only auto number when a key between 'A' and 'Z' is pressed
            if ((e.key >= "A" && e.key <= "Z") ||
                (e.key >= "a" && e.key <= "z"))
                this.getNextNumber();
        });
    }

    protected afterLoadEntity() {
        super.afterLoadEntity();

        // fill next number in new record mode
        if (this.isNew())
            this.getNextNumber();
    }

    private getNextNumber() {

        var val = this.form.CustomerID.value?.trim();

        // we will only get next number when customer ID is empty or 1 character in length
        if (!val || val.length <= 1) {

            // if no customer ID yet (new record mode probably) use 'C' as a prefix
            var prefix = (val || 'C').toUpperCase();

            // call our service, see CustomerEndpoint.cs and CustomerRepository.cs
            CustomerService.GetNextNumber({
                Prefix: prefix,
                Length: 5 // we want service to search for and return serials of 5 in length
            }, response => {
                this.form.CustomerID.value = response.Serial;

                // this is to mark numerical part after prefix
                (this.form.CustomerID.element[0] as any).setSelectionRange(prefix.length, response.Serial.length);
            });
        }
    }

}