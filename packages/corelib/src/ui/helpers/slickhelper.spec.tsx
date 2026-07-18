import { Column } from "@serenity-is/sleekgrid";
import { addLocalText } from "../../base";
import { SlickHelper } from "./slickhelper";

describe("SlickHelper.setDefaults", () => {
    it("sets sortable to true by default", () => {
        const columns: Column[] = [
            { field: "Name" }
        ];
        SlickHelper.setDefaults(columns);
        expect(columns[0].sortable).toBe(true);
    });

    it("preserves sortable when explicitly set", () => {
        const columns: Column[] = [
            { field: "Name", sortable: false }
        ];
        SlickHelper.setDefaults(columns);
        expect(columns[0].sortable).toBe(false);
    });

    it("uses field as id when id is null", () => {
        const columns: Column[] = [
            { field: "City" }
        ];
        SlickHelper.setDefaults(columns);
        expect(columns[0].id).toBe("City");
    });

    it("preserves existing id", () => {
        const columns: Column[] = [
            { id: "customId", field: "Name" }
        ];
        SlickHelper.setDefaults(columns);
        expect(columns[0].id).toBe("customId");
    });

    it("applies localTextPrefix for column names", () => {
        addLocalText({ "Db.Customer.Name": "Customer Name" });
        const columns: Column[] = [
            { field: "Name", id: "Name" }
        ];
        SlickHelper.setDefaults(columns, "Db.Customer.");
        expect(columns[0].name).toBe("Customer Name");
    });

    it("resolves ~ prefixed names with localTextPrefix using id as fallback", () => {
        addLocalText({ "Db.Customer.Email": "Email Address" });
        const columns: Column[] = [
            { field: "Email", id: "Email", name: "~Email" }
        ];
        SlickHelper.setDefaults(columns, "Db.Customer.");
        expect(columns[0].name).toBe("Email Address");
    });

    it("does not override explicit name", () => {
        const columns: Column[] = [
            { field: "Name", id: "Name", name: "Explicit Name" }
        ];
        SlickHelper.setDefaults(columns, "Db.Customer.");
        expect(columns[0].name).toBe("Explicit Name");
    });

    it("returns the columns array", () => {
        const columns: Column[] = [
            { field: "Name" }
        ];
        const result = SlickHelper.setDefaults(columns);
        expect(result).toBe(columns);
    });

    it("handles null localTextPrefix", () => {
        const columns: Column[] = [
            { field: "Name" }
        ];
        expect(() => SlickHelper.setDefaults(columns, null)).not.toThrow();
    });
});
