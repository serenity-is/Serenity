import { describe, expect, it } from "vitest";
import { EnumTypeRegistry } from "../types/enumtyperegistry";
import { ColumnSelection, ListRequestCapabilities, RetrieveColumnSelection } from "./servicetypes";
import { Enum, peekTypeInfo } from "./system";

describe("ColumnSelection and RetrieveColumnSelection", () => {
    it("are found by the full names of the server-side enums, e.g. for the options of a service lookup editor", () => {
        expect(EnumTypeRegistry.get("Serenity.Services.ColumnSelection")).toBe(ColumnSelection);
        expect(EnumTypeRegistry.get("Serenity.Services.RetrieveColumnSelection")).toBe(RetrieveColumnSelection);
    });
});

describe("ListRequestCapabilities", () => {
    it("is found by the full name of the server-side enum, as a set of flags", () => {
        expect(EnumTypeRegistry.get("Serenity.Services.ListRequestCapabilities")).toBe(ListRequestCapabilities);
        expect(peekTypeInfo(ListRequestCapabilities)?.enumFlags).toBe(true);
        expect(Enum.toString(ListRequestCapabilities, ListRequestCapabilities.ContainsText | ListRequestCapabilities.Sort)).toBe("ContainsText | Sort");
    });
});
