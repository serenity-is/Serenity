import { EntityDialog } from "@serenity-is/corelib";
import { waitForAjaxRequests } from "./waitutils";

export class EntityDialogWrapper<TDialog extends EntityDialog<any, any>> {
    constructor(public readonly actual: TDialog) {
    }

    clickDeleteButton(): Promise<void> {
        var button = this.actual.element.findFirst(".delete-button");
        if (!button.length)
            throw "Delete button not found in the dialog!";
        if (button.hasClass("disabled"))
            throw "Delete button is disabled!";
        const spy = jest.spyOn(window, "confirm").mockReturnValue(true);
        button.click();
        spy.mockRestore();
        return waitForAjaxRequests();
    }

    clickSaveButton(): Promise<void> {
        var button = this.actual.element.findFirst(".save-and-close-button");
        if (!button.length)
            throw "Save button not found in the dialog!";
        if (button.hasClass("disabled"))
            throw "Save button is disabled!";
        button.click();
        return waitForAjaxRequests();
    }

    getTextInput(name: string) {
        var input = this.actual["byId"](name);
        if (!input.length)
            throw `getTextInput: Input with name ${name} is not found in the dialog!`;
        return input.val();
    }

    setTextInput(name: string, value: any) {
        var input = this.actual["byId"](name);
        if (!input.length)
            throw `setTextInput: Input with name ${name} is not found in the dialog!`;
        input.val(value).trigger("change");
    }

    waitForAjaxRequests(timeout: number = 10000): Promise<void> {
        return waitForAjaxRequests(timeout);
    }

    getForm<TForm>(type: { new(prefix: string): TForm }): TForm {
        return new type(this.actual.idPrefix);
    }
}