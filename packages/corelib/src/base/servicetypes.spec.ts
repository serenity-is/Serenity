import { describe, expect, it } from "vitest";
import { EnumTypeRegistry } from "../types/enumtyperegistry";
import { ColumnSelection, RetrieveColumnSelection } from "./servicetypes";

describe("ColumnSelection and RetrieveColumnSelection", () => {
    it("are found by the full names of the server-side enums, e.g. for the options of a service lookup editor", () => {
        expect(EnumTypeRegistry.get("Serenity.Services.ColumnSelection")).toBe(ColumnSelection);
        expect(EnumTypeRegistry.get("Serenity.Services.RetrieveColumnSelection")).toBe(RetrieveColumnSelection);
    });
});
