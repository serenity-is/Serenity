beforeEach(() => {
    jest.resetModules();
    jest.clearAllMocks();
});

describe("parseDayHourAndMin", () => {
    it("returns null for null, undefined and empty string", async function () {
        var formatting = (await import("./formatting-compat"));
        expect(formatting.parseDayHourAndMin(null)).toBeNull();
        expect(formatting.parseDayHourAndMin(undefined)).toBeNull();
        expect(formatting.parseDayHourAndMin("")).toBeNull();
        expect(formatting.parseDayHourAndMin("   ")).toBeNull();
    });

    it("returns number of minutes for day.hour:min", async function () {
        var formatting = (await import("./formatting-compat"));
        expect(formatting.parseDayHourAndMin("5.10:25")).toBe(5 * 24 * 60 + 10 * 60 + 25);
        expect(formatting.parseDayHourAndMin("5.10:00")).toBe(5 * 24 * 60 + 10 * 60);
    });

    it("returns number of minutes for hour:min", async function () {
        var formatting = (await import("./formatting-compat"));
        expect(formatting.parseDayHourAndMin("10:25")).toBe(10 * 60 + 25);
        expect(formatting.parseDayHourAndMin("10:00")).toBe(10 * 60);
        expect(formatting.parseDayHourAndMin("3:00")).toBe(3 * 60);
    });

    it("returns number of minutes for days only", async function () {
        var formatting = (await import("./formatting-compat"));
        expect(formatting.parseDayHourAndMin("10")).toBe(10 * 24 * 60);
    });

    it("returns NaN for invalid formats", async function () {
        var formatting = (await import("./formatting-compat"));
        expect(formatting.parseDayHourAndMin("10:61")).toBeNaN();
        expect(formatting.parseDayHourAndMin("25:00")).toBeNaN();
        expect(formatting.parseDayHourAndMin("1.10:61")).toBeNaN();
        expect(formatting.parseDayHourAndMin("5.25:00")).toBeNaN();
        expect(formatting.parseDayHourAndMin("5.25:00:3532")).toBeNaN();
        expect(formatting.parseDayHourAndMin("5.25:00.3532")).toBeNaN();
        expect(formatting.parseDayHourAndMin("5.325:1")).toBeNaN();
    });
});

describe("parseHourAndMin", () => {
    it("returns null for null, undefined and empty string", async function () {
        var formatting = (await import("./formatting-compat"));
        expect(formatting.parseHourAndMin(null)).toBeNull();
        expect(formatting.parseHourAndMin(undefined)).toBeNull();
        expect(formatting.parseHourAndMin("")).toBeNull();
        expect(formatting.parseHourAndMin("   ")).toBeNull();
    });

    it("returns number of minutes for hour:min", async function () {
        var formatting = (await import("./formatting-compat"));
        expect(formatting.parseHourAndMin("10:25")).toBe(10 * 60 + 25);
        expect(formatting.parseHourAndMin("10:00")).toBe(10 * 60);
        expect(formatting.parseHourAndMin("3:00")).toBe(3 * 60);
    });

    it("returns NaN for invalid formats", async function () {
        var formatting = (await import("./formatting-compat"));
        expect(formatting.parseHourAndMin("10:61")).toBeNaN();
        expect(formatting.parseHourAndMin("25:00")).toBeNaN();
        expect(formatting.parseHourAndMin("1.10:61")).toBeNaN();
        expect(formatting.parseHourAndMin("5.25:00")).toBeNaN();
        expect(formatting.parseHourAndMin("5.25:00:3532")).toBeNaN();
        expect(formatting.parseHourAndMin("5.25:00.3532")).toBeNaN();
        expect(formatting.parseHourAndMin("5.325:1")).toBeNaN();
    });
});

describe("formatDayHourAndMin", () => {
    it("returns empty string for null and undefined", async function () {
        var formatting = (await import("./formatting-compat"));
        expect(formatting.formatDayHourAndMin(null)).toBe("");
        expect(formatting.formatDayHourAndMin(undefined)).toBe("");
    });

    it("returns day part if total is more than one day", async function () {
        var formatting = (await import("./formatting-compat"));
        expect(formatting.formatDayHourAndMin(5 * 24 * 60 + 10 * 60 + 25)).toBe("5.10:25");
        expect(formatting.formatDayHourAndMin(5 * 24 * 60 + 10 * 60)).toBe("5.10:00");
    });

    it("returns no day part if total is less than one day", async function () {
        var formatting = (await import("./formatting-compat"));
        expect(formatting.formatDayHourAndMin(10 * 60 + 25)).toBe("10:25");
        expect(formatting.formatDayHourAndMin(10 * 60)).toBe("10:00");
        expect(formatting.formatDayHourAndMin(3 * 60)).toBe("03:00");
    });

    it("returns day only if without hour/min", async function () {
        var formatting = (await import("./formatting-compat"));
        expect(formatting.formatDayHourAndMin(10 * 24 * 60)).toBe("10");
    });

    it("returns '0' for 0", async function () {
        var formatting = (await import("./formatting-compat"));
        expect(formatting.formatDayHourAndMin(0)).toBe("0");
    });
});

describe("turkishLocaleToUpper", () => {
    it("ignores returns empty values as is", async () => {
        var formatting = (await import("./formatting-compat"));
        expect(formatting.turkishLocaleToUpper("")).toBe("");
        expect(formatting.turkishLocaleToUpper(null)).toBe(null);
        expect(formatting.turkishLocaleToUpper(undefined)).toBe(undefined);
    });

    it("converts i to İ and ı to I", async () => {
        var formatting = (await import("./formatting-compat"));
        expect(formatting.turkishLocaleToUpper("xıIiİıİia")).toBe("XIIİİIİİA");
    });
});

describe("turkishLocaleCompare", () => {
    it("is same with Culture.stringCompare", async () => {
        var formatting = (await import("./formatting-compat"));
        expect(formatting.turkishLocaleCompare).toBe((await import("../base")).Culture.stringCompare);
    });
});

export {}