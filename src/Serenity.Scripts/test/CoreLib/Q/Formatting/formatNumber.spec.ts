import { formatNumber, Culture } from "@Q/Formatting";

test('format number with "." as decimal separator', function() {
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

test('format number with "," as decimal separator', function() {
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


test('format number with prefix and suffix', function() {
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