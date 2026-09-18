import { formatterContext } from "@serenity-is/sleekgrid";
import { CountryWithFlagFormatter, getCountryFlagEmoji, iso2CodesByCountryName, validIso2CountryCodes } from "../../Modules/Shared/CountryWithFlagFormatter";

describe("CountryWithFlagFormatter", () => {
    const formatter = new CountryWithFlagFormatter();

    it("returns empty string for empty values", () => {
        expect(formatter.format(formatterContext({ value: null }))).toBe("");
        expect(formatter.format(formatterContext({ value: "" }))).toBe("");
    });

    it("renders a flag for a country name", () => {
        const result = formatter.format(formatterContext({ value: "United States" })) as any;
        expect(result).toBeTruthy();
        const div = document.createElement("div");
        div.append(result);
        expect(div.innerHTML).toContain("country-with-flag");
    });

    it("renders a flag for an iso code", () => {
        const result = formatter.format(formatterContext({ value: "US" })) as any;
        expect(result).toBeTruthy();
    });

    it("escapes unknown countries", () => {
        expect(formatter.format(formatterContext({ value: "Nowhereland" }))).toBe("Nowhereland");
    });

    it("escapes non string values", () => {
        expect(formatter.format(formatterContext({ value: 42 }))).toBe("42");
    });
});

describe("getCountryFlagEmoji", () => {
    it("returns null for short or unknown values", () => {
        expect(getCountryFlagEmoji("")).toBeNull();
        expect(getCountryFlagEmoji("X")).toBeNull();
        expect(getCountryFlagEmoji("ZZ")).toBeNull();
    });

    it("maps country names to flags", () => {
        expect(getCountryFlagEmoji("United States")).toBeTruthy();
        expect(getCountryFlagEmoji("czech republic")).toBeTruthy();
    });

    it("maps iso codes to flags", () => {
        expect(getCountryFlagEmoji("us")).toBeTruthy();
        expect(getCountryFlagEmoji("GB")).toBeTruthy();
    });

    it("exposes the country code tables", () => {
        expect(Object.keys(iso2CodesByCountryName).length).toBeGreaterThan(100);
        expect(validIso2CountryCodes.has("US")).toBe(true);
    });
});
