var $ = require("jQuery");
global.$ = global.jQuery = $;
require("SerenityCoreLibBase");

test('FormatDateWorks', function() {
    var backupDec = Q.Culture.dateSeparator;
    var backupDateFormat = Q.Culture.dateFormat;
    var backupDateTimeFormat = Q.Culture.dateTimeFormat;
    try {
        Q.Culture.dateSeparator = '/';
        Q.Culture.dateFormat = 'dd/MM/yyyy';
        Q.Culture.dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';
        var date = new Date(2029, 0, 2, 3, 4, 5, 6);
        // 02.01.2029 03:04:05.006
        expect(Q.formatDate(date, 'dd/MM/yyyy')).toBe('02/01/2029', "'/': dd/MM/yyy");
        expect(Q.formatDate(date, 'd/M/yy')).toBe('2/1/29', "'/': d/M/yy");
        expect(Q.formatDate(date, 'd.M.yyyy')).toBe('2.1.2029', "'/': d.M.yyy");
        expect(Q.formatDate(date, 'yyyyMMdd')).toBe('20290102', "'/': yyyyMMdd");
        expect(Q.formatDate(date, 'hh:mm tt')).toBe('03:04 AM', "'/': hh:mm tt");
        expect(Q.formatDate(date, 'yyyy-MM-ddTHH:mm:ss.fff')).toBe('2029-01-02T03:04:05.006', "'/': yyyy-MM-ddTHH:mm:ss.fff");
        expect(Q.formatDate(date, 'd')).toBe('02/01/2029', "'/': d");
        expect(Q.formatDate(date, 'g')).toBe('02/01/2029 03:04', "'/': g");
        expect(Q.formatDate(date, 'G')).toBe('02/01/2029 03:04:05', "'/': G");
        expect(Q.formatDate(date, 's')).toBe('2029-01-02T03:04:05', "'/': s");
        expect(Q.formatDate(date, 'u')).toBe(Q.formatISODateTimeUTC(date), "'/': u");
        Q.Culture.dateSeparator = '.';
        expect(Q.formatDate(date, 'dd/MM/yyyy')).toBe('02.01.2029', "'.': dd/MM/yyy");
        expect(Q.formatDate(date, 'd/M/yy')).toBe('2.1.29', "'.': d/M/yy");
        expect(Q.formatDate(date, 'd-M-yyyy')).toBe('2-1-2029', "'.': d-M-yyy");
        expect(Q.formatDate(date, 'yyyy-MM-ddTHH:mm:ss.fff')).toBe('2029-01-02T03:04:05.006', "'.': yyyy-MM-ddTHH:mm:ss.fff");
        expect(Q.formatDate(date, 'g')).toBe('02.01.2029 03:04', "'.': g");
        expect(Q.formatDate(date, 'G')).toBe('02.01.2029 03:04:05', "'.': G");
        expect(Q.formatDate(date, 's')).toBe('2029-01-02T03:04:05', "'.': s");
        expect(Q.formatDate(date, 'u')).toBe(Q.formatISODateTimeUTC(date), "'.': u");
    }
    finally {
        Q.Culture.decimalSeparator = backupDec;
        Q.Culture.dateFormat = backupDateFormat;
        Q.Culture.dateTimeFormat = backupDateTimeFormat;
    }
});
test('FormatDateWorksWithISOString', function() {
    var backupDec = Q.Culture.dateSeparator;
    var backupDateFormat = Q.Culture.dateFormat;
    var backupDateTimeFormat = Q.Culture.dateTimeFormat;
    try {
        Q.Culture.dateSeparator = '/';
        Q.Culture.dateFormat = 'dd/MM/yyyy';
        Q.Culture.dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';
        expect(Q.formatDate('2029-01-02', null)).toBe('02/01/2029'); // date only, empty format
        expect(Q.formatDate('2029-01-02T16:35:24', null)).toBe('02/01/2029'); // date with time, empty format
        expect(Q.formatDate('2029-01-02T16:35:24', 'g')).toBe('02/01/2029 16:35'); // date with time, g format
        Q.Culture.dateSeparator = '.';
        expect(Q.formatDate('2029-01-02', null)).toBe('02.01.2029'); //date only, empty format
        expect(Q.formatDate('2029-01-02T16:35:24', null)).toBe('02.01.2029') // date with time, empty format
        expect(Q.formatDate('2029-01-02T16:35:24', 'g')).toBe('02.01.2029 16:35'); // date with time, g format
    }
    finally {
        Q.Culture.decimalSeparator = backupDec;
        Q.Culture.dateFormat = backupDateFormat;
        Q.Culture.dateTimeFormat = backupDateTimeFormat;
    }
});
test('FormatDateWorksWithDateString', function() {
    var backupDec = Q.Culture.dateSeparator;
    var backupDateFormat = Q.Culture.dateFormat;
    var backupDateTimeFormat = Q.Culture.dateTimeFormat;
    try {
        Q.Culture.dateSeparator = '/';
        Q.Culture.dateFormat = 'dd/MM/yyyy';
        Q.Culture.dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';
        expect(Q.formatDate('2/1/2029', null)).toBe('02/01/2029'); // date only, empty format
        Q.Culture.dateSeparator = '.';
        expect(Q.formatDate('2/1/2029', null)).toBe('02.01.2029'); // date only, empty format
    }
    finally {
        Q.Culture.decimalSeparator = backupDec;
        Q.Culture.dateFormat = backupDateFormat;
        Q.Culture.dateTimeFormat = backupDateTimeFormat;
    }
});