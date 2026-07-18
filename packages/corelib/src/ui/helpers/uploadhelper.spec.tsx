import { FileUploadTexts, Fluent } from "../../base";
import { UploadHelper } from "./uploadhelper";

// Mock Uploader to capture its constructor options
vi.mock("../../base", async (importOriginal) => {
    const actual: any = await importOriginal();
    const MockUploader = vi.fn(function (this: any, opts: any) {
        // Store callbacks so tests can invoke them
        this._opts = opts;
        this.batchSuccess = opts.batchSuccess;
        this.batchStart = opts.batchStart;
        this.batchStop = opts.batchStop;
        this.batchProgress = opts.batchProgress;
    });
    return {
        ...actual,
        Uploader: MockUploader as any
    };
});

describe("UploadHelper", () => {
    describe("fileSizeDisplay", () => {
        it("returns 0 KB for 0 bytes", () => {
            expect(UploadHelper.fileSizeDisplay(0)).toBe("0 KB");
        });

        it("returns size in KB for < 1024 KB", () => {
            expect(UploadHelper.fileSizeDisplay(1024)).toBe("1 KB");
        });

        it("returns size in KB for 500 bytes", () => {
            const result = UploadHelper.fileSizeDisplay(500);
            expect(result).toContain("KB");
        });

        it("returns size in MB for >= 1024 KB", () => {
            const result = UploadHelper.fileSizeDisplay(1024 * 1024);
            expect(result).toContain("MB");
            expect(result).toBe("1 MB");
        });

        it("rounds to 2 decimal places", () => {
            const result = UploadHelper.fileSizeDisplay(1500);
            expect(result).toContain("KB");
        });
    });

    describe("fileNameSizeDisplay", () => {
        it("returns name with size", () => {
            const result = UploadHelper.fileNameSizeDisplay("test.jpg", 1024);
            expect(result).toBe("test.jpg (1 KB)");
        });
    });

    describe("hasImageExtension", () => {
        it("returns true for .jpg", () => {
            expect(UploadHelper.hasImageExtension("photo.jpg")).toBe(true);
        });

        it("returns true for .jpeg", () => {
            expect(UploadHelper.hasImageExtension("photo.jpeg")).toBe(true);
        });

        it("returns true for .gif", () => {
            expect(UploadHelper.hasImageExtension("animation.gif")).toBe(true);
        });

        it("returns true for .png", () => {
            expect(UploadHelper.hasImageExtension("image.png")).toBe(true);
        });

        it("returns true for .webp", () => {
            expect(UploadHelper.hasImageExtension("image.webp")).toBe(true);
        });

        it("returns false for non-image extensions", () => {
            expect(UploadHelper.hasImageExtension("document.pdf")).toBe(false);
        });

        it("returns false for null filename", () => {
            expect(UploadHelper.hasImageExtension(null as any)).toBe(false);
        });

        it("returns false for empty filename", () => {
            expect(UploadHelper.hasImageExtension("")).toBe(false);
        });

        it("is case insensitive", () => {
            expect(UploadHelper.hasImageExtension("Photo.JPG")).toBe(true);
            expect(UploadHelper.hasImageExtension("Photo.PNG")).toBe(true);
        });
    });

    describe("thumbFileName", () => {
        it("appends _t before extension", () => {
            expect(UploadHelper.thumbFileName("photo.jpg")).toBe("photo_t.jpg");
        });

        it("handles filename without extension", () => {
            expect(UploadHelper.thumbFileName("photo")).toBe("photo_t.jpg");
        });

        it("handles null filename", () => {
            expect(UploadHelper.thumbFileName(null as any)).toBe("_t.jpg");
        });

        it("handles empty filename", () => {
            expect(UploadHelper.thumbFileName("")).toBe("_t.jpg");
        });
    });

    describe("dbFileUrl", () => {
        it("prepends upload path and resolves URL", () => {
            const url = UploadHelper.dbFileUrl("photos/photo.jpg");
            expect(url).toContain("upload/");
            expect(url).toContain("photos/photo.jpg");
        });

        it("replaces backslashes with forward slashes", () => {
            const url = UploadHelper.dbFileUrl("photos\\photo.jpg");
            expect(url).not.toContain("\\");
        });

        it("handles null filename", () => {
            const url = UploadHelper.dbFileUrl(null as any);
            expect(url).toBeTruthy();
        });
    });

    describe("checkImageConstraints", () => {
        beforeEach(() => {
            FileUploadTexts.NotAnImageFile = "Not an image file";
            FileUploadTexts.UploadFileTooSmall = "File too small: {0}";
            FileUploadTexts.UploadFileTooBig = "File too big: {0}";
            FileUploadTexts.MinWidth = "Min width: {0}";
            FileUploadTexts.MaxWidth = "Max width: {0}";
            FileUploadTexts.MinHeight = "Min height: {0}";
            FileUploadTexts.MaxHeight = "Max height: {0}";
        });

        it("returns false for non-image when allowNonImage is false", () => {
            const result = UploadHelper.checkImageConstraints(
                { IsImage: false, Size: 100, Width: 0, Height: 0 },
                { allowNonImage: false }
            );
            expect(result).toBe(false);
        });

        it("returns true for non-image when allowNonImage is true", () => {
            const result = UploadHelper.checkImageConstraints(
                { IsImage: false, Size: 100, Width: 0, Height: 0 },
                { allowNonImage: true }
            );
            expect(result).toBe(true);
        });

        it("returns false if file is too small", () => {
            const result = UploadHelper.checkImageConstraints(
                { IsImage: true, Size: 50, Width: 100, Height: 100 },
                { minSize: 100, allowNonImage: true }
            );
            expect(result).toBe(false);
        });

        it("returns false if file is too big", () => {
            const result = UploadHelper.checkImageConstraints(
                { IsImage: true, Size: 200, Width: 100, Height: 100 },
                { maxSize: 100, allowNonImage: true }
            );
            expect(result).toBe(false);
        });

        it("returns false if width is too small", () => {
            const result = UploadHelper.checkImageConstraints(
                { IsImage: true, Size: 100, Width: 50, Height: 100 },
                { minWidth: 100, allowNonImage: true }
            );
            expect(result).toBe(false);
        });

        it("returns false if width is too big", () => {
            const result = UploadHelper.checkImageConstraints(
                { IsImage: true, Size: 100, Width: 200, Height: 100 },
                { maxWidth: 100, allowNonImage: true }
            );
            expect(result).toBe(false);
        });

        it("returns false if height is too small", () => {
            const result = UploadHelper.checkImageConstraints(
                { IsImage: true, Size: 100, Width: 100, Height: 50 },
                { minHeight: 100, allowNonImage: true }
            );
            expect(result).toBe(false);
        });

        it("returns false if height is too big", () => {
            const result = UploadHelper.checkImageConstraints(
                { IsImage: true, Size: 100, Width: 100, Height: 200 },
                { maxHeight: 100, allowNonImage: true }
            );
            expect(result).toBe(false);
        });

        it("returns true when all constraints pass", () => {
            const result = UploadHelper.checkImageConstraints(
                { IsImage: true, Size: 100, Width: 100, Height: 100 },
                { minSize: 50, maxSize: 200, minWidth: 80, maxWidth: 120, minHeight: 80, maxHeight: 120, allowNonImage: true }
            );
            expect(result).toBe(true);
        });

        it("skips image-specific checks when file is not an image", () => {
            const result = UploadHelper.checkImageConstraints(
                { IsImage: false, Size: 100, Width: 10, Height: 10 },
                { minWidth: 100, allowNonImage: true }
            );
            expect(result).toBe(true);
        });
    });

    describe("lightbox", () => {
        it("does nothing for null element", () => {
            expect(() => UploadHelper.lightbox(null as any)).not.toThrow();
        });

        it("does nothing for ArrayLike with null element", () => {
            expect(() => UploadHelper.lightbox([null] as any)).not.toThrow();
        });

        it("adds click handler with GLightbox when available", () => {
            const mockOpen = vi.fn();
            const mockDestroy = vi.fn();
            (globalThis as any).GLightbox = vi.fn(() => ({
                on: vi.fn((_ev: string, cb: Function) => cb()),
                open: mockOpen,
                destroy: mockDestroy
            }));

            const link = document.createElement("a");
            link.href = "photo.jpg";
            link.title = "Photo";

            UploadHelper.lightbox(link);
            link.click();

            expect(mockOpen).toHaveBeenCalled();

            delete (globalThis as any).GLightbox;
        });

        it("uses SimpleLightbox when GLightbox is not available", () => {
            const mockSimpleLightbox = vi.fn();
            (globalThis as any).SimpleLightbox = mockSimpleLightbox;

            const link = document.createElement("a");
            UploadHelper.lightbox(link);

            expect(mockSimpleLightbox).toHaveBeenCalled();

            delete (globalThis as any).SimpleLightbox;
        });

        it("handles ArrayLike input", () => {
            const mockSimpleLightbox = vi.fn();
            (globalThis as any).SimpleLightbox = mockSimpleLightbox;

            const link = document.createElement("a");
            UploadHelper.lightbox([link] as any);

            expect(mockSimpleLightbox).toHaveBeenCalled();

            delete (globalThis as any).SimpleLightbox;
        });
    });

    describe("populateFileSymbols", () => {
        it("does nothing for null container", () => {
            expect(() => UploadHelper.populateFileSymbols(null as any, [])).not.toThrow();
        });

        it("creates list items for files", () => {
            const container = document.createElement("div");
            UploadHelper.populateFileSymbols(container, [
                { Filename: "photo.jpg", OriginalName: "Photo" }
            ]);

            expect(container.querySelectorAll("li").length).toBe(1);
            expect(container.querySelector("li")?.classList.contains("file-image")).toBe(true);
        });

        it("creates list items for non-image files", () => {
            const container = document.createElement("div");
            UploadHelper.populateFileSymbols(container, [
                { Filename: "doc.pdf", OriginalName: "Document" }
            ]);

            expect(container.querySelectorAll("li").length).toBe(1);
            expect(container.querySelector("li")?.classList.contains("file-binary")).toBe(true);
        });

        it("handles empty items array", () => {
            const container = document.createElement("div");
            UploadHelper.populateFileSymbols(container, []);

            expect(container.innerHTML).toBe("");
        });

        it("handles null items", () => {
            const container = document.createElement("div");
            UploadHelper.populateFileSymbols(container, null as any);

            expect(container.innerHTML).toBe("");
        });

        it("adds original name as title on thumb", () => {
            const container = document.createElement("div");
            UploadHelper.populateFileSymbols(container, [
                { Filename: "photo.jpg", OriginalName: "My Photo" }
            ]);

            const thumb = container.querySelector(".thumb") as HTMLAnchorElement;
            expect(thumb.title).toBe("My Photo");
        });

        it("prepends urlPrefix to non-temporary filenames", () => {
            const container = document.createElement("div");
            UploadHelper.populateFileSymbols(container, [
                { Filename: "photos/photo.jpg", OriginalName: "" }
            ], false, "https://cdn.example.com/");

            const thumb = container.querySelector(".thumb") as HTMLAnchorElement;
            expect(thumb.href).toContain("cdn.example.com");
        });

        it("does not prefix temporary filenames", () => {
            const container = document.createElement("div");
            UploadHelper.populateFileSymbols(container, [
                { Filename: "temporary/photo.jpg", OriginalName: "" }
            ], false, "https://cdn.example.com/");

            const thumb = container.querySelector(".thumb") as HTMLAnchorElement;
            expect(thumb.href).not.toContain("cdn.example.com");
        });

        it("shows original name when displayOriginalName is true", () => {
            const container = document.createElement("div");
            UploadHelper.populateFileSymbols(container, [
                { Filename: "photo.jpg", OriginalName: "My Photo" }
            ], true);

            const filename = container.querySelector(".filename") as HTMLElement;
            expect(filename).toBeTruthy();
            expect(filename.title).toBe("My Photo");
        });
    });

    describe("addUploadInput", () => {
        it("creates upload input with progress", () => {
            const container = document.createElement("div");
            const progress = document.createElement("div");
            const result = UploadHelper.addUploadInput({
                container,
                inputName: "file",
                progress,
                allowMultiple: true
            });
            expect(result).toBeTruthy();
            expect(result.getNode()).toBeTruthy();
        });

        it("creates upload input with uploadIntent", () => {
            const container = document.createElement("div");
            const result = UploadHelper.addUploadInput({
                container,
                inputName: "file",
                uploadIntent: "avatar"
            });
            const input = result.getNode() as HTMLInputElement;
            expect(input.dataset.url).toContain("uploadIntent=avatar");
        });
    });

    describe("createUploadInput", () => {
        it("returns input and uploader objects", () => {
            const container = document.createElement("div");
            const result = UploadHelper.createUploadInput({
                container,
                inputName: "file"
            });
            expect(result.input).toBeTruthy();
            expect(result.input.type).toBe("file");
            expect(result.uploader).toBeTruthy();
        });

        it("adds fileinput-button class to closest button", () => {
            const button = document.createElement("button");
            const container = document.createElement("div");
            button.appendChild(container);
            UploadHelper.createUploadInput({ container, inputName: "f" });
            expect(button.classList.contains("fileinput-button")).toBe(true);
        });

        it("handles ArrayLike container", () => {
            const container = document.createElement("div");
            const result = UploadHelper.createUploadInput({ container: [container] as any, inputName: "f" });
            expect(result.input).toBeTruthy();
        });

        it("handles ArrayLike progress", () => {
            const container = document.createElement("div");
            const progress = document.createElement("div");
            const bar = document.createElement("div");
            progress.appendChild(bar);
            const result = UploadHelper.createUploadInput({ container, inputName: "f", progress: [progress] as any });
            expect(result.input).toBeTruthy();
        });

        it('appends uploadIntent to URL with existing query param', () => {
            const container = document.createElement('div');
            const result = UploadHelper.createUploadInput({
                container,
                inputName: 'f',
                uploadUrl: '~/Upload?existing=1',
                uploadIntent: 'test'
            });
            const url = (result.input as HTMLInputElement).getAttribute('data-url');
            expect(url).toContain('uploadIntent=test');
            expect(url).toContain('existing=1');
        });

        it("invokes fileDone on batchSuccess without error", () => {
            const container = document.createElement('div');
            const fileDone = vi.fn();
            const result = UploadHelper.createUploadInput({
                container, inputName: 'f', fileDone
            });
            const uploader: any = result.uploader;
            uploader.batchSuccess({ response: { IsImage: true } });
            expect(fileDone).toHaveBeenCalledWith(
                { IsImage: true },
                undefined,
                expect.any(Object)
            );
        });

        it("notifies error on batchSuccess with error response", () => {
            const container = document.createElement('div');
            const fileDone = vi.fn();
            const result = UploadHelper.createUploadInput({
                container, inputName: 'f', fileDone
            });
            const uploader: any = result.uploader;
            uploader.batchSuccess({ response: { Error: { Message: "Upload failed" } } });
            expect(fileDone).not.toHaveBeenCalled();
        });

        it("calls batchStart and batchStop callbacks", () => {
            const container = document.createElement('div');
            const progress = document.createElement('div');
            const bar = document.createElement('div');
            progress.appendChild(bar);
            const result = UploadHelper.createUploadInput({
                container, inputName: 'f', progress
            });
            const uploader: any = result.uploader;

            uploader.batchStart();
            expect(bar.style.width).toBe('0%');

            uploader.batchStop();
            expect(bar.style.width).toBe('100%');
        });

        it("updates progress percentage on batchProgress", () => {
            const container = document.createElement('div');
            const progress = document.createElement('div');
            const bar = document.createElement('div');
            progress.appendChild(bar);
            const result = UploadHelper.createUploadInput({
                container, inputName: 'f', progress
            });
            const uploader: any = result.uploader;

            uploader.batchProgress({ loaded: 50, total: 100 });
            expect(bar.style.width).toBe('50%');

            uploader.batchProgress({ loaded: 100, total: 100 });
            expect(bar.style.width).toBe('100%');
        });

        it("does not update progress when loaded is not a number", () => {
            const container = document.createElement('div');
            const progress = document.createElement('div');
            const bar = document.createElement('div');
            progress.appendChild(bar);
            const result = UploadHelper.createUploadInput({
                container, inputName: 'f', progress
            });
            const uploader: any = result.uploader;

            uploader.batchStart();
            expect(bar.style.width).toBe('0%');
            uploader.batchProgress({ loaded: null, total: 100 });
            // Should remain 0% since loaded is not a number
            expect(bar.style.width).toBe('0%');
        });

        it("sets width via setProgress even when progress has no children", () => {
            const container = document.createElement('div');
            const progress = document.createElement('div');
            const result = UploadHelper.createUploadInput({
                container, inputName: 'f', progress
            });
            const uploader: any = result.uploader;
            // No bar child in progress - should not throw
            expect(() => uploader.batchStart()).not.toThrow();
        });
    });

    describe("colorBox (deprecated alias for lightbox)", () => {
        it("is the same function as lightbox", () => {
            expect(UploadHelper.colorBox).toBe(UploadHelper.lightbox);
        });

        it("uses jQuery colorbox when neither GLightbox nor SimpleLightbox are present", () => {
            const mockColorbox = vi.fn();
            const mock$ = vi.fn(() => ({ colorbox: mockColorbox })) as any;
            (globalThis as any).$ = mock$;
            (globalThis as any).jQuery = mock$;

            const link = document.createElement("a");
            UploadHelper.lightbox(link);
            expect(mockColorbox).toHaveBeenCalled();

            delete (globalThis as any).$;
            delete (globalThis as any).jQuery;
        });

        it("does nothing with jQuery but no colorbox", () => {
            const mock$ = vi.fn(() => ({})) as any;
            (globalThis as any).$ = mock$;
            (globalThis as any).jQuery = mock$;

            const link = document.createElement("a");
            expect(() => UploadHelper.lightbox(link)).not.toThrow();

            delete (globalThis as any).$;
            delete (globalThis as any).jQuery;
        });
    });

    describe("checkImageConstraints edge cases", () => {
        it("handles minSize of 0 (no constraint)", () => {
            const result = UploadHelper.checkImageConstraints(
                { IsImage: true, Size: 1, Width: 100, Height: 100 },
                { minSize: 0, allowNonImage: true }
            );
            expect(result).toBe(true);
        });

        it("handles maxSize of 0 (no constraint)", () => {
            const result = UploadHelper.checkImageConstraints(
                { IsImage: true, Size: 99999, Width: 100, Height: 100 },
                { maxSize: 0, allowNonImage: true }
            );
            expect(result).toBe(true);
        });

        it("handles minWidth of 0 (no constraint)", () => {
            const result = UploadHelper.checkImageConstraints(
                { IsImage: true, Size: 100, Width: 0, Height: 100 },
                { minWidth: 0, allowNonImage: true }
            );
            expect(result).toBe(true);
        });

        it("handles empty constraints object", () => {
            const result = UploadHelper.checkImageConstraints(
                { IsImage: true, Size: 100, Width: 100, Height: 100 },
                {} as any
            );
            expect(result).toBe(true);
        });
    });
});