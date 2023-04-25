import { formatNumber, Culture, formatDate, formatISODateTimeUTC } from "./formatting";

describe("Q.formatDate", () => {
    it('formats properly based on format string and culture', function () {
        var backupDec = Culture.dateSeparator;
        var backupDateFormat = Culture.dateFormat;
        var backupDateTimeFormat = Culture.dateTimeFormat;
        try {
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
        }
        finally {
            Culture.decimalSeparator = backupDec;
            Culture.dateFormat = backupDateFormat;
            Culture.dateTimeFormat = backupDateTimeFormat;
        }
    });

    it('accepts iso datetime strings as input', function () {
        var backupDec = Culture.dateSeparator;
        var backupDateFormat = Culture.dateFormat;
        var backupDateTimeFormat = Culture.dateTimeFormat;
        try {
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
        }
        finally {
            Culture.decimalSeparator = backupDec;
            Culture.dateFormat = backupDateFormat;
            Culture.dateTimeFormat = backupDateTimeFormat;
        }
    });

    it('accepts date string as input', function () {
        var backupDec = Culture.dateSeparator;
        var backupDateFormat = Culture.dateFormat;
        var backupDateTimeFormat = Culture.dateTimeFormat;
        try {
            Culture.dateSeparator = '/';
            Culture.dateFormat = 'dd/MM/yyyy';
            Culture.dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';
            expect(formatDate('2/1/2029', null)).toBe('02/01/2029');
            Culture.dateSeparator = '.';
            expect(formatDate('2/1/2029', null)).toBe('02.01.2029');
        }
        finally {
            Culture.decimalSeparator = backupDec;
            Culture.dateFormat = backupDateFormat;
            Culture.dateTimeFormat = backupDateTimeFormat;
        }
    });
});

describe("Q.formatNumber", () => {
    it('formats with "." as decimal separator', function () {
        var backupDec = Culture.decimalSeparator;
        var backupGroup = Culture.groupSeparator;
        try {
            Culture.decimalSeparator = '.';
            Culture.groupSeparator = ',';
            expect(formatNumber(1, '0.00')).toBe('1.00');
            expect(formatNumber(1, '0.0000')).toBe('1.0000');
            expect(formatNumber(1234, '#,##0')).toBe('1,234');
            expect(formatNumber(1234.5, '#,##0.##')).toBe('1,234.5');
            expect(formatNumber(1234.5678, '#,##0.##')).toBe('1,234.57');
            expect(formatNumber(1234.5, '#,##0.00')).toBe('1,234.50');
        }
        finally {
            Culture.decimalSeparator = backupDec;
            Culture.groupSeparator = backupGroup;
        }
    });

    it('formats with "," as decimal separator', function () {
        var backupDec = Culture.decimalSeparator;
        var backupGroup = Culture.groupSeparator;
        try {
            Culture.decimalSeparator = ',';
            Culture.groupSeparator = '.';
            expect(formatNumber(1, '0.00')).toBe('1,00');
            expect(formatNumber(1, '0.0000')).toBe('1,0000');
            expect(formatNumber(1234, '#,##0')).toBe('1.234');
            expect(formatNumber(1234.5, '#,##0.##')).toBe('1.234,5');
            expect(formatNumber(1234.5678, '#,##0.##')).toBe('1.234,57');
            expect(formatNumber(1234.5, '#,##0.00')).toBe('1.234,50');
        }
        finally {
            Culture.decimalSeparator = backupDec;
            Culture.groupSeparator = backupGroup;
        }
    });


    it('formats number with prefix and suffix', function () {
        var backupDec = Culture.decimalSeparator;
        var backupGroup = Culture.groupSeparator;
        try {
            Culture.decimalSeparator = '.';
            Culture.groupSeparator = ',';
            expect(formatNumber(1234.5, '$#,##0.##')).toBe('$1,234.5');
            expect(formatNumber(1234.5, 'R #,##0.##')).toBe('R 1,234.5');
            expect(formatNumber(1234.5, '#,##0.## TL')).toBe('1,234.5 TL');
            expect(formatNumber(1, "','0")).toBe(',1');
            expect(formatNumber(1, "','0','")).toBe(',1,');
            expect(formatNumber(1, "\\,0\\,")).toBe(',1,');
        }
        finally {
            Culture.decimalSeparator = backupDec;
            Culture.groupSeparator = backupGroup;
        }
    });
});
