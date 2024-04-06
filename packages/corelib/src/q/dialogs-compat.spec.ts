import { confirmDialog, informationDialog, successDialog, warningDialog } from "../base";

describe("informationDialog", () => {
    it('is aliased by obsolete inform', async function () {
        const dialogs = (await import("./dialogs-compat"));
        expect(dialogs.information).toBe(informationDialog);
    });        
});

describe("warningDialog", () => {
    it('is aliased by obsolete warning', async function () {
        const dialogs = (await import("./dialogs-compat"));
        expect(dialogs.warning).toBe(warningDialog);
    });
});

describe("confirmDialog", () => {
    it('is aliased by obsolete confirm', async function () {
        const dialogs = (await import("./dialogs-compat"));
        expect(dialogs.confirm).toBe(confirmDialog);
    });
});

describe("successDialog", () => {
    it('is aliased by obsolete success', async function () {
        const dialogs = (await import("./dialogs-compat"));
        expect(dialogs.success).toBe(successDialog);
    });    
});

export { }