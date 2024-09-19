import { Culture } from "../../base";
import { AutoNumeric } from "./autonumeric";
import { type DecimalEditorOptions } from "./decimaleditor";

beforeEach(() => {
    jest.clearAllMocks();
});

const newEditor = async (opt: DecimalEditorOptions) => new (await import("./decimaleditor")).DecimalEditor(opt);

describe("DecimalEditor", () => {
    it("adds default values to autonumeric", async () => {
        Culture.decimalSeparator = ".";

        jest.spyOn(AutoNumeric, "init").mockImplementation((_, options) => {
            expect(options.vMin).toBe('0.00');
            expect(options.vMax).toBe('999999999999.99');
            expect(options.aPad).toBe(true);
            expect(options.aDec).toBe('.');
            expect(options.altDec).toBe(',');
            expect(options.aSep).toBe(',');
            return null;
        });

        await newEditor({});
    });
});