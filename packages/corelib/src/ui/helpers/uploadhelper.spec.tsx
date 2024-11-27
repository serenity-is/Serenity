import { UploadHelper } from "./uploadhelper";

describe("UploadHelper.addUploadInput", () => {
    it("should set inputs multiple attribute to true if options.allowMultiple is true", () => {
        const container = document.body.appendChild(<div/>) as HTMLElement;
        const input = UploadHelper.addUploadInput({
            container,
            inputName: "file",
            allowMultiple: true
        });
        expect((input.getNode() as HTMLInputElement).multiple).toBe(true);
    });
});