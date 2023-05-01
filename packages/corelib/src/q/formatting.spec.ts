beforeEach(() => {
    jest.resetModules();
    jest.clearAllMocks();
});

describe("Q.formatDate", () => {
    it('formats properly based on format string and culture', async function () {
        var formatting = (await import("./formatting"));
        const Culture = formatting.Culture;
        const formatDate = formatting.formatDate;
        const formatISODateTimeUTC = formatting.formatISODateTimeUTC;
        Culture.dateSeparator = '/';
        Culture.dateFormat = 'dd/MM/yyyy';
        Culture.dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';
        var date = new Date(2029, 0, 2, 3, 4, 5, 6);
        // 02.01.2029 03:04:05.006
        expect(formatDate(date, 'dd/MM/yyyy')).toBe('02/01/2029')
        expect(formatDate(date, 'd/M/yy')).toBe('2/1/29');
        expect(formatDate(date, 'd.M.yyyy')).toBe('2.1.2029');
        expect(formatDate(date, 'yyyyMMdd')).toBe('20290102');
        expect(formatDate(date, 'hh:mm tt')).toBe('03:04 AM');
        expect(formatDate(date, 'yyyy-MM-ddTHH:mm:ss.fff')).toBe('2029-01-02T03:04:05.006');
        expect(formatDate(date, 'd')).toBe('02/01/2029');
        expect(formatDate(date, 'g')).toBe('02/01/2029 03:04');
        expect(formatDate(date, 'G')).toBe('02/01/2029 03:04:05');
        expect(formatDate(date, 's')).toBe('2029-01-02T03:04:05');
        expect(formatDate(date, 'u')).toBe(formatISODateTimeUTC(date));
        Culture.dateSeparator = '.';
        expect(formatDate(date, 'dd/MM/yyyy')).toBe('02.01.2029');
        expect(formatDate(date, 'd/M/yy')).toBe('2.1.29');
        expect(formatDate(date, 'd-M-yyyy')).toBe('2-1-2029');
        expect(formatDate(date, 'yyyy-MM-ddTHH:mm:ss.fff')).toBe('2029-01-02T03:04:05.006');
        expect(formatDate(date, 'g')).toBe('02.01.2029 03:04');
        expect(formatDate(date, 'G')).toBe('02.01.2029 03:04:05');
        expect(formatDate(date, 's')).toBe('2029-01-02T03:04:05');
        expect(formatDate(date, 'u')).toBe(formatISODateTimeUTC(date));
    });

    it('accepts iso datetime strings as input', async function () {
        var formatting = (await import("./formatting"));
        const Culture = formatting.Culture;
        const formatDate = formatting.formatDate;
        Culture.dateSeparator = '/';
        Culture.dateFormat = 'dd/MM/yyyy';
        Culture.dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';
        expect(formatDate('2029-01-02', null)).toBe('02/01/2029');
        expect(formatDate('2029-01-02T16:35:24', null)).toBe('02/01/2029');
        expect(formatDate('2029-01-02T16:35:24', 'g')).toBe('02/01/2029 16:35');
        Culture.dateSeparator = '.';
        expect(formatDate('2029-01-02', null)).toBe('02.01.2029');
        expect(formatDate('2029-01-02T16:35:24', null)).toBe('02.01.2029');
        expect(formatDate('2029-01-02T16:35:24', 'g')).toBe('02.01.2029 16:35');
    });

    it('accepts date string as input', async function () {
        var formatting = (await import("./formatting"));
        const Culture = formatting.Culture;
        const formatDate = formatting.formatDate;
        Culture.dateSeparator = '/';
        Culture.dateFormat = 'dd/MM/yyyy';
        Culture.dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';
        expect(formatDate('2/1/2029', null)).toBe('02/01/2029');
        Culture.dateSeparator = '.';
        expect(formatDate('2/1/2029', null)).toBe('02.01.2029');
    });
});

describe("Q.formatNumber", () => {
    it('formats with "." as decimal separator', async function () {
        var formatting = (await import("./formatting"));
        const Culture = formatting.Culture;
        const formatNumber = formatting.formatNumber;
        Culture.decimalSeparator = '.';
        Culture.groupSeparator = ',';
        expect(formatNumber(1, '0.00')).toBe('1.00');
        expect(formatNumber(1, '0.0000')).toBe('1.0000');
        expect(formatNumber(1234, '#,##0')).toBe('1,234');
        expect(formatNumber(1234.5, '#,##0.##')).toBe('1,234.5');
        expect(formatNumber(1234.5678, '#,##0.##')).toBe('1,234.57');
        expect(formatNumber(1234.5, '#,##0.00')).toBe('1,234.50');
    });

    it('formats with "," as decimal separator', async function () {
        var formatting = (await import("./formatting"));
        const Culture = formatting.Culture;
        const formatNumber = formatting.formatNumber;
        Culture.decimalSeparator = ',';
        Culture.groupSeparator = '.';
        expect(formatNumber(1, '0.00')).toBe('1,00');
        expect(formatNumber(1, '0.0000')).toBe('1,0000');
        expect(formatNumber(1234, '#,##0')).toBe('1.234');
        expect(formatNumber(1234.5, '#,##0.##')).toBe('1.234,5');
        expect(formatNumber(1234.5678, '#,##0.##')).toBe('1.234,57');
        expect(formatNumber(1234.5, '#,##0.00')).toBe('1.234,50');
    });


    it('formats number with prefix and suffix', async function () {
        var formatting = (await import("./formatting"));
        const formatNumber = formatting.formatNumber;
        const Culture = formatting.Culture;
        Culture.decimalSeparator = '.';
        expect(formatNumber(1234.5, '$#,##0.##')).toBe('$1,234.5');
        expect(formatNumber(1234.5, 'R #,##0.##')).toBe('R 1,234.5');
        expect(formatNumber(1234.5, '#,##0.## TL')).toBe('1,234.5 TL');
        expect(formatNumber(1, "','0")).toBe(',1');
        expect(formatNumber(1, "','0','")).toBe(',1,');
        expect(formatNumber(1, "\\,0\\,")).toBe(',1,');
    });
});

describe("Invariant.stringCompare", () => {
    it("uses string js string comparison", async function () {
        var formatting = (await import("./formatting"));
        const Invariant = formatting.Invariant;
        expect(Invariant.stringCompare("a", "b")).toBe(-1);
        expect(Invariant.stringCompare("b", "a")).toBe(1);
        expect(Invariant.stringCompare("a", "a")).toBe(0);
    });
});

describe("compareStringFactory", () => {
    it("when order is not specified uses localeCompare", async function () {
        var formatting = (await import("./formatting"));
        var localeCompareSpy = jest.spyOn(String.prototype, "localeCompare");
        try {
            var comparer = formatting.compareStringFactory("");
            expect(comparer("a", "b")).toBe(-1);
            expect(localeCompareSpy).toHaveBeenCalledTimes(1);
        }
        finally {
            localeCompareSpy.mockRestore();
        }
    });

    it("when order is specified uses the character order", async function () {
        var formatting = (await import("./formatting"));
        var localeCompareSpy = jest.spyOn(String.prototype, "localeCompare");
        try {
            var comparer = formatting.compareStringFactory("cba");
            expect(comparer("a", "b")).toBe(1);
            expect(localeCompareSpy).not.toHaveBeenCalled();
        }
        finally {
            localeCompareSpy.mockRestore();
        }
    });

    it("calls localeCompare for strings of different length", async function () {
        var formatting = (await import("./formatting"));
        var localeCompareSpy = jest.spyOn(String.prototype, "localeCompare");
        try {
            var comparer = formatting.compareStringFactory("cba");
            expect(comparer("ab", "abx")).toBe(-1);
            expect(localeCompareSpy).toBeCalledTimes(1);
        }
        finally {
            localeCompareSpy.mockRestore();
        }
    });

    it("returns 0 for same strings", async function () {
        var formatting = (await import("./formatting"));
        var localeCompareSpy = jest.spyOn(String.prototype, "localeCompare");
        try {
            var comparer = formatting.compareStringFactory("");
            expect(comparer("d", "d")).toBe(0);
            expect(localeCompareSpy).not.toHaveBeenCalled();
            expect(comparer("dx", "dx")).toBe(0);
            expect(localeCompareSpy).not.toHaveBeenCalled();

            comparer = formatting.compareStringFactory("yz");
            expect(comparer("dxy", "dxz")).toBe(-1);
            expect(localeCompareSpy).not.toHaveBeenCalled();
            expect(comparer("dyz", "dzz")).toBe(-1);
            expect(localeCompareSpy).not.toHaveBeenCalled();
        }
        finally {
            localeCompareSpy.mockRestore();
        }
    });
});

describe("splitDateString", () => {
    it("returns undefined for null, undefined and empty string", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.splitDateString(null)).toBeUndefined();
        expect(formatting.splitDateString(undefined)).toBeUndefined();
        expect(formatting.splitDateString("")).toBeUndefined();
        expect(formatting.splitDateString("  ")).toBeUndefined();
    });

    it("can handle '/', '.', '\\', '-' as separator", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.splitDateString("2029/01/02")).toEqual(["2029", "01", "02"]);
        expect(formatting.splitDateString("1/2/2029")).toEqual(["1", "2", "2029"]);
        expect(formatting.splitDateString("2029.01.02")).toEqual(["2029", "01", "02"]);
        expect(formatting.splitDateString("1.2.2029")).toEqual(["1", "2", "2029"]);
        expect(formatting.splitDateString("2029\\01\\02")).toEqual(["2029", "01", "02"]);
        expect(formatting.splitDateString("1\\2\\2029")).toEqual(["1", "2", "2029"]);
        expect(formatting.splitDateString("2029-01-02")).toEqual(["2029", "01", "02"]);
        expect(formatting.splitDateString("1-2-2029")).toEqual(["1", "2", "2029"]);
    });

    it("returns an array with the string if no known separators", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.splitDateString("2")).toEqual(["2"]);
        expect(formatting.splitDateString("ab")).toEqual(["ab"]);
    });
});

describe("parseDate", () => {
    it("returns undefined for null, undefined, empty string and whitespace", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseDate(null)).toBeUndefined();
        expect(formatting.parseDate(undefined)).toBeUndefined();
        expect(formatting.parseDate("")).toBeUndefined();
        expect(formatting.parseDate("   ")).toBeUndefined();
    });

    it("returns false for malformed iso date string", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseDate("2023-00-01")?.valueOf()).toBeNaN();
        expect(formatting.parseDate("2023-01-X1")?.valueOf()).toBeNaN();
    });

    it("can handle iso like dates with space between date/time parts", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseDate("2023-01-05 16:30")).toEqual(new Date(2023, 0, 5, 16, 30));
        expect(formatting.parseDate("2023-00-05 16:30")?.valueOf()).toBeNaN();
    });

    it("can handle date only with ymd order", async function () {
        var formatting = (await import("./formatting"));
        formatting.Culture.dateOrder = "ymd";
        expect(formatting.parseDate("2023/03/05")).toEqual(new Date(2023, 2, 5));
    });

    it("can handle date only with mdy order", async function () {
        var formatting = (await import("./formatting"));
        formatting.Culture.dateOrder = "mdy";
        expect(formatting.parseDate("11/23/2023")).toEqual(new Date(2023, 10, 23));
    });

    it("returns NaN if a part is not a number or out of range", async function () {
        var formatting = (await import("./formatting"));
        formatting.Culture.dateOrder = "dmy";
        expect(formatting.parseDate("01/X1/2023")?.valueOf()).toBeNaN();
        expect(formatting.parseDate("01/01/20X3")?.valueOf()).toBeNaN();
        expect(formatting.parseDate("X1/01/2023")?.valueOf()).toBeNaN();
        expect(formatting.parseDate("32/02/2023")?.valueOf()).toBeNaN();
        expect(formatting.parseDate("15/14/2023")?.valueOf()).toBeNaN();
        expect(formatting.parseDate("15/-1/2023")?.valueOf()).toBeNaN();
        expect(formatting.parseDate("15/0/2023")?.valueOf()).toBeNaN();
        expect(formatting.parseDate("15/1/-1")?.valueOf()).toBeNaN();
        expect(formatting.parseDate("2014-25-23")?.valueOf()).toBeNaN();
        expect(formatting.parseDate("2014/03")?.valueOf()).toBeNaN();
    });

    it("handles two digit years", async function () {
        var formatting = (await import("./formatting"));
        jest.useFakeTimers();
        try {
            jest.setSystemTime(new Date(2023, 1, 1));
            expect(formatting.parseDate("15/1/01")).toEqual(new Date(2001, 0, 15));
            expect(formatting.parseDate("15/1/99")).toEqual(new Date(1999, 0, 15));
        }
        finally {
            jest.useRealTimers();
        }
    });

    it("handles JS date string", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseDate("Tue May 02 2023")).toEqual(new Date(2023, 4, 2));
    });
});

describe("parseISODateTime", () => {
    it("returns undefined for null, undefined and empty string", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseISODateTime(null)).toBeUndefined();
        expect(formatting.parseISODateTime(undefined)).toBeUndefined();
        expect(formatting.parseISODateTime("")).toBeUndefined();
    });

    it("returns invalid date for malformed iso date string", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseISODateTime("2023-00-01")?.valueOf()).toBeNaN();
        expect(formatting.parseISODateTime("2023-01-X1")?.valueOf()).toBeNaN();
    });

    it("returns invalid date for non string", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseISODateTime(5 as any as string)?.valueOf()).toBeNaN();
    });

    it("returns false for malformed iso date string", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseISODateTime("2023-00-01")?.valueOf()).toBeNaN();
        expect(formatting.parseISODateTime("2023-01-X1")?.valueOf()).toBeNaN();
    });
});