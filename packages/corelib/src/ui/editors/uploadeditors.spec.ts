import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { Fluent } from "../../base";
import { UploadHelper, UploadInputOptions } from "../helpers/uploadhelper";
import {
    FileUploadEditor,
    ImageUploadEditor,
    MultipleFileUploadEditor,
    MultipleImageUploadEditor
} from "./uploadeditors";

describe("FileUploadEditor", () => {
    let capturedOptions: UploadInputOptions[];

    beforeEach(() => {
        capturedOptions = [];
        vi.spyOn(UploadHelper, "addUploadInput").mockImplementation((options) => {
            capturedOptions.push(options);
            return Fluent(document.createElement("input"));
        });
        vi.spyOn(UploadHelper, "checkImageConstraints").mockReturnValue(true);
    });

    afterEach(() => {
        vi.restoreAllMocks();
        document.body.innerHTML = "";
    });

    function create(options: any = {}): FileUploadEditor<any> {
        return new FileUploadEditor({
            element: el => document.body.appendChild(el),
            ...options
        } as any);
    }

    it("defaults allowNonImage and sets up the DOM", () => {
        const editor = create();
        expect(editor["options"]?.allowNonImage).toBe(true);
        expect(editor.domNode.classList.contains("s-FileUploadEditor")).toBe(true);
        expect(editor.domNode.classList.contains("hide-original-name")).toBe(true);
        expect(editor.domNode.querySelector(".add-file-button")).toBeTruthy();
        expect(editor.domNode.querySelector(".delete-button")).toBeTruthy();
        expect(editor.domNode.querySelector(".upload-progress")).toBeTruthy();
        expect(editor.domNode.querySelector("ul.file-items")).toBeTruthy();
        expect(editor.domNode.querySelector("input.s-offscreen")).toBeTruthy();
        expect(capturedOptions).toHaveLength(1);
        editor.destroy();
    });

    it("keeps the original-name hint when originalNameProperty is set", () => {
        const editor = create({ originalNameProperty: "OriginalName" });
        expect(editor.domNode.classList.contains("hide-original-name")).toBe(false);
        editor.destroy();
    });

    it("applies readonly during construction", () => {
        const editor = create({ readOnly: true });
        expect(editor.get_readOnly()).toBe(true);
        expect(editor.domNode.querySelector(".add-file-button")?.classList.contains("disabled")).toBe(true);
        editor.destroy();
    });

    it("sets a value from an object and populates the symbols", () => {
        const editor = create({ displayFileName: true });
        editor.set_value({ Filename: "temporary/abc.jpg", OriginalName: "abc.jpg" });
        expect(editor.domNode.querySelectorAll("li.file-item").length).toBe(1);
        expect((editor as any).hiddenInput.value).toBe("temporary/abc.jpg");
        expect(editor.get_value()).toEqual({ Filename: "temporary/abc.jpg", OriginalName: "abc.jpg" });
        editor.destroy();
    });

    it("sets a value from a string and extracts the original name", () => {
        const editor = create();
        editor.set_value("temporary/abc.jpg" as any);
        expect(editor.get_value()).toEqual({ Filename: "temporary/abc.jpg", OriginalName: "abc.jpg" });
        editor.set_value("temp\\dir\\file.png" as any);
        expect(editor.get_value().OriginalName).toBe("dir\\file.png");
        editor.set_value("" as any);
        expect(editor.get_value()).toBeNull();
        editor.destroy();
    });

    it("clears the value for blank or null filenames", () => {
        const editor = create();
        editor.set_value({ Filename: "  " } as any);
        expect(editor.get_value()).toBeNull();
        editor.set_value({ Filename: null } as any);
        expect(editor.get_value()).toBeNull();
        editor.set_value(null);
        expect(editor.get_value()).toBeNull();
        editor.destroy();
    });

    it("deletes the entity via the delete button", () => {
        const editor = create();
        editor.set_value({ Filename: "x.jpg", OriginalName: "x.jpg" });
        expect(editor.get_value()).not.toBeNull();
        (editor.domNode.querySelector(".delete-button") as HTMLElement).click();
        expect(editor.get_value()).toBeNull();
        expect((editor as any).hiddenInput.value).toBeFalsy();
        editor.destroy();
    });

    it("toggles readonly state and button availability", () => {
        const editor = create();
        expect(editor.get_readOnly()).toBe(false);
        editor.set_readOnly(true);
        expect(editor.get_readOnly()).toBe(true);
        expect(editor.domNode.querySelector(".add-file-button")?.classList.contains("disabled")).toBe(true);
        editor.set_readOnly(false);
        expect(editor.get_readOnly()).toBe(false);
        expect(editor.domNode.querySelector(".add-file-button")?.classList.contains("disabled")).toBe(false);
        editor.destroy();
    });

    it("toggles the required class on the hidden input", () => {
        const editor = create();
        editor.set_required(true);
        expect(editor.get_required()).toBe(true);
        expect((editor as any).hiddenInput.classList.contains("required")).toBe(true);
        editor.set_required(false);
        expect(editor.get_required()).toBe(false);
        editor.destroy();
    });

    it("serializes the entity into edit values", () => {
        const editor = create();
        editor.set_value({ Filename: "f.txt" } as any);
        const target: any = {};
        editor.getEditValue({ name: "Field" } as any, target);
        expect(target.Field).toBe("f.txt");
        editor.setEditValue({ Field: "g.txt" }, { name: "Field" } as any);
        expect(editor.get_value().Filename).toBe("g.txt");
        editor.destroy();
    });

    it("derives the original name from the display filename", () => {
        const editor = create({ displayFileName: true });
        editor.setEditValue({ Field: "dir\\file.txt" }, { name: "Field" } as any);
        expect(editor.get_value().OriginalName).toBe("file.txt");
        editor.destroy();
    });

    it("reads the original name from a property when configured", () => {
        const editor = create({ originalNameProperty: "Orig" });
        editor.setEditValue({ Field: "f.txt", Orig: "Original.txt" }, { name: "Field" } as any);
        expect(editor.get_value().OriginalName).toBe("Original.txt");
        editor.destroy();
    });

    it("adds a file through the fileDone callback", () => {
        const editor = create();
        capturedOptions[0].fileDone!(
            { TemporaryFile: "t1.jpg", IsImage: true, Size: 10, Width: 10, Height: 10 } as any,
            "name.jpg",
            {});
        expect(editor.get_value()).toEqual({ Filename: "t1.jpg", OriginalName: "name.jpg" });
        expect(editor.domNode.querySelectorAll("li.file-item").length).toBe(1);
        editor.destroy();
    });

    it("skips the file when image constraints fail", () => {
        vi.mocked(UploadHelper.checkImageConstraints).mockReturnValue(false);
        const editor = create();
        capturedOptions[0].fileDone!({ TemporaryFile: "t1.jpg" } as any, "name.jpg", {});
        expect(editor.get_value()).toBeNull();
        expect(editor.domNode.querySelectorAll("li.file-item").length).toBe(0);
        editor.destroy();
    });

    it("returns the add-file button text", () => {
        const editor = create();
        expect(typeof editor["addFileButtonText"]()).toBe("string");
        editor.destroy();
    });

    it("uses the value property getter and setter", () => {
        const editor = create();
        editor.value = { Filename: "x.jpg", OriginalName: "x.jpg" } as any;
        expect(editor.value).toEqual({ Filename: "x.jpg", OriginalName: "x.jpg" });
        editor.destroy();
    });

    it("uses the whole filename as the original name without separators", () => {
        const editor = create({ displayFileName: true });
        editor.setEditValue({ Field: "file.txt" }, { name: "Field" } as any);
        expect(editor.get_value().OriginalName).toBe("file.txt");
        editor.destroy();
    });
});

describe("ImageUploadEditor", () => {
    beforeEach(() => {
        vi.spyOn(UploadHelper, "addUploadInput").mockImplementation(() => Fluent(document.createElement("input")));
    });

    afterEach(() => {
        vi.restoreAllMocks();
        document.body.innerHTML = "";
    });

    it("defaults allowNonImage to false and adds its class", () => {
        const editor = new ImageUploadEditor({ element: el => document.body.appendChild(el) } as any);
        expect(editor["options"]?.allowNonImage).toBe(false);
        expect(editor.domNode.classList.contains("s-ImageUploadEditor")).toBe(true);
        editor.destroy();
    });
});

describe("MultipleFileUploadEditor", () => {
    let capturedOptions: UploadInputOptions[];

    beforeEach(() => {
        capturedOptions = [];
        vi.spyOn(UploadHelper, "addUploadInput").mockImplementation((options) => {
            capturedOptions.push(options);
            return Fluent(document.createElement("input"));
        });
        vi.spyOn(UploadHelper, "checkImageConstraints").mockReturnValue(true);
    });

    afterEach(() => {
        vi.restoreAllMocks();
        document.body.innerHTML = "";
    });

    function create(options: any = {}): MultipleFileUploadEditor<any> {
        return new MultipleFileUploadEditor({
            element: el => document.body.appendChild(el),
            ...options
        } as any);
    }

    it("sets up the DOM with only an add button", () => {
        const editor = create();
        expect(editor.domNode.classList.contains("s-MultipleFileUploadEditor")).toBe(true);
        expect(editor.domNode.querySelector(".add-file-button")).toBeTruthy();
        expect(editor.domNode.querySelector(".delete-button")).toBeFalsy();
        expect(editor.domNode.querySelector(".upload-progress")).toBeTruthy();
        expect(editor.domNode.querySelector("ul.file-items")).toBeTruthy();
        const hidden = (editor as any).hiddenInput as HTMLInputElement;
        expect(hidden.classList.contains("s-offscreen")).toBe(true);
        expect(hidden.hasAttribute("multiple")).toBe(true);
        editor.destroy();
    });

    it("adds multiple entities through fileDone", () => {
        const editor = create();
        capturedOptions[0].fileDone!({ TemporaryFile: "a.jpg", IsImage: true } as any, "a.jpg", {});
        capturedOptions[0].fileDone!({ TemporaryFile: "b.jpg", IsImage: true } as any, "b.jpg", {});
        expect(editor.get_value().length).toBe(2);
        expect(editor.domNode.querySelectorAll("li.file-item").length).toBe(2);
        expect((editor as any).hiddenInput.value).toBe("a.jpg");
        editor.destroy();
    });

    it("sets and clears the collection value", () => {
        const editor = create();
        editor.set_value([{ Filename: "x.jpg", OriginalName: "x.jpg" }]);
        expect(editor.get_value()).toEqual([{ Filename: "x.jpg", OriginalName: "x.jpg" }]);
        editor.set_value(null);
        expect(editor.get_value()).toEqual([]);
        editor.destroy();
    });

    it("removes an entity via its delete link", () => {
        const editor = create();
        editor.set_value([
            { Filename: "x.jpg", OriginalName: "x.jpg" },
            { Filename: "y.jpg", OriginalName: "y.jpg" }
        ]);
        (editor.domNode.querySelectorAll("a.delete")[0] as HTMLElement).click();
        expect(editor.get_value().length).toBe(1);
        expect(editor.get_value()[0].Filename).toBe("y.jpg");
        editor.destroy();
    });

    it("toggles readonly and required state", () => {
        const editor = create();
        editor.set_value([{ Filename: "x.jpg" }]);
        expect(editor.get_readOnly()).toBe(false);
        editor.set_readOnly(true);
        expect(editor.get_readOnly()).toBe(true);
        expect(editor.domNode.querySelector(".add-file-button")?.classList.contains("disabled")).toBe(true);
        editor.set_readOnly(false);
        expect(editor.get_readOnly()).toBe(false);
        editor.set_required(true);
        expect(editor.get_required()).toBe(true);
        editor.destroy();
    });

    it("serializes edit values with optional json encoding", () => {
        const editor = create({ jsonEncodeValue: true });
        editor.set_value([{ Filename: "a.jpg" }]);
        const target: any = {};
        editor.getEditValue({ name: "Field" } as any, target);
        expect(JSON.parse(target.Field)).toEqual([{ Filename: "a.jpg" }]);
        editor.jsonEncodeValue = false;
        editor.getEditValue({ name: "Field2" } as any, target);
        expect(target.Field2).toEqual([{ Filename: "a.jpg" }]);
        expect(editor.jsonEncodeValue).toBe(false);
        editor.destroy();
    });

    it("parses json and plain string edit values", () => {
        const editor = create();
        editor.setEditValue({ Field: '[{"Filename":"a.jpg"}]' }, { name: "Field" } as any);
        expect(editor.get_value()).toEqual([{ Filename: "a.jpg" }]);
        editor.setEditValue({ Field: "plain.txt" }, { name: "Field" } as any);
        expect(editor.get_value()[0]).toEqual({ Filename: "plain.txt", OriginalName: "UnknownFile" });
        editor.setEditValue({ Field: null }, { name: "Field" } as any);
        expect(editor.get_value()).toEqual([]);
        editor.destroy();
    });

    it("skips the file when image constraints fail", () => {
        vi.mocked(UploadHelper.checkImageConstraints).mockReturnValue(false);
        const editor = create();
        capturedOptions[0].fileDone!({ TemporaryFile: "a.jpg" } as any, "a.jpg", {});
        expect(editor.get_value()).toEqual([]);
        expect(editor.domNode.querySelectorAll("li.file-item").length).toBe(0);
        editor.destroy();
    });

    it("uses the value property getter and setter", () => {
        const editor = create();
        editor.value = [{ Filename: "x.jpg", OriginalName: "x.jpg" }] as any;
        expect(editor.value).toEqual([{ Filename: "x.jpg", OriginalName: "x.jpg" }]);
        editor.destroy();
    });
});

describe("MultipleImageUploadEditor", () => {
    beforeEach(() => {
        vi.spyOn(UploadHelper, "addUploadInput").mockImplementation(() => Fluent(document.createElement("input")));
    });

    afterEach(() => {
        vi.restoreAllMocks();
        document.body.innerHTML = "";
    });

    it("adds the multiple-image class", () => {
        const editor = new MultipleImageUploadEditor({ element: el => document.body.appendChild(el) } as any);
        expect(editor.domNode.classList.contains("s-MultipleImageUploadEditor")).toBe(true);
        editor.destroy();
    });
});
