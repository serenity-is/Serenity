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
        expect(formatDate(date, 'U')).toContain('2029');
        expect(formatDate(date, 'U', {} as any)).toContain('2029');
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
    it("returns null for null, undefined and empty string", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.splitDateString(null)).toBeNull();
        expect(formatting.splitDateString(undefined)).toBeNull();
        expect(formatting.splitDateString("")).toBeNull();
        expect(formatting.splitDateString("  ")).toBeNull();
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
    it("returns null for null, undefined, empty string and whitespace", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseDate(null)).toBeNull();
        expect(formatting.parseDate(undefined)).toBeNull();
        expect(formatting.parseDate("")).toBeNull();
        expect(formatting.parseDate("   ")).toBeNull();
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
    it("returns null for null, undefined and empty string", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseISODateTime(null)).toBeNull();
        expect(formatting.parseISODateTime(undefined)).toBeNull();
        expect(formatting.parseISODateTime("")).toBeNull();
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

describe("parseDayHourAndMin", () => {
    it("returns null for null, undefined and empty string", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseDayHourAndMin(null)).toBeNull();
        expect(formatting.parseDayHourAndMin(undefined)).toBeNull();
        expect(formatting.parseDayHourAndMin("")).toBeNull();
        expect(formatting.parseDayHourAndMin("   ")).toBeNull();
    });

    it("returns number of minutes for day.hour:min", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseDayHourAndMin("5.10:25")).toBe(5 * 24 * 60 + 10 * 60 + 25);
        expect(formatting.parseDayHourAndMin("5.10:00")).toBe(5 * 24 * 60 + 10 * 60);
    });

    it("returns number of minutes for hour:min", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseDayHourAndMin("10:25")).toBe(10 * 60 + 25);
        expect(formatting.parseDayHourAndMin("10:00")).toBe(10 * 60);
        expect(formatting.parseDayHourAndMin("3:00")).toBe(3 * 60);
    });

    it("returns number of minutes for days only", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseDayHourAndMin("10")).toBe(10 * 24 * 60);
    });

    it("returns NaN for invalid formats", async function () {
        var formatting = (await import("./formatting"));
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
        var formatting = (await import("./formatting"));
        expect(formatting.parseHourAndMin(null)).toBeNull();
        expect(formatting.parseHourAndMin(undefined)).toBeNull();
        expect(formatting.parseHourAndMin("")).toBeNull();
        expect(formatting.parseHourAndMin("   ")).toBeNull();
    });

    it("returns number of minutes for hour:min", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseHourAndMin("10:25")).toBe(10 * 60 + 25);
        expect(formatting.parseHourAndMin("10:00")).toBe(10 * 60);
        expect(formatting.parseHourAndMin("3:00")).toBe(3 * 60);
    });

    it("returns NaN for invalid formats", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseHourAndMin("10:61")).toBeNaN();
        expect(formatting.parseHourAndMin("25:00")).toBeNaN();
        expect(formatting.parseHourAndMin("1.10:61")).toBeNaN();
        expect(formatting.parseHourAndMin("5.25:00")).toBeNaN();
        expect(formatting.parseHourAndMin("5.25:00:3532")).toBeNaN();
        expect(formatting.parseHourAndMin("5.25:00.3532")).toBeNaN();
        expect(formatting.parseHourAndMin("5.325:1")).toBeNaN();
    });
});

describe("formatISODateTimeUTC", () => {
    it("returns empty string for null and undefined", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.formatISODateTimeUTC(null)).toBe("");
        expect(formatting.formatISODateTimeUTC(undefined)).toBe("");
    });

    it("formats in ISO format with UTC timezone", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.formatISODateTimeUTC(new Date(Date.UTC(2023, 4, 2, 15, 10, 5, 32)))).toBe("2023-05-02T15:10:05.032Z");
        expect(formatting.formatISODateTimeUTC(new Date(Date.UTC(2023, 4, 2, 15, 10, 5, 150)))).toBe("2023-05-02T15:10:05.15Z");
    });
});

describe("formatDayHourAndMin", () => {
    it("returns empty string for null and undefined", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.formatDayHourAndMin(null)).toBe("");
        expect(formatting.formatDayHourAndMin(undefined)).toBe("");
    });

    it("returns day part if total is more than one day", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.formatDayHourAndMin(5 * 24 * 60 + 10 * 60 + 25)).toBe("5.10:25");
        expect(formatting.formatDayHourAndMin(5 * 24 * 60 + 10 * 60)).toBe("5.10:00");
    });

    it("returns no day part if total is less than one day", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.formatDayHourAndMin(10 * 60 + 25)).toBe("10:25");
        expect(formatting.formatDayHourAndMin(10 * 60)).toBe("10:00");
        expect(formatting.formatDayHourAndMin(3 * 60)).toBe("03:00");
    });

    it("returns day only if without hour/min", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.formatDayHourAndMin(10 * 24 * 60)).toBe("10");
    });

    it("returns '0' for 0", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.formatDayHourAndMin(0)).toBe("0");
    });
});

describe("formatDate", () => {
    it("returns empty string for null and undefined", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.formatDate(null)).toBe("");
        expect(formatting.formatDate(undefined)).toBe("");
    });

    it("returns date in format 'dd.MM.yyyy' by default if order is dmy", async function () {
        var formatting = (await import("./formatting"));
        formatting.Culture.dateFormat = "dd/MM/yyyy";
        formatting.Culture.dateSeparator = ".";
        expect(formatting.formatDate(new Date(2023, 4, 2, 15, 10, 5, 32))).toBe("02.05.2023");
    });

    it("returns date in format 'MM.dd.yyyy' if Culture.dateFormat is MM/dd/yyyy", async function () {
        var formatting = (await import("./formatting"));
        formatting.Culture.dateFormat = "MM/dd/yyyy";
        formatting.Culture.dateSeparator = ".";
        expect(formatting.formatDate(new Date(2023, 4, 2, 15, 10, 5, 32))).toBe("05.02.2023");
    });


    it("returns date in format 'yyyy.mm.dd' by default if order is yyyy/MM/dd", async function () {
        var formatting = (await import("./formatting"));
        formatting.Culture.dateFormat = "yyyy/MM/dd";
        formatting.Culture.dateSeparator = ".";
        expect(formatting.formatDate(new Date(2023, 4, 2, 15, 10, 5, 32))).toBe("2023.05.02");
    });

    it("uses passed locale", async function () {
        var formatting = (await import("./formatting"));
        var locale = Object.assign({}, formatting.Culture);
        locale.dateOrder = "mdy";
        locale.dateSeparator = "*";
        expect(formatting.formatDate(new Date(2023, 4, 2, 15, 10, 5, 32), undefined, locale)).toBe("02*05*2023");
    });

    it("uses Culture.dateFormat if locale.dateFormat is null", async function () {
        var formatting = (await import("./formatting"));
        var locale = Object.assign({}, formatting.Culture);
        formatting.Culture.dateFormat = "yyyy/MM/dd";
        locale.dateFormat = null;
        locale.dateSeparator = "*";
        expect(formatting.formatDate(new Date(2023, 4, 2, 15, 10, 5, 32), undefined, locale)).toBe("2023*05*02");
    });

    it("uses Culture.dateFormat for 'd' if locale.dateFormat is null", async function () {
        var formatting = (await import("./formatting"));
        var locale = Object.assign({}, formatting.Culture);
        formatting.Culture.dateFormat = "yyyy/MM/dd";
        locale.dateFormat = null;
        locale.dateSeparator = "*";
        expect(formatting.formatDate(new Date(2023, 4, 2, 15, 10, 5, 32), 'd', locale)).toBe("2023*05*02");
    });

    it("uses Culture.dateTimeFormat for 'g' if locale.dateTimeFormat is null", async function () {
        var formatting = (await import("./formatting"));
        var locale = Object.assign({}, formatting.Culture);
        formatting.Culture.dateTimeFormat = "yyyy/MM/dd HH:mm:ss";
        locale.dateTimeFormat = null;
        locale.dateSeparator = "*";
        expect(formatting.formatDate(new Date(2023, 4, 2, 15, 10, 5, 32), 'g', locale)).toBe("2023*05*02 15:10");
    });

    it("uses Culture.dateTimeFormat for 'G' if locale.dateTimeFormat is null", async function () {
        var formatting = (await import("./formatting"));
        var locale = Object.assign({}, formatting.Culture);
        formatting.Culture.dateTimeFormat = "yyyy/MM/dd HH:mm:ss";
        locale.dateTimeFormat = null;
        locale.dateSeparator = "*";
        expect(formatting.formatDate(new Date(2023, 4, 2, 15, 10, 5, 32), 'G', locale)).toBe("2023*05*02 15:10:05");
    });

    it("uses Culture.dateTimeFormat and Culture.dateFormat for 't'", async function () {
        var formatting = (await import("./formatting"));
        formatting.Culture.dateTimeFormat = "yyyy/MM/dd H:m";
        formatting.Culture.dateFormat = "yyyy/MM/dd";
        expect(formatting.formatDate(new Date(2023, 4, 2, 3, 9, 5, 32), 't')).toBe("3:9");
    });

    it("returns browser toString() for 'i' format", async function () {
        var formatting = (await import("./formatting"));
        var date = new Date(2023, 4, 2, 15, 10, 5, 32);
        expect(formatting.formatDate(date, "i")).toBe(date.toString());
    });

    it("returns browser toDateString() for 'id' format", async function () {
        var formatting = (await import("./formatting"));
        var date = new Date(2023, 4, 2, 15, 10, 5, 32);
        expect(formatting.formatDate(date, "id")).toBe(date.toDateString());
    });

    it("returns browser toTimeString() for 'it' format", async function () {
        var formatting = (await import("./formatting"));
        var date = new Date(2023, 4, 2, 15, 10, 5, 32);
        expect(formatting.formatDate(date, "it")).toBe(date.toTimeString());
    });

    it("can parse strings before formatting", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.formatDate("  ")).toBe("");
        expect(formatting.formatDate("\t")).toBe("");
        expect(formatting.formatDate("")).toBe("");
        expect(formatting.formatDate("xyz")).toBe("xyz");
        expect(formatting.formatDate("xyz", null, formatting.Culture)).toBe("xyz");
    });

    it("can use custom format strings", async function () {
        var formatting = (await import("./formatting"));
        var date = new Date(2023, 5, 2, 15, 10, 5, 32);
        var locale = Object.assign({}, formatting.Culture);
        formatting.Culture.dateSeparator = "*";
        locale.dateSeparator = null;
        locale.monthNames = null;
        locale.dayNames = null;
        locale.shortMonthNames = null;
        locale.shortDayNames = null;
        locale.amDesignator = null;
        locale.pmDesignator = null;
        locale.timeSeparator = null;

        expect(formatting.formatDate(date, "yy/MM/d", locale)).toBe("23*06*2");

        expect(formatting.formatDate(date, "dddd", locale)).toBe("Friday");
        expect(formatting.formatDate(date, "dddd")).toBe("Friday");

        expect(formatting.formatDate(date, "ddd", locale)).toBe("Fri");
        expect(formatting.formatDate(date, "ddd")).toBe("Fri");

        expect(formatting.formatDate(date, "MMMM", locale)).toBe("June");
        expect(formatting.formatDate(date, "MMMM")).toBe("June");

        expect(formatting.formatDate(date, "MMM", locale)).toBe("Jun");
        expect(formatting.formatDate(date, "MMM")).toBe("Jun");

        expect(formatting.formatDate(date, "y", locale)).toBe("23");
        expect(formatting.formatDate(date, "y")).toBe("23");

        expect(formatting.formatDate(date, "h")).toBe("3");
        expect(formatting.formatDate(date, "hh")).toBe("03");

        expect(formatting.formatDate(new Date(2023, 4, 2, 0, 0, 0, 32), "h")).toBe("12");
        expect(formatting.formatDate(new Date(2023, 4, 2, 0, 0, 0, 32), "hh")).toBe("12");

        expect(formatting.formatDate(date, "h:m:s")).toBe("3:10:5");

        expect(formatting.formatDate(date, "H:m t", locale)).toBe("15:10 P");
        expect(formatting.formatDate(date, "H:m t")).toBe("15:10 P");
        expect(formatting.formatDate(date, "H:m tt", locale)).toBe("15:10 PM");
        expect(formatting.formatDate(date, "H:m tt")).toBe("15:10 PM");
        expect(formatting.formatDate(new Date(2023, 4, 2, 1, 0, 0, 32), "h t")).toBe("1 A");
        expect(formatting.formatDate(new Date(2023, 4, 2, 1, 0, 0, 32), "h t", locale)).toBe("1 A");
        expect(formatting.formatDate(new Date(2023, 4, 2, 1, 0, 0, 32), "h tt")).toBe("1 AM");
        expect(formatting.formatDate(new Date(2023, 4, 2, 1, 0, 0, 32), "h tt", locale)).toBe("1 AM");

        expect(formatting.formatDate(new Date(2023, 4, 2, 1, 0, 0, 32), "h f", locale)).toBe("1 0");
        expect(formatting.formatDate(new Date(2023, 4, 2, 1, 0, 0, 32), "h f")).toBe("1 0");

        expect(formatting.formatDate(new Date(2023, 4, 2, 1, 0, 0, 32), "h ff", locale)).toBe("1 03");
        expect(formatting.formatDate(new Date(2023, 4, 2, 1, 0, 0, 32), "h ff")).toBe("1 03");

        expect(formatting.formatDate(new Date(2023, 4, 2, 1, 0, 0, 32), "h fff", locale)).toBe("1 032");
        expect(formatting.formatDate(new Date(2023, 4, 2, 1, 0, 0, 32), "h fff")).toBe("1 032");

        expect(formatting.formatDate(date, "'dMy'", locale)).toBe("dMy");
        expect(formatting.formatDate(date, "'dMy'")).toBe("dMy");

        expect(formatting.formatDate(date, "%'dMy'", locale)).toBe("dMy");
        expect(formatting.formatDate(date, "%'dMy'")).toBe("dMy");

        date.getTimezoneOffset = function () { return +120 };

        expect(formatting.formatDate(date, "h z", locale)).toBe("3 -2");
        expect(formatting.formatDate(date, "h z")).toBe("3 -2");

        expect(formatting.formatDate(date, "h zz", locale)).toBe("3 -02");
        expect(formatting.formatDate(date, "h zz")).toBe("3 -02");

        expect(formatting.formatDate(date, "h zzz", locale)).toBe("3 -02:00");
        expect(formatting.formatDate(date, "h zzz")).toBe("3 -02:00");

        date.getTimezoneOffset = function () { return -120 };

        expect(formatting.formatDate(date, "h z", locale)).toBe("3 +2");
        expect(formatting.formatDate(date, "h z")).toBe("3 +2");

        expect(formatting.formatDate(date, "h zz", locale)).toBe("3 +02");
        expect(formatting.formatDate(date, "h zz")).toBe("3 +02");

        expect(formatting.formatDate(date, "h zzz", locale)).toBe("3 +02:00");
        expect(formatting.formatDate(date, "h zzz")).toBe("3 +02:00");
    });
});

describe("toId", () => {
    it("returns null for undefined, null, empty string, whitespace", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.toId(undefined)).toBeNull();
        expect(formatting.toId(null)).toBeNull();
        expect(formatting.toId("")).toBeNull();
        expect(formatting.toId(" ")).toBeNull();
    });

    it("returns numbers as is", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.toId(5)).toBe(5);
        expect(formatting.toId(-1)).toBe(-1);
        expect(formatting.toId(3.5)).toBe(3.5);
    });

    it("returns strings longer than 15 as is", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.toId("1234567890123456")).toBe("1234567890123456");
        expect(formatting.toId("ab34567890123456")).toBe("ab34567890123456");
    });

    it("returns strings containing non digits as is", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.toId("ab3456789")).toBe("ab3456789");
    });

    it("returns strings containing all digits as number", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.toId("3456789")).toBe(3456789);
        expect(formatting.toId("-1234567890")).toBe(-1234567890);
    });

});

describe("parseDecimal", () => {
    it("returns null for undefined, null, empty string, whitespace", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseDecimal(undefined)).toBeNull();
        expect(formatting.parseDecimal(null)).toBeNull();
        expect(formatting.parseDecimal("")).toBeNull();
        expect(formatting.parseDecimal(" ")).toBeNull();
    });

    it("returns numbers as is", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseDecimal("5")).toBe(5);
        expect(formatting.parseDecimal("-1")).toBe(-1);
        expect(formatting.parseDecimal("3.5")).toBe(3.5);
    });

    it("trims strings", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseDecimal(" 5.0 ")).toBe(5);
        expect(formatting.parseDecimal(" 5 ")).toBe(5);
    });

    it("returns NaN for strings containing non digits", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseDecimal("ab3456789")).toBeNaN();
    });

    it("can handle group separator", async function () {
        var formatting = (await import("./formatting"));
        formatting.Culture.groupSeparator = ",";
        formatting.Culture.decimalSeparator = ".";
        expect(formatting.parseDecimal("3,245,148.4")).toBe(3245148.4);
        expect(formatting.parseDecimal("-3,245,148.4")).toBe(-3245148.4);
    });

    it("can use Culture decimal separator", async function () {
        var formatting = (await import("./formatting"));
        formatting.Culture.groupSeparator = ".";
        formatting.Culture.decimalSeparator = ",";
        expect(formatting.parseDecimal("3.245.148,4")).toBe(3245148.4);
        expect(formatting.parseDecimal("-3.245.148,4")).toBe(-3245148.4);
        expect(formatting.parseDecimal("-3245148,4")).toBe(-3245148.4);
    });
});

describe("parseInteger", () => {
    it("returns null for undefined, null, empty string, whitespace", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseInteger(undefined)).toBeNull();
        expect(formatting.parseInteger(null)).toBeNull();
        expect(formatting.parseInteger("")).toBeNull();
        expect(formatting.parseInteger(" ")).toBeNull();
    });

    it("returns integer numbers as is", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseInteger("5")).toBe(5);
        expect(formatting.parseInteger("-1")).toBe(-1);
        expect(formatting.parseInteger("3")).toBe(3);
    });

    it("trims strings", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseInteger(" 5 ")).toBe(5);
    });

    it("returns NaN for strings containing non digits", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.parseInteger("ab3456789")).toBeNaN();
    });

    it("can handle group separator", async function () {
        var formatting = (await import("./formatting"));
        formatting.Culture.groupSeparator = ",";
        formatting.Culture.decimalSeparator = ".";
        expect(formatting.parseInteger("3,245,148")).toBe(3245148);
        expect(formatting.parseInteger("-3,245,148")).toBe(-3245148);
    });

    it("can use Culture group separator", async function () {
        var formatting = (await import("./formatting"));
        formatting.Culture.groupSeparator = ".";
        expect(formatting.parseDecimal("3.245.148")).toBe(3245148);
        expect(formatting.parseDecimal("-3.245.148")).toBe(-3245148);
        expect(formatting.parseDecimal("-3245148")).toBe(-3245148);
    });
});

describe("formatNumber", () => {

    it("returns empty string for undefined, null", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.formatNumber(undefined)).toBe("");
        expect(formatting.formatNumber(null)).toBe("");
    });

    it("returns Culture.nanSymbol string for NaN", async function () {
        var formatting = (await import("./formatting"));
        formatting.Culture.nanSymbol = "NaN";
        var locale = Object.assign({}, formatting.Culture);
        locale.nanSymbol = "NaNNN";
        expect(formatting.formatNumber(NaN, null, locale)).toBe("NaNNN");
        locale.nanSymbol = null;
        expect(formatting.formatNumber(NaN, null, locale)).toBe("NaN");
    });

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

    it('handles decimal format specifier', async function () {
        var formatting = (await import("./formatting"));
        const formatNumber = formatting.formatNumber;
        const Culture = formatting.Culture;
        Culture.decimalSeparator = '.';
        expect(formatNumber(1234.5, 'D')).toBe('1234');
        expect(formatNumber(1234.5, 'd')).toBe('1234');
        expect(formatNumber(1234.5, 'D6')).toBe('001234');
        expect(formatNumber(-1234.5, 'D6')).toBe('-001234');
        expect(formatNumber(-1234.5, 'd6')).toBe('-001234');
    });

    it('handles hex format specifier', async function () {
        var formatting = (await import("./formatting"));
        const formatNumber = formatting.formatNumber;
        const Culture = formatting.Culture;
        Culture.decimalSeparator = '.';
        expect(formatNumber(255, 'X')).toBe('FF');
        expect(formatNumber(255, 'X4')).toBe('00FF');
        expect(formatNumber(255, 'x')).toBe('ff');
        expect(formatNumber(255, 'x4')).toBe('00ff');
        // can't handle negative hex yet as it would require target number of bits
        // expect(formatNumber(-1, 'x')).toBe('ff');
        // expect(formatNumber(-1, 'x4')).toBe('ffff');
    });

    it('handles exponantional format specifier', async function () {
        var formatting = (await import("./formatting"));
        const formatNumber = formatting.formatNumber;
        const Culture = formatting.Culture;
        Culture.decimalSeparator = '.';
        expect(formatNumber(1052.0329112756, 'E')).toBe('1.052033E+3');
        expect(formatNumber(1052.0329112756, 'e')).toBe('1.052033e+3');
        expect(formatNumber(-1052.0329112756, 'e2')).toBe('-1.05e+3');
        expect(formatNumber(-1052.0329112756, 'E2')).toBe('-1.05E+3');
    });

    it("returns .toString() with decimal replaced if format is null or empty string", async function () {
        var formatting = (await import("./formatting"));
        formatting.Culture.groupSeparator = "!";
        formatting.Culture.decimalSeparator = "*";
        expect(formatting.formatNumber(5.3, null)).toBe("5*3");
        expect(formatting.formatNumber(5.3, undefined)).toBe("5*3");
        expect(formatting.formatNumber(5.3, "")).toBe("5*3");
    });

    it("returns .toString() if format is 'i'", async function () {
        var formatting = (await import("./formatting"));
        formatting.Culture.groupSeparator = "!";
        formatting.Culture.decimalSeparator = "*";
        expect(formatting.formatNumber(5.3, "i")).toBe("5.3");
    });

    it("uses group separator if specified in last argument", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.formatNumber(54321, "#,##0", null, "!")).toBe("54!321");
    });

    it("uses group separator if specified", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.formatNumber(54321.15, "#,##0.00", "!")).toBe("54,321!15");
    });

    it("handles fixed point format specifier", async function () {
        var formatting = (await import("./formatting"));
        const Culture = formatting.Culture;
        Culture.decimalSeparator = '.';
        expect(formatting.formatNumber(1234.567, "f")).toBe("1234.57");
        expect(formatting.formatNumber(1234.567, "F1")).toBe("1234.6");
        expect(formatting.formatNumber(1234.567, "F2")).toBe("1234.57");
        expect(formatting.formatNumber(1234.567, "F3")).toBe("1234.567");
        expect(formatting.formatNumber(1234.567, "F4")).toBe("1234.5670");
        expect(formatting.formatNumber(-1234.567, "f")).toBe("-1234.57");
        expect(formatting.formatNumber(-1234.567, "F1")).toBe("-1234.6");
        expect(formatting.formatNumber(-1234.567, "F2")).toBe("-1234.57");
        expect(formatting.formatNumber(-1234.567, "F3")).toBe("-1234.567");
        expect(formatting.formatNumber(-1234.567, "F4")).toBe("-1234.5670");
        Culture.decimalDigits = 3;
        expect(formatting.formatNumber(1234.567, "f")).toBe("1234.567");
        Culture.decimalDigits = 4;
        var locale = Object.assign({}, Culture, {
            decimalDigits: null
        });
        expect(formatting.formatNumber(1234.567, "f", locale)).toBe("1234.5670");
        locale.decimalSeparator = ",";
        expect(formatting.formatNumber(1234.567, "f", locale)).toBe("1234,5670");
    });

    it("handles numberic format specifier", async function () {
        var formatting = (await import("./formatting"));
        const Culture = formatting.Culture;
        Culture.decimalSeparator = '.';
        expect(formatting.formatNumber(1234.567, "n")).toBe("1,234.57");
        expect(formatting.formatNumber(1234.567, "n1")).toBe("1,234.6");
        expect(formatting.formatNumber(1234.567, "n2")).toBe("1,234.57");
        expect(formatting.formatNumber(1234.567, "N3")).toBe("1,234.567");
        expect(formatting.formatNumber(1234.567, "N4")).toBe("1,234.5670");
        expect(formatting.formatNumber(-1234.567, "N")).toBe("-1,234.57");
        expect(formatting.formatNumber(-1234.567, "N1")).toBe("-1,234.6");
        expect(formatting.formatNumber(-1234.567, "N2")).toBe("-1,234.57");
        expect(formatting.formatNumber(-1234.567, "N3")).toBe("-1,234.567");
        expect(formatting.formatNumber(-1234.567, "N4")).toBe("-1,234.5670");
        Culture.decimalDigits = 3;
        expect(formatting.formatNumber(1234.567, "n")).toBe("1,234.567");
        Culture.decimalDigits = 4;
        var locale = Object.assign({}, Culture, {
            decimalDigits: null
        });
        expect(formatting.formatNumber(1234.567, "n", locale)).toBe("1,234.5670");
        locale.decimalSeparator = ",";
        locale.groupSeparator = ".";
        expect(formatting.formatNumber(1234.567, "n", locale)).toBe("1.234,5670");
        expect(formatting.formatNumber(12.567, "n", locale)).toBe("12,5670");
    });

    it("handles currency format specifier", async function () {
        var formatting = (await import("./formatting"));
        const Culture = formatting.Culture;
        Culture.decimalSeparator = '.';
        Culture.currencySymbol = '₺';
        expect(formatting.formatNumber(1234.567, "c")).toBe("1,234.57₺");
        expect(formatting.formatNumber(1234.567, "c1")).toBe("1,234.6₺");
        expect(formatting.formatNumber(1234.567, "c2")).toBe("1,234.57₺");
        expect(formatting.formatNumber(1234.567, "C3")).toBe("1,234.567₺");
        expect(formatting.formatNumber(1234.567, "c4")).toBe("1,234.5670₺");
        expect(formatting.formatNumber(-1234.567, "C")).toBe("-1,234.57₺");
        expect(formatting.formatNumber(-1234.567, "c1")).toBe("-1,234.6₺");
        expect(formatting.formatNumber(-1234.567, "C2")).toBe("-1,234.57₺");
        expect(formatting.formatNumber(-1234.567, "c3")).toBe("-1,234.567₺");
        expect(formatting.formatNumber(-1234.567, "C4")).toBe("-1,234.5670₺");
        Culture.decimalDigits = 3;
        expect(formatting.formatNumber(1234.567, "c")).toBe("1,234.567₺");
        Culture.decimalDigits = 4;
        Culture.currencySymbol = '?';
        var locale = Object.assign({}, Culture, {
            decimalDigits: null
        });
        expect(formatting.formatNumber(1234.567, "c", locale)).toBe("1,234.5670?");
        locale.decimalSeparator = ",";
        locale.groupSeparator = ".";
        expect(formatting.formatNumber(1234.567, "c", locale)).toBe("1.234,5670?");
        expect(formatting.formatNumber(12.567, "c", locale)).toBe("12,5670?");
    });

    it("handles currency percent specifier", async function () {
        var formatting = (await import("./formatting"));
        const Culture = formatting.Culture;
        Culture.decimalSeparator = '.';
        Culture.currencySymbol = '₺';
        expect(formatting.formatNumber(12.567, "p")).toBe("1,256.70%");
        expect(formatting.formatNumber(12.567, "p1")).toBe("1,256.7%");
        expect(formatting.formatNumber(12.567, "p2")).toBe("1,256.70%");
        expect(formatting.formatNumber(12.567, "p3")).toBe("1,256.700%");
        expect(formatting.formatNumber(12.567, "p4")).toBe("1,256.7000%");
        expect(formatting.formatNumber(-12.567, "p")).toBe("-1,256.70%");
        expect(formatting.formatNumber(-12.567, "p1")).toBe("-1,256.7%");
        expect(formatting.formatNumber(-12.567, "p2")).toBe("-1,256.70%");
        expect(formatting.formatNumber(-12.567, "p3")).toBe("-1,256.700%");
        expect(formatting.formatNumber(-12.567, "p4")).toBe("-1,256.7000%");
        Culture.decimalDigits = 3;
        expect(formatting.formatNumber(12.567, "p")).toBe("1,256.700%");
        Culture.decimalDigits = 4;
        Culture.percentSymbol = '?';
        var locale = Object.assign({}, Culture, {
            decimalDigits: null
        });
        expect(formatting.formatNumber(12.567, "p", locale)).toBe("1,256.7000?");
        locale.decimalSeparator = ",";
        locale.groupSeparator = ".";
        expect(formatting.formatNumber(12.567, "p", locale)).toBe("1.256,7000?");
    });

    it("handles start zero padding", async function() {
        var formatting = (await import("./formatting"));
        const Culture = formatting.Culture;
        Culture.decimalSeparator = '.';
        expect(formatting.formatNumber(1.23, "000")).toBe("001");
    });
});

describe("format", () => {
    it("handles double braces", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.format("{{{0}}}", "abc")).toBe("{abc}");
    });

    it("formats null and undefined as empty string", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.format("{0}", null)).toBe("");
        expect(formatting.format("x{0}y", null)).toBe("xy");
        expect(formatting.format("{0}", undefined)).toBe("");
        expect(formatting.format("x{0}y", undefined)).toBe("xy");
    });

    it("handles number format specifiers", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.format("{0:000}", 5)).toBe("005");
    });

    it("handles date format specifiers", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.format("{0:yyyy-MM-dd}", new Date(2020, 1, 1))).toBe("2020-02-01");
    });
    
    it("calls obj.format if available", async function () {
        var formatting = (await import("./formatting"));
        expect(formatting.format("{0:x}", {format: (s: string) => s})).toBe("x");
    });
});


describe("localeFormat", () => {
    it("handles double braces", async function () {
        var formatting = (await import("./formatting"));
        var locale = Object.assign({}, formatting.Culture);
        expect(formatting.localeFormat(locale, "{{{0}}}", "abc")).toBe("{abc}");
    });

    it("formats null and undefined as empty string", async function () {
        var formatting = (await import("./formatting"));
        var locale = Object.assign({}, formatting.Culture);
        expect(formatting.localeFormat(locale, "{0}", null)).toBe("");
        expect(formatting.localeFormat(locale, "x{0}y", null)).toBe("xy");
        expect(formatting.localeFormat(locale, "{0}", undefined)).toBe("");
        expect(formatting.localeFormat(locale, "x{0}y", undefined)).toBe("xy");
    });

    it("handles number format specifiers", async function () {
        var formatting = (await import("./formatting"));
        var locale = Object.assign({}, formatting.Culture);
        expect(formatting.localeFormat(locale, "{0:000}", 5)).toBe("005");
    });

    it("handles date format specifiers", async function () {
        var formatting = (await import("./formatting"));
        var locale = Object.assign({}, formatting.Culture);
        expect(formatting.localeFormat(locale, "{0:yyyy-MM-dd}", new Date(2020, 1, 1))).toBe("2020-02-01");
    });
    
    it("calls obj.localeFormat if available", async function () {
        var formatting = (await import("./formatting"));
        var locale = Object.assign({}, formatting.Culture);
        expect(formatting.localeFormat(locale, "{0:x}", {format: (s: string) => s})).toBe("x");
    });
});

describe("ScriptCulture", () => {
    it("ignores a script element with ScriptCulture ID but empty content", async () => {
        var script = document.head.appendChild(document.createElement('script'));
        try {
            script.type = "application/json";
            script.id = "ScriptCulture";
            script.innerText = "    ";
            var formatting = (await import("./formatting"));
            expect(formatting.Culture).toBeDefined();
        }
        finally {
            script.remove();
        }
    });

    it("ignores a script element with ScriptCulture ID but empty JSON content", async () => {
        var script = document.head.appendChild(document.createElement('script'));
        try {
            script.type = "application/json";
            script.id = "ScriptCulture";
            script.innerHTML = "{}";
            var formatting = (await import("./formatting"));
            expect(formatting.Culture).toBeDefined();
        }
        finally {
            script.remove();
        }
    });

    it("uses a script element with ScriptCulture ID", async () => {
        var script = document.head.appendChild(document.createElement('script'));
        try {
            script.type = "application/json";
            script.id = "ScriptCulture";
            script.innerHTML = '{ "DecimalSeparator": "&" }';
            var formatting = (await import("./formatting"));
            expect(formatting.Culture.decimalSeparator).toBe('&');
        }
        finally {
            script.remove();
        }
    });

    it("can handle when GroupSeparator is specified same with DecimalSeparator", async () => {
        var script = document.head.appendChild(document.createElement('script'));
        try {
            script.type = "application/json";
            script.id = "ScriptCulture";
            script.textContent = '{ "DecimalSeparator": "&", "GroupSeparator": "&" }';
            var formatting = (await import("./formatting"));
            expect(formatting.Culture.decimalSeparator).toBe('&');
            expect(formatting.Culture.groupSeparator).toBe(',');
        }
        finally {
            script.remove();
        }
    });

    it("can handle when GroupSeparator is specified same with Culture.DecimalSeparator", async () => {
        var script = document.head.appendChild(document.createElement('script'));
        try {
            script.type = "application/json";
            script.id = "ScriptCulture";
            script.textContent = '{ "GroupSeparator": "." }';
            var formatting = (await import("./formatting"));
            expect(formatting.Culture.decimalSeparator).toBe('.');
            expect(formatting.Culture.groupSeparator).toBe(',');
        }
        finally {
            script.remove();
        }
    });

    it("can switch GroupSeparator when DecimalSeparator is specified the same", async () => {
        var script = document.head.appendChild(document.createElement('script'));
        try {
            script.type = "application/json";
            script.id = "ScriptCulture";
            script.textContent = '{ "DecimalSeparator": "," }';
            var formatting = (await import("./formatting"));
            expect(formatting.Culture.decimalSeparator).toBe(',');
            expect(formatting.Culture.groupSeparator).toBe('.');
        }
        finally {
            script.remove();
        }
    });

    it("can set GroupSeparator", async () => {
        var script = document.head.appendChild(document.createElement('script'));
        try {
            script.type = "application/json";
            script.id = "ScriptCulture";
            script.textContent = '{ "GroupSeparator": "$" }';
            var formatting = (await import("./formatting"));
            expect(formatting.Culture.decimalSeparator).toBe('.');
            expect(formatting.Culture.groupSeparator).toBe('$');
        }
        finally {
            script.remove();
        }
    });

    it("can set other Culture members by lowercasing first letter", async () => {
        var script = document.head.appendChild(document.createElement('script'));
        try {
            script.type = "application/json";
            script.id = "ScriptCulture";
            script.textContent = '{ "DateSeparator": "_", "TimeSeparator": "?" }';
            var formatting = (await import("./formatting"));
            expect(formatting.Culture.dateSeparator).toBe('_');
            expect(formatting.Culture.timeSeparator).toBe('?');
        }
        finally {
            script.remove();
        }
    });    
});

describe("turkishLocaleToUpper", () => {
    it("ignores returns empty values as is", async () => {
        var formatting = (await import("./formatting"));
        expect(formatting.turkishLocaleToUpper("")).toBe("");
        expect(formatting.turkishLocaleToUpper(null)).toBe(null);
        expect(formatting.turkishLocaleToUpper(undefined)).toBe(undefined);
    });

    it("converts i to İ and ı to I", async () => {
        var formatting = (await import("./formatting"));
        expect(formatting.turkishLocaleToUpper("xıIiİıİia")).toBe("XIIİİIİİA");
    });
});

describe("turkishLocaleCompare", () => {
    it("is same with Culture.stringCompare", async () => {
        var formatting = (await import("./formatting"));
        expect(formatting.turkishLocaleCompare).toBe(formatting.Culture.stringCompare);
    });
});

export {}