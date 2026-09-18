import { formatterContext } from "@serenity-is/sleekgrid";
import { EmployeeFormatter } from "../../Modules/Employee/EmployeeFormatter";
import { Gender } from "../../Modules/ServerTypes/Demo";

function render(result: any): string {
    const div = document.createElement("div");
    if (result != null)
        div.append(result);
    return div.textContent;
}

describe("EmployeeFormatter", () => {
    it("returns the value when no gender property is configured", () => {
        const formatter = new EmployeeFormatter();
        expect(render(formatter.format(formatterContext({ value: "John" })))).toBe("John");
    });

    it("returns the value when it is empty", () => {
        const formatter = new EmployeeFormatter({ genderProperty: "Gender" });
        expect(render(formatter.format(formatterContext({ value: "  ", item: { Gender: Gender.Female } })))).toBe("  ");
    });

    it("renders a female icon", () => {
        const formatter = new EmployeeFormatter({ genderProperty: "Gender" });
        const result = formatter.format(formatterContext({ value: "Jane", item: { Gender: Gender.Female } }));
        const div = document.createElement("div");
        div.append(result as any);
        expect(div.textContent).toContain("Jane");
        expect(div.querySelector("i").className).toContain("fa-female");
    });

    it("renders a male icon", () => {
        const formatter = new EmployeeFormatter({ genderProperty: "Gender" });
        const result = formatter.format(formatterContext({ value: "John", item: { Gender: Gender.Male } }));
        const div = document.createElement("div");
        div.append(result as any);
        expect(div.textContent).toContain("John");
        expect(div.querySelector("i").className).toContain("fa-male");
    });

    it("initializes columns with referenced fields", () => {
        const formatter = new EmployeeFormatter({ genderProperty: "Gender" });
        const column: any = {};
        formatter.initializeColumn(column);
        expect(column.referencedFields).toEqual(["Gender"]);
    });

    it("initializes columns without a gender property", () => {
        const formatter = new EmployeeFormatter();
        const column: any = { referencedFields: ["Existing"] };
        formatter.initializeColumn(column);
        expect(column.referencedFields).toEqual(["Existing"]);
    });
});
