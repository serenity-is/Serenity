import { Uploader } from "./uploader";

describe("Uploader", () => {
    it("reads multiple from the input if not passed via options", () => {
        const input = <input multiple={true} /> as HTMLInputElement;
        const uploader = new Uploader({ 
            input
        });
        expect(uploader["isMultiple"]?.()).toBe(true);
    });
});