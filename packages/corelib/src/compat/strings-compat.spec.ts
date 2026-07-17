import * as deprecations from "./strings-compat";
import { isEmptyOrNull, isTrimmedEmpty, padLeft, replaceAll, toSingleLine, trim, trimEnd, trimStart, trimToEmpty, zeroPad } from "./strings-compat";

describe("endsWith", () => {
    it("uses String.prototype.endsWith", () => {
        const endsWithSpy = vi.spyOn(String.prototype, "endsWith");
        expect((deprecations as any).endsWith("abc", "c")).toBe(true);
        expect(endsWithSpy).toHaveBeenCalled();
    });
});

describe("isEmptyOrNull", () => {
    it("returns true for null, undefined and empty string", () => {
        expect(isEmptyOrNull(null)).toBe(true);
        expect(isEmptyOrNull(undefined)).toBe(true);
        expect(isEmptyOrNull("")).toBe(true);
    });

    it("returns false for non-empty string", () => {
        expect(isEmptyOrNull(" ")).toBe(false);
        expect(isEmptyOrNull("abc")).toBe(false);
    });
});

describe("isTrimmedEmpty", () => {
    it("returns true for null, undefined and empty string", () => {
        expect(isTrimmedEmpty(null)).toBe(true);
        expect(isTrimmedEmpty(undefined)).toBe(true);
        expect(isTrimmedEmpty("")).toBe(true);
        expect(isTrimmedEmpty(" ")).toBe(true);
        expect(isTrimmedEmpty("\t")).toBe(true);
        expect(isTrimmedEmpty("\n")).toBe(true);
        expect(isTrimmedEmpty(" \t \n ")).toBe(true);
    });

    it("returns false for non-empty string", () => {
        expect(isTrimmedEmpty("abc")).toBe(false);
        expect(isTrimmedEmpty(" abc ")).toBe(false);
        expect(isTrimmedEmpty("abc ")).toBe(false);
    });
});

describe("padLeft", () => {
    it("uses String.prototype.padStart if available", () => {
        if (!(String.prototype as any).padStart) {
            expect(true).toBe(true);
            return;
        }

        const padStartSpy = vi.spyOn(String.prototype as any, "padStart");
        expect(padLeft("abc", 5)).toBe("  abc");
        expect(padStartSpy).toHaveBeenCalled();

        expect(padLeft("abc", 5, " ")).toBe("  abc");
        expect(padLeft("abc", 5)).toBe("  abc");
        expect(padLeft("x", 5, "1")).toBe("1111x");
        expect(padLeft("xyz", 4, "0")).toBe("0xyz");

        expect(padLeft(null, 2)).toBe("  ");
        expect(padLeft(undefined, 2)).toBe("  ");
    });

    it("pads string with given char when String.prototype.padStart not available", () => {
        const oldPadStart = (String.prototype as any).padStart;
        (String.prototype as any).padStart = null;
        try {
            expect(padLeft("abc", 5, " ")).toBe("  abc");
            expect(padLeft("abc", 5)).toBe("  abc");
            expect(padLeft("x", 5, "1")).toBe("1111x");
            expect(padLeft("xyz", 4, "0")).toBe("0xyz");
        } finally {
            (String.prototype as any).padStart = oldPadStart;
        }
    });

});

describe("replaceAll", () => {
    it("uses String.prototype.replaceAll if available", () => {
        if (!(String.prototype as any).replaceAll) {
            expect(true).toBe(true);
            return;
        }

        const replaceAllSpy = vi.spyOn(String.prototype as any, "replaceAll");
        expect(replaceAll("abc", "a", "b")).toBe("bbc");
        expect(replaceAllSpy).toHaveBeenCalled();
    });

    it("replaces all occurrences of search string", () => {
        expect(replaceAll("abc", "a", "b")).toBe("bbc");
        expect(replaceAll("abc", "b", "a")).toBe("aac");
        expect(replaceAll("abc", "c", "")).toBe("ab");
    });

    it("can replace all occurrences of search string if String.prototype.replaceAll not available", () => {
        const oldReplaceAll = (String.prototype as any).replaceAll;
        (String.prototype as any).replaceAll = null;
        try {
            expect(replaceAll("abc", "a", "b")).toBe("bbc");
            expect(replaceAll("abc", "b", "a")).toBe("aac");
            expect(replaceAll("abc", "c", "")).toBe("ab");
        } finally {
            (String.prototype as any).replaceAll = oldReplaceAll;
        }
    });
});

describe("startsWith", () => {
    it("uses String.prototype.startsWith", () => {
        const startsWithSpy = vi.spyOn(String.prototype, "startsWith");
        expect((deprecations as any).startsWith("abc", "a")).toBe(true);
        expect(startsWithSpy).toHaveBeenCalled();
    });
});

describe("toSingleLine", () => {
    it("returns empty string for null and undefined", () => {
        expect(toSingleLine(null)).toBe('');
        expect(toSingleLine(undefined)).toBe('');
    });

    it("replaces line endings with space", () => {
        expect(toSingleLine("abc\ndef")).toBe("abc def");
        expect(toSingleLine("abc\r\ndef")).toBe("abc def");
    });

    it("trims all whitespace and line endings before and after the string", () => {
        expect(toSingleLine(" abc ")).toBe("abc");
        expect(toSingleLine(" abc\n ")).toBe("abc");
        expect(toSingleLine("\n abc\r\n ")).toBe("abc");
    });
});

describe("trim", () => {
    it("uses String.prototype.trim", () => {
        const trimSpy = vi.spyOn(String.prototype, "trim");
        expect(trim(" abc ")).toBe("abc");
        expect(trimSpy).toHaveBeenCalled();
    });

    it("returns undefined for null and undefined", () => {
        expect(trim(null)).toBeUndefined();
        expect(trim(undefined)).toBeUndefined();
    });
});

describe("trimEnd", () => {
    it("uses String.prototype.trimEnd", () => {
        if ((String.prototype as any).trimEnd) {
            const trimEndSpy = vi.spyOn(String.prototype as any, "trimEnd");
            expect(trimEnd(" abc ")).toBe(" abc");
            expect(trimEndSpy).toHaveBeenCalled();
        }
    });

    it("returns empty string for null and undefined", () => {
        expect(trimEnd(null)).toBe('');
        expect(trimEnd(undefined)).toBe('');
    });

    it("trims whitespace and line endings", () => {
        expect(trimEnd(" abc\n\t\r\n ")).toBe(" abc");
    });
});

describe("trimStart", () => {
    it("uses String.prototype.trimStart", () => {
        if ((String.prototype as any).trimStart) {
            const trimStartSpy = vi.spyOn(String.prototype as any, "trimStart");
            expect(trimStart(" abc ")).toBe("abc ");
            expect(trimStartSpy).toHaveBeenCalled();
        }
    });

    it("returns empty string for null and undefined", () => {
        expect(trimStart(null)).toBe('');
        expect(trimStart(undefined)).toBe('');
    });

});

describe("trimToEmpty", () => {
    it("uses String.prototype.trim", () => {
        const trimSpy = vi.spyOn(String.prototype, "trim");
        expect(trimToEmpty(null)).toBe('');
        expect(trimToEmpty(undefined)).toBe('');
        expect(trimToEmpty(" abc ")).toBe("abc");
        expect(trimSpy).toHaveBeenCalled();
    });
});

describe("trimToNull", () => {
    it("returns null for null and undefined", async () => {
        const { trimToNull } = await import('./strings-compat');
        expect(trimToNull(null)).toBeNull();
        expect(trimToNull(undefined)).toBeNull();
    });

    it("returns null for empty or whitespace strings", async () => {
        const { trimToNull } = await import('./strings-compat');
        expect(trimToNull("")).toBeNull();
        expect(trimToNull("   ")).toBeNull();
        expect(trimToNull("\t\n")).toBeNull();
    });

    it("returns trimmed string for non-empty strings", async () => {
        const { trimToNull } = await import('./strings-compat');
        expect(trimToNull(" abc ")).toBe("abc");
        expect(trimToNull("hello")).toBe("hello");
        expect(trimToNull("  world  ")).toBe("world");
    });
});

describe("replaceAll with null/undefined string", () => {
    it("handles null and undefined string argument", async () => {
        const { replaceAll } = await import('./strings-compat');
        expect(replaceAll(null as any, "a", "b")).toBe("");
        expect(replaceAll(undefined as any, "a", "b")).toBe("");
    });
});

describe("trimEnd fallback", () => {
    it("uses fallback when String.prototype.trimEnd is not available", async () => {
        const { trimEnd } = await import('./strings-compat');
        const oldTrimEnd = (String.prototype as any).trimEnd;
        (String.prototype as any).trimEnd = undefined;
        try {
            expect(trimEnd(" hello ")).toBe(" hello");
            expect(trimEnd("test\n")).toBe("test");
            expect(trimEnd("abc  ")).toBe("abc");
        } finally {
            (String.prototype as any).trimEnd = oldTrimEnd;
        }
    });
});

describe("trimStart with actual values", () => {
    it("trims start of string properly", async () => {
        const { trimStart } = await import('./strings-compat');
        expect(trimStart(" hello ")).toBe("hello ");
        expect(trimStart("\n\t test")).toBe("test");
    });

    it("uses fallback when String.prototype.trimStart is not available", async () => {
        const { trimStart } = await import('./strings-compat');
        const oldTrimStart = (String.prototype as any).trimStart;
        (String.prototype as any).trimStart = undefined;
        try {
            expect(trimStart(" hello ")).toBe("hello ");
            expect(trimStart("  abc")).toBe("abc");
        } finally {
            (String.prototype as any).trimStart = oldTrimStart;
        }
    });
});

describe("zeroPad", () => {
    it("returns empty string for null and undefined", () => {
        expect(zeroPad(null, 2)).toBe('');
        expect(zeroPad(undefined, 2)).toBe('');
    });

    it("pads the number with zeros if less than given number", () => {
        expect(zeroPad(1, 2)).toBe("01");
        expect(zeroPad(12, 2)).toBe("12");
        expect(zeroPad(123, 2)).toBe("123");
    });
});