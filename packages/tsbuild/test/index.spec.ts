import { vi, describe, it, expect, beforeEach } from 'vitest';
import { safeGlobSync } from "../dist/index.js";

vi.mock('glob', () => ({
    globSync: vi.fn()
}));

import { globSync } from 'glob';

describe("safeGlobSync", () => {
    beforeEach(() => {
        vi.clearAllMocks();
    });

    it("should return matching files for simple glob", () => {
        (globSync as any).mockReturnValue(['file1.txt', 'file2.txt']);
        const files = safeGlobSync(["**/*.txt"]);
        expect(globSync).toHaveBeenCalledWith(['**/*.txt'], expect.objectContaining({
            ignore: [],
            nodir: true,
            matchBase: true,
            cwd: process.cwd()
        }));
        expect(files).toEqual(['file1.txt', 'file2.txt']);
    });

    it("should normalize patterns without slashes to **/pattern", () => {
        (globSync as any).mockReturnValue([]);
        safeGlobSync(["*.txt"]);
        expect(globSync).toHaveBeenCalledWith(['**/*.txt'], expect.any(Object));
    });

    it("should anchor patterns starting with / to ./", () => {
        (globSync as any).mockReturnValue([]);
        safeGlobSync(["/dir/*.txt"]);
        expect(globSync).toHaveBeenCalledWith(['./dir/*.txt'], expect.any(Object));
    });

    it("should handle excludes", () => {
        (globSync as any).mockReturnValue([]);
        safeGlobSync(["**/*.txt", "!*.log"]);
        expect(globSync).toHaveBeenCalledWith(['**/*.txt'], expect.objectContaining({
            ignore: ['**/*.log']
        }));
    });

    it("should handle anchored excludes", () => {
        (globSync as any).mockReturnValue([]);
        safeGlobSync(["**/*.txt", "!/logs/*.log"]);
        expect(globSync).toHaveBeenCalledWith(['**/*.txt'], expect.objectContaining({
            ignore: ['./logs/*.log']
        }));
    });

    it("should reject invalid patterns", () => {
        expect(() => safeGlobSync(["../outside"])).toThrow("Invalid pattern");
        expect(() => safeGlobSync(["C:\\absolute"])).toThrow("Invalid pattern");
    });

    it("should filter results under rootdir", () => {
        (globSync as any).mockReturnValue(['safe/file.txt', '../outside.txt']);
        const files = safeGlobSync(["**/*.txt"], { rootdir: '/root' } as any);
        expect(files).toEqual(['safe/file.txt']);
    });
});
